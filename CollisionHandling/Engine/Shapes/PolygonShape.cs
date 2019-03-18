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
        public Vector2[] Vertices { get; }
        
        /// <summary>
        /// </summary>
        public Vector2[] Normals { get; }

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
            
            var start = GameHelper.ConvertPositionToTilePosition(aabb.LowerBound);
            var end = GameHelper.ConvertPositionToTilePositionCeiling(aabb.UpperBound);
            
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