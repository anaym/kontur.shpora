using System.Collections.Generic;
using System.Linq;

namespace DLibrary.Graph
{
    class DijkstraData<TNode>
        where TNode : Node
    {
        public DijkstraData(Node from, double cost)
        {
            From = from;
            Cost = cost;
            Oppened = false;
        }

        public Node From;
        public double Cost;
        public bool Oppened;
    }

    public static class BestPathExtention
    {
        public static List<TNode> BestPathWithPositives<TNode>(this TNode start, TNode end)
            where TNode : Node
        {
            var data = new Dictionary<TNode, DijkstraData<TNode> { { start, new DijkstraData<TNode>(null, 0) } };
            while (true)
            {
                var now = data.Where(n => !n.Value.Oppened).OrderBy(n => n.Value.Cost).FirstOr(new KeyValuePair<TNode, DijkstraData<TNode>(null, null)).Key;
                if (now == null)
                    break;
                foreach (var edge in now.OutcomingEdges)
                {
                    var newCost = edge.Cost + data[now].Cost;
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
            if (end == null || !data.ContainsKey(end)) return null;
            var path = new List<TNode> { end };
            var position = data[end];
            while (position.From != null)
            {
                path.Add(position.From);
                position = data[position.From];
            }
            path.Reverse();
            return path;
        }
    }
}