namespace DABRAS_Software
{
    partial class BatchNumber
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
            this.Submit_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.Batch_ComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Enter Sample ID";
            // 
            // Submit_Button
            // 
            this.Submit_Button.Location = new System.Drawing.Point(12, 58);
            this.Submit_Button.Name = "Submit_Button";
            this.Submit_Button.Size = new System.Drawing.Size(75, 23);
            this.Submit_Button.TabIndex = 1;
            this.Submit_Button.Text = "Submit";
            this.Submit_Button.UseVisualStyleBackColor = true;
            this.Submit_Button.Click += new System.EventHandler(this.Submit_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Location = new System.Drawing.Point(173, 58);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_Button.TabIndex = 2;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // Batch_ComboBox
            // 
            this.Batch_ComboBox.FormattingEnabled = true;
            this.Batch_ComboBox.Location = new System.Drawing.Point(16, 33);
            this.Batch_ComboBox.Name = "Batch_ComboBox";
            this.Batch_ComboBox.Size = new System.Drawing.Size(232, 21);
            this.Batch_ComboBox.TabIndex = 4;
            this.Batch_ComboBox.Click += new System.EventHandler(this.Batch_ComboBox_Click);
            // 
            // BatchNumber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 111);
            this.Controls.Add(this.Batch_ComboBox);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.Submit_Button);
            this.Controls.Add(this.label1);
            this.Name = "BatchNumber";
            this.Text = "BatchNumber";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Submit_Button;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.ComboBox Batch_ComboBox;
    }
}