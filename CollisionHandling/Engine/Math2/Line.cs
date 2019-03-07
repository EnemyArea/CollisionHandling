#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Math2
{
    /// <summary>
    ///     Describes a line. Does not have position and is meant to be reused.
    /// </summary>
    public class Line
    {
        /// <summary>
        ///     Where the line begins
        /// </summary>
        public readonly Vector2 Start;

        /// <summary>
        ///     Where the line ends
        /// </summary>
        public readonly Vector2 End;

        /// <summary>
        ///     End - Start
        /// </summary>
        public readonly Vector2 Delta;

        /// <summary>
        ///     Normalized Delta
        /// </summary>
        public readonly Vector2 Axis;

        /// <summary>
        ///     The normalized normal of axis.
        /// </summary>
        public readonly Vector2 Normal;

        /// <summary>
        ///     Square of the magnitude of this line
        /// </summary>
        public readonly float MagnitudeSquared;

        /// <summary>
        ///     Magnitude of this line
        /// </summary>
        public readonly float Magnitude;

        /// <summary>
        ///     Min x
        /// </summary>
        public readonly float MinX;

        /// <summary>
        ///     Min y
        /// </summary>
        public readonly float MinY;

        /// <summary>
        ///     Max x
        /// </summary>
        public readonly float MaxX;

        /// <summary>
        ///     Max y
        /// </summary>
        public readonly float MaxY;

        /// <summary>
        ///     Slope of this line
        /// </summary>
        public readonly float Slope;

        /// <summary>
        ///     Where this line would hit the y intercept. NaN if vertical line.
        /// </summary>
        public readonly float YIntercept;

        /// <summary>
        ///     If this line is horizontal
        /// </summary>
        public readonly bool Horizontal;

        /// <summary>
        ///     If this line is vertical
        /// </summary>
        public readonly bool Vertical;


        /// <summary>
        ///     Creates a line from start to end
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        public Line(Vector2 start, Vector2 end)
        {
            if (MathHelper.Approximately(start, end))
                throw new ArgumentException($"start is approximately end - that's a point, not a line. start={start}, end={end}");

            this.Start = start;
            this.End = end;

            this.Delta = this.End - this.Start;
            this.Axis = Vector2.Normalize(this.Delta);
            this.Normal = Vector2.Normalize(MathHelper.Perpendicular(this.Delta));
            this.MagnitudeSquared = this.Delta.LengthSquared();
            this.Magnitude = (float)Math.Sqrt(this.MagnitudeSquared);

            this.MinX = Math.Min(this.Start.X, this.End.X);
            this.MinY = Math.Min(this.Start.Y, this.End.Y);
            this.MaxX = Math.Max(this.Start.X, this.End.X);
            this.MaxY = Math.Max(this.Start.X, this.End.X);

            var k = Math.Abs(this.End.Y - this.Start.Y) / Math.Abs(this.End.X - this.Start.X);
            this.Horizontal = (int)k == 0;
            this.Vertical = float.IsInfinity(k);

            if (this.Vertical)
            {
                this.Slope = float.PositiveInfinity;
            }
            else
            {
                this.Slope = (this.End.Y - this.Start.Y) / (this.End.X - this.Start.X);
            }

            if (this.Vertical)
            {
                this.YIntercept = float.NaN;
            }
            else
            {
                // y = mx + b
                // Start.Y = Slope * Start.X + b
                // b = Start.Y - Slope * Start.X
                this.YIntercept = this.Start.Y - this.Slope * this.Start.X;
            }
        }


        /// <summary>
        ///     Create a human-readable representation of this line
        /// </summary>
        /// <returns>human-readable string</returns>
        public override string ToString()
        {
            return $"[{this.Start} to {this.End}]";
        }
    }
}