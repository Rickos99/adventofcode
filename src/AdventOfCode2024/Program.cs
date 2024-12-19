using System.Reflection;

string[] exitInputs = { "q" };
Console.WriteLine("Enter 'q' to exit.\n");

var puzzleTypes = GetImplementations<Puzzle>();
var puzzles = puzzleTypes
    .Select(puzzle =>
    {
        var info = puzzle.GetCustomAttribute<PuzzleInfoAttribute>()
            ?? throw new InvalidDataException($"Puzzle {puzzle.FullName} is missing PuzzleInfoAttribute");
        return (info, puzzle);
    });

while (true)
{
    ShowMenu();

    var input = Console.ReadLine()?.Trim();
    if (exitInputs.Contains(input)) break;
    if (!int.TryParse(input, out var day))
    {
        Console.WriteLine("Invalid input.");
        continue;
    }

    var puzzle = puzzles.FirstOrDefault(p => p.info.Day == day);
    if (puzzle == default)
    {
        Console.WriteLine("Puzzle not found.");
        continue;
    }

    var puzzleInstance = (Puzzle?)Activator.CreateInstance(puzzle.puzzle);

    Console.WriteLine();
    Console.WriteLine($"Solution part 1: {puzzleInstance?.SolveFirstPart()}");
    Console.WriteLine($"Solution part 2: {puzzleInstance?.SolveSecondPart()}");
    Console.WriteLine();
}

void ShowMenu()
{
    Console.WriteLine("-------------------------------");
    Console.WriteLine("Available puzzles:");

    foreach (var puzzle in puzzles.OrderBy(puzzle => puzzle.info.Day))
    {
        Console.WriteLine($"{puzzle.info.Day,-2} - {puzzle.info.Name}");
    }

    Console.Write("Enter puzzle number to solve: ");
}

static IEnumerable<Type> GetImplementations<TAbstract>()
{
    var abstractType = typeof(TAbstract);
    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    var implementations = assemblies
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => type.IsClass && !type.IsAbstract && abstractType.IsAssignableFrom(type));

    return implementations;
}