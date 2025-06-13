using System;

public class UnitsAPI
{
    public static double RadianToDegrees(double radian)
    {
        return radian * 180 / Math.PI;
    }

    public static double DegreesToRadian(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
