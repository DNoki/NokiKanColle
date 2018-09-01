using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Threading;
using System.IO;
using System.Net;

using NokiKanColle.Function;
using static NokiKanColle.Function.Functions;
using static NokiKanColle.Data.DataPond;
using NokiKanColle.Data;

namespace NokiKanColle.Utility.Process
{
    /// <summary>
    /// 远征控制类
    /// </summary>
    public class Expedition : GameProcessWrapper
    {
        /// <summary>
        /// 舰队派遣次数
        /// </summary>
        private List<int> _frequency = new List<int>() { 0, 0, 0 };
        /// <summary>
        /// 已执行单舰派遣
        /// </summary>
        private bool _singleDepart = false;
        /// <summary>
        /// 临时记录派遣的远征ID
        /// </summary>
        private List<Color> _tempExpeditionID = new List<Color>();

        /// <summary>
        /// 更改远征状态栏
        /// </summary>
        /// <param name="text">要改变的文字</param>
        /// <param name="fontColor">文字字体颜色</param>
        /// <param name="backColor">背景框颜色</param>
        /// <param name="bold">字体是否粗体</param>
        private void Set_ChangeStatus(string text, Color fontColor, Color backColor, bool bold = true)
        {
            if (GetMain_Form.InvokeRequired)
            {
                GetMain_Form.BeginInvoke(new Action<string, Color, Color, bool>(Set_ChangeStatus), text, fontColor, backColor, bold);
            }
            else
            {
                GetMain_Form.GameExpedition_Status_textBox.Text = text;
                GetMain_Form.GameExpedition_Status_textBox.ForeColor = fontColor;
                GetMain_Form.GameExpedition_Status_textBox.BackColor = backColor;
                if (bold)
                    GetMain_Form.GameExpedition_Status_textBox.Font = new Font(GetMain_Form.GameExpedition_Status_textBox.Font, FontStyle.Bold);
                else
                    GetMain_Form.GameExpedition_Status_textBox.Font = new Font(GetMain_Form.GameExpedition_Status_textBox.Font, FontStyle.Regular);
            }
        }
        /// <summary>
        /// 添加一条信息到远征信息栏
        /// </summary>
        /// <param name="text"></param>
        protected override void Set_AddMessage(string text)
        {
            base.Set_AddMessage("远征:" + text);
            /*
            if (GetMain_Form.InvokeRequired)
            {
                GetMain_Form.BeginInvoke(new Action<string>(Set_AddMessage), text);
            }
            else
            {
                if (GetMain_Form.GameExpedition_Massage_textBox.Lines.Length > 120)
                {
                    var temp = GetMain_Form.GameExpedition_Massage_textBox.Lines;
                    var newLines = new string[temp.Length - 10];
                    Array.Copy(temp, 0, newLines, 0, newLines.Length);
                    GetMain_Form.GameExpedition_Massage_textBox.Lines = newLines;
                }
                GetMain_Form.GameExpedition_Massage_textBox.SelectionStart = 0;
                var time = DateTime.Now;
                GetMain_Form.GameExpedition_Massage_textBox.SelectedText += $"{time.ToShortDateString()} {time.ToLongTimeString()}: {text.Replace("\n", "|")}" + Environment.NewLine;
            }*/
        }
        /// <summary>
        /// 取得队伍远征海域
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        private int Get_ExpeditionNumber(int team)
        {
            if (GetMain_Form.InvokeRequired)
            {
                return (int)GetMain_Form.Invoke(new Func<int, int>(Get_ExpeditionNumber), team);
            }
            else
            {
                switch (team)
                {
                    case 2: return GetMain_Form.GameExpedition_ExpeditionNumber2_comboBox.SelectedIndex + 1;
                    case 3: return GetMain_Form.GameExpedition_ExpeditionNumber3_comboBox.SelectedIndex + 1;
                    case 4: return GetMain_Form.GameExpedition_ExpeditionNumber4_comboBox.SelectedIndex + 1;
                    default: return -1;
                }
            }
        }

        /// <summary>
        /// 当计时器存在且计时结束时返回真
        /// </summary>
        /// <param name="i">计时器编号</param>
        /// <returns></returns>
        private bool TimerCanRun(int i)
        {
            ExpeditionTimer timer;
            timer = Function.FunctionThread.GetThread<ExpeditionTimer>("远征计时器_" + i.ToString());
            if (timer != null)
            {
                if (timer.IsThreadAlive && !timer.IsWorking)
                {
                    return true;
                }
                else return false;
            }
            return false;
        }
        /// <summary>
        /// 获取舰队派遣次数
        /// </summary>
        /// <param name="team">队伍编号</param>
        /// <returns></returns>
        public int GetFrequency(int team)
        {
            if (team > 4 || team < 2) return 0;
            return _frequency[team - 2];
        }
        /// <summary>
        /// 重置舰队派遣次数（0为重置全部）
        /// </summary>
        /// <param name="team">队伍编号（0为重置全部）</param>
        public void FrequencyReset(int team)
        {
            if (team == 0)
            {
                for (int i = 0; i < _frequency.Count; i++)
                {
                    _frequency[i] = 0;
                }
            }
            else
                _frequency[team - 2] = 0;
        }

