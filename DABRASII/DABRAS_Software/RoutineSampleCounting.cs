using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Media;

namespace DABRAS_Software
{
    public partial class RoutineSampleCountingForm : Form
    {
        #region Enums
        public enum BackgroundType { Annual, Daily };

        public enum RoutineSampleCountType { Time, MDA };
        #endregion

        #region Data Members
        private readonly HomeForm LaunchedFrom;
        private DABRAS DABRAS;
        private bool DABRASModified = false;
        private Logger Logger;

        private BackgroundType BGType = BackgroundType.Annual;
        private RoutineSampleCountType RType = RoutineSampleCountType.Time;

        private DefaultConfigurations DC;
        private bool DefaultConfigModified = false;
        
        private List<Radioactive_Source> ListOfSources;

        private const double ConfidenceRange = 1.645;
        private double Current_Alpha_Eff = -1;
        private double Current_Beta_Eff = -1;

        private int BadgeNo;

        private RoutineSampleListener RL;

        private Thread BackgroundThread;

        private double Alpha_SelfAbsorbtion = 0;
        private double Beta_SelfAbsorbtion = 0;
        private double Beta_Backscatter = 0;
        private double Contaminant_Removal_Fraction = 0; //Between 0 and 1
        private double Sample_Area = 0;

        private int SampleTime = 0;
        private double AlphaMDA = 0;
        private double BetaMDA = 0;

        private int index = 1; //incremented every time a sample is taken.

        private const double ConfRange = 1.645; //95 % confidence range

        public delegate void UpdateFormCallback();

        private bool Calibrating = false; //This should ONLY be set to true when using in quickcal mode

        private event EventHandler CallbackComplete; //used for QuickCal

        private double AlphaGCPM;
        private double BetaGCPM;

        private double AlphaDPM;
        private double BetaDPM;

        private bool UnsavedData = false;

        private ModFactors MF;
        #endregion

