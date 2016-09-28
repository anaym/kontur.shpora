using SpurRoguelike.Core.Primitives;
using SpurRoguelike.Core.Views;

namespace DummyPlayerBot
{
    public class SmartBot : IBot
    {
        public Location Exit { get; }

        public SmartBot(LevelView level, int levelIndex)
        {
            
        }

        public Turn Iteration(LevelView level, IMessageReporter reporter)
        {
            //если мало хп - бежим регениться
            //если рядом много ботов и резко выросла стоимость дойти до аптечки - trap - убегаем
            //если можем атаковать - атакуем [и если есть аптечки на уровне]
            //бежим к дальней цели
            //выбираем дальнюю цель (врага, мега бонус)
            //если есть повреждения - хилимся
            //сваливаем
            throw new System.NotImplementedException();
        }
    }
}