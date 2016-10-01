using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DummyPlayerBot;
using SpurRoguelike.Core;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.Extension
{
    public static class FieldViewExtension
    {
        public static IEnumerable<IEnumerable<CellType>> RowAccross(this FieldView fields, Location leftTop, Location rightBottom)
        {
            return leftTop.RowRectangleAcross(rightBottom).ToCellsTypes(fields);
        }

        public static IEnumerable<IEnumerable<CellType>> RowAccross(this FieldView fields, Location centre, int widthRadius, int heightRadius)
        {
            return centre.RowRectangleAcross(widthRadius, heightRadius).ToCellsTypes(fields);
        }

        public static IEnumerable<IEnumerable<CellType>> RowAccross(this FieldView fields, Location centre, int radius)
        {
            return centre.RowRectangleAcross(radius).ToCellsTypes(fields);
        }

        public static IEnumerable<IEnumerable<CellType>> ToCellsTypes(this IEnumerable<IEnumerable<Location>> locations, FieldView fields)
        {
            return locations.Select(r => r.Select(l => fields[l]));
        }
    }
}
