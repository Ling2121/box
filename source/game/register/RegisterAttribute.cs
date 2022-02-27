using System;

namespace Box {
    public class RegisterAttribute : Attribute
    {
        public RegisterType RegisterType;
        public string RegisterName;

        public RegisterAttribute(RegisterType type,string name)
        {
            RegisterType = type;
            RegisterName = name;
        }
    }
}