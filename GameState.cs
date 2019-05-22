using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

using System;
namespace Potential
{
    using Utilities;
    public class GameState
    {
        public enum UIFlags
        {
            TRACERS_ON,
            PAUSED,
            FPS_ON,
            MOUSE_LOCATION_ON
        }
        public enum MouseButtonCode
        {
            LEFT,
            RIGHT,
            MIDDLE
        }
        private Color ColorFG = Color.White;
        private Color ColorBG = Color.Black;
        private SpriteFont Font { get; set; } = null;
        readonly SmartFramerate FPS = new SmartFramerate(5, (4, 0));
        private Keys? LastPressed { get; set; } = null;
        private MouseButtonCode? LastClicked { get; set; } = null;
        public HashSet<UIFlags> Flags { get; private set; } = new HashSet<UIFlags>();
        public bool IsPaused { get => Flags.Contains(UIFlags.PAUSED); }
        public static GameState state;

        public static GameState GetState(SpriteFont font = null)
        {
            if (state == null)
            {
                if (font == null)
                {
                    throw new ArgumentNullException();
                }
                state = new GameState(font);
            }

            return state;

        }
        private GameState(SpriteFont font)
        {
            Font = font;
            Flags.Add(UIFlags.PAUSED);
            Flags.Add(UIFlags.FPS_ON);
            Flags.Add(UIFlags.MOUSE_LOCATION_ON);
            Flags.Add(UIFlags.TRACERS_ON);
        }

        public ErrorCodes Draw(SpriteBatch batch, GameWindow window = null)
        {
            var TextUI = new List<string>();
            if (Flags.Contains(UIFlags.FPS_ON))
            {
                TextUI.Add($"{Math.Round(FPS.Framerate, 0)}FPS");
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
            if (!Flags.Contains(UIFlags.TRACERS_ON))
            {
                TextUI.Add("Tracers: OFF");
            }
            var s = string.Join("    ", TextUI);
            batch.DrawString(Font, s, FPS.Position, ColorFG);
            return ErrorCodes.SUCCESS;
        }
        private ErrorCodes MouseClick(MouseState pressed, MouseButtonCode button, Func<ErrorCodes> func)
        {
            var clicked = (button == MouseButtonCode.LEFT) ? pressed.LeftButton : (button == MouseButtonCode.RIGHT) ? pressed.RightButton : pressed.MiddleButton;

            if (clicked == ButtonState.Released && LastClicked.HasValue && LastClicked.Value == button)
            {
                func();
                LastClicked = null;
            }
            else if (clicked == ButtonState.Pressed)
            {
                LastClicked = button;
            }
            return ErrorCodes.SUCCESS;
        }

        private void KeyPress(KeyboardState pressed, Keys key, Func<ErrorCodes> func)
        {
            if (pressed.IsKeyUp(key) && LastPressed.HasValue && LastPressed.Value == key)
            {
                func();
                LastPressed = null;
            }
            else if (pressed.IsKeyDown(key))
            {
                LastPressed = key;
            }
        }
        public ErrorCodes Update(KeyboardState keyboardState, MouseState mouseState, GameTime time = null, World world = null, GameState state = null)
        {
            KeyPress(keyboardState, Keys.Space, () =>
            {
                if (Flags.Contains(UIFlags.PAUSED))
                {
                    Flags.Remove(UIFlags.PAUSED);
                }
                else
                {
                    Flags.Add(UIFlags.PAUSED);
                }
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.T, () =>
            {
                if (Flags.Contains(UIFlags.TRACERS_ON))
                {
                    Flags.Remove(UIFlags.TRACERS_ON);
                }
                else
                {
                    Flags.Add(UIFlags.TRACERS_ON);
                }
                return ErrorCodes.SUCCESS;
            });
            MouseClick(mouseState, MouseButtonCode.LEFT, () =>
            {
                Console.WriteLine($"{mouseState.X}, {mouseState.Y}");
                return ErrorCodes.SUCCESS;
            });


            FPS.Update(time);
            return ErrorCodes.SUCCESS;
        }
        public object Clone()
        {
            return state;
        }

    }
}