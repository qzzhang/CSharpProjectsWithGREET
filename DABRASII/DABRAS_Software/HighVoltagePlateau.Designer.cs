namespace DABRAS_Software
{
    partial class HighVoltagePlateau
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
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aquireCtrlAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopCtrlSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeDataToFileCtrlWToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.writeImageCtrlIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openWebBasedSurveyFormF12ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRSOSharepointF11ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRSOHomeF10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectDisconnectToAPortCtrlPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.StartV_TB = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.EndV_TB = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.StepSize_TB = new System.Windows.Forms.TextBox();
            this.Sec_TB = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Min_TB = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.AquireButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.VoltagePlateauDataGridView = new System.Windows.Forms.DataGridView();
            this.WriteFileButton = new System.Windows.Forms.Button();
            this.DataChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.SetButton = new System.Windows.Forms.Button();
            this.DABRAS_Status_Label = new System.Windows.Forms.Label();
            this.DABRAS_Firmware_Label = new System.Windows.Forms.Label();
            this.DABRAS_SN_Label = new System.Windows.Forms.Label();
            this.ImageWriteButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VoltagePlateauDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataChart)).BeginInit();
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
            this.menuStrip1.Size = new System.Drawing.Size(1663, 24);
            this.menuStrip1.TabIndex = 0;
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
            this.aquireCtrlAToolStripMenuItem,
            this.stopCtrlSToolStripMenuItem,
            this.writeDataToFileCtrlWToolStripMenuItem,
            this.writeImageCtrlIToolStripMenuItem,
            this.openWebBasedSurveyFormF12ToolStripMenuItem,
            this.openRSOSharepointF11ToolStripMenuItem,
            this.openRSOHomeF10ToolStripMenuItem});
            this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
            this.functionToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.functionToolStripMenuItem.Text = "Function";
            // 
            // aquireCtrlAToolStripMenuItem
            // 
            this.aquireCtrlAToolStripMenuItem.Name = "aquireCtrlAToolStripMenuItem";
            this.aquireCtrlAToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.aquireCtrlAToolStripMenuItem.Text = "Aquire (Ctrl + A)";
            this.aquireCtrlAToolStripMenuItem.Click += new System.EventHandler(this.aquireCtrlAToolStripMenuItem_Click);
            // 
            // stopCtrlSToolStripMenuItem
            // 
            this.stopCtrlSToolStripMenuItem.Enabled = false;
            this.stopCtrlSToolStripMenuItem.Name = "stopCtrlSToolStripMenuItem";
            this.stopCtrlSToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.stopCtrlSToolStripMenuItem.Text = "Stop (Ctrl + S)";
            this.stopCtrlSToolStripMenuItem.Click += new System.EventHandler(this.stopCtrlSToolStripMenuItem_Click);
            // 
            // writeDataToFileCtrlWToolStripMenuItem
            // 
            this.writeDataToFileCtrlWToolStripMenuItem.Name = "writeDataToFileCtrlWToolStripMenuItem";
            this.writeDataToFileCtrlWToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.writeDataToFileCtrlWToolStripMenuItem.Text = "Write Data To File (Ctrl + W)";
            this.writeDataToFileCtrlWToolStripMenuItem.Click += new System.EventHandler(this.writeDataToFileCtrlWToolStripMenuItem_Click);
            // 
            // writeImageCtrlIToolStripMenuItem
            // 
            this.writeImageCtrlIToolStripMenuItem.Name = "writeImageCtrlIToolStripMenuItem";
            this.writeImageCtrlIToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.writeImageCtrlIToolStripMenuItem.Text = "Write Image (Ctrl + I)";
            this.writeImageCtrlIToolStripMenuItem.Click += new System.EventHandler(this.writeImageCtrlIToolStripMenuItem_Click);
            // 
            // openWebBasedSurveyFormF12ToolStripMenuItem
            // 
            this.openWebBasedSurveyFormF12ToolStripMenuItem.Name = "openWebBasedSurveyFormF12ToolStripMenuItem";
            this.openWebBasedSurveyFormF12ToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.openWebBasedSurveyFormF12ToolStripMenuItem.Text = "Open Web-Based Survey Form (F12)";
            this.openWebBasedSurveyFormF12ToolStripMenuItem.Click += new System.EventHandler(this.openWebBasedSurveyFormF12ToolStripMenuItem_Click);
            // 
            // openRSOSharepointF11ToolStripMenuItem
            // 
            this.openRSOSharepointF11ToolStripMenuItem.Name = "openRSOSharepointF11ToolStripMenuItem";
            this.openRSOSharepointF11ToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.openRSOSharepointF11ToolStripMenuItem.Text = "Open RSO Sharepoint (F11)";
            this.openRSOSharepointF11ToolStripMenuItem.Click += new System.EventHandler(this.openRSOSharepointF11ToolStripMenuItem_Click);
            // 
            // openRSOHomeF10ToolStripMenuItem
            // 
            this.openRSOHomeF10ToolStripMenuItem.Name = "openRSOHomeF10ToolStripMenuItem";
            this.openRSOHomeF10ToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.openRSOHomeF10ToolStripMenuItem.Text = "Open RSO Home (F10)";
            this.openRSOHomeF10ToolStripMenuItem.Click += new System.EventHandler(this.openRSOHomeF10ToolStripMenuItem_Click);
            // 
            // utilityToolStripMenuItem
            // 
            this.utilityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectDisconnectToAPortCtrlPToolStripMenuItem});
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
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Perform High Voltage Plateau from ";
            // 
            // StartV_TB
            // 
            this.StartV_TB.Location = new System.Drawing.Point(277, 37);
            this.StartV_TB.Name = "StartV_TB";
            this.StartV_TB.Size = new System.Drawing.Size(53, 20);
            this.StartV_TB.TabIndex = 2;
            this.StartV_TB.Text = "2.5";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(336, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Volts to ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(468, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(165, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Volts in increments of ";
            // 
            // EndV_TB
            // 
            this.EndV_TB.Location = new System.Drawing.Point(409, 35);
            this.EndV_TB.Name = "EndV_TB";
            this.EndV_TB.Size = new System.Drawing.Size(53, 20);
            this.EndV_TB.TabIndex = 4;
            this.EndV_TB.Text = "5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(698, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Volts";
            // 
            // StepSize_TB
            // 
            this.StepSize_TB.Location = new System.Drawing.Point(639, 35);
            this.StepSize_TB.Name = "StepSize_TB";
            this.StepSize_TB.Size = new System.Drawing.Size(53, 20);
            this.StepSize_TB.TabIndex = 6;
            this.StepSize_TB.Text = ".1";
            // 
            // Sec_TB
            // 
            this.Sec_TB.Location = new System.Drawing.Point(253, 72);
            this.Sec_TB.Name = "Sec_TB";
            this.Sec_TB.Size = new System.Drawing.Size(53, 20);
            this.Sec_TB.TabIndex = 11;
            this.Sec_TB.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(234, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = ":";
            // 
            // Min_TB
            // 
            this.Min_TB.Location = new System.Drawing.Point(175, 72);
            this.Min_TB.Name = "Min_TB";
            this.Min_TB.Size = new System.Drawing.Size(53, 20);
            this.Min_TB.TabIndex = 9;
            this.Min_TB.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 20);
            this.label6.TabIndex = 8;
            this.label6.Text = "Count each point for ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(183, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "min";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(264, 95);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "sec";
            // 
            // AquireButton
            // 
            this.AquireButton.Location = new System.Drawing.Point(312, 72);
            this.AquireButton.Name = "AquireButton";
            this.AquireButton.Size = new System.Drawing.Size(91, 23);
            this.AquireButton.TabIndex = 14;
            this.AquireButton.Text = "Aquire (Ctrl + A)";
            this.AquireButton.UseVisualStyleBackColor = true;
            this.AquireButton.Click += new System.EventHandler(this.AquireButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Enabled = false;
            this.StopButton.Location = new System.Drawing.Point(409, 72);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(139, 23);
            this.StopButton.TabIndex = 15;
            this.StopButton.Text = "Stop Aquiring (Ctrl + S)";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // VoltagePlateauDataGridView
            // 
            this.VoltagePlateauDataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.VoltagePlateauDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.VoltagePlateauDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.VoltagePlateauDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.VoltagePlateauDataGridView.Location = new System.Drawing.Point(16, 123);
            this.VoltagePlateauDataGridView.Name = "VoltagePlateauDataGridView";
            this.VoltagePlateauDataGridView.Size = new System.Drawing.Size(765, 328);
            this.VoltagePlateauDataGridView.TabIndex = 16;
            // 
            // WriteFileButton
            // 
            this.WriteFileButton.Location = new System.Drawing.Point(554, 72);
            this.WriteFileButton.Name = "WriteFileButton";
            this.WriteFileButton.Size = new System.Drawing.Size(138, 23);
            this.WriteFileButton.TabIndex = 18;
            this.WriteFileButton.Text = "Write To CSV (Ctrl + W)";
            this.WriteFileButton.UseVisualStyleBackColor = true;
            this.WriteFileButton.Click += new System.EventHandler(this.WriteFileButton_Click);
            // 
            // DataChart
            // 
            chartArea1.Name = "ChartArea1";
            this.DataChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.DataChart.Legends.Add(legend1);
            this.DataChart.Location = new System.Drawing.Point(807, 123);
            this.DataChart.Name = "DataChart";
            this.DataChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Alpha";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Beta";
            this.DataChart.Series.Add(series1);
            this.DataChart.Series.Add(series2);
            this.DataChart.Size = new System.Drawing.Size(814, 328);
            this.DataChart.TabIndex = 19;
            this.DataChart.Text = "chart1";
            this.DataChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DataChart_MouseClick);
            // 
            // SetButton
            // 
            this.SetButton.Location = new System.Drawing.Point(749, 35);
            this.SetButton.Name = "SetButton";
            this.SetButton.Size = new System.Drawing.Size(75, 23);
            this.SetButton.TabIndex = 20;
            this.SetButton.Text = "Set";
            this.SetButton.UseVisualStyleBackColor = true;
            this.SetButton.Click += new System.EventHandler(this.SetButton_Click);
            // 
            // DABRAS_Status_Label
            // 
            this.DABRAS_Status_Label.AutoSize = true;
            this.DABRAS_Status_Label.Location = new System.Drawing.Point(1499, 483);
            this.DABRAS_Status_Label.Name = "DABRAS_Status_Label";
            this.DABRAS_Status_Label.Size = new System.Drawing.Size(122, 13);
            this.DABRAS_Status_Label.TabIndex = 28;
            this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
            // 
            // DABRAS_Firmware_Label
            // 
            this.DABRAS_Firmware_Label.AutoSize = true;
            this.DABRAS_Firmware_Label.Location = new System.Drawing.Point(767, 483);
            this.DABRAS_Firmware_Label.Name = "DABRAS_Firmware_Label";
            this.DABRAS_Firmware_Label.Size = new System.Drawing.Size(40, 13);
            this.DABRAS_Firmware_Label.TabIndex = 27;
            this.DABRAS_Firmware_Label.Text = "F X.XX";
            // 
            // DABRAS_SN_Label
            // 
            this.DABRAS_SN_Label.AutoSize = true;
            this.DABRAS_SN_Label.Location = new System.Drawing.Point(12, 483);
            this.DABRAS_SN_Label.Name = "DABRAS_SN_Label";
            this.DABRAS_SN_Label.Size = new System.Drawing.Size(99, 13);
            this.DABRAS_SN_Label.TabIndex = 26;
            this.DABRAS_SN_Label.Text = "s/n: XXXXXXXXXX";
            // 
            // ImageWriteButton
            // 
            this.ImageWriteButton.Location = new System.Drawing.Point(698, 72);
            this.ImageWriteButton.Name = "ImageWriteButton";
            this.ImageWriteButton.Size = new System.Drawing.Size(126, 23);
            this.ImageWriteButton.TabIndex = 29;
            this.ImageWriteButton.Text = "Write to Image (Ctrl + I)";
            this.ImageWriteButton.UseVisualStyleBackColor = true;
            this.ImageWriteButton.Click += new System.EventHandler(this.ImageWriteButton_Click);
            // 
            // HighVoltagePlateau
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1663, 505);
            this.Controls.Add(this.ImageWriteButton);
            this.Controls.Add(this.DABRAS_Status_Label);
            this.Controls.Add(this.DABRAS_Firmware_Label);
            this.Controls.Add(this.DABRAS_SN_Label);
            this.Controls.Add(this.SetButton);
            this.Controls.Add(this.DataChart);
            this.Controls.Add(this.WriteFileButton);
            this.Controls.Add(this.VoltagePlateauDataGridView);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.AquireButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Sec_TB);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Min_TB);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.StepSize_TB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.EndV_TB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.StartV_TB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "HighVoltagePlateau";
            this.Text = "HighVoltagePlateau";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HighVoltagePlateau_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.HighVoltagePlateau_VisibleChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VoltagePlateauDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem functionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aquireCtrlAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopCtrlSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectDisconnectToAPortCtrlPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox StartV_TB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox EndV_TB;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox StepSize_TB;
        private System.Windows.Forms.TextBox Sec_TB;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Min_TB;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button AquireButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.DataGridView VoltagePlateauDataGridView;
        private System.Windows.Forms.Button WriteFileButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart DataChart;
        private System.Windows.Forms.Button SetButton;
        private System.Windows.Forms.Label DABRAS_Status_Label;
        private System.Windows.Forms.Label DABRAS_Firmware_Label;
        private System.Windows.Forms.Label DABRAS_SN_Label;
        private System.Windows.Forms.ToolStripMenuItem writeDataToFileCtrlWToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openWebBasedSurveyFormF12ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRSOSharepointF11ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openRSOHomeF10ToolStripMenuItem;
        private System.Windows.Forms.Button ImageWriteButton;
        private System.Windows.Forms.ToolStripMenuItem writeImageCtrlIToolStripMenuItem;
    }
}