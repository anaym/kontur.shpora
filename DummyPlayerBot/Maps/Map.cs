using System;
using System.Collections.Generic;
using System.Linq;
using DummyPlayerBot.Extension;
using SpurRoguelike.Core.Primitives;
using LinqExtention = DummyPlayerBot.Extension.LinqExtention;

namespace DummyPlayerBot.Maps
{

    //mutable collection. Мог бы сделать immutable, но особого выигрыша в скорости от этого небыло бы
    public class Map
    {
        protected readonly long[,] weigthes;
        protected readonly bool[,] travable;
        private Dictionary<Location, Dictionary<Location, DijkstraData>> cache;

        public int Multiplyer { get; set; }

        protected Map(int width, int heigth)
        {
            weigthes = new long[width, heigth];
            travable = new bool[width, heigth];
            cache = new Dictionary<Location, Dictionary<Location, DijkstraData>>();
            Multiplyer = 1;
        }

        public Map(long[, ] weigthes, bool[, ] travable) : this(weigthes.GetLength(0), weigthes.GetLength(1))
        {
            this.weigthes = weigthes; //TODO: можно изменить исзодный массив, по-хорошему - копироваьб, так же могут не совпадать размерности массивов
            this.travable = travable;
        }

        public long GetWeight(int x, int y)
        {
            return weigthes[x, y] * Multiplyer;
        }

        public long GetWeight(Location pos) => GetWeight(pos.X, pos.Y);

        public bool IsTravaible(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;
            return travable[x, y];
        }

        public bool IsTravaible(Location pos) => IsTravaible(pos.X, pos.Y);

        public int Width => weigthes.GetLength(0);
        public int Height => weigthes.GetLength(1);

        public Dictionary<Location, DijkstraData> CreateStatistic(Location point)
        {
            if (cache.ContainsKey(point) && cache[point] != null)
            {
                return cache[point];
            }

            var data = new Dictionary<Location, DijkstraData> { { point, new DijkstraData(null, 0) } };
            var notOpenned = new List<Location> { point };
            while (true)
            {
                Location mink = new Location();
                long minv = long.MaxValue;
                bool exist = false;
                foreach (var key in notOpenned)
                {

                    if (data[key].Cost < minv)
                    {
                        minv = data[key].Cost;
                        mink = key;
                        exist = true;
                    }

                }
                if (!exist)
                    break;
                var now = mink;
                foreach (var to in GetNearest(now))
                {
                    var newCost = GetWeight(to) + data[now].Cost;
                    if (!data.ContainsKey(to))
                    {
                        data.Add(to, new DijkstraData(now, newCost));
                        notOpenned.Add(to);
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
                notOpenned.Remove(now);
                data[now].Oppened = true;
            }
            cache.Add(point, data);
            return data;
        }

        public List<Location> FindPath(Dictionary<Location, DijkstraData> statistic, Location end)
        {
            if (!statistic.ContainsKey(end)) return null;
            var path = new List<Location> { end };
            var position = statistic[end];
            while (position.From != null)
            {
                path.Add(position.From ?? new Location(0, 0));
                position = statistic[position.From ?? new Location(0, 0)];
            }
            path.Reverse();
            return path;
        }

        public long? GetDistance(Location start, Location end)
        {
            var sts = CreateStatistic(start);
            if (!sts.ContainsKey(end))
                return null;
            return sts[end].Cost;
        }

        /// <summary>
        /// Поиск кратчайшего пути алгоритмом Дейкстры
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Location> FindPath(Location start, Location end)
        {
            return FindPath(CreateStatistic(start), end);
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
            var wghts = new long[input[0].Width, input[0].Height];
            var trvbls = new bool[input[0].Width, input[0].Height];
            for (int x = 0; x < input[0].Width; x++)
            {
                for (int y = 0; y < input[0].Height; y++)
                {
                    wghts[x, y] = input.Sum(m => m.GetWeight(x, y));
                    trvbls[x, y] = input.All(m => m.IsTravaible(x, y));
                }
            }
            return new Map(wghts, trvbls);
        }

        public static Map Sum(params Map[] maps) => Sum(maps.Select(o => o));
    }

    public class DijkstraData
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