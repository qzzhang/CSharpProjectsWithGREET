namespace DABRAS_Software
{
    partial class ChangePasswordForm
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
            this.Old_PW_TB = new System.Windows.Forms.TextBox();
            this.New_PW_TB_1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.Submit_Button = new System.Windows.Forms.Button();
            this.New_PW_TB_2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Enter old password:";
            // 
            // Old_PW_TB
            // 
            this.Old_PW_TB.Location = new System.Drawing.Point(194, 24);
            this.Old_PW_TB.Name = "Old_PW_TB";
            this.Old_PW_TB.PasswordChar = '•';
            this.Old_PW_TB.Size = new System.Drawing.Size(158, 20);
            this.Old_PW_TB.TabIndex = 0;
            // 
            // New_PW_TB_1
            // 
            this.New_PW_TB_1.Location = new System.Drawing.Point(194, 54);
            this.New_PW_TB_1.Name = "New_PW_TB_1";
            this.New_PW_TB_1.PasswordChar = '•';
            this.New_PW_TB_1.Size = new System.Drawing.Size(158, 20);
            this.New_PW_TB_1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Enter new password:";
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Location = new System.Drawing.Point(97, 151);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_Button.TabIndex = 4;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // Submit_Button
            // 
            this.Submit_Button.Location = new System.Drawing.Point(16, 151);
            this.Submit_Button.Name = "Submit_Button";
            this.Submit_Button.Size = new System.Drawing.Size(75, 23);
            this.Submit_Button.TabIndex = 3;
            this.Submit_Button.Text = "Submit";
            this.Submit_Button.UseVisualStyleBackColor = true;
            this.Submit_Button.Click += new System.EventHandler(this.Submit_Button_Click);
            // 
            // New_PW_TB_2
            // 
            this.New_PW_TB_2.Location = new System.Drawing.Point(194, 80);
            this.New_PW_TB_2.Name = "New_PW_TB_2";
            this.New_PW_TB_2.PasswordChar = '•';
            this.New_PW_TB_2.Size = new System.Drawing.Size(158, 20);
            this.New_PW_TB_2.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Reenter new password:";
            // 
            // ChangePasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 193);
            this.Controls.Add(this.New_PW_TB_2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Submit_Button);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.New_PW_TB_1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Old_PW_TB);
            this.Controls.Add(this.label1);
            this.Name = "ChangePasswordForm";
            this.Text = "ChangePasswordForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Old_PW_TB;
        private System.Windows.Forms.TextBox New_PW_TB_1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.Button Submit_Button;
        private System.Windows.Forms.TextBox New_PW_TB_2;
        private System.Windows.Forms.Label label3;
    }
}