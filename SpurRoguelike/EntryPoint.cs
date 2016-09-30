using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Fclp;
using SpurRoguelike.ConsoleGUI;
using SpurRoguelike.ConsoleGUI.TextScreen;
using SpurRoguelike.Content;
using SpurRoguelike.Core;
using SpurRoguelike.Generators;

namespace SpurRoguelike
{
    internal class EntryPoint
    {
        public static void Main(string[] args)
        {
            var commandLineParser = new FluentCommandLineParser<GameOptions>();

            commandLineParser
                .Setup(options => options.PlayerName)
                .As('p')
                .SetDefault("Player")
                .WithDescription("Player name");

            commandLineParser
                .Setup(options => options.Random)
                .As('r')
                .SetDefault(false)
                .WithDescription("Random seed?");

            commandLineParser
                .Setup(options => options.WaitKey)
                .As('k')
                .SetDefault(false)
                .WithDescription("Wait key?");

            commandLineParser
                .Setup(options => options.PlayerController)
                .As('c')
                .WithDescription("Path to assembly containing player controller");

            commandLineParser
                .Setup(options => options.Seed)
                .As('s')
                .SetDefault(0)
                .WithDescription("Seed for level generation");

            commandLineParser
                .Setup(options => options.SeedInc)
                .As('i')
                .SetDefault(1)
                .WithDescription("Seed inc");

            commandLineParser
                .Setup(options => options.TestCount)
                .As('m')
                .SetDefault(-1)
                .WithDescription("Many tests");

            commandLineParser
                .Setup(options => options.LevelCount)
                .As('n')
                .SetDefault(5)
                .WithDescription("Number of levels to generate");

            commandLineParser
                .SetupHelp("h", "help")
                .WithHeader($"{AppDomain.CurrentDomain.FriendlyName} [-p name] [-c controller] [-s seed] [-n number]")
                .Callback(text => Console.WriteLine(text));

            if (commandLineParser.Parse(args).HelpCalled)
                return;

            RunGame(commandLineParser.Object);
        }

        private static void RunGame(GameOptions options)
        {
            if (options.TestCount != -1)
            {
                RunManyGames(options);    
                return;
            }

            var levels = GenerateLevels(options.Seed, options.LevelCount);

            var gui = new ConsoleGui(new TextScreen());

            var playerController = options.PlayerController == null ?
                new ConsolePlayerController(gui) :
                BotLoader.LoadPlayerController(options.PlayerController);

            var engine = new Engine(options.PlayerName, playerController, levels.First(), new ConsoleRenderer(gui), new ConsoleEventReporter(gui));

            engine.GameLoop();
        }

        private static void RunManyGames(GameOptions options)
        {
            NullIO io = new NullIO(options.Seed);
            var cx = 0;
            for (int i = 0; i < options.TestCount; i++)
            {
                int s = 0;
                if (options.Random)
                {
                    s = new Random().Next();
                    Console.WriteLine("S" + s);
                }
                else
                {
                    s = options.Seed + cx;
                    cx += options.SeedInc;
                }
                RunOneGame(s, options.LevelCount, options.PlayerController, io);
            }
            Console.WriteLine($"Games: {io.GameComleted}/{options.TestCount} = {100*io.GameComleted/options.TestCount}");
            Console.WriteLine($"Level completed: {io.LevelsCompleted}/{options.TestCount*options.LevelCount} = {100*io.LevelsCompleted/(options.TestCount * options.LevelCount)}");
            File.AppendText("res.txt").WriteLine($"Games: {io.GameComleted}/{options.TestCount} = {100 * io.GameComleted / options.TestCount}\n"+ $"Level completed: {io.LevelsCompleted}/{options.TestCount * options.LevelCount} = {100 * io.LevelsCompleted / (options.TestCount * options.LevelCount)}");
            if (options.WaitKey) Console.ReadKey();
        }

        private static void RunOneGame(int seed, int count, string pc, NullIO io)
        {
            var levels = GenerateLevels(seed, count);

            var playerController = BotLoader.LoadPlayerController(pc);

            var engine = new Engine("~", playerController, levels.First(), io, io);

            engine.GameLoop();
        }

