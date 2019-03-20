#region

using System;
using System.Collections.Generic;
using System.Linq;
using CollisionFloatTestNewMono.Engine.Math2;
using Microsoft.Xna.Framework;
using BoundingBox = CollisionFloatTestNewMono.Engine.Math2.BoundingBox;

#endregion

namespace CollisionFloatTestNewMono.Engine.Collision
{
    /// <summary>
    ///     Contains utility functions for doing math in two-dimensions that
    ///     don't fit elsewhere. Also contains any necessary constants.
    /// </summary>
    public static class CollisionHelper
    {
        #region AxisAlignedLine

        /// <summary>
        ///     Determines if line1 intersects line2.
        /// </summary>
        /// <param name="line1">Line 1</param>
        /// <param name="line">Line 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If line1 and line2 intersect</returns>
        /// <exception cref="ArgumentException">if line1.Axis != line2.Axis</exception>
        public static bool Intersects(AxisAlignedLine line1, AxisAlignedLine line, bool strict)
        {
            if (line1.Axis != line.Axis)
                throw new ArgumentException($"Lines {line1} and {line} are not aligned - you will need to convert to Line2 to check intersection.");

            return Intersects(line1.Min, line1.Max, line.Min, line.Max, strict, false);
        }

        /// <summary>
        ///     Determines the best way for line1 to move to prevent intersection with line2
        /// </summary>
        /// <param name="line1">Line1</param>
        /// <param name="line">Line2</param>
        /// <returns>MTV for line1</returns>
        public static float? IntersectMtv(AxisAlignedLine line1, AxisAlignedLine line)
        {
            if (line1.Axis != line.Axis)
                throw new ArgumentException($"Lines {line1} and {line} are not aligned - you will need to convert to Line2 to check intersection.");

            return IntersectMtv(line1.Min, line1.Max, line.Min, line.Max, false);
        }

        /// <summary>
        ///     Determines if the specified line contains the specified point.
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="point">The point</param>
        /// <param name="strict">If the edges of the line are excluded</param>
        /// <returns>if line contains point</returns>
        public static bool Contains(AxisAlignedLine line, float point, bool strict)
        {
            return Contains(line.Min, line.Max, point, strict, false);
        }

        /// <summary>
        ///     Determines if axis aligned line (min1, max1) intersects (min2, max2)
        /// </summary>
        /// <param name="min1">Min 1</param>
        /// <param name="max1">Max 1</param>
        /// <param name="min2">Min 2</param>
        /// <param name="max2">Max 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <param name="correctMinMax">If true (default true) mins and maxes will be swapped if in the wrong order</param>
        /// <returns>If (min1, max1) intersects (min2, max2)</returns>
        public static bool Intersects(float min1, float max1, float min2, float max2, bool strict, bool correctMinMax = true)
        {
            if (correctMinMax)
            {
                float tmp1 = min1, tmp2 = max1;
                min1 = Math.Min(tmp1, tmp2);
                max1 = Math.Max(tmp1, tmp2);

                tmp1 = min2;
                tmp2 = max2;
                min2 = Math.Min(tmp1, tmp2);
                max2 = Math.Max(tmp1, tmp2);
            }

            if (strict)
                return min1 <= min2 && max1 > min2 || min2 <= min1 && max2 > min1;

            return min1 <= min2 && max1 >= min2 || min2 <= min1 && max2 >= min1;
        }

        /// <summary>
        ///     Determines the translation to move line 1 to have line 1 not intersect line 2. Returns
        ///     null if line1 does not intersect line1.
        /// </summary>
        /// <param name="min1">Line 1 min</param>
        /// <param name="max1">Line 1 max</param>
        /// <param name="min2">Line 2 min</param>
        /// <param name="max2">Line 2 max</param>
        /// <param name="correctMinMax">If mins and maxs might be reversed</param>
        /// <returns>a number to move along the projected axis (positive or negative) or null if no intersection</returns>
        public static float? IntersectMtv(float min1, float max1, float min2, float max2, bool correctMinMax = true)
        {
            if (correctMinMax)
            {
                float tmp1 = min1, tmp2 = max1;
                min1 = Math.Min(tmp1, tmp2);
                max1 = Math.Max(tmp1, tmp2);

                tmp1 = min2;
                tmp2 = max2;
                min2 = Math.Min(tmp1, tmp2);
                max2 = Math.Max(tmp1, tmp2);
            }

            if (min1 <= min2 && max1 > min2)
                return min2 - max1;

            if (min2 <= min1 && max2 > min1)
                return max2 - min1;

            return null;
        }

        /// <summary>
        ///     Determines if the line from (min, max) contains point
        /// </summary>
        /// <param name="min">Min of line</param>
        /// <param name="max">Max of line</param>
        /// <param name="point">Point to check</param>
        /// <param name="strict">If edges are excluded</param>
        /// <param name="correctMinMax">if true (default true) min and max will be swapped if in the wrong order</param>
        /// <returns>if line (min, max) contains point</returns>
        public static bool Contains(float min, float max, float point, bool strict, bool correctMinMax = true)
        {
            if (correctMinMax)
            {
                float tmp1 = min, tmp2 = max;
                min = Math.Min(tmp1, tmp2);
                max = Math.Max(tmp1, tmp2);
            }

            if (strict)
                return min < point && max > point;

            return min <= point && max >= point;
        }

        /// <summary>
        ///     Detrmines the shortest distance from the line to get to point. Returns
        ///     null if the point is on the line (not strict). Always returns a positive value.
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="line">Line.</param>
        /// <param name="point">Point.</param>
        public static float? MinDistance(AxisAlignedLine line, float point)
        {
            return MinDistance(line.Min, line.Max, point, false);
        }

        /// <summary>
        ///     Determines the shortest distance for line1 to go to touch line2. Returns
        ///     null if line1 and line 2 intersect (not strictly)
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="line1">Line1.</param>
        /// <param name="line">Line2.</param>
        public static float? MinDistance(AxisAlignedLine line1, AxisAlignedLine line)
        {
            return MinDistance(line1.Min, line1.Max, line.Min, line.Max, false);
        }

        /// <summary>
        ///     Determines the shortest distance from the line (min, max) to the point. Returns
        ///     null if the point is on the line (not strict). Always returns a positive value.
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="min">Minimum of line.</param>
        /// <param name="max">Maximum of line.</param>
        /// <param name="point">Point to check.</param>
        /// <param name="correctMinMax">If set to <c>true</c> will correct minimum max being reversed if they are</param>
        public static float? MinDistance(float min, float max, float point, bool correctMinMax = true)
        {
            if (correctMinMax)
            {
                float tmp1 = min, tmp2 = max;
                min = Math.Min(tmp1, tmp2);
                max = Math.Max(tmp1, tmp2);
            }

            if (point < min)
                return min - point;

            if (point > max)
                return point - max;

            return null;
        }

        /// <summary>
        ///     Calculates the shortest distance for line1 (min1, max1) to get to line2 (min2, max2).
        ///     Returns null if line1 and line2 intersect (not strictly)
        /// </summary>
        /// <returns>The distance along the mutual axis or null.</returns>
        /// <param name="min1">Min1.</param>
        /// <param name="max1">Max1.</param>
        /// <param name="min2">Min2.</param>
        /// <param name="max2">Max2.</param>
        /// <param name="correctMinMax">If set to <c>true</c> correct minimum max being potentially reversed.</param>
        public static float? MinDistance(float min1, float max1, float min2, float max2, bool correctMinMax = true)
        {
            if (correctMinMax)
            {
                float tmp1 = min1, tmp2 = max1;
                min1 = Math.Min(tmp1, tmp2);
                max1 = Math.Max(tmp1, tmp2);

                tmp1 = min2;
                tmp2 = max2;
                min2 = Math.Min(tmp1, tmp2);
                max2 = Math.Max(tmp1, tmp2);
            }

            if (min1 < min2)
            {
                if (max1 < min2)
                    return min2 - max1;

                return null;
            }

            if (min2 < min1)
            {
                if (max2 < min1)
                    return min1 - max2;

                return null;
            }

            return null;
        }

        #endregion


        #region BoundingBox

