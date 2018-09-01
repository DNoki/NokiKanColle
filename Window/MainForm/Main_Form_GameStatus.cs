using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using NokiKanColle.Function;

namespace NokiKanColle.Window
{
    class Main_Form_GameStatus
    {
    }
    public partial class Main_Form : Form
    {
        /// <summary>
        /// 移除选中线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameStatus_ThreadClose_button_Click(object sender, EventArgs e)
        {
            if (GameStatus_Thread_listBox.Items.Count > 0 && GameStatus_Thread_listBox.SelectedIndex >= 0)
            {
                if (GameStatus_Thread_listBox.SelectedItem as string != "消息队列（不可关闭）")
                {
                    Function.FunctionThread.CloseThread(GameStatus_Thread_listBox.SelectedItem as string);
                }
            }
        }
        
        /// <summary>
        /// 添加一个线程项目到UI控件
        /// </summary>
        /// <param name="name"></param>
        internal void AddThreadItem(string name)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AddThreadItem), name);
            }
            else
            {
                this.GameStatus_Thread_listBox.Items.Add(name);
            }
        }
        /// <summary>
        /// 从UI控件中删除指定线程项目
        /// </summary>
        /// <param name="name"></param>
        internal void RemoveThreadItem(string name)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action<string>(RemoveThreadItem), name);
            }
            else
            {
                this.GameStatus_Thread_listBox.Items.Remove(name);
            }
        }

        /// <summary>
        /// 更改当前正在运行线程的文本
        /// </summary>
        /// <param name="name"></param>
        internal void ChangeThreadNowText(string name = "")
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(ChangeThreadNowText), name);
            }
            else
            {
                if (name=="")
                {
                    this.GameStatus_ThreadNow_label.Text = $"当前没有正在执行的线程...";
                    ChangeJudgeCoordinate("0,0", 1);
                    ChangeClickCoordinate("0,0", 1);
                }
                else
                    this.GameStatus_ThreadNow_label.Text = $"当前 {name} 正在执行...";

            }
        }
        /// <summary>
        /// 添加一条信息到状态信息栏
        /// </summary>
        /// <param name="text"></param>
        internal void AddStatusMessage(string text)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(AddStatusMessage), text);
                Function.Functions.Delay(5);
            }
            else
            {
                // 禁止输出相同句
                if (GameStatus_Massage_textBox.Text != "")
                    if (text!=" ")
                        if (GameStatus_Massage_textBox.Lines[0].Contains(text)) return;

                if (GameStatus_Massage_textBox.Lines.Length > 520)
                {
                    var temp = GameStatus_Massage_textBox.Lines;
                    var newLines = new string[temp.Length - 10];
                    Array.Copy(temp, 0, newLines, 0, newLines.Length);
                    GameStatus_Massage_textBox.Lines = newLines;
                }
                this.GameStatus_Massage_textBox.SelectionStart = 0;
                var time = DateTime.Now;
                this.GameStatus_Massage_textBox.SelectedText += $"{time.ToShortDateString()} {time.ToLongTimeString()}: {text.Replace("\n", "|")}" + Environment.NewLine;
            }
        }
        /// <summary>
        /// 更改判定坐标的显示
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mode"></param>
        internal void ChangeJudgeCoordinate(string xy,int mode)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action<string, int>(ChangeJudgeCoordinate), xy, mode);
                Function.Functions.Delay(2);
            }
            else
            {
                switch (mode)
                {
                    case 1:
                        // 停止
                        this.GameStatus_JudgeCoordinate_textBox.ForeColor = Color.Black;
                        this.GameStatus_JudgeCoordinate_textBox.BackColor = Color.White;
                        this.GameStatus_JudgeCoordinate_textBox.Font = new Font(this.GameStatus_JudgeCoordinate_textBox.Font, FontStyle.Regular);
                        break;
                    case 2:
                        // 工作
                        this.GameStatus_JudgeCoordinate_textBox.ForeColor = Color.White;
                        this.GameStatus_JudgeCoordinate_textBox.BackColor = Color.Orange;
                        this.GameStatus_JudgeCoordinate_textBox.Font = new Font(this.GameStatus_JudgeCoordinate_textBox.Font, FontStyle.Bold);
                        break;
                    default:
                        break;
                }
                this.GameStatus_JudgeCoordinate_textBox.Text = xy;
            }
        }
        /// <summary>
        /// 更改选择坐标的显示
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="mode"></param>
        internal void ChangeClickCoordinate(string xy, int mode)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke(new Action<string, int>(ChangeClickCoordinate), xy, mode);
                Function.Functions.Delay(2);
            }
            else
            {
                switch (mode)
                {
                    case 1:
                        // 停止
                        this.GameStatus_ClickCoordinate_textBox.ForeColor = Color.Black;
                        this.GameStatus_ClickCoordinate_textBox.BackColor = Color.White;
                        this.GameStatus_ClickCoordinate_textBox.Font = new Font(this.GameStatus_ClickCoordinate_textBox.Font, FontStyle.Regular);
                        break;
                    case 2:
                        // 工作
                        this.GameStatus_ClickCoordinate_textBox.ForeColor = Color.White;
                        this.GameStatus_ClickCoordinate_textBox.BackColor = Color.Orange;
                        this.GameStatus_ClickCoordinate_textBox.Font = new Font(this.GameStatus_ClickCoordinate_textBox.Font, FontStyle.Bold);
                        break;
                    default:
                        break;
                }
                this.GameStatus_ClickCoordinate_textBox.Text = xy;
            }
        }
    }


}
