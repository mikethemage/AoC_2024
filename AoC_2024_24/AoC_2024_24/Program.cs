namespace AoC_2024_24;

public class Program
{
    static void Main(string[] args)
    {
        const string filename = "input.txt";
        //const string filename = "sample.txt";

        var lines = File.ReadAllLines(filename);

        Dictionary<string, Wire> wires = new Dictionary<string, Wire>();
        List<Gate> gates = new List<Gate>();
        HashSet<string> requiredWireNames = new HashSet<string>();

        bool processGates = false;
        foreach (var line in lines)
        {
            if (processGates)
            {
                Gate newGate = CreateGateFromLine(line);
                requiredWireNames.Add(newGate.Input1);
                requiredWireNames.Add(newGate.Input2);
                requiredWireNames.Add(newGate.Output);
                gates.Add(newGate);
            }
            else if (string.IsNullOrEmpty(line))
            {
                processGates = true;
            }
            else
            {
                Wire newWire = CreateWireFromLine(line);
                wires.Add(newWire.Name, newWire);
            }
        }

        foreach (string requiredWireName in requiredWireNames)
        {
            if (!wires.ContainsKey(requiredWireName))
            {
                wires.Add(requiredWireName, new Wire(requiredWireName));
            }
        }

        ProcessAll(wires, gates);
        List<string> outputNames = wires.Keys.Where(x => x.StartsWith("z")).Order().ToList();
        long accumulator = GetAccumulator(wires, outputNames);

        Console.WriteLine($"Final value from data: {accumulator}");

        //Part 2:
        List<string> inputXNames = wires.Keys.Where(x => x.StartsWith("x")).Order().ToList();
        List<string> inputYNames = wires.Keys.Where(x => x.StartsWith("y")).Order().ToList();

        Console.WriteLine($"Number of X inputs: {inputXNames.Count}, Number of Y inputs: {inputYNames.Count}, Number of Outputs: {outputNames.Count}");
        if (inputXNames.Count != inputYNames.Count)
        {
            Console.WriteLine("Input count mismatch!");
        }
        if (outputNames.Count != inputXNames.Count + 1)
        {
            Console.WriteLine("Invalid number of outputs!");
        }

        Dictionary<string, Wire> correctWires = new Dictionary<string, Wire>();
        foreach (string wireName in inputXNames)
        {
            correctWires.Add(wireName, wires[wireName]);
        }

        foreach (string wireName in inputYNames)
        {
            correctWires.Add(wireName, wires[wireName]);
        }

        foreach (string wireName in outputNames)
        {
            correctWires.Add(wireName, new Wire(wireName));
        }



        List<Gate> correctGates = new List<Gate>();

        string carryName = "HalfAdder0Carry";
        for (int i = 0; i < inputXNames.Count; i++)
        {
            if (i == 0)
            {
                HalfAdder halfAdder = new HalfAdder("HalfAdder0", "x00", "y00", "z00", carryName);
                correctGates.AddRange(halfAdder.InternalGates);
                correctWires.Add(carryName, new Wire(carryName));
            }
            else
            {
                string nextCarryName;
                if (i == inputXNames.Count - 1)
                {
                    nextCarryName = $"z{i + 1:D2}";
                }
                else
                {
                    nextCarryName = $"FullAdderCarry{i:D2}";
                    correctWires.Add(nextCarryName, new Wire(nextCarryName));
                }
                FullAdder fullAdder = new FullAdder($"FullAdder{i:D2}", $"x{i:D2}", $"y{i:D2}", carryName, $"z{i:D2}", nextCarryName);
                carryName = nextCarryName;
                correctGates.AddRange(fullAdder.InternalGates);
                foreach (Wire internalWire in fullAdder.InternalWires)
                {
                    correctWires.Add(internalWire.Name, internalWire);
                }
            }
        }

        ProcessAll(correctWires, correctGates);

        long correctAccumulator = GetAccumulator(correctWires, outputNames);

        Console.WriteLine($"Final corrected value: {correctAccumulator}");        


    }

    private static long GetAccumulator(Dictionary<string, Wire> wires, List<string> outputNames)
    {
        long multiplier = 1;
        long accumulator = 0;

        foreach (string wireName in outputNames)
        {
            Console.WriteLine($"{wireName}: {wires[wireName].Value}");
            accumulator += (long)wires[wireName].Value! * multiplier;
            multiplier *= 2;
        }

        return accumulator;
    }

    private static void ProcessAll(Dictionary<string, Wire> wires, List<Gate> gates)
    {
        List<Gate> unprocessedGates = new List<Gate>(gates);
        List<Gate> canBeProcessed = new List<Gate>();

        while (unprocessedGates.Count > 0)
        {
            if (canBeProcessed.Count == 0)
            {
                canBeProcessed = unprocessedGates.Where(g => wires[g.Input1].Value != null && wires[g.Input2].Value != null).ToList();
            }
            Gate? nextGate = canBeProcessed.FirstOrDefault();
            if (nextGate == null)
            {
                throw new Exception("This should never happen!");
            }
            canBeProcessed.Remove(nextGate);
            unprocessedGates.Remove(nextGate);
            ProcessGate(nextGate, wires);

        }
    }

