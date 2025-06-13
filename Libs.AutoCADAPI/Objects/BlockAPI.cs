using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace Libs.AutoCADAPI.Objects
{
    public class BlockAPI
    {
        public static void SetTagValue(BlockReference blAtt, string tagName, string tagValue)
        {
            tagName = tagName.ToUpper();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in blAtt.AttributeCollection)
                {
                    AttributeReference att = tr.GetObject(id, OpenMode.ForWrite) as AttributeReference;
                    if (att.Tag.ToUpper() == tagName)
                    {
                        att.TextString = tagValue;
                        break;
                    }
                }
                tr.Commit();
            }
        }

        public static string GetTagValue(BlockReference blAtt, string tagName, bool hasMultiLines = false)
        {
            using (Transaction tr = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in blAtt.AttributeCollection)
                {
                    AttributeReference att = tr.GetObject(id, OpenMode.ForRead) as AttributeReference;
                    if (att.Tag.Equals(tagName))
                    {
                        return hasMultiLines ? att.MTextAttribute.Text : att.TextString;
                    }
                }
            }
            return "";
        }

        public static Dictionary<string, string> GetAllTagAndValue(BlockReference blAtt)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in blAtt.AttributeCollection)
                {
                    AttributeReference att = tr.GetObject(id, OpenMode.ForRead) as AttributeReference;
                    if (att != null) result.Add(att.Tag, att.TextString);
                }
            }
            return result;
        }

        public static bool HasBlockReference(string blockName)
        {
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable tb = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                return tb.Has(blockName);
            }
        }

        public static bool HasBlockReferences(List<string> blockNames, out List<string> notExist)
        {
            notExist = new List<string>();
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable tb = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                foreach (string blockName in blockNames)
                {
                    if (!tb.Has(blockName)) notExist.Add(blockName);
                }
            }
            return notExist.Count == 0;
        }

        public static HashSet<string> ListBlockReferences()
        {
            HashSet<string> blockNames = new HashSet<string>();
            Database db = Application.DocumentManager.MdiActiveDocument.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                // Duyệt tất cả các BlockTableRecord trong BlockTable
                foreach (ObjectId btrId in bt)
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                    // Duyệt các entity trong từng BlockTableRecord
                    foreach (ObjectId entId in btr)
                    {
                        Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                        if (ent is BlockReference blockRef)
                        {
                            // Lấy tên của Block mà BlockReference này tham chiếu
                            BlockTableRecord blockDef = (BlockTableRecord)tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead);
                            blockNames.Add(blockDef.Name);
                        }
                    }
                }
            }
            return blockNames;
        }

        public static HashSet<string> ListBlockReferences(Database db)
        {
            HashSet<string> blockNames = new HashSet<string>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                // Duyệt tất cả các BlockTableRecord trong BlockTable
                foreach (ObjectId btrId in bt)
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                    // Duyệt các entity trong từng BlockTableRecord
                    foreach (ObjectId entId in btr)
                    {
                        Entity ent = tr.GetObject(entId, OpenMode.ForRead) as Entity;
                        if (ent is BlockReference blockRef)
                        {
                            // Lấy tên của Block mà BlockReference này tham chiếu
                            BlockTableRecord blockDef = (BlockTableRecord)tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead);
                            blockNames.Add(blockDef.Name);
                        }
                    }
                }
            }
            return blockNames;
        }

        public static HashSet<string> ListBlockReferences(string dwgPath)
        {
            using (Database db = new Database(false, true))
            {
                db.ReadDwgFile(dwgPath, FileOpenMode.OpenForReadAndAllShare, true, "");
                return ListBlockReferences(db);
            }
        }

        public static string InsertBlockReference(string blockName, Point3d ptInsert, double scaleX = 1, double scaleY = 1, double? rotation = null, bool isMirror = false, string layerName = "")
        {
            string strHandle = "";
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable tb = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (tb.Has(blockName))
                {
                    BlockTableRecord recSource = tr.GetObject(tb[blockName], OpenMode.ForRead) as BlockTableRecord;
                    using (BlockReference bl = new BlockReference(ptInsert, recSource.Id))
                    {
                        bl.ScaleFactors = new Scale3d(scaleX, scaleY, 1);
                        if (rotation != null) bl.Rotation = rotation.Value;
                        if (isMirror) bl.ScaleFactors = new Scale3d(-bl.ScaleFactors.X, bl.ScaleFactors.Y, -bl.ScaleFactors.Z);

                        BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                        rec.AppendEntity(bl);
                        tr.AddNewlyCreatedDBObject(bl, true);
                        if (layerName != "") bl.Layer = layerName;
                        strHandle = bl.Handle.ToString();
                    }
                    tr.Commit();
                }
            }
            return strHandle;
        }

        public static ObjectId InsertBlockAttribute(string blockName, Point3d ptInsert, Dictionary<string, string> parameters, out string handle, double scale = 1, string layerName = "")
        {
            handle = "";
            ObjectId blockId = ObjectId.Null;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable tb = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (tb.Has(blockName))
                {
                    BlockTableRecord recSource = tr.GetObject(tb[blockName], OpenMode.ForRead) as BlockTableRecord;
                    using (BlockReference bl = new BlockReference(ptInsert, recSource.Id))
                    {
                        bl.ScaleFactors = new Scale3d(scale);
                        BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                        rec.AppendEntity(bl);
                        tr.AddNewlyCreatedDBObject(bl, true);

                        foreach (ObjectId id in recSource)
                        {
                            DBObject obj = tr.GetObject(id, OpenMode.ForWrite);
                            if (obj is AttributeDefinition att)
                            {
                                if (parameters.ContainsKey(att.Tag))
                                {
                                    using (AttributeReference attRef = new AttributeReference())
                                    {
                                        attRef.SetAttributeFromBlock(att, bl.BlockTransform);
                                        attRef.Position = att.Position.TransformBy(bl.BlockTransform);
                                        attRef.TextString = parameters[att.Tag];
                                        bl.AttributeCollection.AppendAttribute(attRef);
                                        tr.AddNewlyCreatedDBObject(attRef, true);
                                        if (layerName != "") bl.Layer = layerName;
                                    }
                                }
                            }
                        }
                        blockId = bl.ObjectId;
                        handle = bl.Handle.ToString();
                    }
                    tr.Commit();
                }
            }
            return blockId;
        }

        public static void InsertBlockDynamic(string blockName, Point3d ptInsert, Dictionary<string, object> properties, double scale = 1)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable tb = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (!tb.Has(blockName)) return;
                BlockTableRecord recSource = tr.GetObject(tb[blockName], OpenMode.ForRead) as BlockTableRecord;
                using (BlockReference bl = new BlockReference(ptInsert, recSource.Id))
                {
                    bl.ScaleFactors = new Scale3d(scale);
                    BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                    rec.AppendEntity(bl);
                    tr.AddNewlyCreatedDBObject(bl, true);

                    foreach (DynamicBlockReferenceProperty property in bl.DynamicBlockReferencePropertyCollection)
                    {
                        if (properties.ContainsKey(property.PropertyName))
                        {
                            if (property.Value is double)
                            {
                                property.Value = (double)properties[property.PropertyName];
                            }
                            else if (property.Value is int)
                            {
                                property.Value = (int)properties[property.PropertyName];
                            }
                            else property.Value = properties[property.PropertyName];
                        }
                    }
                    bl.RecordGraphicsModified(true);
                }
                tr.Commit();
            }
        }

        public static void Create(string blockName, List<Entity> entities, Point3d origin, bool deleteEntities)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable tb = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                using (BlockTableRecord rec = new BlockTableRecord())
                {
                    rec.Name = blockName;
                    rec.Origin = origin;
                    foreach (var ent in entities)
                    {
                        rec.AppendEntity(ent.Clone() as Entity);
                        if (deleteEntities) EntityAPI.Delete(ent.Id);
                    }
                    tb.Add(rec);
                    tr.AddNewlyCreatedDBObject(rec, true);
                }
                tr.Commit();
            }
        }

        public static List<ObjectId> Explode(ObjectId blockId, bool deleteBlock)
        {
            List<ObjectId> ids = new List<ObjectId>();
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                BlockReference bl = tr.GetObject(blockId, OpenMode.ForWrite) as BlockReference;
                DBObjectCollection entitys = new DBObjectCollection();
                bl.Explode(entitys);
                foreach (DBObject obj in entitys)
                {
                    rec.AppendEntity(obj as Entity);
                    tr.AddNewlyCreatedDBObject(obj, true);
                    ids.Add(obj.Id);
                }
                if (deleteBlock) bl.Erase(true);
                tr.Commit();
            }
            return ids;
        }

        public static void Scale(ObjectId blockId, double hs)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockReference bl = tr.GetObject(blockId, OpenMode.ForWrite) as BlockReference;
                var old = bl.ScaleFactors;
                bl.ScaleFactors = new Scale3d(old.X * hs, old.Y * hs, old.Z * hs);
                tr.Commit();
            }
        }

        void CreateBlockSample(string blockName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable tb = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                if (!tb.Has(blockName))
                {
                    using (BlockTableRecord rec = new BlockTableRecord())
                    {
                        rec.Name = blockName;
                        Point3d pt = new Point3d(0, 0, 0);
                        rec.Origin = pt;
                        Circle c = new Circle() { Center = pt, Radius = 1, ColorIndex = 1 };
                        rec.AppendEntity(c);
                        tb.Add(rec);
                        tr.AddNewlyCreatedDBObject(rec, true);
                    }
                    tr.Commit();
                }
            }
        }

        void CreateBlockAttSample(string blockName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable tb = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
                if (!tb.Has(blockName))
                {
                    using (BlockTableRecord rec = new BlockTableRecord())
                    {
                        rec.Name = blockName;
                        Point3d pt = new Point3d(0, 0, 0);
                        rec.Origin = pt;

                        Circle c = new Circle() { Center = pt, Radius = 1, ColorIndex = 1 };
                        rec.AppendEntity(c);
                        tb.Add(rec);
                        tr.AddNewlyCreatedDBObject(rec, true);

                        using (AttributeDefinition att = new AttributeDefinition())
                        {
                            att.Position = new Point3d(0, 0, 0);
                            att.Verifiable = true;
                            att.Tag = "TAG";
                            att.Prompt = "Prompt TAG";
                            att.ColorIndex = 256;
                            att.Height = 0.3;
                            rec.AppendEntity(att);
                        }
                    }
                    tr.Commit();
                }
            }
        }
    }
}