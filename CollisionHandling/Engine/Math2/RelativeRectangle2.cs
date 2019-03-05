using Microsoft.Xna.Framework;

namespace CollisionFloatTestNewMono.Engine.Math2
{
    /// <summary>
    /// Describes a rectangle that is describing the percentages to go
    /// of the true rectangle. Useful in some UI circumstances.
    /// </summary>
	public class RelativeRectangle2 : Rect2
    {
        public RelativeRectangle2(Vector2 min, Vector2 max) : base(min, max)
        {
        }

        public RelativeRectangle2(float x, float y, float w, float h) : base(new Vector2(x, y), new Vector2(x + w, y + h))
        {
        }

        public Rect2 ToRect(Rect2 original)
        {
            return new Rect2(original.Min * this.Min, original.Max * this.Max);
        }

#if !NOT_MONOGAME
        public Rect2 ToRect(Rectangle original) {
			return new Rect2(
				new Vector2(original.Left + original.Width * this.Min.X, original.Top  + original.Height * this.Min.Y),
					new Vector2(original.Left + original.Width * this.Max.X, original.Top + original.Height * this.Max.Y)
			);
		}
#endif
    }
}
