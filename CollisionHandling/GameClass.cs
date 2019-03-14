#region

using CollisionFloatTestNewMono.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace CollisionFloatTestNewMono
{
    /// <summary>
    ///     This is the main type for your game
    /// </summary>
    public class GameClass : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private RenderEngine renderEngine;
        private FrameRateCounter frameRateCounter;


        /// <summary>
        /// </summary>
        public GameClass()
        {
            // Fenster verkleinern
            this.graphics = new GraphicsDeviceManager(this);
            this.graphics.PreferredBackBufferWidth = 1280;
            this.graphics.PreferredBackBufferHeight = 720;
            this.IsMouseVisible = true;
            this.Content.RootDirectory = "Content";
        }


        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.frameRateCounter = new FrameRateCounter(this, this.graphics);
            this.frameRateCounter.LoadContent();

            this.renderEngine = new RenderEngine();
            this.renderEngine.LoadMap(this.GraphicsDevice, this.Content);
        }


        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            this.frameRateCounter.StartUpdateTimer(gameTime);

            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            this.renderEngine.Update(gameTime);

            // Update ausführen
            base.Update(gameTime);

            this.frameRateCounter.EndUpdateTimer(gameTime);
        }


        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.frameRateCounter.StartDrawTimer();

            // Alles clearen
            this.GraphicsDevice.Clear(Color.Black);

            // Karte zeichnen
            this.renderEngine.Draw(gameTime, this.spriteBatch);

            // Draw ausführen
            base.Draw(gameTime);

            this.frameRateCounter.EndDrawTimer(gameTime);
        }
    }
}