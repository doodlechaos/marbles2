namespace GameCore
{
    public class GameCore
    {
        public GameTile GameTile1 = new GameTile(1);
        public GameTile GameTile2 = new GameTile(2);


        public void FixedUpdate()
        {

        }

        private void Step(){
            GameTile1.Step();
            GameTile2.Step();
        }
    }
}
