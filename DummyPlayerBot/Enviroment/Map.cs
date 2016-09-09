using System.Collections.Generic;
using System.Linq;
using DLibrary.Graph;
using DummyPlayer;
using DummyPlayerBot.Enviroment.CostExtractors;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayerBot.Enviroment
{
    public class Map
    {
        private Dictionary<Location, Node> nodes;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map()
        {
            nodes = new Dictionary<Location, Node>();
            Width = Height = 0;
        }

        public void Update(Enviroment enviroment)
        {
            CheckResizeMap(enviroment);
            foreach (var node in nodes)
            {
                node.Value.CellType = enviroment.View.Field[node.Key];
                node.Value.IsEnemy = false;
            }
            foreach (var monster in enviroment.View.Monsters)
            {
                nodes[monster.Location].IsEnemy = true;
            }
        }

        public Location? FindNearest(Location centre, CellType cellType, int radius)
        {
            for (int x = centre.X - radius; x < centre.X + radius; x++)
            {
                for (int y = centre.Y - radius; y < centre.Y + radius; y++)
                {
                    var location = new Location(x, y);
                    if (nodes.ContainsKey(location))
                    {
                        var node = nodes[location];
                        if (node.CellType == cellType)
                            return node.Location;
                    }
                }
            }
            return null;
        }

        public IEnumerable<Location> FindPath(Location start, Location end, IPathFinder finder, ICostExtractor costExtractor, bool ignoreTrap = true, bool ignoreEnemy = true)
        {
            foreach (var node in nodes)
            {
                node.Value.Cost = costExtractor.GetCost(node.Key);
                node.Value.CanStay = (!ignoreTrap || node.Value.CellType != CellType.Trap) &&
                                     (!ignoreEnemy || !node.Value.IsEnemy) &&
                                     node.Value.CellType != CellType.Wall;
            }
            finder.Start = nodes[start];
            finder.End = nodes[end];
            var path = finder.FindPath()?.ToList() ?? new List<INode>();
            return path.Select(n => (n as Node).Location);
        }

        private void CheckResizeMap(Enviroment enviroment)
        {
            if (enviroment.View.Field.Width != Width || enviroment.View.Field.Height != Height)
            {
                Width = enviroment.View.Field.Width;
                Height = enviroment.View.Field.Height;
                nodes.Clear();
                nodes = new Dictionary<Location, Node>(Width*Height);
                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        var location = new Location(x, y);
                        nodes.Add(location, new Node(location));
                    }
                }
                foreach (var node in nodes)
                {
                    node.Value.ConnectMany(GetNearest(node.Key));
                }
            }
        }

        private IEnumerable<Node> GetNearest(Location loc)
        {
            var near = new[] {loc.Right(), loc.Up(), loc.Left(), loc.Down()};
            var onMap = near.Where(l => l.InRect(new Location(0, 0), new Location(Width - 1, Height - 1)));
            return onMap.Select(l => nodes[l]);
        }
    }

    //TODO: refactor
    internal class Node : INode
    {
        public Location Location;
        private List<Edge> edges { get; }
        internal CellType CellType { get; set; }
        internal bool IsEnemy { get; set; }
        public Node(Location location)
        {
            Location = location;
            edges = new List<Edge>();
        }

        public void Connect(Node other)
        {
            edges.Add(new Edge(this, other));
        }

        public void ConnectMany(IEnumerable<Node> others)
        {
            edges.AddRange(others.Select(n => new Edge(this, n)));
        }

        public UDouble Cost { get; internal set; }
        public bool CanStay { get; internal set; }

        public IEnumerable<IEdge> OutcomingEdges => edges.Where(n => (n.To as Node)?.CanStay ?? false);
    }

    internal class Edge : IPositiveWeightedEdge
    {
        public Edge(Node from, Node to)
        {
            From = from;
            To = to;
        }

        public INode From { get; }
        public INode To { get; }
        public UDouble Weight => ((To as Node)?.Cost ?? UDouble.MaxValue) + UDouble.One;
    }
}