using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackshotsEsolang.Lexing
{
    public enum AllowedChar
    {
        Error,
        N, M, G, H
    }
    public struct Cluster(AllowedChar ch, int n = 1)
    {
        public readonly AllowedChar Character = ch;
        public int Quantity = n;

        public Cluster Add(int n = 1)
        {
            // Console.Write($"cluster {Character} n before = {Quantity}, ");
            if (Character != AllowedChar.Error) Quantity += n;
            // Console.WriteLine($"n after = {Quantity} ");
            return this;
        }

        public readonly bool Is(AllowedChar ch, int n)
        {
            return n == Quantity && ch == Character;
        }
        public readonly bool Is(Cluster other)
        {
            return other.Quantity == Quantity && other.Character == Character;
        }
    }
    public readonly struct Token(Cluster[] clusters, uint line)
    {
        public readonly List<Cluster> Clusters = [.. clusters];
        public readonly uint line = line;

        public override string ToString()
        {
            StringBuilder sb = new ();

            foreach (Cluster cluster in Clusters)
            {
                sb.Append($"[ {cluster.Character}, {cluster.Quantity} ]");
            }
            sb.Append(" at line " + line);

            return sb.ToString();
        }
        public readonly void Add(Cluster cluster)
        {
            Clusters.Add(cluster);
        }
        public void AddChar(AllowedChar ch)
        {
            // Console.WriteLine($"Add {ch} to Token");
            // Console.WriteLine($"\t before: {Utils.ArrayToString([this], "")}");
            if (Clusters.Count != 0 && ch == Clusters[^1].Character)
            {
                Clusters[^1] = Clusters[^1].Add();
            }
            else
            {
                Add(new Cluster(ch));
            }
            // Console.WriteLine($"\t after: {Utils.ArrayToString([this], "")}");
            
        }
    }
}
