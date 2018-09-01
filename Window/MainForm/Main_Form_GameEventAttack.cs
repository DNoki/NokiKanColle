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
    class Main_Form_GameEventAttack
    {
    }
    public partial class Main_Form : Form
    {
        /// <summary>
        /// 获取是否联合舰队
        /// </summary>
        public bool GetEventAttackIsUnion => this.GameEventAttack_IsUnion_checkBox.Checked;
        /// <summary>
        /// 获取是否补给陆基
        /// </summary>
        public bool GetEventAttackIsBaseAirCorps => this.GameEventAttack_BaseAirCorps_checkBox.Checked;
        /// <summary>
        /// 获取是否入渠
        /// </summary>
        public bool GetEventAttackIsDock => this.GameEventAttack_IsDock_checkBox.Checked;
        /// <summary>
        /// 入渠基准
        /// </summary>
        public int GetEventAttackDockBenchmark => this.GameEventAttack_DockBenchmark_comboBox.SelectedIndex;
        /// <summary>
        /// 撤退条件
        /// </summary>
        public int GetEventAttackDetectionStatus => this.GameEventAttack_DetectionStatus_comboBox.SelectedIndex;

        /// <summary>
        /// 设置EventAttack状态栏
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontColor"></param>
        /// <param name="backColor"></param>
        /// <param name="bold"></param>
        public void SetEventAttackStatus(string text, Color fontColor, Color backColor, bool bold = true)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string, Color, Color, bool>(SetEventAttackStatus), text, fontColor, backColor, bold);
            }
            else
            {
                this.GameEventAttack_Status_textBox.Text = text;
                this.GameEventAttack_Status_textBox.ForeColor = fontColor;
                this.GameEventAttack_Status_textBox.BackColor = backColor;
                if (bold)
                    this.GameEventAttack_Status_textBox.Font = new Font(this.GameEventAttack_Status_textBox.Font, FontStyle.Bold);
                else
                    this.GameEventAttack_Status_textBox.Font = new Font(this.GameEventAttack_Status_textBox.Font, FontStyle.Regular);
            }
        }
        

        private void GameEventAttack_Initialization()
        {

        }
        private void GameEventAttack_ReadPlacement(string strFilePath)
        {

        }
        private void GameEventAttack_WritePlacement(string strFilePath)
        {

        }

        /// <summary>
        /// 保存配置页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameEventAttack_SavePlacement_button_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 读取配置页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameEventAttack_LoadPlacement_button_Click(object sender, EventArgs e)
        {

        }

        private void GameEventAttack_Start_button_Click(object sender, EventArgs e)
        {
            new Utility.Process.EventAttack();
        }
        private void GameEventAttack_Stop_button_Click(object sender, EventArgs e)
        {
            FunctionThread.CloseThread(nameof(Utility.Process.EventAttack));
        }
    }
}
