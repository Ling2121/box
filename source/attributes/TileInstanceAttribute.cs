using System;

namespace Box {
    public class TileInstanceAttribute : Attribute
    {
        public string TileName;
        public bool IsAddToScene;

        public TileInstanceAttribute(string tile_name,bool is_add_to_scene)
        {
            TileName = tile_name;
            IsAddToScene = is_add_to_scene;
        }
    }
}