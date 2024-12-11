
namespace AoC_2024_11;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";

        List<long> initialStones = LoadData(filename);
        Stack<(long Value, int BlinkCount)> stones = new Stack<(long Value, int BlinkCount)>();
        for (int i = initialStones.Count - 1; i >= 0; i--)
        {
            stones.Push(( initialStones[i], 0 ));
        }

        const int numberOfBlinks = 25;
        int totalFinalStones = 0;

        while (stones.Count>0)
        {
            (long Value, int BlinkCount) currentStone = stones.Pop();
            totalFinalStones += GetFinalScore(currentStone, numberOfBlinks);
        }

        Console.WriteLine($"Final number of stones: {totalFinalStones}");

    }

    private static int GetFinalScore((long Value, int BlinkCount) currentStone, int numberOfBlinks)
    {
        if (currentStone.BlinkCount == numberOfBlinks)
        {
            return 1;
        }

        List<long> nextStones = GetNextStones(currentStone.Value);
        int total = 0;
        foreach (long stone in nextStones)
        {
            total += GetFinalScore((stone, currentStone.BlinkCount+1), numberOfBlinks);
        }

        return total;
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

