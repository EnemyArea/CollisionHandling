#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    ///     An axis aligned bounding box.
    ///     https://github.com/VelcroPhysics/VelcroPhysics/blob/1456abf40e4c30065bf122f409ce60ce3873ff09/VelcroPhysics/Shared/AABB.cs
    /// </summary>
    public struct Aabb
    {
        /// <summary>
        ///     The lower vertex
        /// </summary>
        public Vector2 LowerBound;

        /// <summary>
        ///     The upper vertex
        /// </summary>
        public Vector2 UpperBound;

        /// <summary>
        /// </summary>
        public float Width => this.UpperBound.X - this.LowerBound.X;

        /// <summary>
        /// </summary>
        public float Height => this.UpperBound.Y - this.LowerBound.Y;
    }
}