        #region Constructor
        public RoutineSampleCountingForm(HomeForm Parent, int BadgeNo)
        {
            InitializeComponent();

            this.LaunchedFrom = Parent;
            this.DABRAS = LaunchedFrom.GetDABRAS();
            this.Logger = LaunchedFrom.GetLogger();

            this.DC = LaunchedFrom.GetDefaultConfig();
            this.ListOfSources = LaunchedFrom.GetListOfSources();
            
            this.BadgeNo = BadgeNo;
            this.Sample_ID_TB.Text = Convert.ToString(this.BadgeNo);

            this.BGType = DC.GetRoutineCalibrationBackgroundType();

            /*Initialize drop down boxes*/
            this.Alpha_ComboBox.Items.Clear();
            this.Beta_ComboBox.Items.Clear();

            foreach (Radioactive_Source i in ListOfSources)
            {
                if (i.GetSourceType() == Radioactive_Source.RadiationType.Alpha)
                {
                    Alpha_ComboBox.Items.Add(i.GetName());
                }

                if (i.GetSourceType() == Radioactive_Source.RadiationType.Beta)
                {
                    Beta_ComboBox.Items.Add(i.GetName());
                }
            }

            if (Alpha_ComboBox.Items.Count > 0)
            {
                Alpha_ComboBox.Text = Convert.ToString(Alpha_ComboBox.Items[0]);

                this.Current_Alpha_Eff = ListOfSources.Find(x => x.GetName() == Alpha_ComboBox.Text).GetAlphaEfficiency();
            }

            if (Beta_ComboBox.Items.Count > 0)
            {
                Beta_ComboBox.Text = Convert.ToString(Beta_ComboBox.Items[0]);

                this.Current_Beta_Eff = ListOfSources.Find(x => x.GetName() == Beta_ComboBox.Text).GetBetaEfficiency();
            }

            /*Don't need to error check - default values*/
            try
            {
                this.Preset_Time_Label.Text = String.Format("Preset time: {0}:{1}", Convert.ToInt32(Min_TB.Text), Convert.ToInt32(Sec_TB.Text));

                this.MF = DC.GetModFactors();

                this.Alpha_SelfAbsorbtion = MF.GetAlphaSelfAbsorbtion();
                this.Alpha_Absorption_Mod_TB.Text = Convert.ToString(this.Alpha_SelfAbsorbtion);
                
                this.Beta_SelfAbsorbtion = MF.GetBetaSelfAbsorbtion();
                this.Beta_Absorption_Mod_TB.Text = Convert.ToString(this.Beta_SelfAbsorbtion);

                this.Beta_Backscatter = MF.GetBetaBackscatter();
                this.Beta_Backscatter_Mod_TB.Text = Convert.ToString(this.Beta_Backscatter);

                this.Contaminant_Removal_Fraction = MF.GetRemovalFrac();
                this.Removal_Percentage_Mod_TB.Text = Convert.ToString(this.Contaminant_Removal_Fraction);

                this.Sample_Area = MF.GetRemovalFrac();
                this.Area_Mod_TB.Text = Convert.ToString(Sample_Area);
            }
            catch
            {
                ;
            }


            this.Elapsed_Time_Label.Text = "Elapsed time: 0:00";

            if (DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = "Firmware Version: " + DABRAS.Firmware_Version;
                this.DABRAS_SN_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                this.DABRAS_Status_Label.Text = "STATUS: Connected!";
            }

            SetTimeButton_Click(this, null);

            #region DataGridView Initialization

            FullDataResults.Columns.Add("Headers", "");
            DataGridViewColumn Headers = FullDataResults.Columns[0];
            Headers.Width = 175;

            /*33 rows, indexed 0-32*/
            /*In groups of 10*/
            FullDataResults.Rows.Add("Id Number");
            FullDataResults.Rows.Add("Description");
            FullDataResults.Rows.Add("Start Date/Time");
            FullDataResults.Rows.Add("End Date/Time");
            FullDataResults.Rows.Add("Count Time (min)");
            FullDataResults.Rows.Add("Gross Alpha Count");
            FullDataResults.Rows.Add("Gross Beta Count");
            FullDataResults.Rows.Add("Gross Alpha CPM");
            FullDataResults.Rows.Add("Gross Beta CPM");

            if (this.BGType == BackgroundType.Annual)
            {
                FullDataResults.Rows.Add("Annual Background Reference Date/Time");

                FullDataResults.Rows.Add("Annual Background Sample Time");
                FullDataResults.Rows.Add("Annual Background Alpha");
                FullDataResults.Rows.Add("Annual Background Beta");
            }

            else
            {

                FullDataResults.Rows.Add("Daily Background Reference Date/Time");

                FullDataResults.Rows.Add("Daily Background Sample Time");
                FullDataResults.Rows.Add("Daily Background Alpha");
                FullDataResults.Rows.Add("Daily Background Beta");
            }
            FullDataResults.Rows.Add("Alpha Lc (NCPM)");
            FullDataResults.Rows.Add("Beta Lc (NCPM)");
            FullDataResults.Rows.Add("Alpha MDA(DPM)");
            FullDataResults.Rows.Add("Beta MDA (DPM)");
            FullDataResults.Rows.Add("Net Alpha CPM");
            FullDataResults.Rows.Add("Net Beta CPM");
            FullDataResults.Rows.Add("Alpha Uncertainty (CPM)");

            FullDataResults.Rows.Add("Beta Uncertainty (CPM)");
            FullDataResults.Rows.Add("Alpha 4pi Efficiency (%)");
            FullDataResults.Rows.Add("Beta 4pi Efficiency (%)");
            FullDataResults.Rows.Add(String.Format("Alpha Efficiency for {0}", Alpha_ComboBox.Text));
            FullDataResults.Rows.Add(String.Format("Beta Efficiency for {0}", Beta_ComboBox.Text));
            FullDataResults.Rows.Add("Alpha Mod factor");
            FullDataResults.Rows.Add("Beta Mod factor");
            FullDataResults.Rows.Add("Beta Backscatter Factor");
            FullDataResults.Rows.Add("Contaminant Removal Fraction");
            FullDataResults.Rows.Add("Sample Area (square cm)");

            FullDataResults.Rows.Add("Alpha Limit (DPM)");
            FullDataResults.Rows.Add("Beta Limit (DPM)");
            FullDataResults.Rows.Add("Alpha Uncertainty (DPM)");
            FullDataResults.Rows.Add("Beta Uncertainty (DPM)");
            FullDataResults.Rows.Add("Alpha Activity (DPM)");
            FullDataResults.Rows.Add("Beta Activity (DPM)");

            /*Don't let the user edit the headertext (can't set whole table to readonly because need to be able to edit description cell*/
            for (int i = 0; i < FullDataResults.RowCount; i++)
            {
                FullDataResults[0, i].ReadOnly = true;
            }

            #endregion

            /*Attempt to find folder with CSV data*/
            string CurrentEnv = Environment.CurrentDirectory;
            CurrentEnv += String.Format("\\data\\Routine\\{0}", this.BadgeNo);
            if (Directory.Exists(CurrentEnv))
            {
                string[] Files = Directory.GetFiles(CurrentEnv, "*.csv");
                string Identifier = String.Format("{0}_{1}_{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                string FoundFileName = null;
                foreach (string s in Files)
                {
                    if (s.IndexOf(Identifier) != -1)
                    {
                        FoundFileName = s;
                        break;
                    }
                }

                if (FoundFileName != null)
                {
                    try
                    {
                        using (StreamReader Sr = new StreamReader(FoundFileName))
                        {
                            string NewLine = Sr.ReadLine();
                            string[] NumCols = NewLine.Split(',');

                            FullDataResults.Columns[0].HeaderText = NumCols[0];
                            for (int i = 1; i < NumCols.Length - 1; i++)
                            {
                                FullDataResults.Columns.Add("NewColumn", NumCols[i]);
                                this.index++;
                            }

                            int row = 1;
                            NewLine = Sr.ReadLine();
                            while (NewLine != null)
                            {
                                string[] SplitLine = NewLine.Split(',');

                                for (int col = 1; col < SplitLine.Length; col++)
                                {
                                    FullDataResults[col - 1, row - 1].Value = SplitLine[col - 1];
                                }

                                row++;
                                if (row >= FullDataResults.RowCount)
                                {
                                    FullDataResults.Rows.Add();
                                }
                                NewLine = Sr.ReadLine();
                            }

                            while (FullDataResults.RowCount > 36)
                            {
                                FullDataResults.Rows.RemoveAt(36);
                            }
                        }
                    }
                    catch
                    {
                        ;
                    }
                }
            }

            this.Continue_Count_Button.Enabled = false;
            this.Stop_Count_Button.Enabled = false;
            this.continueCountCtrlOToolStripMenuItem.Enabled = false;
            this.stopAquisitionCtrlSToolStripMenuItem.Enabled = false;

            /*Set text for radio buttons*/
            try
            {
                this._100_Beta_Button.Text = String.Format("100-200KeV (Eff = {0}%)", StaticMethods.RoundToSigFigs(ListOfSources.Find(x => x.GetName() == "C-14").GetBetaEfficiency()));
            }
            catch { ;}

            try
            {
                this._200_Beta_Button.Text = String.Format("200-400KeV (Eff = {0}%)", StaticMethods.RoundToSigFigs(ListOfSources.Find(x => x.GetName() == "Cs-137").GetBetaEfficiency()));
            }
            catch { ;}

            try
            {
                this._400_Beta_Button.Text = String.Format("400-1200KeV (Eff = {0}%)", StaticMethods.RoundToSigFigs(ListOfSources.Find(x => x.GetName() == "Tc-99").GetBetaEfficiency()));
            }
            catch { ;}

            try
            {
                this._1200_Beta_Button.Text = String.Format(">1200KeV (Eff = {0}%)", StaticMethods.RoundToSigFigs(ListOfSources.Find(x => x.GetName() == "Sr-90").GetBetaEfficiency()));
            }
            catch { ;}

            /*Add keypress event handlers*/
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region New Count Handler
        private void New_Count_Button_Click(object sender, EventArgs e)
        {
            /*Check to see if the machine is calibrated*/
            Radioactive_Source Background = ListOfSources.Find(x => x.GetName() == "Background");
            Radioactive_Source AlphaSource = ListOfSources.Find(x => x.GetName() == Alpha_ComboBox.Text);
            Radioactive_Source BetaSource = ListOfSources.Find(x => x.GetName() == Beta_ComboBox.Text);

            bool RequireCalibrationPassword = false;

            /*Check annual calibrations*/

            if (!Calibrating)
            {
                /*Verify that both sources are in calibration*/
                /*Annual calibration will be due at the end of the month*/
                TimeSpan T = DateTime.Now.Subtract(Background.GetAnnualCalibratedTime());
                DateTime CalDue = Background.GetAnnualCalibratedTime().AddYears(1);

                if ( ((TimeSpan.Compare(T, new TimeSpan(365, 0, 0, 0))) > 0) && (CalDue.Month != DateTime.Now.Month) && (CalDue.Year != DateTime.Now.Year))
                {
                    MessageBox.Show(String.Format("Error: Background out of calibration. Last calibrated at {0}", Background.GetAnnualCalibratedTime()));
                    RequireCalibrationPassword = true;
                }

                T = DateTime.Now.Subtract(AlphaSource.GetAnnualCalibratedTime());

                if ((TimeSpan.Compare(T, new TimeSpan(365, 0, 0, 0))) > 0)
                {
                    MessageBox.Show(String.Format("Error: Alpha Source out of calibration. Last calibrated at {0}", AlphaSource.GetAnnualCalibratedTime()));
                    RequireCalibrationPassword = true;
                }

                T = DateTime.Now.Subtract(BetaSource.GetAnnualCalibratedTime());

                if ((TimeSpan.Compare(T, new TimeSpan(365, 0, 0, 0))) > 0)
                {
                    MessageBox.Show(String.Format("Error: Beta Source out of calibration. Last calibrated at {0}", BetaSource.GetAnnualCalibratedTime()));
                    RequireCalibrationPassword = true;
                }
            }
        
            if (!Calibrating)
            {
                /*Verify that both sources are in calibration*/
                TimeSpan T = DateTime.Now.Subtract(Background.GetDailyCalibratedTime());
                if ((TimeSpan.Compare(T, new TimeSpan(1, 0, 0, 0))) > 0)
                {
                    MessageBox.Show(String.Format("Error: Background operational check needed. Last performed at {0}", Background.GetDailyCalibratedTime()));
                    RequireCalibrationPassword = true;
                }

                T = DateTime.Now.Subtract(AlphaSource.GetDailyCalibratedTime());

                if ((TimeSpan.Compare(T, new TimeSpan(1, 0, 0, 0))) > 0)
                {
                    MessageBox.Show(String.Format("Error: Alpha Source operational check needed. Last performed at {0}", AlphaSource.GetAnnualCalibratedTime()));
                    RequireCalibrationPassword = true;
                }

                T = DateTime.Now.Subtract(ListOfSources.Find(x => x.GetName() == "Sr-90").GetDailyCalibratedTime());

                if ((TimeSpan.Compare(T, new TimeSpan(1, 0, 0, 0))) > 0)
                {
                    MessageBox.Show(String.Format("Error: Beta Source operational check needed. Last performed at {0}", BetaSource.GetAnnualCalibratedTime()));
                    RequireCalibrationPassword = true;
                }
            }
        

            /*Check Chi Squared*/
            DateTime DateCalibrated = DC.GetChiSquaredDate();

            if (DC.GetChiSquaredTimespan() != TimeSpan.MaxValue)
            {
                DateTime ChiSqExpires = DateCalibrated.Add(DC.GetChiSquaredTimespan());
                
                if (!Calibrating)
                {
                    if (DateTime.Compare(ChiSqExpires, DateTime.Now) < 0)
                    {
                        MessageBox.Show("Error: Chi squared test required.");
                        RequireCalibrationPassword = true;
                    }
                }
            }
            

            /*Check continuous monitor*/
            if (!Calibrating)
            {
                bool AutoCalOK= (DABRAS.ValidateContinuousAlphaBackground() && DABRAS.ValidateContinuousBetaBackground()) || (DateTime.Compare(DABRAS.GetValidationDate(), Background.GetDailyCalibratedTime()) < 0);
                
                if (!AutoCalOK)
                {
                    MessageBox.Show("Error: The continuous background monitor detects high levels of background radiation. Either disable the continuous background monitor or recalibrate the background.");
                    RequireCalibrationPassword = true;
                }
            }
            
            /*Allow override with calibration password*/
            if (RequireCalibrationPassword)
            {
                if (MessageBox.Show("Override with Calibration Password?", "Attempt to Override?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ChallengePassword CP= new ChallengePassword();
                    CP.ShowDialog();
                    if (CP.GetUserEnteredPassword() == DC.GetPassword())
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

            if (!DABRAS.IsConnected())
            {
                MessageBox.Show("Error: No connection to the DABRAS. Please re-connect, and try again.");
                return;
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

            FullDataResults["NewColumn", 0].Value = this.BadgeNo;
            FullDataResults["NewColumn", 1].Value = Description_TB.Text;
            FullDataResults["NewColumn", 2].Value = DateTime.Now;

            if (this.RType == RoutineSampleCountType.Time)
            {
                FullDataResults["NewColumn", 4].Value = String.Format("{0}:{1:00}", ((SampleTime + 1) / 60), ((SampleTime + 1) % 60));
            }

            if (this.BGType == BackgroundType.Annual)
            {
                FullDataResults["NewColumn", 9].Value = ListOfSources.Find(x => x.GetName() == "Background").GetAnnualCalibratedTime();
                FullDataResults["NewColumn", 10].Value = ListOfSources.Find(x => x.GetName() == "Background").GetAnnualCalibratedTimespan();
                FullDataResults["NewColumn", 11].Value = ListOfSources.Find(x => x.GetName() == "Background").GetAnnualAlphaCPM();
                FullDataResults["NewColumn", 12].Value = ListOfSources.Find(x => x.GetName() == "Background").GetAnnualBetaCPM();
            }
            else if (this.BGType == BackgroundType.Daily)
            {
                FullDataResults["NewColumn", 9].Value = ListOfSources.Find(x => x.GetName() == "Background").GetDailyCalibratedTime();
                FullDataResults["NewColumn", 10].Value = ListOfSources.Find(x => x.GetName() == "Background").GetDailyCalibratedTimespan();
                FullDataResults["NewColumn", 11].Value = ListOfSources.Find(x => x.GetName() == "Background").GetDailyAlphaCPM();
                FullDataResults["NewColumn", 12].Value = ListOfSources.Find(x => x.GetName() == "Background").GetDailyBetaCPM();
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

            /*Don't allow the user to edit anything except for the description (It doesn't work the other way!)*/
            for (int i = 0; i < FullDataResults.RowCount; i++)
            {
                if (i != 1)
                {
                    FullDataResults[1, i].ReadOnly = true;
                }
            }

            if (this.BGType == BackgroundType.Annual)
            {
                RL = new RoutineSampleListener(this.DABRAS, SampleTime, 0, this.FullDataResults, Convert.ToInt32(Background.GetAnnualAlphaCPM()), Convert.ToInt32(Background.GetAnnualBetaCPM()), this.Current_Alpha_Eff, this.Current_Beta_Eff, this.Alpha_SelfAbsorbtion, this.Beta_SelfAbsorbtion, this.Beta_Backscatter, this.Contaminant_Removal_Fraction, this.Sample_Area, Background.GetAnnualCalibratedTimespan(), RType, this.AlphaMDA, this.BetaMDA);
            }

            if (this.BGType == BackgroundType.Daily)
            {
                RL = new RoutineSampleListener(this.DABRAS, SampleTime, 0, this.FullDataResults, Convert.ToInt32(Background.GetDailyAlphaCPM()), Convert.ToInt32(Background.GetDailyBetaCPM()), this.Current_Alpha_Eff, this.Current_Beta_Eff, this.Alpha_SelfAbsorbtion, this.Beta_SelfAbsorbtion, this.Beta_Backscatter, this.Contaminant_Removal_Fraction, this.Sample_Area, Background.GetDailyCalibratedTimespan(), RType, this.AlphaMDA, this.BetaMDA);
            }

            BackgroundThread = new Thread(() => RL.Run_Sample());
            BackgroundThread.Start();
            
            RL.BackgroundSampleThreadFinished += (s, args) => {InvokeCallback(); };
            RL.PacketReceived += (s, args) => { InvokeCallback(); };

            SetGUI(true);

            this.Alpha_Activity_TB.ForeColor = Color.Black;
            this.Beta_Activity_TB.ForeColor = Color.Black;

            this.index++;
             
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

            SetGUI(true);

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

            FullDataResults["NewColumn", 3].Value = "CANCELLED";

            SetGUI(false);

        }
        #endregion

        #region Pause Handler
        private void PauseButton_Click(object sender, EventArgs e)
        {
            if (RL != null)
            {
                RL.RequestPause();
            }

            SetGUI(false);
            
        }
        #endregion

        #region Callback Functions
        private void InvokeCallback()
        {
            if (this != null)
            {
                this.Invoke(new UpdateFormCallback(this.Update_Form));
            }
        }

        private void Update_Form()
        {
            if (RL.IsDone())
            {
                Status_Label.Text = "Stopped";

                if (RL.WasTestCompleted())
                {
                    FullDataResults["NewColumn", 3].Value = DateTime.Now;

                    if (RL.GetTestType() == RoutineSampleCountType.MDA)
                    {
                        FullDataResults["NewColumn", 4].Value = String.Format("{0}:{1:00}", (RL.GetElapsedTime() + 1) / 60, (RL.GetElapsedTime() + 1) % 60);
                    }
                }
                else
                {
                    FullDataResults["NewColumn", 3].Value = "CANCELLED";

                    if (RL.GetTestType() == RoutineSampleCountType.MDA)
                    {
                        FullDataResults["NewColumn", 4].Value = String.Format("{0}:{1:00}", (RL.GetElapsedTime() + 1) / 60, (RL.GetElapsedTime() + 1) % 60);
                    }
                }

                /*Set GUI elements*/
                SetGUI(false);

                /*Compute final limits, turning red if bad*/
                try
                {
                    double MaxAlphaActivity = Convert.ToDouble(GrossAlphaDPMLimit_TB.Text);
                    {
                        if (RL.GetAlphaDPM() > MaxAlphaActivity)
                        {
                            Alpha_Activity_TB.ForeColor = Color.Red;
                        }

                        else
                        {
                            Alpha_Activity_TB.ForeColor = Color.Green;
                        }
                    }

                    double MaxBetaActivity = Convert.ToDouble(GrossBetaDPMLimit_TB.Text);
                    {
                        if (RL.GetBetaDPM() > MaxBetaActivity)
                        {
                            Beta_Activity_TB.ForeColor = Color.Red;
                        }

                        else
                        {
                            Beta_Activity_TB.ForeColor = Color.Green;
                        }
                    }

                    double AlphaMDA = Convert.ToDouble(Alpha_MDA_TB.Text);
                    double BetaMDA = Convert.ToDouble(Beta_MDA_TB.Text);

                    if (((AlphaMDA > MaxAlphaActivity) || (BetaMDA > MaxBetaActivity)) && (RL.GetTestType() == RoutineSampleCountType.Time))
                    {
                        MessageBox.Show(String.Format("Warning: MDA not met. Recommend extending count time until Alpha MDA < {0} and Beta MDA < {1}", MaxAlphaActivity, MaxBetaActivity));
                    }
                }

                finally
                {
                    this.UnsavedData = true;
                }

                /*File writing time!*/

                /*Verify input string*/
                string DescriptionString = Description_TB.Text;
                bool StringVerified = false;
                
                while (!StringVerified)
                {
                    try
                    {
                        DescriptionString = ReplaceSpaces(DescriptionString);
                        StringVerified = VerifyDescription(DescriptionString);
                    }
                    catch
                    {
                        StringVerified = false;
                    }

                    if (!StringVerified)
                    {
                        Prompt NewForm = new Prompt("The Description entered is not valid. Do not use {\\,//,.,?,%,*,:,|,\", <, >} in description name.");
                        NewForm.ShowDialog();
                        DescriptionString = NewForm.GetResponse();
                    }
                }

                if (DescriptionString == "")
                {
                    DescriptionString = "NoDescription";
                }

                /*Verify badge number*/
                int BadgeNo = 0;
                bool BadgeVerified = false;
                while (!BadgeVerified)
                {
                    try
                    {
                        BadgeNo = Convert.ToInt32(this.Sample_ID_TB.Text);
                        BadgeVerified = true;
                    }
                    catch
                    {
                        Prompt NewForm = new Prompt("The badge number entered is not a valid badge number. Please enter your badge number.");
                        NewForm.ShowDialog();
                        this.Sample_ID_TB.Text = NewForm.GetResponse();
                    }
                }

                /*Write to file*/
                string CustomFilePath = String.Format("{0}\\data\\Routine\\{1}\\{2}_{3}_{4}_{5}_{6}_RSC.csv", Environment.CurrentDirectory, BadgeNo, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DescriptionString, DABRAS.Serial_Number);
                string MasterPath = String.Format("{0}\\data\\Routine\\Master\\Master.csv", Environment.CurrentDirectory);
                string CustomDir = String.Format("{0}\\data\\Routine\\{1}", Environment.CurrentDirectory, BadgeNo);
                string[,] DataToWrite = MakeDataWritable(FullDataResults);
                try
                {
                    if (!Directory.Exists(CustomDir))
                    {
                        Directory.CreateDirectory(CustomDir);
                    }

                    if (!File.Exists(CustomFilePath))
                    {
                        File.Create(CustomFilePath).Dispose();
                    }

                    if (!File.Exists(MasterPath))
                    {
                        File.Create(MasterPath).Dispose();
                    }

                    using (FileStream F = new FileStream(CustomFilePath, FileMode.Create))
                    {
                        Logger.WriteCSV(F, DataToWrite);
                        //Toast T = new Toast(String.Format("File written to {0}", CustomFilePath));
                        Toast T = new Toast("File Written.");
                        T.Show();
                    }

                    using (FileStream F = new FileStream(MasterPath, FileMode.Append))
                    {
                        Logger.WriteCSV(F, DataToWrite);
                    }
                }
                catch
                {
                    MessageBox.Show("Automatic write failed.");
                }

                if (!DABRAS.IsConnected())
                {
                    this.DABRAS_Firmware_Label.Text = "Firmware Version: XX";
                    this.DABRAS_SN_Label.Text = "Serial Number: XX";
                    this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
                }
            }

            else
            {
                this.Elapsed_Time_Label.Text = String.Format("Elapsed Time: {0}:{1:00}", (RL.GetElapsedTime() + 1) / 60, ((RL.GetElapsedTime() + 1) % 60));
            }

            this.Alpha_MDA_TB.Text = Convert.ToString(FullDataResults[1, 15].Value);
            this.Beta_MDA_TB.Text = Convert.ToString(FullDataResults[1, 16].Value);

            this.Alpha_Count_TB.Text = String.Format("{0:G3}±{1:G3}", FullDataResults[1, 7].Value, FullDataResults[1, 19].Value);
            this.Beta_Count_TB.Text = String.Format("{0:G3}±{1:G3}", FullDataResults[1, 8].Value, FullDataResults[1, 20].Value);

            this.LC_Alpha_TB.Text = String.Format("{0:G3}", FullDataResults[1, 13].Value);
            this.LC_Beta_TB.Text = String.Format("{0:G3}", FullDataResults[1, 14].Value);

            this.Alpha_GCPM_TB.Text = String.Format("{0:G3}", FullDataResults[1, 7].Value);
            this.Beta_GCPM_TB.Text = String.Format("{0:G3}", FullDataResults[1, 8].Value);

            this.Alpha_Activity_TB.Text = String.Format("{0:G3}±{1:G3}", FullDataResults[1, 34].Value, FullDataResults[1, 32].Value);
            this.Beta_Activity_TB.Text = String.Format("{0:G3}±{1:G3}", FullDataResults[1, 35].Value, FullDataResults[1, 33].Value);

            /*For calibration*/
            this.AlphaGCPM = Convert.ToDouble(FullDataResults[1, 7].Value);
            this.BetaGCPM = Convert.ToDouble(FullDataResults[1, 8].Value);

            this.AlphaDPM = RL.GetAlphaDPM();
            this.BetaDPM = RL.GetBetaDPM();

            return;
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

        #region Save CSV Button Handler
        private void SaveButton_Click(object sender, EventArgs e)
        {
            string[,] DataToWrite = MakeDataWritable(this.FullDataResults);

            SaveFileDialog S = new SaveFileDialog();
            S.Filter = "Comma Separated Value|*.csv";
            S.ShowDialog();
            if (S.FileName != "")
            {
                FileStream F = (FileStream)S.OpenFile();
                string FilePath = S.FileName;
                Logger.WriteCSV(F, DataToWrite);

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

        #region Connect Handler
        private void connectDisconnectPortCtrlPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*Show the dialogue*/
            VCPConnect NewPopup = new VCPConnect();
            if (NewPopup.ShowDialog() == DialogResult.OK)
            {
                /*The user clicked OK! Attempt to connect to the DABRAS*/
                bool Successful = DABRAS.Get_Coms(NewPopup.VCP_Port);
                if (!(Successful))
                {
                    MessageBox.Show("Error: Communication could not be established.");
                    Logger.WriteLineToStatusLog(String.Format("Communication attempt failed at time {0}", DateTime.Now));

                    this.DABRAS_Firmware_Label.Text = "Firmware Version: ??";
                    this.DABRAS_SN_Label.Text = "Serial Number: ??";
                    this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
                    DABRAS.VCP_Instance = "";
                    this.DABRASModified = true;

                    return;
                }
                else
                {
                    MessageBox.Show("Communication Established.");
                    Logger.WriteLineToStatusLog(String.Format("Communication successfully established at time {0}", DateTime.Now));
                }

                DABRAS.Initialize();

                this.DABRAS_Firmware_Label.Text = "Firmware Version: " + DABRAS.Firmware_Version;
                this.DABRAS_SN_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                this.DABRAS_Status_Label.Text = "STATUS: Connected!";

                /*Write to constants*/
                this.DABRASModified = true;
            }

            if (NewPopup.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }

        #endregion

        #region WebForm Handler
        private void openWebBasedSurveySystemF12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DC.GetWebSurvey());
            NewForm.Show();
        }

        private void openRSOSharepointF11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DC.GetRSOLink());
            NewForm.Show();
        }

        private void openRSOHomeF10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DC.GetRSOHome());
            NewForm.Show();
        }
        #endregion

        #region Private Utility Functions
        private void AlphaMDALimit_TB_TextChanged(object sender, EventArgs e)
        {
            this.CountMDARadioButton.Checked = true;
        }

        private void BetaMDALimit_TB_TextChanged(object sender, EventArgs e)
        {
            this.CountMDARadioButton.Checked = true;
        }

        private void Min_TB_TextChanged(object sender, EventArgs e)
        {
            this.TimeRadioButton.Checked = true;
        }

        private void Sec_TB_TextChanged(object sender, EventArgs e)
        {
            this.TimeRadioButton.Checked = true;
        }

        private bool VerifyDescription(string InString)
        {
            if (InString.Length > 200)
            {
                return false;
            }

            foreach (char c in InString)
            {
                if (c < 32)
                {
                    return false;
                }

                switch (c)
                {
                    case '/':
                    case '\\':
                    case '?':
                    case '%':
                    case '*':
                    case ':':
                    case '|':
                    case '"':
                    case '<':
                    case '>':
                    case '.':
                        return false;
                    default:
                        break;
                }
            }

            return true;
        }

        private string ReplaceSpaces(string InString)
        {
            string ReturnString = "";
            for(int i = 0; i <InString.Length; i++)
            {
                if (InString[i] == ' ')
                {
                    ReturnString += '_';
                }
                else
                {
                    ReturnString += InString[i];
                }
            }

            return ReturnString;
        }

        private string[,] MakeDataWritable(DataGridView DG)
        {

            int Rows = DG.RowCount + 1;
            int Cols = DG.ColumnCount;

            string[,] ReturnString = new string[Rows, Cols];

            foreach (DataGridViewColumn i in DG.Columns)
            {
                ReturnString[0, i.Index] = i.HeaderText;
            }


            for (int i = 1; i < DG.RowCount + 1; i++)
            {
                for (int j = 0; j < DG.ColumnCount; j++)
                {
                    ReturnString[i, j] = "";
                    ReturnString[i, j] = Convert.ToString(DG[j, i - 1].Value);
                }
            }

            return ReturnString;
        }

        private void AbortAll()
        {
            if (this.RL != null)
            {
                RL.RequestStop();
                while (RL.IsRunning())
                {
                    Thread.Sleep(1);
                }
            }
            
            this.DialogResult = DialogResult.Abort;
            this.Close();
            return;
        }

        private void SetTypeOfSample()
        {
            if (this.TimeRadioButton.Checked)
            {
                this.RType = RoutineSampleCountType.Time;
            }

            else if (this.CountMDARadioButton.Checked)
            {
                this.RType = RoutineSampleCountType.MDA;
            }

            return;
        }

        private void SetGUI(bool Running)
        {
            this.SaveButton.Enabled = !Running;
            this.ImagePrintButton.Enabled = !Running;
            this.saveCtrlVToolStripMenuItem.Enabled = !Running;
            this.saveImageCtrlIToolStripMenuItem.Enabled = !Running;

            this.New_Count_Button.Enabled = !Running;
            this.newCountCtrlAToolStripMenuItem.Enabled = !Running;
            this.Continue_Count_Button.Enabled = !Running;
            this.continueCountCtrlOToolStripMenuItem.Enabled = !Running;
            this.setSampleTimeCtrlTToolStripMenuItem.Enabled = !Running;
            this.SetTimeButton.Enabled = !Running;

            this.connectDisconnectPortCtrlPToolStripMenuItem.Enabled = !Running;
            this.setBackgroundTypeToolStripMenuItem.Enabled = !Running;

            this.PauseButton.Enabled = Running;
            this.pauseCountCtrlKToolStripMenuItem.Enabled = Running;
            this.Stop_Count_Button.Enabled = Running;
            this.stopAquisitionCtrlSToolStripMenuItem.Enabled = Running;

            this.Min_TB.Enabled = !Running;
            this.Sec_TB.Enabled = !Running;
            this.AlphaMDALimit_TB.Enabled = !Running;
            this.BetaMDALimit_TB.Enabled = !Running;
            this.TimeRadioButton.Enabled = !Running;
            this.CountMDARadioButton.Enabled = !Running;
            
        }
        #endregion

        #region Set Background Type Handler
        private void setBackgroundTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*First, authenticate*/
            ChallengePassword CP = new ChallengePassword();
            if (CP.ShowDialog() != DialogResult.OK)
            {
                /*User did not submit password*/
                if (CP.DialogResult == DialogResult.Abort)
                {
                    AbortAll();
                }

                CP.Dispose();
                return;
            }

            /*Check password. Check this awesome security!*/
            if (String.Compare(CP.GetUserEnteredPassword(), DC.GetPassword()) != 0)
            {
                MessageBox.Show("Incorrect Password.");
                CP.Dispose();
                return;
            }

            BackgroundTypeForm NewForm = new BackgroundTypeForm(this.BGType);

            NewForm.ShowDialog();

            if (NewForm.DialogResult == DialogResult.OK)
            {
                this.BGType = NewForm.GetBackgroundType();

                this.DC.SetRoutineCalibrationBackgroundType(this.BGType);
                this.DefaultConfigModified = true;
            }

            if (NewForm.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }
        #endregion

        #region Combobox_Handlers
        private void Alpha_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Radioactive_Source R = ListOfSources.Find(x => x.GetName() == Alpha_ComboBox.Text);

            /*Set Alpha Efficiency*/
            this.Current_Alpha_Eff = R.GetAlphaEfficiency();
        }

        private void Beta_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Radioactive_Source R = ListOfSources.Find(x => x.GetName() == Beta_ComboBox.Text);

            /*Set Beta Efficiency*/
            this.Current_Beta_Eff = R.GetBetaEfficiency();

            /*Set the correct radio button based on the source selected*/
            if ((R != null) && (R.GetSourceType() == Radioactive_Source.RadiationType.Beta))
            {
                switch (R.GetEnergyBand())
                {
                    case Radioactive_Source.EnergyBand.Beta_100_200KeV:
                        _100_Beta_Button.Checked = true;
                        break;
                    case Radioactive_Source.EnergyBand.Beta_200_400KeV:
                        _200_Beta_Button.Checked = true;
                        break;
                    case Radioactive_Source.EnergyBand.Beta_400_1200KeV:
                        _400_Beta_Button.Checked = true;
                        break;
                    case Radioactive_Source.EnergyBand.Beta_More_1200KeV:
                        _1200_Beta_Button.Checked = true;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region About Handler
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm NewForm = new AboutForm();
            NewForm.ShowDialog();

            if (NewForm.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }
        #endregion

        #region Checkbox Handlers
        private void Show_Full_Data_Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            this.FullDataResults.Visible = Show_Full_Data_Checkbox.Checked;
            return;
        }

        private void _100_Beta_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (_100_Beta_Button.Checked)
            {
                /*Attempt to find a source with 100-200 KeV of energy*/
                /*If found, set combobox to this one, and use its efficiency*/

                Radioactive_Source R = ListOfSources.Find(x => x.GetEnergyBand() == Radioactive_Source.EnergyBand.Beta_100_200KeV);

                if (R != null)
                {
                    Beta_ComboBox.SelectedIndex = Beta_ComboBox.Items.IndexOf(R.GetName());

                    this.Current_Beta_Eff = R.GetBetaEfficiency();
                }
            }
        }

        private void _200_Beta_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (_200_Beta_Button.Checked)
            {
                /*Attempt to find a source with 200-400 KeV of energy*/
                /*If found, set combobox to this one, and use its efficiency*/

                Radioactive_Source R = ListOfSources.Find(x => x.GetEnergyBand() == Radioactive_Source.EnergyBand.Beta_200_400KeV);

                if (R != null)
                {
                    Beta_ComboBox.SelectedIndex = Beta_ComboBox.Items.IndexOf(R.GetName());

                    this.Current_Beta_Eff = R.GetBetaEfficiency();
                }
            }
        }

        private void _400_Beta_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (_400_Beta_Button.Checked)
            {
                /*Attempt to find a source with 400-1200 KeV of energy*/
                /*If found, set combobox to this one, and use its efficiency*/

                Radioactive_Source R = ListOfSources.Find(x => x.GetEnergyBand() == Radioactive_Source.EnergyBand.Beta_400_1200KeV);

                if (R != null)
                {
                    Beta_ComboBox.SelectedIndex = Beta_ComboBox.Items.IndexOf(R.GetName());

                    this.Current_Beta_Eff = R.GetBetaEfficiency();
                }
            }
        }

        private void _1200_Beta_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (_1200_Beta_Button.Checked)
            {
                /*Attempt to find a source with 400-1200 KeV of energy*/
                /*If found, set combobox to this one, and use its efficiency*/

                Radioactive_Source R = ListOfSources.Find(x => x.GetEnergyBand() == Radioactive_Source.EnergyBand.Beta_More_1200KeV);

                if (R != null)
                {
                    Beta_ComboBox.SelectedIndex = Beta_ComboBox.Items.IndexOf(R.GetName());

                    this.Current_Beta_Eff = R.GetBetaEfficiency();
                }
            }
        }

