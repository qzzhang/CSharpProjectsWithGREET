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
    public partial class ChiSquaredCalibrationRequirements : Form
    {
        #region Data members
        private DefaultConfigurations DC;
        #endregion

        #region Constructor
        public ChiSquaredCalibrationRequirements(DefaultConfigurations _DC)
        {
            InitializeComponent();

            this.DC = _DC;
        }
        #endregion

        #region Set Button Handler
        private void Set_Button_Click(object sender, EventArgs e)
        {
            if (this.Never_Button.Checked)
            {
                TimeSpan T = new TimeSpan();
                T = T.Add(TimeSpan.MaxValue);
                DC.SetChiSquaredTimespan(T);
            }

            else if (this.Annually_Button.Checked)
            {
                DC.SetChiSquaredTimespan(new TimeSpan(365, 0, 0, 0));
            }

            else if (this.Monthly_Button.Checked)
            {
                DC.SetChiSquaredTimespan(new TimeSpan(31, 0, 0, 0));
            }

            else if (this.Daily_Button.Checked)
            {
                DC.SetChiSquaredTimespan(new TimeSpan(1, 0, 0, 0));
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }
        #endregion

        #region Cancel Button Handler
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            return;
        }
        #endregion

        #region Getters
        public DefaultConfigurations GetDefaultConfigurations()
        {
            return this.DC;
        }
        #endregion
    }
}
