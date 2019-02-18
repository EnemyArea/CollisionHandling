#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public class Contact
    {
        public Shape Shape { get; }
        public Shape ShapeObs { get; }
        public Vector2 Vector { get; }

        public Contact(Shape shape, Shape shapeObs, Vector2 vector)
        {
            this.Shape = shape;
            this.ShapeObs = shapeObs;
            this.Vector = vector;
        }
    }
}