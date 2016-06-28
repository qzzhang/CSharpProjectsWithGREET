namespace DABRAS_Software
{
    partial class QCMain
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dailyBackgroundCheckCtrlBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dailyAm241SourceCheckCtrlAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dailySr90SourceCheckCtrlSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeDailyQCResultsToFileCtrlWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.performChiSquaredTestCtrlXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewControlGraphsF9ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewBetaEfficiencyGraphsF8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWebBasedSurveySystemF12ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRSOSharepointF11ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRSOHomeF10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectDisconnectToAPortCtrlPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automationControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCalibrationDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Background_Button = new System.Windows.Forms.Button();
            this.AlphaButton = new System.Windows.Forms.Button();
            this.BetaButton = new System.Windows.Forms.Button();
            this.VCP_Status_Label = new System.Windows.Forms.Label();
            this.DABRAS_Firmware_Label = new System.Windows.Forms.Label();
            this.DABRAS_Serial_Label = new System.Windows.Forms.Label();
            this.DummyButton = new System.Windows.Forms.Button();
            this.ForceClearButton = new System.Windows.Forms.Button();
            this.WriteToFileButton = new System.Windows.Forms.Button();
            this.ChiSquaredButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(667, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
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
            this.dailyBackgroundCheckCtrlBToolStripMenuItem,
            this.dailyAm241SourceCheckCtrlAToolStripMenuItem,
            this.dailySr90SourceCheckCtrlSToolStripMenuItem,
            this.writeDailyQCResultsToFileCtrlWToolStripMenuItem,
            this.performChiSquaredTestCtrlXToolStripMenuItem,
            this.viewControlGraphsF9ToolStripMenuItem,
            this.viewBetaEfficiencyGraphsF8ToolStripMenuItem,
            this.openWebBasedSurveySystemF12ToolStripMenuItem,
            this.openRSOSharepointF11ToolStripMenuItem,
            this.openRSOHomeF10ToolStripMenuItem});
            this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
            this.functionToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.functionToolStripMenuItem.Text = "Function";
            // 
            // dailyBackgroundCheckCtrlBToolStripMenuItem
            // 
            this.dailyBackgroundCheckCtrlBToolStripMenuItem.Name = "dailyBackgroundCheckCtrlBToolStripMenuItem";
            this.dailyBackgroundCheckCtrlBToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.dailyBackgroundCheckCtrlBToolStripMenuItem.Text = "Daily Background Check (Ctrl + B)";
            this.dailyBackgroundCheckCtrlBToolStripMenuItem.Click += new System.EventHandler(this.dailyBackgroundCheckCtrlBToolStripMenuItem_Click);
            // 
            // dailyAm241SourceCheckCtrlAToolStripMenuItem
            // 
            this.dailyAm241SourceCheckCtrlAToolStripMenuItem.Name = "dailyAm241SourceCheckCtrlAToolStripMenuItem";
            this.dailyAm241SourceCheckCtrlAToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.dailyAm241SourceCheckCtrlAToolStripMenuItem.Text = "Daily Am-241 Source Check (Ctrl + A)";
            this.dailyAm241SourceCheckCtrlAToolStripMenuItem.Click += new System.EventHandler(this.dailyAm241SourceCheckCtrlAToolStripMenuItem_Click);
            // 
            // dailySr90SourceCheckCtrlSToolStripMenuItem
            // 
            this.dailySr90SourceCheckCtrlSToolStripMenuItem.Name = "dailySr90SourceCheckCtrlSToolStripMenuItem";
            this.dailySr90SourceCheckCtrlSToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.dailySr90SourceCheckCtrlSToolStripMenuItem.Text = "Daily Sr-90 Source Check (Ctrl + S)";
            this.dailySr90SourceCheckCtrlSToolStripMenuItem.Click += new System.EventHandler(this.dailySr90SourceCheckCtrlSToolStripMenuItem_Click);
            // 
            // writeDailyQCResultsToFileCtrlWToolStripMenuItem
            // 
            this.writeDailyQCResultsToFileCtrlWToolStripMenuItem.Name = "writeDailyQCResultsToFileCtrlWToolStripMenuItem";
            this.writeDailyQCResultsToFileCtrlWToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.writeDailyQCResultsToFileCtrlWToolStripMenuItem.Text = "Write Daily QC Results to File (Ctrl + W)";
            this.writeDailyQCResultsToFileCtrlWToolStripMenuItem.Click += new System.EventHandler(this.writeDailyQCResultsToFileCtrlWToolStripMenuItem_Click);
            // 
            // performChiSquaredTestCtrlXToolStripMenuItem
            // 
            this.performChiSquaredTestCtrlXToolStripMenuItem.Name = "performChiSquaredTestCtrlXToolStripMenuItem";
            this.performChiSquaredTestCtrlXToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.performChiSquaredTestCtrlXToolStripMenuItem.Text = "Perform Chi Squared Test (Ctrl + X)";
            this.performChiSquaredTestCtrlXToolStripMenuItem.Click += new System.EventHandler(this.performChiSquaredTestCtrlXToolStripMenuItem_Click);
            // 
            // viewControlGraphsF9ToolStripMenuItem
            // 
            this.viewControlGraphsF9ToolStripMenuItem.Name = "viewControlGraphsF9ToolStripMenuItem";
            this.viewControlGraphsF9ToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.viewControlGraphsF9ToolStripMenuItem.Text = "View Control Charts (F9)";
            this.viewControlGraphsF9ToolStripMenuItem.Click += new System.EventHandler(this.viewControlGraphsF9ToolStripMenuItem_Click);
            // 
            // viewBetaEfficiencyGraphsF8ToolStripMenuItem
            // 
            this.viewBetaEfficiencyGraphsF8ToolStripMenuItem.Name = "viewBetaEfficiencyGraphsF8ToolStripMenuItem";
            this.viewBetaEfficiencyGraphsF8ToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.viewBetaEfficiencyGraphsF8ToolStripMenuItem.Text = "View Beta Efficiency Graphs (F8)";
            this.viewBetaEfficiencyGraphsF8ToolStripMenuItem.Click += new System.EventHandler(this.viewBetaEfficiencyGraphsF8ToolStripMenuItem_Click);
            // 
            // openWebBasedSurveySystemF12ToolStripMenuItem
            // 
            this.openWebBasedSurveySystemF12ToolStripMenuItem.Name = "openWebBasedSurveySystemF12ToolStripMenuItem";
            this.openWebBasedSurveySystemF12ToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.openWebBasedSurveySystemF12ToolStripMenuItem.Text = "Open Web-Based Survey System (F12)";
            this.openWebBasedSurveySystemF12ToolStripMenuItem.Click += new System.EventHandler(this.openWebBasedSurveySystemF12ToolStripMenuItem_Click);
            // 
            // openRSOSharepointF11ToolStripMenuItem
            // 
            this.openRSOSharepointF11ToolStripMenuItem.Name = "openRSOSharepointF11ToolStripMenuItem";
            this.openRSOSharepointF11ToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.openRSOSharepointF11ToolStripMenuItem.Text = "Open RSO Sharepoint (F11)";
            this.openRSOSharepointF11ToolStripMenuItem.Click += new System.EventHandler(this.openRSOSharepointF11ToolStripMenuItem_Click);
            // 
            // openRSOHomeF10ToolStripMenuItem
            // 
            this.openRSOHomeF10ToolStripMenuItem.Name = "openRSOHomeF10ToolStripMenuItem";
            this.openRSOHomeF10ToolStripMenuItem.Size = new System.Drawing.Size(281, 22);
            this.openRSOHomeF10ToolStripMenuItem.Text = "Open RSO Home (F10)";
            this.openRSOHomeF10ToolStripMenuItem.Click += new System.EventHandler(this.openRSOHomeF10ToolStripMenuItem_Click);
            // 
            // utilityToolStripMenuItem
            // 
            this.utilityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectDisconnectToAPortCtrlPToolStripMenuItem,
            this.automationControlsToolStripMenuItem,
            this.showCalibrationDetailsToolStripMenuItem});
            this.utilityToolStripMenuItem.Name = "utilityToolStripMenuItem";
            this.utilityToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.utilityToolStripMenuItem.Text = "Utility";
            // 
            // connectDisconnectToAPortCtrlPToolStripMenuItem
            // 
            this.connectDisconnectToAPortCtrlPToolStripMenuItem.Name = "connectDisconnectToAPortCtrlPToolStripMenuItem";
            this.connectDisconnectToAPortCtrlPToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.connectDisconnectToAPortCtrlPToolStripMenuItem.Text = "Connect / Disconnect to a Port (Ctrl + P)";
            this.connectDisconnectToAPortCtrlPToolStripMenuItem.Click += new System.EventHandler(this.connectDisconnectToAPortCtrlPToolStripMenuItem_Click);
            // 
            // automationControlsToolStripMenuItem
            // 
            this.automationControlsToolStripMenuItem.Name = "automationControlsToolStripMenuItem";
            this.automationControlsToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.automationControlsToolStripMenuItem.Text = "Automation Controls";
            this.automationControlsToolStripMenuItem.Click += new System.EventHandler(this.automationControlsToolStripMenuItem_Click);
            // 
            // showCalibrationDetailsToolStripMenuItem
            // 
            this.showCalibrationDetailsToolStripMenuItem.Name = "showCalibrationDetailsToolStripMenuItem";
            this.showCalibrationDetailsToolStripMenuItem.Size = new System.Drawing.Size(288, 22);
            this.showCalibrationDetailsToolStripMenuItem.Text = "Show Calibration Details";
            this.showCalibrationDetailsToolStripMenuItem.Click += new System.EventHandler(this.showCalibrationDetailsToolStripMenuItem_Click);
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
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // Background_Button
            // 
            this.Background_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Background_Button.Location = new System.Drawing.Point(64, 57);
            this.Background_Button.Name = "Background_Button";
            this.Background_Button.Size = new System.Drawing.Size(197, 61);
            this.Background_Button.TabIndex = 1;
            this.Background_Button.Text = "Daily Background Check (Ctrl + B)";
            this.Background_Button.UseVisualStyleBackColor = true;
            this.Background_Button.Click += new System.EventHandler(this.Background_Button_Click);
            // 
            // AlphaButton
            // 
            this.AlphaButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AlphaButton.Location = new System.Drawing.Point(333, 57);
            this.AlphaButton.Name = "AlphaButton";
            this.AlphaButton.Size = new System.Drawing.Size(197, 61);
            this.AlphaButton.TabIndex = 2;
            this.AlphaButton.Text = "Daily Am-241 Source Check (Ctrl + A)";
            this.AlphaButton.UseVisualStyleBackColor = true;
            this.AlphaButton.Click += new System.EventHandler(this.AlphaButton_Click);
            // 
            // BetaButton
            // 
            this.BetaButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BetaButton.Location = new System.Drawing.Point(64, 166);
            this.BetaButton.Name = "BetaButton";
            this.BetaButton.Size = new System.Drawing.Size(197, 61);
            this.BetaButton.TabIndex = 3;
            this.BetaButton.Text = "Daily Sr-90  Source Check (Ctrl + S)";
            this.BetaButton.UseVisualStyleBackColor = true;
            this.BetaButton.Click += new System.EventHandler(this.BetaButton_Click);
            // 
            // VCP_Status_Label
            // 
            this.VCP_Status_Label.AutoSize = true;
            this.VCP_Status_Label.Location = new System.Drawing.Point(527, 421);
            this.VCP_Status_Label.Name = "VCP_Status_Label";
            this.VCP_Status_Label.Size = new System.Drawing.Size(122, 13);
            this.VCP_Status_Label.TabIndex = 9;
            this.VCP_Status_Label.Text = "STATUS: Disconnected";
            // 
            // DABRAS_Firmware_Label
            // 
            this.DABRAS_Firmware_Label.AutoSize = true;
            this.DABRAS_Firmware_Label.Location = new System.Drawing.Point(292, 421);
            this.DABRAS_Firmware_Label.Name = "DABRAS_Firmware_Label";
            this.DABRAS_Firmware_Label.Size = new System.Drawing.Size(88, 13);
            this.DABRAS_Firmware_Label.TabIndex = 8;
            this.DABRAS_Firmware_Label.Text = "Firmware v. X.XX";
            // 
            // DABRAS_Serial_Label
            // 
            this.DABRAS_Serial_Label.AutoSize = true;
            this.DABRAS_Serial_Label.Location = new System.Drawing.Point(12, 421);
            this.DABRAS_Serial_Label.Name = "DABRAS_Serial_Label";
            this.DABRAS_Serial_Label.Size = new System.Drawing.Size(149, 13);
            this.DABRAS_Serial_Label.TabIndex = 7;
            this.DABRAS_Serial_Label.Text = "Serial Number: XXXXXXXXXX";
            // 
            // DummyButton
            // 
            this.DummyButton.Location = new System.Drawing.Point(333, 277);
            this.DummyButton.Name = "DummyButton";
            this.DummyButton.Size = new System.Drawing.Size(75, 61);
            this.DummyButton.TabIndex = 10;
            this.DummyButton.Text = "Make Dummy QC List";
            this.DummyButton.UseVisualStyleBackColor = true;
            this.DummyButton.Visible = false;
            this.DummyButton.Click += new System.EventHandler(this.DummyButton_Click);
            // 
            // ForceClearButton
            // 
            this.ForceClearButton.Location = new System.Drawing.Point(449, 277);
            this.ForceClearButton.Name = "ForceClearButton";
            this.ForceClearButton.Size = new System.Drawing.Size(75, 61);
            this.ForceClearButton.TabIndex = 11;
            this.ForceClearButton.Text = "Force Clear List";
            this.ForceClearButton.UseVisualStyleBackColor = true;
            this.ForceClearButton.Visible = false;
            this.ForceClearButton.Click += new System.EventHandler(this.ForceClearButton_Click);
            // 
            // WriteToFileButton
            // 
            this.WriteToFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WriteToFileButton.Location = new System.Drawing.Point(333, 166);
            this.WriteToFileButton.Name = "WriteToFileButton";
            this.WriteToFileButton.Size = new System.Drawing.Size(197, 61);
            this.WriteToFileButton.TabIndex = 12;
            this.WriteToFileButton.Text = "Write Daily QC Results to file (Ctrl + W)";
            this.WriteToFileButton.UseVisualStyleBackColor = true;
            this.WriteToFileButton.Click += new System.EventHandler(this.WriteToFileButton_Click);
            // 
            // ChiSquaredButton
            // 
            this.ChiSquaredButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChiSquaredButton.Location = new System.Drawing.Point(64, 276);
            this.ChiSquaredButton.Name = "ChiSquaredButton";
            this.ChiSquaredButton.Size = new System.Drawing.Size(197, 61);
            this.ChiSquaredButton.TabIndex = 13;
            this.ChiSquaredButton.Text = "Perform Chi Squared Test (Ctrl + X)";
            this.ChiSquaredButton.UseVisualStyleBackColor = true;
            this.ChiSquaredButton.Click += new System.EventHandler(this.ChiSquaredButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(551, 287);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // QCMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 443);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ChiSquaredButton);
            this.Controls.Add(this.WriteToFileButton);
            this.Controls.Add(this.ForceClearButton);
            this.Controls.Add(this.DummyButton);
            this.Controls.Add(this.VCP_Status_Label);
            this.Controls.Add(this.DABRAS_Firmware_Label);
            this.Controls.Add(this.DABRAS_Serial_Label);
            this.Controls.Add(this.BetaButton);
            this.Controls.Add(this.AlphaButton);
            this.Controls.Add(this.Background_Button);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "QCMain";
            this.Text = "QCMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.QCMain_FormClosing);
            this.Shown += new System.EventHandler(this.QCMain_Shown);
            this.VisibleChanged += new System.EventHandler(this.QCMain_VisibleChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem functionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dailyBackgroundCheckCtrlBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dailyAm241SourceCheckCtrlAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dailySr90SourceCheckCtrlSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectDisconnectToAPortCtrlPToolStripMenuItem;
        private System.Windows.Forms.Button Background_Button;
        private System.Windows.Forms.Button AlphaButton;
        private System.Windows.Forms.Button BetaButton;
        private System.Windows.Forms.Label VCP_Status_Label;
        private System.Windows.Forms.Label DABRAS_Firmware_Label;
        private System.Windows.Forms.Label DABRAS_Serial_Label;
        private System.Windows.Forms.ToolStripMenuItem automationControlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWebBasedSurveySystemF12ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRSOSharepointF11ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRSOHomeF10ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewControlGraphsF9ToolStripMenuItem;
        private System.Windows.Forms.Button DummyButton;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button ForceClearButton;
        private System.Windows.Forms.Button WriteToFileButton;
        private System.Windows.Forms.Button ChiSquaredButton;
        private System.Windows.Forms.ToolStripMenuItem viewBetaEfficiencyGraphsF8ToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem writeDailyQCResultsToFileCtrlWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem performChiSquaredTestCtrlXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showCalibrationDetailsToolStripMenuItem;
    }
}