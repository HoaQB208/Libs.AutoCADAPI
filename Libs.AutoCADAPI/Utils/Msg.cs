namespace Libs.AutoCADAPI.Utils
{
    public class Msg
    {
        public static void Show(object obj)
        {
            string msg = obj is null ? "Null" : obj.ToString();
            System.Windows.MessageBox.Show(msg);
        }
    }
}
