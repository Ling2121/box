using System;
using System.Collections.Generic;

namespace Box {
    public enum RegisterType {
        Block,
        Item,
        Biome,
    }

    public class Register : Singleton<Register> {
        Dictionary<string,Type> items;
        Dictionary<string,Type> blocks;
        Dictionary<string,IBiome> biomes;
    }
}