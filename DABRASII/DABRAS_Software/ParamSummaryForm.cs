using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;


/*************************************
 * 
 * THIS FORM IS NOW DEFUNCT. DO NOT USE
 * I AM ONLY KEEPING IT AROUND FOR REFERENCE
 * IN CASE ANYONE WANTS TO KNOW WHAT IS ON
 * THE FRAM
 *************************************/



namespace DABRAS_Software
{
    public partial class ParamSummaryForm : Form
    {
        #region Data Members
        private readonly HomeForm LaunchedFrom; //For getting references to 
        public DABRAS DABRAS;
        public Logger CurrentLogger;
        private DefaultConfigurations DC;
        private bool NewDABRAS;
        private bool _Refresh = true; //goofy name to avoid hiding
        #endregion

        #region Constructor
        public ParamSummaryForm(HomeForm Parent)
        {
            InitializeComponent();

            this.LaunchedFrom = Parent;
            DABRAS = LaunchedFrom.GetDABRAS();
            CurrentLogger = LaunchedFrom.GetLogger();
            NewDABRAS = false;
            this.DC = LaunchedFrom.GetDefaultConfig();
        }
        #endregion

        #region OnLoad/Refresh Function
        public void ParamSummaryForm_Load(object sender, EventArgs e)
        {
            if (DABRAS.IsConnected())
            {

                /*Pause any background threads*/
                DABRAS.Cut();

                this.SN_Label.Text = "DABRAS-II s/n " + DABRAS.Serial_Number;
                this.VCP_Label.Text = "Connected on " + DABRAS.VCP_Instance;

                #region HV_Control
                /*Get HV Control Signal*/
                DABRAS.ClearPacketFlag();
                DABRAS.ClearRaw();
                DABRAS.Write_To_Serial_Port("S");
                while (!(DABRAS.IsDataReady()))
                {
                    Thread.Sleep(1);
                }

                string ReturnedHVControlSignal = DABRAS.ReadRawSerialString();
                ReturnedHVControlSignal = ReturnedHVControlSignal.Replace("\n", "").Replace("\r", "").Replace("S", "");
                double HV_Control = 0;
                try
                {
                    HV_Control = Convert.ToDouble(ReturnedHVControlSignal);
                }
                catch
                {
                    MessageBox.Show(String.Format("Error: bad HV control. Received {0}", ReturnedHVControlSignal));
                    return;
                }

                this.HVC_Label.Text = String.Format("HV Control set to {0:0.0000} V", ((HV_Control * 5) / 16383));
                #endregion

                /*Put the machine into utility mode*/
                DABRAS.ClearPacketFlag();
                DABRAS.ClearRaw();

                DABRAS.Write_To_Serial_Port("U");
                while (!DABRAS.IsDataReady())
                {
                    Thread.Sleep(1);
                }

                string ResultOfUtility = DABRAS.ReadRawSerialString().Replace("\n", "").Replace("\r", "");
                if (String.Compare(ResultOfUtility, "OK") != 0)
                {
                    MessageBox.Show("Error entering utility mode");
                    return;
                }

                DABRAS.ClearPacketFlag();
                DABRAS.ClearRaw();

                #region Background
                /*Dump page zero to a string. This will be the background efficiencies.*/
                /*The packet is coming back in this format:
                 * <"Bkgd Data", <Int - Alpha backgroud> , <Int - Beta backgroud> , <String - date/time> , <int - ????>
                 */
                DABRAS.Write_To_Serial_Port("D");

                Thread.Sleep(150);

                DABRAS.Write_To_Serial_Port("000");

                while (!DABRAS.IsDataReady())
                {
                    Thread.Sleep(1);
                }

                string BackgroundDump = DABRAS.ReadRawSerialString();

                /*Split the packet into parts by comma*/
                string[] SplitPacket = BackgroundDump.Split(',');

                /*Write to labels*/
                this.Alpha_BG_Label.Text = "Alpha Background: " + SplitPacket[1];
                this.Beta_BG_Label.Text = "Beta Background: " + SplitPacket[2];
                this.DateTime_Determined_Label.Text = "Background Determined: " + SplitPacket[3];

                #endregion

                #region Efficiencies

                /*The data is stored on page 1 for Am-241, 2 for Tc-99, and 4 for Sr-90. Do NOT use page 3, that will cause the program to hang.*/
                /*The data packet comes back in this format:*/
                /* <Source name, Type of radiation ("Alpha" or "Beta"), Alpha BG, Alpha error, Beta BG, Beta Error, Time last calibrated>*/
                /*For convenience, Extract_Correct_Coefficients() will take in this data packet and select the correct values for the label.*/

                #region Am-241
                /*For Am-241*/
                DABRAS.Write_To_Serial_Port("D");

                Thread.Sleep(150);

                DABRAS.Write_To_Serial_Port("001");

                while (!DABRAS.IsDataReady())
                {
                    Thread.Sleep(1);
                }

                string AmDump = DABRAS.ReadRawSerialString();
                SplitPacket = null;
                SplitPacket = AmDump.Split(',');

                double[] EffValues = Extract_Correct_Efficiencies(SplitPacket);

                this.Efficiency_AM241_Label.Text = "Efficiency for Am-241 = " + Convert.ToString(EffValues[0]) + " c/d ± " + Convert.ToString(EffValues[1]) + " c/d";
                #endregion

                #region Tc-99

                /*For Tc-99*/
                DABRAS.Write_To_Serial_Port("D");

                Thread.Sleep(150);

                DABRAS.Write_To_Serial_Port("002");

                while (!DABRAS.IsDataReady())
                {
                    Thread.Sleep(1);
                }

                string TcDump = DABRAS.ReadRawSerialString();
                SplitPacket = null;
                SplitPacket = TcDump.Split(',');

                EffValues = Extract_Correct_Efficiencies(SplitPacket);

                this.Efficiency_TC99_Label.Text = "Efficiency for Tc-99 = " + Convert.ToString(EffValues[0]) + " c/d± " + Convert.ToString(EffValues[1]) + " c/d";
                #endregion

                #region Sr-90
                /*For Sr-90*/
                DABRAS.Write_To_Serial_Port("D");

                Thread.Sleep(150);

                DABRAS.Write_To_Serial_Port("004"); //Who the hell knows why this is on page 4? Who the hell knows why attempting to read from page 3 freezes the program? LOL.

                while (!DABRAS.IsDataReady())
                {
                    Thread.Sleep(1);
                }

                string SrDump = DABRAS.ReadRawSerialString();
                SplitPacket = null;
                SplitPacket = SrDump.Split(',');

                EffValues = Extract_Correct_Efficiencies(SplitPacket);

                this.Efficiency_Sr90_Label.Text = "Efficiency for Sr-90 = " + Convert.ToString(EffValues[0]) + " c/d ± " + Convert.ToString(EffValues[1]) + " c/d";
                #endregion

                #endregion


                /*Remove the machine from utility mode*/
                DABRAS.Write_To_Serial_Port("u");

                /*Make lables visible*/
                this.SN_Label.Visible = true;
                this.VCP_Label.Visible = true;
                this.HVC_Label.Visible = true;
                this.Alpha_BG_Label.Visible = true;
                this.Beta_BG_Label.Visible = true;
                this.DateTime_Determined_Label.Visible = true;
                this.Efficiency_AM241_Label.Visible = true;
                this.Efficiency_Sr90_Label.Visible = true;
                this.Efficiency_TC99_Label.Visible = true;

                /*Hide not connected label*/
                this.NoConnect_Label.Visible = false;
            }

            /*No connection*/
            else
            {
                /*Hide all labels*/
                this.SN_Label.Visible = false;
                this.VCP_Label.Visible = false;
                this.HVC_Label.Visible = false;
                this.Alpha_BG_Label.Visible = false;
                this.Beta_BG_Label.Visible = false;
                this.DateTime_Determined_Label.Visible = false;
                this.Efficiency_AM241_Label.Visible = false;
                this.Efficiency_Sr90_Label.Visible = false;
                this.Efficiency_TC99_Label.Visible = false;

                /*Show not connected label*/
                this.NoConnect_Label.Visible = true;

                /*Show error message*/
                MessageBox.Show("WARNING: DABRAS not connected.");
            }

            /*Add keypress event handlers*/
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

            this.KeyUp += new KeyEventHandler(KeyReleased);

            /*Reinstate any background threads*/
            DABRAS.ResumeBackgroundMonitors();

            return;
        }
        #endregion

