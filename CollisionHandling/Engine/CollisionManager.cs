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
        private const float LinearSlop = 0.005f;
        
        /// <summary>
        /// The maximum number of contact points between two convex shapes.
        /// DO NOT CHANGE THIS VALUE!
        /// </summary>
        private const int MaxManifoldPoints = 2;


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
        /// <param name="lineShape"></param>
        /// <param name="circleShape"></param>
        /// <returns></returns>
        public Vector2 CircleAndLine(LineShape lineShape, CircleShape circleShape)
        {
            var center = circleShape.Position + circleShape.Velocity;
            var radius = circleShape.Radius;
            var lineStart = lineShape.Start;
            var lineEnd = lineShape.End;
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
        ///     https://github.com/VelcroPhysics/VelcroPhysics/blob/1456abf40e4c30065bf122f409ce60ce3873ff09/VelcroPhysics/Collision/Narrowphase/CollideCircle.cs
        /// </summary>
        /// <param name="polygonShape"></param>
        /// <param name="circleShape"></param>
        public Vector2 CollidesPolygonAndCircle(PolygonShape polygonShape, CircleShape circleShape)
        {
            // Find the min separating edge.
            var normalIndex = 0;
            var separation = -MaxFloat;
            var radius = circleShape.Radius;
            var vertexCount = polygonShape.Vertices.Length;
            var vertices = polygonShape.Vertices;
            var normals = VectorHelper.CreateNormals(vertices);
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
                return new Vector2(circleShape.Radius) * normals[normalIndex] - separation * normals[normalIndex];

            // Compute barycentric coordinates
            var u1 = Vector2.Dot(center - v1, v2 - v1);
            var u2 = Vector2.Dot(center - v2, v1 - v2);

            if (u1 <= 0.0f)
            {
                if (Vector2.DistanceSquared(center, v1) > radius * radius)
                    return Vector2.Zero;

                var localNormal = center - v1;
                localNormal.Normalize();

                return new Vector2(circleShape.Radius) * localNormal - separation * localNormal;
            }

            if (u2 <= 0.0f)
            {
                if (Vector2.DistanceSquared(center, v2) > radius * radius)
                    return Vector2.Zero;

                var localNormal = center - v2;
                localNormal.Normalize();

                return new Vector2(circleShape.Radius) * localNormal - separation * localNormal;
            }

            var faceCenter = 0.5f * (v1 + v2);
            var sDot = Vector2.Dot(center - faceCenter, normals[vertIndex1]);
            if (sDot > radius)
                return Vector2.Zero;

            return new Vector2(circleShape.Radius) * normals[vertIndex1] - sDot * normals[vertIndex1];
        }


        /// <summary>
        /// </summary>
        /// <param name="circleShape"></param>
        /// <param name="circleShapeObs"></param>
        /// <returns></returns>
        public Vector2 CollidesCircleAndCircle(CircleShape circleShape, CircleShape circleShapeObs)
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
        ///     Clipping for contact manifolds.
        /// </summary>
        /// <param name="vOut">The v out.</param>
        /// <param name="vIn">The v in.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="vertexIndexA">The vertex index A.</param>
        /// <returns></returns>
        private static int ClipSegmentToLine(out FixedArray2<ClipVertex> vOut, ref FixedArray2<ClipVertex> vIn, Vector2 normal, float offset, int vertexIndexA)
        {
            vOut = new FixedArray2<ClipVertex>();

            // Start with no output points
            int numOut = 0;

            // Calculate the distance of end points to the line
            float distance0 = Vector2.Dot(normal, vIn.Value0.V) - offset;
            float distance1 = Vector2.Dot(normal, vIn.Value1.V) - offset;

            // If the points are behind the plane
            if (distance0 <= 0.0f) vOut[numOut++] = vIn.Value0;
            if (distance1 <= 0.0f) vOut[numOut++] = vIn.Value1;

            // If the points are on different sides of the plane
            if (distance0 * distance1 < 0.0f)
            {
                // Find intersection point of edge and plane
                float interp = distance0 / (distance0 - distance1);

                ClipVertex cv = vOut[numOut];
                cv.V = vIn.Value0.V + interp * (vIn.Value1.V - vIn.Value0.V);

                // VertexA is hitting edgeB.
                cv.ID.ContactFeature.IndexA = (byte)vertexIndexA;
                cv.ID.ContactFeature.IndexB = vIn.Value0.ID.ContactFeature.IndexB;
                //cv.ID.ContactFeature.TypeA = ContactFeatureType.Vertex;
                //cv.ID.ContactFeature.TypeB = ContactFeatureType.Face;
                vOut[numOut] = cv;

                ++numOut;
            }

            return numOut;
        }


        /// <summary>
        ///     Compute the collision manifold between two polygons.
        /// </summary>
        public Vector2 CollidePolygons(PolygonShape polyA, PolygonShape polyB)
        {
            // Find edge normal of max separation on A - return if separating axis is found
            // Find edge normal of max separation on B - return if separation axis is found
            // Choose reference edge as min(minA, minB)
            // Find incident edge
            // Clip

            float totalRadius = polyA.Radius + polyB.Radius;

            var xfA = polyA.Transform;
            var xfB = polyB.Transform;

            int edgeA;
            float separationA = FindMaxSeparation(out edgeA, polyA, ref xfA, polyB, ref xfB);
            if (separationA > totalRadius)
                return Vector2.Zero;

            int edgeB;
            float separationB = FindMaxSeparation(out edgeB, polyB, ref xfB, polyA, ref xfA);
            if (separationB > totalRadius)
                return Vector2.Zero;

            PolygonShape poly1; // reference polygon
            PolygonShape poly2; // incident polygon
            Transform xf1, xf2;
            int edge1; // reference edge
            bool flip;
            const float k_tol = 0.1f * LinearSlop;

            if (separationB > separationA + k_tol)
            {
                poly1 = polyB;
                poly2 = polyA;
                xf1 = xfB;
                xf2 = xfA;
                edge1 = edgeB;
                //manifold.Type = ManifoldType.FaceB;
                flip = true;
            }
            else
            {
                poly1 = polyA;
                poly2 = polyB;
                xf1 = xfA;
                xf2 = xfB;
                edge1 = edgeA;
                //manifold.Type = ManifoldType.FaceA;
                flip = false;
            }

            FixedArray2<ClipVertex> incidentEdge;
            FindIncidentEdge(out incidentEdge, poly1, ref xf1, edge1, poly2, ref xf2);

            int count1 = poly1.Vertices.Length;
            Vector2[] vertices1 = poly1.Vertices;

            int iv1 = edge1;
            int iv2 = edge1 + 1 < count1 ? edge1 + 1 : 0;

            Vector2 v11 = vertices1[iv1];
            Vector2 v12 = vertices1[iv2];

            Vector2 localTangent = v12 - v11;
            localTangent.Normalize();

            Vector2 localNormal = MathUtils.Cross(localTangent, 1.0f);
            Vector2 planePoint = 0.5f * (v11 + v12);

            Vector2 tangent = MathUtils.Mul(ref xf1.Rotation, localTangent);
            Vector2 normal = MathUtils.Cross(tangent, 1.0f);

            v11 = MathUtils.Mul(ref xf1, v11);
            v12 = MathUtils.Mul(ref xf1, v12);

            // Face offset.
            float frontOffset = Vector2.Dot(normal, v11);

            // Side offsets, extended by polytope skin thickness.
            float sideOffset1 = -Vector2.Dot(tangent, v11) + totalRadius;
            float sideOffset2 = Vector2.Dot(tangent, v12) + totalRadius;

            // Clip incident edge against extruded edge1 side edges.
            FixedArray2<ClipVertex> clipPoints1;
            FixedArray2<ClipVertex> clipPoints2;

            // Clip to box side 1
            int np = ClipSegmentToLine(out clipPoints1, ref incidentEdge, -tangent, sideOffset1, iv1);

            if (np < 2)
                return Vector2.Zero;

            // Clip to negative box side 1
            np = ClipSegmentToLine(out clipPoints2, ref clipPoints1, tangent, sideOffset2, iv2);

            if (np < 2)
                return Vector2.Zero;

            //// Now clipPoints2 contains the clipped points.
            //int pointCount = 0;
            //for (int i = 0; i < MaxManifoldPoints; ++i)
            //{
            //    float separation = Vector2.Dot(normal, clipPoints2[i].V) - frontOffset;

            //    if (separation <= totalRadius)
            //    {
            //        ManifoldPoint cp = manifold.Points[pointCount];
            //        cp.LocalPoint = MathUtils.MulT(ref xf2, clipPoints2[i].V);
            //        cp.Id = clipPoints2[i].ID;

            //        if (flip)
            //        {
            //            // Swap features
            //            ContactFeature cf = cp.Id.ContactFeature;
            //            cp.Id.ContactFeature.IndexA = cf.IndexB;
            //            cp.Id.ContactFeature.IndexB = cf.IndexA;
            //            cp.Id.ContactFeature.TypeA = cf.TypeB;
            //            cp.Id.ContactFeature.TypeB = cf.TypeA;
            //        }

            //        manifold.Points[pointCount] = cp;

            //        ++pointCount;
            //    }
            //}

            //manifold.PointCount = pointCount;
            return Vector2.Zero;
        }


        /// <summary>
        ///     Find the max separation between poly1 and poly2 using edge normals from poly1.
        /// </summary>
        private static float FindMaxSeparation(out int edgeIndex, PolygonShape poly1, ref Transform xf1, PolygonShape poly2, ref Transform xf2)
        {
            int count1 = poly1.Vertices.Length;
            int count2 = poly2.Vertices.Length;
            Vector2[] n1s = poly1.Normals;
            Vector2[] v1s = poly1.Vertices;
            Vector2[] v2s = poly2.Vertices;
            Transform xf = MathUtils.MulT(xf2, xf1);

            int bestIndex = 0;
            float maxSeparation = -MaxFloat;
            for (int i = 0; i < count1; ++i)
            {
                // Get poly1 normal in frame2.
                Vector2 n = MathUtils.Mul(ref xf.Rotation, n1s[i]);
                Vector2 v1 = MathUtils.Mul(ref xf, v1s[i]);

                // Find deepest point for normal i.
                float si = MaxFloat;
                for (int j = 0; j < count2; ++j)
                {
                    float sij = Vector2.Dot(n, v2s[j] - v1);
                    if (sij < si)
                    {
                        si = sij;
                    }
                }

                if (si > maxSeparation)
                {
                    maxSeparation = si;
                    bestIndex = i;
                }
            }

            edgeIndex = bestIndex;
            return maxSeparation;
        }


        /// <summary>
        /// </summary>
        /// <param name="c"></param>
        /// <param name="poly1"></param>
        /// <param name="xf1"></param>
        /// <param name="edge1"></param>
        /// <param name="poly2"></param>
        /// <param name="xf2"></param>
        private static void FindIncidentEdge(out FixedArray2<ClipVertex> c, PolygonShape poly1, ref Transform xf1, int edge1, PolygonShape poly2, ref Transform xf2)
        {
            Vector2[] normals1 = poly1.Normals;

            int count2 = poly2.Vertices.Length;
            Vector2[] vertices2 = poly2.Vertices;
            Vector2[] normals2 = poly2.Normals;

            // Get the normal of the reference edge in poly2's frame.
            Vector2 normal1 = MathUtils.MulT(ref xf2.Rotation, MathUtils.Mul(ref xf1.Rotation, normals1[edge1]));

            // Find the incident edge on poly2.
            int index = 0;
            float minDot = MaxFloat;
            for (int i = 0; i < count2; ++i)
            {
                float dot = Vector2.Dot(normal1, normals2[i]);
                if (dot < minDot)
                {
                    minDot = dot;
                    index = i;
                }
            }

            // Build the clip vertices for the incident edge.
            int i1 = index;
            int i2 = i1 + 1 < count2 ? i1 + 1 : 0;

            c = new FixedArray2<ClipVertex>();
            c.Value0.V = MathUtils.Mul(ref xf2, vertices2[i1]);
            c.Value0.ID.ContactFeature.IndexA = (byte)edge1;
            c.Value0.ID.ContactFeature.IndexB = (byte)i1;
            //c.Value0.ID.ContactFeature.TypeA = ContactFeatureType.Face;
            //c.Value0.ID.ContactFeature.TypeB = ContactFeatureType.Vertex;

            c.Value1.V = MathUtils.Mul(ref xf2, vertices2[i2]);
            c.Value1.ID.ContactFeature.IndexA = (byte)edge1;
            c.Value1.ID.ContactFeature.IndexB = (byte)i2;
            //c.Value1.ID.ContactFeature.TypeA = ContactFeatureType.Face;
            //c.Value1.ID.ContactFeature.TypeB = ContactFeatureType.Vertex;
        }
    }
}