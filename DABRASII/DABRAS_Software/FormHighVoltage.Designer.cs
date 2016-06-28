namespace DABRAS_Software
{
    partial class FormHighVoltage
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
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
            this.ImageWriteButton = new System.Windows.Forms.Button();
            this.frmHVtooltip = new System.Windows.Forms.ToolTip(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbl_progress = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.VoltagePlateauDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataChart)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Perform High Voltage Plateau from ";
            // 
            // StartV_TB
            // 
            this.StartV_TB.Location = new System.Drawing.Point(283, 24);
            this.StartV_TB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StartV_TB.Name = "StartV_TB";
            this.StartV_TB.Size = new System.Drawing.Size(53, 21);
            this.StartV_TB.TabIndex = 2;
            this.StartV_TB.Text = "2.5";
            this.StartV_TB.TextChanged += new System.EventHandler(this.StartV_TB_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(349, 23);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Volts to ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(477, 23);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Volts      in increments of ";
            // 
            // EndV_TB
            // 
            this.EndV_TB.Location = new System.Drawing.Point(424, 22);
            this.EndV_TB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.EndV_TB.Name = "EndV_TB";
            this.EndV_TB.Size = new System.Drawing.Size(45, 21);
            this.EndV_TB.TabIndex = 4;
            this.EndV_TB.Text = "5";
            this.EndV_TB.TextChanged += new System.EventHandler(this.EndV_TB_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(737, 20);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Volts";
            // 
            // StepSize_TB
            // 
            this.StepSize_TB.Location = new System.Drawing.Point(674, 21);
            this.StepSize_TB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StepSize_TB.Name = "StepSize_TB";
            this.StepSize_TB.Size = new System.Drawing.Size(55, 21);
            this.StepSize_TB.TabIndex = 6;
            this.StepSize_TB.Text = ".1";
            this.StepSize_TB.TextChanged += new System.EventHandler(this.StepSize_TB_TextChanged);
            // 
            // Sec_TB
            // 
            this.Sec_TB.Location = new System.Drawing.Point(283, 64);
            this.Sec_TB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Sec_TB.Name = "Sec_TB";
            this.Sec_TB.Size = new System.Drawing.Size(69, 21);
            this.Sec_TB.TabIndex = 11;
            this.Sec_TB.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(258, 62);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = ":";
            // 
            // Min_TB
            // 
            this.Min_TB.Location = new System.Drawing.Point(179, 64);
            this.Min_TB.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Min_TB.Name = "Min_TB";
            this.Min_TB.Size = new System.Drawing.Size(69, 21);
            this.Min_TB.TabIndex = 9;
            this.Min_TB.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 62);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 20);
            this.label6.TabIndex = 8;
            this.label6.Text = "Count each point for ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(200, 88);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 15);
            this.label7.TabIndex = 12;
            this.label7.Text = "min";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(303, 88);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 15);
            this.label8.TabIndex = 13;
            this.label8.Text = "sec";
            // 
            // AquireButton
            // 
            this.AquireButton.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.AquireButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.AquireButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AquireButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AquireButton.ForeColor = System.Drawing.Color.LimeGreen;
            this.AquireButton.Location = new System.Drawing.Point(377, 64);
            this.AquireButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.AquireButton.Name = "AquireButton";
            this.AquireButton.Size = new System.Drawing.Size(112, 39);
            this.AquireButton.TabIndex = 14;
            this.AquireButton.Text = "Start";
            this.frmHVtooltip.SetToolTip(this.AquireButton, "Start counting");
            this.AquireButton.UseVisualStyleBackColor = false;
            this.AquireButton.Click += new System.EventHandler(this.AquireButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.StopButton.Enabled = false;
            this.StopButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.StopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StopButton.ForeColor = System.Drawing.Color.Red;
            this.StopButton.Location = new System.Drawing.Point(495, 64);
            this.StopButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(105, 39);
            this.StopButton.TabIndex = 15;
            this.StopButton.Text = "Stop";
            this.frmHVtooltip.SetToolTip(this.StopButton, "Stop counting");
            this.StopButton.UseVisualStyleBackColor = false;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // VoltagePlateauDataGridView
            // 
            this.VoltagePlateauDataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.VoltagePlateauDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.VoltagePlateauDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.VoltagePlateauDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.VoltagePlateauDataGridView.Location = new System.Drawing.Point(21, 121);
            this.VoltagePlateauDataGridView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.VoltagePlateauDataGridView.Name = "VoltagePlateauDataGridView";
            this.VoltagePlateauDataGridView.Size = new System.Drawing.Size(881, 377);
            this.VoltagePlateauDataGridView.TabIndex = 16;
            // 
            // WriteFileButton
            // 
            this.WriteFileButton.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.WriteFileButton.Enabled = false;
            this.WriteFileButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.WriteFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.WriteFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WriteFileButton.Location = new System.Drawing.Point(608, 64);
            this.WriteFileButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.WriteFileButton.Name = "WriteFileButton";
            this.WriteFileButton.Size = new System.Drawing.Size(174, 39);
            this.WriteFileButton.TabIndex = 18;
            this.WriteFileButton.Text = "Save to CSV";
            this.frmHVtooltip.SetToolTip(this.WriteFileButton, "Save results to a csv file");
            this.WriteFileButton.UseVisualStyleBackColor = false;
            this.WriteFileButton.Click += new System.EventHandler(this.WriteFileButton_Click);
            // 
            // DataChart
            // 
            chartArea2.Name = "ChartArea1";
            this.DataChart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.DataChart.Legends.Add(legend2);
            this.DataChart.Location = new System.Drawing.Point(13, 504);
            this.DataChart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.DataChart.Name = "DataChart";
            this.DataChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Alpha";
            series4.ChartArea = "ChartArea1";
            series4.Legend = "Legend1";
            series4.Name = "Beta";
            this.DataChart.Series.Add(series3);
            this.DataChart.Series.Add(series4);
            this.DataChart.Size = new System.Drawing.Size(886, 351);
            this.DataChart.TabIndex = 19;
            this.DataChart.Text = "chart1";
            this.DataChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DataChart_MouseClick);
            // 
            // SetButton
            // 
            this.SetButton.BackColor = System.Drawing.Color.Goldenrod;
            this.SetButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.SetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SetButton.Location = new System.Drawing.Point(790, 20);
            this.SetButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SetButton.Name = "SetButton";
            this.SetButton.Size = new System.Drawing.Size(175, 32);
            this.SetButton.TabIndex = 20;
            this.SetButton.Text = "Set sample range";
            this.frmHVtooltip.SetToolTip(this.SetButton, "Set the voltage start/end/interval values");
            this.SetButton.UseVisualStyleBackColor = false;
            this.SetButton.Click += new System.EventHandler(this.SetButton_Click);
            // 
            // ImageWriteButton
            // 
            this.ImageWriteButton.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.ImageWriteButton.Enabled = false;
            this.ImageWriteButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.ImageWriteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImageWriteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImageWriteButton.Location = new System.Drawing.Point(790, 64);
            this.ImageWriteButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ImageWriteButton.Name = "ImageWriteButton";
            this.ImageWriteButton.Size = new System.Drawing.Size(175, 39);
            this.ImageWriteButton.TabIndex = 29;
            this.ImageWriteButton.Text = "Save to Image";
            this.frmHVtooltip.SetToolTip(this.ImageWriteButton, "Save the results to an image file");
            this.ImageWriteButton.UseVisualStyleBackColor = false;
            this.ImageWriteButton.Click += new System.EventHandler(this.ImageWriteButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(372, 201);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(290, 23);
            this.progressBar1.TabIndex = 30;
            this.progressBar1.UseWaitCursor = true;
            this.progressBar1.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbl_progress
            // 
            this.lbl_progress.AutoSize = true;
            this.lbl_progress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_progress.Location = new System.Drawing.Point(371, 178);
            this.lbl_progress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_progress.Name = "lbl_progress";
            this.lbl_progress.Size = new System.Drawing.Size(90, 20);
            this.lbl_progress.TabIndex = 31;
            this.lbl_progress.Text = "Stablizing...";
            this.lbl_progress.Visible = false;
            // 
            // FormHighVoltage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1093, 867);
            this.Controls.Add(this.lbl_progress);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.ImageWriteButton);
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
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FormHighVoltage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "High Voltage";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HighVoltagePlateau_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.HighVoltagePlateau_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.VoltagePlateauDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.Button ImageWriteButton;
        private System.Windows.Forms.ToolTip frmHVtooltip;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbl_progress;
    }
}