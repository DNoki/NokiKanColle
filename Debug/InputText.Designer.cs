namespace NokiKanColle.Debug
{
    partial class InputText
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
            this.Debug_InputText_textBox = new System.Windows.Forms.TextBox();
            this.Debug_Yes_button = new System.Windows.Forms.Button();
            this.Debug_No_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Debug_InputText_textBox
            // 
            this.Debug_InputText_textBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Debug_InputText_textBox.Location = new System.Drawing.Point(12, 12);
            this.Debug_InputText_textBox.Name = "Debug_InputText_textBox";
            this.Debug_InputText_textBox.Size = new System.Drawing.Size(501, 23);
            this.Debug_InputText_textBox.TabIndex = 0;
            // 
            // Debug_Yes_button
            // 
            this.Debug_Yes_button.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Debug_Yes_button.Location = new System.Drawing.Point(12, 41);
            this.Debug_Yes_button.Name = "Debug_Yes_button";
            this.Debug_Yes_button.Size = new System.Drawing.Size(246, 23);
            this.Debug_Yes_button.TabIndex = 1;
            this.Debug_Yes_button.Text = "Yes";
            this.Debug_Yes_button.UseVisualStyleBackColor = true;
            this.Debug_Yes_button.Click += new System.EventHandler(this.Debug_Yes_button_Click);
            // 
            // Debug_No_button
            // 
            this.Debug_No_button.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Debug_No_button.Location = new System.Drawing.Point(266, 41);
            this.Debug_No_button.Name = "Debug_No_button";
            this.Debug_No_button.Size = new System.Drawing.Size(246, 23);
            this.Debug_No_button.TabIndex = 2;
            this.Debug_No_button.Text = "No";
            this.Debug_No_button.UseVisualStyleBackColor = true;
            this.Debug_No_button.Click += new System.EventHandler(this.Debug_No_button_Click);
            // 
            // InputText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 69);
            this.Controls.Add(this.Debug_No_button);
            this.Controls.Add(this.Debug_Yes_button);
            this.Controls.Add(this.Debug_InputText_textBox);
            this.Name = "InputText";
            this.Text = "InptuText";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InputText_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Debug_InputText_textBox;
        private System.Windows.Forms.Button Debug_Yes_button;
        private System.Windows.Forms.Button Debug_No_button;
    }
}