namespace NokiKanColle.Window
{
    partial class CatchData_Form
    {
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
            this.CatchData_GamePicture = new System.Windows.Forms.PictureBox();
            this.CatchData_LoadPicture = new System.Windows.Forms.Button();
            this.CatchData_CatchXY = new System.Windows.Forms.TextBox();
            this.CatchData_CatchWH = new System.Windows.Forms.TextBox();
            this.CatchData_CurrentXY = new System.Windows.Forms.TextBox();
            this.CatchData_LabelCatchXY = new System.Windows.Forms.Label();
            this.CatchData_LabelCurrentXY = new System.Windows.Forms.Label();
            this.CatchData_LabelCatchHW = new System.Windows.Forms.Label();
            this.CatchData_GetDataShow = new System.Windows.Forms.TextBox();
            this.CatchData_GetData = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.CatchData_GamePicture)).BeginInit();
            this.SuspendLayout();
            // 
            // CatchData_GamePicture
            // 
            this.CatchData_GamePicture.Location = new System.Drawing.Point(12, 12);
            this.CatchData_GamePicture.Name = "CatchData_GamePicture";
            this.CatchData_GamePicture.Size = new System.Drawing.Size(800, 480);
            this.CatchData_GamePicture.TabIndex = 0;
            this.CatchData_GamePicture.TabStop = false;
            this.CatchData_GamePicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CatchData_GamePicture_MouseDown);
            this.CatchData_GamePicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CatchData_GamePicture_MouseMove);
            this.CatchData_GamePicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CatchData_GamePicture_MouseUp);
            // 
            // CatchData_LoadPicture
            // 
            this.CatchData_LoadPicture.Location = new System.Drawing.Point(818, 12);
            this.CatchData_LoadPicture.Name = "CatchData_LoadPicture";
            this.CatchData_LoadPicture.Size = new System.Drawing.Size(188, 55);
            this.CatchData_LoadPicture.TabIndex = 1;
            this.CatchData_LoadPicture.Text = "载入图像";
            this.CatchData_LoadPicture.UseVisualStyleBackColor = true;
            this.CatchData_LoadPicture.Click += new System.EventHandler(this.CatchData_LoadPicture_Click);
            // 
            // CatchData_CatchXY
            // 
            this.CatchData_CatchXY.Location = new System.Drawing.Point(898, 88);
            this.CatchData_CatchXY.Name = "CatchData_CatchXY";
            this.CatchData_CatchXY.Size = new System.Drawing.Size(108, 21);
            this.CatchData_CatchXY.TabIndex = 2;
            // 
            // CatchData_CatchWH
            // 
            this.CatchData_CatchWH.Location = new System.Drawing.Point(898, 115);
            this.CatchData_CatchWH.Name = "CatchData_CatchWH";
            this.CatchData_CatchWH.Size = new System.Drawing.Size(108, 21);
            this.CatchData_CatchWH.TabIndex = 2;
            // 
            // CatchData_CurrentXY
            // 
            this.CatchData_CurrentXY.Location = new System.Drawing.Point(898, 470);
            this.CatchData_CurrentXY.Name = "CatchData_CurrentXY";
            this.CatchData_CurrentXY.Size = new System.Drawing.Size(108, 21);
            this.CatchData_CurrentXY.TabIndex = 2;
            // 
            // CatchData_LabelCatchXY
            // 
            this.CatchData_LabelCatchXY.AutoSize = true;
            this.CatchData_LabelCatchXY.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CatchData_LabelCatchXY.Location = new System.Drawing.Point(818, 88);
            this.CatchData_LabelCatchXY.Name = "CatchData_LabelCatchXY";
            this.CatchData_LabelCatchXY.Size = new System.Drawing.Size(56, 17);
            this.CatchData_LabelCatchXY.TabIndex = 3;
            this.CatchData_LabelCatchXY.Text = "起始坐标";
            // 
            // CatchData_LabelCurrentXY
            // 
            this.CatchData_LabelCurrentXY.AutoSize = true;
            this.CatchData_LabelCurrentXY.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CatchData_LabelCurrentXY.Location = new System.Drawing.Point(818, 470);
            this.CatchData_LabelCurrentXY.Name = "CatchData_LabelCurrentXY";
            this.CatchData_LabelCurrentXY.Size = new System.Drawing.Size(56, 17);
            this.CatchData_LabelCurrentXY.TabIndex = 3;
            this.CatchData_LabelCurrentXY.Text = "当前坐标";
            // 
            // CatchData_LabelCatchHW
            // 
            this.CatchData_LabelCatchHW.AutoSize = true;
            this.CatchData_LabelCatchHW.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CatchData_LabelCatchHW.Location = new System.Drawing.Point(818, 115);
            this.CatchData_LabelCatchHW.Name = "CatchData_LabelCatchHW";
            this.CatchData_LabelCatchHW.Size = new System.Drawing.Size(56, 17);
            this.CatchData_LabelCatchHW.TabIndex = 3;
            this.CatchData_LabelCatchHW.Text = "图像宽高";
            // 
            // CatchData_GetDataShow
            // 
            this.CatchData_GetDataShow.Location = new System.Drawing.Point(818, 203);
            this.CatchData_GetDataShow.Multiline = true;
            this.CatchData_GetDataShow.Name = "CatchData_GetDataShow";
            this.CatchData_GetDataShow.Size = new System.Drawing.Size(188, 85);
            this.CatchData_GetDataShow.TabIndex = 4;
            // 
            // CatchData_GetData
            // 
            this.CatchData_GetData.Location = new System.Drawing.Point(818, 142);
            this.CatchData_GetData.Name = "CatchData_GetData";
            this.CatchData_GetData.Size = new System.Drawing.Size(188, 55);
            this.CatchData_GetData.TabIndex = 5;
            this.CatchData_GetData.Text = "获取信息";
            this.CatchData_GetData.UseVisualStyleBackColor = true;
            this.CatchData_GetData.Click += new System.EventHandler(this.CatchData_GetData_Click);
            // 
            // CatchData_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 503);
            this.Controls.Add(this.CatchData_GetData);
            this.Controls.Add(this.CatchData_GetDataShow);
            this.Controls.Add(this.CatchData_LabelCurrentXY);
            this.Controls.Add(this.CatchData_LabelCatchHW);
            this.Controls.Add(this.CatchData_LabelCatchXY);
            this.Controls.Add(this.CatchData_CatchWH);
            this.Controls.Add(this.CatchData_CurrentXY);
            this.Controls.Add(this.CatchData_CatchXY);
            this.Controls.Add(this.CatchData_LoadPicture);
            this.Controls.Add(this.CatchData_GamePicture);
            this.Name = "CatchData_Form";
            this.Text = "CatchData_Form";
            ((System.ComponentModel.ISupportInitialize)(this.CatchData_GamePicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox CatchData_GamePicture;
        private System.Windows.Forms.Button CatchData_LoadPicture;
        private System.Windows.Forms.TextBox CatchData_CatchXY;
        private System.Windows.Forms.TextBox CatchData_CatchWH;
        private System.Windows.Forms.TextBox CatchData_CurrentXY;
        private System.Windows.Forms.Label CatchData_LabelCatchXY;
        private System.Windows.Forms.Label CatchData_LabelCurrentXY;
        private System.Windows.Forms.Label CatchData_LabelCatchHW;
        private System.Windows.Forms.TextBox CatchData_GetDataShow;
        private System.Windows.Forms.Button CatchData_GetData;
    }
}