        #region Private Utility Functions
        private double[] Extract_Correct_Efficiencies(string[] Raw)
        {
            /*Determine type of radiation*/
            double[] EffValues = { 0, 0 };
            if (string.Compare(Raw[1], "Alpha") == 0)
            {
                /*It's alpha! Return members 2,3*/
                
                EffValues[0] = Convert.ToDouble(Raw[2]);
                EffValues[1] = Convert.ToDouble(Raw[3]);
            }
            else
            {
                /*It's beta! Return members 4,5*/
                EffValues[0] = Convert.ToDouble(Raw[4]);
                EffValues[1] = Convert.ToDouble(Raw[5]);
            }

            return EffValues;
        }

        private void AbortAll()
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
            return;
        }
        #endregion

        #region Port Connect Handler
        private void connectOrDisconnectAPortCtrlPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*This is pretty much copy-pasted from the home form*/
            /*Show the dialogue*/
            VCPConnect NewPopup = new VCPConnect("Connect");
            if (NewPopup.ShowDialog() == DialogResult.OK)
            {
                /*The user clicked OK! Attempt to connect to the DABRAS*/
                DABRAS = new DABRAS();
                bool Successful = DABRAS.Get_Coms(NewPopup.VCP_Port);
                if (!(Successful))
                {
                    MessageBox.Show("Error: Communication could not be established.");
                    CurrentLogger.WriteLineToStatusLog(String.Format("Communication attempt failed at time {0}", DateTime.Now));

                    return;
                }
                else
                {
                    MessageBox.Show("Communication Established.");
                    CurrentLogger.WriteLineToStatusLog(String.Format("Communication successfully established at time {0}", DateTime.Now));
                }

                DABRAS.Initialize();
                this.NewDABRAS = true;
            }

