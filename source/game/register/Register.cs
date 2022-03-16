using System.Reflection;
using System;
using System.Collections.Generic;
using Godot;

namespace Box {
    public class Register : Singleton<Register> {
        public enum RegisterType {
            Entity,
            Block,
            Item,
            Biome,

            Undefined,
        }

        protected Dictionary<string,Type> items;
        protected Dictionary<string,Type> blocks;
        protected Dictionary<string,Type> entities;
        protected Dictionary<string,IBiome> biomes;

        protected Type EntityType = typeof(IEntity);
        protected Type BlockType = typeof(IBlock);
        protected Type ItemType = typeof(IItem);
        protected Type BiomeType = typeof(IBiome);

        protected class UndefinedRegisterType :Exception {
            public UndefinedRegisterType(Type type):base($"未定的注册类型. -> {type.FullName}"){}
        }

        protected class NotScene :Exception {
            public NotScene(string path):base($"不存在场景. -> {path}"){}
        }

        protected class RegisterTypeDefineError : Exception {
            public RegisterTypeDefineError(Type type):base($"需要继承自Godot.Node -> {type.FullName}"){}
        }

        protected RegisterType GetRegisterType(Type t) {
            RegisterType type = RegisterType.Undefined;

            if(EntityType.IsAssignableFrom(t)) type = RegisterType.Entity;
            else if (BlockType.IsAssignableFrom(t)) type = RegisterType.Block;
            else if (ItemType.IsAssignableFrom(t)) type = RegisterType.Item;
            else if (BiomeType.IsAssignableFrom(t)) type = RegisterType.Biome;

            return type;
        }

        public void Init() {
            var assembly = Assembly.GetAssembly(typeof(Register));
            foreach(Type type in assembly.GetTypes()) {
                RegisterAttribute reg_info = (RegisterAttribute)Attribute.GetCustomAttribute(type,typeof(RegisterAttribute));
                if(reg_info != null) {
                    string reg_name = reg_info.RegisterName;
                    RegisterType reg_type = GetRegisterType(type);
                    if(reg_type == RegisterType.Undefined) {
                        throw new UndefinedRegisterType(type);
                    }
                    
                    switch(reg_type) {
                        case RegisterType.Entity : {
                            entities[reg_name] = type;
                        }break;
                        case RegisterType.Block : {
                            blocks[reg_name] = type;
                        }break;
                        case RegisterType.Item : {
                            items[reg_name] = type;
                        }break;
                        case RegisterType.Biome : {
                            IBiome biome = (IBiome)Activator.CreateInstance(type);
                            biomes[reg_name] = biome;
                        }break;
                    }
                }
            }
        }


        protected Node TryCreatePackedScene(Type type) {
            BindSceneAttribute bind_scene_info = (BindSceneAttribute)Attribute.GetCustomAttribute(type,typeof(BindSceneAttribute));
            if(bind_scene_info == null) return null;
            PackedScene packed_scene = GD.Load<PackedScene>(bind_scene_info.ScenePath);
            if(packed_scene == null) {
                throw new NotScene(bind_scene_info.ScenePath);
            }
            return packed_scene.Instance();
        }

        public Node Create(RegisterType reg_type,string name) {
            if(!entities.ContainsKey(name)) return null;
            Type type = entities[name];
            switch(reg_type) {
                case RegisterType.Entity : {
                    type = entities[name];
                }break;
                case RegisterType.Block : {
                    type = blocks[name];
                }break;
                case RegisterType.Item : {
                    type = items[name];
                }break;

            }
            IEntity entity = (IEntity)Activator.CreateInstance(type);
            Node node = TryCreatePackedScene(type);
            if(node == null) {
                if(!(entity is Node)) throw new RegisterTypeDefineError(type);
                node = (Node)entity;
            }

            return node;
        }

        public Node CreateEntity(string name) {
            return Create(RegisterType.Entity,name);
        }

        public Node CreateBlock(string name) {
            return Create(RegisterType.Block,name);
        }

        public Node CreateItem(string name) {
            return Create(RegisterType.Item,name);
        }

        public IBiome GetBiome(string name) {
            if(!biomes.ContainsKey(name)) return null;
            return biomes[name];
        }
    }
}