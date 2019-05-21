using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
namespace Potential
{
    public class GameState
    {
        private enum UIFlags
        {
            TRACER_ON,
            PAUSED,
            FPS_ON,
            MOUSE_LOCATION_ON,
        }
        private Color ColorFG = Color.White;
        private Color ColorBG = Color.Black;
        private SpriteFont Font { get; set; } = null;
        Utilities.SmartFramerate FPS = new Utilities.SmartFramerate(5, (4, 0));
        private Keys? LastPressed { get; set; } = null;
        private HashSet<UIFlags> Flags = new HashSet<UIFlags>();
        public bool IsPaused { get => Flags.Contains(UIFlags.PAUSED); }
        public static GameState state = null;

        public static GameState GetState(SpriteFont font)
        {
            if (state == null)
                state = new GameState(font);
            return state;

        }
        private GameState(SpriteFont font)
        {
            Font = font;
            Flags.Add(UIFlags.PAUSED);
            Flags.Add(UIFlags.FPS_ON);
            Flags.Add(UIFlags.MOUSE_LOCATION_ON);
        }

        public Utilities.ErrorCodes Draw(SpriteBatch batch, GameWindow window = null)
        {
            var TextUI = new List<string>();
            if (Flags.Contains(UIFlags.FPS_ON))
            {
                TextUI.Add($"{System.Math.Round(FPS.Framerate, 0)}FPS");
            }
            if (Flags.Contains(UIFlags.MOUSE_LOCATION_ON))
            {
                var mouse_state = Mouse.GetState();
                int X = mouse_state.X, Y = mouse_state.Y;
                if ((mouse_state.X < 0 || mouse_state.X > window.ClientBounds.Width) ||
                    (mouse_state.Y < 0 || mouse_state.Y > window.ClientBounds.Height))
                {
                    X = 0;
                    Y = 0;
                }
                TextUI.Add($"({X}, {Y})");
            }
            if (Flags.Contains(UIFlags.PAUSED))
            {
                TextUI.Add("PAUSED");
            }
            var s = string.Join("    ", TextUI);
            batch.DrawString(Font, s, FPS.Position, ColorFG);
            return Utilities.ErrorCodes.SUCCESS;
        }
        public Utilities.ErrorCodes Update(GameTime time = null, World world = null, GameState state = null, object keyboard_state = null)
        {
            var pressed = ((KeyboardState)keyboard_state);
            if (pressed.IsKeyUp(Keys.Space) && LastPressed.HasValue && LastPressed.Value == Keys.Space)
            {
                if (Flags.Contains(UIFlags.PAUSED))
                {
                    Flags.Remove(UIFlags.PAUSED);
                }
                else
                {
                    Flags.Add(UIFlags.PAUSED);
                }
                LastPressed = null;
            }
            if (pressed.IsKeyDown(Keys.Space))
            {
                LastPressed = Keys.Space;
            }
            FPS.Update(time);
            return Utilities.ErrorCodes.SUCCESS;
        }
        public object Clone()
        {
            return GameState.state;
        }

    }
}