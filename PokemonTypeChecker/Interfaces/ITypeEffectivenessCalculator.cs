using PokemonTypeChecker.Models;

namespace PokemonTypeChecker.Services;

public interface ITypeEffectivenessCalculator
{
    Task<TypeEffectiveness> CalculateEffectivenessAsync(string pokemonName);
}