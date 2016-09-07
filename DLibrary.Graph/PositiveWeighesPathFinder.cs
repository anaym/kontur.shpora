using System;
using System.Collections.Generic;
using System.Linq;

namespace DLibrary.Graph
{
    public class PositiveWeighesPathFinder : IPathFinder
    {
        public PositiveWeighesPathFinder()
        {
            Start = End = null;
        }

        public PositiveWeighesPathFinder(INode start, INode end)
        {
            Start = start;
            End = end;
        }

        public INode Start { get; set; }
        public INode End { get; set; }
        
        public IEnumerable<INode> FindPath()
        {
            var data = new Dictionary<INode, DijkstraData> { { Start, new DijkstraData(null, UDouble.Zero) } };
            while (true)
            {
                var now = data.Where(n => !n.Value.Oppened).OrderBy(n => n.Value.Cost).FirstOr(new KeyValuePair<INode, DijkstraData>(null, null)).Key;
                if (now == null)
                    break;
                foreach (var e in now.OutcomingEdges)
                {
                    var edge = e as IPositiveWeightedEdge;
                    if (edge == null)
                        throw new TypeAccessException("Bad edge type, expected implemention of IPositiveWeightedEdge");
                    var newCost = edge.Weight + data[now].Cost;
                    if (!data.ContainsKey(edge.To))
                    {
                        data.Add(edge.To, new DijkstraData(edge.From, newCost));
                    }
                    else
                    {
                        var nodeData = data[edge.To];
                        if (nodeData.Cost > newCost)
                        {
                            nodeData.Cost = newCost;
                            nodeData.From = edge.From;
                        }
                    }
                }
                data[now].Oppened = true;
            }
            if (End == null || !data.ContainsKey(End)) return null;
            var path = new List<INode> { End };
            var position = data[End];
            while (position.From != null)
            {
                path.Add(position.From);
                position = data[position.From];
            }
            path.Reverse();
            return path;
        }

        private class DijkstraData
        {
            public DijkstraData(INode from, UDouble cost)
            {
                From = from;
                Cost = cost;
                Oppened = false;
            }

            public INode From;
            public UDouble Cost;
            public bool Oppened;
        }
    }
}