namespace AoC_2024_19;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";
        
        string[] lines = File.ReadAllLines(filename);
        bool designMode = false;
        List<string> availablePatterns = new List<string>();
        List<string> designs = new List<string>();
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                designMode = true;
            }
            else if (designMode)
            {
                designs.Add(line);
            }
            else
            {
                availablePatterns = line.Split(", ").ToList();
            }
        }

        int validCount = 0;
        long differentWays = 0;
        foreach (var design in designs)
        {
            List<(string, long)> possibleDesigns = new List<(string, long)> { ("",1) };
            long countToAdd = 0;
            while (possibleDesigns.Count > 0 && !possibleDesigns.Any(x=>x.Item1==design))
            {
                var newPossibleDesigns = new List<(string, long)>();
                foreach (var pattern in availablePatterns)
                {
                    newPossibleDesigns.AddRange(
                     possibleDesigns.Select(x => (x.Item1 + pattern, x.Item2))
                        .Where(x => design.StartsWith(x.Item1)));
                }
                possibleDesigns = newPossibleDesigns.GroupBy(x => x.Item1).Select(x=>(x.Key,x.Sum(y=>y.Item2))).ToList();

                countToAdd += possibleDesigns.Where(x => x.Item1 == design).Sum(y => y.Item2);

                possibleDesigns = possibleDesigns.Where(x=>x.Item1!=design && x.Item1.Length<design.Length).ToList();
            }
            if(countToAdd>0)
            {
                validCount++;
                differentWays += countToAdd;
            }
        }

        Console.WriteLine($"Part one total: {validCount}");

        Console.WriteLine($"Part two total: {differentWays}");
    }
}
