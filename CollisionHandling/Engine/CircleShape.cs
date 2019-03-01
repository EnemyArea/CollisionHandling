﻿#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    public class CircleShape : Shape
    {
        public int Radius { get; }
        public Point TilePosition { get; set; }

        private Vector2 position;

        public override Vector2 Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
                this.TilePosition = GameHelper.ConvertPositionToTilePosition(value);
            }
        }


        public CircleShape(string name, Vector2 position, int radius)
            : base(ShapeType.Circle, name)
        {
            this.Position = position;
            this.Radius = radius;
            this.TilePosition = GameHelper.ConvertPositionToTilePosition(position);
        }


        public override string ToString()
        {
            return $"{this.Name} / {this.Radius} / {this.Position} / {this.TilePosition}";
        }
    }
}