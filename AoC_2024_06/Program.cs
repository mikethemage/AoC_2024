namespace AoC_2024_06;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";
        char[][] map = LoadInput(filename);

        int guardX = -1;
        int guardY = -1;
        int guardFacing = 0;
        HashSet<(int, int)> obstacles = new();
        for (int j = 0; j < map.Length; j++)
        {
            for (int i = 0; i < map[j].Length; i++)
            {
                if (map[j][i] == '^')
                {
                    guardX = i;
                    guardY = j;                    
                }

                if(map[j][i] =='#')
                {
                    obstacles.Add((i, j));
                }
            }            
        }

        PuzzleState initialPuzzleState = new PuzzleState(guardX, guardY, guardFacing, obstacles, map.Length, map[0].Length);

        PuzzleState partOnePuzzleState = new PuzzleState(initialPuzzleState);
        partOnePuzzleState.RunPuzzle();        

        Console.WriteLine($"Number of steps: {partOnePuzzleState.GetUniqueHistory().Count()}");

        HashSet<(int, int)> possibleObstacleLocations = new HashSet<(int,int)>(partOnePuzzleState.GetUniqueHistory());
        if(possibleObstacleLocations.Contains((guardX,guardY)))
        {
            possibleObstacleLocations.Remove((guardX,guardY));
        }

        int loopCount = 0;
        foreach ((int, int) possibleObstacleLocation in possibleObstacleLocations)
        {
            PuzzleState partTwoPuzzleState = new PuzzleState(initialPuzzleState);
            partTwoPuzzleState.AddObstacle(possibleObstacleLocation);
            partTwoPuzzleState.RunPuzzle();
            if(partTwoPuzzleState.ContainsLoop)
            {
                loopCount++;
            }
        }

        Console.WriteLine($"Number of loops: {loopCount}");

    }

    private static char[][] LoadInput(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        int height = lines.Length;
        char[][] map = new char[height][];
        for (int i = 0; i < height; i++)
        {
            map[i] = lines[i].ToCharArray();
        }
        return map;
    }
}

internal class PuzzleState
{
    private readonly PositionAndFacing _guard;
    private readonly Dictionary<(int, int), HashSet<int>> _history = new();
    private readonly HashSet<(int, int)> _obstacles;
    private readonly int _height;
    private readonly int _width;    
    public bool ContainsLoop { get; private set; } = false;

    public PuzzleState(int guardX, int guardY, int guardFacing, HashSet<(int, int)> obstacles, int height, int width)
    {
        _obstacles = obstacles;
        _height = height;
        _width = width;
        _guard = new PositionAndFacing { X = guardX, Y = guardY, Facing = guardFacing };
    }

    public PuzzleState(PuzzleState copyFrom)
    {
        _guard = new PositionAndFacing { X = copyFrom._guard.X, Y = copyFrom._guard.Y, Facing = copyFrom._guard.Facing };
        _obstacles = new HashSet<(int, int)>(copyFrom._obstacles);
        _height = copyFrom._height;
        _width = copyFrom._width;
    }

    public HashSet<(int,int)> GetUniqueHistory()
    {
        return _history.Keys.ToHashSet();
    }

    public void AddObstacle((int,int) newObstacle)
    {
        _obstacles.Add(newObstacle);
    }

    public void RunPuzzle()
    {        
        while (_guard.X >= 0 && _guard.Y >= 0 && _guard.X < _width && _guard.Y < _height && !ContainsLoop)
        {
            int xOffset = 0;
            int yOffset = 0;
            switch (_guard.Facing)
            {
                case 0:
                    yOffset = -1;
                    break;
                case 1:
                    xOffset = 1;
                    break;
                case 2:
                    yOffset = 1;
                    break;
                case 3:
                    xOffset = -1;
                    break;
                default:
                    throw new Exception("Invalid facing!");
            }

            int newX = _guard.X + xOffset;
            int newY = _guard.Y + yOffset;
            if (_obstacles.Contains((newX, newY)))
            {
                _guard.Facing = (_guard.Facing + 1) % 4;
            }
            else
            {                
                if (_history.ContainsKey((_guard.X, _guard.Y)))
                {
                    if(_history[(_guard.X, _guard.Y)].Contains(_guard.Facing))
                    {
                        ContainsLoop = true;
                    }
                    else
                    {
                        _history[(_guard.X, _guard.Y)].Add(_guard.Facing);
                    }                    
                }
                else
                {
                    _history.Add((_guard.X, _guard.Y), new HashSet<int> { _guard.Facing });
                }
                _guard.X = newX;
                _guard.Y = newY;
            }                
        }
    }

}

internal class PositionAndFacing
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Facing { get; set; }
}
