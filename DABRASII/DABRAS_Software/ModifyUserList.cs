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
    public partial class ModifyUserList : Form
    {
        #region Data Members
        private DefaultConfigurations DC;
        private DABRAS DABRAS;
        private Logger Logger;
        private Form LaunchedFrom;
        private List<User> ListOfUsers;

        private User TargetedUser;
        #endregion

        #region Constructor
        public ModifyUserList(Form _LaunchedFrom, DefaultConfigurations _DC, DABRAS _DAB, Logger _L)
        {
            InitializeComponent();

            this.LaunchedFrom = _LaunchedFrom;
            this.DC = _DC;
            this.DABRAS = _DAB;
            this.Logger = _L;

            this.ListOfUsers = DC.GetListOfUsers();

            #region DataGridView Initialization
            EmployeeGridView.Columns.Add("BadgeNo", "Badge Number");
            EmployeeGridView.Columns.Add("Name", "Employee Name");

            PopulateDataGridView();
            #endregion

            /*Load first element*/
            EmployeeGridView_CellClick(this, new DataGridViewCellEventArgs(0, 0));

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

        }
        #endregion

        #region Click Handler
        private void EmployeeGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow DGR = EmployeeGridView.Rows[e.RowIndex];

                try
                {
                    int BadgeNo = Convert.ToInt32(DGR.Cells["BadgeNo"].Value);
                    string Name = Convert.ToString(DGR.Cells["Name"].Value);

                    this.TargetedUser = ListOfUsers.Find(x => (x.GetBadgeNo() == BadgeNo && x.GetName() == Name));

                    this.Badge_TB.Text = Convert.ToString(BadgeNo);
                    this.Name_TB.Text = Name;
                }
                catch
                {
                    MessageBox.Show("Error: bad values");
                }
            }
        }
        #endregion

        #region Add Button Handler
        private void AddButton_Click(object sender, EventArgs e)
        {
            int NewBadge = 0;
            string NewName = "";
            try
            {
                NewBadge = Convert.ToInt32(this.Badge_TB.Text);
                NewName = Convert.ToString(this.Name_TB.Text);
            }

            catch
            {
                MessageBox.Show("Error: Bad values.");
                return;
            }

            User NewUser = new User(NewBadge, NewName);
            if (ListOfUsers == null)
            {
                ListOfUsers = new List<User>();
            }
            ListOfUsers.Add(NewUser);

            PopulateDataGridView();
        }
        #endregion

        #region Edit Button Handler
        private void EditButton_Click(object sender, EventArgs e)
        {
            if (TargetedUser == null)
            {
                try
                {
                    TargetedUser = ListOfUsers.Find(x => ((x.GetName() == Convert.ToString(this.Name_TB.Text)) && (x.GetBadgeNo() == Convert.ToInt32(this.Badge_TB.Text))));
                }

                catch
                {
                    MessageBox.Show("Error: Bad values.");
                    return;
                }
            }

            if (TargetedUser == null)
            {
                MessageBox.Show("Error: User not found. Please create a new user by clicking \"Add\"");
                return;
            }

            TargetedUser.SetBadgeNo(Convert.ToInt32(this.Badge_TB.Text));
            TargetedUser.SetName(Convert.ToString(this.Name_TB.Text));
            
            PopulateDataGridView();

            MessageBox.Show("Changes saved.");
            return;
        }
        #endregion

        #region Delete Button Handler
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            User DeleteUser;
            try
            {
                DeleteUser = ListOfUsers.Find(x => ((x.GetName() == Convert.ToString(this.Name_TB.Text)) && (x.GetBadgeNo() == Convert.ToInt32(this.Badge_TB.Text))));
            }

            catch
            {
                MessageBox.Show("Error: Bad values.");
                return;
            }

            if (DeleteUser == null)
            {
                MessageBox.Show("Error: User does not exist.");
                return;
            }

            ListOfUsers.Remove(DeleteUser);

            PopulateDataGridView();

            MessageBox.Show("User removed!");
            return;
        }
        #endregion

        #region Private Utility Functions
        private void AbortAll()
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        private void PopulateDataGridView()
        {
            EmployeeGridView.Rows.Clear();

            if (ListOfUsers != null)
            {
                foreach (User U in ListOfUsers)
                {
                    EmployeeGridView.Rows.Add(new object[] { U.GetBadgeNo(), U.GetName() });
                }
            }
            return;
        }
        #endregion

        #region Port Connect Handler
        private void connectDisconnectAPortCtrlPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VCPConnect NewPopup = new VCPConnect("Connect");
            if ((NewPopup.ShowDialog()) == DialogResult.OK)
            {
                /*The user clicked OK! Attempt to connect to the DABRAS*/
                bool Successful = DABRAS.Get_Coms(NewPopup.VCP_Port);
                if (!(Successful))
                {
                    MessageBox.Show("Error: Communication could not be established.");
                    Logger.WriteLineToStatusLog(String.Format("Communication attempt failed at time {0}", DateTime.Now));

                    return;
                }
                else
                {
                    MessageBox.Show("Communication Established.");
                    Logger.WriteLineToStatusLog(String.Format("Communication successfully established at time {0}", DateTime.Now));
                }

                DABRAS.Initialize();

                /*Write to constants*/
                return;
            }

            if (NewPopup.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }
        #endregion

        #region KeyPress Handler

        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Shift && Key.Alt)
            {
                AbortAll();
            }

            if (Key.Control)
            {
                if (Key.KeyCode == Keys.Q)
                {
                    closeCtrlQToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.A)
                {
                    addCtrlAToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.E)
                {
                    editCtrlEToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.D)
                {
                    deleteCtrlDToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.P)
                {
                    connectDisconnectAPortCtrlPToolStripMenuItem_Click(this, null);
                }

            }
        }

        #endregion

        #region Dummy Overloads
        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void addCtrlAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddButton_Click(this, null);
        }

        private void editCtrlEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditButton_Click(this, null);
        }

        private void deleteCtrlDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteButton_Click(this, null);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm NewForm = new AboutForm();
            NewForm.ShowDialog();
        }
        #endregion

        #region Finalization
        private void ModifyUserList_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*Write list of users to environment*/
            this.DC.SetListOfUsers(this.ListOfUsers);
            
            this.LaunchedFrom.Show();
        }
        #endregion

    }
}
