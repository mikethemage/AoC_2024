using System.Runtime.CompilerServices;

namespace AoC_2024_07;

internal class Program
{
    static void Main(string[] args)
    {        
        string filename = "sample.txt";
        //string filename = "input.txt";
        
        List<CalibrationInput> calibrationInputs = InitializeData(filename);
        long total = CalculateTotal(calibrationInputs, 1);
        Console.WriteLine($"Part One total true values: {total}");

        total = CalculateTotal(calibrationInputs, 2);
        Console.WriteLine($"Part Two total true values: {total}");
    }

    private static long CalculateTotal(List<CalibrationInput> calibrationInputs, int partNumber)
    {
        long total = 0;
        foreach (var calibrationInput in calibrationInputs)
        {
            if (PossibleEquation(calibrationInput, partNumber))
            {
                total += calibrationInput.TestValue;
            }
        }

        return total;
    }

    private static bool PossibleEquation(CalibrationInput calibrationInput, int partNumber)
    {
        List<long> possibleSolutions = new List<long> { calibrationInput.Numbers[0] };

        for (int i = 1; i < calibrationInput.Numbers.Count; i++)
        {
            List<long> newPossibleSolutions = new List<long>();
            foreach (var possibleSolution in possibleSolutions)
            {
                long newValue = possibleSolution + calibrationInput.Numbers[i];
                if (newValue <= calibrationInput.TestValue)
                {
                    newPossibleSolutions.Add(newValue);
                }

                newValue = possibleSolution * calibrationInput.Numbers[i];
                if (newValue <= calibrationInput.TestValue)
                {
                    newPossibleSolutions.Add(newValue);
                }

                if(partNumber==2)
                {
                    newValue = long.Parse(possibleSolution.ToString() + calibrationInput.Numbers[i].ToString());
                    if (newValue <= calibrationInput.TestValue)
                    {
                        newPossibleSolutions.Add(newValue);
                    }
                }
            }

            if(newPossibleSolutions.Count == 0)
            {
                return false;
            }
            possibleSolutions = newPossibleSolutions;
        }

        if (possibleSolutions.Contains(calibrationInput.TestValue))
        {
            return true;
        }
        return false;
    }

    private static List<CalibrationInput> InitializeData(string filename)
    {
        var lines = File.ReadAllLines(filename);
        List<CalibrationInput> calibrationInputs = new List<CalibrationInput>();
        foreach (var line in lines)
        {
            var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            CalibrationInput calibrationInput = new CalibrationInput { TestValue = long.Parse(tokens[0].Replace(":", "")) };
            for (int i = 1; i < tokens.Length; i++)
            {
                calibrationInput.Numbers.Add(long.Parse(tokens[i]));
            }
            calibrationInputs.Add(calibrationInput);
        }
        return calibrationInputs;
    }
}

internal class CalibrationInput
{
    public required long TestValue { get; set; }
    public List<long> Numbers { get; private set; } = new List<long>();
}