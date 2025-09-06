using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Windows;

namespace Libs.AutoCADAPI.Utils
{
    public class DirectionAPI
    {
        public static bool IsHorizontal(Line line, double angleDegreesTolerance)
        {
            // Nằm ngang khi góc nhọn của đường line hợp với phương nằm ngang bé hơn góc sai số
            double phandu = line.Angle % Math.PI;
            if (phandu > Math.PI / 2) phandu = Math.PI - phandu; // Lấy góc nhọn
            double angle = UnitsAPI.RadianToDegrees(phandu);
            return angle <= angleDegreesTolerance;
        }

        public static bool IsVertical(Line line, double angleDegreesTolerance)
        {
            // Line đứng khi góc nhọn của đường line hợp với phương thẳng đứng bé hơn góc sai số
            double phandu = (line.Angle + Math.PI / 2) % Math.PI;
            if (phandu > Math.PI / 2) phandu = Math.PI - phandu; // Lấy góc nhọn
            double angle = UnitsAPI.RadianToDegrees(phandu);
            return angle <= angleDegreesTolerance;
        }

        /// <summary>
        ///  Kiểm tra 2 vector có cùng phương (song song, không quan tâm chiều)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsCollinear(Vector3d v1, Vector3d v2, double tolerance = 0.001)
        {
            // Nếu 1 trong 2 vector là zero thì không xác định
            if (v1.IsZeroLength() || v2.IsZeroLength()) return false;
            var cross = v1.CrossProduct(v2);
            return cross.Length < tolerance;
        }
    }
}
