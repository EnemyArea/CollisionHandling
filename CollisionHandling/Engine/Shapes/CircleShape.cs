#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Shapes
{
    /// <summary>
    /// </summary>
    public sealed class CircleShape : Shape
    {
        /// <summary>
        /// </summary>
        public float Radius { get; }

        /// <summary>
        /// </summary>
        public Point TilePosition { get; private set; }


        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="isStatic"></param>
        public CircleShape(string name, Vector2 position, float radius, bool isStatic = true)
            : base(ShapeType.Circle, name, position, 0, isStatic)
        {
            this.Radius = radius;
            this.UpdateBoundingBox();
        }


        /// <summary>
        /// </summary>
        protected override void UpdateBoundingBox()
        {
            this.TilePosition = GameHelper.ConvertPositionToTilePosition(this.Position);

            var aabb = AabbHelper.ComputeCircleAabb(this.Position, this.Radius);
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
            return $"{this.Name} / {this.Radius} / {this.Position} / {this.TilePosition}";
        }
    }
}