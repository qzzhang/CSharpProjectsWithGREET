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
    public partial class LinkTargets : Form
    {
        #region Data Members
        private DefaultConfigurations DC;
        #endregion

        #region Constructor
        public LinkTargets(DefaultConfigurations _DC)
        {
            InitializeComponent();

            this.DC = _DC;

            this.Web_Survey_TB.Text = DC.GetWebSurvey();
            this.RSO_Home_TB.Text = DC.GetRSOHome();
            this.RSO_Link_TB.Text = DC.GetRSOLink();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

            return;
        }
        #endregion

        #region Save Button Handler
        private void Save_Button_Click(object sender, EventArgs e)
        {
            DC.SetWebSurvey(Web_Survey_TB.Text);
            DC.SetRSOHome(RSO_Home_TB.Text);
            DC.SetRSOLink(RSO_Link_TB.Text);

            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
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

        #region Reset Button Handler
        private void ResetButton_Click(object sender, EventArgs e)
        {
            DC.SetWebSurvey("https://webapps.inside.anl.gov/rwp/");
            DC.SetRSOLink("https://sharepoint.anl.gov/Divisions/esq/rso/HealthPhysicsProcedures/HPP%203.0%20Rev.%209.pdf");
            DC.SetRSOHome("https://sharepoint.anl.gov/Divisions/esq/rso/HealthPhysicsProcedures/Forms/AllItems.aspx");

            this.Web_Survey_TB.Text = "https://webapps.inside.anl.gov/rwp/";
            this.RSO_Home_TB.Text = "https://sharepoint.anl.gov/Divisions/esq/rso/HealthPhysicsProcedures/Forms/AllItems.aspx";
            this.RSO_Link_TB.Text = "https://sharepoint.anl.gov/Divisions/esq/rso/HealthPhysicsProcedures/HPP%203.0%20Rev.%209.pdf";

            return;
        }
        #endregion

        #region Getters
        public DefaultConfigurations GetDefaultConfig()
        {
            return this.DC;
        }
        #endregion

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
