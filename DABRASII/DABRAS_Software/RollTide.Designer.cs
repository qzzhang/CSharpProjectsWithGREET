namespace DABRAS_Software
{
    partial class RollTide
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
            this.RTR_Image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.RTR_Image)).BeginInit();
            this.SuspendLayout();
            // 
            // RTR_Image
            // 
            this.RTR_Image.Location = new System.Drawing.Point(13, 13);
            this.RTR_Image.Name = "RTR_Image";
            this.RTR_Image.Size = new System.Drawing.Size(300, 214);
            this.RTR_Image.TabIndex = 0;
            this.RTR_Image.TabStop = false;
            // 
            // RollTide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 236);
            this.Controls.Add(this.RTR_Image);
            this.Name = "RollTide";
            this.Text = "RollTide";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RollTide_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.RTR_Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox RTR_Image;
    }
}