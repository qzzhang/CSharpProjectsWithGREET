namespace DABRAS_Software
{
    partial class FormCLB
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Determine_BG_Button = new System.Windows.Forms.Button();
            this.for_label = new System.Windows.Forms.Label();
            this.Sec_BG_Label = new System.Windows.Forms.Label();
            this.Min_BG_Label = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Sec_BG_TB = new System.Windows.Forms.TextBox();
            this.Min_BG_TB = new System.Windows.Forms.TextBox();
            this.Background_Results_GridView = new System.Windows.Forms.DataGridView();
            this.BG_NumCounts_TB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveBackgroundCalibrationButton = new System.Windows.Forms.Button();
            this.btnManageSources = new System.Windows.Forms.Button();
            this.btnEstablishHiLoLimits = new System.Windows.Forms.Button();
            this.btnHighVoltage = new System.Windows.Forms.Button();
            this.ImageSaveButton = new System.Windows.Forms.Button();
            this.SaveEfficiencyDataButton = new System.Windows.Forms.Button();
            this.grpbBetaEnergy = new System.Windows.Forms.GroupBox();
            this._100_Energy_Button = new System.Windows.Forms.RadioButton();
            this._200_400_Energy_Button = new System.Windows.Forms.RadioButton();
            this._400_1200_Energy_Button = new System.Windows.Forms.RadioButton();
            this._1200_Energy_Button = new System.Windows.Forms.RadioButton();
            this._100_200_Energy_Button = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.Calibration_Results_GridView = new System.Windows.Forms.DataGridView();
            this.Stop_Count_Button = new System.Windows.Forms.Button();
            this.Seconds_Label = new System.Windows.Forms.Label();
            this.Minutes_Label = new System.Windows.Forms.Label();
            this.Colon_Label = new System.Windows.Forms.Label();
            this.EFF_NumCounts_TB = new System.Windows.Forms.TextBox();
            this.Sec_EFF_TB = new System.Windows.Forms.TextBox();
            this.Min_EFF_TB = new System.Windows.Forms.TextBox();
            this.Ef_Label = new System.Windows.Forms.Label();
            this.Determine_Efficiency_Button = new System.Windows.Forms.Button();
            this.TypeOfRadiationLabel2 = new System.Windows.Forms.Label();
            this.TypeOfRadiationLabel1 = new System.Windows.Forms.Label();
            this.Source_ComboBox = new System.Windows.Forms.ComboBox();
            this.Source_Label = new System.Windows.Forms.Label();
            this.clbTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.btnSetHVCtrl = new System.Windows.Forms.Button();
            this.BackgroudFootDGV = new System.Windows.Forms.DataGridView();
            this.EfficiencyFootDGV = new System.Windows.Forms.DataGridView();
            this.BGdate = new System.Windows.Forms.Label();
            this.lbl_BGcalDate = new System.Windows.Forms.Label();
            this.lbl_EFFcalDate = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Background_Results_GridView)).BeginInit();
            this.grpbBetaEnergy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Calibration_Results_GridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackgroudFootDGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EfficiencyFootDGV)).BeginInit();
            this.SuspendLayout();
            // 
            // Determine_BG_Button
            // 
            this.Determine_BG_Button.BackColor = System.Drawing.Color.LightCyan;
            this.Determine_BG_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Olive;
            this.Determine_BG_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.Determine_BG_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Determine_BG_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Determine_BG_Button.ForeColor = System.Drawing.Color.DarkGreen;
            this.Determine_BG_Button.Location = new System.Drawing.Point(5, 53);
            this.Determine_BG_Button.Margin = new System.Windows.Forms.Padding(4);
            this.Determine_BG_Button.Name = "Determine_BG_Button";
            this.Determine_BG_Button.Size = new System.Drawing.Size(213, 38);
            this.Determine_BG_Button.TabIndex = 8;
            this.Determine_BG_Button.Text = "Get Background Count";
            this.clbTooltip.SetToolTip(this.Determine_BG_Button, "Start the Background counting");
            this.Determine_BG_Button.UseVisualStyleBackColor = false;
            this.Determine_BG_Button.Click += new System.EventHandler(this.Determine_BG_Button_Click);
            // 
            // for_label
            // 
            this.for_label.AutoSize = true;
            this.for_label.Location = new System.Drawing.Point(330, 68);
            this.for_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.for_label.Name = "for_label";
            this.for_label.Size = new System.Drawing.Size(23, 16);
            this.for_label.TabIndex = 16;
            this.for_label.Text = "for";
            // 
            // Sec_BG_Label
            // 
            this.Sec_BG_Label.AutoSize = true;
            this.Sec_BG_Label.Location = new System.Drawing.Point(467, 45);
            this.Sec_BG_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Sec_BG_Label.Name = "Sec_BG_Label";
            this.Sec_BG_Label.Size = new System.Drawing.Size(70, 16);
            this.Sec_BG_Label.TabIndex = 21;
            this.Sec_BG_Label.Text = "Second(s)";
            // 
            // Min_BG_Label
            // 
            this.Min_BG_Label.AutoSize = true;
            this.Min_BG_Label.Location = new System.Drawing.Point(378, 45);
            this.Min_BG_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Min_BG_Label.Name = "Min_BG_Label";
            this.Min_BG_Label.Size = new System.Drawing.Size(61, 20);
            this.Min_BG_Label.TabIndex = 20;
            this.Min_BG_Label.Text = "Minute(s)";
            this.Min_BG_Label.UseCompatibleTextRendering = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(448, 66);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 16);
            this.label3.TabIndex = 19;
            this.label3.Text = ":";
            // 
            // Sec_BG_TB
            // 
            this.Sec_BG_TB.Location = new System.Drawing.Point(471, 65);
            this.Sec_BG_TB.Margin = new System.Windows.Forms.Padding(4);
            this.Sec_BG_TB.Name = "Sec_BG_TB";
            this.Sec_BG_TB.Size = new System.Drawing.Size(63, 22);
            this.Sec_BG_TB.TabIndex = 7;
            this.Sec_BG_TB.Text = "0";
            // 
            // Min_BG_TB
            // 
            this.Min_BG_TB.Location = new System.Drawing.Point(378, 65);
            this.Min_BG_TB.Margin = new System.Windows.Forms.Padding(4);
            this.Min_BG_TB.Name = "Min_BG_TB";
            this.Min_BG_TB.Size = new System.Drawing.Size(63, 22);
            this.Min_BG_TB.TabIndex = 6;
            this.Min_BG_TB.Text = "1";
            // 
            // Background_Results_GridView
            // 
            this.Background_Results_GridView.AllowUserToAddRows = false;
            this.Background_Results_GridView.AllowUserToDeleteRows = false;
            this.Background_Results_GridView.AllowUserToOrderColumns = true;
            this.Background_Results_GridView.AllowUserToResizeColumns = false;
            this.Background_Results_GridView.AllowUserToResizeRows = false;
            this.Background_Results_GridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.Background_Results_GridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Background_Results_GridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Background_Results_GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Background_Results_GridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.Background_Results_GridView.Location = new System.Drawing.Point(5, 116);
            this.Background_Results_GridView.Margin = new System.Windows.Forms.Padding(4);
            this.Background_Results_GridView.Name = "Background_Results_GridView";
            this.Background_Results_GridView.Size = new System.Drawing.Size(865, 263);
            this.Background_Results_GridView.TabIndex = 22;
            // 
            // BG_NumCounts_TB
            // 
            this.BG_NumCounts_TB.Location = new System.Drawing.Point(241, 65);
            this.BG_NumCounts_TB.Margin = new System.Windows.Forms.Padding(4);
            this.BG_NumCounts_TB.Name = "BG_NumCounts_TB";
            this.BG_NumCounts_TB.Size = new System.Drawing.Size(47, 22);
            this.BG_NumCounts_TB.TabIndex = 5;
            this.BG_NumCounts_TB.Text = "10";
            this.BG_NumCounts_TB.TextChanged += new System.EventHandler(this.BG_NumCounts_TB_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(297, 68);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 27;
            this.label1.Text = "counts of ";
            // 
            // SaveBackgroundCalibrationButton
            // 
            this.SaveBackgroundCalibrationButton.BackColor = System.Drawing.Color.LightCyan;
            this.SaveBackgroundCalibrationButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.SaveBackgroundCalibrationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveBackgroundCalibrationButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveBackgroundCalibrationButton.Location = new System.Drawing.Point(686, 53);
            this.SaveBackgroundCalibrationButton.Margin = new System.Windows.Forms.Padding(4);
            this.SaveBackgroundCalibrationButton.Name = "SaveBackgroundCalibrationButton";
            this.SaveBackgroundCalibrationButton.Size = new System.Drawing.Size(184, 38);
            this.SaveBackgroundCalibrationButton.TabIndex = 10;
            this.SaveBackgroundCalibrationButton.Text = "Save to CSV";
            this.clbTooltip.SetToolTip(this.SaveBackgroundCalibrationButton, "Save the background data into a csv file");
            this.SaveBackgroundCalibrationButton.UseVisualStyleBackColor = false;
            this.SaveBackgroundCalibrationButton.Click += new System.EventHandler(this.SaveBackgroundCalibrationButton_Click);
            // 
            // btnManageSources
            // 
            this.btnManageSources.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnManageSources.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnManageSources.FlatAppearance.CheckedBackColor = System.Drawing.SystemColors.Control;
            this.btnManageSources.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Olive;
            this.btnManageSources.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.btnManageSources.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManageSources.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManageSources.ForeColor = System.Drawing.Color.Blue;
            this.btnManageSources.Location = new System.Drawing.Point(953, 0);
            this.btnManageSources.Name = "btnManageSources";
            this.btnManageSources.Size = new System.Drawing.Size(219, 35);
            this.btnManageSources.TabIndex = 103;
            this.btnManageSources.Text = "Manage Sources";
            this.clbTooltip.SetToolTip(this.btnManageSources, "To View/Edit, Add or Delete a radionuclide or a source");
            this.btnManageSources.UseVisualStyleBackColor = false;
            this.btnManageSources.Click += new System.EventHandler(this.btnManageSources_Click);
            // 
            // btnEstablishHiLoLimits
            // 
            this.btnEstablishHiLoLimits.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnEstablishHiLoLimits.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnEstablishHiLoLimits.FlatAppearance.CheckedBackColor = System.Drawing.SystemColors.Control;
            this.btnEstablishHiLoLimits.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Olive;
            this.btnEstablishHiLoLimits.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.btnEstablishHiLoLimits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEstablishHiLoLimits.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEstablishHiLoLimits.ForeColor = System.Drawing.Color.Blue;
            this.btnEstablishHiLoLimits.Location = new System.Drawing.Point(729, 0);
            this.btnEstablishHiLoLimits.Name = "btnEstablishHiLoLimits";
            this.btnEstablishHiLoLimits.Size = new System.Drawing.Size(224, 35);
            this.btnEstablishHiLoLimits.TabIndex = 104;
            this.btnEstablishHiLoLimits.Text = "Establish HiLo Limits";
            this.clbTooltip.SetToolTip(this.btnEstablishHiLoLimits, "Establish Background, Am-241 and Sr-90 Hi-Lo Limits");
            this.btnEstablishHiLoLimits.UseVisualStyleBackColor = false;
            this.btnEstablishHiLoLimits.Click += new System.EventHandler(this.btnEstablishHiLoLimits_Click);
            // 
            // btnHighVoltage
            // 
            this.btnHighVoltage.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnHighVoltage.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnHighVoltage.FlatAppearance.CheckedBackColor = System.Drawing.SystemColors.Control;
            this.btnHighVoltage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Olive;
            this.btnHighVoltage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.btnHighVoltage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHighVoltage.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHighVoltage.ForeColor = System.Drawing.Color.Blue;
            this.btnHighVoltage.Location = new System.Drawing.Point(505, 0);
            this.btnHighVoltage.Name = "btnHighVoltage";
            this.btnHighVoltage.Size = new System.Drawing.Size(224, 35);
            this.btnHighVoltage.TabIndex = 105;
            this.btnHighVoltage.Text = "High Voltage Plateau";
            this.clbTooltip.SetToolTip(this.btnHighVoltage, "High voltage plateau and set high voltage control");
            this.btnHighVoltage.UseVisualStyleBackColor = false;
            this.btnHighVoltage.Click += new System.EventHandler(this.btnHighVoltage_Click);
            // 
            // ImageSaveButton
            // 
            this.ImageSaveButton.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ImageSaveButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.ImageSaveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.ImageSaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImageSaveButton.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImageSaveButton.Location = new System.Drawing.Point(954, 341);
            this.ImageSaveButton.Margin = new System.Windows.Forms.Padding(4);
            this.ImageSaveButton.Name = "ImageSaveButton";
            this.ImageSaveButton.Size = new System.Drawing.Size(219, 71);
            this.ImageSaveButton.TabIndex = 122;
            this.ImageSaveButton.Text = "Save Image";
            this.clbTooltip.SetToolTip(this.ImageSaveButton, "Save the results to an image file");
            this.ImageSaveButton.UseVisualStyleBackColor = false;
            this.ImageSaveButton.Click += new System.EventHandler(this.ImageSaveButton_Click);
            // 
            // SaveEfficiencyDataButton
            // 
            this.SaveEfficiencyDataButton.BackColor = System.Drawing.Color.LemonChiffon;
            this.SaveEfficiencyDataButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.SaveEfficiencyDataButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveEfficiencyDataButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveEfficiencyDataButton.Location = new System.Drawing.Point(859, 503);
            this.SaveEfficiencyDataButton.Margin = new System.Windows.Forms.Padding(4);
            this.SaveEfficiencyDataButton.Name = "SaveEfficiencyDataButton";
            this.SaveEfficiencyDataButton.Size = new System.Drawing.Size(140, 42);
            this.SaveEfficiencyDataButton.TabIndex = 117;
            this.SaveEfficiencyDataButton.Text = "Save to CSV";
            this.clbTooltip.SetToolTip(this.SaveEfficiencyDataButton, "Save the efficiency data into a csv file");
            this.SaveEfficiencyDataButton.UseVisualStyleBackColor = false;
            this.SaveEfficiencyDataButton.Click += new System.EventHandler(this.SaveEfficiencyDataButton_Click);
            // 
            // grpbBetaEnergy
            // 
            this.grpbBetaEnergy.Controls.Add(this._100_Energy_Button);
            this.grpbBetaEnergy.Controls.Add(this._200_400_Energy_Button);
            this.grpbBetaEnergy.Controls.Add(this._400_1200_Energy_Button);
            this.grpbBetaEnergy.Controls.Add(this._1200_Energy_Button);
            this.grpbBetaEnergy.Controls.Add(this._100_200_Energy_Button);
            this.grpbBetaEnergy.Controls.Add(this.label2);
            this.grpbBetaEnergy.Location = new System.Drawing.Point(523, 459);
            this.grpbBetaEnergy.Margin = new System.Windows.Forms.Padding(4);
            this.grpbBetaEnergy.Name = "grpbBetaEnergy";
            this.grpbBetaEnergy.Padding = new System.Windows.Forms.Padding(4);
            this.grpbBetaEnergy.Size = new System.Drawing.Size(335, 74);
            this.grpbBetaEnergy.TabIndex = 109;
            this.grpbBetaEnergy.TabStop = false;
            // 
            // _100_Energy_Button
            // 
            this._100_Energy_Button.AutoSize = true;
            this._100_Energy_Button.Enabled = false;
            this._100_Energy_Button.Location = new System.Drawing.Point(9, 23);
            this._100_Energy_Button.Name = "_100_Energy_Button";
            this._100_Energy_Button.Size = new System.Drawing.Size(82, 20);
            this._100_Energy_Button.TabIndex = 126;
            this._100_Energy_Button.Text = "<100 KeV";
            this._100_Energy_Button.UseVisualStyleBackColor = true;
            // 
            // _200_400_Energy_Button
            // 
            this._200_400_Energy_Button.AutoSize = true;
            this._200_400_Energy_Button.Location = new System.Drawing.Point(223, 23);
            this._200_400_Energy_Button.Margin = new System.Windows.Forms.Padding(4);
            this._200_400_Energy_Button.Name = "_200_400_Energy_Button";
            this._200_400_Energy_Button.Size = new System.Drawing.Size(100, 20);
            this._200_400_Energy_Button.TabIndex = 2;
            this._200_400_Energy_Button.TabStop = true;
            this._200_400_Energy_Button.Text = "201-400 KeV";
            this._200_400_Energy_Button.UseVisualStyleBackColor = true;
            this._200_400_Energy_Button.CheckedChanged += new System.EventHandler(this._200_400_Energy_Button_CheckedChanged);
            // 
            // _400_1200_Energy_Button
            // 
            this._400_1200_Energy_Button.AutoSize = true;
            this._400_1200_Energy_Button.Location = new System.Drawing.Point(9, 50);
            this._400_1200_Energy_Button.Margin = new System.Windows.Forms.Padding(4);
            this._400_1200_Energy_Button.Name = "_400_1200_Energy_Button";
            this._400_1200_Energy_Button.Size = new System.Drawing.Size(107, 20);
            this._400_1200_Energy_Button.TabIndex = 1;
            this._400_1200_Energy_Button.TabStop = true;
            this._400_1200_Energy_Button.Text = "400-1200 KeV";
            this._400_1200_Energy_Button.UseVisualStyleBackColor = true;
            this._400_1200_Energy_Button.CheckedChanged += new System.EventHandler(this._400_1200_Energy_Button_CheckedChanged);
            // 
            // _1200_Energy_Button
            // 
            this._1200_Energy_Button.AutoSize = true;
            this._1200_Energy_Button.Location = new System.Drawing.Point(160, 50);
            this._1200_Energy_Button.Margin = new System.Windows.Forms.Padding(4);
            this._1200_Energy_Button.Name = "_1200_Energy_Button";
            this._1200_Energy_Button.Size = new System.Drawing.Size(89, 20);
            this._1200_Energy_Button.TabIndex = 3;
            this._1200_Energy_Button.TabStop = true;
            this._1200_Energy_Button.Text = ">1200 KeV";
            this._1200_Energy_Button.UseVisualStyleBackColor = true;
            this._1200_Energy_Button.CheckedChanged += new System.EventHandler(this._1200_Energy_Button_CheckedChanged);
            // 
            // _100_200_Energy_Button
            // 
            this._100_200_Energy_Button.AutoSize = true;
            this._100_200_Energy_Button.Location = new System.Drawing.Point(103, 23);
            this._100_200_Energy_Button.Margin = new System.Windows.Forms.Padding(4);
            this._100_200_Energy_Button.Name = "_100_200_Energy_Button";
            this._100_200_Energy_Button.Size = new System.Drawing.Size(100, 20);
            this._100_200_Energy_Button.TabIndex = 0;
            this._100_200_Energy_Button.TabStop = true;
            this._100_200_Energy_Button.Text = "100-200 KeV";
            this._100_200_Energy_Button.UseVisualStyleBackColor = true;
            this._100_200_Energy_Button.CheckedChanged += new System.EventHandler(this._100_200_Energy_Button_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, -3);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 16);
            this.label2.TabIndex = 28;
            this.label2.Text = "Beta Energy Level:";
            // 
            // Calibration_Results_GridView
            // 
            this.Calibration_Results_GridView.AllowUserToAddRows = false;
            this.Calibration_Results_GridView.AllowUserToDeleteRows = false;
            this.Calibration_Results_GridView.AllowUserToOrderColumns = true;
            this.Calibration_Results_GridView.AllowUserToResizeColumns = false;
            this.Calibration_Results_GridView.AllowUserToResizeRows = false;
            this.Calibration_Results_GridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.Calibration_Results_GridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Calibration_Results_GridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.Calibration_Results_GridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Calibration_Results_GridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.Calibration_Results_GridView.Location = new System.Drawing.Point(2, 552);
            this.Calibration_Results_GridView.Margin = new System.Windows.Forms.Padding(4);
            this.Calibration_Results_GridView.Name = "Calibration_Results_GridView";
            this.Calibration_Results_GridView.RowHeadersWidth = 40;
            this.Calibration_Results_GridView.Size = new System.Drawing.Size(997, 265);
            this.Calibration_Results_GridView.TabIndex = 110;
            // 
            // Stop_Count_Button
            // 
            this.Stop_Count_Button.BackColor = System.Drawing.Color.LightSteelBlue;
            this.Stop_Count_Button.Enabled = false;
            this.Stop_Count_Button.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.Stop_Count_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.Stop_Count_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Stop_Count_Button.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Stop_Count_Button.ForeColor = System.Drawing.Color.Red;
            this.Stop_Count_Button.Location = new System.Drawing.Point(954, 259);
            this.Stop_Count_Button.Margin = new System.Windows.Forms.Padding(4);
            this.Stop_Count_Button.Name = "Stop_Count_Button";
            this.Stop_Count_Button.Size = new System.Drawing.Size(219, 73);
            this.Stop_Count_Button.TabIndex = 119;
            this.Stop_Count_Button.Text = "Stop Counting";
            this.clbTooltip.SetToolTip(this.Stop_Count_Button, "Stop the current activities");
            this.Stop_Count_Button.UseVisualStyleBackColor = false;
            this.Stop_Count_Button.Click += new System.EventHandler(this.Stop_Count_Button_Click);
            // 
            // Seconds_Label
            // 
            this.Seconds_Label.AutoSize = true;
            this.Seconds_Label.Location = new System.Drawing.Point(448, 477);
            this.Seconds_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Seconds_Label.Name = "Seconds_Label";
            this.Seconds_Label.Size = new System.Drawing.Size(70, 16);
            this.Seconds_Label.TabIndex = 121;
            this.Seconds_Label.Text = "Second(s)";
            // 
            // Minutes_Label
            // 
            this.Minutes_Label.AutoSize = true;
            this.Minutes_Label.Location = new System.Drawing.Point(372, 477);
            this.Minutes_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Minutes_Label.Name = "Minutes_Label";
            this.Minutes_Label.Size = new System.Drawing.Size(62, 16);
            this.Minutes_Label.TabIndex = 120;
            this.Minutes_Label.Text = "Minute(s)";
            // 
            // Colon_Label
            // 
            this.Colon_Label.AutoSize = true;
            this.Colon_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Colon_Label.Location = new System.Drawing.Point(441, 498);
            this.Colon_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Colon_Label.Name = "Colon_Label";
            this.Colon_Label.Size = new System.Drawing.Size(12, 16);
            this.Colon_Label.TabIndex = 118;
            this.Colon_Label.Text = ":";
            // 
            // EFF_NumCounts_TB
            // 
            this.EFF_NumCounts_TB.Location = new System.Drawing.Point(238, 494);
            this.EFF_NumCounts_TB.Margin = new System.Windows.Forms.Padding(4);
            this.EFF_NumCounts_TB.Name = "EFF_NumCounts_TB";
            this.EFF_NumCounts_TB.Size = new System.Drawing.Size(47, 22);
            this.EFF_NumCounts_TB.TabIndex = 107;
            this.EFF_NumCounts_TB.Text = "10";
            this.EFF_NumCounts_TB.TextChanged += new System.EventHandler(this.Num_Counts_TB_TextChanged);
            // 
            // Sec_EFF_TB
            // 
            this.Sec_EFF_TB.Location = new System.Drawing.Point(452, 496);
            this.Sec_EFF_TB.Margin = new System.Windows.Forms.Padding(4);
            this.Sec_EFF_TB.Name = "Sec_EFF_TB";
            this.Sec_EFF_TB.Size = new System.Drawing.Size(63, 22);
            this.Sec_EFF_TB.TabIndex = 112;
            this.Sec_EFF_TB.Text = "0";
            // 
            // Min_EFF_TB
            // 
            this.Min_EFF_TB.Location = new System.Drawing.Point(372, 496);
            this.Min_EFF_TB.Margin = new System.Windows.Forms.Padding(4);
            this.Min_EFF_TB.Name = "Min_EFF_TB";
            this.Min_EFF_TB.Size = new System.Drawing.Size(63, 22);
            this.Min_EFF_TB.TabIndex = 111;
            this.Min_EFF_TB.Text = "1";
            // 
            // Ef_Label
            // 
            this.Ef_Label.AutoSize = true;
            this.Ef_Label.Location = new System.Drawing.Point(293, 498);
            this.Ef_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Ef_Label.Name = "Ef_Label";
            this.Ef_Label.Size = new System.Drawing.Size(64, 16);
            this.Ef_Label.TabIndex = 116;
            this.Ef_Label.Text = "counts of ";
            // 
            // Determine_Efficiency_Button
            // 
            this.Determine_Efficiency_Button.BackColor = System.Drawing.Color.LemonChiffon;
            this.Determine_Efficiency_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Olive;
            this.Determine_Efficiency_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.Determine_Efficiency_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Determine_Efficiency_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Determine_Efficiency_Button.ForeColor = System.Drawing.Color.DarkGreen;
            this.Determine_Efficiency_Button.Location = new System.Drawing.Point(3, 488);
            this.Determine_Efficiency_Button.Margin = new System.Windows.Forms.Padding(4);
            this.Determine_Efficiency_Button.Name = "Determine_Efficiency_Button";
            this.Determine_Efficiency_Button.Size = new System.Drawing.Size(227, 41);
            this.Determine_Efficiency_Button.TabIndex = 114;
            this.Determine_Efficiency_Button.Text = "Scan Detector Efficiency";
            this.clbTooltip.SetToolTip(this.Determine_Efficiency_Button, "Start getting the efficiencies");
            this.Determine_Efficiency_Button.UseVisualStyleBackColor = false;
            this.Determine_Efficiency_Button.Click += new System.EventHandler(this.Determine_Efficiency_Button_Click);
            // 
            // TypeOfRadiationLabel2
            // 
            this.TypeOfRadiationLabel2.AutoSize = true;
            this.TypeOfRadiationLabel2.Location = new System.Drawing.Point(359, 454);
            this.TypeOfRadiationLabel2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TypeOfRadiationLabel2.Name = "TypeOfRadiationLabel2";
            this.TypeOfRadiationLabel2.Size = new System.Drawing.Size(42, 16);
            this.TypeOfRadiationLabel2.TabIndex = 115;
            this.TypeOfRadiationLabel2.Text = "NULL";
            this.TypeOfRadiationLabel2.Visible = false;
            // 
            // TypeOfRadiationLabel1
            // 
            this.TypeOfRadiationLabel1.AutoSize = true;
            this.TypeOfRadiationLabel1.Location = new System.Drawing.Point(304, 454);
            this.TypeOfRadiationLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TypeOfRadiationLabel1.Name = "TypeOfRadiationLabel1";
            this.TypeOfRadiationLabel1.Size = new System.Drawing.Size(42, 16);
            this.TypeOfRadiationLabel1.TabIndex = 113;
            this.TypeOfRadiationLabel1.Text = "NULL";
            // 
            // Source_ComboBox
            // 
            this.Source_ComboBox.FormattingEnabled = true;
            this.Source_ComboBox.Location = new System.Drawing.Point(111, 451);
            this.Source_ComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.Source_ComboBox.Name = "Source_ComboBox";
            this.Source_ComboBox.Size = new System.Drawing.Size(160, 24);
            this.Source_ComboBox.TabIndex = 106;
            this.Source_ComboBox.SelectedIndexChanged += new System.EventHandler(this.Source_ComboBox_SelectedIndexChanged);
            // 
            // Source_Label
            // 
            this.Source_Label.AutoSize = true;
            this.Source_Label.Location = new System.Drawing.Point(8, 454);
            this.Source_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Source_Label.Name = "Source_Label";
            this.Source_Label.Size = new System.Drawing.Size(98, 16);
            this.Source_Label.TabIndex = 108;
            this.Source_Label.Text = "Select Source: ";
            // 
            // btnSetHVCtrl
            // 
            this.btnSetHVCtrl.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnSetHVCtrl.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnSetHVCtrl.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.btnSetHVCtrl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetHVCtrl.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetHVCtrl.ForeColor = System.Drawing.Color.DarkMagenta;
            this.btnSetHVCtrl.Location = new System.Drawing.Point(991, 153);
            this.btnSetHVCtrl.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetHVCtrl.Name = "btnSetHVCtrl";
            this.btnSetHVCtrl.Size = new System.Drawing.Size(181, 77);
            this.btnSetHVCtrl.TabIndex = 125;
            this.btnSetHVCtrl.Text = "Set HV Control";
            this.btnSetHVCtrl.UseVisualStyleBackColor = false;
            this.btnSetHVCtrl.Click += new System.EventHandler(this.btnSetHVCtrl_Click);
            // 
            // BackgroudFootDGV
            // 
            this.BackgroudFootDGV.AllowUserToAddRows = false;
            this.BackgroudFootDGV.AllowUserToDeleteRows = false;
            this.BackgroudFootDGV.AllowUserToResizeColumns = false;
            this.BackgroudFootDGV.AllowUserToResizeRows = false;
            this.BackgroudFootDGV.BackgroundColor = System.Drawing.SystemColors.Control;
            this.BackgroudFootDGV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.BackgroudFootDGV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.BackgroudFootDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.BackgroudFootDGV.ColumnHeadersVisible = false;
            this.BackgroudFootDGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.BackgroudFootDGV.Location = new System.Drawing.Point(5, 383);
            this.BackgroudFootDGV.Margin = new System.Windows.Forms.Padding(4, 0, 4, 4);
            this.BackgroudFootDGV.Name = "BackgroudFootDGV";
            this.BackgroudFootDGV.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.BackgroudFootDGV.Size = new System.Drawing.Size(865, 51);
            this.BackgroudFootDGV.TabIndex = 126;
            // 
            // EfficiencyFootDGV
            // 
            this.EfficiencyFootDGV.AllowUserToAddRows = false;
            this.EfficiencyFootDGV.AllowUserToDeleteRows = false;
            this.EfficiencyFootDGV.AllowUserToResizeColumns = false;
            this.EfficiencyFootDGV.AllowUserToResizeRows = false;
            this.EfficiencyFootDGV.BackgroundColor = System.Drawing.SystemColors.Control;
            this.EfficiencyFootDGV.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.EfficiencyFootDGV.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.EfficiencyFootDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.EfficiencyFootDGV.ColumnHeadersVisible = false;
            this.EfficiencyFootDGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.EfficiencyFootDGV.Location = new System.Drawing.Point(2, 821);
            this.EfficiencyFootDGV.Margin = new System.Windows.Forms.Padding(4, 0, 4, 4);
            this.EfficiencyFootDGV.Name = "EfficiencyFootDGV";
            this.EfficiencyFootDGV.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.EfficiencyFootDGV.Size = new System.Drawing.Size(997, 96);
            this.EfficiencyFootDGV.TabIndex = 127;
            // 
            // BGdate
            // 
            this.BGdate.AutoSize = true;
            this.BGdate.Location = new System.Drawing.Point(317, 96);
            this.BGdate.Name = "BGdate";
            this.BGdate.Size = new System.Drawing.Size(107, 16);
            this.BGdate.TabIndex = 128;
            this.BGdate.Text = "Calibration Date:";
            // 
            // lbl_BGcalDate
            // 
            this.lbl_BGcalDate.AutoSize = true;
            this.lbl_BGcalDate.Location = new System.Drawing.Point(430, 95);
            this.lbl_BGcalDate.Name = "lbl_BGcalDate";
            this.lbl_BGcalDate.Size = new System.Drawing.Size(45, 16);
            this.lbl_BGcalDate.TabIndex = 129;
            this.lbl_BGcalDate.Text = "label5";
            // 
            // lbl_EFFcalDate
            // 
            this.lbl_EFFcalDate.AutoSize = true;
            this.lbl_EFFcalDate.Location = new System.Drawing.Point(430, 530);
            this.lbl_EFFcalDate.Name = "lbl_EFFcalDate";
            this.lbl_EFFcalDate.Size = new System.Drawing.Size(45, 16);
            this.lbl_EFFcalDate.TabIndex = 131;
            this.lbl_EFFcalDate.Text = "label5";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(317, 531);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 16);
            this.label5.TabIndex = 130;
            this.label5.Text = "Calibration Date:";
            // 
            // FormCLB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.ClientSize = new System.Drawing.Size(1186, 782);
            this.Controls.Add(this.lbl_EFFcalDate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lbl_BGcalDate);
            this.Controls.Add(this.BGdate);
            this.Controls.Add(this.EfficiencyFootDGV);
            this.Controls.Add(this.BackgroudFootDGV);
            this.Controls.Add(this.Background_Results_GridView);
            this.Controls.Add(this.btnSetHVCtrl);
            this.Controls.Add(this.ImageSaveButton);
            this.Controls.Add(this.SaveEfficiencyDataButton);
            this.Controls.Add(this.grpbBetaEnergy);
            this.Controls.Add(this.Calibration_Results_GridView);
            this.Controls.Add(this.Stop_Count_Button);
            this.Controls.Add(this.Seconds_Label);
            this.Controls.Add(this.Minutes_Label);
            this.Controls.Add(this.Colon_Label);
            this.Controls.Add(this.EFF_NumCounts_TB);
            this.Controls.Add(this.Sec_EFF_TB);
            this.Controls.Add(this.Min_EFF_TB);
            this.Controls.Add(this.Ef_Label);
            this.Controls.Add(this.Determine_Efficiency_Button);
            this.Controls.Add(this.TypeOfRadiationLabel2);
            this.Controls.Add(this.TypeOfRadiationLabel1);
            this.Controls.Add(this.Source_ComboBox);
            this.Controls.Add(this.Source_Label);
            this.Controls.Add(this.btnHighVoltage);
            this.Controls.Add(this.btnEstablishHiLoLimits);
            this.Controls.Add(this.btnManageSources);
            this.Controls.Add(this.SaveBackgroundCalibrationButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BG_NumCounts_TB);
            this.Controls.Add(this.Sec_BG_Label);
            this.Controls.Add(this.Min_BG_Label);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Sec_BG_TB);
            this.Controls.Add(this.Min_BG_TB);
            this.Controls.Add(this.for_label);
            this.Controls.Add(this.Determine_BG_Button);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Maroon;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormCLB";
            this.Text = "Calibration";
            this.clbTooltip.SetToolTip(this, "Save the screenshot");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCLB_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.CalibrationForm_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.Background_Results_GridView)).EndInit();
            this.grpbBetaEnergy.ResumeLayout(false);
            this.grpbBetaEnergy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Calibration_Results_GridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackgroudFootDGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EfficiencyFootDGV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Determine_BG_Button;
        private System.Windows.Forms.Label for_label;
        private System.Windows.Forms.Label Sec_BG_Label;
        private System.Windows.Forms.Label Min_BG_Label;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Sec_BG_TB;
        private System.Windows.Forms.TextBox Min_BG_TB;
        private System.Windows.Forms.DataGridView Background_Results_GridView;
        private System.Windows.Forms.TextBox BG_NumCounts_TB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SaveBackgroundCalibrationButton;
        private System.Windows.Forms.Button btnManageSources;
        private System.Windows.Forms.Button btnEstablishHiLoLimits;
        private System.Windows.Forms.Button btnHighVoltage;
        private System.Windows.Forms.Button ImageSaveButton;
        private System.Windows.Forms.Button SaveEfficiencyDataButton;
        private System.Windows.Forms.GroupBox grpbBetaEnergy;
        private System.Windows.Forms.RadioButton _200_400_Energy_Button;
        private System.Windows.Forms.RadioButton _400_1200_Energy_Button;
        private System.Windows.Forms.RadioButton _1200_Energy_Button;
        private System.Windows.Forms.RadioButton _100_200_Energy_Button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView Calibration_Results_GridView;
        private System.Windows.Forms.Button Stop_Count_Button;
        private System.Windows.Forms.Label Seconds_Label;
        private System.Windows.Forms.Label Minutes_Label;
        private System.Windows.Forms.Label Colon_Label;
        private System.Windows.Forms.TextBox EFF_NumCounts_TB;
        private System.Windows.Forms.TextBox Sec_EFF_TB;
        private System.Windows.Forms.TextBox Min_EFF_TB;
        private System.Windows.Forms.Label Ef_Label;
        private System.Windows.Forms.Button Determine_Efficiency_Button;
        private System.Windows.Forms.Label TypeOfRadiationLabel2;
        private System.Windows.Forms.Label TypeOfRadiationLabel1;
        private System.Windows.Forms.ComboBox Source_ComboBox;
        private System.Windows.Forms.Label Source_Label;
        private System.Windows.Forms.ToolTip clbTooltip;
        private System.Windows.Forms.Button btnSetHVCtrl;
        private System.Windows.Forms.RadioButton _100_Energy_Button;
        private System.Windows.Forms.DataGridView BackgroudFootDGV;
        private System.Windows.Forms.DataGridView EfficiencyFootDGV;
        private System.Windows.Forms.Label BGdate;
        private System.Windows.Forms.Label lbl_BGcalDate;
        private System.Windows.Forms.Label lbl_EFFcalDate;
        private System.Windows.Forms.Label label5;
    }
}