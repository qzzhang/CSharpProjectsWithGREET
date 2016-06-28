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
    public partial class VCPConnect : Form
    {
        #region Data Members
        public string VCP_Port;
        #endregion

        #region Constructor
        public VCPConnect(string constr)
        {
            VCP_Port = null;
            InitializeComponent();
            this.btnConnect.Text = constr;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
            this.initComPorts();
        }
        #endregion

        #region Connect Button Handler
        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.VCP_Port = VCP_Ports.Text;
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region Misc GUI Functions
        /*VCP_Ports_Gotfocus
         * This method will be triggered every time focus is given to the VCP combobox.
         * A standard method to retreive the list of VCP data
         * 
         * @author Mitchell Spryn
         * @version 0.1
         */

        private void VCP_Ports_GotFocus(object sender, EventArgs e)
        {
            this.initComPorts();
        }
        private void initComPorts()
        {
            string[] AvailableCOMPorts;

            // Clear combobox
            this.VCP_Ports.Items.Clear();
            this.VCP_Ports.Text = "";

            // Determine the COM ports currently available
            AvailableCOMPorts = System.IO.Ports.SerialPort.GetPortNames();
            var desc = from s in AvailableCOMPorts
                       orderby s descending
                       select s;

            //for (int j = 0; j < AvailableCOMPorts.Length; j++)
            foreach (string c in desc)
            {
                // Add it to the combo box.
                //this.VCP_Ports.Items.Add(AvailableCOMPorts[j]);
                this.VCP_Ports.Items.Add(c);
            }
            this.VCP_Ports.Text = this.VCP_Ports.Items[0].ToString();
            this.VCP_Ports.SelectedIndex = 0;
        }
        #endregion

        #region Keypress Handler

        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }
        }

        #endregion
    }
}
