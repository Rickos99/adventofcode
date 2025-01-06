using AdventOfCode.Core.Models;
using AdventOfCode.Core.Utility.Collections;

namespace AdventOfCode2024.Day18;

[PuzzleInfo(18, "RAM Run")]
internal sealed class Solution() : Puzzle(18)
{
    private readonly Position _size = new(70, 70);
    private readonly int _simulationSteps = 1024;

    public override string SolveFirstPart()
    {
        var memorySpace = new MemorySpace(_size);
        memorySpace.Load(ReadMemoryPositions().Take(_simulationSteps));

        return ShortestPath(memorySpace, new(0, 0), _size).ToString();
    }

    public override string SolveSecondPart()
    {
        var memory = ReadMemoryPositions().ToList();
        var memorySpace = new MemorySpace(_size);
        memorySpace.Load(memory.Take(_simulationSteps));

        var bytesToLoad = _simulationSteps;
        while (bytesToLoad < memory.Count)
        {
            memorySpace.Load(memory[bytesToLoad]);
            if (ShortestPath(memorySpace, new(0, 0), _size) == int.MaxValue) break;
            bytesToLoad++;
        }

        var lastLoadedByte = memory[bytesToLoad];
        return $"{lastLoadedByte.Col},{lastLoadedByte.Row}";
    }

    private int ShortestPath(MemorySpace memorySpace, Position start, Position end)
    {
        var map = Collection2DGenerator.Generate<(bool visited, int distance)>(_size + 1, (false, int.MaxValue));
        map[start.Row][start.Col] = (true, 0);

        var queue = new Queue<Position>([start]);
        while (queue.TryDequeue(out var next))
        {
            foreach (var memoryPosition in MemorySpace.GetAdjacentPositions(next).Where(memorySpace.IsSafe))
            {
                if (!map[memoryPosition.Row][memoryPosition.Col].visited)
                {
                    map[memoryPosition.Row][memoryPosition.Col] = (true, map[next.Row][next.Col].distance + 1);
                    queue.Enqueue(memoryPosition);
                }
            }
        }

        return map[end.Row][end.Col].distance;
    }

    private IEnumerable<Position> ReadMemoryPositions()
        => _puzzleInput
            .Select(line => line.Split(',').Select(int.Parse).ToList())
            .Select(position => new Position(position[1], position[0]));
}

internal class MemorySpace : List<List<char>>
{
    public MemorySpace(Position _size) => AddRange(Collection2DGenerator.Generate(_size + 1, '.'));

    public void Load(IEnumerable<Position> bytesToLoad) => bytesToLoad.ToList().ForEach(Load);

    public void Load(Position byteToLoad) => this[byteToLoad.Row][byteToLoad.Col] = '#';

    public bool IsSafe(Position position) => IsInSpace(position) && this[position] == '.';

    public char this[Position position] => this[position.Row][position.Col];

    public static List<Position> GetAdjacentPositions(Position position)
        => [
            new(position.Row - 1, position.Col),
            new(position.Row + 1, position.Col),
            new(position.Row, position.Col - 1),
            new(position.Row, position.Col + 1)
        ];

    private bool IsInSpace(Position position)
        => position.Row >= 0 && position.Col >= 0
        && position.Row < Count && position.Col < this[0].Count;
}
