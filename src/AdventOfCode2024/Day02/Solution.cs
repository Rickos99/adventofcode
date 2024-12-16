using AdventOfCode.Core.Extensions;

namespace AdventOfCode2024.Day02;

internal record struct Report(List<int> Levels)
{
    public readonly bool IsDeacreasing => Levels[0] > Levels[1];
}

[PuzzleInfo(2, "Red-Nosed Reports")]
internal sealed class Solution() : Puzzle(2)
{
    public override long SolveFirstPart()
    {
        return GetReports().Count(IsValidReport);
    }

    public override long SolveSecondPart()
    {
        return GetReports().Count(IsValidReportWithDampener);
    }

    private bool IsValidReport(Report report)
    {
        var prev = report.Levels.First();
        foreach (var level in report.Levels.Skip(1))
        {
            if (!IsValidLevel(prev, level, report.IsDeacreasing)) return false;
            prev = level;
        }
        return true;
    }

    private bool IsValidReportWithDampener(Report report)
    {
        if (IsValidReport(report)) return true;

        for (int i = 0; i < report.Levels.Count; i++)
        {
            var leanerReport = report with { Levels = report.Levels.RemoveElementAt(i).ToList() };
            if (IsValidReport(leanerReport)) return true;
        }
        return false;
    }

    private bool IsValidLevel(int previousLevel, int currentLevel, bool shouldBeDecreasingLevel)
    {
        if (shouldBeDecreasingLevel && previousLevel < currentLevel) return false;
        if (!shouldBeDecreasingLevel && previousLevel > currentLevel) return false;

        var prevDiff = Math.Abs(currentLevel - previousLevel);
        if (prevDiff is 0 or > 3) return false;
        return true;
    }

    private IEnumerable<Report> GetReports()
        => _puzzleInput.Select(line => new Report(line.Split().Select(int.Parse).ToList()));
}
