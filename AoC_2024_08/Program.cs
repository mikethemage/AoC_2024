

namespace AoC_2024_08;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";
        var lines = File.ReadAllLines(filename);

        Dictionary<char, List<(int, int)>> antennae = new Dictionary<char, List<(int, int)>>();

        int height = lines.Length;
        int width = lines[0].Length;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                char antennaFrequency = lines[row][col];
                if (antennaFrequency != '.')
                {
                    //Console.WriteLine($"{antennaFrequency}: ({col},{row})");

                    if (antennae.ContainsKey(antennaFrequency))
                    {
                        antennae[antennaFrequency].Add((col, row));
                    }
                    else
                    {
                        antennae.Add(antennaFrequency, new List<(int, int)> { (col, row) });
                    }
                }
            }
        }

        HashSet<(int, int)> antinodesPartOne = GetAntinodes(antennae, height, width, 1);

        //OutputMapOfAntinodes(height, width, antinodesPartOne);

        Console.WriteLine($"Total number of antinodes for part 1: {antinodesPartOne.Count}");

        HashSet<(int, int)> antinodesPartTwo = GetAntinodes(antennae, height, width, 2);

        //OutputMapOfAntinodes(height, width, antinodesPartTwo);

        Console.WriteLine($"Total number of antinodes for part 2: {antinodesPartTwo.Count}");
    }

    private static HashSet<(int, int)> GetAntinodes(Dictionary<char, List<(int, int)>> antennae, int height, int width, int partNumber)
    {
        HashSet<(int, int)> antinodes = new HashSet<(int, int)>();
        foreach (char frequency in antennae.Keys)
        {
            List<(int, int)> currentSet = antennae[frequency];
            for (int i = 0; i < currentSet.Count - 1; i++)
            {
                (int, int) firstAntenna = currentSet[i];
                for (int j = i + 1; j < currentSet.Count; j++)
                {
                    (int, int) secondAntenna = currentSet[j];
                    List<(int, int)> newAntinodes = GetAntinodes(firstAntenna, secondAntenna, height, width, partNumber);
                    foreach (var item in newAntinodes)
                    {
                        if (item.Item1 >= 0 && item.Item1 < width && item.Item2 >= 0 && item.Item2 < height)
                        {
                            antinodes.Add(item);
                        }
                    }
                }
            }
        }

        return antinodes;
    }

    private static void OutputMapOfAntinodes(int height, int width, HashSet<(int, int)> antinodes)
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if(antinodes.Contains((col,row)))
                {
                    Console.Write('#');
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
    }

    private static List<(int, int)> GetAntinodes((int, int) firstAntenna, (int, int) secondAntenna, int height, int width, int partNumber)
    {
        if(partNumber<1 || partNumber > 2)
        {
            throw new Exception("Invalid Part Number!");
        }

        int xRate = secondAntenna.Item1 - firstAntenna.Item1;
        int yRate = secondAntenna.Item2 - firstAntenna.Item2;

        List<(int,int)> output = new List<(int, int)>();

        if (partNumber == 2)
        {
            output.Add(firstAntenna);
            output.Add(secondAntenna);
        }

        int timesApplied = 1;
        bool outsideBounds = false;
        do
        {
            var newX = secondAntenna.Item1 + (xRate * timesApplied);
            var newY = secondAntenna.Item2 + (yRate * timesApplied);
            if (newX < 0 || newX >= width || newY < 0 || newY >= height)
            {
                outsideBounds = true;
            }
            else
            {
                output.Add((newX, newY));
            }
            timesApplied++;
        } while (!outsideBounds && partNumber==2);

        timesApplied = 1;
        outsideBounds = false;
        do
        {
            var newX = firstAntenna.Item1 - (xRate * timesApplied);
            var newY = firstAntenna.Item2 - (yRate * timesApplied);
            if (newX < 0 || newX >= width || newY < 0 || newY >= height)
            {
                outsideBounds = true;
            }
            else
            {
                output.Add((newX, newY));
            }
            timesApplied++;
        } while (!outsideBounds && partNumber == 2);
        

        return output;
    }
}
