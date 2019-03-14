using System;
using System.Diagnostics;
using System.Text;
using CollisionFloatTestNewMono.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// </summary>
    public sealed class FrameRateCounter
    {
        /// <summary>
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// </summary>
        private readonly Game game;

        /// <summary>
        /// </summary>
        private readonly GraphicsDeviceManager graphicsDeviceManager;

        /// <summary>
        /// </summary>
        private int frameRate;

        /// <summary>
        /// </summary>
        private int frameCounter;

        /// <summary>
        /// </summary>
        private TimeSpan elapsedTime = TimeSpan.Zero;
        
        /// <summary>
        /// </summary>
        private readonly TimeSpan oneSec = TimeSpan.FromSeconds(1);

        /// <summary>
        /// </summary>
        private readonly Vector2 fpsPosition = new Vector2(3, 0);

        /// <summary>
        /// </summary>
        private readonly Vector2 slowPosition = new Vector2(280, 0);

        /// <summary>
        /// </summary>
        private readonly Stopwatch drawTimer = new Stopwatch();

        /// <summary>
        /// </summary>
        private readonly Stopwatch updateTimer = new Stopwatch();

        /// <summary>
        /// </summary>
        private readonly StringBuilder stringBuilder = new StringBuilder();
        
        /// <summary>
        /// </summary>
        public bool DebugModus { get; set; }

        /// <summary>
        /// </summary>
        public TimeSpan PlayTime { get; private set; }

        /// <summary>
        /// </summary>
        public SpriteFont SpriteFont { get; private set; }


        /// <summary>
        /// </summary>
        /// <param name="game"></param>
        /// <param name="graphics"></param>
        public FrameRateCounter(Game game, GraphicsDeviceManager graphics)
        {
            this.game = game;
            this.graphicsDeviceManager = graphics;
            this.DebugModus = true;
        }


        /// <summary>
        /// </summary>
        public void LoadContent()
        {
            var resxContent = new ResourceContentManager(this.game.Services, Resources.ResourceManager);
            this.spriteBatch = new SpriteBatch(this.game.GraphicsDevice);
            this.SpriteFont = resxContent.Load<SpriteFont>("interfaceFontSmall");
        }


        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateFrameTimer(GameTime gameTime)
        {
            // Debugmodus
            this.elapsedTime += gameTime.ElapsedGameTime;
            this.PlayTime += gameTime.ElapsedGameTime;

            if (this.elapsedTime > this.oneSec)
            {
                this.elapsedTime -= this.oneSec;
                this.frameRate = this.frameCounter;
                this.frameCounter = 0;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        private void DrawTimers(GameTime gameTime)
        {
            // Debugmodus
            this.frameCounter++;

            this.stringBuilder.Length = 0;
            this.stringBuilder.Append("FPS: ");
            this.stringBuilder.Concat(this.frameRate);
            this.stringBuilder.Append(" T: ");
            this.stringBuilder.Append(this.PlayTime.ToString("hh\\:mm\\:ss"));
            this.stringBuilder.Append(" U: ");
            this.stringBuilder.Concat((float)this.updateTimer.Elapsed.TotalSeconds, 5);
            this.stringBuilder.Append(" D: ");
            this.stringBuilder.Concat((float)this.drawTimer.Elapsed.TotalSeconds, 5);

            this.spriteBatch.Begin();

            this.spriteBatch.DrawString(this.SpriteFont, this.stringBuilder, this.fpsPosition, Color.White);

            // Falls es mal wieder länger dauert, gibts trotzdem kein Mars
            if (gameTime.IsRunningSlowly)
                this.spriteBatch.DrawString(this.SpriteFont, "THE GAME IS RUNNING SLOW !!!", this.slowPosition, Color.White);

            this.spriteBatch.End();
        }


        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public void StartUpdateTimer(GameTime gameTime)
        {
            // Debugmodus
            if (this.DebugModus)
            {
                this.updateTimer.Restart();
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public void EndUpdateTimer(GameTime gameTime)
        {
            // Debugmodus
            if (this.DebugModus)
            {
                this.UpdateFrameTimer(gameTime);
                this.updateTimer.Stop();
            }
        }


        /// <summary>
        /// </summary>
        public void StartDrawTimer()
        {
            // Debugmodus
            if (this.DebugModus)
            {
                this.drawTimer.Restart();
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public void EndDrawTimer(GameTime gameTime)
        {
            // Debugmodus
            if (this.DebugModus)
            {
                this.drawTimer.Stop();

                // Anzeigen
                this.DrawTimers(gameTime);
            }
        }
    }
}