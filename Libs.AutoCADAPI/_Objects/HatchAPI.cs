using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

public class HatchAPI
{
    public static void Create(List<Point3d> points, string hatchName, double patternScale)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
            Polyline pline = new Polyline();
            pline.SetDatabaseDefaults();
            for (int i = 0; i < points.Count; i++)
            {
                Point3d pt = points[i];
                pline.AddVertexAt(i, new Point2d(pt.X, pt.Y), 0, 0, 0);
            }
            rec.AppendEntity(pline);
            tr.AddNewlyCreatedDBObject(pline, true);

            Hatch hatch = new Hatch();
            hatch.SetDatabaseDefaults();
            hatch.SetHatchPattern(HatchPatternType.CustomDefined, hatchName);
            hatch.AppendLoop(HatchLoopTypes.Outermost, new ObjectIdCollection { pline.ObjectId });
            hatch.EvaluateHatch(true);
            hatch.PatternScale = patternScale;
            hatch.Associative = true;
            rec.AppendEntity(hatch);
            tr.AddNewlyCreatedDBObject(hatch, true);
            pline.Erase(true);
            tr.Commit();
        }
    }

}
