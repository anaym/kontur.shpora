using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLibrary.Graph
{
    public interface IPathFinder
    {
        INode Start { get; set; }
        INode End { get; set; }

        IEnumerable<INode> FindPath();
    }
}
