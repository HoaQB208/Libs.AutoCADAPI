using Autodesk.AutoCAD.DatabaseServices;

public class LayoutAPI
{
    public static ObjectId Create(string layoutName, bool makeCurrent = true)
    {
        ObjectId layoutId = LayoutManager.Current.CreateLayout(layoutName);
        if (makeCurrent) LayoutManager.Current.CurrentLayout = layoutName;
        return layoutId;
    }
}
