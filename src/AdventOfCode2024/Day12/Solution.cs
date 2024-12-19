using AdventOfCode.Core.Extensions;
using AdventOfCode.Core.Models;

namespace AdventOfCode2024.Day12;

public record struct RegionInformation(int Area, int Perimeter, int Sides);

[PuzzleInfo(12, "Garden Groups")]
internal sealed class Solution() : Puzzle(12)
{
    public override long SolveFirstPart() => GetRegionInformation().Sum(region => region.Area * region.Perimeter);

    public override long SolveSecondPart() => GetRegionInformation().Sum(region => region.Area * region.Sides);

    private IEnumerable<RegionInformation> GetRegionInformation()
    {
        var visitedPlants = new HashSet<Position>();
        for (int row = 0; row < _puzzleInput.Length; row++)
        {
            for (int col = 0; col < _puzzleInput[0].Length; col++)
            {
                if (visitedPlants.Contains(new(row, col))) continue;
                yield return RegionInformation(new(row, col), visitedPlants);
            }
        }
    }

    private RegionInformation RegionInformation(Position startPosition, HashSet<Position> visitedPlants)
    {
        var perimeter = 0;

        var regionPlant = _puzzleInput[startPosition.Row][startPosition.Col];
        var queue = new Queue<Position>([startPosition]);
        var regionPositions = new HashSet<Position>();

        while (queue.TryDequeue(out var position))
        {
            regionPositions.Add(position);
            if (!visitedPlants.Add(position)) continue;

            var adjecentPositionsInSameRegion = GetAdjacentPositions(position)
                .Where(nextPosition => IsWithinBoundsAndMatchesPlant(nextPosition, regionPlant))
                .ToList();

            perimeter += 4 - adjecentPositionsInSameRegion.Count;

            queue.EnqueueRange(adjecentPositionsInSameRegion, p => !visitedPlants.Contains(p));
        }

        var sides = CountSides(regionPositions);
        return new(regionPositions.Count, perimeter, sides);
    }

    private static int CountSides(HashSet<Position> region)
    {
        var (colLowerBound, colUpperBound, rowLowerBound, rowUpperBound) = GetBounds(region);

        var sides = 0;
        for (int col = colLowerBound; col <= colUpperBound; col++)
        {
            for (int row = rowLowerBound; row <= rowUpperBound; row++)
            {
                var previousRowWasRegion = region.Contains(new(row - 1, col));
                var previousColWasRegion = region.Contains(new(row, col - 1));

                var hasLeftSide = region.Contains(new(row, col)) && !previousColWasRegion;
                var previousRowHasLeftSide = previousRowWasRegion && !region.Contains(new(row - 1, col - 1));
                if (hasLeftSide && (!previousRowWasRegion || !previousRowHasLeftSide)) sides++;

                var hasRightSide = region.Contains(new(row, col)) && !region.Contains(new(row, col + 1));
                var previousRowHasRightSide = previousRowWasRegion && !region.Contains(new(row - 1, col + 1));
                if (hasRightSide && (!previousRowWasRegion || !previousRowHasRightSide)) sides++;

                var hasUpperSide = region.Contains(new(row, col)) && !previousRowWasRegion;
                var previousColHasUpperSide = previousColWasRegion && !region.Contains(new(row - 1, col - 1));
                if (hasUpperSide && (!previousColWasRegion || !previousColHasUpperSide)) sides++;

                var hasLowerSide = region.Contains(new(row, col)) && !region.Contains(new(row + 1, col));
                var previousColHasLowerSide = previousColWasRegion && !region.Contains(new(row + 1, col - 1));
                if (hasLowerSide && (!previousColWasRegion || !previousColHasLowerSide)) sides++;
            }
        }

        return sides;
    }

    private static (int colLowerBound, int colUpperBound, int rowLowerBound, int rowUpperBound) GetBounds(HashSet<Position> region)
    {
        int colLowerBound = int.MaxValue;
        int colUpperBound = int.MinValue;
        int rowLowerBound = int.MaxValue;
        int rowUpperBound = int.MinValue;

        foreach (var position in region)
        {
            if (position.Col < colLowerBound) colLowerBound = position.Col;
            if (position.Col > colUpperBound) colUpperBound = position.Col;
            if (position.Row < rowLowerBound) rowLowerBound = position.Row;
            if (position.Row > rowUpperBound) rowUpperBound = position.Row;
        }

        return (colLowerBound, colUpperBound, rowLowerBound, rowUpperBound);
    }

    private static List<Position> GetAdjacentPositions(Position position)
        => [
            new(position.Row - 1, position.Col),
            new(position.Row + 1, position.Col),
            new(position.Row, position.Col - 1),
            new(position.Row, position.Col + 1)
        ];

    private bool IsWithinBoundsAndMatchesPlant(Position nextPosition, char regionPlant)
        => nextPosition.Row >= 0 && nextPosition.Row < _puzzleInput.Length
        && nextPosition.Col >= 0 && nextPosition.Col < _puzzleInput[0].Length
        && _puzzleInput[nextPosition.Row][nextPosition.Col] == regionPlant;
}
