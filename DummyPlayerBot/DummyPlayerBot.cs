using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DummyPlayerBot;
using DummyPlayerBot.AI;
using DummyPlayerBot.Extension;
using SpurRoguelike.Core;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

//TODO: присоедениться к чату в телеграме
//TODO: понять логику движения врагов
//TODO: написать небольшую графовую библиотеку и внедрить её

namespace DummyPlayerBot
{
    public class DummyPlayerBot : IPlayerController
    {
        public IAi Ai;
        public int LevelIndex;
        public Dictionary<int, IAiFactory> AiFactories;
        public int MonsterCount;

        private Queue<Location> history;
        private int historySize;

        public DummyPlayerBot()
        {
            Ai = null;
            LevelIndex = -1;
            
            AiFactories = new Dictionary<int, IAiFactory>();
            AiFactories.Add(0, new LambdaAiFactory((v, i) => new FastKillAi(v)));
            AiFactories.Add(1, new LambdaAiFactory((v, i) => new SmartAi(v, i)));

            history = new Queue<Location>();
            historySize = 20;
        }

        public Turn MakeTurn(LevelView levelView, IMessageReporter messageReporter)
        {
            if (Ai == null || !levelView.Field.GetCellsOfType(CellType.Exit).First().Equals(Ai.Exit) || MonsterCount < levelView.Monsters.Count())
            {
                LevelIndex++;
                if (IsLastLevel(levelView))
                {
                    Ai = new ArenaDestroyerAi(levelView);
                }
                else if (AiFactories.ContainsKey(LevelIndex))
                {
                    Ai = AiFactories[LevelIndex].CreateBot(levelView, LevelIndex);
                }
                else
                {
                    Ai = AiFactories[AiFactories.Keys.OrderBy(k => Math.Abs(k - LevelIndex)).First()].CreateBot(levelView, LevelIndex);
                }
                history = new Queue<Location>(historySize);
            }
            MonsterCount = levelView.Monsters.Count();
            //if (LevelIndex == 2)
                //Thread.Sleep(200);
            var isAttack = false;
            var action =  Ai.Iteration(levelView, messageReporter, out isAttack);
            if (!isAttack)
            {
                history.Enqueue(levelView.Player.Location);
                if (history.Count > historySize)
                {
                    history.Dequeue();
                    if (new HashSet<Location>(history).Count < (historySize*Ai.CriticalPercentageInactivity/100))
                    {
                        messageReporter.ReportMessage("T");
                        history.Clear();
                        if (levelView.Monsters.Any(m => m.Location.IsInRange(levelView.Player.Location, 1)))
                        {
                            messageReporter.ReportMessage("A");
                            return Turn.Attack(levelView.Monsters.First(m => m.Location.IsInRange(levelView.Player.Location, 1)).Location - levelView.Player.Location);
                        }
                        var solve = Ai.HandleCycle(levelView);
                        if (solve != null)
                            return solve;
                        return Turn.Step((StepDirection)new Random().Next(0, 4));
                    } 
                }
            }
            return action;
        }

        private static bool IsLastLevel(LevelView level)
        {
            var exit = level.Field.GetCellsOfType(CellType.Exit).First();
            var neir = new Location[] {exit.Up(), exit.Down(), exit.Left(), exit.Right()};
            return neir.All(p => level.Field[p] == CellType.Wall);
        }
    }
}
