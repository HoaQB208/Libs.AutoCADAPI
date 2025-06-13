using Autodesk.AutoCAD.DatabaseServices;
using System;

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
}
