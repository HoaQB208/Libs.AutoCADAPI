using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

public class IntersectAPI
{
    public static List<Point3d> Intersect(Entity entThis, Entity entArg, Intersect intersectType)
    {
        Point3dCollection points = new Point3dCollection();
        entThis.IntersectWith(entArg, intersectType, points, IntPtr.Zero, IntPtr.Zero);
        return points.Cast<Point3d>().ToList();
    }
}
