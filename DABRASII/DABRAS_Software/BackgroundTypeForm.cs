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
    public partial class BackgroundTypeForm : Form
    {
        #region Data Members
        private mainFramework.BackgroundType Type;
        #endregion

        #region Constructor
        public BackgroundTypeForm(mainFramework.BackgroundType _Type)
        {
            InitializeComponent();

            this.Type = _Type;

            if (this.Type == mainFramework.BackgroundType.Annual)
            {
                this.Annual_Button.Checked = true;
            }

            else if (this.Type == mainFramework.BackgroundType.Daily)
            {
                this.Daily_Button.Checked = true;
            }

            /*Add keypress event handlers*/
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Save Handler
        private void Save_Button_Click(object sender, EventArgs e)
        {
            if (this.Annual_Button.Checked)
            {
                this.Type = mainFramework.BackgroundType.Annual;
            }

            if (this.Daily_Button.Checked)
            {
                this.Type = mainFramework.BackgroundType.Daily;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }
        #endregion

        #region Private Utility Functions
        public mainFramework.BackgroundType GetBackgroundType()
        {
            return this.Type;
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
            
            if (Key.Control)
            {
                if (Key.KeyCode == Keys.S)
                {
                    Save_Button_Click(this, null);
                }
            }
        }
        #endregion
    }
}
