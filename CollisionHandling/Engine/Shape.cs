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

        public ShapeType ShapeType { get;}

        protected Shape(ShapeType shapeType)
        {
            this.ShapeType = shapeType;
            this.Color = Color.Gray;
        }
    }
}