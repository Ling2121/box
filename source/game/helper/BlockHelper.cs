using System;
using Godot;
using System.Reflection;

namespace Box {
    public static class BlockHelper {
        public static BindCellAttribute GetCellBindInfo(Node block) {
            Type type = block.GetType();
            return type.GetCustomAttribute<BindCellAttribute>();
        }
    }
}