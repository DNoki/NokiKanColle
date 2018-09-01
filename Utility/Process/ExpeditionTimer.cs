using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NokiKanColle.Utility.Process
{
    /// <summary>
    /// 远征计时器类
    /// </summary>
    public class ExpeditionTimer : TimerWrapper
    {
        /// <summary>
        /// 计时器编号（2,3,4）
        /// </summary>
        private int _timerNumber;
        /// <summary>
        /// 线程锁
        /// </summary>
        private static readonly object _locked = new object();


        /// <summary>
        /// 从窗口控件读取时间
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetUITime()
        {
            if (GetMain_Form.InvokeRequired)
                return (TimeSpan)GetMain_Form.Invoke(new Func<TimeSpan>(GetUITime));
            else
            {
                try
                {
                    switch (_timerNumber)
                    {
                        case 2:
                            return new TimeSpan(Convert.ToInt32(GetMain_Form.GameExpedition_Timer2Hour_textBox.Text),
                                Convert.ToInt32(GetMain_Form.GameExpedition_Timer2Minute_textBox.Text),
                                Convert.ToInt32(GetMain_Form.GameExpedition_Timer2Second_textBox.Text));
                        case 3:
                            return new TimeSpan(Convert.ToInt32(GetMain_Form.GameExpedition_Timer3Hour_textBox.Text),
                                Convert.ToInt32(GetMain_Form.GameExpedition_Timer3Minute_textBox.Text),
                                Convert.ToInt32(GetMain_Form.GameExpedition_Timer3Second_textBox.Text));
                        case 4:
                            return new TimeSpan(Convert.ToInt32(GetMain_Form.GameExpedition_Timer4Hour_textBox.Text),
                                Convert.ToInt32(GetMain_Form.GameExpedition_Timer4Minute_textBox.Text),
                                Convert.ToInt32(GetMain_Form.GameExpedition_Timer4Second_textBox.Text));
                        default: return TimeSpan.Zero;
                    }
                }
                catch (Exception)
                { return TimeSpan.Zero; }
            }
        }
        /// <summary>
        /// 将时间写入窗口控件
        /// </summary>
        /// <param name="Time"></param>
        private void SetUITime(TimeSpan Time)
        {
            if (GetMain_Form.InvokeRequired)
            {
                GetMain_Form.BeginInvoke(new Action<TimeSpan>(SetUITime), Time);
            }
            else
            {
                switch (_timerNumber)
                {
                    case 2:
                        GetMain_Form.GameExpedition_Timer2Hour_textBox.Text = GetHour(Time).ToString();
                        GetMain_Form.GameExpedition_Timer2Minute_textBox.Text = Time.Minutes.ToString();
                        GetMain_Form.GameExpedition_Timer2Second_textBox.Text = Time.Seconds.ToString();
                        break;
                    case 3:
                        GetMain_Form.GameExpedition_Timer3Hour_textBox.Text = GetHour(Time).ToString();
                        GetMain_Form.GameExpedition_Timer3Minute_textBox.Text = Time.Minutes.ToString();
                        GetMain_Form.GameExpedition_Timer3Second_textBox.Text = Time.Seconds.ToString();
                        break;
                    case 4:
                        GetMain_Form.GameExpedition_Timer4Hour_textBox.Text = GetHour(Time).ToString();
                        GetMain_Form.GameExpedition_Timer4Minute_textBox.Text = Time.Minutes.ToString();
                        GetMain_Form.GameExpedition_Timer4Second_textBox.Text = Time.Seconds.ToString();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 设置远征勾选框UI
        /// </summary>
        /// <param name="team">远征队伍编号（2,3,4）</param>
        /// <param name="check">设定是否勾选</param>
        private void Set_ChangeTeamChecked(int team, bool check)
        {
            if (GetMain_Form.InvokeRequired)
            {
                GetMain_Form.BeginInvoke(new Action<int, bool>(Set_ChangeTeamChecked), team, check);
            }
            else
            {
                switch (team)
                {
                    case 2: GetMain_Form.GameExpedition_Team2Checked_checkBox.Checked = check; break;
                    case 3: GetMain_Form.GameExpedition_Team3Checked_checkBox.Checked = check; break;
                    case 4: GetMain_Form.GameExpedition_Team4Checked_checkBox.Checked = check; break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// 线程入口
        /// </summary>
        protected override void Entrance()
        {
            try
            { ExpeditionTimerStart(); }
            catch (Exception)
            {
                StopThread();
            }
        }
        /// <summary>
        /// 计时器线程入口
        /// </summary>
        private void ExpeditionTimerStart()
        {
            if (!Function.FunctionThread.Contains("自动远征"))
                new Utility.Process.Expedition();
            while (true)
            {
                var nowTime = DateTime.Now;//当前时间
                var startTime = nowTime;//计时器开始时刻
                var totalTimeSpan = GetTimeLeft == TimeSpan.Zero ? GetUITime() : GetTimeLeft;//计时时间
                var passingTimeSpan = TimeSpan.Zero;//流逝时间（=当前时刻-计时器开始时刻）

                IsWorking = true;//开始计时

                while (IsWorking)
                {
                    nowTime = DateTime.Now;
                    passingTimeSpan = nowTime - startTime;
                    if (passingTimeSpan >= totalTimeSpan)
                    {//计时结束
                        SetTime(TimeSpan.Zero);
                        SetUITime(TimeSpan.Zero);
                        IsWorking = false;
                        if (!Function.FunctionThread.Contains("自动远征"))
                        {
                            //new Utility.Process.Expedition();
                        }
                        continue;
                    }
                    IsWorking = true;
                    //剩余时间（=计时时间-流逝时间）
                    SetTime(totalTimeSpan - passingTimeSpan);
                    SetUITime(totalTimeSpan - passingTimeSpan);
                    Delay(999);
                }
                while (!IsWorking)// 等待远征结束
                {
                    if (!Function.FunctionThread.Contains("自动远征"))
                    {
                        this.StopThread();
                    }
                    Delay(999);
                }
            }
        }


        /// <summary>
        /// 关闭计时器线程
        /// </summary>
        public override void StopThread()
        {
            // 关闭UI勾选框
            Set_ChangeTeamChecked(_timerNumber, false);
            if (IsDisposed) return;
            IsWorking = false;
            SetTime(TimeSpan.Zero);
            SetUITime(TimeSpan.Zero);
            base.StopThread();
        }

        /// <summary>
        /// 远征计时器构造函数
        /// </summary>
        /// <param name="i"></param>
        public ExpeditionTimer(int i) : base("远征计时器_" + i.ToString())
        {
            _timerNumber = i;
            this.StartThread();
        }
    }
}
