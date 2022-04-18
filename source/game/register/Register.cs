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
            InterplayEvent,

            Undefined,
        }

        protected Dictionary<string,Type> items = new Dictionary<string, Type>();
        protected Dictionary<string,Type> blocks = new Dictionary<string, Type>();
        protected Dictionary<string,Type> entities = new Dictionary<string, Type>();
        protected Dictionary<string,IBiome> biomes = new Dictionary<string, IBiome>();
        protected Dictionary<string,IEvent> events = new Dictionary<string, IEvent>();
        protected Dictionary<string,string> tile_bind_blocks = new Dictionary<string, string>();

        protected Dictionary<string,IItem> item_singletons = new Dictionary<string, IItem>();
        protected Dictionary<string,IBlock> block_singletons = new Dictionary<string, IBlock>();
        protected Dictionary<string,IEntity> entity_singletons = new Dictionary<string, IEntity>();

        public List<string> BiomeNameList = new List<string>();

        protected Type EntityType = typeof(IEntity);
        protected Type BlockType = typeof(IBlock);
        protected Type ItemType = typeof(IItem);
        protected Type BiomeType = typeof(IBiome);
        protected Type InterplayEventType = typeof(IEvent);

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
            else if (InterplayEventType.IsAssignableFrom(t)) type = RegisterType.InterplayEvent;
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
                            if(reg_info.IsCreateSingleton){
                                entity_singletons[reg_name] = Activator.CreateInstance(type) as IEntity;
                            }
                        }break;
                        case RegisterType.Block : {
                            blocks[reg_name] = type;
                            var bind_tile = type.GetCustomAttribute<BindCellAttribute>();
                            if(bind_tile != null) {
                                tile_bind_blocks[bind_tile.CellName] = reg_name;
                            }
                            if(reg_info.IsCreateSingleton){
                                block_singletons[reg_name] = Activator.CreateInstance(type) as IBlock;
                            }
                        }break;
                        case RegisterType.Item : {
                            items[reg_name] = type;
                            if(reg_info.IsCreateSingleton){
                                item_singletons[reg_name] = Activator.CreateInstance(type) as IItem;
                            }
                        }break;
                        case RegisterType.Biome : {
                            IBiome biome = (IBiome)Activator.CreateInstance(type);
                            BiomeNameList.Add(reg_name);
                            biomes[reg_name] = biome;
                        }break;
                        case RegisterType.InterplayEvent : {
                            IEvent event_ = (IEvent)Activator.CreateInstance(type);
                            events[reg_name] = event_;
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
            Type type = null;
            switch(reg_type) {
                case RegisterType.Entity : {
                    if(!entities.ContainsKey(name)) return null;
                    type = entities[name];
                }break;
                case RegisterType.Block : {
                    if(!blocks.ContainsKey(name)) return null;
                    type = blocks[name];
                }break;
                case RegisterType.Item : {
                    if(!items.ContainsKey(name)) return null;
                    type = items[name];
                }break;

                default:return null;

            }
            Node node = TryCreatePackedScene(type);
            return node == null?(Node)Activator.CreateInstance(type):node;
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

        public Node CreateTileBindBlock(string tile_name) {
            if(!tile_bind_blocks.ContainsKey(tile_name)) return null;
            return Create(RegisterType.Block,tile_bind_blocks[tile_name]);
        }

        public string GetTileBindBlockName(string tile_name) {
            if(!tile_bind_blocks.ContainsKey(tile_name)) return "";
            return tile_bind_blocks[tile_name];
        }

        public IBiome GetBiome(string name) {
            if(!biomes.ContainsKey(name)) return null;
            return biomes[name];
        }

        public IEvent GetEvent(string name) {
            if(!events.ContainsKey(name)) return null;
            return events[name];
        }

        public IEntity GetEntityInstance(string name) {
            if(!entity_singletons.ContainsKey(name)) return null;
            return entity_singletons[name];
        }

        public IBlock GetBlockInstance(string name) {
            if(!block_singletons.ContainsKey(name)) return null;
            return block_singletons[name];
        }

        public IItem GetItemInstance(string name) {
            if(!item_singletons.ContainsKey(name)) return null;
            return item_singletons[name];
        }

        public Type GetEntityType(string name) {
            if(!entities.ContainsKey(name)) return null;
            return entities[name];
        }

        public Type GetBlockType(string name) {
            if(!blocks.ContainsKey(name)) return null;
            return blocks[name];
        }

        public Type GetItemType(string name) {
            if(!items.ContainsKey(name)) return null;
            return items[name];
        }
    }
}