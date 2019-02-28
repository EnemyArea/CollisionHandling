#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public abstract class Shape
    {
        public Color Color { get; set; }

        public Vector2 Velocity { get; set; }

        public virtual Vector2 Position { get; set; }

        public ShapeContactType ShapeContactType { get;}

        protected Shape(ShapeContactType shapeContactType)
        {
            this.ShapeContactType = shapeContactType;
            this.Color = Color.Gray;
        }
    }
}