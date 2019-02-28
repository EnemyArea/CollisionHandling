#region

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public class PolygonShape : Shape
    {
        public IList<Vector2> Normals { get; }
        public Vector2[] Vertices { get; }
        public Point[] PointVertices { get; }
        public int Radius { get; }

        public PolygonShape(IEnumerable<Vector2> vertices) : base(ShapeContactType.Polygon)
        {
            this.Vertices = vertices.ToArray();
            this.Normals = new List<Vector2>();
            this.PointVertices = this.GeneratePoints().ToArray();

            // Compute normals. Ensure the edges have non-zero length.
            for (var i = 0; i < this.Vertices.Length; ++i)
            {
                var i1 = i;
                var i2 = i + 1 < this.Vertices.Length ? i + 1 : 0;
                var edge = this.Vertices[i2] - this.Vertices[i1];
                var temp = MathUtils.Cross(edge, 1.0f);
                temp.Normalize();
                this.Normals.Add(temp);
            }
        }

        private IEnumerable<Point> GeneratePoints()
        {
            foreach (var vertex in this.Vertices)
            {
                yield return GameHelper.ConvertPositionToTilePosition(vertex);
            }
        }
    }
}