#region

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// </summary>
    public class PolygonShape : Shape
    {
        /// <summary>
        /// </summary>
        public Vector2[] Normals { get; }

        /// <summary>
        /// </summary>
        public Vector2[] Vertices { get; }

        /// <summary>
        /// </summary>
        public Point[] PointVertices { get; private set; }

        /// <summary>
        /// </summary>
        public Vector2 Center { get; }


        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="vertices"></param>
        /// <param name="degrees"></param>
        public PolygonShape(string name, Vector2 position, IEnumerable<Vector2> vertices, float degrees = 0)
            : base(ShapeType.Polygon, name, position, 0)
        {
            this.Vertices = vertices.ToArray();
            this.Normals = VectorHelper.CreateNormals(this.Vertices);

            var vertexCount = this.Vertices.Length;
            for (var i = 0; i < vertexCount; ++i)
                this.Center += this.Vertices[i];

            this.Center *= 1.0f / this.Vertices.Length;

            this.SetRotation(MathHelper.ToRadians(degrees));
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Point> GeneratePoints()
        {
            foreach (var vertex in this.Vertices)
                yield return GameHelper.ConvertPositionToTilePosition(vertex);
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Name} / {this.Position}/ {this.Vertices} / {this.Normals} / {this.PointVertices}";
        }
    }
}