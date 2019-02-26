#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public abstract class Shape
    {
        public Color Color { get; set; }

        public Vector2 Velocity { get; set; }

        protected Shape()
        {
            this.Color = Color.Gray;
        }
    }
}