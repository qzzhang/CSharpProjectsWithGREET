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
    public partial class Prompt : Form
    {
        #region Data Members
        private string Response = "";
        #endregion

        #region Constructor
        public Prompt(string Field)
        {
            InitializeComponent();
            this.MessageLabel.Text = Field;
        }
        #endregion

        #region Submit Handler
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            this.Response = this.String_TB.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region Getters
        public string GetResponse()
        {
            return this.Response;
        }
        #endregion
    }
}
