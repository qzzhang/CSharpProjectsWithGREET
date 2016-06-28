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
    public partial class ModFactorForm : Form
    {
        #region Data Members
        private ModFactors MF;
        #endregion

        #region Constructor
        public ModFactorForm(ModFactors _MF)
        {
            InitializeComponent();

            if (_MF != null)
            {
                this.Alpha_Absorption_Mod_TB.Text = Convert.ToString(_MF.GetAlphaSelfAbsorbtion());
                this.Beta_Absorption_Mod_TB.Text = Convert.ToString(_MF.GetBetaSelfAbsorbtion());
                this.Beta_Backscatter_Mod_TB.Text = Convert.ToString(_MF.GetBetaBackscatter());
                this.Removal_Percentage_Mod_TB.Text = Convert.ToString(_MF.GetRemovalFrac());
                this.Area_Mod_TB.Text = Convert.ToString(_MF.GetDefaultSampleArea());
            }

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Submit Button Handler
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                MF = new ModFactors(Convert.ToDouble(this.Alpha_Absorption_Mod_TB.Text), Convert.ToDouble(this.Beta_Absorption_Mod_TB.Text), Convert.ToDouble(this.Beta_Backscatter_Mod_TB.Text), Convert.ToDouble(this.Removal_Percentage_Mod_TB.Text), Convert.ToDouble(this.Area_Mod_TB.Text));
                this.DialogResult = DialogResult.OK;
                return;
            }
            catch
            {
                MessageBox.Show("Error: Bad Values.");
            }
        }
        #endregion

        #region CancelButton Handler
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion

        #region Getters
        public ModFactors GetModFactors()
        {
            return this.MF;
        }
        #endregion

        #region KeyPress Handler
        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                this.DialogResult = DialogResult.Abort;
                return;
            }
        }
        #endregion
    }
}
