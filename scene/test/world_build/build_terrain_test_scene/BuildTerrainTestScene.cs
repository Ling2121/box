using Godot;
using System;
using Box;
using Box.WorldBuilds.VoronoiPort;

public class BuildTerrainTestScene : Node2D
{

    int Seed {
        get {
            return (int)BuildData.Seed;
        }

        set {
            BuildData.Seed = (ulong)value;
        }
    }

    
    int Width {
        get {
            return BuildData.Width;
        }

        set {
            BuildData.Width = value;
        }
    }

    
    int Height {
        get {
            return BuildData.Height;
        }

        set {
            BuildData.Height = value;
        }
    }

    
    int PointNumber {
        get {
            return BuildData.顶点生成数;
        }

        set {
            BuildData.顶点生成数 = value;
        }
    }

    
    int Octaves {
        get {
            return BuildData.Noise.Octaves;
        }

        set {
            BuildData.Noise.Octaves= value;
        }
    }

    
    float Period{
        get {
            return BuildData.Noise.Period;
        }

        set {
            BuildData.Noise.Period= value;
        }
    }

    
    float Persistence{
        get {
            return BuildData.Noise.Persistence;
        }

        set {
            BuildData.Noise.Persistence = value;
        }
    }

    
    float Lacunarity {
        get {
            return BuildData.Noise.Persistence;
        }

        set {
            BuildData.Noise.Persistence= value;
        }
    }

    
    byte MountainHeight {
        get {
            return BuildData.山脉高度;
        }

        set {
            BuildData.山脉高度= value;
        }
    }
    
    byte LandHeight {
        get {
            return BuildData.陆地高度;
        }

        set {
            BuildData.陆地高度= value;
        }
    }

    ImageTexture terrain_canvas1;

    public VoronoiWorldBuilderData BuildData = new VoronoiWorldBuilderData();

    public VoronoiWorldBuilder Builder = new VoronoiWorldBuilder();

    public override void _Ready()
    {
        Seed = 23333;
        Octaves = 3;
        Period = 55;
        Persistence = 0.5f;
        Lacunarity = 1.68f;
        Width = 512;
        Height = 512;
        PointNumber = 200;
        MountainHeight = 80;
        LandHeight = 10;
        BuildData.湖泊生成尝试数 = 5;
        BuildData.湖泊生成几率 = 100;
        BuildData.火山湖生成尝试数 = 2;
        BuildData.火山湖生成几率 = 100;
        Build();
    }

    public void Build() {
        Builder.Build(BuildData);
        Update();
    }

    public override void _Draw()
    {
        if(!BuildData.Canvases.ContainsKey("terrain_canvas1")) return;
        terrain_canvas1 =  BuildData.Canvases["terrain_canvas1"].ToImage();
        DrawTexture(terrain_canvas1,Vector2.Zero);
    }
}
