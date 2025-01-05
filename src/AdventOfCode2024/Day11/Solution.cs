namespace AdventOfCode2024.Day11;

[PuzzleInfo(11, "Plutonian Pebbles")]
internal sealed class Solution() : Puzzle(11)
{
    private readonly static Dictionary<(long stone, int blinks), long> _memo = [];

    public override string SolveFirstPart() => Solve(GetStones(), 25).ToString();
    public override string SolveSecondPart() => Solve(GetStones(), 75).ToString();

    private static long Solve(List<long> stones, int blinks)
    {
        return stones.Sum(stone => CountStonesAfterBlink(stone, blinks));
    }

    private static long CountStonesAfterBlink(long stone, int blinks)
    {
        if (_memo.TryGetValue((stone, blinks), out var count)) return count;
        if (blinks == 0) return _memo[(stone, blinks)] = 1;

        count = PlutofyStone(stone).Sum(s => CountStonesAfterBlink(s, blinks - 1));
        return _memo[(stone, blinks)] = count;
    }

    private static List<long> PlutofyStone(long stone)
    {
        if (stone == 0)
        {
            return [1];
        }
        else if (IsEvenDigitCount(stone, out var numberOfDigits))
        {
            var divider = (int)Math.Pow(10, (numberOfDigits + 1) / 2);
            return [stone / divider, stone % divider];
        }
        else
        {
            return [stone * 2024];
        }
    }

    private static bool IsEvenDigitCount(long number, out int numberOfDigits)
    {
        numberOfDigits = (int)Math.Floor(Math.Log10(number) + 1);
        return numberOfDigits % 2 == 0;
    }

    private List<long> GetStones()
        => _puzzleInput[0].Split(' ').Select(long.Parse).ToList();
}
