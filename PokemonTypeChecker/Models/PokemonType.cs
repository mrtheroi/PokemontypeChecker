using Newtonsoft.Json;

namespace PokemonTypeChecker.Models;

public class PokemonType
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("damage_relations")]
    public DamageRelations DamageRelations { get; set; } = new();
}

public class DamageRelations
{
    [JsonProperty("double_damage_from")]
    public List<TypeReference> DoubleDamageFrom { get; set; } = new();
    
    [JsonProperty("double_damage_to")]
    public List<TypeReference> DoubleDamageTo { get; set; } = new();
    
    [JsonProperty("half_damage_from")]
    public List<TypeReference> HalfDamageFrom { get; set; } = new();
    
    [JsonProperty("half_damage_to")]
    public List<TypeReference> HalfDamageTo { get; set; } = new();
    
    [JsonProperty("no_damage_from")]
    public List<TypeReference> NoDamageFrom { get; set; } = new();
    
    [JsonProperty("no_damage_to")]
    public List<TypeReference> NoDamageTo { get; set; } = new();
}

public class TypeReference
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}