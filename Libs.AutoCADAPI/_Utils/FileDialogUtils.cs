using Microsoft.Win32;

namespace BIMZone.AutoCAD_API._Utils
{
    public class FileDialogUtils
    {
        /// <summary>
        /// "Data Setting(*.json)|*.json"
        ///  DefaultExt = ".json",
        /// </summary>
        public static string SaveFile(string filter = "", string defaultExt = "")
        {
            string selectedPath = null;

            SaveFileDialog dialog = new SaveFileDialog
            {
                RestoreDirectory = true,
                DefaultExt = defaultExt,
                Filter = filter
            };

            bool? rs = dialog.ShowDialog();
            if (rs.HasValue) if (rs.Value) selectedPath = dialog.FileName;

            return selectedPath;
        }

        public static string OpenFile(string filters, string defaultExt = "")
        {
            string selectedPath = null;

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = filters,
                DefaultExt = defaultExt,
                CheckFileExists = true,
                RestoreDirectory = true
            };

            bool? rs = dialog.ShowDialog();
            if (rs.HasValue) if (rs.Value) selectedPath = dialog.FileName;

            return selectedPath;
        }
    }
}