        /// <summary>
        ///     Determines if box1 with origin pos1 intersects box2 with origin pos2.
        /// </summary>
        /// <param name="box1">Box 1</param>
        /// <param name="box2">Box 2</param>
        /// <param name="pos1">Origin of box 1</param>
        /// <param name="pos2">Origin of box 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If box1 intersects box2 when box1 is at pos1 and box2 is at pos2</returns>
        public static bool Intersects(BoundingBox box1, BoundingBox box2, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(box1.Min.X + pos1.X, box1.Max.X + pos1.X, box2.Min.X + pos2.X, box2.Max.X + pos2.X, strict, false)
                   && Intersects(box1.Min.Y + pos1.Y, box1.Max.Y + pos1.Y, box2.Min.Y + pos2.Y, box2.Max.Y + pos2.Y, strict, false);
        }

        /// <summary>
        ///     Determines if the box when at pos contains point.
        /// </summary>
        /// <param name="box">The box</param>
        /// <param name="pos">Origin of box</param>
        /// <param name="point">Point to check</param>
        /// <param name="strict">true if the edges do not count</param>
        /// <returns>If the box at pos contains point</returns>
        public static bool Contains(BoundingBox box, Vector2 pos, Vector2 point, bool strict)
        {
            return Contains(box.Min.X + pos.X, box.Max.X + pos.X, point.X, strict, false)
                   && Contains(box.Min.Y + pos.Y, box.Max.Y + pos.Y, point.Y, strict, false);
        }

        /// <summary>
        ///     Projects the rectangle at pos along axis.
        /// </summary>
        /// <param name="rect">The rectangle to project</param>
        /// <param name="pos">The origin of the rectangle</param>
        /// <param name="axis">The axis to project on</param>
        /// <returns>The projection of rect at pos along axis</returns>
        public static AxisAlignedLine ProjectAlongAxis(BoundingBox rect, Vector2 pos, Vector2 axis)
        {
            return ProjectAlongAxis(axis, pos, Rotation.Zero, rect.Center, rect.Min, rect.UpperRight, rect.LowerLeft, rect.Max);
        }

        #endregion


        #region Circle

        /// <summary>
        ///     Determines if the circle at the specified position contains the point
        /// </summary>
        /// <param name="radius">The circle</param>
        /// <param name="pos">The top-left of the circles bounding box</param>
        /// <param name="point">The point to check if is in the circle at pos</param>
        /// <param name="strict">If the edges do not count</param>
        /// <returns>If the circle at pos contains point</returns>
        public static bool Contains(float radius, Vector2 pos, Vector2 point, bool strict)
        {
            var distSq = (point - new Vector2(pos.X + radius, pos.Y + radius)).LengthSquared();

            if (strict)
                return distSq < radius * radius;

            return distSq <= radius * radius;
        }


        /// <summary>
        ///     Determines if the first circle of specified radius and (bounding box top left) intersects
        ///     the second circle of specified radius and (bounding box top left)
        /// </summary>
        /// <param name="radius1">Radius of the first circle</param>
        /// <param name="radius2">Radius of the second circle</param>
        /// <param name="pos1">Top-left of the bounding box of the first circle</param>
        /// <param name="pos2">Top-left of the bounding box of the second circle</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If circle1 of radius=radius1, topleft=pos1 intersects circle2 of radius=radius2, topleft=pos2</returns>
        public static bool Intersects(float radius1, float radius2, Vector2 pos1, Vector2 pos2, bool strict)
        {
            var vecCenterToCenter = pos1 - pos2;
            vecCenterToCenter.X += radius1 - radius2;
            vecCenterToCenter.Y += radius1 - radius2;
            var distSq = vecCenterToCenter.LengthSquared();

            if (strict)
                return distSq < (radius1 + radius2) * (radius1 + radius2);

            return distSq <= (radius1 + radius2) * (radius1 + radius2);
        }


        /// <summary>
        ///     Determines the shortest axis and overlap for which the first circle, specified by its radius and its bounding
        ///     box's top-left, intersects the second circle specified by its radius and bounding box top-left. Returns null if
        ///     the circles do not overlap.
        /// </summary>
        /// <param name="radius1">Radius of the first circle</param>
        /// <param name="radius2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns>The direction and magnitude to move pos1 to prevent intersection</returns>
        public static Vector2 IntersectMtv(float radius1, float radius2, Vector2 pos1, Vector2 pos2)
        {
            var betweenVec = pos1 - pos2;
            betweenVec.X += radius1 - radius2;
            betweenVec.Y += radius1 - radius2;

            var lengthSq = betweenVec.LengthSquared();
            if (lengthSq < (radius1 + radius2) * (radius1 + radius2))
            {
                var len = Math.Sqrt(lengthSq);
                betweenVec *= (float)(1 / len);

                return betweenVec * (radius1 + radius2 - (float)len);
            }

            return Vector2.Zero;
        }


        /// <summary>
        ///     Projects a circle defined by its radius and the top-left of its bounding box along
        ///     the specified axis.
        /// </summary>
        /// <param name="radius">Radius of the circle to project</param>
        /// <param name="pos">Position of the circle</param>
        /// <param name="axis">Axis to project on</param>
        /// <returns></returns>
        public static AxisAlignedLine ProjectAlongAxis(float radius, Vector2 pos, Vector2 axis)
        {
            var centerProj = Vector2.Dot(new Vector2(pos.X + radius, pos.Y + radius), axis);
            return new AxisAlignedLine(axis, centerProj - radius, centerProj + radius);
        }

        #endregion


        #region Line

        /// <summary>
        ///     Determines if line1 intersects line2, when line1 is offset by pos1 and line2
        ///     is offset by pos2.
        /// </summary>
        /// <param name="line1">Line 1</param>
        /// <param name="line">Line 2</param>
        /// <param name="pos1">Origin of line 1</param>
        /// <param name="pos2">Origin of line 2</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If line1 intersects line2</returns>
        public static bool Intersects(Line line1, Line line, Vector2 pos1, Vector2 pos2, bool strict)
        {
            if (line1.Horizontal && line.Horizontal)
            {
                return Intersects(line1.MinX + pos1.X, line1.MaxX + pos1.X, line.MinX + pos2.X, line.MaxX + pos2.X, strict, false);
            }

            if (line1.Vertical && line.Vertical)
            {
                return Intersects(line1.MinY + pos1.Y, line1.MaxY + pos1.Y, line.MinY + pos2.Y, line.MaxY + pos2.Y, strict, false);
            }

            if (line1.Horizontal || line.Horizontal)
            {
                if (line.Horizontal)
                {
                    // swap line 1 and 2 to prevent duplicating everything
                    var tmp = line1;
                    var tmpp = pos1;
                    line1 = line;
                    pos1 = pos2;
                    line = tmp;
                    pos2 = tmpp;
                }

                if (line.Vertical)
                {
                    return Contains(line1.MinX + pos1.X, line1.MaxX + pos1.X, line.Start.X + pos2.X, strict, false)
                           && Contains(line.MinY + pos2.Y, line.MaxY + pos2.Y, line1.Start.Y + pos1.Y, strict, false);
                }

                // recalculate line2 y intercept
                // y = mx + b
                // b = y - mx
                var line2YIntInner = line.Start.Y + pos2.Y - line.Slope * (line.Start.X + pos2.X);
                // check line2.x at line1.y
                // line2.y = line2.slope * line2.x + line2.yintercept
                // line1.y = line2.slope * line2.x + line2.yintercept
                // line1.y - line2.yintercept = line2.slope * line2.x
                // (line1.y - line2.yintercept) / line2.slope = line2.x
                var line2XAtLine1Y = (line1.Start.Y + pos1.Y - line2YIntInner) / line.Slope;
                return Contains(line1.MinX + pos1.X, line1.MaxX + pos1.X, line2XAtLine1Y, strict, false)
                       && Contains(line.MinX + pos2.X, line.MaxX + pos2.X, line2XAtLine1Y, strict, false);
            }

            if (line1.Vertical)
            {
                // vertical line with regular line
                var line2YIntInner = line.Start.Y + pos2.Y - line.Slope * (line.Start.X + pos2.X);
                var line2YAtLine1X = line.Slope * (line1.Start.X + pos1.X) + line2YIntInner;
                return Contains(line1.MinY + pos1.Y, line1.MaxY + pos1.Y, line2YAtLine1X, strict, false)
                       && Contains(line.MinY + pos2.Y, line.MaxY + pos2.Y, line2YAtLine1X, strict, false);
            }

            // two non-vertical, non-horizontal lines
            var line1YInt = line1.Start.Y + pos1.Y - line1.Slope * (line1.Start.X + pos1.X);
            var line2YInt = line.Start.Y + pos2.Y - line.Slope * (line.Start.X + pos2.X);

            if (Math.Abs(line1.Slope - line.Slope) <= MathUtils.DefaultEpsilon)
            {
                // parallel lines
                if (line1YInt != line2YInt)
                    return false; // infinite lines don't intersect

                // parallel lines with equal y intercept. Intersect if ever at same X coordinate.
                return Intersects(line1.MinX + pos1.X, line1.MaxX + pos1.X, line.MinX + pos2.X, line.MaxX + pos2.X, strict, false);
            }
            // two non-parallel lines. Only one possible intersection point

            // y1 = y2
            // line1.Slope * x + line1.YIntercept = line2.Slope * x + line2.YIntercept
            // line1.Slope * x - line2.Slope * x = line2.YIntercept - line1.YIntercept
            // x (line1.Slope - line2.Slope) = line2.YIntercept - line1.YIntercept
            // x = (line2.YIntercept - line1.YIntercept) / (line1.Slope - line2.Slope)
            var x = (line2YInt - line1YInt) / (line1.Slope - line.Slope);

            return Contains(line1.MinX + pos1.X, line1.MaxX + pos1.X, x, strict, false)
                   && Contains(line.MinX + pos1.X, line.MaxX + pos2.X, x, strict, false);
        }

