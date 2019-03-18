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
            
            var aabb = AabbHelper.ComputeLineAabb(this.Start, this.End, Vector2.Zero, 0);
            
            var start = GameHelper.ConvertPositionToTilePosition(aabb.LowerBound);
            var end = GameHelper.ConvertPositionToTilePositionCeiling(aabb.UpperBound);
            
            var boundingBoxTileMap = new Rectangle(Math.Min(start.X, end.X),
                Math.Min(start.Y, end.Y),
                Math.Abs(start.X - end.X),
                Math.Abs(start.Y - end.Y));

            this.BoundingBoxTileMap = boundingBoxTileMap;
        }
    }
}