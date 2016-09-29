using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace DummyPlayerBot
{
    public static class LinqExtention
    {
        public static T FirstOr<T>(this IEnumerable<T> enumerable, T def)
        {
            var enumerator = enumerable?.GetEnumerator();
            var exist = enumerator?.MoveNext() ?? false;
            return exist ? enumerator.Current : def;
        }

        public static long MaxOr<T>(this IEnumerable<T> seq, Func<T, long> numExtractor, long def = 0)
        {
            long max = long.MinValue;
            bool ex = false;
            foreach (var item in seq)
            {
                var n = numExtractor(item);
                if (n > max)
                {
                    ex = true;
                    max = n;
                }
            }
            if (!ex)
                return def;
            return max;
        }

        public static IEnumerable<T[]> NGramm<T>(this IEnumerable<T> en, int n)
        {
            var queue = new Queue<T>(n);
            foreach (var item in en)
            {
                queue.Enqueue(item);
                if (queue.Count == n)
                {
                    yield return queue.ToArray();
                    queue.Dequeue();
                }
            }
        }

        public static int MinOr<T>(this IEnumerable<T> en, Func<T, int> selector, int def)
        {
            var min = int.MaxValue;
            var notEmpty = false;
            foreach (var item in en)
            {
                notEmpty = true;
                min = Math.Min(selector(item), min);
            }
            return notEmpty ? min : def;
        }
    }
}