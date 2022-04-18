using System;

namespace Box {
    public class BindCellAttribute : Attribute
    {
        public string CellName;
        public bool IsAddToScene;

        public BindCellAttribute(string cell,bool is_add_to_scene)
        {
            CellName = cell;
            IsAddToScene = is_add_to_scene;
        }
    }
}