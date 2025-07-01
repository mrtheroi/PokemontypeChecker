# Pokemon Type Checker

A console application that uses the PokéAPI to determine a Pokémon's effectiveness against other Pokémon types.

## Features

- Look up any Pokémon by name
- View the Pokémon's type(s)
- See which types the Pokémon is strong against
- See which types the Pokémon is weak against
- Beautiful console UI with colored tables
- Support for dual-type Pokémon with combined effectiveness calculations
- Comprehensive unit tests with mocking
- Clean architecture with dependency injection
- Robust error handling

## Prerequisites

- .NET 8.0 SDK or later
- Internet connection (for PokéAPI access)

## Project Structure

```
PokemonTypeChecker/
├── PokemonTypeChecker/           # Main application
│   ├── Interfaces/               # Service contracts
│   │   ├── IPokemonService.cs
│   │   └── ITypeEffectivenessCalculator.cs
│   ├── Models/                   # Data models
│   │   ├── Pokemon.cs
│   │   ├── PokemonType.cs
│   │   └── TypeEffectiveness.cs
│   ├── Services/                 # Business logic implementations
│   │   ├── PokemonService.cs
│   │   └── TypeEffectivenessCalculator.cs
│   ├── UI/                       # Console interface
│   │   └── ConsoleUI.cs
│   └── Program.cs                # Entry point with DI setup
└── PokemonTypeChecker.Tests/     # Unit tests
    ├── Services/                 # Service tests
    └── Helpers/                  # Test utilities
```

## Installation

1. Clone the repository:
```bash
git clone [your-repository-url]
cd PokemonTypeChecker
```

2. Restore dependencies:
```bash
dotnet restore
```

## How to Run

### Running the Application

1. Navigate to the project directory:
```bash
cd PokemonTypeChecker/PokemonTypeChecker
```

2. Run the application:
```bash
dotnet run
```

3. When prompted, enter a Pokémon name (e.g., "pikachu", "charizard", "bulbasaur")

4. The application will display:
   - The Pokémon's type(s)
   - Types it's strong against (with reasons)
   - Types it's weak against (with reasons)

5. Type "exit" to quit the application

### Running Tests

1. Navigate to the solution root:
```bash
cd PokemonTypeChecker
```

2. Run all tests:
```bash
dotnet test
```

3. Run tests with detailed output:
```bash
dotnet test --logger "console;verbosity=detailed"
```

4. Run integration tests (requires internet):
```bash
dotnet test --filter "FullyQualifiedName~IntegrationTests" --no-skip
```

## Example Usage

```
Enter a Pokemon name (or 'exit' to quit): pikachu

╭─ Pokemon Info ──────────────────────────╮
│ PIKACHU                                 │
│ Types: electric                         │
╰─────────────────────────────────────────╯

┌─ STRONG AGAINST ────────┬─ WEAK AGAINST ──────────┐
│ Type      │ Advantages │ Type     │ Disadvantages│
├───────────┼────────────┼──────────┼──────────────┤
│ flying    │ • Deals 2x │ ground   │ • Takes 2x   │
│           │   damage   │          │   damage     │
│ water     │ • Deals 2x │          │              │
│           │   damage   │          │              │
└───────────┴────────────┴──────────┴──────────────┘
```

## Type Effectiveness Rules

A Pokémon is **strong against** another type if it:
- Deals double damage to that type
- Takes no damage from that type
- Takes half damage from that type

A Pokémon is **weak against** another type if it:
- Deals no damage to that type
- Deals half damage to that type
- Takes double damage from that type

## Architecture

The application follows clean architecture principles with dependency injection and clear separation of concerns:

- **Interfaces**: Service contracts separated from implementations for better abstraction
   - `IPokemonService`: Contract for PokéAPI communication
   - `ITypeEffectivenessCalculator`: Contract for effectiveness calculations
- **Models**: Data structures for Pokémon and type information (POCOs)
- **Services**: Business logic implementations
   - `PokemonService`: Handles API communication with error handling
   - `TypeEffectivenessCalculator`: Combines type relationships and calculates effectiveness
- **UI**: Console interface using Spectre.Console for rich formatting
- **Tests**: Comprehensive unit tests with mocking

### Design Patterns Used

- **Dependency Injection**: For loose coupling and testability
- **Repository Pattern**: Abstract API communication through interfaces
- **Service Pattern**: Business logic separation
- **Interface Segregation**: Clean contracts in dedicated folder
- **Separation of Concerns**: Interfaces separated from implementations

## Dependencies

### Production
- Microsoft.Extensions.DependencyInjection - Dependency injection container
- Microsoft.Extensions.Http - HTTP client factory
- Newtonsoft.Json (13.0.3) - JSON parsing
- Spectre.Console - Console UI formatting

### Testing
- xUnit - Test framework
- Moq (4.20.70) - Mocking framework
- FluentAssertions (6.12.0) - Readable test assertions

## Error Handling

The application handles various error scenarios gracefully:
- Invalid Pokémon names (404 from API)
- Network connectivity issues
- API timeout scenarios (30-second timeout configured)
- Empty responses
- Malformed JSON responses

All errors display user-friendly messages without exposing technical details.

## Testing

The project includes comprehensive unit tests covering:

### Unit Tests
- **PokemonServiceTests**: API communication with mocked HTTP responses
- **TypeEffectivenessCalculatorTests**: Business logic for type calculations
   - Single-type Pokémon
   - Dual-type Pokémon with combined effectiveness
   - Edge cases (empty damage relations, not found)

### Integration Tests
- End-to-end tests against real PokéAPI (skipped by default)
- Can be run manually when internet is available

### Test Coverage Areas
- ✅ Happy path scenarios
- ✅ Error handling (404, network errors, timeouts)
- ✅ Edge cases (empty data, null responses)
- ✅ Data transformation and calculations

## Limitations

- Requires internet connection for PokéAPI access
- English names only
- Does not consider abilities or move sets, only base type matchups
- No caching (each query makes fresh API calls)

## Future Improvements

Potential enhancements for the project:
- Add caching to reduce API calls
- Support for different languages
- Include abilities and move sets in calculations
- Export results to CSV/JSON
- Compare two Pokémon head-to-head
- Add performance metrics and logging
- Create a web API version
- Implement rate limiting for API calls

