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
    public class EventAttack : GameProcessWrapper
    {
        /// <summary>
        /// 是否联合舰队
        /// </summary>
        private bool isUnion => Convert.ToBoolean(GetMain_Form.Invoke(new Func<bool>(() => this.GetMain_Form.GetEventAttackIsUnion)));
        /// <summary>
        /// 是否进行陆基补给
        /// </summary>
        private bool isBaseAirCorps => Convert.ToBoolean(GetMain_Form.Invoke(new Func<bool>(() => this.GetMain_Form.GetEventAttackIsBaseAirCorps)));
        /// <summary>
        /// 是否入渠
        /// </summary>
        private bool isDock => Convert.ToBoolean(GetMain_Form.Invoke(new Func<bool>(() => this.GetMain_Form.GetEventAttackIsDock)));
        /// <summary>
        /// 入渠基准
        /// </summary>
        private int dockBenchmark => Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => this.GetMain_Form.GetEventAttackDockBenchmark)));
        /// <summary>
        /// 撤退条件
        /// </summary>
        private int detectionStatus => Convert.ToInt32(GetMain_Form.Invoke(new Func<int>(() => this.GetMain_Form.GetEventAttackDetectionStatus)));



        private string str期间限定海域 = "144,149 @ 87745D|D0BFAD|86745C|91816C|C6B8A7|AF9D8A";
        private List<string> str第N航空队 = new List<string>() {
            "616,120 @ 61513E|665642|695944|8B8170|938C7D",
            "682,119 @ 776C5C|716350|6D5D48|8A7E6D|877C6A",
            "751,121 @ 6A5A46|6C5C47|736452|5E4E3B|5C4C39"
        };
        private string str大淀信息 = "705,84 @ EAF5F9|516E6D|638E8B|29C6B6|6ADBD4";

        private List<string> str点击第N航空队 = new List<string>()
        {
            "604,119|631,122",
            "672,119|704,125",
            "744,118|764,123"
        };
        private string str地图 = "609,371|653,385";

        private Dictionary<string, DataClick> ClickDataPond = new Dictionary<string, DataClick>()
        {
            { "关闭陆基",new DataClick("131,113|142,123","关闭陆基（位于海域选择下面的舵图标）") }
        };

        /// <summary>
        /// 更改状态栏
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontColor"></param>
        /// <param name="backColor"></param>
        /// <param name="bold"></param>
        private void Set_ChangeStatus(string text, Color fontColor, Color backColor, bool bold = true)
        => GetMain_Form.SetEventAttackStatus(text, fontColor, backColor, bold);
        /// <summary>
        /// 添加信息到信息栏
        /// </summary>
        /// <param name="text"></param>
        protected override void Set_AddMessage(string text) => base.Set_AddMessage(text);


        protected override void Entrance()
        {
            try
            {
                EventAttackStart();
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                FunctionExceptionLog.Write($"发生致命错误，出击线程已关闭:", e);
                this.StopThread();
            }
        }
        private void EventAttackStart()
        {
            while (true)
            {
                WaitQueue();

                // 工作代码
                WorkStart:
                try
                {
                    Process1();
                }
                catch (ThreadAbortException) { }
                catch (Exceptions.ReactivateThreadException)
                {
                    // 在工作模式内重新开始线程
                    Delay(500);
                    goto WorkStart;
                    throw;
                }
                catch (Exceptions.TimeOutException)
                {
                    // 超时
                    OvertimeProcess();
                }
                catch (Exceptions.DataErrorException e)
                {
                    // 数据异常
                    FunctionExceptionLog.Write($"数据异常，出击线程已关闭:", e);
                    this.StopThread();
                }
                IsWorking = false;
            }
            this.StopThread();
        }
        private void Process1()
        {
            //SingleProcess(CreateInstruction(
            //    (JudgeFunction(LogicalRelationship.AND,
            //        ),
            //        ClickAction(), true)));

            // 母港
            SingleProcess(CreateInstruction(
                (JudgeFunction(dataes: DataJudgePonds.母港1), ClickAction(DataClickPonds.母港出击), true),
                (JudgeFunction(LogicalRelationship.OR, DataJudgePonds.远征回港, DataJudgePonds.远征归来1, DataJudgePonds.远征归来2), ClickAction(DataClickPonds.舰娘立绘), false))
                , 5000);

            // 出击选择
            SingleProcess(CreateInstruction(
                (JudgeFunction(dataes: new DataJudge("140,77 @ 1EAAAB|1CB2B5|1DBDC0|1EAAAC|1EAAAC|208F90", "出击选择")),
                ClickAction(new DataClick("230,128|170,155|145,228|180,284|259,296|306,260|316,199|277,159", "出击")), true)));

            // 海域选择
            SingleProcess(CreateInstruction(
                (JudgeFunction(LogicalRelationship.AND,
                new DataJudge("574,122 @ 7D2D1F|671600|FFEFCE|82895B", "海域选择"),
                new DataJudge(str期间限定海域, "期间限定海域")),
                null, true),
                (JudgeFunction(LogicalRelationship.AND,
                new DataJudge("574,122 @ 7D2D1F|671600|FFEFCE|82895B", "海域选择"),
                new DataJudge(str期间限定海域, "期间限定海域", false)),
                ClickAction(new DataClick("710,446|727,453", "点击期间限定海域")), false)));
            //var i = 0;
            //while (true)
            //{
            //    if (Judge(new Data.DataJudge("140,77 @ 1EAAAB|1CB2B5|1DBDC0|1EAAAC|1EAAAC|208F90", "出击选择")))
            //    {
            //        Click(new Data.DataClick("230,128|170,155|145,228|180,284|259,296|306,260|316,199|277,159", "出击"));
            //        break;
            //    }
            //    Overtime(ref i, 30000);
            //}

            //// 海域选择
            //i = 0;
            //while (true)
            //{
            //    if (Judge(new Data.DataJudge("574,122 @ 7D2D1F|671600|FFEFCE|82895B", "海域选择")))
            //    {
            //        if (Judge(new DataJudge(str期间限定海域, "期间限定海域")))
            //        {
            //            // 已经进入期间限定海域页面
            //            break;
            //        }
            //        else
            //        {
            //            Click(new DataClick("710,446|727,453", "点击期间限定海域"));
            //            continue;
            //        }
            //    }
            //    Overtime(ref i, 30000);
            //}

            // 陆基补给
            if (this.isBaseAirCorps)
            {
                Process1Sub1();
            }

            // 选择活动图
            SingleProcess(CreateInstruction(
                (JudgeFunction(LogicalRelationship.AND,
                new DataJudge(str期间限定海域, "期间限定海域")),
                ClickAction(new DataClick(str地图, "点击地图")), true)));

            // 大淀信息
            SingleProcess(CreateInstruction(
                (JudgeFunction(LogicalRelationship.AND,
                DataJudgePonds.出击详细),
                ClickAction(DataClickPonds.出击决定), true),
                (JudgeFunction(LogicalRelationship.AND,
                new DataJudge(str大淀信息, "大淀信息")),
                ClickAction(ClickDataPond["关闭陆基"]), false)));


            //// 选择活动图
            //var i = 0;
            //while (true)
            //{
            //    if (Judge(new DataJudge(str期间限定海域, "期间限定海域")))
            //    {
            //        Click(new DataClick(str地图, "点击地图"));
            //        break;
            //    }
            //    Overtime(ref i, 30000);
            //}

            //// 大淀信息
            //i = 0;
            //while (true)
            //{
            //    if (Judge(new DataJudge(str大淀信息, "大淀信息")))
            //    {
            //        Click(ClickDataPond["关闭陆基"]);
            //        continue;
            //    }
            //    else if (Judge(DataJudgePonds.出击详细))
            //    {
            //        Click(DataClickPonds.出击决定);
            //        break;
            //    }
            //    Overtime(ref i, 30000);
            //}

            //// 舰队选择
            //i = 0;
            //while (true)
            //{
            //    if (Judge(DataJudgePonds.出击舰队选择))
            //    {
            //        // 出击前检测破损
            //        if (DamagedReadiness(new bool[] { true, true, true, true, true, true }, this.detectionStatus))
            //        {

            //        }
            //    }
            //}
        }
        /// <summary>
        /// 进行陆基补给
        /// </summary>
        private void Process1Sub1()
        {
            // 期间限定海域
            SingleProcess(CreateInstruction(
                (JudgeFunction(LogicalRelationship.AND,
                new DataJudge(str期间限定海域, "期间限定海域"),
                new DataJudge("198,391 @ F6F6F5|F8F8F7|3A4A3B|738270|6B7B67|6F7F6C", "陆基补给")),
                ClickAction(new DataClick("191,392|261,397", "打开陆基")), true)));

            // 指定有几队航空队
            for (int i = 0; i < str第N航空队.Count; i++)
            {
                while (true)
                {
                    Delay(200);
                    // 补给指定的航空队
                    SingleProcess(CreateInstruction(
                    (JudgeFunction(LogicalRelationship.AND,
                    new DataJudge(str第N航空队[i], $"第{i + 1}航空队")),
                    null, true),
                    (JudgeFunction(LogicalRelationship.AND,
                    new DataJudge("201,391 @ 5C6859|556252|586556|556252|556252", "陆基补给已打开")),
                    ClickAction(new DataClick(str点击第N航空队[i], $"点击第{i + 1}航空队")), false
                    )));
                    
                    var agine = false;
                    // 判断到第i+1航空队
                    using (Bitmap nowImage = new Bitmap(this.GetGameBitmap))
                    {
                        if (!Judge(new DataJudge(str第N航空队[i], $"第{i + 1}航空队"), nowImage) &&
                            Judge(new DataJudge("201,391 @ 5C6859|556252|586556|556252|556252", "陆基补给已打开"), nowImage))
                            throw new Exceptions.TimeOutException("航空队图像截取错误！");
                        for (int k = 0; k < 4/* 陆基有四个格子 */; k++)
                        {
                            if (Judge(new DataJudge($"682,{250 + k * 70} @ 72B467|B2D4B2|79B478|81CC53", "陆基未补给"), nowImage))
                            {
                                // 进行补给
                                agine = true;
                                Delay(100);
                                Click(new DataClick("774,218|775,219", "陆基全补充"));
                                Click(new DataClick("774,218|775,219", "陆基全补充"));
                                Click(new DataClick("774,218|775,219", "陆基全补充"));
                                break;
                            }
                        }
                        if (agine) continue;
                        // 无需补给
                        break;
                    }
                }
            }

            // 补给完成回到海域
            SingleProcess(CreateInstruction(
                (JudgeFunction(dataes: new DataJudge("575,116 @ 601704|FFE9FE|9C7AB0|95180D", "海域选择")),
                null, true),
                (JudgeFunction(dataes: new DataJudge("201,391 @ 5C6859|556252|586556|556252|556252", "陆基补给已打开")),
                ClickAction(ClickDataPond["关闭陆基"]), false)));



            //var i = 0;
            //while (true)
            //{
            //    if (Judge(new DataJudge(str期间限定海域, "期间限定海域")) &&
            //        Judge(new DataJudge("198,391 @ F6F6F5|F8F8F7|3A4A3B|738270|6B7B67|6F7F6C", "陆基补给")))
            //    {
            //        // 已经进入期间限定海域页面
            //        Click(new DataClick("191,392|261,397", "打开陆基"));
            //        break;
            //    }
            //    Overtime(ref i, 30000);
            //}

            //i = 0;
            //// 指定有几队航空队
            //for (int j = 0; j < str第N航空队.Count; j++)
            //{
            //    Delay(200);
            //    // 补给指定的航空队
            //    i = 0;
            //    while (true)
            //    {
            //        var agine = false;
            //        if (Judge(new DataJudge(str第N航空队[j], $"第{j + 1}航空队")))
            //        {
            //            // 判断到第j+1航空队
            //            using (Bitmap nowImage = new Bitmap(this.GetGameBitmap))
            //            {
            //                if (!Judge(new DataJudge(str第N航空队[j], $"第{j + 1}航空队"), nowImage) &&
            //                    Judge(new DataJudge("201,391 @ 5C6859|556252|586556|556252|556252", "陆基补给已打开"), nowImage))
            //                    throw new Exceptions.TimeOutException("航空队图像截取错误！");
            //                for (int k = 0; k < 4/* 陆基有四个格子 */; k++)
            //                {
            //                    if (Judge(new DataJudge($"682,{250 + k * 70} @ 72B467|B2D4B2|79B478|81CC53", "陆基未补给"), nowImage))
            //                    {
            //                        // 进行补给
            //                        agine = true;
            //                        Click(new DataClick("770,219|779,222", "陆基全补充"));
            //                        break;
            //                    }
            //                }
            //            }
            //            if (agine) continue;
            //            // 无需补给
            //            break;
            //        }
            //        else if (Judge(new DataJudge("201,391 @ 5C6859|556252|586556|556252|556252", "陆基补给已打开")))
            //        {
            //            Click(new DataClick(str点击第N航空队[j], $"点击第{j + 1}航空队"));
            //            continue;
            //        }
            //        Overtime(ref i, 30000);
            //    }
            //}

            //// 补给完成回到海域
            //i = 0;
            //while (true)
            //{
            //    if (Judge(new DataJudge("201,391 @ 5C6859|556252|586556|556252|556252", "陆基补给已打开")))
            //    {
            //        Click(ClickDataPond["关闭陆基"]);
            //        continue;
            //    }
            //    else if (Judge(new DataJudge("575,116 @ 601704|FFE9FE|9C7AB0|95180D", "海域选择")))
            //    {
            //        break;
            //    }
            //    Overtime(ref i, 30000);
            //}
        }





        ///// <summary>
        ///// 等待系统
        ///// </summary>
        //private void WaitTimeProcess()
        //{
        //    TimeSpan time = SecondToTime(WaitTime);
        //    var waitTime = Convert.ToInt32(time.TotalMilliseconds);
        //    while (waitTime > 0)
        //    {
        //        if (IsDisposed) this.Thread.Abort();// 检测线程是否被关闭
        //        time = MillisecondToTime(waitTime);
        //        Set_ChangeStatus($"剩余时间 {(time.Hours * 24 + time.Hours) * 60 + time.Minutes}:{time.Seconds}", Color.White, Color.Lime);
        //        Delay(1000);
        //        waitTime -= 1000;
        //    }
        //    Set_ChangeStatus($"剩余时间 0:0", Color.White, Color.Lime);
        //    Delay(200);
        //}
        /// <summary>
        /// 超时
        /// </summary>
        private void OvertimeProcess()
        {
            throw new NotImplementedException();
        }

        public override void StartThread()
        {
            try
            {
                base.StartThread();
            }
            catch (Exceptions.NoPermitException)
            {
                throw new NotImplementedException();
                this.StopThread();
            }
            catch (Exceptions.NoGameHandleException)
            {
                throw new NotImplementedException();
                this.StopThread();
            }
        }
        public override void StopThread()
        {
            if (IsDisposed) return;
            this.IsWorking = false;
            throw new NotImplementedException();
            base.StopThread();
        }

        public EventAttack() : base(nameof(EventAttack))
        {
            this.StartThread();
        }
    }
}
