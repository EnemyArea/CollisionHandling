#region

using System.Collections.Generic;
using CollisionFloatTestNewMono.Engine.Collision;
using CollisionFloatTestNewMono.Engine.Math2;
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
        public Rotation Rotation { get; private set; }

        /// <summary>
        /// </summary>
        public Rectangle BoundingBox { get; protected set; }

        /// <summary>
        /// </summary>
        public Rectangle BoundingBoxTileMap { get; protected set; }

        /// <summary>
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// </summary>
        public bool IsSensor { get; set; }

        /// <summary>
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// </summary>
        public IList<Shape> IgnoredCollisions { get; private set; }

        /// <summary>
        /// </summary>
        public CollisionCategory CollisionCategory { get; set; }

        /// <summary>
        /// </summary>
        public CollisionCategory CollidesOnyWithCategories { get; set; }


        /// <summary>
        /// </summary>
        /// <param name="shapeType"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="angle"></param>
        /// <param name="isStatic"></param>
        protected Shape(ShapeType shapeType, string name, Vector2 position, float angle, bool isStatic)
        {
            this.ShapeType = shapeType;
            this.Name = name;
            this.Color = Color.Gray;
            this.Position = position;
            this.Rotation = new Rotation(angle);
            this.IsStatic = isStatic;
            this.IsEnabled = true;
            this.IgnoredCollisions = new List<Shape>();
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
        /// <param name="angle"></param>
        public virtual void SetRotation(float angle)
        {
            this.Rotation = new Rotation(angle);
            this.UpdateBoundingBox();
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