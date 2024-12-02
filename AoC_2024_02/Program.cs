namespace AoC_2024_02;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";

        var reports = InitializeData(filename);

        int safeReports = 0;
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
                safeReports++;
                //Console.WriteLine("Safe");
            }
            //else
            //{
            //    Console.WriteLine("Unsafe");
            //}
        }

        Console.WriteLine($"Number of safe reports: {safeReports}");

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
