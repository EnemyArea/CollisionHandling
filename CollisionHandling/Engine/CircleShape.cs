#region

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public class PolygonShape : Shape
    {
        public Vector2[] Vertices { get; }
        public Point[] PointVertices { get; }

        public PolygonShape(Vector2[] vertices)
        {
            this.Vertices = vertices;
            this.PointVertices = this.GeneratePoints(vertices).ToArray();
        }

        private IEnumerable<Point> GeneratePoints(Vector2[] vertices)
        {
            foreach (var vertex in vertices)
            {
                yield return GameHelper.ConvertPositionToTilePosition(vertex);
            }
        }
    }

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

    public class CircleShape : Shape
    {
        public string Name { get; }
        public int Radius { get; }
        public Point TilePosition { get; set; }

        private Vector2 position;

        public Vector2 Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
                this.TilePosition = GameHelper.ConvertPositionToTilePosition(value);
            }
        }


        public CircleShape(string name, Vector2 position, int radius)
        {
            this.Position = position;
            this.Name = name;
            this.Radius = radius;
            this.TilePosition = GameHelper.ConvertPositionToTilePosition(position);
        }


        public override string ToString()
        {
            return $"{this.Name} / {this.Radius} / {this.Position} / {this.TilePosition}";
        }
    }
}