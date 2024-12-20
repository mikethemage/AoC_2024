﻿
namespace AoC_2024_15;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "largesample.txt";
        //string filename = "input.txt";

        for(int partNumber = 1; partNumber<=2;partNumber++)
        {
            PuzzleData puzzleData = LoadData(filename);

            if (partNumber == 2)
            {
                puzzleData.DoubleWidth();
            }

            MapFeature robot = puzzleData.Map.Where(f => f.FeatureType == '@').First();

            //Console.WriteLine("Initial State:");
            //puzzleData.DrawMap();
            //Console.WriteLine();

            foreach (char instruction in puzzleData.Instructions)
            {
                MoveRobot(robot, instruction, puzzleData.Map);
                //Console.WriteLine($"Move {instruction}:");
                //puzzleData.DrawMap();
                //Console.WriteLine();
            }

            Console.WriteLine($"Part {partNumber}: Sum of all boxes GPS: {puzzleData.GetAllBoxGps()}");
        }        
    }

    private static void MoveRobot(MapFeature robot, char instruction, List<MapFeature> map)
    {
        (int xOffset, int yOffset) offset;
        switch (instruction)
        {
            case '>':
                offset = (1, 0);
                break;

            case '<':
                offset = (-1, 0);
                break;

            case '^':
                offset = (0, -1);
                break;

            case 'v':
                offset = (0, 1);
                break;

            default:
                throw new Exception($"Invalid instruction!: {instruction}");
        }

        

        MapFeature? nextFeature = null;

        Stack<MapFeature> potentialFeatures = new Stack<MapFeature>();
        potentialFeatures.Push(robot);
        HashSet<MapFeature> featuresToMove = new HashSet<MapFeature>();

        while(potentialFeatures.Count > 0) 
        {
            MapFeature currentFeature = potentialFeatures.Pop();
            featuresToMove.Add(currentFeature);

            int currentX = currentFeature.X + offset.xOffset;
            int currentY = currentFeature.Y + offset.yOffset;
            nextFeature=map.Where(f=>f.X==currentX && f.Y==currentY).FirstOrDefault();
            
            if (nextFeature!=null)
            {
                if(nextFeature.FeatureType=='#')
                {
                    return;
                }
                else
                {                    
                    potentialFeatures.Push(nextFeature);
                    if (offset.xOffset == 0)
                    {
                        if (nextFeature.FeatureType == '[')
                        {
                            potentialFeatures.Push(map.Where(f => f.X == nextFeature.X + 1 && f.Y == nextFeature.Y).First());
                        }
                        if (nextFeature.FeatureType == ']')
                        {
                            potentialFeatures.Push(map.Where(f => f.X == nextFeature.X - 1 && f.Y == nextFeature.Y).First());
                        }
                    }
                    
                }
            }

        } 

        foreach (MapFeature feature in featuresToMove)
        {
            feature.X += offset.xOffset;
            feature.Y += offset.yOffset;
        }
    }

    private static PuzzleData LoadData(string filename)
    {
        List<char> instructions = new List<char>();
        string[] lines = File.ReadAllLines(filename);
        bool readInstructions = false;
        List<MapFeature> map = new List<MapFeature>();
        int height = 0;
        for (int row = 0; row < lines.Length; row++)
        {
            if (string.IsNullOrEmpty(lines[row]))
            {
                readInstructions = true;
                height = row;
            }
            else if (!readInstructions)
            {
                for (int col = 0; col < lines[row].Length; col++)
                {
                    if(lines[row][col]!='.')
                    {
                        map.Add(new MapFeature(lines[row][col], col, row));
                    }                    
                }
            }
            else
            {
                instructions.AddRange(lines[row].ToCharArray());
            }
        }
        return new PuzzleData(map,instructions, lines[0].Length, height);
    }
}

internal class PuzzleData
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public List<MapFeature> Map { get; private set; }
    public List<char> Instructions { get; private set; }
    public PuzzleData(List<MapFeature> map, List<char> instructions, int width, int height)
    {
        Map = map;
        Instructions = instructions;
        Width = width;
        Height = height;
    }

    public void DoubleWidth()
    {
        Width *= 2;
        List<MapFeature> newFeatures = new List<MapFeature>();
        foreach (MapFeature feature in Map)
        {
            feature.X *= 2;
            if(feature.FeatureType=='#')
            {
                newFeatures.Add(new MapFeature('#', feature.X + 1, feature.Y));
            }
            if(feature.FeatureType=='O')
            {
                feature.FeatureType = '[';
                newFeatures.Add(new MapFeature(']', feature.X + 1, feature.Y));
            }
        }
        Map.AddRange(newFeatures);
    }

    public int GetAllBoxGps()
    {
        return Map.Where(f=>f.FeatureType=='O'||f.FeatureType=='[').Sum(f=>f.GetGps());
    }

    public void DrawMap()
    {
        for (int row = 0; row<Height; row++)
        {
            for(int col = 0; col<Width; col++)
            {
                MapFeature? mapFeature = Map.Where(f=>f.X == col && f.Y==row).FirstOrDefault();
                if (mapFeature != null)
                {
                    Console.Write(mapFeature.FeatureType);
                }
                else
                {
                    Console.Write('.');
                }
            }
            Console.WriteLine();
        }
    }
}

internal class MapFeature
{
    public int X { get; set; }
    public int Y { get; set; }
    public char FeatureType { get; set; }
    public bool Moveable { get; private set; }

    public int GetGps()
    {
        /*
         * GPS coordinate of a box is equal to 100 times its distance from the top edge of the map plus its distance from the left edge of the map.
         */
        return (100 * Y) + X;
    }

    public MapFeature(char featureType, int initialX, int initialY)
    {
        X = initialX;
        Y = initialY;
        FeatureType = featureType;
        if(FeatureType == '#')
        {
            Moveable=false;
        }
        else
        {
            Moveable = true;
        }
    }

}