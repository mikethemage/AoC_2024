namespace AoC_2024_22;

internal class Program
{
    static void Main(string[] args)
    {
        //const string filename = "sample.txt";
        const string filename = "input.txt";

        string[] lines = File.ReadAllLines(filename);

        //Part One:
        long total = 0;
        foreach (string line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            long secretNumber = GetNthSecretNumber(long.Parse(line), 2000);
            total += secretNumber;
            Console.WriteLine($"{line}: {secretNumber}");
        }

        Console.WriteLine($"Part One Total: {total}");


        //Part Two:
        List<Monkey> monkeys = new List<Monkey>();

        foreach (string line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            long secretNumber = long.Parse(line);

            Monkey monkey = new Monkey();
            monkeys.Add(monkey);
            monkey.SecretNumbers.Add(secretNumber);

            for (int i = 0; i < 2000; i++)
            {
                secretNumber = GetNewSecretNumber(secretNumber);
                monkey.SecretNumbers.Add(secretNumber);
            }

            monkey.CalculatePrices();
            monkey.CalculatePriceDifferences();
            monkey.CalculatePriceMap();
        }

        List<(int seq1, int seq2, int seq3, int seq4)> availableSequences = monkeys.SelectMany(x=>x.PriceMap.Keys).Distinct().ToList();
        int bestPrice = 0;
        foreach (var sequence in availableSequences)
        {
            Console.Write(sequence);
            int totalPrice = monkeys.Select(x => {
                if(x.PriceMap.TryGetValue(sequence, out int bestPrice))
                {
                    return bestPrice;
                }
                return 0;

                }).Sum();
            Console.WriteLine($" Price: {totalPrice}");

            if(sequence == (-2, 1, -1, 3))
            {
                Console.WriteLine("CHECK!");
            }

            bestPrice = Math.Max(bestPrice, totalPrice);
        }
        Console.WriteLine($"Part 2 - best price: {bestPrice}");
    }

    private static long GetNthSecretNumber(long initialNumber, int numberOfInterations)
    {
        long secretNumber = initialNumber;
        for (int i = 0; i < numberOfInterations; i++)
        {
            secretNumber = GetNewSecretNumber(secretNumber);
        }
        return secretNumber;
    }

    private static long GetNewSecretNumber(long secretNumber)
    {
        /*
        Calculate the result of multiplying the secret number by 64. 
        Then, mix this result into the secret number. 
        Finally, prune the secret number.
        */
        long result = secretNumber * 64;
        secretNumber = Mix(secretNumber, result);
        secretNumber = Prune(secretNumber);

        /*
        Calculate the result of dividing the secret number by 32. 
        Round the result down to the nearest integer. 
        Then, mix this result into the secret number. 
        Finally, prune the secret number.
        */
        result = secretNumber / 32;
        secretNumber = Mix(secretNumber, result);
        secretNumber = Prune(secretNumber);

        /*
        Calculate the result of multiplying the secret number by 2048. 
        Then, mix this result into the secret number. 
        Finally, prune the secret number.
        */
        result = secretNumber * 2048;
        secretNumber = Mix(secretNumber, result);
        return Prune(secretNumber);
    }

    private static long Prune(long secretNumber)
    {
        /*
        Calculate the value of the secret number modulo 16777216. 
        Then, the secret number becomes the result of that operation. 
        (If the secret number is 100000000 and you were to prune the secret number, the secret number would become 16113920.)
        */
        return secretNumber % 16777216;

    }

    private static long Mix(long secretNumber, long result)
    {
        /*
        Calculate the bitwise XOR of the given value and the secret number. 
        Then, the secret number becomes the result of that operation. 
        (If the secret number is 42 and you were to mix 15 into the secret number, the secret number would become 37.)
        */
        return secretNumber ^ result;
    }
}

public class Monkey
{
    public List<long> SecretNumbers { get; private set; } = new List<long>();

    public List<int> Prices { get; private set; } = new List<int>();

    public List<int> PriceDifferences { get; private set; } = new List<int>();

    public Dictionary<(int seq1, int seq2, int seq3, int seq4), int> PriceMap { get; private set; } = new Dictionary<(int seq1, int seq2, int seq3, int seq4), int>();

    public void CalculatePrices()
    {
        Prices = SecretNumbers.Select(x => (int)(x % 10)).ToList();
    }

    public void CalculatePriceDifferences()
    {
        for (int i = 1; i < Prices.Count; i++)
        {
            PriceDifferences.Add(Prices[i] - Prices[i - 1]);
        }
    }

    public void CalculatePriceMap()
    {
        for(int i = 3; i < PriceDifferences.Count; i++)
        {
            var key = (PriceDifferences[i-3], PriceDifferences[i - 2], PriceDifferences[i - 1], PriceDifferences[i]);

            if (key == (-2, 1, -1, 3))
            {
                Console.WriteLine("CHECK!");
            }
            if (!PriceMap.ContainsKey(key))
            {
                var price = Prices[i+1];
                PriceMap.Add(key, price);
            }
        }
    }
}