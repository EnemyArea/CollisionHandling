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
        /// <param name="shapeContactType"></param>
        /// <param name="shapeA"></param>
        /// <param name="shapeB"></param>
        public ShapeCollision(ShapeContactType shapeContactType, Shape shapeA, Shape shapeB)
        {
            this.ShapeContactType = shapeContactType;
            this.ShapeA = shapeA;
            this.ShapeB = shapeB;
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.ShapeContactType} => {this.ShapeA.Name} & {this.ShapeB.Name}";
        }
    }
}