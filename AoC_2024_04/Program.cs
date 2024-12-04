
namespace AoC_2024_04;

internal class Program
{
    static void Main(string[] args)
    {
        string filename = "sample.txt";
        //string filename = "input.txt";

        char[][] grid = LoadData(filename);

        int matchesPartOne = PartOne(grid);
        Console.WriteLine($"Part One: {matchesPartOne}");

        int matchesPartTwo = PartTwo(grid);
        Console.WriteLine($"Part Two: {matchesPartTwo}");
    }

    private static int PartTwo(char[][] grid)
    {
        int matches = 0;
        for (int j = 1; j < grid.Length - 1; j++)
        {
            for (int i = 1; i < grid[j].Length - 1; i++)
            {
                if (grid[j][i] == 'A')
                {
                    if (((grid[j - 1][i - 1] == 'M' && grid[j + 1][i + 1] == 'S') || (grid[j - 1][i - 1] == 'S' && grid[j + 1][i + 1] == 'M')) &&
                          ((grid[j - 1][i + 1] == 'M' && grid[j + 1][i - 1] == 'S') || (grid[j - 1][i + 1] == 'S' && grid[j + 1][i - 1] == 'M'))
                        )
                    {
                        matches++;
                    }
                }
            }
        }
        return matches;
    }

    private static int PartOne(char[][] grid)
    {
        int matches = 0;
        for (int j = 0; j < grid.Length; j++)
        {
            for (int i = 0; i < grid[j].Length; i++)
            {
                if (grid[j][i] == 'X')
                {
                    //Horizontal forwards
                    if (i + 3 < grid[j].Length)
                    {
                        if (grid[j][i + 1] == 'M' && grid[j][i + 2] == 'A' && grid[j][i + 3] == 'S')
                        {
                            matches++;
                        }
                    }

                    //Vertical downwards
                    if (j + 3 < grid.Length)
                    {
                        if (grid[j + 1][i] == 'M' && grid[j + 2][i] == 'A' && grid[j + 3][i] == 'S')
                        {
                            matches++;
                        }
                    }

                    //Horizontal backwards
                    if (i - 3 >= 0)
                    {
                        if (grid[j][i - 1] == 'M' && grid[j][i - 2] == 'A' && grid[j][i - 3] == 'S')
                        {
                            matches++;
                        }
                    }

                    //Vertical upwards
                    if (j - 3 >= 0)
                    {
                        if (grid[j - 1][i] == 'M' && grid[j - 2][i] == 'A' && grid[j - 3][i] == 'S')
                        {
                            matches++;
                        }
                    }

                    //Diagonal forwards and down
                    if (i + 3 < grid[j].Length && j + 3 < grid.Length)
                    {
                        if (grid[j + 1][i + 1] == 'M' && grid[j + 2][i + 2] == 'A' && grid[j + 3][i + 3] == 'S')
                        {
                            matches++;
                        }
                    }

                    //Diagonal forwards and up
                    if (i + 3 < grid[j].Length && j - 3 >= 0)
                    {
                        if (grid[j - 1][i + 1] == 'M' && grid[j - 2][i + 2] == 'A' && grid[j - 3][i + 3] == 'S')
                        {
                            matches++;
                        }
                    }

                    //diagonal backwards and up
                    if (i - 3 >= 0 && j - 3 >= 0)
                    {
                        if (grid[j - 1][i - 1] == 'M' && grid[j - 2][i - 2] == 'A' && grid[j - 3][i - 3] == 'S')
                        {
                            matches++;
                        }
                    }

                    //diagonal backwards and down
                    if (i - 3 >= 0 && j + 3 < grid.Length)
                    {
                        if (grid[j + 1][i - 1] == 'M' && grid[j + 2][i - 2] == 'A' && grid[j + 3][i - 3] == 'S')
                        {
                            matches++;
                        }
                    }
                }
            }
        }

        return matches;
    }

    private static char[][] LoadData(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        char[][] grid = new char[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
        {           
            grid[i] = lines[i].ToCharArray();
        }
        return grid;
    }
}
