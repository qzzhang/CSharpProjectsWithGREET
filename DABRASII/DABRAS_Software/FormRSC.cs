using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace DABRAS_Software
{
    public partial class FormRSC : Form
    {
        #region Enums
        public enum RoutineSampleCountType { Time, MDA };
        #endregion

        #region Data Members
        private readonly mainFramework frmParent;
        private DABRAS dabras;
        private RoutineSampleCountType RType = RoutineSampleCountType.Time;
        private RoutineSampleListener RL;
        private Thread BackgroundThread;

        private const double ConfidenceRange = 1.645;
        private const double ConfRange = 1.645; //95 % confidence range
        private double Current_Alpha_Eff = -1;
        private double Current_Beta_Eff = -1;

        private double Alpha_SelfAbsorbtion = 0;
        private double Beta_SelfAbsorbtion = 0;
        private double Beta_Backscatter = 0;
        private double Contaminant_Removal_Fraction = 0; //Between 0 and 1
        private double Sample_Area = 0;
        private double AlphaMDA = 0;
        private double BetaMDA = 0;

        private int SampleTime = 0;
        private int index = 1; //incremented every time a sample is taken.

        public delegate void UpdateFormCallback();

        private bool Calibrating = false; //This should ONLY be set to true when using in quickcal mode

        private double AlphaGCPM;
        private double BetaGCPM;

        private double AlphaDPM;
        private double BetaDPM;
        private bool UnsavedData = false;

        private ModFactors MF;

        #endregion

        #region Constructor
        public FormRSC(mainFramework _parent)
        {
            this.frmParent = _parent;
            this.dabras = this.frmParent.GetDABRAS();
            InitializeComponent();
            this.initRSC();
            this.initRSCgridByFile();
            this.SetGUI_RSC(false);
            this.Continue_Count_Button.Enabled = false; //At the birth of this form, there is no suspended counting, so the continue button should not be enabled.

            //Add keypress event handlers
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region New Count Handler
        private void New_Count_Button_Click(object sender, EventArgs e)
        {
            //Check to see if the machine is calibrated
            RadionuclideFamily Background = this.frmParent.GetListOfFamily().Find(x => x.GetName() == "Background");
            RadionuclideFamily AlphaSource = this.frmParent.GetListOfFamily().Find(x => x.GetName() == Alpha_ComboBox.Text);
            RadionuclideFamily BetaSource = this.frmParent.GetListOfFamily().Find(x => x.GetName() == Beta_ComboBox.Text);
            bool RequireCalibrationPassword = false;
            
            RequireCalibrationPassword = this.CheckAnnualCalib(Background, AlphaSource, BetaSource);
            RequireCalibrationPassword = this.CheckDailyCalib(Background, AlphaSource, BetaSource);
            RequireCalibrationPassword = this.CheckChiSquared();

            //Check continuous monitor
            if (!Calibrating)
            {
                bool AutoCalOK = (this.dabras.ValidateContinuousAlphaBackground() && this.dabras.ValidateContinuousBetaBackground()) || (DateTime.Compare(this.dabras.GetValidationDate(), Background.GetCurrentSource().GetDailyCalibratedTime()) < 0);

                if (!AutoCalOK)
                {
                    MessageBox.Show("Error: The continuous background monitor detects high levels of background radiation. Either disable the continuous background monitor or recalibrate the background.");
                    RequireCalibrationPassword = true;
                }
            }
            
            /*//Allow override with calibration password
            if (RequireCalibrationPassword)
            {
                if (MessageBox.Show("Override with Calibration Password?", "Attempt to Override?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ChallengePassword CP = new ChallengePassword();
                    CP.ShowDialog();
                    if (CP.GetUserEnteredPassword() == this.frmParent.DC.GetPassword())
                    {
                        this.Calibrating = true;
                        MessageBox.Show("Password accepted. Form unlocked.");
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Password.");
                        return;
                    }
                }
                else
                {
                    return;
                }
            }*/

            if (!this.dabras.IsConnected())
            {
                MessageBox.Show("Error: No connection to the DABRAS. Please re-connect, and try again.");
                this.frmParent.refreshConnectStatus();
                this.endFormActivities();
                return;
            }

            double Alpha_Limit = 0;
            double Beta_Limit = 0;

            try
            {
                this.Alpha_SelfAbsorbtion = Convert.ToDouble(this.Alpha_Absorption_Mod_TB.Text);
                this.Beta_SelfAbsorbtion = Convert.ToDouble(this.Beta_Absorption_Mod_TB.Text);
                this.Beta_Backscatter = Convert.ToDouble(this.Beta_Backscatter_Mod_TB.Text);
                this.Contaminant_Removal_Fraction = Convert.ToDouble(this.Removal_Percentage_Mod_TB.Text);

                Alpha_Limit = Convert.ToDouble(this.GrossAlphaDPMLimit_TB.Text);
                Beta_Limit = Convert.ToDouble(this.GrossBetaDPMLimit_TB.Text);

                if (this.Contaminant_Removal_Fraction > 1)
                {
                    this.Contaminant_Removal_Fraction /= 100;
                }

                this.Sample_Area = Convert.ToDouble(Area_Mod_TB.Text);
            }
            catch
            {
                MessageBox.Show("Bad values");
                return;
            }

            if (!Calibrating)
            {
                if (this.RType == RoutineSampleCountType.Time)
                {
                    if ((MessageBox.Show(String.Format("Count for {0}:{1:00}?", ((SampleTime + 1) / 60), ((SampleTime + 1) % 60)), "Confirm", MessageBoxButtons.YesNo)) != DialogResult.Yes)
                    {
                        MessageBox.Show("Aborted.");
                        return;
                    }
                }

                else if (this.RType == RoutineSampleCountType.MDA)
                {
                    if (MessageBox.Show(String.Format("Count until an alpha MDA of {0} and a beta MDA of {1} is reached?", AlphaMDA, BetaMDA), "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        MessageBox.Show("Aborted");
                        return;
                    }
                }

            }

            Elapsed_Time_Label.Text = "Elapsed time: 0:00";

            Status_Label.Text = "Aquiring";

            string NewColumnName = String.Format("NewColumn{0}", this.index);

            if (FullDataResults.ColumnCount > 1)
            {
                DataGridViewColumn DGC = (DataGridViewColumn)FullDataResults.Columns[1].Clone();
                DGC.HeaderText = Convert.ToString(index);
                FullDataResults.Columns.Insert(1, DGC);
                FullDataResults[1, 1].ReadOnly = false; //Allow for user to edit description field
            }

            else
            {
                FullDataResults.Columns.Add("NewColumn", Convert.ToString(index));
            }

            FullDataResults["NewColumn", 0].Value = this.Sample_ID_TB.Text;//changed by QZ from "this.BadgeNo;"
            FullDataResults["NewColumn", 1].Value = "Sample description";//this.Description_TB.Text;
            FullDataResults["NewColumn", 1].Selected = true;//added by QZ on Feb. 16, 2016
            FullDataResults["NewColumn", 2].Value = DateTime.Now;

            if (this.RType == RoutineSampleCountType.Time)
            {
                FullDataResults["NewColumn", 4].Value = String.Format("{0}:{1:00}", ((SampleTime + 1) / 60), ((SampleTime + 1) % 60));
            }

            if (this.frmParent.BGType == mainFramework.BackgroundType.Annual)
            {
                FullDataResults["NewColumn", 9].Value = this.frmParent.GetListOfFamily().Find(x => x.GetName() == "Background").GetCurrentSource().GetAnnualCalibratedTime();
                FullDataResults["NewColumn", 10].Value = this.frmParent.GetListOfFamily().Find(x => x.GetName() == "Background").GetCurrentSource().GetAnnualCalibratedTimespan();
                FullDataResults["NewColumn", 11].Value = this.frmParent.GetListOfFamily().Find(x => x.GetName() == "Background").GetCurrentSource().GetAnnualAlphaCPM();
                FullDataResults["NewColumn", 12].Value = this.frmParent.GetListOfFamily().Find(x => x.GetName() == "Background").GetCurrentSource().GetAnnualBetaCPM();
            }
            else if (this.frmParent.BGType == mainFramework.BackgroundType.Daily)
            {
                FullDataResults["NewColumn", 9].Value = this.frmParent.GetListOfFamily().Find(x => x.GetName() == "Background").GetCurrentSource().GetDailyCalibratedTime();
                FullDataResults["NewColumn", 10].Value = this.frmParent.GetListOfFamily().Find(x => x.GetName() == "Background").GetCurrentSource().GetDailyCalibratedTimespan();
                FullDataResults["NewColumn", 11].Value = this.frmParent.GetListOfFamily().Find(x => x.GetName() == "Background").GetCurrentSource().GetDailyAlphaCPM();
                FullDataResults["NewColumn", 12].Value = this.frmParent.GetListOfFamily().Find(x => x.GetName() == "Background").GetCurrentSource().GetDailyBetaCPM();
            }

            FullDataResults["NewColumn", 21].Value = this.Current_Alpha_Eff;
            FullDataResults["NewColumn", 22].Value = this.Current_Beta_Eff;
            FullDataResults["NewColumn", 23].Value = this.Current_Alpha_Eff;
            FullDataResults["NewColumn", 24].Value = this.Current_Beta_Eff;

            FullDataResults["NewColumn", 25].Value = this.Alpha_SelfAbsorbtion;
            FullDataResults["NewColumn", 26].Value = this.Beta_SelfAbsorbtion;
            FullDataResults["NewColumn", 27].Value = this.Beta_Backscatter;
            FullDataResults["NewColumn", 28].Value = this.Contaminant_Removal_Fraction;
            FullDataResults["NewColumn", 29].Value = this.Sample_Area;

            FullDataResults["NewColumn", 30].Value = String.Format("{0:G3}", Alpha_Limit);
            FullDataResults["NewColumn", 31].Value = String.Format("{0:G3}", Beta_Limit);

            //Don't allow the user to edit anything except for the description (It doesn't work the other way!)
            for (int i = 0; i < FullDataResults.RowCount; i++)
            {
                if (i != 1)
                {
                    FullDataResults[1, i].ReadOnly = true;
                }
            }

            if (this.frmParent.BGType == mainFramework.BackgroundType.Annual)
            {
                RL = new RoutineSampleListener(this.dabras, SampleTime, 0, this.FullDataResults, Convert.ToInt32(Background.GetCurrentSource().GetAnnualAlphaCPM()), Convert.ToInt32(Background.GetCurrentSource().GetAnnualBetaCPM()), this.Current_Alpha_Eff, this.Current_Beta_Eff, this.Alpha_SelfAbsorbtion, this.Beta_SelfAbsorbtion, this.Beta_Backscatter, this.Contaminant_Removal_Fraction, this.Sample_Area, Background.GetCurrentSource().GetAnnualCalibratedTimespan(), RType, this.AlphaMDA, this.BetaMDA);
            }

            if (this.frmParent.BGType == mainFramework.BackgroundType.Daily)
            {
                RL = new RoutineSampleListener(this.dabras, SampleTime, 0, this.FullDataResults, Convert.ToInt32(Background.GetCurrentSource().GetDailyAlphaCPM()), Convert.ToInt32(Background.GetCurrentSource().GetDailyBetaCPM()), this.Current_Alpha_Eff, this.Current_Beta_Eff, this.Alpha_SelfAbsorbtion, this.Beta_SelfAbsorbtion, this.Beta_Backscatter, this.Contaminant_Removal_Fraction, this.Sample_Area, Background.GetCurrentSource().GetDailyCalibratedTimespan(), RType, this.AlphaMDA, this.BetaMDA);
            }

            BackgroundThread = new Thread(() => RL.Run_Sample());
            BackgroundThread.Start();

            RL.BackgroundSampleThreadFinished += (s, args) => { InvokeCallback(); };
            RL.PacketReceived += (s, args) => { InvokeCallback(); };

            SetGUI_RSC(true);

            this.Alpha_Activity_TB.ForeColor = Color.Black;
            this.Beta_Activity_TB.ForeColor = Color.Black;

            this.index++;
        }

        //Check annual calibrations
        private bool CheckAnnualCalib(RadionuclideFamily bkgrnd, RadionuclideFamily alphasrc, RadionuclideFamily betasrc)
        {
            bool retval = false;
            if (!Calibrating)
            {
                //Verify that both sources are in calibration
                //Annual calibration will be due at the end of the month
                TimeSpan T = DateTime.Now.Subtract(bkgrnd.GetCurrentSource().GetAnnualCalibratedTime());
                DateTime CalDue = bkgrnd.GetCurrentSource().GetAnnualCalibratedTime().AddYears(1);

                if (((TimeSpan.Compare(T, new TimeSpan(365, 0, 0, 0))) > 0) && (CalDue.Month != DateTime.Now.Month) && (CalDue.Year != DateTime.Now.Year))
                {
                    MessageBox.Show(String.Format("Error: Background out of calibration. Last calibrated at {0}", bkgrnd.GetCurrentSource().GetAnnualCalibratedTime()));
                    retval = true;
                }

                T = DateTime.Now.Subtract(alphasrc.GetCurrentSource().GetAnnualCalibratedTime());

                if ((TimeSpan.Compare(T, new TimeSpan(365, 0, 0, 0))) > 0)
                {
                    MessageBox.Show(String.Format("Error: Alpha Source out of calibration. Last calibrated at {0}", alphasrc.GetCurrentSource().GetAnnualCalibratedTime()));
                    retval = true;
                }

                T = DateTime.Now.Subtract(betasrc.GetCurrentSource().GetAnnualCalibratedTime());

                if ((TimeSpan.Compare(T, new TimeSpan(365, 0, 0, 0))) > 0)
                {
                    MessageBox.Show(String.Format("Error: Beta Source out of calibration. Last calibrated at {0}", betasrc.GetCurrentSource().GetAnnualCalibratedTime()));
                    retval = true;
                }
            }
            return retval;
        }

        private bool CheckDailyCalib(RadionuclideFamily bkgrnd, RadionuclideFamily alphasrc, RadionuclideFamily betasrc)
        {
            bool retval = false;
            if (!Calibrating)
            {
                //Verify that both sources are in calibration
                TimeSpan T = DateTime.Now.Subtract(bkgrnd.GetCurrentSource().GetDailyCalibratedTime());
                if ((TimeSpan.Compare(T, new TimeSpan(1, 0, 0, 0))) > 0)
                {
                    MessageBox.Show(String.Format("Error: Background operational check needed. Last performed at {0}", bkgrnd.GetCurrentSource().GetDailyCalibratedTime()));
                    retval = true;
                }

                T = DateTime.Now.Subtract(alphasrc.GetCurrentSource().GetDailyCalibratedTime());

                if ((TimeSpan.Compare(T, new TimeSpan(1, 0, 0, 0))) > 0)
                {
                    MessageBox.Show(String.Format("Error: Alpha Source operational check needed. Last performed at {0}", alphasrc.GetCurrentSource().GetDailyCalibratedTime()));
                    retval = true;
                }

                T = DateTime.Now.Subtract(betasrc.GetCurrentSource().GetDailyCalibratedTime());
                //T = DateTime.Now.Subtract(this.frmParent.GetListOfFamily().Find(x => x.GetName() == "Sr-90").GetDailyCalibratedDate());

                if ((TimeSpan.Compare(T, new TimeSpan(1, 0, 0, 0))) > 0)
                {
                    MessageBox.Show(String.Format("Error: Beta Source operational check needed. Last performed at {0}", betasrc.GetCurrentSource().GetAnnualCalibratedTime()));
                    retval = true;
                }
            }
            return retval;
        }

        //Check Chi Squared
        private bool CheckChiSquared()
        {
            bool retval = false;
            
            DateTime DateCalibrated = this.frmParent.GetDefaultConfig().GetChiSquaredDate();

            if (this.frmParent.GetDefaultConfig().GetChiSquaredTimespan() != TimeSpan.MaxValue)
            {
                DateTime ChiSqExpires = DateCalibrated.Add(this.frmParent.GetDefaultConfig().GetChiSquaredTimespan());

                if (!Calibrating)
                {
                    if (DateTime.Compare(ChiSqExpires, DateTime.Now) < 0)
                    {
                        MessageBox.Show("Error: Chi squared test required.");
                        retval = true;
                    }
                }
            }
            return retval;
        }
        #endregion

        #region Show_Full_Data Checkbox Handlers
        private void Show_Full_Data_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            this.FullDataResults.Visible = Show_Full_Data_Checkbox.Checked;
            return;
        }
        #endregion

        #region Continue Sample Handler
        private void Continue_Count_Button_Click(object sender, EventArgs e)
        {
            if (RL != null)
            {
                if (RL.IsPaused())
                {
                    RL.Continue();
                }
            }

            SetGUI_RSC(true);

        }
        #endregion

        #region Pause Handler
        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (RL != null)
            {
                RL.RequestPause();
            }

            SetGUI_RSC(false);
        }
        #endregion

        #region Stop Sample Handler
        private void Stop_Count_Button_Click(object sender, EventArgs e)
        {
            if (RL != null)
            {
                RL.RequestStop();
                BackgroundThread.Join();
            }

            this.FullDataResults["NewColumn", 3].Value = "CANCELLED";
            this.Status_Label.Text = "Stopped";

            SetGUI_RSC(false);
        }
        #endregion

        #region Set Time Handler
        private void SetTimeButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.SampleTime = (60 * Convert.ToInt32(Min_TB.Text)) + Convert.ToInt32(Sec_TB.Text) - 1;

                this.AlphaMDA = Convert.ToDouble(this.AlphaMDALimit_TB.Text);
                this.BetaMDA = Convert.ToDouble(this.BetaMDALimit_TB.Text);

                if (this.RType == RoutineSampleCountType.Time)
                {
                    this.Preset_Time_Label.Text = String.Format("Preset time: {0}:{1:00}", Convert.ToInt32(Min_TB.Text), Convert.ToInt32(Sec_TB.Text));
                }

                else if (this.RType == RoutineSampleCountType.MDA)
                {
                    this.Preset_Time_Label.Text = String.Format("Alpha MDA < {0}\nBetaMDA < {1}", this.AlphaMDA, this.BetaMDA);
                }

                return;
            }

            catch
            {
                MessageBox.Show("Error: Bad Values");
                return;
            }
        }
        #endregion

        #region Combobox_Handlers
        private void Alpha_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadionuclideFamily RF = this.frmParent.GetListOfFamily().Find(x => x.GetName() == Alpha_ComboBox.Text);

            /*Set Alpha Efficiency*/
            if(RF != null)
                this.Current_Alpha_Eff = RF.GetCurrentSource().GetAlphaEfficiency();
        }

        private void Beta_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadionuclideFamily RF = this.frmParent.GetListOfFamily().Find(x => x.GetName() == Beta_ComboBox.Text);

            /*Set Beta Efficiency*/
            if(RF != null)
                this.Current_Beta_Eff = RF.GetCurrentSource().GetBetaEfficiency();
        }
        #endregion

        #region Count Type Radiobutton Handlers
        private void TimeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetTypeOfSample();
        }

        private void CountAlphaMDARadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetTypeOfSample();
        }

        private void CountBetaMDARadioButton_CheckedChanged(object sender, EventArgs e)
        {
            SetTypeOfSample();
        }
        #endregion

        #region Save CSV Button Handler
        private void btnSaveRSCcsv_Click(object sender, EventArgs e)
        {
            string[,] DataToWrite = this.frmParent.MakeDataWritable(this.FullDataResults);

            SaveFileDialog S = new SaveFileDialog();
            S.Filter = "Comma Separated Value|*.csv";
            S.ShowDialog();
            if (S.FileName != "")
            {
                FileStream F = (FileStream)S.OpenFile();
                string FilePath = S.FileName;
                this.frmParent.GetLogger().WriteCSV(F, DataToWrite);

                this.UnsavedData = false;
            }

            return;
        }
        #endregion

        #region Image Save Handler
        private void ImagePrintButton_Click(object sender, EventArgs e)
        {
            Rectangle Bounds = this.Bounds;
            using (Bitmap b = new Bitmap(Bounds.Width, Bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.CopyFromScreen(new Point(Bounds.Left, Bounds.Top), Point.Empty, Bounds.Size);
                }

                SaveFileDialog SD = new SaveFileDialog();
                SD.Filter = "JPEG|*.jpeg";
                SD.ShowDialog();
                if (SD.FileName != "")
                {
                    b.Save(SD.FileName, ImageFormat.Jpeg);
                }



            }
            MessageBox.Show("Done!");
            return;
        }
        #endregion

        #region Getters
        public double GetAlphaDPM()
        {
            return this.AlphaDPM;
        }

        public double GetBetaDPM()
        {
            return this.BetaDPM;
        }

        public bool WasTestCompleted()
        {
            if (RL != null)
            {
                return RL.WasTestCompleted();
            }

            return false;
        }

        public double GetAlphaGCPM()
        {
            return this.AlphaGCPM;
        }

        public double GetBetaGCPM()
        {
            return this.BetaGCPM;
        }

        public bool WasDABRASModified()
        {
            return this.frmParent.DABRASModified;
        }

        public DABRAS GetDABRAS()
        {
            return this.dabras;
        }

        public bool WasDCModified()
        {
            return this.frmParent.DefaultConfigModified;
        }

        public DefaultConfigurations GetDefaultConfig()
        {
            return this.frmParent.GetDefaultConfig();
        }

        #endregion

        #region KeyPresses

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
                    //closeCtrlQToolStripMenuItem_Click(this, null);
                    return;
                }
            }
        }

        #endregion

        #region Finalization
        private void RoutineSampleCountingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            endFormActivities();
        }
        public void endFormActivities()
        {
            if (RL != null)
            {
                RL.RequestStop();
                BackgroundThread.Join();
            }
            /*
            if (this.UnsavedData && !Calibrating)
            {
                if (MessageBox.Show("Save data?", "Confirm Save.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //SaveButton_Click(this, null);
                }
            }
            */
            RL = null;
        }
        #endregion
    }
}
