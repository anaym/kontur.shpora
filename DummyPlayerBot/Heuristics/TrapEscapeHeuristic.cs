using System.Collections.Generic;
using System.Linq;
using DLibrary.Graph;
using DummyPlayerBot.Enviroment;
using DummyPlayerBot.Enviroment.CostExtractors;
using SpurRoguelike.Core.Primitives;

namespace DummyPlayer
{
    public class TrapEscapeHeuristic : IHeuristic
    {
        public Location? Target { get; private set; }
        public int CriticalPercentage { get; private set; }
        public int MinCount { get; private set; }
        public int SquarPercentageSize { get; private set; }

        public TrapEscapeHeuristic(int criticalPercentage, int minCount = 4, int squarePercentageSize = 50)
        {
            CriticalPercentage = criticalPercentage;
            MinCount = minCount;
            SquarPercentageSize = squarePercentageSize;

            Status = "[x]";
        }

        private int SquareWigth(Enviroment enviroment) => enviroment.Map.Width*SquarPercentageSize/100;
        private int SquareHeigth(Enviroment enviroment) => enviroment.Map.Height*SquarPercentageSize/100;

        public bool ItIsATrap(Enviroment enviroment)
        {
            var mySquare = Square.FromCentre(enviroment.View.Player.Location, SquareWigth(enviroment), SquareHeigth(enviroment));
            var nearEnemy = mySquare.EnemyCount(enviroment);
            var totalEnemy = enviroment.View.Monsters.Count();
            Status = $"N{nearEnemy}, T{totalEnemy}, P{100 * nearEnemy / (totalEnemy==0? 1:totalEnemy )}";
            if (nearEnemy <= MinCount)
                return false;
            var percentage = 100*nearEnemy/totalEnemy;
            return percentage >= CriticalPercentage;
        }

        public Turn Solve(Enviroment level)
        {
            if (ItIsATrap(level))
            {
                if (Target == null)
                {
                    var centre = new Location(level.Map.Width/2, level.Map.Height/2);
                    var self = level.View.Player.Location;
                    var antipod = self + (centre - self);
                    var spot = level.Map.FindNearest(antipod, CellType.Empty, 10);
                    Target = spot;
                }
            }
            else
            {
                Target = null;
            }
            if (Target != null)
                return
                    level.Map.FindPath(level.View.Player.Location, (Location)Target, new PositiveWeighesPathFinder(),
                        new CellTypeDependendCostExtractor(level, 5)).ToTurn().FirstOr(null);
            return null;
        }

        public string Name => "Trap escaper";
        public string Status { get; private set; }
    }

    public class Square
    {
        public readonly Location Left;
        public readonly Location Right;

        public static Square FromCentre(Location c, int width, int heigth)
        {
            var rw = width / 2;
            var rh = heigth / 2;
            return new Square(new Location(c.X - rw, c.Y - rh), new Location(c.X + rw, c.Y + rh));
        }

        public Square(Location left, Location right)
        {
            Left = left;
            Right = right;
        }

        public int EnemyCount(Enviroment enviroment)
        {
            return enviroment.View.Monsters.Count(m => m.Location.InRect(Left, Right));
        }
    }
}