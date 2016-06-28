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
    public partial class DailyQCLog : Form
    {

        private List<QCCalResultNode> ListOfNodes;
        private Form LaunchedFrom;
        private DABRAS DABRAS;
        private Logger Logger;

        private int SelectedIndex;

        public DailyQCLog(Form Parent, List<QCCalResultNode> _LN, DABRAS _D, Logger _L)
        {
            InitializeComponent();

            this.LaunchedFrom = Parent;
            this.ListOfNodes = _LN;
            this.DABRAS = _D;
            this.Logger = _L;

            if (ListOfNodes != null && ListOfNodes.Count > 0)
            {
                FillData(ListOfNodes[ListOfNodes.Count() - 1]);
                this.SelectedIndex = ListOfNodes.Count() - 1;
                this.NextButton.Enabled = false;
            }

            else
            {
                MessageBox.Show("No nodes to display.");
                this.Close();
                return;
            }

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }

       

        private void FillData(QCCalResultNode Node)
        {
            this.BadgeNo_TB.Text = Convert.ToString(Node.GetBadgeNo());
            this.Comment_TB.Text = Node.GetComment();
            this.Name_TB.Text = Node.GetName();
            this.NetAlpha_TB.Text = Convert.ToString(Node.GetNetAlphaCPM());
            this.NetBeta_TB.Text = Convert.ToString(Node.GetNetBetaCPM());
            this.TimeCompleted_TB.Text = Convert.ToString(Node.GetDateTimeCompleted());

            switch (Node.GetTypeOfQC())
            {
                case FormQC.TypeOfQC.Alpha:
                    this.QCType_TB.Text = "Alpha";
                    break;
                case FormQC.TypeOfQC.Beta:
                    this.QCType_TB.Text = "Beta";
                    break;
                case FormQC.TypeOfQC.Background:
                    this.QCType_TB.Text = "Background";
                    break;
                default:
                    break;
            }

            return;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            SelectedIndex--;
            FillData(ListOfNodes[SelectedIndex]);

            BackButton.Enabled = !(SelectedIndex == 0);
            NextButton.Enabled = !(SelectedIndex == (ListOfNodes.Count() - 1));
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            SelectedIndex++;
            FillData(ListOfNodes[SelectedIndex]);

            NextButton.Enabled = !(SelectedIndex == (ListOfNodes.Count() - 1));
            BackButton.Enabled = !(SelectedIndex == 0);
        }

        private void DailyQCLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.LaunchedFrom.Show();
        }

        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void nextCtrlNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NextButton.Enabled)
            {
                NextButton_Click(this, null);
            }
        }

        private void backCtrlBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BackButton.Enabled)
            {
                BackButton_Click(this, null);
            }
        }

        private void connectOrDisconnectAPortCtrlPToolStripMenuItem_Click(object sender, EventArgs e)
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

                    DABRAS.VCP_Instance = "";

                    return;
                }
                else
                {
                    MessageBox.Show("Communication Established.");
                    Logger.WriteLineToStatusLog(String.Format("Communication successfully established at time {0}", DateTime.Now));
                }

                DABRAS.Initialize();

                return;
            }

            if (NewPopup.DialogResult == DialogResult.Abort)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm AF = new AboutForm();
            AF.ShowDialog();
        }

        void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Shift && Key.Alt)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }

            if (Key.Control)
            {
                if (Key.KeyCode == Keys.Q)
                {
                    closeCtrlQToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.N)
                {
                    nextCtrlNToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.B)
                {
                    backCtrlBToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.P)
                {
                    connectOrDisconnectAPortCtrlPToolStripMenuItem_Click(this, null);
                }
            }
        }
        
        
    }
}
