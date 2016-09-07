using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DLibrary.Graph;
using SpurRoguelike.Core;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

//TODO: присоедениться к чату в телеграме
//TODO: понять логику движения врагов
//TODO: написать небольшую графовую библиотеку и внедрить её

namespace DummyPlayer
{
    public class DummyPlayerBot : IPlayerController
    {
        public int MinTrapD = Int32.MaxValue;
        public int MaxK = Int32.MinValue;

        public Turn MakeTurn(LevelView levelView, IMessageReporter messageReporter)
        {
            int d = 0;
            int x = 0;
            int y = 0;
            try
            {
                x = (int)levelView.Monsters.Where(m => m.Location.Distance(levelView.Player.Location) < 5).Average(m => m.Location.X);
                y = (int)levelView.Monsters.Where(m => m.Location.Distance(levelView.Player.Location) < 5).Average(m => m.Location.Y);
                d = levelView.Player.Location.Distance(new Location(x, y));
            }
            catch (Exception)
            {
                d = Int16.MaxValue;
            }

            var k = 100 * levelView.Monsters.Count(m => m.Location.Distance(levelView.Player.Location) < 5) / levelView.Monsters.Count();

            MinTrapD = Math.Min(MinTrapD, d);
            MaxK = Math.Max(k, MaxK);


            messageReporter.ReportMessage($"Dummy system has been activated {levelView.Monsters.Count()}, Trap: {d}/{MinTrapD}, K={k}/{MaxK}");

            
            var map = new Map(levelView,(k > 30 ? (Location?)new Location(x, y) : null));
            var now = map[levelView.Player.Location];
            var path = new PositiveWeighesPathFinder(now, map.Exit).FindPath();

            if (path == null)
                return Turn.None;

            var next = path.Skip(1).First() as MapNode;
            var offset = new Offset(next.Location.X - now.Location.X, next.Location.Y - now.Location.Y);

            var nearbyMonster =
                levelView.Monsters.FirstOrDefault(m => IsInAttackRange(levelView.Player.Location, m.Location));
            
            //Thread.Sleep(k*10);

            if (nearbyMonster.HasValue && false)
                return Turn.Attack(nearbyMonster.Location - levelView.Player.Location);

            return Turn.Step(offset);

            if (levelView.Random.NextDouble() < 0.1)
                return Turn.None;

            return Turn.Step((StepDirection) levelView.Random.Next(4));
        }

        private static bool IsInAttackRange(Location a, Location b)
        {
            return a.IsInRange(b, 1);

        }
    }
}
