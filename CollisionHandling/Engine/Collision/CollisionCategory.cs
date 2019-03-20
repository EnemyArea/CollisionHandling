#region

using System;

#endregion

namespace CollisionFloatTestNewMono.Engine.Collision
{
    /// <summary>
    /// </summary>
    [Flags]
    public enum CollisionCategory
    {
        All = 0,
        Cat0 = 1,
        Cat1 = 2,
        Cat2 = 4,
        Cat3 = 8,
        Cat4 = 16,
        Cat5 = 32,
        Cat6 = 64,
        Cat7 = 128,
        Cat8 = 256,
        Cat9 = 512
    }
}