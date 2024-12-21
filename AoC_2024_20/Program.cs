namespace AoC_2024_20;

internal class Program
{
    private readonly static List<(int X, int Y)> _directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        //const int improvementsOverOrEqualTo = 50;

        string filename = "input.txt";
        const int improvementsOverOrEqualTo = 100;

        const int maxCheatsPart1 = 2;
        const int maxCheatsPart2 = 20;
        const int improvementsOver = improvementsOverOrEqualTo - 1;

        string[] lines = File.ReadAllLines(filename);
        PuzzleData puzzleData = new PuzzleData(lines);

        var currentStep = new Step { Location = puzzleData.Start, StepsTaken = 0 };

        Dictionary<(int X, int Y), Step> history = new Dictionary<(int X, int Y), Step>();
        history.Add(puzzleData.Start, currentStep);

        while (currentStep.Location != puzzleData.End)
        {
            foreach (var direction in _directions)
            {
                var nextLocation = (currentStep.Location.X + direction.X, currentStep.Location.Y + direction.Y);
                Step? nextStep = GetNextStep(puzzleData, currentStep, history, nextLocation);
                if (nextStep != null)
                {
                    history.Add(nextStep.Location, nextStep);
                    currentStep = nextStep;
                    break;
                }
            }
        }

        Console.WriteLine($"Steps taken without cheating: {history[puzzleData.End].StepsTaken}");

        Console.WriteLine("Part 1:");
        FindCheats(maxCheatsPart1, improvementsOver, puzzleData, history);

        Console.WriteLine("");
        Console.WriteLine("Part 2:");
        FindCheats(maxCheatsPart2, improvementsOver, puzzleData, history);
    }

    private static void FindCheats(int maxCheats, int improvementsOver, PuzzleData puzzleData, Dictionary<(int X, int Y), Step> history)
    {
        List<int> improvements = new List<int>();
        foreach (var location in history.Keys)
        {
            Dictionary<(int X, int Y), Step> cheats = new Dictionary<(int X, int Y), Step>();

            Dictionary<(int X, int Y), int> placesICanGetTo = GetPlaces(maxCheats, history, location, puzzleData);
            foreach (var place in placesICanGetTo)
            {
                int improvement = history[place.Key].StepsTaken - (history[location].StepsTaken + place.Value);
                if (improvement > improvementsOver)
                {
                    improvements.Add(improvement);
                }
            }
        }

        int total = 0;
        foreach (var item in improvements.GroupBy(x => x).OrderBy(x => x.Key))
        {
            Console.WriteLine($"There are {item.Count()} cheats that save {item.Key} picoseconds.");
            total += item.Count();
        }
        Console.WriteLine();
        Console.WriteLine($"Total: {total}");
    }

    private static Dictionary<(int X, int Y), int> GetPlaces(int remainingSteps, Dictionary<(int X, int Y), Step> history, (int X, int Y) location, PuzzleData puzzleData)
    {
        Dictionary<(int X, int Y), int> placesICanGetTo = new Dictionary<(int X, int Y), int>();
        for (int xOffset = -remainingSteps; xOffset <= remainingSteps; xOffset++)
        {
            for (int yOffset = -remainingSteps; yOffset <= remainingSteps; yOffset++)
            {
                if ((Math.Abs(xOffset) > 1 ||
                    Math.Abs(yOffset) > 1)
                    && Math.Abs(xOffset) + Math.Abs(yOffset) <= remainingSteps
                    )
                {
                    (int X, int Y) cheatLocation = (location.X + xOffset, location.Y + yOffset);
                    if (history.ContainsKey(cheatLocation))
                    {
                        int newStepsTaken = Math.Abs(xOffset) + Math.Abs(yOffset);

                        placesICanGetTo.Add(cheatLocation, newStepsTaken);
                        
                    }
                    else
                    {
                        if (!puzzleData.Walls.Contains(cheatLocation))
                        {
                            if (cheatLocation.X >= 0 && cheatLocation.X < puzzleData.Width && cheatLocation.Y >= 0 && cheatLocation.Y < puzzleData.Height)
                            {
                                throw new Exception("Invalid place!");
                            }
                        }
                    }
                }
            }
        }

        return placesICanGetTo;
    }

    private static Step? GetNextStep(PuzzleData puzzleData, Step currentStep, Dictionary<(int X, int Y), Step> history, (int, int Y) nextLocation)
    {
        Step? nextStep = null;
        if (!history.ContainsKey(nextLocation) && !puzzleData.Walls.Contains(nextLocation))
        {
            nextStep = new Step { Location = nextLocation, StepsTaken = currentStep.StepsTaken + 1 };
        }

        return nextStep;
    }
}

internal class Step
{
    public required (int X, int Y) Location { get; set; }

    public required int StepsTaken { get; set; }
}

internal class PuzzleData
{
    public int Width { get; private set; } = 0;
    public int Height { get; private set; } = 0;
    public (int X, int Y) Start { get; private set; }
    public (int X, int Y) End { get; private set; }
    public HashSet<(int X, int Y)> Walls { get; private set; } = new HashSet<(int X, int Y)>();
    public PuzzleData(string[] lines)
    {
        Height = lines.Length;
        Width = lines[0].Length;
        for (int row = 0; row < lines.Length; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                switch (lines[row][col])
                {
                    case '#':
                        Walls.Add((col, row));
                        break;
                    case 'S':
                        Start = (col, row);
                        break;
                    case 'E':
                        End = (col, row);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
