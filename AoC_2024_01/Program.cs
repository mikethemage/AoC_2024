namespace AoC_2024_01;

internal class Program
{
    static void Main(string[] args)
    {
        string filename = "sample.txt";
        //string filename = "input.txt";
        List<int> firstIds, secondIds;
        InitializeData(filename, out firstIds, out secondIds);

        PartOne(firstIds, secondIds);
        PartTwo(firstIds, secondIds);
    }

    private static void InitializeData(string filename, out List<int> firstIds, out List<int> secondIds)
    {
        string[] lines = File.ReadAllLines(filename);
        firstIds = new List<int>();
        secondIds = new List<int>();
        foreach (string line in lines)
        {
            var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            firstIds.Add(int.Parse(tokens[0]));
            secondIds.Add(int.Parse(tokens[1]));
        }
    }

    private static void PartOne(List<int> firstIds, List<int> secondIds)
    {
        firstIds.Sort();
        secondIds.Sort();

        List<int> idDifferences = new List<int>();

        for (int i = 0; i < firstIds.Count; i++)
        {
            idDifferences.Add(Math.Abs(firstIds[i] - secondIds[i]));
        }

        Console.WriteLine($"Part One Answer: {idDifferences.Sum()}");
    }

    private static void PartTwo(List<int> firstIds, List<int> secondIds)
    {
        Dictionary<int, int> secondIdFrequencies = new Dictionary<int, int>();
        foreach (var numberOfTimes in secondIds.GroupBy(x=>x))
        {
            secondIdFrequencies.Add(numberOfTimes.Key, numberOfTimes.Count());
        }

        int total = 0;
        foreach (int id in firstIds)
        {
            if(secondIdFrequencies.ContainsKey(id))
            {
                total += id * secondIdFrequencies[id];                 
            }            
        }

        Console.WriteLine($"Part Two Answer: {total}");
    }
}
