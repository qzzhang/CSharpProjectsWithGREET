namespace DABRAS_Software
{
    partial class DailyQCLog
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
            this.BackButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextCtrlNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.TimeCompleted_TB = new System.Windows.Forms.TextBox();
            this.QCType_TB = new System.Windows.Forms.TextBox();
            this.NetAlpha_TB = new System.Windows.Forms.TextBox();
            this.NetBeta_TB = new System.Windows.Forms.TextBox();
            this.Name_TB = new System.Windows.Forms.TextBox();
            this.BadgeNo_TB = new System.Windows.Forms.TextBox();
            this.Comment_TB = new System.Windows.Forms.RichTextBox();
            this.backCtrlBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BackButton
            // 
            this.BackButton.Location = new System.Drawing.Point(12, 428);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(100, 36);
            this.BackButton.TabIndex = 0;
            this.BackButton.Text = "Back (Ctrl + B)";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(320, 428);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(82, 36);
            this.NextButton.TabIndex = 1;
            this.NextButton.Text = "Next (Ctrl + N)";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.functionToolStripMenuItem,
            this.utilityToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(421, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeCtrlQToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeCtrlQToolStripMenuItem
            // 
            this.closeCtrlQToolStripMenuItem.Name = "closeCtrlQToolStripMenuItem";
            this.closeCtrlQToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.closeCtrlQToolStripMenuItem.Text = "Close (Ctrl + Q)";
            this.closeCtrlQToolStripMenuItem.Click += new System.EventHandler(this.closeCtrlQToolStripMenuItem_Click);
            // 
            // functionToolStripMenuItem
            // 
            this.functionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nextCtrlNToolStripMenuItem,
            this.backCtrlBToolStripMenuItem});
            this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
            this.functionToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.functionToolStripMenuItem.Text = "Function";
            // 
            // nextCtrlNToolStripMenuItem
            // 
            this.nextCtrlNToolStripMenuItem.Name = "nextCtrlNToolStripMenuItem";
            this.nextCtrlNToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.nextCtrlNToolStripMenuItem.Text = "Next (Ctrl + N)";
            this.nextCtrlNToolStripMenuItem.Click += new System.EventHandler(this.nextCtrlNToolStripMenuItem_Click);
            // 
            // utilityToolStripMenuItem
            // 
            this.utilityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem});
            this.utilityToolStripMenuItem.Name = "utilityToolStripMenuItem";
            this.utilityToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.utilityToolStripMenuItem.Text = "Utility";
            // 
            // connectOrDisconnectAPortCtrlPToolStripMenuItem
            // 
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem.Name = "connectOrDisconnectAPortCtrlPToolStripMenuItem";
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem.Size = new System.Drawing.Size(280, 22);
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem.Text = "Connect or Disconnect a Port (Ctrl + P)";
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem.Click += new System.EventHandler(this.connectOrDisconnectAPortCtrlPToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Date Completed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "QC Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(117, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Net Alpha CPM";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Net Beta CPM";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 198);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 20);
            this.label5.TabIndex = 7;
            this.label5.Text = "Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 234);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 20);
            this.label6.TabIndex = 8;
            this.label6.Text = "Badge Number";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 271);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 20);
            this.label7.TabIndex = 9;
            this.label7.Text = "Comment";
            // 
            // TimeCompleted_TB
            // 
            this.TimeCompleted_TB.Location = new System.Drawing.Point(172, 66);
            this.TimeCompleted_TB.Name = "TimeCompleted_TB";
            this.TimeCompleted_TB.Size = new System.Drawing.Size(230, 20);
            this.TimeCompleted_TB.TabIndex = 10;
            // 
            // QCType_TB
            // 
            this.QCType_TB.Location = new System.Drawing.Point(172, 98);
            this.QCType_TB.Name = "QCType_TB";
            this.QCType_TB.Size = new System.Drawing.Size(230, 20);
            this.QCType_TB.TabIndex = 11;
            // 
            // NetAlpha_TB
            // 
            this.NetAlpha_TB.Location = new System.Drawing.Point(172, 131);
            this.NetAlpha_TB.Name = "NetAlpha_TB";
            this.NetAlpha_TB.Size = new System.Drawing.Size(230, 20);
            this.NetAlpha_TB.TabIndex = 12;
            // 
            // NetBeta_TB
            // 
            this.NetBeta_TB.Location = new System.Drawing.Point(172, 166);
            this.NetBeta_TB.Name = "NetBeta_TB";
            this.NetBeta_TB.Size = new System.Drawing.Size(230, 20);
            this.NetBeta_TB.TabIndex = 13;
            // 
            // Name_TB
            // 
            this.Name_TB.Location = new System.Drawing.Point(172, 200);
            this.Name_TB.Name = "Name_TB";
            this.Name_TB.Size = new System.Drawing.Size(230, 20);
            this.Name_TB.TabIndex = 14;
            // 
            // BadgeNo_TB
            // 
            this.BadgeNo_TB.Location = new System.Drawing.Point(172, 236);
            this.BadgeNo_TB.Name = "BadgeNo_TB";
            this.BadgeNo_TB.Size = new System.Drawing.Size(230, 20);
            this.BadgeNo_TB.TabIndex = 15;
            // 
            // Comment_TB
            // 
            this.Comment_TB.Location = new System.Drawing.Point(16, 295);
            this.Comment_TB.Name = "Comment_TB";
            this.Comment_TB.Size = new System.Drawing.Size(386, 96);
            this.Comment_TB.TabIndex = 16;
            this.Comment_TB.Text = "";
            // 
            // backCtrlBToolStripMenuItem
            // 
            this.backCtrlBToolStripMenuItem.Name = "backCtrlBToolStripMenuItem";
            this.backCtrlBToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.backCtrlBToolStripMenuItem.Text = "Back (Ctrl + B)";
            this.backCtrlBToolStripMenuItem.Click += new System.EventHandler(this.backCtrlBToolStripMenuItem_Click);
            // 
            // DailyQCLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 476);
            this.Controls.Add(this.Comment_TB);
            this.Controls.Add(this.BadgeNo_TB);
            this.Controls.Add(this.Name_TB);
            this.Controls.Add(this.NetBeta_TB);
            this.Controls.Add(this.NetAlpha_TB);
            this.Controls.Add(this.QCType_TB);
            this.Controls.Add(this.TimeCompleted_TB);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DailyQCLog";
            this.Text = "DailyQCLog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DailyQCLog_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem functionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextCtrlNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectOrDisconnectAPortCtrlPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TimeCompleted_TB;
        private System.Windows.Forms.TextBox QCType_TB;
        private System.Windows.Forms.TextBox NetAlpha_TB;
        private System.Windows.Forms.TextBox NetBeta_TB;
        private System.Windows.Forms.TextBox Name_TB;
        private System.Windows.Forms.TextBox BadgeNo_TB;
        private System.Windows.Forms.RichTextBox Comment_TB;
        private System.Windows.Forms.ToolStripMenuItem backCtrlBToolStripMenuItem;
    }
}