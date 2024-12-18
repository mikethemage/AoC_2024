
namespace AoC_2024_18;

internal class Program
{
    static void Main(string[] args)
    {
        const int sampleSize = 7;
        const int sampleBytes = 12;

        const int inputSize = 71;
        const int inputBytes = 1024;

        string filename = "sample.txt";
        int height = sampleSize;
        int numberBytes = sampleBytes;

        //string filename = "input.txt";
        //int height = inputSize;
        //int numberBytes = inputBytes;

        int width = height;
        string[] lines = File.ReadAllLines(filename);
        List<(int X, int Y)> fallingBytes = new List<(int X, int Y)>();
        foreach (string line in lines)
        {
            var tokens = line.Split(',').Select(int.Parse).ToArray();
            (int X, int Y) fallingByte = (tokens[0], tokens[1]);
            fallingBytes.Add(fallingByte);
        }

        (int X, int Y) start = (0, 0);
        (int X, int Y) goal = (width-1, height-1);


        HashSet<(int X, int Y)> fallenBytes = new HashSet<(int X, int Y)> ( fallingBytes.Take(numberBytes).ToList());        
        PriorityQueue<((int X, int Y) Position, int Steps), int>  priorityQueue = new PriorityQueue<((int X, int Y) Position, int Steps), int>();
        priorityQueue.Enqueue((start,0), width+height);
        Dictionary<(int X, int Y), int> visited = new Dictionary<(int X, int Y), int>();
        int shortestSteps = -1;
        while (priorityQueue.Count > 0)
        {
            ((int X, int Y) Position, int Steps) currentState = priorityQueue.Dequeue();

            if (visited.ContainsKey(currentState.Position))
            {
                if (currentState.Steps >= visited[currentState.Position])
                {
                    continue;
                }
                else
                {
                    visited[currentState.Position] = currentState.Steps;
                }
            }
            else
            {
                visited.Add(currentState.Position,currentState.Steps);
            }

            if (shortestSteps >= 0)
            {
                if (currentState.Steps > shortestSteps)
                {
                    continue;
                }
            }

            if (currentState.Position == goal)
            {
                shortestSteps = currentState.Steps;
                continue;
            }

            List<(int X, int Y)> nextPositions = GetNextPositions(currentState.Position, fallenBytes, width, height);
            foreach ((int X, int Y) nextPosition in nextPositions)
            {
                priorityQueue.Enqueue((nextPosition, currentState.Steps + 1), (width-nextPosition.X) + (height-nextPosition.Y));
            }
        }

        if (shortestSteps >= 0)
        {
            Console.WriteLine($"Shortest steps: {shortestSteps}");
        }
        else
        {
            Console.WriteLine("Path not found!");
        }
    }

    private static List<(int X, int Y)> GetNextPositions((int X, int Y) position, HashSet<(int X, int Y)> fallenBytes, int width, int height)
    {
        List<(int X,int Y)> possibles = new List<(int X, int Y)>();
        if(position.X > 0)
        {
            possibles.Add((position.X-1, position.Y));
        }
        if (position.Y > 0)
        {
            possibles.Add((position.X, position.Y-1));
        }
        if (position.X < width-1)
        {
            possibles.Add((position.X+1,position.Y));
        }
        if (position.Y < height - 1)
        {
            possibles.Add((position.X,position.Y+1));
        }
        List<(int X, int Y)> output = new List<(int X, int Y)>();
        for (int i = 0; i < possibles.Count; i++)
        {
            if (!fallenBytes.Contains(possibles[i]))
            {
                output.Add(possibles[i]);
            }
        }
        return output;
    }
}
