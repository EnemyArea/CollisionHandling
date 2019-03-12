// *************************************************************************** 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org>
// ***************************************************************************

#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Collision
{
    /// <summary>
    ///     https://github.com/UnterrainerInformatik/collisiongrid
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CollisionGrid<T>
    {
        private readonly object lockObject = new object();

        private Dictionary<Point, List<T>> Grid { get; set; }
        private Dictionary<T, List<Point>> ItemDictionary { get; set; }
        private Queue<List<Point>> ListOfPointQueue { get; set; }
        private Queue<List<T>> ListOfItemQueue { get; set; }

        private readonly List<Point> lop = new List<Point>();
        private readonly List<T> result = new List<T>();

        public IEnumerable<T> Items => this.ItemDictionary.Keys;

        public IEnumerable<Point> OccupiedCells => this.Grid.Keys;

        public int OccupiedCellCount => this.ItemDictionary.Keys.Count;

        public int ItemCount => this.Grid.Keys.Count;

        public float Width { get; }
        public float Height { get; }

        public float CellWidth { get; }
        public float CellHeight { get; }

        public int NumberOfCellsX { get; }
        public int NumberOfCellsY { get; }

        public CollisionGrid(float width, float height, int numberOfCellsX, int numberOfCellsY)
        {
            this.Width = width;
            this.Height = height;
            this.NumberOfCellsX = numberOfCellsX;
            this.NumberOfCellsY = numberOfCellsY;
            this.CellWidth = this.Width / this.NumberOfCellsX;
            this.CellHeight = this.Height / this.NumberOfCellsY;

            this.ItemDictionary = new Dictionary<T, List<Point>>();
            this.Grid = new Dictionary<Point, List<T>>();

            this.ListOfPointQueue = new Queue<List<Point>>();
            this.ListOfItemQueue = new Queue<List<T>>();
        }

        public T[] Get(Point cell)
        {
            lock (this.lockObject)
            {
                this.Grid.TryGetValue(this.Clamp(cell), out var contents);
                if (contents == null)
                {
                    return new T[0];
                }

                return contents.ToArray();
            }
        }

        /// <summary>
        ///     Gets the first item encountered on the given cell.
        /// </summary>
        /// <param name="cell">The cell to search</param>
        /// <returns>The item or default(T)</returns>
        public T First(Point cell)
        {
            lock (this.lockObject)
            {
                this.Grid.TryGetValue(this.Clamp(cell), out var contents);
                if (contents != null && contents.Count > 0)
                {
                    return contents[0];
                }

                return default(T);
            }
        }

        /// <summary>
        ///     Adds a given item to a given cell.
        ///     If the cell already contains the item, it is not added a second time.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="cell">The cell to add the item to</param>
        public void Add(T item, Point cell)
        {
            lock (this.lockObject)
            {
                var c = this.Clamp(cell);
                this.AddToGrid(item, c);
                this.AddToItems(item, c);
            }
        }

        private void AddToGrid(T item, Point cell)
        {
            this.Grid.TryGetValue(cell, out var l);
            if (l == null)
            {
                if (this.ListOfItemQueue.Count > 0)
                {
                    l = this.ListOfItemQueue.Dequeue();
                }
                else
                {
                    l = new List<T>();
                }

                l.Add(item);
                this.Grid.Add(cell, l);
            }
            else
            {
                if (!l.Contains(item))
                {
                    l.Add(item);
                }
            }
        }

        private void AddToItems(T item, Point cell)
        {
            this.ItemDictionary.TryGetValue(item, out var pl);
            if (pl == null)
            {
                if (this.ListOfPointQueue.Count > 0)
                {
                    pl = this.ListOfPointQueue.Dequeue();
                }
                else
                {
                    pl = new List<Point>();
                }

                pl.Add(cell);
                this.ItemDictionary.Add(item, pl);
            }
            else
            {
                if (!pl.Contains(cell))
                {
                    pl.Add(cell);
                }
            }
        }

        /// <summary>
        ///     Removes all items from the given cell.
        ///     If the items don't occupy another cell, they are removed as well.
        /// </summary>
        /// <param name="cell">The cell to remove items from</param>
        public void Remove(Point cell)
        {
            lock (this.lockObject)
            {
                var c = this.Clamp(cell);
                this.Grid.TryGetValue(c, out var l);

                if (l != null)
                {
                    foreach (var i in l)
                    {
                        this.ItemDictionary.TryGetValue(i, out var pl);
                        if (pl != null)
                        {
                            pl.Remove(c);
                            if (pl.Count == 0)
                            {
                                this.ListOfPointQueue.Enqueue(pl);
                                this.ItemDictionary.Remove(i);
                            }
                        }
                    }

                    l.Clear();
                    this.ListOfItemQueue.Enqueue(l);
                    this.Grid.Remove(cell);
                }
            }
        }

        /// <summary>
        ///     Removes all occurrences of the given item and re-adds it at the new given cell.
        ///     If the item hasn't been in the grid before, this will just add it.
        /// </summary>
        /// <param name="item">The item to move</param>
        /// <param name="cell">The cell to move it to</param>
        public void Move(T item, Point cell)
        {
            lock (this.lockObject)
            {
                this.Remove(item);
                this.Add(item, cell);
            }
        }

        public bool IsEmpty(Point cell)
        {
            lock (this.lockObject)
            {
                return this.Get(cell).Length == 0;
            }
        }

        private void FillList(Rectangle aabb)
        {
            var r = this.Clamp(aabb);
            this.lop.Clear();
            for (var y = 0; y < r.Size.Y; y++)
            {
                for (var x = 0; x < r.Size.X; x++)
                {
                    this.lop.Add(new Point(r.X + x, r.Y + y));
                }
            }
        }

        public T[] Get(Rectangle aabb)
        {
            lock (this.lockObject)
            {
                this.FillList(aabb);
                this.result.Clear();
                foreach (var p in this.lop)
                {
                    foreach (var i in this.Get(p))
                    {
                        if (!this.result.Contains(i))
                        {
                            this.result.Add(i);
                        }
                    }
                }

                return this.result.ToArray();
            }
        }

        /// <summary>
        ///     Gets the first item encountered in the cells that are hit by the given Axis-Aligned-Bounding-Box.
        /// </summary>
        /// <param name="aabb">The Axis-Aligned-Bounding-Box given in int-cell-coordinates</param>
        /// <returns>
        ///     The item or default(T)
        /// </returns>
        public T First(Rectangle aabb)
        {
            lock (this.lockObject)
            {
                this.FillList(aabb);
                this.result.Clear();
                foreach (var p in this.lop)
                {
                    var content = this.First(p);
                    if (!content.Equals(default(T)))
                    {
                        return content;
                    }
                }

                return default(T);
            }
        }

        /// <summary>
        ///     Adds a given item to the cells that are hit by the given Axis-Aligned-Bounding-Box.
        ///     If the cell already contains the item, it is not added a second time.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="aabb">The Axis-Aligned-Bounding-Box given in int-cell-coordinates</param>
        public void Add(T item, Rectangle aabb)
        {
            lock (this.lockObject)
            {
                this.FillList(aabb);
                foreach (var p in this.lop)
                {
                    this.Add(item, p);
                }
            }
        }

        /// <summary>
        ///     Removes all items from the cells that are hit by the given Axis-Aligned-Bounding-Box.
        ///     If the items don't occupy another cell, they are removed as well.
        /// </summary>
        /// <param name="aabb">The Axis-Aligned-Bounding-Box given in int-cell-coordinates</param>
        public void Remove(Rectangle aabb)
        {
            lock (this.lockObject)
            {
                this.FillList(aabb);
                foreach (var p in this.lop)
                {
                    this.Remove(p);
                }
            }
        }

        /// <summary>
        ///     Removes all occurrences of the given item and re-adds it at the new cells that are hit by the given
        ///     Axis-Aligned-Bounding-Box.
        ///     If the item hasn't been in the grid before, this will just add it.
        /// </summary>
        /// <param name="item">The item to move</param>
        /// <param name="aabb">The Axis-Aligned-Bounding-Box given in int-cell-coordinates</param>
        public void Move(T item, Rectangle aabb)
        {
            lock (this.lockObject)
            {
                this.Remove(item);
                this.FillList(aabb);
                foreach (var p in this.lop)
                {
                    this.Add(item, p);
                }
            }
        }

        public bool IsEmpty(Rectangle aabb)
        {
            lock (this.lockObject)
            {
                this.FillList(aabb);
                foreach (var p in this.lop)
                {
                    if (!this.IsEmpty(p))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public Point[] Get(T item)
        {
            lock (this.lockObject)
            {
                this.ItemDictionary.TryGetValue(item, out var pl);
                if (pl == null)
                {
                    return new Point[0];
                }

                return pl.ToArray();
            }
        }

        /// <summary>
        ///     Removes the given item from the grid.
        /// </summary>
        /// <param name="item">The item to remove</param>
        public void Remove(T item)
        {
            lock (this.lockObject)
            {
                this.ItemDictionary.TryGetValue(item, out var pl);
                if (pl == null)
                {
                    return;
                }

                foreach (var p in pl)
                {
                    this.RemoveFromGrid(item, p);
                }

                pl.Clear();
                this.ListOfPointQueue.Enqueue(pl);
                this.ItemDictionary.Remove(item);
            }
        }

        private void RemoveFromGrid(T item, Point cell)
        {
            this.Grid.TryGetValue(cell, out var tl);
            if (tl != null)
            {
                tl.Remove(item);
                if (tl.Count == 0)
                {
                    this.ListOfItemQueue.Enqueue(tl);
                    this.Grid.Remove(cell);
                }
            }
        }

        private Point Cell(Vector2 position)
        {
            return new Point((int)(position.X / this.CellWidth), (int)(position.Y / this.CellHeight));
        }

        private Rectangle Clamp(Rectangle rectangle)
        {
            var tl = this.Clamp(rectangle.Location);
            var br = this.Clamp(rectangle.Location + rectangle.Size - new Point(1, 1));
            var s = br - tl + new Point(1, 1);
            return new Rectangle(tl, s);
        }

        private Point Clamp(Point p)
        {
            var nx = p.X;
            if (nx >= this.NumberOfCellsX)
            {
                nx = this.NumberOfCellsX - 1;
            }

            if (nx < 0)
            {
                nx = 0;
            }

            var ny = p.Y;
            if (ny >= this.NumberOfCellsY)
            {
                ny = this.NumberOfCellsY - 1;
            }

            if (ny < 0)
            {
                ny = 0;
            }

            return new Point(nx, ny);
        }

        public void Dispose()
        {
            lock (this.lockObject)
            {
                this.ListOfPointQueue.Clear();
                this.ListOfPointQueue = null;
                this.ListOfItemQueue.Clear();
                this.ListOfItemQueue = null;
                this.Grid.Clear();
                this.Grid = null;
                this.ItemDictionary.Clear();
                this.ItemDictionary = null;
            }
        }
    }
}