using System;
using Godot;

namespace Box
{
    [ClassName(nameof(TimeSystem))]
    public class TimeSystem : Timer
    {
        [Signal]
        public delegate void minute_step();

        public const int HOUR_MINUTE = 60;//1小时 = 60分钟
        public const int DAY_HOUR = 24;//1天 = 24小时
        public const int WEEK_DAY = 7;//1周 = 7天
        public const int MONTH_DAY = 28;//1月 = 28天
        public const int YEAR_MONTH = 12;//1年=14月


        public const int DAY_MINUTE =  DAY_HOUR * HOUR_MINUTE;
        public const int MONTH_MINUTE = MONTH_DAY * DAY_MINUTE;
        public const int YEAR_MINUTE = YEAR_MONTH * MONTH_MINUTE;

        public const int HOUR_SECONDS = DAY_HOUR * HOUR_MINUTE;

        public const int BASE_YEAR = 1970;//1.1

        [Export]//基准秒(现实秒数为单位)。1基准秒=1游戏秒
        public float BenchmarkTime = 0.01f;
        [Export]
        public int Year {get;protected set;} = 2100;//年
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

        public long Timestamp {get;protected set;}= 0;

        public static int ToNumberTime(int h,int m)
        {
            return h * 100 + m;
        }

        public void UpdateTimestamp() {
            int y = Year - BASE_YEAR - 1;
            long m = y * YEAR_MINUTE;
            Timestamp = m + 
                ((Month - 1) * MONTH_MINUTE) +
                ((Day - 1) * DAY_MINUTE) +
                (Hour * HOUR_MINUTE)
            ;
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

        public override string ToString()
        {
            return $"{this.Year}:{this.Month}:{this.Day}   {this.Hour}:{this.Minute}";
        }

        public void _Timeout()
        {
            Minute++;
            Timestamp++;
            EmitSignal(nameof(minute_step));

            //时进
            if(Minute >= HOUR_MINUTE)
            {
                Minute = 0;
                Hour++;
            }

            //日进
            if(Hour >= DAY_HOUR)
            {
                Hour = 0;
                Day++;
            }

            //周进
            if(Week >= WEEK_DAY)
            {
                Week = 1;
            }

            //月进
            if(Day >= MONTH_DAY)
            {
                Day = 1;
                Month++;
            }

            //年进
            if(Month >= MONTH_DAY)
            {
                Month = 1;
                Year++;
            }
        }
    } 
}