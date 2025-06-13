using System.ComponentModel;

public enum SelectionType
{
    GetSelection,
    InsidePolygon,
    CrossingPolygon,
    InsideWindow,
    CrossingWindow,
    SelectAtPoint,
    All
}

public enum ObjectType
{
    [Description("LINE")]
    Line,

    [Description("POLYLINE,LWPOLYLINE")]
    Polyline,

    [Description("INSERT")]
    Block,

    [Description("TEXT")]
    DText,

    [Description("MTEXT")]
    MText
}