﻿namespace CollisionFloatTestNewMono.Engine
{
    /// <summary>
    /// </summary>
    public enum ShapeContactType : byte
    {
        NotSupported,
        Polygon,
        PolygonAndCircle,
        Circle,
        LineAndPolygon,
        LineAndCircle
    }
}