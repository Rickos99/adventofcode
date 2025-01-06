using AdventOfCode.Core.Models;

namespace AdventOfCode.Core.Utility.Collections;
public static class Collection2DGenerator
{
    public static List<List<T>> Generate<T>(Position size, T defaultValue)
        => Generate(size.Row, size.Col, defaultValue);

    public static List<List<T>> Generate<T>(int rows, int cols, T defaultValue)
    {
        var map = new List<List<T>>();
        for (int row = 0; row < rows; row++)
        {
            map.Add([]);
            for (int col = 0; col < cols; col++)
            {
                map[row].Add(defaultValue);
            }
        }
        return map;
    }
}
