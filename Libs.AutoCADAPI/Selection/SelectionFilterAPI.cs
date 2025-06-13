using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Libs.AutoCADAPI.Utils;
using System.Collections.Generic;

namespace Libs.AutoCADAPI.Selection
{
    public class SelectionFilterAPI
    {
        public static SelectionFilter ByObjectType(ObjectType objectType)
        {
            string description = EnumUtils.GetDescription(objectType);
            return new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.Start, description) });
        }

        public static SelectionFilter ByObjectTypes(List<ObjectType> objectTypes)
        {
            List<string> descriptions = new List<string>();
            foreach (ObjectType objectType in objectTypes)
            {
                descriptions.Add(EnumUtils.GetDescription(objectType));
            }
            return new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.Start, string.Join(",", descriptions)) });
        }


        public static SelectionFilter ByLayer(string layerName)
        {
            return new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.LayerName, layerName) });
        }

        public static SelectionFilter ByTypeAndLayer(ObjectType objectType, string layerName)
        {
            string description = EnumUtils.GetDescription(objectType);
            return new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.Start, description), new TypedValue((int)DxfCode.LayerName, layerName) });
        }

        public static SelectionFilter ByBlock(string blockName)
        {
            return new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.BlockName, blockName) });
        }

        public static SelectionFilter ByXData(string regAppName)
        {
            return new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.ExtendedDataRegAppName, regAppName) });
        }
    }
}