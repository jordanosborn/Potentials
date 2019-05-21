using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Potential
{
    public class PotentialGame : Game
    {
        private Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        private SpriteFont Font = null;
        private Color ColorFG = Color.White;
        private Color ColorBG = Color.Black;
        private World GameWorld = new World();
        private MouseCursor mouseCursor = null;
        GameState State = GameState.Create();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SmartFramerate FPS = new SmartFramerate(5, (0, 0));
        private void HandleResizeEvent(Object sender, EventArgs a)
        {
            if (Window.ClientBounds.Height < 600)
                graphics.PreferredBackBufferHeight = 600;
            if (Window.ClientBounds.Width < 600)
                graphics.PreferredBackBufferWidth = 600;
            graphics.ApplyChanges();
        }

        public PotentialGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 600;
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
            GameWorld.AddParticle(new Particle(Textures["moon"], new Vector3(100, 100, 0),
                new Vector3(10, 0, 0), 50, mass: 1.0f, energy: 5.0f));
            GameWorld.AddParticle(new Particle(Textures["blackhole"], new Vector3(300, 300, 0),
                new Vector3(0, 0, 0), 50, mass: 1.0f, angular_velocity: -0.8f, isfixed: true));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Textures["moon"] = Content.Load<Texture2D>("moon");
            Textures["blackhole"] = Content.Load<Texture2D>("blackhole");
            Textures["cursor"] = Content.Load<Texture2D>("cursor");
            Font = Content.Load<SpriteFont>("Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            World newWorld = (World)GameWorld.Clone();
            newWorld.Update(gameTime, GameWorld);
            GameWorld = newWorld;
            FPS.Update(gameTime);
            base.Update(gameTime);
        }

        protected void DrawUI(GameTime gameTime)
        {
            //FPS counter
            spriteBatch.DrawString(Font, $"{System.Math.Round(FPS.Framerate, 1)} fps", FPS.Position, ColorFG);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ColorBG);
            spriteBatch.Begin();
            GameWorld.Draw(spriteBatch);
            DrawUI(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
