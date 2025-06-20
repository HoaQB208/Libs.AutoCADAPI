using Autodesk.AutoCAD.ApplicationServices;

namespace Libs.AutoCADAPI.Objects
{
    public class EditorAPI
    {
        public static void WriteMessage(string msg, bool isForce = false)
        {
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"\n{msg}");
            if(isForce) System.Windows.Forms.Application.DoEvents();
        }
    }
}