using Newtonsoft.Json;

namespace PokemonTypeChecker.Models;

public class Pokemon
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("types")]
    public List<PokemonTypeSlot> Types { get; set; } = new();
}

public class PokemonTypeSlot
{
    [JsonProperty("slot")]
    public int Slot { get; set; }
    
    [JsonProperty("type")]
    public PokemonTypeInfo Type { get; set; } = new();
}

public class PokemonTypeInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}