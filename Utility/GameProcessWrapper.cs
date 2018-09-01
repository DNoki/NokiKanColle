using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

using NokiKanColle.Data;
using NokiKanColle.Function;
using static NokiKanColle.Function.Functions;
using static NokiKanColle.Data.DataPond;

namespace NokiKanColle.Utility
{
    /// <summary>
    /// 流程抽象类
    /// </summary>
    public abstract class GameProcessWrapper : ThreadsWrapper
    {
        /// <summary>
        /// 是否处于工作模式（只有处于工作模式的线程才能对游戏造成影响，当结束工作模式时，消息队列会移出该线程）
        /// </summary>
        public virtual bool IsWorking { get; set; }
        protected Bitmap GetGameBitmap => GlobalObject.GameHandle.GetGameBitmap();

        /// <summary>
        /// 开启流程
        /// </summary>
        public override void StartThread()
        {
            if (!GlobalObject.ProgramPermit.GetPermit)
            {
                throw new Exceptions.NoPermitException();
            }
            // 若没有抓到游戏窗口则不允许开启线程
            if (!GlobalObject.GameHandle.IsSuccess)
            {
                throw new Exceptions.NoGameHandleException();
            }
            base.StartThread();
        }
        public override void StopThread()
        {
            // 移除消息队列中的流程
            if (Function.FunctionThread.Contains("消息队列（不可关闭）"))
                Function.FunctionThread.GetThread<MessageQueue>("消息队列（不可关闭）").Remove(this, false);
            this.IsWorking = false;
            base.StopThread();
        }

        /// <summary>
        /// 单流程
        /// </summary>
        /// <param name="instruction">(判断，行为，跳出)指令集合</param>
        /// <param name="overtime">设定超时时间</param>
        protected void SingleProcess((Func<bool>, Action, bool)[] instruction, int overtime = 30000)
        {
            // 判断页面，然后点击，然后指定是否跳出循环
            var t = 0;
            while (true)
            {
                // 循环判断页面，当判断到指定页面则执行相对应的指令
                foreach (var order in instruction)
                {
                    // 判断指令
                    if (order.Item1.Invoke())
                    {
                        // 若指令存在则执行
                        order.Item2?.Invoke();
                        if (order.Item3) return;// 指定跳出循环
                        else
                        {
                            t = 0; // 重置时间
                            break; // 继续执行页面判断
                        }
                    }
                }
                Overtime(ref t, overtime);
            }
        }
        /// <summary>
        /// 创建指令集
        /// </summary>
        /// <param name="instructions">(判断，行为，跳出)指令集</param>
        /// <returns></returns>
        protected (Func<bool>, Action, bool)[] CreateInstruction(params (Func<bool>, Action, bool)[] instructions) => instructions;
        /// <summary>
        /// 返回一个闭包函数，该函数会对当前页面进行判断并返回结果
        /// </summary>
        /// <param name="logical">判断数据的逻辑关系</param>
        /// <param name="dataes">判断数据集</param>
        /// <returns></returns>
        protected Func<bool> JudgeFunction(LogicalRelationship logical = LogicalRelationship.AND, params DataJudge[] dataes)
        {
            return new Func<bool>(() =>
            {
                using (var bitmap = this.GetGameBitmap)
                {
                    var result = false;
                    foreach (var data in dataes)
                    {
                        if (Judge(data, bitmap))
                        {
                            if (logical == LogicalRelationship.AND)
                            {
                                result = true;
                                continue;
                            }
                            else if (logical == LogicalRelationship.OR) return true;
                        }
                        else
                        {
                            if (logical == LogicalRelationship.AND) return false;
                            else if (logical == LogicalRelationship.OR) continue;
                        }

                    }
                    return result;
                }
            });
        }
        /// <summary>
        /// 返回一个闭包函数，该函数会执行一次点击
        /// </summary>
        /// <param name="dataClick">点击数据</param>
        /// <returns></returns>
        protected Action ClickAction(DataClick dataClick) => new Action(() => Click(dataClick));

        /// <summary>
        /// 逻辑状态
        /// </summary>
        public enum LogicalRelationship
        {
            AND, OR
        }

