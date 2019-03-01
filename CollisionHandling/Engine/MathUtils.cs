#region

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public static class MathUtils
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


        /// <summary>
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static Vector2[] Mul(ref Transform transform, Vector2[] vertices)
        {
            var verticesNew = new Vector2[vertices.Length];
            for (var i = 0; i < vertices.Length; i++)
                verticesNew[i] = Mul(ref transform, vertices[i]);

            return verticesNew;
        }


        /// <summary>
        /// </summary>
        /// <param name="T"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Mul(ref Transform T, Vector2 v)
        {
            return Mul(ref T, ref v);
        }


        /// <summary>
        /// </summary>
        /// <param name="T"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Mul(Transform T, Vector2 v)
        {
            return Mul(ref T, ref v);
        }


        /// <summary>
        /// </summary>
        /// <param name="T"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Mul(ref Transform T, ref Vector2 v)
        {
            var x = (T.Rotation.Cosine * v.X - T.Rotation.Sine * v.Y) + T.Position.X;
            var y = (T.Rotation.Sine * v.X + T.Rotation.Cosine * v.Y) + T.Position.Y;

            return new Vector2(x, y);
        }


        /// <summary>
        /// </summary>
        /// <param name="rot"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static Vector2 Mul(ref Rotation rot, Vector2 axis)
        {
            return Mul(rot, axis);
        }


        /// <summary>
        /// </summary>
        /// <param name="rot"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static Vector2 MulT(ref Rotation rot, Vector2 axis)
        {
            return MulT(rot, axis);
        }


        /// <summary>
        /// </summary>
        /// <param name="T"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 MulT(ref Transform T, Vector2 v)
        {
            return MulT(ref T, ref v);
        }


        /// <summary>
        /// </summary>
        /// <param name="T"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 MulT(ref Transform T, ref Vector2 v)
        {
            var px = v.X - T.Position.X;
            var py = v.Y - T.Position.Y;
            var x = (T.Rotation.Cosine * px + T.Rotation.Sine * py);
            var y = (-T.Rotation.Sine * px + T.Rotation.Cosine * py);

            return new Vector2(x, y);
        }


        /// <summary>
        ///     Rotate a vector
        /// </summary>
        /// <param name="q"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Mul(Rotation q, Vector2 v)
        {
            return new Vector2(q.Cosine * v.X - q.Sine * v.Y, q.Sine * v.X + q.Cosine * v.Y);
        }


        /// <summary>
        ///     Inverse rotate a vector
        /// </summary>
        /// <param name="q"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 MulT(Rotation q, Vector2 v)
        {
            return new Vector2(q.Cosine * v.X + q.Sine * v.Y, -q.Sine * v.X + q.Cosine * v.Y);
        }


        /// <summary>
        ///     Transpose multiply two rotations: qT * r
        /// </summary>
        /// <param name="q"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Rotation MulT(Rotation q, Rotation r)
        {
            // [ qc qs] * [rc -rs] = [qc*rc+qs*rs -qc*rs+qs*rc]
            // [-qs qc]   [rs  rc]   [-qs*rc+qc*rs qs*rs+qc*rc]
            // s = qc * rs - qs * rc
            // c = qc * rc + qs * rs
            Rotation qr;
            qr.Sine = q.Cosine * r.Sine - q.Sine * r.Cosine;
            qr.Cosine = q.Cosine * r.Cosine + q.Sine * r.Sine;
            return qr;
        }


        /// <summary>
        ///     v2 = A.q' * (B.q * v1 + B.p - A.p)
        ///     = A.q' * B.q * v1 + A.q' * (B.p - A.p)
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static Transform MulT(Transform A, Transform B)
        {
            var C = new Transform();
            C.Rotation = MulT(A.Rotation, B.Rotation);
            C.Position = MulT(A.Rotation, B.Position - A.Position);
            return C;
        }


        public static float Cross(ref Vector2 a, ref Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static float Cross(Vector2 a, Vector2 b)
        {
            return Cross(ref a, ref b);
        }

        /// Perform the cross product on two vectors.
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        public static Vector2 Cross(Vector2 a, float s)
        {
            return new Vector2(s * a.Y, -s * a.X);
        }

        public static Vector2 Cross(float s, Vector2 a)
        {
            return new Vector2(-s * a.Y, s * a.X);
        }

        public static Vector2 Abs(Vector2 v)
        {
            return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
        }

        /// Get the skew vector such that dot(skew_vec, other) == cross(vec, other)
        public static Vector2 Skew(Vector2 input)
        {
            return new Vector2(-input.Y, input.X);
        }

        /// <summary>
        ///     This function is used to ensure that a floating point number is
        ///     not a NaN or infinity.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>
        ///     <c>true</c> if the specified x is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(float x)
        {
            if (float.IsNaN(x))
            {
                // NaN.
                return false;
            }

            return !float.IsInfinity(x);
        }

        public static bool IsValid(this Vector2 x)
        {
            return IsValid(x.X) && IsValid(x.Y);
        }

        /// <summary>
        ///     This is a approximate yet fast inverse square-root.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static float InvSqrt(float x)
        {
            var convert = new FloatConverter();
            convert.x = x;
            var xhalf = 0.5f * x;
            convert.i = 0x5f3759df - (convert.i >> 1);
            x = convert.x;
            x = x * (1.5f - xhalf * x * x);
            return x;
        }

        public static int Clamp(int a, int low, int high)
        {
            return Math.Max(low, Math.Min(a, high));
        }

        public static float Clamp(float a, float low, float high)
        {
            return Math.Max(low, Math.Min(a, high));
        }

        public static Vector2 Clamp(Vector2 a, Vector2 low, Vector2 high)
        {
            return Vector2.Max(low, Vector2.Min(a, high));
        }

        public static void Cross(ref Vector2 a, ref Vector2 b, out float c)
        {
            c = a.X * b.Y - a.Y * b.X;
        }

        /// <summary>
        ///     Return the angle between two vectors on a plane
        ///     The angle is from vector 1 to vector 2, positive anticlockwise
        ///     The result is between -pi -> pi
        /// </summary>
        public static double VectorAngle(ref Vector2 p1, ref Vector2 p2)
        {
            var theta1 = Math.Atan2(p1.Y, p1.X);
            var theta2 = Math.Atan2(p2.Y, p2.X);
            var dtheta = theta2 - theta1;
            while (dtheta > Math.PI)
                dtheta -= (2 * Math.PI);
            while (dtheta < -Math.PI)
                dtheta += (2 * Math.PI);

            return (dtheta);
        }

        /// Perform the dot product on two vectors.
        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static double VectorAngle(Vector2 p1, Vector2 p2)
        {
            return VectorAngle(ref p1, ref p2);
        }

        /// <summary>
        ///     Returns a positive number if c is to the left of the line going from a to b.
        /// </summary>
        /// <returns>
        ///     Positive number if point is left, negative if point is right,
        ///     and 0 if points are collinear.
        /// </returns>
        public static float Area(Vector2 a, Vector2 b, Vector2 c)
        {
            return Area(ref a, ref b, ref c);
        }

        /// <summary>
        ///     Returns a positive number if c is to the left of the line going from a to b.
        /// </summary>
        /// <returns>
        ///     Positive number if point is left, negative if point is right,
        ///     and 0 if points are collinear.
        /// </returns>
        public static float Area(ref Vector2 a, ref Vector2 b, ref Vector2 c)
        {
            return a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y);
        }

        /// <summary>
        ///     Determines if three vertices are collinear (ie. on a straight line)
        /// </summary>
        /// <param name="a">First vertex</param>
        /// <param name="b">Second vertex</param>
        /// <param name="c">Third vertex</param>
        /// <param name="tolerance">The tolerance</param>
        /// <returns></returns>
        public static bool IsCollinear(ref Vector2 a, ref Vector2 b, ref Vector2 c, float tolerance = 0)
        {
            return FloatInRange(Area(ref a, ref b, ref c), -tolerance, tolerance);
        }

        public static void Cross(float s, ref Vector2 a, out Vector2 b)
        {
            b = new Vector2(-s * a.Y, s * a.X);
        }


        /// <summary>
        ///     Checks if a floating point Value is equal to another,
        ///     within a certain tolerance.
        /// </summary>
        /// <param name="value1">The first floating point Value.</param>
        /// <param name="value2">The second floating point Value.</param>
        /// <param name="delta">The floating point tolerance.</param>
        /// <returns>True if the values are "equal", false otherwise.</returns>
        public static bool FloatEquals(float value1, float value2, float delta)
        {
            return FloatInRange(value1, value2 - delta, value2 + delta);
        }

        /// <summary>
        ///     Checks if a floating point Value is within a specified
        ///     range of values (inclusive).
        /// </summary>
        /// <param name="value">The Value to check.</param>
        /// <param name="min">The minimum Value.</param>
        /// <param name="max">The maximum Value.</param>
        /// <returns>
        ///     True if the Value is within the range specified,
        ///     false otherwise.
        /// </returns>
        public static bool FloatInRange(float value, float min, float max)
        {
            return (value >= min && value <= max);
        }


        #region Nested type: FloatConverter

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatConverter
        {
            [FieldOffset(0)]
            public float x;

            [FieldOffset(0)]
            public int i;
        }

        #endregion
    }
}