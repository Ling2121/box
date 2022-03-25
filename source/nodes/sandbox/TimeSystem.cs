using System;
using Godot;

namespace Box
{
    [ClassName(nameof(TimeSystem))]
    public class TimeSystem : Timer
    {
        public const int SECONDS_LENGHT = 60;
        public const int MINUTE_LENGHT = 60;
        public const int HOUR_LENGHT = 60;//1小时 = 60分钟
        public const int DAY_LENGHT = 24;//1天 = 24小时
        public const int WEEK_LENGHT = 7;//1周 = 7天
        public const int MONTH_LENGHT = 30;//1月 = 30天
        public const int YEAR_LENGHT = 14;//1年=14月

        [Export]//基准秒(现实秒数为单位)。1基准秒=1游戏秒
        public float BenchmarkTime = 0.01f;
        [Export]
        public int Year {get;protected set;} = 2780;//年
        [Export]
        public int Month {get;protected set;}  = 1;//月
        [Export]
        public int Week {get;protected set;} = 1;//周
        [Export]
        public int Day {get;protected set;} = 1;//日
        [Export]
        public int Hour {get;protected set;} = 1;//时
        [Export]
        public int Minute {get;protected set;} = 0;//分
        [Export]
        public int Seconds {get;protected set;} = 0;//秒

        public static int ToNumberTime(int h,int m)
        {
            return h * 100 + m;
        }

        public override void _EnterTree()
        {
            Game.Instance.TimeSystem = this;
        }

        public override void _Ready()
        {
            base._Ready();
            WaitTime = BenchmarkTime;
            Autostart = true;
            Connect("timeout",this,nameof(_Timeout));
        }

        public void _Timeout(float dt)
        {
            Seconds++;

            if(Seconds > SECONDS_LENGHT) {
                //分进
                Seconds = 0;
                Minute++;
            }

            //时进
            if(Minute >= HOUR_LENGHT)
            {
                Minute = 0;
                Hour++;
            }

            //日进
            if(Hour >= DAY_LENGHT)
            {
                Hour = 0;
                Day++;
            }

            //周进
            if(Week >= WEEK_LENGHT)
            {
                Week = 1;
            }

            //月进
            if(Day >= MONTH_LENGHT)
            {
                Day = 1;
                Month++;
            }

            //年进
            if(Month >= MONTH_LENGHT)
            {
                Month = 1;
                Year++;
            }
        }
    } 
}