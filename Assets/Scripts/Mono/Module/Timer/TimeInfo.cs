using System;
using System.Text;

namespace TaoTie
{
    public class TimeInfo: IDisposable
    {
        public const long OneDay = 86400000;
        public const long Hour = 3600000;
        public const long Minute = 60000;

        public static TimeInfo Instance = new TimeInfo();

        private int timeZone;
        
        public int TimeZone
        {
            get
            {
                return this.timeZone;
            }
            set
            {
                this.timeZone = value;
                dt = dt1970.AddHours(TimeZone);
            }
        }
        
        private readonly DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public long ServerMinusClientTime { private get; set; }

        public long FrameTime;

        private TimeInfo()
        {
            this.FrameTime = this.ClientNow();
        }

        public void Update()
        {
            this.FrameTime = this.ClientNow();
        }
        
        /// <summary> 
        /// 根据时间戳获取时间 
        /// </summary>  
        public DateTime ToDateTime(long timeStamp)
        {
            return dt.AddTicks(timeStamp * 10000);
        }
        
        // 线程安全
        public long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - this.dt1970.Ticks) / 10000;
        }
        
        public long ServerNow()
        {
            return ClientNow() + Instance.ServerMinusClientTime;
        }
        
        public long ClientFrameTime()
        {
            return this.FrameTime;
        }
        
        public long ServerFrameTime()
        {
            return this.FrameTime + Instance.ServerMinusClientTime;
        }
        
        public long Transition(DateTime d)
        {
            return (d.Ticks - dt.Ticks) / 10000;
        }

        public string TransitionToStr(long time)
        {
            var d = time/OneDay;
            time %= OneDay;
            var h = time/Hour;
            time %= Hour;
            var m = time / Minute;
            time %= Minute;
            var s = time / 1000;
            StringBuilder sb = new StringBuilder();
            if (d > 0)
            {
                sb.Append(d + "d");
            }
            if (h > 0)
            {
                sb.Append(h + "h");
            }
            if (m > 0)
            {
                sb.Append(m + "m");
            }
            if (s > 0)
            {
                sb.Append(s + "s");
            }

            return sb.ToString();
        }
        
        public string TransitionToStr2(long time)
        {
            var d = time/OneDay;
            time %= OneDay;
            var h = time/Hour;
            time %= Hour;
            var m = time / Minute;
            time %= Minute;
            var s = time / 1000;
            StringBuilder sb = new StringBuilder();
            bool has = d + m + h + s == 1;
            if (d > 0)
            {
                sb.Append((has?"":d) + I18NBridge.Instance.GetText("Text_Time_Day"));
            }
            if (h > 0)
            {
                sb.Append((has?"":h) + I18NBridge.Instance.GetText("Text_Time_Hour"));
            }
            if (m > 0)
            {
                sb.Append((has?"":m) + I18NBridge.Instance.GetText("Text_Time_Minute"));
            }
            if (s > 0)
            {
                sb.Append((has?"":s) + I18NBridge.Instance.GetText("Text_Time_Second"));
            }

            return sb.ToString();
        }
        public void Dispose()
        {
            Instance = null;
        }
    }
}