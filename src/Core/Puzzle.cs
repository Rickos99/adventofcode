namespace AdventOfCode.Core;
public abstract class Puzzle
{
    private const string _inputFileName = "data.in";

    protected readonly string[] _puzzleInput;

    protected Puzzle(int day)
    {
        var filePath = Path.Combine($"Day{day:D2}", _inputFileName);
        if (!File.Exists(filePath)) throw new FileNotFoundException($"File not found: {filePath}");

        _puzzleInput = File.ReadAllLines(filePath);
    }

    abstract public int SolveFirstPart();
    abstract public int SolveSecondPart();
}
