using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

public class LayerAPI
{
    public static void Create(string layerName, short color = 1, string lineTypeName = "Continuous", LineWeight lineWeight = LineWeight.ByLineWeightDefault, bool canPrint = true)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            LayerTable tb = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
            if (!tb.Has(layerName))
            {
                LayerTableRecord rec = new LayerTableRecord
                {
                    Name = layerName,
                    Color = Color.FromColorIndex(ColorMethod.ByAci, color),
                    LinetypeObjectId = LineTypeAPI.GetLineTypeId(lineTypeName),
                    LineWeight = lineWeight,
                    IsPlottable = canPrint,

                };
                tb.UpgradeOpen();
                tb.Add(rec);
                tr.AddNewlyCreatedDBObject(rec, true);
                tr.Commit();
            }
        }
    }

    public static void SetCurrentLayer(string layerName)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            LayerTable tb = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
            if (tb.Has(layerName))
            {
                db.Clayer = tb[layerName];
                tr.Commit();
            }
        }
    }

    public static string GetCurrentLayerName()
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            LayerTableRecord ltr = tr.GetObject(db.Clayer, OpenMode.ForRead) as LayerTableRecord;
            return ltr.Name;
        }
    }

    public static void SetLayer(ObjectId id, string layerName)
    {
        if (id == ObjectId.Null) return;
        try
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Entity ent = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                ent.Layer = layerName;
                tr.Commit();
            }
        }
        catch { }
    }

    public static List<string> GetAllLayerNames()
    {
        List<string> layerNames = new List<string>();
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            LayerTable tb = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
            foreach (ObjectId id in tb)
            {
                LayerTableRecord rec = id.GetObject(OpenMode.ForRead) as LayerTableRecord;
                layerNames.Add(rec.Name);
            }
        }
        return layerNames;
    }

    public static LayerTableRecord GetLayer(string layerName)
    {
        Database db = Application.DocumentManager.MdiActiveDocument.Database;
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            LayerTable tb = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
            foreach (ObjectId id in tb)
            {
                LayerTableRecord rec = id.GetObject(OpenMode.ForRead) as LayerTableRecord;
                if (rec.Name == layerName) return rec;
            }
        }
        return null;
    }
}
