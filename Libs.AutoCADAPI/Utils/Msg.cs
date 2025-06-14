using System.Windows;

namespace Libs.AutoCADAPI.Utils
{
    public class Msg
    {
        public static void Info(object content)
        {
            MessageBox.Show(content.ToString());
        }

        public static void Warning(object content)
        {
            MessageBox.Show(content.ToString(), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void Error(object content)
        {
            MessageBox.Show(content.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}