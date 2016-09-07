using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace DLibrary.Graph
{
    public class BestPathWithNegativeCost<TNode> where TNode : Node
    {
        public BestPathWithNegativeCost(TNode start, TNode end)
        {
            Start = start;
            End = end;
        }

        public TNode Start { get; set; }
        public TNode End { get; set; }

        //public List<TNode> BestPath(int iterations)
        //{
        //    var nowGeneration = new Generation(new Dictionary<TNode, Data> { {Start, new Data(Start, 0)} });
        //    Generation newGeneration = null;
        //    for (int g = 0; g < iterations; g++)
        //    { 
        //        newGeneration = new Generation(nowGeneration);
        //        foreach (var node in nowGeneration)
        //        {
        //            foreach (var edge in node.OutcomingEdges)
        //            {
        //                var data = newGeneration[edge.To as TNode];
        //                //if (!)
        //                //{
                            
        //                //}
        //                if (data.Cost > (nowGeneration[node].Cost + edge.Cost))
        //                {
        //                    data.From = node;
        //                    data.Cost = nowGeneration[node].Cost + edge.Cost;
        //                }
        //            }
        //        }
        //    }
        //}

        class Generation : IEnumerable<TNode>
        {
            private Dictionary<TNode, Data> data;

            public Data this[TNode node] => data[node];

            public Generation(Generation other) : this(other.data)
            { }

            public Generation(Dictionary<TNode, Data> data)
            {
                this.data = new Dictionary<TNode, Data>(data.Count);
                foreach (var item in data)
                {
                    this.data.Add(item.Key, new Data(item.Value.From, item.Value.Cost));
                }
            }

            public IEnumerator<TNode> GetEnumerator() => data.Select(p => p.Key).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        struct Data
        {
            public Data(TNode from, double cost)
            {
                From = from;
                Cost = cost;
            }

            public TNode From;
            public double Cost;
        }
    }
}