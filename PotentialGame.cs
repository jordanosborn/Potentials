using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
        GameState State = null;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
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
            GameWorld.AddParticle(new Particle(Textures["blackhole"], new Vector3(400, 400, 0),
                new Vector3(0, -50, 0), 50, mass: 0, angular_velocity: -0.8f, isfixed: false));
            GameWorld.Particles[0].ParticleTracer = new Tracer(Textures["tracer"], GameWorld.Particles[0], color: Color.Red);
            GameWorld.AddParticle(new Particle(Textures["moon"], new Vector3(400, 400, 0),
                new Vector3(0, 50, 0), 50, mass: 0));
            GameWorld.Particles[1].ParticleTracer = new Tracer(Textures["tracer"], GameWorld.Particles[1], color: Color.Blue);
            GameWorld.Particles[1].AddInterParticleForceSymmetric(
            GameWorld.Particles[0],
            Force.Factory.SpringForce(0.001f, 0.0f)
            );
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
            State = GameState.GetState(Font);
        }

        protected override void Update(GameTime gameTime)
        {
            ButtonState[] MouseButtonStates = { Mouse.GetState().LeftButton, Mouse.GetState().MiddleButton, Mouse.GetState().RightButton };
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (MouseButtonStates.Any((b) => b == ButtonState.Pressed))
            {
                Mouse.SetCursor(MouseCursor.Crosshair);
            }
            else if (MouseButtonStates.Any((b) => b == ButtonState.Released))
            {
                Mouse.SetCursor(mouseCursor);
            }
            State.Update(Keyboard.GetState(), Mouse.GetState(), gameTime, GameWorld, null);
            if (!State.IsPaused)
            {
                World newWorld = (World)GameWorld.Clone();
                newWorld.Update(gameTime, GameWorld);
                GameWorld = newWorld;
                base.Update(gameTime);
            }
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
            State.Draw(spriteBatch, Window);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
