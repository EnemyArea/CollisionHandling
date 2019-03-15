#region

using System;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Shapes
{
    /// <summary>
    /// </summary>
    public sealed class LineShape : Shape
    {
        /// <summary>
        /// </summary>
        public Vector2 Start { get; }

        /// <summary>
        /// </summary>
        public Vector2 End { get; }

        /// <summary>
        /// </summary>
        public Point StartTilePosition { get; private set; }

        /// <summary>
        /// </summary>
        public Point EndTilePosition { get; private set; }


        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="isStatic"></param>
        public LineShape(Vector2 start, Vector2 end, bool isStatic = true)
            : base(ShapeType.Line, "Line", Vector2.Zero, 0, isStatic)
        {
            this.Start = start;
            this.End = end;
            this.Color = Color.Gray;

            this.UpdateBoundingBox();
        }


        /// <summary>
        /// </summary>
        protected override void UpdateBoundingBox()
        {
            this.StartTilePosition = GameHelper.ConvertPositionToTilePosition(this.Start);
            this.EndTilePosition = GameHelper.ConvertPositionToTilePosition(this.End);

            //if (this.StartTilePosition.X == this.EndTilePosition.X)
            //{
            //    this.BoundingBox = new Rectangle(this.StartTilePosition.X, this.StartTilePosition.Y, 1, Math.Abs(this.EndTilePosition.Y - this.StartTilePosition.Y));
            //}
            //else
            //{
            //    if (this.StartTilePosition.Y == this.EndTilePosition.Y)
            //    {
            //        this.BoundingBox = new Rectangle(this.StartTilePosition.X, this.StartTilePosition.Y, Math.Abs(this.EndTilePosition.X - this.StartTilePosition.X), 1);
            //    }
            //}

            var aabb = AabbHelper.ComputeLineAabb(this.Start, this.End, Vector2.Zero, 0);

            this.BoundingBox = new Rectangle(
                (int)aabb.LowerBound.X,
                (int)aabb.LowerBound.Y,
                (int)aabb.Width,
                (int)aabb.Height);
            this.BoundingBoxTileMap = GameHelper.ConvertPositionToTilePosition(this.BoundingBox);
        }
    }
}