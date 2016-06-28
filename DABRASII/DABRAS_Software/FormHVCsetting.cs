using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DABRAS_Software
{
    public partial class FormHVCsetting : Form
    {
        #region Data Members
        private double LowValue = 0;
        private double HighValue = 0;
        public double FinalValue { get; set; }
        private DABRAS dbrs;
        #endregion

        #region Constructors
        public FormHVCsetting(double lValue, double hValue, DABRAS dbs)
        {
            InitializeComponent();
            this.LowValue = lValue;
            this.HighValue = hValue;
            this.rdSetTopHV.Text = String.Format("Set HV Control to the range top at {0} mV", this.HighValue);
            this.rdSetBottomHV.Text = String.Format("Set HV Control to the range bottom at {0} mV", this.LowValue);
            this.dbrs = dbs;

            this.checkHVCsignal();
        }

        public FormHVCsetting(DABRAS dbs)
        {
            InitializeComponent();
            this.rdSetBottomHV.Enabled = false;
            this.rdSetTopHV.Enabled = false;
            this.rdSetBottomHV.Visible = false;
            this.rdSetTopHV.Visible = false;

            this.rdSetMidHV.Checked = true;
            this.dbrs = dbs;

            this.checkHVCsignal();
        }
        #endregion

        #region OK Button Handler
        private void checkHVCsignal()
        {
            double d_HVC = dbrs.GetHVC();
            if (d_HVC != -1)
            {
                this.CurrentHighVoltageLabel.ForeColor = Color.Black;
                this.CurrentHighVoltageLabel.Text = String.Format("The Current voltage Setting is: {0} mV", StaticMethods.RoundToSpecificSigFigs((d_HVC * 5000 / 16383), 4));
            }
            else
            {
                this.CurrentHighVoltageLabel.ForeColor = Color.Red;
                this.CurrentHighVoltageLabel.Text = "The Current voltage setting is invalid.";
            }
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            if (this.rdSetTopHV.Checked)
            {
                this.FinalValue = this.HighValue;
            }

            if (this.rdSetBottomHV.Checked)
            {
                this.FinalValue = this.LowValue;
            }

            if (this.rdSetMidHV.Checked)
            {
                try
                {
                    this.FinalValue = Convert.ToDouble(this.MidValue_TB.Text);
                }
                catch
                {
                    MessageBox.Show("Error: Bad Values");
                    return;
                }
            }

            if (this.dbrs.SetHVC(this.FinalValue))
            {
                Thread.Sleep(25);
                this.checkHVCsignal();
                return;
            }
            else
            {
                this.CurrentHighVoltageLabel.ForeColor = Color.Red;
                this.CurrentHighVoltageLabel.Text = "The Current voltage setting failed.";
                return;
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


        private void btnCancelHVCset_Click(object sender, EventArgs e)
        {
            this.CloseForm();
            /*
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            return;
             */
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
