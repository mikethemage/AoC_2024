namespace AoC_2024_03;

internal class Program
{
    static void Main(string[] args)
    {
        string filename = "sample.txt";
        //string filename = "input.txt";
        string input = File.ReadAllText(filename);

        int position = 0;
        string lookingFor = "mul";
        string firstNumberText = string.Empty;
        string secondNumberText = string.Empty;
        int total = 0;
        while (position < input.Length)
        {
            switch (lookingFor)
            {
                case "mul":
                    if (input.Length - position > 3 && input.Substring(position, 3) == "mul")
                    {
                        position += 3;
                        lookingFor = "(";
                    }
                    else
                    {
                        position++;
                    }
                    break;

                case "(":
                    if (input[position] == '(')
                    {
                        lookingFor = "firstNumber";
                    }
                    else
                    {
                        lookingFor = "mul";
                    }
                    position++;
                    break;

                case "firstNumber":
                    if (input[position] == ',' && firstNumberText.Length > 0)
                    {
                        lookingFor = "secondNumber";
                    }
                    else if (input[position] >= '0' && input[position] <= '9')
                    {
                        firstNumberText += input[position];
                    }
                    else
                    {
                        firstNumberText=string.Empty;
                        lookingFor = "mul";
                    }
                    position++;
                    break;

                case "secondNumber":
                    if (input[position] == ')' && secondNumberText.Length > 0)
                    {
                        int result = int.Parse(firstNumberText) * int.Parse(secondNumberText);
                        total += result;

                        lookingFor = "mul";
                        firstNumberText = string.Empty;
                        secondNumberText = string.Empty;
                    }
                    else if (input[position] >= '0' && input[position] <= '9')
                    {
                        secondNumberText += input[position];
                    }
                    else
                    {
                        firstNumberText=string.Empty;
                        secondNumberText=string.Empty;
                        lookingFor = "mul";
                    }
                    position++;
                    break;

                default:
                    throw new Exception("Invalid expected value!");
            }
        }

        Console.WriteLine(total);
    }
}
