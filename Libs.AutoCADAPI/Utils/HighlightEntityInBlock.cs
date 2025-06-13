using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using System;

public class HighlightEntityInBlock : IDisposable
{
    private Entity entClone = null;

    public HighlightEntityInBlock(ObjectId id, Matrix3d transform, int colorIndex = 4)
    {
        ClearHighlight();
        using (Transaction tr = id.Database.TransactionManager.StartTransaction())
        {
            Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
            entClone = ent.Clone() as Entity;
            entClone.ColorIndex = colorIndex;
            entClone.TransformBy(transform);
            tr.Commit();
        }
        TransientManager.CurrentTransientManager.AddTransient(entClone, TransientDrawingMode.Highlight, 128, new IntegerCollection());
    }

    public void Dispose()
    {
        ClearHighlight();
    }

    private void ClearHighlight()
    {
        if (entClone != null)
        {
            TransientManager.CurrentTransientManager.EraseTransient(entClone, new IntegerCollection());
            entClone.Dispose();
            entClone = null;
        }
    }
}

