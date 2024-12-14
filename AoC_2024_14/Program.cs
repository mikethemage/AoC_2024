
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

        List<Robot> robots = LoadData(filename);

        Console.WriteLine("Before:");
        OutputPositions(robots, width, height);
        Console.WriteLine();

        int numberOfSeconds = 100;
        foreach (var robot in robots)
        {
            robot.Move(numberOfSeconds, width, height);
        }

        Console.WriteLine("After:");
        OutputPositions(robots, width, height);

        var halfWidth=width/2;
        var halfHeight=height/2;
        var topLeft = robots.Where(r => r.Position.X < halfWidth && r.Position.Y < halfHeight).Count();
        var topRight = robots.Where(r => r.Position.X > halfWidth && r.Position.Y < halfHeight).Count();
        var bottomLeft = robots.Where(r => r.Position.X < halfWidth && r.Position.Y > halfHeight).Count();
        var bottomRight = robots.Where(r => r.Position.X > halfWidth && r.Position.Y > halfHeight).Count();
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