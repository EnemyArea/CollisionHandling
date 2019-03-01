#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    ///     A transform contains translation and rotation. It is used to represent
    ///     the position and orientation of rigid frames.
    /// </summary>
    public struct Transform
    {
        public Vector2 p;
        public Rotation q;


        /// <summary>
        ///     Initialize using a position vector and a rotation matrix.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The r.</param>
        public Transform(Vector2 position, Rotation rotation)
        {
            this.p = position;
            this.q = rotation;
        }


        /// <summary>
        ///     Set this to the identity transform.
        /// </summary>
        public void SetIdentity()
        {
            this.p = Vector2.Zero;
            this.q.SetIdentity();
        }


        /// <summary>
        ///     Set this based on the position and angle.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="angle">The angle.</param>
        public void Set(Vector2 position, float angle)
        {
            this.p = position;
            this.q.Set(angle);
        }
    }
}