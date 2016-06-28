namespace DABRAS_Software
{
    partial class ChiSquaredCalibrationRequirements
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
            this.label1 = new System.Windows.Forms.Label();
            this.Never_Button = new System.Windows.Forms.RadioButton();
            this.Annually_Button = new System.Windows.Forms.RadioButton();
            this.Monthly_Button = new System.Windows.Forms.RadioButton();
            this.Weekly_Button = new System.Windows.Forms.RadioButton();
            this.Daily_Button = new System.Windows.Forms.RadioButton();
            this.Set_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(339, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Force Chi Squared Calibration to be Performed";
            // 
            // Never_Button
            // 
            this.Never_Button.AutoSize = true;
            this.Never_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Never_Button.Location = new System.Drawing.Point(16, 45);
            this.Never_Button.Name = "Never_Button";
            this.Never_Button.Size = new System.Drawing.Size(63, 20);
            this.Never_Button.TabIndex = 1;
            this.Never_Button.TabStop = true;
            this.Never_Button.Text = "Never";
            this.Never_Button.UseVisualStyleBackColor = true;
            // 
            // Annually_Button
            // 
            this.Annually_Button.AutoSize = true;
            this.Annually_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Annually_Button.Location = new System.Drawing.Point(16, 71);
            this.Annually_Button.Name = "Annually_Button";
            this.Annually_Button.Size = new System.Drawing.Size(77, 20);
            this.Annually_Button.TabIndex = 2;
            this.Annually_Button.TabStop = true;
            this.Annually_Button.Text = "Annually";
            this.Annually_Button.UseVisualStyleBackColor = true;
            // 
            // Monthly_Button
            // 
            this.Monthly_Button.AutoSize = true;
            this.Monthly_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Monthly_Button.Location = new System.Drawing.Point(16, 97);
            this.Monthly_Button.Name = "Monthly_Button";
            this.Monthly_Button.Size = new System.Drawing.Size(72, 20);
            this.Monthly_Button.TabIndex = 3;
            this.Monthly_Button.TabStop = true;
            this.Monthly_Button.Text = "Monthly";
            this.Monthly_Button.UseVisualStyleBackColor = true;
            // 
            // Weekly_Button
            // 
            this.Weekly_Button.AutoSize = true;
            this.Weekly_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Weekly_Button.Location = new System.Drawing.Point(16, 123);
            this.Weekly_Button.Name = "Weekly_Button";
            this.Weekly_Button.Size = new System.Drawing.Size(72, 20);
            this.Weekly_Button.TabIndex = 4;
            this.Weekly_Button.TabStop = true;
            this.Weekly_Button.Text = "Weekly";
            this.Weekly_Button.UseVisualStyleBackColor = true;
            // 
            // Daily_Button
            // 
            this.Daily_Button.AutoSize = true;
            this.Daily_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Daily_Button.Location = new System.Drawing.Point(16, 149);
            this.Daily_Button.Name = "Daily_Button";
            this.Daily_Button.Size = new System.Drawing.Size(57, 20);
            this.Daily_Button.TabIndex = 5;
            this.Daily_Button.TabStop = true;
            this.Daily_Button.Text = "Daily";
            this.Daily_Button.UseVisualStyleBackColor = true;
            // 
            // Set_Button
            // 
            this.Set_Button.Location = new System.Drawing.Point(123, 45);
            this.Set_Button.Name = "Set_Button";
            this.Set_Button.Size = new System.Drawing.Size(75, 46);
            this.Set_Button.TabIndex = 6;
            this.Set_Button.Text = "Set";
            this.Set_Button.UseVisualStyleBackColor = true;
            this.Set_Button.Click += new System.EventHandler(this.Set_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Location = new System.Drawing.Point(123, 109);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(75, 60);
            this.Cancel_Button.TabIndex = 7;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // ChiSquaredCalibrationRequirements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 185);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.Set_Button);
            this.Controls.Add(this.Daily_Button);
            this.Controls.Add(this.Weekly_Button);
            this.Controls.Add(this.Monthly_Button);
            this.Controls.Add(this.Annually_Button);
            this.Controls.Add(this.Never_Button);
            this.Controls.Add(this.label1);
            this.Name = "ChiSquaredCalibrationRequirements";
            this.Text = "ChiSquaredCalibrationRequirements";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton Never_Button;
        private System.Windows.Forms.RadioButton Annually_Button;
        private System.Windows.Forms.RadioButton Monthly_Button;
        private System.Windows.Forms.RadioButton Weekly_Button;
        private System.Windows.Forms.RadioButton Daily_Button;
        private System.Windows.Forms.Button Set_Button;
        private System.Windows.Forms.Button Cancel_Button;
    }
}