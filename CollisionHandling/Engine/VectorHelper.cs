#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using VelcroPhysics.Shared;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// </summary>
    public static class VectorHelper
    {
        /// <summary>
        ///     Build vertices to represent an axis-aligned box.
        /// </summary>
        /// <param name="hx">the half-width.</param>
        /// <param name="hy">the half-height.</param>
        public static Vertices CreateRectangle(float hx, float hy)
        {
            var vertices = new Vertices(4);
            vertices.Add(new Vector2(-hx, -hy));
            vertices.Add(new Vector2(hx, -hy));
            vertices.Add(new Vector2(hx, hy));
            vertices.Add(new Vector2(-hx, hy));

            return vertices;
        }


        /// <summary>
        ///     Build vertices to represent an oriented box.
        /// </summary>
        /// <param name="hx">the half-width.</param>
        /// <param name="hy">the half-height.</param>
        /// <param name="center">the center of the box in local coordinates.</param>
        /// <param name="angle">the rotation of the box in local coordinates.</param>
        public static Vertices CreateRectangle(float hx, float hy, Vector2 center, float angle)
        {
            var vertices = CreateRectangle(hx, hy);

            var xf = new Transform();
            xf.p = center;
            xf.q.Set(angle);

            // Transform vertices
            for (var i = 0; i < 4; ++i)
            {
                vertices[i] = MathUtils.Mul(ref xf, vertices[i]);
            }

            return vertices;
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
                var temp = MathUtils.Cross(edge, 1.0f);
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
}