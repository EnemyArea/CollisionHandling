#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    ///     Rotation
    /// </summary>
    public struct Rotation
    {
        /// <summary>
        /// Sine
        /// </summary>
        public float Sine;

        /// <summary>
        /// Cosine
        /// </summary>
        public float Cosine;


        /// <summary>
        ///     Initialize from an angle in radians
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        public Rotation(float angle)
        {
            // TODO_ERIN optimize
            this.Sine = (float)Math.Sin(angle);
            this.Cosine = (float)Math.Cos(angle);
        }


        /// <summary>
        ///     Set using an angle in radians.
        /// </summary>
        /// <param name="angle"></param>
        public void Set(float angle)
        {
            //Velcro: Optimization
            if (angle == 0)
            {
                this.Sine = 0;
                this.Cosine = 1;
            }
            else
            {
                // TODO_ERIN optimize
                this.Sine = (float)Math.Sin(angle);
                this.Cosine = (float)Math.Cos(angle);
            }
        }


        /// <summary>
        ///     Set to the identity rotation
        /// </summary>
        public void SetIdentity()
        {
            this.Sine = 0.0f;
            this.Cosine = 1.0f;
        }


        /// <summary>
        ///     Get the angle in radians
        /// </summary>
        public float GetAngle()
        {
            return (float)Math.Atan2(this.Sine, this.Cosine);
        }


        /// <summary>
        ///     Get the x-axis
        /// </summary>
        public Vector2 GetXAxis()
        {
            return new Vector2(this.Cosine, this.Sine);
        }


        /// <summary>
        ///     Get the y-axis
        /// </summary>
        public Vector2 GetYAxis()
        {
            return new Vector2(-this.Sine, this.Cosine);
        }
    }
}