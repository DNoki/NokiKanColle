using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Drawing;

using KopiLua;
using NLua;
using NokiKanColle.Data;
using NokiKanColle.Function;
using static NokiKanColle.Data.DataPond;

namespace NokiKanColle.Utility.Process
{
    public class LuaScript : GameProcessWrapper, IDisposable
    {
        /// <summary>
        /// 脚本名
        /// </summary>
        private string _scriptName = "";
        /// <summary>
        /// 将 Lua 脚本读取到流
        /// </summary>
        private StreamReader _luaScriptText;
        /// <summary>
        /// 是否已释放资源的标志
        /// </summary>
        private bool _isDisposed = false;
        /// <summary>
        /// 循环等待时间（单位：毫秒）
        /// </summary>
        public int WaitTime = 3000;
        /// <summary>
        /// 循环次数（0 为始终循环）
        /// </summary>
        public int Cycles = 1;
        /// <summary>
        /// 已执行的次数
        /// </summary>
        public int ExecutedNumber = 0;

        /// <summary>
        /// 添加一条信息到信息栏
        /// </summary>
        /// <param name="text">要添加的文字</param>
        public new void Set_AddMessage(string text)
        {
            base.Set_AddMessage("Lua:" + text);
            /*
            if (GetMain_Form.InvokeRequired)
            {
                GetMain_Form.BeginInvoke(new Action<string>(Set_AddMessage), text);
            }
            else
            {
                if (GetMain_Form.GameLuaScript_Message_textBox.Lines.Length > 120)
                {
                    var temp = GetMain_Form.GameLuaScript_Message_textBox.Lines;
                    var newLines = new string[temp.Length - 10];
                    Array.Copy(temp, 0, newLines, 0, newLines.Length);
                    GetMain_Form.GameLuaScript_Message_textBox.Lines = newLines;
                }
                GetMain_Form.GameLuaScript_Message_textBox.SelectionStart = 0;
                var time = DateTime.Now;
                GetMain_Form.GameLuaScript_Message_textBox.SelectedText += $"{time.ToShortDateString()} {time.ToLongTimeString()}: {text.Replace("\n", "|")}" + Environment.NewLine;
            }*/
        }
        /// <summary>
        /// 修改Lua脚本状态栏
        /// </summary>
        /// <param name="text">要改变的文字</param>
        /// <param name="fontColor">文字字体颜色</param>
        /// <param name="backColor">背景框颜色</param>
        /// <param name="bold">字体是否粗体</param>
        public void Set_ChangeStatus(string text, Color fontColor, Color backColor, bool bold = true)
        {
            if (GetMain_Form.InvokeRequired)
            {
                GetMain_Form.BeginInvoke(new Action<string, Color, Color, bool>(Set_ChangeStatus), text, fontColor, backColor, bold);
            }
            else
            {
                GetMain_Form.GameLuaScript_Status_textBox.Text = text;
                GetMain_Form.GameLuaScript_Status_textBox.ForeColor = fontColor;
                GetMain_Form.GameLuaScript_Status_textBox.BackColor = backColor;
                if (bold)
                    GetMain_Form.GameLuaScript_Status_textBox.Font = new Font(GetMain_Form.GameLuaScript_Status_textBox.Font, FontStyle.Bold);
                else
                    GetMain_Form.GameLuaScript_Status_textBox.Font = new Font(GetMain_Form.GameLuaScript_Status_textBox.Font, FontStyle.Regular);
            }
        }


