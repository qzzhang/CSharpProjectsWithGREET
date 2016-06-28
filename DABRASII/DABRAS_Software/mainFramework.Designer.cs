namespace DABRAS_Software
{
    partial class mainFramework
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
            this.mainTab = new System.Windows.Forms.TabControl();
            this.tpCalib = new System.Windows.Forms.TabPage();
            this.tpQCT = new System.Windows.Forms.TabPage();
            this.tpRSC = new System.Windows.Forms.TabPage();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectDisconnectPortCtrlPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setBackgroundTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DABRAS_Status_Label = new System.Windows.Forms.Label();
            this.DABRAS_Firmware_Label = new System.Windows.Forms.Label();
            this.DABRAS_Serial_Label = new System.Windows.Forms.Label();
            this.mainTab.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTab
            // 
            this.mainTab.Controls.Add(this.tpQCT);
            this.mainTab.Controls.Add(this.tpRSC);
            this.mainTab.Controls.Add(this.tpCalib);
            this.mainTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTab.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainTab.ItemSize = new System.Drawing.Size(188, 33);
            this.mainTab.Location = new System.Drawing.Point(0, 25);
            this.mainTab.Margin = new System.Windows.Forms.Padding(4);
            this.mainTab.Multiline = true;
            this.mainTab.Name = "mainTab";
            this.mainTab.SelectedIndex = 0;
            this.mainTab.Size = new System.Drawing.Size(1276, 749);
            this.mainTab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.mainTab.TabIndex = 0;
            this.mainTab.SelectedIndexChanged += new System.EventHandler(this.mainTab_SelectedIndexChanged);
            // 
            // tpCalib
            // 
            this.tpCalib.BackColor = System.Drawing.Color.PaleTurquoise;
            this.tpCalib.ForeColor = System.Drawing.Color.Maroon;
            this.tpCalib.Location = new System.Drawing.Point(4, 37);
            this.tpCalib.Margin = new System.Windows.Forms.Padding(4);
            this.tpCalib.Name = "tpCalib";
            this.tpCalib.Size = new System.Drawing.Size(1268, 708);
            this.tpCalib.TabIndex = 2;
            this.tpCalib.Text = "Calibration";
            // 
            // tpQCT
            // 
            this.tpQCT.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tpQCT.Location = new System.Drawing.Point(4, 37);
            this.tpQCT.Margin = new System.Windows.Forms.Padding(4);
            this.tpQCT.Name = "tpQCT";
            this.tpQCT.Size = new System.Drawing.Size(1268, 708);
            this.tpQCT.TabIndex = 1;
            this.tpQCT.Text = "QC Testing";
            // 
            // tpRSC
            // 
            this.tpRSC.BackColor = System.Drawing.Color.PowderBlue;
            this.tpRSC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tpRSC.Location = new System.Drawing.Point(4, 37);
            this.tpRSC.Margin = new System.Windows.Forms.Padding(4);
            this.tpRSC.Name = "tpRSC";
            this.tpRSC.Size = new System.Drawing.Size(1268, 708);
            this.tpRSC.TabIndex = 0;
            this.tpRSC.Text = "Routine Sample Counting ";
            // 
            // mainMenu
            // 
            this.mainMenu.AllowItemReorder = true;
            this.mainMenu.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.utilityToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(9, 2, 0, 2);
            this.mainMenu.Size = new System.Drawing.Size(1276, 25);
            this.mainMenu.TabIndex = 106;
            this.mainMenu.Text = "mainMenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeCtrlQToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeCtrlQToolStripMenuItem
            // 
            this.closeCtrlQToolStripMenuItem.Name = "closeCtrlQToolStripMenuItem";
            this.closeCtrlQToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.closeCtrlQToolStripMenuItem.Text = "Exit";
            this.closeCtrlQToolStripMenuItem.Click += new System.EventHandler(this.closeCtrlQToolStripMenuItem_Click);
            // 
            // utilityToolStripMenuItem
            // 
            this.utilityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectDisconnectPortCtrlPToolStripMenuItem,
            this.setBackgroundTypeToolStripMenuItem});
            this.utilityToolStripMenuItem.Name = "utilityToolStripMenuItem";
            this.utilityToolStripMenuItem.Size = new System.Drawing.Size(52, 21);
            this.utilityToolStripMenuItem.Text = "Utility";
            // 
            // connectDisconnectPortCtrlPToolStripMenuItem
            // 
            this.connectDisconnectPortCtrlPToolStripMenuItem.Name = "connectDisconnectPortCtrlPToolStripMenuItem";
            this.connectDisconnectPortCtrlPToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.connectDisconnectPortCtrlPToolStripMenuItem.Text = "Connect / Disconnect Port";
            this.connectDisconnectPortCtrlPToolStripMenuItem.Click += new System.EventHandler(this.connectDisconnectPortCtrlPToolStripMenuItem_Click);
            // 
            // setBackgroundTypeToolStripMenuItem
            // 
            this.setBackgroundTypeToolStripMenuItem.Name = "setBackgroundTypeToolStripMenuItem";
            this.setBackgroundTypeToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.setBackgroundTypeToolStripMenuItem.Text = "Set Background Type";
            this.setBackgroundTypeToolStripMenuItem.Click += new System.EventHandler(this.setBackgroundTypeToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // DABRAS_Status_Label
            // 
            this.DABRAS_Status_Label.AutoSize = true;
            this.DABRAS_Status_Label.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.DABRAS_Status_Label.Location = new System.Drawing.Point(836, 5);
            this.DABRAS_Status_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DABRAS_Status_Label.Name = "DABRAS_Status_Label";
            this.DABRAS_Status_Label.Size = new System.Drawing.Size(172, 16);
            this.DABRAS_Status_Label.TabIndex = 109;
            this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
            // 
            // DABRAS_Firmware_Label
            // 
            this.DABRAS_Firmware_Label.AutoSize = true;
            this.DABRAS_Firmware_Label.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.DABRAS_Firmware_Label.Location = new System.Drawing.Point(579, 5);
            this.DABRAS_Firmware_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DABRAS_Firmware_Label.Name = "DABRAS_Firmware_Label";
            this.DABRAS_Firmware_Label.Size = new System.Drawing.Size(122, 16);
            this.DABRAS_Firmware_Label.TabIndex = 108;
            this.DABRAS_Firmware_Label.Text = "Firmware v. X.XX";
            // 
            // DABRAS_Serial_Label
            // 
            this.DABRAS_Serial_Label.AutoSize = true;
            this.DABRAS_Serial_Label.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.DABRAS_Serial_Label.Location = new System.Drawing.Point(257, 5);
            this.DABRAS_Serial_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DABRAS_Serial_Label.Name = "DABRAS_Serial_Label";
            this.DABRAS_Serial_Label.Size = new System.Drawing.Size(205, 16);
            this.DABRAS_Serial_Label.TabIndex = 107;
            this.DABRAS_Serial_Label.Text = "Serial Number: XXXXXXXXXX";
            // 
            // mainFramework
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1276, 774);
            this.Controls.Add(this.DABRAS_Status_Label);
            this.Controls.Add(this.DABRAS_Firmware_Label);
            this.Controls.Add(this.DABRAS_Serial_Label);
            this.Controls.Add(this.mainTab);
            this.Controls.Add(this.mainMenu);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "mainFramework";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DABARS Main Framework";
            this.mainTab.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl mainTab;
        private System.Windows.Forms.TabPage tpRSC;
        private System.Windows.Forms.TabPage tpCalib;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectDisconnectPortCtrlPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setBackgroundTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TabPage tpQCT;
        private System.Windows.Forms.Label DABRAS_Status_Label;
        private System.Windows.Forms.Label DABRAS_Firmware_Label;
        private System.Windows.Forms.Label DABRAS_Serial_Label;
    }
}