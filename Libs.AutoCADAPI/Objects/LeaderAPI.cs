using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace Libs.AutoCADAPI.Objects
{
    public class LeaderAPI
    {
        public static void Create(List<Point3d> points, double arrowSize, ObjectId mTextId)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                Leader leader = new Leader
                {
                    HasArrowHead = true,
                    Dimasz = arrowSize,
                    Dimtad = 0 // Centered
                };
                if (mTextId != ObjectId.Null) leader.Annotation = mTextId;
                foreach (Point3d pt in points) leader.AppendVertex(pt);
                leader.SetDatabaseDefaults();
                leader.EvaluateLeader();
                rec.AppendEntity(leader);
                tr.AddNewlyCreatedDBObject(leader, true);
                tr.Commit();
            }
        }

    }
}
