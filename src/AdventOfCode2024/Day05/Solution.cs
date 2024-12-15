using AdventOfCode.Core.Extensions;
using Update = System.Collections.Generic.List<int>;

namespace AdventOfCode2024.Day05;

internal record PageOrderRule(int First, int Then);

[PuzzleInfo(5, "Print Queue")]
internal sealed class Solution() : Puzzle(5)
{
    public override int SolveFirstPart()
    {
        var (pageOrderRules, updates) = ParseInput();
        var ruleBook = CreateRuleBook(pageOrderRules);

        return updates
            .Where(update => UpdateIsValid(update, ruleBook))
            .Select(update => update[update.Count / 2])
            .Sum();
    }

    public override int SolveSecondPart()
    {
        var (pageOrderRules, updates) = ParseInput();
        var ruleBook = CreateRuleBook(pageOrderRules);

        var comparer = Comparer<int>.Create((a, b) =>
        {
            if (ruleBook.ContainsKey(a) && ruleBook[a].Contains(b)) return -1;
            if (ruleBook.ContainsKey(b) && ruleBook[b].Contains(a)) return 1;
            return 0;
        });

        return updates
            .Where(update => !UpdateIsValid(update, ruleBook))
            .Select(update => update.Order(comparer).ToList())
            .Sum(update => update[update.Count / 2]);
    }

    private bool UpdateIsValid(Update update, Dictionary<int, HashSet<int>> rulebook)
    {
        for (int i = 0; i < update.Count; i++)
        {
            var page = update[i];
            if (!rulebook.ContainsKey(page)) continue;

            foreach (var item in update.Take(i))
            {
                if (rulebook[page].Contains(item)) return false;
            }
        }
        return true;
    }

    private static Dictionary<int, HashSet<int>> CreateRuleBook(List<PageOrderRule> pageOrderRules)
        => pageOrderRules
            .GroupBy(x => x.First, (key, elements) => (key: key, rules: elements.Select(r => r.Then).ToHashSet()))
            .ToDictionary();

    private (List<PageOrderRule> pageOrderRules, List<Update> updates) ParseInput()
    {
        var sectionSplitIndex = _puzzleInput.IndexOfFirst(string.IsNullOrEmpty);

        var pageOrderRules = _puzzleInput
            .Take(sectionSplitIndex)
            .Select(input =>
            {
                var pageOrderRuleInput = input.Split('|');
                return new PageOrderRule(int.Parse(pageOrderRuleInput[0]), int.Parse(pageOrderRuleInput[1]));
            }).ToList();

        var updates = _puzzleInput
            .Skip(sectionSplitIndex + 1)
            .Select(input => input.Split(',').Select(int.Parse).ToList())
            .ToList();

        return (pageOrderRules, updates);
    }
}
