#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    ///     Describes a line that's projected onto a specified axis. This is a useful
    ///     mathematical concept. Axis aligned lines *do* have position because they
    ///     are only used as an interim calculation, where position won't change.
    /// </summary>
    public struct AxisAlignedLine
    {
        /// <summary>
        ///     The axis that this projected line is on. Optional.
        /// </summary>
        public readonly Vector2 Axis;

        /// <summary>
        ///     The minimum of this line
        /// </summary>
        public readonly float Min;

        /// <summary>
        ///     The maximum of this line
        /// </summary>
        public readonly float Max;


        /// <summary>
        ///     Initializes an an axis aligned line. Will autocorrect if min &gt; max
        /// </summary>
        /// <param name="axis">The axis</param>
        /// <param name="min">The min</param>
        /// <param name="max">The max</param>
        public AxisAlignedLine(Vector2 axis, float min, float max)
        {
            this.Axis = axis;

            this.Min = Math.Min(min, max);
            this.Max = Math.Max(min, max);
        }


        /// <summary>
        ///     Creates a human-readable representation of this line
        /// </summary>
        /// <returns>string representation of this vector</returns>
        public override string ToString()
        {
            return $"[{this.Min} -> {this.Max} along {this.Axis}]";
        }
    }
}