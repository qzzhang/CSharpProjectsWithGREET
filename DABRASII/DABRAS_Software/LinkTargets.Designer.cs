namespace DABRAS_Software
{
    partial class LinkTargets
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
            this.Save_Button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Web_Survey_TB = new System.Windows.Forms.TextBox();
            this.RSO_Link_TB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.RSO_Home_TB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.ResetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Save_Button
            // 
            this.Save_Button.Location = new System.Drawing.Point(16, 160);
            this.Save_Button.Name = "Save_Button";
            this.Save_Button.Size = new System.Drawing.Size(75, 23);
            this.Save_Button.TabIndex = 0;
            this.Save_Button.Text = "Save";
            this.Save_Button.UseVisualStyleBackColor = true;
            this.Save_Button.Click += new System.EventHandler(this.Save_Button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Web Survey URL";
            // 
            // Web_Survey_TB
            // 
            this.Web_Survey_TB.Location = new System.Drawing.Point(16, 32);
            this.Web_Survey_TB.Name = "Web_Survey_TB";
            this.Web_Survey_TB.Size = new System.Drawing.Size(343, 20);
            this.Web_Survey_TB.TabIndex = 2;
            // 
            // RSO_Link_TB
            // 
            this.RSO_Link_TB.Location = new System.Drawing.Point(16, 78);
            this.RSO_Link_TB.Name = "RSO_Link_TB";
            this.RSO_Link_TB.Size = new System.Drawing.Size(343, 20);
            this.RSO_Link_TB.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "RSO SharePoint Link";
            // 
            // RSO_Home_TB
            // 
            this.RSO_Home_TB.Location = new System.Drawing.Point(16, 134);
            this.RSO_Home_TB.Name = "RSO_Home_TB";
            this.RSO_Home_TB.Size = new System.Drawing.Size(343, 20);
            this.RSO_Home_TB.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "RSO Homepage Link";
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Location = new System.Drawing.Point(284, 160);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_Button.TabIndex = 7;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(131, 160);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(103, 23);
            this.ResetButton.TabIndex = 8;
            this.ResetButton.Text = "Reset to Defaults";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // LinkTargets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 204);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.RSO_Home_TB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.RSO_Link_TB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Web_Survey_TB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Save_Button);
            this.Name = "LinkTargets";
            this.Text = "LinkTargets";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Save_Button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Web_Survey_TB;
        private System.Windows.Forms.TextBox RSO_Link_TB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox RSO_Home_TB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.Button ResetButton;
    }
}