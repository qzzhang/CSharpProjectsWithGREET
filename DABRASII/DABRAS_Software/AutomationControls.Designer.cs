namespace DABRAS_Software
{
    partial class AutomationControls
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
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Daily_CB = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Daily_Datetime_Hr_TB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Continuous_CB = new System.Windows.Forms.CheckBox();
            this.Daily_DateTime_Min_TB = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Daily_AMPM_Combobox = new System.Windows.Forms.ComboBox();
            this.Daily_Time_Min_TB = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Daily_Time_Hr_TB = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Daily_Time_Sec_TB = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.Daily_AlphaLo_TB = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.Daily_AlphaHi_TB = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.Daily_BetaHi_TB = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.Daily_BetaLo_TB = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.Continuous_BetaHi_TB = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.Continuous_BetaLo_TB = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.Continuous_AlphaHi_TB = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.Continuous_AlphaLo_TB = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.Continuous_Min_TB = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.Continuous_Hour_TB = new System.Windows.Forms.TextBox();
            this.label30 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Kill_BG_Button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Daily Background Checks";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(781, 24);
            this.menuStrip1.TabIndex = 1;
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
            // Daily_CB
            // 
            this.Daily_CB.AutoSize = true;
            this.Daily_CB.Location = new System.Drawing.Point(19, 14);
            this.Daily_CB.Name = "Daily_CB";
            this.Daily_CB.Size = new System.Drawing.Size(235, 17);
            this.Daily_CB.TabIndex = 0;
            this.Daily_CB.Text = "Enable Automatic Daily Background Checks";
            this.Daily_CB.UseVisualStyleBackColor = true;
            this.Daily_CB.CheckedChanged += new System.EventHandler(this.Daily_CB_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Check the Background at ";
            // 
            // Daily_Datetime_Hr_TB
            // 
            this.Daily_Datetime_Hr_TB.Location = new System.Drawing.Point(154, 40);
            this.Daily_Datetime_Hr_TB.Name = "Daily_Datetime_Hr_TB";
            this.Daily_Datetime_Hr_TB.Size = new System.Drawing.Size(35, 20);
            this.Daily_Datetime_Hr_TB.TabIndex = 2;
            this.Daily_Datetime_Hr_TB.Text = "12";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(400, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(237, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Continuous Background Monitor";
            // 
            // Continuous_CB
            // 
            this.Continuous_CB.AutoSize = true;
            this.Continuous_CB.Location = new System.Drawing.Point(18, 15);
            this.Continuous_CB.Name = "Continuous_CB";
            this.Continuous_CB.Size = new System.Drawing.Size(228, 17);
            this.Continuous_CB.TabIndex = 6;
            this.Continuous_CB.Text = "Enable Continuous Background Monitoring";
            this.Continuous_CB.UseVisualStyleBackColor = true;
            this.Continuous_CB.CheckedChanged += new System.EventHandler(this.Continuous_CB_CheckedChanged);
            // 
            // Daily_DateTime_Min_TB
            // 
            this.Daily_DateTime_Min_TB.Location = new System.Drawing.Point(211, 40);
            this.Daily_DateTime_Min_TB.Name = "Daily_DateTime_Min_TB";
            this.Daily_DateTime_Min_TB.Size = new System.Drawing.Size(35, 20);
            this.Daily_DateTime_Min_TB.TabIndex = 3;
            this.Daily_DateTime_Min_TB.Text = "00";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(195, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = ":";
            // 
            // Daily_AMPM_Combobox
            // 
            this.Daily_AMPM_Combobox.FormattingEnabled = true;
            this.Daily_AMPM_Combobox.Location = new System.Drawing.Point(253, 40);
            this.Daily_AMPM_Combobox.Name = "Daily_AMPM_Combobox";
            this.Daily_AMPM_Combobox.Size = new System.Drawing.Size(50, 21);
            this.Daily_AMPM_Combobox.TabIndex = 4;
            this.Daily_AMPM_Combobox.Text = "AM";
            // 
            // Daily_Time_Min_TB
            // 
            this.Daily_Time_Min_TB.Location = new System.Drawing.Point(211, 66);
            this.Daily_Time_Min_TB.Name = "Daily_Time_Min_TB";
            this.Daily_Time_Min_TB.Size = new System.Drawing.Size(35, 20);
            this.Daily_Time_Min_TB.TabIndex = 7;
            this.Daily_Time_Min_TB.Text = "00";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(195, 69);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(10, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = ":";
            // 
            // Daily_Time_Hr_TB
            // 
            this.Daily_Time_Hr_TB.Location = new System.Drawing.Point(154, 66);
            this.Daily_Time_Hr_TB.Name = "Daily_Time_Hr_TB";
            this.Daily_Time_Hr_TB.Size = new System.Drawing.Size(35, 20);
            this.Daily_Time_Hr_TB.TabIndex = 6;
            this.Daily_Time_Hr_TB.Text = "4";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "For a period of";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(162, 89);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(18, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Hr";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(218, 89);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(24, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Min";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(275, 89);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(26, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Sec";
            // 
            // Daily_Time_Sec_TB
            // 
            this.Daily_Time_Sec_TB.Location = new System.Drawing.Point(268, 66);
            this.Daily_Time_Sec_TB.Name = "Daily_Time_Sec_TB";
            this.Daily_Time_Sec_TB.Size = new System.Drawing.Size(35, 20);
            this.Daily_Time_Sec_TB.TabIndex = 8;
            this.Daily_Time_Sec_TB.Text = "00";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(252, 69);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(10, 13);
            this.label11.TabIndex = 18;
            this.label11.Text = ":";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(16, 112);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(105, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "Tolerance Levels";
            // 
            // Daily_AlphaLo_TB
            // 
            this.Daily_AlphaLo_TB.Location = new System.Drawing.Point(195, 140);
            this.Daily_AlphaLo_TB.Name = "Daily_AlphaLo_TB";
            this.Daily_AlphaLo_TB.Size = new System.Drawing.Size(48, 20);
            this.Daily_AlphaLo_TB.TabIndex = 9;
            this.Daily_AlphaLo_TB.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 143);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(176, 13);
            this.label13.TabIndex = 22;
            this.label13.Text = "Accept Alpha Background between";
            // 
            // Daily_AlphaHi_TB
            // 
            this.Daily_AlphaHi_TB.Location = new System.Drawing.Point(280, 140);
            this.Daily_AlphaHi_TB.Name = "Daily_AlphaHi_TB";
            this.Daily_AlphaHi_TB.Size = new System.Drawing.Size(35, 20);
            this.Daily_AlphaHi_TB.TabIndex = 10;
            this.Daily_AlphaHi_TB.Text = "150";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(249, 143);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(25, 13);
            this.label14.TabIndex = 24;
            this.label14.Text = "and";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(321, 143);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(30, 13);
            this.label15.TabIndex = 26;
            this.label15.Text = "CPM";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(321, 172);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 13);
            this.label16.TabIndex = 31;
            this.label16.Text = "CPM";
            // 
            // Daily_BetaHi_TB
            // 
            this.Daily_BetaHi_TB.Location = new System.Drawing.Point(280, 169);
            this.Daily_BetaHi_TB.Name = "Daily_BetaHi_TB";
            this.Daily_BetaHi_TB.Size = new System.Drawing.Size(35, 20);
            this.Daily_BetaHi_TB.TabIndex = 12;
            this.Daily_BetaHi_TB.Text = "400";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(249, 172);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(25, 13);
            this.label17.TabIndex = 29;
            this.label17.Text = "and";
            // 
            // Daily_BetaLo_TB
            // 
            this.Daily_BetaLo_TB.Location = new System.Drawing.Point(195, 169);
            this.Daily_BetaLo_TB.Name = "Daily_BetaLo_TB";
            this.Daily_BetaLo_TB.Size = new System.Drawing.Size(48, 20);
            this.Daily_BetaLo_TB.TabIndex = 11;
            this.Daily_BetaLo_TB.Text = "150";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(16, 172);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(171, 13);
            this.label18.TabIndex = 27;
            this.label18.Text = "Accept Beta Background between";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(320, 181);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(30, 13);
            this.label19.TabIndex = 42;
            this.label19.Text = "CPM";
            // 
            // Continuous_BetaHi_TB
            // 
            this.Continuous_BetaHi_TB.Location = new System.Drawing.Point(279, 178);
            this.Continuous_BetaHi_TB.Name = "Continuous_BetaHi_TB";
            this.Continuous_BetaHi_TB.Size = new System.Drawing.Size(35, 20);
            this.Continuous_BetaHi_TB.TabIndex = 8;
            this.Continuous_BetaHi_TB.Text = "400";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(248, 181);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(25, 13);
            this.label20.TabIndex = 40;
            this.label20.Text = "and";
            // 
            // Continuous_BetaLo_TB
            // 
            this.Continuous_BetaLo_TB.Location = new System.Drawing.Point(194, 178);
            this.Continuous_BetaLo_TB.Name = "Continuous_BetaLo_TB";
            this.Continuous_BetaLo_TB.Size = new System.Drawing.Size(48, 20);
            this.Continuous_BetaLo_TB.TabIndex = 7;
            this.Continuous_BetaLo_TB.Text = "150";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(15, 181);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(171, 13);
            this.label21.TabIndex = 38;
            this.label21.Text = "Accept Beta Background between";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(320, 152);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(30, 13);
            this.label22.TabIndex = 37;
            this.label22.Text = "CPM";
            // 
            // Continuous_AlphaHi_TB
            // 
            this.Continuous_AlphaHi_TB.Location = new System.Drawing.Point(279, 149);
            this.Continuous_AlphaHi_TB.Name = "Continuous_AlphaHi_TB";
            this.Continuous_AlphaHi_TB.Size = new System.Drawing.Size(35, 20);
            this.Continuous_AlphaHi_TB.TabIndex = 6;
            this.Continuous_AlphaHi_TB.Text = "150";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(248, 152);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(25, 13);
            this.label23.TabIndex = 35;
            this.label23.Text = "and";
            // 
            // Continuous_AlphaLo_TB
            // 
            this.Continuous_AlphaLo_TB.Location = new System.Drawing.Point(194, 149);
            this.Continuous_AlphaLo_TB.Name = "Continuous_AlphaLo_TB";
            this.Continuous_AlphaLo_TB.Size = new System.Drawing.Size(48, 20);
            this.Continuous_AlphaLo_TB.TabIndex = 5;
            this.Continuous_AlphaLo_TB.Text = "0";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(15, 152);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(176, 13);
            this.label24.TabIndex = 5;
            this.label24.Text = "Accept Alpha Background between";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(15, 121);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(105, 13);
            this.label25.TabIndex = 32;
            this.label25.Text = "Tolerance Levels";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(255, 64);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(24, 13);
            this.label27.TabIndex = 48;
            this.label27.Text = "Min";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(199, 64);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(18, 13);
            this.label28.TabIndex = 47;
            this.label28.Text = "Hr";
            // 
            // Continuous_Min_TB
            // 
            this.Continuous_Min_TB.Location = new System.Drawing.Point(248, 41);
            this.Continuous_Min_TB.Name = "Continuous_Min_TB";
            this.Continuous_Min_TB.Size = new System.Drawing.Size(35, 20);
            this.Continuous_Min_TB.TabIndex = 2;
            this.Continuous_Min_TB.Text = "10";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(232, 44);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(10, 13);
            this.label29.TabIndex = 45;
            this.label29.Text = ":";
            // 
            // Continuous_Hour_TB
            // 
            this.Continuous_Hour_TB.Location = new System.Drawing.Point(191, 41);
            this.Continuous_Hour_TB.Name = "Continuous_Hour_TB";
            this.Continuous_Hour_TB.Size = new System.Drawing.Size(35, 20);
            this.Continuous_Hour_TB.TabIndex = 1;
            this.Continuous_Hour_TB.Text = "0";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(15, 44);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(170, 13);
            this.label30.TabIndex = 0;
            this.label30.Text = "Average readings over a period of:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Kill_BG_Button);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Controls.Add(this.Continuous_Min_TB);
            this.groupBox1.Controls.Add(this.label29);
            this.groupBox1.Controls.Add(this.Continuous_Hour_TB);
            this.groupBox1.Controls.Add(this.label30);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.Continuous_BetaHi_TB);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.Continuous_BetaLo_TB);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.Continuous_AlphaHi_TB);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.Continuous_AlphaLo_TB);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.Continuous_CB);
            this.groupBox1.Location = new System.Drawing.Point(402, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(363, 229);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // Kill_BG_Button
            // 
            this.Kill_BG_Button.Location = new System.Drawing.Point(18, 89);
            this.Kill_BG_Button.Name = "Kill_BG_Button";
            this.Kill_BG_Button.Size = new System.Drawing.Size(75, 23);
            this.Kill_BG_Button.TabIndex = 4;
            this.Kill_BG_Button.Text = "Kill Monitor";
            this.Kill_BG_Button.UseVisualStyleBackColor = true;
            this.Kill_BG_Button.Click += new System.EventHandler(this.Kill_BG_Button_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.Daily_BetaHi_TB);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.Daily_BetaLo_TB);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.Daily_AlphaHi_TB);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.Daily_AlphaLo_TB);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.Daily_Time_Sec_TB);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.Daily_Time_Min_TB);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.Daily_Time_Hr_TB);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.Daily_AMPM_Combobox);
            this.groupBox2.Controls.Add(this.Daily_DateTime_Min_TB);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.Daily_Datetime_Hr_TB);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.Daily_CB);
            this.groupBox2.Location = new System.Drawing.Point(16, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(371, 229);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // AutomationControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 319);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "AutomationControls";
            this.Text = "AutomationControls";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutomationControls_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeCtrlQToolStripMenuItem;
        private System.Windows.Forms.CheckBox Daily_CB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Daily_Datetime_Hr_TB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox Continuous_CB;
        private System.Windows.Forms.TextBox Daily_DateTime_Min_TB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox Daily_AMPM_Combobox;
        private System.Windows.Forms.TextBox Daily_Time_Min_TB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Daily_Time_Hr_TB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox Daily_Time_Sec_TB;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox Daily_AlphaLo_TB;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox Daily_AlphaHi_TB;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox Daily_BetaHi_TB;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox Daily_BetaLo_TB;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox Continuous_BetaHi_TB;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox Continuous_BetaLo_TB;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox Continuous_AlphaHi_TB;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox Continuous_AlphaLo_TB;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox Continuous_Min_TB;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.TextBox Continuous_Hour_TB;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Kill_BG_Button;
    }
}