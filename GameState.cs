using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace Potential
{
    public class GameState : GameObject
    {
        private Keys? LastPressed { get; set; } = null;

        public bool IsPaused { get; set; } = true;
        public static GameState state = null;

        public static GameState GetState()
        {
            if (state == null)
                state = new GameState();
            return state;

        }
        private GameState() { }

        public Utilities.ErrorCodes Draw(SpriteBatch batch)
        {

            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Update(GameTime time = null, World world = null, GameState state = null, object keyboard_state = null)
        {
            var pressed = ((KeyboardState)keyboard_state);
            if (pressed.IsKeyUp(Keys.Space) && LastPressed.HasValue && LastPressed.Value == Keys.Space)
            {
                IsPaused = !IsPaused;
                LastPressed = null;
            }
            if (pressed.IsKeyDown(Keys.Space))
            {
                LastPressed = Keys.Space;
            }
            return Utilities.ErrorCodes.SUCCESS;
        }
        public object Clone()
        {
            return GameState.state;
        }

    }
}