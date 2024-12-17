using System.Text;

namespace AoC_2024_17;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample2.txt";
        string filename = "input.txt";
        string[] lines = File.ReadAllLines(filename);
        List<int> program = new List<int>();
        Dictionary<char, long> registers = new Dictionary<char, long>();
        int initialB = 0;
        int initialC = 0;
        foreach (string line in lines)
        {
            if (!string.IsNullOrEmpty(line))
            {
                var tokens = line.Split(": ");
                if (tokens[0].StartsWith("Register"))
                {
                    var register = tokens[0][^1];
                    var value = int.Parse(tokens[1]);
                    registers[register] = value;
                    if (register == 'B')
                    {
                        initialB = value;
                    }
                    if (register == 'C')
                    {
                        initialC = value;
                    }
                }
                else if (tokens[0] == "Program")
                {
                    program = tokens[1].Split(",").Select(int.Parse).ToList();
                }
            }
        }

        int intstructionPointer = 0;
        List<long> output = new List<long>();

        while (intstructionPointer < program.Count - 1)
        {
            intstructionPointer = RunInstruction(program[intstructionPointer], program[intstructionPointer + 1], registers, intstructionPointer, output);
        }

        Console.WriteLine($"Part One: {string.Join(",", output)}");




        long initialA = 0;
        var goal = string.Join(",", program);
        string result = "";

        List<string> binaryDigits = program.Select(x=>"000").ToList();
        


        while (result!=goal)
        {

            initialA = GetInitialA(binaryDigits);            

            intstructionPointer = 0;
            output = new List<long>();

            registers['A'] = initialA;
            registers['B'] = initialB;
            registers['C'] = initialC;

            //int outputPointer = -1;

            while (intstructionPointer < program.Count - 1)
            {
                try
                {
                    intstructionPointer = RunInstruction(program[intstructionPointer], program[intstructionPointer + 1], registers, intstructionPointer, output);
                }
                catch
                {
                    break;
                }
                

                //if(output.Count > outputPointer+1)
                //{
                //    outputPointer++;
                //    if (outputPointer >= program.Count )//|| output[outputPointer] != program[outputPointer])
                //    {
                //        break;
                //    }
                //}
            }

            //if(output.Count>maxValid)
            //{
            //    maxValid = output.Count-1;
            //}

            if(output.Count == program.Count)
            {
                var aaa = Convert.ToString(initialA, 2);
                var zeroes = 3 - (aaa.Length % 3);
                if (zeroes < 3)
                {
                    aaa = string.Join("", Enumerable.Repeat("0", zeroes)) + aaa;
                }

                Console.WriteLine($"A: {initialA}, Binary: {aaa}, Output: {string.Join(", ", output)}");
            }
            else
            {
                Console.WriteLine($"A: {initialA}, Length: {output.Count}");
            }

            if(output.Count < program.Count)
            {
                IncrementBinaryDigit(binaryDigits, binaryDigits.Count-1);
            }
            else
            {
                for (int i = program.Count - 1; i >= 0; i--)
                {
                    if(program[i] != output[i])
                    {
                        IncrementBinaryDigit(binaryDigits, i);
                        break;
                    }
                }
            }
            
            result = string.Join(",", output);
        } 

        Console.WriteLine($"Part Two: {initialA}");
    }

    private static void IncrementBinaryDigit(List<string> binaryDigits, int i)
    {
        string oldDigit = binaryDigits[i];
        int oldValue = Convert.ToInt32(oldDigit,2);
        int newValue = oldValue + 1;
        string newDigit = Convert.ToString(newValue, 2);
        if(newDigit.Length>3)
        {
            binaryDigits[i] = "000";
            IncrementBinaryDigit(binaryDigits, i+1);
        }
        else
        {
            binaryDigits[i] = string.Join("", Enumerable.Repeat("0", 3 - newDigit.Length)) + newDigit;
        }
       
       
    }

    private static long GetInitialA(List<string> binaryDigits)
    {
        StringBuilder sb = new();
        for (int i = binaryDigits.Count - 1; i >= 0; i--)
        {
            sb.Append(binaryDigits[i]);
        }
        return Convert.ToInt64(sb.ToString(), 2);
    }

    private static int RunInstruction(int opcode, int operand, Dictionary<char, long> registers, int instructionPointer, List<long> output)
    {
        switch (opcode)
        {
            case 0:
                registers['A'] = registers['A'] / (int)Math.Pow(2, GetCombo(operand, registers));
                break;

            case 1:
                registers['B'] = registers['B'] ^ operand;
                break;

            case 2:
                registers['B'] = GetCombo(operand, registers) % 8;
                break;

            case 3:
                if (registers['A'] != 0)
                {
                    return operand;
                }
                break;

            case 4:
                registers['B'] = registers['B'] ^ registers['C'];
                break;

            case 5:
                output.Add(GetCombo(operand, registers) % 8);
                break;

            case 6:
                registers['B'] = registers['A'] / (int)Math.Pow(2, GetCombo(operand, registers));
                break;

            case 7:
                registers['C'] = registers['A'] / (int)Math.Pow(2, GetCombo(operand, registers));
                break;

            default:
                throw new Exception("Invalid Instruction!");
        }

        return instructionPointer + 2;
    }

    private static long GetCombo(int operand, Dictionary<char, long> registers)
    {
        if (operand <= 3)
        {
            return operand;
        }
        if (operand == 4)
        {
            return registers['A'];
        }
        if (operand == 5)
        {
            return registers['B'];
        }
        if (operand == 6)
        {
            return registers['C'];
        }
        throw new Exception("Invalid operand!");
    }
}