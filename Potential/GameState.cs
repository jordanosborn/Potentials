using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Potential.Utilities;
using System.Diagnostics;
//  TODO: cap ParamValues, add incermenent modifier 
namespace Potential
{
    public class GameState
    {
        public enum UiFlags
        {
            TRACERS_ON,
            PAUSED,
            FPS_ON,
            MOUSE_LOCATION_ON
        }

        public enum DirectionSelected
        {
            X,
            Y,
            Z
        }

        private static readonly int ParameterCount = Enum.GetValues(typeof(ParticleParams.Param)).Length;
        private static GameState _state;
        private readonly Color ColorFg = Color.White;
        private readonly SmartFramerate Fps = new SmartFramerate(5, (4, 0));
        private readonly Vector2 SelectedPosition = new Vector2(4, 20);
        private Color ColorBg = Color.Black;

        private GameState(SpriteFont font)
        {
            Font = font;
            Flags.Add(UiFlags.PAUSED);
            Flags.Add(UiFlags.FPS_ON);
            Flags.Add(UiFlags.MOUSE_LOCATION_ON);
            Flags.Add(UiFlags.TRACERS_ON);
        }

        private SpriteFont Font { get; }
        private Keys? LastPressed { get; set; }
        private MouseButtonCode? LastClicked { get; set; }
        private ParticleParams.Param ParamSelected { get; set; } = ParticleParams.Param.MASS;
        private ParticleParams ParamValues { get; } = new ParticleParams();
        private DirectionSelected Direction {get; set; } = DirectionSelected.X;
        public HashSet<UiFlags> Flags { get; } = new HashSet<UiFlags>();
        public bool IsPaused => Flags.Contains(UiFlags.PAUSED);

        public static GameState GetState(SpriteFont font = null)
        {
            if (_state == null)
            {
                if (font == null) throw new ArgumentNullException();
                _state = new GameState(font);
            }

            return _state;
        }

        private void DrawParamUi(SpriteBatch batch, Vector2 location)
        {
            var textUi = new List<string>
            {
                $"Mass: {ParamValues.Mass:F1}",
                $"Charge: {ParamValues.Charge:F1}",
                $"Radius: {ParamValues.Radius:F1}",
                $"Position: ({ParamValues.Position.X:F1}, {ParamValues.Position.Y:F1}, {ParamValues.Position.Z:F1})",
                $"Velocity: ({ParamValues.Velocity.X:F1}, {ParamValues.Velocity.Y:F1}, {ParamValues.Velocity.Z:F1})"
            };
            string filterText;
            switch (ParamSelected)
            {
                case ParticleParams.Param.MASS:
                    filterText = "Mass";
                    break;
                case ParticleParams.Param.CHARGE:
                    filterText = "Charge";
                    break;
                case ParticleParams.Param.RADIUS:
                    filterText = "Radius";
                    break;
                case ParticleParams.Param.POSITION:
                    filterText = "Position";
                    break;
                case ParticleParams.Param.VELOCITY:
                    filterText = "Velocity";
                    break;
                default:
                    filterText = "";
                    break;
            }

            var selectedText = string.Join("    ", textUi.Where(s => s.Contains(filterText)).ToList());
            var unselectedText = string.Join("    ", textUi.Where(s => !s.Contains(filterText)).ToList());
            batch.DrawString(Font, unselectedText, location, ColorFg);
            batch.DrawString(Font, selectedText, SelectedPosition, Color.Green);
        }

