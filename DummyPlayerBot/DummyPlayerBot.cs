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
            if (Bot == null || !levelView.Field.GetCellsOfType(CellType.Exit).First().Equals(Bot.Exit))
            {
                Level++;
                if (BotFactories.ContainsKey(Level))
                {
                    Bot = BotFactories[Level].CreateBot(levelView, Level);
                }
                else
                {
                    Bot = BotFactories[BotFactories.Keys.Min(k => Math.Abs(k - Level))].CreateBot(levelView, Level);
                }
            }
            if (Level == 2)
                Thread.Sleep(200);
            return Bot.Iteration(levelView, messageReporter);
        }

        private static bool IsInAttackRange(Location a, Location b)
        {
            return a.IsInRange(b, 1);

        }
    }
}
