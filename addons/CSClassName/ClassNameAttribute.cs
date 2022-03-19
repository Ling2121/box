using System;

namespace Godot
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassNameAttribute : System.Attribute
    {
        public String name;
        public String icon_path = "Base";

        public ClassNameAttribute(){}

        public ClassNameAttribute(String name,String icon = "Base")
        {
            this.name = name;
            this.icon_path = icon;
        }
    }
}