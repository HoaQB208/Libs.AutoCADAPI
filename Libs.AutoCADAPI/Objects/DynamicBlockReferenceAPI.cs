using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Libs.AutoCADAPI.Objects
{
    public class DynamicBlockReferenceAPI
    {
        public static void Insert(string blockName, Point3d ptInsert, Dictionary<string, object> properties, double scale = 1, double rotate = 0)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (doc.LockDocument())
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable tb = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (!tb.Has(blockName)) return;
                BlockTableRecord recSource = tr.GetObject(tb[blockName], OpenMode.ForRead) as BlockTableRecord;
                using (BlockReference bl = new BlockReference(ptInsert, recSource.Id))
                {
                    bl.ScaleFactors = new Scale3d(scale);
                    bl.Rotation = rotate;
                    BlockTableRecord rec = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;
                    rec.AppendEntity(bl);
                    tr.AddNewlyCreatedDBObject(bl, true);

                    foreach (DynamicBlockReferenceProperty property in bl.DynamicBlockReferencePropertyCollection)
                    {
                        if (!properties.ContainsKey(property.PropertyName)) continue;

                        object val = properties[property.PropertyName];
                        if (property.PropertyTypeCode == 3)
                        {
                            var allowedValues = property.GetAllowedValues();
                            if (allowedValues != null && allowedValues.Count() > 0)
                            {
                                var match = allowedValues.Cast<object>().FirstOrDefault(v => string.Equals(v.ToString().Trim(), val.ToString().Trim(), StringComparison.OrdinalIgnoreCase));
                                if (match != null) property.Value = match;
                            }
                        }
                        else
                        {
                            if (property.Value is double)
                            {
                                if (double.TryParse(val?.ToString(), out double numb))
                                {
                                    property.Value = numb;
                                }
                            }
                            else if (property.Value is int)
                            {
                                if (int.TryParse(val?.ToString(), out int numb))
                                {
                                    property.Value = numb;
                                }
                            }
                            else property.Value = val;
                        }
                    }
                    bl.RecordGraphicsModified(true);
                }
                tr.Commit();
            }
        }

        public static string GetPropertyValue(BlockReference bl, string propertyName)
        {
            using (Transaction tr = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                DynamicBlockReferencePropertyCollection props = bl.DynamicBlockReferencePropertyCollection;
                foreach (DynamicBlockReferenceProperty prop in props)
                {
                    if (prop.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return prop.Value.ToString();
                    }
                }
            }
            return "";
        }

        public static string GetLookupParameterValue(BlockReference bl, string paraName)
        {
            using (Transaction tr = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    // Lấy COM document
                    object acadApp = Application.AcadApplication;
                    object acadDoc = acadApp.GetType().InvokeMember("ActiveDocument", BindingFlags.GetProperty, null, acadApp, null);
                    // Lấy đối tượng COM qua Handle
                    string handle = bl.Handle.ToString();
                    object comBlock = acadDoc.GetType().InvokeMember("HandleToObject", BindingFlags.InvokeMethod, null, acadDoc, new object[] { handle });
                    // Gọi GetDynamicBlockProperties
                    object propsObj = comBlock.GetType().InvokeMember("GetDynamicBlockProperties", BindingFlags.InvokeMethod, null, comBlock, null);
                    // Ép về mảng object
                    IEnumerable props = propsObj as IEnumerable;
                    foreach (object prop in props)
                    {
                        string name = prop.GetType().InvokeMember("PropertyName", BindingFlags.GetProperty, null, prop, null).ToString();
                        if (name == paraName)
                        {
                            return prop.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, prop, null).ToString();
                        }
                    }
                }
                catch { }
            }
            return "";
        }

        public static List<object> GetLookupParameterAllowedValues(BlockReference bl, string paraName)
        {
            List<object> objs = new List<object>();
            using (Transaction tr = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    // Lấy COM document
                    object acadApp = Application.AcadApplication;
                    object acadDoc = acadApp.GetType().InvokeMember("ActiveDocument", BindingFlags.GetProperty, null, acadApp, null);
                    // Lấy đối tượng COM qua Handle
                    string handle = bl.Handle.ToString();
                    object comBlock = acadDoc.GetType().InvokeMember("HandleToObject", BindingFlags.InvokeMethod, null, acadDoc, new object[] { handle });
                    // Gọi GetDynamicBlockProperties
                    object propsObj = comBlock.GetType().InvokeMember("GetDynamicBlockProperties", BindingFlags.InvokeMethod, null, comBlock, null);
                    // Ép về mảng object
                    IEnumerable props = propsObj as IEnumerable;
                    foreach (object prop in props)
                    {
                        string name = prop.GetType().InvokeMember("PropertyName", BindingFlags.GetProperty, null, prop, null).ToString();
                        if (name == paraName)
                        {
                            object allowedValues = prop.GetType().InvokeMember("AllowedValues", BindingFlags.GetProperty, null, prop, null);
                            if (allowedValues is IEnumerable values)
                            {
                                foreach (var val in values)
                                {
                                    objs.Add(val);
                                }
                            }
                        }
                    }
                }
                catch { }
            }
            return objs;
        }

        public static bool SetLookupParameterValue(BlockReference bl, string paraName, object newValue)
        {
            using (Transaction tr = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                try
                {
                    // Lấy COM document
                    object acadApp = Application.AcadApplication;
                    object acadDoc = acadApp.GetType().InvokeMember("ActiveDocument", BindingFlags.GetProperty, null, acadApp, null);
                    // Lấy đối tượng COM qua Handle
                    string handle = bl.Handle.ToString();
                    object comBlock = acadDoc.GetType().InvokeMember("HandleToObject", BindingFlags.InvokeMethod, null, acadDoc, new object[] { handle });
                    // Gọi GetDynamicBlockProperties
                    object propsObj = comBlock.GetType().InvokeMember("GetDynamicBlockProperties", BindingFlags.InvokeMethod, null, comBlock, null);
                    // Ép về mảng object
                    IEnumerable props = propsObj as IEnumerable;
                    foreach (object prop in props)
                    {
                        string name = prop.GetType().InvokeMember("PropertyName", BindingFlags.GetProperty, null, prop, null).ToString();
                        if (name == paraName)
                        {
                            prop.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, prop, new object[] { newValue });
                            tr.Commit();
                            return true;
                        }
                    }
                }
                catch { }
            }
            return false;
        }
    }
}
