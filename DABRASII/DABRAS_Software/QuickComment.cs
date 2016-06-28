using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DABRAS_Software
{
    public partial class QuickComment : Form
    {
        #region Data Members
        private string Comment = "";
        #endregion

        #region Constructor
        public QuickComment()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Submit Button Click
        private void Submit_Button_Click(object sender, EventArgs e)
        {
            this.Comment = Comment_TB.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }
        #endregion

        #region Cancel Button Click
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            return;
        }
        #endregion

        #region Getters
        public string GetComment()
        {
            return this.Comment;
        }
        #endregion

        #region KeyPress Handlers
        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }

            if (Key.Control)
            {
                if (Key.KeyCode == Keys.S)
                {
                    Submit_Button_Click(this, null);
                    return;
                }

                if (Key.KeyCode == Keys.C)
                {
                    Cancel_Button_Click(this, null);
                    return;
                }
            }
        }
        #endregion
    }
}
