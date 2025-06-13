using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

public class CircleAPI
{
    public static ObjectId Create(Point3d center, double radius)
    {
        ObjectId id = ObjectId.Null;
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
            Circle circle = new Circle();
            circle.SetDatabaseDefaults();
            circle.Center = center;
            circle.Radius = radius;
            rec.AppendEntity(circle);
            tr.AddNewlyCreatedDBObject(circle, true);
            tr.Commit();
            id = circle.ObjectId;
        }
        return id;
    }

}
