using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace Libs.AutoCADAPI.Objects
{
    public class DocumentAPI
    {
        public static void Regen()
        {
            Application.DocumentManager.MdiActiveDocument.Editor.Regen();
        }

        public static void SendStringToExecute(string command)
        {
            Application.DocumentManager.MdiActiveDocument.SendStringToExecute(command + " ", true, false, true);
        }

        public static Document GetDocument()
        {
            return Application.DocumentManager.MdiActiveDocument;
        }

        public static Database GetDatabase()
        {
            return Application.DocumentManager.MdiActiveDocument.Database;
        }

        public static Transaction GetTransaction()
        {
            return Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction();
        }
    }
}
