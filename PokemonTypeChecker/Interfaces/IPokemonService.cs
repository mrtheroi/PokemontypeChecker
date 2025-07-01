using PokemonTypeChecker.Models;

namespace PokemonTypeChecker.Services;

public interface IPokemonService
{
    Task<Pokemon?> GetPokemonAsync(string name);
    Task<PokemonType?> GetPokemonTypeAsync(string typeName);
}