#region

using System.Collections.Generic;
using CollisionFloatTestNewMono.Engine.Math2;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    ///     https://github.com/VelcroPhysics/VelcroPhysics/blob/1456abf40e4c30065bf122f409ce60ce3873ff09/VelcroPhysics/Collision/AABBHelper.cs
    /// </summary>
    public static class AabbHelper
    {
        /// <summary>
        ///     A small length used as a collision and constraint tolerance. Usually it is
        ///     chosen to be numerically significant, but visually insignificant.
        /// </summary>
        private const float LinearSlop = 0.005f;

        /// <summary>
        ///     The radius of the polygon/edge shape skin. This should not be modified. Making
        ///     this smaller means polygons will have an insufficient buffer for continuous collision.
        ///     Making it larger may create artifacts for vertex collision.
        /// </summary>
        private const float PolygonRadius = 2.0f * LinearSlop;


        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="origin"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Aabb ComputeLineAabb(Vector2 start, Vector2 end, Vector2 origin, float angle)
        {
            var roation = new Rotation(angle);
            var v1 = MathUtils.Rotate(start, origin, roation);
            var v2 = MathUtils.Rotate(end, origin, roation);

            var aabb = new Aabb();
            aabb.LowerBound = Vector2.Min(v1, v2);
            aabb.UpperBound = Vector2.Max(v1, v2);

            var r = new Vector2(PolygonRadius, PolygonRadius);
            aabb.LowerBound = aabb.LowerBound - r;
            aabb.UpperBound = aabb.UpperBound + r;

            return aabb;
        }


        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Aabb ComputeCircleAabb(Vector2 position, float radius)
        {
            var aabb = new Aabb();
            aabb.LowerBound = new Vector2(position.X - radius, position.Y - radius);
            aabb.UpperBound = new Vector2(position.X + radius, position.Y + radius);
            return aabb;
        }


        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="vertices"></param>
        /// <param name="origin"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static Aabb ComputePolygonAabb(Vector2 position, IList<Vector2> vertices, Vector2 origin, Rotation rotation)
        {
            var lower = position + MathUtils.Rotate(vertices[0], origin, rotation);
            var upper = lower;

            for (var i = 1; i < vertices.Count; ++i)
            {
                var v = position + MathUtils.Rotate(vertices[i], origin, rotation);
                lower = Vector2.Min(lower, v);
                upper = Vector2.Max(upper, v);
            }

            var r = new Vector2(PolygonRadius, PolygonRadius);

            var aabb = new Aabb();
            aabb.LowerBound = lower - r;
            aabb.UpperBound = upper + r;
            return aabb;
        }
    }
}