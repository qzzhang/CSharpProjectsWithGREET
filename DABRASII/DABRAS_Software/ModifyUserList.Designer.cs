namespace DABRAS_Software
{
    partial class ModifyUserList
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
            this.EmployeeGridView = new System.Windows.Forms.DataGridView();
            this.Badge_TB = new System.Windows.Forms.TextBox();
            this.Name_TB = new System.Windows.Forms.TextBox();
            this.AddButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeCtrlQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCtrlAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCtrlEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteCtrlDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectDisconnectAPortCtrlPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.EmployeeGridView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // EmployeeGridView
            // 
            this.EmployeeGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.EmployeeGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.EmployeeGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.EmployeeGridView.GridColor = System.Drawing.SystemColors.Control;
            this.EmployeeGridView.Location = new System.Drawing.Point(12, 112);
            this.EmployeeGridView.Name = "EmployeeGridView";
            this.EmployeeGridView.Size = new System.Drawing.Size(721, 409);
            this.EmployeeGridView.TabIndex = 0;
            this.EmployeeGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.EmployeeGridView_CellClick);
            // 
            // Badge_TB
            // 
            this.Badge_TB.Location = new System.Drawing.Point(217, 45);
            this.Badge_TB.Name = "Badge_TB";
            this.Badge_TB.Size = new System.Drawing.Size(100, 20);
            this.Badge_TB.TabIndex = 1;
            // 
            // Name_TB
            // 
            this.Name_TB.Location = new System.Drawing.Point(217, 71);
            this.Name_TB.Name = "Name_TB";
            this.Name_TB.Size = new System.Drawing.Size(100, 20);
            this.Name_TB.TabIndex = 2;
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(412, 48);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(100, 43);
            this.AddButton.TabIndex = 3;
            this.AddButton.Text = "Add (Ctrl + A)";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Location = new System.Drawing.Point(624, 48);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(100, 43);
            this.DeleteButton.TabIndex = 4;
            this.DeleteButton.Text = "Delete (Ctrl + D)";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // EditButton
            // 
            this.EditButton.Location = new System.Drawing.Point(518, 48);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(100, 43);
            this.EditButton.TabIndex = 5;
            this.EditButton.Text = "Edit (Ctrl + E)";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(127, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Badge No:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(82, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(129, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Employee Name:";
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
            this.menuStrip1.Size = new System.Drawing.Size(751, 24);
            this.menuStrip1.TabIndex = 8;
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
            this.addCtrlAToolStripMenuItem,
            this.editCtrlEToolStripMenuItem,
            this.deleteCtrlDToolStripMenuItem});
            this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
            this.functionToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.functionToolStripMenuItem.Text = "Function";
            // 
            // addCtrlAToolStripMenuItem
            // 
            this.addCtrlAToolStripMenuItem.Name = "addCtrlAToolStripMenuItem";
            this.addCtrlAToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.addCtrlAToolStripMenuItem.Text = "Add (Ctrl + A)";
            this.addCtrlAToolStripMenuItem.Click += new System.EventHandler(this.addCtrlAToolStripMenuItem_Click);
            // 
            // editCtrlEToolStripMenuItem
            // 
            this.editCtrlEToolStripMenuItem.Name = "editCtrlEToolStripMenuItem";
            this.editCtrlEToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.editCtrlEToolStripMenuItem.Text = "Edit (Ctrl + E)";
            this.editCtrlEToolStripMenuItem.Click += new System.EventHandler(this.editCtrlEToolStripMenuItem_Click);
            // 
            // deleteCtrlDToolStripMenuItem
            // 
            this.deleteCtrlDToolStripMenuItem.Name = "deleteCtrlDToolStripMenuItem";
            this.deleteCtrlDToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.deleteCtrlDToolStripMenuItem.Text = "Delete (Ctrl + D)";
            this.deleteCtrlDToolStripMenuItem.Click += new System.EventHandler(this.deleteCtrlDToolStripMenuItem_Click);
            // 
            // utilityToolStripMenuItem
            // 
            this.utilityToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectDisconnectAPortCtrlPToolStripMenuItem});
            this.utilityToolStripMenuItem.Name = "utilityToolStripMenuItem";
            this.utilityToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.utilityToolStripMenuItem.Text = "Utility";
            // 
            // connectDisconnectAPortCtrlPToolStripMenuItem
            // 
            this.connectDisconnectAPortCtrlPToolStripMenuItem.Name = "connectDisconnectAPortCtrlPToolStripMenuItem";
            this.connectDisconnectAPortCtrlPToolStripMenuItem.Size = new System.Drawing.Size(274, 22);
            this.connectDisconnectAPortCtrlPToolStripMenuItem.Text = "Connect / Disconnect a Port (Ctrl + P)";
            this.connectDisconnectAPortCtrlPToolStripMenuItem.Click += new System.EventHandler(this.connectDisconnectAPortCtrlPToolStripMenuItem_Click);
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
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // ModifyUserList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 533);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EditButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.Name_TB);
            this.Controls.Add(this.Badge_TB);
            this.Controls.Add(this.EmployeeGridView);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ModifyUserList";
            this.Text = "ModifyUserList";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModifyUserList_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.EmployeeGridView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView EmployeeGridView;
        private System.Windows.Forms.TextBox Badge_TB;
        private System.Windows.Forms.TextBox Name_TB;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.Button EditButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeCtrlQToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem functionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addCtrlAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editCtrlEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteCtrlDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilityToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectDisconnectAPortCtrlPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}