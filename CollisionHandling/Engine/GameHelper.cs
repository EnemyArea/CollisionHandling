#region

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
        ///     Returns the convex hull from the given vertices.
        ///     Giftwrap convex hull algorithm.
        ///     O(n * h) time complexity, where n is the number of points and h is the number of points on the convex hull.
        ///     See http://en.wikipedia.org/wiki/Gift_wrapping_algorithm for more details.
        ///     Extracted from Box2D
        ///     https://github.com/VelcroPhysics/VelcroPhysics/blob/1456abf40e4c30065bf122f409ce60ce3873ff09/VelcroPhysics/Tools/ConvexHull/GiftWrap/GiftWrap.cs
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        public static IList<Vector2> GetConvexHull(IList<Vector2> vertices)
        {
            if (vertices.Count <= 3)
                return vertices;

            // Find the right most point on the hull
            var i0 = 0;
            var x0 = vertices[0].X;
            for (var i = 1; i < vertices.Count; ++i)
            {
                var x = vertices[i].X;
                if (x > x0 || (x == x0 && vertices[i].Y < vertices[i0].Y))
                {
                    i0 = i;
                    x0 = x;
                }
            }

            var hull = new int[vertices.Count];
            var m = 0;
            var ih = i0;

            while (true)
            {
                hull[m] = ih;

                var ie = 0;
                for (var j = 1; j < vertices.Count; ++j)
                {
                    if (ie == ih)
                    {
                        ie = j;
                        continue;
                    }

                    var r = vertices[ie] - vertices[hull[m]];
                    var v = vertices[j] - vertices[hull[m]];
                    var c = MathUtils.Cross(ref r, ref v);
                    if (c < 0.0f)
                        ie = j;

                    // Collinearity check
                    if (c == 0.0f && v.LengthSquared() > r.LengthSquared())
                        ie = j;
                }

                ++m;
                ih = ie;

                if (ie == i0)
                {
                    break;
                }
            }

            var result = new List<Vector2>(m);

            // Copy vertices.
            for (var i = 0; i < m; ++i)
                result.Add(vertices[hull[i]]);

            return result;
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
        public static IEnumerable<Vector2> CreateVerticesFromRectangle(Rectangle rectangle)
        {
            yield return new Vector2(0, 0);
            yield return new Vector2(rectangle.Width, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, rectangle.Height);
            yield return new Vector2(0, rectangle.Height);
            yield return new Vector2(rectangle.Width, rectangle.Height);
            yield return new Vector2(rectangle.Width, 0);
            yield return new Vector2(rectangle.Width, rectangle.Height);
        }
    }
}