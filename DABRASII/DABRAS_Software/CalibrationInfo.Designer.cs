namespace DABRAS_Software
{
    partial class CalibrationInfo
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
            this.Beta_KEV_TB = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.Beta_Eff_TB = new System.Windows.Forms.TextBox();
            this.Source_ComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Alpha_Eff_TB = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Alpha_Avg_TB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Beta_Avg_TB = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Alpha_UL_TB = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Alpha_LL_TB = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Beta_UL_TB = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Beta_LL_TB = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Beta_KEV_TB
            // 
            this.Beta_KEV_TB.Location = new System.Drawing.Point(163, 155);
            this.Beta_KEV_TB.Name = "Beta_KEV_TB";
            this.Beta_KEV_TB.ReadOnly = true;
            this.Beta_KEV_TB.Size = new System.Drawing.Size(100, 20);
            this.Beta_KEV_TB.TabIndex = 44;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(12, 153);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(145, 20);
            this.label10.TabIndex = 45;
            this.label10.Text = "Beta Energy (KeV):";
            // 
            // Beta_Eff_TB
            // 
            this.Beta_Eff_TB.Location = new System.Drawing.Point(163, 116);
            this.Beta_Eff_TB.Name = "Beta_Eff_TB";
            this.Beta_Eff_TB.ReadOnly = true;
            this.Beta_Eff_TB.Size = new System.Drawing.Size(100, 20);
            this.Beta_Eff_TB.TabIndex = 26;
            // 
            // Source_ComboBox
            // 
            this.Source_ComboBox.FormattingEnabled = true;
            this.Source_ComboBox.Location = new System.Drawing.Point(159, 46);
            this.Source_ComboBox.Name = "Source_ComboBox";
            this.Source_ComboBox.Size = new System.Drawing.Size(121, 21);
            this.Source_ComboBox.TabIndex = 23;
            this.Source_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Source_ComboBox_TextUpdate);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 20);
            this.label4.TabIndex = 37;
            this.label4.Text = "Beta Efficiency:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 20);
            this.label1.TabIndex = 36;
            this.label1.Text = "Select Source: ";
            // 
            // Alpha_Eff_TB
            // 
            this.Alpha_Eff_TB.Location = new System.Drawing.Point(163, 89);
            this.Alpha_Eff_TB.Name = "Alpha_Eff_TB";
            this.Alpha_Eff_TB.ReadOnly = true;
            this.Alpha_Eff_TB.Size = new System.Drawing.Size(100, 20);
            this.Alpha_Eff_TB.TabIndex = 46;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(12, 87);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(126, 20);
            this.label11.TabIndex = 47;
            this.label11.Text = "Alpha Efficiency:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 195);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(206, 20);
            this.label2.TabIndex = 48;
            this.label2.Text = "HiLo Calibration Information";
            // 
            // Alpha_Avg_TB
            // 
            this.Alpha_Avg_TB.Location = new System.Drawing.Point(163, 235);
            this.Alpha_Avg_TB.Name = "Alpha_Avg_TB";
            this.Alpha_Avg_TB.ReadOnly = true;
            this.Alpha_Avg_TB.Size = new System.Drawing.Size(100, 20);
            this.Alpha_Avg_TB.TabIndex = 51;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 233);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 20);
            this.label3.TabIndex = 52;
            this.label3.Text = "Alpha Average:";
            // 
            // Beta_Avg_TB
            // 
            this.Beta_Avg_TB.Location = new System.Drawing.Point(163, 262);
            this.Beta_Avg_TB.Name = "Beta_Avg_TB";
            this.Beta_Avg_TB.ReadOnly = true;
            this.Beta_Avg_TB.Size = new System.Drawing.Size(100, 20);
            this.Beta_Avg_TB.TabIndex = 49;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 260);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 20);
            this.label5.TabIndex = 50;
            this.label5.Text = "Beta Average:";
            // 
            // Alpha_UL_TB
            // 
            this.Alpha_UL_TB.Location = new System.Drawing.Point(180, 291);
            this.Alpha_UL_TB.Name = "Alpha_UL_TB";
            this.Alpha_UL_TB.ReadOnly = true;
            this.Alpha_UL_TB.Size = new System.Drawing.Size(100, 20);
            this.Alpha_UL_TB.TabIndex = 55;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 289);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 20);
            this.label6.TabIndex = 56;
            this.label6.Text = "Alpha Upper Control:";
            // 
            // Alpha_LL_TB
            // 
            this.Alpha_LL_TB.Location = new System.Drawing.Point(180, 318);
            this.Alpha_LL_TB.Name = "Alpha_LL_TB";
            this.Alpha_LL_TB.ReadOnly = true;
            this.Alpha_LL_TB.Size = new System.Drawing.Size(100, 20);
            this.Alpha_LL_TB.TabIndex = 53;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 316);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(156, 20);
            this.label7.TabIndex = 54;
            this.label7.Text = "Alpha Lower Control:";
            // 
            // Beta_UL_TB
            // 
            this.Beta_UL_TB.Location = new System.Drawing.Point(180, 345);
            this.Beta_UL_TB.Name = "Beta_UL_TB";
            this.Beta_UL_TB.ReadOnly = true;
            this.Beta_UL_TB.Size = new System.Drawing.Size(100, 20);
            this.Beta_UL_TB.TabIndex = 59;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(13, 343);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(150, 20);
            this.label8.TabIndex = 60;
            this.label8.Text = "Beta Upper Control:";
            // 
            // Beta_LL_TB
            // 
            this.Beta_LL_TB.Location = new System.Drawing.Point(180, 372);
            this.Beta_LL_TB.Name = "Beta_LL_TB";
            this.Beta_LL_TB.ReadOnly = true;
            this.Beta_LL_TB.Size = new System.Drawing.Size(100, 20);
            this.Beta_LL_TB.TabIndex = 57;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(13, 370);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(149, 20);
            this.label9.TabIndex = 58;
            this.label9.Text = "Beta Lower Control:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(305, 24);
            this.menuStrip1.TabIndex = 61;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeCtrlQToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeCtrlQToolStripMenuItem
            // 
            this.closeCtrlQToolStripMenuItem.Name = "closeCtrlQToolStripMenuItem";
            this.closeCtrlQToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.closeCtrlQToolStripMenuItem.Text = "Close (Ctrl + Q)";
            this.closeCtrlQToolStripMenuItem.Click += new System.EventHandler(this.closeCtrlQToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // CalibrationInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 420);
            this.Controls.Add(this.Beta_UL_TB);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.Beta_LL_TB);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.Alpha_UL_TB);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Alpha_LL_TB);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Alpha_Avg_TB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Beta_Avg_TB);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Alpha_Eff_TB);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.Beta_KEV_TB);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.Beta_Eff_TB);
            this.Controls.Add(this.Source_ComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CalibrationInfo";
            this.Text = "CalibrationInfo";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Beta_KEV_TB;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox Beta_Eff_TB;
        private System.Windows.Forms.ComboBox Source_ComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Alpha_Eff_TB;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Alpha_Avg_TB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Beta_Avg_TB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Alpha_UL_TB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Alpha_LL_TB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox Beta_UL_TB;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox Beta_LL_TB;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}