using System;

namespace Box {
    public class RegisterAttribute : Attribute
    {
        public string RegisterName;

        public RegisterAttribute(string name)
        {
            RegisterName = name;
        }
    }
}