namespace DABRAS_Software
{
    partial class FormHVCsetting
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
            this.label2 = new System.Windows.Forms.Label();
            this.OK_Button = new System.Windows.Forms.Button();
            this.MidValue_TB = new System.Windows.Forms.TextBox();
            this.SetBottom_Button = new System.Windows.Forms.RadioButton();
            this.SetTop_Button = new System.Windows.Forms.RadioButton();
            this.rdSetMidHV = new System.Windows.Forms.RadioButton();
            this.rdSetBottomHV = new System.Windows.Forms.RadioButton();
            this.rdSetTopHV = new System.Windows.Forms.RadioButton();
            this.CurrentHighVoltageLabel = new System.Windows.Forms.Label();
            this.btnCancelHVCset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(353, 133);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 16);
            this.label2.TabIndex = 11;
            this.label2.Text = "mV";
            // 
            // OK_Button
            // 
            this.OK_Button.BackColor = System.Drawing.Color.Bisque;
            this.OK_Button.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.OK_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.OK_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OK_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.OK_Button.ForeColor = System.Drawing.Color.DarkGreen;
            this.OK_Button.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.OK_Button.Location = new System.Drawing.Point(127, 199);
            this.OK_Button.Margin = new System.Windows.Forms.Padding(6);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(128, 43);
            this.OK_Button.TabIndex = 10;
            this.OK_Button.Text = "Set";
            this.OK_Button.UseVisualStyleBackColor = false;
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // MidValue_TB
            // 
            this.MidValue_TB.Location = new System.Drawing.Point(195, 127);
            this.MidValue_TB.Margin = new System.Windows.Forms.Padding(6);
            this.MidValue_TB.Name = "MidValue_TB";
            this.MidValue_TB.Size = new System.Drawing.Size(132, 22);
            this.MidValue_TB.TabIndex = 9;
            this.MidValue_TB.Text = "4500";
            // 
            // SetBottom_Button
            // 
            this.SetBottom_Button.AutoSize = true;
            this.SetBottom_Button.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SetBottom_Button.Location = new System.Drawing.Point(-57, 121);
            this.SetBottom_Button.Margin = new System.Windows.Forms.Padding(6);
            this.SetBottom_Button.Name = "SetBottom_Button";
            this.SetBottom_Button.Size = new System.Drawing.Size(38, 20);
            this.SetBottom_Button.TabIndex = 8;
            this.SetBottom_Button.Text = "\"\"";
            this.SetBottom_Button.UseVisualStyleBackColor = true;
            // 
            // SetTop_Button
            // 
            this.SetTop_Button.AutoSize = true;
            this.SetTop_Button.Checked = true;
            this.SetTop_Button.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SetTop_Button.Location = new System.Drawing.Point(-56, 78);
            this.SetTop_Button.Margin = new System.Windows.Forms.Padding(6);
            this.SetTop_Button.Name = "SetTop_Button";
            this.SetTop_Button.Size = new System.Drawing.Size(38, 20);
            this.SetTop_Button.TabIndex = 7;
            this.SetTop_Button.TabStop = true;
            this.SetTop_Button.Text = "\"\"";
            this.SetTop_Button.UseVisualStyleBackColor = true;
            // 
            // rdSetMidHV
            // 
            this.rdSetMidHV.AutoSize = true;
            this.rdSetMidHV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdSetMidHV.Location = new System.Drawing.Point(9, 127);
            this.rdSetMidHV.Margin = new System.Windows.Forms.Padding(6);
            this.rdSetMidHV.Name = "rdSetMidHV";
            this.rdSetMidHV.Size = new System.Drawing.Size(154, 20);
            this.rdSetMidHV.TabIndex = 15;
            this.rdSetMidHV.Text = "Set HV Control To:";
            this.rdSetMidHV.UseVisualStyleBackColor = true;
            // 
            // rdSetBottomHV
            // 
            this.rdSetBottomHV.AutoSize = true;
            this.rdSetBottomHV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdSetBottomHV.Location = new System.Drawing.Point(9, 82);
            this.rdSetBottomHV.Margin = new System.Windows.Forms.Padding(6);
            this.rdSetBottomHV.Name = "rdSetBottomHV";
            this.rdSetBottomHV.Size = new System.Drawing.Size(38, 20);
            this.rdSetBottomHV.TabIndex = 14;
            this.rdSetBottomHV.Text = "\"\"";
            this.rdSetBottomHV.UseVisualStyleBackColor = true;
            // 
            // rdSetTopHV
            // 
            this.rdSetTopHV.AutoSize = true;
            this.rdSetTopHV.Checked = true;
            this.rdSetTopHV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdSetTopHV.Location = new System.Drawing.Point(10, 39);
            this.rdSetTopHV.Margin = new System.Windows.Forms.Padding(6);
            this.rdSetTopHV.Name = "rdSetTopHV";
            this.rdSetTopHV.Size = new System.Drawing.Size(38, 20);
            this.rdSetTopHV.TabIndex = 13;
            this.rdSetTopHV.TabStop = true;
            this.rdSetTopHV.Text = "\"\"";
            this.rdSetTopHV.UseVisualStyleBackColor = true;
            // 
            // CurrentHighVoltageLabel
            // 
            this.CurrentHighVoltageLabel.AutoSize = true;
            this.CurrentHighVoltageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentHighVoltageLabel.ForeColor = System.Drawing.Color.Black;
            this.CurrentHighVoltageLabel.Location = new System.Drawing.Point(22, 294);
            this.CurrentHighVoltageLabel.Name = "CurrentHighVoltageLabel";
            this.CurrentHighVoltageLabel.Size = new System.Drawing.Size(51, 20);
            this.CurrentHighVoltageLabel.TabIndex = 16;
            this.CurrentHighVoltageLabel.Text = "label1";
            // 
            // btnCancelHVCset
            // 
            this.btnCancelHVCset.BackColor = System.Drawing.Color.SeaShell;
            this.btnCancelHVCset.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnCancelHVCset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.btnCancelHVCset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelHVCset.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancelHVCset.ForeColor = System.Drawing.Color.DarkGreen;
            this.btnCancelHVCset.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancelHVCset.Location = new System.Drawing.Point(267, 199);
            this.btnCancelHVCset.Margin = new System.Windows.Forms.Padding(6);
            this.btnCancelHVCset.Name = "btnCancelHVCset";
            this.btnCancelHVCset.Size = new System.Drawing.Size(128, 43);
            this.btnCancelHVCset.TabIndex = 17;
            this.btnCancelHVCset.Text = "Close";
            this.btnCancelHVCset.UseVisualStyleBackColor = false;
            this.btnCancelHVCset.Click += new System.EventHandler(this.btnCancelHVCset_Click);
            // 
            // FormHVCsetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 332);
            this.Controls.Add(this.btnCancelHVCset);
            this.Controls.Add(this.CurrentHighVoltageLabel);
            this.Controls.Add(this.rdSetMidHV);
            this.Controls.Add(this.rdSetBottomHV);
            this.Controls.Add(this.rdSetTopHV);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OK_Button);
            this.Controls.Add(this.MidValue_TB);
            this.Controls.Add(this.SetBottom_Button);
            this.Controls.Add(this.SetTop_Button);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormHVCsetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HV Finalizing";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.TextBox MidValue_TB;
        private System.Windows.Forms.RadioButton SetBottom_Button;
        private System.Windows.Forms.RadioButton SetTop_Button;
        private System.Windows.Forms.RadioButton rdSetMidHV;
        private System.Windows.Forms.RadioButton rdSetBottomHV;
        private System.Windows.Forms.RadioButton rdSetTopHV;
        private System.Windows.Forms.Label CurrentHighVoltageLabel;
        private System.Windows.Forms.Button btnCancelHVCset;
    }
}