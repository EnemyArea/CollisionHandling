using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// https://github.com/VelcroPhysics/VelcroPhysics/blob/1456abf40e4c30065bf122f409ce60ce3873ff09/VelcroPhysics/Shared/Vertices.cs
    /// </summary>
    [DebuggerDisplay("Count = {Count} Vertices = {ToString()}")]
    public class Vertices : List<Vector2>
    {
        public Vertices() { }

        public Vertices(int capacity) : base(capacity) { }

        public Vertices(IEnumerable<Vector2> vertices)
        {
            this.AddRange(vertices);
        }

        internal bool AttachedToBody { get; set; }

        /// <summary>
        /// You can add holes to this collection.
        /// It will get respected by some of the triangulation algoithms, but otherwise not used.
        /// </summary>
        public List<Vertices> Holes { get; set; }

        /// <summary>
        /// Gets the next index. Used for iterating all the edges with wrap-around.
        /// </summary>
        /// <param name="index">The current index</param>
        public int NextIndex(int index)
        {
            return (index + 1 > this.Count - 1) ? 0 : index + 1;
        }

        /// <summary>
        /// Gets the next vertex. Used for iterating all the edges with wrap-around.
        /// </summary>
        /// <param name="index">The current index</param>
        public Vector2 NextVertex(int index)
        {
            return this[this.NextIndex(index)];
        }

        /// <summary>
        /// Gets the previous index. Used for iterating all the edges with wrap-around.
        /// </summary>
        /// <param name="index">The current index</param>
        public int PreviousIndex(int index)
        {
            return index - 1 < 0 ? this.Count - 1 : index - 1;
        }

        /// <summary>
        /// Gets the previous vertex. Used for iterating all the edges with wrap-around.
        /// </summary>
        /// <param name="index">The current index</param>
        public Vector2 PreviousVertex(int index)
        {
            return this[this.PreviousIndex(index)];
        }

        /// <summary>
        /// Gets the signed area.
        /// If the area is less than 0, it indicates that the polygon is clockwise winded.
        /// </summary>
        /// <returns>The signed area</returns>
        public float GetSignedArea()
        {
            //The simplest polygon which can exist in the Euclidean plane has 3 sides.
            if (this.Count < 3)
                return 0;

            int i;
            float area = 0;

            for (i = 0; i < this.Count; i++)
            {
                int j = (i + 1) % this.Count;

                Vector2 vi = this[i];
                Vector2 vj = this[j];

                area += vi.X * vj.Y;
                area -= vi.Y * vj.X;
            }
            area /= 2.0f;
            return area;
        }

        /// <summary>
        /// Gets the area.
        /// </summary>
        /// <returns></returns>
        public float GetArea()
        {
            float area = this.GetSignedArea();
            return (area < 0 ? -area : area);
        }

        /// <summary>
        /// Gets the centroid.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetCentroid()
        {
            //The simplest polygon which can exist in the Euclidean plane has 3 sides.
            if (this.Count < 3)
                return new Vector2(float.NaN, float.NaN);

            // Same algorithm is used by Box2D
            Vector2 c = Vector2.Zero;
            float area = 0.0f;
            const float inv3 = 1.0f / 3.0f;

            for (int i = 0; i < this.Count; ++i)
            {
                // Triangle vertices.
                Vector2 current = this[i];
                Vector2 next = (i + 1 < this.Count ? this[i + 1] : this[0]);

                float triangleArea = 0.5f * (current.X * next.Y - current.Y * next.X);
                area += triangleArea;

                // Area weighted centroid
                c += triangleArea * inv3 * (current + next);
            }

            // Centroid
            c *= 1.0f / area;
            return c;
        }
        
        /// <summary>
        /// Translates the vertices with the specified vector.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Translate(Vector2 value)
        {
            this.Translate(ref value);
        }

        /// <summary>
        /// Translates the vertices with the specified vector.
        /// </summary>
        /// <param name="value">The vector.</param>
        public void Translate(ref Vector2 value)
        {
            Debug.Assert(!this.AttachedToBody, "Translating vertices that are used by a Body can result in unstable behavior. Use Body.Position instead.");

            for (int i = 0; i < this.Count; i++)
                this[i] = Vector2.Add(this[i], value);

            if (this.Holes != null && this.Holes.Count > 0)
            {
                foreach (Vertices hole in this.Holes)
                {
                    hole.Translate(ref value);
                }
            }
        }

        /// <summary>
        /// Scales the vertices with the specified vector.
        /// </summary>
        /// <param name="value">The Value.</param>
        public void Scale(Vector2 value)
        {
            this.Scale(ref value);
        }

        /// <summary>
        /// Scales the vertices with the specified vector.
        /// </summary>
        /// <param name="value">The Value.</param>
        public void Scale(ref Vector2 value)
        {
            Debug.Assert(!this.AttachedToBody, "Scaling vertices that are used by a Body can result in unstable behavior.");

            for (int i = 0; i < this.Count; i++)
                this[i] = Vector2.Multiply(this[i], value);

            if (this.Holes != null && this.Holes.Count > 0)
            {
                foreach (Vertices hole in this.Holes)
                {
                    hole.Scale(ref value);
                }
            }
        }

        /// <summary>
        /// Rotate the vertices with the defined value in radians.
        /// Warning: Using this method on an active set of vertices of a Body,
        /// will cause problems with collisions. Use Body.Rotation instead.
        /// </summary>
        /// <param name="value">The amount to rotate by in radians.</param>
        public void Rotate(float value)
        {
            Debug.Assert(!this.AttachedToBody, "Rotating vertices that are used by a Body can result in unstable behavior.");

            float num1 = (float)Math.Cos(value);
            float num2 = (float)Math.Sin(value);

            for (int i = 0; i < this.Count; i++)
            {
                Vector2 position = this[i];
                this[i] = new Vector2((position.X * num1 + position.Y * -num2), (position.X * num2 + position.Y * num1));
            }

            if (this.Holes != null && this.Holes.Count > 0)
            {
                foreach (Vertices hole in this.Holes)
                {
                    hole.Rotate(value);
                }
            }
        }

        /// <summary>
        /// Determines whether the polygon is convex.
        /// O(n^2) running time.
        /// Assumptions:
        /// - The polygon is in counter clockwise order
        /// - The polygon has no overlapping edges
        /// </summary>
        /// <returns>
        /// <c>true</c> if it is convex; otherwise, <c>false</c>.
        /// </returns>
        public bool IsConvex()
        {
            //The simplest polygon which can exist in the Euclidean plane has 3 sides.
            if (this.Count < 3)
                return false;

            //Triangles are always convex
            if (this.Count == 3)
                return true;

            // Checks the polygon is convex and the interior is to the left of each edge.
            for (int i = 0; i < this.Count; ++i)
            {
                int next = i + 1 < this.Count ? i + 1 : 0;
                Vector2 edge = this[next] - this[i];

                for (int j = 0; j < this.Count; ++j)
                {
                    // Don't check vertices on the current edge.
                    if (j == i || j == next)
                        continue;

                    Vector2 r = this[j] - this[i];

                    float s = edge.X * r.Y - edge.Y * r.X;

                    if (s <= 0.0f)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Indicates if the vertices are in counter clockwise order.
        /// Warning: If the area of the polygon is 0, it is unable to determine the winding.
        /// </summary>
        public bool IsCounterClockWise()
        {
            //The simplest polygon which can exist in the Euclidean plane has 3 sides.
            if (this.Count < 3)
                return false;

            return (this.GetSignedArea() > 0.0f);
        }

        /// <summary>
        /// Forces the vertices to be counter clock wise order.
        /// </summary>
        public void ForceCounterClockWise()
        {
            //The simplest polygon which can exist in the Euclidean plane has 3 sides.
            if (this.Count < 3)
                return;

            if (!this.IsCounterClockWise())
                this.Reverse();
        }
        
        /// <summary>
        /// Projects to axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        public void ProjectToAxis(ref Vector2 axis, out float min, out float max)
        {
            // To project a point on an axis use the dot product
            float dotProduct = Vector2.Dot(axis, this[0]);
            min = dotProduct;
            max = dotProduct;

            for (int i = 0; i < this.Count; i++)
            {
                dotProduct = Vector2.Dot(this[i], axis);
                if (dotProduct < min)
                {
                    min = dotProduct;
                }
                else
                {
                    if (dotProduct > max)
                    {
                        max = dotProduct;
                    }
                }
            }
        }

        /// <summary>
        /// Transforms the polygon using the defined matrix.
        /// </summary>
        /// <param name="transform">The matrix to use as transformation.</param>
        public void Transform(ref Matrix transform)
        {
            // Transform main polygon
            for (int i = 0; i < this.Count; i++)
                this[i] = Vector2.Transform(this[i], transform);

            // Transform holes
            if (this.Holes != null && this.Holes.Count > 0)
            {
                for (int i = 0; i < this.Holes.Count; i++)
                {
                    Vector2[] temp = this.Holes[i].ToArray();
                    Vector2.Transform(temp, ref transform, temp);

                    this.Holes[i] = new Vertices(temp);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                builder.Append(this[i]);
                if (i < this.Count - 1)
                {
                    builder.Append(" ");
                }
            }
            return builder.ToString();
        }
    }
}