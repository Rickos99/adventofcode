namespace AdventOfCode.Core;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class PuzzleInfoAttribute : Attribute
{
    public int Day { get; private init; }

    public string Name { get; private init; }

    public PuzzleInfoAttribute(int day, string name)
    {
        Day = day;
        Name = name;
    }
}
