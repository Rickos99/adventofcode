namespace AdventOfCode2024.Day07;

internal record struct Equation(long Value, List<long> Terms);

[PuzzleInfo(7, "Bridge Repair")]
internal sealed class Solution() : Puzzle(7)
{
    public override long SolveFirstPart()
    {
        return ReadEquations()
            .Where(equation => Evaluates(equation, equation.Terms[0], 0))
            .Sum(equation => equation.Value);
    }

    public override long SolveSecondPart()
    {
        return ReadEquations()
            .Where(equation => Evaluates(equation, equation.Terms[0], 0, allowConcatenation: true))
            .Sum(equation => equation.Value);
    }

    private bool Evaluates(Equation equation, long accumulated, int termIndex, bool allowConcatenation = false)
    {
        if (termIndex + 1 == equation.Terms.Count) return accumulated == equation.Value;
        if (accumulated > equation.Value) return false;

        var nextTerm = equation.Terms[termIndex + 1];
        return Evaluates(equation, accumulated + nextTerm, termIndex + 1, allowConcatenation)
            || Evaluates(equation, accumulated * nextTerm, termIndex + 1, allowConcatenation)
            || (allowConcatenation && Evaluates(equation, long.Parse($"{accumulated}{nextTerm}"), termIndex + 1, allowConcatenation));
    }

    private List<Equation> ReadEquations()
    {
        return _puzzleInput.Select(ReadEquation).ToList();
    }

    private static Equation ReadEquation(string equation)
    {
        var equationSplit = equation.Split(": ", 2);
        var equationValue = long.Parse(equationSplit[0]);
        var equationTerms = equationSplit[1].Split(' ').Select(long.Parse).ToList();
        return new Equation(equationValue, equationTerms);
    }
}