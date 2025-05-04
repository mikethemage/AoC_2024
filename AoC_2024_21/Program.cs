namespace AoC_2024_21;

class Program
{
    static void Main()
    {
        string[] codes = File.ReadAllLines("sample.txt");
        //string[] codes = File.ReadAllLines("input.txt");

        KeypadType numericKeypad = GenerateNumericKeypad();
        KeypadType directionalKeypad = GenerateDirectionalKeypad();

        MemoizedBFSProvider memoizedBFSProvider = new MemoizedBFSProvider();
        int totalComplexity = 0;
        foreach (var code in codes)
        {
            totalComplexity += RunForCode(numericKeypad, directionalKeypad, memoizedBFSProvider, code);
        }
        
        
        Console.WriteLine($"Total Complexity: {totalComplexity}");
    }

    private static int RunForCode(KeypadType numericKeypad, KeypadType directionalKeypad, MemoizedBFSProvider memoizedBFSProvider, string code)
    {
        char currentKey = 'A';
        string finalSequence = string.Empty;
        foreach (char nextKey in code)
        {

            List<string> solutions = GetSolutionsAtLevel(2, currentKey, nextKey, numericKeypad, directionalKeypad, memoizedBFSProvider);
            int minSolutionLength = solutions.Min(s => s.Length);
            var solution = solutions.Where(x => x.Length == minSolutionLength).FirstOrDefault();
            if (solution != null)
            {
                Console.WriteLine($"Inputing Key: {nextKey}, Sequence: {solution}, Length: {solution.Length}");
                finalSequence += solution;
            }
            currentKey = nextKey;
        }

        int numericPartOfCode = int.Parse(code.Replace("A", ""));

        int complexity = numericPartOfCode * finalSequence.Length;

        Console.WriteLine($"Code: {code}, Numeric Part: {numericPartOfCode}, Final Sequence: {finalSequence}, Length: {finalSequence.Length}, Complexity: {complexity}");
    
        return complexity;
    }

    private static List<string> GetSolutionsAtLevel(int solutionLevel, char startKey, char targetKey, KeypadType numericKeypad, KeypadType directionalKeypad, MemoizedBFSProvider memoizedBFSProvider)
    {
        if(solutionLevel == 0)
        {
            return memoizedBFSProvider.MemoizedBFS(numericKeypad, startKey, targetKey);
        }
        else
        {
            var lowerLevelSolutions = GetSolutionsAtLevel(solutionLevel - 1, startKey, targetKey, numericKeypad, directionalKeypad, memoizedBFSProvider);
            List<string> temp = new List<string>();
            foreach (var lowerLevelSolution in lowerLevelSolutions)
            {
                List<string> temp2 = new List<string>();
                char currentKey = 'A';
                foreach (var nextKey in lowerLevelSolution)
                {
                    List<string> temp3 = memoizedBFSProvider.MemoizedBFS(directionalKeypad, currentKey, nextKey);
                    currentKey = nextKey;

                    if (temp2.Count == 0)
                    {
                        temp2 = temp3;
                    }
                    else
                    {
                        temp2 = temp2.SelectMany(x=>temp3.Select(y=>x+y)).ToList();
                    }
                }
                temp.AddRange(temp2);                
            }
            return temp;
        }        
    }

    private static KeypadType GenerateNumericKeypad()
    {
        string[] numeric = { "789", "456", "123", " 0A" };
        return new KeypadType(numeric);
    }

    private static KeypadType GenerateDirectionalKeypad()
    {
        string[] directional = { " ^A", "<v>" };
        return new KeypadType(directional);
    }    
}

public class MemoizedBFSProvider
{
    private Dictionary<(KeypadType keypadType, char sourceKey, char targetKey), List<string>> cache = new Dictionary<(KeypadType keypadType, char sourceKey, char targetKey), List<string>>();
    
    public List<string> MemoizedBFS(KeypadType keypadType, char sourceKey, char targetKey)
    {
        var key = (keypadType, sourceKey, targetKey);
        if (cache.ContainsKey(key))
        {
            return cache[key];
        }
        else
        {
            List<KeypadState> currentStates = new List<KeypadState> { new KeypadState(sourceKey) };
            while (!currentStates.Any(x => x.CurrentKey == targetKey))
            {
                List<KeypadState> nextStates = new List<KeypadState>();
                foreach (var state in currentStates)
                {
                    nextStates.AddRange(state.GetNextStates(keypadType));
                }
                currentStates = nextStates;
            }
            int minSequenceLength = currentStates.Where(x => x.CurrentKey == targetKey).Min(x => x.SequenceLength);

            List<string> solutions = currentStates.Where(x => x.CurrentKey == targetKey && x.SequenceLength == minSequenceLength).Select(x => x.Sequence + 'A').ToList();

            cache.Add(key, solutions);

            return solutions;
        }           
    }
}

public class KeypadState
{
    public char CurrentKey { get; private set; }
    public int SequenceLength { get; private set; }
    public string Sequence { get; private set; }
    public KeypadState(char currentKey)
    {
        CurrentKey = currentKey;
        SequenceLength = 0;
        Sequence = string.Empty;
    }
    public KeypadState(char currentKey, int sequenceLength, string sequence)
    {
        CurrentKey = currentKey;
        SequenceLength = sequenceLength;
        Sequence = sequence;
    }

    public List<KeypadState> GetNextStates(KeypadType keypadType)
    {
        int nextSequenceLength = SequenceLength + 1;
        return keypadType.KeyDestinations[CurrentKey]
            .Select(destination => new KeypadState(destination.key,
                nextSequenceLength,
                Sequence + destination.direction))
            .ToList();
    }
}

public class KeypadType
{
    public Dictionary<char, List<(char direction, char key)>> KeyDestinations { get; private set; } = new Dictionary<char, List<(char direction, char key)>>();

    private void AddDirection(char sourceKey, char direction, char destinationKey)
    {
        if (destinationKey != ' ')
        {
            KeyDestinations[sourceKey].Add((direction, destinationKey));
        }
    }

    public KeypadType(string[] keypadArray)
    {
        for (int row = 0; row < keypadArray.Length; row++)
        {
            for (int column = 0; column < keypadArray[row].Length; column++)
            {
                KeyDestinations.Add(keypadArray[row][column], new List<(char direction, char key)>());
                if (row > 0)
                {
                    AddDirection(keypadArray[row][column], '^', keypadArray[row - 1][column]);
                }

                if (row < keypadArray.Length - 1)
                {
                    AddDirection(keypadArray[row][column], 'v', keypadArray[row + 1][column]);
                }

                if (column > 0)
                {
                    AddDirection(keypadArray[row][column], '<', keypadArray[row][column - 1]);
                }

                if (column < keypadArray[row].Length - 1)
                {
                    AddDirection(keypadArray[row][column], '>', keypadArray[row][column + 1]);
                }
            }

        }
    }
}