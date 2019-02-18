#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// </summary>
    public static class GameHelper
    {
        /// <summary>
        /// </summary>
        public const int TileSize = 32;


        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public static float GetElapsedSecondsFromGameTime(GameTime gameTime)
        {
            return (float)gameTime.ElapsedGameTime.TotalSeconds;
        }


        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        public static float GetTotalSecondsFromGameTime(GameTime gameTime)
        {
            return (float)gameTime.TotalGameTime.TotalSeconds;
        }


        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Point ConvertPositionToTilePosition(Vector2 position)
        {
            return new Point((int)(position.X / TileSize), (int)(position.Y / TileSize));
        }
    }
}