using System.Collections.Concurrent;

namespace AoC_2024_21;

class Program
{
    static void Main()
    {
        List<string> input = File.ReadAllLines("sample.txt").ToList();
        //List<string> input = File.ReadAllLines("input.txt").ToList();

        Console.WriteLine(Solve(input, 2));
        Console.WriteLine(Solve(input, 25));
    }

    private static long Solve(List<string> input, int depth)
    {
        var keypad1 = ParseKeypad(["789", "456", "123", " 0A"]);
        var keypad2 = ParseKeypad([" ^A", "<v>"]);
        var keypads = Enumerable.Repeat(keypad2, depth).Prepend(keypad1).ToArray();

        var cache = new ConcurrentDictionary<(char currentKey, char nextKey, int depth), long>();
        var res = 0L;

        foreach (var line in input)
        {
            var num = int.Parse(line[..^1]);
            res += num * EncodeKeys(line, keypads, cache);
        }
        return res;
    }

    // Determines the length of the shortest sequence that is needed to enter the given 
    // keys. An empty keypad array means that the sequence is simply entered by a human 
    // and no further encoding is needed. Otherwise the sequence is entered by a robot
    // which needs to be programmed. In practice this means that the keys are encoded 
    // using the robots keypad (the first keypad), generating an other sequence of keys.
    // This other sequence is then recursively encoded using the rest of the keypads.
    private static long EncodeKeys(string keys, Dictionary<Vec2, char>[] keypads, ConcurrentDictionary<(char currentKey, char nextKey, int depth), long> cache)
    {
        if (keypads.Length == 0)
        {
            return keys.Length;
        }
        else
        {
            // invariant: the robot starts and finishes by pointing at the 'A' key
            var currentKey = 'A';
            var length = 0L;

            foreach (var nextKey in keys)
            {
                length += EncodeKey(currentKey, nextKey, keypads, cache);
                // while the sequence is entered the current key changes accordingly
                currentKey = nextKey;
            }

            // at the end the current key should be reset to 'A'
            if (currentKey != 'A')
            {
                Console.WriteLine("The robot should point at the 'A' key");
            }                

            return length;
        }
    }

    private static long EncodeKey(char currentKey, char nextKey, Dictionary<Vec2, char>[] keypads, ConcurrentDictionary<(char currentKey, char nextKey, int depth), long> cache)
    {
        return cache.GetOrAdd(
            (currentKey, nextKey, keypads.Length), _ =>
            {
                var keypad = keypads[0];

                var currentPos = keypad.Single(kvp => kvp.Value == currentKey).Key;
                var nextPos = keypad.Single(kvp => kvp.Value == nextKey).Key;

                var dy = nextPos.y - currentPos.y;
                var vert = new string(dy < 0 ? 'v' : '^', Math.Abs(dy));

                var dx = nextPos.x - currentPos.x;
                var horiz = new string(dx < 0 ? '<' : '>', Math.Abs(dx));

                var cost = long.MaxValue;
                // we can usually go vertical first then horizontal or vica versa,
                // but we should check for the extra condition and don't position
                // the robot over the ' ' key:
                if (keypad[new Vec2(currentPos.x, nextPos.y)] != ' ')
                {
                    cost = Math.Min(cost, EncodeKeys($"{vert}{horiz}A", keypads[1..], cache));
                }

                if (keypad[new Vec2(nextPos.x, currentPos.y)] != ' ')
                {
                    cost = Math.Min(cost, EncodeKeys($"{horiz}{vert}A", keypads[1..], cache));
                }
                return cost;
            }
        );
    }

    private static Dictionary<Vec2, char> ParseKeypad(List<string> lines)
    {
        return (
            from y in Enumerable.Range(0, lines.Count)
            from x in Enumerable.Range(0, lines[0].Length)
            select new KeyValuePair<Vec2, char>(new Vec2(x, -y), lines[y][x])
        ).ToDictionary();
    }
}

record struct Vec2(int x, int y);