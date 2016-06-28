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
    public partial class AutomationControls : Form
    {
        #region Data Members
        private DABRAS DABRAS;
        private bool DABRASModified = false;
        private DefaultConfigurations DC;
        private bool DCModified = false;

        private Logger Logger;
        private Form LaunchedFrom;
        #endregion

        #region Constructor
        public AutomationControls(QCMain Parent)
        {
            InitializeComponent();

            this.DABRAS = Parent.GetDABRAS();

            this.DC = Parent.GetDefaultConfigurations();

            this.Daily_AMPM_Combobox.Items.Add("AM");
            this.Daily_AMPM_Combobox.Items.Add("PM");
            this.Daily_AMPM_Combobox.Text = "AM";

            this.Continuous_CB.Checked = this.DABRAS.ContinuousBackgroundMonitorAlive();
            this.Daily_CB.Checked = this.DABRAS.DailyBackgroundMonitorAlive();

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

            this.LaunchedFrom = Parent;

            return;
        }
        #endregion

        #region Close Form Handler
        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }
        #endregion

        #region Start/Stop Continuous Handler
        private void Continuous_CB_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Continuous_CB.Checked)
            {
                if (!DABRAS.IsConnected())
                {
                    MessageBox.Show("Error: Must be connected to the DABRAS to enable this feature. Please re-connect and try again.");
                    this.Continuous_CB.Checked = false;
                    return;
                }
                
                int Timespan = 0;
                int AlphaHi = 0;
                int AlphaLo = 0;
                int BetaHi = 0;
                int BetaLo = 0;

                try
                {
                    Timespan = (3600 * Convert.ToInt32(this.Continuous_Hour_TB.Text)) + (60 * Convert.ToInt32(this.Continuous_Min_TB.Text));
                    AlphaHi = Convert.ToInt32(this.Continuous_AlphaHi_TB.Text);
                    AlphaLo = Convert.ToInt32(this.Continuous_AlphaLo_TB.Text);
                    BetaHi = Convert.ToInt32(this.Continuous_BetaHi_TB.Text);
                    BetaLo = Convert.ToInt32(this.Continuous_BetaLo_TB.Text);
                }

                catch
                {
                    MessageBox.Show("Error: Bad values.");
                    return;
                }

                /*Swap values if they are in wrong places for the alphahi/alphalo and betahi/betalo pairs*/
                if (AlphaLo > AlphaHi)
                {
                    int temp = AlphaLo;
                    AlphaLo = AlphaHi;
                    AlphaHi = temp;

                    string tempstring = this.Continuous_AlphaLo_TB.Text;
                    this.Continuous_AlphaLo_TB.Text = this.Continuous_AlphaHi_TB.Text;
                    this.Continuous_AlphaHi_TB.Text = tempstring;
                }

                if (BetaLo > BetaHi)
                {
                    int temp = BetaLo;
                    BetaLo = BetaHi;
                    BetaHi = temp;

                    string tempstring = this.Continuous_BetaLo_TB.Text;
                    this.Continuous_BetaLo_TB.Text = this.Continuous_BetaHi_TB.Text;
                    this.Continuous_BetaHi_TB.Text = tempstring;
                }


                /*Start a new Background thread on the DABRAS*/
                this.DABRAS.StartMonitoringBackgroundContinuously(AlphaLo, BetaLo, AlphaHi, BetaHi, Timespan);
                this.DABRASModified = true;
            }

            else
            {
                if (this.DABRAS.ContinuousBackgroundMonitorAlive())
                {
                    this.DABRAS.KillContinuousBackgroundMonitor();
                    this.DABRASModified = true;
                }
            }
        }

        #endregion

        #region Kill Background Button Handler
        private void Kill_BG_Button_Click(object sender, EventArgs e)
        {
            if (this.DABRAS.ContinuousBackgroundMonitorAlive())
            {
                this.DABRAS.KillContinuousBackgroundMonitor();
                this.DABRASModified = true;
                MessageBox.Show("Background monitor disabled.");
            }

            this.Continuous_CB.Checked = false;
        }
        #endregion

        #region Enable/Disable Daily Monitor Handler
        private void Daily_CB_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Daily_CB.Checked)
            {
                if (!DABRAS.IsConnected())
                {
                    MessageBox.Show("Error: Must be connected to the DABRAS to enable this feature. Please re-connect and try again.");
                    this.Daily_CB.Checked = false;
                    return;
                }
                
                DateTime StartTime = DateTime.Now;
                int TimeToRun = 0;
                int AlphaHi = 0;
                int AlphaLo = 0;
                int BetaHi = 0;
                int BetaLo = 0;

                try
                {
                    /*Set to midnight today*/
                    DateTime Target = DateTime.Today;

                    if (Daily_AMPM_Combobox.Text == "AM")
                    {
                        int TargetHour = Convert.ToInt32(this.Daily_Datetime_Hr_TB.Text);

                        if ((TargetHour < 0) || (TargetHour > 12))
                        {
                            ThrowException();
                        }

                        if (TargetHour == 12)
                        {
                            TargetHour = 0;
                        }

                        Target = Target.AddHours(TargetHour);
                    }

                    else
                    {
                        int TargetHour = Convert.ToInt32(this.Daily_Datetime_Hr_TB.Text) + 12;

                        if ((TargetHour < 12) || (TargetHour > 24))
                        {
                            ThrowException();
                        }

                        if (TargetHour == 24)
                        {
                            TargetHour = 12;
                        }

                        Target = Target.AddHours(TargetHour);
                    }

                    int TargetMinutes = Convert.ToInt32(this.Daily_DateTime_Min_TB.Text);

                    if ((TargetMinutes > 60) || (TargetMinutes < 0))
                    {
                        ThrowException();
                    }

                    Target = Target.AddMinutes(TargetMinutes);

                    /*Check to see if this event occours today or tomorrow*/
                    int Diff = DateTime.Compare(Target, DateTime.Now);
                    if (Diff < 0)
                    {
                        Target = Target.AddDays(1);
                    }

                    StartTime = Target;

                    TimeToRun = (Convert.ToInt32(this.Daily_Time_Hr_TB.Text) * 3600) + (Convert.ToInt32(this.Daily_Time_Min_TB.Text) * 60) + (Convert.ToInt32(this.Daily_Time_Sec_TB.Text));
                    AlphaLo = Convert.ToInt32(this.Daily_AlphaLo_TB.Text);
                    AlphaHi = Convert.ToInt32(this.Daily_AlphaHi_TB.Text);
                    BetaLo = Convert.ToInt32(this.Daily_BetaLo_TB.Text);
                    BetaHi = Convert.ToInt32(this.Daily_BetaHi_TB.Text);
                }

                catch
                {
                    MessageBox.Show("Bad Values!");
                    return;
                }

                this.DABRAS.StartMonitoringBackgroundDaily(AlphaLo, BetaLo, AlphaHi, BetaHi, TimeToRun, StartTime);
                this.DABRASModified = true;
            }
        }
        #endregion

        #region Privat Utility Functions
        public void ThrowException()
        {
            DivideByZeroException ex = new DivideByZeroException();
            throw ex;
        }
        #endregion

        #region Getters
        public bool WasDABRASModified()
        {
            return this.DABRASModified;
        }

        public bool WasDefaultConfigurationsModified()
        {
            return this.DCModified;
        }

        public DABRAS GetDABRAS()
        {
            return this.DABRAS;
        }

        public DefaultConfigurations GetDefaultConfig()
        {
            return this.DC;
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
        }
        #endregion

        #region Finalization
        private void AutomationControls_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.LaunchedFrom.Show();
        }
        #endregion

    }
}
