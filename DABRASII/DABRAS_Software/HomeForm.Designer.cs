namespace DABRAS_Software
{
    partial class HomeForm
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
            this.DABRAS_Serial_Label = new System.Windows.Forms.Label();
            this.DABRAS_Firmware_Label = new System.Windows.Forms.Label();
            this.VCP_Status_Label = new System.Windows.Forms.Label();
            this.Parameter_Summary_Button = new System.Windows.Forms.Button();
            this.QC_Testing_Button = new System.Windows.Forms.Button();
            this.Routine_Sample_Button = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.routineSampleCountingF2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qCTestingF3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parameterSummaryF4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calibrationF5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickCalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWebBasedSurveyF12ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRSOSharepointF11ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRSOHomeF10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilityToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToAPortCtrlPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editEmbeddedLinkTargetsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editUserListCtrlKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DABRAS_Serial_Label
            // 
            this.DABRAS_Serial_Label.AutoSize = true;
            this.DABRAS_Serial_Label.Location = new System.Drawing.Point(12, 378);
            this.DABRAS_Serial_Label.Name = "DABRAS_Serial_Label";
            this.DABRAS_Serial_Label.Size = new System.Drawing.Size(149, 13);
            this.DABRAS_Serial_Label.TabIndex = 4;
            this.DABRAS_Serial_Label.Text = "Serial Number: XXXXXXXXXX";
            // 
            // DABRAS_Firmware_Label
            // 
            this.DABRAS_Firmware_Label.AutoSize = true;
            this.DABRAS_Firmware_Label.Location = new System.Drawing.Point(264, 378);
            this.DABRAS_Firmware_Label.Name = "DABRAS_Firmware_Label";
            this.DABRAS_Firmware_Label.Size = new System.Drawing.Size(88, 13);
            this.DABRAS_Firmware_Label.TabIndex = 5;
            this.DABRAS_Firmware_Label.Text = "Firmware v. X.XX";
            // 
            // VCP_Status_Label
            // 
            this.VCP_Status_Label.AutoSize = true;
            this.VCP_Status_Label.Location = new System.Drawing.Point(511, 378);
            this.VCP_Status_Label.Name = "VCP_Status_Label";
            this.VCP_Status_Label.Size = new System.Drawing.Size(122, 13);
            this.VCP_Status_Label.TabIndex = 6;
            this.VCP_Status_Label.Text = "STATUS: Disconnected";
            // 
            // Parameter_Summary_Button
            // 
            this.Parameter_Summary_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Parameter_Summary_Button.Location = new System.Drawing.Point(439, 157);
            this.Parameter_Summary_Button.Name = "Parameter_Summary_Button";
            this.Parameter_Summary_Button.Size = new System.Drawing.Size(194, 85);
            this.Parameter_Summary_Button.TabIndex = 9;
            this.Parameter_Summary_Button.Text = "Parameter Summary (F4)";
            this.Parameter_Summary_Button.UseVisualStyleBackColor = true;
            this.Parameter_Summary_Button.Visible = false;
            this.Parameter_Summary_Button.Click += new System.EventHandler(this.Parameter_Summary_Button_Click);
            // 
            // QC_Testing_Button
            // 
            this.QC_Testing_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QC_Testing_Button.Location = new System.Drawing.Point(214, 245);
            this.QC_Testing_Button.Name = "QC_Testing_Button";
            this.QC_Testing_Button.Size = new System.Drawing.Size(194, 85);
            this.QC_Testing_Button.TabIndex = 8;
            this.QC_Testing_Button.Text = "QC Testing (F3)";
            this.QC_Testing_Button.UseVisualStyleBackColor = true;
            this.QC_Testing_Button.Click += new System.EventHandler(this.QC_Testing_Button_Click);
            // 
            // Routine_Sample_Button
            // 
            this.Routine_Sample_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Routine_Sample_Button.Location = new System.Drawing.Point(214, 77);
            this.Routine_Sample_Button.Name = "Routine_Sample_Button";
            this.Routine_Sample_Button.Size = new System.Drawing.Size(194, 85);
            this.Routine_Sample_Button.TabIndex = 7;
            this.Routine_Sample_Button.Text = "Routine Sample Counting (F2)";
            this.Routine_Sample_Button.UseVisualStyleBackColor = true;
            this.Routine_Sample_Button.Click += new System.EventHandler(this.Routine_Sample_Button_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.utilityToolStripMenuItem,
            this.utilityToolStripMenuItem1,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(661, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitCtrlQToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitCtrlQToolStripMenuItem
            // 
            this.exitCtrlQToolStripMenuItem.Name = "exitCtrlQToolStripMenuItem";
            this.exitCtrlQToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.exitCtrlQToolStripMenuItem.Text = "Exit (Ctrl + Q)";
            this.exitCtrlQToolStripMenuItem.Click += new System.EventHandler(this.exitCtrlQToolStripMenuItem_Click);
            // 
            // utilityToolStripMenuItem
            // 
            this.utilityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.routineSampleCountingF2ToolStripMenuItem,
            this.qCTestingF3ToolStripMenuItem,
            this.parameterSummaryF4ToolStripMenuItem,
            this.calibrationF5ToolStripMenuItem,
            this.quickCalToolStripMenuItem,
            this.openWebBasedSurveyF12ToolStripMenuItem,
            this.openRSOSharepointF11ToolStripMenuItem,
            this.openRSOHomeF10ToolStripMenuItem});
            this.utilityToolStripMenuItem.Name = "utilityToolStripMenuItem";
            this.utilityToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.utilityToolStripMenuItem.Text = "Function";
            // 
            // routineSampleCountingF2ToolStripMenuItem
            // 
            this.routineSampleCountingF2ToolStripMenuItem.Name = "routineSampleCountingF2ToolStripMenuItem";
            this.routineSampleCountingF2ToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.routineSampleCountingF2ToolStripMenuItem.Text = "Routine Sample Counting (F2)";
            this.routineSampleCountingF2ToolStripMenuItem.Click += new System.EventHandler(this.routineSampleCountingF2ToolStripMenuItem_Click);
            // 
            // qCTestingF3ToolStripMenuItem
            // 
            this.qCTestingF3ToolStripMenuItem.Name = "qCTestingF3ToolStripMenuItem";
            this.qCTestingF3ToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.qCTestingF3ToolStripMenuItem.Text = "QC Testing (F3)";
            this.qCTestingF3ToolStripMenuItem.Click += new System.EventHandler(this.qCTestingF3ToolStripMenuItem_Click);
            // 
            // parameterSummaryF4ToolStripMenuItem
            // 
            this.parameterSummaryF4ToolStripMenuItem.Name = "parameterSummaryF4ToolStripMenuItem";
            this.parameterSummaryF4ToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.parameterSummaryF4ToolStripMenuItem.Text = "Parameter Summary (F4)";
            this.parameterSummaryF4ToolStripMenuItem.Visible = false;
            this.parameterSummaryF4ToolStripMenuItem.Click += new System.EventHandler(this.parameterSummaryF4ToolStripMenuItem_Click);
            // 
            // calibrationF5ToolStripMenuItem
            // 
            this.calibrationF5ToolStripMenuItem.Name = "calibrationF5ToolStripMenuItem";
            this.calibrationF5ToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.calibrationF5ToolStripMenuItem.Text = "Calibration (F5)";
            this.calibrationF5ToolStripMenuItem.Click += new System.EventHandler(this.calibrationF5ToolStripMenuItem_Click);
            // 
            // quickCalToolStripMenuItem
            // 
            this.quickCalToolStripMenuItem.Name = "quickCalToolStripMenuItem";
            this.quickCalToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.quickCalToolStripMenuItem.Text = "QuickCal (F6)";
            this.quickCalToolStripMenuItem.Click += new System.EventHandler(this.quickCalToolStripMenuItem_Click);
            // 
            // openWebBasedSurveyF12ToolStripMenuItem
            // 
            this.openWebBasedSurveyF12ToolStripMenuItem.Name = "openWebBasedSurveyF12ToolStripMenuItem";
            this.openWebBasedSurveyF12ToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openWebBasedSurveyF12ToolStripMenuItem.Text = "Open Web-Based Survey (F12)";
            this.openWebBasedSurveyF12ToolStripMenuItem.Click += new System.EventHandler(this.openWebBasedSurveyF12ToolStripMenuItem_Click);
            // 
            // openRSOSharepointF11ToolStripMenuItem
            // 
            this.openRSOSharepointF11ToolStripMenuItem.Name = "openRSOSharepointF11ToolStripMenuItem";
            this.openRSOSharepointF11ToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openRSOSharepointF11ToolStripMenuItem.Text = "Open RSO Sharepoint (F11)";
            this.openRSOSharepointF11ToolStripMenuItem.Click += new System.EventHandler(this.openRSOSharepointF11ToolStripMenuItem_Click);
            // 
            // openRSOHomeF10ToolStripMenuItem
            // 
            this.openRSOHomeF10ToolStripMenuItem.Name = "openRSOHomeF10ToolStripMenuItem";
            this.openRSOHomeF10ToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openRSOHomeF10ToolStripMenuItem.Text = "Open RSO Home (F10)";
            this.openRSOHomeF10ToolStripMenuItem.Click += new System.EventHandler(this.openRSOHomeF10ToolStripMenuItem_Click);
            // 
            // utilityToolStripMenuItem1
            // 
            this.utilityToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToAPortCtrlPToolStripMenuItem,
            this.editEmbeddedLinkTargetsToolStripMenuItem,
            this.editUserListCtrlKToolStripMenuItem});
            this.utilityToolStripMenuItem1.Name = "utilityToolStripMenuItem1";
            this.utilityToolStripMenuItem1.Size = new System.Drawing.Size(50, 20);
            this.utilityToolStripMenuItem1.Text = "Utility";
            // 
            // connectToAPortCtrlPToolStripMenuItem
            // 
            this.connectToAPortCtrlPToolStripMenuItem.Name = "connectToAPortCtrlPToolStripMenuItem";
            this.connectToAPortCtrlPToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.connectToAPortCtrlPToolStripMenuItem.Text = "Connect to a Port (Ctrl + P)";
            this.connectToAPortCtrlPToolStripMenuItem.Click += new System.EventHandler(this.connectToAPortCtrlPToolStripMenuItem_Click);
            // 
            // editEmbeddedLinkTargetsToolStripMenuItem
            // 
            this.editEmbeddedLinkTargetsToolStripMenuItem.Name = "editEmbeddedLinkTargetsToolStripMenuItem";
            this.editEmbeddedLinkTargetsToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.editEmbeddedLinkTargetsToolStripMenuItem.Text = "Edit Embedded Link Targets...";
            this.editEmbeddedLinkTargetsToolStripMenuItem.Click += new System.EventHandler(this.editEmbeddedLinkTargetsToolStripMenuItem_Click);
            // 
            // editUserListCtrlKToolStripMenuItem
            // 
            this.editUserListCtrlKToolStripMenuItem.Name = "editUserListCtrlKToolStripMenuItem";
            this.editUserListCtrlKToolStripMenuItem.Size = new System.Drawing.Size(230, 22);
            this.editUserListCtrlKToolStripMenuItem.Text = "Edit User List (Ctrl + K)";
            this.editUserListCtrlKToolStripMenuItem.Click += new System.EventHandler(this.editUserListCtrlKToolStripMenuItem_Click);
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
            // HomeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 407);
            this.Controls.Add(this.Parameter_Summary_Button);
            this.Controls.Add(this.QC_Testing_Button);
            this.Controls.Add(this.Routine_Sample_Button);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.VCP_Status_Label);
            this.Controls.Add(this.DABRAS_Firmware_Label);
            this.Controls.Add(this.DABRAS_Serial_Label);
            this.Name = "HomeForm";
            this.Text = "HomeForm";
            this.Activated += new System.EventHandler(this.HomeForm_Activated);
            this.VisibleChanged += new System.EventHandler(this.HomeForm_VisibleChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label DABRAS_Serial_Label;
        private System.Windows.Forms.Label DABRAS_Firmware_Label;
        private System.Windows.Forms.Label VCP_Status_Label;
        private System.Windows.Forms.Button Parameter_Summary_Button;
        private System.Windows.Forms.Button QC_Testing_Button;
        private System.Windows.Forms.Button Routine_Sample_Button;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem routineSampleCountingF2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qCTestingF3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parameterSummaryF4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calibrationF5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilityToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem connectToAPortCtrlPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWebBasedSurveyF12ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRSOSharepointF11ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRSOHomeF10ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editEmbeddedLinkTargetsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickCalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editUserListCtrlKToolStripMenuItem;

    }
}

