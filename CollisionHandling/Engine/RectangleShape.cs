using Microsoft.Xna.Framework;

namespace CollisionFloatTestNewMono.Engine
{
    public class RectangleShape : Shape
    {
        public string Name { get; }

        public Rectangle Rectangle { get; }

        public Rectangle TileRectangle { get; }

        public RectangleShape(string name, Rectangle rectangle)
        {
            this.Name = name;
            this.Rectangle = rectangle;
            this.TileRectangle = GameHelper.ConvertPositionToTilePosition(rectangle);
        }
    }
}