        /// <summary>
        /// 线程入口
        /// </summary>
        protected override void Entrance()
        {
            try
            { LuaScriptStart(); }
            catch (Exception e)
            {
                if ((e.InnerException ?? e).GetType() == typeof(ThreadAbortException))
                { return; }
                Function.FunctionExceptionLog.Write("脚本已因错误关闭：", e.InnerException ?? e);
            }
            finally
            {
                this.IsWorking = false;
                this.StopThread();
            }
        }
        /// <summary>
        /// Lua 脚本线程入口
        /// </summary>
        private void LuaScriptStart()
        {
            using (var luaObject = new NLua.Lua())// Lua 对象
            {
                luaObject["C"] = this;// // 提供给 Lua 可使用的接口 
                luaObject.RegisterFunction("MessageBoxShow", typeof(MessageBox).GetMethod("Show", new Type[] { typeof(string) }));
                luaObject.RegisterFunction("CreateDataJudge", this, this.GetType().GetMethod("CreateDataJudge", new Type[] { typeof(string), typeof(string) }));
                luaObject.RegisterFunction("CreateDataClick", this, this.GetType().GetMethod("CreateDataClick", new Type[] { typeof(string), typeof(string) }));
                luaObject.RegisterFunction("Delay", this, this.GetType().GetMethod("Delay", new Type[] { typeof(int) }));
                luaObject.RegisterFunction("StopThread", this, this.GetType().GetMethod("StopThread"));
                //luaObject.RegisterFunction("Set_ChangeStatus",this,this.GetType().GetMethod("Set_ChangeStatus",new Type[] { typeof(string), typeof(Color), typeof(Color), typeof(bool)}));
                //luaObject.RegisterFunction("Set_AddMassage", this, this.GetType().GetMethod("Set_AddMassage", new Type[] { typeof(string) }));

                while (true)
                {
                    Set_AddMessage($"Lua脚本:{this._scriptName} 开始执行...");
                    if (Cycles == 0) { }
                    else if (ExecutedNumber >= Cycles) break;
                    ExecutedNumber++;

                    WaitQueue();// 加入到消息队列
                    WorkStart:
                    try
                    {
                        // 改变状态栏
                        Set_ChangeStatus($"{ExecutedNumber}:Lua 脚本正在执行...", Color.White, Color.Orange);
                        // 执行读入的脚本
                        _luaScriptText.BaseStream.Position = 0;
                        var ret = luaObject.DoString(_luaScriptText.ReadToEnd()).First();

                        // 结束脚本工作
                        this.IsWorking = false;

                        // 是否循环
                        if (Cycles == 0) { }
                        else if (ExecutedNumber >= Cycles) break;
                        Set_AddMessage($"{this._scriptName} 已挂起等待，等待时间：{WaitTime}");
                        Delay(2000);
                        var wt = new TimeSpan();
                        while (WaitTime > 0)
                        {
                            wt = Functions.MillisecondToTime(WaitTime);
                            Set_ChangeStatus($"{ExecutedNumber}:等待时间：{wt.Days * 24 + wt.Hours}:{wt.Minutes}:{wt.Seconds}", Color.White, Color.Lime);
                            Delay(1000);
                            WaitTime -= 1000;
                        }
                    }
                    catch (ThreadAbortException) { }
                    catch (NLua.Exceptions.LuaScriptException e)// 来自脚本的错误
                    {
                        if ((e.InnerException ?? e).GetType() == typeof(Exceptions.TimeOutException))
                        {
                            // 超时
                            OvertimeProcess();
                            goto WorkStart;
                        }
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// 检测是否有未补给舰船
        /// </summary>
        /// <param name="GroupMember"></param>
        /// <returns></returns>
        public bool SupplyReadiness(LuaTable GroupMember)
        {
            using (Bitmap bmp = new Bitmap(this.GetGameBitmap))
            {
                if (!Judge(new DataJudge("637,130 @ 716E6D|FFF6F2|3A3A3A|E1D9D6", "出击舰队选择"), bmp))
                    throw new Exceptions.TimeOutException("判定异常错误！！");
                for (int j = 0; j < GroupMember.Keys.Count; j++)
                {
                    if ((bool)GroupMember[j + 1])
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
        /// <summary>
        /// 检测出击时是否有破损船
        /// </summary>
        /// <param name="GroupMember"></param>
        /// <param name="DetectionStatus">要检测的破损程度（3=仅检测大破、2=检测中破和大破、1=检测小破中破和大破）</param>
        /// <returns></returns>
        public bool DamagedReadiness(LuaTable GroupMember, int DetectionStatus)
        {
            using (Bitmap bmp = new Bitmap(this.GetGameBitmap))
            {
                if (!Judge(new DataJudge("637,130 @ 716E6D|FFF6F2|3A3A3A|E1D9D6", "出击舰队选择"), bmp))
                    throw new Exceptions.TimeOutException("出击检测破损页面判定异常错误！！");
                for (int j = 0; j < GroupMember.Keys.Count; j++)
                {
                    if ((bool)GroupMember[j + 1])
                    {
                        if (DetectionStatus <= 3)
                        {
                            if ((Judge(DataJudgePonds.出击大破1List[j], bmp) && Judge(DataJudgePonds.出击大破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击黄脸大破1List[j], bmp) && Judge(DataJudgePonds.出击黄脸大破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击红脸大破1List[j], bmp) && Judge(DataJudgePonds.出击红脸大破2List[j], bmp)))
                                return false;
                        }
                        if (DetectionStatus <= 2)
                        {
                            if ((Judge(DataJudgePonds.出击中破1List[j], bmp) && Judge(DataJudgePonds.出击中破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击黄脸中破1List[j], bmp) && Judge(DataJudgePonds.出击黄脸中破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击红脸中破1List[j], bmp) && Judge(DataJudgePonds.出击红脸中破2List[j], bmp)))
                                return false;
                        }
                        if (DetectionStatus <= 1)
                        {
                            if ((Judge(DataJudgePonds.出击小破1List[j], bmp) && Judge(DataJudgePonds.出击小破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击黄脸小破1List[j], bmp) && Judge(DataJudgePonds.出击黄脸小破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击红脸小破1List[j], bmp) && Judge(DataJudgePonds.出击红脸小破2List[j], bmp)))
                                return false;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 检测进击时是否有破损船
        /// </summary>
        /// <param name="GroupMember"></param>
        /// <param name="DetectionStatus"></param>
        /// <returns></returns>
        public bool AdvancingDamagedReadiness(LuaTable GroupMember, int DetectionStatus)
        {
            using (Bitmap bmp = new Bitmap(this.GetGameBitmap))
            {
                if (!(Judge(new DataJudge("49,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "成果2"), bmp) &&
                    Judge(new DataJudge("762,443 @ CDDADB", "出击成果可点"), bmp, 80)))
                    throw new Exceptions.TimeOutException("进击检测破损页面判定异常错误！！");
                var 进击大破1 = new DataJudge(new List<DataJudge>()
                {
                    new DataJudge("312,208 @ 99403B|8D3834","大破1"),
                    new DataJudge("312,208 @ A94530|A14029","黄脸大破1"),
                    new DataJudge("312,208 @ A93430|A12D29","红脸大破1"),
                });
                var 进击大破2 = new DataJudge(new List<DataJudge>()
                {
                    new DataJudge("317,194 @ A94B46","大破2"),
                    new DataJudge("317,194 @ BD5034","黄脸大破2"),
                    new DataJudge("317,194 @ BD3734","红脸大破2"),
                });
                var 进击中破1 = new DataJudge(new List<DataJudge>()
                {
                    new DataJudge("312,202 @ FDBA5C|FDBA5C","中破1"),
                    new DataJudge("312,202 @ FBA94B|FBA84A","黄脸中破1"),
                    new DataJudge("312,202 @ FB984B|FB954A","红脸中破1"),
                });
                var 进击中破2 = new DataJudge(new List<DataJudge>()
                {
                    new DataJudge("315,192 @ FFBC5D|FDBA5C","中破2"),
                    new DataJudge("315,192 @ FDA848|FBA446","黄脸中破2"),
                    new DataJudge("315,192 @ FD9248|F98C45","红脸中破2"),
                });
                var 进击小破1 = new DataJudge(new List<DataJudge>()
                {
                    new DataJudge("318,197 @ F7E25A|F6E159","小破1"),
                    new DataJudge("318,197 @ F7BF42|F6BD3F","黄脸小破1"),
                    new DataJudge("318,197 @ F7A542|F6A13F","红脸小破1"),
                });
                var 进击小破2 = new DataJudge(new List<DataJudge>()
                {
                    new DataJudge("320,212 @ ECD856","小破2"),
                    new DataJudge("320,212 @ EFB43C","黄脸小破2"),
                    new DataJudge("320,212 @ EF973C","红脸小破2"),
                });
                for (int j = 0; j < GroupMember.Keys.Count; j++)
                {
                    if ((bool)GroupMember[j + 1])
                    {
                        if (DetectionStatus <= 3)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (Judge(进击大破1[k], bmp, 0, 0, 45 * j) && Judge(进击大破2[k], bmp, 0, 0, 45 * j))
                                    return false;
                            }
                        }
                        if (DetectionStatus <= 2)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (Judge(进击中破1[k], bmp, 0, 0, 45 * j) && Judge(进击中破2[k], bmp, 0, 0, 45 * j))
                                    return false;
                            }
                        }
                        if (DetectionStatus <= 1)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (Judge(进击小破1[k], bmp, 0, 0, 45 * j) && Judge(进击小破2[k], bmp, 0, 0, 45 * j))
                                    return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 补给舰队
        /// </summary>
        /// <param name="GroupMember"></param>
        public void Supply(LuaTable GroupMember)
        {
            补给队伍:
            int i = 0;
            /*
            while (true)
            {
                if (Judge(DataJudgePonds.补给页))
                {
                    if (Judge(new DataJudge("145,118 @ 23A0A1|23A0A1|D4E1DE|FFF6F2", "补给队伍1")))
                        break;
                    else
                    {
                        Click(new DataClick("145,117|152,120", "补给队伍1"));
                        i = 0;
                    }
                }
                Overtime(ref i, 30000);
            }*/

            if (!Judge(DataJudgePonds.补给页))
            { throw new Exceptions.TimeOutException("补给页判定异常错误！！"); }

            // 检测舰船数量
            int p = 0;
            for (i = 0; i < 6; i++)
            {
                if (Judge(DataJudgePonds.补给舰娘存活List[i]))
                    p++;
            }
            p = 6 - p;// 队伍中的舰娘数量

            bool isOnece = true;
            foreach (dynamic item in GroupMember)
            {
                if ((bool)(item.Value) == false)
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
                    if ((bool)GroupMember[j + 1])
                    {
                        if (Judge(DataJudgePonds.补油判定List[j]) && Judge(DataJudgePonds.补弹判定List[j]))
                            continue;
                        else
                        {
                            i = 0;
                            while (true)
                            {
                                if (Judge(new DataJudge("695,440 @ 494949|535353|A0A0A0|B9B9B9", "まとめと补给灰")) &&
                                    !(Judge(DataJudgePonds.补油判定List[j]) && Judge(DataJudgePonds.补弹判定List[j])))
                                {
                                    Click(单舰补给选择框, 200, 0, 52 * j);
                                    i = 0;
                                }
                                else if (Judge(new DataJudge("695,440 @ 045758|285950|D1D6CB|FFF5F1", "まとめと补给可选")) ||
                                    Judge(new DataJudge("695,440 @ C5560D|B06F2C|FFCFAC|FFE0D3", "まとめと补给已选")))
                                {
                                    Click(new DataClick("671,435|740,448", "まとめて補給"));
                                    i = 0;
                                }
                                else if (Judge(DataJudgePonds.补油判定List[j]) && Judge(DataJudgePonds.补弹判定List[j]))
                                    break;
                                Overtime(ref i, 30000);
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
                    i = 0;
                    while (true)
                    {
                        if (Judge(DataJudgePonds.非全舰补给) || Judge(DataJudgePonds.全舰补给))
                        {
                            Click(DataClickPonds.一括补给);
                            Delay(200);
                            goto 补给队伍;
                        }
                        Overtime(ref i, 30000);
                    }
                }
            }
        }

        /// <summary>
        /// 自动入渠队伍1
        /// </summary>
        /// <param name="isTeam2">是否入渠队伍2</param>
        /// <param name="DetectionStatus">修复基准</param>
        public void Dock(bool isTeam2, int DetectionStatus)
        {
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
                            Set_ChangeStatus($"{this.ExecutedNumber}:检测到入渠位1已占用！！！", Color.White, Color.Red);
                            Delay(500);
                            Set_ChangeStatus($"{this.ExecutedNumber}:检测到入渠位1已占用！！！", Color.Black, Color.Ivory);
                            Delay(500);
                        }
                        i = 0;
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
                       (isTeam2 && Judge(new DataJudge("391,138 @ 41A9AA|CDDEDB|D4E2DF", "队伍2标志"), null, 0, 0, j * 31))))
                        {
                            if (Readiness(j, DetectionStatus))
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
        }
        /// <summary>
        /// 超时系统
        /// </summary>
        public void OvertimeProcess()
        {
            Delay(200);
            if (Judge(DataJudgePonds.母港1)) return;
            DateTime Pt = DateTime.Now;//超时时刻
            TimeSpan Ptm = TimeSpan.Zero;

            Set_AddMessage("判定已超时。");
            Set_ChangeStatus($"{ExecutedNumber}:Lua 脚本判定已超时 {((int)Ptm.TotalSeconds).ToString()} 秒", Color.White, Color.Red);

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
                Set_ChangeStatus($"{ExecutedNumber}:Lua 脚本判定已超时 {((int)Ptm.TotalSeconds).ToString()} 秒", Color.White, Color.Red);

                if (Judge(DataJudgePonds.母港1))
                {
                    Delay(200);
                    Set_AddMessage($"超时检测已结束，共超时： {((int)Ptm.TotalSeconds).ToString()}秒。");
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
                    this.StopThread();
                }
            }
        }

        /// <summary>
        /// 创建一个新的判断数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataJudge CreateDataJudge(string value, string name = "")
        {
            return new DataJudge(value, name);
        }
        /// <summary>
        /// 创建一个新的点击数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataClick CreateDataClick(string value, string name = "")
        {
            return new DataClick(value, name);
        }
        /// <summary>
        /// 获取指定位置与大小的游戏界面颜色（用于判别界面是否有改变）
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public string GetColorForPoint(string xy, int width = 4, int height = 1)
        {
            using (Bitmap bmp = new Bitmap(this.GetGameBitmap))
            {
                return FunctionBitmap.GetColorData(bmp, xy, width, height);
            }
        }
        /// <summary>
        /// 抛出一个超时错误
        /// </summary>
        public void ThrowTimeOutException()
        {
            throw new Exceptions.TimeOutException($"{this._scriptName} 界面判定已超时！");
        }
        /// <summary>
        /// 抛出一个ThreadAbortException异常
        /// </summary>
        public void ThrowThreadAbortException()
        {
            this.Thread.Abort();
        }


        /// <summary>
        /// 开启 Lua 线程
        /// </summary>
        public override void StartThread()
        {
            try
            {
                if (this._scriptName == string.Empty) return;
                // 载入 Lua 脚本
                _luaScriptText = new StreamReader(Application.StartupPath + $@"\LuaScript\{this._scriptName}.lua", Encoding.UTF8);
                if (_luaScriptText != null)
                {
                    base.StartThread();
                }
            }
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
            catch (Exception e)
            {
                Set_ChangeStatus("Lua 脚本打开失败！", Color.Red, Color.White);
                Function.FunctionExceptionLog.Write("Lua 脚本打开失败。", e);
                this.StopThread();
            }
        }
        /// <summary>
        /// 关闭 Lua 线程
        /// </summary>
        public override void StopThread()
        {
            if (this.IsDisposed) return;
            if (GetMain_Form.GameLuaScript_Status_textBox.BackColor != Color.White)
            {
                Set_ChangeStatus("Lua 脚本已关闭", Color.Black, Color.White, false);
            }
            try
            { base.StopThread(); }
            finally
            { Dispose(); }
        }

        /// <summary>
        /// 释放由 LuaScript 对象使用的所有资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._isDisposed)
            {
                if (disposing) { }
                _luaScriptText?.Dispose();
            }
            this._isDisposed = true; // 标识此对象已释放
        }



        /// <summary>
        /// 构造 Lua 脚本对象
        /// </summary>
        /// <param name="scriptName">脚本名</param>
        public LuaScript(string scriptName) : base("Lua脚本")
        {
            this._scriptName = scriptName;
            this.StartThread();
        }
        ~LuaScript() { Dispose(false); }
    }
}