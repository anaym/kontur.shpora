using System;
using System.Dynamic;

namespace DLibrary.Graph
{
    public interface IPositiveWeightedEdge : IEdge
    {
        UDouble Weight { get; }
    }
}