using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Libs.AutoCADAPI.Objects;
using System.Collections.Generic;

namespace Libs.AutoCADAPI.Utils
{
    public class TransformAPI
    {
        public static void Move(ObjectId id, Vector3d displacement)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                ent.TransformBy(Matrix3d.Displacement(displacement));
                tr.Commit();
            }
        }

        public static void Rotation(ObjectId id, double angle, Vector3d axis, Point3d center)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                ent.TransformBy(Matrix3d.Rotation(angle, axis, center));
                tr.Commit();
            }
        }

        public static void Scaling(ObjectId id, double scale, Point3d center)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                ent.TransformBy(Matrix3d.Scaling(scale, center));
                tr.Commit();
            }
        }

        public static void Mirroring(ObjectId id, Point3d pt)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                ent.TransformBy(Matrix3d.Mirroring(pt));
                tr.Commit();
            }
        }

        public static void Mirroring(ObjectId id, Line3d line)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                ent.TransformBy(Matrix3d.Mirroring(line));
                tr.Commit();
            }
        }

        public static void Mirroring(ObjectId id, Plane plane)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                ent.TransformBy(Matrix3d.Mirroring(plane));
                tr.Commit();
            }
        }

        public static List<ObjectId> Offset(ObjectId curveId, double dist, bool deleteOrigin = false)
        {
            List<ObjectId> ids = new List<ObjectId>();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                Curve originCurve = tr.GetObject(curveId, OpenMode.ForWrite) as Curve;
                DBObjectCollection obj = originCurve.GetOffsetCurves(dist);
                foreach (Entity ent in obj)
                {
                    rec.AppendEntity(ent);
                    tr.AddNewlyCreatedDBObject(ent, true);
                    ids.Add(ent.ObjectId);
                }
                if (deleteOrigin) originCurve.Erase(true);
                tr.Commit();
            }
            return ids;
        }

        public static string ArrayBlock(Point3d startPoint, Vector3d snapDirection, string blockName, int numb)
        {
            List<string> handles = new List<string>();
            for (int i = 0; i < numb; i++)
            {
                Point3d ptInsert = startPoint + snapDirection.MultiplyBy(i);
                string handle = BlockAPI.InsertBlockReference(blockName, ptInsert);
                handles.Add(handle);
            }
            return ObjectIdAPI.JoinHandles(handles);
        }
    }
}