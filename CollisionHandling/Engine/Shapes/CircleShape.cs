#region

using Microsoft.Xna.Framework;

#endregion

namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// </summary>
    public class CircleShape : Shape
    {
        /// <summary>
        /// </summary>
        public int Radius { get; }

        /// <summary>
        /// </summary>
        public Point TilePosition { get; private set; }


        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="radius"></param>
        public CircleShape(string name, Vector2 position, int radius)
            : base(ShapeType.Circle, name, position, 0)
        {
            this.Radius = radius;
            this.TilePosition = GameHelper.ConvertPositionToTilePosition(position);
        }


        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        public override void SetPosition(Vector2 position)
        {
            base.SetPosition(position);
            this.TilePosition = GameHelper.ConvertPositionToTilePosition(position);
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Name} / {this.Radius} / {this.Position} / {this.TilePosition}";
        }
    }
}