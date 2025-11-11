

using MemoryPack;

namespace GameCoreLib
{
    [MemoryPackable]
    public partial class GameCore
    {
        public GameTile GameTile1 = new GameTile(1);
        public GameTile GameTile2 = new GameTile(2);


        private void Step(){
            GameTile1.Step();
            GameTile2.Step();
        }
    }
}
