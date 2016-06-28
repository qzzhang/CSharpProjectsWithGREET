namespace DABRAS_Software
{
    partial class FullDataSetPopup
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
            this.FullDataSet = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeToFileCtrlVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.FullDataSet)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FullDataSet
            // 
            this.FullDataSet.BackgroundColor = System.Drawing.SystemColors.Control;
            this.FullDataSet.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FullDataSet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FullDataSet.Location = new System.Drawing.Point(12, 27);
            this.FullDataSet.Name = "FullDataSet";
            this.FullDataSet.Size = new System.Drawing.Size(1180, 210);
            this.FullDataSet.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1204, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeCtrlQToolStripMenuItem,
            this.writeToFileCtrlVToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeCtrlQToolStripMenuItem
            // 
            this.closeCtrlQToolStripMenuItem.Name = "closeCtrlQToolStripMenuItem";
            this.closeCtrlQToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.closeCtrlQToolStripMenuItem.Text = "Close (Ctrl + Q)";
            this.closeCtrlQToolStripMenuItem.Click += new System.EventHandler(this.closeCtrlQToolStripMenuItem_Click);
            // 
            // writeToFileCtrlVToolStripMenuItem
            // 
            this.writeToFileCtrlVToolStripMenuItem.Name = "writeToFileCtrlVToolStripMenuItem";
            this.writeToFileCtrlVToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.writeToFileCtrlVToolStripMenuItem.Text = "Write To File (Ctrl + V)";
            this.writeToFileCtrlVToolStripMenuItem.Click += new System.EventHandler(this.writeToFileCtrlVToolStripMenuItem_Click);
            // 
            // FullDataSetPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 249);
            this.Controls.Add(this.FullDataSet);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FullDataSetPopup";
            this.Text = "FullDataSetPopup";
            ((System.ComponentModel.ISupportInitialize)(this.FullDataSet)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView FullDataSet;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem writeToFileCtrlVToolStripMenuItem;
    }
}