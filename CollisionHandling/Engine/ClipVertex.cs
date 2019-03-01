using Microsoft.Xna.Framework;
using VelcroPhysics.Collision.ContactSystem;

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// Used for computing contact manifolds.
    /// </summary>
    internal struct ClipVertex
    {
        public ContactID ID;
        public Vector2 V;
    }
}