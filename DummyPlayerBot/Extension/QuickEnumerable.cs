using System;
using System.Collections;
using System.Collections.Generic;

namespace DummyPlayerBot.Extension
{
    public class QuickEnumerable<T> : IEnumerable<T>
    {
        public readonly Func<EnumerationItem<T>> ExtractNextItem;

        public QuickEnumerable(Func<EnumerationItem<T>> extractNextItem)
        {
            ExtractNextItem = extractNextItem;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var item = ExtractNextItem();
            while (item.Exist)
            {
                yield return item.Item;
                item = ExtractNextItem();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct EnumerationItem<T>
    {
        public readonly bool Exist;
        public readonly T Item;

        public static EnumerationItem<T> Stop => new EnumerationItem<T>(false, default(T));
        public static EnumerationItem<T> Next(T item) => new EnumerationItem<T>(true, item);

        public EnumerationItem(bool exist, T item)
        {
            Exist = exist;
            Item = item;
        }
    }
}