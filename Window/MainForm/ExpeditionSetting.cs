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

using NokiKanColle.Function;

namespace NokiKanColle.Window
{
    public class ExpeditionSetting : Form
    {
        public ExpeditionSetting()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
        }

        /// <summary>
        /// 是否单舰派遣
        /// </summary>
        public static bool IsSingleDepart { get; set; } = false;
        /// <summary>
        /// 单舰派遣间隔时间
        /// </summary>
        public static int SingleDepartWaitTime { get; set; } = 0;
        /// <summary>
        /// 获取远征收取等待时间
        /// </summary>
        public static int ExpeditionWaitTime { get; set; } = 0;
        /// <summary>
        /// 获取远征收取等待随机时间
        /// </summary>
        public static int ExpeditionRanWaitTime { get; set; } = 0;

        /// <summary>
        /// 是否开启远征次数限制
        /// </summary>
        public bool IsFrequency
        {
            get => this.GameExpeditionSetting_IsFrequency_checkBox.Checked;
            set => this.GameExpeditionSetting_IsFrequency_checkBox.Checked = value;
        }
        public int GetFrequency(int team)
        {
            if (!(2 <= team && team <= 4)) throw new Exceptions.DataErrorException();
            return Convert.ToInt32((new NumericUpDown[3] { GameExpeditionSetting_Frequency2_numericUpDown, GameExpeditionSetting_Frequency3_numericUpDown, GameExpeditionSetting_Frequency4_numericUpDown })[team - 2].Value);
        }
        /// <summary>
        /// 是否开启远征结束时间
        /// </summary>
        public bool IsOverTime
        {
            get => this.GameExpeditionSetting_IsOverTime_checkBox.Checked;
            set => this.GameExpeditionSetting_IsOverTime_checkBox.Checked = value;
        }
        public DateTime GetOverTime(int team)
        {
            //Convert.ToDateTime("yyyy-MM-dd hh:mm:ss");
            if (!(2 <= team && team <= 4)) throw new Exceptions.DataErrorException();
            var overtime = new TextBox[3]
            {
                this.GameExpeditionSetting_OverTime2_textBox,
                this.GameExpeditionSetting_OverTime3_textBox,
                this.GameExpeditionSetting_OverTime4_textBox
            }[team - 2];
            DateTime time = DateTime.MaxValue;
            try
            {
                overtime.BackColor = Color.White;
                time = Convert.ToDateTime(overtime.Text);
            }
            catch (FormatException)
            {
                overtime.Text = "输入的格式不正确：yyyy-MM-dd hh:mm:ss";
                overtime.BackColor = Color.Red;
            }
            return time;
        }

