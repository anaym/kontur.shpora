using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayer
{
    public class Map
    {
        private double CountNearlyMonster(int radius, LevelView levelView, Location loc)
        {
            var dangerous = 0.0;
            double maxDangerous = levelView.Field.Height + levelView.Field.Width + 2;
            foreach (var monster in levelView.Monsters)
            {
                var md = monster.Location.Distance(loc);
                dangerous += 10*maxDangerous/(md == 0 ? 1 : md*md*md*md*md);
            }
            foreach (var wall in levelView.Field.GetCellsOfType(CellType.Wall).Where(w => w.Distance(loc) < 10))
            {
                dangerous += wall.Distance(loc)/2;
            }
            return 10*maxDangerous - levelView.Monsters.Min(m => m.Location.Distance(loc));
            return dangerous;
        }

        public MapNode Exit;

        public Map(LevelView levelView, Location? trap = null)
        {
            LevelView = levelView;
            Location? exit = null;
            Nodes = new Dictionary<Location, MapNode>();


            for (int x = 0; x < levelView.Field.Width; x++)
            {
                for (int y = 0; y < levelView.Field.Height; y++)
                {
                    var field = levelView.Field[new Location(x, y)];
                    if (field != CellType.Wall && field != CellType.Trap)
                    {
                        Nodes.Add(new Location(x, y),
                            new MapNode(new Location(x, y), 1 + 10*CountNearlyMonster(4, levelView, new Location(x, y)),
                                this));
                    }
                    if (field == CellType.Exit)
                    {
                        exit = new Location(x, y);
                    }
                }
            }
            foreach (var monster in levelView.Monsters)
            {
                Nodes.Remove(monster.Location);
            }
            if (exit == null)
                throw new ArgumentException();
            Exit = this[levelView.Field.GetCellsOfType(CellType.Exit).First()];
            //if (trap != null)
            //{
            //    var vector = levelView.Player.Location - trap;
            //    var spot = levelView.Player.Location + vector;
            //    try
            //    {
            //        Exit =
            //            Nodes.Where(n => n.Key.Distance(levelView.Player.Location) == 1)
            //                .OrderBy(m => m.Value.Cost)
            //                .First()
            //                .Value;
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //}
        }

        public IEnumerable<MapNode> GetNearly(Location loc)
        {
            return
                new List<MapNode> {this[loc.Up()], this[loc.Down()], this[loc.Left()], this[loc.Right()]}
                    .Where(n => n != null);
        }

        public MapNode this[int x, int y] => this[new Location(x, y)];
        public MapNode this[Location location] => !Nodes.ContainsKey(location) ? null : Nodes[location];

        public Dictionary<Location, MapNode> Nodes;
        public LevelView LevelView { get; }
    }
}