using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

public class PolylineAPI
{
    public static ObjectId Create(List<Point3d> points, out string handle, bool isClosed = true, double elevation = 0, string layer = "", double? width = null)
    {
        ObjectId id = ObjectId.Null;
        handle = "";
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
            Polyline pline = new Polyline();
            pline.SetDatabaseDefaults();
            double w = width.HasValue ? width.Value : 0;
            if (isClosed & points[0] != points[points.Count - 1]) points.Add(points[0]);
            for (int i = 0; i < points.Count; i++)
            {
                Point3d pt = points[i];
                pline.AddVertexAt(i, new Point2d(pt.X, pt.Y), 0, w, w);
            }
            pline.Closed = isClosed;
            pline.Elevation = elevation;
            try { if (layer != "") pline.Layer = layer; } catch { }
            rec.AppendEntity(pline);
            tr.AddNewlyCreatedDBObject(pline, true);
            tr.Commit();
            id = pline.ObjectId;
            handle = pline.Handle.ToString();
        }
        return id;
    }

    public static void AddVertexAtLast(ObjectId plineId, Point3d point)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            Polyline pline = tr.GetObject(plineId, OpenMode.ForWrite) as Polyline;
            var w = pline.GetStartWidthAt(0);
            pline.AddVertexAt(pline.NumberOfVertices, new Point2d(point.X, point.Y), 0, w, w);
            tr.Commit();
        }
    }

    public static void AddVertexBeforeLast(ObjectId plineId, Point3d point)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            Polyline pline = tr.GetObject(plineId, OpenMode.ForWrite) as Polyline;
            var w = pline.GetStartWidthAt(0);
            pline.AddVertexAt(pline.NumberOfVertices - 1, new Point2d(point.X, point.Y), 0, w, w);
            tr.Commit();
        }
    }

    public static void RemoveVertexBeforeLast(ObjectId plineId)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            Polyline pline = tr.GetObject(plineId, OpenMode.ForWrite) as Polyline;
            pline.RemoveVertexAt(pline.NumberOfVertices - 2);
            tr.Commit();
        }
    }


    public static void AddVertexAt(ObjectId plineId, int index, Point3d point)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            Polyline pline = tr.GetObject(plineId, OpenMode.ForWrite) as Polyline;
            pline.AddVertexAt(index, new Point2d(point.X, point.Y), 0, 0, 0);
            tr.Commit();
        }
    }

    public static ObjectId CreateRectangle(Point3d cornerPt1, Point3d cornerPt2)
    {
        List<Point3d> points = new List<Point3d>() { cornerPt1, new Point3d(cornerPt1.X, cornerPt2.Y, 0), cornerPt2, new Point3d(cornerPt2.X, cornerPt1.Y, 0) };
        return Create(points, out _, true);
    }

    public static ObjectId CreateRectangle(Point3d center, double width, double height)
    {
        Point3dCollection collection = PointAPI.Rectangle(center, width, height);
        List<Point3d> points = new List<Point3d>();
        foreach (Point3d pt in collection) points.Add(pt);
        return Create(points.ToList(), out _, true);
    }

    public static double Height(Polyline pline)
    {
        List<Point3d> points = PointAPI.GetPoints(pline);
        IEnumerable<double> lsY = points.Select(p => p.Y);
        double maxY = lsY.Max();
        double minY = lsY.Min();
        return maxY - minY;
    }

    public static void FilletAll(ObjectId plineId, double radius)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            Polyline pline = plineId.GetObject(OpenMode.ForWrite) as Polyline;

            int n = pline.Closed ? 0 : 1;
            for (int i = n; i < pline.NumberOfVertices - n; i += 1 + FilletAt(pline, i, radius)) { }

            tr.Commit();
        }
    }

    private static int FilletAt(Polyline pline, int index, double radius)
    {
        int prev = index == 0 && pline.Closed ? pline.NumberOfVertices - 1 : index - 1;
        if (pline.GetSegmentType(prev) != SegmentType.Line || pline.GetSegmentType(index) != SegmentType.Line) return 0;

        LineSegment2d seg1 = pline.GetLineSegment2dAt(prev);
        LineSegment2d seg2 = pline.GetLineSegment2dAt(index);
        Vector2d vec1 = seg1.StartPoint - seg1.EndPoint;
        Vector2d vec2 = seg2.EndPoint - seg2.StartPoint;
        double angle = (Math.PI - vec1.GetAngleTo(vec2)) / 2.0;
        double dist = radius * Math.Tan(angle);
        if (dist == 0.0 || dist > seg1.Length || dist > seg2.Length) return 0;

        Point2d pt1 = seg1.EndPoint + vec1.GetNormal() * dist;
        Point2d pt2 = seg2.StartPoint + vec2.GetNormal() * dist;
        double bulge = Math.Tan(angle / 2.0);
        if (PointAPI.IsClockWise(seg1.StartPoint, seg1.EndPoint, seg2.EndPoint))
        {
            bulge = -bulge;
        }
        pline.AddVertexAt(index, pt1, bulge, 0.0, 0.0);
        pline.SetPointAt(index + 1, pt2);
        return 1;
    }

    public static Point3dCollection GetPoint3dCol(Polyline pline)
    {
        Point3dCollection point3DCollection = new Point3dCollection();
        for (int i = 0; i < pline.NumberOfVertices; i++)
        {
            var p = pline.GetPoint3dAt(i);
            point3DCollection.Add(p);
        }
        return point3DCollection;
    }
}
