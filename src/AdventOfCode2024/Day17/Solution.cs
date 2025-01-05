using AdventOfCode.Core.Extensions;

namespace AdventOfCode2024.Day17;

[PuzzleInfo(17, "Chronospatial Computer")]
internal sealed class Solution() : Puzzle(17)
{
    public override string SolveFirstPart() => LoadProgram().Run();

    public override string SolveSecondPart() => string.Empty;

    private Program LoadProgram()
        => new Program
        {
            Memory = new Memory
            {
                RegisterA = _puzzleInput[0][12..].ToInt(),
                RegisterB = _puzzleInput[1][12..].ToInt(),
                RegisterC = _puzzleInput[2][12..].ToInt(),
            },
            Instructions = _puzzleInput[4][9..].Split(',').Select(int.Parse).ToList()
        };
}

internal sealed class Program
{
    public required List<int> Instructions { get; init; }
    public required Memory Memory { get; init; }
    public List<long> Output { get; } = [];
    public int InstructionPointer { get; set; }

    public long GetComboOperand(int operand)
        => operand switch
        {
            > 0 and <= 3 => operand,
            4 => Memory.RegisterA,
            5 => Memory.RegisterB,
            6 => Memory.RegisterC,
            _ => throw new InvalidOperationException($"Unknown operand: {operand}")
        };

    public string Run()
    {
        Action<Memory>? nextInstruction;
        while ((nextInstruction = GetNextInstruction()) is not null)
        {
            nextInstruction?.Invoke(Memory);
            InstructionPointer += 2;
        }
        return string.Join(',', Output);
    }

    private Action<Memory>? GetNextInstruction()
    {
        if (InstructionPointer > Instructions.Count - 1) return null;

        var opcode = Instructions[InstructionPointer];
        var operand = Instructions[InstructionPointer + 1];
        return opcode switch
        {
            0 => mem => mem.RegisterA /= 1 << (int)GetComboOperand(operand),
            1 => mem => mem.RegisterB ^= operand,
            2 => mem => mem.RegisterB = GetComboOperand(operand) % 8,
            3 => mem => InstructionPointer = mem.RegisterA == 0 ? InstructionPointer : operand - 2,
            4 => mem => mem.RegisterB ^= mem.RegisterC,
            5 => mem => Output.Add(GetComboOperand(operand) % 8),
            6 => mem => mem.RegisterB = mem.RegisterA / (1 << (int)GetComboOperand(operand)),
            7 => mem => mem.RegisterC = mem.RegisterA / (1 << (int)GetComboOperand(operand)),
            _ => throw new InvalidOperationException($"Unknown opcode: {opcode}")
        };
    }
}

internal sealed class Memory
{
    public long RegisterA { get; set; }
    public long RegisterB { get; set; }
    public long RegisterC { get; set; }
}
