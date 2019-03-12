#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Shapes
{
    public class RectangleShape : PolygonShape
    {
        public Rectangle Rectangle { get; }

        public Rectangle TileRectangle { get; }

        public RectangleShape(string name, Rectangle rectangle)
            : base(name, new Vector2(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f),
                MathUtils.CreateRectangle(rectangle.Width / 2f, rectangle.Height / 2f))
        {
            this.Rectangle = rectangle;
            this.TileRectangle = GameHelper.ConvertPositionToTilePosition(rectangle);
        }
    }
}