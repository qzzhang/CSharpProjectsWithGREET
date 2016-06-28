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
    public partial class HV_Exiting : Form
    {
        #region Data Members
        public double LowValue = 0;
        public double HighValue = 0;

        public double FinalValue { get; set; }
        #endregion

        #region Constructor
        public HV_Exiting(double LowValue, double HighValue)
        {
            InitializeComponent();

            this.SetTop_Button.Text = String.Format("Leave HV Control at {0} mV", HighValue);
            this.SetBottom_Button.Text = String.Format("Set HV Control to {0} mV", LowValue);

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region OK Button Handler
        private void OK_Button_Click(object sender, EventArgs e)
        {
            if (this.SetTop_Button.Checked)
            {
                FinalValue = HighValue;
                CloseForm();
            }

            if (this.SetBottom_Button.Checked)
            {
                FinalValue = LowValue;
                CloseForm();
            }

            if (this.SetMid_Button.Checked)
            {
                try
                {
                    FinalValue = Convert.ToDouble(this.MidValue_TB.Text);
                    CloseForm();
                }

                catch
                {
                    MessageBox.Show("Error: Bad Values");
                    return;
                }
            }
        }
        #endregion

        #region Close Handler
        private void CloseForm()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
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

            if (Key.KeyCode == Keys.Enter)
            {
                OK_Button_Click(this, null);
            }
        }
        #endregion
    }
}
