using Godot;
using System;


namespace Box.Scene.NoiseEditor {
    public class NoiseEditor : Node2D
    {   
        public NoiseEditorUIInput SeedInput;
        public NoiseEditorUIInput SizeInput;
        public NoiseEditorUIInput PeriodInput; 
        public NoiseEditorUIInput OctavesInput;
        public NoiseEditorUIInput PersistenceInput;
        public NoiseEditorUIInput LacunarityInput;
        public Button BuildButton;
        public Sprite DisplaySprite;
        public NoiseGenerator Noise;
        public Image image = new Image();

        public override void _Ready()
        {
            SeedInput = GetNode<NoiseEditorUIInput>("UI/Control/SeedInput");
            SizeInput = GetNode<NoiseEditorUIInput>("UI/Control/SizeInput");
            PeriodInput = GetNode<NoiseEditorUIInput>("UI/Control/PeriodInput");
            OctavesInput = GetNode<NoiseEditorUIInput>("UI/Control/OctavesInput");
            PersistenceInput = GetNode<NoiseEditorUIInput>("UI/Control/PersistenceInput");
            LacunarityInput = GetNode<NoiseEditorUIInput>("UI/Control/LacunarityInput");

            DisplaySprite = GetNode<Sprite>("DisplaySprite");
            Noise = new NoiseGenerator();
            UpdateNoiseSetting();
            UpdateDraw();

            PeriodInput.Connect("_ChangeValue",this,"_ChangeValue");
            OctavesInput.Connect("_ChangeValue",this,"_ChangeValue");
            PersistenceInput.Connect("_ChangeValue",this,"_ChangeValue");
            LacunarityInput.Connect("_ChangeValue",this,"_ChangeValue");
            SeedInput.Connect("_ChangeValue",this,"_ChangeValue");
            SizeInput.Connect("_ChangeValue",this,"_ChangeValue");
        }

        public void UpdateNoiseSetting( ) {
            Noise.Seed = (int)SeedInput.Value;
            Noise.Period = PeriodInput.Value;
            Noise.Octaves = (int)OctavesInput.Value;
            Noise.Persistence = PersistenceInput.Value;
            Noise.Lacunarity = LacunarityInput.Value;
        }

        public void UpdateDraw() {
            int size = (int)SizeInput.Value;
            image.Create(size,size,false,Image.Format.Rgba8);
            image.Lock();
            for(int y = 0;y<size;y++) {
                for(int x = 0;x<size;x++) {
                    float noise = Noise.IslandNoise(new Vector2(x,y),size,size);
                    //byte gray = (byte)(((noise + 1)* 128));
                    int gray = (int)(noise * 256);
                    // if(noise < 0) {
                    //     GD.Print(gray,"  ",noise,(int)(noise * 256));
                    // }
                    image.SetPixel(x,y,new Color(noise,noise,noise));
                }
            }
            image.Unlock();
            ImageTexture texture = (ImageTexture)DisplaySprite.Texture;
            texture.CreateFromImage(image);
        }

        public void _ChangeValue(float value) {
            UpdateNoiseSetting();
            UpdateDraw();
        }

        public override void _Input(InputEvent _event)
        {
            if(_event is InputEventMouseButton) {
                InputEventMouseButton mouse_input = (InputEventMouseButton)_event;
                if(mouse_input.Pressed) {
                    ImageTexture texture = (ImageTexture)DisplaySprite.Texture;
                    Image image = texture.GetData();
                    image.Lock();
                    GD.Print(image.GetPixelv(DisplaySprite.GetLocalMousePosition()) * 255);
                    image.Unlock();
                }
            }
        }
    }

}
