using Autodesk.AutoCAD.ApplicationServices;

namespace BIMZone.AutoCAD_API._Objects
{
    public class EditorAPI
    {
        public static void WriteMessage(string msg)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\n{msg}");
        }
    }
}