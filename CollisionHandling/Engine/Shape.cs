#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// </summary>
    public static class VectorHelper
    {
        /// <summary>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 NormalizeWithNanCheck(this Vector2 vector)
        {
            // Richtung bestimmen
            var direction = Vector2.Normalize(vector);

            if (float.IsNaN(direction.X) || float.IsNaN(direction.Y))
                direction = Vector2.Zero;

            return direction;
        }


        /// <summary>
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }


        /// <summary>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
    }

    public abstract class Shape
    {
        public Color Color { get; set; }

        public Vector2 Velocity { get; set; }

        protected Shape()
        {
            this.Color = Color.Gray;
        }
    }
}