namespace AoC_2024_09;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";

        string[] filesystem = LoadData(filename);
        //Console.WriteLine("Initial state:");
        //Console.WriteLine(string.Join(",", filesystem));
        //Console.WriteLine("--------------");

        CompactPartOne(filesystem);

        //Console.WriteLine("Compacted state:");
        //Console.WriteLine(string.Join(",",filesystem));
        //Console.WriteLine("--------------");

        long checksum = CalculateChecksum(filesystem);
        
        Console.WriteLine($"Part One Checksum: {checksum}");
    }

    private static long CalculateChecksum(string[] filesystem)
    {
        long checksum = 0;
        for (long i = 0; i < filesystem.Length; i++)
        {
            if (filesystem[i] == ".")
            {
                break;
            }
            else
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
                if(filesystem[blockPos] != ".")
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

    private static string[] LoadData(string filename)
    {
        string line = File.ReadAllText(filename);

        List<string> output= new List<string>();
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
}
