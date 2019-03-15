#region

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using BoundingBox = CollisionFloatTestNewMono.Engine.Math2.BoundingBox;

#endregion

namespace CollisionFloatTestNewMono.Engine.Shapes
{
    /// <summary>
    /// </summary>
    public class PolygonShape : Shape
    {
        /// <summary>
        /// </summary>
        public Vector2[] Normals { get; }

        /// <summary>
        /// </summary>
        public Vector2[] Vertices { get; }

        /// <summary>
        /// </summary>
        public Point[] PointVertices { get; private set; }

        /// <summary>
        /// </summary>
        public Vector2 Center { get; }


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

            this.SetRotation(MathHelper.ToRadians(degrees));
            this.UpdateBoundingBox();
        }


        /// <summary>
        /// </summary>
        protected override void UpdateBoundingBox()
        {
            var aabb = AabbHelper.ComputePolygonAabb(this.Position, this.Vertices, this.Center, this.Rotation);

            this.BoundingBox = new Rectangle(
                (int)aabb.LowerBound.X,
                (int)aabb.LowerBound.Y,
                (int)aabb.Width,
                (int)aabb.Height);
            
            this.BoundingBoxTileMap = GameHelper.ConvertPositionToTilePosition(this.BoundingBox);
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Point> GeneratePoints()
        {
            foreach (var vertex in this.Vertices)
                yield return GameHelper.ConvertPositionToTilePosition(vertex);
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Name} / {this.Position}/ {this.Vertices} / {this.Normals} / {this.PointVertices}";
        }
    }
}