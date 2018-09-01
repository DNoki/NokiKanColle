using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace NokiKanColle.Window
{
    public class AttackSetting : Form
    {
        public AttackSetting()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
        }

        private Label GameAttackSetting_PreWaitTime_label;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GameAttackSetting_PreWaitTime_label = new System.Windows.Forms.Label();
            this.GameAttackSetting_PreWaitTime_textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // GameAttackSetting_PreWaitTime_label
            // 
            this.GameAttackSetting_PreWaitTime_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameAttackSetting_PreWaitTime_label.BackColor = System.Drawing.Color.Transparent;
            this.GameAttackSetting_PreWaitTime_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameAttackSetting_PreWaitTime_label.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.GameAttackSetting_PreWaitTime_label.Location = new System.Drawing.Point(11, 5);
            this.GameAttackSetting_PreWaitTime_label.Name = "GameAttackSetting_PreWaitTime_label";
            this.GameAttackSetting_PreWaitTime_label.Size = new System.Drawing.Size(216, 22);
            this.GameAttackSetting_PreWaitTime_label.TabIndex = 82;
            this.GameAttackSetting_PreWaitTime_label.Text = "第一次出击前等待时间:                  秒";
            this.GameAttackSetting_PreWaitTime_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GameAttackSetting_PreWaitTime_label.UseMnemonic = false;
            // 
            // GameAttackSetting_PreWaitTime_textBox
            // 
            this.GameAttackSetting_PreWaitTime_textBox.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameAttackSetting_PreWaitTime_textBox.Location = new System.Drawing.Point(143, 7);
            this.GameAttackSetting_PreWaitTime_textBox.MaxLength = 6;
            this.GameAttackSetting_PreWaitTime_textBox.Name = "GameAttackSetting_PreWaitTime_textBox";
            this.GameAttackSetting_PreWaitTime_textBox.Size = new System.Drawing.Size(43, 21);
            this.GameAttackSetting_PreWaitTime_textBox.TabIndex = 80;
            this.GameAttackSetting_PreWaitTime_textBox.Text = "00000";
            this.GameAttackSetting_PreWaitTime_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.GameAttackSetting_PreWaitTime_textBox.TextChanged += new System.EventHandler(this.GameAttackSetting_PreWaitTime_textBox_TextChanged);
            // 
            // AttackSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(240, 49);
            this.Controls.Add(this.GameAttackSetting_PreWaitTime_textBox);
            this.Controls.Add(this.GameAttackSetting_PreWaitTime_label);
            this.Name = "AttackSetting";
            this.Text = "出击设置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox GameAttackSetting_PreWaitTime_textBox;

        /// <summary>
        /// 出击前置等待时间
        /// </summary>
        public static int PreWaitTime = 0;

        public void Reset()
        {
            GameAttackSetting_PreWaitTime_textBox.Text = PreWaitTime.ToString();
        }


        /// <summary>
        /// 规范输入数据并存入变量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameAttackSetting_PreWaitTime_textBox_TextChanged(object sender, EventArgs e)
        {
            GameAttackSetting_PreWaitTime_textBox.Text = Regex.Replace(GameAttackSetting_PreWaitTime_textBox.Text, @"[^\d]*", "");
            if (GameAttackSetting_PreWaitTime_textBox.Text == "")
            { GameAttackSetting_PreWaitTime_textBox.Text = "0"; }
            if (PreWaitTime != int.Parse(GameAttackSetting_PreWaitTime_textBox.Text))
                PreWaitTime = int.Parse(GameAttackSetting_PreWaitTime_textBox.Text);
        }

    }
}