        #endregion


        #region Polygon

        /// <summary>
        ///     Determines if the specified polygon at the specified position and rotation contains the specified point
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="pos">Origin of the polygon</param>
        /// <param name="rot">Rotation of the polygon</param>
        /// <param name="point">Point to check</param>
        /// <param name="strict">True if the edges do not count as inside</param>
        /// <returns>If the polygon at pos with rotation rot about its center contains point</returns>
        public static bool Contains(Polygon poly, Vector2 pos, Rotation rot, Vector2 point, bool strict)
        {
            if (!Contains(poly.Aabb, pos, point, strict))
                return false;

            // Calculate the area of the triangles constructed by the lines of the polygon. If it
            // matches the area of the polygon, we're inside the polygon. 
            float myArea = 0;

            var center = poly.Center + pos;
            var last = MathUtils.Rotate(poly.Vertices[poly.Vertices.Length - 1], poly.Center, rot) + pos;
            for (var i = 0; i < poly.Vertices.Length; i++)
            {
                var curr = MathUtils.Rotate(poly.Vertices[i], poly.Center, rot) + pos;

                myArea += MathUtils.AreaOfTriangle(center, last, curr);

                last = curr;
            }

            return MathUtils.Approximately(myArea, poly.Area, poly.Area / 1000);
        }

