namespace AdventOfCode2024.Day01;

[PuzzleInfo(1, "Historian Hysteria")]
internal sealed class Solution() : Puzzle(1)
{
    public override string SolveFirstPart()
    {
        var (left, right) = ReadLists();
        left.Sort();
        right.Sort();
        return left.Zip(right, (l, r) => Math.Abs(l - r)).Sum().ToString();
    }

    public override string SolveSecondPart()
    {
        var (left, right) = ReadLists();

        var rightIndex = right
            .GroupBy(x => x, (key, elements) => (key: key, value: elements.Count()))
            .ToDictionary();

        var similarityScore = left
            .Where(rightIndex.ContainsKey)
            .Sum(left => left * rightIndex[left]);

        return similarityScore.ToString();
    }

    private (List<int> left, List<int> right) ReadLists()
    {
        var left = new List<int>();
        var right = new List<int>();
        foreach (var line in _puzzleInput)
        {
            var parts = line.Split("   ");
            left.Add(int.Parse(parts[0]));
            right.Add(int.Parse(parts[1]));
        }
        return (left, right);
    }
}
