namespace AoC_2024_20;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";
        string[] lines = File.ReadAllLines(filename);
        PuzzleData puzzleData = new PuzzleData(lines);
        const int maxCheats = 20;

        List<State> endStateNoCheating = RunSimulation(puzzleData, new State { Location = puzzleData.Start, History = new() }, -1, false, maxCheats);

        if (endStateNoCheating.Count>0)
        {
            Console.WriteLine($"Shortest path without cheating: {endStateNoCheating[0].StepsTaken}");


            Dictionary<((int X, int Y) Start, (int X, int Y) End), int> cheats = new();

            foreach( State cheatStartState in endStateNoCheating[0].History.Values.Where(x=>x.Location!=puzzleData.End) )
            {
                List<State> intermediateStates = cheatStartState.GetNextStates(puzzleData,true,maxCheats).Where(x=>puzzleData.Walls.Contains(x.Location)).ToList();
                List<State> cheatEndStates = new List<State>();

                for (int i = 0; i < maxCheats-1; i++)
                {
                    List<State> nextIntermediateStates = new List<State>();
                    foreach (var intermediateState in intermediateStates)
                    {                        
                        var temp = intermediateState.GetNextStates(puzzleData, true, maxCheats);
                        cheatEndStates.AddRange(temp.Where(x => !puzzleData.Walls.Contains(x.Location)));
                        nextIntermediateStates.AddRange(temp.Where(x => puzzleData.Walls.Contains(x.Location)));
                    }
                    intermediateStates = nextIntermediateStates;
                }
                

                foreach (var cheatEndState in cheatEndStates)
                {
                    if(endStateNoCheating[0].Location==cheatEndState.Location)
                    {
                        var improvement = endStateNoCheating[0].StepsTaken - cheatEndState.StepsTaken;
                        if (improvement > 0)
                        {
                            if (cheats.ContainsKey((cheatStartState.Location, cheatEndState.Location)))
                            {
                                if (improvement > cheats[(cheatStartState.Location, cheatEndState.Location)])
                                {
                                    cheats[(cheatStartState.Location, cheatEndState.Location)] = improvement;
                                }
                            }
                            else
                            {
                                cheats.Add((cheatStartState.Location, cheatEndState.Location), improvement);
                            }
                        }
                    }

                    if(endStateNoCheating[0].History.ContainsKey(cheatEndState.Location))
                    {
                        var skipToState = endStateNoCheating[0].History[cheatEndState.Location];
                        var improvement = skipToState.StepsTaken - cheatEndState.StepsTaken;

                        if (improvement > 0)
                        {
                            if (cheats.ContainsKey((cheatStartState.Location, cheatEndState.Location)))
                            {
                                if(improvement > cheats[(cheatStartState.Location, cheatEndState.Location)])
                                {
                                    cheats[(cheatStartState.Location, cheatEndState.Location)] = improvement;
                                }                                
                            }
                            else
                            {
                                cheats.Add((cheatStartState.Location, cheatEndState.Location), improvement);
                            }
                        }
                    }
                }
            }

            //Console.WriteLine();

            Dictionary<int,int> countsByImprovment = new Dictionary<int,int>();
            foreach (var key in cheats.Keys)
            {
                //Console.WriteLine($"{key}: {cheats[key]}");
                if(countsByImprovment.ContainsKey(cheats[key]))
                {
                    countsByImprovment[cheats[key]]++;
                }
                else
                {
                    countsByImprovment.Add(cheats[key], 1);
                }
            }

            const int cutoff = 50;
            Console.WriteLine();
            int total = 0;
            foreach (var improvement in countsByImprovment.Keys.Where(x=>x>=cutoff).OrderBy(x=>x))
            {
                Console.WriteLine($"Improvement of {improvement}: Count={countsByImprovment[improvement]}");
                total += countsByImprovment[improvement];
            }

            Console.WriteLine();
            Console.WriteLine($"Total: {total}");

            //int total = 0;
            //List<State> endStateQuickestCheats = RunSimulation(puzzleData, new State { Location = puzzleData.Start, History = new() }, endStateNoCheating[0].StepsTaken, true);
            //foreach (var endStateQuickestCheat in endStateQuickestCheats
            //    .Where(x => x.CheatsUsed > 0 && x.CheatStartLocation != null && x.CheatEndLocation != null)
            //    .GroupBy(x => (x.CheatStartLocation, x.CheatEndLocation))
            //    .GroupBy(x=>x.Min(y=>y.StepsTaken))
            //    .OrderByDescending(x=>x.Key))
            //{

            //    //Console.WriteLine($"{endStateQuickestCheat.Key.CheatStartLocation?.X},{endStateQuickestCheat.Key.CheatStartLocation?.Y} to {endStateQuickestCheat.Key.CheatEndLocation?.X},{endStateQuickestCheat.Key.CheatEndLocation?.Y}: {endStateQuickestCheat.Min(x=>x.StepsTaken)}");
            //    Console.WriteLine($"{endStateNoCheating[0].StepsTaken-endStateQuickestCheat.Key}: {endStateQuickestCheat.Count()}");

            //    //Console.WriteLine($"Paths with cheating: {endStateNoCheating[0].StepsTaken-endStateQuickestCheat.Key} steps shorter: {endStateQuickestCheat.Count()}");
                
            //    if(endStateNoCheating[0].StepsTaken - endStateQuickestCheat.Key>=100)
            //    {
            //        total +=
            //            endStateQuickestCheat.Count();
            //    }
            //}

            //Console.WriteLine($"Total saving at least 100: {total}");

        }
    }

    private static List<State> RunSimulation(PuzzleData puzzleData, State startState, int maxSteps, bool cheatingAllowed, int maxCheats)
    {
        List<State> output = new List<State>();
        PriorityQueue<State, int> priorityQueue = new PriorityQueue<State, int>();
        priorityQueue.Enqueue(startState, startState.StepsTaken);

        while (priorityQueue.Count > 0)
        {
            //Console.WriteLine($"Queue size: {priorityQueue.Count}");
            
            State state = priorityQueue.Dequeue();

            //Console.WriteLine($"Head of queue steps taken: {state.StepsTaken}");
            //Console.WriteLine($"Head of queue Location: {state.Location.X},{state.Location.Y}, cheat count: {state.CheatsUsed}");
            //Console.ReadKey();

            if (state.CheatsUsed==2 && (
                state.Location.X<0 ||state.Location.Y<0 || state.Location.X > puzzleData.Walls.Max(w=>w.X) || state.Location.Y > puzzleData.Walls.Max(w=>w.Y)
                ))
            {
                continue;
            }            

            if (maxSteps >= 0 && state.StepsTaken >= maxSteps)
            {
                continue;
            }

            if (state.Location == puzzleData.End)
            {
                

                output.Add(state);

                if (maxSteps<0)
                {
                    break;
                }
                else
                {
                    continue;
                }                   
            }

            foreach (State nextState in state.GetNextStates(puzzleData, cheatingAllowed, maxCheats))
            {
                priorityQueue.Enqueue(nextState, nextState.StepsTaken);
            }
        }

        return output;
    }
}

