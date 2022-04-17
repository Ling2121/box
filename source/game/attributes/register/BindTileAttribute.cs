using System;

namespace Box {
    public class BindTileAttribute : Attribute
    {
        public string TileName;

        public BindTileAttribute(string tile)
        {
            TileName = tile;
        }
    }
}