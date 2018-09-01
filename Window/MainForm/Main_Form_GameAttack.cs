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
    class Main_Form_GameAttack
    {
    }
    public partial class Main_Form : Form
    {
        private void GameAttack_Initialization()
        {
            GameAttack_Seas_combobox.SelectedIndex = 0;
            GameAttack_Reset_combobox_Click(null, null);
            GameAttack_DockBenchmark_comboBox.SelectedIndex = 1;
            GameAttack_WaitTime_textBox.Text = 12.ToString();
            GameAttack_WaitTimeRan_textBox.Text = 120.ToString();
        }
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="strFilePath"></param>
        private void GameAttack_ReadPlacement(string strFilePath)
        {
            switch (OperINI.ReadIni("出击", "选定的出击配置", 1.ToString(), strFilePath))
            {
                case "1":
                    this.GameAttack_Placement1_radioButton.Checked = true;
                    break;
                case "2":
                    this.GameAttack_Placement2_radioButton.Checked = true;
                    break;
                case "3":
                    this.GameAttack_Placement3_radioButton.Checked = true;
                    break;
                default:
                    this.GameAttack_Placement1_radioButton.Checked = true;
                    break;
            }
            GameAttack_Placement_radioButton_CheckedChanged(null, null);

            // 出击设置
            AttackSetting.PreWaitTime = int.Parse(OperINI.ReadIni("出击", "出击前置等待时间", 0.ToString(), strFilePath));
        }
        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="strFilePath"></param>
        private void GameAttack_WritePlacement(string strFilePath)
        {
            var temp = "1";
            if (this.GameAttack_Placement1_radioButton.Checked)
                temp = "1";
            if (this.GameAttack_Placement2_radioButton.Checked)
                temp = "2";
            if (this.GameAttack_Placement3_radioButton.Checked)
                temp = "3";
            OperINI.WriteIni("出击", "选定的出击配置", temp, strFilePath);
            //GameAttack_SavePlacement_combobox_Click(null, null);
            // 出击设置
            OperINI.WriteIni("出击", "出击前置等待时间", AttackSetting.PreWaitTime.ToString(), strFilePath);
        }

        /// <summary>
        /// 保存当前配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttack_SavePlacement_combobox_Click(object sender, EventArgs e)
        {
            if (GameAttack_Placement1_radioButton.Checked)
            {
                WritePlacement(1);
            }
            else if (GameAttack_Placement2_radioButton.Checked)
            {
                WritePlacement(2);
            }
            else if (GameAttack_Placement3_radioButton.Checked)
            {
                WritePlacement(3);
            }
            else { }
        }
        /// <summary>
        /// 切换配置页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttack_Placement_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (GameAttack_Placement1_radioButton.Checked)
            {
                ReadPlacement(1);
            }
            else if (GameAttack_Placement2_radioButton.Checked)
            {
                ReadPlacement(2);
            }
            else if (GameAttack_Placement3_radioButton.Checked)
            {
                ReadPlacement(3);
            }
            else { }
        }
        /// <summary>
        /// 存储当前配置页到文件
        /// </summary>
        /// <param name="num"></param>
        private void WritePlacement(int num)
        {
            string strFilePath = Application.StartupPath + @"\配置文件.ini";
            OperINI.WriteIni($"出击配置{num}", "出击海域", (this.GameAttack_Seas_combobox.SelectedIndex + 1).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "出击地图", (this.GameAttack_Map_combobox.SelectedIndex + 1).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "战斗发生判定1", (this.GameAttack_Battle1_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "战斗1阵型", (this.GameAttack_Formation1_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "战斗发生判定2", (this.GameAttack_Battle2_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "战斗2阵型", (this.GameAttack_Formation2_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "战斗发生判定3", (this.GameAttack_Battle3_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "战斗3阵型", (this.GameAttack_Formation3_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "战斗发生判定4", (this.GameAttack_Battle4_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "战斗4阵型", (this.GameAttack_Formation4_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "战斗发生判定5", (this.GameAttack_Battle5_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "战斗5阵型", (this.GameAttack_Formation5_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "是否入渠", (this.GameAttack_IsDock_checkBox.Checked).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "撤退条件", (this.GameAttack_DetectionStatus_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "入渠基准", (this.GameAttack_DockBenchmark_comboBox.SelectedIndex).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "出击次数", (this.GameAttack_Frequency_numericUpDown.Value).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "等待固定时间", (this.GameAttack_WaitTime_textBox.Text).ToString(), strFilePath);
            OperINI.WriteIni($"出击配置{num}", "等待随机时间", (this.GameAttack_WaitTimeRan_textBox.Text).ToString(), strFilePath);
        }
        /// <summary>
        /// 读取文件中的配置
        /// </summary>
        /// <param name="num"></param>
        private void ReadPlacement(int num)
        {
            string strFilePath = Application.StartupPath + @"\配置文件.ini";
            this.GameAttack_Seas_combobox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "出击海域", 1.ToString(), strFilePath)) - 1;
            this.GameAttack_Map_combobox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "出击地图", 1.ToString(), strFilePath)) - 1;
            this.GameAttack_Battle1_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "战斗发生判定1", 0.ToString(), strFilePath));
            this.GameAttack_Formation1_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "战斗1阵型", 0.ToString(), strFilePath));
            this.GameAttack_Battle2_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "战斗发生判定2", 0.ToString(), strFilePath));
            this.GameAttack_Formation2_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "战斗2阵型", 0.ToString(), strFilePath));
            this.GameAttack_Battle3_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "战斗发生判定3", 0.ToString(), strFilePath));
            this.GameAttack_Formation3_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "战斗3阵型", 0.ToString(), strFilePath));
            this.GameAttack_Battle4_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "战斗发生判定4", 0.ToString(), strFilePath));
            this.GameAttack_Formation4_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "战斗4阵型", 0.ToString(), strFilePath));
            this.GameAttack_Battle5_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "战斗发生判定5", 0.ToString(), strFilePath));
            this.GameAttack_Formation5_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "战斗5阵型", 0.ToString(), strFilePath));
            this.GameAttack_IsDock_checkBox.Checked = Convert.ToBoolean(OperINI.ReadIni($"出击配置{num}", "是否入渠", true.ToString(), strFilePath));
            this.GameAttack_DetectionStatus_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "撤退条件", 2.ToString(), strFilePath));
            this.GameAttack_DockBenchmark_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "入渠基准", 1.ToString(), strFilePath));
            this.GameAttack_Frequency_numericUpDown.Value = Convert.ToInt32(OperINI.ReadIni($"出击配置{num}", "出击次数", 5.ToString(), strFilePath));
            this.GameAttack_WaitTime_textBox.Text = OperINI.ReadIni($"出击配置{num}", "等待固定时间", 12.ToString(), strFilePath);
            this.GameAttack_WaitTimeRan_textBox.Text = OperINI.ReadIni($"出击配置{num}", "等待随机时间", 120.ToString(), strFilePath);
        }

        /// <summary>
        /// 设置EventAttack状态栏
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontColor"></param>
        /// <param name="backColor"></param>
        /// <param name="bold"></param>
        public void SetAttackStatus(string text, Color fontColor, Color backColor, bool bold = true)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string, Color, Color, bool>(SetAttackStatus), text, fontColor, backColor, bold);
            }
            else
            {
                this.GameAttack_Status_textBox.Text = text;
                this.GameAttack_Status_textBox.ForeColor = fontColor;
                this.GameAttack_Status_textBox.BackColor = backColor;
                if (bold)
                    this.GameAttack_Status_textBox.Font = new Font(this.GameAttack_Status_textBox.Font, FontStyle.Bold);
                else
                    this.GameAttack_Status_textBox.Font = new Font(this.GameAttack_Status_textBox.Font, FontStyle.Regular);
            }
        }


        /// <summary>
        /// 更改远征海域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttack_Seas_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameAttack_Map_combobox.Items.Clear();
            switch (GameAttack_Seas_combobox.SelectedIndex)
            {
                case 0:
                    GameAttack_Map_combobox.Items.AddRange(new string[] { "1. 鎮守府正面海域", "2. 南西諸島沖", "3. 製油所地帯沿岸", "4. 南西諸島防衛線", "5. 鎮守府近海対潜哨戒", "6. 鎮守府近海航路" });
                    break;
                case 1:
                    GameAttack_Map_combobox.Items.AddRange(new string[] { "1. カムラン半島", "2. バシー島沖", "3. 東部オリョール海", "4. 沖ノ島海域", "5. 沖ノ島沖戦闘哨戒" });
                    break;
                case 2:
                    GameAttack_Map_combobox.Items.AddRange(new string[] { "1. モーレイ海哨戒", "2. キス島撤退作戦", "3. アルフォンシーノ方面進出", "4. 北方海域艦隊決戦", "5. 北方海域戦闘哨戒" });
                    break;
                case 3:
                    GameAttack_Map_combobox.Items.AddRange(new string[] { "1. ジャム島攻略作戦", "2. カレー洋制圧戦", "3. リランカ島空襲", "4. カスガダマ沖海戦", "5. 深海東洋艦隊漸減作戦" });
                    break;
                case 4:
                    GameAttack_Map_combobox.Items.AddRange(new string[] { "1. 南方海域進出作戦", "2. 珊瑚諸島沖海戦", "3. 第一次サーモン沖海戦", "4. 東京急行", "5. 第二次サーモン海戦" });
                    break;
                case 5:
                    GameAttack_Map_combobox.Items.AddRange(new string[] { "1. 潜水艦作戦", "2. ＭＳ諸島防衛戦", "3. Ｋ作戦", "4. 離島再攻略作戦", "5. KW環礁沖海域" });
                    break;
                default: break;
            }
            GameAttack_Map_combobox.SelectedIndex = 0;
        }

        /// <summary>
        /// 关卡2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttack_Battle2_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameAttack_Battle2_comboBox.SelectedIndex == 2)
            {
                GameAttack_Formation2_comboBox.Enabled = false;
                GameAttack_Battle3_comboBox.SelectedIndex = 2;
                GameAttack_Battle3_comboBox.Enabled = false;
            }
            else
            {
                GameAttack_Formation2_comboBox.Enabled = true;
                GameAttack_Battle3_comboBox.Enabled = true;
            }
        }
        private void GameAttack_Battle3_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameAttack_Battle3_comboBox.SelectedIndex == 2)
            {
                GameAttack_Formation3_comboBox.Enabled = false;
                GameAttack_Battle4_comboBox.SelectedIndex = 2;
                GameAttack_Battle4_comboBox.Enabled = false;
            }
            else
            {
                GameAttack_Formation3_comboBox.Enabled = true;
                GameAttack_Battle4_comboBox.Enabled = true;
            }
        }
        private void GameAttack_Battle4_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameAttack_Battle4_comboBox.SelectedIndex == 2)
            {
                GameAttack_Formation4_comboBox.Enabled = false;
                GameAttack_Battle5_comboBox.SelectedIndex = 2;
                GameAttack_Battle5_comboBox.Enabled = false;
            }
            else
            {
                GameAttack_Formation4_comboBox.Enabled = true;
                GameAttack_Battle5_comboBox.Enabled = true;
            }
        }
        private void GameAttack_Battle5_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GameAttack_Battle5_comboBox.SelectedIndex == 2)
            {
                GameAttack_Formation5_comboBox.Enabled = false;
            }
            else
            {
                GameAttack_Formation5_comboBox.Enabled = true;
            }
        }

        /// <summary>
        /// 出击次数，数字输入限定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttack_Frequency_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GameAttack_Frequency_numericUpDown.Value = Convert.ToInt32(this.GameAttack_Frequency_numericUpDown.Value);
        }
        /// <summary>
        /// 等待时间，数字输入限定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttack_WaitTime_textBox_TextChanged(object sender, EventArgs e)
        {
            this.GameAttack_WaitTime_textBox.Text = Regex.Replace(this.GameAttack_WaitTime_textBox.Text, @"[^\d]*", "");
            if (GameAttack_WaitTime_textBox.Text == "")
                GameAttack_WaitTime_textBox.Text = "0";
            this.GameAttack_WaitTimeRan_textBox.Text = Regex.Replace(this.GameAttack_WaitTimeRan_textBox.Text, @"[^\d]*", "");
            if (GameAttack_WaitTimeRan_textBox.Text == "")
                GameAttack_WaitTimeRan_textBox.Text = "0";
        }

        /// <summary>
        /// 重置出击阵型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttack_Reset_combobox_Click(object sender, EventArgs e)
        {
            GameAttack_Battle1_comboBox.SelectedIndex = 0;
            GameAttack_Formation1_comboBox.SelectedIndex = 0;
            GameAttack_Battle2_comboBox.SelectedIndex = 0;
            GameAttack_Formation2_comboBox.SelectedIndex = 0;
            GameAttack_Battle3_comboBox.SelectedIndex = 0;
            GameAttack_Formation3_comboBox.SelectedIndex = 0;
            GameAttack_Battle4_comboBox.SelectedIndex = 0;
            GameAttack_Formation4_comboBox.SelectedIndex = 0;
            GameAttack_Battle5_comboBox.SelectedIndex = 0;
            GameAttack_Formation5_comboBox.SelectedIndex = 0;
            GameAttack_IsDock_checkBox.Checked = true;
            GameAttack_DetectionStatus_comboBox.SelectedIndex = 2;
            GameAttack_Frequency_numericUpDown.Value = 5;
        }

        /// <summary>
        /// 出击前置等待时间
        /// </summary>
        public int GetAttackPreWaitTime => AttackSetting.PreWaitTime;
        public AttackSetting AttackSettingWindow = new AttackSetting();
        /// <summary>
        /// 打开设置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttack_Setting_button_Click(object sender, EventArgs e)
        {
            if (AttackSettingWindow.IsDisposed)
                AttackSettingWindow = new AttackSetting();
            AttackSettingWindow.Location = new Point(this.Location.X + 50, this.Location.Y + 50);
            AttackSettingWindow.Reset();
            AttackSettingWindow.Show();
        }

        /// <summary>
        /// 开始出击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttack_Start_button_Click(object sender, EventArgs e)
        {
            new Utility.Process.Attack();
        }
        /// <summary>
        /// 结束出击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttack_Stop_button_Click(object sender, EventArgs e)
        {
            FunctionThread.CloseThread("自动出击");
        }
    }
}