using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace DABRAS_Software
{
    public partial class SetHighVoltageForm : Form
    {
        #region Data Members
        private DABRAS DABRAS;
        #endregion

        #region Constructor
        public SetHighVoltageForm(DABRAS _DABRAS)
        {
            InitializeComponent();
            this.DABRAS = _DABRAS;
            
            if (!DABRAS.IsConnected())
            {
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            
            CurrentHighVoltageLabel.Text = String.Format("The Current voltage Setting is: {0} mV", StaticMethods.RoundToSpecificSigFigs((DABRAS.GetHVC() * 5000 / 16383), 4));

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Set Button Handler
        private void SetButton_Click(object sender, EventArgs e)
        {
            double SetVoltage = 0;
            try
            {
                SetVoltage = Convert.ToDouble(this.HV_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Bad value.");
                return;
            }

            DABRAS.SetHVC(SetVoltage);

            MessageBox.Show("Voltage set!");
            CurrentHighVoltageLabel.Text = String.Format("The Current voltage Setting is: {0} mV", StaticMethods.RoundToSpecificSigFigs((DABRAS.GetHVC() * 5000 / 16383), 4));
        }
        #endregion

        #region Cancel Button Handler
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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
                SetButton_Click(this, null);
            }
        }
        #endregion
    }
}
