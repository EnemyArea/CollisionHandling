#region

using CollisionFloatTestNewMono.Engine.Shapes;
using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Collision
{
    /// <summary>
    /// </summary>
    public struct ShapeCollision
    {
        /// <summary>
        /// </summary>
        public ShapeContactType ShapeContactType { get; }

        /// <summary>
        /// </summary>
        public Shape ShapeA { get; }

        /// <summary>
        /// </summary>
        public Shape ShapeB { get; }

        /// <summary>
        /// </summary>
        public Vector2 Velocity { get; }


        /// <summary>
        /// </summary>
        /// <param name="shapeContactType"></param>
        /// <param name="shapeA"></param>
        /// <param name="shapeB"></param>
        /// <param name="velocity"></param>
        public ShapeCollision(ShapeContactType shapeContactType, Shape shapeA, Shape shapeB, Vector2 velocity)
        {
            this.ShapeContactType = shapeContactType;
            this.ShapeA = shapeA;
            this.ShapeB = shapeB;
            this.Velocity = velocity;
        }
    }
}