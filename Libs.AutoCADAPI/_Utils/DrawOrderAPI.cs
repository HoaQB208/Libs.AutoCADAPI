using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

public class DrawOrderAPI
{
    static public void BringToFront(ObjectIdCollection ids)
    {
        Execute(ids, DrawOrderType.BringToFront, ObjectId.Null);
    }

    static public void SendToBack(ObjectIdCollection ids)
    {
        Execute(ids, DrawOrderType.SendToBack, ObjectId.Null);
    }

    static public void SendToUnder(ObjectIdCollection ids, ObjectId target)
    {
        if (target != ObjectId.Null) Execute(ids, DrawOrderType.SendToBelow, target);
    }

    static public void BringToAbove(ObjectIdCollection ids, ObjectId target)
    {
        if (target != ObjectId.Null) Execute(ids, DrawOrderType.BringToAbove, target);
    }

    private static void Execute(ObjectIdCollection ids, DrawOrderType drawOrderType, ObjectId target)
    {
        ObjectIdCollection notNull = new ObjectIdCollection();
        foreach (ObjectId id in ids)
        {
            if (id != ObjectId.Null) notNull.Add(id);
        }
        if (notNull.Count == 0) return;

        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            BlockTableRecord block = tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead) as BlockTableRecord;
            DrawOrderTable drawOrder = tr.GetObject(block.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;
            switch (drawOrderType)
            {
                case DrawOrderType.BringToFront:
                    drawOrder.MoveToTop(ids);
                    break;
                case DrawOrderType.SendToBack:
                    drawOrder.MoveToBottom(ids);
                    break;
                case DrawOrderType.SendToBelow:
                    drawOrder.MoveBelow(ids, target);
                    break;
                case DrawOrderType.BringToAbove:
                    drawOrder.MoveAbove(ids, target);
                    break;
            }
            tr.Commit();
        }
    }

    enum DrawOrderType
    {
        BringToFront,
        SendToBack,
        SendToBelow,
        BringToAbove
    }
}
