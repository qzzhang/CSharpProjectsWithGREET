using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Media;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace DABRAS_Software
{
    public partial class FormRSC
    {
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
            if (!this.dabras.IsConnected())
            {
                this.frmParent.refreshConnectStatus();
                this.endFormActivities();
                return;
            }

            if (RL.IsDone())
            {
                Status_Label.Text = "Completed!";//"Stopped";

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
                SetGUI_RSC(false);

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

                this.autoWriteToFiles();

                if (!dabras.IsConnected())
                {
                    this.frmParent.disconnectCleanup("Not Connected");
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

        private void initRSC()
        {
            //Initialize RSC dropdown boxes
            this.updateRSCCombos();

            //Don't need to error check - default values
            try
            {
                this.Preset_Time_Label.Text = String.Format("Preset time: {0}:{1}", Convert.ToInt32(Min_TB.Text), Convert.ToInt32(Sec_TB.Text));

                this.MF = this.frmParent.GetDefaultConfig().GetModFactors();

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

            this.initDataResults();

            this.Elapsed_Time_Label.Text = "Elapsed time: 0:00";

            this.SetTimeButton_Click(this, null);
        }
        private RadionuclideFamily findSourceFamily(Radioactive_Source src)
        {
            return (this.frmParent.GetListOfFamily().Find(x => (x.GetFamilyID() == src.GetFamilyID())));
        }
        public void updateRSCCombos()
        {
            //Initialize RSC dropdown boxes
            this.Alpha_ComboBox.Items.Clear();
            this.Beta_ComboBox.Items.Clear();

            foreach (RadionuclideFamily rf in this.frmParent.GetListOfFamily())
            {
                if (rf.GetSourceType() == RadionuclideFamily.RadiationType.Alpha)
                {
                    Alpha_ComboBox.Items.Add(rf.GetName());
                }

                if (rf.GetSourceType() == RadionuclideFamily.RadiationType.Beta)
                {
                    Beta_ComboBox.Items.Add(rf.GetName());
                }
            }

            if (Alpha_ComboBox.Items.Count > 0)
            {
                Alpha_ComboBox.Text = Convert.ToString(Alpha_ComboBox.Items[0]);

                this.Current_Alpha_Eff = this.frmParent.GetListOfFamily().Find(x => x.GetName() == Alpha_ComboBox.Text).GetCurrentSource().GetAlphaEfficiency();
            }

            if (Beta_ComboBox.Items.Count > 0)
            {
                Beta_ComboBox.Text = Convert.ToString(Beta_ComboBox.Items[0]);

                this.Current_Beta_Eff = this.frmParent.GetListOfFamily().Find(x => x.GetName() == Alpha_ComboBox.Text).GetCurrentSource().GetBetaEfficiency();
            }
        }

        private void initDataResults()//DataGridView Initialization
        {
            FullDataResults.Columns.Add("Headers", "");
            DataGridViewColumn Headers = FullDataResults.Columns[0];
            Headers.Width = 175;

            //33 rows, indexed 0-32 in groups of 10
            FullDataResults.Rows.Add("Id Number");
            FullDataResults.Rows.Add("Detailed Description");
            FullDataResults.Rows.Add("Start Date/Time");
            FullDataResults.Rows.Add("End Date/Time");
            FullDataResults.Rows.Add("Count Time (min)");
            FullDataResults.Rows.Add("Gross Alpha Count");
            FullDataResults.Rows.Add("Gross Beta Count");
            FullDataResults.Rows.Add("Gross Alpha CPM");
            FullDataResults.Rows.Add("Gross Beta CPM");

            if (this.frmParent.BGType == mainFramework.BackgroundType.Annual)
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

            //Don't let the user edit the headertext (can't set whole table to readonly because need to be able to edit description cell
            for (int i = 0; i < FullDataResults.RowCount; i++)
            {
                FullDataResults[0, i].ReadOnly = true;
            }
        }

        private void initRSCgridByFile()
        {
            //Attempt to find folder with CSV data
            string CurrentEnv = Environment.CurrentDirectory;
            CurrentEnv += String.Format("\\data\\Routine\\{0}", this.Sample_ID_TB.Text);
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
        }

        #region untility functions
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
            for (int i = 0; i < InString.Length; i++)
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

        #endregion

        #region Private Utility Functions for the RSC portion
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

        private void SetGUI_RSC(bool Running)
        {
            this.New_Count_Button.Enabled = !Running;
            this.Continue_Count_Button.Enabled = !Running;
            this.SetTimeButton.Enabled = !Running;
            this.Min_TB.Enabled = !Running;
            this.Sec_TB.Enabled = !Running;
            this.AlphaMDALimit_TB.Enabled = !Running;
            this.BetaMDALimit_TB.Enabled = !Running;
            this.TimeRadioButton.Enabled = !Running;
            this.CountMDARadioButton.Enabled = !Running;

            this.PauseButton.Enabled = Running;
            this.Stop_Count_Button.Enabled = Running;
        }

        /*Verify input string*/
        private string verifySampleDesc()
        {
            string DescriptionString = this.Description_TB.Text;
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
            return DescriptionString;
        }

        private bool verifySampleNum()//Verify sample number, has to be numerical
        {
            int sampleNo = 0;
            bool sampleVerified = false;
            while (!sampleVerified)
            {
                try
                {
                    sampleNo = Convert.ToInt32(this.Sample_ID_TB.Text);
                    sampleVerified = true;
                }
                catch
                {
                    Prompt NewForm = new Prompt("The badge number entered is not a valid badge number. Please enter your badge number.");
                    NewForm.ShowDialog();
                    this.Sample_ID_TB.Text = NewForm.GetResponse();
                }
            }
            return sampleVerified;
        }

        private void autoWriteToFiles()
        {
            if (this.verifySampleNum())
            {
                string desc = this.verifySampleDesc();
                int smpNo = Convert.ToInt32(this.Sample_ID_TB.Text);

                string CustomFilePath = String.Format("{0}\\data\\Routine\\{1}\\{2}_{3}_{4}_{5}_{6}_RSC.csv", Environment.CurrentDirectory, smpNo, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, desc, this.GetDABRAS().Serial_Number);
                string MasterPath = String.Format("{0}\\data\\Routine\\Master\\Master.csv", Environment.CurrentDirectory);
                string CustomDir = String.Format("{0}\\data\\Routine\\{1}", Environment.CurrentDirectory, smpNo);
                string[,] DataToWrite = this.frmParent.MakeDataWritable(this.FullDataResults);
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
                        this.frmParent.GetLogger().WriteCSV(F, DataToWrite);
                        //Toast T = new Toast(String.Format("File written to {0}", CustomFilePath));
                        //Toast T = new Toast("File Written.");
                        //T.Show();
                    }

                    using (FileStream F = new FileStream(MasterPath, FileMode.Append))
                    {
                        this.frmParent.GetLogger().WriteCSV(F, DataToWrite);
                    }
                }
                catch
                {
                    MessageBox.Show("Automatic write failed.");
                }
            }
        }
        #endregion
/*
        #region Quick Calibration Functions
        public void Start1minCheck(object SenderForm, List<Radioactive_Source> ListOfCalibratedSources, string NameOfAlpha, string NameOfBeta)
        {
            //Don't run if this isn't called from the correct form
            if (!(SenderForm is QuickCalibrationController))
            {
                MessageBox.Show("Bad form type. Stopping...");
                //closeCtrlQToolStripMenuItem_Click(this, null);
                return;
            }

            //Set sources
            this.Alpha_ComboBox.Text = NameOfAlpha;
            Alpha_ComboBox_SelectedIndexChanged(this, null);

            this.Beta_ComboBox.Text = NameOfBeta;
            Beta_ComboBox_SelectedIndexChanged(this, null);

            this.frmParent.BGType = mainFramework.BackgroundType.Annual;

            this.frmParent.ListOfSources = ListOfCalibratedSources;

            this.Min_TB.Text = "1";
            this.Sec_TB.Text = "0";
            this.Calibrating = true;

            SetTimeButton_Click(this, null);
            New_Count_Button_Click(this, null);

            return;
        }

        public void StartUniformityTest(object SenderForm, List<Radioactive_Source> ListOfCalibratedSources)
        {
            //Don't run if this isn't called from the correct form
            if (!(SenderForm is QuickCalibrationController))
            {
                MessageBox.Show("Bad form type. Stopping...");
                //closeCtrlQToolStripMenuItem_Click(this, null);
                return;
            }

            this.frmParent.BGType = mainFramework.BackgroundType.Annual;

            this.frmParent.ListOfSources = ListOfCalibratedSources;

            this.Min_TB.Text = "1";
            this.Sec_TB.Text = "0";
            this.Calibrating = true;

            SetTimeButton_Click(this, null);
            New_Count_Button_Click(this, null);

            return;
        }

        #endregion
*/
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

        private FormRSC.RoutineSampleCountType Type;

        public event EventHandler BackgroundSampleThreadFinished;
        public event EventHandler PacketReceived;

        private const double ConfRange = 1.645; //95 % confidence range

        private bool Running = false;

        public mainFramework frmCaller;
        #endregion

        #region Constructor
        public RoutineSampleListener(DABRAS _DABRAS, int _SampleTime, double _MDAStopValue, DataGridView _FullTable, int _AlphaBG, int _BetaBG, double _AlphaE, double _BetaE, double _AlphaAbsorbtion, double _BetaAbsorbtion, double _BetaBackscatter, double _RemovalPercentage, double _Area, int _BGCountTime, FormRSC.RoutineSampleCountType _Type, double _AlphaMDA, double _BetaMDA)
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

            if (this.Type == FormRSC.RoutineSampleCountType.MDA)
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

        public FormRSC.RoutineSampleCountType GetTestType()
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
            //TODO: Figure out what to do with these...
            double Alpha_SelfAbsorption = Convert.ToDouble(FullResults_Table["NewColumn", 25].Value);
            double Beta_SelfAbsorption = Convert.ToDouble(FullResults_Table["NewColumn", 26].Value);
            double Beta_BackScatter = Convert.ToDouble(FullResults_Table["NewColumn", 27].Value);
            double Contaminant_Removal_Fraction = Convert.ToDouble(FullResults_Table["NewColumn", 28].Value);
            double Sample_Area = Convert.ToDouble(FullResults_Table["NewColumn", 29].Value);

            //Stop background threads
           // DABRAS.Cut();

            //Set aquisition time
            DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
            Thread.Sleep(500);

            //Start count
            DABRAS.Write_To_Serial_Port("g");

            //Clear any data left in the buffer
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();
            DABRAS.EnableWatchdog();

            bool RowComplete = false;

            //Check for the first good packet*
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
                    MessageBox.Show("Error: Connection lost by RSC.");
                    this.frmCaller.refreshConnectStatus();
                    DABRAS.DisableWatchdog();
                    this.Done = true;
                    BackgroundSampleThreadFinished(this, null);
                    //mainFramework.disconnectCleanup();
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

            //Do not increment the row index until the current sample time has elapsed
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

                //Wait for incoming data packet
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
                    MessageBox.Show("Error: Connection lost by RSC.");
                    this.frmCaller.refreshConnectStatus();
                    DABRAS.DisableWatchdog();
                    this.Done = true;
                    BackgroundSampleThreadFinished(this, null);
                    return;
                }

                SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                //Grab handles to form
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

                    //Parse data to form
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

                        //TODO: Add more formulas here!
                        //Lc Formula
                        AlphaLcCell.Value = StaticMethods.RoundToSigFigs(((ConfRange) * Math.Sqrt(AlphaBackground * ((1 / (Convert.ToDouble(BackgroundCountTime) / 60)) + (1 / (Convert.ToDouble(ElapsedTime) / 60))))));
                        BetaLcCell.Value = StaticMethods.RoundToSigFigs(((ConfRange) * Math.Sqrt(BetaBackground * ((1 / (Convert.ToDouble(BackgroundCountTime) / 60)) + (1 / (Convert.ToDouble(ElapsedTime) / 60))))));

                        //Check for NaN
                        if (Convert.ToDouble(AlphaLcCell.Value) != Convert.ToDouble(AlphaLcCell.Value))
                        {
                            AlphaLcCell.Value = 0;
                        }

                        if (Convert.ToDouble(BetaLcCell.Value) != Convert.ToDouble(BetaLcCell.Value))
                        {
                            BetaLcCell.Value = 0;
                        }

                        //TODO: Need to show this?
                        double AlphaLd = (((ConfRange) * (ConfRange)) + (2 * Convert.ToDouble(AlphaLcCell.Value))) / (Convert.ToDouble(ElapsedTime) / 60);
                        double BetaLd = (((ConfRange) * (ConfRange)) + (2 * Convert.ToDouble(BetaLcCell.Value))) / (Convert.ToDouble(ElapsedTime) / 60);

                        //Check for NaN
                        /*
                        if (AlphaLd != AlphaLd)
                        {
                            AlphaLd = 0;
                        }

                        if (BetaLd != BetaLd)
                        {

                            BetaLd = 0;
                        }
                        */
                        //MDA Formula
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

                        //Net counts
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

                        //Re-draw table
                        FullResults_Table.Invalidate();

                        if (PacketReceived != null && !ShouldStop)
                        {
                            PacketReceived(this, null);
                        }

                        if (this.Type == FormRSC.RoutineSampleCountType.Time)
                        {
                            //If the sample time has elapsed, increment the row.
                            if ((IncomingData.ElTime >= SampleTime))
                            {
                                RowComplete = true;
                            }
                        }

                        if (this.Type == FormRSC.RoutineSampleCountType.MDA)
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

            //Resume background threads, if they exist
            //DABRAS.ResumeBackgroundMonitors();
            Running = false;

            return;
        }
        #endregion
    }
}