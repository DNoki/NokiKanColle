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
    class Main_Form_GameExpedition
    {
    }
    public partial class Main_Form : Form
    {
        private void GameExpedition_Initialization()
        {
            this.GameExpedition_ExpeditionNumber2_comboBox.SelectedIndex = 1;
            this.GameExpedition_ExpeditionNumber3_comboBox.SelectedIndex = 2;
            this.GameExpedition_ExpeditionNumber4_comboBox.SelectedIndex = 4;
        }
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="strFilePath"></param>
        private void GameExpedition_ReadPlacement(string strFilePath)
        {
            this.GameExpedition_ExpeditionNumber2_comboBox.SelectedIndex = -1;
            this.GameExpedition_ExpeditionNumber3_comboBox.SelectedIndex = -1;
            this.GameExpedition_ExpeditionNumber4_comboBox.SelectedIndex = -1;
            this.GameExpedition_ExpeditionNumber2_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni("远征海域", "远征队伍2", 2.ToString(), strFilePath)) - 1;
            this.GameExpedition_ExpeditionNumber3_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni("远征海域", "远征队伍3", 5.ToString(), strFilePath)) - 1;
            this.GameExpedition_ExpeditionNumber4_comboBox.SelectedIndex = Convert.ToInt32(OperINI.ReadIni("远征海域", "远征队伍4", 6.ToString(), strFilePath)) - 1;

            // 远征设置
            ExpeditionSetting.IsSingleDepart = bool.Parse(OperINI.ReadIni("远征海域", "远征单发", false.ToString(), strFilePath));
            ExpeditionSetting.SingleDepartWaitTime = int.Parse(OperINI.ReadIni("远征海域", "远征单发等待间隔", 0.ToString(), strFilePath));
            ExpeditionSetting.ExpeditionWaitTime = int.Parse(OperINI.ReadIni("远征海域", "远征归来等待时间", 0.ToString(), strFilePath));
            ExpeditionSetting.ExpeditionRanWaitTime = int.Parse(OperINI.ReadIni("远征海域", "远征归来等待随机时间", 0.ToString(), strFilePath));
        }
        /// <summary>
        /// 写入配置
        /// </summary>
        /// <param name="strFilePath"></param>
        private void GameExpedition_WritePlacement(string strFilePath)
        {
            OperINI.WriteIni("远征海域", "远征队伍2", (this.GameExpedition_ExpeditionNumber2_comboBox.SelectedIndex + 1).ToString(), strFilePath);
            OperINI.WriteIni("远征海域", "远征队伍3", (this.GameExpedition_ExpeditionNumber3_comboBox.SelectedIndex + 1).ToString(), strFilePath);
            OperINI.WriteIni("远征海域", "远征队伍4", (this.GameExpedition_ExpeditionNumber4_comboBox.SelectedIndex + 1).ToString(), strFilePath);
            // 远征设置
            OperINI.WriteIni("远征海域", "远征单发", (ExpeditionSetting.IsSingleDepart).ToString(), strFilePath);
            OperINI.WriteIni("远征海域", "远征单发等待间隔", ExpeditionSetting.SingleDepartWaitTime.ToString(), strFilePath);
            OperINI.WriteIni("远征海域", "远征归来等待时间", ExpeditionSetting.ExpeditionWaitTime.ToString(), strFilePath);
            OperINI.WriteIni("远征海域", "远征归来等待随机时间", ExpeditionSetting.ExpeditionRanWaitTime.ToString(), strFilePath);
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameExpedition_Reset_button_Click(object sender, EventArgs e)
        {
            if (!GameExpedition_Team2Checked_checkBox.Checked)
            {
                GameExpedition_Timer2Hour_textBox.Text = "0";
                GameExpedition_Timer2Minute_textBox.Text = "0";
                GameExpedition_Timer2Second_textBox.Text = "0";
            }
            if (!GameExpedition_Team3Checked_checkBox.Checked)
            {
                GameExpedition_Timer3Hour_textBox.Text = "0";
                GameExpedition_Timer3Minute_textBox.Text = "0";
                GameExpedition_Timer3Second_textBox.Text = "0";
            }
            if (!GameExpedition_Team4Checked_checkBox.Checked)
            {
                GameExpedition_Timer4Hour_textBox.Text = "0";
                GameExpedition_Timer4Minute_textBox.Text = "0";
                GameExpedition_Timer4Second_textBox.Text = "0";
            }
        }
        // 远征计时器（设定为只允许输入数字）
        private void GameExpedition_Timer2Hour_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpedition_Timer2Hour_textBox.Text = Regex.Replace(GameExpedition_Timer2Hour_textBox.Text, @"[^\d]*", "");
            if (GameExpedition_Timer2Hour_textBox.Text == "")
            { GameExpedition_Timer2Hour_textBox.Text = "0"; }
        }
        private void GameExpedition_Timer2Minute_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpedition_Timer2Minute_textBox.Text = Regex.Replace(GameExpedition_Timer2Minute_textBox.Text, @"[^\d]*", "");
            if (GameExpedition_Timer2Minute_textBox.Text == "")
            { GameExpedition_Timer2Minute_textBox.Text = "0"; }
        }
        private void GameExpedition_Timer2Second_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpedition_Timer2Second_textBox.Text = Regex.Replace(GameExpedition_Timer2Second_textBox.Text, @"[^\d]*", "");
            if (GameExpedition_Timer2Second_textBox.Text == "")
            { GameExpedition_Timer2Second_textBox.Text = "0"; }
        }
        private void GameExpedition_Timer3Hour_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpedition_Timer3Hour_textBox.Text = Regex.Replace(GameExpedition_Timer3Hour_textBox.Text, @"[^\d]*", "");
            if (GameExpedition_Timer3Hour_textBox.Text == "")
            { GameExpedition_Timer3Hour_textBox.Text = "0"; }
        }
        private void GameExpedition_Timer3Minute_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpedition_Timer3Minute_textBox.Text = Regex.Replace(GameExpedition_Timer3Minute_textBox.Text, @"[^\d]*", "");
            if (GameExpedition_Timer3Minute_textBox.Text == "")
            { GameExpedition_Timer3Minute_textBox.Text = "0"; }
        }
        private void GameExpedition_Timer3Second_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpedition_Timer3Second_textBox.Text = Regex.Replace(GameExpedition_Timer3Second_textBox.Text, @"[^\d]*", "");
            if (GameExpedition_Timer3Second_textBox.Text == "")
            { GameExpedition_Timer3Second_textBox.Text = "0"; }
        }
        private void GameExpedition_Timer4Hour_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpedition_Timer4Hour_textBox.Text = Regex.Replace(GameExpedition_Timer4Hour_textBox.Text, @"[^\d]*", "");
            if (GameExpedition_Timer4Hour_textBox.Text == "")
            { GameExpedition_Timer4Hour_textBox.Text = "0"; }
        }
        private void GameExpedition_Timer4Minute_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpedition_Timer4Minute_textBox.Text = Regex.Replace(GameExpedition_Timer4Minute_textBox.Text, @"[^\d]*", "");
            if (GameExpedition_Timer4Minute_textBox.Text == "")
            { GameExpedition_Timer4Minute_textBox.Text = "0"; }
        }
        private void GameExpedition_Timer4Second_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpedition_Timer4Second_textBox.Text = Regex.Replace(GameExpedition_Timer4Second_textBox.Text, @"[^\d]*", "");
            if (GameExpedition_Timer4Second_textBox.Text == "")
            { GameExpedition_Timer4Second_textBox.Text = "0"; }
        }

        // 队伍勾选框
        /// <summary>
        /// 如果计时器全部关闭则停止远征线程
        /// </summary>
        private void IfNoTimer()
        {
            // 如果计时器全部关闭则停止远征线程
            if (!GameExpedition_Team2Checked_checkBox.Checked &&
                !GameExpedition_Team3Checked_checkBox.Checked &&
                !GameExpedition_Team4Checked_checkBox.Checked)
                Function.FunctionThread.CloseThread("自动远征");
        }
        /// <summary>
        /// 是否锁定控件
        /// </summary>
        /// <param name="team"></param>
        /// <param name="locked"></param>
        private void LockControls(int team, bool locked)
        {
            switch (team)
            {
                case 2:
                    if (locked)
                    {
                        this.GameExpedition_Timer2Hour_textBox.ReadOnly = true;
                        this.GameExpedition_Timer2Minute_textBox.ReadOnly = true;
                        this.GameExpedition_Timer2Second_textBox.ReadOnly = true;
                        this.GameExpedition_ExpeditionNumber2_comboBox.Enabled = false;
                    }
                    else
                    {
                        this.GameExpedition_Timer2Hour_textBox.ReadOnly = false;
                        this.GameExpedition_Timer2Minute_textBox.ReadOnly = false;
                        this.GameExpedition_Timer2Second_textBox.ReadOnly = false;
                        this.GameExpedition_ExpeditionNumber2_comboBox.Enabled = true;
                    }
                    break;
                case 3:
                    if (locked)
                    {
                        this.GameExpedition_Timer3Hour_textBox.ReadOnly = true;
                        this.GameExpedition_Timer3Minute_textBox.ReadOnly = true;
                        this.GameExpedition_Timer3Second_textBox.ReadOnly = true;
                        this.GameExpedition_ExpeditionNumber3_comboBox.Enabled = false;
                    }
                    else
                    {
                        this.GameExpedition_Timer3Hour_textBox.ReadOnly = false;
                        this.GameExpedition_Timer3Minute_textBox.ReadOnly = false;
                        this.GameExpedition_Timer3Second_textBox.ReadOnly = false;
                        this.GameExpedition_ExpeditionNumber3_comboBox.Enabled = true;
                    }
                    break;
                case 4:
                    if (locked)
                    {
                        this.GameExpedition_Timer4Hour_textBox.ReadOnly = true;
                        this.GameExpedition_Timer4Minute_textBox.ReadOnly = true;
                        this.GameExpedition_Timer4Second_textBox.ReadOnly = true;
                        this.GameExpedition_ExpeditionNumber4_comboBox.Enabled = false;
                    }
                    else
                    {
                        this.GameExpedition_Timer4Hour_textBox.ReadOnly = false;
                        this.GameExpedition_Timer4Minute_textBox.ReadOnly = false;
                        this.GameExpedition_Timer4Second_textBox.ReadOnly = false;
                        this.GameExpedition_ExpeditionNumber4_comboBox.Enabled = true;
                    }
                    break;
                default: break;
            }
        }
        private void GameExpedition_Team2Checked_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (GameExpedition_Team2Checked_checkBox.Checked)
            {
                LockControls(2, true);
                new Utility.Process.ExpeditionTimer(2);
            }
            else
            {
                LockControls(2, false);
                FunctionThread.CloseThread("远征计时器_2");
                IfNoTimer();
            }
        }
        private void GameExpedition_Team3Checked_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (GameExpedition_Team3Checked_checkBox.Checked)
            {
                LockControls(3, true);
                new Utility.Process.ExpeditionTimer(3);
            }
            else
            {
                LockControls(3, false);
                FunctionThread.CloseThread("远征计时器_3");
                IfNoTimer();
            }
        }
        private void GameExpedition_Team4Checked_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (GameExpedition_Team4Checked_checkBox.Checked)
            {
                LockControls(4, true);
                new Utility.Process.ExpeditionTimer(4);
            }
            else
            {
                LockControls(4, false);
                FunctionThread.CloseThread("远征计时器_4");
                IfNoTimer();
            }
        }

        // 远征海域
        private ComboBox[] ExpeditionNumbers = null;
        private int[] ExpeditionNumbersSelect = new int[3] { 0, 0, 0 };
        private void ExpeditionNumber_SelectedIndexChanged(int number)
        {
            if (ExpeditionNumbers == null)
            {
                this.ExpeditionNumbers = new ComboBox[3]
                {
                    this.GameExpedition_ExpeditionNumber2_comboBox,
                    this.GameExpedition_ExpeditionNumber3_comboBox,
                    this.GameExpedition_ExpeditionNumber4_comboBox
                };
                this.ExpeditionNumbersSelect = new int[3] {
                    this.GameExpedition_ExpeditionNumber2_comboBox.SelectedIndex,
                    this.GameExpedition_ExpeditionNumber3_comboBox.SelectedIndex,
                    this.GameExpedition_ExpeditionNumber4_comboBox.SelectedIndex
              };
            }
            var expeditionNumber = ExpeditionNumbers[number - 2];
            int temp = expeditionNumber.SelectedIndex;
            if (temp == -1) return;

            List<ComboBox> expeditionNumberList = new List<ComboBox>(this.ExpeditionNumbers);
            expeditionNumberList.Remove(expeditionNumber);

            foreach (var item in expeditionNumberList)
            {
                if (temp == item.SelectedIndex)
                {
                    if (temp > ExpeditionNumbersSelect[number - 2])
                        // 选择项目被+1
                        temp++;
                    else temp--;
                    if (temp < 0 || temp >= expeditionNumber.Items.Count)
                    {
                        temp = ExpeditionNumbersSelect[number - 2];
                    }
                    foreach (var item2 in expeditionNumberList)
                    {
                        if (temp == item2.SelectedIndex)
                        {
                            if (temp > ExpeditionNumbersSelect[number - 2]) temp++;
                            else temp--;
                            if (temp < 0 || temp >= expeditionNumber.Items.Count)
                            {
                                temp = ExpeditionNumbersSelect[number - 2];
                            }
                        }
                    }
                }
            }
            expeditionNumber.SelectedIndex = temp;
            ExpeditionNumbersSelect[number - 2] = temp;
        }
        private void GameExpedition_ExpeditionNumber2_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExpeditionNumber_SelectedIndexChanged(2);
        }
        private void GameExpedition_ExpeditionNumber3_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExpeditionNumber_SelectedIndexChanged(3);
        }
        private void GameExpedition_ExpeditionNumber4_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ExpeditionNumber_SelectedIndexChanged(4);
        }

        //private void GameExpedition_ExpeditionNumber2_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    int temp = GameExpedition_ExpeditionNumber2_comboBox.SelectedIndex;
        //    if (temp == -1) return;
        //    for (int i = 0; i < 3; i++)
        //    {
        //        if ((temp == GameExpedition_ExpeditionNumber3_comboBox.SelectedIndex) ||
        //    temp == GameExpedition_ExpeditionNumber4_comboBox.SelectedIndex)
        //            temp = i;
        //    }
        //    GameExpedition_ExpeditionNumber2_comboBox.SelectedIndex = temp;
        //}
        //private void GameExpedition_ExpeditionNumber3_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    int temp = GameExpedition_ExpeditionNumber3_comboBox.SelectedIndex;
        //    if (temp == -1) return;
        //    for (int i = 0; i < 3; i++)
        //    {
        //        if ((temp == GameExpedition_ExpeditionNumber2_comboBox.SelectedIndex) ||
        //    temp == GameExpedition_ExpeditionNumber4_comboBox.SelectedIndex)
        //            temp = i;
        //    }
        //    GameExpedition_ExpeditionNumber3_comboBox.SelectedIndex = temp;
        //}
        //private void GameExpedition_ExpeditionNumber4_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    int temp = GameExpedition_ExpeditionNumber4_comboBox.SelectedIndex;
        //    if (temp == -1) return;
        //    for (int i = 0; i < 3; i++)
        //    {
        //        if ((temp == GameExpedition_ExpeditionNumber3_comboBox.SelectedIndex) ||
        //    temp == GameExpedition_ExpeditionNumber2_comboBox.SelectedIndex)
        //            temp = i;
        //    }
        //    GameExpedition_ExpeditionNumber4_comboBox.SelectedIndex = temp;
        //}

        private void GameExpedition_StopAll_button_Click(object sender, EventArgs e)
        {
            Function.FunctionThread.CloseThread("自动远征");
            GameExpedition_Team2Checked_checkBox.Checked = false;
            GameExpedition_Team3Checked_checkBox.Checked = false;
            GameExpedition_Team4Checked_checkBox.Checked = false;
        }
        private void GameExpedition_StartAll_button_Click(object sender, EventArgs e)
        {
            GameExpedition_Team2Checked_checkBox.Checked = true;
            GameExpedition_Team3Checked_checkBox.Checked = true;
            GameExpedition_Team4Checked_checkBox.Checked = true;
        }

        /// <summary>
        /// 是否单舰派遣
        /// </summary>
        public bool GetExpeditionIsSingleDepart => ExpeditionSetting.IsSingleDepart;
        /// <summary>
        /// 单舰派遣间隔时间
        /// </summary>
        public TimeSpan GetExpeditionSingleDepartWaitTime => TimeSpan.FromSeconds(ExpeditionSetting.SingleDepartWaitTime);
        /// <summary>
        /// 获取远征收取等待时间（固定+随机）
        /// </summary>
        public TimeSpan GetExpeditionWaitTime => TimeSpan.FromSeconds(ExpeditionSetting.ExpeditionWaitTime + Functions.Random(ExpeditionSetting.ExpeditionRanWaitTime));
        public ExpeditionSetting ExpeditionOtherSetting = new ExpeditionSetting();
        /// <summary>
        /// 远征详细设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameExpedition_Setting_button_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(GetIsSingleDepart+Environment.NewLine+
            //    GetSingleDepartWaitTime+Environment.NewLine+
            //    GetExpeditionWaitTime.ToString());
            if (ExpeditionOtherSetting.IsDisposed)
            {
                ExpeditionOtherSetting = new ExpeditionSetting();
            }
            ExpeditionOtherSetting.Location = new Point(this.Location.X + 50, this.Location.Y + 50);
            ExpeditionOtherSetting.Reset();
            ExpeditionOtherSetting.Show();
        }
    }
}
