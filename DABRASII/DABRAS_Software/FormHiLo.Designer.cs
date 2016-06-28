namespace DABRAS_Software
{
    partial class FormHiLo
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
            this.Override_CB = new System.Windows.Forms.CheckBox();
            this.StopButton = new System.Windows.Forms.Button();
            this.Recompute_Limits_Button = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdBtnBeta = new System.Windows.Forms.RadioButton();
            this.rdBtnAlpha = new System.Windows.Forms.RadioButton();
            this.rdBtnBackground = new System.Windows.Forms.RadioButton();
            this.lbl_HiLocalDate = new System.Windows.Forms.Label();
            this.BGdate = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.HiLo_Results_DataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CollectLabel
            // 
            this.CollectLabel.AutoSize = true;
            this.CollectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CollectLabel.Location = new System.Drawing.Point(294, 92);
            this.CollectLabel.Name = "CollectLabel";
            this.CollectLabel.Size = new System.Drawing.Size(61, 20);
            this.CollectLabel.TabIndex = 2;
            this.CollectLabel.Text = "Collect ";
            // 
            // Num_Counts_TB
            // 
            this.Num_Counts_TB.Location = new System.Drawing.Point(361, 94);
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
            this.label1.Location = new System.Drawing.Point(420, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Counts of ";
            // 
            // Min_TB
            // 
            this.Min_TB.Location = new System.Drawing.Point(508, 94);
            this.Min_TB.Name = "Min_TB";
            this.Min_TB.Size = new System.Drawing.Size(38, 20);
            this.Min_TB.TabIndex = 5;
            this.Min_TB.Text = "1";
            // 
            // Colon_Label
            // 
            this.Colon_Label.AutoSize = true;
            this.Colon_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Colon_Label.Location = new System.Drawing.Point(548, 94);
            this.Colon_Label.Name = "Colon_Label";
            this.Colon_Label.Size = new System.Drawing.Size(13, 20);
            this.Colon_Label.TabIndex = 6;
            this.Colon_Label.Text = ":";
            // 
            // Sec_TB
            // 
            this.Sec_TB.Location = new System.Drawing.Point(562, 94);
            this.Sec_TB.Name = "Sec_TB";
            this.Sec_TB.Size = new System.Drawing.Size(38, 20);
            this.Sec_TB.TabIndex = 7;
            this.Sec_TB.Text = "0";
            // 
            // MinLabel
            // 
            this.MinLabel.AutoSize = true;
            this.MinLabel.Location = new System.Drawing.Point(505, 78);
            this.MinLabel.Name = "MinLabel";
            this.MinLabel.Size = new System.Drawing.Size(23, 13);
            this.MinLabel.TabIndex = 8;
            this.MinLabel.Text = "min";
            // 
            // SecLabel
            // 
            this.SecLabel.AutoSize = true;
            this.SecLabel.Location = new System.Drawing.Point(559, 78);
            this.SecLabel.Name = "SecLabel";
            this.SecLabel.Size = new System.Drawing.Size(24, 13);
            this.SecLabel.TabIndex = 9;
            this.SecLabel.Text = "sec";
            // 
            // AquireButton
            // 
            this.AquireButton.BackColor = System.Drawing.Color.SkyBlue;
            this.AquireButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.AquireButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.AquireButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AquireButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AquireButton.ForeColor = System.Drawing.Color.Green;
            this.AquireButton.Location = new System.Drawing.Point(606, 84);
            this.AquireButton.Name = "AquireButton";
            this.AquireButton.Size = new System.Drawing.Size(124, 38);
            this.AquireButton.TabIndex = 10;
            this.AquireButton.Text = "Start Counting";
            this.AquireButton.UseVisualStyleBackColor = false;
            this.AquireButton.Click += new System.EventHandler(this.AquireButton_Click);
            // 
            // StdDevButton
            // 
            this.StdDevButton.AutoSize = true;
            this.StdDevButton.Checked = true;
            this.StdDevButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StdDevButton.Location = new System.Drawing.Point(10, 515);
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
            this.PercentButton.Location = new System.Drawing.Point(10, 538);
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
            this.StdDev_TB.Location = new System.Drawing.Point(182, 517);
            this.StdDev_TB.Name = "StdDev_TB";
            this.StdDev_TB.Size = new System.Drawing.Size(52, 20);
            this.StdDev_TB.TabIndex = 13;
            this.StdDev_TB.Text = "3";
            // 
            // Percent_TB
            // 
            this.Percent_TB.Location = new System.Drawing.Point(182, 542);
            this.Percent_TB.Name = "Percent_TB";
            this.Percent_TB.Size = new System.Drawing.Size(52, 20);
            this.Percent_TB.TabIndex = 14;
            this.Percent_TB.Text = "30";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(238, 517);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 20);
            this.label2.TabIndex = 15;
            this.label2.Text = "Standard deviations";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(239, 543);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 20);
            this.label3.TabIndex = 16;
            this.label3.Text = "% difference";
            // 
            // HiLo_Results_DataGridView
            // 
            this.HiLo_Results_DataGridView.AllowDrop = true;
            this.HiLo_Results_DataGridView.AllowUserToAddRows = false;
            this.HiLo_Results_DataGridView.AllowUserToDeleteRows = false;
            this.HiLo_Results_DataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.HiLo_Results_DataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.HiLo_Results_DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.HiLo_Results_DataGridView.Location = new System.Drawing.Point(10, 128);
            this.HiLo_Results_DataGridView.Name = "HiLo_Results_DataGridView";
            this.HiLo_Results_DataGridView.Size = new System.Drawing.Size(942, 356);
            this.HiLo_Results_DataGridView.TabIndex = 17;
            // 
            // Net_CPM_Label
            // 
            this.Net_CPM_Label.AutoSize = true;
            this.Net_CPM_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Net_CPM_Label.Location = new System.Drawing.Point(201, 575);
            this.Net_CPM_Label.Name = "Net_CPM_Label";
            this.Net_CPM_Label.Size = new System.Drawing.Size(76, 20);
            this.Net_CPM_Label.TabIndex = 18;
            this.Net_CPM_Label.Text = "Net CPM:";
            // 
            // LL_Beta_Label
            // 
            this.LL_Beta_Label.AutoSize = true;
            this.LL_Beta_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LL_Beta_Label.Location = new System.Drawing.Point(6, 624);
            this.LL_Beta_Label.Name = "LL_Beta_Label";
            this.LL_Beta_Label.Size = new System.Drawing.Size(184, 20);
            this.LL_Beta_Label.TabIndex = 19;
            this.LL_Beta_Label.Text = "Low Limit, Beta Channel:";
            // 
            // LL_Alpha_Label
            // 
            this.LL_Alpha_Label.AutoSize = true;
            this.LL_Alpha_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LL_Alpha_Label.Location = new System.Drawing.Point(6, 596);
            this.LL_Alpha_Label.Name = "LL_Alpha_Label";
            this.LL_Alpha_Label.Size = new System.Drawing.Size(191, 20);
            this.LL_Alpha_Label.TabIndex = 20;
            this.LL_Alpha_Label.Text = "Low Limit, Alpha Channel:";
            // 
            // LL_Alpha_TB
            // 
            this.LL_Alpha_TB.Enabled = false;
            this.LL_Alpha_TB.Location = new System.Drawing.Point(203, 598);
            this.LL_Alpha_TB.Name = "LL_Alpha_TB";
            this.LL_Alpha_TB.Size = new System.Drawing.Size(100, 20);
            this.LL_Alpha_TB.TabIndex = 21;
            // 
            // LL_Beta_TB
            // 
            this.LL_Beta_TB.Enabled = false;
            this.LL_Beta_TB.Location = new System.Drawing.Point(203, 624);
            this.LL_Beta_TB.Name = "LL_Beta_TB";
            this.LL_Beta_TB.Size = new System.Drawing.Size(100, 20);
            this.LL_Beta_TB.TabIndex = 22;
            // 
            // HL_Beta_TB
            // 
            this.HL_Beta_TB.Enabled = false;
            this.HL_Beta_TB.Location = new System.Drawing.Point(506, 624);
            this.HL_Beta_TB.Name = "HL_Beta_TB";
            this.HL_Beta_TB.Size = new System.Drawing.Size(100, 20);
            this.HL_Beta_TB.TabIndex = 26;
            // 
            // HL_Alpha_TB
            // 
            this.HL_Alpha_TB.Enabled = false;
            this.HL_Alpha_TB.Location = new System.Drawing.Point(506, 598);
            this.HL_Alpha_TB.Name = "HL_Alpha_TB";
            this.HL_Alpha_TB.Size = new System.Drawing.Size(100, 20);
            this.HL_Alpha_TB.TabIndex = 25;
            // 
            // HL_Alpha_Label
            // 
            this.HL_Alpha_Label.AutoSize = true;
            this.HL_Alpha_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HL_Alpha_Label.Location = new System.Drawing.Point(309, 596);
            this.HL_Alpha_Label.Name = "HL_Alpha_Label";
            this.HL_Alpha_Label.Size = new System.Drawing.Size(195, 20);
            this.HL_Alpha_Label.TabIndex = 24;
            this.HL_Alpha_Label.Text = "High Limit, Alpha Channel:";
            // 
            // HL_Beta_Label
            // 
            this.HL_Beta_Label.AutoSize = true;
            this.HL_Beta_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HL_Beta_Label.Location = new System.Drawing.Point(309, 624);
            this.HL_Beta_Label.Name = "HL_Beta_Label";
            this.HL_Beta_Label.Size = new System.Drawing.Size(188, 20);
            this.HL_Beta_Label.TabIndex = 23;
            this.HL_Beta_Label.Text = "High Limit, Beta Channel:";
            // 
            // Net_CPM_Label2
            // 
            this.Net_CPM_Label2.AutoSize = true;
            this.Net_CPM_Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Net_CPM_Label2.Location = new System.Drawing.Point(502, 575);
            this.Net_CPM_Label2.Name = "Net_CPM_Label2";
            this.Net_CPM_Label2.Size = new System.Drawing.Size(76, 20);
            this.Net_CPM_Label2.TabIndex = 27;
            this.Net_CPM_Label2.Text = "Net CPM:";
            // 
            // Override_CB
            // 
            this.Override_CB.AutoSize = true;
            this.Override_CB.Enabled = false;
            this.Override_CB.Location = new System.Drawing.Point(618, 598);
            this.Override_CB.Name = "Override_CB";
            this.Override_CB.Size = new System.Drawing.Size(66, 17);
            this.Override_CB.TabIndex = 29;
            this.Override_CB.Text = "Override";
            this.Override_CB.UseVisualStyleBackColor = true;
            this.Override_CB.Visible = false;
            this.Override_CB.CheckedChanged += new System.EventHandler(this.Override_CB_CheckedChanged);
            // 
            // StopButton
            // 
            this.StopButton.BackColor = System.Drawing.Color.SkyBlue;
            this.StopButton.Enabled = false;
            this.StopButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.StopButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.StopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StopButton.ForeColor = System.Drawing.Color.Red;
            this.StopButton.Location = new System.Drawing.Point(736, 84);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(82, 38);
            this.StopButton.TabIndex = 33;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = false;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // Recompute_Limits_Button
            // 
            this.Recompute_Limits_Button.AutoEllipsis = true;
            this.Recompute_Limits_Button.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.Recompute_Limits_Button.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.Recompute_Limits_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.Recompute_Limits_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Recompute_Limits_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Recompute_Limits_Button.ForeColor = System.Drawing.Color.DarkGreen;
            this.Recompute_Limits_Button.Location = new System.Drawing.Point(469, 517);
            this.Recompute_Limits_Button.Name = "Recompute_Limits_Button";
            this.Recompute_Limits_Button.Size = new System.Drawing.Size(180, 30);
            this.Recompute_Limits_Button.TabIndex = 34;
            this.Recompute_Limits_Button.Text = "Recompute Limits";
            this.Recompute_Limits_Button.UseVisualStyleBackColor = false;
            this.Recompute_Limits_Button.Click += new System.EventHandler(this.Recompute_Limits_Button_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.SaveButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.SaveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.YellowGreen;
            this.SaveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveButton.Location = new System.Drawing.Point(664, 517);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(180, 30);
            this.SaveButton.TabIndex = 35;
            this.SaveButton.Text = "Write Results To CSV";
            this.SaveButton.UseVisualStyleBackColor = false;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.SkyBlue;
            this.groupBox1.Controls.Add(this.rdBtnBeta);
            this.groupBox1.Controls.Add(this.rdBtnAlpha);
            this.groupBox1.Controls.Add(this.rdBtnBackground);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(50, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(902, 69);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Type of HiLo";
            // 
            // rdBtnBeta
            // 
            this.rdBtnBeta.AutoSize = true;
            this.rdBtnBeta.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdBtnBeta.Location = new System.Drawing.Point(765, 20);
            this.rdBtnBeta.Name = "rdBtnBeta";
            this.rdBtnBeta.Size = new System.Drawing.Size(73, 24);
            this.rdBtnBeta.TabIndex = 2;
            this.rdBtnBeta.Text = "BETA";
            this.rdBtnBeta.UseVisualStyleBackColor = true;
            this.rdBtnBeta.CheckedChanged += new System.EventHandler(this.rdBtnBeta_CheckedChanged);
            // 
            // rdBtnAlpha
            // 
            this.rdBtnAlpha.AutoSize = true;
            this.rdBtnAlpha.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdBtnAlpha.Location = new System.Drawing.Point(438, 21);
            this.rdBtnAlpha.Name = "rdBtnAlpha";
            this.rdBtnAlpha.Size = new System.Drawing.Size(85, 24);
            this.rdBtnAlpha.TabIndex = 1;
            this.rdBtnAlpha.Text = "ALPHA";
            this.rdBtnAlpha.UseVisualStyleBackColor = true;
            this.rdBtnAlpha.CheckedChanged += new System.EventHandler(this.rdBtnAlpha_CheckedChanged);
            // 
            // rdBtnBackground
            // 
            this.rdBtnBackground.AutoSize = true;
            this.rdBtnBackground.Checked = true;
            this.rdBtnBackground.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdBtnBackground.Location = new System.Drawing.Point(62, 20);
            this.rdBtnBackground.Name = "rdBtnBackground";
            this.rdBtnBackground.Size = new System.Drawing.Size(152, 24);
            this.rdBtnBackground.TabIndex = 0;
            this.rdBtnBackground.TabStop = true;
            this.rdBtnBackground.Text = "BACKGROUND";
            this.rdBtnBackground.UseVisualStyleBackColor = true;
            this.rdBtnBackground.CheckedChanged += new System.EventHandler(this.rdBtnBackground_CheckedChanged);
            // 
            // lbl_HiLocalDate
            // 
            this.lbl_HiLocalDate.AutoSize = true;
            this.lbl_HiLocalDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_HiLocalDate.ForeColor = System.Drawing.Color.Maroon;
            this.lbl_HiLocalDate.Location = new System.Drawing.Point(481, 494);
            this.lbl_HiLocalDate.Name = "lbl_HiLocalDate";
            this.lbl_HiLocalDate.Size = new System.Drawing.Size(51, 16);
            this.lbl_HiLocalDate.TabIndex = 131;
            this.lbl_HiLocalDate.Text = "label5";
            // 
            // BGdate
            // 
            this.BGdate.AutoSize = true;
            this.BGdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BGdate.ForeColor = System.Drawing.Color.Maroon;
            this.BGdate.Location = new System.Drawing.Point(318, 495);
            this.BGdate.Name = "BGdate";
            this.BGdate.Size = new System.Drawing.Size(160, 16);
            this.BGdate.TabIndex = 130;
            this.BGdate.Text = "HiLo Calibration Date:";
            // 
            // FormHiLo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(997, 665);
            this.Controls.Add(this.lbl_HiLocalDate);
            this.Controls.Add(this.BGdate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.Recompute_Limits_Button);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.Override_CB);
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
            this.Name = "FormHiLo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HiLo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormHiLo_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.HiLo_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.HiLo_Results_DataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.CheckBox Override_CB;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button Recompute_Limits_Button;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdBtnBeta;
        private System.Windows.Forms.RadioButton rdBtnAlpha;
        private System.Windows.Forms.RadioButton rdBtnBackground;
        private System.Windows.Forms.Label lbl_HiLocalDate;
        private System.Windows.Forms.Label BGdate;
    }
}