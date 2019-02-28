#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private const float MaxFloat = float.MaxValue;

        /// <summary>
        /// </summary>
        private const float Epsilon = 1.192092896e-07f;


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
        ///     Returns the convex hull from the given vertices.
        ///     Giftwrap convex hull algorithm.
        ///     O(n * h) time complexity, where n is the number of points and h is the number of points on the convex hull.
        ///     See http://en.wikipedia.org/wiki/Gift_wrapping_algorithm for more details.
        ///     Extracted from Box2D
        ///     https://github.com/VelcroPhysics/VelcroPhysics/blob/1456abf40e4c30065bf122f409ce60ce3873ff09/VelcroPhysics/Tools/ConvexHull/GiftWrap/GiftWrap.cs
        /// </summary>
        /// <param name="vertices">The vertices.</param>
        public IList<Vector2> GetConvexHull(IList<Vector2> vertices)
        {
            if (vertices.Count <= 3)
                return vertices;

            // Find the right most point on the hull
            var i0 = 0;
            var x0 = vertices[0].X;
            for (var i = 1; i < vertices.Count; ++i)
            {
                var x = vertices[i].X;
                if (x > x0 || (x == x0 && vertices[i].Y < vertices[i0].Y))
                {
                    i0 = i;
                    x0 = x;
                }
            }

            var hull = new int[vertices.Count];
            var m = 0;
            var ih = i0;

            while (true)
            {
                hull[m] = ih;

                var ie = 0;
                for (var j = 1; j < vertices.Count; ++j)
                {
                    if (ie == ih)
                    {
                        ie = j;
                        continue;
                    }

                    var r = vertices[ie] - vertices[hull[m]];
                    var v = vertices[j] - vertices[hull[m]];
                    var c = MathUtils.Cross(ref r, ref v);
                    if (c < 0.0f)
                    {
                        ie = j;
                    }

                    // Collinearity check
                    if (c == 0.0f && v.LengthSquared() > r.LengthSquared())
                    {
                        ie = j;
                    }
                }

                ++m;
                ih = ie;

                if (ie == i0)
                {
                    break;
                }
            }

            var result = new List<Vector2>(m);

            // Copy vertices.
            for (var i = 0; i < m; ++i)
            {
                result.Add(vertices[hull[i]]);
            }

            return result;
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
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <returns></returns>
        private Vector2 CircleAndLine(Vector2 position, float radius, Vector2 lineStart, Vector2 lineEnd)
        {
            var nearestVector = Vector2.Zero;
            var pointOnLine = ProjectPointOnLine(lineStart, lineEnd, position);
            var localNearestVector = FindNearestExitVector(pointOnLine, position, radius);

            var isPointInSegment = IsProjectedPointOnLine(lineStart, lineEnd, pointOnLine);
            if (isPointInSegment)
            {
                nearestVector.X += localNearestVector.X;
                nearestVector.Y += localNearestVector.Y;

                position.X += localNearestVector.X;
                position.Y += localNearestVector.Y;
            }
            else
            {
                // check if line points are intersecting
                var nv1 = FindNearestExitVector(lineStart, position, radius);

                nearestVector.X += nv1.X;
                nearestVector.Y += nv1.Y;
                position.X += nv1.X;
                position.Y += nv1.Y;

                var nv2 = FindNearestExitVector(lineEnd, position, radius);
                nearestVector.X += nv2.X;
                nearestVector.Y += nv2.Y;
                position.X += nv2.X;
                position.Y += nv2.Y;
            }

            return nearestVector;
        }


        /// <summary>
        /// </summary>
        /// <param name="circleShape"></param>
        /// <param name="lineShape"></param>
        /// <returns></returns>
        public Vector2 CircleAndLine(CircleShape circleShape, LineShape lineShape)
        {
            var center = circleShape.Position + circleShape.Velocity;
            var radius = circleShape.Radius;
            var lineStart = lineShape.Start;
            var lineEnd = lineShape.End;

            return this.CircleAndLine(center, radius, lineStart, lineEnd);
        }


        /// <summary>
        ///     Compute the collision between a polygon and a circle.
        ///     https://github.com/VelcroPhysics/VelcroPhysics/blob/1456abf40e4c30065bf122f409ce60ce3873ff09/VelcroPhysics/Collision/Narrowphase/CollideCircle.cs
        /// </summary>
        /// <param name="polygonShape"></param>
        /// <param name="circleShape"></param>
        public Vector2 CollidePolygonAndCircle(PolygonShape polygonShape, CircleShape circleShape)
        {
            // Find the min separating edge.
            var normalIndex = 0;
            var separation = -MaxFloat;
            var radius = polygonShape.Radius + circleShape.Radius;
            var vertexCount = polygonShape.Vertices.Length;
            var vertices = polygonShape.Vertices;
            var normals = polygonShape.Normals;
            var center = circleShape.Position + circleShape.Velocity;

            for (var i = 0; i < vertexCount; ++i)
            {
                var s = Vector2.Dot(normals[i], center - vertices[i]);
                if (s > radius)
                    return Vector2.Zero;

                if (s > separation)
                {
                    separation = s;
                    normalIndex = i;
                }
            }

            // Vertices that subtend the incident face.
            var vertIndex1 = normalIndex;
            var vertIndex2 = vertIndex1 + 1 < vertexCount ? vertIndex1 + 1 : 0;
            var v1 = vertices[vertIndex1];
            var v2 = vertices[vertIndex2];

            // If the center is inside the polygon ...
            if (separation < Epsilon)
            {
                return separation * normals[normalIndex] - new Vector2(circleShape.Radius) * normals[normalIndex];
            }

            // Compute barycentric coordinates
            var u1 = Vector2.Dot(center - v1, v2 - v1);
            var u2 = Vector2.Dot(center - v2, v1 - v2);

            if (u1 <= 0.0f)
            {
                if (Vector2.DistanceSquared(center, v1) > radius * radius)
                    return Vector2.Zero;

                var localNormal = center - v1;
                localNormal.Normalize();

                return separation * localNormal - new Vector2(circleShape.Radius) * localNormal;
            }

            if (u2 <= 0.0f)
            {
                if (Vector2.DistanceSquared(center, v2) > radius * radius)
                    return Vector2.Zero;

                var localNormal = center - v2;
                localNormal.Normalize();

                return separation * localNormal - new Vector2(circleShape.Radius) * localNormal;
            }

            var faceCenter = 0.5f * (v1 + v2);
            var sDot = Vector2.Dot(center - faceCenter, normals[vertIndex1]);
            if (sDot > radius)
                return Vector2.Zero;

            return sDot * normals[vertIndex1] - new Vector2(circleShape.Radius) * normals[vertIndex1];
        }


        /// <summary>
        /// </summary>
        /// <param name="circleShape"></param>
        /// <param name="circleShapeObs"></param>
        /// <returns></returns>
        public Vector2 CircleAndCircle(CircleShape circleShape, CircleShape circleShapeObs)
        {
            var direction = (circleShape.Position + circleShape.Velocity) - (circleShapeObs.Position + circleShapeObs.Velocity);
            var distance = (float)Math.Round(direction.Length());
            var sumOfRadii = circleShape.Radius + circleShapeObs.Radius;

            if (!(sumOfRadii - distance <= 0))
            {
                var depth = sumOfRadii - distance;
                direction.Normalize();
                return direction * depth;
            }

            return Vector2.Zero;
        }


        /// <summary>
        /// </summary>
        /// <param name="circleShape"></param>
        /// <param name="rectangleShape"></param>
        /// <returns></returns>
        public Vector2 CircleAndRectangle(CircleShape circleShape, RectangleShape rectangleShape)
        {
            var v = new Vector2(
                MathHelper.Clamp(circleShape.Position.X + circleShape.Velocity.X, rectangleShape.Rectangle.Left, rectangleShape.Rectangle.Right),
                MathHelper.Clamp(circleShape.Position.Y + circleShape.Velocity.Y, rectangleShape.Rectangle.Top, rectangleShape.Rectangle.Bottom));

            var newVelocity = Vector2.Zero;
            var collisionDirection = (circleShape.Position + circleShape.Velocity) - v;
            var distanceSquared = collisionDirection.LengthSquared();

            var isColliding = distanceSquared > 0 && (distanceSquared < circleShape.Radius * circleShape.Radius);
            if (isColliding)
            {
                var lineTopStart = new Vector2(rectangleShape.Rectangle.Left, rectangleShape.Rectangle.Top);
                var lineTopEnd = new Vector2(rectangleShape.Rectangle.Right, rectangleShape.Rectangle.Top);

                var nearestVectorTop = this.CircleAndLine(circleShape.Position + circleShape.Velocity + newVelocity, circleShape.Radius, lineTopStart, lineTopEnd);
                if (nearestVectorTop != Vector2.Zero)
                {
                    newVelocity += nearestVectorTop;
                }

                var lineLeftStart = new Vector2(rectangleShape.Rectangle.Left, rectangleShape.Rectangle.Top);
                var lineLeftEnd = new Vector2(rectangleShape.Rectangle.Left, rectangleShape.Rectangle.Bottom);

                var nearestVectorLeft = this.CircleAndLine(circleShape.Position + circleShape.Velocity + newVelocity, circleShape.Radius, lineLeftStart, lineLeftEnd);
                if (nearestVectorLeft != Vector2.Zero)
                {
                    newVelocity += nearestVectorLeft;
                }

                var lineBottomStart = new Vector2(rectangleShape.Rectangle.Left, rectangleShape.Rectangle.Bottom);
                var lineBottomEnd = new Vector2(rectangleShape.Rectangle.Right, rectangleShape.Rectangle.Bottom);

                var nearestVectorBottom = this.CircleAndLine(circleShape.Position + circleShape.Velocity + newVelocity, circleShape.Radius, lineBottomStart, lineBottomEnd);
                if (nearestVectorBottom != Vector2.Zero)
                {
                    newVelocity += nearestVectorBottom;
                }

                var lineRightStart = new Vector2(rectangleShape.Rectangle.Right, rectangleShape.Rectangle.Top);
                var lineRightEnd = new Vector2(rectangleShape.Rectangle.Right, rectangleShape.Rectangle.Bottom);

                var nearestVectorRight = this.CircleAndLine(circleShape.Position + circleShape.Velocity + newVelocity, circleShape.Radius, lineRightStart, lineRightEnd);
                if (nearestVectorRight != Vector2.Zero)
                {
                    newVelocity += nearestVectorRight;
                }
            }

            return newVelocity;
        }
    }
}