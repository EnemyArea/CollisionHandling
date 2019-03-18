#region

using System.Collections.Generic;
using System.Diagnostics;
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
        /// 
        /// </summary>
        private readonly int gridWidthInTiles;

        /// <summary>
        /// 
        /// </summary>
        private readonly int gridHeightInTiles;

        /// <summary>
        /// </summary>
        private readonly Dictionary<Point, List<Shape>> storage = new Dictionary<Point, List<Shape>>();

        /// <summary>
        /// </summary>
        private readonly List<Shape> allShapesAround = new List<Shape>();


        /// <summary>
        /// </summary>
        /// <param name="gridWidthInTiles"></param>
        /// <param name="gridHeightInTiles"></param>
        /// <param name="shapes"></param>
        public SpatialGrid(int gridWidthInTiles, int gridHeightInTiles, IEnumerable<Shape> shapes)
        {
            var sw = new Stopwatch();
            sw.Start();

            this.gridWidthInTiles = gridWidthInTiles;
            this.gridHeightInTiles = gridHeightInTiles;

            for (var y = 0; y < gridHeightInTiles + 1; y++)
            {
                for (var x = 0; x < gridWidthInTiles + 1; x++)
                {
                    var point = new Point(x, y);
                    this.storage[point] = new List<Shape>();
                }
            }

            foreach (var shape in shapes)
            {
                var shapeX = shape.BoundingBoxTileMap.X;
                var shapeY = shape.BoundingBoxTileMap.Y;
                var width = shape.BoundingBoxTileMap.Width;
                var height = shape.BoundingBoxTileMap.Height;

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        this.Insert(new Point(shapeX + x, shapeY + y), shape);
                    }
                }
            }

            sw.Stop();
            Debug.WriteLine($"SpatialGrid: {sw.Elapsed}");
            // SpatialGrid: 00:00:00.0030600
        }


        /// <summary>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="shape"></param>
        public void Insert(Point point, Shape shape)
        {
            if (point.X < 0 || point.Y < 0 || point.X >= this.gridWidthInTiles || point.Y >= this.gridHeightInTiles)
                return;

            this.storage[point].Add(shape);
        }


        /// <summary>
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public IList<Shape> GetFromArea(Rectangle rectangle)
        {
            this.allShapesAround.Clear();

            for (var y = 0; y < rectangle.Size.Y; y++)
            {
                for (var x = 0; x < rectangle.Size.X; x++)
                {
                    var point = new Point(rectangle.X + x, rectangle.Y + y);
                    if (!this.storage.TryGetValue(point, out var shapes))
                        continue;

                    for (var index = 0; index < shapes.Count; index++)
                    {
                        var shape = shapes[index];
                        this.allShapesAround.Add(shape);
                    }
                }
            }

            return this.allShapesAround;
        }


        /// <summary>
        /// </summary>
        /// <param name="oldPosition"></param>
        /// <param name="newPosition"></param>
        /// <param name="shape"></param>
        public void Move(Point oldPosition, Point newPosition, Shape shape)
        {
            if (!this.storage.TryGetValue(oldPosition, out var shapes))
                return;

            shapes.Remove(shape);
            this.Insert(newPosition, shape);
        }
    }
}