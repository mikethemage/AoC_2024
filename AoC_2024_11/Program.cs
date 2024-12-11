
namespace AoC_2024_11;

internal class Program
{
    static void Main(string[] args)
    {
        string filename = "sample.txt";
        //string filename = "input.txt";

        List<long> currentStones = LoadData(filename);

        const int numberOfBlinks = 25;

        for (int i = 0; i < numberOfBlinks; i++)
        {
            Console.WriteLine(i+1);
            //OutputCurrentState(currentStones, i);
            List<long> nextStones = new List<long>();
            foreach (var stone in currentStones)
            {
                nextStones.AddRange(GetNextStones(stone));
            }
            currentStones = nextStones;
        }

        //OutputCurrentState(currentStones, numberOfBlinks);

        Console.WriteLine($"Number of stones: {currentStones.Count}");

    }

    private static List<long> GetNextStones(long stone)
    {
        List<long> nextStones = new List<long>();
        string stoneText=stone.ToString();

        if (stone == 0)
        {
            nextStones.Add(1);
        }
        else if (stoneText.Length % 2 == 0)
        {
            string leftText = stoneText.Substring(0, stoneText.Length / 2);
            long left = long.Parse(leftText);
            string rightText = stoneText.Substring(stoneText.Length / 2).TrimStart('0');
            long right = string.IsNullOrEmpty(rightText) ? 0 : long.Parse(rightText);

            nextStones.Add(left);
            nextStones.Add(right);
        }
        else
        {
            nextStones.Add(stone*2024);
        }

        return nextStones;
    }

    private static void OutputCurrentState(List<long> currentStones, int numberOfBlinks)
    {
        Console.WriteLine($"After {numberOfBlinks} blink(s): {string.Join(" ", currentStones)}");
    }

    private static List<long> LoadData(string filename)
    {
        var line = File.ReadAllText(filename);
        line = line.Replace("\n", " ");
        var tokens = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        List<long> initialStones = tokens.Select(long.Parse).ToList();
        return initialStones;
    }
}
