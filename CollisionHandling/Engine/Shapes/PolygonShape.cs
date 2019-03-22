#region

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Shapes
{
    /// <summary>
    /// </summary>
    public class PolygonShape : Shape
    {
        /// <summary>
        /// </summary>
        public Vector2[] Vertices { get; }

        /// <summary>
        /// </summary>
        public Vector2[] Normals { get; }

        /// <summary>
        ///     The lines of this polygon, such that any two adjacent (wrapping)
        ///     lines share a vertex
        /// </summary>
        public Line[] Lines { get; }

        /// <summary>
        /// </summary>
        public Vector2 Center { get; }
        
        /// <summary>
        /// </summary>
        public float Area { get; }

        /// <summary>
        ///     If this polygon is defined clockwise
        /// </summary>
        public bool Clockwise { get; }

        /// <summary>
        /// </summary>
        public AABB Aabb { get; private set; }


        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="vertices"></param>
        /// <param name="degrees"></param>
        /// <param name="isStatic"></param>
        public PolygonShape(string name, Vector2 position, IEnumerable<Vector2> vertices, float degrees = 0, bool isStatic = true)
            : base(ShapeType.Polygon, name, position, 0, isStatic)
        {
            this.Vertices = vertices.ToArray();
            this.Normals = MathUtils.CreateNormals(this.Vertices);

            var vertexCount = this.Vertices.Length;
            for (var i = 0; i < vertexCount; ++i)
                this.Center += this.Vertices[i];

            this.Center *= 1.0f / this.Vertices.Length;

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

            this.SetRotation(MathHelper.ToRadians(degrees));
            this.UpdateBoundingBox();
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        private AABB ComputePolygonAabb()
        {
            var vertexCount = this.Vertices.Length;
            var tempVertices = new Vector2[vertexCount];
            for (var i = 0; i < vertexCount; ++i)
                tempVertices[i] = MathUtils.Rotate(this.Vertices[i], this.Center, this.Rotation);

            var min = new Vector2(tempVertices[0].X, tempVertices[0].Y);
            var max = new Vector2(min.X, min.Y);
            for (var i = 1; i < tempVertices.Length; i++)
            {
                min.X = Math.Min(min.X, tempVertices[i].X);
                min.Y = Math.Min(min.Y, tempVertices[i].Y);
                max.X = Math.Max(max.X, tempVertices[i].X);
                max.Y = Math.Max(max.Y, tempVertices[i].Y);
            }

            return new AABB(min, max);
        }


        /// <summary>
        /// </summary>
        protected override void UpdateBoundingBox()
        {
            this.Aabb = this.ComputePolygonAabb();
            this.BoundingBox = new Rectangle(
                (int)(this.Position.X + this.Aabb.LowerLeft.X),
                (int)(this.Position.Y + this.Aabb.UpperRight.Y),
                (int)this.Aabb.Width,
                (int)this.Aabb.Height);

            var start = GameHelper.ConvertPositionToTilePosition(new Vector2(this.BoundingBox.Left, this.BoundingBox.Top));
            var end = GameHelper.ConvertPositionToTilePositionCeiling(new Vector2(this.BoundingBox.Right, this.BoundingBox.Bottom));

            var boundingBoxTileMap = new Rectangle(Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                Math.Abs(start.X - end.X),
                Math.Abs(start.Y - end.Y));

            this.BoundingBoxTileMap = boundingBoxTileMap;
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Name} / {this.Position}/ {this.Vertices} / {this.Normals}";
        }
    }
}