using System.Collections.Concurrent;
using System.Text.RegularExpressions;

internal static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var part1Files = new[]
            {
                ("input.txt", "output_01_01.txt")
               
            };

            foreach (var (inputFile, outputFile) in part1Files)
            {
                SolvePart1(inputFile, outputFile);
            }

            var part2Files = new[]
            {
                ("input.txt", "output_02_03.txt", "ADD")
                
            };

            foreach (var (inputFile, outputFile, operation) in part2Files)
            {
                SolvePart2(inputFile, outputFile, operation);
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
    }

    private record Gate(string InputA, string InputB, string Operation);

    private record Problem(
        IDictionary<string, int> Values,
        IDictionary<string, Gate> Gates,
        int Digits
    );

    private static void SolvePart1(string inputFile, string outputFile)
    {
        if (string.IsNullOrWhiteSpace(inputFile) || string.IsNullOrWhiteSpace(outputFile))
        {
            throw new ArgumentException("File paths cannot be null or whitespace.");
        }

        var problem = ParseInput(inputFile);
        var value = ReadDec(problem, "z");
        File.WriteAllText(outputFile, value.ToString());
    }

    private static void SolvePart2(string inputFile, string outputFile, string operation)
    {
        if (string.IsNullOrWhiteSpace(inputFile) || string.IsNullOrWhiteSpace(outputFile))
        {
            throw new ArgumentException("File paths cannot be null or whitespace.");
        }

        var problem = ParseInput(inputFile);

        var ox = ReadDec(problem, "x");
        var oy = ReadDec(problem, "y");

        Console.WriteLine("#" + operation + ":");

        var tests = new List<(long X, long Y, long Expected)>
        {
            (0, 0, operation == "ADD" ? 0 + 0 : 0 & 0)
        };

        for (int i = 0; i < problem.Digits - 1; i++)
        {
            long x = 1L << i;
            long y = 1L << i;

            tests.Add((x, y, operation == "ADD" ? x + y : x & y));
            tests.Add((0, y, operation == "ADD" ? 0 + y : 0 & y));
            tests.Add((x, 0, operation == "ADD" ? x + 0 : x & 0));
        }

        // Test from input
        tests.Add((ox, oy, operation == "ADD" ? ox + oy : ox & oy));

        // More random tests
        var random = new Random();
        int max = 1 << (problem.Digits - 1);
        for (int i = 0; i < 500; i++)
        {
            var x = random.NextInt64(max);
            var y = random.NextInt64(max);
            tests.Add((x, y, operation == "ADD" ? x + y : x & y));
        }

        var badFixes = new HashSet<string>();
        var fixes = new List<List<string[]>> { new List<string[]>() };

        bool failedAnyTest = true;
        (long X, long Y, long Expected) lastTest = default;

        while (failedAnyTest)
        {
            failedAnyTest = false;
            int testCount = 0;
            var newFixes = new List<List<string[]>>();

            foreach (var test in tests)
            {
                testCount++;
                lastTest = test;

                var results = fixes
                    .AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount)
                    .Select(fix => CheckFix(problem, badFixes, test, fix))
                    .ToList();

                var anyFailed = false;
                var foundFixes = new List<List<string[]>>();

                foreach (var (failed, foundGoodFixes, foundBadFixes) in results)
                {
                    anyFailed |= failed;
                    foundFixes.AddRange(foundGoodFixes);
                    badFixes.UnionWith(foundBadFixes);
                }

                newFixes.AddRange(foundFixes);

                if (anyFailed)
                {
                    failedAnyTest = true;
                    break;
                }
            }

            fixes = newFixes.Distinct().ToList();

            Console.WriteLine($"Tests passed: {(failedAnyTest ? (testCount - 1) : testCount)}");
            Console.WriteLine($"Failed test: X={lastTest.X}, Y={lastTest.Y}, Expected={lastTest.Expected}");
            Console.WriteLine($"Found new fixes for last test: {fixes.Count}");
        }

        var answers = fixes
            .Select(fix =>
                string.Join(",", fix.SelectMany(swap => swap).Order()))
            .ToList();

        File.WriteAllLines(outputFile, answers);
    }

    private static (bool failed, List<List<string[]>> goodFixes, HashSet<string> badFixes)
        CheckFix(Problem problem, HashSet<string> badfixes, (long X, long Y, long Expected) test, List<string[]> fix)
    {
        var newFixes = new List<List<string[]>>();
        var newBadFixes = new HashSet<string>();

        var fixKey = GetFixKey(fix);

        if (!badfixes.Contains(fixKey) && IsFixesProblem(problem, test, fix))
        {
            newFixes.Add(fix);
            return (false, newFixes, newBadFixes);
        }

        newBadFixes.Add(fixKey);

        if (fix.Count == 4)
        {
            return (true, newFixes, newBadFixes);
        }

        var wrongOutputs = ComputeWiresWithWrongOutputs(problem, test, fix);
        var swaps = GenerateSwapVariants(wrongOutputs.ToArray(), 2);

        foreach (var swapPair in swaps)
        {
            var singleSwapFix = new List<string[]> { swapPair };
            var singleSwapKey = GetFixKey(singleSwapFix);

            if (!badfixes.Contains(singleSwapKey))
            {
                // works without this, but I believe that without this it is possible to miss some fixes
                // if (IsFixesProblem(problem, test, singleSwapFix))
                // {
                //     newFixes.Add(singleSwapFix);
                // }
                // else
                // {
                //     newBadFixes.Add(singleSwapKey);
                // }
            }

            if (fix.Any(f => f.Contains(swapPair[0]) || f.Contains(swapPair[1])))
            {
                continue;
            }

            var appendedFix = new List<string[]>(fix) { swapPair };
            var appendedKey = GetFixKey(appendedFix);

            if (!badfixes.Contains(appendedKey))
            {
                if (IsFixesProblem(problem, test, appendedFix))
                {
                    newFixes.Add(appendedFix);
                }
                else
                {
                    newBadFixes.Add(appendedKey);
                }
            }
        }

        return (true, newFixes, newBadFixes);
    }

    private static readonly ConcurrentDictionary<((long X, long Y, long Expected) test, string fixKey), IEnumerable<string>> ComputeWiresWithWrongOutputsCache
        = new();

    private static IEnumerable<string> ComputeWiresWithWrongOutputs(
        Problem problem,
        (long X, long Y, long Expected) test,
        List<string[]> fix)
    {
        var key = (test, GetFixKey(fix));
        if (ComputeWiresWithWrongOutputsCache.TryGetValue(key, out var cachedResult))
        {
            return cachedResult;
        }

        var copy = Copy(problem);
        WriteDec(copy, test.X, "x");
        WriteDec(copy, test.Y, "y");
        _ = ReadDec(copy, "z");

        DoSwap(copy, fix);

        var wrongOutputs = BackPropagateError(copy, DecToBin(test.Expected, copy.Digits));
        ComputeWiresWithWrongOutputsCache[key] = wrongOutputs;
        return wrongOutputs;
    }

    private static string GetFixKey(IEnumerable<string[]> fix)
    {
        var normalized = fix
            .Select(p => p.Order().ToArray())
            .OrderBy(p => p[0])
            .ToList();

        return string.Join(",", normalized.Select(arr => string.Join(',', arr)));
    }

    private static readonly ConcurrentDictionary<((long X, long Y, long Expected) test, string fixKey), bool> IsFixesProblemCache
        = new();

    private static bool IsFixesProblem(
        Problem problem,
        (long X, long Y, long Expected) test,
        IEnumerable<string[]> fix)
    {
        var cacheKey = (test, GetFixKey(fix));
        if (IsFixesProblemCache.TryGetValue(cacheKey, out var cachedResult))
        {
            return cachedResult;
        }

        var copy = Copy(problem);
        DoSwap(copy, fix);

        WriteDec(copy, test.X, "x");
        WriteDec(copy, test.Y, "y");

        try
        {
            var actualZ = ReadDec(copy, "z");
            var isGood = actualZ == test.Expected;
            IsFixesProblemCache[cacheKey] = isGood;
            return isGood;
        }
        catch (InvalidOperationException)
        {
            IsFixesProblemCache[cacheKey] = false;
            return false;
        }
    }

    private static void DoSwap(Problem copy, IEnumerable<string[]> swaps)
    {
        foreach (var swap in swaps)
        {
            var gateA = copy.Gates[swap[0]];
            var gateB = copy.Gates[swap[1]];

            copy.Gates[swap[0]] = gateB;
            copy.Gates[swap[1]] = gateA;
        }
    }

    private static Problem Copy(Problem problem) =>
        new(
            new Dictionary<string, int>(problem.Values),
            new Dictionary<string, Gate>(problem.Gates),
            problem.Digits
        );

    private static IEnumerable<string> BackPropagateError(Problem problem, string expectedFull)
    {
        var possibleWrongOutputs = new HashSet<string>();
        var queue = new Queue<Dictionary<string, int>>();
        var visited = new HashSet<string>();

        var expectedOutputsInitial = new Dictionary<string, int>();
        for (int i = problem.Digits - 1; i >= 0; i--)
        {
            var wireName = "z" + i.ToString("D2");
            expectedOutputsInitial[wireName] = expectedFull[expectedFull.Length - 1 - i] == '1' ? 1 : 0;
        }

        queue.Enqueue(expectedOutputsInitial);

        while (queue.Count > 0)
        {
            var count = queue.Count;
            for (int i = 0; i < count; i++)
            {
                var currentExpected = queue.Dequeue();
                var key = string.Join(";", currentExpected.OrderBy(p => p.Key)
                                                          .Select(p => $"{p.Key}={p.Value}"));
                if (!visited.Add(key))
                {
                    continue;
                }

                var possibleInputs = new Dictionary<string, HashSet<(int, int)>>();

                foreach (var (wireKey, expectedValue) in currentExpected)
                {
                    var actualValue = ComputeValue(problem, wireKey);
                    if (actualValue == expectedValue)
                    {
                        continue;
                    }

                    possibleWrongOutputs.Add(wireKey);

                    if (problem.Gates.TryGetValue(wireKey, out var gate))
                    {
                        AddPossibleInputs(possibleInputs, wireKey, expectedValue, gate);
                    }
                }

                var variants = GenerateExpectedOutputsVariants(problem, possibleInputs);
                foreach (var variant in variants)
                {
                    var variantKey = string.Join(";", variant.OrderBy(p => p.Key)
                                                             .Select(p => $"{p.Key}={p.Value}"));
                    if (!visited.Contains(variantKey))
                    {
                        queue.Enqueue(variant);
                    }
                }
            }
        }

        return possibleWrongOutputs;
    }

    private static IEnumerable<string[]> GenerateSwapVariants(string[] keys, int maxLength)
    {
        var result = new List<string[]>();
        var queue = new Queue<List<string>>();
        queue.Enqueue(new List<string>());

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Count > 0 && current.Count <= maxLength && current.Count % 2 == 0)
            {
                result.Add(current.ToArray());
            }

            if (current.Count >= maxLength || current.Count >= keys.Length)
            {
                continue;
            }

            var startIndex = current.Count == 0
                ? 0
                : Array.IndexOf(keys, current[^1]);

            for (int i = startIndex; i < keys.Length; i++)
            {
                if (current.Contains(keys[i])) continue;
                var newSubset = new List<string>(current) { keys[i] };
                queue.Enqueue(newSubset);
            }
        }

        return result;
    }

    private static List<Dictionary<string, int>> GenerateExpectedOutputsVariants(
        Problem problem,
        Dictionary<string, HashSet<(int, int)>> possibleInputs)
    {
        var keys = possibleInputs.Keys.ToArray();
        var result = new List<Dictionary<string, int>>();
        var queue = new Queue<(Dictionary<string, int>, int)>();
        queue.Enqueue((new Dictionary<string, int>(), 0));

        while (queue.Count > 0)
        {
            var (currentExpected, idx) = queue.Dequeue();
            if (idx == keys.Length)
            {
                if (currentExpected.Count > 0)
                {
                    result.Add(currentExpected);
                }
                continue;
            }

            var key = keys[idx];
            if (!problem.Gates.TryGetValue(key, out var gate))
            {
                continue;
            }

            foreach (var (inA, inB) in possibleInputs[key])
            {
                var newExpected = new Dictionary<string, int>(currentExpected);
                if (problem.Gates.ContainsKey(gate.InputA))
                {
                    newExpected[gate.InputA] = inA;
                }
                if (problem.Gates.ContainsKey(gate.InputB))
                {
                    newExpected[gate.InputB] = inB;
                }

                queue.Enqueue((newExpected, idx + 1));
            }
        }

        return result;
    }

    private static void AddPossibleInputs(
        Dictionary<string, HashSet<(int, int)>> expectedInputs,
        string key,
        int expected,
        Gate gate)
    {
        if (!expectedInputs.ContainsKey(key))
        {
            expectedInputs[key] = new HashSet<(int, int)>();
        }

        switch (gate.Operation)
        {
            case "AND":
                if (expected == 0)
                {
                    expectedInputs[key].Add((0, 0));
                    expectedInputs[key].Add((0, 1));
                    expectedInputs[key].Add((1, 0));
                }
                else
                {
                    expectedInputs[key].Add((1, 1));
                }
                break;

            case "OR":
                if (expected == 0)
                {
                    expectedInputs[key].Add((0, 0));
                }
                else
                {
                    expectedInputs[key].Add((0, 1));
                    expectedInputs[key].Add((1, 0));
                    expectedInputs[key].Add((1, 1));
                }
                break;

            case "XOR":
                if (expected == 0)
                {
                    expectedInputs[key].Add((0, 0));
                    expectedInputs[key].Add((1, 1));
                }
                else
                {
                    expectedInputs[key].Add((0, 1));
                    expectedInputs[key].Add((1, 0));
                }
                break;
        }
    }

    private static string DecToBin(long value, int n) =>
        Convert.ToString(value, 2).PadLeft(n, '0');

    private static long ReadDec(Problem problem, string prefix)
    {
        long value = 0;
        for (int i = problem.Digits - 1; i >= 0; i--)
        {
            value <<= 1;
            value += ComputeValue(problem, $"{prefix}{i:D2}");
        }
        return value;
    }

    private static void WriteDec(Problem problem, long value, string prefix) =>
        WriteBin(problem, DecToBin(value, problem.Digits), prefix);

    private static void WriteBin(Problem problem, string value, string prefix)
    {
        for (int i = problem.Digits - 1; i >= 0; i--)
        {
            var key = prefix + i.ToString("D2");
            if (problem.Values.ContainsKey(key))
            {
                problem.Values[key] = value[value.Length - 1 - i] == '0' ? 0 : 1;
            }
        }
    }

    private static int ComputeValue(Problem problem, string key, HashSet<string> visited = null)
    {
        if (problem.Values.TryGetValue(key, out var value))
        {
            return value;
        }

        if (!problem.Gates.TryGetValue(key, out var gate))
        {
            return 0;
        }

        visited ??= new HashSet<string>();
        if (!visited.Add(key))
        {
            throw new InvalidOperationException($"Cycle detected on wire '{key}'.");
        }

        var inputA = ComputeValue(problem, gate.InputA, visited);
        var inputB = ComputeValue(problem, gate.InputB, visited);

        var output = gate.Operation switch
        {
            "AND" => inputA & inputB,
            "OR" => inputA | inputB,
            "XOR" => inputA ^ inputB,
            _ => throw new InvalidOperationException($"Unknown operation {gate.Operation}")
        };

        problem.Values[key] = output;
        return output;
    }

    private static Problem ParseInput(string file)
    {
        using var sr = new StreamReader(file);

        var valueRegex = new Regex(@"^(?<key>\w+):\s*(?<val>\d+)$");
        var operationRegex = new Regex(@"^(?<inA>\w+)\s+(?<op>AND|XOR|OR)\s+(?<inB>\w+)\s+->\s+(?<out>\w+)$");

        var values = new Dictionary<string, int>();
        int maxIndex = 0;

        string? line;
        while (!string.IsNullOrEmpty(line = sr.ReadLine()))
        {
            var match = valueRegex.Match(line);
            if (!match.Success)
            {
                break;
            }

            var key = match.Groups["key"].Value;
            var val = int.Parse(match.Groups["val"].Value);

            values[key] = val;

            if (key.StartsWith("z", StringComparison.OrdinalIgnoreCase) && int.TryParse(key.Substring(1), out var num))
            {
                if (num > maxIndex) maxIndex = num;
            }
        }

        var gates = new Dictionary<string, Gate>();
        while (!sr.EndOfStream && (line = sr.ReadLine()) != null)
        {
            var match = operationRegex.Match(line);
            if (!match.Success) continue;

            var inA = match.Groups["inA"].Value;
            var op = match.Groups["op"].Value;
            var inB = match.Groups["inB"].Value;
            var outW = match.Groups["out"].Value;

            gates[outW] = new Gate(inA, inB, op);

            if (outW.StartsWith("z", StringComparison.OrdinalIgnoreCase) && int.TryParse(outW[1..], out var num))
            {
                if (num > maxIndex) maxIndex = num;
            }
        }

        return new Problem(values, gates, maxIndex + 1);
    }
}