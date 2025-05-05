
using System.Linq;

namespace AoC_2024_23;

public class Program
{
    static void Main(string[] args)
    {
        //const string filename = "sample.txt";
        const string filename = "input.txt";

        string[] lines = File.ReadAllLines(filename);
        Dictionary<string, Pc> Pcs = ExtractData(lines);

        //Part 1:
        //HashSet<HashSet<Pc>> linkedGroups = new HashSet<HashSet<Pc>>();
        //foreach (Pc pc in Pcs.Values)
        //{
        //    foreach (var linkedPc in pc.LinkedPcs)
        //    {
        //        var intersection = pc.LinkedPcs.Intersect(linkedPc.LinkedPcs).ToHashSet();
        //        if (intersection.Count>0)
        //        {
        //            foreach (var thirdPc in intersection)
        //            {
        //                HashSet<Pc> linkedGroup = new HashSet<Pc>();
        //                linkedGroup.Add(pc);
        //                linkedGroup.Add(linkedPc);
        //                linkedGroup.Add(thirdPc);

        //                if(!linkedGroups.Any(x=>x.SetEquals(linkedGroup)))
        //                {
        //                    linkedGroups.Add(linkedGroup);
        //                }                        
        //            }                    
        //        }
        //    }
        //}

        //var GroupsWithTList = linkedGroups.Where(x => x.Any(y => y.Name.StartsWith("t"))).ToList();

        //foreach (var g in GroupsWithTList)
        //{
        //    Console.WriteLine(string.Join(',',g.Select(x=>x.Name)));
        //}

        //Console.WriteLine($"Number that start with 't': {GroupsWithTList.Count}");


        var cliques = new List<HashSet<Pc>>();
        BronKerbosch(new HashSet<Pc>(), new HashSet<Pc>(Pcs.Values), new HashSet<Pc>(), Pcs, cliques);

        var result = cliques.OrderByDescending(x => x.Count()).FirstOrDefault();

        if (result != null)
        {
            Console.WriteLine(string.Join(',',result.OrderBy(x=>x.Name).Select(x=>x.Name)));
        }

    }

    private static void BronKerbosch(HashSet<Pc> R, HashSet<Pc> P, HashSet<Pc> X, Dictionary<string, Pc> graph, List<HashSet<Pc>> cliques)
    {
        if (P.Count == 0 && X.Count == 0)
        {
            cliques.Add(new HashSet<Pc>(R));
            return;
        }

        foreach (var v in P.ToList())
        {
            var neighbors = graph[v.Name].LinkedPcs;
            BronKerbosch(
                new HashSet<Pc>(R) { v },
                new HashSet<Pc>(P.Intersect(neighbors)),
                new HashSet<Pc>(X.Intersect(neighbors)),
                graph,
                cliques
            );

            P.Remove(v);
            X.Add(v);
        }
    }

    private static Dictionary<string, Pc> ExtractData(string[] lines)
    {
        List<(string pc1, string pc2)> Links = new List<(string pc1, string pc2)>();
        Dictionary<string, Pc> Pcs = new Dictionary<string, Pc>();
        foreach (string line in lines)
        {
            string[] tokens = line.Split("-");
            Links.Add((tokens[0], tokens[1]));
            if (!Pcs.ContainsKey(tokens[0]))
            {
                Pcs.Add(tokens[0], new Pc(tokens[0]));
            }

            if (!Pcs.ContainsKey(tokens[1]))
            {
                Pcs.Add(tokens[1], new Pc(tokens[1]));
            }
        }
        foreach (var link in Links)
        {
            Pcs[link.pc1].LinkedPcs.Add(Pcs[link.pc2]);
            Pcs[link.pc2].LinkedPcs.Add(Pcs[link.pc1]);
        }

        return Pcs;
    }
}

public class Pc
{
    public string Name { get; private set; }
    public HashSet<Pc> LinkedPcs { get; private set; } = new HashSet<Pc>();
    public Pc(string name)
    {
        Name = name;
    }
}