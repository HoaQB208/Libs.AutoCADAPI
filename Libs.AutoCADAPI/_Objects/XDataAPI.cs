using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

public class XDataAPI
{
    public static void CreateRegApp(string regAppName)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            RegAppTable tb = tr.GetObject(db.RegAppTableId, OpenMode.ForRead, false) as RegAppTable;
            if (!tb.Has(regAppName))
            {
                tb.UpgradeOpen();
                RegAppTableRecord rec = new RegAppTableRecord { Name = regAppName };
                tb.Add(rec);
                tr.AddNewlyCreatedDBObject(rec, true);
                tr.Commit();
            }
        }
    }

}
