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
        private readonly Vector2[] baseVertices;


        /// <summary>
        /// </summary>
        public Vector2[] Normals { get; private set; }

        /// <summary>
        /// </summary>
        public Vector2[] Vertices { get; private set; }

        /// <summary>
        /// </summary>
        public Point[] PointVertices { get; private set; }


        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="vertices"></param>
        public PolygonShape(string name, Vector2 position, IEnumerable<Vector2> vertices)
            : base(ShapeType.Polygon, name, position, 0)
        {
            this.baseVertices = vertices.ToArray();
            this.UpdateVertices();
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
        private void UpdateVertices()
        {
            this.Vertices = MathUtils.Mul(ref this.transform, this.baseVertices.ToArray());
            this.Normals = VectorHelper.CreateNormals(this.Vertices);
            this.PointVertices = this.GeneratePoints().ToArray();
        }


        /// <summary>
        /// </summary>
        /// <param name="rotation"></param>
        public override void SetRotation(float rotation)
        {
            base.SetRotation(rotation);
            this.UpdateVertices();
        }


        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        public override void SetPosition(Vector2 position)
        {
            base.SetPosition(position);
            this.UpdateVertices();
        }


        /// <summary>
        /// </summary>
        /// <param name="velocity"></param>
        public override void ApplyVelocity(Vector2 velocity)
        {
            base.ApplyVelocity(velocity);
            this.UpdateVertices();
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