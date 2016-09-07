using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLibrary.Graph
{
    public interface INode
    {
        IEnumerable<IEdge> OutcomingEdges { get; }
    }
}