    static void ProcessGate(Gate gate, Dictionary<string, Wire> wires)
    {
        Wire input1 = wires[gate.Input1];
        Wire input2 = wires[gate.Input2];
        if (input1.Value is null)
        {
            throw new Exception($"Invald input wire state on wire: {input1.Name}");
        }
        if (input2.Value is null)
        {
            throw new Exception($"Invald input wire state on wire: {input2.Name}");
        }

        int? outputValue = null;
        switch (gate.GateType)
        {
            case "AND":
                if (input1.Value == 1 && input2.Value == 1)
                {
                    outputValue = 1;
                }
                else
                {
                    outputValue = 0;
                }
                break;

            case "OR":
                if (input1.Value == 1 || input2.Value == 1)
                {
                    outputValue = 1;
                }
                else
                {
                    outputValue = 0;
                }
                break;

            case "XOR":
                if (input1.Value != input2.Value)
                {
                    outputValue = 1;
                }
                else
                {
                    outputValue = 0;
                }
                break;

            default:
                throw new Exception($"Invalid gate type:{gate.GateType}");
        }

        Wire output = wires[gate.Output];
        output.Value = outputValue;
    }

    static Wire CreateWireFromLine(string line)
    {
        string[] tokens = line.Split(": ");
        string name = tokens[0].Trim();
        int initialValue = int.Parse(tokens[1].Trim());
        return new Wire(name, initialValue);
    }

    static Gate CreateGateFromLine(string line)
    {
        string[] tokens = line.Split(" -> ");
        string output = tokens[1].Trim();
        string[] subTokens = tokens[0].Split(" ");
        string input1 = subTokens[0].Trim();
        string gateType = subTokens[1].Trim();
        string input2 = subTokens[2].Trim();
        return new Gate { GateType = gateType, Input1 = input1, Input2 = input2, Output = output };
    }
}

public class Wire
{
    public string Name { get; set; }
    public int? Value { get; set; }
    private int? _initialValue;

    public Wire(string name, int initialValue)
    {
        Name = name;
        _initialValue = initialValue;
        Value = initialValue;
    }

    public Wire(string name)
    {
        Name = name;
        _initialValue = null;
        Value = null;
    }

    public void Reset()
    {
        Value = _initialValue;
    }
}

public class Gate
{
    public required string Input1 { get; set; }
    public required string Input2 { get; set; }
    public required string GateType { get; init; }
    public required string Output { get; set; }
}

public class HalfAdder
{
    public string Name { get; init; }
    public string Input1 { get; init; }
    public string Input2 { get; init; }
    public string Output { get; init; }
    public string Carry { get; init; }

    private Gate _xorGate;
    private Gate _andGate;

    public List<Gate> InternalGates
    {
        get
        {
            List<Gate> internalGates = new List<Gate>();
            internalGates.Add(_andGate);
            internalGates.Add(_xorGate);
            return internalGates;
        }
    }

    public HalfAdder(string name, string input1, string input2, string output, string carry)
    {
        Name = name;
        Input1 = input1;
        Input2 = input2;
        Output = output;
        Carry = carry;
        _xorGate = new Gate { GateType = "XOR", Input1 = input1, Input2 = input2, Output = output };
        _andGate = new Gate { GateType = "AND", Input1 = input1, Input2 = input2, Output = carry };
    }
}

public class FullAdder
{
    public string Name { get; init; }
    public string Input1 { get; init; }
    public string Input2 { get; init; }
    public string CarryInput { get; init; }
    public string Output { get; init; }
    public string CarryOutput { get; init; }

    public List<Wire> InternalWires { get; private set; } = new List<Wire>();

    private HalfAdder _halfAdder1;
    private HalfAdder _halfAdder2;
    private Gate _orGate;

    public List<Gate> InternalGates
    {
        get
        {
            List<Gate> internalGates = new List<Gate>();

            internalGates.Add(_orGate);
            internalGates.AddRange(_halfAdder1.InternalGates);
            internalGates.AddRange(_halfAdder2.InternalGates);

            return internalGates;
        }
    }

    public FullAdder(string name, string input1, string input2, string carryInput, string output, string carryOutput)
    {
        Name = name;
        Input1 = input1;
        Input2 = input2;
        CarryInput = carryInput;
        Output = output;
        CarryOutput = carryOutput;

        InternalWires.Add(new Wire(name + "InternalSum1"));
        InternalWires.Add(new Wire(name + "InternalCarry1"));
        InternalWires.Add(new Wire(name + "InternalCarry2"));

        _halfAdder1 = new HalfAdder(name + "HalfAdder1", input1, input2, name + "InternalSum1", name + "InternalCarry1");
        _halfAdder2 = new HalfAdder(name + "HalfAdder2", name + "InternalSum1", carryInput, output, name + "InternalCarry2");
        _orGate = new Gate { GateType = "OR", Input1 = name + "InternalCarry1", Input2 = name + "InternalCarry2", Output = carryOutput };
    }
}


