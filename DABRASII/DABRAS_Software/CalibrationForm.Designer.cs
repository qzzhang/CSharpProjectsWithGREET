namespace DABRAS_Software
{
    partial class CalibrationForm
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
            this.saveEfficiencyDataCtrlKToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBackgroundDataCtrlJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveImageCtrlIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.determineEfficiencyCtrlEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundCheckCtrlBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopAquisitionCtrlSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWebBasedSurveySystemF12ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRSOSharepointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRSOHomeF10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourcesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewEditCtrlOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSourceCtrlAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSourceCtrlDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hiLoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.establishBackgroundHiLoLimitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.establishAm241ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.establishSrToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.highVoltagePlateauCtrlLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setHighVoltageCtrlHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changePasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setBackgroundTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setChiSquaredCalibrationRequirementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setModFactorRequirementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setBuildingNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Source_Label = new System.Windows.Forms.Label();
            this.Source_ComboBox = new System.Windows.Forms.ComboBox();
            this.TypeOfRadiationLabel1 = new System.Windows.Forms.Label();
            this.TypeOfRadiationLabel2 = new System.Windows.Forms.Label();
            this.Determine_Efficiency_Button = new System.Windows.Forms.Button();
            this.Ef_Label = new System.Windows.Forms.Label();
            this.Minutes_TB = new System.Windows.Forms.TextBox();
            this.Seconds_TB = new System.Windows.Forms.TextBox();
            this.Num_Counts_TB = new System.Windows.Forms.TextBox();
            this.Colon_Label = new System.Windows.Forms.Label();
            this.Minutes_Label = new System.Windows.Forms.Label();
            this.Seconds_Label = new System.Windows.Forms.Label();
            this.Stop_Count_Button = new System.Windows.Forms.Button();
            this.Calibration_Results_GridView = new System.Windows.Forms.DataGridView();
            this.Determine_BG_Button = new System.Windows.Forms.Button();
            this.for_label = new System.Windows.Forms.Label();
            this.Sec_BG_Label = new System.Windows.Forms.Label();
            this.Min_BG_Label = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Sec_BG_TB = new System.Windows.Forms.TextBox();
            this.Min_BG_TB = new System.Windows.Forms.TextBox();
            this.Background_Results_GridView = new System.Windows.Forms.DataGridView();
            this.DABRAS_SN_Label = new System.Windows.Forms.Label();
            this.DABRAS_Firmware_Label = new System.Windows.Forms.Label();
            this.DABRAS_Status_Label = new System.Windows.Forms.Label();
            this.BG_NumCounts_TB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._100_200_Energy_Button = new System.Windows.Forms.RadioButton();
            this._1200_Energy_Button = new System.Windows.Forms.RadioButton();
            this._400_1200_Energy_Button = new System.Windows.Forms.RadioButton();
            this._200_400_Energy_Button = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SaveEfficiencyDataButton = new System.Windows.Forms.Button();
            this.SaveBackgroundCalibrationButton = new System.Windows.Forms.Button();
            this.ImageSaveButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Calibration_Results_GridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Background_Results_GridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.functionToolStripMenuItem,
            this.sourcesToolStripMenuItem,
            this.hiLoToolStripMenuItem,
            this.utilityToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(953, 24);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeCtrlQToolStripMenuItem,
            this.saveEfficiencyDataCtrlKToolStripMenuItem,
            this.saveBackgroundDataCtrlJToolStripMenuItem,
            this.saveImageCtrlIToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeCtrlQToolStripMenuItem
            // 
            this.closeCtrlQToolStripMenuItem.Name = "closeCtrlQToolStripMenuItem";
            this.closeCtrlQToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.closeCtrlQToolStripMenuItem.Text = "Close (Ctrl + Q)";
            this.closeCtrlQToolStripMenuItem.Click += new System.EventHandler(this.closeCtrlQToolStripMenuItem_Click);
            // 
            // saveEfficiencyDataCtrlKToolStripMenuItem
            // 
            this.saveEfficiencyDataCtrlKToolStripMenuItem.Name = "saveEfficiencyDataCtrlKToolStripMenuItem";
            this.saveEfficiencyDataCtrlKToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.saveEfficiencyDataCtrlKToolStripMenuItem.Text = "Save Efficiency Data (Ctrl + K)";
            this.saveEfficiencyDataCtrlKToolStripMenuItem.Click += new System.EventHandler(this.saveEfficiencyDataCtrlKToolStripMenuItem_Click);
            // 
            // saveBackgroundDataCtrlJToolStripMenuItem
            // 
            this.saveBackgroundDataCtrlJToolStripMenuItem.Name = "saveBackgroundDataCtrlJToolStripMenuItem";
            this.saveBackgroundDataCtrlJToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.saveBackgroundDataCtrlJToolStripMenuItem.Text = "Save Background Data (Ctrl + J)";
            this.saveBackgroundDataCtrlJToolStripMenuItem.Click += new System.EventHandler(this.saveBackgroundDataCtrlJToolStripMenuItem_Click);
            // 
            // saveImageCtrlIToolStripMenuItem
            // 
            this.saveImageCtrlIToolStripMenuItem.Name = "saveImageCtrlIToolStripMenuItem";
            this.saveImageCtrlIToolStripMenuItem.Size = new System.Drawing.Size(240, 22);
            this.saveImageCtrlIToolStripMenuItem.Text = "Save Image (Ctrl + I)";
            this.saveImageCtrlIToolStripMenuItem.Click += new System.EventHandler(this.saveImageCtrlIToolStripMenuItem_Click);
            // 
            // functionToolStripMenuItem
            // 
            this.functionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.determineEfficiencyCtrlEToolStripMenuItem,
            this.backgroundCheckCtrlBToolStripMenuItem,
            this.stopAquisitionCtrlSToolStripMenuItem,
            this.openWebBasedSurveySystemF12ToolStripMenuItem,
            this.openRSOSharepointToolStripMenuItem,
            this.openRSOHomeF10ToolStripMenuItem});
            this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
            this.functionToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.functionToolStripMenuItem.Text = "Function";
            // 
            // determineEfficiencyCtrlEToolStripMenuItem
            // 
            this.determineEfficiencyCtrlEToolStripMenuItem.Name = "determineEfficiencyCtrlEToolStripMenuItem";
            this.determineEfficiencyCtrlEToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.determineEfficiencyCtrlEToolStripMenuItem.Text = "Determine Efficiency (Ctrl + E)";
            this.determineEfficiencyCtrlEToolStripMenuItem.Click += new System.EventHandler(this.determineEfficiencyCtrlEToolStripMenuItem_Click);
            // 
            // backgroundCheckCtrlBToolStripMenuItem
            // 
            this.backgroundCheckCtrlBToolStripMenuItem.Name = "backgroundCheckCtrlBToolStripMenuItem";
            this.backgroundCheckCtrlBToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.backgroundCheckCtrlBToolStripMenuItem.Text = "Background Check (Ctrl + B)";
            this.backgroundCheckCtrlBToolStripMenuItem.Click += new System.EventHandler(this.backgroundCheckCtrlBToolStripMenuItem_Click);
            // 
            // stopAquisitionCtrlSToolStripMenuItem
            // 
            this.stopAquisitionCtrlSToolStripMenuItem.Enabled = false;
            this.stopAquisitionCtrlSToolStripMenuItem.Name = "stopAquisitionCtrlSToolStripMenuItem";
            this.stopAquisitionCtrlSToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.stopAquisitionCtrlSToolStripMenuItem.Text = "Stop Aquisition (Ctrl + S)";
            this.stopAquisitionCtrlSToolStripMenuItem.Click += new System.EventHandler(this.stopAquisitionCtrlSToolStripMenuItem_Click);
            // 
            // openWebBasedSurveySystemF12ToolStripMenuItem
            // 
            this.openWebBasedSurveySystemF12ToolStripMenuItem.Name = "openWebBasedSurveySystemF12ToolStripMenuItem";
            this.openWebBasedSurveySystemF12ToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.openWebBasedSurveySystemF12ToolStripMenuItem.Text = "Open Web-Based Survey System (F12)";
            this.openWebBasedSurveySystemF12ToolStripMenuItem.Click += new System.EventHandler(this.openWebBasedSurveySystemF12ToolStripMenuItem_Click);
            // 
            // openRSOSharepointToolStripMenuItem
            // 
            this.openRSOSharepointToolStripMenuItem.Name = "openRSOSharepointToolStripMenuItem";
            this.openRSOSharepointToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.openRSOSharepointToolStripMenuItem.Text = "Open RSO Sharepoint  (F11)";
            this.openRSOSharepointToolStripMenuItem.Click += new System.EventHandler(this.openRSOSharepointToolStripMenuItem_Click);
            // 
            // openRSOHomeF10ToolStripMenuItem
            // 
            this.openRSOHomeF10ToolStripMenuItem.Name = "openRSOHomeF10ToolStripMenuItem";
            this.openRSOHomeF10ToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.openRSOHomeF10ToolStripMenuItem.Text = "Open RSO Home (F10)";
            this.openRSOHomeF10ToolStripMenuItem.Click += new System.EventHandler(this.openRSOHomeF10ToolStripMenuItem_Click);
            // 
            // sourcesToolStripMenuItem
            // 
            this.sourcesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewEditCtrlOToolStripMenuItem,
            this.addSourceCtrlAToolStripMenuItem,
            this.deleteSourceCtrlDToolStripMenuItem});
            this.sourcesToolStripMenuItem.Name = "sourcesToolStripMenuItem";
            this.sourcesToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.sourcesToolStripMenuItem.Text = "Sources";
            // 
            // viewEditCtrlOToolStripMenuItem
            // 
            this.viewEditCtrlOToolStripMenuItem.Name = "viewEditCtrlOToolStripMenuItem";
            this.viewEditCtrlOToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.viewEditCtrlOToolStripMenuItem.Text = "View / Edit (Ctrl + O)";
            this.viewEditCtrlOToolStripMenuItem.Click += new System.EventHandler(this.viewEditCtrlOToolStripMenuItem_Click);
            // 
            // addSourceCtrlAToolStripMenuItem
            // 
            this.addSourceCtrlAToolStripMenuItem.Name = "addSourceCtrlAToolStripMenuItem";
            this.addSourceCtrlAToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.addSourceCtrlAToolStripMenuItem.Text = "Add Source (Ctrl + A)";
            this.addSourceCtrlAToolStripMenuItem.Click += new System.EventHandler(this.addSourceCtrlAToolStripMenuItem_Click);
            // 
            // deleteSourceCtrlDToolStripMenuItem
            // 
            this.deleteSourceCtrlDToolStripMenuItem.Name = "deleteSourceCtrlDToolStripMenuItem";
            this.deleteSourceCtrlDToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.deleteSourceCtrlDToolStripMenuItem.Text = "Delete Source (Ctrl + D)";
            this.deleteSourceCtrlDToolStripMenuItem.Click += new System.EventHandler(this.deleteSourceCtrlDToolStripMenuItem_Click);
            // 
            // hiLoToolStripMenuItem
            // 
            this.hiLoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.establishBackgroundHiLoLimitsToolStripMenuItem,
            this.establishAm241ToolStripMenuItem,
            this.establishSrToolStripMenuItem});
            this.hiLoToolStripMenuItem.Name = "hiLoToolStripMenuItem";
            this.hiLoToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.hiLoToolStripMenuItem.Text = "HiLo";
            // 
            // establishBackgroundHiLoLimitsToolStripMenuItem
            // 
            this.establishBackgroundHiLoLimitsToolStripMenuItem.Name = "establishBackgroundHiLoLimitsToolStripMenuItem";
            this.establishBackgroundHiLoLimitsToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
            this.establishBackgroundHiLoLimitsToolStripMenuItem.Text = "Establish Background Hi-Lo Limits (Ctrl + Alt + B)";
            this.establishBackgroundHiLoLimitsToolStripMenuItem.Click += new System.EventHandler(this.establishBackgroundHiLoLimitsToolStripMenuItem_Click);
            // 
            // establishAm241ToolStripMenuItem
            // 
            this.establishAm241ToolStripMenuItem.Name = "establishAm241ToolStripMenuItem";
            this.establishAm241ToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
            this.establishAm241ToolStripMenuItem.Text = "Establish Am-241 Hi-Lo Limits (Ctrl + Alt + A)";
            this.establishAm241ToolStripMenuItem.Click += new System.EventHandler(this.establishAm241ToolStripMenuItem_Click);
            // 
            // establishSrToolStripMenuItem
            // 
            this.establishSrToolStripMenuItem.Name = "establishSrToolStripMenuItem";
            this.establishSrToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
            this.establishSrToolStripMenuItem.Text = "Establish Sr-90 Hi-Lo Limits (Ctrl + Alt + S)";
            this.establishSrToolStripMenuItem.Click += new System.EventHandler(this.establishSrToolStripMenuItem_Click);
            // 
            // utilityToolStripMenuItem
            // 
            this.utilityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem,
            this.highVoltagePlateauCtrlLToolStripMenuItem,
            this.setHighVoltageCtrlHToolStripMenuItem,
            this.changePasswordToolStripMenuItem,
            this.setBackgroundTypeToolStripMenuItem,
            this.setChiSquaredCalibrationRequirementsToolStripMenuItem,
            this.setModFactorRequirementsToolStripMenuItem,
            this.setBuildingNumberToolStripMenuItem});
            this.utilityToolStripMenuItem.Name = "utilityToolStripMenuItem";
            this.utilityToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.utilityToolStripMenuItem.Text = "Utility";
            // 
            // connectOrDisconnectAPortCtrlPToolStripMenuItem
            // 
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem.Name = "connectOrDisconnectAPortCtrlPToolStripMenuItem";
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem.Text = "Connect or Disconnect a Port (Ctrl + P)";
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem.Click += new System.EventHandler(this.connectOrDisconnectAPortCtrlPToolStripMenuItem_Click);
            // 
            // highVoltagePlateauCtrlLToolStripMenuItem
            // 
            this.highVoltagePlateauCtrlLToolStripMenuItem.Name = "highVoltagePlateauCtrlLToolStripMenuItem";
            this.highVoltagePlateauCtrlLToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.highVoltagePlateauCtrlLToolStripMenuItem.Text = "High Voltage Plateau (Ctrl + L)";
            this.highVoltagePlateauCtrlLToolStripMenuItem.Click += new System.EventHandler(this.highVoltagePlateauCtrlLToolStripMenuItem_Click);
            // 
            // setHighVoltageCtrlHToolStripMenuItem
            // 
            this.setHighVoltageCtrlHToolStripMenuItem.Name = "setHighVoltageCtrlHToolStripMenuItem";
            this.setHighVoltageCtrlHToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.setHighVoltageCtrlHToolStripMenuItem.Text = "Set High Voltage (Ctrl + H)";
            this.setHighVoltageCtrlHToolStripMenuItem.Click += new System.EventHandler(this.setHighVoltageCtrlHToolStripMenuItem_Click);
            // 
            // changePasswordToolStripMenuItem
            // 
            this.changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
            this.changePasswordToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.changePasswordToolStripMenuItem.Text = "Change Password";
            this.changePasswordToolStripMenuItem.Click += new System.EventHandler(this.changePasswordToolStripMenuItem_Click);
            // 
            // setBackgroundTypeToolStripMenuItem
            // 
            this.setBackgroundTypeToolStripMenuItem.Name = "setBackgroundTypeToolStripMenuItem";
            this.setBackgroundTypeToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.setBackgroundTypeToolStripMenuItem.Text = "Set Background Type";
            this.setBackgroundTypeToolStripMenuItem.Click += new System.EventHandler(this.setBackgroundTypeToolStripMenuItem_Click);
            // 
            // setChiSquaredCalibrationRequirementsToolStripMenuItem
            // 
            this.setChiSquaredCalibrationRequirementsToolStripMenuItem.Name = "setChiSquaredCalibrationRequirementsToolStripMenuItem";
            this.setChiSquaredCalibrationRequirementsToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.setChiSquaredCalibrationRequirementsToolStripMenuItem.Text = "Set Chi Squared Calibration Requirements";
            this.setChiSquaredCalibrationRequirementsToolStripMenuItem.Click += new System.EventHandler(this.setChiSquaredCalibrationRequirementsToolStripMenuItem_Click);
            // 
            // setModFactorRequirementsToolStripMenuItem
            // 
            this.setModFactorRequirementsToolStripMenuItem.Name = "setModFactorRequirementsToolStripMenuItem";
            this.setModFactorRequirementsToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.setModFactorRequirementsToolStripMenuItem.Text = "Set Mod Factor Requirements";
            this.setModFactorRequirementsToolStripMenuItem.Click += new System.EventHandler(this.setModFactorRequirementsToolStripMenuItem_Click);
            // 
            // setBuildingNumberToolStripMenuItem
            // 
            this.setBuildingNumberToolStripMenuItem.Name = "setBuildingNumberToolStripMenuItem";
            this.setBuildingNumberToolStripMenuItem.Size = new System.Drawing.Size(294, 22);
            this.setBuildingNumberToolStripMenuItem.Text = "Change Set/Building Number";
            this.setBuildingNumberToolStripMenuItem.Click += new System.EventHandler(this.setBuildingNumberToolStripMenuItem_Click);
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
            // Source_Label
            // 
            this.Source_Label.AutoSize = true;
            this.Source_Label.Location = new System.Drawing.Point(12, 45);
            this.Source_Label.Name = "Source_Label";
            this.Source_Label.Size = new System.Drawing.Size(80, 13);
            this.Source_Label.TabIndex = 1;
            this.Source_Label.Text = "Select Source: ";
            // 
            // Source_ComboBox
            // 
            this.Source_ComboBox.FormattingEnabled = true;
            this.Source_ComboBox.Location = new System.Drawing.Point(98, 42);
            this.Source_ComboBox.Name = "Source_ComboBox";
            this.Source_ComboBox.Size = new System.Drawing.Size(121, 21);
            this.Source_ComboBox.TabIndex = 0;
            this.Source_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Source_ComboBox_SelectedIndexChanged);
            this.Source_ComboBox.Click += new System.EventHandler(this.Source_ComboBox_Click);
            // 
            // TypeOfRadiationLabel1
            // 
            this.TypeOfRadiationLabel1.AutoSize = true;
            this.TypeOfRadiationLabel1.Location = new System.Drawing.Point(243, 45);
            this.TypeOfRadiationLabel1.Name = "TypeOfRadiationLabel1";
            this.TypeOfRadiationLabel1.Size = new System.Drawing.Size(35, 13);
            this.TypeOfRadiationLabel1.TabIndex = 3;
            this.TypeOfRadiationLabel1.Text = "NULL";
            // 
            // TypeOfRadiationLabel2
            // 
            this.TypeOfRadiationLabel2.AutoSize = true;
            this.TypeOfRadiationLabel2.Location = new System.Drawing.Point(284, 45);
            this.TypeOfRadiationLabel2.Name = "TypeOfRadiationLabel2";
            this.TypeOfRadiationLabel2.Size = new System.Drawing.Size(35, 13);
            this.TypeOfRadiationLabel2.TabIndex = 4;
            this.TypeOfRadiationLabel2.Text = "NULL";
            // 
            // Determine_Efficiency_Button
            // 
            this.Determine_Efficiency_Button.Location = new System.Drawing.Point(17, 83);
            this.Determine_Efficiency_Button.Name = "Determine_Efficiency_Button";
            this.Determine_Efficiency_Button.Size = new System.Drawing.Size(160, 23);
            this.Determine_Efficiency_Button.TabIndex = 4;
            this.Determine_Efficiency_Button.Text = "Determine Efficiency (Ctrl + E)";
            this.Determine_Efficiency_Button.UseVisualStyleBackColor = true;
            this.Determine_Efficiency_Button.Click += new System.EventHandler(this.Determine_Efficiency_Button_Click);
            // 
            // Ef_Label
            // 
            this.Ef_Label.AutoSize = true;
            this.Ef_Label.Location = new System.Drawing.Point(224, 88);
            this.Ef_Label.Name = "Ef_Label";
            this.Ef_Label.Size = new System.Drawing.Size(54, 13);
            this.Ef_Label.TabIndex = 6;
            this.Ef_Label.Text = "counts of ";
            // 
            // Minutes_TB
            // 
            this.Minutes_TB.Location = new System.Drawing.Point(284, 88);
            this.Minutes_TB.Name = "Minutes_TB";
            this.Minutes_TB.Size = new System.Drawing.Size(48, 20);
            this.Minutes_TB.TabIndex = 2;
            this.Minutes_TB.Text = "1";
            // 
            // Seconds_TB
            // 
            this.Seconds_TB.Location = new System.Drawing.Point(354, 88);
            this.Seconds_TB.Name = "Seconds_TB";
            this.Seconds_TB.Size = new System.Drawing.Size(48, 20);
            this.Seconds_TB.TabIndex = 3;
            this.Seconds_TB.Text = "0";
            // 
            // Num_Counts_TB
            // 
            this.Num_Counts_TB.Location = new System.Drawing.Point(183, 86);
            this.Num_Counts_TB.Name = "Num_Counts_TB";
            this.Num_Counts_TB.Size = new System.Drawing.Size(36, 20);
            this.Num_Counts_TB.TabIndex = 1;
            this.Num_Counts_TB.Text = "10";
            this.Num_Counts_TB.TextChanged += new System.EventHandler(this.Num_Counts_TB_TextChanged);
            // 
            // Colon_Label
            // 
            this.Colon_Label.AutoSize = true;
            this.Colon_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Colon_Label.Location = new System.Drawing.Point(336, 89);
            this.Colon_Label.Name = "Colon_Label";
            this.Colon_Label.Size = new System.Drawing.Size(12, 16);
            this.Colon_Label.TabIndex = 10;
            this.Colon_Label.Text = ":";
            // 
            // Minutes_Label
            // 
            this.Minutes_Label.AutoSize = true;
            this.Minutes_Label.Location = new System.Drawing.Point(284, 72);
            this.Minutes_Label.Name = "Minutes_Label";
            this.Minutes_Label.Size = new System.Drawing.Size(24, 13);
            this.Minutes_Label.TabIndex = 11;
            this.Minutes_Label.Text = "Min";
            // 
            // Seconds_Label
            // 
            this.Seconds_Label.AutoSize = true;
            this.Seconds_Label.Location = new System.Drawing.Point(351, 72);
            this.Seconds_Label.Name = "Seconds_Label";
            this.Seconds_Label.Size = new System.Drawing.Size(26, 13);
            this.Seconds_Label.TabIndex = 12;
            this.Seconds_Label.Text = "Sec";
            // 
            // Stop_Count_Button
            // 
            this.Stop_Count_Button.Enabled = false;
            this.Stop_Count_Button.Location = new System.Drawing.Point(798, 35);
            this.Stop_Count_Button.Name = "Stop_Count_Button";
            this.Stop_Count_Button.Size = new System.Drawing.Size(114, 23);
            this.Stop_Count_Button.TabIndex = 11;
            this.Stop_Count_Button.Text = "Stop Count (Ctrl + S)";
            this.Stop_Count_Button.UseVisualStyleBackColor = true;
            this.Stop_Count_Button.Click += new System.EventHandler(this.Stop_Count_Button_Click);
            // 
            // Calibration_Results_GridView
            // 
            this.Calibration_Results_GridView.AllowUserToAddRows = false;
            this.Calibration_Results_GridView.AllowUserToDeleteRows = false;
            this.Calibration_Results_GridView.AllowUserToResizeColumns = false;
            this.Calibration_Results_GridView.AllowUserToResizeRows = false;
            this.Calibration_Results_GridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.Calibration_Results_GridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Calibration_Results_GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Calibration_Results_GridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.Calibration_Results_GridView.GridColor = System.Drawing.SystemColors.Control;
            this.Calibration_Results_GridView.Location = new System.Drawing.Point(17, 113);
            this.Calibration_Results_GridView.Name = "Calibration_Results_GridView";
            this.Calibration_Results_GridView.RowHeadersWidth = 40;
            this.Calibration_Results_GridView.Size = new System.Drawing.Size(870, 319);
            this.Calibration_Results_GridView.TabIndex = 2;
            // 
            // Determine_BG_Button
            // 
            this.Determine_BG_Button.Location = new System.Drawing.Point(17, 456);
            this.Determine_BG_Button.Name = "Determine_BG_Button";
            this.Determine_BG_Button.Size = new System.Drawing.Size(171, 23);
            this.Determine_BG_Button.TabIndex = 8;
            this.Determine_BG_Button.Text = "Determine Background (Ctrl + B)";
            this.Determine_BG_Button.UseVisualStyleBackColor = true;
            this.Determine_BG_Button.Click += new System.EventHandler(this.Determine_BG_Button_Click);
            // 
            // for_label
            // 
            this.for_label.AutoSize = true;
            this.for_label.Location = new System.Drawing.Point(261, 461);
            this.for_label.Name = "for_label";
            this.for_label.Size = new System.Drawing.Size(19, 13);
            this.for_label.TabIndex = 16;
            this.for_label.Text = "for";
            // 
            // Sec_BG_Label
            // 
            this.Sec_BG_Label.AutoSize = true;
            this.Sec_BG_Label.Location = new System.Drawing.Point(364, 442);
            this.Sec_BG_Label.Name = "Sec_BG_Label";
            this.Sec_BG_Label.Size = new System.Drawing.Size(26, 13);
            this.Sec_BG_Label.TabIndex = 21;
            this.Sec_BG_Label.Text = "Sec";
            // 
            // Min_BG_Label
            // 
            this.Min_BG_Label.AutoSize = true;
            this.Min_BG_Label.Location = new System.Drawing.Point(297, 442);
            this.Min_BG_Label.Name = "Min_BG_Label";
            this.Min_BG_Label.Size = new System.Drawing.Size(24, 13);
            this.Min_BG_Label.TabIndex = 20;
            this.Min_BG_Label.Text = "Min";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(349, 459);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 16);
            this.label3.TabIndex = 19;
            this.label3.Text = ":";
            // 
            // Sec_BG_TB
            // 
            this.Sec_BG_TB.Location = new System.Drawing.Point(367, 458);
            this.Sec_BG_TB.Name = "Sec_BG_TB";
            this.Sec_BG_TB.Size = new System.Drawing.Size(48, 20);
            this.Sec_BG_TB.TabIndex = 7;
            this.Sec_BG_TB.Text = "0";
            // 
            // Min_BG_TB
            // 
            this.Min_BG_TB.Location = new System.Drawing.Point(297, 458);
            this.Min_BG_TB.Name = "Min_BG_TB";
            this.Min_BG_TB.Size = new System.Drawing.Size(48, 20);
            this.Min_BG_TB.TabIndex = 6;
            this.Min_BG_TB.Text = "1";
            // 
            // Background_Results_GridView
            // 
            this.Background_Results_GridView.AllowUserToAddRows = false;
            this.Background_Results_GridView.AllowUserToDeleteRows = false;
            this.Background_Results_GridView.AllowUserToResizeColumns = false;
            this.Background_Results_GridView.AllowUserToResizeRows = false;
            this.Background_Results_GridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.Background_Results_GridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Background_Results_GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Background_Results_GridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.Background_Results_GridView.Location = new System.Drawing.Point(17, 485);
            this.Background_Results_GridView.Name = "Background_Results_GridView";
            this.Background_Results_GridView.Size = new System.Drawing.Size(680, 325);
            this.Background_Results_GridView.TabIndex = 22;
            // 
            // DABRAS_SN_Label
            // 
            this.DABRAS_SN_Label.AutoSize = true;
            this.DABRAS_SN_Label.Location = new System.Drawing.Point(12, 858);
            this.DABRAS_SN_Label.Name = "DABRAS_SN_Label";
            this.DABRAS_SN_Label.Size = new System.Drawing.Size(99, 13);
            this.DABRAS_SN_Label.TabIndex = 23;
            this.DABRAS_SN_Label.Text = "s/n: XXXXXXXXXX";
            // 
            // DABRAS_Firmware_Label
            // 
            this.DABRAS_Firmware_Label.AutoSize = true;
            this.DABRAS_Firmware_Label.Location = new System.Drawing.Point(464, 858);
            this.DABRAS_Firmware_Label.Name = "DABRAS_Firmware_Label";
            this.DABRAS_Firmware_Label.Size = new System.Drawing.Size(40, 13);
            this.DABRAS_Firmware_Label.TabIndex = 24;
            this.DABRAS_Firmware_Label.Text = "F X.XX";
            // 
            // DABRAS_Status_Label
            // 
            this.DABRAS_Status_Label.AutoSize = true;
            this.DABRAS_Status_Label.Location = new System.Drawing.Point(765, 858);
            this.DABRAS_Status_Label.Name = "DABRAS_Status_Label";
            this.DABRAS_Status_Label.Size = new System.Drawing.Size(122, 13);
            this.DABRAS_Status_Label.TabIndex = 25;
            this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
            // 
            // BG_NumCounts_TB
            // 
            this.BG_NumCounts_TB.Location = new System.Drawing.Point(194, 458);
            this.BG_NumCounts_TB.Name = "BG_NumCounts_TB";
            this.BG_NumCounts_TB.Size = new System.Drawing.Size(36, 20);
            this.BG_NumCounts_TB.TabIndex = 5;
            this.BG_NumCounts_TB.Text = "10";
            this.BG_NumCounts_TB.TextChanged += new System.EventHandler(this.BG_NumCounts_TB_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(236, 461);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "counts of ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(464, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 28;
            this.label2.Text = "Beta Energy Level:";
            // 
            // _100_200_Energy_Button
            // 
            this._100_200_Energy_Button.AutoSize = true;
            this._100_200_Energy_Button.Location = new System.Drawing.Point(20, 18);
            this._100_200_Energy_Button.Name = "_100_200_Energy_Button";
            this._100_200_Energy_Button.Size = new System.Drawing.Size(87, 17);
            this._100_200_Energy_Button.TabIndex = 0;
            this._100_200_Energy_Button.TabStop = true;
            this._100_200_Energy_Button.Text = "100-200 KeV";
            this._100_200_Energy_Button.UseVisualStyleBackColor = true;
            this._100_200_Energy_Button.CheckedChanged += new System.EventHandler(this._100_200_Energy_Button_CheckedChanged);
            // 
            // _1200_Energy_Button
            // 
            this._1200_Energy_Button.AutoSize = true;
            this._1200_Energy_Button.Location = new System.Drawing.Point(119, 41);
            this._1200_Energy_Button.Name = "_1200_Energy_Button";
            this._1200_Energy_Button.Size = new System.Drawing.Size(78, 17);
            this._1200_Energy_Button.TabIndex = 3;
            this._1200_Energy_Button.TabStop = true;
            this._1200_Energy_Button.Text = ">1200 KeV";
            this._1200_Energy_Button.UseVisualStyleBackColor = true;
            this._1200_Energy_Button.CheckedChanged += new System.EventHandler(this._1200_Energy_Button_CheckedChanged);
            // 
            // _400_1200_Energy_Button
            // 
            this._400_1200_Energy_Button.AutoSize = true;
            this._400_1200_Energy_Button.Location = new System.Drawing.Point(20, 41);
            this._400_1200_Energy_Button.Name = "_400_1200_Energy_Button";
            this._400_1200_Energy_Button.Size = new System.Drawing.Size(93, 17);
            this._400_1200_Energy_Button.TabIndex = 1;
            this._400_1200_Energy_Button.TabStop = true;
            this._400_1200_Energy_Button.Text = "400-1200 KeV";
            this._400_1200_Energy_Button.UseVisualStyleBackColor = true;
            this._400_1200_Energy_Button.CheckedChanged += new System.EventHandler(this._400_1200_Energy_Button_CheckedChanged);
            // 
            // _200_400_Energy_Button
            // 
            this._200_400_Energy_Button.AutoSize = true;
            this._200_400_Energy_Button.Location = new System.Drawing.Point(119, 18);
            this._200_400_Energy_Button.Name = "_200_400_Energy_Button";
            this._200_400_Energy_Button.Size = new System.Drawing.Size(87, 17);
            this._200_400_Energy_Button.TabIndex = 2;
            this._200_400_Energy_Button.TabStop = true;
            this._200_400_Energy_Button.Text = "201-400 KeV";
            this._200_400_Energy_Button.UseVisualStyleBackColor = true;
            this._200_400_Energy_Button.CheckedChanged += new System.EventHandler(this._200_400_Energy_Button_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._200_400_Energy_Button);
            this.groupBox1.Controls.Add(this._400_1200_Energy_Button);
            this.groupBox1.Controls.Add(this._1200_Energy_Button);
            this.groupBox1.Controls.Add(this._100_200_Energy_Button);
            this.groupBox1.Location = new System.Drawing.Point(574, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(218, 71);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // SaveEfficiencyDataButton
            // 
            this.SaveEfficiencyDataButton.Location = new System.Drawing.Point(408, 85);
            this.SaveEfficiencyDataButton.Name = "SaveEfficiencyDataButton";
            this.SaveEfficiencyDataButton.Size = new System.Drawing.Size(160, 23);
            this.SaveEfficiencyDataButton.TabIndex = 9;
            this.SaveEfficiencyDataButton.Text = "Save Efficiency Data (Ctrl + K)";
            this.SaveEfficiencyDataButton.UseVisualStyleBackColor = true;
            this.SaveEfficiencyDataButton.Click += new System.EventHandler(this.SaveEfficiencyDataButton_Click);
            // 
            // SaveBackgroundCalibrationButton
            // 
            this.SaveBackgroundCalibrationButton.Location = new System.Drawing.Point(421, 455);
            this.SaveBackgroundCalibrationButton.Name = "SaveBackgroundCalibrationButton";
            this.SaveBackgroundCalibrationButton.Size = new System.Drawing.Size(182, 23);
            this.SaveBackgroundCalibrationButton.TabIndex = 10;
            this.SaveBackgroundCalibrationButton.Text = "Save Background Data (Ctrl + J)";
            this.SaveBackgroundCalibrationButton.UseVisualStyleBackColor = true;
            this.SaveBackgroundCalibrationButton.Click += new System.EventHandler(this.SaveBackgroundCalibrationButton_Click);
            // 
            // ImageSaveButton
            // 
            this.ImageSaveButton.Location = new System.Drawing.Point(799, 65);
            this.ImageSaveButton.Name = "ImageSaveButton";
            this.ImageSaveButton.Size = new System.Drawing.Size(113, 43);
            this.ImageSaveButton.TabIndex = 12;
            this.ImageSaveButton.Text = "Save form Image to File (Ctrl + I)";
            this.ImageSaveButton.UseVisualStyleBackColor = true;
            this.ImageSaveButton.Click += new System.EventHandler(this.ImageSaveButton_Click);
            // 
            // CalibrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(953, 880);
            this.Controls.Add(this.ImageSaveButton);
            this.Controls.Add(this.SaveBackgroundCalibrationButton);
            this.Controls.Add(this.SaveEfficiencyDataButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BG_NumCounts_TB);
            this.Controls.Add(this.DABRAS_Status_Label);
            this.Controls.Add(this.DABRAS_Firmware_Label);
            this.Controls.Add(this.DABRAS_SN_Label);
            this.Controls.Add(this.Background_Results_GridView);
            this.Controls.Add(this.Sec_BG_Label);
            this.Controls.Add(this.Min_BG_Label);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Sec_BG_TB);
            this.Controls.Add(this.Min_BG_TB);
            this.Controls.Add(this.for_label);
            this.Controls.Add(this.Determine_BG_Button);
            this.Controls.Add(this.Calibration_Results_GridView);
            this.Controls.Add(this.Stop_Count_Button);
            this.Controls.Add(this.Seconds_Label);
            this.Controls.Add(this.Minutes_Label);
            this.Controls.Add(this.Colon_Label);
            this.Controls.Add(this.Num_Counts_TB);
            this.Controls.Add(this.Seconds_TB);
            this.Controls.Add(this.Minutes_TB);
            this.Controls.Add(this.Ef_Label);
            this.Controls.Add(this.Determine_Efficiency_Button);
            this.Controls.Add(this.TypeOfRadiationLabel2);
            this.Controls.Add(this.TypeOfRadiationLabel1);
            this.Controls.Add(this.Source_ComboBox);
            this.Controls.Add(this.Source_Label);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CalibrationForm";
            this.Text = "Calibration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalibrationForm_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.CalibrationForm_VisibleChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Calibration_Results_GridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Background_Results_GridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem functionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem determineEfficiencyCtrlEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundCheckCtrlBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopAquisitionCtrlSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourcesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewEditCtrlOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hiLoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem establishBackgroundHiLoLimitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem establishAm241ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem establishSrToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectOrDisconnectAPortCtrlPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem highVoltagePlateauCtrlLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setHighVoltageCtrlHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changePasswordToolStripMenuItem;
        private System.Windows.Forms.Label Source_Label;
        private System.Windows.Forms.ComboBox Source_ComboBox;
        private System.Windows.Forms.Label TypeOfRadiationLabel1;
        private System.Windows.Forms.Label TypeOfRadiationLabel2;
        private System.Windows.Forms.Button Determine_Efficiency_Button;
        private System.Windows.Forms.Label Ef_Label;
        private System.Windows.Forms.TextBox Minutes_TB;
        private System.Windows.Forms.TextBox Seconds_TB;
        private System.Windows.Forms.TextBox Num_Counts_TB;
        private System.Windows.Forms.Label Colon_Label;
        private System.Windows.Forms.Label Minutes_Label;
        private System.Windows.Forms.Label Seconds_Label;
        private System.Windows.Forms.Button Stop_Count_Button;
        private System.Windows.Forms.DataGridView Calibration_Results_GridView;
        private System.Windows.Forms.Button Determine_BG_Button;
        private System.Windows.Forms.Label for_label;
        private System.Windows.Forms.Label Sec_BG_Label;
        private System.Windows.Forms.Label Min_BG_Label;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Sec_BG_TB;
        private System.Windows.Forms.TextBox Min_BG_TB;
        private System.Windows.Forms.DataGridView Background_Results_GridView;
        private System.Windows.Forms.Label DABRAS_SN_Label;
        private System.Windows.Forms.Label DABRAS_Firmware_Label;
        private System.Windows.Forms.Label DABRAS_Status_Label;
        private System.Windows.Forms.ToolStripMenuItem addSourceCtrlAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSourceCtrlDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWebBasedSurveySystemF12ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRSOSharepointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRSOHomeF10ToolStripMenuItem;
        private System.Windows.Forms.TextBox BG_NumCounts_TB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton _100_200_Energy_Button;
        private System.Windows.Forms.RadioButton _1200_Energy_Button;
        private System.Windows.Forms.RadioButton _400_1200_Energy_Button;
        private System.Windows.Forms.RadioButton _200_400_Energy_Button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button SaveEfficiencyDataButton;
        private System.Windows.Forms.Button SaveBackgroundCalibrationButton;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setBackgroundTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setChiSquaredCalibrationRequirementsToolStripMenuItem;
        private System.Windows.Forms.Button ImageSaveButton;
        private System.Windows.Forms.ToolStripMenuItem saveEfficiencyDataCtrlKToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveBackgroundDataCtrlJToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveImageCtrlIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setModFactorRequirementsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setBuildingNumberToolStripMenuItem;
    }
}