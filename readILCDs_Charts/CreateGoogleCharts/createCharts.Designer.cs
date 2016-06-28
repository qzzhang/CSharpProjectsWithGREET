namespace CreateGoogleCharts
{
    partial class createCharts
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
            this.rawDataDGV = new System.Windows.Forms.DataGridView();
            this.btnChartDashboard = new System.Windows.Forms.Button();
            this.btnCloseCharting = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkListBox_cols = new System.Windows.Forms.CheckedListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnLoadData = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.rawDataDGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rawDataDGV
            // 
            this.rawDataDGV.AllowUserToAddRows = false;
            this.rawDataDGV.AllowUserToDeleteRows = false;
            this.rawDataDGV.AllowUserToResizeColumns = false;
            this.rawDataDGV.AllowUserToResizeRows = false;
            this.rawDataDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.rawDataDGV.Location = new System.Drawing.Point(12, 317);
            this.rawDataDGV.MultiSelect = false;
            this.rawDataDGV.Name = "rawDataDGV";
            this.rawDataDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.rawDataDGV.Size = new System.Drawing.Size(441, 315);
            this.rawDataDGV.TabIndex = 12;
            // 
            // btnChartDashboard
            // 
            this.btnChartDashboard.Location = new System.Drawing.Point(468, 597);
            this.btnChartDashboard.Name = "btnChartDashboard";
            this.btnChartDashboard.Size = new System.Drawing.Size(140, 35);
            this.btnChartDashboard.TabIndex = 17;
            this.btnChartDashboard.Text = "Create Chart Dashboard";
            this.btnChartDashboard.UseVisualStyleBackColor = true;
            this.btnChartDashboard.Click += new System.EventHandler(this.btnChartDashboard_Click);
            // 
            // btnCloseCharting
            // 
            this.btnCloseCharting.Location = new System.Drawing.Point(614, 597);
            this.btnCloseCharting.Name = "btnCloseCharting";
            this.btnCloseCharting.Size = new System.Drawing.Size(79, 35);
            this.btnCloseCharting.TabIndex = 16;
            this.btnCloseCharting.Text = "Close";
            this.btnCloseCharting.UseVisualStyleBackColor = true;
            this.btnCloseCharting.Click += new System.EventHandler(this.btnCloseCharting_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 52);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listView1);
            this.splitContainer1.Size = new System.Drawing.Size(410, 207);
            this.splitContainer1.SplitterDistance = 133;
            this.splitContainer1.TabIndex = 15;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(133, 207);
            this.treeView1.TabIndex = 19;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(273, 207);
            this.listView1.TabIndex = 19;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Last Modified";
            // 
            // chkListBox_cols
            // 
            this.chkListBox_cols.FormattingEnabled = true;
            this.chkListBox_cols.Location = new System.Drawing.Point(479, 317);
            this.chkListBox_cols.Name = "chkListBox_cols";
            this.chkListBox_cols.Size = new System.Drawing.Size(180, 184);
            this.chkListBox_cols.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Select the CSV file for charting:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(526, 295);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Select columns:";
            // 
            // btnLoadData
            // 
            this.btnLoadData.Location = new System.Drawing.Point(12, 280);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(86, 28);
            this.btnLoadData.TabIndex = 21;
            this.btnLoadData.Text = "Load Data";
            this.btnLoadData.UseVisualStyleBackColor = true;
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            // 
            // createCharts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 668);
            this.Controls.Add(this.btnLoadData);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkListBox_cols);
            this.Controls.Add(this.btnChartDashboard);
            this.Controls.Add(this.btnCloseCharting);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rawDataDGV);
            this.Name = "createCharts";
            this.Text = "Generate Google Chart Dashboard";
            ((System.ComponentModel.ISupportInitialize)(this.rawDataDGV)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView rawDataDGV;
        private System.Windows.Forms.Button btnChartDashboard;
        private System.Windows.Forms.Button btnCloseCharting;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckedListBox chkListBox_cols;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnLoadData;
    }
}