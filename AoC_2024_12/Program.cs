namespace AoC_2024_12;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";
        string[] lines = File.ReadAllLines(filename);
        HashSet<Region> regions = new HashSet<Region>();
        Plot[][] grid = new Plot[lines.Length][];
        for (int row = 0; row < lines.Length; row++)
        {
            char[] tokens = lines[row].ToCharArray();
            grid[row] = new Plot[tokens.Length];
            for (int col = 0; col < tokens.Length; col++)
            {
                grid[row][col] = new Plot { Row = row, Column = col, PlantType = tokens[col] };
                regions.Add(grid[row][col].ContainingRegion);
            }
        }

        for (int row = 0; row < grid.Length; row++)
        {
            for (int col = 0; col < grid[row].Length; col++)
            {                    
                if(row>0 && grid[row - 1][col].PlantType== grid[row][col].PlantType)
                {
                    grid[row][col].Perimeter--;
                    grid[row][col].TopPerimeter = false;

                    grid[row - 1][col].Perimeter--;
                    grid[row - 1][col].BottomPerimeter = false;


                    if (grid[row][col].ContainingRegion != grid[row - 1][col].ContainingRegion)
                    {
                        var regionToRemove = grid[row][col].ContainingRegion;
                        regions.Remove(regionToRemove);
                        foreach (var plot in regionToRemove.Plots)
                        {
                            plot.ContainingRegion = grid[row - 1][col].ContainingRegion;
                            plot.ContainingRegion.Plots.Add(plot);
                        }                            
                    }                       
                        
                }
                if (col >0 && grid[row][col-1].PlantType == grid[row][col].PlantType)
                {
                    grid[row][col].Perimeter--;
                    grid[row][col].LeftPerimeter = false;

                    grid[row ][col-1].Perimeter--;
                    grid[row][col-1].RightPerimeter = false;



                    if (grid[row][col].ContainingRegion != grid[row][col-1].ContainingRegion)
                    {
                        var regionToRemove = grid[row][col].ContainingRegion;
                        regions.Remove(regionToRemove);
                        foreach (var plot in regionToRemove.Plots)
                        {
                            plot.ContainingRegion = grid[row][col-1].ContainingRegion;
                            plot.ContainingRegion.Plots.Add(plot);
                        }
                    }
                        
                }                   
            }
        }

        int part1Total = 0;
        int part2Total = 0;
        foreach (Region region in regions)
        {
            if (region.Plots.Count > 0)
            {
                Console.Write($"Type: {region.Plots.First().PlantType} ");
            }
            Console.Write($"Region with area: {region.Area}, Perimeter: {region.Perimeter}, Cost: {region.Area*region.Perimeter}");
            part1Total += region.Area * region.Perimeter;

            var sides = CountContiguous(region.Plots.Where(x => x.LeftPerimeter), true);
            sides += CountContiguous(region.Plots.Where(x => x.RightPerimeter), true);
            sides  += CountContiguous(region.Plots.Where(x => x.TopPerimeter), false);
            sides += CountContiguous(region.Plots.Where(x => x.BottomPerimeter), false);

            Console.WriteLine($", Sides: {sides}, new cost: {region.Area * sides}");

            part2Total += sides * region.Area;

        }
        Console.WriteLine($"Part 1 Total: {part1Total}");
        Console.WriteLine($"Part 2 Total: {part2Total}");
    }

    private static int CountContiguous(IEnumerable<Plot> plots, bool vertical)
    {
        int output = 0;
        if (vertical)
        {
            var groupedPlots = plots.GroupBy(x => x.Column);
            foreach (var group in groupedPlots)
            {
                var rows = group.OrderBy(x=>x.Row).Select(x => x.Row).ToList();
                int sideCount = 1;
                for (int i = 1; i < rows.Count; i++)
                {
                    if (rows[i - 1] != rows[i]-1)
                    {
                        sideCount++;
                    }
                }
                output += sideCount;
            }
        }
        else //horizontal
        {
            var groupedPlots = plots.GroupBy(x => x.Row);
            foreach (var group in groupedPlots)
            {
                var columns = group.OrderBy(x=>x.Column).Select(x=>x.Column).ToList();
                int sideCount = 1;
                for (int i = 1; i < columns.Count; i++)
                {
                    if (columns[i-1] != columns[i]-1)
                    {
                        sideCount++;
                    }
                }
                output += sideCount;
            }
        }
        return output;
    }
}

internal class Plot
{
    public required int Row { get; set; }
    public required int Column { get; set; }
    public required char PlantType { get; set; }
    public int Perimeter { get; set; } = 4;
    public int Area { get; set; } = 1;
    public Region ContainingRegion { get; set; }
    public Plot()
    {
        ContainingRegion = new Region();
        ContainingRegion.Plots.Add(this);
    }    

    public bool TopPerimeter { get; set; } = true;
    public bool BottomPerimeter { get; set; } = true;
    public bool LeftPerimeter { get; set; } = true;
    public bool RightPerimeter { get; set; }= true;
}

internal class Region
{
    public HashSet<Plot> Plots { get; set; } = new HashSet<Plot>();
    public int Area
    {
        get
        {
            return Plots.Sum(p => p.Area);
        }
    }

    public int Perimeter
    {
        get
        {
            return Plots.Sum(p => p.Perimeter);
        }
    }
}