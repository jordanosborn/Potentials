namespace Potential
{
    public class GameState
    {
        public static GameState state = null;

        public static GameState Create()
        {
            if (state == null)
                state = new GameState();
            return state;

        }
        private GameState() { }

    }
}