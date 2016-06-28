namespace DABRAS_Software
{
    partial class BetaEfficiencyChart
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Energy_Level_TB = new System.Windows.Forms.TextBox();
            this.Efficiency_TB = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ComputeButton = new System.Windows.Forms.Button();
            this.ImageSaveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).BeginInit();
            this.SuspendLayout();
            // 
            // Chart
            // 
            chartArea1.Name = "ChartArea1";
            this.Chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.Chart.Legends.Add(legend1);
            this.Chart.Location = new System.Drawing.Point(12, 12);
            this.Chart.Name = "Chart";
            this.Chart.Size = new System.Drawing.Size(721, 444);
            this.Chart.TabIndex = 7;
            this.Chart.Text = "chart1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(740, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Compute Beta Efficiency";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(744, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Energy Level (KeV)";
            // 
            // Energy_Level_TB
            // 
            this.Energy_Level_TB.Location = new System.Drawing.Point(759, 54);
            this.Energy_Level_TB.Name = "Energy_Level_TB";
            this.Energy_Level_TB.Size = new System.Drawing.Size(100, 20);
            this.Energy_Level_TB.TabIndex = 0;
            // 
            // Efficiency_TB
            // 
            this.Efficiency_TB.Location = new System.Drawing.Point(759, 95);
            this.Efficiency_TB.Name = "Efficiency_TB";
            this.Efficiency_TB.Size = new System.Drawing.Size(100, 20);
            this.Efficiency_TB.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(744, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Efficiency";
            // 
            // ComputeButton
            // 
            this.ComputeButton.Location = new System.Drawing.Point(747, 122);
            this.ComputeButton.Name = "ComputeButton";
            this.ComputeButton.Size = new System.Drawing.Size(112, 23);
            this.ComputeButton.TabIndex = 2;
            this.ComputeButton.Text = "Compute (Ctrl + K)";
            this.ComputeButton.UseVisualStyleBackColor = true;
            this.ComputeButton.Click += new System.EventHandler(this.ComputeButton_Click);
            // 
            // ImageSaveButton
            // 
            this.ImageSaveButton.Location = new System.Drawing.Point(747, 152);
            this.ImageSaveButton.Name = "ImageSaveButton";
            this.ImageSaveButton.Size = new System.Drawing.Size(112, 23);
            this.ImageSaveButton.TabIndex = 3;
            this.ImageSaveButton.Text = "Save Image (Ctrl + I)";
            this.ImageSaveButton.UseVisualStyleBackColor = true;
            this.ImageSaveButton.Click += new System.EventHandler(this.ImageSaveButton_Click);
            // 
            // BetaEfficiencyChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 468);
            this.Controls.Add(this.ImageSaveButton);
            this.Controls.Add(this.ComputeButton);
            this.Controls.Add(this.Efficiency_TB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Energy_Level_TB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Chart);
            this.Name = "BetaEfficiencyChart";
            this.Text = "BetaEfficiencyChart";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BetaEfficiencyChart_FormClosing);
            this.Load += new System.EventHandler(this.BetaEfficiencyChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart Chart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Energy_Level_TB;
        private System.Windows.Forms.TextBox Efficiency_TB;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ComputeButton;
        private System.Windows.Forms.Button ImageSaveButton;
    }
}