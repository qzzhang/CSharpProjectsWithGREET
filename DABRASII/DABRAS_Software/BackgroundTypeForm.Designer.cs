namespace DABRAS_Software
{
    partial class BackgroundTypeForm
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
            this.Annual_Button = new System.Windows.Forms.RadioButton();
            this.Daily_Button = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Save_Button = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(352, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Background Type to use in Routine Computation";
            // 
            // Annual_Button
            // 
            this.Annual_Button.AutoSize = true;
            this.Annual_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Annual_Button.Location = new System.Drawing.Point(6, 19);
            this.Annual_Button.Name = "Annual_Button";
            this.Annual_Button.Size = new System.Drawing.Size(512, 24);
            this.Annual_Button.TabIndex = 1;
            this.Annual_Button.TabStop = true;
            this.Annual_Button.Text = "Use Annually Computed CPM values for Background Alpha and Beta";
            this.Annual_Button.UseVisualStyleBackColor = true;
            // 
            // Daily_Button
            // 
            this.Daily_Button.AutoSize = true;
            this.Daily_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Daily_Button.Location = new System.Drawing.Point(6, 41);
            this.Daily_Button.Name = "Daily_Button";
            this.Daily_Button.Size = new System.Drawing.Size(486, 24);
            this.Daily_Button.TabIndex = 2;
            this.Daily_Button.TabStop = true;
            this.Daily_Button.Text = "Use Daily Computed CPM values for Background Alpha and Beta";
            this.Daily_Button.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Daily_Button);
            this.groupBox1.Controls.Add(this.Annual_Button);
            this.groupBox1.Location = new System.Drawing.Point(16, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(532, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // Save_Button
            // 
            this.Save_Button.Location = new System.Drawing.Point(16, 110);
            this.Save_Button.Name = "Save_Button";
            this.Save_Button.Size = new System.Drawing.Size(107, 23);
            this.Save_Button.TabIndex = 1;
            this.Save_Button.Text = "Save (Ctrl + S)";
            this.Save_Button.UseVisualStyleBackColor = true;
            this.Save_Button.Click += new System.EventHandler(this.Save_Button_Click);
            // 
            // BackgroundTypeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 157);
            this.Controls.Add(this.Save_Button);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "BackgroundTypeForm";
            this.Text = "BackgroundTypeForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton Annual_Button;
        private System.Windows.Forms.RadioButton Daily_Button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Save_Button;
    }
}