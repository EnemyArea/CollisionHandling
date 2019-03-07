﻿#region

using System;

#endregion

namespace CollisionFloatTestNewMono.Engine.Math2
{
    /// <summary>
    ///     Describes a rotation about the z axis, with sin and cos of theta
    ///     cached.
    /// </summary>
    public struct Rotation
    {
        /// <summary>
        ///     Rotation Theta=0
        /// </summary>
        public static readonly Rotation Zero = new Rotation(0, 1, 0);

        /// <summary>
        ///     Theta in radians.
        /// </summary>
        public readonly float Theta;

        /// <summary>
        ///     Math.Cos(Theta)
        /// </summary>
        public readonly float CosTheta;

        /// <summary>
        ///     Math.Sin(Theta)
        /// </summary>
        public readonly float SinTheta;

        /// <summary>
        ///     Create a new rotation by specifying the theta, its cosin, and its sin.
        ///     Theta will be normalized to 0 &lt;= theta &lt;= 2pi
        /// </summary>
        /// <param name="theta"></param>
        /// <param name="cosTheta"></param>
        /// <param name="sinTheta"></param>
        public Rotation(float theta, float cosTheta, float sinTheta)
        {
            if (float.IsInfinity(theta) || float.IsNaN(theta))
                throw new ArgumentException($"Invalid theta: {theta}", nameof(theta));

            if (theta < 0)
            {
                var numToAdd = (int)Math.Ceiling(-theta / (Math.PI * 2));
                theta += (float)Math.PI * 2 * numToAdd;
            }
            else if (theta >= Math.PI * 2)
            {
                var numToReduce = (int)Math.Floor(theta / (Math.PI * 2));
                theta -= (float)Math.PI * 2 * numToReduce;
            }

            this.Theta = theta;
            this.CosTheta = cosTheta;
            this.SinTheta = sinTheta;
        }

        /// <summary>
        ///     Create a new rotation at the specified theta, calculating the cos and sin.
        ///     Theta will be normalized to 0 &lt;= theta &lt;= 2pi
        /// </summary>
        /// <param name="theta"></param>
        public Rotation(float theta)
            : this(theta, (float)Math.Cos(theta), (float)Math.Sin(theta))
        {
        }

        /// <summary>
        ///     Determine if the two rotations have the same theta
        /// </summary>
        /// <param name="r1">First rotation</param>
        /// <param name="r2">Second rotation</param>
        /// <returns>if r1 and r2 are the same logical rotation</returns>
        public static bool operator ==(Rotation r1, Rotation r2)
        {
            return r1.Theta == r2.Theta;
        }

        /// <summary>
        ///     Determine if the two rotations are not the same
        /// </summary>
        /// <param name="r1">first rotation</param>
        /// <param name="r2">second rotation</param>
        /// <returns>if r1 and r2 are not the same logical rotation</returns>
        public static bool operator !=(Rotation r1, Rotation r2)
        {
            return r1.Theta != r2.Theta;
        }

        /// <summary>
        ///     Determine if obj is a rotation that is logically equal to this one
        /// </summary>
        /// <param name="obj">the object</param>
        /// <returns>if it is logically equal</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Rotation))
                return false;

            return this == (Rotation)obj;
        }

        /// <summary>
        ///     The hashcode of this rotation based on just Theta
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Theta.GetHashCode();
        }

        /// <summary>
        ///     Create a human-readable representation of this rotation
        /// </summary>
        /// <returns>string representation</returns>
        public override string ToString()
        {
            return $"{this.Theta} rads";
        }
    }
}