using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLibrary.Graph
{
    //TODO: проверить описания
    //TODO: вынести алгоритмы, как Extension методы?
    public abstract class Node
    {
        public abstract IEnumerable<IEdge> OutcomingEdges { get; }

        /// <summary>
        /// Algoritm Forda-Bellbana (difficult: O(NE) ~ O(N^3))
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        
        //public List<IEdge> BestPathWithNegativeCoast(Node end)
        //{
        //    var nodes = new Dictionary<Node, double> {{this, 0}};
        //    while (true)
        //    {
        //        var edges = nodes.SelectMany(n => n.Key.OutcomingEdges).ToList();
        //        foreach (var edge in edges)
        //        {
        //            if (!nodes.ContainsKey(edge.To))
        //            {
        //                nodes.Add(edge.To, nodes[edge.From] + edge.Cost);
        //            }
        //            else
        //            {
        //                var oldCost = nodes[edge.To];
        //                var newCost = nodes[edge.To]
        //            }
        //        }
        //    }

        //}
    

        //public List<Node> BestPathWithoutWeightes(Node end)
        //{
            
        //}

        public abstract bool Equals(Node other);

        public sealed override bool Equals(object obj) => ((obj as Node)?.GetHashCode() ?? 0) == GetHashCode();
        public abstract override int GetHashCode();
    }
}
