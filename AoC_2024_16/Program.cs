
namespace AoC_2024_16;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample2.txt";
        string filename = "input.txt";
        char[][] map = LoadData(filename);

        (int X, int Y) startPosition = (0, 0);
        (int X, int Y) endPosition = (0, 0);
        for (int row = 0; row < map.Length; row++)
        {
            for (int col = 0; col < map[row].Length; col++)
            {
                if (map[row][col] == 'S')
                {
                    startPosition = (col, row);
                    map[row][col] = '.';
                }

                if (map[row][col] == 'E')
                {
                    endPosition = (col, row);
                    map[row][col] = '.';
                }
            }
        }

        StateInfo initialState = new StateInfo { State= (startPosition, 1), Steps =0 };
        HashSet<((int X, int Y) Position, int Facing)> stateHistory = new HashSet<((int X, int Y) Position, int Facing)>();
        PriorityQueue<StateInfo, int> priorityQueue = new PriorityQueue<StateInfo, int>();
        priorityQueue.Enqueue(initialState, 0);
        int bestScore = -1;

        while (priorityQueue.Count > 0)
        {
            StateInfo currentState = priorityQueue.Dequeue();
            if (bestScore >= 0)
            {
                if (currentState.Steps > bestScore)
                {
                    continue;
                }
            }

            if(currentState.State.Position==endPosition)
            {
                Console.WriteLine($"Part 1 solved: {currentState.Steps}");
                Console.WriteLine();

                currentState.Visited.Add(endPosition);
                UpdateMap(currentState.Visited, map);

                bestScore = currentState.Steps;
                continue;
            }

            stateHistory.Add(currentState.State);
            var movedState = MoveForward(currentState);
            if (!IsWall(movedState.State.Position, map)) {
                EnqueueIfNotInHistory(movedState, priorityQueue, stateHistory);
            }
            EnqueueIfNotInHistory(Rotate(currentState, 1), priorityQueue, stateHistory);
            EnqueueIfNotInHistory(Rotate(currentState, -1), priorityQueue, stateHistory);
        }

        //OutputMap(map);
        Console.WriteLine();

        Console.WriteLine($"Part 2 solved: {TilesOnPaths(map)}");
        

    }

    private static void UpdateMap(HashSet<(int X, int Y)> visited, char[][] map)
    {
        foreach ((int X, int Y) location in visited)
        {
            map[location.Y][location.X] = 'O';
        }
    }

    private static void OutputMap(char[][] map)
    {
        for (int row = 0; row < map.Length; row++)
        {
            for (int col = 0; col < map[row].Length; col++)
            {
                Console.Write(map[row][col]);
            }
            Console.WriteLine();
        }
    }

    private static int TilesOnPaths(char[][] map)
    {
        int output = 0;
        for (int row = 0; row < map.Length; row++)
        {
            for (int col = 0; col < map[row].Length; col++)
            {
                if (map[row][col] == 'O')
                { 
                    output++;
                }
            }            
        }
        return output;
    }

    private static bool IsWall((int X, int Y) position, char[][] map)
    {
        return map[position.Y][position.X] == '#';
    }

    private static void EnqueueIfNotInHistory(StateInfo newState, PriorityQueue<StateInfo, int> priorityQueue, HashSet<((int X, int Y) Position, int Facing)> stateHistory)
    {
        if (!stateHistory.Contains(newState.State))
        {
            priorityQueue.Enqueue(newState, newState.Steps);
        }
    }

    private static StateInfo Rotate(StateInfo currentState, int direction)
    {
        int facing = currentState.State.Facing;
        facing += direction;

        while(facing<0)
        {
            facing += 4;
        }

        facing %= 4;
        int steps = currentState.Steps;
        steps += 1000;

        StateInfo output = new StateInfo { State=(currentState.State.Position, facing), Steps=steps, Visited=currentState.Visited };

        return output;
    }

    private static StateInfo MoveForward(StateInfo currentState)
    {
        (int X, int Y) position;
        switch (currentState.State.Facing)
        {
            case 0:
                position = (currentState.State.Position.X, currentState.State.Position.Y - 1);
                break;
            case 1:
                position = (currentState.State.Position.X + 1, currentState.State.Position.Y);
                break;
            case 2:
                position = (currentState.State.Position.X, currentState.State.Position.Y + 1);
                break;
            case 3:
                position = (currentState.State.Position.X-1, currentState.State.Position.Y);
                break;
            default:
                throw new Exception("Invalid facing!");
        }
        HashSet<(int X, int Y)> history = new HashSet<(int X, int Y)>(currentState.Visited);
        history.Add(currentState.State.Position);
        StateInfo output = new StateInfo { State=(position, currentState.State.Facing), Steps = currentState.Steps + 1, Visited= history };

        return output;
    }

    private static char[][] LoadData(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        char[][] map = new char[lines.Length][];
        for (int row = 0; row < lines.Length; row++)
        {
            map[row] = lines[row].ToCharArray();
        }

        return map;
    }
}

internal class StateInfo
{
    public ((int X, int Y) Position, int Facing) State { get; set; }
    public int Steps { get; set; }
    public HashSet<(int X, int Y)> Visited { get; set; } = new HashSet<(int X, int Y)>();
}