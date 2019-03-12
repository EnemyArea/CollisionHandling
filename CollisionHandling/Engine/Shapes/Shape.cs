#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine.Shapes
{
    /// <summary>
    /// </summary>
    public abstract class Shape
    {
        /// <summary>
        /// </summary>
        public ShapeType ShapeType { get; }

        /// <summary>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// </summary>
        public Vector2 Velocity { get; private set; }

        /// <summary>
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        ///     In radians
        /// </summary>
        public float Rotation { get; private set; }

        /// <summary>
        /// </summary>
        public Rectangle BoundingBox { get; protected set; }


        /// <summary>
        /// </summary>
        /// <param name="shapeType"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        protected Shape(ShapeType shapeType, string name, Vector2 position, int rotation)
        {
            this.ShapeType = shapeType;
            this.Name = name;
            this.Color = Color.Gray;
            this.Position = position;
            this.Rotation = rotation;
        }


        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        public virtual void SetPosition(Vector2 position)
        {
            this.Position = position;      
            this.UpdateBoundingBox();   
        }


        /// <summary>
        /// </summary>
        /// <param name="velocity"></param>
        public virtual void MoveByVelocity(Vector2 velocity)
        {
            this.Position += velocity;
            this.UpdateBoundingBox();
        }


        /// <summary>
        /// </summary>
        /// <param name="radians"></param>
        public virtual void SetRotation(float radians)
        {
            this.Rotation = radians;
        }


        /// <summary>
        /// </summary>
        protected virtual void UpdateBoundingBox()
        {
        }


        /// <summary>
        /// </summary>
        public void ResetVelocity()
        {
            this.Velocity = Vector2.Zero;
        }


        /// <summary>
        /// </summary>
        /// <param name="velocity"></param>
        public virtual void ApplyVelocity(Vector2 velocity)
        {
            this.Velocity += velocity;
        }
    }
}