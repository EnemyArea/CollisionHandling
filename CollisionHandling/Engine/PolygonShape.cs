#region

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public class PolygonShape : Shape
    {
        public Vector2[] Normals { get; }
        public Vector2[] Vertices { get; }
        public Point[] PointVertices { get; }

        public PolygonShape(string name, Vector2 position, IEnumerable<Vector2> vertices)
            : base(ShapeType.Polygon, name)
        {
            this.Position = position;
            this.Vertices = vertices.ToArray();
            this.Normals = VectorHelper.CreateNormals(this.Vertices);
            this.PointVertices = this.GeneratePoints().ToArray();
        }

        private IEnumerable<Point> GeneratePoints()
        {
            foreach (var vertex in this.Vertices)
            {
                yield return GameHelper.ConvertPositionToTilePosition(vertex);
            }
        }


        public override string ToString()
        {
            return $"{this.Name} / {this.Position}/ {this.Vertices} / {this.Normals} / {this.PointVertices}";
        }
    }
}