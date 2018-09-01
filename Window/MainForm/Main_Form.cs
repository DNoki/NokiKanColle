using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using NokiKanColle.Utility;
using NokiKanColle.Function;

namespace NokiKanColle.Window
{
    public partial class Main_Form : Form
    {
        private System.Threading.AutoResetEvent autoResetEvent = new System.Threading.AutoResetEvent(false);

        /// <summary>
        /// 当主窗口开启时发生的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Form_Load(object sender, EventArgs e)
        {
            // 添加版本号
            this.Text = $"绅士提督@无限舰制 V.{Function.GlobalObject.ProgramPermit.Version}";

            // 判断是否过期
            GlobalObject.ProgramPermit.Security(this.Main_JianshuMode_radioButton.Checked ? 1 : 2);
            //UI窗口对外接口
            AddGetUIFormMethod();
            //开启消息队列线程
            var queue = new Utility.MessageQueue();
            queue.StartThread();
            // 初始化参数
            GameExpedition_Initialization();
            GameAttack_Initialization();
            GameEventAttack_Initialization();
            // 读取配置文件
            if (File.Exists(Application.StartupPath + @"\配置文件.ini"))
            {
                string strFilePath = Application.StartupPath + @"\配置文件.ini";
                GameWindow_ReadPlacement(strFilePath);
                GameExpedition_ReadPlacement(strFilePath);
                GameAttack_ReadPlacement(strFilePath);
            }
        }
        /// <summary>
        /// 当主窗口关闭发生的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 写入配置
            if (true || File.Exists(Application.StartupPath + @"\配置文件.ini"))
            {
                string strFilePath = Application.StartupPath + @"\配置文件.ini";
                GameWindow_WritePlacement(strFilePath);
                GameExpedition_WritePlacement(strFilePath);
                GameAttack_WritePlacement(strFilePath);
            }
            Function.FunctionThread.KillAllThreads();//关闭所有线程
        }

        /// <summary>
        /// 计时器1000毫秒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Delay1000_timer_Tick(object sender, EventArgs e)
        {

        }

        //Main_Form对外接口
        private static readonly object _objectLock = new object();
        /// <summary>
        /// 向“UI窗口对外接口”委托添加方法
        /// </summary>
        private void AddGetUIFormMethod()
        { GlobalObject.GetUIForm = new Func<Main_Form>(() => { lock (_objectLock) { return this; } }); }
        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_ToolStripMenuItem_File_Exit_Click(object sender, EventArgs e)
        {
            /*
             * 1.this.Close();   只是关闭当前窗口，若不是主窗体的话，是无法退出程序的，另外若有托管线程（非主线程），也无法干净地退出；
             * 2.Application.Exit();  强制所有消息中止，退出所有的窗体，但是若有托管线程（非主线程），也无法干净地退出；
             * 3.Application.ExitThread(); 强制中止调用线程上的所有消息，同样面临其它线程无法正确退出的问题；
             * 4.System.Environment.Exit(0);   这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。
             */
            System.Environment.Exit(0);
        }

        /// <summary>
        /// 坐标取色窗口实例
        /// </summary>
        public Window.CatchData_Form CatchData_Form = new Window.CatchData_Form();
        /// <summary>
        /// 坐标取色事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_ToolStripMenuItem_Tool_CatchData_Click(object sender, EventArgs e)
        {
            if (CatchData_Form.IsDisposed)
                CatchData_Form = new Window.CatchData_Form();
            CatchData_Form.Show();
        }
        /// <summary>
        /// 激活软件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameHome_Permit_button_Click(object sender, EventArgs e)
        {
            if (GlobalObject.ProgramPermit.Security(this.Main_JianshuMode_radioButton.Checked ? 1 : 2))
            {
                MessageBox.Show($"激活成功！");/*当前有效期至{GlobalObject.ProgramPermit.Expiration.ToShortDateString()}*/
            }
        }

        /// <summary>
        /// 挂起线程
        /// </summary>
        public void Suspend()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(Suspend));
            }
            else
            {
                autoResetEvent.WaitOne();
            }
        }
        /// <summary>
        /// 恢复线程
        /// </summary>
        public void Resume()
        {
            autoResetEvent.Set();
        }





        public Main_Form()
        {
            InitializeComponent();
        }



        /*
        private void Button3_Click(object sender, EventArgs e)
        {
            var text = textBox1.Text;
            var temp = text.Split('|');
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = temp[i].Substring(4, 2) + temp[i].Substring(2, 2) + temp[i].Substring(0, 2);
            }
            text = "";
            for (int i = 0; i < temp.Length; i++)
            {
                text += temp[i] + "|";
            }
            text = text.Substring(0, text.Length - 1);
            textBox2.Text = text;
            Clipboard.Clear();
            Clipboard.SetText(text);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            var x = Convert.ToInt32(textBox3.Text) - 4;
            var y = Convert.ToInt32(textBox4.Text) - 27;
            var xy = new Point(x, y);
            xy.Offset(GlobalObject.GameHandle.XY);
            FunctionClick.ClickInHandle(GlobalObject.GameHandle, xy, GlobalObject.GameHandle.XY);

            void program()
            {
                while (true)
                {
                    Functions.Delay(500);
                    if (FunctionJudge.Judge(new Data.DataJudge("398,128 @ 72706C|1B1915|191713|AFADA9", "罗盘娘")))
                    { FunctionClick.Click(new Data.DataClick("564,181|712,376")); continue; }
                    Functions.Delay(500);
                    if (FunctionJudge.Judge(new Data.DataJudge("756,437 @ 36A7AA|D3E3E3|72B9BA", "掉落"), 100))
                    { FunctionClick.Click(new Data.DataClick("564,181|712,376")); continue; }
                    Functions.Delay(500);
                    if (FunctionJudge.Judge(new Data.DataJudge("85,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "出击成果1")))
                    { FunctionClick.Click(new Data.DataClick("564,181|712,376")); continue; }
                    Functions.Delay(500);
                    if (FunctionJudge.Judge(new Data.DataJudge("49,78 @ 24292C|FFF6F2|FFF6F2|FFF6F2|24292C", "出击成果2")))
                    { FunctionClick.Click(new Data.DataClick("564,181|712,376")); continue; }
                }
            }
            //var Thread = new System.Threading.Thread(program);
            //Thread.Start();
        }
        //*/
    }
}