using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace Libs.AutoCADAPI.Objects
{
    public class LineTypeAPI
    {
        public static ObjectId GetLineTypeId(string lineTypeName, string fromFile = "acad.lin")
        {
            ObjectId id = ObjectId.Null;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LinetypeTable rec = tr.GetObject(db.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
                if (!rec.Has(lineTypeName)) db.LoadLineTypeFile(lineTypeName, fromFile);
                if (rec.Has(lineTypeName)) id = rec[lineTypeName];
                else id = rec["Continuous"];
            }
            return id;
        }
    }
}
