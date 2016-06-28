namespace DABRAS_Software
{
    partial class DailyQCGraph
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.Chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeToFileCtrlSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSupressedNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.BG_Alpha_CheckBox = new System.Windows.Forms.CheckBox();
            this.BG_Beta_CheckBox = new System.Windows.Forms.CheckBox();
            this.Beta_Beta_CheckBox = new System.Windows.Forms.CheckBox();
            this.Beta_Alpha_CheckBox = new System.Windows.Forms.CheckBox();
            this.Alpha_Beta_CheckBox = new System.Windows.Forms.CheckBox();
            this.Alpha_Alpha_CheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Beta_Beta_10p_CB = new System.Windows.Forms.CheckBox();
            this.Beta_Alpha_10p_CB = new System.Windows.Forms.CheckBox();
            this.Alpha_Beta_10p_CB = new System.Windows.Forms.CheckBox();
            this.Alpha_Alpha_10p_CB = new System.Windows.Forms.CheckBox();
            this.BG_Beta_10p_CB = new System.Windows.Forms.CheckBox();
            this.BG_Alpha_10p_CB = new System.Windows.Forms.CheckBox();
            this.Beta_Beta_Avg_CB = new System.Windows.Forms.CheckBox();
            this.Beta_Alpha_Avg_CB = new System.Windows.Forms.CheckBox();
            this.Alpha_Beta_Avg_CB = new System.Windows.Forms.CheckBox();
            this.Alpha_Alpha_Avg_CB = new System.Windows.Forms.CheckBox();
            this.BG_Beta_Avg_CB = new System.Windows.Forms.CheckBox();
            this.BG_Alpha_Avg_CB = new System.Windows.Forms.CheckBox();
            this.BG_Beta_3Sig_CB = new System.Windows.Forms.CheckBox();
            this.BG_Beta_2Sig_CB = new System.Windows.Forms.CheckBox();
            this.BG_Alpha_3Sig_CB = new System.Windows.Forms.CheckBox();
            this.BG_Alpha_2Sig_CB = new System.Windows.Forms.CheckBox();
            this.Alpha_Beta_3Sig_CB = new System.Windows.Forms.CheckBox();
            this.Alpha_Beta_2Sig_CB = new System.Windows.Forms.CheckBox();
            this.Alpha_Alpha_3Sig_CB = new System.Windows.Forms.CheckBox();
            this.Alpha_Alpha_2Sig_CB = new System.Windows.Forms.CheckBox();
            this.Beta_Beta_3Sig_CB = new System.Windows.Forms.CheckBox();
            this.Beta_Beta_2Sig_CB = new System.Windows.Forms.CheckBox();
            this.Beta_Alpha_3Sig_CB = new System.Windows.Forms.CheckBox();
            this.Beta_Alpha_2Sig_CB = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SaveCurrentButton = new System.Windows.Forms.Button();
            this.ImageSaveButton = new System.Windows.Forms.Button();
            this.CorrectForDecayCheckBox = new System.Windows.Forms.CheckBox();
            this.SaveAllButton = new System.Windows.Forms.Button();
            this.FullDataSet_CB = new System.Windows.Forms.CheckBox();
            this.ViewSuppressedNodeButton = new System.Windows.Forms.Button();
            this.AutoCalibration_CB = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Chart
            // 
            chartArea1.Name = "ChartArea1";
            this.Chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.Chart.Legends.Add(legend1);
            this.Chart.Location = new System.Drawing.Point(12, 27);
            this.Chart.Name = "Chart";
            this.Chart.Size = new System.Drawing.Size(995, 607);
            this.Chart.TabIndex = 0;
            this.Chart.Text = "chart1";
            this.Chart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Chart_MouseClick);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeCtrlQToolStripMenuItem,
            this.writeToFileCtrlSToolStripMenuItem,
            this.showSupressedNodesToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeCtrlQToolStripMenuItem
            // 
            this.closeCtrlQToolStripMenuItem.Name = "closeCtrlQToolStripMenuItem";
            this.closeCtrlQToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.closeCtrlQToolStripMenuItem.Text = "Close (Ctrl + Q)";
            this.closeCtrlQToolStripMenuItem.Click += new System.EventHandler(this.closeCtrlQToolStripMenuItem_Click);
            // 
            // writeToFileCtrlSToolStripMenuItem
            // 
            this.writeToFileCtrlSToolStripMenuItem.Name = "writeToFileCtrlSToolStripMenuItem";
            this.writeToFileCtrlSToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            // 
            // showSupressedNodesToolStripMenuItem
            // 
            this.showSupressedNodesToolStripMenuItem.Name = "showSupressedNodesToolStripMenuItem";
            this.showSupressedNodesToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.showSupressedNodesToolStripMenuItem.Text = "Show Suppressed Nodes...";
            this.showSupressedNodesToolStripMenuItem.Click += new System.EventHandler(this.showSuppressedNodesToolStripMenuItem_Click);
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
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem1.Text = "About...";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // BG_Alpha_CheckBox
            // 
            this.BG_Alpha_CheckBox.AutoSize = true;
            this.BG_Alpha_CheckBox.Location = new System.Drawing.Point(13, 19);
            this.BG_Alpha_CheckBox.Name = "BG_Alpha_CheckBox";
            this.BG_Alpha_CheckBox.Size = new System.Drawing.Size(213, 17);
            this.BG_Alpha_CheckBox.TabIndex = 2;
            this.BG_Alpha_CheckBox.Text = "Display Alpha Background Test Results";
            this.BG_Alpha_CheckBox.UseVisualStyleBackColor = true;
            this.BG_Alpha_CheckBox.CheckedChanged += new System.EventHandler(this.BG_Alpha_CheckBox_CheckedChanged);
            // 
            // BG_Beta_CheckBox
            // 
            this.BG_Beta_CheckBox.AutoSize = true;
            this.BG_Beta_CheckBox.Location = new System.Drawing.Point(13, 65);
            this.BG_Beta_CheckBox.Name = "BG_Beta_CheckBox";
            this.BG_Beta_CheckBox.Size = new System.Drawing.Size(208, 17);
            this.BG_Beta_CheckBox.TabIndex = 3;
            this.BG_Beta_CheckBox.Text = "Display Beta Background Test Results";
            this.BG_Beta_CheckBox.UseVisualStyleBackColor = true;
            this.BG_Beta_CheckBox.CheckedChanged += new System.EventHandler(this.BG_Beta_CheckBox_CheckedChanged);
            // 
            // Beta_Beta_CheckBox
            // 
            this.Beta_Beta_CheckBox.AutoSize = true;
            this.Beta_Beta_CheckBox.Location = new System.Drawing.Point(10, 333);
            this.Beta_Beta_CheckBox.Name = "Beta_Beta_CheckBox";
            this.Beta_Beta_CheckBox.Size = new System.Drawing.Size(216, 17);
            this.Beta_Beta_CheckBox.TabIndex = 4;
            this.Beta_Beta_CheckBox.Text = "Display Beta Results from the Sr-90 Test";
            this.Beta_Beta_CheckBox.UseVisualStyleBackColor = true;
            this.Beta_Beta_CheckBox.CheckedChanged += new System.EventHandler(this.Beta_Beta_CheckBox_CheckedChanged);
            // 
            // Beta_Alpha_CheckBox
            // 
            this.Beta_Alpha_CheckBox.AutoSize = true;
            this.Beta_Alpha_CheckBox.Location = new System.Drawing.Point(13, 287);
            this.Beta_Alpha_CheckBox.Name = "Beta_Alpha_CheckBox";
            this.Beta_Alpha_CheckBox.Size = new System.Drawing.Size(221, 17);
            this.Beta_Alpha_CheckBox.TabIndex = 5;
            this.Beta_Alpha_CheckBox.Text = "Display Alpha Results from the Sr-90 Test";
            this.Beta_Alpha_CheckBox.UseVisualStyleBackColor = true;
            this.Beta_Alpha_CheckBox.CheckedChanged += new System.EventHandler(this.Beta_Alpha_CheckBox_CheckedChanged);
            // 
            // Alpha_Beta_CheckBox
            // 
            this.Alpha_Beta_CheckBox.AutoSize = true;
            this.Alpha_Beta_CheckBox.Location = new System.Drawing.Point(13, 199);
            this.Alpha_Beta_CheckBox.Name = "Alpha_Beta_CheckBox";
            this.Alpha_Beta_CheckBox.Size = new System.Drawing.Size(227, 17);
            this.Alpha_Beta_CheckBox.TabIndex = 6;
            this.Alpha_Beta_CheckBox.Text = "Display Beta Results from the Am-241 Test";
            this.Alpha_Beta_CheckBox.UseVisualStyleBackColor = true;
            this.Alpha_Beta_CheckBox.CheckedChanged += new System.EventHandler(this.Alpha_Beta_CheckBox_CheckedChanged);
            // 
            // Alpha_Alpha_CheckBox
            // 
            this.Alpha_Alpha_CheckBox.AutoSize = true;
            this.Alpha_Alpha_CheckBox.Location = new System.Drawing.Point(13, 153);
            this.Alpha_Alpha_CheckBox.Name = "Alpha_Alpha_CheckBox";
            this.Alpha_Alpha_CheckBox.Size = new System.Drawing.Size(232, 17);
            this.Alpha_Alpha_CheckBox.TabIndex = 7;
            this.Alpha_Alpha_CheckBox.Text = "Display Alpha Results from the Am-241 Test";
            this.Alpha_Alpha_CheckBox.UseVisualStyleBackColor = true;
            this.Alpha_Alpha_CheckBox.CheckedChanged += new System.EventHandler(this.Alpha_Alpha_CheckBox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Beta_Beta_10p_CB);
            this.groupBox1.Controls.Add(this.Beta_Alpha_10p_CB);
            this.groupBox1.Controls.Add(this.Alpha_Beta_10p_CB);
            this.groupBox1.Controls.Add(this.Alpha_Alpha_10p_CB);
            this.groupBox1.Controls.Add(this.BG_Beta_10p_CB);
            this.groupBox1.Controls.Add(this.BG_Alpha_10p_CB);
            this.groupBox1.Controls.Add(this.Beta_Beta_Avg_CB);
            this.groupBox1.Controls.Add(this.Beta_Alpha_Avg_CB);
            this.groupBox1.Controls.Add(this.Alpha_Beta_Avg_CB);
            this.groupBox1.Controls.Add(this.Alpha_Alpha_Avg_CB);
            this.groupBox1.Controls.Add(this.BG_Beta_Avg_CB);
            this.groupBox1.Controls.Add(this.BG_Alpha_Avg_CB);
            this.groupBox1.Controls.Add(this.BG_Beta_3Sig_CB);
            this.groupBox1.Controls.Add(this.BG_Beta_2Sig_CB);
            this.groupBox1.Controls.Add(this.BG_Alpha_3Sig_CB);
            this.groupBox1.Controls.Add(this.BG_Alpha_2Sig_CB);
            this.groupBox1.Controls.Add(this.Alpha_Beta_3Sig_CB);
            this.groupBox1.Controls.Add(this.Alpha_Beta_2Sig_CB);
            this.groupBox1.Controls.Add(this.Alpha_Alpha_3Sig_CB);
            this.groupBox1.Controls.Add(this.Alpha_Alpha_2Sig_CB);
            this.groupBox1.Controls.Add(this.Beta_Beta_3Sig_CB);
            this.groupBox1.Controls.Add(this.Beta_Beta_2Sig_CB);
            this.groupBox1.Controls.Add(this.Beta_Alpha_3Sig_CB);
            this.groupBox1.Controls.Add(this.Beta_Alpha_2Sig_CB);
            this.groupBox1.Controls.Add(this.Alpha_Alpha_CheckBox);
            this.groupBox1.Controls.Add(this.Alpha_Beta_CheckBox);
            this.groupBox1.Controls.Add(this.Beta_Alpha_CheckBox);
            this.groupBox1.Controls.Add(this.Beta_Beta_CheckBox);
            this.groupBox1.Controls.Add(this.BG_Beta_CheckBox);
            this.groupBox1.Controls.Add(this.BG_Alpha_CheckBox);
            this.groupBox1.Location = new System.Drawing.Point(1017, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(392, 388);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // Beta_Beta_10p_CB
            // 
            this.Beta_Beta_10p_CB.AutoSize = true;
            this.Beta_Beta_10p_CB.Enabled = false;
            this.Beta_Beta_10p_CB.Location = new System.Drawing.Point(148, 356);
            this.Beta_Beta_10p_CB.Name = "Beta_Beta_10p_CB";
            this.Beta_Beta_10p_CB.Size = new System.Drawing.Size(55, 17);
            this.Beta_Beta_10p_CB.TabIndex = 28;
            this.Beta_Beta_10p_CB.Text = "± 10%";
            this.Beta_Beta_10p_CB.UseVisualStyleBackColor = true;
            this.Beta_Beta_10p_CB.CheckedChanged += new System.EventHandler(this.Beta_Beta_10p_CB_CheckedChanged);
            // 
            // Beta_Alpha_10p_CB
            // 
            this.Beta_Alpha_10p_CB.AutoSize = true;
            this.Beta_Alpha_10p_CB.Enabled = false;
            this.Beta_Alpha_10p_CB.Location = new System.Drawing.Point(148, 310);
            this.Beta_Alpha_10p_CB.Name = "Beta_Alpha_10p_CB";
            this.Beta_Alpha_10p_CB.Size = new System.Drawing.Size(55, 17);
            this.Beta_Alpha_10p_CB.TabIndex = 27;
            this.Beta_Alpha_10p_CB.Text = "± 10%";
            this.Beta_Alpha_10p_CB.UseVisualStyleBackColor = true;
            this.Beta_Alpha_10p_CB.CheckedChanged += new System.EventHandler(this.Beta_Alpha_10p_CB_CheckedChanged);
            // 
            // Alpha_Beta_10p_CB
            // 
            this.Alpha_Beta_10p_CB.AutoSize = true;
            this.Alpha_Beta_10p_CB.Enabled = false;
            this.Alpha_Beta_10p_CB.Location = new System.Drawing.Point(148, 222);
            this.Alpha_Beta_10p_CB.Name = "Alpha_Beta_10p_CB";
            this.Alpha_Beta_10p_CB.Size = new System.Drawing.Size(55, 17);
            this.Alpha_Beta_10p_CB.TabIndex = 26;
            this.Alpha_Beta_10p_CB.Text = "± 10%";
            this.Alpha_Beta_10p_CB.UseVisualStyleBackColor = true;
            this.Alpha_Beta_10p_CB.CheckedChanged += new System.EventHandler(this.Alpha_Beta_10p_CB_CheckedChanged);
            // 
            // Alpha_Alpha_10p_CB
            // 
            this.Alpha_Alpha_10p_CB.AutoSize = true;
            this.Alpha_Alpha_10p_CB.Enabled = false;
            this.Alpha_Alpha_10p_CB.Location = new System.Drawing.Point(148, 176);
            this.Alpha_Alpha_10p_CB.Name = "Alpha_Alpha_10p_CB";
            this.Alpha_Alpha_10p_CB.Size = new System.Drawing.Size(55, 17);
            this.Alpha_Alpha_10p_CB.TabIndex = 19;
            this.Alpha_Alpha_10p_CB.Text = "± 10%";
            this.Alpha_Alpha_10p_CB.UseVisualStyleBackColor = true;
            this.Alpha_Alpha_10p_CB.CheckedChanged += new System.EventHandler(this.Alpha_Alpha_10p_CB_CheckedChanged);
            // 
            // BG_Beta_10p_CB
            // 
            this.BG_Beta_10p_CB.AutoSize = true;
            this.BG_Beta_10p_CB.Enabled = false;
            this.BG_Beta_10p_CB.Location = new System.Drawing.Point(148, 85);
            this.BG_Beta_10p_CB.Name = "BG_Beta_10p_CB";
            this.BG_Beta_10p_CB.Size = new System.Drawing.Size(55, 17);
            this.BG_Beta_10p_CB.TabIndex = 19;
            this.BG_Beta_10p_CB.Text = "± 10%";
            this.BG_Beta_10p_CB.UseVisualStyleBackColor = true;
            this.BG_Beta_10p_CB.CheckedChanged += new System.EventHandler(this.BG_Beta_10p_CB_CheckedChanged);
            // 
            // BG_Alpha_10p_CB
            // 
            this.BG_Alpha_10p_CB.AutoSize = true;
            this.BG_Alpha_10p_CB.Enabled = false;
            this.BG_Alpha_10p_CB.Location = new System.Drawing.Point(148, 42);
            this.BG_Alpha_10p_CB.Name = "BG_Alpha_10p_CB";
            this.BG_Alpha_10p_CB.Size = new System.Drawing.Size(55, 17);
            this.BG_Alpha_10p_CB.TabIndex = 18;
            this.BG_Alpha_10p_CB.Text = "± 10%";
            this.BG_Alpha_10p_CB.UseVisualStyleBackColor = true;
            this.BG_Alpha_10p_CB.CheckedChanged += new System.EventHandler(this.BG_Alpha_10p_CB_CheckedChanged);
            // 
            // Beta_Beta_Avg_CB
            // 
            this.Beta_Beta_Avg_CB.AutoSize = true;
            this.Beta_Beta_Avg_CB.Enabled = false;
            this.Beta_Beta_Avg_CB.Location = new System.Drawing.Point(209, 356);
            this.Beta_Beta_Avg_CB.Name = "Beta_Beta_Avg_CB";
            this.Beta_Beta_Avg_CB.Size = new System.Drawing.Size(66, 17);
            this.Beta_Beta_Avg_CB.TabIndex = 25;
            this.Beta_Beta_Avg_CB.Text = "Average";
            this.Beta_Beta_Avg_CB.UseVisualStyleBackColor = true;
            this.Beta_Beta_Avg_CB.CheckedChanged += new System.EventHandler(this.Beta_Beta_Avg_CB_CheckedChanged);
            // 
            // Beta_Alpha_Avg_CB
            // 
            this.Beta_Alpha_Avg_CB.AutoSize = true;
            this.Beta_Alpha_Avg_CB.Enabled = false;
            this.Beta_Alpha_Avg_CB.Location = new System.Drawing.Point(209, 310);
            this.Beta_Alpha_Avg_CB.Name = "Beta_Alpha_Avg_CB";
            this.Beta_Alpha_Avg_CB.Size = new System.Drawing.Size(66, 17);
            this.Beta_Alpha_Avg_CB.TabIndex = 24;
            this.Beta_Alpha_Avg_CB.Text = "Average";
            this.Beta_Alpha_Avg_CB.UseVisualStyleBackColor = true;
            this.Beta_Alpha_Avg_CB.CheckedChanged += new System.EventHandler(this.Beta_Alpha_Avg_CB_CheckedChanged);
            // 
            // Alpha_Beta_Avg_CB
            // 
            this.Alpha_Beta_Avg_CB.AutoSize = true;
            this.Alpha_Beta_Avg_CB.Enabled = false;
            this.Alpha_Beta_Avg_CB.Location = new System.Drawing.Point(209, 222);
            this.Alpha_Beta_Avg_CB.Name = "Alpha_Beta_Avg_CB";
            this.Alpha_Beta_Avg_CB.Size = new System.Drawing.Size(66, 17);
            this.Alpha_Beta_Avg_CB.TabIndex = 23;
            this.Alpha_Beta_Avg_CB.Text = "Average";
            this.Alpha_Beta_Avg_CB.UseVisualStyleBackColor = true;
            this.Alpha_Beta_Avg_CB.CheckedChanged += new System.EventHandler(this.Alpha_Beta_Avg_CB_CheckedChanged);
            // 
            // Alpha_Alpha_Avg_CB
            // 
            this.Alpha_Alpha_Avg_CB.AutoSize = true;
            this.Alpha_Alpha_Avg_CB.Enabled = false;
            this.Alpha_Alpha_Avg_CB.Location = new System.Drawing.Point(209, 176);
            this.Alpha_Alpha_Avg_CB.Name = "Alpha_Alpha_Avg_CB";
            this.Alpha_Alpha_Avg_CB.Size = new System.Drawing.Size(66, 17);
            this.Alpha_Alpha_Avg_CB.TabIndex = 22;
            this.Alpha_Alpha_Avg_CB.Text = "Average";
            this.Alpha_Alpha_Avg_CB.UseVisualStyleBackColor = true;
            this.Alpha_Alpha_Avg_CB.CheckedChanged += new System.EventHandler(this.Alpha_Alpha_Avg_CB_CheckedChanged);
            // 
            // BG_Beta_Avg_CB
            // 
            this.BG_Beta_Avg_CB.AutoSize = true;
            this.BG_Beta_Avg_CB.Enabled = false;
            this.BG_Beta_Avg_CB.Location = new System.Drawing.Point(209, 85);
            this.BG_Beta_Avg_CB.Name = "BG_Beta_Avg_CB";
            this.BG_Beta_Avg_CB.Size = new System.Drawing.Size(66, 17);
            this.BG_Beta_Avg_CB.TabIndex = 21;
            this.BG_Beta_Avg_CB.Text = "Average";
            this.BG_Beta_Avg_CB.UseVisualStyleBackColor = true;
            this.BG_Beta_Avg_CB.CheckedChanged += new System.EventHandler(this.BG_Beta_Avg_CB_CheckedChanged);
            // 
            // BG_Alpha_Avg_CB
            // 
            this.BG_Alpha_Avg_CB.AutoSize = true;
            this.BG_Alpha_Avg_CB.Enabled = false;
            this.BG_Alpha_Avg_CB.Location = new System.Drawing.Point(209, 42);
            this.BG_Alpha_Avg_CB.Name = "BG_Alpha_Avg_CB";
            this.BG_Alpha_Avg_CB.Size = new System.Drawing.Size(66, 17);
            this.BG_Alpha_Avg_CB.TabIndex = 20;
            this.BG_Alpha_Avg_CB.Text = "Average";
            this.BG_Alpha_Avg_CB.UseVisualStyleBackColor = true;
            this.BG_Alpha_Avg_CB.CheckedChanged += new System.EventHandler(this.BG_Alpha_Avg_CB_CheckedChanged);
            // 
            // BG_Beta_3Sig_CB
            // 
            this.BG_Beta_3Sig_CB.AutoSize = true;
            this.BG_Beta_3Sig_CB.Enabled = false;
            this.BG_Beta_3Sig_CB.Location = new System.Drawing.Point(94, 85);
            this.BG_Beta_3Sig_CB.Name = "BG_Beta_3Sig_CB";
            this.BG_Beta_3Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.BG_Beta_3Sig_CB.TabIndex = 19;
            this.BG_Beta_3Sig_CB.Text = "± 3σ";
            this.BG_Beta_3Sig_CB.UseVisualStyleBackColor = true;
            this.BG_Beta_3Sig_CB.CheckedChanged += new System.EventHandler(this.BG_Beta_3Sig_CB_CheckedChanged);
            // 
            // BG_Beta_2Sig_CB
            // 
            this.BG_Beta_2Sig_CB.AutoSize = true;
            this.BG_Beta_2Sig_CB.Enabled = false;
            this.BG_Beta_2Sig_CB.Location = new System.Drawing.Point(40, 85);
            this.BG_Beta_2Sig_CB.Name = "BG_Beta_2Sig_CB";
            this.BG_Beta_2Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.BG_Beta_2Sig_CB.TabIndex = 18;
            this.BG_Beta_2Sig_CB.Text = "± 2σ";
            this.BG_Beta_2Sig_CB.UseVisualStyleBackColor = true;
            this.BG_Beta_2Sig_CB.CheckedChanged += new System.EventHandler(this.BG_Beta_2Sig_CB_CheckedChanged);
            // 
            // BG_Alpha_3Sig_CB
            // 
            this.BG_Alpha_3Sig_CB.AutoSize = true;
            this.BG_Alpha_3Sig_CB.Enabled = false;
            this.BG_Alpha_3Sig_CB.Location = new System.Drawing.Point(94, 42);
            this.BG_Alpha_3Sig_CB.Name = "BG_Alpha_3Sig_CB";
            this.BG_Alpha_3Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.BG_Alpha_3Sig_CB.TabIndex = 17;
            this.BG_Alpha_3Sig_CB.Text = "± 3σ";
            this.BG_Alpha_3Sig_CB.UseVisualStyleBackColor = true;
            this.BG_Alpha_3Sig_CB.CheckedChanged += new System.EventHandler(this.BG_Alpha_3Sig_CB_CheckedChanged);
            // 
            // BG_Alpha_2Sig_CB
            // 
            this.BG_Alpha_2Sig_CB.AutoSize = true;
            this.BG_Alpha_2Sig_CB.Enabled = false;
            this.BG_Alpha_2Sig_CB.Location = new System.Drawing.Point(40, 42);
            this.BG_Alpha_2Sig_CB.Name = "BG_Alpha_2Sig_CB";
            this.BG_Alpha_2Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.BG_Alpha_2Sig_CB.TabIndex = 16;
            this.BG_Alpha_2Sig_CB.Text = "± 2σ";
            this.BG_Alpha_2Sig_CB.UseVisualStyleBackColor = true;
            this.BG_Alpha_2Sig_CB.CheckedChanged += new System.EventHandler(this.BG_Alpha_2Sig_CB_CheckedChanged);
            // 
            // Alpha_Beta_3Sig_CB
            // 
            this.Alpha_Beta_3Sig_CB.AutoSize = true;
            this.Alpha_Beta_3Sig_CB.Enabled = false;
            this.Alpha_Beta_3Sig_CB.Location = new System.Drawing.Point(94, 222);
            this.Alpha_Beta_3Sig_CB.Name = "Alpha_Beta_3Sig_CB";
            this.Alpha_Beta_3Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.Alpha_Beta_3Sig_CB.TabIndex = 15;
            this.Alpha_Beta_3Sig_CB.Text = "± 3σ";
            this.Alpha_Beta_3Sig_CB.UseVisualStyleBackColor = true;
            this.Alpha_Beta_3Sig_CB.CheckedChanged += new System.EventHandler(this.Alpha_Beta_3Sig_CB_CheckedChanged);
            // 
            // Alpha_Beta_2Sig_CB
            // 
            this.Alpha_Beta_2Sig_CB.AutoSize = true;
            this.Alpha_Beta_2Sig_CB.Enabled = false;
            this.Alpha_Beta_2Sig_CB.Location = new System.Drawing.Point(36, 222);
            this.Alpha_Beta_2Sig_CB.Name = "Alpha_Beta_2Sig_CB";
            this.Alpha_Beta_2Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.Alpha_Beta_2Sig_CB.TabIndex = 14;
            this.Alpha_Beta_2Sig_CB.Text = "± 2σ";
            this.Alpha_Beta_2Sig_CB.UseVisualStyleBackColor = true;
            this.Alpha_Beta_2Sig_CB.CheckedChanged += new System.EventHandler(this.Alpha_Beta_2Sig_CB_CheckedChanged);
            // 
            // Alpha_Alpha_3Sig_CB
            // 
            this.Alpha_Alpha_3Sig_CB.AutoSize = true;
            this.Alpha_Alpha_3Sig_CB.Enabled = false;
            this.Alpha_Alpha_3Sig_CB.Location = new System.Drawing.Point(94, 176);
            this.Alpha_Alpha_3Sig_CB.Name = "Alpha_Alpha_3Sig_CB";
            this.Alpha_Alpha_3Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.Alpha_Alpha_3Sig_CB.TabIndex = 13;
            this.Alpha_Alpha_3Sig_CB.Text = "± 3σ";
            this.Alpha_Alpha_3Sig_CB.UseVisualStyleBackColor = true;
            this.Alpha_Alpha_3Sig_CB.CheckedChanged += new System.EventHandler(this.Alpha_Alpha_3Sig_CB_CheckedChanged);
            // 
            // Alpha_Alpha_2Sig_CB
            // 
            this.Alpha_Alpha_2Sig_CB.AutoSize = true;
            this.Alpha_Alpha_2Sig_CB.Enabled = false;
            this.Alpha_Alpha_2Sig_CB.Location = new System.Drawing.Point(36, 176);
            this.Alpha_Alpha_2Sig_CB.Name = "Alpha_Alpha_2Sig_CB";
            this.Alpha_Alpha_2Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.Alpha_Alpha_2Sig_CB.TabIndex = 12;
            this.Alpha_Alpha_2Sig_CB.Text = "± 2σ";
            this.Alpha_Alpha_2Sig_CB.UseVisualStyleBackColor = true;
            this.Alpha_Alpha_2Sig_CB.CheckedChanged += new System.EventHandler(this.Alpha_Alpha_2Sig_CB_CheckedChanged);
            // 
            // Beta_Beta_3Sig_CB
            // 
            this.Beta_Beta_3Sig_CB.AutoSize = true;
            this.Beta_Beta_3Sig_CB.Enabled = false;
            this.Beta_Beta_3Sig_CB.Location = new System.Drawing.Point(98, 356);
            this.Beta_Beta_3Sig_CB.Name = "Beta_Beta_3Sig_CB";
            this.Beta_Beta_3Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.Beta_Beta_3Sig_CB.TabIndex = 11;
            this.Beta_Beta_3Sig_CB.Text = "± 3σ";
            this.Beta_Beta_3Sig_CB.UseVisualStyleBackColor = true;
            this.Beta_Beta_3Sig_CB.CheckedChanged += new System.EventHandler(this.Beta_Beta_3Sig_CB_CheckedChanged);
            // 
            // Beta_Beta_2Sig_CB
            // 
            this.Beta_Beta_2Sig_CB.AutoSize = true;
            this.Beta_Beta_2Sig_CB.Enabled = false;
            this.Beta_Beta_2Sig_CB.Location = new System.Drawing.Point(36, 356);
            this.Beta_Beta_2Sig_CB.Name = "Beta_Beta_2Sig_CB";
            this.Beta_Beta_2Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.Beta_Beta_2Sig_CB.TabIndex = 10;
            this.Beta_Beta_2Sig_CB.Text = "± 2σ";
            this.Beta_Beta_2Sig_CB.UseVisualStyleBackColor = true;
            this.Beta_Beta_2Sig_CB.CheckedChanged += new System.EventHandler(this.Beta_Beta_2Sig_CB_CheckedChanged);
            // 
            // Beta_Alpha_3Sig_CB
            // 
            this.Beta_Alpha_3Sig_CB.AutoSize = true;
            this.Beta_Alpha_3Sig_CB.Enabled = false;
            this.Beta_Alpha_3Sig_CB.Location = new System.Drawing.Point(98, 310);
            this.Beta_Alpha_3Sig_CB.Name = "Beta_Alpha_3Sig_CB";
            this.Beta_Alpha_3Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.Beta_Alpha_3Sig_CB.TabIndex = 9;
            this.Beta_Alpha_3Sig_CB.Text = "± 3σ";
            this.Beta_Alpha_3Sig_CB.UseVisualStyleBackColor = true;
            this.Beta_Alpha_3Sig_CB.CheckedChanged += new System.EventHandler(this.Beta_Alpha_3Sig_CB_CheckedChanged);
            // 
            // Beta_Alpha_2Sig_CB
            // 
            this.Beta_Alpha_2Sig_CB.AutoSize = true;
            this.Beta_Alpha_2Sig_CB.Enabled = false;
            this.Beta_Alpha_2Sig_CB.Location = new System.Drawing.Point(36, 310);
            this.Beta_Alpha_2Sig_CB.Name = "Beta_Alpha_2Sig_CB";
            this.Beta_Alpha_2Sig_CB.Size = new System.Drawing.Size(48, 17);
            this.Beta_Alpha_2Sig_CB.TabIndex = 8;
            this.Beta_Alpha_2Sig_CB.Text = "± 2σ";
            this.Beta_Alpha_2Sig_CB.UseVisualStyleBackColor = true;
            this.Beta_Alpha_2Sig_CB.CheckedChanged += new System.EventHandler(this.Beta_Alpha_2Sig_CB_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(1013, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 20);
            this.label1.TabIndex = 9;
            this.label1.Text = "Data to Display";
            // 
            // SaveCurrentButton
            // 
            this.SaveCurrentButton.Location = new System.Drawing.Point(1017, 468);
            this.SaveCurrentButton.Name = "SaveCurrentButton";
            this.SaveCurrentButton.Size = new System.Drawing.Size(275, 23);
            this.SaveCurrentButton.TabIndex = 10;
            this.SaveCurrentButton.Text = "Save Data from Current Time Period (Ctrl + S)";
            this.SaveCurrentButton.UseVisualStyleBackColor = true;
            this.SaveCurrentButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ImageSaveButton
            // 
            this.ImageSaveButton.Location = new System.Drawing.Point(1017, 526);
            this.ImageSaveButton.Name = "ImageSaveButton";
            this.ImageSaveButton.Size = new System.Drawing.Size(275, 23);
            this.ImageSaveButton.TabIndex = 11;
            this.ImageSaveButton.Text = "Save image (Ctrl + I)";
            this.ImageSaveButton.UseVisualStyleBackColor = true;
            this.ImageSaveButton.Click += new System.EventHandler(this.ImageSaveButton_Click);
            // 
            // CorrectForDecayCheckBox
            // 
            this.CorrectForDecayCheckBox.AutoSize = true;
            this.CorrectForDecayCheckBox.Location = new System.Drawing.Point(1136, 40);
            this.CorrectForDecayCheckBox.Name = "CorrectForDecayCheckBox";
            this.CorrectForDecayCheckBox.Size = new System.Drawing.Size(112, 17);
            this.CorrectForDecayCheckBox.TabIndex = 12;
            this.CorrectForDecayCheckBox.Text = "Correct For Decay";
            this.CorrectForDecayCheckBox.UseVisualStyleBackColor = true;
            this.CorrectForDecayCheckBox.CheckedChanged += new System.EventHandler(this.CorrectForDecayCheckBox_CheckedChanged);
            // 
            // SaveAllButton
            // 
            this.SaveAllButton.Location = new System.Drawing.Point(1017, 497);
            this.SaveAllButton.Name = "SaveAllButton";
            this.SaveAllButton.Size = new System.Drawing.Size(275, 23);
            this.SaveAllButton.TabIndex = 13;
            this.SaveAllButton.Text = "Save All Data (Ctrl + Alt + S)";
            this.SaveAllButton.UseVisualStyleBackColor = true;
            this.SaveAllButton.Click += new System.EventHandler(this.SaveAllButton_Click);
            // 
            // FullDataSet_CB
            // 
            this.FullDataSet_CB.AutoSize = true;
            this.FullDataSet_CB.Location = new System.Drawing.Point(1255, 40);
            this.FullDataSet_CB.Name = "FullDataSet_CB";
            this.FullDataSet_CB.Size = new System.Drawing.Size(93, 17);
            this.FullDataSet_CB.TabIndex = 14;
            this.FullDataSet_CB.Text = "Show All Data";
            this.FullDataSet_CB.UseVisualStyleBackColor = true;
            this.FullDataSet_CB.CheckedChanged += new System.EventHandler(this.LastCalButton_CheckedChanged);
            // 
            // ViewSuppressedNodeButton
            // 
            this.ViewSuppressedNodeButton.Location = new System.Drawing.Point(1017, 556);
            this.ViewSuppressedNodeButton.Name = "ViewSuppressedNodeButton";
            this.ViewSuppressedNodeButton.Size = new System.Drawing.Size(275, 23);
            this.ViewSuppressedNodeButton.TabIndex = 15;
            this.ViewSuppressedNodeButton.Text = "View Suppressed Nodes (Ctrl + V)";
            this.ViewSuppressedNodeButton.UseVisualStyleBackColor = true;
            this.ViewSuppressedNodeButton.Click += new System.EventHandler(this.ViewSuppressedNodeButton_Click);
            // 
            // AutoCalibration_CB
            // 
            this.AutoCalibration_CB.AutoSize = true;
            this.AutoCalibration_CB.Location = new System.Drawing.Point(1136, 12);
            this.AutoCalibration_CB.Name = "AutoCalibration_CB";
            this.AutoCalibration_CB.Size = new System.Drawing.Size(143, 17);
            this.AutoCalibration_CB.TabIndex = 16;
            this.AutoCalibration_CB.Text = "Include Auto Calibrations";
            this.AutoCalibration_CB.UseVisualStyleBackColor = true;
            this.AutoCalibration_CB.CheckedChanged += new System.EventHandler(this.AutoCalibration_CB_CheckedChanged);
            // 
            // DailyQCGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1435, 646);
            this.Controls.Add(this.AutoCalibration_CB);
            this.Controls.Add(this.ViewSuppressedNodeButton);
            this.Controls.Add(this.FullDataSet_CB);
            this.Controls.Add(this.SaveAllButton);
            this.Controls.Add(this.CorrectForDecayCheckBox);
            this.Controls.Add(this.ImageSaveButton);
            this.Controls.Add(this.SaveCurrentButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Chart);
            this.Name = "DailyQCGraph";
            this.Text = "Graph";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DailyQCGraph_FormClosing);
            this.Load += new System.EventHandler(this.DailyQCGraph_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart Chart;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem writeToFileCtrlSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.CheckBox BG_Alpha_CheckBox;
        private System.Windows.Forms.CheckBox BG_Beta_CheckBox;
        private System.Windows.Forms.CheckBox Beta_Beta_CheckBox;
        private System.Windows.Forms.CheckBox Beta_Alpha_CheckBox;
        private System.Windows.Forms.CheckBox Alpha_Beta_CheckBox;
        private System.Windows.Forms.CheckBox Alpha_Alpha_CheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SaveCurrentButton;
        private System.Windows.Forms.CheckBox Beta_Beta_3Sig_CB;
        private System.Windows.Forms.CheckBox Beta_Beta_2Sig_CB;
        private System.Windows.Forms.CheckBox Beta_Alpha_3Sig_CB;
        private System.Windows.Forms.CheckBox Beta_Alpha_2Sig_CB;
        private System.Windows.Forms.CheckBox BG_Beta_3Sig_CB;
        private System.Windows.Forms.CheckBox BG_Beta_2Sig_CB;
        private System.Windows.Forms.CheckBox BG_Alpha_3Sig_CB;
        private System.Windows.Forms.CheckBox BG_Alpha_2Sig_CB;
        private System.Windows.Forms.CheckBox Alpha_Beta_3Sig_CB;
        private System.Windows.Forms.CheckBox Alpha_Beta_2Sig_CB;
        private System.Windows.Forms.CheckBox Alpha_Alpha_3Sig_CB;
        private System.Windows.Forms.CheckBox Alpha_Alpha_2Sig_CB;
        private System.Windows.Forms.CheckBox Beta_Beta_Avg_CB;
        private System.Windows.Forms.CheckBox Beta_Alpha_Avg_CB;
        private System.Windows.Forms.CheckBox Alpha_Beta_Avg_CB;
        private System.Windows.Forms.CheckBox Alpha_Alpha_Avg_CB;
        private System.Windows.Forms.CheckBox BG_Beta_Avg_CB;
        private System.Windows.Forms.CheckBox BG_Alpha_Avg_CB;
        private System.Windows.Forms.ToolStripMenuItem showSupressedNodesToolStripMenuItem;
        private System.Windows.Forms.Button ImageSaveButton;
        private System.Windows.Forms.CheckBox CorrectForDecayCheckBox;
        private System.Windows.Forms.CheckBox Beta_Beta_10p_CB;
        private System.Windows.Forms.CheckBox Beta_Alpha_10p_CB;
        private System.Windows.Forms.CheckBox Alpha_Beta_10p_CB;
        private System.Windows.Forms.CheckBox Alpha_Alpha_10p_CB;
        private System.Windows.Forms.CheckBox BG_Beta_10p_CB;
        private System.Windows.Forms.CheckBox BG_Alpha_10p_CB;
        private System.Windows.Forms.Button SaveAllButton;
        private System.Windows.Forms.CheckBox FullDataSet_CB;
        private System.Windows.Forms.Button ViewSuppressedNodeButton;
        private System.Windows.Forms.CheckBox AutoCalibration_CB;
    }
}