using PokemonTypeChecker.Models;

namespace PokemonTypeChecker.Tests.Helpers;

public static class TestDataBuilder
{
    public static Pokemon BuildPokemon(string name, params string[] types)
    {
        return new Pokemon
        {
            Name = name,
            Types = types.Select((type, index) => new PokemonTypeSlot
            {
                Slot = index + 1,
                Type = new PokemonTypeInfo { Name = type }
            }).ToList()
        };
    }

    public static PokemonType BuildPokemonType(string name)
    {
        return new PokemonType
        {
            Name = name,
            DamageRelations = new DamageRelations()
        };
    }

    public static DamageRelations BuildDamageRelations()
    {
        return new DamageRelations
        {
            DoubleDamageFrom = new List<TypeReference>(),
            DoubleDamageTo = new List<TypeReference>(),
            HalfDamageFrom = new List<TypeReference>(),
            HalfDamageTo = new List<TypeReference>(),
            NoDamageFrom = new List<TypeReference>(),
            NoDamageTo = new List<TypeReference>()
        };
    }

    public static TypeReference BuildTypeReference(string name)
    {
        return new TypeReference
        {
            Name = name,
            Url = $"https://pokeapi.co/api/v2/type/{name}/"
        };
    }
}