#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
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
        /// </summary>
        public float Rotation { get; private set; }


        /// <summary>
        /// </summary>
        protected Transform transform = new Transform();

        /// <summary>
        /// </summary>
        public Transform Transform
        {
            get { return this.transform; }
        }


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
            this.UpdateTransform();
        }


        /// <summary>
        /// </summary>
        protected virtual void UpdateTransform()
        {
            this.transform.Set(this.Position + this.Velocity, this.Rotation);
        }


        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        public virtual void SetPosition(Vector2 position)
        {
            this.Position = position;
            this.UpdateTransform();
        }


        /// <summary>
        /// </summary>
        /// <param name="velocity"></param>
        public virtual void MoveByVelocity(Vector2 velocity)
        {
            this.Position += velocity;
            this.UpdateTransform();
        }


        /// <summary>
        /// </summary>
        /// <param name="rotation"></param>
        public virtual void SetRotation(float rotation)
        {
            this.Rotation = rotation;
            this.UpdateTransform();
        }


        /// <summary>
        /// </summary>
        public void ResetVelocity()
        {
            this.Velocity = Vector2.Zero;
            this.UpdateTransform();
        }


        /// <summary>
        /// </summary>
        /// <param name="velocity"></param>
        public virtual void ApplyVelocity(Vector2 velocity)
        {
            this.Velocity += velocity;
            this.UpdateTransform();
        }
    }
}