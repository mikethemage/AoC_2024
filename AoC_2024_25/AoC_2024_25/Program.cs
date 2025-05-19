namespace AoC_2024_25;

internal class Program
{
    static void Main(string[] args)
    {
        //const string filename = "sample.txt";
        const string filename = "input.txt";

        List<KeyOrLock> keysAndLocks = ReadInput(filename);

        List<KeyOrLock> keys = keysAndLocks.Where(x=>x.IsKey).ToList();
        List<KeyOrLock> locks = keysAndLocks.Where(x => x.IsLock).ToList();

        int count = 0;
        foreach (var key in keys)
        {
            foreach(var @lock in locks)
            {
                if(KeyFitsLock(key, @lock))
                {
                    count++;
                }
            }
        }
        Console.WriteLine(count);

    }

    private static bool KeyFitsLock(KeyOrLock key, KeyOrLock @lock)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < key.Heights.Count; i++)
        {
            result.Add(@lock.Heights[i] + key.Heights[i]);
        }
        if (result.Any(x => x > 5))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private static List<KeyOrLock> ReadInput(string filename)
    {
        string[] lines = File.ReadAllLines(filename);
        List<KeyOrLock> keysAndLocks = new List<KeyOrLock>();
        KeyOrLock current = new KeyOrLock();
        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                keysAndLocks.Add(current);
                current = new KeyOrLock();
            }
            else
            {
                current.Lines.Add(line);
            }
        }
        if (!keysAndLocks.Contains(current))
        {
            keysAndLocks.Add(current);
        }

        return keysAndLocks;
    }
}

public class KeyOrLock
{
    public List<string> Lines { get; private set; } = new List<string>();

    private bool? _isKey;
    public bool IsKey
    {
        get
        {
            if (_isKey == null)
            {
                if (Lines[0].All(x => x == '#'))
                {
                    _isKey = true;
                }
                else
                {
                    _isKey = false;
                }
            }
            return (bool)_isKey;
        }
    }

    public bool IsLock
    {
        get
        {
            return !IsKey;
        }
    }

    private List<int>? _heights;

    public List<int> Heights
    {
        get
        {
            if (_heights == null)
            {
                _heights = new List<int>();

                for (int i = 0; i < Lines[0].Length; i++)
                {
                    _heights.Add(Lines.Select(x => x[i]).Count(y => y == '#') - 1);
                }
            }
            return _heights;
        }
    }
}
