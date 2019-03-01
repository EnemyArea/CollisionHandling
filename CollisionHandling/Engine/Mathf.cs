#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    
    /// <summary>
    /// Rotation
    /// </summary>
    public struct Rot
    {
        /// Sine and cosine
        public float s,
            c;

        /// <summary>
        /// Initialize from an angle in radians
        /// </summary>
        /// <param name="angle">Angle in radians</param>
        public Rot(float angle)
        {
            // TODO_ERIN optimize
            s = (float)Math.Sin(angle);
            c = (float)Math.Cos(angle);
        }

        /// <summary>
        /// Set using an angle in radians.
        /// </summary>
        /// <param name="angle"></param>
        public void Set(float angle)
        {
            //Velcro: Optimization
            if (angle == 0)
            {
                s = 0;
                c = 1;
            }
            else
            {
                // TODO_ERIN optimize
                s = (float)Math.Sin(angle);
                c = (float)Math.Cos(angle);
            }
        }

        /// <summary>
        /// Set to the identity rotation
        /// </summary>
        public void SetIdentity()
        {
            s = 0.0f;
            c = 1.0f;
        }

        /// <summary>
        /// Get the angle in radians
        /// </summary>
        public float GetAngle()
        {
            return (float)Math.Atan2(s, c);
        }

        /// <summary>
        /// Get the x-axis
        /// </summary>
        public Vector2 GetXAxis()
        {
            return new Vector2(c, s);
        }

        /// <summary>
        /// Get the y-axis
        /// </summary>
        public Vector2 GetYAxis()
        {
            return new Vector2(-s, c);
        }
    }

    
    /// <summary>
    /// A transform contains translation and rotation. It is used to represent
    /// the position and orientation of rigid frames.
    /// </summary>
    public struct Transform
    {
        public Vector2 p;
        public Rot q;

        /// <summary>
        /// Initialize using a position vector and a rotation matrix.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The r.</param>
        public Transform(Vector2 position, Rot rotation)
        {
            p = position;
            q = rotation;
        }

        /// <summary>
        /// Set this to the identity transform.
        /// </summary>
        public void SetIdentity()
        {
            p = Vector2.Zero;
            q.SetIdentity();
        }

        /// <summary>
        /// Set this based on the position and angle.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="angle">The angle.</param>
        public void Set(Vector2 position, float angle)
        {
            p = position;
            q.Set(angle);
        }
    }


    /// <summary>
    /// </summary>
    public static class Mathf
    {
        /// <summary>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float Floor(float f)
        {
            return (float)Math.Floor(f);
        }


        /// <summary>
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static float Abs(float f)
        {
            return Math.Abs(f);
        }


        /// <summary>
        /// </summary>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static float Repeat(float t, float length)
        {
            return t - Floor(t / length) * length;
        }


        /// <summary>
        ///     PingPongs the value t, so that it is never larger than length and never smaller than 0.
        ///     The returned value will move back and forth between 0 and length.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static float PingPong(float time, float length)
        {
            time = Repeat(time, length * 2f);
            return length - Abs(time - length);
        }
    }
}