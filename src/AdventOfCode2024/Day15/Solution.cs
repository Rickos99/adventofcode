using AdventOfCode.Core.Models;
using System.Text;

namespace AdventOfCode2024.Day15;

internal class Robot(Position position)
{
    public Position Position { get; set; } = position;
}

internal record Warehouse(List<List<char>> Map, bool HasWideBoxes)
{
    public char this[Position position] => Map[position.Row][position.Col];

    public string ToString(Robot robot)
    {
        var sb = new StringBuilder();
        for (var row = 0; row < Map.Count; row++)
        {
            for (var col = 0; col < Map[0].Count; col++)
            {
                if (robot.Position == new Position(row, col)) sb.Append('@');
                else sb.Append(Map[row][col]);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

[PuzzleInfo(15, "Warehouse Woes")]
internal sealed class Solution() : Puzzle(15)
{
    public override long SolveFirstPart()
    {
        var (warehouse, instructions, robot) = ParseInput(useWideBoxes: false);

        ExecuteInstructions(warehouse, instructions, robot, interactive: false);

        return warehouse.Map
            .SelectMany((row, rowIndex) => row.Select((cell, colIndex) => new { cell, rowIndex, colIndex }))
            .Where(x => x.cell == 'O')
            .Sum(x => 100 * x.rowIndex + x.colIndex);
    }

    public override long SolveSecondPart()
    {
        var (warehouse, instructions, robot) = ParseInput(useWideBoxes: true);

        ExecuteInstructions(warehouse, instructions, robot, interactive: false);

        return warehouse.Map
            .SelectMany((row, rowIndex) => row.Select((cell, colIndex) => new { cell, rowIndex, colIndex }))
            .Where(x => x.cell == '[')
            .Sum(x => 100 * x.rowIndex + x.colIndex);
    }

    private static void ExecuteInstructions(Warehouse warehouse, List<char> instructions, Robot robot, bool interactive)
    {
        if (interactive) InteractiveWarehouse(warehouse, robot);
        else instructions.ForEach(instruction => ExecuteInstruction(warehouse, instruction, robot));
    }

    private static void InteractiveWarehouse(Warehouse warehouse, Robot robot)
    {
        Console.Clear();
        Console.WriteLine(warehouse.ToString(robot));
        ConsoleKeyInfo instruction;
        while ((instruction = Console.ReadKey()).Key is not ConsoleKey.Escape)
        {
            var direction = instruction.KeyChar switch
            {
                'w' => '^',
                'd' => '>',
                's' => 'v',
                'a' => '<',
                _ => '\0'
            };
            if (direction is '\0') continue;
            ExecuteInstruction(warehouse, direction, robot);
            Console.Clear();
            Console.WriteLine(warehouse.ToString(robot));
        }
    }

    private static void ExecuteInstruction(Warehouse warehouse, char instruction, Robot robot)
    {
        var direction = instruction switch
        {
            '^' => new Position(-1, 0),
            '>' => new Position(0, 1),
            'v' => new Position(1, 0),
            '<' => new Position(0, -1),
            _ => throw new InvalidOperationException("Invalid instruction")
        };

        var nextPosition = new Position(robot.Position.Row + direction.Row, robot.Position.Col + direction.Col);
        if (warehouse[nextPosition] == '#') return;
        if (warehouse[nextPosition] == '.')
        {
            robot.Position = nextPosition;
            return;
        }

        if (warehouse.HasWideBoxes) MoveWideBoxes(warehouse, robot, direction);
        else MoveSmallBoxes(warehouse, robot, direction);
    }

    private static void MoveSmallBoxes(Warehouse warehouse, Robot robot, Position direction)
    {
        var closestBox = robot.Position + direction;
        var nextPosition = closestBox;
        while (warehouse[nextPosition] == 'O')
        {
            nextPosition += direction;
        }

        if (warehouse[nextPosition] == '#') return;
        if (warehouse[nextPosition] == '.')
        {
            warehouse.Map[nextPosition.Row][nextPosition.Col] = 'O';
            warehouse.Map[closestBox.Row][closestBox.Col] = '.';
            robot.Position += direction;
        }
    }

    private static void MoveWideBoxes(Warehouse warehouse, Robot robot, Position direction)
    {
        if (direction.Row == 0)
        {
            if (!PushWideBoxesHorizontally(warehouse, robot.Position + direction, direction, robot)) return;
            robot.Position = robot.Position + direction;
            return;
        }

        var boxesToMove = new List<Position>();
        var robotNextStep = robot.Position + direction;
        if (!PushWideBoxesVertically(warehouse, robotNextStep, direction, boxesToMove))
        {
            return;
        }

        robot.Position = robotNextStep;

        foreach (var box in boxesToMove)
        {
            warehouse.Map[box.Row][box.Col] = '.';
            warehouse.Map[box.Row][box.Col + 1] = '.';
        }

        foreach (var box in boxesToMove)
        {
            warehouse.Map[box.Row + direction.Row][box.Col] = '[';
            warehouse.Map[box.Row + direction.Row][box.Col + 1] = ']';
        }
    }

    private static bool PushWideBoxesHorizontally(Warehouse warehouse, Position box, Position direction, Robot robot)
    {
        box = warehouse[box] == ']' ? box with { Col = box.Col - 1 } : box;

        // Boxes to move
        var boxesToMove = new List<Position>();
        var nextPosition = box;
        while (warehouse[nextPosition] == '[')
        {
            boxesToMove.Add(nextPosition);
            nextPosition += direction * 2;
        }

        // If wall, return false and do not move any box
        var wallPointer = robot.Position + direction;
        while (warehouse[wallPointer] is ('[' or ']'))
        {
            wallPointer += direction;
        }
        if (warehouse[wallPointer] == '#') return false;

        // Clear all spots until next position
        warehouse.Map[robot.Position.Row][robot.Position.Col + direction.Col] = '.';

        // Actually move boxes
        if (direction.Col == -1)
        {
            foreach (var boxToMove in boxesToMove)
            {
                warehouse.Map[boxToMove.Row][boxToMove.Col - 1] = '[';
                warehouse.Map[boxToMove.Row][boxToMove.Col] = ']';
            }
        }
        else
        {
            foreach (var boxToMove in boxesToMove)
            {
                warehouse.Map[boxToMove.Row][boxToMove.Col + 1] = '[';
                warehouse.Map[boxToMove.Row][boxToMove.Col + 2] = ']';
            }
        }

        return true;
    }

    private static bool PushWideBoxesVertically(Warehouse warehouse, Position box, Position direction, List<Position> boxesToMove)
    {
        box = warehouse[box] == ']' ? box with { Col = box.Col - 1 } : box;

        var boxRightSide = box with { Col = box.Col + 1 };
        if (warehouse[box + direction] == '#' || warehouse[boxRightSide + direction] == '#') return false;

        boxesToMove.Add(box);

        var leftNextBoxPosition = box + direction with { Col = direction.Col - 1 };
        var rightNextBoxPosition = box + direction with { Col = direction.Col + 1 };
        var adjecentNextBoxPostion = box + direction;

        if (warehouse[leftNextBoxPosition] == '[' && warehouse[rightNextBoxPosition] == '[')
            return PushWideBoxesVertically(warehouse, leftNextBoxPosition, direction, boxesToMove)
                && PushWideBoxesVertically(warehouse, rightNextBoxPosition, direction, boxesToMove);
        else if (warehouse[leftNextBoxPosition] == '[')
            return PushWideBoxesVertically(warehouse, leftNextBoxPosition, direction, boxesToMove);
        else if (warehouse[rightNextBoxPosition] == '[')
            return PushWideBoxesVertically(warehouse, rightNextBoxPosition, direction, boxesToMove);
        else if (warehouse[adjecentNextBoxPostion] == '[')
            return PushWideBoxesVertically(warehouse, adjecentNextBoxPostion, direction, boxesToMove);
        else return true;
    }

    private (Warehouse warehouse, List<char> instructions, Robot robot) ParseInput(bool useWideBoxes)
    {
        var warehouseMap = new List<List<char>>();
        var instructions = new List<char>();
        var robot = default(Robot);

        var inputLine = 0;
        for (; _puzzleInput[inputLine] != ""; inputLine++)
        {
            warehouseMap.Add([]);
            for (var i = 0; i < _puzzleInput[inputLine].Length; i++)
            {
                if (_puzzleInput[inputLine][i] == '@')
                {
                    robot = new Robot(new(inputLine, i));
                    warehouseMap[inputLine].Add('.');
                }
                else warehouseMap[inputLine].Add(_puzzleInput[inputLine][i]);
            }
        }

        for (; inputLine < _puzzleInput.Length; inputLine++)
        {
            instructions.AddRange(_puzzleInput[inputLine].ToList());
        }

        if (robot is default(Robot)) throw new InvalidOperationException("Robot not found");

        if (useWideBoxes) warehouseMap = ConvertToWideBoxes(warehouseMap, robot);

        return (new Warehouse(warehouseMap, useWideBoxes), instructions, robot);
    }

    private static List<List<char>> ConvertToWideBoxes(List<List<char>> warehouse, Robot robot)
    {
        var largeWarehouse = new List<List<char>>(warehouse.Select(row => new List<char>(Enumerable.Repeat('\0', row.Count * 2)).ToList()));
        for (var row = 0; row < warehouse.Count; row++)
        {
            for (var col = 0; col < warehouse[0].Count; col++)
            {
                largeWarehouse[row][col * 2] = warehouse[row][col] == 'O' ? '[' : warehouse[row][col];
                largeWarehouse[row][col * 2 + 1] = warehouse[row][col] == 'O' ? ']' : warehouse[row][col];
            }
        }
        robot.Position = robot.Position with { Col = robot.Position.Col * 2 };
        return largeWarehouse;
    }
}
