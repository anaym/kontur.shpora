using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DLibrary.Graph;
using DummyPlayerBot;
using SpurRoguelike.Core;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;
using DummyPlayer;

//TODO: присоедениться к чату в телеграме
//TODO: понять логику движения врагов
//TODO: написать небольшую графовую библиотеку и внедрить её

namespace DummyPlayer
{
    public class DummyPlayerBot : IPlayerController
    {
        public IBot Bot;
        public int Level;
        public Dictionary<int, IBotFactory> BotFactories;
        public int MonsterCount;

        public DummyPlayerBot()
        {
            Bot = null;
            Level = -1;
            
            BotFactories = new Dictionary<int, IBotFactory>();
            BotFactories.Add(0, new LambdaBotFactory((v, i) => new FastKillBot(v)));
            BotFactories.Add(1, new LambdaBotFactory((v, i) => new SmartBot(v, i)));
        }

        public Turn MakeTurn(LevelView levelView, IMessageReporter messageReporter)
        {
            if (Bot == null || !levelView.Field.GetCellsOfType(CellType.Exit).First().Equals(Bot.Exit) || MonsterCount < levelView.Monsters.Count())
            {
                Level++;
                if (IsLastLevel(levelView))
                {
                    Bot = new ArenaDestroyerBot(levelView);
                }
                else if (BotFactories.ContainsKey(Level))
                {
                    Bot = BotFactories[Level].CreateBot(levelView, Level);
                }
                else
                {
                    Bot = BotFactories[BotFactories.Keys.OrderBy(k => Math.Abs(k - Level)).First()].CreateBot(levelView, Level);
                }
            }
            MonsterCount = levelView.Monsters.Count();
            //if (Level == 2)
                //Thread.Sleep(200);
            return Bot.Iteration(levelView, messageReporter);
        }

        private static bool IsLastLevel(LevelView level)
        {
            var exit = level.Field.GetCellsOfType(CellType.Exit).First();
            var neir = new Location[] {exit.Up(), exit.Down(), exit.Left(), exit.Right()};
            return neir.All(p => level.Field[p] == CellType.Wall);
        }
    }
}
