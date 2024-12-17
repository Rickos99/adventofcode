namespace AdventOfCode2024.Day09;

internal sealed class File(int? Id, int BlockSize)
{
    public int? Id { get; set; } = Id;
    public int BlockSize { get; set; } = BlockSize;
}

[PuzzleInfo(9, "Disk Fragmenter")]
internal sealed class Solution() : Puzzle(9)
{
    public override long SolveFirstPart()
    {
        var blocks = ReadBlocksFromDisk();
        MoveFiles(blocks);
        return Checksum(blocks);
    }

    public override long SolveSecondPart()
    {
        var files = ReadFilesFromDisk();
        MoveFiles(files);
        return Checksum(files);
    }

    private static long Checksum(List<File> files)
    {
        var sum = 0L;
        var index = 0;
        for (int i = 0; i < files.Count; i++)
        {
            if (files[i].Id is null)
            {
                index += files[i].BlockSize;
                continue;
            }

            for (int j = 0; j < files[i].BlockSize; j++)
            {
                sum += (int)files[i].Id! * index++;
            }
        }

        return sum;
    }

    private static void MoveFiles(List<File> files)
    {
        for (int rightPointer = files.Count - 1; rightPointer >= 0; rightPointer--)
        {
            var rightFile = files[rightPointer];
            if (rightFile.Id is null) continue;

            for (int leftPointer = 0; leftPointer < files.Count; leftPointer++)
            {
                if (leftPointer == rightPointer) break;

                var leftFile = files[leftPointer];
                if (leftFile.Id is not null) continue;

                if (leftFile.BlockSize >= rightFile.BlockSize)
                {
                    var blockSizeDiff = leftFile.BlockSize - rightFile.BlockSize;

                    leftFile.Id = rightFile.Id;
                    leftFile.BlockSize = rightFile.BlockSize;
                    rightFile.Id = null;

                    if (blockSizeDiff > 0) files.Insert(leftPointer + 1, new File(null, blockSizeDiff));

                    break;
                }
            }
        }
    }

    private List<File> ReadFilesFromDisk()
        => _puzzleInput[0]
            .Select(c => c - '0')
            .Select((blockSize, id) => new File(id % 2 == 0 ? id / 2 : null, blockSize))
            .ToList();

    private List<File> ReadBlocksFromDisk()
        => _puzzleInput[0]
            .Select(c => c - '0')
            .SelectMany((blockSize, id) => Enumerable.Repeat<object?>(null, blockSize).Select(_ => new File(id % 2 == 0 ? id / 2 : null, 1)))
            .ToList();
}
