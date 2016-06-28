using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DABRAS_Software
{
    public partial class BatchNumber : Form
    {
        #region Data Members
        private int sampleID;
        #endregion

        #region Constructor
        public BatchNumber()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

            this.ActiveControl = this.Batch_ComboBox;
        }
        #endregion

        #region Private Utility Functions
        private void Batch_ComboBox_Click(object sender, EventArgs e)
        {
            this.Batch_ComboBox.Items.Clear();

            string RoutineRoot = String.Format("{0}\\data\\Routine", Environment.CurrentDirectory);

            try
            {
                if (!Directory.Exists(RoutineRoot))
                {
                    Directory.CreateDirectory(RoutineRoot);
                }
            }
            catch
            {
                return;
            }

            string[] ListOfUserDirectories = Directory.GetDirectories(RoutineRoot);

            foreach (string s in ListOfUserDirectories)
            {
                try
                {
                    DateTime LastWritten = Directory.GetLastAccessTime(s);
                    DateTime Today = DateTime.Now;

                    if ((Today.Year == LastWritten.Year) && (Today.Month == LastWritten.Month) && (Today.Day == LastWritten.Day))
                    {
                        string FolderName = s.Remove(0, RoutineRoot.Length);

                        this.Batch_ComboBox.Items.Add(Convert.ToInt32(FolderName.Replace('\\', '0'))); //This will fail for Master.
                    }
                }

                catch
                {
                    ;
                }
            }
        }
        #endregion

        #region Submit Handler
        private void Submit_Button_Click(object sender, EventArgs e)
        {
            /*Uncomment to check for an integer*/
            string smpid = this.Batch_ComboBox.Text;
            if (smpid == "") 
            {/*Pressing “Submit” with nothing entered should proceed to the Routine Sample Counting Screen*/
                this.DialogResult = DialogResult.OK;
                return;
            }

            try
            {
                sampleID = Convert.ToInt32(smpid);
            }

            catch
            {
                MessageBox.Show("Error: Not a valid number");
                //this.DialogResult = DialogResult.Abort; //Uncomment to kill the rest of this. 
                return;
            }

            /*Comment this next line out when integer checking*/
            //GroupID = this.Batch_ComboBox.Text;

            this.DialogResult = DialogResult.OK;
            return;
        }
        #endregion

        #region Cancel Handler
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            string smpid = this.Batch_ComboBox.Text;
            if (smpid == "")
            {/*Pressing “Cancel” with nothing entered should proceed to the Routine Sample Counting Screen*/
                this.DialogResult = DialogResult.OK;
                return;
            }
            //this.Close();
        }
        #endregion

        #region Getters
        public int Get_Group_ID()
        {
            return sampleID;
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
                Submit_Button_Click(this, null);
                return;
            }

            if (Key.KeyCode == Keys.Escape)
            {
                Cancel_Button_Click(this,null);
            }
        }
        #endregion

    }
}
