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
        for (int j = 0; j < map.Length; j++)
        {
            for (int i = 0; i < map[j].Length; i++)
            {
                if (map[j][i] == '^')
                {
                    guardX = i;
                    guardY = j;
                    break;
                }
            }
            if (guardX > -1)
            {
                break;
            }
        }
        map[guardY][guardX] = '.';

        int steps = 0;
        while (guardX >= 0 && guardY >= 0 && guardX < map[0].Length && guardY < map.Length)
        {
            int xOffset = 0;
            int yOffset = 0;
            switch (guardFacing)
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

            int newX = guardX + xOffset;
            int newY = guardY + yOffset;

            if (newY < 0 || newY >= map.Length || newX < 0 || newX >= map[0].Length || map[newY][newX]=='.' || map[newY][newX] == 'X')
            {                
                if(map[guardY][guardX] == '.')
                {
                    steps++;
                    map[guardY][guardX] = 'X';
                }
                guardX = newX;
                guardY = newY;
            }
            else
            {
                guardFacing = (guardFacing + 1) % 4;
            }
        }

        Console.WriteLine($"Number of steps: {steps}");

        for (int j = 0; j < map.Length; j++)
        {
            for (int i = 0; i < map[0].Length; i++)
            {
                Console.Write(map[j][i]);
            }
            Console.WriteLine();
        }
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
