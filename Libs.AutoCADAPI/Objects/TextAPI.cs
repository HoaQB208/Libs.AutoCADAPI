using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Libs.AutoCADAPI.Objects
{
    public class TextAPI
    {
        public static (ObjectId, string) CreateDText(string text, Point3d position, double heigh, string textStyle = "", int color = 256, TextHorizontalMode horizontal = TextHorizontalMode.TextLeft, double rotation = 0, string layerName = null)
        {
            string strHand = "";
            ObjectId id = ObjectId.Null;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                DBText dbtext = new DBText();
                dbtext.SetDatabaseDefaults();
                TextStyleTable textStyleTb = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                if (textStyleTb.Has(textStyle)) dbtext.TextStyleId = textStyleTb[textStyle];
                dbtext.TextString = text;
                dbtext.Height = heigh;
                dbtext.ColorIndex = color;
                dbtext.Position = position;
                dbtext.Rotation = rotation;
                dbtext.HorizontalMode = horizontal;
                dbtext.VerticalMode = TextVerticalMode.TextVerticalMid;
                dbtext.AlignmentPoint = position;
                if (layerName != null) try { dbtext.Layer = layerName; } catch { }
                rec.AppendEntity(dbtext);
                tr.AddNewlyCreatedDBObject(dbtext, true);
                tr.Commit();
                id = dbtext.Id;
                strHand = dbtext.Handle.ToString();
            }
            return (id, strHand);
        }

        public static string CreateMText(string content, Point3d position, double textHeight, string textStyle = "", string layer = null, int color = 256, double rotation = 0, AttachmentPoint attachment = AttachmentPoint.TopLeft)
        {
            string handle = "";

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                MText mText = new MText();
                mText.SetDatabaseDefaults();
                if (textStyle != "")
                {
                    TextStyleTable textStyleTb = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                    if (textStyleTb.Has(textStyle)) mText.TextStyleId = textStyleTb[textStyle];
                }
                if (layer != null) try { mText.Layer = layer; } catch { }
                mText.Contents = content;
                mText.TextHeight = textHeight;
                mText.Attachment = attachment;
                mText.ColorIndex = color;
                mText.Location = position;
                mText.Rotation = rotation;
                rec.AppendEntity(mText);
                tr.AddNewlyCreatedDBObject(mText, true);
                tr.Commit();

                handle = mText.Handle.ToString();
            }

            return handle;
        }

    }
}
