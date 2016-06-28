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
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            ArgonneLogoBox.Image = DABRAS_Software.Properties.Resources.Logo;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }
        }
    }
}
