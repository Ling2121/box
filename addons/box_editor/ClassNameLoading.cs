using Godot;
using System;

namespace Godot
{
    public class ClassNameLoading : Godot.Object
    {
        public String GetObjectBaseType(Type type)
        {
            var b = type.BaseType;
            while(b.BaseType != null && b.Namespace != "Godot")
            {
                b = b.BaseType;
            }
            return b.Name;
        }
        public object[] GetObjectRegistered(object obj)
        {
            var type = obj.GetType();
            var attributes = type.GetCustomAttributes(false);
            object[] objarr = new object[3];
            foreach (var attribute in attributes)
            {
                if(attribute is ClassNameAttribute)
                {
                    objarr[0] = ((ClassNameAttribute)attribute).name;
                    objarr[1] = GetObjectBaseType(type);
                    objarr[2] = ((ClassNameAttribute)attribute).icon_path;
                    return objarr;
                }
            }
            return null;
        }
    }
}