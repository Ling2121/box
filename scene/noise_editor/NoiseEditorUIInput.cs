using Godot;
using System;

namespace Box.Scene.NoiseEditor {
    [Tool]
    public class NoiseEditorUIInput : LineEdit
    {
        [Signal]
        delegate void _ChangeValue(float value);


        [Export]
        public float DefaultValue = 0;

        [Export]
        public string Label {
            get {
                var label = GetNodeOrNull<Label>("InputName");
                if(label == null) return "";
                return label.Text;
            }

            set {
                var label = GetNodeOrNull<Label>("InputName");
                if(label == null) return;
                label.Text = value.ToString();
            }
        }


        bool open_slider = true;
        [Export]
        public bool OpenSlider {
            get {
                return open_slider;
            }

            set {
                open_slider = value;
                if(open_slider) {
                    if(!HasNode("HSlider")) {
                        if(slider != null) {
                            AddChild(slider);
                        }
                    }else {
                        if(slider == null) {
                            slider = GetNode<HSlider>("HSlider");
                        }
                    }
                }else {
                    if(HasNode("HSlider")) {
                        RemoveChild(GetNode("HSlider"));
                    }
                }
            }
        }

        [Export]
        public float MinValue = 0;
        [Export]
        public float MaxValue = 100;

        public float Value {
            get {
                float v;
                if(!float.TryParse(Text,out v)) {
                    return 0;
                }
                return v;
            }
        }

        protected HSlider slider;
        public override void _Ready()
        {
            slider = GetNode<HSlider>("HSlider");
            slider.MaxValue = MaxValue;
            slider.MinValue = MinValue;

            slider.Value = DefaultValue;
            Text = ((float)DefaultValue).ToString();

            Connect("text_entered",this,nameof(_TextEntered));
            slider.Connect("value_changed",this,nameof(_ValueChange));
        }

        public void _TextEntered(string text) {
            slider.Value = Value;
            EmitSignal(nameof(_ChangeValue),Value);
        }

        public void _ValueChange(double value) {
            Text = ((float)value).ToString();
            EmitSignal(nameof(_ChangeValue),Value);
        }
    }

}
