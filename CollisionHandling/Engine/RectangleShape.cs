using System.Linq;
using Microsoft.Xna.Framework;

namespace CollisionFloatTestNewMono.Engine
{
    public class RectangleShape : PolygonShape
    {
        public string Name { get; }

        public Rectangle Rectangle { get; }

        public Rectangle TileRectangle { get; }

        public RectangleShape(string name, Rectangle rectangle)
            : base(GameHelper.GetConvexHull(GameHelper.CreateVerticesFromRectangle(rectangle).ToArray()))
        {
            this.Name = name;
            this.Rectangle = rectangle;
            this.TileRectangle = GameHelper.ConvertPositionToTilePosition(rectangle);
        }
    }
}