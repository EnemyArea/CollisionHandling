#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Math2
{
    /// <summary>
    ///     Describes a simple polygon based on it's vertices. Does not
    ///     have position - most functions require specifying the origin of the
    ///     polygon. Polygons are meant to be reused.
    /// </summary>
    public class Polygon
    {
        /// <summary>
        ///     The vertices of this polygon, such that any two adjacent vertices
        ///     create a line of the polygon
        /// </summary>
        public readonly Vector2[] Vertices;

        /// <summary>
        ///     The lines of this polygon, such that any two adjacent (wrapping)
        ///     lines share a vertex
        /// </summary>
        public readonly Line[] Lines;

        /// <summary>
        ///     The center of this polyogn
        /// </summary>
        public readonly Vector2 Center;

        /// <summary>
        ///     The three normal vectors of this polygon, normalized
        /// </summary>
        public readonly List<Vector2> Normals;

        /// <summary>
        ///     The bounding box.
        /// </summary>
        public readonly BoundingBox Aabb;

        /// <summary>
        ///     The longest line that can be created inside this polygon.
        ///     <example>
        ///         var poly = ShapeUtils.CreateRectangle(2, 3);
        ///         Console.WriteLine($"corner-to-corner = longest axis = Math.Sqrt(2 * 2 + 3 * 3) = {Math.Sqrt(2 * 2 + 3 * 3)} =
        ///         {poly.LongestAxisLength}");
        ///     </example>
        /// </summary>
        public readonly float LongestAxisLength;

        /// <summary>
        ///     The area of this polygon
        /// </summary>
        public readonly float Area;

        /// <summary>
        ///     If this polygon is defined clockwise
        /// </summary>
        public readonly bool Clockwise;

        /// <summary>
        ///     Initializes a polygon with the specified vertices
        /// </summary>
        /// <param name="vertices">Vertices</param>
        /// <exception cref="ArgumentNullException">If vertices is null</exception>
        public Polygon(Vector2[] vertices)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));

            this.Vertices = vertices;

            this.Normals = new List<Vector2>();
            Vector2 tmp;
            for (var i = 1; i < vertices.Length; i++)
            {
                tmp = MathUtils.MakeStandardNormal(Vector2.Normalize(MathUtils.Perpendicular(vertices[i] - vertices[i - 1])));
                if (!this.Normals.Contains(tmp))
                    this.Normals.Add(tmp);
            }

            tmp = MathUtils.MakeStandardNormal(Vector2.Normalize(MathUtils.Perpendicular(vertices[0] - vertices[vertices.Length - 1])));
            if (!this.Normals.Contains(tmp))
                this.Normals.Add(tmp);

            var min = new Vector2(vertices[0].X, vertices[0].Y);
            var max = new Vector2(min.X, min.Y);
            for (var i = 1; i < vertices.Length; i++)
            {
                min.X = Math.Min(min.X, vertices[i].X);
                min.Y = Math.Min(min.Y, vertices[i].Y);
                max.X = Math.Max(max.X, vertices[i].X);
                max.Y = Math.Max(max.Y, vertices[i].Y);
            }

            this.Aabb = new BoundingBox(min, max);

            this.Center = new Vector2(0, 0);
            foreach (var vert in this.Vertices)
            {
                this.Center += vert;
            }

            this.Center *= 1.0f / this.Vertices.Length;

            // Find longest axis
            float longestAxisLenSq = -1;
            for (var i = 1; i < vertices.Length; i++)
            {
                var vec = vertices[i] - vertices[i - 1];
                longestAxisLenSq = Math.Max(longestAxisLenSq, vec.LengthSquared());
            }

            longestAxisLenSq = Math.Max(longestAxisLenSq, (vertices[0] - vertices[vertices.Length - 1]).LengthSquared());
            this.LongestAxisLength = (float)Math.Sqrt(longestAxisLenSq);

            // Area and lines
            float area = 0;
            this.Lines = new Line[this.Vertices.Length];
            var last = this.Vertices[this.Vertices.Length - 1];
            for (var i = 0; i < this.Vertices.Length; i++)
            {
                var next = this.Vertices[i];
                this.Lines[i] = new Line(last, next);
                area += MathUtils.AreaOfTriangle(last, next, this.Center);
                last = next;
            }

            this.Area = area;

            last = this.Vertices[this.Vertices.Length - 1];
            var centToLast = last - this.Center;
            var angLast = Math.Atan2(centToLast.Y, centToLast.X);
            var cwCounter = 0;
            var ccwCounter = 0;
            var foundDefinitiveResult = false;
            for (var i = 0; i < this.Vertices.Length; i++)
            {
                var curr = this.Vertices[i];
                var centToCurr = curr - this.Center;
                var angCurr = Math.Atan2(centToCurr.Y, centToCurr.X);

                var clockwise = angCurr < angLast;
                if (clockwise)
                    cwCounter++;
                else
                    ccwCounter++;

                this.Clockwise = clockwise;
                if (Math.Abs(angLast - angCurr) > MathUtils.DefaultEpsilon)
                {
                    foundDefinitiveResult = true;
                    break;
                }

                angLast = angCurr;
            }

            if (!foundDefinitiveResult)
                this.Clockwise = cwCounter > ccwCounter;
        }
    }
}