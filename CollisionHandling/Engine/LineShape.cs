#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public sealed class LineShape : Shape
    {
        public Vector2 Start { get; }
        public Vector2 End { get; }
        public Point StartTilePosition { get; }
        public Point EndTilePosition { get; }

        public LineShape(Vector2 start, Vector2 end) : base(ShapeType.Line)
        {
            this.Start = start;
            this.End = end;
            this.Color = Color.Gray;

            this.StartTilePosition = GameHelper.ConvertPositionToTilePosition(start);
            this.EndTilePosition = GameHelper.ConvertPositionToTilePosition(end);
        }
    }
}