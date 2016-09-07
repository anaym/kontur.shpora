using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace DLibrary.Graph
{
    public static class LinqExtention
    {
        public static T FirstOr<T>(this IEnumerable<T> enumerable, T def)
        {
            var enumerator = enumerable?.GetEnumerator();
            var exist = enumerator?.MoveNext() ?? false;
            return exist ? enumerator.Current : def;

        }
    }
}