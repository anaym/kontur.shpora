using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DLibrary.Graph;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayer
{
    //TODO: fix Equals
    public class MapNode : INode, IComparer<MapNode>
    {
        public readonly Location Location;
        public Map Map;

        public MapNode(Location location, double cost, Map map)
        {
            Location = location;
            Map = map;
            Cost = cost;
        }

        public IEnumerable<IEdge> OutcomingEdges => Map.GetNearly(Location).Select(to => new MapEdge(this, to, (UDouble)to.Cost));

        public double Cost;

        public int Compare(MapNode x, MapNode y) => 0;

        public override bool Equals(object other) => (other as MapNode)?.Location.Equals(Location) ?? false;


    }
}