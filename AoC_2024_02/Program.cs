namespace AoC_2024_02;

internal class Program
{
    static void Main(string[] args)
    {
        string filename = "sample.txt";
        //string filename = "input.txt";

        var reports = InitializeData(filename);

        int partOneSafeReports = 0;
        int partTwoSafeReports = 0;

        foreach (var report in reports)
        {
            List<int> diffs = new List<int>();
            for (int i = 1; i < report.Count; i++)
            {
                diffs.Add(report[i] - report[i-1]);
            }            

            if (diffs.All(x => x >= 1 && x <= 3) ||
                diffs.All(x => x <= -1 && x >= -3))
            {
                partOneSafeReports++;
                partTwoSafeReports++;
            }
            else
            {
                var increasingAnomolies = diffs.Count(x => x < 1 || x > 3);
                if(increasingAnomolies>0 && increasingAnomolies <= 2)
                {
                    int diffIndex = 0;
                    for (int i = 0; i < diffs.Count; i++)
                    {
                        if (diffs[i]<1 || diffs[i]>3)
                        {
                            diffIndex = i; 
                            break;
                        }
                    }
                    var tempList = new List<int>(report);
                    tempList.RemoveAt(diffIndex);
                    diffs = new List<int>();
                    for (int i = 1; i < tempList.Count; i++)
                    {
                        diffs.Add(tempList[i] - tempList[i - 1]);
                    }
                    if (diffs.All(x => x >= 1 && x <= 3))
                    {
                        partTwoSafeReports++;
                    }
                    else
                    {
                        tempList = new List<int>(report);
                        tempList.RemoveAt(diffIndex + 1);
                        diffs = new List<int>();
                        for (int i = 1; i < tempList.Count; i++)
                        {
                            diffs.Add(tempList[i] - tempList[i - 1]);
                        }
                        if (diffs.All(x => x >= 1 && x <= 3))
                        {
                            partTwoSafeReports++;
                        }
                    }
                    
                }

                var decreasingAnomolies = diffs.Count(x => x > -1 || x < -3);
                if (decreasingAnomolies > 0 && decreasingAnomolies <= 2)
                {
                    int diffIndex = 0;
                    for (int i = 0; i < diffs.Count; i++)
                    {
                        if (diffs[i] > -1 || diffs[i] < -3)
                        {
                            diffIndex = i;
                            break;
                        }
                    }
                    var tempList = new List<int>(report);
                    tempList.RemoveAt(diffIndex);
                    diffs = new List<int>();
                    for (int i = 1; i < tempList.Count; i++)
                    {
                        diffs.Add(tempList[i] - tempList[i - 1]);
                    }
                    if (diffs.All(x => x <= -1 && x >= -3))
                    {
                        partTwoSafeReports++;
                    }
                    else
                    {
                        tempList = new List<int>(report);
                        tempList.RemoveAt(diffIndex + 1);
                        diffs = new List<int>();
                        for (int i = 1; i < tempList.Count; i++)
                        {
                            diffs.Add(tempList[i] - tempList[i - 1]);
                        }
                        if (diffs.All(x => x <= -1 && x >= -3))
                        {
                            partTwoSafeReports++;
                        }
                    }
                    
                }
            }
            


        }

        Console.WriteLine($"Part One: Number of safe reports: {partOneSafeReports}");
        Console.WriteLine($"Part Two: Number of safe reports: {partTwoSafeReports}");



    }

    private static List<List<int>> InitializeData(string filename)
    {
        List<List<int>> reports = new List<List<int>>();

        var lines = File.ReadAllLines(filename);
        foreach (var line in lines)
        {
            var tokens = line.Split(' ');
            reports.Add(tokens.Select(x => int.Parse(x)).ToList());
        }
        return reports;
    }
}
