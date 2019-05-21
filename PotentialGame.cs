﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Potential
{
    public class PotentialGame : Game
    {
        private Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        private (int, int) DefaultSize = (600, 600);
        private (int, int) MinSize = (600, 600);
        private SpriteFont Font = null;
        private Color ColorFG = Color.White;
        private Color ColorBG = Color.Black;
        private World GameWorld = new World();
        private MouseCursor mouseCursor = null;
        GameState State = GameState.GetState();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Utilities.SmartFramerate FPS = new Utilities.SmartFramerate(5, (4, 0));
        private void HandleResizeEvent(Object sender, EventArgs a)
        {
            if (Window.ClientBounds.Height < MinSize.Item2)
                graphics.PreferredBackBufferHeight = MinSize.Item2;
            if (Window.ClientBounds.Width < MinSize.Item1)
                graphics.PreferredBackBufferWidth = MinSize.Item1;
            graphics.ApplyChanges();
        }

        public PotentialGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = DefaultSize.Item1;
            graphics.PreferredBackBufferHeight = DefaultSize.Item2;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "Potential";
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += HandleResizeEvent;
            LoadContent();
        }

        protected override void Initialize()
        {
            mouseCursor = MouseCursor.FromTexture2D(Textures["cursor"],
                Textures["cursor"].Width / 2, Textures["cursor"].Height / 2);
            Mouse.SetCursor(mouseCursor);
            GameWorld.AddParticle(new Particle(Textures["blackhole"], new Vector3(300, 300, 0),
                new Vector3(0, 0, 0), 50, mass: 50000.0f, angular_velocity: -0.8f, isfixed: false));
            GameWorld.Particles[0].ParticleTracer = new Tracer(Textures["tracer"], GameWorld.Particles[0].Position, color: Color.Red);
            GameWorld.AddParticle(new Particle(Textures["moon"], new Vector3(200, 200, 0),
                new Vector3(50, 0, 0), 50, mass: 50_000.0f));
            GameWorld.Particles[1].ParticleTracer = new Tracer(Textures["tracer"], GameWorld.Particles[1].Position, color: Color.Blue);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Textures["moon"] = Content.Load<Texture2D>("moon");
            Textures["blackhole"] = Content.Load<Texture2D>("blackhole");
            Textures["cursor"] = Content.Load<Texture2D>("cursor");
            Textures["tracer"] = Content.Load<Texture2D>("circle");
            Textures["background"] = Content.Load<Texture2D>("background");
            Font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                State.IsPaused = !State.IsPaused;
            }
            if (!State.IsPaused)
            {
                World newWorld = (World)GameWorld.Clone();
                newWorld.Update(gameTime, GameWorld);
                GameWorld = newWorld;
                FPS.Update(gameTime);
                base.Update(gameTime);
            }
        }

        protected void DrawUI(GameTime gameTime)
        {
            var mouse_state = Mouse.GetState();
            int X = mouse_state.X, Y = mouse_state.Y;
            if ((mouse_state.X < 0 || mouse_state.X > Window.ClientBounds.Width) ||
                (mouse_state.Y < 0 || mouse_state.Y > Window.ClientBounds.Height))
            {
                X = 0;
                Y = 0;
            }
            var spacing = "    ";
            var s = new StringBuilder($"{System.Math.Round(FPS.Framerate, 0)}FPS");
            s.Append(spacing);
            s.Append($"({X}, {Y})");
            s.Append(spacing);
            if (State.IsPaused)
                s.Append("PAUSED");
            s.Append(spacing);
            //TODO: draw item selection so can click and create
            spriteBatch.DrawString(Font, s.ToString(), FPS.Position, ColorFG);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ColorBG);
            spriteBatch.Begin();
            var bg = Textures["background"];
            spriteBatch.Draw(bg, position: Vector2.Zero,
                origin: Vector2.Zero, color: Color.White,
                scale: new Vector2(Window.ClientBounds.Width / (float)bg.Width, Window.ClientBounds.Height / (float)bg.Height)
            );
            GameWorld.Draw(spriteBatch);
            DrawUI(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