        /// <summary>
        /// 检测是否有未补给舰船
        /// </summary>
        /// <param name="GroupMember"></param>
        /// <returns></returns>
        public virtual bool SupplyReadiness(bool[] GroupMember)
        {
            using (Bitmap bmp = new Bitmap(GlobalObject.GameHandle.GetGameBitmap()))
            {
                if (!Judge(new DataJudge("637,130 @ 716E6D|FFF6F2|3A3A3A|E1D9D6", "出击舰队选择"), bmp))
                    throw new Exceptions.TimeOutException("判定异常错误！！");
                if (GroupMember.Length != 6) throw new Exceptions.DataErrorException("舰船数量错误！！");
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
        /// <summary>
        /// 检测出击时是否有破损船
        /// </summary>
        /// <param name="GroupMember"></param>
        /// <param name="DetectionStatus">要检测的破损程度（3=仅检测大破、2=检测中破和大破、1=检测小破中破和大破）</param>
        /// <returns></returns>
        public virtual bool DamagedReadiness(bool[] GroupMember, int DetectionStatus)
        {
            using (Bitmap bmp = new Bitmap(this.GetGameBitmap))
            {
                if (!Judge(new DataJudge("637,130 @ 716E6D|FFF6F2|3A3A3A|E1D9D6", "出击舰队选择"), bmp))
                    throw new Exceptions.TimeOutException("出击检测破损页面判定异常错误！！");
                if (GroupMember.Length != 6) throw new Exceptions.DataErrorException("舰船数量错误！！");
                for (int j = 0; j < GroupMember.Length; j++)
                {
                    if (GroupMember[j])
                    {
                        if (DetectionStatus <= 3)
                        {
                            if ((Judge(DataJudgePonds.出击大破1List[j], bmp) && Judge(DataJudgePonds.出击大破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击黄脸大破1List[j], bmp) && Judge(DataJudgePonds.出击黄脸大破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击红脸大破1List[j], bmp) && Judge(DataJudgePonds.出击红脸大破2List[j], bmp)))
                                return true;
                        }
                        if (DetectionStatus <= 2)
                        {
                            if ((Judge(DataJudgePonds.出击中破1List[j], bmp) && Judge(DataJudgePonds.出击中破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击黄脸中破1List[j], bmp) && Judge(DataJudgePonds.出击黄脸中破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击红脸中破1List[j], bmp) && Judge(DataJudgePonds.出击红脸中破2List[j], bmp)))
                                return true;
                        }
                        if (DetectionStatus <= 1)
                        {
                            if ((Judge(DataJudgePonds.出击小破1List[j], bmp) && Judge(DataJudgePonds.出击小破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击黄脸小破1List[j], bmp) && Judge(DataJudgePonds.出击黄脸小破2List[j], bmp)) ||
                            (Judge(DataJudgePonds.出击红脸小破1List[j], bmp) && Judge(DataJudgePonds.出击红脸小破2List[j], bmp)))
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 检测进击时是否有破损船
        /// </summary>
        /// <param name="GroupMember"></param>
        /// <param name="DetectionStatus">要检测的破损程度（3=仅检测大破、2=检测中破和大破、1=检测小破中破和大破）</param>
        /// <returns></returns>
        public virtual bool AdvancingDamagedReadiness(bool[] GroupMember, int DetectionStatus)
        {
            using (Bitmap bmp = new Bitmap(this.GetGameBitmap))
            {
                if (!(Judge(new DataJudge("49,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "成果2"), bmp) &&
                    Judge(new DataJudge("762,443 @ CDDADB", "出击成果可点"), bmp, 80)))
                    throw new Exceptions.TimeOutException("进击检测破损页面判定异常错误！！");
                if (GroupMember.Length != 6) throw new Exceptions.DataErrorException("舰船数量错误！！");
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
                for (int j = 0; j < GroupMember.Length; j++)
                {
                    if (GroupMember[j])
                    {
                        if (DetectionStatus <= 3)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (Judge(进击大破1[k], bmp, 0, 0, 45 * j) && Judge(进击大破2[k], bmp, 0, 0, 45 * j))
                                    return true;
                            }
                        }
                        if (DetectionStatus <= 2)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (Judge(进击中破1[k], bmp, 0, 0, 45 * j) && Judge(进击中破2[k], bmp, 0, 0, 45 * j))
                                    return true;
                            }
                        }
                        if (DetectionStatus <= 1)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                if (Judge(进击小破1[k], bmp, 0, 0, 45 * j) && Judge(进击小破2[k], bmp, 0, 0, 45 * j))
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 发送点击指令
        /// </summary>
        /// <param name="XY">点击坐标</param>
        /// <param name="delayTime">延迟时间（单位：毫秒）</param>
        /// <param name="OffsetX"></param>
        /// <param name="OffsetY"></param>
        /// <returns></returns>
        public virtual Point Click(DataClick XY, int delayTime = 0, int OffsetX = 0, int OffsetY = 0)
        {
            if (XY == null) throw new Exceptions.DataErrorException("给定的点击数据异常！");
            // 若没有抓到游戏窗口则关闭线程
            if (!GlobalObject.GameHandle.IsSuccess)
            {
                throw new Exceptions.NoGameHandleException();
            }
            return FunctionClick.Click(XY, delayTime, OffsetX, OffsetY);
        }
        ///// <summary>
        ///// 判断界面
        ///// </summary>
        ///// <param name="data">判断数据</param>
        ///// <param name="rin">容许色差阈值（0~441）</param>
        ///// <param name="OffsetX">X坐标偏移</param>
        ///// <param name="OffsetY">Y坐标偏移</param>
        ///// <returns></returns>
        //public virtual bool Judge(DataJudge data, int rin = 0, int OffsetX = 0, int OffsetY = 0)
        //{
        //    if (data == null) throw new Exceptions.DataErrorException("给定的判断数据异常！");
        //    // 若没有抓到游戏窗口则关闭线程
        //    if (!GlobalObject.GameHandle.IsSuccess)
        //    {
        //        throw new Exceptions.NoGameHandleException();
        //    }
        //    return FunctionJudge.Judge(data, rin, OffsetX, OffsetY);
        //}
        public virtual bool Judge(DataJudge data, Bitmap bmp = null, int rin = 0, int OffsetX = 0, int OffsetY = 0)
        {
            if (data == null) throw new Exceptions.DataErrorException("给定的判断数据异常！");
            // 若没有抓到游戏窗口则关闭线程
            if (!GlobalObject.GameHandle.IsSuccess)
            {
                throw new Exceptions.NoGameHandleException();
            }
            var result = FunctionJudge.Judge(data, bmp, rin, OffsetX, OffsetY);
            return data.IsTrueResult ? result : !result;
        }

        /// <summary>
        /// 将线程加到消息队列，并等待开始工作指令
        /// </summary>
        public virtual void WaitQueue()
        {
            try
            {
                Function.FunctionThread.GetThread<MessageQueue>("消息队列（不可关闭）").Enqueue(this);
                while (!IsWorking)//等待开始工作指令
                {
                    if (!Function.FunctionThread.GetThread<MessageQueue>("消息队列（不可关闭）").Contains(this))
                    {
                        Function.FunctionThread.GetThread<MessageQueue>("消息队列（不可关闭）").Enqueue(this);
                    }
                    Delay(1000);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                Function.FunctionExceptionLog.Write(e.InnerException ?? e);
                StopThread();
            }

        }

        /// <summary>
        /// 超时系统
        /// </summary>
        /// <param name="time">已经过时间（单位：毫秒）</param>
        /// <param name="setTime">设定超时时间（单位：毫秒）</param>
        /// <param name="waitTime">等待时间（单位：毫秒）</param>
        /// <returns></returns>
        public void Overtime(ref int time, int setTime, int waitTime = 200)
        {
            if (time > setTime)
            {
                throw new Exceptions.TimeOutException("界面判定已超时！");
            }
            Delay(waitTime);
            time += waitTime;
            return;
        }


        public GameProcessWrapper(string name) : base(name) { }
    }
}