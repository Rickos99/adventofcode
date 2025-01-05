using AdventOfCode.Core.Models;

namespace AdventOfCode2024.Day10;

[PuzzleInfo(10, "Hoof It")]
internal sealed class Solution() : Puzzle(10)
{
    public override string SolveFirstPart()
        => GetStartPositions().Sum(startPosition => Trails(startPosition, [], false)).ToString();

    public override string SolveSecondPart()
        => GetStartPositions().Sum(startPosition => Trails(startPosition, [], true)).ToString();

    private int Trails(Position position, HashSet<Position> visitedPositions, bool distinctTrails)
    {
        if (!distinctTrails && !visitedPositions.Add(position)) return 0;
        if (_puzzleInput[position.Row][position.Col] == '9') return 1;

        var nextPositions = new Position[]
        {
            new(position.Row - 1, position.Col),
            new(position.Row + 1, position.Col),
            new(position.Row, position.Col - 1),
            new(position.Row, position.Col + 1)
        };

        return nextPositions
            .Where(nextPosition => IsValidNextPosition(position, nextPosition))
            .Sum(nextPosition => Trails(nextPosition, visitedPositions, distinctTrails));
    }

    private bool IsValidNextPosition(Position currentPosition, Position nextPosition)
    {
        return nextPosition.Row >= 0 && nextPosition.Row < _puzzleInput.Length &&
               nextPosition.Col >= 0 && nextPosition.Col < _puzzleInput[0].Length &&
               _puzzleInput[nextPosition.Row][nextPosition.Col] == _puzzleInput[currentPosition.Row][currentPosition.Col] + 1;
    }

    private List<Position> GetStartPositions()
    {
        var startPositions = new List<Position>();

        for (int i = 0; i < _puzzleInput.Length; i++)
        {
            for (var j = 0; j < _puzzleInput[0].Length; j++)
            {
                if (_puzzleInput[i][j] == '0') startPositions.Add(new(i, j));
            }
        }

        return startPositions;
    }
}
