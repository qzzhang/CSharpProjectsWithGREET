namespace DABRAS_Software
{
    partial class ChallengePassword
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
            this.Submit_button = new System.Windows.Forms.Button();
            this.Password_Textbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Submit_button
            // 
            this.Submit_button.Location = new System.Drawing.Point(167, 85);
            this.Submit_button.Name = "Submit_button";
            this.Submit_button.Size = new System.Drawing.Size(75, 23);
            this.Submit_button.TabIndex = 0;
            this.Submit_button.Text = "Submit";
            this.Submit_button.UseVisualStyleBackColor = true;
            this.Submit_button.Click += new System.EventHandler(this.Submit_button_Click);
            // 
            // Password_Textbox
            // 
            this.Password_Textbox.Location = new System.Drawing.Point(31, 50);
            this.Password_Textbox.Name = "Password_Textbox";
            this.Password_Textbox.PasswordChar = '•';
            this.Password_Textbox.Size = new System.Drawing.Size(211, 20);
            this.Password_Textbox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(258, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Enter the administrator\'s password:";
            // 
            // ChallengePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 120);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Password_Textbox);
            this.Controls.Add(this.Submit_button);
            this.Name = "ChallengePassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChallengePassword";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Submit_button;
        private System.Windows.Forms.TextBox Password_Textbox;
        private System.Windows.Forms.Label label1;
    }
}