        private static List<Level> GenerateLevels(int seed, int count)
        {
            count = Math.Max(2, count);

            var nameGenerator = new NameGenerator(seed);
            var levelGenerator = new LevelGenerator(seed, nameGenerator);
            var monsterClassesGenerator = new MonsterClassesGenerator(seed, nameGenerator);
            var itemClassesGenerator = new ItemClassesGenerator(seed, nameGenerator);

            var itemClasses = itemClassesGenerator.Generate(7,
                new ItemClassOptions { Level = 3, Rarity = 1 },
                new ItemClassOptions { Level = 10, Rarity = 0.15 },
                new ItemClassOptions { Level = 30, Rarity = 0.1 });

            var monsterClasses = monsterClassesGenerator.Generate(5,
                new MonsterClassOptions { Skill = 0.5, Rarity = 0.02, Factory = (name, skill, health, attack, defence) => new Dimwit(name, attack, defence, health, health) },
                new MonsterClassOptions { Skill = 0.6, Rarity = 0.04, Factory = (name, skill, health, attack, defence) => new Dimwit(name, attack, defence, health, health) },
                new MonsterClassOptions { Skill = 0.6, Rarity = 0.06, Factory = (name, skill, health, attack, defence) => new Reptiloid(name, attack, defence, health, health, skill) },
                new MonsterClassOptions { Skill = 0.7, Rarity = 0.1, Factory = (name, skill, health, attack, defence) => new Dimwit(name, attack, defence, health, health) },
                new MonsterClassOptions { Skill = 0.7, Rarity = 0.2, Factory = (name, skill, health, attack, defence) => new Reptiloid(name, attack, defence, health, health, skill) },
                new MonsterClassOptions { Skill = 0.8, Rarity = 1, Factory = (name, skill, health, attack, defence) => new Reptiloid(name, attack, defence, health, health, skill) });
          
            var levels = new List<Level>();

            var settings = FillDefaultSettings();
            
            for (int i = 0; i < count - 1; i++)
            {
                levels.Add(levelGenerator.Generate(settings, monsterClasses, itemClasses));

                if (i > 0)
                    levels[i - 1].SetNextLevel(levels[i]);

                settings.Monsters.MinSkill += 0.1;
                settings.Monsters.MaxSkill += 0.1;

                if (i == 1)
                    settings.Items.MaxLevel = 100;
            }

            var lastLevelSettigns = FillLastLevelSettings();
            var lastLevelMonsterClasses = monsterClassesGenerator.Generate(1, 
                new MonsterClassOptions { Skill = 1.2, Rarity = 1, Factory = (name, skill, health, attack, defence) => new ArenaFighter(name, attack, defence, health, health, skill) });

            var lastLevel = new ArenaGenerator(seed, nameGenerator).Generate(lastLevelSettigns, lastLevelMonsterClasses, itemClasses);

            levels[levels.Count - 1].SetNextLevel(lastLevel);

            levels.Add(lastLevel);

            return levels;
        }

        private static LevelGenerationSettings FillDefaultSettings()
        {
            return new LevelGenerationSettings
            {
                Field = new LevelGenerationSettings.FieldOptions
                {
                    FreeSpaceShare = 0.7,
                    MinWidth = 40,
                    MaxWidth = 50,
                    MinHeight = 35,
                    MaxHeight = 45
                },
                Monsters = new LevelGenerationSettings.MonsterOptions
                {
                    Density = 0.01,
                    MinSkill = 0.4,
                    MaxSkill = 0.5
                },
                Items = new LevelGenerationSettings.ItemOptions
                {
                    Density = 0.01,
                    MinLevel = 0,
                    MaxLevel = 10
                },
                Traps = new LevelGenerationSettings.TrapOptions
                {
                    Density = 0.05
                },
                HealthPacks = new LevelGenerationSettings.HealthPackOptions
                {
                    Density = 0.02
                }
            };
        }

        private static LevelGenerationSettings FillLastLevelSettings()
        {
            return new LevelGenerationSettings
            {
                Field = new LevelGenerationSettings.FieldOptions
                {
                    FreeSpaceShare = 0.9,
                    MinWidth = 50,
                    MaxWidth = 50,
                    MinHeight = 40,
                    MaxHeight = 40
                },
                Monsters = new LevelGenerationSettings.MonsterOptions
                {
                    MinSkill = 0,
                    MaxSkill = 100
                },
                Items = new LevelGenerationSettings.ItemOptions
                {
                    Density = 0.01,
                    MinLevel = 0,
                    MaxLevel = 100
                },
                Traps = new LevelGenerationSettings.TrapOptions
                {
                    Density = 0.01
                },
                HealthPacks = new LevelGenerationSettings.HealthPackOptions
                {
                    Density = 0.01
                }
            };
        }

        private class GameOptions
        {
            public bool WaitKey { get; set; }
            public int SeedInc { get; set; }
            public bool Random { get; set; }
            public int TestCount { get; set; }

            public string PlayerName { get; set; }

            public string PlayerController { get; set; }

            public int Seed { get; set; }

            public int LevelCount { get; set; }
        }
    }
}
