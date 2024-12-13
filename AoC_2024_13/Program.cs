namespace AoC_2024_13;

internal class Program
{
    static void Main(string[] args)
    {
        string filename = "sample.txt";
        // string filename = "input.txt";

        List<Machine> machines = LoadData(filename);

        int totalPrizes = 0;
        long totalCost = 0;
        foreach (Machine machine in machines)
        {
            long cheapestPrize = GetCheapestPrizeCost(machine);
            if (cheapestPrize > 0)
            {
                Console.WriteLine($"Cheapest prize cost: {cheapestPrize}");
                totalCost += cheapestPrize;
                totalPrizes++;
            }
            else
            {
                Console.WriteLine("Cannot win a prize!");
            }
        }

        Console.WriteLine($"Part One Number of Prizes: {totalPrizes}, Total cost: {totalCost}");


        //totalPrizes = 0;
        //totalCost = 0;
        //foreach (Machine machine in machines)
        //{
        //    machine.Prize = (machine.Prize.XLocation + 10000000000000, machine.Prize.YLocation + 10000000000000);

        //    long cheapestPrize = GetCheapestPrizeCost(machine);
        //    if (cheapestPrize > 0)
        //    {
        //        Console.WriteLine($"Cheapest prize cost: {cheapestPrize}");
        //        totalCost += cheapestPrize;
        //        totalPrizes++;
        //    }
        //    else
        //    {
        //        Console.WriteLine("Cannot win a prize!");
        //    }
        //}

        //Console.WriteLine($"Part Two Number of Prizes: {totalPrizes}, Total cost: {totalCost}");
    }

    private static long GetCheapestPrizeCost(Machine machine)
    {
        State initialState = new State();
        foreach (var button in machine.Buttons)
        {
            initialState.TimesButtonsPressed.Add(button.Name, 0);
        }

        PriorityQueue<State, long> priorityQueue = new PriorityQueue<State, long>();
        priorityQueue.Enqueue(initialState, 0);

        HashSet<string> history = new HashSet<string>();
            

        while (priorityQueue.Count > 0)
        {
            //Console.WriteLine($"State count: {priorityQueue.Count}");
            State state = priorityQueue.Dequeue();

            string buttonPushes = state.GetEncodedButtonPushes();
            if(history.Contains(buttonPushes))
            {
                continue;
            }
            history.Add(buttonPushes);

            //Console.WriteLine($"Goal is: {machine.Prize.XLocation}, {machine.Prize.YLocation}.  Current state is: {state.CurrentLocation.X}, {state.CurrentLocation.Y}");

            if (state.CurrentLocation==machine.Prize)
            {
                return state.CurrentCost;
            }

            if(state.CurrentLocation.X > machine.Prize.XLocation
                ||
                state.CurrentLocation.Y > machine.Prize.YLocation)
            {
                continue;
            }

            foreach (var button in machine.Buttons)
            {
                if (state.TimesButtonsPressed[button.Name]<100)
                {
                    State newState = new State();
                    foreach (var key in state.TimesButtonsPressed.Keys)
                    {
                        newState.TimesButtonsPressed[key] = state.TimesButtonsPressed[key];
                    }
                    newState.TimesButtonsPressed[button.Name]++;
                    newState.CurrentCost=state.CurrentCost + button.Cost;
                    newState.CurrentLocation = (state.CurrentLocation.X+button.Movement.XOffset,
                                                state.CurrentLocation.Y+button.Movement.YOffset);
                    priorityQueue.Enqueue(newState, newState.CurrentCost);
                }
            }
        }        

        return -1;
    }

    private static List<Machine> LoadData(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        List<Machine> machines = new List<Machine>();
        List<Button> buttonsTemp = new List<Button>();

        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                buttonsTemp = new List<Button>();
            }
            else
            {
                if (line.Substring(0, 6) == "Button")
                {
                    var button = line.Substring(7, 1);
                    int cost = 0;
                    switch (button)
                    {
                        case "A":
                            cost = 3;
                            break;
                        case "B":
                            cost = 1;
                            break;
                        default:
                            throw new Exception("Invalid button!");
                    }

                    var tokens = line.Substring(10).Split(", ");
                    int x = int.Parse(tokens[0].Replace("X", "").Replace("+", ""));
                    int y = int.Parse(tokens[1].Replace("Y", "").Replace("+", ""));

                    Button buttonTemp = new Button { Name = button, Cost = cost, Movement = (x, y) };
                    buttonsTemp.Add(buttonTemp);
                }
                else
                {
                    var tokens = line.Replace("Prize: ", "").Split(", ");
                    int x = int.Parse(tokens[0].Replace("X=", ""));
                    int y = int.Parse(tokens[1].Replace("Y=", ""));
                    Machine machineTemp = new Machine { Prize = (x, y), Buttons = buttonsTemp };
                    machines.Add(machineTemp);
                }
            }
        }
        return machines;
    }
}

internal class State
{
    public Dictionary<string, int> TimesButtonsPressed { get; private set; } = new Dictionary<string, int>();
    public long CurrentCost { get; set; } = 0;
    public (long X, long Y) CurrentLocation { get; set; } = (0, 0);

    public string GetEncodedButtonPushes()
    {
        return string.Join(",", TimesButtonsPressed.OrderBy(x => x.Key).Select(x => $"{x.Key}:{x.Value}"));
    }
}


internal class Machine
{
    public (long XLocation, long YLocation) Prize { get; set; }

    public List<Button> Buttons { get; set; } = new List<Button>();
}

internal class Button
{
    public string Name { get; set; } = string.Empty;
    public (int XOffset, int YOffset) Movement { get; set; }
    public int Cost { get; set; }
}