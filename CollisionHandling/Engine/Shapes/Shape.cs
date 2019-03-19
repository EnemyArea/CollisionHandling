﻿#region

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
        /// <param name="shapeType"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="isStatic"></param>
        protected Shape(ShapeType shapeType, string name, Vector2 position, int rotation, bool isStatic)
        {
            this.ShapeType = shapeType;
            this.Name = name;
            this.Color = Color.Gray;
            this.Position = position;
            this.Rotation = rotation;
            this.IsStatic = isStatic;
            this.IsEnabled = true;
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