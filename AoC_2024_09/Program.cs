
namespace AoC_2024_09;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";

        string[] filesystemPartOne = LoadDataPartOne(filename);
        //Console.WriteLine("Initial state:");
        //Console.WriteLine(string.Join(",", filesystem));
        //Console.WriteLine("--------------");

        CompactPartOne(filesystemPartOne);

        //Console.WriteLine("Compacted state:");
        //Console.WriteLine(string.Join(",",filesystem));
        //Console.WriteLine("--------------");

        long checksum = CalculateChecksum(filesystemPartOne);

        Console.WriteLine($"Part One Checksum: {checksum}");


        List<Block> filesystemPartTwo = LoadDataPartTwo(filename);

        CompactPartTwo(filesystemPartTwo);

        //Console.WriteLine(string.Join("", filesystemPartTwo.Select(x => string.Join("", Enumerable.Repeat(x.Id, x.Length)))));

        string[] newFilesystemPartTwo = filesystemPartTwo.SelectMany(x => Enumerable.Repeat(x.Id, x.Length)).ToArray();

        checksum = CalculateChecksum(newFilesystemPartTwo);

        Console.WriteLine($"Part Two Checksum: {checksum}");
    }    

    private static long CalculateChecksum(string[] filesystem)
    {
        long checksum = 0;
        for (long i = 0; i < filesystem.Length; i++)
        {
            if (filesystem[i] != ".")            
            {
                checksum += i * long.Parse(filesystem[i]);
            }
        }
        return checksum;
    }

    private static void CompactPartOne(string[] filesystem)
    {
        int freeSpacePos = 0;
        int blockPos = filesystem.Length - 1;
        while (freeSpacePos < blockPos)
        {
            for (freeSpacePos = 0; freeSpacePos < filesystem.Length; freeSpacePos++)
            {
                if (filesystem[freeSpacePos] == ".")
                {
                    break;
                }
            }
            for (blockPos = filesystem.Length - 1; blockPos >= 0; blockPos--)
            {
                if (filesystem[blockPos] != ".")
                {
                    break;
                }
            }
            if (freeSpacePos < blockPos)
            {
                filesystem[freeSpacePos] = filesystem[blockPos];
                filesystem[blockPos] = ".";
            }
        }
    }

    private static void CompactPartTwo(List<Block> filesystem)
    {
        
        for(int currentId = filesystem.Where(x=>x.Id!=".").Max(x=>int.Parse(x.Id)); currentId>0; currentId--)
        {
            var current = filesystem.Where(x => x.Id == currentId.ToString()).First();

            int lastIndex = filesystem.IndexOf(current);

            for (int i = 0; i < lastIndex; i++)
            {
                if (filesystem[i].Id=="." && filesystem[i].Length>=current.Length)
                {                    
                    int lengthDiff = filesystem[i].Length - current.Length;
                    filesystem[i].Id = current.Id;
                    current.Id = ".";

                    if (lengthDiff > 0)
                    {
                        filesystem[i].Length=current.Length;
                        filesystem.Insert(i+1,new Block { Id=".", Length=lengthDiff });
                    }
                    
                    break;
                }
            }
        }
    }

    private static string[] LoadDataPartOne(string filename)
    {
        string line = File.ReadAllText(filename);

        List<string> output = new List<string>();
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] != '\n')
            {
                int blocks = int.Parse(line[i].ToString());
                if (i % 2 == 0)
                {
                    output.AddRange(Enumerable.Repeat((i / 2).ToString(), blocks));
                }
                else
                {
                    output.AddRange(Enumerable.Repeat(".", blocks));
                }
            }
        }

        return output.ToArray();
    }

    private static List<Block> LoadDataPartTwo(string filename)
    {
        string line = File.ReadAllText(filename);

        List<Block> output = new List<Block>();
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] != '\n')
            {
                int blocks = int.Parse(line[i].ToString());
                if (i % 2 == 0)
                {
                    output.Add(new Block { Length = blocks, Id = (i / 2).ToString() });
                }
                else
                {
                    output.Add(new Block { Length = blocks, Id = "." });
                }
            }
        }

        return output;
    }
}

internal class Block
{
    public required int Length { get; set; }
    public required string Id { get; set; }

}
