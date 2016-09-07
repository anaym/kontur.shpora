using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DLibrary.Graph;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayer
{
    //TODO: fix Equals
    public class MapNode : Node
    {
        public readonly Location Location;
        public Map Map;

        public MapNode(Location location, double cost, Map map)
        {
            Location = location;
            Map = map;
            Cost = cost;
        }

        public override IEnumerable<IEdge> OutcomingEdges => Map.GetNearly(Location).Select(to => new MapEdge(this, to));

        public double Cost;

        public override bool Equals(Node other) => Equals((object) other);

        public override int GetHashCode() => Location.GetHashCode();

    }

    public class MapEdge : IEdge
    {
        public MapEdge(MapNode from, MapNode to)
        {
            From = from;
            To = to;
        }

        public Node From { get; }
        public Node To { get; }
        public double Cost => (To as MapNode)?.Cost ?? -1;
    }
}