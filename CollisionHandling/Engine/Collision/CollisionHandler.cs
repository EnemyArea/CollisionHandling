namespace CollisionFloatTestNewMono.Engine.Collision
{
    /// <summary>
    /// </summary>
    /// <param name="collisionManager"></param>
    /// <param name="shapeCollision"></param>
    /// <returns></returns>
    public delegate bool SeperationHandler(CollisionManager collisionManager, ShapeCollision shapeCollision);

    /// <summary>
    /// </summary>
    /// <param name="collisionManager"></param>
    /// <param name="shapeCollision"></param>
    /// <returns></returns>
    public delegate void CollisionHandler(CollisionManager collisionManager, ShapeCollision shapeCollision);
}