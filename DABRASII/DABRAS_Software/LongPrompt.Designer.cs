namespace DABRAS_Software
{
    partial class LongPrompt
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
            this.SubmitButton = new System.Windows.Forms.Button();
            this.PromptLable = new System.Windows.Forms.Label();
            this.TextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // SubmitButton
            // 
            this.SubmitButton.Location = new System.Drawing.Point(218, 138);
            this.SubmitButton.Name = "SubmitButton";
            this.SubmitButton.Size = new System.Drawing.Size(75, 23);
            this.SubmitButton.TabIndex = 0;
            this.SubmitButton.Text = "Submit";
            this.SubmitButton.UseVisualStyleBackColor = true;
            this.SubmitButton.Click += new System.EventHandler(this.SubmitButton_Click);
            // 
            // PromptLable
            // 
            this.PromptLable.AutoSize = true;
            this.PromptLable.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PromptLable.Location = new System.Drawing.Point(13, 13);
            this.PromptLable.Name = "PromptLable";
            this.PromptLable.Size = new System.Drawing.Size(18, 20);
            this.PromptLable.TabIndex = 1;
            this.PromptLable.Text = "?";
            // 
            // TextBox
            // 
            this.TextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox.Location = new System.Drawing.Point(13, 36);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(526, 96);
            this.TextBox.TabIndex = 2;
            this.TextBox.Text = "";
            // 
            // LongPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 173);
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.PromptLable);
            this.Controls.Add(this.SubmitButton);
            this.Name = "LongPrompt";
            this.Text = "LongPrompt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SubmitButton;
        private System.Windows.Forms.Label PromptLable;
        private System.Windows.Forms.RichTextBox TextBox;
    }
}