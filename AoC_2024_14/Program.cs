

namespace AoC_2024_14;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        //int width = 11;
        //int height = 7;

        string filename = "input.txt";
        int width = 101;
        int height = 103;

        List<Robot> part1Robots = LoadData(filename);
        List<Robot> part2Robots = part1Robots.Select(x => new Robot(x)).ToList();
        SolvePartOne(width, height, part1Robots);

        Console.ReadKey();

        SolvePartTwo(width, height, part2Robots);
    }

    private static void SolvePartTwo(int width, int height, List<Robot> part2Robots)
    {
        Console.Clear();
        Console.WriteLine("\x1b[3J");
        Console.WriteLine("Initial State:");
        OutputPositions(part2Robots, width, height);
        int seconds = 0;
        while(true)
        {
            
            seconds++;
            
            foreach (var robot in part2Robots)
            {
                robot.Move(1, width, height);
            }

            var halfWidth = width / 2;
            var halfHeight = height / 2;
            var topLeft = part2Robots.Where(r => r.Position.X < halfWidth && r.Position.Y < halfHeight).Count();
            var topRight = part2Robots.Where(r => r.Position.X > halfWidth && r.Position.Y < halfHeight).Count();
            var bottomLeft = part2Robots.Where(r => r.Position.X < halfWidth && r.Position.Y > halfHeight).Count();
            var bottomRight = part2Robots.Where(r => r.Position.X > halfWidth && r.Position.Y > halfHeight).Count();
            var totalRobots = part2Robots.Count();

            if(topLeft > totalRobots/2
                || topRight > totalRobots/2
                || bottomLeft > totalRobots/2
                || bottomRight > totalRobots/2
                )
            {
                Console.Clear();
                Console.WriteLine("\x1b[3J");
                Console.WriteLine($"After {seconds} seconds:");
                OutputPositions(part2Robots, width, height);
                Console.ReadKey();
                break;
            }

            
        }
    }

    private static void SolvePartOne(int width, int height, List<Robot> part1Robots)
    {
        Console.WriteLine("Before:");
        OutputPositions(part1Robots, width, height);
        Console.WriteLine();

        int numberOfSeconds = 100;
        foreach (var robot in part1Robots)
        {
            robot.Move(numberOfSeconds, width, height);
        }

        Console.WriteLine("After:");
        OutputPositions(part1Robots, width, height);

        var halfWidth = width / 2;
        var halfHeight = height / 2;
        var topLeft = part1Robots.Where(r => r.Position.X < halfWidth && r.Position.Y < halfHeight).Count();
        var topRight = part1Robots.Where(r => r.Position.X > halfWidth && r.Position.Y < halfHeight).Count();
        var bottomLeft = part1Robots.Where(r => r.Position.X < halfWidth && r.Position.Y > halfHeight).Count();
        var bottomRight = part1Robots.Where(r => r.Position.X > halfWidth && r.Position.Y > halfHeight).Count();
        var safteyFactor = topLeft * topRight * bottomLeft * bottomRight;
        Console.WriteLine($"Top Left: {topLeft}, Top Right: {topRight}, Bottom Left: {bottomLeft}, Bottom Right: {bottomRight}, Safety Factor: {safteyFactor}");
    }

    private static void OutputPositions(List<Robot> robots, int width, int height)
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                var numRobots = robots.Where(r => r.Position == (col, row)).Count();
                if (numRobots > 0)
                {
                    Console.Write(numRobots);
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
    }

    private static List<Robot> LoadData(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        List<Robot> robots = new List<Robot>();
        foreach (string line in lines)
        {
            int[][] tokens = line.Split(' ')
                .Select(pairs => pairs.Split('=')[1]
                                .Split(',')
                                .Select(dimension => int.Parse(dimension))
                                .ToArray()
                                )
                .ToArray();
            int[] position = tokens[0];
            int[] velocity = tokens[1];
            Robot robot = new Robot(position, velocity);
            robots.Add(robot);
        }
        return robots;
    }
}

internal class Robot
{
    public (int X, int Y) Position { get; set; }
    public (int X, int Y) Velocity { get; set; }
    public Robot(int[] position, int[] velocity)
    {
        Position = (position[0], position[1]);
        Velocity = (velocity[0], velocity[1]);
    }
    public Robot(Robot copyFrom)
    {
        Position=copyFrom.Position;
        Velocity=copyFrom.Velocity;
    }

    public void Move(int seconds, int width, int height)
    {
        int newX = Position.X + (seconds * Velocity.X);
        while (newX < 0)
        {
            newX += width;
        }
        newX %= width;

        int newY= Position.Y + (seconds * Velocity.Y);
        while (newY < 0)
        {
            newY += height;
        }
        newY %= height;

        Position = (newX, newY);
    }
}