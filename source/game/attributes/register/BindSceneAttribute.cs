using System;

namespace Box {
    public class BindSceneAttribute : Attribute
    {
        public string ScenePath;

        public BindSceneAttribute(string path)
        {
            ScenePath = path;
        }
    }
}