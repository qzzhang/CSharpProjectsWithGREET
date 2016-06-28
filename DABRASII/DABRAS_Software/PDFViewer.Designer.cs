namespace DABRAS_Software
{
    partial class PDFViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PDFViewer));
            this.PDF_Space = new AxAcroPDFLib.AxAcroPDF();
            ((System.ComponentModel.ISupportInitialize)(this.PDF_Space)).BeginInit();
            this.SuspendLayout();
            // 
            // PDF_Space
            // 
            this.PDF_Space.Enabled = true;
            this.PDF_Space.Location = new System.Drawing.Point(12, 12);
            this.PDF_Space.Name = "PDF_Space";
            this.PDF_Space.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("PDF_Space.OcxState")));
            this.PDF_Space.Size = new System.Drawing.Size(930, 715);
            this.PDF_Space.TabIndex = 0;
            // 
            // PDFViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 739);
            this.Controls.Add(this.PDF_Space);
            this.Name = "PDFViewer";
            this.Text = "PDFViewer";
            ((System.ComponentModel.ISupportInitialize)(this.PDF_Space)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxAcroPDFLib.AxAcroPDF PDF_Space;

    }
}