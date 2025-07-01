using PokemonTypeChecker.Models;

namespace PokemonTypeChecker.Services;

public class TypeEffectivenessCalculator : ITypeEffectivenessCalculator
{
    private readonly IPokemonService _pokemonService;
    
    public TypeEffectivenessCalculator(IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
    }
    
    public async Task<TypeEffectiveness> CalculateEffectivenessAsync(string pokemonName)
    {
        // Get Pokemon data
        var pokemon = await _pokemonService.GetPokemonAsync(pokemonName);
        if (pokemon == null)
        {
            throw new Exception($"Pokemon '{pokemonName}' not found.");
        }
        
        // Get type data for each of the Pokemon's types
        var typeDataTasks = pokemon.Types
            .Select(t => _pokemonService.GetPokemonTypeAsync(t.Type.Name))
            .ToList();
        
        var typeData = await Task.WhenAll(typeDataTasks);
        
        // Calculate combined effectiveness
        var effectiveness = new TypeEffectiveness
        {
            PokemonName = pokemon.Name,
            Types = pokemon.Types.Select(t => t.Type.Name).ToList()
        };
        
        var strongAgainstDict = new Dictionary<string, HashSet<string>>();
        var weakAgainstDict = new Dictionary<string, HashSet<string>>();
        
        foreach (var type in typeData.Where(t => t != null))
        {
            // Strong against (offensive advantages)
            ProcessTypeAdvantages(type!.DamageRelations.DoubleDamageTo, 
                strongAgainstDict, "Deals 2x damage");
            
            ProcessTypeAdvantages(type.DamageRelations.HalfDamageFrom, 
                strongAgainstDict, "Takes 0.5x damage");
            
            ProcessTypeAdvantages(type.DamageRelations.NoDamageFrom, 
                strongAgainstDict, "Takes no damage");
            
            // Weak against (defensive disadvantages)
            ProcessTypeAdvantages(type.DamageRelations.DoubleDamageFrom, 
                weakAgainstDict, "Takes 2x damage");
            
            ProcessTypeAdvantages(type.DamageRelations.HalfDamageTo, 
                weakAgainstDict, "Deals 0.5x damage");
            
            ProcessTypeAdvantages(type.DamageRelations.NoDamageTo, 
                weakAgainstDict, "Deals no damage");
        }
        
        // Convert dictionaries to TypeRelation lists
        effectiveness.StrongAgainst = strongAgainstDict
            .Select(kvp => new TypeRelation(kvp.Key) { Reasons = kvp.Value.ToList() })
            .OrderBy(t => t.TypeName)
            .ToList();
        
        effectiveness.WeakAgainst = weakAgainstDict
            .Select(kvp => new TypeRelation(kvp.Key) { Reasons = kvp.Value.ToList() })
            .OrderBy(t => t.TypeName)
            .ToList();
        
        return effectiveness;
    }
    
    private void ProcessTypeAdvantages(List<TypeReference> types, 
        Dictionary<string, HashSet<string>> dict, string reason)
    {
        foreach (var type in types)
        {
            if (!dict.ContainsKey(type.Name))
            {
                dict[type.Name] = new HashSet<string>();
            }
            dict[type.Name].Add(reason);
        }
    }
}