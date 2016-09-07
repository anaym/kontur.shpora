using DLibrary.Graph;

namespace DummyPlayer
{
    public class MapEdge : IPositiveWeightedEdge
    {
        public MapEdge(MapNode from, MapNode to, UDouble weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }

        public INode From { get; }
        public INode To { get; }
        public UDouble Weight { get; }
    }
}