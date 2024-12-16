using System.Text.RegularExpressions;

namespace AdventOfCode2024.Day03;

internal abstract record Instruction();
internal sealed record Mul(int Arg1, int Arg2) : Instruction
{
    public int Product => Arg1 * Arg2;
}
internal sealed record Do() : Instruction;
internal sealed record Dont() : Instruction;

[PuzzleInfo(3, "Mull It Over")]
internal partial class Solution() : Puzzle(3)
{
    public override long SolveFirstPart()
    {
        var instructions = GetInstructions().Where(type => type is Mul).ToList();
        return Execute(instructions);
    }

    public override long SolveSecondPart()
    {
        var instructions = GetInstructions().ToList();
        return Execute(instructions);
    }

    private static int Execute(List<Instruction> instructions)
        => instructions.Aggregate((calcIsEnabled: true, sum: 0), (state, instruction) => instruction switch
        {
            Mul inst => state.calcIsEnabled ? (state.calcIsEnabled, state.sum + inst.Product) : state,
            Do inst => (true, state.sum),
            Dont inst => (false, state.sum),
            _ => throw new InvalidOperationException("Unknown instruction.")
        }).sum;

    private List<Instruction> GetInstructions()
        => InstructionRegex().Matches(ReadConcatenatedInput()).Select(GetInstruction).ToList();

    private static Instruction GetInstruction(Match instructionString)
    {
        var instruction = instructionString.Groups["instr"].Value;
        switch (instruction)
        {
            case "do":
                return new Do();
            case "don't":
                return new Dont();
            case "mul":
                var left = int.Parse(instructionString.Groups["arg1"].Value);
                var right = int.Parse(instructionString.Groups["arg2"].Value);
                return new Mul(left, right);
            default:
                throw new InvalidOperationException($"Unknown command '{instruction}'");
        }
    }

    [GeneratedRegex(@"(?:(?<instr>mul)\((?<arg1>\d+),(?<arg2>\d+)\))|(?:(?<instr>do)\(\))|(?:(?<instr>don\'t)\(\))")]
    private static partial Regex InstructionRegex();

    private string ReadConcatenatedInput() => string.Join("", _puzzleInput);
}
