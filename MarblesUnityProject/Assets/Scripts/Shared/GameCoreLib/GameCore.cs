

using System.Collections.Generic;
using MemoryPack;

namespace GameCoreLib
{
    [MemoryPackable]
    public partial class GameCore
    {
        public ushort Seq;

        public GameTile GameTile1 = new GameTile(1);
        public GameTile GameTile2 = new GameTile(2);


        public void Step(List<InputEvent> inputEvents)
        {
            GameTile1.Step();
            GameTile2.Step();

            Seq = Seq.WrappingAdd(1);
        }
    }
}
