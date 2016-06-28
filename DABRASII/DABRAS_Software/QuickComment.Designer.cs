namespace DABRAS_Software
{
    partial class QuickComment
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
            this.Comment_TB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Submit_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Comment_TB
            // 
            this.Comment_TB.Location = new System.Drawing.Point(12, 37);
            this.Comment_TB.Name = "Comment_TB";
            this.Comment_TB.Size = new System.Drawing.Size(401, 20);
            this.Comment_TB.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Comment";
            // 
            // Submit_Button
            // 
            this.Submit_Button.Location = new System.Drawing.Point(17, 83);
            this.Submit_Button.Name = "Submit_Button";
            this.Submit_Button.Size = new System.Drawing.Size(128, 23);
            this.Submit_Button.TabIndex = 2;
            this.Submit_Button.Text = "Submit (Ctrl + S)";
            this.Submit_Button.UseVisualStyleBackColor = true;
            this.Submit_Button.Click += new System.EventHandler(this.Submit_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Location = new System.Drawing.Point(298, 83);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(114, 23);
            this.Cancel_Button.TabIndex = 3;
            this.Cancel_Button.Text = "Cancel (Ctrl + C)";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // QuickComment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(437, 118);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.Submit_Button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Comment_TB);
            this.Name = "QuickComment";
            this.Text = "QuickComment";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Comment_TB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Submit_Button;
        private System.Windows.Forms.Button Cancel_Button;
    }
}