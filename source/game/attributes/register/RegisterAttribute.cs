using System;

namespace Box {
    public class RegisterAttribute : Attribute
    {
        public string RegisterName;
        public bool IsCreateSingleton;

        public RegisterAttribute(string name,bool is_create_singleton = false)
        {
            RegisterName = name;
            IsCreateSingleton = is_create_singleton;
        }
    }
}