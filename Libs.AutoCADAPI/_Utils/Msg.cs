public class Msg
{
    public static void Show(object obj)
    {
        System.Windows.MessageBox.Show(obj.ToString());
    }

    public static void Error(object obj)
    {
        System.Windows.MessageBox.Show(obj.ToString(), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
    }

    public static void Warning(object obj)
    {
        System.Windows.MessageBox.Show(obj.ToString(), "Warning", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
    }
}