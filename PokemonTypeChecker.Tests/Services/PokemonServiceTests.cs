using System.Net;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PokemonTypeChecker.Models;
using PokemonTypeChecker.Services;

namespace PokemonTypeChecker.Tests.Services;

public class PokemonServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly PokemonService _pokemonService;

    public PokemonServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://pokeapi.co/api/v2/")
        };
        _pokemonService = new PokemonService(_httpClient);
    }

    [Fact]
    public async Task GetPokemonAsync_ValidPokemonName_ReturnsPokemon()
    {
        // Arrange
        var pokemonName = "pikachu";
        var expectedPokemon = new Pokemon
        {
            Id = 25,
            Name = "pikachu",
            Types = new List<PokemonTypeSlot>
            {
                new()
                {
                    Slot = 1,
                    Type = new PokemonTypeInfo { Name = "electric", Url = "https://pokeapi.co/api/v2/type/13/" }
                }
            }
        };

        var jsonResponse = JsonConvert.SerializeObject(expectedPokemon);
        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _pokemonService.GetPokemonAsync(pokemonName);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("pikachu");
        result.Id.Should().Be(25);
        result.Types.Should().HaveCount(1);
        result.Types[0].Type.Name.Should().Be("electric");
    }

    [Fact]
    public async Task GetPokemonAsync_InvalidPokemonName_ReturnsNull()
    {
        // Arrange
        var pokemonName = "fakepokemon";
        SetupHttpResponse(HttpStatusCode.NotFound, "");

        // Act
        var result = await _pokemonService.GetPokemonAsync(pokemonName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPokemonAsync_NetworkError_ThrowsExceptionWithMessage()
    {
        // Arrange
        var pokemonName = "pikachu";
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        var act = () => _pokemonService.GetPokemonAsync(pokemonName);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Error connecting to PokéAPI. Please check your internet connection.");
    }

    [Fact]
    public async Task GetPokemonAsync_Timeout_ThrowsExceptionWithMessage()
    {
        // Arrange
        var pokemonName = "pikachu";
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException());

        // Act & Assert
        var act = () => _pokemonService.GetPokemonAsync(pokemonName);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Request timed out. Please try again.");
    }

    [Fact]
    public async Task GetPokemonTypeAsync_ValidType_ReturnsTypeData()
    {
        // Arrange
        var typeName = "electric";
        var expectedType = new PokemonType
        {
            Name = "electric",
            DamageRelations = new DamageRelations
            {
                DoubleDamageTo = new List<TypeReference>
                {
                    new() { Name = "water", Url = "https://pokeapi.co/api/v2/type/11/" },
                    new() { Name = "flying", Url = "https://pokeapi.co/api/v2/type/3/" }
                },
                DoubleDamageFrom = new List<TypeReference>
                {
                    new() { Name = "ground", Url = "https://pokeapi.co/api/v2/type/5/" }
                }
            }
        };

        var jsonResponse = JsonConvert.SerializeObject(expectedType);
        SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _pokemonService.GetPokemonTypeAsync(typeName);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("electric");
        result.DamageRelations.DoubleDamageTo.Should().HaveCount(2);
        result.DamageRelations.DoubleDamageFrom.Should().HaveCount(1);
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string content)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content)
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }
}