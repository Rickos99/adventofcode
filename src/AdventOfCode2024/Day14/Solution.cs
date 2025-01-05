using AdventOfCode.Core.Extensions;
using AdventOfCode.Core.Models;
using System.Text.RegularExpressions;

namespace AdventOfCode2024.Day14;

[PuzzleInfo(14, "Restroom Redoubt")]
internal sealed partial class Solution() : Puzzle(14)
{
    private const int _spaceHeight = 103;
    private const int _spaceWidth = 101;

    public override string SolveFirstPart()
    {
        var robots = GetRobots();
        for (var second = 0; second < 100; second++)
        {
            robots.ForEach(robot => robot.Move());
        }
        return SafetyFactor(robots).ToString();
    }

    public override string SolveSecondPart()
    {
        var robots = GetRobots();
        for (var second = 1; ; second++)
        {
            robots.ForEach(robot => robot.Move());

            var robotPositions = robots.Select(r => r.Position).ToHashSet();
            foreach (var robot in robots)
            {
                if (RobotClusterAreaSize(robot.Position, robotPositions) > robots.Count / 3)
                {
                    PrintSpace(robots);
                    return second.ToString();
                }
            }
        }
    }

    private static int SafetyFactor(List<Robot> robots)
    {
        var quadrants = new int[4] { 0, 0, 0, 0 };

        foreach (var robot in robots)
        {
            if (robot.Position.Row < _spaceHeight / 2 && robot.Position.Col < _spaceWidth / 2) quadrants[0]++;
            else if (robot.Position.Row < _spaceHeight / 2 && robot.Position.Col > _spaceWidth / 2) quadrants[1]++;
            else if (robot.Position.Row > _spaceHeight / 2 && robot.Position.Col < _spaceWidth / 2) quadrants[2]++;
            else if (robot.Position.Row > _spaceHeight / 2 && robot.Position.Col > _spaceWidth / 2) quadrants[3]++;
        }

        return quadrants.Aggregate(1, (x, y) => x * y);
    }

    private List<Robot> GetRobots()
    {
        var robots = new List<Robot>();
        var regex = new Regex(@"-?\d+", RegexOptions.NonBacktracking);
        foreach (var input in _puzzleInput)
        {
            var values = regex.Matches(input).Select(m => int.Parse(m.Value)).ToList();
            var position = new Position(values[1], values[0]);
            var velocity = new Position(values[3], values[2]);
            robots.Add(new Robot(position, velocity));
        }
        return robots;
    }

    private static void PrintSpace(List<Robot> robots)
    {
        var space = new char[_spaceHeight, _spaceWidth];
        foreach (var robot in robots)
        {
            space[robot.Position.Row, robot.Position.Col] = '#';
        }
        for (var i = 0; i < _spaceHeight; i++)
        {
            for (var j = 0; j < _spaceWidth; j++)
            {
                Console.Write(space[i, j] == '#' ? '#' : '.');
            }
            Console.WriteLine();
        }
    }

    private class Robot(Position position, Position velocity)
    {
        public Position Position { get; private set; } = position;
        public Position Velocity { get; } = velocity;

        public void Move()
        {
            Position = Position with
            {
                Col = (_spaceWidth + Position.Col + Velocity.Col) % _spaceWidth,
                Row = (_spaceHeight + Position.Row + Velocity.Row) % _spaceHeight,
            };
        }
    }

    private static int RobotClusterAreaSize(Position robot, HashSet<Position> otherRobots)
    {
        var queue = new Queue<Position>([robot]);
        var regionPositions = new HashSet<Position>();
        while (queue.TryDequeue(out var position))
        {
            regionPositions.Add(position);
            queue.EnqueueRange(GetAdjacentPositions(position).Where(p => !regionPositions.Contains(p) && otherRobots.Contains(p)));
        }

        return regionPositions.Count;
    }

    private static List<Position> GetAdjacentPositions(Position position)
        => [
            new(position.Row - 1, position.Col),
            new(position.Row + 1, position.Col),
            new(position.Row, position.Col - 1),
            new(position.Row, position.Col + 1)
        ];
}