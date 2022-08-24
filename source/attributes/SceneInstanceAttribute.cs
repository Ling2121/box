using System;

namespace Box {
    public class SceneInstanceAttribute : Attribute
    {
        public string ScenePath;

        public SceneInstanceAttribute(string path)
        {
            ScenePath = path;
        }
    }
}