#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    ///     Describes a rectangle. Meant to be reused.
    /// </summary>
    public struct AABB
    {
        /// <summary>
        ///     The corner with the smallest x and y coordinates on this
        ///     rectangle.
        /// </summary>
        public readonly Vector2 Min;

        /// <summary>
        ///     The corner with the largest x and y coordinates on this
        ///     rectangle
        /// </summary>
        public readonly Vector2 Max;

        /// <summary>
        ///     The corner with the largest x and smallest y coordinates on
        ///     this rectangle
        /// </summary>
        public readonly Vector2 UpperRight;

        /// <summary>
        ///     The corner with the smallest x and largest y coordinates on this
        ///     rectangle
        /// </summary>
        public readonly Vector2 LowerLeft;

        /// <summary>
        ///     The center of this rectangle
        /// </summary>
        public readonly Vector2 Center;

        /// <summary>
        ///     The width of this rectangle
        /// </summary>
        public readonly float Width;

        /// <summary>
        ///     The height of this rectangle
        /// </summary>
        public readonly float Height;


        /// <summary>
        ///     Creates a bounding box with the specified upper-left and bottom-right.
        ///     Will autocorrect if min.X > max.X or min.Y > max.Y
        /// </summary>
        /// <param name="min">Min x, min y</param>
        /// <param name="max">Max x, max y</param>
        /// <exception cref="ArgumentException">If min and max do not make a box</exception>
        public AABB(Vector2 min, Vector2 max)
        {
            if (MathUtils.Approximately(min, max))
                throw new ArgumentException($"Min is approximately max: min={min}, max={max} - tha'ts a point, not a box");
            if (Math.Abs(min.X - max.X) <= MathUtils.DefaultEpsilon)
                throw new ArgumentException($"Min x is approximately max x: min={min}, max={max} - that's a line, not a box");
            if (Math.Abs(min.Y - max.Y) <= MathUtils.DefaultEpsilon)
                throw new ArgumentException($"Min y is approximately max y: min={min}, max={max} - that's a line, not a box");

            float tmpX1 = min.X, tmpX2 = max.X;
            float tmpY1 = min.Y, tmpY2 = max.Y;

            min.X = Math.Min(tmpX1, tmpX2);
            min.Y = Math.Min(tmpY1, tmpY2);
            max.X = Math.Max(tmpX1, tmpX2);
            max.Y = Math.Max(tmpY1, tmpY2);

            this.Min = min;
            this.Max = max;
            this.UpperRight = new Vector2(this.Max.X, this.Min.Y);
            this.LowerLeft = new Vector2(this.Min.X, this.Max.Y);

            this.Center = new Vector2((this.Min.X + this.Max.X) / 2, (this.Min.Y + this.Max.Y) / 2);

            this.Width = this.Max.X - this.Min.X;
            this.Height = this.Max.Y - this.Min.Y;
        }
    }
}