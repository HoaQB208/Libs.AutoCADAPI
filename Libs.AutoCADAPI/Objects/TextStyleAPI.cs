using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace Libs.AutoCADAPI.Objects
{
    public class TextStyleAPI
    {
        public static void Create(string textStyleName, string fontName, bool isSHX, double textSize = 0, double xScale = 1)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable tb = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                if (!tb.Has(textStyleName))
                {
                    TextStyleTableRecord textStyle = new TextStyleTableRecord()
                    {
                        Name = textStyleName,
                        ObliquingAngle = 0,
                        XScale = xScale,
                        TextSize = textSize,
                        IsVertical = false,
                        IsShapeFile = false
                    };
                    if (isSHX)
                    {
                        textStyle.FileName = fontName;
                        textStyle.BigFontFileName = default;
                        textStyle.Font = default;
                    }
                    else textStyle.Font = new Autodesk.AutoCAD.GraphicsInterface.FontDescriptor(fontName, false, false, default, default);
                    tb.UpgradeOpen();
                    tb.Add(textStyle);
                    tr.AddNewlyCreatedDBObject(textStyle, true);
                    tr.Commit();
                }
            }
        }
    }
}
