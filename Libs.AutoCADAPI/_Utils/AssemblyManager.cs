using System.IO;
using System.Reflection;

namespace BIMZone.AutoCAD_API._Utils
{
    public class AssemblyManager
    {
        public static void Load(string assemblyPath)
        {
            if (File.Exists(assemblyPath))
            {
                try
                {
                    AssemblyName assembly = AssemblyName.GetAssemblyName(assemblyPath);
                    Assembly.Load(assembly);
                }
                catch (System.Exception ex)
                {
                    Msg.Error(ex.Message);
                }
            }
        }
    }
}