using System.Collections.Generic;

namespace AoC_2024_20;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        //const int improvementsOverOrEqualTo = 1;

        string filename = "input.txt";
        const int improvementsOverOrEqualTo = 100;

        const int maxCheats = 20;       
        const int improvementsOver = improvementsOverOrEqualTo - 1;

        string[] lines = File.ReadAllLines(filename);
        PuzzleData puzzleData = new PuzzleData(lines);

        var currentStep = new Step { Location = puzzleData.Start, StepsTaken = 0 };

        Dictionary<(int X, int Y), Step> history = new Dictionary<(int X, int Y), Step>();
        history.Add(puzzleData.Start, currentStep);

        while (currentStep.Location != puzzleData.End)
        {
            var nextLocation = (currentStep.Location.X + 1, currentStep.Location.Y);
            Step? nextStep = GetNextStep(puzzleData, currentStep, history, nextLocation);

            if (nextStep == null)
            {
                nextLocation = (currentStep.Location.X - 1, currentStep.Location.Y);
                nextStep = GetNextStep(puzzleData, currentStep, history, nextLocation);
            }

            if (nextStep == null)
            {
                nextLocation = (currentStep.Location.X, currentStep.Location.Y + 1);
                nextStep = GetNextStep(puzzleData, currentStep, history, nextLocation);
            }

            if (nextStep == null)
            {
                nextLocation = (currentStep.Location.X, currentStep.Location.Y - 1);
                nextStep = GetNextStep(puzzleData, currentStep, history, nextLocation);
            }

            if (nextStep == null)
            {
                throw new Exception("No next step from here!");
            }

            history.Add(nextStep.Location, nextStep);
            currentStep = nextStep;
        }

        Console.WriteLine($"Steps taken without cheating: {history[puzzleData.End].StepsTaken}");

        List<int> improvements = new List<int>();
        foreach (var location in history.Keys)
        {
            Dictionary<(int X, int Y), Step> cheets = new Dictionary<(int X, int Y), Step>();

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
        foreach (var item in improvements.GroupBy(x=>x).OrderBy(x=>x.Key))
        {
            Console.WriteLine($"There are {item.Count()} cheats that save {item.Key} picoseconds.");
            total += item.Count();
        }
        Console.WriteLine();
        Console.WriteLine($"Total: {total}");

    }

    private static Dictionary<(int RemainingSteps, (int X, int Y) Location), Dictionary<(int X, int Y), int>> Memo = new();

    private static Dictionary<(int X, int Y), int> GetPlaces(int remainingSteps, Dictionary<(int X, int Y), Step> history, (int X, int Y) location, PuzzleData puzzleData)
    {
        if(Memo.ContainsKey((remainingSteps, location)))
        {
            return Memo[(remainingSteps, location)];
        }

        Dictionary<(int X, int Y), int> placesICanGetTo = new Dictionary<(int X, int Y), int>();
        for (int xOffset = -remainingSteps; xOffset <= remainingSteps; xOffset++)
        {
            for (int yOffset = -remainingSteps; yOffset <= remainingSteps; yOffset++)
            {
                if ((Math.Abs(xOffset) >= 1 && yOffset == 0) ||
                    (Math.Abs(yOffset) >= 1) && xOffset == 0)
                {
                    (int X, int Y) cheatLocation = (location.X + xOffset, location.Y + yOffset);
                    if (history.ContainsKey(cheatLocation))
                    {
                        int newStepsTaken = Math.Abs(xOffset) + Math.Abs(yOffset);

                        if (placesICanGetTo.ContainsKey(cheatLocation))
                        {
                            if (placesICanGetTo[cheatLocation]>newStepsTaken)
                            {
                                placesICanGetTo[cheatLocation] = newStepsTaken;

                                if(cheatLocation!=puzzleData.End)
                                {
                                    foreach (var place in GetPlaces(remainingSteps - newStepsTaken, history, cheatLocation, puzzleData))
                                    {
                                        if (placesICanGetTo.ContainsKey(place.Key))
                                        {
                                            if (placesICanGetTo[place.Key] > newStepsTaken + place.Value)
                                            {
                                                placesICanGetTo[place.Key] = newStepsTaken + place.Value;
                                            }
                                        }
                                        else
                                        {
                                            placesICanGetTo.Add(place.Key, newStepsTaken + place.Value);
                                        }
                                    }
                                }                                
                            }
                        }
                        else
                        {
                            placesICanGetTo.Add(cheatLocation, newStepsTaken);

                            if (cheatLocation != puzzleData.End)
                            {
                                foreach (var place in GetPlaces(remainingSteps - newStepsTaken, history, cheatLocation, puzzleData))
                                {
                                    if (placesICanGetTo.ContainsKey(place.Key))
                                    {
                                        if (placesICanGetTo[place.Key] > newStepsTaken + place.Value)
                                        {
                                            placesICanGetTo[place.Key] = newStepsTaken + place.Value;
                                        }
                                    }
                                    else
                                    {
                                        placesICanGetTo.Add(place.Key, newStepsTaken + place.Value);
                                    }
                                }
                            }                                
                        }
                    }
                    else
                    {
                        if(!puzzleData.Walls.Contains(cheatLocation))
                        {
                            if(cheatLocation.X>=0 && cheatLocation.X<puzzleData.Width && cheatLocation.Y>=0 && cheatLocation.Y<puzzleData.Height)
                            {
                                throw new Exception("Invalid place!");
                            }                            
                        }
                    }
                }
            }
        }

        Memo.Add((remainingSteps, location),placesICanGetTo);
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
