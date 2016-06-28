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
    public partial class ChangePasswordForm : Form
    {
        #region Data Members
        /*There's not really any error checking to be done here...*/
        private DefaultConfigurations DC;
        #endregion

        #region Constructor
        public ChangePasswordForm(DefaultConfigurations _DC)
        {
            InitializeComponent();
            this.DC = _DC;

            /*Add keypress event handlers*/
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Cancel Handler
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region Submit Button Handler
        private void Submit_Button_Click(object sender, EventArgs e)
        {
            if (this.Old_PW_TB.Text == DC.GetPassword())
            {
                if (this.New_PW_TB_1.Text == this.New_PW_TB_2.Text)
                {
                    DC.SetPassword(this.New_PW_TB_1.Text);
                    MessageBox.Show("Password Changed.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }

                else
                {
                    MessageBox.Show("Error: Passwords do not match. Password not changed.");
                    return;
                }
            }

            else
            {
                MessageBox.Show("Error: Incorrect old password. Password not changed");
                return;
            }
        }
        #endregion

        #region KeyPress Handler
        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }
            
            if (Key.KeyCode == Keys.Escape)
            {
                Cancel_Button_Click(this, null);
            }

            if (Key.KeyCode == Keys.Enter)
            {
                Submit_Button_Click(this, null);
            }
        }

        #endregion
    }
}
