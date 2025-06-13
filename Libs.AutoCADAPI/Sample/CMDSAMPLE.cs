using Autodesk.AutoCAD.Runtime;

namespace AutoCAD_API.Samples
{
    public class CMDSAMPLE // Must be 'public'
    {
        public const string Cmd = nameof(CMDSAMPLE); // The const string, currently get from class name

        [CommandMethod(Cmd)] // The command is called
        public void Execute()
        {
            // Todo something here
            System.Windows.MessageBox.Show(Cmd);
            // 
        }
    }
}