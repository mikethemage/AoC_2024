namespace AoC_2024_10;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";

        Topography topography = LoadTopography(filename);

        List<TrailPosition> trailHeads = topography.GetTrailHeads();

        int totalScore = 0;
        int totalRating = 0;

        foreach (TrailPosition trailHead in trailHeads)
        {
            List<TrailPosition> paths = new List<TrailPosition> { trailHead };
            List<TrailPosition> destinations = new List<TrailPosition>();

            while (paths.Count > 0)
            {
                destinations.AddRange(paths.Where(x => topography.GetHeight(x) == 9));

                List<TrailPosition> newPaths = new List<TrailPosition>();
                foreach (var path in paths)
                {
                    newPaths.AddRange(topography.GetNextPositions(path));
                }
                //currentScore++;
                paths = newPaths;
            }

            totalScore += destinations.Select(d=>(d.X, d.Y)).Distinct().Count();
            totalRating += destinations.Count;
        }

        Console.WriteLine($"Part One - Total score: {totalScore}");

        Console.WriteLine($"Part Two - Total ratings: {totalRating}");

    }

    private static Topography LoadTopography(string filename)
    {
        string[] lines = File.ReadAllLines(filename);

        int[][] map = new int[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
        {
            map[i] = lines[i].Select(x => int.Parse(x.ToString())).ToArray();
        }

        return new Topography(map);
    }
}

internal class Topography
{
    public int Height { get; private set; }

    public int Width { get; private set; }

    private readonly int[][] _map;

    public Topography(int[][] map)
    {
        _map = map;
        Height = _map.Length;
        if (Height > 0)
        {
            Width = _map[0].Length;
        }
    }

    public int GetHeight(TrailPosition trailPosition)
    {
        return _map[trailPosition.Y][trailPosition.X];
    }

    public List<TrailPosition> GetTrailHeads()
    {
        List<TrailPosition> trailHeads = new List<TrailPosition>();
        for (int row = 0; row < _map.Length; row++)
        {
            for (int column = 0; column < _map[row].Length; column++)
            {
                if (_map[row][column] == 0)
                {
                    trailHeads.Add(new TrailPosition { X = column, Y = row });
                }
            }
        }
        return trailHeads;
    }

    public List<TrailPosition> GetNextPositions(TrailPosition currentPosition)
    {
        int currentHeight = _map[currentPosition.Y][currentPosition.X];
        List<TrailPosition> potentials = new List<TrailPosition>();

        if (currentPosition.X > 0)
        {
            potentials.Add(new TrailPosition { X = currentPosition.X - 1, Y = currentPosition.Y });
        }

        if (currentPosition.Y > 0)
        {
            potentials.Add(new TrailPosition { X = currentPosition.X, Y = currentPosition.Y - 1 });
        }

        if (currentPosition.X < Width - 1)
        {
            potentials.Add(new TrailPosition { X = currentPosition.X + 1, Y = currentPosition.Y });
        }

        if (currentPosition.Y < Height - 1)
        {
            potentials.Add(new TrailPosition { X = currentPosition.X, Y = currentPosition.Y + 1 });
        }

        return potentials.Where(t => _map[t.Y][t.X] == currentHeight + 1).ToList();
    }
}

internal class TrailPosition
{
    public required int X { get; set; }
    public required int Y { get; set; }
}