using System;
using System.Collections.Generic;
using System.Linq;
using DLibrary.Graph;
using SpurRoguelike.Core.Primitives;
using LinqExtention = DummyPlayerBot.LinqExtention;

namespace DummyPlayer
{
    public class Map
    {
        private readonly long[,] weigthes;
        private readonly bool[,] travable;

        public Map(int width, int height)
        {
            weigthes = new long[width,height];
            travable = new bool[width,height];
        }

        public long GetWeight(int x, int y)
        {
            return weigthes[x, y];
        }

        public long GetWeight(Location pos) => GetWeight(pos.X, pos.Y);

        public void SetWeight(int x, int y, long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Weight must be a positive!");
            weigthes[x, y] = value;
        }

        public void SetWeight(Location pos, long value) => SetWeight(pos.X, pos.Y, value);

        public bool IsTravaible(int x, int y)
        {
            return travable[x, y];
        }

        public bool IsTravaible(Location pos) => IsTravaible(pos.X, pos.Y);

        public void SetTravaible(int x, int y, bool status)
        {
            travable[x, y] = status;
        }

        public void SetTravaible(Location pos, bool status) => SetTravaible(pos.X, pos.Y, status);

        public int Width => weigthes.GetLength(0);
        public int Height => weigthes.GetLength(1);

        public List<Location> FindPath(Location start, Location end)
        {
            var data = new Dictionary<Location, DijkstraData> { { start, new DijkstraData(null, 0) } };
            while (true)
            {
                var finded = LinqExtention.FirstOr(data.Where(n => !n.Value.Oppened).OrderBy(n => n.Value.Cost), new KeyValuePair<Location, DijkstraData>(start, null));
                if (finded.Value == null)
                    break;
                var now = finded.Key;
                foreach (var to in GetNearest(now))
                {
                    var newCost = GetWeight(to) + data[now].Cost;
                    if (!data.ContainsKey(to))
                    {
                        data.Add(to, new DijkstraData(now, newCost));
                    }
                    else
                    {
                        var nodeData = data[to];
                        if (nodeData.Cost > newCost)
                        {
                            nodeData.Cost = newCost;
                            nodeData.From = now;
                        }
                    }
                }
                data[now].Oppened = true;
            }
            if (!data.ContainsKey(end)) return null;
            var path = new List<Location> { end };
            var position = data[end];
            while (position.From != null)
            {
                path.Add(position.From ?? new Location(0, 0));
                position = data[position.From ?? new Location(0, 0)];
            }
            path.Reverse();
            return path;
        }

        private IEnumerable<Location> GetNearest(Location pos)
        {
            return new[] {pos.Up(), pos.Down(), pos.Left(), pos.Right()}
                .Where(p => p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Height)
                .Where(IsTravaible);
        }

        public static Map Sum(IEnumerable<Map> maps)
        {
            var input = maps.ToList();
            var res = new Map(input[0].Width, input[0].Height);
            for (int x = 0; x < res.Width; x++)
            {
                for (int y = 0; y < res.Height; y++)
                {
                    res.SetWeight(x, y, input.Sum(m =>
                    {
                        return m.GetWeight(x, y);

                    }));
                    res.SetTravaible(x, y, input.All(m => m.IsTravaible(x, y)));
                }
            }
            return res;
        }

        public static Map Sum(params Map[] maps) => Sum(maps.Select(o => o));
    }

    internal class DijkstraData
    {
        public DijkstraData(Location? from, long cost)
        {
            From = from;
            Cost = cost;
            Oppened = false;
        }

        public Location? From;
        public long Cost;
        public bool Oppened;
    }
}