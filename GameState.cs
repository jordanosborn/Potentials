using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

using System;
namespace Potential
{
    using Utilities;
    public class GameState
    {
        private class ParamValues
        {
            public float Mass { get; set; } = 0;
            public float Charge { get; set; } = 0;
            public float Radius { get; set; } = 0;
            public Vector3 Position { get; set; } = Vector3.Zero;
            public Vector3 Velocity { get; set; } = Vector3.Zero;
            public void Update(float? mass, float? charge = null, float? radius = null, Vector3? position = null, Vector3? velocity = null)
            {
                Mass = mass ?? Mass;
                Charge = charge ?? Charge;
                Radius = radius ?? Radius;
                Position = position ?? Position;
                Velocity = velocity ?? Velocity;
            }
        }
        public enum Param
        {
            MASS = 0,
            CHARGE = 1,
            RADIUS = 2,
            POSITION = 3,
            VELOCITY = 4,
        }
        public enum UIFlags
        {
            TRACERS_ON,
            PAUSED,
            FPS_ON,
            MOUSE_LOCATION_ON
        }
        private static int ParameterCount = (Enum.GetValues(typeof(Param))).Length;
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
        readonly Vector2 SelectedPosition = new Vector2(4, 20);
        private Keys? LastPressed { get; set; } = null;
        private MouseButtonCode? LastClicked { get; set; } = null;
        private Param ParamSelected { get; set; } = Param.MASS;
        private ParamValues paramValues { get; set; } = new ParamValues();
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

        private void DrawParamUI(SpriteBatch batch, Vector2 location)
        {
            var TextUI = new List<string>();
            TextUI.Add($"Mass: {paramValues.Mass}");
            TextUI.Add($"Charge: {paramValues.Charge}");
            TextUI.Add($"Radius: {paramValues.Radius}");
            TextUI.Add($"Position: {paramValues.Position}");
            TextUI.Add($"Velocity: {paramValues.Velocity}");
            string filterText;
            switch (ParamSelected)
            {
                case Param.MASS:
                    filterText = "Mass";
                    break;
                case Param.CHARGE:
                    filterText = "Charge";
                    break;
                case Param.RADIUS:
                    filterText = "Radius";
                    break;
                case Param.POSITION:
                    filterText = "Position";
                    break;
                case Param.VELOCITY:
                    filterText = "Velocity";
                    break;
                default:
                    filterText = "";
                    break;
            }
            string selectedText = string.Join("    ", TextUI.Where((s) => s.Contains(filterText)).ToList());
            string unselectedText = string.Join("    ", TextUI.Where((s) => !s.Contains(filterText)).ToList());
            batch.DrawString(Font, unselectedText, location, ColorFG);
            batch.DrawString(Font, selectedText, SelectedPosition, Color.Green);
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
            DrawParamUI(batch, new Vector2(4, window.ClientBounds.Height - 20));
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
            KeyPress(keyboardState, Keys.Tab, () =>
            {
                ParamSelected = (Param)(((int)ParamSelected + 1) % (ParameterCount));
                return ErrorCodes.SUCCESS;
            });
            MouseClick(mouseState, MouseButtonCode.LEFT, () =>
            {
                paramValues.Position = new Vector3(mouseState.X, mouseState.Y, 0);
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