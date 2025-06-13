using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

public class InputAPI
{
    public static string GetString(string msg = "Nhập vào chuỗi")
    {
        PromptResult result = Application.DocumentManager.MdiActiveDocument.Editor.GetString(new PromptStringOptions("\n" + msg));
        if (result.Status != PromptStatus.OK) return null;
        return result.StringResult;
    }

    public static int? GetInteger(string msg = "Nhập vào số nguyên")
    {
        PromptIntegerResult result = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(new PromptIntegerOptions("\n" + msg));
        if (result.Status != PromptStatus.OK) return null;
        return result.Value;
    }
    public static double? GetDouble(string msg = "Nhập vào số thực")
    {
        PromptDoubleResult result = Application.DocumentManager.MdiActiveDocument.Editor.GetDouble(new PromptDoubleOptions("\n" + msg));
        if (result.Status != PromptStatus.OK) return null;
        return result.Value;
    }

    public static double? GetDistance(string msg = "Chọn khoảng cách")
    {
        PromptDoubleResult result = Application.DocumentManager.MdiActiveDocument.Editor.GetDistance(new PromptDistanceOptions("\n" + msg));
        if (result.Status != PromptStatus.OK) return null;
        return result.Value;
    }

    public static double? GetAngle(string msg = "Chọn góc")
    {
        PromptDoubleResult result = Application.DocumentManager.MdiActiveDocument.Editor.GetAngle(new PromptAngleOptions("\n" + msg));
        if (result.Status != PromptStatus.OK) return null;
        return result.Value;
    }

    public static string GetKeyword(string msg = "Nhấn phím", bool allowNone = true)
    {
        var result = Application.DocumentManager.MdiActiveDocument.Editor.GetKeywords(new PromptKeywordOptions("\n" + msg) { AllowNone = allowNone });
        if (result.Status != PromptStatus.OK) return null;
        return result.StringResult;
    }
}
