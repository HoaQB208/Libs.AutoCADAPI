using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System.Collections.Generic;

public class ImportAPI
{
    public static string Blocks(string pathSourceDwg, List<string> blockNames)
    {
        string error = "";
        try
        {
            Database sourceDb = new Database(false, true);
            sourceDb.ReadDwgFile(pathSourceDwg, FileOpenMode.OpenTryForReadShare, true, "");
            ObjectIdCollection blockIds = new ObjectIdCollection();
            using (Transaction tr = sourceDb.TransactionManager.StartTransaction())
            {
                BlockTable tb = tr.GetObject(sourceDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                foreach (string blockName in blockNames)
                {
                    if (tb.Has(blockName))
                    {
                        BlockTableRecord rec = tr.GetObject(tb[blockName], OpenMode.ForRead) as BlockTableRecord;
                        blockIds.Add(rec.ObjectId);
                    }
                }
            }
            Database currentDb = Application.DocumentManager.MdiActiveDocument.Database;
            currentDb.WblockCloneObjects(blockIds, currentDb.CurrentSpaceId, new IdMapping(), DuplicateRecordCloning.Replace, false);
            sourceDb.Dispose();
        }
        catch (System.Exception ex)
        {
            error = ex.Message;
        }
        return error;
    }
}