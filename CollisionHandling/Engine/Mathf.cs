#region

using System;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
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