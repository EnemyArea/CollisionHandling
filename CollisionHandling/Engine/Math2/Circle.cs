namespace CollisionFloatTestNewMono.Engine.Math2
{
    /// <summary>
    ///     Describes a circle in the x-y plane.
    /// </summary>
    public class Circle
    {
        /// <summary>
        ///     The radius of the circle
        /// </summary>
        public readonly float Radius;

        /// <summary>
        ///     Constructs a circle with the specified radius
        /// </summary>
        /// <param name="radius">Radius of the circle</param>
        public Circle(float radius)
        {
            this.Radius = radius;
        }

        /// <summary>
        ///     Determines if the first circle is equal to the second circle
        /// </summary>
        /// <param name="c1">The first circle</param>
        /// <param name="c2">The second circle</param>
        /// <returns>If c1 is equal to c2</returns>
        public static bool operator ==(Circle c1, Circle c2)
        {
            if (ReferenceEquals(c1, null) || ReferenceEquals(c2, null))
                return ReferenceEquals(c1, c2);

            return c1.Radius == c2.Radius;
        }

        /// <summary>
        ///     Determines if the first circle is not equal to the second circle
        /// </summary>
        /// <param name="c1">The first circle</param>
        /// <param name="c2">The second circle</param>
        /// <returns>If c1 is not equal to c2</returns>
        public static bool operator !=(Circle c1, Circle c2)
        {
            if (ReferenceEquals(c1, null) || ReferenceEquals(c2, null))
                return !ReferenceEquals(c1, c2);

            return c1.Radius != c2.Radius;
        }

        /// <summary>
        ///     Determines if this circle is logically the same as the
        ///     specified object.
        /// </summary>
        /// <param name="obj">The object to compare against</param>
        /// <returns>if it is a circle with the same radius</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Circle))
                return false;

            var other = (Circle)obj;
            return this == other;
        }

        /// <summary>
        ///     Calculate a hashcode based solely on the radius of this circle.
        /// </summary>
        /// <returns>hashcode</returns>
        public override int GetHashCode()
        {
            return this.Radius.GetHashCode();
        }
    }
}