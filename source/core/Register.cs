using Godot;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Box {
    public enum RegisterType {
        Undefined,
        Entity,
        Block,
        Item,
        Biome,
        Event,
    }

    public class RegisterItem {
        public Type Type; 
        public object Singleton;
        public T GetSingleton<T>() where T: class,new(){
            return Singleton as T;
        }
        public T CreateInstance<T>() where T : class,new(){
            return Activator.CreateInstance(Type) as T;
        }
    }

    public class Register : Singleton<Register> {
        public class NotScene :Exception {
            public NotScene(string path):base($"不存在场景. -> {path}"){}
        }

        public readonly Type EntityType = typeof(IEntity);
        public readonly Type BlockType = typeof(IBlock);
        public readonly Type ItemType = typeof(IItem);
        public readonly Type BiomeType = typeof(IBiome);
        public readonly Type EventType = typeof(IEvent);
        public readonly Type RegisterAttributeType = typeof(RegisterAttribute);

        protected Dictionary<RegisterType,Dictionary<string,RegisterItem>> Registers = new Dictionary<RegisterType, Dictionary<string, RegisterItem>>();
    
        public Dictionary<string,RegisterItem> GetRegister(Type t) {
            if(EntityType   .IsAssignableFrom(t)) return Registers[RegisterType.Entity];
            if(BlockType    .IsAssignableFrom(t)) return Registers[RegisterType.Block];
            if(ItemType     .IsAssignableFrom(t)) return Registers[RegisterType.Item];
            if(BiomeType    .IsAssignableFrom(t)) return Registers[RegisterType.Biome];
            if(EventType    .IsAssignableFrom(t)) return Registers[RegisterType.Event];
            return null;
        }

        public Register() {
            var assembly = Assembly.GetAssembly(typeof(Register));
            foreach(Type type in assembly.GetTypes()) { 
                RegisterAttribute register_info = (RegisterAttribute)Attribute.GetCustomAttribute(type,RegisterAttributeType);
                if(register_info != null) {
                    var register = GetRegister(type);
                    if(register != null) {
                        RegisterItem item = new RegisterItem();
                        item.Type = type;
                        if(register_info.IsCreateSingleton) {
                            item.Singleton = Activator.CreateInstance(type);
                        }
                        register[register_info.RegisterName] = item;
                    }
                }
            }
        }

        protected Node TryCreatePackedScene(Type type) {
            SceneInstanceAttribute scene_instance_info = (SceneInstanceAttribute)Attribute.GetCustomAttribute(type,typeof(SceneInstanceAttribute));
            if(scene_instance_info == null) return null;
            PackedScene packed_scene = GD.Load<PackedScene>(scene_instance_info.ScenePath);
            if(packed_scene == null) {
                throw new NotScene(scene_instance_info.ScenePath);
            }
            return packed_scene.Instance();
        }


        public RegisterItem GetRegisterItem(RegisterType type,string type_name) {
            var register = Registers[type];
            if(!register.ContainsKey(type_name)) return null;
            return  register[type_name];
        }

        public T Create<T>(RegisterType type,string type_name) where T : class {
            var item = GetRegisterItem(type,type_name);
            if(item == null) return null;
            return Activator.CreateInstance(item.Type) as T;
        }

    }
}