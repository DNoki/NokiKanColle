using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NokiKanColle.Function;

namespace NokiKanColle.Utility
{
    /// <summary>
    /// 计时器抽象类
    /// </summary>
    public abstract class TimerWrapper : ThreadsWrapper
    {
        /// <summary>
        /// 计时器剩余时间
        /// </summary>
        private TimeSpan _timeLeft = TimeSpan.Zero;

        /// <summary>
        /// 是否正在计时（true表示计时中，false表示计时结束）
        /// </summary>
        public virtual bool IsWorking { get; set; }
        /// <summary>
        /// 计时时间
        /// </summary>
        public TimeSpan GetTimeLeft { get { return _timeLeft; } }
        /// <summary>
        /// 剩余毫秒数
        /// </summary>
        public int GetGetTimeLeftToMilliseconds
        {
            get
            {
                return (int)_timeLeft.TotalMilliseconds;
            }
        }


        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        public void SetTime(int hour, int minute, int second)
        { _timeLeft = new TimeSpan(hour, minute, second); }
        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        public void SetTime(TimeSpan time)
        { _timeLeft = time; }

        /// <summary>
        /// 剩余小时数
        /// </summary>
        public int GetHour()
        { return GetHour(_timeLeft); }
        /// <summary>
        /// 剩余小时数
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        public int GetHour(TimeSpan Time)
        { return (int)Math.Floor(Time.TotalHours); }

        public override void StartThread()
        {
            if (!IsThreadAlive)
            {
                IsWorking = true;//开始计时
            }
            base.StartThread();
        }

        public TimerWrapper(string name) : base(name) { }
    }
}
