using System.Net;
using Newtonsoft.Json;
using PokemonTypeChecker.Models;

namespace PokemonTypeChecker.Services;

public class PokemonService : IPokemonService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://pokeapi.co/api/v2/";
    
    public PokemonService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }
    
    public async Task<Pokemon?> GetPokemonAsync(string name)
    {
        try
        {
            var response = await _httpClient.GetAsync($"pokemon/{name.ToLower()}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Pokemon>(json);
        }
        catch (HttpRequestException)
        {
            throw new Exception("Error connecting to PokéAPI. Please check your internet connection.");
        }
        catch (TaskCanceledException)
        {
            throw new Exception("Request timed out. Please try again.");
        }
    }
    
    public async Task<PokemonType?> GetPokemonTypeAsync(string typeName)
    {
        try
        {
            var response = await _httpClient.GetAsync($"type/{typeName.ToLower()}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PokemonType>(json);
        }
        catch (HttpRequestException)
        {
            throw new Exception($"Error fetching type data for {typeName}");
        }
    }
}