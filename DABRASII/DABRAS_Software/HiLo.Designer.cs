namespace DABRAS_Software
{
    partial class HiLo
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
            this.closeWindowCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeResultsToFileCtrlVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageCtrlIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aquireCtrlAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopAquiringCtrlSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWebBasedSurveyF12ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRSOSharepointF11ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRSOHomeF10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.TypeOfHiLoLabel = new System.Windows.Forms.Label();
            this.CollectLabel = new System.Windows.Forms.Label();
            this.Num_Counts_TB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Min_TB = new System.Windows.Forms.TextBox();
            this.Colon_Label = new System.Windows.Forms.Label();
            this.Sec_TB = new System.Windows.Forms.TextBox();
            this.MinLabel = new System.Windows.Forms.Label();
            this.SecLabel = new System.Windows.Forms.Label();
            this.AquireButton = new System.Windows.Forms.Button();
            this.StdDevButton = new System.Windows.Forms.RadioButton();
            this.PercentButton = new System.Windows.Forms.RadioButton();
            this.StdDev_TB = new System.Windows.Forms.TextBox();
            this.Percent_TB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.HiLo_Results_DataGridView = new System.Windows.Forms.DataGridView();
            this.Net_CPM_Label = new System.Windows.Forms.Label();
            this.LL_Beta_Label = new System.Windows.Forms.Label();
            this.LL_Alpha_Label = new System.Windows.Forms.Label();
            this.LL_Alpha_TB = new System.Windows.Forms.TextBox();
            this.LL_Beta_TB = new System.Windows.Forms.TextBox();
            this.HL_Beta_TB = new System.Windows.Forms.TextBox();
            this.HL_Alpha_TB = new System.Windows.Forms.TextBox();
            this.HL_Alpha_Label = new System.Windows.Forms.Label();
            this.HL_Beta_Label = new System.Windows.Forms.Label();
            this.Net_CPM_Label2 = new System.Windows.Forms.Label();
            this.Save_Limits_Button = new System.Windows.Forms.Button();
            this.Override_CB = new System.Windows.Forms.CheckBox();
            this.DABRAS_Status_Label = new System.Windows.Forms.Label();
            this.DABRAS_Firmware_Label = new System.Windows.Forms.Label();
            this.DABRAS_SN_Label = new System.Windows.Forms.Label();
            this.StopButton = new System.Windows.Forms.Button();
            this.Recompute_Limits_Button = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ImageSaveButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HiLo_Results_DataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.functionToolStripMenuItem,
            this.utilityToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(997, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeWindowCtrlQToolStripMenuItem,
            this.writeResultsToFileCtrlVToolStripMenuItem,
            this.saveImageCtrlIToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeWindowCtrlQToolStripMenuItem
            // 
            this.closeWindowCtrlQToolStripMenuItem.Name = "closeWindowCtrlQToolStripMenuItem";
            this.closeWindowCtrlQToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.closeWindowCtrlQToolStripMenuItem.Text = "Close Window (Ctrl + Q)";
            this.closeWindowCtrlQToolStripMenuItem.Click += new System.EventHandler(this.closeWindowCtrlQToolStripMenuItem_Click);
            // 
            // writeResultsToFileCtrlVToolStripMenuItem
            // 
            this.writeResultsToFileCtrlVToolStripMenuItem.Name = "writeResultsToFileCtrlVToolStripMenuItem";
            this.writeResultsToFileCtrlVToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.writeResultsToFileCtrlVToolStripMenuItem.Text = "Write Results to File (Ctrl + V)";
            this.writeResultsToFileCtrlVToolStripMenuItem.Click += new System.EventHandler(this.writeResultsToFileCtrlVToolStripMenuItem_Click);
            // 
            // saveImageCtrlIToolStripMenuItem
            // 
            this.saveImageCtrlIToolStripMenuItem.Name = "saveImageCtrlIToolStripMenuItem";
            this.saveImageCtrlIToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.saveImageCtrlIToolStripMenuItem.Text = "Save Image (Ctrl + I)";
            this.saveImageCtrlIToolStripMenuItem.Click += new System.EventHandler(this.saveImageCtrlIToolStripMenuItem_Click);
            // 
            // functionToolStripMenuItem
            // 
            this.functionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aquireCtrlAToolStripMenuItem,
            this.stopAquiringCtrlSToolStripMenuItem,
            this.openWebBasedSurveyF12ToolStripMenuItem,
            this.openRSOSharepointF11ToolStripMenuItem,
            this.openRSOHomeF10ToolStripMenuItem});
            this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
            this.functionToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.functionToolStripMenuItem.Text = "Function";
            // 
            // aquireCtrlAToolStripMenuItem
            // 
            this.aquireCtrlAToolStripMenuItem.Name = "aquireCtrlAToolStripMenuItem";
            this.aquireCtrlAToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.aquireCtrlAToolStripMenuItem.Text = "Aquire (Ctrl + A)";
            this.aquireCtrlAToolStripMenuItem.Click += new System.EventHandler(this.aquireCtrlAToolStripMenuItem_Click);
            // 
            // stopAquiringCtrlSToolStripMenuItem
            // 
            this.stopAquiringCtrlSToolStripMenuItem.Enabled = false;
            this.stopAquiringCtrlSToolStripMenuItem.Name = "stopAquiringCtrlSToolStripMenuItem";
            this.stopAquiringCtrlSToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.stopAquiringCtrlSToolStripMenuItem.Text = "Stop Aquiring (Ctrl + S)";
            this.stopAquiringCtrlSToolStripMenuItem.Click += new System.EventHandler(this.stopAquiringCtrlSToolStripMenuItem_Click);
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
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.aboutToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // TypeOfHiLoLabel
            // 
            this.TypeOfHiLoLabel.AutoSize = true;
            this.TypeOfHiLoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TypeOfHiLoLabel.Location = new System.Drawing.Point(12, 40);
            this.TypeOfHiLoLabel.Name = "TypeOfHiLoLabel";
            this.TypeOfHiLoLabel.Size = new System.Drawing.Size(93, 20);
            this.TypeOfHiLoLabel.TabIndex = 1;
            this.TypeOfHiLoLabel.Text = "TypeOfHiLo";
            // 
            // CollectLabel
            // 
            this.CollectLabel.AutoSize = true;
            this.CollectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CollectLabel.Location = new System.Drawing.Point(12, 73);
            this.CollectLabel.Name = "CollectLabel";
            this.CollectLabel.Size = new System.Drawing.Size(61, 20);
            this.CollectLabel.TabIndex = 2;
            this.CollectLabel.Text = "Collect ";
            // 
            // Num_Counts_TB
            // 
            this.Num_Counts_TB.Location = new System.Drawing.Point(79, 75);
            this.Num_Counts_TB.Name = "Num_Counts_TB";
            this.Num_Counts_TB.Size = new System.Drawing.Size(53, 20);
            this.Num_Counts_TB.TabIndex = 3;
            this.Num_Counts_TB.Text = "10";
            this.Num_Counts_TB.TextChanged += new System.EventHandler(this.Num_Counts_TB_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(138, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Counts of ";
            // 
            // Min_TB
            // 
            this.Min_TB.Location = new System.Drawing.Point(226, 75);
            this.Min_TB.Name = "Min_TB";
            this.Min_TB.Size = new System.Drawing.Size(38, 20);
            this.Min_TB.TabIndex = 5;
            this.Min_TB.Text = "1";
            // 
            // Colon_Label
            // 
            this.Colon_Label.AutoSize = true;
            this.Colon_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Colon_Label.Location = new System.Drawing.Point(270, 75);
            this.Colon_Label.Name = "Colon_Label";
            this.Colon_Label.Size = new System.Drawing.Size(13, 20);
            this.Colon_Label.TabIndex = 6;
            this.Colon_Label.Text = ":";
            // 
            // Sec_TB
            // 
            this.Sec_TB.Location = new System.Drawing.Point(289, 75);
            this.Sec_TB.Name = "Sec_TB";
            this.Sec_TB.Size = new System.Drawing.Size(38, 20);
            this.Sec_TB.TabIndex = 7;
            this.Sec_TB.Text = "0";
            // 
            // MinLabel
            // 
            this.MinLabel.AutoSize = true;
            this.MinLabel.Location = new System.Drawing.Point(223, 59);
            this.MinLabel.Name = "MinLabel";
            this.MinLabel.Size = new System.Drawing.Size(23, 13);
            this.MinLabel.TabIndex = 8;
            this.MinLabel.Text = "min";
            // 
            // SecLabel
            // 
            this.SecLabel.AutoSize = true;
            this.SecLabel.Location = new System.Drawing.Point(286, 59);
            this.SecLabel.Name = "SecLabel";
            this.SecLabel.Size = new System.Drawing.Size(24, 13);
            this.SecLabel.TabIndex = 9;
            this.SecLabel.Text = "sec";
            // 
            // AquireButton
            // 
            this.AquireButton.Location = new System.Drawing.Point(334, 49);
            this.AquireButton.Name = "AquireButton";
            this.AquireButton.Size = new System.Drawing.Size(99, 23);
            this.AquireButton.TabIndex = 10;
            this.AquireButton.Text = "Aquire (Ctrl + A)";
            this.AquireButton.UseVisualStyleBackColor = true;
            this.AquireButton.Click += new System.EventHandler(this.AquireButton_Click);
            // 
            // StdDevButton
            // 
            this.StdDevButton.AutoSize = true;
            this.StdDevButton.Checked = true;
            this.StdDevButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StdDevButton.Location = new System.Drawing.Point(450, 59);
            this.StdDevButton.Name = "StdDevButton";
            this.StdDevButton.Size = new System.Drawing.Size(166, 24);
            this.StdDevButton.TabIndex = 11;
            this.StdDevButton.TabStop = true;
            this.StdDevButton.Text = "Calculate limits as ±";
            this.StdDevButton.UseVisualStyleBackColor = true;
            this.StdDevButton.CheckedChanged += new System.EventHandler(this.StdDevButton_CheckedChanged);
            // 
            // PercentButton
            // 
            this.PercentButton.AutoSize = true;
            this.PercentButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PercentButton.Location = new System.Drawing.Point(450, 82);
            this.PercentButton.Name = "PercentButton";
            this.PercentButton.Size = new System.Drawing.Size(166, 24);
            this.PercentButton.TabIndex = 12;
            this.PercentButton.TabStop = true;
            this.PercentButton.Text = "Calculate limits as ±";
            this.PercentButton.UseVisualStyleBackColor = true;
            this.PercentButton.CheckedChanged += new System.EventHandler(this.PercentButton_CheckedChanged);
            // 
            // StdDev_TB
            // 
            this.StdDev_TB.Location = new System.Drawing.Point(622, 61);
            this.StdDev_TB.Name = "StdDev_TB";
            this.StdDev_TB.Size = new System.Drawing.Size(52, 20);
            this.StdDev_TB.TabIndex = 13;
            this.StdDev_TB.Text = "3";
            // 
            // Percent_TB
            // 
            this.Percent_TB.Location = new System.Drawing.Point(622, 86);
            this.Percent_TB.Name = "Percent_TB";
            this.Percent_TB.Size = new System.Drawing.Size(52, 20);
            this.Percent_TB.TabIndex = 14;
            this.Percent_TB.Text = "30";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(680, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 20);
            this.label2.TabIndex = 15;
            this.label2.Text = "Standard Deviations";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(681, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 20);
            this.label3.TabIndex = 16;
            this.label3.Text = "%";
            // 
            // HiLo_Results_DataGridView
            // 
            this.HiLo_Results_DataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.HiLo_Results_DataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.HiLo_Results_DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.HiLo_Results_DataGridView.Location = new System.Drawing.Point(16, 117);
            this.HiLo_Results_DataGridView.Name = "HiLo_Results_DataGridView";
            this.HiLo_Results_DataGridView.Size = new System.Drawing.Size(969, 304);
            this.HiLo_Results_DataGridView.TabIndex = 17;
            // 
            // Net_CPM_Label
            // 
            this.Net_CPM_Label.AutoSize = true;
            this.Net_CPM_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Net_CPM_Label.Location = new System.Drawing.Point(207, 424);
            this.Net_CPM_Label.Name = "Net_CPM_Label";
            this.Net_CPM_Label.Size = new System.Drawing.Size(76, 20);
            this.Net_CPM_Label.TabIndex = 18;
            this.Net_CPM_Label.Text = "Net CPM:";
            // 
            // LL_Beta_Label
            // 
            this.LL_Beta_Label.AutoSize = true;
            this.LL_Beta_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LL_Beta_Label.Location = new System.Drawing.Point(12, 473);
            this.LL_Beta_Label.Name = "LL_Beta_Label";
            this.LL_Beta_Label.Size = new System.Drawing.Size(184, 20);
            this.LL_Beta_Label.TabIndex = 19;
            this.LL_Beta_Label.Text = "Low Limit, Beta Channel:";
            // 
            // LL_Alpha_Label
            // 
            this.LL_Alpha_Label.AutoSize = true;
            this.LL_Alpha_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LL_Alpha_Label.Location = new System.Drawing.Point(12, 445);
            this.LL_Alpha_Label.Name = "LL_Alpha_Label";
            this.LL_Alpha_Label.Size = new System.Drawing.Size(191, 20);
            this.LL_Alpha_Label.TabIndex = 20;
            this.LL_Alpha_Label.Text = "Low Limit, Alpha Channel:";
            // 
            // LL_Alpha_TB
            // 
            this.LL_Alpha_TB.Enabled = false;
            this.LL_Alpha_TB.Location = new System.Drawing.Point(209, 447);
            this.LL_Alpha_TB.Name = "LL_Alpha_TB";
            this.LL_Alpha_TB.Size = new System.Drawing.Size(100, 20);
            this.LL_Alpha_TB.TabIndex = 21;
            // 
            // LL_Beta_TB
            // 
            this.LL_Beta_TB.Enabled = false;
            this.LL_Beta_TB.Location = new System.Drawing.Point(209, 473);
            this.LL_Beta_TB.Name = "LL_Beta_TB";
            this.LL_Beta_TB.Size = new System.Drawing.Size(100, 20);
            this.LL_Beta_TB.TabIndex = 22;
            // 
            // HL_Beta_TB
            // 
            this.HL_Beta_TB.Enabled = false;
            this.HL_Beta_TB.Location = new System.Drawing.Point(512, 473);
            this.HL_Beta_TB.Name = "HL_Beta_TB";
            this.HL_Beta_TB.Size = new System.Drawing.Size(100, 20);
            this.HL_Beta_TB.TabIndex = 26;
            // 
            // HL_Alpha_TB
            // 
            this.HL_Alpha_TB.Enabled = false;
            this.HL_Alpha_TB.Location = new System.Drawing.Point(512, 447);
            this.HL_Alpha_TB.Name = "HL_Alpha_TB";
            this.HL_Alpha_TB.Size = new System.Drawing.Size(100, 20);
            this.HL_Alpha_TB.TabIndex = 25;
            // 
            // HL_Alpha_Label
            // 
            this.HL_Alpha_Label.AutoSize = true;
            this.HL_Alpha_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HL_Alpha_Label.Location = new System.Drawing.Point(315, 445);
            this.HL_Alpha_Label.Name = "HL_Alpha_Label";
            this.HL_Alpha_Label.Size = new System.Drawing.Size(195, 20);
            this.HL_Alpha_Label.TabIndex = 24;
            this.HL_Alpha_Label.Text = "High Limit, Alpha Channel:";
            // 
            // HL_Beta_Label
            // 
            this.HL_Beta_Label.AutoSize = true;
            this.HL_Beta_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HL_Beta_Label.Location = new System.Drawing.Point(315, 473);
            this.HL_Beta_Label.Name = "HL_Beta_Label";
            this.HL_Beta_Label.Size = new System.Drawing.Size(188, 20);
            this.HL_Beta_Label.TabIndex = 23;
            this.HL_Beta_Label.Text = "High Limit, Beta Channel:";
            // 
            // Net_CPM_Label2
            // 
            this.Net_CPM_Label2.AutoSize = true;
            this.Net_CPM_Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Net_CPM_Label2.Location = new System.Drawing.Point(508, 424);
            this.Net_CPM_Label2.Name = "Net_CPM_Label2";
            this.Net_CPM_Label2.Size = new System.Drawing.Size(76, 20);
            this.Net_CPM_Label2.TabIndex = 27;
            this.Net_CPM_Label2.Text = "Net CPM:";
            // 
            // Save_Limits_Button
            // 
            this.Save_Limits_Button.Location = new System.Drawing.Point(622, 470);
            this.Save_Limits_Button.Name = "Save_Limits_Button";
            this.Save_Limits_Button.Size = new System.Drawing.Size(75, 23);
            this.Save_Limits_Button.TabIndex = 28;
            this.Save_Limits_Button.Text = "Save Limits";
            this.Save_Limits_Button.UseVisualStyleBackColor = true;
            this.Save_Limits_Button.Click += new System.EventHandler(this.Save_Limits_Button_Click);
            // 
            // Override_CB
            // 
            this.Override_CB.AutoSize = true;
            this.Override_CB.Location = new System.Drawing.Point(624, 447);
            this.Override_CB.Name = "Override_CB";
            this.Override_CB.Size = new System.Drawing.Size(66, 17);
            this.Override_CB.TabIndex = 29;
            this.Override_CB.Text = "Override";
            this.Override_CB.UseVisualStyleBackColor = true;
            this.Override_CB.CheckedChanged += new System.EventHandler(this.Override_CB_CheckedChanged);
            // 
            // DABRAS_Status_Label
            // 
            this.DABRAS_Status_Label.AutoSize = true;
            this.DABRAS_Status_Label.Location = new System.Drawing.Point(863, 523);
            this.DABRAS_Status_Label.Name = "DABRAS_Status_Label";
            this.DABRAS_Status_Label.Size = new System.Drawing.Size(122, 13);
            this.DABRAS_Status_Label.TabIndex = 32;
            this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
            // 
            // DABRAS_Firmware_Label
            // 
            this.DABRAS_Firmware_Label.AutoSize = true;
            this.DABRAS_Firmware_Label.Location = new System.Drawing.Point(447, 523);
            this.DABRAS_Firmware_Label.Name = "DABRAS_Firmware_Label";
            this.DABRAS_Firmware_Label.Size = new System.Drawing.Size(40, 13);
            this.DABRAS_Firmware_Label.TabIndex = 31;
            this.DABRAS_Firmware_Label.Text = "F X.XX";
            // 
            // DABRAS_SN_Label
            // 
            this.DABRAS_SN_Label.AutoSize = true;
            this.DABRAS_SN_Label.Location = new System.Drawing.Point(6, 523);
            this.DABRAS_SN_Label.Name = "DABRAS_SN_Label";
            this.DABRAS_SN_Label.Size = new System.Drawing.Size(99, 13);
            this.DABRAS_SN_Label.TabIndex = 30;
            this.DABRAS_SN_Label.Text = "s/n: XXXXXXXXXX";
            // 
            // StopButton
            // 
            this.StopButton.Enabled = false;
            this.StopButton.Location = new System.Drawing.Point(333, 78);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(100, 23);
            this.StopButton.TabIndex = 33;
            this.StopButton.Text = "Stop (Ctrl + S)";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // Recompute_Limits_Button
            // 
            this.Recompute_Limits_Button.AutoEllipsis = true;
            this.Recompute_Limits_Button.Location = new System.Drawing.Point(840, 57);
            this.Recompute_Limits_Button.Name = "Recompute_Limits_Button";
            this.Recompute_Limits_Button.Size = new System.Drawing.Size(75, 49);
            this.Recompute_Limits_Button.TabIndex = 34;
            this.Recompute_Limits_Button.Text = "Recompute Limits";
            this.Recompute_Limits_Button.UseVisualStyleBackColor = true;
            this.Recompute_Limits_Button.Click += new System.EventHandler(this.Recompute_Limits_Button_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(704, 469);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(162, 23);
            this.SaveButton.TabIndex = 35;
            this.SaveButton.Text = "Write Results To CSV (Ctrl + V)";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ImageSaveButton
            // 
            this.ImageSaveButton.Location = new System.Drawing.Point(704, 440);
            this.ImageSaveButton.Name = "ImageSaveButton";
            this.ImageSaveButton.Size = new System.Drawing.Size(162, 23);
            this.ImageSaveButton.TabIndex = 36;
            this.ImageSaveButton.Text = "Save Image (Ctrl + I)";
            this.ImageSaveButton.UseVisualStyleBackColor = true;
            this.ImageSaveButton.Click += new System.EventHandler(this.ImageSaveButton_Click);
            // 
            // HiLo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 545);
            this.Controls.Add(this.ImageSaveButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.Recompute_Limits_Button);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.DABRAS_Status_Label);
            this.Controls.Add(this.DABRAS_Firmware_Label);
            this.Controls.Add(this.DABRAS_SN_Label);
            this.Controls.Add(this.Override_CB);
            this.Controls.Add(this.Save_Limits_Button);
            this.Controls.Add(this.Net_CPM_Label2);
            this.Controls.Add(this.HL_Beta_TB);
            this.Controls.Add(this.HL_Alpha_TB);
            this.Controls.Add(this.HL_Alpha_Label);
            this.Controls.Add(this.HL_Beta_Label);
            this.Controls.Add(this.LL_Beta_TB);
            this.Controls.Add(this.LL_Alpha_TB);
            this.Controls.Add(this.LL_Alpha_Label);
            this.Controls.Add(this.LL_Beta_Label);
            this.Controls.Add(this.Net_CPM_Label);
            this.Controls.Add(this.HiLo_Results_DataGridView);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Percent_TB);
            this.Controls.Add(this.StdDev_TB);
            this.Controls.Add(this.PercentButton);
            this.Controls.Add(this.StdDevButton);
            this.Controls.Add(this.AquireButton);
            this.Controls.Add(this.SecLabel);
            this.Controls.Add(this.MinLabel);
            this.Controls.Add(this.Sec_TB);
            this.Controls.Add(this.Colon_Label);
            this.Controls.Add(this.Min_TB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Num_Counts_TB);
            this.Controls.Add(this.CollectLabel);
            this.Controls.Add(this.TypeOfHiLoLabel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "HiLo";
            this.Text = "HiLo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HiLo_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.HiLo_VisibleChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HiLo_Results_DataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeWindowCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem functionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aquireCtrlAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopAquiringCtrlSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectOrDisconnectAPortCtrlPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.Label TypeOfHiLoLabel;
        private System.Windows.Forms.Label CollectLabel;
        private System.Windows.Forms.TextBox Num_Counts_TB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Min_TB;
        private System.Windows.Forms.Label Colon_Label;
        private System.Windows.Forms.TextBox Sec_TB;
        private System.Windows.Forms.Label MinLabel;
        private System.Windows.Forms.Label SecLabel;
        private System.Windows.Forms.Button AquireButton;
        private System.Windows.Forms.RadioButton StdDevButton;
        private System.Windows.Forms.RadioButton PercentButton;
        private System.Windows.Forms.TextBox StdDev_TB;
        private System.Windows.Forms.TextBox Percent_TB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView HiLo_Results_DataGridView;
        private System.Windows.Forms.Label Net_CPM_Label;
        private System.Windows.Forms.Label LL_Beta_Label;
        private System.Windows.Forms.Label LL_Alpha_Label;
        private System.Windows.Forms.TextBox LL_Alpha_TB;
        private System.Windows.Forms.TextBox LL_Beta_TB;
        private System.Windows.Forms.TextBox HL_Beta_TB;
        private System.Windows.Forms.TextBox HL_Alpha_TB;
        private System.Windows.Forms.Label HL_Alpha_Label;
        private System.Windows.Forms.Label HL_Beta_Label;
        private System.Windows.Forms.Label Net_CPM_Label2;
        private System.Windows.Forms.Button Save_Limits_Button;
        private System.Windows.Forms.CheckBox Override_CB;
        private System.Windows.Forms.Label DABRAS_Status_Label;
        private System.Windows.Forms.Label DABRAS_Firmware_Label;
        private System.Windows.Forms.Label DABRAS_SN_Label;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.ToolStripMenuItem openWebBasedSurveyF12ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRSOSharepointF11ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRSOHomeF10ToolStripMenuItem;
        private System.Windows.Forms.Button Recompute_Limits_Button;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.ToolStripMenuItem writeResultsToFileCtrlVToolStripMenuItem;
        private System.Windows.Forms.Button ImageSaveButton;
        private System.Windows.Forms.ToolStripMenuItem saveImageCtrlIToolStripMenuItem;
    }
}