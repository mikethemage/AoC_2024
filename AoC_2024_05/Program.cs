namespace AoC_2024_05;

internal class Program
{
    static void Main(string[] args)
    {
        //string filename = "sample.txt";
        string filename = "input.txt";
        string[] lines = File.ReadAllLines(filename);

        bool switchMode=false;
        List<(int, int)> pageOrderingRules = new();
        List<List<int>> pagesToProduce = new();

        foreach (string line in lines)
        {
            if(string.IsNullOrEmpty(line))
            {
                switchMode = true;
            }
            else if (switchMode)
            {
                var tokens=line.Split(',');
                pagesToProduce.Add(tokens.Select(x=>int.Parse(x)).ToList());
            }
            else
            {
                var tokens=line.Split('|');
                pageOrderingRules.Add((int.Parse(tokens[0]), int.Parse(tokens[1])));
            }
        }

        int sumOfMiddleValues = 0;
        List<List<int>> incorrectlyOrderedUpdates = new();

        foreach (var manualUpdate in pagesToProduce)
        {
            Console.WriteLine($"Considering manual update: {string.Join(',',manualUpdate)}");
            List<int> pagesProduced = new();
            bool accepted = true;
            foreach (int page in manualUpdate)
            {
                Console.WriteLine($"Page: {page}");
                IEnumerable<(int, int)> mustBeAfters = pageOrderingRules.Where(x=>x.Item1 == page);
                foreach ((int, int) mustBeAfter in mustBeAfters)
                {
                    Console.WriteLine($"Affected by rule: {mustBeAfter.Item1}|{mustBeAfter.Item2}");
                    if(pagesProduced.Contains(mustBeAfter.Item2))
                    {
                        Console.WriteLine($"Already included page: {mustBeAfter.Item2}, failed.");
                        accepted = false; 
                        break;
                    }
                }
                if (!accepted)
                {
                    incorrectlyOrderedUpdates.Add(manualUpdate);
                    break;
                }
                pagesProduced.Add(page);
            }
            if (accepted)
            {
                Console.WriteLine("Accepted");
                int valueToUse = manualUpdate[manualUpdate.Count / 2];
                Console.WriteLine($"Middle value={valueToUse}");
                sumOfMiddleValues += valueToUse;
            }
            Console.WriteLine("------------");
            Console.WriteLine();
        }

        Console.WriteLine($"Sum of middle values: {sumOfMiddleValues}");
        Console.WriteLine("");

        int totals = 0;

        foreach (List<int> incorrectlyOrderedUpdate in incorrectlyOrderedUpdates)
        {
            Console.WriteLine(string.Join(',', incorrectlyOrderedUpdate));
            TreeNode root = new TreeNode { Value = incorrectlyOrderedUpdate[0] };
            for (int i = 1; i<incorrectlyOrderedUpdate.Count; i++)
            {
                AddToTree(root, incorrectlyOrderedUpdate[i], pageOrderingRules);
            }
            List<int> sortedList = new List<int>();
            TreeToList(root, sortedList);
            Console.WriteLine(string.Join(',', sortedList));
            int middleValue = sortedList[sortedList.Count / 2];
            totals += middleValue;
            Console.WriteLine(middleValue);
        }

        Console.WriteLine($"Total: {totals}");
    }

    private static void TreeToList(TreeNode node, List<int> list)
    {
        if(node.Left!=null)
        {
            TreeToList(node.Left, list);
        }
        list.Add(node.Value);
        if (node.Right != null)
        {
            TreeToList(node.Right, list);
        }
    }

    private static void AddToTree(TreeNode node, int value, List<(int, int)> pageOrderingRules)
    {
        if (pageOrderingRules.Any(x => x.Item1 == value && x.Item2 == node.Value))
        {
            if (node.Left == null)
            {
                TreeNode newNode = new TreeNode { Value = value };
                node.Left = newNode;
            }
            else
            {
                AddToTree(node.Left, value, pageOrderingRules);
            }
        }
        else
        {
            if (node.Right == null)
            {
                TreeNode newNode = new TreeNode { Value = value };
                node.Right = newNode;
            }
            else
            {
                AddToTree(node.Right, value, pageOrderingRules);
            }
        }
    }
}

internal class TreeNode
{
    public int Value { get; set; }
    public TreeNode? Left { get; set; }
    public TreeNode? Right { get; set; }
}
