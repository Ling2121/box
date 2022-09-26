# Box 

这是一个基于Godot的沙盒游戏开发框架，拥有许多游戏开发的基本内容。本框架用于开发Minecraft或者饥荒类似的沙盒生存游戏，内置了地图的生成以及对象的构建的基本内容。

> 开发阶段 2022.9.26
> 计划:
>   1.世界生成器 53%
>   2.世界运行时（已完成，在旧项目中，需要重构）

# 文件夹结构

 * addons 插件
 * scene 场景
 * source 源码
    * attributes 所有属性
    * interfaces 所有接口
    * entities 游戏实体
    * components 实体的组件
    * base 基础类
    * core 游戏核心类


# 基础类
 * Storage 存储类
 * Voronoi Voronoi图
 * IndexPool 索引池
 * NoiseGenerator 噪声生成器
 * Singleton 单例
 * Interval 区间类型

# 游戏核心
 * Sandbox 沙盒类(世界运行时)
 * Register 注册表
 * WorldBuilder 世界生成器
    * VoronoiWorldBuilder 基于Voronoi图的世界生成器

 
