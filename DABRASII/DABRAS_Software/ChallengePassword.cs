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
    public partial class ChallengePassword : Form
    {
        #region Data Members
        private string UserEnteredPassword;
        #endregion

        #region Constructor
        public ChallengePassword()
        {
            InitializeComponent();
            this.UserEnteredPassword = "";

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(UserPressedAKey);

            this.ActiveControl = this.Password_Textbox;
        }
        #endregion

        #region Submit Handler
        private void Submit_button_Click(object sender, EventArgs e)
        {
            this.UserEnteredPassword = this.Password_Textbox.Text;
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region Getters
        public string GetUserEnteredPassword()
        {
            return this.UserEnteredPassword;
        }
        #endregion

        #region Keypress Handler
        private void UserPressedAKey(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }
            
            if (Key.KeyCode == Keys.Enter)
            {
                Submit_button_Click(this, null);
            }

            if (Key.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
        #endregion
    }
}