            if (NewPopup.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }

        }
        #endregion

        #region Close Handler
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region WebForm Handler
        private void openWebBasedSurveyFormF12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DC.GetWebSurvey());
            NewForm.Show();
        }

        private void openRSOSharepointF11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DC.GetRSOLink());
            NewForm.Show();
        }

        private void openRSOHomepageF10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DC.GetRSOHome());
            NewForm.Show();
        }
        #endregion

        #region Getters

        public bool New_Instance_Of_DABRAS()
        {
            return this.NewDABRAS;
        }
        #endregion 

        #region Dummy Overloads
        private void refreshCtrlRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this._Refresh)
            {
                this._Refresh = false;
                ParamSummaryForm_Load(this, null);
            }
        }

        #endregion

        #region KeyPress Handlers
        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                AbortAll();
            }
            
            if (Key.Control)
            {
                if (Key.KeyCode == Keys.Q)
                {
                    closeToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.R)
                {
                    refreshCtrlRToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.P)
                {
                    connectOrDisconnectAPortCtrlPToolStripMenuItem_Click(this, null);
                }
            }

            if (Key.KeyCode == Keys.F12)
            {
                openWebBasedSurveyFormF12ToolStripMenuItem_Click(this, null);
            }

            if (Key.KeyCode == Keys.F11)
            {
                openRSOSharepointF11ToolStripMenuItem_Click(this, null);
            }

            if (Key.KeyCode == Keys.F10)
            {
                openRSOHomepageF10ToolStripMenuItem_Click(this, null);
            }
        }

        private void KeyReleased(object sender, KeyEventArgs Key)
        {
            if (Key.KeyCode == Keys.R)
            {
                this._Refresh = true;
            }
        }
        #endregion

        #region Finalization
        private void ParamSummaryForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.LaunchedFrom.Show();
        }
        #endregion
    }

}
