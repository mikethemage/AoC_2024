namespace AoC_2024_13;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";

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


        totalPrizes = 0;
        totalCost = 0;
        foreach (Machine machine in machines)
        {
            machine.Prize = (machine.Prize.XLocation + 10000000000000, machine.Prize.YLocation + 10000000000000);

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

        Console.WriteLine($"Part Two Number of Prizes: {totalPrizes}, Total cost: {totalCost}");
    }

    private static long GetCheapestPrizeCost(Machine machine)
    {
        long px = machine.Prize.XLocation;
        long py = machine.Prize.YLocation;
        long ax = machine.Buttons.Where(c=>c.Name=="A").First().Movement.XOffset;
        long ay = machine.Buttons.Where(c => c.Name == "A").First().Movement.YOffset;
        long bx = machine.Buttons.Where(c => c.Name == "B").First().Movement.XOffset;
        long by = machine.Buttons.Where(c => c.Name == "B").First().Movement.YOffset;
        long n = ((px * by) - (py * bx)) / ((ax * by) - (ay * bx));
        long m = (py - n * ay) / by;

        long tx = n * ax + m * bx;
        long ty = n * ay + m * by;
        if(tx==px && ty==py)
        {
            return (n * machine.Buttons.Where(c => c.Name == "A").First().Cost) +
                (m * machine.Buttons.Where(c => c.Name == "B").First().Cost);
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