
namespace AoC_2024_11;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";

        List<long> initialStones = LoadData(filename);    

        const int numberOfBlinks = 75;
        long totalFinalStones = 0;

        foreach (var stone in initialStones)        
        {
            totalFinalStones += GetFinalScore((stone, 0), numberOfBlinks);
        }

        Console.WriteLine($"Final number of stones: {totalFinalStones}");
    }

    private static Dictionary<(long Value, int BlinkCount), long> _totalStoneHistory = new Dictionary<(long Value, int BlinkCount), long> ();

    private static long GetFinalScore((long Value, int BlinkCount) currentStone, int numberOfBlinks)
    {
        if(_totalStoneHistory.ContainsKey(currentStone))
        {
            return _totalStoneHistory[currentStone];
        }

        if (currentStone.BlinkCount == numberOfBlinks)
        {
            _totalStoneHistory.Add(currentStone, 1 );
            return 1;
        }

        List<long> nextStones = GetNextStones(currentStone.Value);
        long total = 0;
        foreach (long stone in nextStones)
        {
            total += GetFinalScore((stone, currentStone.BlinkCount+1), numberOfBlinks);
        }

        _totalStoneHistory.Add(currentStone, total);
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

    private static List<long> LoadData(string filename)
    {
        var line = File.ReadAllText(filename);
        line = line.Replace("\n", " ");
        var tokens = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        List<long> initialStones = tokens.Select(long.Parse).ToList();
        return initialStones;
    }
}

