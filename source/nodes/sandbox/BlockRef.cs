using Godot;

namespace Box {
    /*
        Block的来源有两个，一个是来自于Tile，一个是来自于Node
        来自于Tile的又分单例的和独立的，所以需要一个容器用来索引
    */
    public class BlockRef : Godot.Object {
        public bool IsTile;
        public SandboxLayer Layer;
        public IBlock Block;
        public Vector2 Position;

        public override int GetHashCode()
        {
            return Block.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(!(obj is BlockRef)) return false;
            return (obj as BlockRef).Block == Block;
        }

        public void Remove() {
            if(IsTile) {
                Game.Instance.Sandbox.SetCell(Layer,(int)Position.x,(int)Position.y,"");
            } else {
                if(Block is Node) {
                    (Block as Node).Free();
                }
            }
        }
    }
}