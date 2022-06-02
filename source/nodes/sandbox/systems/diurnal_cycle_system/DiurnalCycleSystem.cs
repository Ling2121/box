using System.Collections.Generic;
using Godot;

namespace Box {

    public enum PeriodOfTime
    {
        Night,//夜晚
        Sunset,//日出
        Daytime,//白天
        Sunrise,//日落
        
    }

    [ClassName(nameof(DiurnalCycleSystem))]
    public class DiurnalCycleSystem : Node {
        public Dictionary<PeriodOfTime,int> PeriodOfTimeInterval = new Dictionary<PeriodOfTime, int>();
        public Dictionary<PeriodOfTime,float> PeriodOfTimeLight = new Dictionary<PeriodOfTime, float>();

        public TimeSystem TimeSystem;
        public Sandbox Sandbox;

        public ShaderMaterial LightShader;

        public PeriodOfTime CurrentPeriodOfTime;

        public override void _EnterTree()
        {
            int night   = (int)(TimeSystem.DAY_HOUR * 0.2500f+0.5f) * TimeSystem.HOUR_MINUTE;
            int sunset  = (int)(TimeSystem.DAY_HOUR * 0.2916f+0.5f) * TimeSystem.HOUR_MINUTE;
            int daytime = (int)(TimeSystem.DAY_HOUR * 0.7500f+0.5f) * TimeSystem.HOUR_MINUTE;
            int sunrise = (int)(TimeSystem.DAY_HOUR * 1.0000f+0.5f) * TimeSystem.HOUR_MINUTE;

            PeriodOfTimeInterval[PeriodOfTime.Night]    = night;
            PeriodOfTimeInterval[PeriodOfTime.Sunset]   = sunset;
            PeriodOfTimeInterval[PeriodOfTime.Daytime]  = daytime;
            PeriodOfTimeInterval[PeriodOfTime.Sunrise]  = sunrise;

            PeriodOfTimeLight[PeriodOfTime.Night]    = 0.2f;
            PeriodOfTimeLight[PeriodOfTime.Sunset]   = 0.6f;
            PeriodOfTimeLight[PeriodOfTime.Daytime]  = 1.0f;
            PeriodOfTimeLight[PeriodOfTime.Sunrise]  = 0.6f;

            Game.Instance.DiurnalCycleSystem = this;
        }

        public override void _Ready()
        {
            TimeSystem = Game.Instance.TimeSystem;
            Sandbox = Game.Instance.Sandbox;
            
            LightShader = new ShaderMaterial();
            LightShader.Shader =  GD.Load<Shader>("res://source/shader/light.shader");
            Sandbox.Material = LightShader;

            TimeSystem.Connect(nameof(TimeSystem.minute_step),this,nameof(_MinuteStep));

            CurrentPeriodOfTime = PeriodOfTime.Night;
            UpdateLight();
        }

        public void UpdateLight() {
            LightShader.SetShaderParam("light_intensity",PeriodOfTimeLight[CurrentPeriodOfTime]);
        }

        public void _MinuteStep() {
            int minute = (TimeSystem.Hour * TimeSystem.HOUR_MINUTE) + TimeSystem.Minute;
            foreach(var item in PeriodOfTimeInterval) {
                if(item.Value > minute) {
                    if(CurrentPeriodOfTime != item.Key) {
                        CurrentPeriodOfTime = item.Key;
                        UpdateLight();
                    }
                    break;
                }
            }
            
        }
    }
}