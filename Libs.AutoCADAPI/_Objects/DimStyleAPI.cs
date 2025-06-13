using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

public class DimStyleAPI
{
    public class DimStyleSettings
    {
        public string Name { get; set; } = "DTL";
        // Line
        public double ExtendBeyondTicks { get; set; } = 25;
        public double ExtendBeyondDimLine { get; set; } = 25;
        public double OffsetFromOrigin { get; set; } = 38;
        public short ColorDimLine { get; set; } = 5;
        public short ColorExtendLine { get; set; } = 5;
        // Sysbols
        public double Arrow_Size { get; set; } = 38;
        // Text
        public string nameTextStyle { get; set; } = "PECC5_CHUTHUONG";
        public double TextHeight { get; set; } = 55;
        public short colorText { get; set; } = 2;
        public double OffsetFromDimLine { get; set; } = 50;
        // Fit
        public double fit { get; set; } = 1;
        // Primary Units
        public double scaleFactor { get; set; } = 1;
        public int Precision { get; set; } = 0;
        public double LamTron { get; set; } = 0;
    }


    public static void MakeCurrent(string nameDimStyle)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            DimStyleTable tb = tr.GetObject(db.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;
            if (tb.Has(nameDimStyle))
            {
                DimStyleTableRecord rec = tr.GetObject(tb[nameDimStyle], OpenMode.ForRead) as DimStyleTableRecord;
                db.Dimstyle = rec.ObjectId;
                db.SetDimstyleData(rec);
            }
            tr.Commit();
        }
    }

    public static void CreateDimStyles(DimStyleSettings dimStyleSetting)
    {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Database db = doc.Database;
        using (doc.LockDocument())
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("DIMBLK", "_ARCHTICK");
            BlockTable acBlkTbl = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord acBlkTblRec = tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
            DimStyleTable acDimStyleTbl = tr.GetObject(db.DimStyleTableId, OpenMode.ForRead) as DimStyleTable;
            if (!acDimStyleTbl.Has(dimStyleSetting.Name))
            {
                if (acDimStyleTbl.IsWriteEnabled == false) acDimStyleTbl.UpgradeOpen();
                DimStyleTableRecord acDimStyleTblRec = new DimStyleTableRecord { Name = dimStyleSetting.Name };
                // Lines:
                acDimStyleTblRec.Dimdle = dimStyleSetting.ExtendBeyondTicks;   // Extend beyond ticks: 0.1
                acDimStyleTblRec.Dimdli = 30;   // Baseline spacing: 30
                acDimStyleTblRec.Dimexe = dimStyleSetting.ExtendBeyondDimLine;   // Extend beyond dim line: 0.1
                acDimStyleTblRec.Dimexo = dimStyleSetting.OffsetFromOrigin;       // Offset from origin: 0.2
                acDimStyleTblRec.Dimclrd = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByColor, dimStyleSetting.ColorDimLine);       // Color Dim line: 5
                acDimStyleTblRec.Dimclre = Autodesk.AutoCAD.Colors.Color.FromColorIndex(ColorMethod.ByColor, dimStyleSetting.ColorExtendLine);       // Color Extend line: 5                                                                                                // ' Symbols and Arrows:
                // Sysbols
                acDimStyleTblRec.Dimcen = 0.25;   // Center marks: Mark: 0.25
                acDimStyleTblRec.Dimasz = dimStyleSetting.Arrow_Size;  // Arrow size //Kích thước mũi tên: 0.05
                acDimStyleTblRec.Dimtsz = 0.05;  // Arrow size //Kích thước gạch chéo: 0.05
                // Text:
                TextStyleTable acTextStyleTable1 = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
                acDimStyleTblRec.Dimtxsty = acTextStyleTable1[dimStyleSetting.nameTextStyle];
                acDimStyleTblRec.Dimtxt = dimStyleSetting.TextHeight;  // Text heigh //Chiều cao text: 0.15
                acDimStyleTblRec.Dimtfac = 1;    // Fraction heigh scale: 1
                acDimStyleTblRec.Dimclrt = Color.FromColorIndex(ColorMethod.ByAci, dimStyleSetting.colorText);   // Màu cho text: 7
                acDimStyleTblRec.Dimtad = 1; // Bằng 0: text nằm dọc, giữa đường Dim: 1
                acDimStyleTblRec.Dimgap = dimStyleSetting.OffsetFromDimLine;   // Offset from dim line: 0.085
                acDimStyleTblRec.Dimfrac = 0;
                acDimStyleTblRec.Dimtmove = 2;
                acDimStyleTblRec.Dimtih = false;     // False: DimY Text đứng: False
                // Fit:
                acDimStyleTblRec.Dimatfit = 0;   //Chọn 0 //0:Places both text and arrows outside extension lines; 1:Moves arrows first, then text; 2:Moves text first, then arrows; 3:Moves either text or arrows, whichever fits best
                acDimStyleTblRec.Dimscale = dimStyleSetting.fit; // Use overall scale of:
                acDimStyleTblRec.Dimtofl = true; // Draw dim line between ext lines: true
                acDimStyleTblRec.Dimtix = true;  // Always keep text between ext line: true
                // Primary Units
                acDimStyleTblRec.Dimdec = dimStyleSetting.Precision;     // Precision: 0
                acDimStyleTblRec.Dimrnd = dimStyleSetting.LamTron;   // Round off: 10
                acDimStyleTblRec.Dimlfac = dimStyleSetting.scaleFactor;    // Scale factor: 1000
                // Altermate Units
                acDimStyleTblRec.Dimaltf = 25.4;      // Multiplier for alt units: 25.4
                acDimStyleTblRec.Dimaltrnd = 0.0;    // Round distances to: 0
                // Tolerances
                acDimStyleTblRec.Dimtp = 0.0;    // Upper value: 0
                acDimStyleTblRec.Dimtm = 0.0;    // Lower value: 0
                acDimStyleTbl.Add(acDimStyleTblRec);
                tr.AddNewlyCreatedDBObject(acDimStyleTblRec, true);
                tr.Commit();
            }
        }
    }

}