        public ErrorCodes Draw(SpriteBatch batch, World world, GameWindow window = null)
        {
            var TextUI = new List<string>
            {
                $"Particles: {world.Particles.Count()}"
            };
            if (Flags.Contains(UiFlags.FPS_ON)) TextUI.Add($"{Math.Round(Fps.Framerate, 0)}FPS");
            if (Flags.Contains(UiFlags.MOUSE_LOCATION_ON))
            {
                var mouse_state = Mouse.GetState();
                int X = mouse_state.X, Y = mouse_state.Y;
                if (mouse_state.X < 0 || mouse_state.X > window.ClientBounds.Width || mouse_state.Y < 0 ||
                    mouse_state.Y > window.ClientBounds.Height)
                {
                    X = 0;
                    Y = 0;
                }

                TextUI.Add($"({X}, {Y})");
            }

            TextUI.Add($"Direction Selected: {Direction}");
            if (Flags.Contains(UiFlags.PAUSED)) TextUI.Add("PAUSED");
            if (!Flags.Contains(UiFlags.TRACERS_ON)) TextUI.Add("Tracers: OFF");
            var s = string.Join("    ", TextUI);
            batch.DrawString(Font, s, Fps.Position, ColorFg);
            DrawParamUi(batch, new Vector2(4, window.ClientBounds.Height - 20));
            if (LastClicked == MouseButtonCode.LEFT)
            {
                var texture = world.Textures["arrow"];
                var textureTail = world.Textures["arrow_tail"];
                var originTail = new Vector2(0, textureTail.Height / 2);
                var origin = new Vector2(texture.Width / 2, texture.Height / 2);
                var position = new Vector2(ParamValues.Position.X, ParamValues.Position.Y);
                var (mouseX, mouseY) = Mouse.GetState().Position;
                var arrowLength = (new Vector2(mouseX, mouseY) - position).Length() / textureTail.Width;
                ParamValues.Velocity = new Vector3(mouseX, mouseY, 0) - ParamValues.Position;
                var velMagnitude = ParamValues.Velocity.Length();
                var (x, y, _) = ParamValues.Velocity / (Math.Abs(velMagnitude) < float.Epsilon ? 1 : velMagnitude);

                var theta = (float) Math.Acos(Vector2.Dot(new Vector2(x, y), Vector2.UnitX));

                const float scale = 20.0f;
                batch.Draw(
                    textureTail,
                    position,
                    origin: originTail,
                    sourceRectangle: null,
                    color: ColorFg,
                    rotation: y < 0 ? -theta : theta,
                    scale: new Vector2(arrowLength, scale / textureTail.Height),
                    effects: SpriteEffects.None,
                    layerDepth: 0
                );
                if (Math.Abs(velMagnitude) > float.Epsilon)
                    batch.Draw(
                        texture,
                        new Vector2(mouseX, mouseY),
                        origin: origin,
                        sourceRectangle: null,
                        color: ColorFg,
                        rotation: y < 0 ? -theta : theta,
                        scale: new Vector2(scale / texture.Width, scale / texture.Height),
                        effects: SpriteEffects.None,
                        layerDepth: 0
                    );
            }

            return ErrorCodes.SUCCESS;
        }

        private ErrorCodes MouseClick(MouseState pressed, MouseButtonCode button, Func<ErrorCodes> before,
            Func<ErrorCodes> after)
        {
            var clicked = button == MouseButtonCode.LEFT ? pressed.LeftButton :
                button == MouseButtonCode.RIGHT ? pressed.RightButton : pressed.MiddleButton;

            if (clicked == ButtonState.Released && LastClicked.HasValue && LastClicked.Value == button)
            {
                LastClicked = null;
                after();
            }
            else if (clicked == ButtonState.Pressed && LastClicked != button)
            {
                LastClicked = button;
                before();
            }

            return ErrorCodes.SUCCESS;
        }

        private void KeyPress(KeyboardState pressed, Keys key, Func<ErrorCodes> func)
        {
            if (pressed.IsKeyUp(key) && LastPressed.HasValue && LastPressed.Value == key)
            {
                Console.WriteLine(key.ToString());
                func();
                LastPressed = null;
            }
            else if (pressed.IsKeyDown(key))
            {
                LastPressed = key;
            }
        }

        private float KeyHeldAcceleration = 1.0f;
        private float KeyHeldWaitTime = 0.2f;
        private float KeyHeldAccelerationFactor = 1.1f;
        private Stopwatch KeyHeldStopWatch = new Stopwatch();

        private void KeyHeld(KeyboardState pressed, Keys key, Func<ErrorCodes> func)
        {
            if (pressed.IsKeyDown(key))
            {
                if (!KeyHeldStopWatch.IsRunning) {
                    func();
                    KeyHeldStopWatch.Start();
                } else if (KeyHeldStopWatch.Elapsed.TotalSeconds > KeyHeldWaitTime) {
                    func();
                    KeyHeldStopWatch.Restart();
                }
                LastPressed = key;
            }
            else if (pressed.IsKeyUp(key) && LastPressed.HasValue && LastPressed.Value == key)
            {
                Console.WriteLine(key.ToString());
                KeyHeldAcceleration = 1.0f;
                KeyHeldWaitTime = 0.2f;
                KeyHeldStopWatch.Reset();
                LastPressed = null;
            }
            
        }
        private void UpdateParamSelected(float increment) {
            switch (ParamSelected) {
                case ParticleParams.Param.MASS:
                    ParamValues.Mass += increment;
                    break;
                case ParticleParams.Param.CHARGE:
                    ParamValues.Charge += increment;
                    break;
                case ParticleParams.Param.RADIUS:
                    ParamValues.Radius += increment;
                    break;
                case ParticleParams.Param.POSITION:
                    if (Direction == DirectionSelected.X) {
                        ParamValues.Position.X += increment;
                    }
                    else if (Direction == DirectionSelected.Y) {
                        ParamValues.Position.Y += increment;
                    }
                    else {
                        ParamValues.Position.Z += increment;
                    }
                    break;
                case ParticleParams.Param.VELOCITY:
                    if (Direction == DirectionSelected.X) {
                        ParamValues.Velocity.X += increment;
                    }
                    else if (Direction == DirectionSelected.Y) {
                        ParamValues.Velocity.Y += increment;
                    }
                    else {
                        ParamValues.Velocity.Z += increment;
                    }
                    break;
            }
        }

