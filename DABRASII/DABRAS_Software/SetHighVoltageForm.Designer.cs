namespace DABRAS_Software
{
    partial class SetHighVoltageForm
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
            this.CloseButton = new System.Windows.Forms.Button();
            this.SetButton = new System.Windows.Forms.Button();
            this.CurrentHighVoltageLabel = new System.Windows.Forms.Label();
            this.NewHighVoltageLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.HV_TB = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(103, 81);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // SetButton
            // 
            this.SetButton.Location = new System.Drawing.Point(16, 81);
            this.SetButton.Name = "SetButton";
            this.SetButton.Size = new System.Drawing.Size(75, 23);
            this.SetButton.TabIndex = 1;
            this.SetButton.Text = "Set";
            this.SetButton.UseVisualStyleBackColor = true;
            this.SetButton.Click += new System.EventHandler(this.SetButton_Click);
            // 
            // CurrentHighVoltageLabel
            // 
            this.CurrentHighVoltageLabel.AutoSize = true;
            this.CurrentHighVoltageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentHighVoltageLabel.Location = new System.Drawing.Point(12, 9);
            this.CurrentHighVoltageLabel.Name = "CurrentHighVoltageLabel";
            this.CurrentHighVoltageLabel.Size = new System.Drawing.Size(51, 20);
            this.CurrentHighVoltageLabel.TabIndex = 2;
            this.CurrentHighVoltageLabel.Text = "label1";
            // 
            // NewHighVoltageLabel
            // 
            this.NewHighVoltageLabel.AutoSize = true;
            this.NewHighVoltageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewHighVoltageLabel.Location = new System.Drawing.Point(12, 48);
            this.NewHighVoltageLabel.Name = "NewHighVoltageLabel";
            this.NewHighVoltageLabel.Size = new System.Drawing.Size(166, 20);
            this.NewHighVoltageLabel.TabIndex = 3;
            this.NewHighVoltageLabel.Text = "Set Voltage Control to";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(248, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "mV";
            // 
            // HV_TB
            // 
            this.HV_TB.Location = new System.Drawing.Point(184, 48);
            this.HV_TB.Name = "HV_TB";
            this.HV_TB.Size = new System.Drawing.Size(58, 20);
            this.HV_TB.TabIndex = 5;
            // 
            // SetHighVoltageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 140);
            this.Controls.Add(this.HV_TB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NewHighVoltageLabel);
            this.Controls.Add(this.CurrentHighVoltageLabel);
            this.Controls.Add(this.SetButton);
            this.Controls.Add(this.CloseButton);
            this.Name = "SetHighVoltageForm";
            this.Text = "SetHighVoltageForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button SetButton;
        private System.Windows.Forms.Label CurrentHighVoltageLabel;
        private System.Windows.Forms.Label NewHighVoltageLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox HV_TB;
    }
}