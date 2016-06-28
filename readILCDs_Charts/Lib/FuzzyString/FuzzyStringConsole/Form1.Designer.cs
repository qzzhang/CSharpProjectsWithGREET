namespace FuzzyStringConsole
{
    partial class Form1
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listView5 = new System.Windows.Forms.ListView();
            this.listView6 = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(48, 40);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(570, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // listView5
            // 
            this.listView5.Location = new System.Drawing.Point(48, 66);
            this.listView5.Name = "listView5";
            this.listView5.Size = new System.Drawing.Size(186, 130);
            this.listView5.TabIndex = 5;
            this.listView5.UseCompatibleStateImageBehavior = false;
            // 
            // listView6
            // 
            this.listView6.Location = new System.Drawing.Point(262, 66);
            this.listView6.Name = "listView6";
            this.listView6.Size = new System.Drawing.Size(186, 130);
            this.listView6.TabIndex = 6;
            this.listView6.UseCompatibleStateImageBehavior = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 498);
            this.Controls.Add(this.listView6);
            this.Controls.Add(this.listView5);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ListView listView5;
        private System.Windows.Forms.ListView listView6;
    }
}