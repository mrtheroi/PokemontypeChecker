namespace PokemonTypeChecker.Models;

public class TypeEffectiveness
{
    public string PokemonName { get; set; } = string.Empty;
    public List<string> Types { get; set; } = new();
    public List<TypeRelation> StrongAgainst { get; set; } = new();
    public List<TypeRelation> WeakAgainst { get; set; } = new();
}

public class TypeRelation
{
    public string TypeName { get; set; } = string.Empty;
    public List<string> Reasons { get; set; } = new();
    
    public TypeRelation(string typeName)
    {
        TypeName = typeName;
        Reasons = new List<string>();
    }
}

public enum EffectivenessType
{
    DoubleDamageTo,
    HalfDamageFrom,
    NoDamageFrom,
    DoubleDamageFrom,
    HalfDamageTo,
    NoDamageTo
}