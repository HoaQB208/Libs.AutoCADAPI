using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

public class DimAPI
{
    public static void DimX(Point3d startDim, Point3d endDim, double offset, string textDim = null)
    {
        if (startDim.X == endDim.X) return;

        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
            double Y = offset > 0 ? Math.Max(startDim.Y, endDim.Y) : Math.Min(startDim.Y, endDim.Y);
            RotatedDimension dim = new RotatedDimension
            {
                XLine1Point = startDim,
                XLine2Point = endDim,
                Rotation = 0,
                DimensionStyle = db.Dimstyle,
                DimLinePoint = new Point3d((startDim.X + endDim.X) / 2, Y + offset, 0)
            };
            if (textDim != null) dim.DimensionText = textDim;
            dim.SetDatabaseDefaults();
            rec.AppendEntity(dim);
            tr.AddNewlyCreatedDBObject(dim, true);
            tr.Commit();
        }
    }

    public static void DimY(Point3d startDim, Point3d endDim, double offset, string textDim = null)
    {
        if (startDim.Y == endDim.Y) return;

        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
            double X = offset > 0 ? Math.Max(startDim.X, endDim.X) : Math.Min(startDim.X, endDim.X);
            RotatedDimension dim = new RotatedDimension
            {
                XLine1Point = startDim,
                XLine2Point = endDim,
                Rotation = Math.PI / 2,
                DimensionStyle = db.Dimstyle,
                DimLinePoint = new Point3d(X + offset, (startDim.Y + endDim.Y) / 2, 0)
            };
            if (textDim != null) dim.DimensionText = textDim;
            dim.SetDatabaseDefaults();
            rec.AppendEntity(dim);
            tr.AddNewlyCreatedDBObject(dim, true);
            tr.Commit();
        }
    }

    public static string DimAligned(Point3d pXLine1, Point3d pXLine2, double offset, string textDim = null)
    {
        string stHandle = "";
        if (pXLine1.DistanceTo(pXLine2) > 0.001)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                Vector3d vector3D = pXLine2 - pXLine1;
                Vector3d vt = vector3D.CrossProduct(Vector3d.ZAxis).GetNormal().MultiplyBy(offset);
                Point3d pDimLine = pXLine1 + vt;
                AlignedDimension dim = new AlignedDimension
                {
                    XLine1Point = pXLine1,
                    XLine2Point = pXLine2,
                    DimLinePoint = pDimLine,
                    DimensionStyle = db.Dimstyle
                };
                if (textDim != null) dim.DimensionText = textDim;
                rec.AppendEntity(dim);
                tr.AddNewlyCreatedDBObject(dim, true);
                tr.Commit();
                stHandle = dim.Handle.ToString();
            }
        }
        return stHandle;
    }
}
