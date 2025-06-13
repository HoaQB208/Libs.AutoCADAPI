using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Libs.AutoCADAPI.Objects
{
    public class WipeoutAPI
    {
        public static ObjectId Create(Point2dCollection points2d, out string handle)
        {
            ObjectId id = ObjectId.Null;
            handle = "";
            if (points2d[0] != points2d[points2d.Count - 1]) points2d.Add(points2d[0]);
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                Wipeout wp = new Wipeout() { ColorIndex = 256 };
                wp.SetDatabaseDefaults();
                wp.SetFrom(points2d, new Vector3d(0, 0, 1));
                rec.AppendEntity(wp);
                tr.AddNewlyCreatedDBObject(wp, true);
                tr.Commit();
                id = wp.Id;
                handle = wp.Handle.ToString();
            }
            return id;
        }
    }
}