        private Label GameExpeditionSetting_ExpeditionWaitTime_label;
        private Label GameExpeditionSetting_SingleDepartWaitTime_label;
        internal TextBox GameExpeditionSetting_ExpeditionWaitTime_textBox;
        internal TextBox GameExpeditionSetting_ExpeditionRanWaitTime_textBox;
        internal CheckBox GameExpeditionSetting_SingleDepart_checkBox;
        internal TextBox GameExpeditionSetting_SingleDepartWaitTime_textBox;
        internal CheckBox GameExpeditionSetting_IsFrequency_checkBox;
        private Label GameExpeditionSetting_IsFrequency_label;
        internal NumericUpDown GameExpeditionSetting_Frequency2_numericUpDown;
        private Label GameExpeditionSetting_Team2_label;
        private Label GameExpeditionSetting_Team3_label;
        private Label GameExpeditionSetting_Team4_label;
        internal NumericUpDown GameExpeditionSetting_Frequency3_numericUpDown;
        internal NumericUpDown GameExpeditionSetting_Frequency4_numericUpDown;
        private Label GameExpeditionSetting_Frequency_label;
        private Label GameExpeditionSetting_OverTime_label;
        private TextBox GameExpeditionSetting_OverTime2_textBox;
        private TextBox GameExpeditionSetting_OverTime3_textBox;
        internal CheckBox GameExpeditionSetting_IsOverTime_checkBox;
        private Label GameExpeditionSetting_IsOverTime_label;
        private TextBox GameExpeditionSetting_OverTime4_textBox;
        private GroupBox GameExpeditionSetting_LimitSetting_textBox;


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
            this.GameExpeditionSetting_ExpeditionWaitTime_label = new System.Windows.Forms.Label();
            this.GameExpeditionSetting_SingleDepartWaitTime_label = new System.Windows.Forms.Label();
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox = new System.Windows.Forms.TextBox();
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox = new System.Windows.Forms.TextBox();
            this.GameExpeditionSetting_SingleDepart_checkBox = new System.Windows.Forms.CheckBox();
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox = new System.Windows.Forms.TextBox();
            this.GameExpeditionSetting_IsFrequency_checkBox = new System.Windows.Forms.CheckBox();
            this.GameExpeditionSetting_IsFrequency_label = new System.Windows.Forms.Label();
            this.GameExpeditionSetting_Frequency2_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.GameExpeditionSetting_Team2_label = new System.Windows.Forms.Label();
            this.GameExpeditionSetting_Team3_label = new System.Windows.Forms.Label();
            this.GameExpeditionSetting_Team4_label = new System.Windows.Forms.Label();
            this.GameExpeditionSetting_Frequency3_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.GameExpeditionSetting_Frequency4_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.GameExpeditionSetting_Frequency_label = new System.Windows.Forms.Label();
            this.GameExpeditionSetting_OverTime_label = new System.Windows.Forms.Label();
            this.GameExpeditionSetting_OverTime2_textBox = new System.Windows.Forms.TextBox();
            this.GameExpeditionSetting_OverTime3_textBox = new System.Windows.Forms.TextBox();
            this.GameExpeditionSetting_IsOverTime_checkBox = new System.Windows.Forms.CheckBox();
            this.GameExpeditionSetting_IsOverTime_label = new System.Windows.Forms.Label();
            this.GameExpeditionSetting_OverTime4_textBox = new System.Windows.Forms.TextBox();
            this.GameExpeditionSetting_LimitSetting_textBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.GameExpeditionSetting_Frequency2_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GameExpeditionSetting_Frequency3_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GameExpeditionSetting_Frequency4_numericUpDown)).BeginInit();
            this.GameExpeditionSetting_LimitSetting_textBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // GameExpeditionSetting_ExpeditionWaitTime_label
            // 
            this.GameExpeditionSetting_ExpeditionWaitTime_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameExpeditionSetting_ExpeditionWaitTime_label.BackColor = System.Drawing.Color.Transparent;
            this.GameExpeditionSetting_ExpeditionWaitTime_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_ExpeditionWaitTime_label.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.GameExpeditionSetting_ExpeditionWaitTime_label.Location = new System.Drawing.Point(15, 48);
            this.GameExpeditionSetting_ExpeditionWaitTime_label.Name = "GameExpeditionSetting_ExpeditionWaitTime_label";
            this.GameExpeditionSetting_ExpeditionWaitTime_label.Size = new System.Drawing.Size(346, 24);
            this.GameExpeditionSetting_ExpeditionWaitTime_label.TabIndex = 79;
            this.GameExpeditionSetting_ExpeditionWaitTime_label.Text = "远征队伍抵达母港后等待时间:                  秒 + 随机                  秒 ";
            this.GameExpeditionSetting_ExpeditionWaitTime_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GameExpeditionSetting_ExpeditionWaitTime_label.UseMnemonic = false;
            // 
            // GameExpeditionSetting_SingleDepartWaitTime_label
            // 
            this.GameExpeditionSetting_SingleDepartWaitTime_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameExpeditionSetting_SingleDepartWaitTime_label.BackColor = System.Drawing.Color.Transparent;
            this.GameExpeditionSetting_SingleDepartWaitTime_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_SingleDepartWaitTime_label.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.GameExpeditionSetting_SingleDepartWaitTime_label.Location = new System.Drawing.Point(35, 12);
            this.GameExpeditionSetting_SingleDepartWaitTime_label.Name = "GameExpeditionSetting_SingleDepartWaitTime_label";
            this.GameExpeditionSetting_SingleDepartWaitTime_label.Size = new System.Drawing.Size(346, 24);
            this.GameExpeditionSetting_SingleDepartWaitTime_label.TabIndex = 80;
            this.GameExpeditionSetting_SingleDepartWaitTime_label.Text = "远征队伍一个一个出发：出发间隔                  秒 ";
            this.GameExpeditionSetting_SingleDepartWaitTime_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GameExpeditionSetting_SingleDepartWaitTime_label.UseMnemonic = false;
            this.GameExpeditionSetting_SingleDepartWaitTime_label.Visible = false;
            // 
            // GameExpeditionSetting_ExpeditionWaitTime_textBox
            // 
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox.Location = new System.Drawing.Point(180, 51);
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox.MaxLength = 3;
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox.Name = "GameExpeditionSetting_ExpeditionWaitTime_textBox";
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox.Size = new System.Drawing.Size(43, 21);
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox.TabIndex = 1;
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox.Text = "0";
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.GameExpeditionSetting_ExpeditionWaitTime_textBox.TextChanged += new System.EventHandler(this.GameExpedition_ExpeditionWaitTime_textBox_TextChanged);
            // 
            // GameExpeditionSetting_ExpeditionRanWaitTime_textBox
            // 
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Location = new System.Drawing.Point(282, 51);
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox.MaxLength = 3;
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Name = "GameExpeditionSetting_ExpeditionRanWaitTime_textBox";
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Size = new System.Drawing.Size(43, 21);
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox.TabIndex = 75;
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Text = "0";
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox.TextChanged += new System.EventHandler(this.GameExpedition_ExpeditionRanWaitTime_textBox_TextChanged);
            // 
            // GameExpeditionSetting_SingleDepart_checkBox
            // 
            this.GameExpeditionSetting_SingleDepart_checkBox.Enabled = false;
            this.GameExpeditionSetting_SingleDepart_checkBox.Location = new System.Drawing.Point(14, 20);
            this.GameExpeditionSetting_SingleDepart_checkBox.Name = "GameExpeditionSetting_SingleDepart_checkBox";
            this.GameExpeditionSetting_SingleDepart_checkBox.Size = new System.Drawing.Size(15, 14);
            this.GameExpeditionSetting_SingleDepart_checkBox.TabIndex = 76;
            this.GameExpeditionSetting_SingleDepart_checkBox.UseVisualStyleBackColor = true;
            this.GameExpeditionSetting_SingleDepart_checkBox.Visible = false;
            this.GameExpeditionSetting_SingleDepart_checkBox.CheckedChanged += new System.EventHandler(this.GameExpedition_SingleDepart_checkBox_CheckedChanged);
            // 
            // GameExpeditionSetting_SingleDepartWaitTime_textBox
            // 
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.Enabled = false;
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.Location = new System.Drawing.Point(221, 15);
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.MaxLength = 3;
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.Name = "GameExpeditionSetting_SingleDepartWaitTime_textBox";
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.Size = new System.Drawing.Size(43, 21);
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.TabIndex = 78;
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.Text = "0";
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.Visible = false;
            this.GameExpeditionSetting_SingleDepartWaitTime_textBox.TextChanged += new System.EventHandler(this.GameExpedition_SingleDepartWaitTime_textBox_TextChanged);
            // 
            // GameExpeditionSetting_IsFrequency_checkBox
            // 
            this.GameExpeditionSetting_IsFrequency_checkBox.Location = new System.Drawing.Point(10, 25);
            this.GameExpeditionSetting_IsFrequency_checkBox.Name = "GameExpeditionSetting_IsFrequency_checkBox";
            this.GameExpeditionSetting_IsFrequency_checkBox.Size = new System.Drawing.Size(15, 14);
            this.GameExpeditionSetting_IsFrequency_checkBox.TabIndex = 86;
            this.GameExpeditionSetting_IsFrequency_checkBox.UseVisualStyleBackColor = true;
            // 
            // GameExpeditionSetting_IsFrequency_label
            // 
            this.GameExpeditionSetting_IsFrequency_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameExpeditionSetting_IsFrequency_label.BackColor = System.Drawing.Color.Transparent;
            this.GameExpeditionSetting_IsFrequency_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_IsFrequency_label.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.GameExpeditionSetting_IsFrequency_label.Location = new System.Drawing.Point(30, 17);
            this.GameExpeditionSetting_IsFrequency_label.Name = "GameExpeditionSetting_IsFrequency_label";
            this.GameExpeditionSetting_IsFrequency_label.Size = new System.Drawing.Size(122, 24);
            this.GameExpeditionSetting_IsFrequency_label.TabIndex = 85;
            this.GameExpeditionSetting_IsFrequency_label.Text = "启用远征次数限制";
            this.GameExpeditionSetting_IsFrequency_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GameExpeditionSetting_IsFrequency_label.UseMnemonic = false;
            // 
            // GameExpeditionSetting_Frequency2_numericUpDown
            // 
            this.GameExpeditionSetting_Frequency2_numericUpDown.BackColor = System.Drawing.SystemColors.Window;
            this.GameExpeditionSetting_Frequency2_numericUpDown.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.GameExpeditionSetting_Frequency2_numericUpDown.Location = new System.Drawing.Point(72, 69);
            this.GameExpeditionSetting_Frequency2_numericUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.GameExpeditionSetting_Frequency2_numericUpDown.Name = "GameExpeditionSetting_Frequency2_numericUpDown";
            this.GameExpeditionSetting_Frequency2_numericUpDown.Size = new System.Drawing.Size(50, 23);
            this.GameExpeditionSetting_Frequency2_numericUpDown.TabIndex = 87;
            // 
            // GameExpeditionSetting_Team2_label
            // 
            this.GameExpeditionSetting_Team2_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameExpeditionSetting_Team2_label.BackColor = System.Drawing.Color.Transparent;
            this.GameExpeditionSetting_Team2_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_Team2_label.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.GameExpeditionSetting_Team2_label.Location = new System.Drawing.Point(6, 66);
            this.GameExpeditionSetting_Team2_label.Name = "GameExpeditionSetting_Team2_label";
            this.GameExpeditionSetting_Team2_label.Size = new System.Drawing.Size(59, 24);
            this.GameExpeditionSetting_Team2_label.TabIndex = 88;
            this.GameExpeditionSetting_Team2_label.Text = "第二舰队";
            this.GameExpeditionSetting_Team2_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GameExpeditionSetting_Team2_label.UseMnemonic = false;
            // 
            // GameExpeditionSetting_Team3_label
            // 
            this.GameExpeditionSetting_Team3_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameExpeditionSetting_Team3_label.BackColor = System.Drawing.Color.Transparent;
            this.GameExpeditionSetting_Team3_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_Team3_label.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.GameExpeditionSetting_Team3_label.Location = new System.Drawing.Point(6, 95);
            this.GameExpeditionSetting_Team3_label.Name = "GameExpeditionSetting_Team3_label";
            this.GameExpeditionSetting_Team3_label.Size = new System.Drawing.Size(59, 24);
            this.GameExpeditionSetting_Team3_label.TabIndex = 89;
            this.GameExpeditionSetting_Team3_label.Text = "第三舰队";
            this.GameExpeditionSetting_Team3_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GameExpeditionSetting_Team3_label.UseMnemonic = false;
            // 
            // GameExpeditionSetting_Team4_label
            // 
            this.GameExpeditionSetting_Team4_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameExpeditionSetting_Team4_label.BackColor = System.Drawing.Color.Transparent;
            this.GameExpeditionSetting_Team4_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_Team4_label.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.GameExpeditionSetting_Team4_label.Location = new System.Drawing.Point(6, 124);
            this.GameExpeditionSetting_Team4_label.Name = "GameExpeditionSetting_Team4_label";
            this.GameExpeditionSetting_Team4_label.Size = new System.Drawing.Size(59, 24);
            this.GameExpeditionSetting_Team4_label.TabIndex = 90;
            this.GameExpeditionSetting_Team4_label.Text = "第四舰队";
            this.GameExpeditionSetting_Team4_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GameExpeditionSetting_Team4_label.UseMnemonic = false;
            // 
            // GameExpeditionSetting_Frequency3_numericUpDown
            // 
            this.GameExpeditionSetting_Frequency3_numericUpDown.BackColor = System.Drawing.SystemColors.Window;
            this.GameExpeditionSetting_Frequency3_numericUpDown.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.GameExpeditionSetting_Frequency3_numericUpDown.Location = new System.Drawing.Point(72, 98);
            this.GameExpeditionSetting_Frequency3_numericUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.GameExpeditionSetting_Frequency3_numericUpDown.Name = "GameExpeditionSetting_Frequency3_numericUpDown";
            this.GameExpeditionSetting_Frequency3_numericUpDown.Size = new System.Drawing.Size(50, 23);
            this.GameExpeditionSetting_Frequency3_numericUpDown.TabIndex = 91;
            // 
            // GameExpeditionSetting_Frequency4_numericUpDown
            // 
            this.GameExpeditionSetting_Frequency4_numericUpDown.BackColor = System.Drawing.SystemColors.Window;
            this.GameExpeditionSetting_Frequency4_numericUpDown.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.GameExpeditionSetting_Frequency4_numericUpDown.Location = new System.Drawing.Point(72, 127);
            this.GameExpeditionSetting_Frequency4_numericUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.GameExpeditionSetting_Frequency4_numericUpDown.Name = "GameExpeditionSetting_Frequency4_numericUpDown";
            this.GameExpeditionSetting_Frequency4_numericUpDown.Size = new System.Drawing.Size(50, 23);
            this.GameExpeditionSetting_Frequency4_numericUpDown.TabIndex = 92;
            // 
            // GameExpeditionSetting_Frequency_label
            // 
            this.GameExpeditionSetting_Frequency_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameExpeditionSetting_Frequency_label.BackColor = System.Drawing.Color.Transparent;
            this.GameExpeditionSetting_Frequency_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_Frequency_label.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.GameExpeditionSetting_Frequency_label.Location = new System.Drawing.Point(68, 42);
            this.GameExpeditionSetting_Frequency_label.Name = "GameExpeditionSetting_Frequency_label";
            this.GameExpeditionSetting_Frequency_label.Size = new System.Drawing.Size(59, 24);
            this.GameExpeditionSetting_Frequency_label.TabIndex = 93;
            this.GameExpeditionSetting_Frequency_label.Text = "设定次数";
            this.GameExpeditionSetting_Frequency_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GameExpeditionSetting_Frequency_label.UseMnemonic = false;
            // 
            // GameExpeditionSetting_OverTime_label
            // 
            this.GameExpeditionSetting_OverTime_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameExpeditionSetting_OverTime_label.BackColor = System.Drawing.Color.Transparent;
            this.GameExpeditionSetting_OverTime_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_OverTime_label.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.GameExpeditionSetting_OverTime_label.Location = new System.Drawing.Point(146, 41);
            this.GameExpeditionSetting_OverTime_label.Name = "GameExpeditionSetting_OverTime_label";
            this.GameExpeditionSetting_OverTime_label.Size = new System.Drawing.Size(203, 24);
            this.GameExpeditionSetting_OverTime_label.TabIndex = 94;
            this.GameExpeditionSetting_OverTime_label.Text = "设定时间(yyyy-MM-dd hh:mm:ss)";
            this.GameExpeditionSetting_OverTime_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GameExpeditionSetting_OverTime_label.UseMnemonic = false;
            // 
            // GameExpeditionSetting_OverTime2_textBox
            // 
            this.GameExpeditionSetting_OverTime2_textBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_OverTime2_textBox.Location = new System.Drawing.Point(128, 71);
            this.GameExpeditionSetting_OverTime2_textBox.Name = "GameExpeditionSetting_OverTime2_textBox";
            this.GameExpeditionSetting_OverTime2_textBox.Size = new System.Drawing.Size(212, 23);
            this.GameExpeditionSetting_OverTime2_textBox.TabIndex = 95;
            this.GameExpeditionSetting_OverTime2_textBox.TextChanged += new System.EventHandler(this.GameExpeditionSetting_OverTime_textBox_TextChanged);
            // 
            // GameExpeditionSetting_OverTime3_textBox
            // 
            this.GameExpeditionSetting_OverTime3_textBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_OverTime3_textBox.Location = new System.Drawing.Point(128, 100);
            this.GameExpeditionSetting_OverTime3_textBox.Name = "GameExpeditionSetting_OverTime3_textBox";
            this.GameExpeditionSetting_OverTime3_textBox.Size = new System.Drawing.Size(212, 23);
            this.GameExpeditionSetting_OverTime3_textBox.TabIndex = 96;
            this.GameExpeditionSetting_OverTime3_textBox.TextChanged += new System.EventHandler(this.GameExpeditionSetting_OverTime_textBox_TextChanged);
            // 
            // GameExpeditionSetting_IsOverTime_checkBox
            // 
            this.GameExpeditionSetting_IsOverTime_checkBox.Location = new System.Drawing.Point(185, 25);
            this.GameExpeditionSetting_IsOverTime_checkBox.Name = "GameExpeditionSetting_IsOverTime_checkBox";
            this.GameExpeditionSetting_IsOverTime_checkBox.Size = new System.Drawing.Size(15, 14);
            this.GameExpeditionSetting_IsOverTime_checkBox.TabIndex = 98;
            this.GameExpeditionSetting_IsOverTime_checkBox.UseVisualStyleBackColor = true;
            this.GameExpeditionSetting_IsOverTime_checkBox.CheckedChanged += new System.EventHandler(this.GameExpeditionSetting_IsOverTime_checkBox_CheckedChanged);
            // 
            // GameExpeditionSetting_IsOverTime_label
            // 
            this.GameExpeditionSetting_IsOverTime_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.GameExpeditionSetting_IsOverTime_label.BackColor = System.Drawing.Color.Transparent;
            this.GameExpeditionSetting_IsOverTime_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_IsOverTime_label.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.GameExpeditionSetting_IsOverTime_label.Location = new System.Drawing.Point(206, 17);
            this.GameExpeditionSetting_IsOverTime_label.Name = "GameExpeditionSetting_IsOverTime_label";
            this.GameExpeditionSetting_IsOverTime_label.Size = new System.Drawing.Size(134, 24);
            this.GameExpeditionSetting_IsOverTime_label.TabIndex = 97;
            this.GameExpeditionSetting_IsOverTime_label.Text = "启用远征结束时间";
            this.GameExpeditionSetting_IsOverTime_label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.GameExpeditionSetting_IsOverTime_label.UseMnemonic = false;
            // 
            // GameExpeditionSetting_OverTime4_textBox
            // 
            this.GameExpeditionSetting_OverTime4_textBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_OverTime4_textBox.Location = new System.Drawing.Point(128, 125);
            this.GameExpeditionSetting_OverTime4_textBox.Name = "GameExpeditionSetting_OverTime4_textBox";
            this.GameExpeditionSetting_OverTime4_textBox.Size = new System.Drawing.Size(212, 23);
            this.GameExpeditionSetting_OverTime4_textBox.TabIndex = 99;
            this.GameExpeditionSetting_OverTime4_textBox.TextChanged += new System.EventHandler(this.GameExpeditionSetting_OverTime_textBox_TextChanged);
            // 
            // GameExpeditionSetting_LimitSetting_textBox
            // 
            this.GameExpeditionSetting_LimitSetting_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_OverTime4_textBox);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_IsOverTime_label);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_IsOverTime_checkBox);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_OverTime3_textBox);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_OverTime2_textBox);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_OverTime_label);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_Frequency_label);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_Frequency4_numericUpDown);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_Frequency3_numericUpDown);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_Team4_label);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_Team3_label);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_Team2_label);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_Frequency2_numericUpDown);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_IsFrequency_label);
            this.GameExpeditionSetting_LimitSetting_textBox.Controls.Add(this.GameExpeditionSetting_IsFrequency_checkBox);
            this.GameExpeditionSetting_LimitSetting_textBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GameExpeditionSetting_LimitSetting_textBox.Location = new System.Drawing.Point(12, 83);
            this.GameExpeditionSetting_LimitSetting_textBox.Name = "GameExpeditionSetting_LimitSetting_textBox";
            this.GameExpeditionSetting_LimitSetting_textBox.Size = new System.Drawing.Size(346, 158);
            this.GameExpeditionSetting_LimitSetting_textBox.TabIndex = 83;
            this.GameExpeditionSetting_LimitSetting_textBox.TabStop = false;
            this.GameExpeditionSetting_LimitSetting_textBox.Text = "远征次数设置";
            // 
            // ExpeditionSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 252);
            this.Controls.Add(this.GameExpeditionSetting_LimitSetting_textBox);
            this.Controls.Add(this.GameExpeditionSetting_SingleDepartWaitTime_textBox);
            this.Controls.Add(this.GameExpeditionSetting_SingleDepart_checkBox);
            this.Controls.Add(this.GameExpeditionSetting_ExpeditionRanWaitTime_textBox);
            this.Controls.Add(this.GameExpeditionSetting_ExpeditionWaitTime_textBox);
            this.Controls.Add(this.GameExpeditionSetting_SingleDepartWaitTime_label);
            this.Controls.Add(this.GameExpeditionSetting_ExpeditionWaitTime_label);
            this.Name = "ExpeditionSetting";
            this.Text = "远征设置";
            ((System.ComponentModel.ISupportInitialize)(this.GameExpeditionSetting_Frequency2_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GameExpeditionSetting_Frequency3_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GameExpeditionSetting_Frequency4_numericUpDown)).EndInit();
            this.GameExpeditionSetting_LimitSetting_textBox.ResumeLayout(false);
            this.GameExpeditionSetting_LimitSetting_textBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void GameExpedition_SingleDepart_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (GameExpeditionSetting_SingleDepart_checkBox.Checked)
                GameExpeditionSetting_SingleDepartWaitTime_textBox.Enabled = true;
            else GameExpeditionSetting_SingleDepartWaitTime_textBox.Enabled = false;
            if (GameExpeditionSetting_SingleDepart_checkBox.Checked != IsSingleDepart)
                IsSingleDepart = GameExpeditionSetting_SingleDepart_checkBox.Checked;
        }
        private void GameExpedition_SingleDepartWaitTime_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpeditionSetting_SingleDepartWaitTime_textBox.Text = Regex.Replace(GameExpeditionSetting_SingleDepartWaitTime_textBox.Text, @"[^\d]*", "");
            if (GameExpeditionSetting_SingleDepartWaitTime_textBox.Text == "")
            { GameExpeditionSetting_SingleDepartWaitTime_textBox.Text = "0"; }
            if (int.Parse(GameExpeditionSetting_SingleDepartWaitTime_textBox.Text) != SingleDepartWaitTime)
                SingleDepartWaitTime = int.Parse(GameExpeditionSetting_SingleDepartWaitTime_textBox.Text);
        }

        private void GameExpedition_ExpeditionWaitTime_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpeditionSetting_ExpeditionWaitTime_textBox.Text = Regex.Replace(GameExpeditionSetting_ExpeditionWaitTime_textBox.Text, @"[^\d]*", "");
            if (GameExpeditionSetting_ExpeditionWaitTime_textBox.Text == "")
            { GameExpeditionSetting_ExpeditionWaitTime_textBox.Text = "0"; }
            if (int.Parse(GameExpeditionSetting_ExpeditionWaitTime_textBox.Text) != ExpeditionWaitTime)
                ExpeditionWaitTime = int.Parse(GameExpeditionSetting_ExpeditionWaitTime_textBox.Text);
        }
        private void GameExpedition_ExpeditionRanWaitTime_textBox_TextChanged(object sender, EventArgs e)
        {
            GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Text = Regex.Replace(GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Text, @"[^\d]*", "");
            if (GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Text == "")
            { GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Text = "0"; }
            if (ExpeditionRanWaitTime != int.Parse(GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Text))
                ExpeditionRanWaitTime = int.Parse(GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Text);
        }

        public void Reset()
        {
            GameExpeditionSetting_SingleDepart_checkBox.Checked = IsSingleDepart;
            GameExpeditionSetting_SingleDepartWaitTime_textBox.Text = SingleDepartWaitTime.ToString();
            GameExpeditionSetting_ExpeditionWaitTime_textBox.Text = ExpeditionWaitTime.ToString();
            GameExpeditionSetting_ExpeditionRanWaitTime_textBox.Text = ExpeditionRanWaitTime.ToString();
        }

        private void GameExpeditionSetting_OverTime_textBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox == false) return;
            var overtime = sender as TextBox;
            try
            {
                var time = Convert.ToDateTime(overtime.Text);
            }
            catch (FormatException)
            {
                overtime.BackColor = Color.Red;
                return;
            }
            overtime.BackColor = Color.White;
        }

        private void GameExpeditionSetting_IsOverTime_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            var isOverTime = sender as CheckBox;
            if (isOverTime.Checked)
            {
                var time = DateTime.Now.ToString();
                if (GameExpeditionSetting_OverTime2_textBox.Text == "")
                    GameExpeditionSetting_OverTime2_textBox.Text = time;
                if (GameExpeditionSetting_OverTime3_textBox.Text == "")
                    GameExpeditionSetting_OverTime3_textBox.Text = time;
                if (GameExpeditionSetting_OverTime4_textBox.Text == "")
                    GameExpeditionSetting_OverTime4_textBox.Text = time;
            }
        }
    }
}
