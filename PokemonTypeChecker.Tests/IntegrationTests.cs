using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PokemonTypeChecker.Services;

namespace PokemonTypeChecker.Tests;

public class IntegrationTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly ITypeEffectivenessCalculator _calculator;

    public IntegrationTests()
    {
        var services = new ServiceCollection();
        
        // Register services similar to Program.cs
        services.AddHttpClient<IPokemonService, PokemonService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("User-Agent", "PokemonTypeChecker.Tests/1.0");
        });
        
        services.AddScoped<ITypeEffectivenessCalculator, TypeEffectivenessCalculator>();
        
        _serviceProvider = services.BuildServiceProvider();
        _calculator = _serviceProvider.GetRequiredService<ITypeEffectivenessCalculator>();
    }

    [Fact(Skip = "Integration test - requires internet connection")]
    public async Task CalculateEffectiveness_RealPokemonApi_Pikachu_ReturnsExpectedResults()
    {
        // Act
        var result = await _calculator.CalculateEffectivenessAsync("pikachu");
        
        // Assert
        result.Should().NotBeNull();
        result.PokemonName.Should().Be("pikachu");
        result.Types.Should().ContainSingle().Which.Should().Be("electric");
        
        // Pikachu (Electric) should be strong against Water and Flying
        result.StrongAgainst.Should().Contain(x => x.TypeName == "water");
        result.StrongAgainst.Should().Contain(x => x.TypeName == "flying");
        
        // Pikachu (Electric) should be weak against Ground
        result.WeakAgainst.Should().Contain(x => x.TypeName == "ground");
    }

    [Fact(Skip = "Integration test - requires internet connection")]
    public async Task CalculateEffectiveness_RealPokemonApi_InvalidPokemon_ThrowsException()
    {
        // Act & Assert
        var act = () => _calculator.CalculateEffectivenessAsync("notarealpokemon123");
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Pokemon 'notarealpokemon123' not found.");
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}