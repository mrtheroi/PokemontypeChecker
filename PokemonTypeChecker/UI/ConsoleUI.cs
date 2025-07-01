using Spectre.Console;
using PokemonTypeChecker.Models;
using PokemonTypeChecker.Services;

namespace PokemonTypeChecker.UI;

public class ConsoleUI
{
    private readonly ITypeEffectivenessCalculator _calculator;
    
    public ConsoleUI(ITypeEffectivenessCalculator calculator)
    {
        _calculator = calculator;
    }
    
    public async Task RunAsync()
    {
        AnsiConsole.Write(
            new FigletText("Pokemon Type Checker")
                .Centered()
                .Color(Color.Yellow));
        
        while (true)
        {
            AnsiConsole.WriteLine();
            var pokemonName = AnsiConsole.Ask<string>("Enter a Pokemon name (or 'exit' to quit):");
            
            if (pokemonName.ToLower() == "exit")
            {
                AnsiConsole.MarkupLine("[yellow]Thanks for using Pokemon Type Checker![/]");
                break;
            }
            
            await ProcessPokemonAsync(pokemonName);
        }
    }
    
    private async Task ProcessPokemonAsync(string pokemonName)
    {
        try
        {
            TypeEffectiveness? effectiveness = null;
            
            await AnsiConsole.Status()
                .Start($"Fetching data for {pokemonName}...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    ctx.SpinnerStyle(Style.Parse("yellow"));
                    effectiveness = await _calculator.CalculateEffectivenessAsync(pokemonName);
                });
            
            DisplayResults(effectiveness!);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        }
    }
    
    private void DisplayResults(TypeEffectiveness effectiveness)
    {
        AnsiConsole.WriteLine();
        
        // Display Pokemon info
        var panel = new Panel($"[bold yellow]{effectiveness.PokemonName.ToUpper()}[/]\n" +
                             $"Types: {string.Join(", ", effectiveness.Types)}")
        {
            Header = new PanelHeader("Pokemon Info"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel);
        
        AnsiConsole.WriteLine();
        
        // Create tables side by side
        var layout = new Layout("Root")
            .SplitColumns(
                new Layout("Strong"),
                new Layout("Weak"));
        
        // Strong Against table
        var strongTable = new Table()
            .Title("[green]STRONG AGAINST[/]")
            .Border(TableBorder.Rounded)
            .AddColumn("[green]Type[/]")
            .AddColumn("[green]Advantages[/]");
        
        if (effectiveness.StrongAgainst.Any())
        {
            foreach (var relation in effectiveness.StrongAgainst)
            {
                strongTable.AddRow(
                    $"[bold]{relation.TypeName}[/]",
                    string.Join("\n", relation.Reasons.Select(r => $"• {r}"))
                );
            }
        }
        else
        {
            strongTable.AddRow("[grey]None[/]", "[grey]No type advantages[/]");
        }
        
        // Weak Against table
        var weakTable = new Table()
            .Title("[red]WEAK AGAINST[/]")
            .Border(TableBorder.Rounded)
            .AddColumn("[red]Type[/]")
            .AddColumn("[red]Disadvantages[/]");
        
        if (effectiveness.WeakAgainst.Any())
        {
            foreach (var relation in effectiveness.WeakAgainst)
            {
                weakTable.AddRow(
                    $"[bold]{relation.TypeName}[/]",
                    string.Join("\n", relation.Reasons.Select(r => $"• {r}"))
                );
            }
        }
        else
        {
            weakTable.AddRow("[grey]None[/]", "[grey]No type disadvantages[/]");
        }
        
        layout["Strong"].Update(strongTable);
        layout["Weak"].Update(weakTable);
        
        AnsiConsole.Write(layout);
    }
}