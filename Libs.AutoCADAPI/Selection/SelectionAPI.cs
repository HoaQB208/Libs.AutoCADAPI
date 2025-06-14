using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Libs.AutoCADAPI.Objects;
using System;
using System.Collections.Generic;

namespace Libs.AutoCADAPI.Selection
{
    public class SelectionAPI
    {
        public static List<Entity> Entities(SelectionType selectionType, SelectionFilter filter = null, string msg = "Select object", Point3dCollection polygon = null, Point3d? pWindow1 = null, Point3d? pWindow2 = null, Point3d? selectAtPoint = null, double toleranceFromPoint = 0.1)
        {
            List<ObjectId> ids = ObjectIds(selectionType, filter, msg, polygon, pWindow1, pWindow2, selectAtPoint, toleranceFromPoint);
            return EntityAPI.GetEntities(ids);
        }

        public static List<ObjectId> ObjectIds(SelectionType selectionType, SelectionFilter filter = null, string msg = "Select object", Point3dCollection polygon = null, Point3d? pWindow1 = null, Point3d? pWindow2 = null, Point3d? selectAtPoint = null, double toleranceFromPoint = 0.1)
        {
            SelectionSet set = GetSelectionSet(selectionType, filter, msg, polygon, pWindow1, pWindow2, selectAtPoint, toleranceFromPoint);
            return ObjectIds(set);
        }

        public static Entity Entity(string msg = "Pick object", List<Type> allowedClasss = null)
        {
            Entity entity = null;
            ObjectId id = PickObject(msg, allowedClasss);
            if (id != ObjectId.Null)
            {
                entity = EntityAPI.GetEntity(id);
            }
            return entity;
        }

        public static List<ObjectId> ObjectIds(SelectionSet set)
        {
            List<ObjectId> ids = new List<ObjectId>();
            if (set != null)
            {
                foreach (SelectedObject obj in set)
                {
                    ids.Add(obj.ObjectId);
                }
            }
            return ids;
        }

        public static ObjectId PickObject(string msg = "Pick object", List<Type> allowedClasss = null)
        {
            PromptEntityOptions options = new PromptEntityOptions("\n" + msg);
            options.SetRejectMessage("\n" + msg);
            if (allowedClasss != null)
            {
                foreach (var allowedClass in allowedClasss) options.AddAllowedClass(allowedClass, true);
            }

            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            PromptEntityResult res = editor.GetEntity(options);
            if (res.Status == PromptStatus.OK) return res.ObjectId;
            else return ObjectId.Null;
        }

        public static Point3d? PickPoint(string msg = "Click chọn điểm")
        {
            PromptPointResult res = Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(new PromptPointOptions("\n" + msg));
            if (res.Status == PromptStatus.OK) return res.Value;
            return null;
        }

        public static ObjectId EntityInBlock(out Matrix3d transform, string msg = "Pick object in block")
        {
            ObjectId id = ObjectId.Null;
            transform = Matrix3d.Identity;

            Document doc = Application.DocumentManager.MdiActiveDocument;
            using (Transaction tr = doc.TransactionManager.StartOpenCloseTransaction())
            {
                PromptNestedEntityResult res = doc.Editor.GetNestedEntity("\n" + msg);
                if (res.Status == PromptStatus.OK)
                {
                    id = res.ObjectId;
                    transform = res.Transform;
                }
            }

            return id;
        }

        private static SelectionSet GetSelectionSet(SelectionType selectionType, SelectionFilter filter, string msg, Point3dCollection polygon, Point3d? pWindow1, Point3d? pWindow2, Point3d? selectAtPoint, double toleranceFromPoint = 0.1)
        {
            Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
            SelectionSet selectionSet = null;
            switch (selectionType)
            {
                case SelectionType.GetSelection:
                    PromptSelectionOptions opts = new PromptSelectionOptions { MessageForAdding = "\n" + msg };
                    selectionSet = editor.GetSelection(opts, filter).Value;
                    break;
                case SelectionType.InsidePolygon:
                    if (polygon != null) selectionSet = editor.SelectWindowPolygon(polygon, filter).Value;
                    break;
                case SelectionType.CrossingPolygon:
                    if (polygon != null) selectionSet = editor.SelectCrossingPolygon(polygon, filter).Value;
                    break;
                case SelectionType.InsideWindow:
                    if (pWindow1 != null & pWindow2 != null) selectionSet = editor.SelectWindow(pWindow1.Value, pWindow2.Value, filter).Value;
                    break;
                case SelectionType.CrossingWindow:
                    if (pWindow1 != null & pWindow2 != null) selectionSet = editor.SelectCrossingWindow(pWindow1.Value, pWindow2.Value, filter).Value;
                    break;
                case SelectionType.SelectAtPoint:
                    if (selectAtPoint != null)
                    {
                        Point3dCollection collection = PointAPI.Rectangle(selectAtPoint.Value, toleranceFromPoint, toleranceFromPoint);
                        selectionSet = editor.SelectCrossingPolygon(collection, filter).Value;
                    }
                    break;
                case SelectionType.All:
                    selectionSet = editor.SelectAll(filter).Value;
                    break;
            }
            return selectionSet;
        }

        public static List<Point3d> GetPointsFromEntities(List<Entity> entities)
        {
            List<Point3d> points = new List<Point3d>();

            foreach (Entity ent in entities)
            {
                if (ent is BlockReference bl) points.Add(bl.Position);
                else if (ent is Polyline pl)
                {
                    for (int i = 0; i < pl.NumberOfVertices; i++) points.Add(pl.GetPoint3dAt(i));
                }
            }
            return points;
        }

        public static List<BlockReference> GetBlockRefByNames(List<string> blockNames, string msg = "Quét chọn các block")
        {
            var blockIds = ObjectIds(SelectionType.GetSelection, SelectionFilterAPI.ByObjectTypes(new List<ObjectType>() { ObjectType.Block }), msg: msg);
            List<BlockReference> blocks = new List<BlockReference>();
            using (var tr = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                foreach (var id in blockIds)
                {
                    var bl = tr.GetObject(id, OpenMode.ForRead) as BlockReference;
                    string blockName = BlockAPI.GetBlockName(bl);
                    if (blockNames.Contains(blockName))
                    {
                        blocks.Add(bl);
                    }
                }
            }
            return blocks;
        }

        public static List<BlockReference> GetAllBlockRefByName(string blockName)
        {
            var blockIds = ObjectIds(SelectionType.All, SelectionFilterAPI.ByObjectTypes(new List<ObjectType>() { ObjectType.Block }));
            List<BlockReference> blocks = new List<BlockReference>();
            using (var tr = Application.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
            {
                foreach (var id in blockIds)
                {
                    var bl = tr.GetObject(id, OpenMode.ForRead) as BlockReference;
                    string blName = BlockAPI.GetBlockName(bl);
                    if (blName == blockName)
                    {
                        blocks.Add(bl);
                    }
                }
            }
            return blocks;
        }
    }
}
