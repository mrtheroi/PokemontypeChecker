using Microsoft.Extensions.DependencyInjection;
using PokemonTypeChecker.Services;
using PokemonTypeChecker.UI;

namespace PokemonTypeChecker;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure services
        var serviceProvider = ConfigureServices();
        
        // Run the application
        var ui = serviceProvider.GetRequiredService<ConsoleUI>();
        await ui.RunAsync();
    }
    
    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // Register HttpClient for PokemonService
        services.AddHttpClient<IPokemonService, PokemonService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Add("User-Agent", "PokemonTypeChecker/1.0");
        });
        
        // Register other services
        services.AddScoped<ITypeEffectivenessCalculator, TypeEffectivenessCalculator>();
        services.AddScoped<ConsoleUI>();
        
        return services.BuildServiceProvider();
    }
}