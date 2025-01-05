using AdventOfCode.Core.Models;
using System.Text;

namespace AdventOfCode2024.Day08;


[PuzzleInfo(8, "Resonant Collinearity")]
internal sealed class Solution() : Puzzle(8)
{
    public override string SolveFirstPart()
    {
        var map = ReadMap();
        var antennas = CreateAntennaGroups(map);
        var antennaPairs = CreateAntennaPairs(antennas);
        var antiNodes = CalculateAntiNodes(antennaPairs, map, continousAntiNodes: false);
        //Console.WriteLine(map.ToString(antiNodes));

        return antiNodes.Count.ToString();
    }

    public override string SolveSecondPart()
    {
        var map = ReadMap();
        var antennas = CreateAntennaGroups(map);
        var antennaPairs = CreateAntennaPairs(antennas);
        var antiNodes = CalculateAntiNodes(antennaPairs, map, continousAntiNodes: true);
        //Console.WriteLine(map.ToString(antiNodes));

        return antiNodes.Count.ToString();
    }

    private static HashSet<Position> CalculateAntiNodes(IEnumerable<(Position antennaA, Position antennaB)> antennaPairs, Map map, bool continousAntiNodes)
        => antennaPairs.SelectMany(pair => GenerateAntiNodes(pair, map, continousAntiNodes)).ToHashSet();

    private static IEnumerable<Position> GenerateAntiNodes((Position antennaA, Position antennaB) antennaPair, Map map, bool continuous = false)
    {
        var (antennaA, antennaB) = antennaPair;
        var rowDist = antennaA.Row - antennaB.Row;
        var colDist = antennaA.Col - antennaB.Col;

        if (continuous)
        {
            for (var iteration = 0; ; iteration++)
            {
                var nodeA = new Position(antennaA.Row + rowDist * iteration, antennaA.Col + colDist * iteration);
                if (!map.IsOnMap(nodeA)) break;
                yield return nodeA;
            }

            for (var iteration = 0; ; iteration++)
            {
                var nodeB = new Position(antennaB.Row - rowDist * iteration, antennaB.Col - colDist * iteration);
                if (!map.IsOnMap(nodeB)) break;
                yield return nodeB;
            }
        }
        else
        {
            var nodeA = new Position(antennaA.Row + rowDist, antennaA.Col + colDist);
            if (map.IsOnMap(nodeA)) yield return nodeA;

            var nodeB = new Position(antennaB.Row - rowDist, antennaB.Col - colDist);
            if (map.IsOnMap(nodeB)) yield return nodeB;
        }
    }

    private IEnumerable<(Position antennaA, Position antennaB)> CreateAntennaPairs(Dictionary<char, List<Position>> antennaGroups)
    {
        foreach (var (_, antennas) in antennaGroups)
        {
            for (int i = 0; i < antennas.Count; i++)
            {
                for (int j = i + 1; j < antennas.Count; j++)
                {
                    yield return (antennas[i], antennas[j]);
                }
            }
        }
    }

    private Dictionary<char, List<Position>> CreateAntennaGroups(Map map)
    {
        var antennas = new Dictionary<char, List<Position>>();
        for (int row = 0; row < map.Count; row++)
        {
            for (int col = 0; col < map[0].Count; col++)
            {
                var antennaGroupSymbol = map[row][col];
                if (antennaGroupSymbol == Map.EmptyPosition) continue;

                if (!antennas.ContainsKey(antennaGroupSymbol))
                    antennas.Add(antennaGroupSymbol, []);

                antennas[antennaGroupSymbol].Add(new Position(row, col));
            }
        }
        return antennas;
    }

    private Map ReadMap() => new Map(_puzzleInput.Select(Enumerable.ToList).ToList());
}

internal sealed class Map
{
    private readonly List<List<char>> _internalMap;

    public Map(List<List<char>> map)
    {
        _internalMap = map;
    }

    public const char EmptyPosition = '.';
    public const char Node = '#';

    public List<char> this[int x] => _internalMap[x];

    public int Count => _internalMap.Count;

    public bool IsOnMap(Position position)
        => position.Row >= 0 && position.Col >= 0
        && position.Row < _internalMap.Count && position.Col < _internalMap[0].Count;

    public string ToString(HashSet<Position> antiNodes)
    {
        var sb = new StringBuilder();
        for (int row = 0; row < _internalMap.Count; row++)
        {
            for (int col = 0; col < _internalMap[row].Count; col++)
            {
                if (antiNodes.Contains(new Position(row, col))) sb.Append(Node);
                else sb.Append(_internalMap[row][col]);
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }
}