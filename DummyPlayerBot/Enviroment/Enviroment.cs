using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot.Enviroment
{
    public class Enviroment
    {
        public LevelView View { get; private set; }
        public Map Map { get; private set; }
        public int Level { get; private set; }

        public Location Exit { get; private set; }

        public Enviroment(LevelView view)
        {
            Level = -1;
            View = view;
            Map = new Map();
            Update(view);
        }

        public void Update(LevelView view)
        {
            View = view;
            var old = Exit;
            for (int x = 0; x < view.Field.Width; x++)
                for (int y = 0; y < view.Field.Height; y++)
                    if (view.Field[new Location(x, y)] == CellType.Exit)
                        Exit = new Location(x, y);
            if (old != Exit)
            {
                File.AppendAllLines("log.txt", new[] {$"[PDB]: ================================="}, Encoding.UTF8);
                Level += 1;
            }
            if (Level == 4)
                Thread.Sleep(100);
            Map.Update(this);
        }
    }
}