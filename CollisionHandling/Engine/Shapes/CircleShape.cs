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
        public AABB AABB { get; private set; }


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

            this.AABB = new AABB(-new Vector2(this.Radius), new Vector2(this.Radius));
            this.BoundingBox = new Rectangle(
                (int)(this.Position.X + this.AABB.LowerLeft.X),
                (int)(this.Position.Y + this.AABB.UpperRight.Y),
                (int)this.AABB.Width,
                (int)this.AABB.Height);

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
            return $"{this.Name} / {this.Radius} / {this.Position} / {this.TilePosition}";
        }
    }
}