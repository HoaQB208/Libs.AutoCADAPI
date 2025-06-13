using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

public class ArcAPI
{
    public static ObjectId Create(Point3d center, double radius, double startAngle, double endAngle)
    {
        ObjectId id = ObjectId.Null;
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
            Arc arc = new Arc(center, radius, startAngle, endAngle);
            arc.SetDatabaseDefaults();
            rec.AppendEntity(arc);
            tr.AddNewlyCreatedDBObject(arc, true);
            tr.Commit();
            id = arc.ObjectId;
        }
        return id;
    }

}
