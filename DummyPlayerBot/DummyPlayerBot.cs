using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DLibrary.Graph;
using DummyPlayerBot.Enviroment;
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
        public List<IHeuristic> Sense;
        public Enviroment Enviroment;
        public DummyPlayerBot()
        {
            Sense = new List<IHeuristic>();
            Sense.Add(new EnemyCountDependendHealingHeuristic(25, 60));
            Sense.Add(new TrapEscapeHeuristic(50, 4, 40));
            Sense.Add(new HealingHeuristic(25));
            Sense.Add(new NearAttackHeuristic());
            Sense.Add(new HealingHeuristic(100));
            //Sense.Add(new FarAttackHeuristic(10));
            Sense.Add(new FarAttackHeuristic(1000));
            Sense.Add(new BonusCollectionHeuristic(int.MaxValue, false));
            Sense.Add(new HealingHeuristic(100));
            Sense.Add(new SuicideAttack());
            Sense.Add(new LevelLeavingHeuristic());
        }

        public Turn MakeTurn(LevelView levelView, IMessageReporter messageReporter)
        {
            if (Enviroment == null)
                Enviroment = new Enviroment(levelView);
            Enviroment.Update(levelView);

            //Thread.Sleep(50);
            foreach (var heuristic in Sense)
            {
                var result = heuristic.Solve(Enviroment);
                if (result != null)
                {
                    messageReporter.ReportMessage($"[DPB]: {heuristic.Name} {heuristic.Status}");
                    File.AppendAllLines("log.txt", new []{ $"[DPB]: {heuristic}" }, Encoding.UTF8);
                    return result;
                }
            }
            messageReporter.ReportMessage($"[DPB]: Critical error");
            File.AppendAllLines("log.txt", new[] { $"[DPB]: Critical error" }, Encoding.UTF8);
            return Turn.None;
        }

        private static bool IsInAttackRange(Location a, Location b)
        {
            return a.IsInRange(b, 1);

        }
    }
}
