namespace AdventOfCode2024.Day13;

internal record struct Button(double Dx, double Dy);
internal record struct Prize(long X, long Y)
{
    public Prize Adjust() => this with { X = X + 10000000000000, Y = Y + 10000000000000 };
}

[PuzzleInfo(13, "Claw Contraption")]
internal sealed class Solution() : Puzzle(13)
{
    public override string SolveFirstPart() => PushButtons(adjustForConversion: false).ToString();

    public override string SolveSecondPart() => PushButtons(adjustForConversion: true).ToString();

    private long PushButtons(bool adjustForConversion)
    {
        return GetClawMachines()
            .Select(cm => ButtonPushes(cm.buttonA, cm.buttonB, adjustForConversion ? cm.prize.Adjust() : cm.prize))
            .Where(push => (long)push.a == push.a && (long)push.b == push.b)
            .Sum(x => (long)(x.a * 3 + x.b));
    }

    private static (double a, double b) ButtonPushes(Button buttonA, Button buttonB, Prize prize)
    {
        var b = (buttonA.Dx * prize.Y - buttonA.Dy * prize.X) / (buttonA.Dx * buttonB.Dy - buttonA.Dy * buttonB.Dx);
        var a = (prize.X - buttonB.Dx * b) / buttonA.Dx;

        return (a, b);
    }

    private IEnumerable<(Button buttonA, Button buttonB, Prize prize)> GetClawMachines()
    {
        for (int i = 0; i < _puzzleInput.Length; i += 4)
        {
            yield return (ParseButton(_puzzleInput[i]), ParseButton(_puzzleInput[i + 1]), ParsePrize(_puzzleInput[i + 2]));
        }
    }

    private static Button ParseButton(string input)
    {
        var deltas = input[10..].Split(", ", 2).Select(b => int.Parse(b[2..])).ToList();
        return new Button(deltas[0], deltas[1]);
    }

    private static Prize ParsePrize(string input)
    {
        var coordinates = input[7..].Split(", ", 2).Select(b => int.Parse(b[2..])).ToList();
        return new Prize(coordinates[0], coordinates[1]);
    }
}
