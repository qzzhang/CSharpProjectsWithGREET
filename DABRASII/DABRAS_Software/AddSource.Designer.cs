namespace DABRAS_Software
{
    partial class AddSource
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
            this.SaveButton = new System.Windows.Forms.Button();
            this.CertDate_DTP = new System.Windows.Forms.DateTimePicker();
            this.Beta_Button = new System.Windows.Forms.RadioButton();
            this.Alpha_Beta_Button = new System.Windows.Forms.RadioButton();
            this.Alpha_Button = new System.Windows.Forms.RadioButton();
            this.CurAct_TB = new System.Windows.Forms.TextBox();
            this.CertAct_TB = new System.Windows.Forms.TextBox();
            this.HalfLife_TB = new System.Windows.Forms.TextBox();
            this.Description_TB = new System.Windows.Forms.TextBox();
            this.Serial_TB = new System.Windows.Forms.TextBox();
            this.HalfLife_Combobox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Source_TB = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Unknown_Button = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this._100_200KeV_Button = new System.Windows.Forms.RadioButton();
            this._200_400_KeV_Button = new System.Windows.Forms.RadioButton();
            this._400_1200_KeV_Button = new System.Windows.Forms.RadioButton();
            this._1200_KeV_Button = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(338, 19);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(90, 23);
            this.SaveButton.TabIndex = 10;
            this.SaveButton.Text = "Save Changes";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CertDate_DTP
            // 
            this.CertDate_DTP.Location = new System.Drawing.Point(238, 323);
            this.CertDate_DTP.Name = "CertDate_DTP";
            this.CertDate_DTP.Size = new System.Drawing.Size(200, 20);
            this.CertDate_DTP.TabIndex = 7;
            // 
            // Beta_Button
            // 
            this.Beta_Button.AutoSize = true;
            this.Beta_Button.Location = new System.Drawing.Point(64, 12);
            this.Beta_Button.Name = "Beta_Button";
            this.Beta_Button.Size = new System.Drawing.Size(47, 17);
            this.Beta_Button.TabIndex = 0;
            this.Beta_Button.Text = "Beta";
            this.Beta_Button.UseVisualStyleBackColor = true;
            this.Beta_Button.CheckedChanged += new System.EventHandler(this.Beta_Button_CheckedChanged);
            // 
            // Alpha_Beta_Button
            // 
            this.Alpha_Beta_Button.AutoSize = true;
            this.Alpha_Beta_Button.Location = new System.Drawing.Point(117, 12);
            this.Alpha_Beta_Button.Name = "Alpha_Beta_Button";
            this.Alpha_Beta_Button.Size = new System.Drawing.Size(123, 17);
            this.Alpha_Beta_Button.TabIndex = 1;
            this.Alpha_Beta_Button.Text = "Both Alpha and Beta";
            this.Alpha_Beta_Button.UseVisualStyleBackColor = true;
            this.Alpha_Beta_Button.CheckedChanged += new System.EventHandler(this.AlphaBeta_Button_CheckedChanged);
            // 
            // Alpha_Button
            // 
            this.Alpha_Button.AutoSize = true;
            this.Alpha_Button.Checked = true;
            this.Alpha_Button.Location = new System.Drawing.Point(6, 12);
            this.Alpha_Button.Name = "Alpha_Button";
            this.Alpha_Button.Size = new System.Drawing.Size(52, 17);
            this.Alpha_Button.TabIndex = 3;
            this.Alpha_Button.TabStop = true;
            this.Alpha_Button.Text = "Alpha";
            this.Alpha_Button.UseVisualStyleBackColor = true;
            this.Alpha_Button.CheckedChanged += new System.EventHandler(this.Alpha_Button_CheckedChanged);
            // 
            // CurAct_TB
            // 
            this.CurAct_TB.Location = new System.Drawing.Point(238, 417);
            this.CurAct_TB.Name = "CurAct_TB";
            this.CurAct_TB.Size = new System.Drawing.Size(100, 20);
            this.CurAct_TB.TabIndex = 9;
            // 
            // CertAct_TB
            // 
            this.CertAct_TB.Location = new System.Drawing.Point(238, 373);
            this.CertAct_TB.Name = "CertAct_TB";
            this.CertAct_TB.Size = new System.Drawing.Size(100, 20);
            this.CertAct_TB.TabIndex = 8;
            // 
            // HalfLife_TB
            // 
            this.HalfLife_TB.Location = new System.Drawing.Point(238, 274);
            this.HalfLife_TB.Name = "HalfLife_TB";
            this.HalfLife_TB.Size = new System.Drawing.Size(100, 20);
            this.HalfLife_TB.TabIndex = 5;
            // 
            // Description_TB
            // 
            this.Description_TB.Location = new System.Drawing.Point(156, 122);
            this.Description_TB.Name = "Description_TB";
            this.Description_TB.Size = new System.Drawing.Size(330, 20);
            this.Description_TB.TabIndex = 2;
            // 
            // Serial_TB
            // 
            this.Serial_TB.Location = new System.Drawing.Point(156, 87);
            this.Serial_TB.Name = "Serial_TB";
            this.Serial_TB.Size = new System.Drawing.Size(100, 20);
            this.Serial_TB.TabIndex = 1;
            // 
            // HalfLife_Combobox
            // 
            this.HalfLife_Combobox.FormattingEnabled = true;
            this.HalfLife_Combobox.Location = new System.Drawing.Point(365, 273);
            this.HalfLife_Combobox.Name = "HalfLife_Combobox";
            this.HalfLife_Combobox.Size = new System.Drawing.Size(121, 21);
            this.HalfLife_Combobox.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(9, 417);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(186, 20);
            this.label8.TabIndex = 29;
            this.label8.Text = "Currently Applied Activity:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(9, 371);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(170, 20);
            this.label7.TabIndex = 28;
            this.label7.Text = "Certified Activity (dpm):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(9, 323);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 20);
            this.label6.TabIndex = 27;
            this.label6.Text = "Certification Date:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(9, 274);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 20);
            this.label5.TabIndex = 26;
            this.label5.Text = "Half Life:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 20);
            this.label4.TabIndex = 25;
            this.label4.Text = "Serial Number:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 20);
            this.label3.TabIndex = 24;
            this.label3.Text = "Description:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 20);
            this.label2.TabIndex = 23;
            this.label2.Text = "Type of Source:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 20);
            this.label1.TabIndex = 22;
            this.label1.Text = "Source Name:";
            // 
            // Source_TB
            // 
            this.Source_TB.Location = new System.Drawing.Point(156, 19);
            this.Source_TB.Name = "Source_TB";
            this.Source_TB.Size = new System.Drawing.Size(100, 20);
            this.Source_TB.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Unknown_Button);
            this.groupBox1.Controls.Add(this.Beta_Button);
            this.groupBox1.Controls.Add(this.Alpha_Beta_Button);
            this.groupBox1.Controls.Add(this.Alpha_Button);
            this.groupBox1.Location = new System.Drawing.Point(156, 158);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 35);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // Unknown_Button
            // 
            this.Unknown_Button.AutoSize = true;
            this.Unknown_Button.Location = new System.Drawing.Point(246, 12);
            this.Unknown_Button.Name = "Unknown_Button";
            this.Unknown_Button.Size = new System.Drawing.Size(71, 17);
            this.Unknown_Button.TabIndex = 2;
            this.Unknown_Button.Text = "Unknown";
            this.Unknown_Button.UseVisualStyleBackColor = true;
            this.Unknown_Button.CheckedChanged += new System.EventHandler(this.Unknown_Button_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(9, 227);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(156, 20);
            this.label9.TabIndex = 45;
            this.label9.Text = "Beta Source Energy:";
            // 
            // _100_200KeV_Button
            // 
            this._100_200KeV_Button.AutoSize = true;
            this._100_200KeV_Button.Checked = true;
            this._100_200KeV_Button.Enabled = false;
            this._100_200KeV_Button.Location = new System.Drawing.Point(25, 19);
            this._100_200KeV_Button.Name = "_100_200KeV_Button";
            this._100_200KeV_Button.Size = new System.Drawing.Size(104, 17);
            this._100_200KeV_Button.TabIndex = 0;
            this._100_200KeV_Button.TabStop = true;
            this._100_200KeV_Button.Text = "100KeV-200KeV";
            this._100_200KeV_Button.UseVisualStyleBackColor = true;
            // 
            // _200_400_KeV_Button
            // 
            this._200_400_KeV_Button.AutoSize = true;
            this._200_400_KeV_Button.Enabled = false;
            this._200_400_KeV_Button.Location = new System.Drawing.Point(135, 19);
            this._200_400_KeV_Button.Name = "_200_400_KeV_Button";
            this._200_400_KeV_Button.Size = new System.Drawing.Size(104, 17);
            this._200_400_KeV_Button.TabIndex = 2;
            this._200_400_KeV_Button.Text = "201KeV-400KeV";
            this._200_400_KeV_Button.UseVisualStyleBackColor = true;
            // 
            // _400_1200_KeV_Button
            // 
            this._400_1200_KeV_Button.AutoSize = true;
            this._400_1200_KeV_Button.Enabled = false;
            this._400_1200_KeV_Button.Location = new System.Drawing.Point(25, 39);
            this._400_1200_KeV_Button.Name = "_400_1200_KeV_Button";
            this._400_1200_KeV_Button.Size = new System.Drawing.Size(110, 17);
            this._400_1200_KeV_Button.TabIndex = 1;
            this._400_1200_KeV_Button.Text = "401KeV-1200KeV";
            this._400_1200_KeV_Button.UseVisualStyleBackColor = true;
            // 
            // _1200_KeV_Button
            // 
            this._1200_KeV_Button.AutoSize = true;
            this._1200_KeV_Button.Enabled = false;
            this._1200_KeV_Button.Location = new System.Drawing.Point(135, 39);
            this._1200_KeV_Button.Name = "_1200_KeV_Button";
            this._1200_KeV_Button.Size = new System.Drawing.Size(75, 17);
            this._1200_KeV_Button.TabIndex = 3;
            this._1200_KeV_Button.Text = ">1200KeV";
            this._1200_KeV_Button.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._1200_KeV_Button);
            this.groupBox2.Controls.Add(this._400_1200_KeV_Button);
            this.groupBox2.Controls.Add(this._200_400_KeV_Button);
            this.groupBox2.Controls.Add(this._100_200KeV_Button);
            this.groupBox2.Location = new System.Drawing.Point(167, 199);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 62);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            // 
            // AddSource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 455);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Source_TB);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.CertDate_DTP);
            this.Controls.Add(this.CurAct_TB);
            this.Controls.Add(this.CertAct_TB);
            this.Controls.Add(this.HalfLife_TB);
            this.Controls.Add(this.Description_TB);
            this.Controls.Add(this.Serial_TB);
            this.Controls.Add(this.HalfLife_Combobox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "AddSource";
            this.Text = "AddSource";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddSource_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.DateTimePicker CertDate_DTP;
        private System.Windows.Forms.RadioButton Beta_Button;
        private System.Windows.Forms.RadioButton Alpha_Beta_Button;
        private System.Windows.Forms.RadioButton Alpha_Button;
        private System.Windows.Forms.TextBox CurAct_TB;
        private System.Windows.Forms.TextBox CertAct_TB;
        private System.Windows.Forms.TextBox HalfLife_TB;
        private System.Windows.Forms.TextBox Description_TB;
        private System.Windows.Forms.TextBox Serial_TB;
        private System.Windows.Forms.ComboBox HalfLife_Combobox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Source_TB;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton _100_200KeV_Button;
        private System.Windows.Forms.RadioButton _200_400_KeV_Button;
        private System.Windows.Forms.RadioButton _400_1200_KeV_Button;
        private System.Windows.Forms.RadioButton _1200_KeV_Button;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton Unknown_Button;
    }
}