using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Libs.AutoCADAPI.Objects
{
    public class ObjectIdAPI
    {
        private const string separator = ";";

        public static ObjectId GetObjectId(string handle)
        {
            ObjectId id = ObjectId.Null;
            if (!string.IsNullOrEmpty(handle))
            {
                try
                {
                    long lg = Int64.Parse(handle, NumberStyles.AllowHexSpecifier);
                    Handle hand = new Handle(lg);
                    return Application.DocumentManager.MdiActiveDocument.Database.GetObjectId(false, hand, 0);
                }
                catch { }
            }
            return id;
        }

        public static List<ObjectId> GetObjectIds(string handles)
        {
            List<ObjectId> ids = new List<ObjectId>();
            string[] arr = handles.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string handle in arr)
            {
                ObjectId id = GetObjectId(handle);
                if (id != ObjectId.Null) ids.Add(id);
            }
            return ids;
        }

        public static string JoinHandles(List<string> handles)
        {
            return string.Join(separator, handles.Where(x => x != ""));
        }

        public static ObjectIdCollection ToObjectIdCollection(List<ObjectId> ids)
        {
            ObjectIdCollection collection = new ObjectIdCollection();
            foreach (ObjectId id in ids) collection.Add(id);
            return collection;
        }
    }
}
