#region

using System.Collections.Generic;
using CollisionFloatTestNewMono.Engine.Shapes;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Collision
{
    /// <summary>
    /// </summary>
    public sealed class SpatialGrid
    {
        /// <summary>
        /// </summary>
        private readonly Dictionary<Point, List<Shape>> storage = new Dictionary<Point, List<Shape>>();
        

        /// <summary>
        /// </summary>
        /// <param name="shapes"></param>
        public SpatialGrid(IEnumerable<Shape> shapes)
        {
            foreach (var shape in shapes)
            {
                var shapeX = shape.BoundingBox.X;
                var shapeY = shape.BoundingBox.Y;
                var width = shape.BoundingBox.Width;
                var height = shape.BoundingBox.Height;

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        this.Insert(new Point(shapeX + x, shapeY + y), shape);
                    }
                }
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="shape"></param>
        public void Insert(Point point, Shape shape)
        {
            if (!this.storage.ContainsKey(point))
                this.storage[point] = new List<Shape>();

            this.storage[point].Add(shape);
        }


        /// <summary>
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public IEnumerable<Shape> GetFromArea(Rectangle rectangle)
        {
            for (var y = 0; y < rectangle.Size.Y; y++)
            {
                for (var x = 0; x < rectangle.Size.X; x++)
                {
                    var point = new Point(rectangle.X + x, rectangle.Y + y);
                    if (this.storage.TryGetValue(point, out var shapes))
                    {
                        foreach (var shape in shapes)
                        {
                            yield return shape;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="oldPosition"></param>
        /// <param name="newPosition"></param>
        /// <param name="shape"></param>
        public void Move(Point oldPosition, Point newPosition, Shape shape)
        {
            if (this.storage.TryGetValue(oldPosition, out var shapes))
            {
                shapes.Remove(shape);

                this.Insert(newPosition, shape);
            }
        }
    }
}