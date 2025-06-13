using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

public class LineAPI
{
    public static ObjectId Create(Point3d ptStart, Point3d ptEnd, out string handle, string layerName = null, int colorIndex = 256, Entity setPropertiesFrom = null, double lineTypeScale = -1)
    {
        ObjectId id = ObjectId.Null;
        handle = "";
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
            Line ln = new Line(ptStart, ptEnd) { ColorIndex = 256 };
            ln.SetDatabaseDefaults();
            if (setPropertiesFrom != null) ln.SetPropertiesFrom(setPropertiesFrom);
            try { if (layerName != null) ln.Layer = layerName; } catch { } // Lỗi truyền vào Layer không có
            ln.ColorIndex = colorIndex;
            rec.AppendEntity(ln);
            tr.AddNewlyCreatedDBObject(ln, true);

            if (lineTypeScale >= 0) ln.LinetypeScale = lineTypeScale;
            tr.Commit();
            id = ln.ObjectId;
            handle = ln.Handle.ToString();
        }
        return id;
    }

}
