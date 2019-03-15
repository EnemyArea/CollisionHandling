#region

using System;
using System.Collections.Generic;
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
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static int ConvertPositionToTilePosition(double coordinate)
        {
            return (int)(coordinate / TileSize);
        }


        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Point ConvertPositionToTilePosition(Vector2 position)
        {
            return new Point((int)(position.X / TileSize), (int)(position.Y / TileSize));
        }


        /// <summary>
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static Rectangle ConvertPositionToTilePosition(Rectangle rectangle)
        {
            return new Rectangle(
                ConvertPositionToTilePosition(rectangle.X),
                ConvertPositionToTilePosition(rectangle.Y),
                ConvertPositionToTilePosition(rectangle.Width),
                ConvertPositionToTilePosition(rectangle.Height));
        }


        /// <summary>
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2> CreatePreShiftVerticesFromRectangle(Rectangle rectangle)
        {
            yield return new Vector2(rectangle.Left, rectangle.Top);
            yield return new Vector2(rectangle.Right, rectangle.Top);
            yield return new Vector2(rectangle.Left, rectangle.Top);
            yield return new Vector2(rectangle.Left, rectangle.Bottom);
            yield return new Vector2(rectangle.Left, rectangle.Bottom);
            yield return new Vector2(rectangle.Right, rectangle.Bottom);
            yield return new Vector2(rectangle.Right, rectangle.Top);
            yield return new Vector2(rectangle.Right, rectangle.Bottom);
        }


        /// <summary>
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static IList<Vector2> CreateVerticesFromRectangle(Rectangle rectangle)
        {
            return new[]
            {
                new Vector2(rectangle.X, rectangle.Y),
                new Vector2(rectangle.X + rectangle.Width, rectangle.Y),
                new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height),
                new Vector2(rectangle.X, rectangle.Y + rectangle.Height)
            };
        }
    }
}