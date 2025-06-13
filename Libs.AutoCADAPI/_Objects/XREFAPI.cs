using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;

namespace BIMZone.AutoCAD_API._Objects
{
    public class XREFAPI
    {
        public static List<string> GetXrefsFromDwg(string dwgPath)
        {
            List<string> xrefList = new List<string>();
            try
            {
                using (Database db = new Database(false, true))
                {
                    db.ReadDwgFile(dwgPath, FileOpenMode.OpenForReadAndAllShare, true, "");
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                        foreach (ObjectId btrId in bt)
                        {
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);

                            if (btr.IsFromExternalReference)
                            {
                                xrefList.Add(btr.Name);
                            }
                        }
                        tr.Commit();
                    }
                }
            }
            catch (Exception ex) { Msg.Show(ex); }
            return xrefList;
        }
    }
}