using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using NokiKanColle.Function;
using static NokiKanColle.Data.DataPond;
using static NokiKanColle.Function.Functions;
using NokiKanColle.Data;

namespace NokiKanColle.Utility.Process
{
    public class Attack : GameProcessWrapper
    {
        /// <summary>
        /// 已出击数
        /// </summary>
        private int Frequency = 0;
        /// <summary>
        /// 远征海域（只读）
        /// </summary>
        private int Seas => Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Seas_combobox.SelectedIndex; })));
        /// <summary>
        /// 远征地图（只读）
        /// </summary>
        private int Map => Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Map_combobox.SelectedIndex; })));
        /// <summary>
        /// 是否入渠
        /// </summary>
        private bool IsDock => Convert.ToBoolean(GetMain_Form.Invoke(new Func<bool>(() => { return this.GetMain_Form.GameAttack_IsDock_checkBox.Checked; })));
        /// <summary>
        /// 撤退条件
        /// </summary>
        private int DetectionStatus => 1 + Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_DetectionStatus_comboBox.SelectedIndex; })));
        /// <summary>
        /// 入渠基准
        /// </summary>
        private int DockBenchmark => 1 + Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_DockBenchmark_comboBox.SelectedIndex; })));
        /// <summary>
        /// 设定出击次数
        /// </summary>
        private int TotalFrequency => Convert.ToInt32(GetMain_Form.Invoke(new Func<decimal>(() => { return this.GetMain_Form.GameAttack_Frequency_numericUpDown.Value; })));
        /// <summary>
        /// 出击后等待时间（秒）
        /// </summary>
        private int WaitTime => Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() =>
        { return (Convert.ToInt32(this.GetMain_Form.GameAttack_WaitTime_textBox.Text) * 60 + Random(Convert.ToInt32(this.GetMain_Form.GameAttack_WaitTimeRan_textBox.Text) + 1)); })));
        /// <summary>
        /// 是否联合舰队模式
        /// </summary>
        private bool IsUnionFleet => false;
        private bool IsWaited = false;

        /// <summary>
        /// 出阵，追击判定
        /// </summary>
        /// <param name="stage">战斗场次</param>
        /// <returns></returns>
        private int Battle(int stage)
        {
            switch (stage)
            {
                case 1:
                    return Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Battle1_comboBox.SelectedIndex; })));
                case 2:
                    return Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Battle2_comboBox.SelectedIndex; })));
                case 3:
                    return Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Battle3_comboBox.SelectedIndex; })));
                case 4:
                    return Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Battle4_comboBox.SelectedIndex; })));
                case 5:
                    return Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Battle5_comboBox.SelectedIndex; })));
                default:
                    return 0;
            }
        }
        /// <summary>
        /// 战斗阵型
        /// </summary>
        /// <param name="stage">战斗场次</param>
        /// <returns></returns>
        private int Formation(int stage)
        {
            switch (stage)
            {
                case 1:
                    return Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Formation1_comboBox.SelectedIndex; })));
                case 2:
                    return Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Formation2_comboBox.SelectedIndex; })));
                case 3:
                    return Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Formation3_comboBox.SelectedIndex; })));
                case 4:
                    return Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Formation4_comboBox.SelectedIndex; })));
                case 5:
                    return Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => { return this.GetMain_Form.GameAttack_Formation5_comboBox.SelectedIndex; })));
                default:
                    return 1;
            }
        }

        /// <summary>
        /// 更改出击状态栏
        /// </summary>
        /// <param name="text">要改变的文字</param>
        /// <param name="fontColor">文字字体颜色</param>
        /// <param name="backColor">背景框颜色</param>
        /// <param name="bold">字体是否粗体</param>
        private void Set_ChangeStatus(string text, Color fontColor, Color backColor, bool bold = true) =>
            GetMain_Form.SetAttackStatus(text, fontColor, backColor, bold);
        protected override void Set_AddMessage(string text)
        {
            base.Set_AddMessage("出击:" + text);
        }
        /// <summary>
        /// 锁定控件
        /// </summary>
        /// <param name="isLock">是否可以控制控件</param>
        private void Set_LockControls(bool isLock)
        {
            if (GetMain_Form.InvokeRequired)
            {
                GetMain_Form.BeginInvoke(new Action<bool>(Set_LockControls), isLock);
            }
            else
            {
                GetMain_Form.GameAttack_Placement1_radioButton.Enabled = isLock;
                GetMain_Form.GameAttack_Placement2_radioButton.Enabled = isLock;
                GetMain_Form.GameAttack_Placement3_radioButton.Enabled = isLock;
                GetMain_Form.GameAttack_Seas_combobox.Enabled = isLock;
                GetMain_Form.GameAttack_Map_combobox.Enabled = isLock;
            }
        }

        protected override void Entrance()
        {
            try
            {
                AttackStart();
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                FunctionExceptionLog.Write($"发生致命错误，出击线程已关闭:", e);
                this.StopThread();
            }
        }
        private void AttackStart()
        {
            // 锁定控件
            Set_LockControls(false);
            // 重置出击数
            Frequency = 0;
            this.IsWaited = false;
            while (true)
            {
                if (Frequency >= TotalFrequency && TotalFrequency > 0)
                {
                    break;
                }
                Frequency++;
                Set_ChangeStatus($"{Frequency}:正在等待...", Color.White, Color.Lime);
                var waitTime = TimeSpan.FromSeconds(GetMain_Form.GetAttackPreWaitTime);
                do
                {
                    if (this.IsWaited)
                    {
                        break;
                    }
                    // 等待设定的出击前置时间
                    Set_ChangeStatus($"等待剩余时间：{((int)waitTime.TotalSeconds)}秒", Color.White, Color.Lime);
                    Delay(1000);
                    waitTime -= TimeSpan.FromSeconds(1);
                } while (waitTime >= TimeSpan.Zero);
                this.IsWaited = true;

                WaitQueue();
                // 工作代码                
                Set_AddMessage($"等待结束，开始执行第 {Frequency} 次出击。");
                WorkStart:
                try
                {
                    Set_ChangeStatus($"{Frequency}:正在执行出击", Color.White, Color.Orange);
                    Process1();
                    Process2();
                    Process3();
                    // 执行补给
                    TempSupply(new bool[] { true, true, true, true, true, true });
                    // 执行入渠
                    if (this.IsDock)
                    {
                        TempDock();
                    }
                }
                catch (ThreadAbortException) { }
                catch (Exceptions.ReactivateThreadException)
                {
                    // 在工作模式内重新开始线程
                    Delay(500);
                    goto WorkStart;
                }
                catch (Exceptions.TimeOutException)
                {
                    // 超时
                    OvertimeProcess();
                    // 超时判定结束后将会重新等待
                    Frequency--;
                }
                catch (Exceptions.DataErrorException e)
                {
                    // 数据异常
                    FunctionExceptionLog.Write($"数据异常，出击线程已关闭:", e);
                    this.StopThread();
                }
                IsWorking = false; // 结束工作
                if (Frequency >= TotalFrequency && TotalFrequency > 0)
                {
                    break;
                }
                WaitTimeProcess(); // 等待时间
            }

            this.StopThread();
        }
        /// <summary>
        /// 从母港进入地图
        /// </summary>
        private void Process1()
        {
            // 母港检测
            int i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.母港1)) break;
                else if (Judge(DataJudgePonds.远征回港) || Judge(DataJudgePonds.远征归来1) || Judge(DataJudgePonds.远征归来2))
                    break;
                Overtime(ref i, 5000);
            }

            // 母港
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.母港1))
                {
                    Click(DataClickPonds.母港出击);
                    break;
                }
                else if (Judge(DataJudgePonds.远征回港) || Judge(DataJudgePonds.远征归来1) || Judge(DataJudgePonds.远征归来2))
                {
                    Click(DataClickPonds.舰娘立绘);
                    i = 0;
                }
                Overtime(ref i, 30000);
            }

            // 出击选择
            i = 0;
            while (true)
            {
                if (Judge(new Data.DataJudge("140,77 @ 1EAAAB|1CB2B5|1DBDC0|1EAAAC|1EAAAC|208F90", "出击选择")))
                {
                    Click(new Data.DataClick("230,128|170,155|145,228|180,284|259,296|306,260|316,199|277,159", "出击"));
                    break;
                }
                Overtime(ref i, 30000);
            }

            // 海域选择
            i = 0;
            while (true)
            {
                if (Judge(new Data.DataJudge("574,122 @ 7D2D1F|671600|FFEFCE|82895B", "海域选择")))
                {
                    break;
                }
                Overtime(ref i, 30000);
            }
        }
        /// <summary>
        /// 选择出击海域
        /// </summary>
        private void Process2()
        {
            Set_ChangeStatus($"{Frequency}:正在选择海域", Color.White, Color.Orange);
            // 选择海域
            int i = 0;
            Data.DataJudge 出击海域判定数据 = new Data.DataJudge(new List<Data.DataJudge>()
           {
                new Data.DataJudge("142,450 @ E76110|EE9570|FFFFFF|E76110"),
                new Data.DataJudge("232,450 @ EE9570|E76110|EA7D4A|FFFFFF"),
                new Data.DataJudge("308,457 @ C55F20|FFAB6F|FFDB96|C96D2A"),
                new Data.DataJudge("371,453 @ F46306|D36D30|FFEEE6|E3C1BB"),
                new Data.DataJudge("463,448 @ F59150|F16722|E95F33|F29C88"),
                new Data.DataJudge("525,453 @ E76110|FDF7F4|EE9570|E76110")
           });
            Data.DataClick 出击海域选择数据 = new Data.DataClick(new List<Data.DataClick>()
           {
                new Data.DataClick("155,424|130,436|127,456|181,456|181,446"),
                new Data.DataClick("234,423|206,439|205,456|260,456|260,444"),
                new Data.DataClick("307,427|284,438|282,455|333,457|333,443"),
                new Data.DataClick("377,423|353,438|351,455|403,456|402,441"),
                new Data.DataClick("452,422|426,435|427,456|477,457|476,435"),
                new Data.DataClick("527,425|501,433|501,457|556,456|557,436")
           });
            while (true)
            {
                if (Judge(出击海域判定数据[this.Seas]))
                {
                    break;
                }
                else
                {
                    Click(出击海域选择数据[this.Seas]);
                    Overtime(ref i, 30000);
                }
            }

            // 选择地图
            i = 0;
            Data.DataClick 出击地图选择数据 = new Data.DataClick(new List<Data.DataClick>()
            {
                  new Data.DataClick("130,141|435,267"),
                  new Data.DataClick("465,142|663,268"),
                  new Data.DataClick("131,286|432,409"),
                  new Data.DataClick("464,286|663,412"),
                  new Data.DataClick("224,143|771,266"),
                  new Data.DataClick("221,284|769,404")
            });
            while (true)
            {
                if (Judge(new Data.DataJudge("574,122 @ 7D2D1F|671600|FFEFCE|82895B", "海域选择")))
                {// 2017.10.18更新应对
                    if (this.Map > 3)
                    {
                        // 扩张作战
                        if (Judge(new Data.DataJudge("700,279 @ 89100D|A10D00", "扩张作战")))
                        {
                            Click(new Data.DataClick("691,225|691,329|779,276", "选择扩张作战"));
                            Delay(300);
                            continue;
                        }
                        else if (Judge(new Data.DataJudge("138,276 @ EF2B2B|EC2B2B|FF2E2B", "从扩张作战返回")))
                        {
                            Click(出击地图选择数据[this.Map]);
                            break;
                        }
                        Overtime(ref i, 30000);
                        continue;
                    }
                    Click(出击地图选择数据[this.Map]);
                    break;
                }
                Overtime(ref i, 30000);
            }

            // 出击详细
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.出击详细))
                {
                    Click(DataClickPonds.出击决定);
                    //Click(new Data.DataClick("658,435|717,451", "出击决定"));// 2017.10.18更新应对
                    break;
                }
                Overtime(ref i, 30000);
            }

            // 舰队选择
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.出击舰队选择))
                {
                    // 可以出击
                    Set_ChangeStatus($"{Frequency}:检测状态中", Color.White, Color.Orange);
                    // 检测破损
                    if (DamagedReadiness(new bool[] { true, true, true, true, true, true }, this.DetectionStatus))
                    {
                        // 检测到破损舰船
                        if (this.IsDock)
                        {
                            if (this.DockBenchmark > this.DetectionStatus)
                            {
                                // 检测程度大于维修程度（出击检测为小破，维修选择大破，则无法维修小破），将导致无法出击
                                Set_ChangeStatus("无法修理的破损舰船，已停止！", Color.Red, Color.White);
                                Set_AddMessage("检测到无法修理的破损舰船，出击已停止！");
                                TempBack();
                                Delay(300);
                                this.StopThread();
                            }
                            // 开始入渠
                            TempDock();
                            throw new Exceptions.ReactivateThreadException("抛出一个错误，以用于再次执行脚本");
                        }
                        else
                        {
                            Set_ChangeStatus("检测到破损舰船，已停止！", Color.Red, Color.White);
                            Set_AddMessage("检测到破损舰船，出击已停止！");
                            TempBack();
                            Delay(300);
                            this.StopThread();
                        }
                    }
                    // 检测补给
                    if (!SupplyReadiness(new bool[] { true, true, true, true, true, true }))
                    {
                        // 检测到未补给舰船，开始补给
                        Set_ChangeStatus($"{Frequency}:检测到未补给舰船，开始补给。", Color.White, Color.Orange);
                        TempSupply(new bool[] { true, true, true, true, true, true });
                        throw new Exceptions.ReactivateThreadException("抛出一个错误，以用于再次执行脚本");
                    }
                    // 出击时检测队伍2状态
                    if (this.IsUnionFleet)
                    {
                        throw new NotImplementedException();
                    }

                    if (Judge(new Data.DataJudge("644,448 @ 330F0E|441516|3E1015", "出击开始可选")) ||
                        Judge(new Data.DataJudge("644,448 @ 6F0A02|7A1508|760D03", "出击开始选中")) ||
                        Judge(new Data.DataJudge("612,436 @ 320807|8F6D69|AD9B92", "出击开始可选(航空基地付)")) ||
                        Judge(new Data.DataJudge("612,436 @ 760B0C|BA605D|D29C99", "出击开始选中(航空基地付)"))
                        )
                    {


                        // 点击出击按钮
                        // 这里有坐标检测机制，必须使用偏移数据
                        if (Judge(new DataJudge("612,436 @ 320807|8F6D69|AD9B92", "出击开始可选(航空基地付)")) ||
                            Judge(new DataJudge("612,436 @ 760B0C|BA605D|D29C99", "出击开始选中(航空基地付)")))
                        {
                            Click(new DataClick("542,427|684,442", "出击开始(航空基地付)"));
                        }
                        else
                        {
                            Click(new DataClick("541,432|683,457", "出击开始"));
                        }
                        break;
                    }
                    else if (Judge(new Data.DataJudge("644,448 @ 252525|303030|272727", "出击开始不可选")) ||
                        Judge(new Data.DataJudge("612,436 @ 313131|565656|898989", "出击开始不可选(航空基地付)")))
                    {
                        // 不可出击
                        TempBack();
                        Set_ChangeStatus("舰队不可出击，已停止！", Color.Red, Color.White);
                        Set_AddMessage("检测到无法出击，已停止！");
                        Delay(300);
                        this.StopThread();
                    }
                }
                Overtime(ref i, 30000);
            }
        }
        /// <summary>
        /// 战斗地图到母港
        /// </summary>
        private void Process3()
        {
            var seas = this.Seas + 1;
            var map = this.Map + 1;
            bool backDecision;
            int i = 0;

            // 战斗场次
            int stage = 0;
            while (true)
            {
                stage++;
                // 输出战斗场次到控制台
                Set_ChangeStatus($"{Frequency}->{stage}:正在执行出击", Color.White, Color.Orange);
                Set_AddMessage($"地图：{seas}-{map}  场次：{stage}  阵型：{DataClickPonds.出击阵型6[Formation(stage)].Name}  " +
                    new Func<string>(() =>
                    {
                        switch (Battle(stage))
                        {
                            case 0: return "不追击";
                            case 1: return "追击";
                            case 2: throw new Exceptions.DataErrorException("追击判定数据异常");
                            default: throw new Exceptions.DataErrorException("追击判定数据异常");
                        }
                    }).Invoke());

                // 检测进入地图后的画面
                i = 0;
                while (true)
                {
                    if (Judge(new DataJudge("398,128 @ 72706C|1B1915|191713|AFADA9", "罗盘娘")) ||
                        Judge(new DataJudge("758,436 @ 2C989B|BCD0D1|A1C3C5|169396", "道具掉落（非舰船掉落）")))
                    {
                        Click(DataClickPonds.舰娘立绘);
                        i = 0;
                    }
                    else if (Judge(new DataJudge("387,3 @ 2DA9D7|41B8D3|5DC1D0", "战斗中")) ||
                        Judge(new DataJudge("386,6 @ 4EABCF|247A9C|004968", "夜战中")))
                    {
                        i = 0;
                        Delay(800);
                    }
                    else if (Judge(new DataJudge("52,33 @ 1DBDC0|222D30|1F7073|1DBDC0", "追击判定")) ||
                        Judge(new DataJudge("85,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "成果1")) ||
                        Judge(new DataJudge("49,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "成果2")))
                    {
                        break;
                    }
                    Delay(200);


                    //180520 重新修正选择阵型
                    if (Judge(new DataJudge("442,184 @ FFF6F2|5C8F8E|D2D7D4", "选择阵型")) ||
                         Judge(new DataJudge("442,184 @ FFF6F2|ED9A53|F9DAC6", "选择阵型(已摁下)")))
                    {
                        // 在这里加入是否有警戒阵的内容

                        if (Judge(new DataJudge("711,188 @ C6CFCC|F4EEEB|8FACAA|086D6E", "选择阵型(可选轮型)")) ||
                         Judge(new DataJudge("711,188 @ F7D3BA|FEEFE8|F1B485|E87E00", "选择阵型(可选轮型，已摁下)")))
                        {
                            // 可选阵型5
                            Click(DataClickPonds.出击阵型6[Formation(stage)]);
                            i = 0;
                            Delay(1000);
                        }
                        else
                        {
                            var formation = Formation(stage);
                            //  可选阵型4
                            if (formation == 0 || formation == 2)
                            {
                                Click(DataClickPonds.出击阵型6[0]);
                            }
                            else if (formation == 1)
                            {
                                Click(DataClickPonds.出击阵型6[1]);
                            }
                            else
                                Click(DataClickPonds.出击阵型6[Formation(stage)], OffsetX: 67);
                            i = 0;
                            Delay(1000);
                        }
                    }

                    // 6-3能动分歧选择下路
                    if (seas == 6 && map == 3)
                    {
                        if (Judge(new DataJudge("218,214 @ 79E7FF|79E7FF|79E7FF|79E7FF", "6-3能动分歧")))
                        {
                            Delay(200);
                            Click(new DataClick("291,295", "6-3能动分歧选择下路"));
                            Delay(200);
                        }
                    }


                    // 错误内容
                    //// 选择阵型
                    //// 171118新加阵型
                    //if ((Judge(new DataJudge("442,184 @ FFF6F2|5C8F8E|D2D7D4", "选择阵型")) ||
                    //     Judge(new DataJudge("442,184 @ FFF6F2|ED9A53|F9DAC6", "选择阵型(已摁下)"))) &&
                    //        (Judge(new DataJudge("711,188 @ C6CFCC|F4EEEB|8FACAA|086D6E", "选择阵型(可选轮型)")) ||
                    //     Judge(new DataJudge("711,188 @ F7D3BA|FEEFE8|F1B485|E87E00", "选择阵型(可选轮型，已摁下)"))))
                    //{
                    //    // 可选阵型6
                    //    Click(DataClickPonds.出击阵型6[Formation(stage)]);
                    //    i = 0;
                    //    Delay(1000);
                    //}
                    //else if ((Judge(new DataJudge("508,184 @ FFF6F2|5C8F8E|D2D7D4", "选择阵型")) ||
                    //     Judge(new DataJudge("508,184 @ FFF6F2|ED9A53|F9DAC6", "选择阵型(已摁下)")))
                    //     //&&
                    //     //   !(Judge(new DataJudge("711,188 @ C6CFCC|F4EEEB|8FACAA|086D6E", "选择阵型(可选轮型)")) ||
                    //     //Judge(new DataJudge("711,188 @ F7D3BA|FEEFE8|F1B485|E87E00", "选择阵型(可选轮型，已摁下)")))
                    //     )
                    //{
                    //    var formation = Formation(stage);
                    //    //  可选阵型5
                    //    if (formation == 0 || formation == 2)
                    //    {
                    //        Click(DataClickPonds.出击阵型6[0], OffsetX: 66);
                    //    }
                    //    else if (formation == 1)
                    //    {
                    //        Click(DataClickPonds.出击阵型6[1], OffsetX: 66);
                    //    }
                    //    else
                    //        Click(DataClickPonds.出击阵型6[Formation(stage)]);
                    //    i = 0;
                    //    Delay(1000);
                    //}

                    // 2017.12.11 恢复至5阵容
                    //if ((Judge(new DataJudge("442,184 @ FFF6F2|5C8F8E|D2D7D4", "选择阵型")) ||
                    //    Judge(new DataJudge("442,184 @ FFF6F2|ED9A53|F9DAC6", "选择阵型(已摁下)"))) &&
                    //       (Judge(new DataJudge("711,188 @ C6CFCC|F4EEEB|8FACAA|086D6E", "选择阵型(可选轮型)")) ||
                    //    Judge(new DataJudge("711,188 @ F7D3BA|FEEFE8|F1B485|E87E00", "选择阵型(可选轮型，已摁下)"))))
                    //{
                    //    // 可选阵型6
                    //    Click(DataClickPonds.出击阵型5[Formation(stage)]);
                    //    i = 0;
                    //    Delay(1000);
                    //}
                    //else if ((Judge(new DataJudge("528,339 @ FFF6F2|C6CFCC|D2D7D4|FFF6F2|B9C6C4|DEDFDC", "选择阵型（梯形阵）")) ||
                    //     Judge(new DataJudge("528,339 @ FFF6F2|F7D3BA|F9DAC6|FFF6F2|F6CBAE|FBE1D2", "选择阵型(梯形阵已摁下)")))
                    //     //&&
                    //     //   !(Judge(new DataJudge("711,188 @ C6CFCC|F4EEEB|8FACAA|086D6E", "选择阵型(可选轮型)")) ||
                    //     //Judge(new DataJudge("711,188 @ F7D3BA|FEEFE8|F1B485|E87E00", "选择阵型(可选轮型，已摁下)")))
                    //     )
                    //{
                    //    var formation = Formation(stage);
                    //    //  可选阵型5
                    //    if (formation == 0 || formation == 2)
                    //    {
                    //        Click(DataClickPonds.出击阵型5[0]);
                    //    }
                    //    else
                    //        Click(DataClickPonds.出击阵型5[Formation(stage)]);
                    //    i = 0;
                    //    Delay(1000);
                    //}

                    Delay(200);

                    // 联合舰队选择阵型
                    if (this.IsUnionFleet)
                    {
                        throw new NotImplementedException();
                    }

                    Delay(200);

                    if (Judge(DataJudgePonds.母港1)) return;
                    else if (Judge(DataJudgePonds.远征回港) || Judge(DataJudgePonds.远征归来1) || Judge(DataJudgePonds.远征归来2))
                    {
                        Click(DataClickPonds.舰娘立绘);
                        i = 0;
                        Delay(1000);
                    }
                    Overtime(ref i, 30000);
                }

                // 战斗中
                i = 0;
                while (true)
                {
                    if (Judge(new DataJudge("387,3 @ 2DA9D7|41B8D3|5DC1D0", "战斗中")) ||
                        Judge(new DataJudge("386,6 @ 4EABCF|247A9C|004968", "夜战中")))
                    {
                        i = 0;
                        Delay(1000);
                    }
                    else if (Judge(new DataJudge("52,33 @ 1DBDC0|222D30|1F7073|1DBDC0", "追击判定")))
                    {
                        // 选择是否追击
                        switch (Battle(stage))
                        {
                            case 0:
                                // 不追击
                                Click(DataClickPonds.追击判定[0]);
                                break;
                            case 1:
                                // 追击
                                Click(DataClickPonds.追击判定[1]);
                                break;
                            case 2:
                                // 无战斗
                                throw new Exceptions.DataErrorException("追击判定数据异常");
                            default:
                                throw new Exceptions.DataErrorException("追击判定数据异常");
                        }
                        i = 0;
                        Delay(1000);
                    }
                    Delay(200);
                    if (Judge(new DataJudge("85,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "成果1")) &&
                        Judge(new DataJudge("762,443 @ CDDADB", "出击成果可点"), null, 80))
                    {
                        i = 0;
                        Click(DataClickPonds.舰娘立绘);
                    }
                    else if (Judge(new DataJudge("49,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "成果2")) &&
                        Judge(new DataJudge("762,443 @ CDDADB", "出击成果可点"), null, 80))
                    {
                        // 战斗结束，开始检测破损
                        break;
                    }
                    Overtime(ref i, 30000);
                }

                // 战斗成果
                i = 0;
                while (true)
                {
                    if (Judge(new DataJudge("49,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "成果2")) &&
                        Judge(new DataJudge("762,443 @ CDDADB", "出击成果可点"), null, 80))
                    {
                        // 检测进击时是否有破损船
                        backDecision = AdvancingDamagedReadiness(new bool[] { true, true, true, true, true, true }, this.DetectionStatus);
                        // backDecision为真时回港，为假时进击
                        if (backDecision)
                        {
                            // 检测到破损舰船！将返回母港
                            Set_ChangeStatus($"{Frequency}->{stage}:检测到破损舰船！将返回母港", Color.Red, Color.Orange);
                            Set_AddMessage("检测到破损舰船！将返回母港！");
                        }

                        // 检测随伴舰队破损状态
                        if (this.IsUnionFleet)
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    }
                    Overtime(ref i, 30000);
                }

                // 回港判定
                i = 0;
                while (true)
                {
                    if ((Judge(new DataJudge("49,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "成果2")) &&
                        Judge(new DataJudge("762,443 @ CDDADB", "出击成果可点"), null, 80)) ||
                        Judge(new DataJudge("756,437 @ 36A7AA|D3E3E3|72B9BA", "舰船掉落"), null, 150) ||
                        Judge(new DataJudge("673,86 @ A2514D|FFD6D3|AF7879|8C343B", "旗舰大破")) ||
                        Judge(new DataJudge("758,436 @ 2C989B|BCD0D1|A1C3C5|169396", "道具掉落（非舰船掉落）")) ||
                        Judge(new DataJudge("750,428 @ D6DDDD|A1C3C6|169396|B2CCCC|3E9DA1|D6DDDD", "通关海域"), null, 20))
                    {
                        Click(DataClickPonds.舰娘立绘);
                        i = 0;
                        Delay(1000);
                    }
                    Delay(200);
                    if (Judge(new DataJudge("267,235 @ 718889|6A8A90|92B2BA", "进击与撤退")))
                    {
                        if (!backDecision)
                        {
                            // 当可以进击时
                            if (this.Battle(stage + 1) == 2)
                                // 决定撤退
                                backDecision = true;
                        }
                        if (backDecision)
                        {
                            // 撤退
                            Click(new DataClick("500,208|460,239|467,271|516,277|552,246|546,214", "撤退"));
                            i = 0;
                            Delay(1000);
                        }
                        else
                        {
                            // 进击
                            Click(new DataClick("283,203|241,238|250,273|299,279|341,247|333,211", "进击"));
                            Delay(1000);
                            break;
                        }
                    }
                    if (Judge(DataJudgePonds.母港1)) return;
                    else if (Judge(DataJudgePonds.远征回港) || Judge(DataJudgePonds.远征归来1) || Judge(DataJudgePonds.远征归来2))
                    {
                        Click(DataClickPonds.舰娘立绘);
                        i = 0;
                        Delay(1000);
                    }
                    Overtime(ref i, 30000);
                }
            }
        }
        /// <summary>
        /// 返回到母港
        /// </summary>
        private void TempBack()
        {
            int i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.母港1)) break;
                else if (Judge(DataJudgePonds.左侧返回母港1) || Judge(DataJudgePonds.左侧返回母港2))
                {
                    Click(DataClickPonds.左侧返回母港);
                    i = 0;
                    Delay(2000);
                }
                else if (Judge(DataJudgePonds.远征回港) || Judge(DataJudgePonds.远征归来1) || Judge(DataJudgePonds.远征归来2))
                {
                    Click(DataClickPonds.舰娘立绘);
                    i = 0;
                }
                Overtime(ref i, 30000);
            }
        }
        /// <summary>
        /// 自动补给
        /// </summary>
        private void TempSupply(bool[] GroupMember)
        {
            Set_ChangeStatus($"{Frequency}:正在补给", Color.White, Color.Orange);

            // 补给舰队
            void SubSupply(bool[] _groupMember)
            {
                补给队伍:
                Delay(200);
                int k = 0;
                if (!Judge(DataJudgePonds.补给页))
                { throw new Exceptions.TimeOutException("补给页判定异常错误！！"); }

                // 检测舰船数量
                int p = 0;
                for (k = 0; k < 6; k++)
                {
                    if (Judge(DataJudgePonds.补给舰娘存活List[k]))
                        p++;
                }
                p = 6 - p;// 队伍中的舰娘数量

                // 是否执行单舰补给
                bool isOnece = true;
                foreach (bool v in _groupMember)
                {
                    if (!v)
                    {
                        isOnece = false;
                        break;
                    }
                }
                if (isOnece == false)
                {
                    // 执行单舰补给
                    var 单舰补给选择框 = new DataClick("115,164|120,169");
                    for (int j = 0; j < p; j++)
                    {
                        if (_groupMember[j])
                        {
                            if (Judge(DataJudgePonds.补油判定List[j]) && Judge(DataJudgePonds.补弹判定List[j]))
                                continue;
                            else
                            {
                                k = 0;
                                while (true)
                                {
                                    if (Judge(new DataJudge("695,440 @ 494949|535353|A0A0A0|B9B9B9", "まとめと补给灰")) &&
                                        !(Judge(DataJudgePonds.补油判定List[j]) && Judge(DataJudgePonds.补弹判定List[j])))
                                    {
                                        Click(单舰补给选择框, 200, 0, 52 * j);
                                        k = 0;
                                    }
                                    else if (Judge(new DataJudge("695,440 @ 045758|285950|D1D6CB|FFF5F1", "まとめと补给可选")) ||
                                        Judge(new DataJudge("695,440 @ C5560D|B06F2C|FFCFAC|FFE0D3", "まとめと补给已选")))
                                    {
                                        Set_AddMessage("已执行单舰补给...");
                                        Click(new DataClick("671,435|740,448", "まとめて補給"));
                                        Delay(1000);
                                        k = 0;
                                    }
                                    else if (Judge(DataJudgePonds.补油判定List[j]) && Judge(DataJudgePonds.补弹判定List[j]))
                                        break;
                                    Overtime(ref k, 30000);
                                }
                            }
                        }
                    }
                    return;
                }

                for (int j = 0; j < p; j++)
                {
                    if (Judge(DataJudgePonds.补油判定List[j]) && Judge(DataJudgePonds.补弹判定List[j]))
                        continue;
                    else
                    {
                        // 执行一键补给 // 2015年10月9日UI更新应对
                        k = 0;
                        while (true)
                        {
                            if (Judge(DataJudgePonds.非全舰补给) || Judge(DataJudgePonds.全舰补给))
                            {
                                Set_AddMessage("已执行一键补给...");
                                Click(DataClickPonds.一括补给);
                                Delay(1000);
                                goto 补给队伍;
                            }
                            Overtime(ref k, 30000);
                        }
                    }
                }
            }

            TempBack();
            // 母港
            int i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.母港1))
                {
                    Click(new DataClick("75,189|46,218|73,248|108,221", "母港补给"));
                    Delay(200);
                    break;
                }
                else if (Judge(DataJudgePonds.远征回港) || Judge(DataJudgePonds.远征归来1) || Judge(DataJudgePonds.远征归来2))
                {
                    Click(DataClickPonds.舰娘立绘);
                    i = 0;
                }
                Overtime(ref i, 30000);
            }

            // 补给队伍1
            i = 0;
            while (true)
            {
                if (Judge(new DataJudge("646,179 @ E2E4D6|818375|797B6D|CCCEC0|EFF1E3|9A9C8E", "补给页")))
                {
                    if (Judge(new DataJudge("145,118 @ 23A0A1|23A0A1|D4E1DE|FFF6F2", "补给队伍1")))
                    {
                        SubSupply(GroupMember);
                        break;
                    }
                    else
                    {
                        Click(new DataClick("145,117|152,120", "补给队伍1"));
                        i = 0;
                    }
                }
                Overtime(ref i, 30000);
            }

            // 补给队伍2
            i = 0;
            if (this.IsUnionFleet)
            {
                while (true)
                {
                    if (Judge(new DataJudge("646,179 @ E2E4D6|818375|797B6D|CCCEC0|EFF1E3|9A9C8E", "补给页")))
                    {
                        if (Judge(new DataJudge("178,119 @ 23A0A1|D4E1DE|FFF6F2|53ADAD", "补给队伍2")))
                        {
                            SubSupply(GroupMember);
                            break;
                        }
                        else
                        {
                            Click(new DataClick("172,114|184,124", "补给队伍2"));
                            i = 0;
                        }
                    }
                    Overtime(ref i, 30000);
                }
            }

            TempBack();
        }
        /// <summary>
        /// 自动入渠队伍
        /// </summary>
        public void TempDock()
        {
            Set_ChangeStatus($"{Frequency}:正在入渠", Color.White, Color.Orange);
            TempBack();
            int i = 0;
            // 检测入渠标准
            bool Readiness(int line, int _status)
            {
                using (var bitmap = this.GetGameBitmap)
                {
                    if (_status <= 3)
                    {
                        if (Judge(new DataJudge("728,136 @ F2625A", "入渠舰娘大破坐标1"), bitmap, 0, 0, line * 31) &&
                            Judge(new DataJudge("742,143 @ FF655D", "入渠舰娘大破坐标2"), bitmap, 0, 0, line * 31))
                            return true;
                    }
                    if (_status <= 2)
                    {
                        if (Judge(new DataJudge("728,136 @ FFBC5D", "入渠舰娘中破坐标1"), bitmap, 0, 0, line * 31) &&
                            Judge(new DataJudge("742,143 @ F2B35A", "入渠舰娘中破坐标2"), bitmap, 0, 0, line * 31))
                            return true;
                    }
                    if (_status <= 1)
                    {
                        if (Judge(new DataJudge("728,136 @ FFEA5D", "入渠舰娘小破坐标1"), bitmap, 0, 0, line * 31) &&
                            Judge(new DataJudge("742,143 @ FFEA5D", "入渠舰娘小破坐标2"), bitmap, 0, 0, line * 31))
                            return true;
                    }
                    return false;
                }
            }
            // 使用高速修复舰船
            void BeginDock()
            {

                // 检测装備ステータス
                i = 0;
                while (true)
                {
                    if (Judge(new DataJudge("679,438 @ 204D4D|92CCC6|C3FFF9", "入渠开始")) ||
                        Judge(new DataJudge("679,438 @ DC872F|FFCA7F|FFE293", "入渠开始摁下")))
                    {
                        if (Judge(new DataJudge("731,289 @ FFF6F2|1E8786|1E8786|FFF6F2", "使用高速修复")))
                        {
                            Click(new DataClick("620,421|609,438|619,453|751,454|761,437|751,420", "入渠开始"));
                            break;
                        }
                        else if (Judge(new DataJudge("731,289 @ 3E3E3E|3E3E3E|3B3B3B", "不使用高速修复")))
                        {
                            Click(new DataClick("715,287|753,292", "使用高速修复"));
                            i = 0;
                            Delay(1000);
                        }
                    }
                    else if (Judge(new DataJudge("679,438 @ 404040|B2B2B2|E3E3E3", "入渠开始灰")))
                    {
                        throw new Exceptions.TimeOutException("检测到入渠开始按钮为灰色！");
                    }
                    Overtime(ref i, 30000);
                }

                // 舰船入渠确认
                i = 0;
                while (true)
                {
                    if (Judge(new DataJudge("495,397 @ 1E8786|80AEAC|FFF6F2", "舰船确认")))
                    {
                        Delay(200);
                        Set_AddMessage("已高速修复了一艘舰船...");
                        Click(new DataClick("480,396|532,403", "入渠确认是"));
                        break;
                    }
                    Overtime(ref i, 30000);
                }
            }

            // 母港:
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.母港1))
                {
                    Click(new DataClick("122,336|103,344|100,376|126,387|150,369|145,346", "母港入渠"));
                    Delay(500);
                    break;
                }
                else if (Judge(DataJudgePonds.远征回港) ||
                    Judge(DataJudgePonds.远征归来1) ||
                    Judge(DataJudgePonds.远征归来2))
                {
                    Click(DataClickPonds.舰娘立绘);
                    i = 0;
                }
                Overtime(ref i, 30000);
            }

            // 入渠页
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.入渠页))
                {
                    if (Judge(new DataJudge("256,168 @ D9D6C4|B5B2A0|D3CFC3", "入渠空渠位1")))
                    {
                        break;
                    }
                    else
                    {
                        Set_AddMessage("检测到入渠位1已占用！！！无法开始自动入渠。");
                        for (int j = 0; j < 30; j++)
                        {
                            Set_ChangeStatus($"{Frequency}:检测到入渠位1已占用！！！", Color.White, Color.Red);
                            Delay(500);
                            Set_ChangeStatus($"{Frequency}:检测到入渠位1已占用！！！", Color.Black, Color.Ivory);
                            Delay(500);
                        }
                        TempBack();
                        return;
                    }
                }
                Overtime(ref i, 30000);
            }

            下一舰船:
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.入渠页))
                {
                    if (Judge(new DataJudge("256,168 @ D9D6C4|B5B2A0|D3CFC3", "入渠空渠位1")))
                    {
                        Click(new DataClick("178,149|328,178", "入渠槽1"));
                        Delay(500);
                        break;
                    }
                }
                Overtime(ref i, 30000);
            }

            // 将符合基准的破损舰引入修理渠
            // 舰船选择
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.入渠舰船选择))
                {
                    if (Judge(new DataJudge("769,115 @ 249596|FFFFFF|249596|FFFFFF|249596", "舰船选择破损度排列"), null, 20))
                        break;
                    else
                    {
                        Click(new DataClick("753,103|789,118", "舰船排列"));
                        i = 0;
                    }
                }
                Overtime(ref i, 30000);
            }


            // 检测可入渠舰娘 // 仅检测第一页
            i = 0; Delay(500);
            for (int j = 0; j < 10; j++)
            {
                while (true)
                {
                    if (Judge(DataJudgePonds.入渠舰船选择))
                    {
                        if ((Judge(new DataJudge("390,135 @ F1EEEA|F4F0ED|7CC0BF", "队伍1标志"), null, 0, 0, j * 31) ||
                       (this.IsUnionFleet && Judge(new DataJudge("391,138 @ 41A9AA|CDDEDB|D4E2DF", "队伍2标志"), null, 0, 0, j * 31))))
                        {
                            if (Readiness(j, this.DockBenchmark))
                            {
                                // 开始入渠
                                Click(new DataClick("410,132|723,138", "入渠舰娘1"), 200, 0, j * 31);
                                BeginDock();// 开始入渠
                                i = 0;
                                goto 下一舰船;
                            }
                            else
                            {
                                i = 0;
                                break;
                            }
                        }
                        else
                        {
                            i = 0;
                            break;
                        }
                    }
                    Overtime(ref i, 30000);
                }
            }
            TempBack();
        }



        /// <summary>
        /// 等待系统
        /// </summary>
        private void WaitTimeProcess()
        {
            TimeSpan time = SecondToTime(WaitTime);
            var waitTime = Convert.ToInt32(time.TotalMilliseconds);
            while (waitTime > 0)
            {
                if (IsDisposed) this.Thread.Abort();// 检测线程是否被关闭
                time = MillisecondToTime(waitTime);
                Set_ChangeStatus($"{Frequency}:剩余时间 {(time.Hours * 24 + time.Hours) * 60 + time.Minutes}:{time.Seconds}", Color.White, Color.Lime);
                Delay(1000);
                waitTime -= 1000;
            }
            Set_ChangeStatus($"{Frequency}:剩余时间 0:0", Color.White, Color.Lime);
            Delay(200);
        }
        /// <summary>
        /// 超时系统
        /// </summary>
        private void OvertimeProcess()
        {
            Delay(200);
            if (Judge(DataJudgePonds.母港1)) return;
            DateTime Pt = DateTime.Now;//超时时刻
            TimeSpan Ptm = TimeSpan.Zero;
            Set_AddMessage("界面判定已超时。");
            Set_ChangeStatus($"{Frequency}:已超时： {((int)Ptm.TotalSeconds).ToString()}秒。", Color.White, Color.Red);

            var 战斗中 = new DataJudge("387,3 @ 2DA9D7|41B8D3|5DC1D0", "战斗中");
            var 夜战中 = new DataJudge("386,6 @ 4EABCF|247A9C|004968", "夜战中");

            var 罗盘娘 = new DataJudge("398,128 @ 72706C|1B1915|191713|AFADA9", "罗盘娘");
            var 成果1 = new DataJudge("85,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "成果1");
            var 成果2 = new DataJudge("49,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "成果2");
            var 舰船掉落 = new DataJudge("756,437 @ 36A7AA|D3E3E3|72B9BA", "舰船掉落");
            var 旗舰大破 = new DataJudge("673,86 @ A2514D|FFD6D3|AF7879|8C343B", "旗舰大破");
            var 道具掉落 = new DataJudge("758,436 @ 2C989B|BCD0D1|A1C3C5|169396", "道具掉落（非舰船掉落）");
            var 不追击与夜战突入 = new DataJudge("52,33 @ 1DBDC0|222D30|1F7073|1DBDC0", "不追击与夜战突入");
            var 进击与撤退 = new DataJudge("267,235 @ 718889|6A8A90|92B2BA", "进击与撤退");
            var 选择阵型 = new DataJudge("442,184 @ FFF6F2|5C8F8E|D2D7D4", "选择阵型");
            var 选择阵型摁下 = new DataJudge("442,184 @ FFF6F2|ED9A53|F9DAC6", "选择阵型(已摁下)");
            var 入渠舰船确认 = new DataJudge("495,397 @ 1E8786|80AEAC|FFF6F2", "舰船确认");

            while (true)
            {
                if (IsDisposed) this.Thread.Abort();// 检测线程是否被关闭
                Delay(1000);
                if (IsDisposed) this.Thread.Abort();
                Ptm = DateTime.Now - Pt;
                Set_ChangeStatus($"{Frequency}:已超时： {((int)Ptm.TotalSeconds).ToString()}秒。", Color.White, Color.Red);

                if (Judge(DataJudgePonds.母港1))
                {
                    Delay(200);
                    Set_ChangeStatus($"{Frequency}:超时检测已结束，共超时： {((int)Ptm.TotalSeconds).ToString()}秒。", Color.White, Color.Red);
                    return;
                }
                else if (Judge(DataJudgePonds.远征回港) ||
                    Judge(DataJudgePonds.远征归来1) ||
                    Judge(DataJudgePonds.远征归来2))
                { Click(DataClickPonds.舰娘立绘); continue; }
                Delay(200);


                if (Judge(DataJudgePonds.左侧返回母港1) || Judge(DataJudgePonds.左侧返回母港2))
                { Click(new DataClick("60,223|86,292", "左侧返回母港")); continue; }
                else if (Judge(战斗中) || Judge(夜战中)) continue;
                Delay(200);

                // 战斗地图中
                if (Judge(罗盘娘) || Judge(成果1) || Judge(成果2)
                    || Judge(舰船掉落, null, 200) || Judge(旗舰大破) || Judge(道具掉落))
                { Click(DataClickPonds.舰娘立绘); continue; }
                Delay(200);
                if (Judge(不追击与夜战突入))
                { Click(new DataClick("285,208|239,236|249,270|298,277|341,246|331,209", "追撃せず")); continue; }
                else if (Judge(进击与撤退))
                { Click(new DataClick("500,208|460,239|467,271|516,277|552,246|546,214", "撤退")); continue; }

                // 入渠
                if (Judge(入渠舰船确认))
                {
                    Click(new DataClick("288,394|342,406", "入渠确认否"));
                }

                // 例外，将关闭线程
                if (Judge(选择阵型) || Judge(选择阵型摁下))
                {
                    Set_AddMessage("脚本遇到了侵蚀鱼雷的袭击已被击沉。");
                    Set_ChangeStatus("脚本遇到了侵蚀鱼雷的袭击已被击沉。", Color.Red, Color.White);
                    this.StopThread();
                }
            }
        }


        /// <summary>
        /// 开启出击线程
        /// </summary>
        public override void StartThread()
        {
            try
            {
                base.StartThread();
            }
            catch (Exceptions.NoPermitException)
            {
                Set_ChangeStatus("软件尚未激活！", Color.Red, Color.White);
                this.StopThread();
            }
            catch (Exceptions.NoGameHandleException)
            {
                Set_ChangeStatus("未检测到游戏窗口！", Color.Red, Color.White);
                this.StopThread();
            }
        }
        /// <summary>
        /// 关闭出击出击线程
        /// </summary>
        public override void StopThread()
        {
            if (IsDisposed) return;
            this.IsWorking = false;
            // 解除锁定控件
            Set_LockControls(true);
            Set_AddMessage("自动出击已关闭。");
            if (GetMain_Form.GameAttack_Status_textBox.BackColor != Color.White)
            {
                Set_ChangeStatus("出击已停止", Color.Black, Color.White, false);
            }
            base.StopThread();
        }
        public Attack() : base("自动出击")
        {
            this.StartThread();
        }
    }
}