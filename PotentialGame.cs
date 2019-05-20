﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Potential
{
    public class PotentialGame : Game
    {
        private Texture2D ParticleTexture;
        private World GameWorld = new World();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
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
            ParticleTexture = Content.Load<Texture2D>("particle");
            GameWorld.AddParticle(new Particle(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 50), ParticleTexture);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            GameWorld.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
