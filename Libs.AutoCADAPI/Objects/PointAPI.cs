using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace Libs.AutoCADAPI.Objects
{
    public class PointAPI
    {
        public static Point2d To2D(Point3d pt)
        {
            return new Point2d(pt.X, pt.Y);
        }

        public static Point3d MidPoint(Point3d pt1, Point3d pt2)
        {
            return new Point3d((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2, (pt1.Z + pt2.Z) / 2);
        }

        public static Point3d MidPoint(Curve curve)
        {
            return GetPointOnCurve(curve, 0.5);
        }

        public static Point3d GetPointOnCurve(Curve curve, double rate)
        {
            double start = curve.GetDistanceAtParameter(curve.StartParam);
            double end = curve.GetDistanceAtParameter(curve.EndParam);
            return curve.GetPointAtDist(start + (end - start) * rate);
        }

        public static Point3d Bottom(List<Point3d> points)
        {
            Point3d rs = points[0];
            for (int i = 1; i < points.Count; i++) if (points[i].Y < rs.Y) rs = points[i];
            return rs;
        }

        public static Point3d Top(List<Point3d> points)
        {
            Point3d rs = points[0];
            for (int i = 1; i < points.Count; i++) if (points[i].Y > rs.Y) rs = points[i];
            return rs;
        }

        public static Point3d Left(List<Point3d> points)
        {
            Point3d rs = points[0];
            for (int i = 1; i < points.Count; i++) if (points[i].X < rs.X) rs = points[i];
            return rs;
        }

        public static Point3d Right(List<Point3d> points)
        {
            Point3d rs = points[0];
            for (int i = 1; i < points.Count; i++) if (points[i].X > rs.X) rs = points[i];
            return rs;
        }

        public static List<Point3d> GetPoints(Polyline pl)
        {
            List<Point3d> points = new List<Point3d>();
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                points.Add(pl.GetPoint3dAt(i));
            }
            return points;
        }

        public static Point3dCollection Rectangle(Point3d center, double width, double height)
        {
            Point3d ptLeftBot = new Point3d(center.X - 0.5 * width, center.Y - 0.5 * width, 0);
            Point3d ptRightBot = new Point3d(ptLeftBot.X + width, ptLeftBot.Y, 0);
            Point3d ptRighTop = new Point3d(ptRightBot.X, ptRightBot.Y + height, 0);
            Point3d ptLeftTop = new Point3d(ptLeftBot.X, ptRighTop.Y, 0);
            return new Point3dCollection() { ptLeftBot, ptRightBot, ptRighTop, ptLeftTop, ptLeftBot };
        }

        public static Point3dCollection GetPoint3dCollection(Polyline pl)
        {
            Point3dCollection points = new Point3dCollection();
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                points.Add(pl.GetPoint3dAt(i));
            }
            return points;
        }

        public static Point2dCollection GetPoint2dCollection(Polyline pl)
        {
            Point2dCollection points = new Point2dCollection();
            for (int i = 0; i < pl.NumberOfVertices; i++)
            {
                points.Add(pl.GetPoint2dAt(i));
            }
            return points;
        }

        public static Point2dCollection GetPoint2dCollection(List<Point3d> pts)
        {
            Point2dCollection points = new Point2dCollection();
            foreach (Point3d p in pts)
            {
                points.Add(To2D(p));
            }
            return points;
        }

        public static double FindX(Point3d pt1, Point3d pt2, double offset, double Y, bool getRight)
        {
            if (pt1.X.Equals(pt2.X))
            {
                if (getRight) return pt1.X + offset;
                else return pt1.X - offset;
            }
            else
            {
                double A, B, C, X_1, X_2, right, left;
                A = (pt1.Y - pt2.Y) / (double)(pt2.X - pt1.X);
                B = 1;
                C = (pt1.X * pt2.Y - pt2.X * pt1.Y) / (double)(pt2.X - pt1.X);
                X_1 = (-offset * Math.Pow((Math.Pow(A, 2) + Math.Pow(B, 2)), 0.5) - C - B * Y) / A;
                X_2 = (offset * Math.Pow((Math.Pow(A, 2) + Math.Pow(B, 2)), 0.5) - C - B * Y) / A;
                right = Math.Min(X_1, X_2);
                left = Math.Max(X_1, X_2);
                if (getRight) return left;
                else return right;
            }
        }

        public static double FindY(Point3d pt1, Point3d pt2, double offset, double X, bool getTop)
        {
            if (pt1.Y.Equals(pt2.Y))
            {
                if (getTop) return pt1.Y + offset;
                else return pt1.Y - offset;
            }
            else
            {
                double A, B, C, Y_1, Y_2, bottom, top;
                A = (pt1.Y - pt2.Y) / (double)(pt2.X - pt1.X);
                B = 1;
                C = (pt1.X * pt2.Y - pt2.X * pt1.Y) / (double)(pt2.X - pt1.X);
                Y_1 = (-offset * Math.Pow((Math.Pow(A, 2) + Math.Pow(B, 2)), 0.5) - C - A * X) / B;
                Y_2 = (offset * Math.Pow((Math.Pow(A, 2) + Math.Pow(B, 2)), 0.5) - C - A * X) / B;
                bottom = Math.Min(Y_1, Y_2);
                top = Math.Max(Y_1, Y_2);
                if (getTop) return top;
                else return bottom;
            }
        }

        public static double DistanceToCurve(Point3d pt, Curve curve, bool extend)
        {
            return pt.DistanceTo(curve.GetClosestPointTo(pt, extend));
        }

        public static double Angle(Point3d start, Point3d center, Point3d end)
        {
            Vector3d vector1 = start - center;
            Vector3d vector2 = end - center;
            return vector1.GetAngleTo(vector2);
        }

        public static bool IsPointOnCurve(Point3d pt, Curve curve)
        {
            Point3d p = curve.GetClosestPointTo(pt, false);
            return (p - pt).Length <= Tolerance.Global.EqualPoint;
        }

        public static bool IsStartPointCloserToPoint(Curve curve, Point3d pt)
        {
            return pt.DistanceTo(curve.StartPoint) < pt.DistanceTo(curve.EndPoint);
        }

        public static bool IsStartPointCloserToCurve(Curve curve, Curve curveArgn)
        {
            double startDis = DistanceToCurve(curve.StartPoint, curveArgn, false);
            double endDis = DistanceToCurve(curve.EndPoint, curveArgn, false);
            return startDis < endDis;
        }

        public static bool IsNearAMoreThanB(Point3d pt, Point3d ptA, Point3d ptB)
        {
            return pt.DistanceTo(ptA) < pt.DistanceTo(ptB);
        }


        public static bool IsPointRightOfLine(Point3d pt, Line line)
        {
            Point3d ptOnLine = line.GetClosestPointTo(pt, true);
            Vector3d vt = pt - ptOnLine;
            return vt.CrossProduct(line.Delta).Z > 0;
        }

        public static bool IsClockWise(Point2d p1, Point2d p2, Point2d p3)
        {
            return ((p2.X - p1.X) * (p3.Y - p1.Y) - (p2.Y - p1.Y) * (p3.X - p1.X)) < 1e-8;
        }

        public static bool IsSame(Point3d p1, Point3d p2, double tolera)
        {
            return p1.DistanceTo(p2) < tolera;
        }
    }
}
