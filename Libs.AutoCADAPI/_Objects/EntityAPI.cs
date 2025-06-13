using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

public class EntityAPI
{
    public static Entity GetEntity(ObjectId id)
    {
        if (id == ObjectId.Null) return null;

        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            return tr.GetObject(id, OpenMode.ForRead) as Entity;
        }
    }

    public static List<Entity> GetEntities(List<ObjectId> ids)
    {
        List<Entity> entities = new List<Entity>();
        if (ids.Count > 0)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in ids)
                {
                    if (id != ObjectId.Null)
                    {
                        if (!id.IsErased)
                        {
                            Entity entity = tr.GetObject(id, OpenMode.ForRead) as Entity;
                            entities.Add(entity);
                        }
                    }
                }
            }
        }
        return entities;
    }

    public static void SetProperties(ObjectId fromObjectId, ObjectId toObjectId)
    {
        Entity objFrom = GetEntity(fromObjectId);
        if (objFrom != null)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity objTo = tr.GetObject(toObjectId, OpenMode.ForWrite) as Entity;
                if (objTo != null)
                {
                    objTo.SetPropertiesFrom(objFrom);
                    tr.Commit();
                }
            }
        }
    }

    public static void SetLayer(ObjectId entId, string newLayer)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            Entity ent = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
            if (ent != null)
            {
                try
                {
                    ent.Layer = newLayer;
                    tr.Commit();
                }
                catch { }
            }
        }
    }

    public static void SetParentId(ObjectId entId, ObjectId parentId, string hyperLinkName = "ParentId")
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            Entity ent = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
            if (ent != null)
            {
                bool added = false;
                foreach (HyperLink item in ent.Hyperlinks)
                {
                    if (item.Name == hyperLinkName)
                    {
                        item.Description = parentId.ToString();
                        added = true;
                    }
                }
                if (!added) ent.Hyperlinks.Add(new HyperLink() { Name = hyperLinkName, Description = parentId.ToString() });
                tr.Commit();
            }
        }
    }


    public static void RemoveParentId(ObjectId entId, string hyperLinkName = "ParentId")
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            Entity ent = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
            if (ent != null)
            {
                HyperLink parent = null;
                foreach (HyperLink item in ent.Hyperlinks)
                {
                    if (item.Name == hyperLinkName)
                    {
                        parent = item; break;
                    }
                }
                if (parent != null)
                {
                    ent.Hyperlinks.Remove(parent);
                    tr.Commit();
                }
            }
        }
    }

    public static ObjectId GetParentId(Entity ent, string hyperLinkName = "ParentId")
    {
        ObjectId parentId = ObjectId.Null;
        if (ent != null)
        {
            foreach (HyperLink item in ent.Hyperlinks)
            {
                if (item.Name == hyperLinkName)
                {
                    string stId = item.Description;
                    parentId = new ObjectId(new IntPtr(long.Parse(stId.Replace("(", "").Replace(")", ""))));
                    break;
                }
            }
        }
        return parentId;
    }

    public static bool IsChild(Entity ent, ObjectId parentId, string hyperLinkName = "ParentId")
    {
        foreach (HyperLink item in ent.Hyperlinks)
        {
            if (item.Name == hyperLinkName)
            {
                if (item.Description == parentId.ToString()) return true;
            }
        }
        return false;
    }

    public static void Delete(ObjectId id)
    {
        if (id != ObjectId.Null & !id.IsErased)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                id.GetObject(OpenMode.ForWrite).Erase();
                tr.Commit();
            }
        }
    }

    public static void Delete(List<ObjectId> ids)
    {
        foreach (ObjectId id in ids) Delete(id);
    }

    public static void Delete(string handles)
    {
        List<ObjectId> ids = ObjectIdAPI.GetObjectIds(handles);
        foreach (ObjectId id in ids) Delete(id);
    }

    public static ObjectId Copy(ObjectId originId, Vector3d? displacement = null)
    {
        ObjectId idReturn = ObjectId.Null;
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
            Entity originEnt = tr.GetObject(originId, OpenMode.ForRead) as Entity;
            if (originEnt != null)
            {
                Entity cloneEnt = originEnt.Clone() as Entity;
                rec.AppendEntity(cloneEnt);
                tr.AddNewlyCreatedDBObject(cloneEnt, true);
                if (displacement.HasValue) cloneEnt.TransformBy(Matrix3d.Displacement(displacement.Value));
                tr.Commit();
                idReturn = cloneEnt.Id;
            }
        }
        return idReturn;
    }
}
