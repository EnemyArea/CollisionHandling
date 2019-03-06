#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CollisionFloatTestNewMono.Engine.Math2;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
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
        public Vector2 CollidesLineAndCircle(LineShape lineA, CircleShape circleB)
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
        public Vector2 CollidesPolygonAndCircle(PolygonShape polyA, CircleShape circleB)
        {
            //// Testing
            //var lines = new List<LineShape>();
            //var rotation = new Rotation2(polygonShape.Rotation);

            //var polygonVertices = polygonShape.Vertices;
            //for (var i = 0; i < polygonVertices.Length - 1; i++)
            //{
            //    var vertex1 = Math2.Math2.Rotate(polygonVertices[i], polygonShape.Center, rotation);
            //    var vertex2 = Math2.Math2.Rotate(polygonVertices[i + 1], polygonShape.Center, rotation);

            //    lines.Add(new LineShape(
            //        vertex1 + polygonShape.Position,
            //        vertex2 + polygonShape.Position
            //    ));

            //    var finalVertex1 = Math2.Math2.Rotate(polygonVertices[polygonVertices.Length - 1], polygonShape.Center, rotation);
            //    var finalVertex2 = Math2.Math2.Rotate(polygonVertices[0], polygonShape.Center, rotation);

            //    lines.Add(new LineShape(
            //        finalVertex1 + polygonShape.Position,
            //        finalVertex2 + polygonShape.Position
            //    ));
            //}

            //foreach (var lineShape in lines)
            //{
            //    var result = this.CollidesLineAndCircle(lineShape, circleShape);
            //    if (result != Vector2.Zero)
            //        return result;
            //}

            //return Vector2.Zero;

            var testPoly = new Polygon2(polyA.Vertices);
            var testCircle = new Circle2(circleB.Radius);
            var rota = new Rotation2(polyA.Rotation);

            if (Shape2.Intersects(testPoly, testCircle,
                polyA.Position + polyA.Velocity,
                circleB.Position - new Vector2(circleB.Radius) + circleB.Velocity,
                rota, true))
            {
                //return Vector2.One;

                var intercectsMtv = Shape2.IntersectMtv(testPoly, testCircle,
                    polyA.Position + polyA.Velocity,
                    circleB.Position - new Vector2(testCircle.Radius) + circleB.Velocity,
                    rota);

                if (intercectsMtv != null)
                    return intercectsMtv.Item1 * intercectsMtv.Item2;
            }

            return Vector2.Zero;
        }


        /// <summary>
        /// </summary>
        /// <param name="circleA"></param>
        /// <param name="circleB"></param>
        /// <returns></returns>
        public Vector2 CollidesCircles(CircleShape circleA, CircleShape circleB)
        {
            //var direction = (circleShape.Position + circleShape.Velocity) - (circleShapeObs.Position + circleShapeObs.Velocity);
            //var distance = (float)Math.Round(direction.Length());
            //var sumOfRadii = circleShape.Radius + circleShapeObs.Radius;

            //if (!(sumOfRadii - distance <= 0))
            //{
            //    var depth = sumOfRadii - distance;
            //    direction.Normalize();
            //    return direction * depth;
            //}

            //return Vector2.Zero;

            var testCircle1 = new Circle2(circleA.Radius);
            var testCircle2 = new Circle2(circleB.Radius);

            var intercectsMtv = Circle2.IntersectMtv(testCircle1, testCircle2,
                circleA.Position + circleA.Velocity,
                circleB.Position + circleB.Velocity);

            if (intercectsMtv != null)
                return intercectsMtv.Item1 * intercectsMtv.Item2;

            return Vector2.Zero;
        }


        /// <summary>
        ///  Compute the collision manifold between two polygons.
        /// </summary>
        /// <param name="polyA"></param>
        /// <param name="polyB"></param>
        /// <returns></returns>
        public Vector2 CollidePolygons(PolygonShape polyA, PolygonShape polyB)
        {
            var testPoly1 = new Polygon2(polyA.Vertices);
            var testPoly2 = new Polygon2(polyB.Vertices);

            var intercectsMtv = Polygon2.IntersectMtv(testPoly1, testPoly2,
                polyA.Position + polyA.Velocity,
                polyB.Position + polyB.Velocity,
                new Rotation2(polyA.Rotation),
                new Rotation2(polyB.Rotation));

            if (intercectsMtv != null)
                return intercectsMtv.Item1 * intercectsMtv.Item2;

            return Vector2.Zero;
        }


        /// <summary>
        /// </summary>
        /// <param name="lineA"></param>
        /// <param name="polyB"></param>
        /// <returns></returns>
        public Vector2 CollidesLineAndPolygon(LineShape lineA, PolygonShape polyB)
        {
            return Vector2.Zero;
        }
    }
}