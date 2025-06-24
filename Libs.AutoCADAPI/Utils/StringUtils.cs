namespace Libs.AutoCADAPI.Utils
{
    public class StringUtils
    {
        public static string RemoveLines(string str)
        {
            if (str == null) return "";
            return str.Replace("\r\n", "\n").Replace("\n", " ").Replace("  ", " ").Trim();
        }
    }
}