        #endregion

        #region Long Description Button Handler
        private void LongDescriptionButton_Click(object sender, EventArgs e)
        {
            LongPrompt NewForm = new LongPrompt("Enter a Sample Description.");
            NewForm.ShowDialog();

            string Response = NewForm.GetResponse();
            if (Response != null && Response != "")
            {
                Response = Response.Replace('\n', ' ');    
            }
            if (Response != null && Response !="")
            {
                this.Description_TB.Text = Response;
            }
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
        
        #region Other Toolbar Functions
        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }

        private void saveCtrlVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveButton.Enabled)
            {
                SaveButton_Click(this, null);
            }
        }
        #endregion
      
        #region Quick Calibration Functions
        public void Start1minCheck(object SenderForm, List<Radioactive_Source> ListOfCalibratedSources, string NameOfAlpha, string NameOfBeta)
        {
            /*Don't run if this isn't called from the correct form*/
            if (!(SenderForm is QuickCalibrationController))
            {
                MessageBox.Show("Bad form type. Stopping...");
                closeCtrlQToolStripMenuItem_Click(this, null);
                return;
            }

            /*Set sources*/
            this.Alpha_ComboBox.Text = NameOfAlpha;
            Alpha_ComboBox_SelectedIndexChanged(this, null);

            this.Beta_ComboBox.Text = NameOfBeta;
            Beta_ComboBox_SelectedIndexChanged(this, null);

            this.BGType = BackgroundType.Annual;

            this.ListOfSources = ListOfCalibratedSources;

            this.Min_TB.Text = "1";
            this.Sec_TB.Text = "0";
            this.Calibrating = true;

            SetTimeButton_Click(this, null);
            New_Count_Button_Click(this, null);

            return;
        }

        public void StartUniformityTest(object SenderForm, List<Radioactive_Source> ListOfCalibratedSources)
        {
            /*Don't run if this isn't called from the correct form*/
            if (!(SenderForm is QuickCalibrationController))
            {
                MessageBox.Show("Bad form type. Stopping...");
                closeCtrlQToolStripMenuItem_Click(this, null);
                return;
            }

            this.BGType = BackgroundType.Annual;

            this.ListOfSources = ListOfCalibratedSources;

            this.Min_TB.Text = "1";
            this.Sec_TB.Text = "0";
            this.Calibrating = true;

            SetTimeButton_Click(this, null);
            New_Count_Button_Click(this, null);

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
            return this.DABRASModified;
        }

        public DABRAS GetDABRAS()
        {
            return this.DABRAS;
        }

        public bool WasDCModified()
        {
            return this.DefaultConfigModified;
        }

        public DefaultConfigurations GetDefaultConfig()
        {
            return this.DC;
        }

        #endregion

        #region Dummy Overloads
        private void newCountCtrlAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (New_Count_Button.Enabled)
            {
                New_Count_Button_Click(this, null);
            }
        }

        private void continueCountCtrlOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Continue_Count_Button.Enabled)
            {
                Continue_Count_Button_Click(this, null);
            }
        }

        private void stopAquisitionCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Stop_Count_Button.Enabled)
            {
                Stop_Count_Button_Click(this, null);
            }
        }

        private void setSampleTimeCtrlTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SetTimeButton.Enabled)
            {
                SetTimeButton_Click(this, null);
            }
        }

        private void pauseCountCtrlKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PauseButton.Enabled)
            {
                PauseButton_Click(this, null);
            }
        }

        private void saveImageCtrlIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImagePrintButton_Click(this, null);
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
                    closeCtrlQToolStripMenuItem_Click(this, null);
                    return;
                }

                if (Key.KeyCode == Keys.V)
                {
                    if (saveCtrlVToolStripMenuItem.Enabled)
                    {
                        saveCtrlVToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.A)
                {
                    if (newCountCtrlAToolStripMenuItem.Enabled)
                    {
                        newCountCtrlAToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.K)
                {
                    if (pauseCountCtrlKToolStripMenuItem.Enabled)
                    {
                        pauseCountCtrlKToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.O)
                {
                    if (continueCountCtrlOToolStripMenuItem.Enabled)
                    {
                        continueCountCtrlOToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.S)
                {
                    if (stopAquisitionCtrlSToolStripMenuItem.Enabled)
                    {
                        stopAquisitionCtrlSToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.T)
                {
                    if (setSampleTimeCtrlTToolStripMenuItem.Enabled)
                    {
                        setSampleTimeCtrlTToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.P)
                {
                    if (connectDisconnectPortCtrlPToolStripMenuItem.Enabled)
                    {
                        connectDisconnectPortCtrlPToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.V)
                {
                    if (SaveButton.Enabled)
                    {
                        SaveButton_Click(this, null);
                    }
                }

                if (Key.KeyCode == Keys.I)
                {
                    if (ImagePrintButton.Enabled)
                    {
                        ImagePrintButton_Click(this, null);
                    }
                }
            }

            if (Key.KeyCode == Keys.F12)
            {
                openWebBasedSurveySystemF12ToolStripMenuItem_Click(this, null);
            }

            if (Key.KeyCode == Keys.F11)
            {
                openRSOSharepointF11ToolStripMenuItem_Click(this, null);
            }

            if (Key.KeyCode == Keys.F10)
            {
                openRSOHomeF10ToolStripMenuItem_Click(this, null);
            }
        }

        #endregion

        #region Finalization
        private void RoutineSampleCountingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RL != null)
            {
                RL.RequestStop();
                BackgroundThread.Join();
            }

            if (this.UnsavedData && !Calibrating)
            {
                if (MessageBox.Show("Save data?", "Confirm Save.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SaveButton_Click(this, null);
                }
            }

            this.LaunchedFrom.Show();
            //RL = null;
        }
        #endregion

        #region Show/Hide Handler
        private void RoutineSampleCountingForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.DABRAS != null)
            {
                if (this.DABRAS.IsConnected())
                {
                    this.DABRAS_Firmware_Label.Text = "Firmware Version: " + DABRAS.Firmware_Version;
                    this.DABRAS_SN_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                    this.DABRAS_Status_Label.Text = "STATUS: Connected!";
                }
            }
        }
        #endregion

    } 

    
    public class RoutineSampleListener
    {

        #region Data Members
        private bool Done = false;
        private DataGridView FullResults_Table;
        private int SampleTime;
        private double AlphaMDASampleValue;
        private double BetaMDASampleValue;
        private DABRAS DABRAS;
        private bool ShouldStop;
        private bool ShouldPause = false;
        private bool WasBackgroundFinishedSuccessfully;
        private DateTime BackgroundFinished;
        
        private int AlphaBackground;
        private int BetaBackground;
        private int BackgroundCountTime;
        private int ElapsedTime = 0;

        private double AlphaEfficiency;
        private double BetaEfficiency;
        private double AlphaDPM;
        private double AlphaDPM_Uncertainty;
        private double BetaDPM;
        private double BetaDPM_Uncertainty;

        private double AlphaSelfAbsorbtion = 0;
        private double BetaSelfAbsorbtion = 0;
        private double BetaBackscatter = 0;
        private double RemovalPercentage = 1; //from 0 to 1
        private double AreaOfSample = 100; //square cm

        private RoutineSampleCountingForm.RoutineSampleCountType Type;

        public event EventHandler BackgroundSampleThreadFinished;
        public event EventHandler PacketReceived;

        private const double ConfRange = 1.645; //95 % confidence range

        private bool Running = false;
        #endregion

        #region Constructor
        public RoutineSampleListener(DABRAS _DABRAS, int _SampleTime, double _MDAStopValue, DataGridView _FullTable, int _AlphaBG, int _BetaBG, double _AlphaE, double _BetaE, double _AlphaAbsorbtion, double _BetaAbsorbtion, double _BetaBackscatter, double _RemovalPercentage, double _Area, int _BGCountTime, RoutineSampleCountingForm.RoutineSampleCountType _Type, double _AlphaMDA, double _BetaMDA)
        {
            this.DABRAS = _DABRAS;
            this.FullResults_Table = _FullTable;
            this.SampleTime = _SampleTime;
            WasBackgroundFinishedSuccessfully = false;

            this.AlphaBackground = _AlphaBG;
            this.BetaBackground = _BetaBG;
            this.BackgroundCountTime = _BGCountTime;

            this.AlphaEfficiency = _AlphaE;
            this.BetaEfficiency = _BetaE;
            
            this.AlphaSelfAbsorbtion = _AlphaAbsorbtion;
            this.BetaSelfAbsorbtion = _BetaAbsorbtion;

            this.BetaBackscatter = _BetaBackscatter;

            this.RemovalPercentage = _RemovalPercentage;
            this.AreaOfSample = _Area;

            this.Type = _Type;

            if (this.Type == RoutineSampleCountingForm.RoutineSampleCountType.MDA)
            {
                this.AlphaMDASampleValue = _AlphaMDA;
                this.BetaMDASampleValue = _BetaMDA;
                this.SampleTime = 9999;
            }

            ShouldStop = false;
        }

        #endregion

        #region Getters
        public bool IsDone()
        {
            return this.Done;
        }

        public RoutineSampleCountingForm.RoutineSampleCountType GetTestType()
        {
            return this.Type;
        }

        public double GetAlphaDPM()
        {
            return this.AlphaDPM;
        }

        public double GetBetaDPM()
        {
            return this.BetaDPM;
        }

        public bool IsRunning()
        {
            return this.Running;
        }
        
        public int GetElapsedTime()
        {
            return this.ElapsedTime;
        }
        
        public int GetAlphaBackground()
        {
            return this.AlphaBackground;
        }

        public int GetBetaBackground()
        {
            return this.BetaBackground;
        }

        public DateTime GetDateTimeCompleted()
        {
            return this.BackgroundFinished;
        }

        public bool WasTestCompleted()
        {
            return this.WasBackgroundFinishedSuccessfully;
        }

        public bool IsPaused()
        {
            return this.ShouldPause;
        }

        #endregion

        #region Utility
        public void Continue()
        {
            ShouldPause = false;
        }

        public void RequestPause()
        {
            ShouldPause = true;
        }

        public void RequestStart()
        {
            ShouldPause = false;
        }

        public void RequestStop()
        {
            ShouldStop = true;
        }

        #endregion

        #region Background Thread
        public void Run_Sample()
        {
            Running = true;
            /*TODO: Figure out what to do with these...*/
            double Alpha_SelfAbsorption = Convert.ToDouble(FullResults_Table["NewColumn", 25].Value);
            double Beta_SelfAbsorption = Convert.ToDouble(FullResults_Table["NewColumn", 26].Value);
            double Beta_BackScatter = Convert.ToDouble(FullResults_Table["NewColumn", 27].Value);
            double Contaminant_Removal_Fraction = Convert.ToDouble(FullResults_Table["NewColumn", 28].Value);
            double Sample_Area = Convert.ToDouble(FullResults_Table["NewColumn", 29].Value);
            
            /*Stop background threads*/
            DABRAS.Cut();

            /*Set aquisition time*/
            DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
            Thread.Sleep(500);

            /*Start count*/
            DABRAS.Write_To_Serial_Port("g");

            /*Clear any data left in the buffer*/
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();
            DABRAS.EnableWatchdog();

            bool RowComplete = false;
            
            /*Check for the first good packet*/
            while (!ShouldStop)
            {
                try
                {
                    while (!DABRAS.IsDataReady() && !ShouldStop)
                    {
                        Thread.Sleep(100);
                        if (!DABRAS.IsConnected())
                        {
                            throw new TimeoutException();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Error: Connection lost.");
                    DABRAS.DisableWatchdog();
                    this.Done = true;
                    BackgroundSampleThreadFinished(this, null);
                    return;
                }

                if (!ShouldStop)
                {
                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                    DABRAS.KickWatchdog();

                    if (IncomingData != null && IncomingData.ElTime == 0 && IncomingData.TargetTime == SampleTime)
                    {
                        break;
                    }

                    if (IncomingData != null && IncomingData.ElTime > 5)
                    {
                        DABRAS.Write_To_Serial_Port("t");
                        Thread.Sleep(250);
                        DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
                        DABRAS.Write_To_Serial_Port("g");
                    }
                }


            }

            /*Do not increment the row index until the current sample time has elapsed*/
            while (!RowComplete && !ShouldStop)
            {
                if (ShouldPause)
                {
                    DABRAS.DisableWatchdog();
                    DABRAS.Write_To_Serial_Port("r"); //send pause command
                    Thread.Sleep(250);

                    while (ShouldPause && !ShouldStop)
                    {
                        Thread.Sleep(100);
                    }
                    
                    if (!ShouldStop)
                    {
                        DABRAS.ClearSerialPacket();
                        DABRAS.ClearPacketFlag();
                        DABRAS.Write_To_Serial_Port("c");
                        Thread.Sleep(100);
                        DABRAS.EnableWatchdog();
                    }
                }
                
                /*Wait for incoming data packet*/
                try
                {
                    while (!DABRAS.IsDataReady() && !ShouldStop)
                    {
                        Thread.Sleep(100);
                        if (!DABRAS.IsConnected())
                        {
                            throw new TimeoutException();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Error: Connection lost.");
                    DABRAS.DisableWatchdog();
                    this.Done = true;
                    BackgroundSampleThreadFinished(this, null);
                    return;
                }

                SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                /*Grab handles to form*/
                if (!ShouldStop)
                {
                    DataGridViewCell GrossAlphaCell = FullResults_Table[1, 5];
                    DataGridViewCell GrossBetaCell = FullResults_Table[1, 6];
                    DataGridViewCell GrossAlphaCPMCell = FullResults_Table[1, 7];
                    DataGridViewCell GrossBetaCPMCell = FullResults_Table[1, 8];

                    DataGridViewCell AlphaLcCell = FullResults_Table[1, 13];
                    DataGridViewCell BetaLcCell = FullResults_Table[1, 14];
                    DataGridViewCell AlphaSrcMDACell = FullResults_Table[1, 15];
                    DataGridViewCell BetaSrcMDACell = FullResults_Table[1, 16];
                    DataGridViewCell NetAlphaCPMCell = FullResults_Table[1, 17];
                    DataGridViewCell NetBetaCPMCell = FullResults_Table[1, 18];
                    DataGridViewCell NetAlphaUncertaintyCell = FullResults_Table[1, 19];
                    DataGridViewCell NetBetaUncertaintyCell = FullResults_Table[1, 20];

                    DataGridViewCell AlphaActivityDPMCell = FullResults_Table[1, 34];
                    DataGridViewCell AlphaUncetaintyDPMCell = FullResults_Table[1, 32];
                    DataGridViewCell BetaActivityDPMCell = FullResults_Table[1, 35];
                    DataGridViewCell BetaUncertaintyDPMCell = FullResults_Table[1, 33];

                    /*Parse data to form*/

                    this.ElapsedTime = IncomingData.ElTime;

                    if (IncomingData != null && this.ElapsedTime != 0)
                    {

                        GrossAlphaCell.Value = IncomingData.AlphaTot;
                        GrossAlphaCPMCell.Value = StaticMethods.RoundToDecimal(Convert.ToInt32(IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)), 1); //Convert raw counts into CPM.
                        GrossBetaCell.Value = IncomingData.BetaTot;
                        GrossBetaCPMCell.Value = StaticMethods.RoundToDecimal(Convert.ToInt32(IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)), 1);

                        NetAlphaCPMCell.Value = StaticMethods.RoundToSigFigs(Convert.ToInt32(Convert.ToDouble(GrossAlphaCPMCell.Value) - AlphaBackground));
                        NetAlphaUncertaintyCell.Value = StaticMethods.RoundToSigFigs(Math.Sqrt(Math.Abs(Convert.ToDouble(NetAlphaCPMCell.Value))));

                        NetBetaCPMCell.Value = StaticMethods.RoundToSigFigs(Convert.ToInt32(Convert.ToDouble(GrossBetaCPMCell.Value) - BetaBackground));
                        NetBetaUncertaintyCell.Value = StaticMethods.RoundToSigFigs(Math.Sqrt(Math.Abs(Convert.ToDouble(NetBetaCPMCell.Value))));

                        /*TODO: Add more formulas here!*/
                        /*Lc Formula*/
                        AlphaLcCell.Value = StaticMethods.RoundToSigFigs(((ConfRange) * Math.Sqrt(AlphaBackground * ((1 / (Convert.ToDouble(BackgroundCountTime) / 60)) + (1 / (Convert.ToDouble(ElapsedTime) / 60))))));
                        BetaLcCell.Value = StaticMethods.RoundToSigFigs(((ConfRange) * Math.Sqrt(BetaBackground * ((1 / (Convert.ToDouble(BackgroundCountTime) / 60)) + (1 / (Convert.ToDouble(ElapsedTime) / 60))))));

                        /*Check for NaN*/
                        if (Convert.ToDouble(AlphaLcCell.Value) != Convert.ToDouble(AlphaLcCell.Value))
                        {
                            AlphaLcCell.Value = 0;
                        }

                        if (Convert.ToDouble(BetaLcCell.Value) != Convert.ToDouble(BetaLcCell.Value))
                        {
                            BetaLcCell.Value = 0;
                        }

                        /*TODO: Need to show this?*/
                        double AlphaLd = (((ConfRange) * (ConfRange)) + (2 * Convert.ToDouble(AlphaLcCell.Value))) / (Convert.ToDouble(ElapsedTime) / 60);
                        double BetaLd = (((ConfRange) * (ConfRange)) + (2 * Convert.ToDouble(BetaLcCell.Value))) / (Convert.ToDouble(ElapsedTime) / 60);

                        /*Check for NaN*/
                        if (AlphaLd != AlphaLd)
                        {
                            AlphaLd = 0;
                        }

                        if (BetaLd != BetaLd )
                        {
                            
                            BetaLd = 0;
                        }

                        /*MDA Formula*/
                        try
                        {
                            AlphaSrcMDACell.Value = StaticMethods.RoundToSigFigs(AlphaLd / (AlphaEfficiency * .01));
                            BetaSrcMDACell.Value = StaticMethods.RoundToSigFigs(BetaLd / (BetaEfficiency * .01));

                            if (Convert.ToDouble(AlphaSrcMDACell.Value) != Convert.ToDouble(AlphaSrcMDACell.Value))
                            {
                                AlphaSrcMDACell.Value = 99999;
                            }

                            if (Convert.ToDouble(BetaSrcMDACell.Value) != Convert.ToDouble(BetaSrcMDACell.Value))
                            {
                                BetaSrcMDACell.Value = 99999;
                            }
                        }

                        catch
                        {
                            AlphaSrcMDACell.Value = 99999;
                            BetaSrcMDACell.Value = 99999;
                        }

                        /*Net counts*/
                        AlphaActivityDPMCell.Value = StaticMethods.RoundToSigFigs(Convert.ToDouble(NetAlphaCPMCell.Value) * 100 / AlphaEfficiency);
                        
                        if (Convert.ToDouble(AlphaActivityDPMCell.Value) != Convert.ToDouble(AlphaActivityDPMCell.Value))
                        {
                            AlphaActivityDPMCell.Value = 0;
                        }
                        
                        this.AlphaDPM = Convert.ToDouble(AlphaActivityDPMCell.Value);
                        AlphaUncetaintyDPMCell.Value = StaticMethods.RoundToSigFigs(Math.Sqrt(Math.Abs(AlphaDPM)));

                        BetaActivityDPMCell.Value = StaticMethods.RoundToSigFigs(Convert.ToDouble(NetBetaCPMCell.Value) * 100 / BetaEfficiency);

                        if (Convert.ToDouble(BetaActivityDPMCell.Value) != Convert.ToDouble(BetaActivityDPMCell.Value))
                        {
                            BetaActivityDPMCell.Value = 0;
                        }

                        this.BetaDPM = Convert.ToDouble(BetaActivityDPMCell.Value);
                        BetaUncertaintyDPMCell.Value = StaticMethods.RoundToSigFigs(Math.Sqrt(Math.Abs(BetaDPM)));

                        /*Re-draw table*/
                        FullResults_Table.Invalidate();

                        if (PacketReceived != null && !ShouldStop)
                        {
                            PacketReceived(this, null);
                        }

                        if (this.Type == RoutineSampleCountingForm.RoutineSampleCountType.Time)
                        {
                            /*If the sample time has elapsed, increment the row.*/
                            if ((IncomingData.ElTime >= SampleTime))
                            {
                                    RowComplete = true;
                            }
                        }

                        if (this.Type == RoutineSampleCountingForm.RoutineSampleCountType.MDA)
                        {
                            if ((Convert.ToDouble(AlphaSrcMDACell.Value) < AlphaMDASampleValue) && (Convert.ToDouble(BetaSrcMDACell.Value) < BetaMDASampleValue))
                            {
                                RowComplete = true;
                            }
                        }

                    }
                    DABRAS.KickWatchdog();
                }
            }

            DABRAS.DisableWatchdog();

            if (!ShouldStop)
            {
                this.BackgroundFinished = DateTime.Now;
                WasBackgroundFinishedSuccessfully = true;
                this.Done = true;
                if (BackgroundSampleThreadFinished != null)
                {
                    using (SoundPlayer S = new SoundPlayer(DABRAS_Software.Properties.Resources.DoneSound))
                    {
                        S.Play();
                    }

                    BackgroundSampleThreadFinished(this, null);
                }
            }

            /*Resume background threads, if they exist*/
            DABRAS.ResumeBackgroundMonitors();
            Running = false;

            return;
        }
        #endregion
    }
}
