using System.Diagnostics;
using System.Text;
using Map = System.Collections.Generic.List<System.Collections.Generic.List<char>>;

namespace AdventOfCode2024.Day06;

internal record struct Position(int Row, int Col);
internal record Direction(int Vertical, int Horizontal);
internal sealed record Up() : Direction(-1, 0);
internal sealed record Right() : Direction(0, 1);
internal sealed record Down() : Direction(1, 0);
internal sealed record Left() : Direction(0, -1);

[PuzzleInfo(6, "Guard Gallivant")]
internal sealed class Solution() : Puzzle(6)
{
    public override long SolveFirstPart()
    {
        var (map, guardPosition) = ReadMap();
        return Walk(map, guardPosition).visitedPositions.Count;
    }

    public override long SolveSecondPart()
    {
        var sc = Stopwatch.StartNew();

        var (map, guardPosition) = ReadMap();

        var (initialVisitedPositions, _) = Walk(map, guardPosition);

        var obstaclePlacements = 0;
        var obstacleQueue = new Queue<Position>(initialVisitedPositions.Skip(1).ToList());
        var triedObstaclePlacements = new HashSet<Position>();
        while (obstacleQueue.TryDequeue(out var nextObstaclePosition))
        {
            var mapClone = map.ConvertAll(row => row.ToList());
            mapClone[nextObstaclePosition.Row][nextObstaclePosition.Col] = MapExtensions.TempObstacle;

            var (visitedPositions, containsLoop) = Walk(mapClone, guardPosition);
            if (containsLoop) obstaclePlacements++;

            triedObstaclePlacements.UnionWith(visitedPositions);
            foreach (var position in visitedPositions)
            {
                if (triedObstaclePlacements.Contains(position)) continue;
                else obstacleQueue.Enqueue(position);
            }
        }

        sc.Stop();
        Debug.WriteLine($"Solved part 2 in {sc.Elapsed.TotalMilliseconds}ms");

        return obstaclePlacements;
    }

    private (HashSet<Position> visitedPositions, bool containsLoop) Walk(Map map, Position guardPosition)
    {
        var visitedPositions = new HashSet<Position>();
        Direction direction = new Up();
        var iterations = 0;
        while (map.IsOnMap(guardPosition))
        {
            if (iterations++ > (map.Count * map.Count) / 2) return (visitedPositions, true);
            visitedPositions.Add(guardPosition);

            var nextGuardPosition = new Position(guardPosition.Row + direction.Vertical, guardPosition.Col + direction.Horizontal);
            var directionChanges = 0;
            while (map.IsObstacle(nextGuardPosition) && directionChanges++ < 4)
            {
                direction = GetNextDirection(direction);
                nextGuardPosition = new Position(guardPosition.Row + direction.Vertical, guardPosition.Col + direction.Horizontal);
            }

            if (directionChanges >= 4)
            {
                return (visitedPositions, true);
            }

            if (directionChanges >= 4) return (visitedPositions, true);

            guardPosition = nextGuardPosition;
        }

        return (visitedPositions, false);
    }

    private (Map map, Position guardPosition) ReadMap()
    {
        var map = _puzzleInput.Select(row => row.ToList()).ToList();
        var guardPosition = map.LocateGuard();

        return (map, guardPosition);
    }

    private static Direction GetNextDirection(Direction direction)
        => direction switch
        {
            Up => new Right(),
            Right => new Down(),
            Down => new Left(),
            Left => new Up(),
            _ => throw new NotImplementedException()
        };
}


file static class MapExtensions
{
    public const char TempObstacle = 'O';

    private const char _obstacle = '#';
    private const char _guard = '^';

    public static Position LocateGuard(this Map map)
    {
        for (var row = 0; row < map.Count; row++)
        {
            for (var col = 0; col < map[row].Count; col++)
            {
                if (map[row][col] == _guard) return new Position(row, col);
            }
        }

        throw new InvalidOperationException("Cannot find guard on map");
    }

    public static bool IsObstacle(this Map map, Position position)
        => map.IsOnMap(position) && map[position.Row][position.Col] is _obstacle or TempObstacle;

    public static bool IsOnMap(this Map map, Position position)
        => position.Row >= 0 && position.Col >= 0
            && position.Row < map.Count && position.Col < map[0].Count;

    public static string Trace(this Map map) => map.Trace([]);

    public static string Trace(this Map map, HashSet<Position> visitedPositions)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < map.Count; i++)
        {
            for (int j = 0; j < map[0].Count; j++)
            {
                sb.Append(visitedPositions.Contains(new Position(i, j)) ? 'X' : map[i][j]);
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }
}