        /// <summary>
        ///     Determines if the first polygon intersects the second polygon when they are at
        ///     the respective positions and rotations.
        /// </summary>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Position of the first polygon</param>
        /// <param name="pos2">Position of the second polygon</param>
        /// <param name="rot1">Rotation of the first polygon</param>
        /// <param name="rot2">Rotation fo the second polyogn</param>
        /// <param name="strict">If overlapping is required for intersection</param>
        /// <returns>If poly1 at pos1 with rotation rot1 intersects poly2 at pos2with rotation rot2</returns>
        public static bool Intersects(Polygon poly1, Polygon poly2, Vector2 pos1, Vector2 pos2, Rotation rot1, Rotation rot2, bool strict)
        {
            foreach (var norm in poly1.Normals.Select(v => Tuple.Create(v, rot1)).Union(poly2.Normals.Select(v => Tuple.Create(v, rot2))))
            {
                var axis = MathUtils.Rotate(norm.Item1, Vector2.Zero, norm.Item2);
                if (!IntersectsAlongAxis(poly1, poly2, pos1, pos2, rot1, rot2, strict, axis))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Determines the mtv to move pos1 by to prevent poly1 at pos1 from intersecting poly2 at pos2.
        ///     Returns null if poly1 and poly2 do not intersect.
        /// </summary>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Position of the first polygon</param>
        /// <param name="pos2">Position of the second polygon</param>
        /// <param name="rot1">Rotation of the first polyogn</param>
        /// <param name="rot2">Rotation of the second polygon</param>
        /// <returns>MTV to move poly1 to prevent intersection with poly2</returns>
        public static Vector2 IntersectMtv(Polygon poly1, Polygon poly2, Vector2 pos1, Vector2 pos2, Rotation rot1, Rotation rot2)
        {
            var bestAxis = Vector2.Zero;
            var bestMagn = float.MaxValue;

            foreach (var norm in poly1.Normals.Select(v => Tuple.Create(v, rot1)).Union(poly2.Normals.Select(v => Tuple.Create(v, rot2))))
            {
                var axis = MathUtils.Rotate(norm.Item1, Vector2.Zero, norm.Item2);
                var mtv = IntersectMtvAlongAxis(poly1, poly2, pos1, pos2, rot1, rot2, axis);
                if (!mtv.HasValue)
                    return Vector2.Zero;

                if (Math.Abs(mtv.Value) < Math.Abs(bestMagn))
                {
                    bestAxis = axis;
                    bestMagn = mtv.Value;
                }
            }

            return bestAxis * bestMagn;
        }

        /// <summary>
        ///     Determines if polygon 1 and polygon 2 at position 1 and position 2, respectively, intersect along axis.
        /// </summary>
        /// <param name="poly1">polygon 1</param>
        /// <param name="poly2">polygon 2</param>
        /// <param name="pos1">Origin of polygon 1</param>
        /// <param name="pos2">Origin of polygon 2</param>
        /// <param name="rot1">Rotation of the first polygon</param>
        /// <param name="rot2">Rotation of the second polygon</param>
        /// <param name="strict">If overlapping is required for intersection</param>
        /// <param name="axis">The axis to check</param>
        /// <returns>If poly1 at pos1 intersects poly2 at pos2 along axis</returns>
        public static bool IntersectsAlongAxis(Polygon poly1, Polygon poly2, Vector2 pos1, Vector2 pos2, Rotation rot1, Rotation rot2, bool strict, Vector2 axis)
        {
            var proj1 = ProjectAlongAxis(poly1, pos1, rot1, axis);
            var proj2 = ProjectAlongAxis(poly2, pos2, rot2, axis);

            return Intersects(proj1, proj2, strict);
        }

        /// <summary>
        ///     Determines the distance along axis, if any, that polygon 1 should be shifted by
        ///     to prevent intersection with polygon 2. Null if no intersection along axis.
        /// </summary>
        /// <param name="poly1">polygon 1</param>
        /// <param name="poly2">polygon 2</param>
        /// <param name="pos1">polygon 1 origin</param>
        /// <param name="pos2">polygon 2 origin</param>
        /// <param name="rot1">polygon 1 rotation</param>
        /// <param name="rot2">polygon 2 rotation</param>
        /// <param name="axis">Axis to check</param>
        /// <returns>
        ///     a number to shift pos1 along axis by to prevent poly1 at pos1 from intersecting poly2 at pos2, or null if no
        ///     int. along axis
        /// </returns>
        public static float? IntersectMtvAlongAxis(Polygon poly1, Polygon poly2, Vector2 pos1, Vector2 pos2, Rotation rot1, Rotation rot2, Vector2 axis)
        {
            var proj1 = ProjectAlongAxis(poly1, pos1, rot1, axis);
            var proj2 = ProjectAlongAxis(poly2, pos2, rot2, axis);

            return IntersectMtv(proj1, proj2);
        }

        /// <summary>
        ///     Projects the polygon at position onto the specified axis.
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="pos">The polygons origin</param>
        /// <param name="rot">the rotation of the polygon</param>
        /// <param name="axis">The axis to project onto</param>
        /// <returns>poly at pos projected along axis</returns>
        public static AxisAlignedLine ProjectAlongAxis(Polygon poly, Vector2 pos, Rotation rot, Vector2 axis)
        {
            return ProjectAlongAxis(axis, pos, rot, poly.Center, poly.Vertices);
        }

        /// <summary>
        ///     Calculates the shortest distance from the specified polygon to the specified point,
        ///     and the axis from polygon to pos.
        ///     Returns null if pt is contained in the polygon (not strictly).
        /// </summary>
        /// <returns>The distance form poly to pt.</returns>
        /// <param name="poly">The polygon</param>
        /// <param name="pos">Origin of the polygon</param>
        /// <param name="rot">Rotation of the polygon</param>
        /// <param name="pt">Point to check.</param>
        public static Vector2 MinDistance(Polygon poly, Vector2 pos, Rotation rot, Vector2 pt)
        {
            /*
             * Definitions
             * 
             * For each line in the polygon, find the normal of the line in the direction of outside the polygon.
             * Call the side of the original line that contains none of the polygon "above the line". The other side is "below the line".
             * 
             * If the point falls above the line:
             *   Imagine two additional lines that are normal to the line and fall on the start and end, respectively.
             *   For each of those two lines, call the side of the line that contains the original line "below the line". The other side is "above the line"
             *   
             *   If the point is above the line containing the start:
             *     The shortest vector is from the start to the point
             *   
             *   If the point is above the line containing the end:
             *     The shortest vector is from the end to the point
             *     
             *   Otherwise
             *     The shortest vector is from the line to the point
             * 
             * If this is not true for ANY of the lines, the polygon does not contain the point.
             */

            var last = MathUtils.Rotate(poly.Vertices[poly.Vertices.Length - 1], poly.Center, rot) + pos;
            for (var i = 0; i < poly.Vertices.Length; i++)
            {
                var curr = MathUtils.Rotate(poly.Vertices[i], poly.Center, rot) + pos;
                var axis = curr - last;

                var norm = poly.Clockwise ? new Vector2(-axis.Y, axis.X) : new Vector2(axis.Y, -axis.X);
                norm = Vector2.Normalize(norm);
                axis = Vector2.Normalize(axis);

                var lineProjOnNorm = Vector2.Dot(norm, last);
                var ptProjOnNorm = Vector2.Dot(norm, pt);

                if (ptProjOnNorm > lineProjOnNorm)
                {
                    var ptProjOnAxis = Vector2.Dot(axis, pt);
                    var stProjOnAxis = Vector2.Dot(axis, last);

                    if (ptProjOnAxis < stProjOnAxis)
                    {
                        var res = pt - last;
                        return Vector2.Normalize(res) * res.Length();
                    }

                    var enProjOnAxis = Vector2.Dot(axis, curr);
                    if (ptProjOnAxis > enProjOnAxis)
                    {
                        var res = pt - curr;
                        return Vector2.Normalize(res) * res.Length();
                    }

                    var distOnNorm = ptProjOnNorm - lineProjOnNorm;
                    return norm * distOnNorm;
                }

                last = curr;
            }

            return Vector2.Zero;
        }


        /// <summary>
        /// </summary>
        /// <param name="poly1"></param>
        /// <param name="poly2"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        private static IEnumerable<Vector2> GetExtraMinDistanceVecsPolyPoly(Polygon poly1, Polygon poly2, Vector2 pos1, Vector2 pos2)
        {
            foreach (var vert in poly1.Vertices)
            {
                foreach (var vert2 in poly2.Vertices)
                {
                    var roughAxis = vert2 + pos2 - (vert + pos1);
                    roughAxis.Normalize();
                    yield return MathUtils.MakeStandardNormal(roughAxis);
                }
            }
        }

        /// <summary>
        ///     Calculates the shortest distance and direction to go from poly1 at pos1 to poly2 at pos2. Returns null
        ///     if the polygons intersect.
        /// </summary>
        /// <returns>The distance.</returns>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Origin of first polygon</param>
        /// <param name="pos2">Origin of second polygon</param>
        /// <param name="rot1">Rotation of first polygon</param>
        /// <param name="rot2">Rotation of second polygon</param>
        public static Vector2 MinDistance(Polygon poly1, Polygon poly2, Vector2 pos1, Vector2 pos2, Rotation rot1, Rotation rot2)
        {
            if (rot1.Theta != 0 || rot2.Theta != 0)
                throw new NotSupportedException("Finding the minimum distance between polygons requires calculating the rotated polygons. This operation is expensive and should be cached. " +
                                                "Create the rotated polygons with Polygon2#GetRotated and call this function with Rotation2.Zero for both rotations.");

            var axises = poly1.Normals.Union(poly2.Normals).Union(GetExtraMinDistanceVecsPolyPoly(poly1, poly2, pos1, pos2));
            Vector2? bestAxis = null; // note this is the one with the longest distance
            float bestDist = 0;
            foreach (var norm in axises)
            {
                var proj1 = ProjectAlongAxis(poly1, pos1, rot1, norm);
                var proj2 = ProjectAlongAxis(poly2, pos2, rot2, norm);

                var dist = MinDistance(proj1, proj2);
                if (dist.HasValue && (bestAxis == null || dist.Value > bestDist))
                {
                    bestDist = dist.Value;
                    if (proj2.Min < proj1.Min && dist > 0)
                        bestAxis = -norm;
                    else
                        bestAxis = norm;
                }
            }

            if (!bestAxis.HasValue)
                return Vector2.Zero; // they intersect

            return bestAxis.Value * bestDist;
        }

        /// <summary>
        ///     Returns a polygon that is created by rotated the original polygon
        ///     about its center by the specified amount. Returns the original polygon if
        ///     rot.Theta == 0.
        /// </summary>
        /// <returns>The rotated polygon.</returns>
        /// <param name="original">Original.</param>
        /// <param name="rot">Rot.</param>
        public static Polygon GetRotated(Polygon original, Rotation rot)
        {
            if (rot.Theta == 0)
                return original;

            var rotatedVerts = new Vector2[original.Vertices.Length];
            for (var i = 0; i < original.Vertices.Length; i++)
                rotatedVerts[i] = MathUtils.Rotate(original.Vertices[i], original.Center, rot);

            return new Polygon(rotatedVerts);
        }


        #region NoRotation

        /// <summary>
        ///     Determines if the specified polygons intersect when at the specified positions and not rotated.
        /// </summary>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Origin of first polygon</param>
        /// <param name="pos2">Origin of second polygon</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If poly1 at pos1 not rotated and poly2 at pos2 not rotated intersect</returns>
        public static bool Intersects(Polygon poly1, Polygon poly2, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(poly1, poly2, pos1, pos2, Rotation.Zero, Rotation.Zero, strict);
        }

        /// <summary>
        ///     Determines if the first polygon at position 1 intersects the second polygon at position 2, where
        ///     neither polygon is rotated.
        /// </summary>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Origin of first polygon</param>
        /// <param name="pos2">Origin of second polygon</param>
        /// <returns>If poly1 at pos1 not rotated intersects poly2 at pos2 not rotated</returns>
        public static Vector2 IntersectMtv(Polygon poly1, Polygon poly2, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMtv(poly1, poly2, pos1, pos2, Rotation.Zero, Rotation.Zero);
        }

        /// <summary>
        ///     Determines the shortest way for the specified polygon at the specified position with
        ///     no rotation to get to the specified point, if point is not (non-strictly) intersected
        ///     the polygon when it's at the specified position with no rotation.
        /// </summary>
        /// <param name="poly">Polygon</param>
        /// <param name="pos">Position of the polygon</param>
        /// <param name="pt">Point to check</param>
        /// <returns>axis to go in, distance to go if pos is not in poly, otherwise null</returns>
        public static Vector2 MinDistance(Polygon poly, Vector2 pos, Vector2 pt)
        {
            return MinDistance(poly, pos, Rotation.Zero, pt);
        }

        /// <summary>
        ///     Determines the shortest way for the first polygon at position 1 to touch the second polygon at
        ///     position 2, assuming the polygons do not intersect (not strictly) and are not rotated.
        /// </summary>
        /// <param name="poly1">First polygon</param>
        /// <param name="poly2">Second polygon</param>
        /// <param name="pos1">Position of first polygon</param>
        /// <param name="pos2">Position of second polygon</param>
        /// <returns>axis to go in, distance to go if poly1 does not intersect poly2, otherwise null</returns>
        public static Vector2 MinDistance(Polygon poly1, Polygon poly2, Vector2 pos1, Vector2 pos2)
        {
            return MinDistance(poly1, poly2, pos1, pos2, Rotation.Zero, Rotation.Zero);
        }

        #endregion

        #endregion


        #region Shape

        /// <summary>
        ///     Determines if polygon at position 1 intersects the rectangle at position 2. Polygon may
        ///     be rotated, but the rectangle cannot (use a polygon if you want to rotate it).
        /// </summary>
        /// <param name="poly">Polygon</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="pos1">Origin of polygon</param>
        /// <param name="pos2">Origin of rectangle</param>
        /// <param name="rot1">Rotation of the polygon.</param>
        /// <param name="strict">If overlapping is required for intersection</param>
        /// <returns>if poly at pos1 intersects rect at pos2</returns>
        public static bool Intersects(Polygon poly, BoundingBox rect, Vector2 pos1, Vector2 pos2, Rotation rot1, bool strict)
        {
            bool checkedX = false, checkedY = false;
            for (var i = 0; i < poly.Normals.Count; i++)
            {
                var norm = MathUtils.Rotate(poly.Normals[i], Vector2.Zero, rot1);
                if (!IntersectsAlongAxis(poly, rect, pos1, pos2, rot1, strict, norm))
                    return false;

                if (norm.X == 0)
                    checkedY = true;

                if (norm.Y == 0)
                    checkedX = true;
            }

            if (!checkedX && !IntersectsAlongAxis(poly, rect, pos1, pos2, rot1, strict, Vector2.UnitX))
                return false;

            if (!checkedY && !IntersectsAlongAxis(poly, rect, pos1, pos2, rot1, strict, Vector2.UnitY))
                return false;

            return true;
        }

        /// <summary>
        ///     Determines the vector, if any, to move poly at pos1 rotated rot1 to prevent intersection of rect
        ///     at pos2.
        /// </summary>
        /// <param name="poly">Polygon</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="pos1">Origin of polygon</param>
        /// <param name="pos2">Origin of rectangle</param>
        /// <param name="rot1">Rotation of the polygon.</param>
        /// <returns>The vector to move pos1 by or null</returns>
        public static Vector2 IntersectMtv(Polygon poly, BoundingBox rect, Vector2 pos1, Vector2 pos2, Rotation rot1)
        {
            bool checkedX = false, checkedY = false;

            var bestAxis = Vector2.Zero;
            var bestMagn = float.MaxValue;

            for (var i = 0; i < poly.Normals.Count; i++)
            {
                var norm = MathUtils.Rotate(poly.Normals[i], Vector2.Zero, rot1);
                var mtv = IntersectMtvAlongAxis(poly, rect, pos1, pos2, rot1, norm);
                if (!mtv.HasValue)
                    return Vector2.Zero;

                if (Math.Abs(mtv.Value) < Math.Abs(bestMagn))
                {
                    bestAxis = norm;
                    bestMagn = mtv.Value;
                }

                if (norm.X == 0)
                    checkedY = true;

                if (norm.Y == 0)
                    checkedX = true;
            }

            if (!checkedX)
            {
                var mtv = IntersectMtvAlongAxis(poly, rect, pos1, pos2, rot1, Vector2.UnitX);
                if (!mtv.HasValue)
                    return Vector2.Zero;

                if (Math.Abs(mtv.Value) < Math.Abs(bestMagn))
                {
                    bestAxis = Vector2.UnitX;
                    bestMagn = mtv.Value;
                }
            }

            if (!checkedY)
            {
                var mtv = IntersectMtvAlongAxis(poly, rect, pos1, pos2, rot1, Vector2.UnitY);
                if (!mtv.HasValue)
                    return Vector2.Zero;

                if (Math.Abs(mtv.Value) < Math.Abs(bestMagn))
                {
                    bestAxis = Vector2.UnitY;
                    bestMagn = mtv.Value;
                }
            }

            return bestAxis * bestMagn;
        }

        /// <summary>
        ///     Determines the vector to move pos1 to get rect not to intersect poly at pos2 rotated
        ///     by rot2 radians.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">Origin of rectangle</param>
        /// <param name="pos2">Origin of </param>
        /// <param name="rot2">Rotation of the polygon</param>
        /// <returns>Offset of pos1 to get rect not to intersect poly</returns>
        public static Vector2 IntersectMtv(BoundingBox rect, Polygon poly, Vector2 pos1, Vector2 pos2, Rotation rot2)
        {
            var res = IntersectMtv(poly, rect, pos2, pos1, rot2);
            return -res;
        }

        /// <summary>
        ///     Determines if the rectangle at pos1 intersects the polygon at pos2.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">Origin of retangle</param>
        /// <param name="pos2">Origin of polygon</param>
        /// <param name="rot2">Rotation of the polygon.</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If rect at pos1 intersects poly at pos2</returns>
        public static bool Intersects(BoundingBox rect, Polygon poly, Vector2 pos1, Vector2 pos2, Rotation rot2, bool strict)
        {
            return Intersects(poly, rect, pos2, pos1, rot2, strict);
        }


        /// <summary>
        ///     Determines if the specified polygon and rectangle where poly is at pos1 and rect is at pos2 intersect
        ///     along the specified axis.
        /// </summary>
        /// <param name="poly">polygon</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="pos1">Origin of polygon</param>
        /// <param name="pos2">Origin of rectangle</param>
        /// <param name="rot1">Rotation of the polygon.</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <param name="axis">Axis to check</param>
        /// <returns>If poly at pos1 intersects rect at pos2 along axis</returns>
        public static bool IntersectsAlongAxis(Polygon poly, BoundingBox rect, Vector2 pos1, Vector2 pos2, Rotation rot1, bool strict, Vector2 axis)
        {
            var proj1 = ProjectAlongAxis(poly, pos1, rot1, axis);
            var proj2 = ProjectAlongAxis(rect, pos2, axis);

            return Intersects(proj1, proj2, strict);
        }

        /// <summary>
        ///     Determines if the specified rectangle and polygon where rect is at pos1 and poly is at pos2 intersect
        ///     along the specified axis.
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="poly">Polygon</param>
        /// <param name="pos1">Origin of rectangle</param>
        /// <param name="pos2">Origin of polygon</param>
        /// <param name="rot2">Rotation of polygon</param>
        /// <param name="strict"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static bool IntersectsAlongAxis(BoundingBox rect, Polygon poly, Vector2 pos1, Vector2 pos2, Rotation rot2, bool strict, Vector2 axis)
        {
            return IntersectsAlongAxis(poly, rect, pos2, pos1, rot2, strict, axis);
        }

        /// <summary>
        ///     Determines the mtv along axis to move poly at pos1 to prevent intersection with rect at pos2.
        /// </summary>
        /// <param name="poly">polygon</param>
        /// <param name="rect">Rectangle</param>
        /// <param name="pos1">Origin of polygon</param>
        /// <param name="pos2">Origin of rectangle</param>
        /// <param name="rot1">Rotation of polygon in radians</param>
        /// <param name="axis">Axis to check</param>
        /// <returns>Number if poly intersects rect along axis, null otherwise</returns>
        public static float? IntersectMtvAlongAxis(Polygon poly, BoundingBox rect, Vector2 pos1, Vector2 pos2, Rotation rot1, Vector2 axis)
        {
            var proj1 = ProjectAlongAxis(poly, pos1, rot1, axis);
            var proj2 = ProjectAlongAxis(rect, pos2, axis);

            return IntersectMtv(proj1, proj2);
        }

        /// <summary>
        ///     Determines the mtv along axis to move rect at pos1 to prevent intersection with poly at pos2
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="poly">polygon</param>
        /// <param name="pos1">Origin of rectangle</param>
        /// <param name="pos2">Origin of polygon</param>
        /// <param name="rot2">Rotation of the polygon in radians</param>
        /// <param name="axis">Axis to check</param>
        /// <returns>Number if rect intersects poly along axis, null otherwise</returns>
        public static float? IntersectMtvAlongAxis(BoundingBox rect, Polygon poly, Vector2 pos1, Vector2 pos2, Rotation rot2, Vector2 axis)
        {
            var proj1 = ProjectAlongAxis(rect, pos1, axis);
            var proj2 = ProjectAlongAxis(poly, pos2, rot2, axis);

            return IntersectMtv(proj1, proj2);
        }

        /// <summary>
        ///     Determines if the specified polygon at the specified position and rotation
        ///     intersects the specified circle at it's respective position.
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="radius">The circle</param>
        /// <param name="pos1">The origin for the polygon</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <param name="rot1">The rotation of the polygon</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If poly at pos1 with rotation rot1 intersects the circle at pos2</returns>
        public static bool Intersects(Polygon poly, float radius, Vector2 pos1, Vector2 pos2, Rotation rot1, bool strict)
        {
            // look at pictures of https://stackoverflow.com/questions/401847/circle-rectangle-collision-detection-intersection if you don't
            // believe this is true
            var intersectsLine = false;
            for (var i = 0; i < poly.Lines.Length; i++)
            {
                var line = poly.Lines[i];
                intersectsLine = CircleIntersectsLine(radius, line, pos2, pos1, rot1, poly.Center, strict);
                if (intersectsLine)
                    break;
            }

            return intersectsLine || Contains(poly, pos1, rot1, new Vector2(pos2.X + radius, pos2.Y + radius), strict);
        }


        /// <summary>
        ///     Determines the minimum translation that must be applied the specified polygon (at the given position
        ///     and rotation) to prevent intersection with the circle (at its given rotation). If the two are not overlapping,
        ///     returns null.
        ///     Returns a tuple of the axis to move the polygon in (unit vector) and the distance to move the polygon.
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="radius">The circle</param>
        /// <param name="pos1">The origin of the polygon</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <param name="rot1">The rotation of the polygon</param>
        /// <returns></returns>
        public static Vector2 IntersectMtv(Polygon poly, float radius, Vector2 pos1, Vector2 pos2, Rotation rot1)
        {
            // We have two situations, either the circle is not strictly intersecting the polygon, or
            // there exists at least one shortest line that you could push the polygon to prevent 
            // intersection with the circle.

            // That line will either go from a vertix of the polygon to a point on the edge of the circle,
            // or it will go from a point on a line of the polygon to the edge of the circle.

            // If the line comes from a vertix of the polygon, the MTV will be along the line produced
            // by going from the center of the circle to the vertix, and the distance can be found by
            // projecting the cirle on that axis and the polygon on that axis and doing 1D overlap.

            // If the line comes from a point on the edge of the polygon, the MTV will be along the
            // normal of that line, and the distance can be found by projecting the circle on that axis
            // and the polygon on that axis and doing 1D overlap.

            // As with all SAT, if we find any axis that the circle and polygon do not overlap, we've
            // proven they do not intersect.

            // The worst case performance is related to 2x the number of vertices of the polygon, the same speed
            // as for 2 polygons of equal number of vertices.

            var checkedAxis = new HashSet<Vector2>();

            var bestAxis = Vector2.Zero;
            var shortestOverlap = float.MaxValue;

            bool CheckAxis(Vector2 axis)
            {
                var standard = MathUtils.MakeStandardNormal(axis);
                if (!checkedAxis.Contains(standard))
                {
                    checkedAxis.Add(standard);
                    var polyProj = ProjectAlongAxis(poly, pos1, rot1, axis);
                    var circleProj = ProjectAlongAxis(radius, pos2, axis);

                    var mtv = IntersectMtv(polyProj, circleProj);
                    if (!mtv.HasValue)
                        return false;

                    if (Math.Abs(mtv.Value) < Math.Abs(shortestOverlap))
                    {
                        bestAxis = axis;
                        shortestOverlap = mtv.Value;
                    }
                }

                return true;
            }

            var circleCenter = new Vector2(pos2.X + radius, pos2.Y + radius);
            var lastVec = MathUtils.Rotate(poly.Vertices[poly.Vertices.Length - 1], poly.Center, rot1) + pos1;
            for (var curr = 0; curr < poly.Vertices.Length; curr++)
            {
                var currVec = MathUtils.Rotate(poly.Vertices[curr], poly.Center, rot1) + pos1;

                // Test along circle center -> vector
                if (!CheckAxis(Vector2.Normalize(currVec - circleCenter)))
                    return Vector2.Zero;

                // Test along line normal
                if (!CheckAxis(Vector2.Normalize(MathUtils.Perpendicular(currVec - lastVec))))
                    return Vector2.Zero;

                lastVec = currVec;
            }

            return bestAxis * shortestOverlap;
        }

        /// <summary>
        ///     Determines if the specified circle, at the given position, intersects the specified polygon,
        ///     at the given position and rotation.
        /// </summary>
        /// <param name="radius">The circle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the polygon</param>
        /// <param name="rot2">The rotation of the polygon</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If circle at pos1 intersects poly at pos2 with rotation rot2</returns>
        public static bool Intersects(float radius, Polygon poly, Vector2 pos1, Vector2 pos2, Rotation rot2, bool strict)
        {
            return Intersects(poly, radius, pos2, pos1, rot2, strict);
        }

        /// <summary>
        ///     Determines the minimum translation vector that must be applied to the circle at the given position to
        ///     prevent overlap with the polygon at the given position and rotation. If the circle and the polygon do
        ///     not overlap, returns null. Otherwise, returns a tuple of the unit axis to move the circle in, and the
        ///     distance to move the circle.
        /// </summary>
        /// <param name="radius">The circle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the polygon</param>
        /// <param name="rot2">The rotation of the polygon</param>
        /// <returns>The mtv to move the circle at pos1 to prevent overlap with the poly at pos2 with rotation rot2</returns>
        public static Vector2 IntersectMtv(float radius, Polygon poly, Vector2 pos1, Vector2 pos2, Rotation rot2)
        {
            var res = IntersectMtv(poly, radius, pos2, pos1, rot2);
            return -res;
        }

        /// <summary>
        ///     Determines if the specified circle an rectangle intersect at their given positions.
        /// </summary>
        /// <param name="radius">The circle</param>
        /// <param name="rect">The rectangle</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the rectangle</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If circle at pos1 intersects rect at pos2</returns>
        public static bool Intersects(float radius, BoundingBox rect, Vector2 pos1, Vector2 pos2, bool strict)
        {
            var circleCenter = new Vector2(pos1.X + radius, pos1.Y + radius);
            return CircleIntersectsHorizontalLine(radius, new Line(rect.Min + pos2, rect.UpperRight + pos2), circleCenter, strict)
                   || CircleIntersectsHorizontalLine(radius, new Line(rect.LowerLeft + pos2, rect.Max + pos2), circleCenter, strict)
                   || CircleIntersectsVerticalLine(radius, new Line(rect.Min + pos2, rect.LowerLeft + pos2), circleCenter, strict)
                   || CircleIntersectsVerticalLine(radius, new Line(rect.UpperRight + pos2, rect.Max + pos2), circleCenter, strict)
                   || Contains(rect, pos2, new Vector2(pos1.X + radius, pos1.Y + radius), strict);
        }

        /// <summary>
        ///     Determines if the specified rectangle and circle intersect at their given positions.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="radius">The circle</param>
        /// <param name="pos1">The origin of the rectangle</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns></returns>
        public static bool Intersects(BoundingBox rect, float radius, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(radius, rect, pos2, pos1, strict);
        }

        /// <summary>
        ///     Determines the minimum translation vector to be applied to the circle to
        ///     prevent overlap with the rectangle, when they are at their given positions.
        /// </summary>
        /// <param name="radius">The circle</param>
        /// <param name="rect">The rectangle</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The rectangles origin</param>
        /// <returns>MTV for circle at pos1 to prevent overlap with rect at pos2</returns>
        public static Vector2 IntersectMtv(float radius, BoundingBox rect, Vector2 pos1, Vector2 pos2)
        {
            // Same as polygon rect, just converted to rects points
            var checkedAxis = new HashSet<Vector2>();

            var bestAxis = Vector2.Zero;
            var shortestOverlap = float.MaxValue;

            bool CheckAxis(Vector2 axis)
            {
                var standard = MathUtils.MakeStandardNormal(axis);
                if (!checkedAxis.Contains(standard))
                {
                    checkedAxis.Add(standard);
                    var circleProj = ProjectAlongAxis(radius, pos1, axis);
                    var rectProj = ProjectAlongAxis(rect, pos2, axis);

                    var mtv = IntersectMtv(circleProj, rectProj);
                    if (!mtv.HasValue)
                        return false;

                    if (Math.Abs(mtv.Value) < Math.Abs(shortestOverlap))
                    {
                        bestAxis = axis;
                        shortestOverlap = mtv.Value;
                    }
                }

                return true;
            }

            var circleCenter = new Vector2(pos1.X + radius, pos1.Y + radius);
            var lastVec = rect.UpperRight + pos2;
            for (var curr = 0; curr < 4; curr++)
            {
                var currVec = Vector2.Zero;
                switch (curr)
                {
                    case 0:
                        currVec = rect.Min + pos2;
                        break;
                    case 1:
                        currVec = rect.LowerLeft + pos2;
                        break;
                    case 2:
                        currVec = rect.Max + pos2;
                        break;
                    case 3:
                        currVec = rect.UpperRight + pos2;
                        break;
                }

                // Test along circle center -> vector
                if (!CheckAxis(Vector2.Normalize(currVec - circleCenter)))
                    return Vector2.Zero;

                // Test along line normal
                if (!CheckAxis(Vector2.Normalize(MathUtils.Perpendicular(currVec - lastVec))))
                    return Vector2.Zero;

                lastVec = currVec;
            }

            return bestAxis * shortestOverlap;
        }

        /// <summary>
        ///     Determines the minimum translation vector to be applied to the rectangle to
        ///     prevent overlap with the circle, when they are at their given positions.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="radius">The circle</param>
        /// <param name="pos1">The origin of the rectangle</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <returns>MTV for rect at pos1 to prevent overlap with circle at pos2</returns>
        public static Vector2 IntersectMtv(BoundingBox rect, float radius, Vector2 pos1, Vector2 pos2)
        {
            var res = IntersectMtv(radius, rect, pos2, pos1);
            return -res;
        }

        /// <summary>
        ///     Projects the polygon from the given points with origin pos along the specified axis.
        /// </summary>
        /// <param name="axis">Axis to project onto</param>
        /// <param name="pos">Origin of polygon</param>
        /// <param name="rot">Rotation of the polygon in radians</param>
        /// <param name="center">Center of the polygon</param>
        /// <param name="points">Points of polygon</param>
        /// <returns>Projection of polygon of points at pos along axis</returns>
        private static AxisAlignedLine ProjectAlongAxis(Vector2 axis, Vector2 pos, Rotation rot, Vector2 center, params Vector2[] points)
        {
            float min = 0;
            float max = 0;

            for (var i = 0; i < points.Length; i++)
            {
                var polyPt = MathUtils.Rotate(points[i], center, rot);
                var tmp = MathUtils.Dot(polyPt.X + pos.X, polyPt.Y + pos.Y, axis.X, axis.Y);

                if (i == 0)
                {
                    min = max = tmp;
                }
                else
                {
                    min = Math.Min(min, tmp);
                    max = Math.Max(max, tmp);
                }
            }

            return new AxisAlignedLine(axis, min, max);
        }

        /// <summary>
        ///     Determines if the circle whose bounding boxs top left is at the first postion intersects the line
        ///     at the second position who is rotated the specified amount about the specified point.
        /// </summary>
        /// <param name="radius">The circle</param>
        /// <param name="line">The line</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the line</param>
        /// <param name="rot2">What rotation the line is under</param>
        /// <param name="about2">What the line is rotated about</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If the circle at pos1 intersects the line at pos2 rotated rot2 about about2</returns>
        private static bool CircleIntersectsLine(float radius, Line line, Vector2 pos1, Vector2 pos2, Rotation rot2, Vector2 about2, bool strict)
        {
            // Make more math friendly
            var actualLine = new Line(MathUtils.Rotate(line.Start, about2, rot2) + pos2, MathUtils.Rotate(line.End, about2, rot2) + pos2);
            var circleCenter = new Vector2(pos1.X + radius, pos1.Y + radius);

            // Check weird situations
            if (actualLine.Horizontal)
                return CircleIntersectsHorizontalLine(radius, actualLine, circleCenter, strict);
            if (actualLine.Vertical)
                return CircleIntersectsVerticalLine(radius, actualLine, circleCenter, strict);

            // Goal:
            // 1. Find closest distance, closestDistance, on the line to the circle (assuming the line was infinite)
            //   1a Determine if closestPoint is intersects the circle according to strict
            //    - If it does not, we've shown there is no intersection.
            // 2. Find closest point, closestPoint, on the line to the circle (assuming the line was infinite)
            // 3. Determine if closestPoint is on the line (including edges)
            //   - If it is, we've shown there is intersection.
            // 4. Determine which edge, edgeClosest, is closest to closestPoint
            // 5. Determine if edgeClosest intersects the circle according to strict
            //   - If it does, we've shown there is intersection
            //   - If it does not, we've shown there is no intersection

            // Step 1
            // We're trying to find closestDistance

            // Recall that the shortest line from a line to a point will be normal to the line
            // Thus, the shortest distance from a line to a point can be found by projecting 
            // the line onto it's own normal vector and projecting the point onto the lines 
            // normal vector; the distance between those points is the shortest distance from
            // the two points. 

            // The projection of a line onto its normal will be a single point, and will be same
            // for any point on that line. So we pick a point that's convienent (the start or end).
            var lineProjectedOntoItsNormal = Vector2.Dot(actualLine.Start, actualLine.Normal);
            var centerOfCircleProjectedOntoNormalOfLine = Vector2.Dot(circleCenter, actualLine.Normal);
            var closestDistance = Math.Abs(centerOfCircleProjectedOntoNormalOfLine - lineProjectedOntoItsNormal);

            // Step 1a
            if (strict)
            {
                if (closestDistance >= radius)
                    return false;
            }
            else
            {
                if (closestDistance > radius)
                    return false;
            }

            // Step 2
            // We're trying to find closestPoint

            // We can just walk the vector from the center to the closest point, which we know is on 
            // the normal axis and the distance closestDistance. However it's helpful to get the signed
            // version End - Start to walk.
            var signedDistanceCircleCenterToLine = lineProjectedOntoItsNormal - centerOfCircleProjectedOntoNormalOfLine;
            var closestPoint = circleCenter - actualLine.Normal * signedDistanceCircleCenterToLine;

            // Step 3
            // Determine if closestPoint is on the line (including edges)

            // We're going to accomplish this by projecting the line onto it's own axis and the closestPoint onto the lines
            // axis. Then we have a 1D comparison.
            var lineStartProjectedOntoLineAxis = Vector2.Dot(actualLine.Start, actualLine.Axis);
            var lineEndProjectedOntoLineAxis = Vector2.Dot(actualLine.End, actualLine.Axis);

            var closestPointProjectedOntoLineAxis = Vector2.Dot(closestPoint, actualLine.Axis);

            if (Contains(lineStartProjectedOntoLineAxis, lineEndProjectedOntoLineAxis, closestPointProjectedOntoLineAxis, false))
            {
                return true;
            }

            // Step 4
            // We're trying to find edgeClosest.
            //
            // We're going to reuse those projections from step 3.
            //
            // (for each "point" in the next paragraph I mean "point projected on the lines axis" but that's wordy)
            //
            // We know that the start is closest iff EITHER the start is less than the end and the 
            // closest point is less than the start, OR the start is greater than the end and 
            // closest point is greater than the end.

            Vector2 closestEdge;
            if (lineStartProjectedOntoLineAxis < lineEndProjectedOntoLineAxis)
                closestEdge = closestPointProjectedOntoLineAxis <= lineStartProjectedOntoLineAxis ? actualLine.Start : actualLine.End;
            else
                closestEdge = closestPointProjectedOntoLineAxis >= lineEndProjectedOntoLineAxis ? actualLine.Start : actualLine.End;

            // Step 5 
            // Circle->Point intersection for closestEdge

            var distToCircleFromClosestEdgeSq = (circleCenter - closestEdge).LengthSquared();
            if (strict)
                return distToCircleFromClosestEdgeSq < radius * radius;
            return distToCircleFromClosestEdgeSq <= radius * radius;

            // If you had trouble following, see the horizontal and vertical cases which are the same process but the projections
            // are simpler
        }

        /// <summary>
        ///     Determines if the circle at the specified position intersects the line,
        ///     which is at its true position and rotation, when the line is assumed to be horizontal.
        /// </summary>
        /// <param name="radius">The circle</param>
        /// <param name="line">The line</param>
        /// <param name="circleCenter">The center of the circle</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If the circle with center circleCenter intersects the horizontal line</returns>
        private static bool CircleIntersectsHorizontalLine(float radius, Line line, Vector2 circleCenter, bool strict)
        {
            // This is exactly the same process as CircleIntersectsLine, except the projetions are easier
            var lineY = line.Start.Y;

            // Step 1 - Find closest distance
            var vecCircleCenterToLine1D = lineY - circleCenter.Y;
            var closestDistance = Math.Abs(vecCircleCenterToLine1D);

            // Step 1a
            if (strict)
            {
                if (closestDistance >= radius)
                    return false;
            }
            else
            {
                if (closestDistance > radius)
                    return false;
            }

            // Step 2 - Find closest point
            var closestPointX = circleCenter.X;

            // Step 3 - Is closest point on line
            if (Contains(line.Start.X, line.End.X, closestPointX, false))
                return true;

            // Step 4 - Find edgeClosest
            float edgeClosestX;
            if (line.Start.X < line.End.X)
                edgeClosestX = closestPointX <= line.Start.X ? line.Start.X : line.End.X;
            else
                edgeClosestX = closestPointX >= line.Start.X ? line.Start.X : line.End.X;

            // Step 5 - Circle-point intersection on closest point
            var distClosestEdgeToCircleSq = new Vector2(circleCenter.X - edgeClosestX, circleCenter.Y - lineY).LengthSquared();

            if (strict)
                return distClosestEdgeToCircleSq < radius * radius;

            return distClosestEdgeToCircleSq <= radius * radius;
        }

        /// <summary>
        ///     Determines if the circle at the specified position intersects the line, which
        ///     is at its true position and rotation, when the line is assumed to be vertical
        /// </summary>
        /// <param name="radius">The circle</param>
        /// <param name="line">The line</param>
        /// <param name="circleCenter">The center of the circle</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If the circle with center circleCenter intersects the line</returns>
        private static bool CircleIntersectsVerticalLine(float radius, Line line, Vector2 circleCenter, bool strict)
        {
            // Same process as horizontal, but axis flipped
            var lineX = line.Start.X;
            // Step 1 - Find closest distance
            var vecCircleCenterToLine1D = lineX - circleCenter.X;
            var closestDistance = Math.Abs(vecCircleCenterToLine1D);

            // Step 1a
            if (strict)
            {
                if (closestDistance >= radius)
                    return false;
            }
            else
            {
                if (closestDistance > radius)
                    return false;
            }

            // Step 2 - Find closest point
            var closestPointY = circleCenter.Y;

            // Step 3 - Is closest point on line
            if (Contains(line.Start.Y, line.End.Y, closestPointY, false))
                return true;

            // Step 4 - Find edgeClosest
            float edgeClosestY;
            if (line.Start.Y < line.End.Y)
                edgeClosestY = closestPointY <= line.Start.Y ? line.Start.Y : line.End.Y;
            else
                edgeClosestY = closestPointY >= line.Start.Y ? line.Start.Y : line.End.Y;

            // Step 5 - Circle-point intersection on closest point
            var distClosestEdgeToCircleSq = new Vector2(circleCenter.X - lineX, circleCenter.Y - edgeClosestY).LengthSquared();

            if (strict)
                return distClosestEdgeToCircleSq < radius * radius;

            return distClosestEdgeToCircleSq <= radius * radius;
        }

        #region NoRotation

        /// <summary>
        ///     Determines if the specified polygon at pos1 with no rotation and rectangle at pos2 intersect
        /// </summary>
        /// <param name="poly">Polygon to check</param>
        /// <param name="rect">Rectangle to check</param>
        /// <param name="pos1">Origin of polygon</param>
        /// <param name="pos2">Origin of rect</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If poly at pos1 intersects rect at pos2</returns>
        public static bool Intersects(Polygon poly, BoundingBox rect, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(poly, rect, pos1, pos2, Rotation.Zero, strict);
        }

        /// <summary>
        ///     Determines if the specified rectangle at pos1 intersects the specified polygon at pos2 with
        ///     no rotation.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">Origin of rectangle</param>
        /// <param name="pos2">Origin of polygon</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If rect at pos1 no rotation intersects poly at pos2</returns>
        public static bool Intersects(BoundingBox rect, Polygon poly, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(rect, poly, pos1, pos2, Rotation.Zero, strict);
        }

        /// <summary>
        ///     Determines if the specified polygon at pos1 with no rotation intersects the specified
        /// </summary>
        /// <param name="poly"></param>
        /// <param name="rect"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static Vector2 IntersectMtv(Polygon poly, BoundingBox rect, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMtv(poly, rect, pos1, pos2, Rotation.Zero);
        }

        /// <summary>
        ///     Determines the minimum translation vector to be applied to the rect to prevent
        ///     intersection with the specified polygon, when they are at the given positions.
        /// </summary>
        /// <param name="rect">The rect</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">The origin of the rect</param>
        /// <param name="pos2">The origin of the polygon</param>
        /// <returns>MTV to move rect at pos1 to prevent overlap with poly at pos2</returns>
        public static Vector2 IntersectMtv(BoundingBox rect, Polygon poly, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMtv(rect, poly, pos1, pos2, Rotation.Zero);
        }

        /// <summary>
        ///     Determines if the polygon and circle intersect when at the given positions.
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="radius">The circle</param>
        /// <param name="pos1">The origin of the polygon</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If poly at pos1 intersects circle at pos2</returns>
        public static bool Intersects(Polygon poly, float radius, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(poly, radius, pos1, pos2, Rotation.Zero, strict);
        }

        /// <summary>
        ///     Determines if the circle and polygon intersect when at the given positions.
        /// </summary>
        /// <param name="radius">The circle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the polygon</param>
        /// <param name="strict">If overlap is required for intersection</param>
        /// <returns>If circle at pos1 intersects poly at pos2</returns>
        public static bool Intersects(float radius, Polygon poly, Vector2 pos1, Vector2 pos2, bool strict)
        {
            return Intersects(radius, poly, pos1, pos2, Rotation.Zero, strict);
        }

        /// <summary>
        ///     Determines the minimum translation vector the be applied to the polygon to prevent
        ///     intersection with the specified circle, when they are at the given positions.
        /// </summary>
        /// <param name="poly">The polygon</param>
        /// <param name="radius">The circle</param>
        /// <param name="pos1">The position of the polygon</param>
        /// <param name="pos2">The top-left of the circles bounding box</param>
        /// <returns>MTV to move poly at pos1 to prevent overlap with circle at pos2</returns>
        public static Vector2 IntersectMtv(Polygon poly, float radius, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMtv(poly, radius, pos1, pos2, Rotation.Zero);
        }

        /// <summary>
        ///     Determines the minimum translation vector to be applied to the circle to prevent
        ///     intersection with the specified polyogn, when they are at the given positions.
        /// </summary>
        /// <param name="radius">The circle</param>
        /// <param name="poly">The polygon</param>
        /// <param name="pos1">The top-left of the circles bounding box</param>
        /// <param name="pos2">The origin of the polygon</param>
        /// <returns></returns>
        public static Vector2 IntersectMtv(float radius, Polygon poly, Vector2 pos1, Vector2 pos2)
        {
            return IntersectMtv(radius, poly, pos1, pos2, Rotation.Zero);
        }

        #endregion

        #endregion
    }
}