        public ErrorCodes Update(KeyboardState keyboardState, MouseState mouseState, World world,
            GameTime time = null, GameState state = null)
        {
            KeyPress(keyboardState, Keys.Space, () =>
            {
                if (Flags.Contains(UiFlags.PAUSED))
                    Flags.Remove(UiFlags.PAUSED);
                else
                    Flags.Add(UiFlags.PAUSED);
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.Enter, () => {
                ParamValues.Texture = world.Textures["moon"];
                world.AddParticle(Particle.FromParams(ParamValues));
                return ErrorCodes.SUCCESS;
            });
            KeyHeld(keyboardState, Keys.OemComma, () => {
                UpdateParamSelected(-0.1f * KeyHeldAcceleration);
                KeyHeldAcceleration *= KeyHeldAccelerationFactor;
                KeyHeldWaitTime /= KeyHeldAccelerationFactor;
                return ErrorCodes.SUCCESS;
            });
            KeyHeld(keyboardState, Keys.OemPeriod, () => {
                UpdateParamSelected(0.1f * KeyHeldAcceleration);
                KeyHeldAcceleration *= KeyHeldAccelerationFactor;
                KeyHeldWaitTime /= KeyHeldAccelerationFactor;
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.X, () => {
                Direction = DirectionSelected.X;
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.Y, () => {
                Direction = DirectionSelected.Y;
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.Z, () => {
                Direction = DirectionSelected.Z;
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.T, () =>
            {
                if (Flags.Contains(UiFlags.TRACERS_ON))
                {
                    Flags.Remove(UiFlags.TRACERS_ON);
                    foreach (var p in world.Particles) p.ParticleTracer?.Reset();
                }
                else
                {
                    Flags.Add(UiFlags.TRACERS_ON);
                }

                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.Tab, () =>
            {
                ParamSelected = (ParticleParams.Param) (((int) ParamSelected + 1) % ParameterCount);
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.M, () =>
            {
                ParamSelected = ParticleParams.Param.MASS;
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.R, () =>
            {
                ParamSelected = ParticleParams.Param.RADIUS;
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.Q, () =>
            {
                ParamSelected = ParticleParams.Param.CHARGE;
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.P, () =>
            {
                ParamSelected = ParticleParams.Param.POSITION;
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.V, () =>
            {
                ParamSelected = ParticleParams.Param.VELOCITY;
                return ErrorCodes.SUCCESS;
            });
            KeyPress(keyboardState, Keys.Back, () =>
            {
                ParamValues.ResetParam(ParamSelected);
                return ErrorCodes.SUCCESS;
            });

            //TODO: select a particle edit stats delete copy params
            if (keyboardState.IsKeyUp(Keys.D) && LastPressed.HasValue && LastPressed.Value == Keys.D)
            {
                world.RemoveParticle(1);
                LastPressed = null;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                LastPressed = Keys.D;
            }

            if (mouseState.LeftButton == ButtonState.Released && LastClicked.HasValue &&
                LastClicked.Value == MouseButtonCode.LEFT)
            {
                ParamValues.Texture = world.Textures["moon"];
                world.AddParticle(Particle.FromParams(ParamValues));
                LastClicked = null;
            }
            else if (mouseState.LeftButton == ButtonState.Pressed && LastClicked != MouseButtonCode.LEFT)
            {
                LastClicked = MouseButtonCode.LEFT;
                ParamValues.Position = new Vector3(mouseState.X, mouseState.Y, 0);
            }

            Fps.Update(time);
            return ErrorCodes.SUCCESS;
        }

        public object Clone()
        {
            return _state;
        }

        private enum MouseButtonCode
        {
            LEFT,
            RIGHT,
            MIDDLE
        }
    }
}
