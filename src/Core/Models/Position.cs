namespace AdventOfCode.Core.Models;

public record struct Position(int Row, int Col)
{
    public static Position operator +(Position a, Position b) => new(a.Row + b.Row, a.Col + b.Col);
    public static Position operator -(Position a, Position b) => new(a.Row + b.Row, a.Col + b.Col);
    public static Position operator *(Position a, int b) => new(a.Row * b, a.Col * b);
}