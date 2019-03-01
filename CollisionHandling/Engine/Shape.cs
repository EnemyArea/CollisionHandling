#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public abstract class Shape
    {
        public string Name { get; }

        public Color Color { get; set; }

        public Vector2 Velocity { get; set; }

        public virtual Vector2 Position { get; set; }

        public ShapeType ShapeType { get;}

        protected Shape(ShapeType shapeType, string name)
        {
            this.ShapeType = shapeType;
            this.Name = name;
            this.Color = Color.Gray;
        }
    }
}