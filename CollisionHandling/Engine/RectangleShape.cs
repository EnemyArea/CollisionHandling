#region

using System.Linq;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public class RectangleShape : PolygonShape
    {
        public Rectangle Rectangle { get; }

        public Rectangle TileRectangle { get; }

        public RectangleShape(string name, Rectangle rectangle)
            : base(name, GameHelper.GetConvexHull(GameHelper.CreateVerticesFromRectangle(rectangle).ToArray()))
        {
            this.Rectangle = rectangle;
            this.TileRectangle = GameHelper.ConvertPositionToTilePosition(rectangle);
        }
    }
}