internal class State
{
    public required (int X, int Y) Location { get; set; }

    public (int X, int Y)? CheatStartLocation { get; set; } = null;
    public (int X, int Y)? CheatEndLocation { get; set; } = null;

    public int StepsTaken
    {
        get
        {
            return History.Count;
        }
    }
    public int CheatsUsed { get; set; } = 0;

    public required Dictionary<(int X, int Y), State> History { get; init; }

    public List<State> GetNextStates(PuzzleData puzzleData, bool cheatingAllowed, int maxCheats)
    {
        List<State> output = new List<State>();

        AddNextState(puzzleData, output, (Location.X + 1, Location.Y), cheatingAllowed, maxCheats);
        AddNextState(puzzleData, output, (Location.X - 1, Location.Y), cheatingAllowed, maxCheats);
        AddNextState(puzzleData, output, (Location.X, Location.Y + 1), cheatingAllowed, maxCheats);
        AddNextState(puzzleData, output, (Location.X, Location.Y - 1), cheatingAllowed, maxCheats);

        return output;
    }

    private void AddNextState(PuzzleData puzzleData, List<State> output, (int X, int Y) nextLocation, bool cheatingAllowed, int maxCheats)
    {
        if (History.ContainsKey(nextLocation))
        {
            return;
        }

        var newHistory = new Dictionary<(int X, int Y), State>(History);
        newHistory.Add(Location, this);
        if (puzzleData.Walls.Contains(nextLocation))
        {
            if (cheatingAllowed && CheatsUsed < maxCheats - 1)
            {
                output.Add(new State { Location = nextLocation, History = newHistory, CheatsUsed = CheatsUsed + 1, CheatStartLocation=CheatStartLocation  ?? Location, CheatEndLocation=null });
            }
        }
        else
        {
            int newCheatsUsed = CheatsUsed;            
            if (CheatsUsed != 0)
            {
                newCheatsUsed = CheatsUsed == maxCheats ?  CheatsUsed : CheatsUsed + 1;
            }
            
            var newCheatEndLocation = CheatEndLocation;
            if(newCheatEndLocation==null && CheatStartLocation!=null)
            {
                newCheatEndLocation= nextLocation;
            }

            output.Add(new State { Location = nextLocation, History = newHistory, CheatsUsed = newCheatsUsed, CheatStartLocation = CheatStartLocation, CheatEndLocation = newCheatEndLocation });
        }
    }
}

internal class PuzzleData
{
    public (int X, int Y) Start { get; private set; }
    public (int X, int Y) End { get; private set; }
    public HashSet<(int X, int Y)> Walls { get; private set; } = new HashSet<(int X, int Y)>();
    public PuzzleData(string[] lines)
    {
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
