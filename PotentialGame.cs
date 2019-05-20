using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Potential
{
    public class PotentialGame : Game
    {
        private Dictionary<string, Texture2D> Textures = null;
        private SpriteFont Font = null;
        private World GameWorld = new World();
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
            GameWorld.AddParticle(new Particle(Textures["moon"], new Vector3(100, 100, 0), new Vector3(10, 0, 0), 50, mass: 0, energy: 5.0f));
            GameWorld.AddParticle(new Particle(Textures["blackhole"], new Vector3(300, 300, 0), new Vector3(0, 0, 0), 50, angular_velocity: -0.8f, isfixed: true));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Textures = new Dictionary<string, Texture2D>();
            Textures["moon"] = Content.Load<Texture2D>("moon");
            Textures["blackhole"] = Content.Load<Texture2D>("blackhole");
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            GameWorld.Draw(spriteBatch);
            spriteBatch.Begin();
            spriteBatch.DrawString(Font, $"{System.Math.Round(FPS.Framerate, 1)} fps", FPS.Position, Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
