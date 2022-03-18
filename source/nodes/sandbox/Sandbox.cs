using System.Threading.Tasks;
using System.Collections;
using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Box {
    public struct CreatePack {
        Register.RegisterType RegisterType;
        String Name;
        Hashtable Datas;
    }

    public class Sandbox : YSort {
        protected static ConcurrentQueue<CreatePack> create_queue = new ConcurrentQueue<CreatePack>();
        public static ConcurrentQueue<CreatePack> CreateQueue {get {return create_queue;}}

        //已加载的区块
        protected Dictionary<int,Dictionary<int,SandboxRegion>> regions = new Dictionary<int, Dictionary<int, SandboxRegion>>();
        //在场景中的区块
        protected Node2D loading_regions = new Node2D();

        //世界生成器
        protected WorldBuild world_build;

        [Export]
        //加载中心
        public Vector2 LoadCenter = new Vector2(0,0);
        [Export]
        //加载半径
        public int LoadRadius = 4;
        [Export]
        //卸载半径
        public int UnloadRadius = 12;

        public void LoadRegion() {
            //创建区块[生成/读取]线程
            Task.Run(()=>{

            });
        }

        public void UnloadRegion() {
            //创建区块卸载线程
            Task.Run(()=>{

            });
        }

        public void StopRegion() {
            
        }
    }
}