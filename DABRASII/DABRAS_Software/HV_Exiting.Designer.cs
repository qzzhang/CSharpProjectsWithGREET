namespace DABRAS_Software
{
    partial class HV_Exiting
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
            this.SetTop_Button = new System.Windows.Forms.RadioButton();
            this.SetBottom_Button = new System.Windows.Forms.RadioButton();
            this.MidValue_TB = new System.Windows.Forms.TextBox();
            this.OK_Button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SetMid_Button = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // SetTop_Button
            // 
            this.SetTop_Button.AutoSize = true;
            this.SetTop_Button.Checked = true;
            this.SetTop_Button.Location = new System.Drawing.Point(13, 13);
            this.SetTop_Button.Name = "SetTop_Button";
            this.SetTop_Button.Size = new System.Drawing.Size(35, 17);
            this.SetTop_Button.TabIndex = 0;
            this.SetTop_Button.TabStop = true;
            this.SetTop_Button.Text = "\"\"";
            this.SetTop_Button.UseVisualStyleBackColor = true;
            // 
            // SetBottom_Button
            // 
            this.SetBottom_Button.AutoSize = true;
            this.SetBottom_Button.Location = new System.Drawing.Point(12, 36);
            this.SetBottom_Button.Name = "SetBottom_Button";
            this.SetBottom_Button.Size = new System.Drawing.Size(35, 17);
            this.SetBottom_Button.TabIndex = 1;
            this.SetBottom_Button.Text = "\"\"";
            this.SetBottom_Button.UseVisualStyleBackColor = true;
            // 
            // MidValue_TB
            // 
            this.MidValue_TB.Location = new System.Drawing.Point(132, 58);
            this.MidValue_TB.Name = "MidValue_TB";
            this.MidValue_TB.Size = new System.Drawing.Size(100, 20);
            this.MidValue_TB.TabIndex = 3;
            this.MidValue_TB.Text = "4500";
            // 
            // OK_Button
            // 
            this.OK_Button.Location = new System.Drawing.Point(12, 80);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(75, 23);
            this.OK_Button.TabIndex = 4;
            this.OK_Button.Text = "OK";
            this.OK_Button.UseVisualStyleBackColor = true;
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(238, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "mV";
            // 
            // SetMid_Button
            // 
            this.SetMid_Button.AutoSize = true;
            this.SetMid_Button.Location = new System.Drawing.Point(12, 59);
            this.SetMid_Button.Name = "SetMid_Button";
            this.SetMid_Button.Size = new System.Drawing.Size(114, 17);
            this.SetMid_Button.TabIndex = 6;
            this.SetMid_Button.Text = "Set HV Control To:";
            this.SetMid_Button.UseVisualStyleBackColor = true;
            // 
            // HV_Exiting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 112);
            this.Controls.Add(this.SetMid_Button);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OK_Button);
            this.Controls.Add(this.MidValue_TB);
            this.Controls.Add(this.SetBottom_Button);
            this.Controls.Add(this.SetTop_Button);
            this.Name = "HV_Exiting";
            this.Text = "HV_Exiting";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton SetTop_Button;
        private System.Windows.Forms.RadioButton SetBottom_Button;
        private System.Windows.Forms.TextBox MidValue_TB;
        private System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton SetMid_Button;
    }
}