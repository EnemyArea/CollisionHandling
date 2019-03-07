#region

using System;
using System.Collections.Generic;
using CollisionFloatTestNewMono.Engine.Math2;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// </summary>
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
        ///     Default epsilon
        /// </summary>
        public const float DefaultEpsilon = 0.001f;
        

        /// <summary>
        ///     Determines if v1, v2, and v3 are collinear
        /// </summary>
        /// <remarks>
        ///     Calculates if the area of the triangle of v1, v2, v3 is less than or equal to epsilon.
        /// </remarks>
        /// <param name="v1">Vector 1</param>
        /// <param name="v2">Vector 2</param>
        /// <param name="v3">Vector 3</param>
        /// <param name="epsilon">How close is close enough</param>
        /// <returns>If v1, v2, v3 is collinear</returns>
        public static bool IsOnLine(Vector2 v1, Vector2 v2, Vector2 v3, float epsilon = DefaultEpsilon)
        {
            return AreaOfTriangle(v1, v2, v3) <= epsilon;
        }

        /// <summary>
        ///     Calculates the square of the area of the triangle made up of the specified points.
        /// </summary>
        /// <param name="v1">First point</param>
        /// <param name="v2">Second point</param>
        /// <param name="v3">Third point</param>
        /// <returns>Area of the triangle made up of the given 3 points</returns>
        public static float AreaOfTriangle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return 0.5f * ((v2.X - v1.X) * (v3.Y - v1.Y) - (v3.X - v1.X) * (v2.Y - v1.Y));
        }

        /// <summary>
        ///     Finds a vector that is perpendicular to the specified vector.
        /// </summary>
        /// <returns>A vector perpendicular to v</returns>
        /// <param name="v">Vector</param>
        public static Vector2 Perpendicular(Vector2 v)
        {
            return new Vector2(-v.Y, v.X);
        }

        /// <summary>
        ///     Finds the dot product of (x1, y1) and (x2, y2)
        /// </summary>
        /// <returns>The dot.</returns>
        /// <param name="x1">The first x value.</param>
        /// <param name="y1">The first y value.</param>
        /// <param name="x2">The second x value.</param>
        /// <param name="y2">The second y value.</param>
        public static float Dot(float x1, float y1, float x2, float y2)
        {
            return x1 * x2 + y1 * y2;
        }

        /// <summary>
        ///     Determines if f1 and f2 are approximately the same.
        /// </summary>
        /// <returns>The approximately.</returns>
        /// <param name="f1">F1.</param>
        /// <param name="f2">F2.</param>
        /// <param name="epsilon">Epsilon.</param>
        public static bool Approximately(float f1, float f2, float epsilon = DefaultEpsilon)
        {
            return Math.Abs(f1 - f2) <= epsilon;
        }

        /// <summary>
        ///     Determines if vectors v1 and v2 are approximately equal, such that
        ///     both coordinates are within epsilon.
        /// </summary>
        /// <returns>If v1 and v2 are approximately equal.</returns>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <param name="epsilon">Epsilon.</param>
        public static bool Approximately(Vector2 v1, Vector2 v2, float epsilon = DefaultEpsilon)
        {
            return Approximately(v1.X, v2.X, epsilon) && Approximately(v1.Y, v2.Y, epsilon);
        }

        /// <summary>
        ///     Rotates the specified vector about the specified vector a rotation of the
        ///     specified amount.
        /// </summary>
        /// <param name="vec">The vector to rotate</param>
        /// <param name="about">The point to rotate vec around</param>
        /// <param name="rotation">The rotation</param>
        /// <returns>The vector vec rotated about about rotation.Theta radians.</returns>
        public static Vector2 Rotate(Vector2 vec, Vector2 about, Rotation rotation)
        {
            if (rotation.Theta == 0)
                return vec;

            var tmp = vec - about;
            return new Vector2(tmp.X * rotation.CosTheta - tmp.Y * rotation.SinTheta + about.X,
                tmp.X * rotation.SinTheta + tmp.Y * rotation.CosTheta + about.Y);
        }

        /// <summary>
        ///     Returns either the vector or -vector such that MakeStandardNormal(vec) == MakeStandardNormal(-vec)
        /// </summary>
        /// <param name="vec">The vector</param>
        /// <returns>Normal such that vec.X is positive (unless vec.X is 0, in which such that vec.Y is positive)</returns>
        public static Vector2 MakeStandardNormal(Vector2 vec)
        {
            if (vec.X < 0)
                return -vec;

            if (vec.X == 0 && vec.Y < 0)
                return -vec;

            return vec;
        }


        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Cross(ref Vector2 a, ref Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }


        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Cross(Vector2 a, Vector2 b)
        {
            return Cross(ref a, ref b);
        }


        /// <summary>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Vector2 Cross(Vector2 a, float s)
        {
            return new Vector2(s * a.Y, -s * a.X);
        }


        /// <summary>
        /// </summary>
        /// <param name="s"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2 Cross(float s, Vector2 a)
        {
            return new Vector2(-s * a.Y, s * a.X);
        }


        /// <summary>
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2 Abs(Vector2 v)
        {
            return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
        }


        /// <summary>
        ///     Returns the convex hull from the given vertices.
        ///     Giftwrap convex hull algorithm.
        ///     O(n * h) time complexity, where n is the number of points and h is the number of points on the convex hull.
        ///     See http://en.wikipedia.org/wiki/Gift_wrapping_algorithm for more details.
        ///     Extracted from Box2D
        ///     https://github.com/VelcroPhysics/VelcroPhysics/blob/1456abf40e4c30065bf122f409ce60ce3873ff09/VelcroPhysics/Tools/ConvexHull/GiftWrap/GiftWrap.cs
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        public static IList<Vector2> GetConvexHull(IList<Vector2> vertices)
        {
            if (vertices.Count <= 3)
                return vertices;

            // Find the right most point on the hull
            var i0 = 0;
            var x0 = vertices[0].X;
            for (var i = 1; i < vertices.Count; ++i)
            {
                var x = vertices[i].X;
                if (x > x0 || (x == x0 && vertices[i].Y < vertices[i0].Y))
                {
                    i0 = i;
                    x0 = x;
                }
            }

            var hull = new int[vertices.Count];
            var m = 0;
            var ih = i0;

            while (true)
            {
                hull[m] = ih;

                var ie = 0;
                for (var j = 1; j < vertices.Count; ++j)
                {
                    if (ie == ih)
                    {
                        ie = j;
                        continue;
                    }

                    var r = vertices[ie] - vertices[hull[m]];
                    var v = vertices[j] - vertices[hull[m]];
                    var c = Cross(ref r, ref v);
                    if (c < 0.0f)
                        ie = j;

                    // Collinearity check
                    if (c == 0.0f && v.LengthSquared() > r.LengthSquared())
                        ie = j;
                }

                ++m;
                ih = ie;

                if (ie == i0)
                {
                    break;
                }
            }

            var result = new List<Vector2>(m);

            // Copy vertices.
            for (var i = 0; i < m; ++i)
                result.Add(vertices[hull[i]]);

            return result;
        }


        /// <summary>
        ///     Build vertices to represent an axis-aligned box.
        /// </summary>
        /// <param name="hx">the half-width.</param>
        /// <param name="hy">the half-height.</param>
        public static IList<Vector2> CreateAxisAlignedBox(float hx, float hy)
        {
            var vertices = new Vector2[4];
            vertices[0] = new Vector2(-hx, -hy);
            vertices[1] = new Vector2(hx, -hy);
            vertices[2] = new Vector2(hx, hy);
            vertices[3] = new Vector2(-hx, hy);

            return vertices;
        }


        /// <summary>
        ///     Fetches a rectangle shape with the given width, height, x and y center.
        /// </summary>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        /// <param name="x">The X center of the rectangle.</param>
        /// <param name="y">The Y center of the rectangle.</param>
        /// <returns>A rectangle shape with the given width, height, x and y center.</returns>
        public static IList<Vector2> CreateRectangle(float width, float height, float x = 0f, float y = 0f)
        {
            return new[]
            {
                new Vector2(x, y),
                new Vector2(x + width, y),
                new Vector2(x + width, y + height),
                new Vector2(x, y + height)
            };
        }


        /// <summary>
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static Vector2[] CreateNormals(IList<Vector2> vertices)
        {
            // Compute normals. Ensure the edges have non-zero length.
            var normals = new Vector2[vertices.Count];
            for (var i = 0; i < vertices.Count; ++i)
            {
                var i1 = i;
                var i2 = i + 1 < vertices.Count ? i + 1 : 0;
                var edge = vertices[i2] - vertices[i1];
                var temp = Cross(edge, 1.0f);
                temp.Normalize();
                normals[i] = temp;
            }

            return normals;
        }


        /// <summary>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 NormalizeWithNanCheck(this Vector2 vector)
        {
            // Richtung bestimmen
            var direction = Vector2.Normalize(vector);

            if (Single.IsNaN(direction.X) || Single.IsNaN(direction.Y))
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
}