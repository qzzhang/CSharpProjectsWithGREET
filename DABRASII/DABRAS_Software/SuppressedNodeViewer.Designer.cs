namespace DABRAS_Software
{
    partial class SuppressedNodeViewer
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
            this.SuppressedNodesDataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.SuppressedNodesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // SupressedNodesDataGridView
            // 
            this.SuppressedNodesDataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.SuppressedNodesDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SuppressedNodesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SuppressedNodesDataGridView.Location = new System.Drawing.Point(12, 12);
            this.SuppressedNodesDataGridView.Name = "SupressedNodesDataGridView";
            this.SuppressedNodesDataGridView.Size = new System.Drawing.Size(1194, 300);
            this.SuppressedNodesDataGridView.TabIndex = 0;
            this.SuppressedNodesDataGridView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.SupressedNodesDataGridView_RowHeaderMouseClick);
            // 
            // SuppressedNodeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1218, 324);
            this.Controls.Add(this.SuppressedNodesDataGridView);
            this.Name = "SuppressedNodeViewer";
            this.Text = "SuppressedNodeViewer";
            ((System.ComponentModel.ISupportInitialize)(this.SuppressedNodesDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView SuppressedNodesDataGridView;
    }
}