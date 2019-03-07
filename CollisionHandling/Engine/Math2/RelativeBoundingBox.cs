#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Math2
{
    /// <summary>
    ///     Describes a rectangle that is describing the percentages to go
    ///     of the true rectangle. Useful in some UI circumstances.
    /// </summary>
    public class RelativeBoundingBox : BoundingBox
    {
        /// <summary>
        ///     Create a new relative rectangle
        /// </summary>
        /// <param name="min">vector of smallest x and y coordinates</param>
        /// <param name="max">vector of largest x and y coordinates</param>
        public RelativeBoundingBox(Vector2 min, Vector2 max)
            : base(min, max)
        {
        }

        /// <summary>
        ///     Create a new relative rectangle
        /// </summary>
        /// <param name="x">smallest x</param>
        /// <param name="y">smallest y</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        public RelativeBoundingBox(float x, float y, float w, float h)
            : base(new Vector2(x, y), new Vector2(x + w, y + h))
        {
        }

        /// <summary>
        ///     Multiply our min with original min and our max with original max and return
        ///     as a rect
        /// </summary>
        /// <param name="original">the original</param>
        /// <returns>scaled rect</returns>
        public BoundingBox ToRect(BoundingBox original)
        {
            return new BoundingBox(original.Min * this.Min, original.Max * this.Max);
        }

        /// <summary>
        ///     Multiply our min with original min and our max with original max and return
        ///     as a rect
        /// </summary>
        /// <param name="original">the monogame original</param>
        /// <returns>the rect</returns>
        public BoundingBox ToRect(Rectangle original)
        {
            return new BoundingBox(
                new Vector2(original.Left + original.Width * this.Min.X, original.Top + original.Height * this.Min.Y),
                new Vector2(original.Left + original.Width * this.Max.X, original.Top + original.Height * this.Max.Y)
            );
        }
    }
}