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
    public partial class LongPrompt : Form
    {
        #region Data Members
        private string Response;
        #endregion

        #region Constructor
        public LongPrompt(string _Prompt)
        {
            InitializeComponent();

            if (_Prompt != null)
            {
                this.PromptLable.Text = _Prompt;
            }
        }
        #endregion

        #region Submit Handler
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            this.Response = this.TextBox.Text;
            this.DialogResult = DialogResult.OK;
            return;
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
