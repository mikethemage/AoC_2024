namespace AoC_2024_22;

internal class Program
{
    static void Main(string[] args)
    {
        const string filename = "sample.txt";
        //const string filename = "input.txt";

        string[] lines = File.ReadAllLines(filename);
        foreach (string line in lines)
        {
            Console.WriteLine(line);
        }
    }
}
