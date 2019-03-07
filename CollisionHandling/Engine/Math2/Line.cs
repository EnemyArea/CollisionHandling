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
        ///     Determines if line1 intersects line2, when line1 is offset by pos1 and line2
        ///     is offset by pos2.
        /// </summary>
        /// <param name="line1">Line 1</param>
        /// <param name="line">Line 2</param>
        /// <param name="pos1">Origin of line 1</param>
        /// <param name="pos2">Origin of line 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If line1 intersects line2</returns>
        public static bool Intersects(Line line1, Line line, Vector2 pos1, Vector2 pos2, bool strict)
        {
            if (line1.Horizontal && line.Horizontal)
            {
                return AxisAlignedLine.Intersects(line1.MinX + pos1.X, line1.MaxX + pos1.X, line.MinX + pos2.X, line.MaxX + pos2.X, strict, false);
            }

            if (line1.Vertical && line.Vertical)
            {
                return AxisAlignedLine.Intersects(line1.MinY + pos1.Y, line1.MaxY + pos1.Y, line.MinY + pos2.Y, line.MaxY + pos2.Y, strict, false);
            }

            if (line1.Horizontal || line.Horizontal)
            {
                if (line.Horizontal)
                {
                    // swap line 1 and 2 to prevent duplicating everything
                    var tmp = line1;
                    var tmpp = pos1;
                    line1 = line;
                    pos1 = pos2;
                    line = tmp;
                    pos2 = tmpp;
                }

                if (line.Vertical)
                {
                    return AxisAlignedLine.Contains(line1.MinX + pos1.X, line1.MaxX + pos1.X, line.Start.X + pos2.X, strict, false)
                           && AxisAlignedLine.Contains(line.MinY + pos2.Y, line.MaxY + pos2.Y, line1.Start.Y + pos1.Y, strict, false);
                }

                // recalculate line2 y intercept
                // y = mx + b
                // b = y - mx
                var line2YIntInner = line.Start.Y + pos2.Y - line.Slope * (line.Start.X + pos2.X);
                // check line2.x at line1.y
                // line2.y = line2.slope * line2.x + line2.yintercept
                // line1.y = line2.slope * line2.x + line2.yintercept
                // line1.y - line2.yintercept = line2.slope * line2.x
                // (line1.y - line2.yintercept) / line2.slope = line2.x
                var line2XAtLine1Y = (line1.Start.Y + pos1.Y - line2YIntInner) / line.Slope;
                return AxisAlignedLine.Contains(line1.MinX + pos1.X, line1.MaxX + pos1.X, line2XAtLine1Y, strict, false)
                       && AxisAlignedLine.Contains(line.MinX + pos2.X, line.MaxX + pos2.X, line2XAtLine1Y, strict, false);
            }

            if (line1.Vertical)
            {
                // vertical line with regular line
                var line2YIntInner = line.Start.Y + pos2.Y - line.Slope * (line.Start.X + pos2.X);
                var line2YAtLine1X = line.Slope * (line1.Start.X + pos1.X) + line2YIntInner;
                return AxisAlignedLine.Contains(line1.MinY + pos1.Y, line1.MaxY + pos1.Y, line2YAtLine1X, strict, false)
                       && AxisAlignedLine.Contains(line.MinY + pos2.Y, line.MaxY + pos2.Y, line2YAtLine1X, strict, false);
            }

            // two non-vertical, non-horizontal lines
            var line1YInt = line1.Start.Y + pos1.Y - line1.Slope * (line1.Start.X + pos1.X);
            var line2YInt = line.Start.Y + pos2.Y - line.Slope * (line.Start.X + pos2.X);

            if (Math.Abs(line1.Slope - line.Slope) <= MathHelper.DefaultEpsilon)
            {
                // parallel lines
                if (line1YInt != line2YInt)
                    return false; // infinite lines don't intersect

                // parallel lines with equal y intercept. Intersect if ever at same X coordinate.
                return AxisAlignedLine.Intersects(line1.MinX + pos1.X, line1.MaxX + pos1.X, line.MinX + pos2.X, line.MaxX + pos2.X, strict, false);
            }
            // two non-parallel lines. Only one possible intersection point

            // y1 = y2
            // line1.Slope * x + line1.YIntercept = line2.Slope * x + line2.YIntercept
            // line1.Slope * x - line2.Slope * x = line2.YIntercept - line1.YIntercept
            // x (line1.Slope - line2.Slope) = line2.YIntercept - line1.YIntercept
            // x = (line2.YIntercept - line1.YIntercept) / (line1.Slope - line2.Slope)
            var x = (line2YInt - line1YInt) / (line1.Slope - line.Slope);

            return AxisAlignedLine.Contains(line1.MinX + pos1.X, line1.MaxX + pos1.X, x, strict, false)
                   && AxisAlignedLine.Contains(line.MinX + pos1.X, line.MaxX + pos2.X, x, strict, false);
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