using Autodesk.AutoCAD.ApplicationServices;

namespace Libs.AutoCADAPI.Objects
{
    public class EditorAPI
    {
        public static void WriteMessage(string msg)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\n{msg}");
        }
    }
}