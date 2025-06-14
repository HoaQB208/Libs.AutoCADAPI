using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Libs.AutoCADAPI.Utils
{
    public class LineUtils
    {
        public static double Distance(Line line, Point3d point, bool extend = false)
        {
            if (line == null) return double.MaxValue;
            Point3d closestPoint = line.GetClosestPointTo(point, extend);
            return point.DistanceTo(closestPoint);
        }
    }
}