        /// <summary>
        /// 远征线程入口
        /// </summary>
        protected override void Entrance()
        {
            try
            { ExpeditionStart(); }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                FunctionExceptionLog.Write($"发生致命错误，远征线程已关闭:", e);
                this.StopThread();
            }
        }
        private void ExpeditionStart()
        {
            while (true)
            {
                // 帰投した艦隊はあるかないか
                if ((TimerCanRun(2) || TimerCanRun(3) || TimerCanRun(4))) { }
                else
                {
                    Set_ChangeStatus("正在远征", Color.White, Color.Lime);
                    Delay(1000);
                    continue;
                }
                Set_ChangeStatus("正在等待", Color.White, Color.Lime);
                WaitQueue();

                // 工作代码
                var waitTime = GetMain_Form.GetExpeditionWaitTime;
                do
                {
                    // 等待设定的收取等待时间
                    Set_ChangeStatus($"等待剩余时间：{((int)waitTime.TotalSeconds)}秒", Color.White, Color.Lime);
                    Delay(1000);
                    waitTime -= TimeSpan.FromSeconds(1);
                } while (waitTime >= TimeSpan.Zero);

                Set_AddMessage("计时结束，开始远征。");

                WorkStart:
                try
                {
                    var teams = new bool[] { TimerCanRun(2), TimerCanRun(3), TimerCanRun(4) };
                    if (teams[0] == false && teams[1] == false && teams[2] == false) goto WorkEnd;

                    if (_singleDepart)
                    {
                        // 执行单舰等待时间
                        _singleDepart = false;
                        var waitTimeForSingleDepart = GetMain_Form.GetExpeditionSingleDepartWaitTime;
                        do
                        {
                            Set_ChangeStatus($"单舰派遣等待时间：{(int)waitTimeForSingleDepart.TotalSeconds}秒", Color.White, Color.Lime);
                            Delay(1000);
                            waitTimeForSingleDepart -= TimeSpan.FromSeconds(1);
                        } while (waitTimeForSingleDepart >= TimeSpan.Zero);
                    }
                    JudgeIsOver();

                    Process1();
                    teams = new bool[] { TimerCanRun(2), TimerCanRun(3), TimerCanRun(4) };
                    if (teams[0] == false && teams[1] == false && teams[2] == false) goto WorkEnd;

                    Process2(teams);
                    var temp = new bool[] { TimerCanRun(2), TimerCanRun(3), TimerCanRun(4) };
                    if (teams[0] == false && teams[1] == false && teams[2] == false) goto WorkEnd;

                    for (int i = 0; i < teams.Length; i++)
                    {
                        if (teams[i] != temp[i])
                            goto WorkStart;
                    }
                    teams = temp;
                    Process3(teams);
                    if (_singleDepart)
                    {
                        Delay(500);
                        goto WorkStart;
                    }

                    JudgeIsOver();
                }
                catch (ThreadAbortException)
                {
                    if (GetMain_Form.GameExpedition_Status_textBox.BackColor != Color.White)
                    {
                        Set_ChangeStatus("远征已停止", Color.Black, Color.White, false);
                    }
                }
                catch (Exceptions.ReactivateThreadException)
                {
                    // 在工作模式内重新开始线程
                    Delay(500);
                    goto WorkStart;
                }
                catch (Exceptions.TimeOutException)
                {
                    // 超时类错误
                    OvertimeProcess();
                    goto WorkStart;
                }
                catch (Exceptions.DataErrorException e)
                {
                    // 数据异常
                    Set_AddMessage($"数据异常，远征线程已关闭:{e.Message}");
                    this.StopThread();
                }
                WorkEnd:
                // 考虑返回结束工作信息的封装
                IsWorking = false;//结束工作
            }
        }
        /// <summary>
        /// 收回归投队伍
        /// </summary>
        private void Process1()
        {
            Set_ChangeStatus("正在返回母港", Color.White, Color.Orange);

            // 检测母港
            int i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.母港1))
                {
                    Click(DataClickPonds.母港出击);
                    break;
                }
                else if (Judge(DataJudgePonds.远征回港) ||
                    Judge(DataJudgePonds.远征归来1) ||
                    Judge(DataJudgePonds.远征归来2))
                {
                    goto 检测远征归投;
                }
                Overtime(ref i, 2000);
            }

            // 检测进击页:
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.出击选择))
                {
                    Click(DataClickPonds.左侧返回母港);
                    Delay(200);
                    break;
                }
                Overtime(ref i, 30000);
            }

            检测远征归投: i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.远征回港) ||
                   Judge(DataJudgePonds.远征归来1) ||
                   Judge(DataJudgePonds.远征归来2))
                {
                    Click(DataClickPonds.舰娘立绘);
                    Delay(1000);
                    i = 0;
                }
                if (Judge(DataJudgePonds.母港1))//完成一次收取归投成果，返回检测母港
                {
                    break;
                }
                Overtime(ref i, 30000);
            }
        }
        /// <summary>
        /// 补给舰队
        /// </summary>
        /// <param name="teams">需要补给的队伍</param>
        private void Process2(bool[] teams)
        {
            Set_ChangeStatus("正在补给", Color.White, Color.Orange);
            int i = 0;
            // 检测母港
            while (true)
            {
                if (Judge(DataJudgePonds.母港1))
                {
                    Click(DataClickPonds.补给); break;
                }
                else if (Judge(DataJudgePonds.远征回港) ||
                   Judge(DataJudgePonds.远征归来1) ||
                   Judge(DataJudgePonds.远征归来2))
                {
                    Click(DataClickPonds.舰娘立绘);
                    Delay(1000);
                    i = 0;
                    //throw new Exceptions.NewFleetReturningException("检测到新的归投队伍！");
                }
                Overtime(ref i, 30000);
            }

            // 补给队伍2
            i = 0;
            while (true)
            {
                if (!teams[0]) break;
                if (Judge(DataJudgePonds.补给页))// 检测补给页
                {
                    if (Judge(DataJudgePonds.补给队伍2))
                    {
                        if (Judge(DataJudgePonds.补给舰娘正在远征))// 判断队伍是否处于远征状态
                            break;
                        Process2Sub1(2);
                        break;
                    }
                    else
                    {
                        Click(DataClickPonds.补给队伍2);
                        i = 0;
                    }
                }
                Overtime(ref i, 30000);
            }

            // 补给队伍3
            i = 0;
            while (true)
            {
                if (!teams[1]) break;
                if (Judge(DataJudgePonds.补给页))// 检测补给页
                {
                    if (Judge(DataJudgePonds.补给队伍3))
                    {
                        if (Judge(DataJudgePonds.补给舰娘正在远征))// 判断队伍是否处于远征状态
                            break;
                        Process2Sub1(3);
                        break;
                    }
                    else
                    {
                        Click(DataClickPonds.补给队伍3);
                        i = 0;
                    }
                }
                Overtime(ref i, 30000);
            }

            // 补给队伍4
            i = 0;
            while (true)
            {
                if (!teams[2]) break;
                if (Judge(DataJudgePonds.补给页))// 检测补给页
                {
                    if (Judge(DataJudgePonds.补给队伍4))
                    {
                        if (Judge(DataJudgePonds.补给舰娘正在远征))// 判断队伍是否处于远征状态
                            break;
                        Process2Sub1(4);
                        break;
                    }
                    else
                    {
                        Click(DataClickPonds.补给队伍4);
                        i = 0;
                    }
                }
                Overtime(ref i, 30000);
            }

            // 返回母港
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.母港1)) break;
                else if (Judge(DataJudgePonds.补给页))
                {
                    Click(DataClickPonds.左侧返回母港);
                    Delay(200);
                    i = 0;
                }
                else if (Judge(DataJudgePonds.远征回港) ||
                   Judge(DataJudgePonds.远征归来1) ||
                   Judge(DataJudgePonds.远征归来2))
                {
                    Click(DataClickPonds.舰娘立绘);
                    Delay(200);
                    i = 0;
                }
                Overtime(ref i, 30000);
            }
        }
        private void Process2Sub1(int team)
        {
            Delay(200);
            检测补给页:
            if (!Judge(DataJudgePonds.补给页))
            {
                throw new Exceptions.TimeOutException("补给页判定异常错误！！");
            }

            int p = 0, i = 0;
            for (i = 0; i < 6; i++)
            {
                if (Judge(DataJudgePonds.补给舰娘存活List[i]))
                    p++;
            }
            p = 6 - p;//队伍中的舰娘数量

            for (int a = 0; a < p; a++)
            {
                if (Judge(DataJudgePonds.补油判定List[a]) && Judge(DataJudgePonds.补弹判定List[a]))
                    continue;
                else
                {
                    // 执行一键补给 // 2015年10月9日UI更新应对
                    i = 0;
                    while (true)
                    {
                        if (Judge(DataJudgePonds.非全舰补给) || Judge(DataJudgePonds.全舰补给))
                        {
                            if ((team == 2 && !TimerCanRun(2)) ||
                                (team == 3 && !TimerCanRun(3)) ||
                                (team == 4 && !TimerCanRun(4)))
                                return;//急停
                            Click(DataClickPonds.一括补给);
                            Delay(200);
                            goto 检测补给页;
                        }
                        Overtime(ref i, 30000);
                    }
                }
            }
        }
        /// <summary>
        /// 派遣舰队
        /// </summary>
        /// <param name="teams">需要派遣的队伍</param>
        private void Process3(bool[] teams)
        {
            Set_ChangeStatus("正在派遣", Color.White, Color.Orange);
            // 检测母港
            int i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.母港1))
                {
                    Click(DataClickPonds.母港出击);
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

            // 检测出击页
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.出击选择))
                {
                    Click(DataClickPonds.远征);
                    break;
                }
                Overtime(ref i, 30000);
            }

            // 检测远征页
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.远征选择))
                    break;
                Overtime(ref i, 30000);
            }

            _singleDepart = false;// 是否单舰派遣（真：已有舰队派遣）
            _tempExpeditionID.Clear();

            // 派遣队伍2
            i = 0;
            int ExpeditionNumber = Get_ExpeditionNumber(2);
            while (true)
            {
                if (!teams[0] || _singleDepart) break;
                Process3Sub1(2, ExpeditionNumber);
                Process3Sub2(2, ExpeditionNumber);
                _singleDepart = GetMain_Form.GetExpeditionIsSingleDepart;
                Delay(1000);
                break;
                //Overtime(ref i, 30000);
            }

            // 派遣队伍3
            i = 0;
            ExpeditionNumber = Get_ExpeditionNumber(3);
            for (int j = 0; j < 3; j++)
            {
                var t = Get_ExpeditionNumber(j + 2);
                if (41 <= t && t <= 44)
                {
                    if ((ExpeditionNumber < Get_ExpeditionNumber(2)))
                        throw new Exceptions.ReactivateThreadException();
                }
            }
            while (true)
            {
                if (!teams[1] || _singleDepart) break;
                Process3Sub1(3, ExpeditionNumber);
                Process3Sub2(3, ExpeditionNumber);
                _singleDepart = GetMain_Form.GetExpeditionIsSingleDepart;
                Delay(1000);
                break;
                //Overtime(ref i, 30000);
            }

            // 派遣队伍4
            i = 0;
            ExpeditionNumber = Get_ExpeditionNumber(4);
            for (int j = 0; j < 3; j++)
            {
                var t = Get_ExpeditionNumber(j + 2);
                if (41 <= t && t <= 44)
                {
                    if ((ExpeditionNumber < Get_ExpeditionNumber(2)))
                        throw new Exceptions.ReactivateThreadException();
                }
            }
            while (true)
            {
                if (!teams[2] || _singleDepart) break;
                Process3Sub1(4, ExpeditionNumber);
                Process3Sub2(4, ExpeditionNumber);
                _singleDepart = GetMain_Form.GetExpeditionIsSingleDepart;
                break;
                //Overtime(ref i, 30000);
            }

            // 返回母港:
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.远征选择) || Judge(DataJudgePonds.远征详细))
                {
                    Click(DataClickPonds.左侧返回母港);
                    Delay(200);
                    i = 0;
                }
                else if (Judge(DataJudgePonds.远征回港) ||
                   Judge(DataJudgePonds.远征归来1) ||
                   Judge(DataJudgePonds.远征归来2))
                {
                    Click(DataClickPonds.舰娘立绘);
                    Delay(200);
                    i = 0;
                }
                else if (Judge(DataJudgePonds.母港1))
                {
                    return;
                }
                Overtime(ref i, 30000);
            }
        }
        private void Process3Sub1(int team, int ExpeditionNumber)
        {
            var detectedIterations = 0;
            Start:
            Delay(1000);
            int seasNumber = 0, itemsNumber = 0;
            if (ExpeditionNumber < 9 || (40 < ExpeditionNumber && ExpeditionNumber < 44)) seasNumber = 1;
            else if ((8 < ExpeditionNumber && ExpeditionNumber < 17) || (43 < ExpeditionNumber && ExpeditionNumber < 45)) seasNumber = 2;
            else if ((16 < ExpeditionNumber && ExpeditionNumber < 25)) seasNumber = 3;
            else if (24 < ExpeditionNumber && ExpeditionNumber < 33) seasNumber = 4;
            else if (32 < ExpeditionNumber && ExpeditionNumber < 41) seasNumber = 5;
            else throw new Exceptions.DataErrorException("远征海域数据错误！");
            if (ExpeditionNumber < 9) itemsNumber = ExpeditionNumber;
            else if (8 < ExpeditionNumber && ExpeditionNumber < 17) itemsNumber = ExpeditionNumber - 8;
            else if (16 < ExpeditionNumber && ExpeditionNumber < 25) itemsNumber = ExpeditionNumber - 16;
            else if (24 < ExpeditionNumber && ExpeditionNumber < 33) itemsNumber = ExpeditionNumber - 24;
            else if (32 < ExpeditionNumber && ExpeditionNumber < 41) itemsNumber = ExpeditionNumber - 32;
            else
            {
                if (40 < ExpeditionNumber && ExpeditionNumber < 45) itemsNumber = ExpeditionNumber;
                else throw new Exceptions.DataErrorException("远征海域数据错误！");
            }

            // 检测远征页
            int i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.远征选择))
                {
                    break;
                }
                else if (Judge(DataJudgePonds.远征舰队选择))
                {
                    Click(DataClickPonds.远征海域List[0]);
                    i = 0;
                }
                Overtime(ref i, 30000);
            }

            选择远征海域:
            i = 0;
            while (true)
            {
                if (seasNumber < 1 || seasNumber > 5 ||
                    itemsNumber < 1 || (itemsNumber > 8 && itemsNumber < 41) ||
                    (itemsNumber > 44))
                    throw new Exceptions.DataErrorException("远征海域数据错误！");
                if (Judge(DataJudgePonds.远征图List[seasNumber - 1]))
                {
                    // 于2017年11月6日，添加A1、A2、A3、B1的远征功能
                    if ((40 < itemsNumber && itemsNumber < 45))
                    {
                        Process3SubForAB(seasNumber, itemsNumber);
                    }
                    else
                        // 选择远征项目
                        Click(DataClickPonds.远征项List[itemsNumber - 1]);

                    // 检测远征决定
                    try
                    {
                        int j = 0;
                        while (true)
                        {
                            if (Judge(DataJudgePonds.远征决定List[0]) || Judge(DataJudgePonds.远征决定List[1]))
                            {
                                Click(DataClickPonds.远征决定);
                                break;
                            }
                            else if (Judge(DataJudgePonds.远征中止与归还List[0]) || Judge(DataJudgePonds.远征中止与归还List[1]))
                            {
                                //已在远征或成功派遣，开始读取时间
                                goto 检测舰队远征时间;
                            }
                            else if (Judge(new DataJudge("727,388 @ 817C75|D9D2C7|E5DED2|E5DED4|E5DED4|E5DED4|E5DED4|E5DED4|635E58|F1EADF,817C75|D9D2C7|E4DDD1|827D76|827D76|827D76|827D76|827D76|57534D|F1EADF,817C75|D8D1C6|C4BEB4|C4BEB5|C4BEB5|C5BFB6|C5BFB6|C5BFB6|635E58|F1EADF,817C75|D7D1C5|B4AEA4|B4AEA6|B4AEA6|B4AEA6|B5AFA6|B5AFA6|B5AFA6|F2EBE0,817C75|D6D0C4|827D76|837E77|837E77|837E77|837E77|837E77|847F78|837E77,7F7A73|D7D1C5|6C6761|F1EADF|F2EBE0|8E8981|C8C2B8|F4EDE2|E1DBD0|7D7972,757069|E2DBCF|BBB5AA|605B56|67625C|615C57|67635D|67635D|5F5B55|BEB8AF", "まもなく帰還します")))
                            {
                                Set_ChangeStatus("正在重新收取远征队伍", Color.White, Color.Orange);
                                TempBack();
                                throw new Exceptions.ReactivateThreadException();
                            }
                            Overtime(ref j, 5000);
                        }
                    }
                    catch (Exceptions.TimeOutException)
                    {
                        Overtime(ref i, 30000);
                        continue;
                    }
                    break;
                }
                else
                {
                    Click(DataClickPonds.远征海域List[seasNumber - 1]);
                    Overtime(ref i, 60000, 1000);
                }
            }

            // 检测舰队选择
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.远征舰队选择))
                {
                    if (Judge(DataJudgePonds.远征舰队选择队伍List[team - 2])) { break; }
                    else
                    {
                        Click(DataClickPonds.远征舰队选择List[team - 2]);
                        i = 0;
                    }
                }
                else if (Judge(DataJudgePonds.远征决定List[0]) || Judge(DataJudgePonds.远征决定List[1]))
                { goto 选择远征海域; }
                Overtime(ref i, 30000);
            }

            // 检测舰队状态
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.远征开始灰))
                {
                    // 海域选择错误
                    throw new Exceptions.DataErrorException("海域选择错误！");
                }
                else if (Judge(DataJudgePonds.远征开始List[0]) || Judge(DataJudgePonds.远征开始List[1]))
                {
                    // 检测派遣状态
                    bool SupplyReadiness(bool[] GroupMember)
                    {
                        using (var bmp = new Bitmap(this.GetGameBitmap))
                        {
                            if (!Judge(DataJudgePonds.远征舰队选择, bmp))
                                throw new Exceptions.TimeOutException("判定异常错误！！");
                            for (int j = 0; j < GroupMember.Length; j++)
                            {
                                if (GroupMember[j])
                                {
                                    if (Judge(DataJudgePonds.舰队未补油1List[j], bmp) || Judge(DataJudgePonds.舰队未补油2List[j], bmp) ||
                                        Judge(DataJudgePonds.舰队未补弹1List[j], bmp) || Judge(DataJudgePonds.舰队未补弹2List[j], bmp))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        return true;
                    }
                    if (!SupplyReadiness(new bool[] { true, true, true, true, true, true }))
                    {
                        Set_AddMessage($"检测到第 {team.ToString()} 舰队中有未补给舰娘。");
                        throw new Exceptions.TimeOutException("舰娘未补给！");
                    }


                    // 急停
                    if ((team == 2 && !TimerCanRun(2)) ||
                                (team == 3 && !TimerCanRun(3)) ||
                                (team == 4 && !TimerCanRun(4)))
                    {
                        TempBack();
                        throw new Exceptions.ReactivateThreadException();
                    }

                    var xy = Click(DataClickPonds.远征开始);
                    _frequency[team - 2]++;// 派遣数加1
                    Set_AddMessage($"已派遣第 {team} 舰队 ({xy.X},{xy.Y})  共计 {GetFrequency(team)} 回。");

                    Delay(3000);
                    break;
                }
                Overtime(ref i, 30000);
            }

            检测舰队远征时间:
            List<Color> GetExpeditionIDColorMessage()
            {
                var a = 0;
                while (true)
                {
                    if (Judge(DataJudgePonds.远征决定List[0]) || Judge(DataJudgePonds.远征决定List[1]) ||
                        Judge(DataJudgePonds.远征中止与归还List[0]) || Judge(DataJudgePonds.远征中止与归还List[1]))
                    {
                        break;
                    }
                    Overtime(ref a, 5000);
                }

                Delay(100);
                using (Bitmap bmg = new Bitmap(this.GetGameBitmap))
                {
                    var idTemp = new List<Color>();
                    for (int j = 0; j < 4; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                idTemp.Add(bmg.GetPixel((15 * k) + 584 + k, 111 + l));//738
                            }
                        }
                    }
                    return idTemp;
                }
            }
            bool IsExpeditionIDEqual(List<Color> id1, List<Color> id2)
            {
                if (id1.Count == 0 || id2.Count == 0) return false;
                for (int a = 0; a < id1.Count; a++)
                {
                    if (id1[a] != id2[a])
                    {
                        return false;
                    }
                }
                return true;// 远征项目相同
            }
            var id = GetExpeditionIDColorMessage();
            if (IsExpeditionIDEqual(_tempExpeditionID, id))
            {
                // 和上一远征项目相同，重新选择
                if (detectedIterations > 6)
                {
                    throw new Exceptions.TimeOutException();
                }
                detectedIterations++;
                goto Start;
            }
            else
            {
                _tempExpeditionID = id;
            }
            i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.远征中止与归还List[0]) || Judge(DataJudgePonds.远征中止与归还List[1]))
                {
                    //已在远征或成功派遣，开始读取时间
                    return;
                }
                Overtime(ref i, 30000);
            }
        }
        /// <summary>
        /// 检测舰队远征时间
        /// </summary>
        /// <param name="team"></param>
        /// <param name="ExpeditionNumber"></param>
        private void Process3Sub2(int team, int ExpeditionNumber)
        {
            // 检测远征中止与归还
            int i = 0;
            while (true)
            {
                if (Judge(DataJudgePonds.远征中止与归还List[0]) || Judge(DataJudgePonds.远征中止与归还List[1]))
                {
                    if (Judge(DataJudgePonds.远征即将归还))
                    {
                        var timer = Function.FunctionThread.GetThread<ExpeditionTimer>("远征计时器_" + team.ToString());
                        timer.SetTime(0, 0, 10);
                        timer.IsWorking = true;
                        return;
                    }
                    break;
                }
                Overtime(ref i, 30000);
            }


            i = 0;
            数字识别:
            Color[] GetColorMessage(Bitmap btm)// 获取数字颜色信息(旧版移植)
            {
                Color[] GetC = new Color[24];
                var _i = 0;
                for (int y = 1; y < 12; y++)
                {
                    for (int x = 1; x < 8; x++)
                    {
                        GetC[_i++] = btm.GetPixel(x, y);
                        x++;
                    }
                    y++;
                }
                btm.Dispose(); btm = null;
                return GetC;
            }
            int NumberOCR(Color[] PColor, int lattice)// 进行数字识别(旧版移植)
            {
                string A_1 = "ece3d7|ece3d7|ece3d7|ece3d7|ece3d7|ece3d7|ede4d8|ede4d8|ece3d7|ece3d7|eee5d9|eee5d9|ece3d7|ebe2d6|f0e7db|efe6da|ece3d7|e8dfd3|f0e7db|f1e8dc|ece3d7|ebe2d6|efe6da|e7ded2";
                string A0 = "f1eadb|bfb9ad|afa9a0|bdb7ac|5b5751|f2ebdc|f1eadd|4d4944|4d4944|f3ecdd|f2ebde|4d4944|4d4944|f4edde|f2ebde|4d4944|67625c|f0e9dc|f0e9dc|6b6760|f1eadd|6b6660|a59f96|f2ebde";
                string A1 = ""; string A2 = ""; string A3 = ""; string A4 = ""; string A5 = ""; string A6 = ""; string A7 = ""; string A8 = ""; string A9 = "";

                string B_1 = "ebe2d6|efe6da|eee5d9|ede4d8|ece3d7|efe6da|eee5d9|ede4d8|ede4d8|efe6da|efe6da|eee5d9|efe6da|f0e7db|efe6da|eee5d9|ede4d8|f0e7db|eae1d5|ede4d8|ebe2d6|ede4d8|eee5d9|efe6da";
                string B0 = "f4ede2|c1bbb1|b0aba2|bcb6ad|5c5752|f4ede2|f3ece1|4d4944|4d4944|f4ede2|f4ede2|4d4944|4d4944|f4ede2|f5eee3|4d4944|67635c|f4ede0|f7f0e3|6c6761|f2ebde|6b6660|a7a198|f2ebde";
                string B1 = "f4ede2|89847d|87827b|f3ece1|f4ede2|c2bcb3|8a857e|f3ece1|f3ece1|c6c0b7|8a857e|f4ede2|f3ece1|c5bfb6|8a847d|f5eee3|f3ecdf|c1bbb0|86817a|f3ecdf|f2ebde|807b74|56524d|eae4d7";
                string B2 = "bfb9b0|7d7972|4d4944|f3ece1|f4ede2|f4ede2|79746d|d2ccc2|f3ece1|f4ede2|6c6761|f4ede2|f3ece1|f4ede2|b0aaa2|f5eee3|f3ecdf|98938a|f7f0e3|f3ecdf|77726b|56524d|524e48|77726b";
                string B3 = "e8e2d7|a29c94|4d4944|f3ece1|f4ede2|f4ede2|9e9890|f3ece1|f3ece1|f2ebe0|aaa49c|f4ede2|f3ece1|f4ede2|99948c|736e68|f3ecdf|f4ede0|f7f0e3|615d57|7f7a73|f2ebde|b7b1a7|f2ebde";
                string B4 = "f4ede2|f4ede2|4d4944|f3ece1|f4ede2|a19b93|4d4944|f3ece1|f3ece1|f4ede2|4d4944|f4ede2|bfb9af|c5bfb6|4d4944|b9b3ab|f3ecdf|f4ede0|4d4944|f3ecdf|f2ebde|f2ebde|4d4944|f2ebde";
                string B5 = "f4ede2|6e6a64|4d4944|f3ece1|f4ede2|f4ede2|f3ece1|f3ece1|d1cbc1|4d4944|847f78|f4ede2|f3ece1|f4ede2|757069|716c66|f3ecdf|f4ede0|f7f0e3|615d57|57534e|f0e9dc|d7d0c5|f2ebde";
                string B6 = ""; string B7 = ""; string B8 = ""; string B9 = "";

                string C_1 = "eee5d9|ebe2d6|ede4d8|ece3d7|eee5d9|ece3d7|ede4d8|ece3d7|ede4d8|eee5d9|eee5d9|ece3d7|ede4d8|efe6da|eee5d9|ece3d7|efe6da|efe6da|ebe2d6|ede4d8|efe6da|efe6da|ece3d7|eee5d9";
                string C0 = "c7c1b7|efe8dd|807b74|ebe4d8|4d4944|f3ece1|ded7cd|706c65|4d4944|f4ede2|f4ede2|4d4944|4d4944|f5eee3|f5eee3|504c47|4d4944|f5eee1|e0dacd|9d978e|e4ddd1|9b958d|7d7871|f2ebde";
                string C1 = "f3ece1|65615b|beb8af|f3ecdf|f3ece1|8a857e|c1bbb2|f3ecdf|f4ede2|8f8a82|c3bcb3|f4ede0|f5eee3|8e8982|c1bbb2|f4ede0|f5eee1|8a847d|bdb7ad|f1eadd|f2ebde|5c5752|7d7871|f2ebde";
                string C2 = "99948c|706c65|4d4944|f3ecdf|f3ece1|f3ece1|4d4944|f3ecdf|f4ede2|f4ede2|4d4944|f4ede0|f5eee3|f5eee3|dcd6cc|f4ede0|f5eee1|c2bcb1|f5eee1|cdc7bc|58534e|56514c|514d48|a7a298";
                string C3 = "bfb9b0|969189|4d4944|f3ecdf|f3ece1|f3ece1|66625c|f3ecdf|f4ede2|ded7cd|bfb9b0|f4ede0|f5eee3|f5eee3|6b6761|a49e95|f5eee1|f5eee1|d5cfc3|969088|a6a198|f4ede0|88837b|f2ebde";
                string C4 = "f3ece1|f3ece1|4d4944|f3ecdf|f3ece1|99948c|4d4944|f3ecdf|cdc7bd|f4ede2|4d4944|f4ede0|c1bbb2|c6c0b6|4d4944|b7b2a7|f5eee1|f5eee1|4d4944|f1eadd|f5eee1|f4ede0|4d4944|f2ebde";
                string C5 = "f3ece1|4d4944|4d4944|f3ecdf|f0e9de|f3ece1|f3ece1|f3ecdf|9b958e|4d4944|9f9991|f4ede0|f5eee3|f5eee3|504c47|a39d94|f5eee1|f5eee1|efe8dc|948f86|56524c|f4ede0|b6b0a6|f2ebde";
                string C6 = ""; string C7 = ""; string C8 = ""; string C9 = "";

                string D_1 = "e8dfd3|eee5d9|ece3d7|ede4d8|ebe2d6|ede4d8|ede4d8|efe6da|ede4d8|ece3d7|eee5d9|f0e7db|ede4d8|ede4d8|ede4d8|eee5d9|ebe2d6|f0e7db|eee5d9|efe8db|ede4d8|f2e9dd|eee5d9|efe8db";
                string D0 = "c6c0b5|eee7da|7f7a73|ede6da|4d4944|f2ebde|ddd6ca|706c65|4d4944|f3ecdf|f2ebde|4d4944|4d4944|f3ecdf|f3ecdf|504c47|4d4944|f6efe2|ded8cc|9d988f|e3dcd0|9c968e|7c7870|f2ebde";
                string D1 = "f2ebde|65615a|bdb7ac|f5eee1|f3ecdf|8a857d|c1bab0|f3ecdf|f3ecdf|8f8981|c1bbb0|f2ebde|f4ede0|8d8880|c0baaf|f4ede0|f4ede0|8a857d|bcb6ac|f2ebde|f1eadd|5c5752|7c7870|f2ebde";
                string D2 = "99938a|706b65|4d4944|f5eee1|f3ecdf|f2ebde|4d4944|f3ecdf|f3ecdf|f3ecdf|4d4944|f2ebde|f4ede0|f3ecdf|dbd4c8|f4ede0|f4ede0|c3bdb2|f3ecdf|cec8bc|58534e|56524c|514d48|a7a298";
                string D3 = "bfb9ae|969188|4d4944|f5eee1|f3ecdf|f2ebde|66625c|f3ecdf|f3ecdf|ddd6cb|beb8ad|f2ebde|f4ede0|f3ecdf|6b6660|a49e95|f4ede0|f6efe2|d3cdc1|969188|a6a097|f6efe2|87827a|f2ebde";
                string D4 = "f2ebde|f2ebde|4d4944|f5eee1|f3ecdf|99938a|4d4944|f3ecdf|ccc6bb|f3ecdf|4d4944|f2ebde|c1bbb0|c4beb4|4d4944|b7b2a7|f4ede0|f6efe2|4d4944|f2ebde|f4ede0|f6efe2|4d4944|f2ebde";
                string D5 = "f2ebde|4d4944|4d4944|f5eee1|f0e9dc|f2ebde|f2ebde|f3ecdf|9a958c|4d4944|9e988f|f2ebde|f4ede0|f3ecdf|504c47|a39d94|f4ede0|f6efe2|ede6da|958f87|56514c|f6efe2|b5afa5|f2ebde";
                string D6 = "f2ebde|bbb5aa|c7c1b6|f5eee1|dcd6ca|938e86|f2ebde|f3ecdf|4d4944|948f87|4d4944|c3bdb2|4d4944|f3ecdf|f1eadd|524e48|4d4944|f6efe2|f3ecdf|79746d|dbd4c8|8c877f|928d84|f2ebde";
                string D7 = "4d4944|4d4944|4d4944|75706a|f3ecdf|f2ebde|f2ebde|f3ecdf|f3ecdf|f3ecdf|b2aca3|f2ebde|f4ede0|f3ecdf|cfc9be|f4ede0|f4ede0|cec8bc|f3ecdf|f2ebde|f4ede0|b3ada3|f3ecdf|f2ebde";
                string D8 = "cbc5b9|f2ebde|bbb5ab|f5eee1|64605a|f2ebde|f2ebde|ccc5ba|e7e0d4|4d4944|aba69c|f2ebde|bdb7ad|f3ecdf|4d4944|b2aca2|4d4944|f6efe2|f3ecdf|8c877f|c6c0b5|aea89e|a19c93|f2ebde";
                string D9 = "9d988f|f2ebde|868179|e5ded2|4d4944|f2ebde|e8e2d5|68645e|4d4944|f3ecdf|eae4d7|57534e|dfd9cd|858078|aaa49b|a39e95|f4ede0|f6efe2|4d4944|f2ebde|f4ede0|635f58|f3ecdf|f2ebde";

                string E_1 = "efe6da|ece3d7|ebe2d6|ece3d7|ede4d8|eee5d9|efe6da|f0e7db|ece3d7|eae1d5|ede4d8|ece3d7|eee5d9|ede4d8|eee5d9|eee5d9|ebe4d7|e9e2d5|ece5d8|ebe4d7|ece5d8|e9e2d5|ece5d8|eae3d6";
                string E0 = "f1eadf|918c85|e0d9cf|8d8881|8e8981|dbd5cb|f2ebe0|4d4944|59554f|f1eadf|f3ece1|4d4944|635f59|e7e0d5|f3ece1|4d4944|9a958d|beb8af|f3ece1|4d4944|f4ede2|605c56|bcb6ad|f4ede2";
                string E1 = "f1eadf|a9a39b|504c47|f3ece1|f1eadf|f4ede2|534f4a|f3ece1|f2ebe0|f3ece1|534f4a|f4ede2|f3ece1|f2ebe0|524e48|f4ede2|f4ede2|f4ede2|4d4944|f4ede2|f4ede2|a7a199|56514c|d0c9c0";
                string E2 = "e4ddd2|89847d|4d4944|f3ece1|f1eadf|f4ede2|afa9a1|9c968e|f2ebe0|f3ece1|a29d94|e2dbd1|f3ece1|f2ebe0|847f78|f4ede2|f4ede2|969089|f3ece1|f4ede2|9d9890|57534d|524e49|56514c";
                string E3 = "f1eadf|959088|4d4944|ded8cd|f1eadf|f4ede2|d3cdc3|c4beb4|f2ebe0|f3ece1|88837c|f4ede2|f3ece1|f2ebe0|bbb5ac|4d4944|f4ede2|f4ede2|f3ece1|4d4944|99948c|f4ede2|e4ddd2|d9d3c9";
                string E4 = "f1eadf|f5eee3|a29c94|ccc6bc|f1eadf|f4ede2|959088|ccc5bc|f2ebe0|c6c0b7|938d86|ccc5bc|9d9790|c3bdb4|8f8982|bbb5ac|f4ede2|f4ede2|8d8881|c4beb4|f4ede2|f4ede2|8a857e|c2bcb3";
                string E5 = "f1eadf|a29c94|4d4944|f1eadf|f1eadf|e8e2d7|f2ebe0|f3ece1|f2ebe0|4d4944|6a655f|f4ede2|f3ece1|f2ebe0|98928b|4d4944|f4ede2|f4ede2|f3ece1|4d4944|8a857e|dad4c9|f3ece1|e6e0d5";
                string E6 = ""; string E7 = ""; string E8 = ""; string E9 = "";

                string F_1 = "efe6da|ede4d8|ede4d8|f1e8dc|f0e7db|ede4d8|ede4d8|efe6da|efe6da|ede4d8|ede4d8|ece3d7|ece3d7|ebe2d6|ede4d8|ece3d7|efe6da|eae1d5|eae1d5|ece3d7|ede4d8|ebe2d6|ece3d7|ebe2d6";
                string F0 = "f2ebe0|918c84|e2dbd1|8f8982|8f8982|dbd5cb|f4ede2|4d4944|59554f|f2ebe0|f3ece1|4d4944|635f59|e8e2d7|f3ece1|4d4944|9a958d|beb8af|f4ede2|4d4944|f4ede2|605c56|bdb7ae|f4ede2";
                string F1 = "f2ebe0|89847d|87827b|f6efe4|f3ece1|c2bcb3|8a857e|f6efe4|f4ede2|c6c0b7|8a857e|f5eee3|f4ede2|c5bfb6|89847d|f4ede2|f4ede2|c1bbb1|858079|f4ede2|f4ede2|817c75|56524d|ece5db";
                string F2 = "bdb7ae|7d7972|4d4944|f6efe4|f3ece1|f4ede2|79746d|d4cec4|f4ede2|f4ede2|6c6761|f5eee3|f4ede2|f4ede2|afa9a1|f4ede2|f4ede2|98938b|f4ede2|f4ede2|78736c|56524d|524e48|78736c";
                string F3 = "e7e0d5|a29c94|4d4944|f6efe4|f3ece1|f4ede2|9e9991|f6efe4|f4ede2|f2ebe0|a9a49b|f5eee3|f4ede2|f4ede2|98938b|726e67|f4ede2|f4ede2|f4ede2|615d57|807b74|f4ede2|b7b1a8|f4ede2";
                string F4 = "f2ebe0|f4ede2|747069|f6efe4|f3ece1|cfc9bf|5f5a55|f6efe4|f4ede2|f4ede2|5b5752|f5eee3|8d8881|c5bfb6|58534e|bab4ab|f4ede2|f4ede2|56524d|f4ede2|f4ede2|f4ede2|534f4a|f4ede2";
                string F5 = "f2ebe0|6e6a64|4d4944|f6efe4|f3ece1|f4ede2|f4ede2|f6efe4|d2ccc2|4d4944|847f78|f5eee3|f4ede2|f4ede2|746f69|706c65|f4ede2|f4ede2|f4ede2|615d57|58534e|f2ebe0|d7d0c6|f4ede2";
                string F6 = "f2ebe0|e3dcd2|a39d95|f6efe4|f3ece1|65615b|f4ede2|f6efe4|7a756e|8a857e|5d5853|959088|4d4944|f4ede2|f3ece1|4d4944|54504b|f4ede2|f4ede2|4d4944|f4ede2|635e58|bbb5ac|f4ede2";
                string F7 = "6e6963|4d4944|4d4944|4d4944|f1eadf|f4ede2|f4ede2|c4beb5|f4ede2|f4ede2|e6dfd4|f5eee3|f4ede2|f4ede2|9d9790|f4ede2|f4ede2|f4ede2|f4ede2|f4ede2|f4ede2|7f7a73|f4ede2|f4ede2";
                string F8 = "f2ebe0|e0d9cf|ebe4d9|cbc4bb|9c968e|c9c3b9|f4ede2|96918a|f4ede2|4d4944|8b867e|f5eee3|e3dcd2|f0e9de|67635d|817c75|5b5751|f4ede2|f4ede2|57534d|e6dfd5|88837c|c0bab1|f4ede2";
                string F9 = "c9c3b9|f4ede2|b3ada4|b3aea5|4d4944|f4ede2|f4ede2|4d4944|4d4944|e4ddd3|f3ece1|4d4944|f4ede2|7e7972|ded7cd|726d67|f4ede2|f4ede2|514d48|f4ede2|f4ede2|807b74|f4ede2|f4ede2";

                try
                {
                    switch (lattice)
                    {
                        case 1:
                            if (NumberOCR2(PColor, A0)) return 0;
                            else if (NumberOCR2(PColor, A1)) return 1;
                            else if (NumberOCR2(PColor, A2)) return 2;
                            else if (NumberOCR2(PColor, A3)) return 3;
                            else if (NumberOCR2(PColor, A4)) return 4;
                            else if (NumberOCR2(PColor, A5)) return 5;
                            else if (NumberOCR2(PColor, A6)) return 6;
                            else if (NumberOCR2(PColor, A7)) return 7;
                            else if (NumberOCR2(PColor, A8)) return 8;
                            else if (NumberOCR2(PColor, A9)) return 9;
                            else if (NumberOCR2(PColor, A_1)) return 0;
                            else return -1;
                        case 2:
                            if (NumberOCR2(PColor, B0)) return 0;
                            else if (NumberOCR2(PColor, B1)) return 1;
                            else if (NumberOCR2(PColor, B2)) return 2;
                            else if (NumberOCR2(PColor, B3)) return 3;
                            else if (NumberOCR2(PColor, B4)) return 4;
                            else if (NumberOCR2(PColor, B5)) return 5;
                            else if (NumberOCR2(PColor, B6)) return 6;
                            else if (NumberOCR2(PColor, B7)) return 7;
                            else if (NumberOCR2(PColor, B8)) return 8;
                            else if (NumberOCR2(PColor, B9)) return 9;
                            else if (NumberOCR2(PColor, B_1)) return 0;
                            else return -1;
                        case 3:
                            if (NumberOCR2(PColor, C0)) return 0;
                            else if (NumberOCR2(PColor, C1)) return 1;
                            else if (NumberOCR2(PColor, C2)) return 2;
                            else if (NumberOCR2(PColor, C3)) return 3;
                            else if (NumberOCR2(PColor, C4)) return 4;
                            else if (NumberOCR2(PColor, C5)) return 5;
                            else if (NumberOCR2(PColor, C6)) return 6;
                            else if (NumberOCR2(PColor, C7)) return 7;
                            else if (NumberOCR2(PColor, C8)) return 8;
                            else if (NumberOCR2(PColor, C9)) return 9;
                            else if (NumberOCR2(PColor, C_1)) return 0;
                            else return -1;
                        case 4:
                            if (NumberOCR2(PColor, D0)) return 0;
                            else if (NumberOCR2(PColor, D1)) return 1;
                            else if (NumberOCR2(PColor, D2)) return 2;
                            else if (NumberOCR2(PColor, D3)) return 3;
                            else if (NumberOCR2(PColor, D4)) return 4;
                            else if (NumberOCR2(PColor, D5)) return 5;
                            else if (NumberOCR2(PColor, D6)) return 6;
                            else if (NumberOCR2(PColor, D7)) return 7;
                            else if (NumberOCR2(PColor, D8)) return 8;
                            else if (NumberOCR2(PColor, D9)) return 9;
                            else if (NumberOCR2(PColor, D_1)) return 0;
                            else return -1;
                        case 5:
                            if (NumberOCR2(PColor, E0)) return 0;
                            else if (NumberOCR2(PColor, E1)) return 1;
                            else if (NumberOCR2(PColor, E2)) return 2;
                            else if (NumberOCR2(PColor, E3)) return 3;
                            else if (NumberOCR2(PColor, E4)) return 4;
                            else if (NumberOCR2(PColor, E5)) return 5;
                            else if (NumberOCR2(PColor, E6)) return 6;
                            else if (NumberOCR2(PColor, E7)) return 7;
                            else if (NumberOCR2(PColor, E8)) return 8;
                            else if (NumberOCR2(PColor, E9)) return 9;
                            else if (NumberOCR2(PColor, E_1)) return 0;
                            else return -1;
                        case 6:
                            if (NumberOCR2(PColor, F0)) return 0;
                            else if (NumberOCR2(PColor, F1)) return 1;
                            else if (NumberOCR2(PColor, F2)) return 2;
                            else if (NumberOCR2(PColor, F3)) return 3;
                            else if (NumberOCR2(PColor, F4)) return 4;
                            else if (NumberOCR2(PColor, F5)) return 5;
                            else if (NumberOCR2(PColor, F6)) return 6;
                            else if (NumberOCR2(PColor, F7)) return 7;
                            else if (NumberOCR2(PColor, F8)) return 8;
                            else if (NumberOCR2(PColor, F9)) return 9;
                            else if (NumberOCR2(PColor, F_1)) return 0;
                            else return -1;
                        default:
                            return -1;
                    }
                }
                catch (Exception)
                { return -1; }
            }
            bool NumberOCR2(Color[] PColor, string D)
            {
                if (D == "") return false;
                string[] DArray = D.Split('|');
                Color[] DColor = new Color[24];
                int q = 0;
                foreach (string _i in DArray)
                { DColor[q++] = ColorTranslator.FromHtml("#" + _i); }
                q = 0;
                foreach (Color _i in PColor)
                { if (!IsSimilarColor(_i, DColor[q++], 60)) return false; }
                return true;
            }
            using (Bitmap timeNowImage = FunctionBitmap.CutImage(this.GetGameBitmap, 722, 387, 63, 13, true))
            {
                List<Color[]> colorArr = new List<Color[]>();
                colorArr.Add(GetColorMessage(FunctionBitmap.CutImage(timeNowImage, 0, 0, 9, 13, false)));// 数字位置1取得颜色值
                colorArr.Add(GetColorMessage(FunctionBitmap.CutImage(timeNowImage, 9, 0, 9, 13, false)));
                colorArr.Add(GetColorMessage(FunctionBitmap.CutImage(timeNowImage, 23, 0, 9, 13, false)));
                colorArr.Add(GetColorMessage(FunctionBitmap.CutImage(timeNowImage, 32, 0, 9, 13, false)));
                colorArr.Add(GetColorMessage(FunctionBitmap.CutImage(timeNowImage, 45, 0, 9, 13, false)));
                colorArr.Add(GetColorMessage(FunctionBitmap.CutImage(timeNowImage, 54, 0, 9, 13, false)));

                List<string> strArr = new List<string>();
                for (int j = 0; j < colorArr.Count; j++)
                {
                    strArr.Add(NumberOCR(colorArr[j], j + 1).ToString());
                }
                for (int j = 0; j < colorArr.Count; j++)
                {
                    if (strArr[j] == "-1") { strArr[j] = "-"; }
                }
                foreach (var item in strArr)
                {
                    if (item == "-")
                    {
                        if (i > 5)//白板数字检测
                        {//未能识别出数字，将图片保存为图片
#warning 保存图片
                            string OutText = "(这个功能尚未实现。)";
                            Set_AddMessage("未能识别出剩余时间，已调用固定时间. . ." + Environment.NewLine + OutText); //$"已将未识别的时间图像保存到程序\"Repair\"文件夹下，请将其发给作者共同改进本软件{OutText}  作者QQ：490889409");
                            string Str;
                            switch (ExpeditionNumber)
                            {
                                case 1: Str = "0:15:00"; break;
                                case 2: Str = "0:30:00"; break;
                                case 3: Str = "0:20:00"; break;
                                case 4: Str = "0:50:00"; break;
                                case 5: Str = "1:30:00"; break;
                                case 6: Str = "0:40:00"; break;
                                case 7: Str = "1:00:00"; break;
                                case 8: Str = "3:00:00"; break;
                                case 9: Str = "4:00:00"; break;
                                case 10: Str = "1:30:00"; break;
                                case 11: Str = "5:00:00"; break;
                                case 12: Str = "8:00:00"; break;
                                case 13: Str = "4:00:00"; break;
                                case 14: Str = "6:00:00"; break;
                                case 15: Str = "12:00:00"; break;
                                case 16: Str = "15:00:00"; break;
                                case 17: Str = "0:45:00"; break;
                                case 18: Str = "5:00:00"; break;
                                case 19: Str = "6:00:00"; break;
                                case 20: Str = "2:00:00"; break;
                                case 21: Str = "2:20:00"; break;
                                case 22: Str = "3:00:00"; break;
                                case 23: Str = "4:00:00"; break;
                                case 24: Str = "8:20:00"; break;
                                case 25: Str = "40:00:00"; break;
                                case 26: Str = "80:00:00"; break;
                                case 27: Str = "20:00:00"; break;
                                case 28: Str = "25:00:00"; break;
                                case 29: Str = "24:00:00"; break;
                                case 30: Str = "48:00:00"; break;
                                case 31: Str = "2:00:00"; break;
                                case 32: Str = "24:00:00"; break;
                                case 33: Str = "0:15:00"; break;
                                case 34: Str = "0:30:00"; break;
                                case 35: Str = "7:00:00"; break;
                                case 36: Str = "9:00:00"; break;
                                case 37: Str = "2:45:00"; break;
                                case 38: Str = "2:55:00"; break;
                                case 39: Str = "30:00:00"; break;
                                case 40: Str = "6:50:00"; break;
                                default: throw new Exceptions.DataErrorException("远征海域数据错误！");
                            }
                            string[] PStr = Str.Split(':');
                            int _h = Convert.ToInt32(PStr[0]);
                            int _m = Convert.ToInt32(PStr[1]);
                            int _s = Convert.ToInt32(PStr[2]);
                            var _timer = Function.FunctionThread.GetThread<ExpeditionTimer>("远征计时器_" + team.ToString());
                            _timer.SetTime(_h, _m, _s);
                            _timer.IsWorking = true;
                            return;
                        }
                        Delay(200);
                        i++; goto 数字识别;
                    }
                }

                int h = Convert.ToInt32(strArr[0] + strArr[1]);
                int m = Convert.ToInt32(strArr[2] + strArr[3]);
                int s = Convert.ToInt32(strArr[4] + strArr[5]);
                //TimeSpan Remaining = Function.Functions.SecondToTime(((0 * 24 + h) * 60 + m) * 60 + s);
                var timer = Function.FunctionThread.GetThread<ExpeditionTimer>("远征计时器_" + team.ToString());
                if (timer != null)
                {
                    timer.SetTime(h, m, s + 10);
                    timer.IsWorking = true;
                }

            }

        }
        /// <summary>
        /// A1、A2、A3、B1的派遣
        /// </summary>
        /// <param name="seasNumber"></param>
        /// <param name="itemsNumber"></param>
        private void Process3SubForAB(int seasNumber, int itemsNumber)
        {
            Delay(1000);
            int i = 0;
            var A1 = new DataJudge("122,379 @ F4EDE2|F4ECE1|F3ECE0|F3ECE1|F3ECE0|F3ECE1|F2EBDF|F2EBDF|F1EADE|F1EADE|F0E9DC|EFE8DB|F1EADD|F0E9DC|F1EADD|EEE7DA|EFE8DB|EEE7DA|EDE6DA,F3ECE1|F4ECE1|F5EEE3|F2EBE0|BFB9B0|5B5752|DDD6CB|F0E9DD|F1EADE|EEE7DB|EFE8DB|F2EBDE|F2EBDE|D6CFC4|88837B|5D5953|EFE8DB|F1EADD|EFE8DC,F3ECE0|F3ECE0|F4EDE1|F2EBDF|847F77|969088|A29D94|F0E9DD|F2EBDF|EFE8DC|EFE8DB|F2EBDE|8D8880|67625C|ACA69D|5D5953|EFE8DB|F1EADD|F0E9DC,F2EBDF|F2EBDF|F2EBDF|E4DED2|5C5852|CDC7BD|67635D|F0E9DD|F2EBDF|F0E9DD|EFE8DB|F2EBDE|C6BFB4|F0E9DC|C6C0B5|5D5953|EFE8DB|F1EADD|F0E9DC,F2EBDF|F2EBDF|F1EADE|A9A39A|948F86|F0E9DD|68645D|C8C2B7|F2EBDF|F1EADE|EFE8DB|F1EADD|F1EADD|F0E9DC|C6C0B5|5D5953|EFE8DB|F1EADD|F0E9DC,F1EADE|F1EADE|EFE8DC|6E6A63|CCC5BB|EFE8DC|A09A92|8D8880|F0E9DD|F1EADE|EFE8DB|F1EADD|F1EADD|F0E9DC|C7C1B6|5D5953|EFE8DB|F2EBDE|F1EADD,F0E9DD|F0E9DD|CEC8BD|69645E|EFE8DC|EFE8DC|D7D0C5|524E49|EDE6DA|F0E9DD|EFE8DB|F1EADD|F1EADD|F0E9DC|C7C1B6|5D5953|EFE8DB|F2EBDE|F1EADD,F0E9DD|F0E9DD|948F87|A29D94|D0CABF|CFC8BE|D0CABF|726D67|B0ABA2|EFE8DC|EFE8DB|F1EADD|F2EBDE|F0E9DC|C7C1B6|5D5953|EFE8DB|F2EBDE|F1EADD,F1EADE|F1EADE|5A5650|7E7A72|7E7972|7D7871|7D7872|7D7871|76726B|EEE7DB|EFE8DB|F0E9DD|F2EBDE|F0E9DC|C7C1B6|5D5953|EFE8DB|F2EBDE|F1EADD,F2EBDF|BBB5AC|706B65|EFE8DC|EFE8DC|EEE7DB|EDE6DB|D7D1C6|4D4944|D7D0C5|EFE8DB|EEE7DB|F0E9DC|EFE8DB|C6C0B5|5D5953|F0E9DC|F0E9DC|F2EBDE,F2EBDF|807B74|A8A299|EFE8DC|EFE8DC|EFE8DC|EFE8DC|EFE8DC|77726B|9E9890|EFE8DB|EFE8DB|EFE8DB|EFE8DB|C5BFB4|5D5953|EFE8DB|EFE8DB|F0E9DC", "A1");
            var A2 = new DataJudge("122,379 @ F4EDE2|F4ECE1|F3ECE0|F3ECE1|F3ECE0|F3ECE1|F2EBDF|F2EBDF|F1EADE|F1EADE|F0E9DC|EFE8DB|F1EADD|F0E9DC|E8E1D5|E5DED1|EFE8DB|EEE7DA|EDE6DA,F3ECE1|F4ECE1|F5EEE3|F2EBE0|BFB9B0|5B5752|DDD6CB|F0E9DD|F1EADE|EEE7DB|EFE8DB|F2EBDE|979289|5B5751|514D48|4D4944|6F6A63|CCC6BA|EFE8DC,F3ECE0|F3ECE0|F4EDE1|F2EBDF|847F77|969088|A29D94|F0E9DD|F2EBDF|EFE8DC|EFE8DB|F2EBDE|969189|D6D0C4|F0E9DC|EFE8DB|9A958C|68645D|F0E9DC,F2EBDF|F2EBDF|F2EBDF|E4DED2|5C5852|CDC7BD|67635D|F0E9DD|F2EBDF|F0E9DD|EFE8DB|F2EBDE|F1EADD|F0E9DC|F0E9DC|EFE8DB|DED7CB|4D4944|EAE3D6,F2EBDF|F2EBDF|F1EADE|A9A39A|948F86|F0E9DD|68645D|C8C2B7|F2EBDF|F1EADE|EFE8DB|F1EADD|F1EADD|F0E9DC|F0E9DC|EFE8DB|D4CDC2|504C47|F0E9DC,F1EADE|F1EADE|EFE8DC|6E6A63|CCC5BB|EFE8DC|A09A92|8D8880|F0E9DD|F1EADE|EFE8DB|F1EADD|F1EADD|F0E9DC|F1EADD|EFE8DB|918C84|8A857D|F1EADD,F0E9DD|F0E9DD|CEC8BD|69645E|EFE8DC|EFE8DC|D7D0C5|524E49|EDE6DA|F0E9DD|EFE8DB|F1EADD|F1EADD|F0E9DC|F1EADD|BDB7AC|504C46|DDD6CA|F1EADD,F0E9DD|F0E9DD|948F87|A29D94|D0CABF|CFC8BE|D0CABF|726D67|B0ABA2|EFE8DC|EFE8DB|F1EADD|F2EBDE|F0E9DC|DBD5C9|5A5650|B5AFA5|F2EBDE|F1EADD,F1EADE|F1EADE|5A5650|7E7A72|7E7972|7D7871|7D7872|7D7871|76726B|EEE7DB|EFE8DB|F0E9DD|F2EBDE|E8E1D4|6F6A64|A39E94|EFE8DB|F2EBDE|F1EADD,F2EBDF|BBB5AC|706B65|EFE8DC|EFE8DC|EEE7DB|EDE6DB|D7D1C6|4D4944|D7D0C5|EFE8DB|EEE7DB|EEE7DA|76716A|A29C93|F0E9DC|F0E9DC|F0E9DC|F2EBDE,F2EBDF|807B74|A8A299|EFE8DC|EFE8DC|EFE8DC|EFE8DC|EFE8DC|77726B|9E9890|EFE8DB|E7E0D3|76716A|A39D94|E2DBCF|E2DBCF|E2DBCF|E2DBCF|E5DFD2", "A2");
            var A3 = new DataJudge("154,383 @ EFE8DB|EFE8DB|F0E9DC|F0E9DC|F0E9DC|EFE8DB|EEE7DA|EDE6DA|EFE8DB|F0E9DC|4C4843|F0E9DC|C8C2B7|757069|EEE7DA|EEE7DA|EFE8DB|C8C2B7|4C4843|F0E9DC|F0E9DC|EFE8DB|EFE8DB|EFE8DB|757069|A8A398|C7C0B5|C7C0B5|6C6760|C7C1B6|9E9990|6C6760|C7C2B7|C7C2B7|8A847C|757069|EFE8DB|EFE8DB|EFE8DB|EFE8DB|F0E9DC|F0E9DC|615D56|D0C9BE|757069|4C4843|4C4843|4C4843|4C4843|4C4843|9E988F|E6DFD3|B2ABA2|ECE5D9|ECE5D8|B1ABA1|7F7A73|56524D|9D978E|B0AAA1|9D988F|56524D|6C6760|9E988F|C7C0B5|D9D2C7|938D86|615D56|9E988F|615D56|EFE8DB|615C56|EFE7DB|DBD3C8|807A73|4C4843|928D84|E4DCD0|EEE7DA|EFE7DB|F0E8DB|C7C0B5|B2ABA2|EFE8DB|C8C1B6|B2ABA2|EFE8DB|9E9990|9E9990|F0E9DC", "A3");
            var B1 = new DataJudge("122,379 @ F4EDE2|F4ECE1|F3ECE0|F3ECE1|F3ECE0|F3ECE1|F2EBDF|F2EBDF|F1EADE|F1EADE|F0E9DC|EFE8DB|F1EADD|F0E9DC|F1EADD|EEE7DA|EFE8DB|EEE7DA|EDE6DA,F3ECE1|F4ECE1|918C85|5B5752|5B5752|605C56|746F69|AFA9A0|F1EADE|EEE7DB|EFE8DB|F2EBDE|F2EBDE|D6CFC4|88837B|5D5953|EFE8DB|F1EADD|EFE8DC,F3ECE0|F3ECE0|918C84|A09A92|EEE7DB|E6DFD3|B9B3AA|4D4944|C3BDB3|EFE8DC|EFE8DB|F2EBDE|8D8880|67625C|ACA69D|5D5953|EFE8DB|F1EADD|F0E9DC,F2EBDF|F2EBDF|908B83|9F9A91|F2EBDF|F2EBDF|F1EADE|8D8880|A09B92|F0E9DD|EFE8DB|F2EBDE|C6BFB4|F0E9DC|C6C0B5|5D5953|EFE8DB|F1EADD|F0E9DC,F2EBDF|F2EBDF|908B83|9F9991|F0E9DD|F0E9DD|F1EADE|88837C|ADA79E|F1EADE|EFE8DB|F1EADD|F1EADD|F0E9DC|C6C0B5|5D5953|EFE8DB|F1EADD|F0E9DC,F1EADE|F1EADE|8F8A82|9F9991|D6CFC4|D6CFC4|ADA79E|7A766F|F0E9DD|F1EADE|EFE8DB|F1EADD|F1EADD|F0E9DC|C7C1B6|5D5953|EFE8DB|F2EBDE|F1EADD,F0E9DD|F0E9DD|8F8A82|7D7871|7E7972|7B766F|65605A|948F86|EFE8DC|F0E9DD|EFE8DB|F1EADD|F1EADD|F0E9DC|C7C1B6|5D5953|EFE8DB|F2EBDE|F1EADD,F0E9DD|F0E9DD|8F8A82|A09A92|F0E9DD|EEE7DB|F0E9DD|89847D|8E8982|EFE8DC|EFE8DB|F1EADD|F2EBDE|F0E9DC|C7C1B6|5D5953|EFE8DB|F2EBDE|F1EADD,F1EADE|F1EADE|908B83|A09B92|F0E9DD|EEE7DB|EFE8DD|C3BDB2|635E58|EEE7DB|EFE8DB|F0E9DD|F2EBDE|F0E9DC|C7C1B6|5D5953|EFE8DB|F2EBDE|F1EADD,F2EBDF|F2EBDF|8F8A82|9E9990|EFE8DC|EEE7DB|EDE6DB|B9B3A9|67625C|EEE7DB|EFE8DB|EEE7DB|F0E9DC|EFE8DB|C6C0B5|5D5953|F0E9DC|F0E9DC|F2EBDE,F2EBDF|F1EADE|8F8A82|9E9990|E6DFD4|E4DDD1|C8C2B7|6C6761|98938B|EFE8DC|EFE8DB|EFE8DB|EFE8DB|EFE8DB|C5BFB4|5D5953|EFE8DB|EFE8DB|F0E9DC", "B1");
            while (true)
            {
                if (Judge(DataJudgePonds.远征图List[seasNumber - 1]))
                {
                    var judgeA1 = seasNumber == 1 ? Judge(A1) : false;
                    var judgeA2 = seasNumber == 1 ? Judge(A2) : false; Delay(100);
                    var judgeA3 = seasNumber == 1 ? Judge(A3) : false;
                    var judgeB1 = seasNumber == 2 ? Judge(B1) : false;
                    if (judgeA1 || judgeA2 || judgeA3 || judgeB1)
                    {
                        if (itemsNumber == 41 && judgeA1 ||
                       itemsNumber == 42 && judgeA2 ||
                       itemsNumber == 43 && judgeA3 ||
                       itemsNumber == 44 && judgeB1)
                        {
                            Click(DataClickPonds.远征项List[7]);
                            break;
                        }
                    }

                    // A2例外 //B1例外
                    if (itemsNumber == 42 && Judge(new DataJudge("185,353 @ D8D1C5|7F7972|4C4843|918C83|E1DACD|EBE4D7|ECE4D8|C5BEB3")) ||
                       itemsNumber == 44 && Judge(new DataJudge("200,353 @ 747069|747069|757069|B2ABA2|EEE7DA|EEE6DA|9D988F|938D85")))
                    {
                        Click(DataClickPonds.远征项List[6]); break;
                    }

                    Delay(100);
                    Click(new DataClick("162,412|170,420"));
                    Delay(100);
                    Overtime(ref i, 60000, 1000);
                }
                else
                {
                    Click(DataClickPonds.远征海域List[seasNumber - 1]);
                    Overtime(ref i, 60000, 1000);
                }
            }
            return;
        }

        /// <summary>
        /// 判断是否达到远征结束设定
        /// </summary>
        private void JudgeIsOver()
        {
            for (int i = 2; i < 5; i++)
            {
                if (!TimerCanRun(i)) continue;
                if (GetMain_Form.ExpeditionOtherSetting.IsFrequency)
                {
                    if (GetFrequency(i) >= GetMain_Form.ExpeditionOtherSetting.GetFrequency(i))
                    {
                        Set_AddMessage($"队伍{i}已达到设定远征次数。");
                        FunctionThread.CloseThread("远征计时器_" + i.ToString());
                    }
                }
                if (GetMain_Form.ExpeditionOtherSetting.IsOverTime)
                {
                    if (GetMain_Form.ExpeditionOtherSetting.GetOverTime(i) < DateTime.Now)
                    {
                        Set_AddMessage($"队伍{i}已达到设定结束时间。");
                        FunctionThread.CloseThread("远征计时器_" + i.ToString());
                    }
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
        /// 超时系统
        /// </summary>
        private void OvertimeProcess()
        {
            Delay(200);
            if (Judge(DataJudgePonds.母港1)) return;
            DateTime Pt = DateTime.Now;//超时时刻
            TimeSpan Ptm = TimeSpan.Zero;
            Set_AddMessage("界面判定已超时。");
            Set_ChangeStatus($"已超时： {((int)Ptm.TotalSeconds).ToString()}秒。", Color.White, Color.Red);
            while (true)
            {
                if (IsDisposed) this.Thread.Abort();// 检测线程是否被关闭
                Delay(1000);
                if (IsDisposed) this.Thread.Abort();
                Ptm = DateTime.Now - Pt;
                Set_ChangeStatus($"已超时： {Ptm.Hours}时{Ptm.Minutes}分{Ptm.Seconds}秒。", Color.White, Color.Red);

                if (Judge(DataJudgePonds.母港1)) return;
                else if (Judge(DataJudgePonds.远征回港) ||
                    Judge(DataJudgePonds.远征归来1) ||
                    Judge(DataJudgePonds.远征归来2))
                { Click(DataClickPonds.舰娘立绘); continue; }
                Delay(200);

                if (Judge(DataJudgePonds.左侧返回母港1) || Judge(DataJudgePonds.左侧返回母港2))
                { Click(DataClickPonds.左侧返回母港); continue; }
            }
        }

        /// <summary>
        /// 开启远征线程
        /// </summary>
        public override void StartThread()
        {
            try
            { base.StartThread(); }
            catch (Exceptions.NoPermitException)
            {
                Set_ChangeStatus("软件未激活！", Color.Red, Color.White);
                this.StopThread();
            }
            catch (Exceptions.NoGameHandleException)
            {
                Set_ChangeStatus("未检测到游戏窗口！", Color.Red, Color.White);
                this.StopThread();
            }
        }
        /// <summary>
        /// 关闭远征线程
        /// </summary>
        public override void StopThread()
        {
            // 关闭所有远征计时器线程
            Function.FunctionThread.CloseThread("远征计时器_2");
            Function.FunctionThread.CloseThread("远征计时器_3");
            Function.FunctionThread.CloseThread("远征计时器_4");
            if (IsDisposed) return;
            Set_AddMessage("自动远征已关闭。");
            if (GetMain_Form.GameExpedition_Status_textBox.BackColor != Color.White)
            {
                Set_ChangeStatus("远征已停止", Color.Black, Color.White, false);
            }
            base.StopThread();
        }
        /// <summary>
        /// 远征构造函数
        /// </summary>
        public Expedition() : base("自动远征") { this.StartThread(); }
    }

}