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
        public int Radius { get; }

        /// <summary>
        /// </summary>
        public Point TilePosition { get; private set; }


        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        public CircleShape(string name, Vector2 position, int radius)
            : base(ShapeType.Circle, name, position, 0)
        {
            this.Radius = radius;
            this.UpdateBoundingBox();
        }


        /// <summary>
        /// </summary>
        protected override void UpdateBoundingBox()
        {
            this.TilePosition = GameHelper.ConvertPositionToTilePosition(this.Position);
            this.BoundingBox = new Rectangle(
                this.TilePosition.X + (this.Radius / GameHelper.TileSize),
                this.TilePosition.Y + (this.Radius / GameHelper.TileSize),
                Math.Max(1, this.Radius / GameHelper.TileSize),
                Math.Max(1, this.Radius / GameHelper.TileSize));
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