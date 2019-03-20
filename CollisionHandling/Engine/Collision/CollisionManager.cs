#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CollisionFloatTestNewMono.Engine.Math2;
using CollisionFloatTestNewMono.Engine.Shapes;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Collision
{
    /// <summary>
    /// </summary>
    public sealed class CollisionManager
    {
        /// <summary>
        /// </summary>
        [Flags]
        private enum BorderlineType
        {
            None = 0,
            Left = 1,
            Top = 2,
            Right = 4,
            Bottom = 8
        }


        /// <summary>
        /// </summary>
        private sealed class BorderlineBlock
        {
            public BorderlineType BorderlineType;
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public bool IsSearchedLeft;
            public bool IsSearchedTop;
            public bool IsSearchedRight;
            public bool IsSearchedBottom;
        }


        /// <summary>
        /// </summary>
        public event SeperationHandler OnSeparation;

        /// <summary>
        /// </summary>
        public event CollisionHandler OnCollision;


        /// <summary>
        /// </summary>
        private readonly SpatialGrid spatialGrid;

        /// <summary>
        /// </summary>
        private readonly List<Shape> shapes = new List<Shape>();

        /// <summary>
        /// </summary>
        private readonly Rectangle searchArea = new Rectangle(-5, -5, 10, 10);

        /// <summary>
        ///     Circle = 0,
        ///     Line = 1,
        ///     Polygon = 2,
        /// </summary>
        private static readonly ShapeContactType[,] registers =
        {
            {
                ShapeContactType.Circle, // 0,0 = Circle->Circle
                ShapeContactType.LineAndCircle, // 0,1 = Circle->Line
                ShapeContactType.PolygonAndCircle, // 0,2 = Circle->Polygon
            },
            {
                ShapeContactType.LineAndCircle, // 1,0 = Line->Circle
                ShapeContactType.NotSupported, // 1,1 = Line->Line
                ShapeContactType.LineAndPolygon // 1,2 = Line->Polygon
            },
            {
                ShapeContactType.PolygonAndCircle, // 2,0 = Polygon->Circle
                ShapeContactType.LineAndPolygon, // 2,1 = Polygon->Line
                ShapeContactType.Polygon // 2,2 = Polygon->Polygon
            }
        };
        
        /// <summary>
        /// </summary>
        private readonly HashSet<ShapeCollision> collisions = new HashSet<ShapeCollision>();

        /// <summary>
        /// </summary>
        public IEnumerable<Shape> Shapes => this.shapes.AsReadOnly();


        /// <summary>
        /// </summary>
        /// <param name="gridWidthInTiles"></param>
        /// <param name="gridHeightInTiles"></param>
        public CollisionManager(int gridWidthInTiles, int gridHeightInTiles)
        {
            this.spatialGrid = new SpatialGrid(gridWidthInTiles, gridHeightInTiles);
        }


        /// <summary>
        /// </summary>
        /// <param name="shape"></param>
        public void AddShape(Shape shape)
        {
            this.shapes.Add(shape);
            this.spatialGrid.Insert(shape);
        }


        /// <summary>
        /// </summary>
        /// <param name="shapes"></param>
        public void AddShapes(ICollection<Shape> shapes)
        {
            this.shapes.AddRange(shapes);
            foreach (var shape in shapes)
                this.spatialGrid.Insert(shape);
        }


        /// <summary>
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public ICollection<Shape> AllShapesAround(Shape shape)
        {
            // Umliegende Shapes
            var currentWorldPosition = shape.Position;
            var currentTilePosition = GameHelper.ConvertPositionToTilePosition(currentWorldPosition);
            return this.spatialGrid.GetFromArea(
                currentTilePosition.X + this.searchArea.X,
                currentTilePosition.Y + this.searchArea.Y,
                this.searchArea.Width, this.searchArea.Height);
        }


        /// <summary>
        /// </summary>
        /// <param name="mapCollisionData"></param>
        /// <returns></returns>
        private static int[,] ReverseMapCollisionData(int[,] mapCollisionData)
        {
            var width = mapCollisionData.GetLength(1);
            var height = mapCollisionData.GetLength(0);
            var reversedMapData = new int[width, height];

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                reversedMapData[x, y] = mapCollisionData[y, x];

            return reversedMapData;
        }


        /// <summary>
        /// </summary>
        /// <param name="borderlineBlock"></param>
        /// <param name="borderlineType"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void MarkAsSearched(BorderlineBlock borderlineBlock, BorderlineType borderlineType)
        {
            switch (borderlineType)
            {
                case BorderlineType.Left:
                    borderlineBlock.IsSearchedLeft = true;
                    break;
                case BorderlineType.Top:
                    borderlineBlock.IsSearchedTop = true;
                    break;
                case BorderlineType.Right:
                    borderlineBlock.IsSearchedRight = true;
                    break;
                case BorderlineType.Bottom:
                    borderlineBlock.IsSearchedBottom = true;
                    break;
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="borderLines"></param>
        /// <param name="borderlineType"></param>
        /// <returns></returns>
        private static IEnumerable<BorderlineBlock> FindLines(int width, int height, BorderlineBlock[,] borderLines, BorderlineType borderlineType)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var borderLine = borderLines[x, y];
                    if (borderLine == null)
                        continue;

                    if (borderlineType == BorderlineType.Left && borderLine.IsSearchedLeft ||
                        borderlineType == BorderlineType.Top && borderLine.IsSearchedTop ||
                        borderlineType == BorderlineType.Right && borderLine.IsSearchedRight ||
                        borderlineType == BorderlineType.Bottom && borderLine.IsSearchedBottom)
                        continue;

                    var newBorderLine = new BorderlineBlock
                    {
                        X = x,
                        Y = y,
                        BorderlineType = borderlineType
                    };

                    var nextNodeX = x;
                    var nextNodeY = y;
                    while (nextNodeX < width && nextNodeY < height &&
                           borderLines[nextNodeX, nextNodeY] != null &&
                           (borderLines[nextNodeX, nextNodeY].BorderlineType & borderlineType) == borderlineType)
                    {
                        switch (borderlineType)
                        {
                            case BorderlineType.Left:
                            case BorderlineType.Right:

                                // Markieren
                                newBorderLine.Height += 1;
                                MarkAsSearched(borderLines[nextNodeX, nextNodeY], borderlineType);
                                nextNodeY += 1;

                                break;
                            case BorderlineType.Top:
                            case BorderlineType.Bottom:

                                // Markieren
                                newBorderLine.Width += 1;
                                MarkAsSearched(borderLines[nextNodeX, nextNodeY], borderlineType);
                                nextNodeX += 1;

                                break;
                        }
                    }

                    // Markieren
                    MarkAsSearched(newBorderLine, borderlineType);

                    // Nur zurückgeben, wenn es etwas gibt
                    if (newBorderLine.Width > 0 || newBorderLine.Height > 0)
                        yield return newBorderLine;
                }
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        private static LineShape CreateLine(int p0, int p1, int p2, int p3)
        {
            return new LineShape(new Vector2(p0, p1), new Vector2(p2, p3));
        }


        /// <summary>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mapCollisionData"></param>
        /// <param name="useOuterBorder"></param>
        public IEnumerable<LineShape> CreateHullForBody(int width, int height, int[,] mapCollisionData, bool useOuterBorder)
        {
            // Neuen Puffer buaen
            var mapDataReversed = ReverseMapCollisionData(mapCollisionData);

            // Die Border bestimmen
            var borderLines = new BorderlineBlock[width, height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (mapDataReversed[x, y] != 1)
                        continue;

                    var borderlineBlock = new BorderlineBlock();

                    if (x + 1 >= width || (x + 1 < width && mapDataReversed[x + 1, y] == 0))
                        borderlineBlock.BorderlineType |= BorderlineType.Right;

                    if (x - 1 < 0 || (x - 1 >= 0 && mapDataReversed[x - 1, y] == 0))
                        borderlineBlock.BorderlineType |= BorderlineType.Left;

                    if (y + 1 >= height || (y + 1 < height && mapDataReversed[x, y + 1] == 0))
                        borderlineBlock.BorderlineType |= BorderlineType.Bottom;

                    if (y - 1 < 0 || (y - 1 >= 0 && mapDataReversed[x, y - 1] == 0))
                        borderlineBlock.BorderlineType |= BorderlineType.Top;

                    borderLines[x, y] = borderlineBlock;
                }
            }

            // Borders
            var leftBorders = FindLines(width, height, borderLines, BorderlineType.Left).ToArray();
            var topBorders = FindLines(width, height, borderLines, BorderlineType.Top).ToArray();
            var rightBorders = FindLines(width, height, borderLines, BorderlineType.Right).ToArray();
            var bottomBorders = FindLines(width, height, borderLines, BorderlineType.Bottom).ToArray();

            // Border bauen
            foreach (var borderlineBlock in leftBorders)
            {
                yield return CreateLine(
                    borderlineBlock.X * GameHelper.TileSize,
                    borderlineBlock.Y * GameHelper.TileSize,
                    borderlineBlock.X * GameHelper.TileSize,
                    borderlineBlock.Y * GameHelper.TileSize + borderlineBlock.Height * GameHelper.TileSize);
            }

            // Border bauen
            foreach (var borderlineBlock in topBorders)
            {
                yield return CreateLine(
                    borderlineBlock.X * GameHelper.TileSize,
                    borderlineBlock.Y * GameHelper.TileSize,
                    borderlineBlock.X * GameHelper.TileSize + borderlineBlock.Width * GameHelper.TileSize,
                    borderlineBlock.Y * GameHelper.TileSize);
            }

            // Border bauen
            foreach (var borderlineBlock in rightBorders)
            {
                yield return CreateLine(
                    borderlineBlock.X * GameHelper.TileSize + borderlineBlock.Width * GameHelper.TileSize + GameHelper.TileSize,
                    borderlineBlock.Y * GameHelper.TileSize,
                    borderlineBlock.X * GameHelper.TileSize + borderlineBlock.Width * GameHelper.TileSize + GameHelper.TileSize,
                    borderlineBlock.Y * GameHelper.TileSize + borderlineBlock.Height * GameHelper.TileSize);
            }

            // Border bauen
            foreach (var borderlineBlock in bottomBorders)
            {
                yield return CreateLine(
                    borderlineBlock.X * GameHelper.TileSize,
                    borderlineBlock.Y * GameHelper.TileSize + borderlineBlock.Height * GameHelper.TileSize + GameHelper.TileSize,
                    borderlineBlock.X * GameHelper.TileSize + borderlineBlock.Width * GameHelper.TileSize,
                    borderlineBlock.Y * GameHelper.TileSize + borderlineBlock.Height * GameHelper.TileSize + GameHelper.TileSize);
            }

            // Äußeren Rand miteinbeziehen
            if (useOuterBorder)
            {
                // Linke Wand
                yield return CreateLine(
                    0, 0,
                    0, height * GameHelper.TileSize);

                // Obere Wand
                yield return CreateLine(
                    0, 0,
                    width * GameHelper.TileSize, 0);

                // Rechte Wand
                yield return CreateLine(
                    width * GameHelper.TileSize, 0,
                    width * GameHelper.TileSize, height * GameHelper.TileSize);

                // Untere Wand
                yield return CreateLine(
                    0, height * GameHelper.TileSize,
                    width * GameHelper.TileSize, height * GameHelper.TileSize);
            }
        }


        /// <summary>
        ///     A + dot(AP,AB) / dot(AB,AB) * AB
        /// </summary>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private static Vector2 ProjectPointOnLine(Vector2 linePoint1, Vector2 linePoint2, Vector2 point)
        {
            var ap = new Vector2(point.X - linePoint1.X, point.Y - linePoint1.Y);
            var ab = new Vector2(linePoint2.X - linePoint1.X, linePoint2.Y - linePoint1.Y);

            var coef = Vector2.Dot(ap, ab) / Vector2.Dot(ab, ab);
            return new Vector2(linePoint1.X + coef * ab.X, linePoint1.Y + coef * ab.Y);
        }


        /// <summary>
        /// </summary>
        /// <param name="linePoint1"></param>
        /// <param name="linePoint2"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private static bool IsProjectedPointOnLine(Vector2 linePoint1, Vector2 linePoint2, Vector2 point)
        {
            return linePoint1.X < point.X && point.X < linePoint2.X ||
                   linePoint1.X > point.X && point.X > linePoint2.X ||
                   linePoint1.Y < point.Y && point.Y < linePoint2.Y ||
                   linePoint1.Y > point.Y && point.Y > linePoint2.Y;
        }


        /// <summary>
        /// </summary>
        /// <param name="projectedPoint"></param>
        /// <param name="circleCenter"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private static Vector2 FindNearestExitVector(Vector2 projectedPoint, Vector2 circleCenter, float radius)
        {
            var l1 = circleCenter.X - projectedPoint.X;
            var l2 = circleCenter.Y - projectedPoint.Y;
            var pointToCircleDist = (float)Math.Sqrt(l1 * l1 + l2 * l2);

            if (pointToCircleDist < radius)
            {
                var minDistance = radius - pointToCircleDist;
                return new Vector2(l1 / pointToCircleDist * minDistance, l2 / pointToCircleDist * minDistance);
            }

            return Vector2.Zero;
        }


        /// <summary>
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="circleB"></param>
        /// <returns></returns>
        private static Vector2 CollidesLineAndCircle(LineShape lineA, CircleShape circleB)
        {
            var center = circleB.Position + circleB.Velocity;
            var radius = circleB.Radius;
            var lineStart = lineA.Start;
            var lineEnd = lineA.End;
            var nearestVector = Vector2.Zero;
            var pointOnLine = ProjectPointOnLine(lineStart, lineEnd, center);
            var localNearestVector = FindNearestExitVector(pointOnLine, center, radius);

            var isPointInSegment = IsProjectedPointOnLine(lineStart, lineEnd, pointOnLine);
            if (isPointInSegment)
            {
                nearestVector.X += localNearestVector.X;
                nearestVector.Y += localNearestVector.Y;

                center.X += localNearestVector.X;
                center.Y += localNearestVector.Y;
            }
            else
            {
                // check if line points are intersecting
                var nv1 = FindNearestExitVector(lineStart, center, radius);

                nearestVector.X += nv1.X;
                nearestVector.Y += nv1.Y;
                center.X += nv1.X;
                center.Y += nv1.Y;

                var nv2 = FindNearestExitVector(lineEnd, center, radius);
                nearestVector.X += nv2.X;
                nearestVector.Y += nv2.Y;
                center.X += nv2.X;
                center.Y += nv2.Y;
            }

            return nearestVector;
        }


        /// <summary>
        ///     Compute the collision between a polygon and a circle.
        /// </summary>
        /// <param name="polyA"></param>
        /// <param name="circleB"></param>
        private static Vector2 CollidesPolygonAndCircle(PolygonShape polyA, CircleShape circleB)
        {
            var testPoly = new Polygon(polyA.Vertices);
            var testCircle = new Circle(circleB.Radius);
            var rota = new Rotation(polyA.Rotation);

            var intercectsMtv = CollisionHelper.IntersectMtv(testPoly, testCircle,
                polyA.Position + polyA.Velocity,
                circleB.Position - new Vector2(testCircle.Radius) + circleB.Velocity,
                rota);

            return intercectsMtv;
        }


        /// <summary>
        ///     Compute the collision between a polygon and a circle.
        /// </summary>
        /// <param name="polyA"></param>
        /// <param name="circleB"></param>
        private static Vector2 CollidesPolygonAndCircle(CircleShape circleB, PolygonShape polyA)
        {
            var testPoly = new Polygon(polyA.Vertices);
            var testCircle = new Circle(circleB.Radius);
            var rota = new Rotation(polyA.Rotation);

            var intercectsMtv = CollisionHelper.IntersectMtv(testCircle, testPoly,
                circleB.Position - new Vector2(testCircle.Radius) + circleB.Velocity,
                polyA.Position + polyA.Velocity,
                rota);

            return intercectsMtv;
        }


        /// <summary>
        /// </summary>
        /// <param name="circleA"></param>
        /// <param name="circleB"></param>
        /// <returns></returns>
        private static Vector2 CollidesCircles(CircleShape circleA, CircleShape circleB)
        {
            var testCircle1 = new Circle(circleA.Radius);
            var testCircle2 = new Circle(circleB.Radius);

            var intercectsMtv = CollisionHelper.IntersectMtv(testCircle1, testCircle2,
                circleA.Position - new Vector2(circleA.Radius) + circleA.Velocity,
                circleB.Position - new Vector2(circleB.Radius) + circleB.Velocity);

            return intercectsMtv;
        }


        /// <summary>
        ///     Compute the collision manifold between two polygons.
        /// </summary>
        /// <param name="polyA"></param>
        /// <param name="polyB"></param>
        /// <returns></returns>
        private static Vector2 CollidePolygons(PolygonShape polyA, PolygonShape polyB)
        {
            var testPoly1 = new Polygon(polyA.Vertices);
            var testPoly2 = new Polygon(polyB.Vertices);

            var intercectsMtv = CollisionHelper.IntersectMtv(testPoly1, testPoly2,
                polyA.Position + polyA.Velocity,
                polyB.Position + polyB.Velocity,
                new Rotation(polyA.Rotation),
                new Rotation(polyB.Rotation));

            return intercectsMtv;
        }


        /// <summary>
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="polyB"></param>
        /// <returns></returns>
        private static Vector2 CollidesLineAndPolygon(LineShape lineA, PolygonShape polyB)
        {
            // TODO: implementieren!
            return Vector2.Zero;
        }


        /// <summary>
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="circleB"></param>
        /// <returns></returns>
        private static bool OverlapLineAndCircle(LineShape lineA, CircleShape circleB)
        {
            // TODO: make that better!
            return CollidesLineAndCircle(lineA, circleB) != Vector2.Zero;
        }


        /// <summary>
        /// </summary>
        /// <param name="circleA"></param>
        /// <param name="circleB"></param>
        /// <returns></returns>
        private static bool OverlapCircles(CircleShape circleA, CircleShape circleB)
        {
            var testCircle1 = new Circle(circleA.Radius);
            var testCircle2 = new Circle(circleB.Radius);

            var intercects = CollisionHelper.Intersects(testCircle1, testCircle2,
                circleA.Position - new Vector2(circleA.Radius) + circleA.Velocity,
                circleB.Position - new Vector2(circleB.Radius) + circleB.Velocity, true);

            return intercects;
        }


        /// <summary>
        /// </summary>
        /// <param name="polyA"></param>
        /// <param name="polyB"></param>
        /// <returns></returns>
        private static bool OverlapPolygons(PolygonShape polyA, PolygonShape polyB)
        {
            var testPoly1 = new Polygon(polyA.Vertices);
            var testPoly2 = new Polygon(polyB.Vertices);

            var intercects = CollisionHelper.Intersects(testPoly1, testPoly2,
                polyA.Position + polyA.Velocity,
                polyB.Position + polyB.Velocity,
                new Rotation(polyA.Rotation),
                new Rotation(polyB.Rotation), true);

            return intercects;
        }


        /// <summary>
        /// </summary>
        /// <param name="polyA"></param>
        /// <param name="circleB"></param>
        /// <returns></returns>
        private static bool OverlapPolygonAndCircle(PolygonShape polyA, CircleShape circleB)
        {
            var testPoly = new Polygon(polyA.Vertices);
            var testCircle = new Circle(circleB.Radius);
            var rota = new Rotation(polyA.Rotation);

            var intercects = CollisionHelper.Intersects(testPoly, testCircle,
                polyA.Position + polyA.Velocity,
                circleB.Position - new Vector2(testCircle.Radius) + circleB.Velocity,
                rota, true);

            return intercects;
        }


        /// <summary>
        /// </summary>
        /// <param name="circleB"></param>
        /// <param name="polyA"></param>
        /// <returns></returns>
        private static bool OverlapPolygonAndCircle(CircleShape circleB, PolygonShape polyA)
        {
            var testPoly = new Polygon(polyA.Vertices);
            var testCircle = new Circle(circleB.Radius);
            var rota = new Rotation(polyA.Rotation);

            var intercects = CollisionHelper.Intersects(testCircle, testPoly,
                circleB.Position - new Vector2(testCircle.Radius) + circleB.Velocity,
                polyA.Position + polyA.Velocity,
                rota, true);

            return intercects;
        }


        /// <summary>
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="polyB"></param>
        /// <returns></returns>
        private static bool OverlapLineAndPolygon(LineShape lineA, PolygonShape polyB)
        {
            // TODO: implementieren!
            return false;
        }


        /// <summary>
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="position"></param>
        public void SetShapePosition(Shape shape, Vector2 position)
        {
            var oldPosition = GameHelper.ConvertPositionToTilePosition(shape.Position);
            shape.SetPosition(position);

            var newPosition = GameHelper.ConvertPositionToTilePosition(shape.Position);
            this.spatialGrid.Move(oldPosition, newPosition, shape);
        }


        /// <summary>
        /// </summary>
        /// <param name="shapeA"></param>
        /// <param name="shapeB"></param>
        public void IgnoreCollisionsOf(Shape shapeA, Shape shapeB)
        {
            shapeA.IgnoredCollisions.Add(shapeB);
            shapeB.IgnoredCollisions.Add(shapeA);
        }


        /// <summary>
        /// </summary>
        /// <param name="shapeA"></param>
        /// <param name="shapeB"></param>
        public void EnableCollisionWith(Shape shapeA, Shape shapeB)
        {
            shapeA.IgnoredCollisions.Remove(shapeB);
            shapeB.IgnoredCollisions.Remove(shapeA);
        }


        /// <summary>
        /// </summary>
        /// <param name="shapeA"></param>
        /// <param name="allShapesAround"></param>
        /// <returns></returns>
        private void SolveCollisions(Shape shapeA, ICollection<Shape> allShapesAround)
        {
            this.collisions.Clear();
            var iterations = 0;
            bool hasCollison;
            do
            {
                hasCollison = false;

                foreach (var shapeB in allShapesAround)
                {
                    if (shapeA.IsStatic ||
                        shapeA == shapeB ||
                        shapeA.IsStatic == shapeB.IsStatic ||
                        !shapeA.IsEnabled ||
                        !shapeB.IsEnabled ||
                        shapeA.IgnoredCollisions.Contains(shapeB) ||
                        shapeB.IgnoredCollisions.Contains(shapeA) || 
                        !shapeA.CollidesOnyWithCategories.HasFlag(shapeB.CollisionCategory))
                        continue;

                    var type1 = shapeA.ShapeType;
                    var type2 = shapeB.ShapeType;
                    Shape sortedShapeA;
                    Shape sortedShapeB;

                    if ((type1 >= type2 || type1 == ShapeType.Line && type2 == ShapeType.Polygon) && !(type2 == ShapeType.Line && type1 == ShapeType.Polygon))
                    {
                        sortedShapeA = shapeA;
                        sortedShapeB = shapeB;
                    }
                    else
                    {
                        sortedShapeA = shapeB;
                        sortedShapeB = shapeA;
                    }

                    var newVelocity = Vector2.Zero;
                    var isSensor = shapeA.IsSensor || shapeB.IsSensor;
                    var isOverlapping = false;
                    var shapeContactType = registers[(int)shapeA.ShapeType, (int)shapeB.ShapeType];
                    switch (shapeContactType)
                    {
                        case ShapeContactType.Circle:

                            if (isSensor)
                            {
                                // Circle->Circle
                                isOverlapping = OverlapCircles((CircleShape)sortedShapeA, (CircleShape)sortedShapeB);
                            }
                            else
                            {
                                // Circle->Circle
                                newVelocity += CollidesCircles((CircleShape)sortedShapeA, (CircleShape)sortedShapeB);
                            }

                            break;
                        case ShapeContactType.PolygonAndCircle:

                            if (shapeA is PolygonShape && shapeB is CircleShape)
                            {
                                if (isSensor)
                                {
                                    // Circle->Circle
                                    isOverlapping = OverlapPolygonAndCircle((PolygonShape)sortedShapeA, (CircleShape)sortedShapeB);
                                }
                                else
                                {
                                    // Polygon->Circle
                                    newVelocity += CollidesPolygonAndCircle((PolygonShape)sortedShapeA, (CircleShape)sortedShapeB);
                                }
                            }
                            else
                            {
                                if (isSensor)
                                {
                                    // Circle->Circle
                                    isOverlapping = OverlapPolygonAndCircle((CircleShape)sortedShapeB, (PolygonShape)sortedShapeA);
                                }
                                else
                                {
                                    // Polygon->Circle
                                    newVelocity += CollidesPolygonAndCircle((CircleShape)sortedShapeB, (PolygonShape)sortedShapeA);
                                }
                            }

                            break;
                        case ShapeContactType.LineAndCircle:

                            if (isSensor)
                            {
                                // Circle->Circle
                                isOverlapping = OverlapLineAndCircle((LineShape)sortedShapeA, (CircleShape)sortedShapeB);
                            }
                            else
                            {
                                // Line->Circle
                                newVelocity += CollidesLineAndCircle((LineShape)sortedShapeA, (CircleShape)sortedShapeB);
                            }

                            break;
                        case ShapeContactType.Polygon:

                            if (isSensor)
                            {
                                // Circle->Circle
                                isOverlapping = OverlapPolygons((PolygonShape)sortedShapeA, (PolygonShape)sortedShapeB);
                            }
                            else
                            {
                                // Polygon->Polygon
                                newVelocity += CollidePolygons((PolygonShape)sortedShapeA, (PolygonShape)sortedShapeB);
                            }

                            break;
                        case ShapeContactType.LineAndPolygon:

                            if (isSensor)
                            {
                                // Circle->Circle
                                isOverlapping = OverlapLineAndPolygon((LineShape)sortedShapeA, (PolygonShape)sortedShapeB);
                            }
                            else
                            {
                                // Polygon->Circle
                                newVelocity += CollidesLineAndPolygon((LineShape)sortedShapeA, (PolygonShape)sortedShapeB);
                            }

                            break;
                    }

                    if (isOverlapping)
                    {
                        // Contact
                        var shapeCollision = new ShapeCollision(shapeContactType, shapeA, shapeB);

                        // Add collison contact
                        this.collisions.Add(shapeCollision);
                    }
                    else
                    {
                        if (newVelocity != Vector2.Zero)
                        {
                            // Contact
                            var shapeCollision = new ShapeCollision(shapeContactType, shapeA, shapeB);

                            // Collision
                            hasCollison = this.OnSeparation?.Invoke(this, new ShapeCollision(shapeContactType, shapeA, shapeB)) ?? true;

                            // Apply velocity
                            if (hasCollison)
                            {
                                // Apply velocity
                                shapeA.ApplyVelocity(newVelocity);

                                // Add collison contact
                                this.collisions.Add(shapeCollision);
                            }
                        }
                    }
                }

                iterations++;
            }
            while (hasCollison && iterations <= 10);
        }


        /// <summary>
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            foreach (var shape in this.shapes)
            {
                // Ignore
                if (shape.IsStatic || !shape.IsEnabled)
                    continue;

                // Umliegende Shapes
                var shapesAround = this.AllShapesAround(shape);

                // Solve collisions
                this.SolveCollisions(shape, shapesAround);

                // Collisions
                if (this.OnCollision != null && this.collisions.Count > 0)
                {
                    foreach (var shapeCollision in this.collisions)
                    {
                        this.OnCollision.Invoke(this, shapeCollision);
                    }
                }

                // Ignore if no velocity
                if (shape.Velocity == Vector2.Zero)
                    continue;

                // Move by velocity
                switch (shape)
                {
                    case CircleShape circleShape:
                        {
                            var oldPosition = circleShape.TilePosition;
                            circleShape.MoveByVelocity(circleShape.Velocity);
                            this.spatialGrid.Move(oldPosition, circleShape.TilePosition, circleShape);
                        }
                        break;
                    case PolygonShape polygonShape:
                        {
                            //var oldPosition = polygonShape.TilePosition;
                            //polygonShape.MoveByVelocity(new Vector2((int)Math.Round(polygonShape.Velocity.X), (int)Math.Round(polygonShape.Velocity.Y)));
                            //this.spatialGrid.Move(oldPosition, polygonShape.TilePosition, polygonShape);
                        }
                        break;
                }

                // Reset
                shape.ResetVelocity();
            }
        }
    }
}