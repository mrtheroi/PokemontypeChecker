using FluentAssertions;
using Moq;
using PokemonTypeChecker.Models;
using PokemonTypeChecker.Services;

namespace PokemonTypeChecker.Tests.Services;

public class TypeEffectivenessCalculatorTests
{
    private readonly Mock<IPokemonService> _pokemonServiceMock;
    private readonly TypeEffectivenessCalculator _calculator;

    public TypeEffectivenessCalculatorTests()
    {
        _pokemonServiceMock = new Mock<IPokemonService>();
        _calculator = new TypeEffectivenessCalculator(_pokemonServiceMock.Object);
    }

    [Fact]
    public async Task CalculateEffectivenessAsync_SingleTypePokemon_CalculatesCorrectly()
    {
        // Arrange
        var pokemonName = "pikachu";
        var pokemon = new Pokemon
        {
            Name = "pikachu",
            Types = new List<PokemonTypeSlot>
            {
                new() { Type = new PokemonTypeInfo { Name = "electric" } }
            }
        };

        var electricType = new PokemonType
        {
            Name = "electric",
            DamageRelations = new DamageRelations
            {
                DoubleDamageTo = new List<TypeReference>
                {
                    new() { Name = "water" },
                    new() { Name = "flying" }
                },
                DoubleDamageFrom = new List<TypeReference>
                {
                    new() { Name = "ground" }
                },
                HalfDamageFrom = new List<TypeReference>
                {
                    new() { Name = "electric" },
                    new() { Name = "flying" },
                    new() { Name = "steel" }
                }
            }
        };

        _pokemonServiceMock.Setup(x => x.GetPokemonAsync(pokemonName))
            .ReturnsAsync(pokemon);
        _pokemonServiceMock.Setup(x => x.GetPokemonTypeAsync("electric"))
            .ReturnsAsync(electricType);

        // Act
        var result = await _calculator.CalculateEffectivenessAsync(pokemonName);

        // Assert
        result.Should().NotBeNull();
        result.PokemonName.Should().Be("pikachu");
        result.Types.Should().ContainSingle().Which.Should().Be("electric");
        
        // Strong against water and flying (deals 2x damage)
        result.StrongAgainst.Should().Contain(x => x.TypeName == "water");
        result.StrongAgainst.Should().Contain(x => x.TypeName == "flying");
        
        // Strong against electric, flying, steel (takes 0.5x damage)
        result.StrongAgainst.Should().Contain(x => x.TypeName == "electric");
        result.StrongAgainst.Should().Contain(x => x.TypeName == "steel");
        
        // Weak against ground (takes 2x damage)
        result.WeakAgainst.Should().Contain(x => x.TypeName == "ground");
    }

    [Fact]
    public async Task CalculateEffectivenessAsync_DualTypePokemon_CombinesEffectiveness()
    {
        // Arrange
        var pokemonName = "charizard";
        var pokemon = new Pokemon
        {
            Name = "charizard",
            Types = new List<PokemonTypeSlot>
            {
                new() { Type = new PokemonTypeInfo { Name = "fire" } },
                new() { Type = new PokemonTypeInfo { Name = "flying" } }
            }
        };

        var fireType = new PokemonType
        {
            Name = "fire",
            DamageRelations = new DamageRelations
            {
                DoubleDamageTo = new List<TypeReference>
                {
                    new() { Name = "grass" },
                    new() { Name = "ice" }
                },
                DoubleDamageFrom = new List<TypeReference>
                {
                    new() { Name = "water" },
                    new() { Name = "rock" }
                }
            }
        };

        var flyingType = new PokemonType
        {
            Name = "flying",
            DamageRelations = new DamageRelations
            {
                DoubleDamageTo = new List<TypeReference>
                {
                    new() { Name = "grass" },
                    new() { Name = "fighting" }
                },
                DoubleDamageFrom = new List<TypeReference>
                {
                    new() { Name = "electric" },
                    new() { Name = "rock" }
                }
            }
        };

        _pokemonServiceMock.Setup(x => x.GetPokemonAsync(pokemonName))
            .ReturnsAsync(pokemon);
        _pokemonServiceMock.Setup(x => x.GetPokemonTypeAsync("fire"))
            .ReturnsAsync(fireType);
        _pokemonServiceMock.Setup(x => x.GetPokemonTypeAsync("flying"))
            .ReturnsAsync(flyingType);

        // Act
        var result = await _calculator.CalculateEffectivenessAsync(pokemonName);

        // Assert
        result.Types.Should().HaveCount(2);
        result.Types.Should().Contain("fire");
        result.Types.Should().Contain("flying");
        
        // Both types are strong against grass
        var grassStrength = result.StrongAgainst.FirstOrDefault(x => x.TypeName == "grass");
        grassStrength.Should().NotBeNull();
        grassStrength!.Reasons.Should().Contain("Deals 2x damage");
        // Note: The current implementation uses a HashSet, so duplicate reasons are merged
        
        // Both types are weak against rock
        var rockWeakness = result.WeakAgainst.FirstOrDefault(x => x.TypeName == "rock");
        rockWeakness.Should().NotBeNull();
        rockWeakness!.Reasons.Should().Contain("Takes 2x damage");
        // Note: The current implementation uses a HashSet, so duplicate reasons are merged
    }

    [Fact]
    public async Task CalculateEffectivenessAsync_PokemonNotFound_ThrowsException()
    {
        // Arrange
        var pokemonName = "fakepokemon";
        _pokemonServiceMock.Setup(x => x.GetPokemonAsync(pokemonName))
            .ReturnsAsync((Pokemon?)null);

        // Act & Assert
        var act = () => _calculator.CalculateEffectivenessAsync(pokemonName);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Pokemon 'fakepokemon' not found.");
    }

    [Fact]
    public async Task CalculateEffectivenessAsync_EmptyDamageRelations_ReturnsEmptyLists()
    {
        // Arrange
        var pokemonName = "test";
        var pokemon = new Pokemon
        {
            Name = "test",
            Types = new List<PokemonTypeSlot>
            {
                new() { Type = new PokemonTypeInfo { Name = "normal" } }
            }
        };

        var normalType = new PokemonType
        {
            Name = "normal",
            DamageRelations = new DamageRelations() // All lists empty by default
        };

        _pokemonServiceMock.Setup(x => x.GetPokemonAsync(pokemonName))
            .ReturnsAsync(pokemon);
        _pokemonServiceMock.Setup(x => x.GetPokemonTypeAsync("normal"))
            .ReturnsAsync(normalType);

        // Act
        var result = await _calculator.CalculateEffectivenessAsync(pokemonName);

        // Assert
        result.StrongAgainst.Should().BeEmpty();
        result.WeakAgainst.Should().BeEmpty();
    }
}