using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

public class ViewportAPI
{
    public static void Create(ObjectId layoutId, Point2d centerPointInModel, Point3d centerPointInLayout, double width, double height, double customScale, bool isLock = true)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            Layout layout = tr.GetObject(layoutId, OpenMode.ForWrite) as Layout;
            BlockTableRecord rec = tr.GetObject(layout.BlockTableRecordId, OpenMode.ForWrite) as BlockTableRecord;
            Viewport vp = new Viewport
            {
                ViewCenter = centerPointInModel,
                CenterPoint = centerPointInLayout,
                CustomScale = customScale,
                Width = width,
                Height = height,
                Locked = isLock
            };
            rec.AppendEntity(vp);
            tr.AddNewlyCreatedDBObject(vp, true);
            vp.On = true;
            vp.GridOn = true;
            tr.Commit();
        }
    }

}
