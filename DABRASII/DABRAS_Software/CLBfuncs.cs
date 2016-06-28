using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Media;
using System.Drawing.Imaging;

namespace DABRAS_Software
{
    public partial class FormCLB : Form
    {
        #region Background Calibration Callbacks
        private void InvokeThreadCallback()
        {
            this.Invoke(new OnBackgroundThreadFinished(UpdateCalibratedSource));
        }

        private void UpdateCalibratedSource()
        {
            if (!this.DABRAS.IsConnected())
            {
                this.frmParent.refreshConnectStatus();
                this.endFormActivities();
                return;
            }

            if (EL != null)
            {
                if (EL.WasTestCompleted())
                {
                    Radioactive_Source CalibratedSource = ListOfValidSources.Find(x => x.GetSerialNumber() == EL.GetCalibratedSource().GetSerialNumber());

                    if ((this.findSourceFamily(CalibratedSource).GetSourceType() == RadionuclideFamily.RadiationType.Alpha) 
                        || (this.findSourceFamily(CalibratedSource).GetSourceType() == RadionuclideFamily.RadiationType.AlphaBeta))
                    {
                        CalibratedSource.SetAnnualAlphaCPM(Convert.ToInt32(EL.GetAlphaNCPM()));
                        this.AlphaEff = EL.GetAlphaEfficiency();
                        CalibratedSource.SetAlphaEfficiency(this.AlphaEff);
                        CalibratedSource.SetAlphaDisintigrationConstant(EL.GetAlphaDecayFactor());
                    }

                    if ((this.findSourceFamily(CalibratedSource).GetSourceType() == RadionuclideFamily.RadiationType.Beta) 
                        || (this.findSourceFamily(CalibratedSource).GetSourceType() == RadionuclideFamily.RadiationType.AlphaBeta))
                    {
                        CalibratedSource.SetAnnualBetaCPM(Convert.ToInt32(EL.GetBetaNCPM()));
                        this.BetaEff = EL.GetBetaEfficiency();
                        CalibratedSource.SetBetaEfficiency(this.BetaEff);
                        CalibratedSource.SetBetaDisintigrationFactor(EL.GetBetaDecayFactor());                    
                    }

                    CalibratedSource.SetAnnualCalibratedDate(EL.GetDateTimeCompleted());

                    //update the configuration file
                    this.updFamilyAndSource(this.ListOfValidFamily, this.ListOfValidSources);
                    this.updParentFamilyAndSource(this.ListOfValidFamily, this.ListOfValidSources);

                    this.EfficiencyCompleted = true;
                    this.lbl_EFFcalDate.Text = (EL.GetDateTimeCompleted()).ToShortDateString();

                    //Write to file
                    string effDir = String.Format("{0}\\data\\Cal\\Eff", Environment.CurrentDirectory);
                    string effPath = String.Format("{0}\\data\\Cal\\Eff\\{1}_{2}_{3}_{4}_{5}_Eff.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DABRAS.Serial_Number, EL.GetCalibratedSource().GetSerialNumber());
                    this.autoWrite2File(effDir, effPath, this.Calibration_Results_GridView);

                    this.EL.RequestStop();
                    this.EL = null;
                }
                else
                {
                    if ((MessageBox.Show(String.Format("Efficiency scanning for {0} failed to pass the criteria of -5%~5%. Repeat the efficiency scan?", this.Source_ComboBox.Text), "Verify", MessageBoxButtons.YesNo)) != DialogResult.Yes)
                    {
                        MessageBox.Show("Calibration incomplete.");
                        this.CalibrationCompleted = false;
                        this.EL.RequestStop();
                        this.EL = null;
                        this.Stop_Count_Button_Click(null, null);
                        return;
                    }
                    else
                    {
                        this.EL.RequestStop();
                        this.EL = null;
                        this.Determine_Efficiency_Button_Click(null, null);
                    }
                }
            }
            else if (BL != null && BL.WasTestCompleted())
            {
                Radioactive_Source BackgroundSource = ListOfValidSources.Find(x => x.GetSerialNumber() == "Background");

                //Type should always be AlphaBeta...
                if (this.findSourceFamily(BackgroundSource).GetSourceType() == RadionuclideFamily.RadiationType.AlphaBeta)
                {
                    BackgroundSource.SetAnnualAlphaCPM(BL.GetAlphaBackground());
                    BackgroundSource.SetAnnualBetaCPM(BL.GetBetaBackground());
                    BackgroundSource.SetAlphaDisintigrationConstant(BL.GetAlphaDecayFactor());
                    BackgroundSource.SetBetaDisintigrationFactor(BL.GetBetaDecayFactor());
                }

                BackgroundSource.SetAnnualCalibratedDate(BL.GetDateTimeCompleted());

                //update the configuration file
                this.updFamilyAndSource(this.ListOfValidFamily, this.ListOfValidSources);
                this.updParentFamilyAndSource(this.ListOfValidFamily, this.ListOfValidSources);

                this.BackgroundCompleted = true;
                this.lbl_BGcalDate.Text = (BL.GetDateTimeCompleted()).ToShortDateString();

                //Write to file
                string bkgdDir = String.Format("{0}\\data\\Cal\\Bkgd", Environment.CurrentDirectory);
                string filePath = String.Format("{0}\\data\\Cal\\Bkgd\\{1}_{2}_{3}_{4}_Bkgd.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DABRAS.Serial_Number);
                string[,] DataToWrite = this.frmParent.MakeDataWritable(Background_Results_GridView);
                this.autoWrite2File(bkgdDir, filePath, Background_Results_GridView);

                this.BL.RequestStop();
                this.BL = null;
                this.Stop_Count_Button_Click(null, null);
            }

            this.CalibrationCompleted = this.EfficiencyCompleted && this.BackgroundCompleted;

            this.SetGUI(false);
            this.NewSources = true;
            return;
        }
        #endregion

        #region Private Utility Functions
        private void FillDataGridView(string[,] arrData, DataGridView dgv)
        {
            int Rows = arrData.GetLength(0);
            int Cols = arrData.GetLength(1);
            int smpTime = Convert.ToInt32(arrData[1, 1]);
            int smpNum = Rows - 1;
            string dgvNm = dgv.Name;

            if (dgvNm == "Background_Results_GridView")
            {
                this.BG_NumCounts_TB.Text = Convert.ToString(Rows - 5);
                this.lbl_BGcalDate.Text = (DateTime.Now).ToShortDateString();
                this.Min_BG_TB.Text = Convert.ToString(smpTime / 60);
                this.Sec_BG_TB.Text = Convert.ToString(smpTime % 60);
            }
            else if (dgvNm == "Calibration_Results_GridView")
            {
                this.EFF_NumCounts_TB.Text = Convert.ToString(Rows - 5);
                this.lbl_EFFcalDate.Text = (DateTime.Now).ToShortDateString();
                this.Min_EFF_TB.Text = Convert.ToString(smpTime / 60);
                this.Sec_EFF_TB.Text = Convert.ToString(smpTime % 60);
            }

            this.Add_Or_Subtract_Rows(smpNum, dgv);

            for (int i = 1; i <= smpNum; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    dgv[j, i - 1].Value = arrData[i, j];
                }
            }
        }
        private void autoWrite2File(string theDir, string theFilePath, DataGridView grdv)
        {
            string grdNm = "";
            string[,] DataToWrite = this.frmParent.MakeDataWritable(grdv);

            if (grdv.Name == "Background_Results_GridView")
            {
                this.bkgrdData = DataToWrite;
                grdNm = "Background";
            }
            else if (grdv.Name == "Calibration_Results_GridView")
            {
                this.eff_Data = DataToWrite;
                grdNm = "Efficiency";
            }

            try
            {
                if (!Directory.Exists(theDir))
                {
                    Directory.CreateDirectory(theDir);
                }

                if (!File.Exists(theFilePath))
                {
                    File.Create(theFilePath).Dispose();
                }

                using (FileStream F = new FileStream(theFilePath, FileMode.Append))
                {
                    this.frmParent.GetLogger().WriteCSV(F, DataToWrite);
                }
            }
            catch (Exception wex)
            {
                MessageBox.Show("Automatic write " + grdNm + " failed." + wex.Message);
            }
        }
        private void SelectCorrectBeta()
        {
            Radioactive_Source R = this.ListOfValidSources.Find(x => x.GetSerialNumber() == Source_ComboBox.Text);
            RadionuclideFamily RF = this.ListOfValidFamily.Find(y => (y.GetFamilyID() == R.GetFamilyID()));
            if (R != null)
            {
                if (RF.GetSourceType() != RadionuclideFamily.RadiationType.Beta)
                {
                    this.grpbBetaEnergy.Visible = false;
                    return;
                }

                //We have a beta source, but they radio buttons are for display only, not for editing
                this.grpbBetaEnergy.Visible = true;
                this.grpbBetaEnergy.Enabled = false;

                RadionuclideFamily.EnergyBand CurrentEnergyLevel = RF.GetEnergyBand();

                switch (CurrentEnergyLevel)
                {
                    case RadionuclideFamily.EnergyBand.Beta_Less_100KeV:
                        this._100_Energy_Button.Checked = true;
                        this._100_200_Energy_Button.Checked = false;
                        this._200_400_Energy_Button.Checked = false;
                        this._400_1200_Energy_Button.Checked = false;
                        this._1200_Energy_Button.Checked = false;
                        return;
                    case RadionuclideFamily.EnergyBand.Beta_100_200KeV:
                        this._100_Energy_Button.Checked = false;
                        this._100_200_Energy_Button.Checked = true;
                        this._200_400_Energy_Button.Checked = false;
                        this._400_1200_Energy_Button.Checked = false;
                        this._1200_Energy_Button.Checked = false;
                        return;
                    case RadionuclideFamily.EnergyBand.Beta_200_400KeV:
                        this._100_Energy_Button.Checked = false;
                        this._100_200_Energy_Button.Checked = false;
                        this._200_400_Energy_Button.Checked = true;
                        this._400_1200_Energy_Button.Checked = false;
                        this._1200_Energy_Button.Checked = false;
                        return;
                    case RadionuclideFamily.EnergyBand.Beta_400_1200KeV:
                        this._100_Energy_Button.Checked = false;
                        this._100_200_Energy_Button.Checked = false;
                        this._200_400_Energy_Button.Checked = false;
                        this._400_1200_Energy_Button.Checked = true;
                        this._1200_Energy_Button.Checked = false;
                        return;
                    case RadionuclideFamily.EnergyBand.Beta_More_1200KeV:
                        this._100_Energy_Button.Checked = false;
                        this._100_200_Energy_Button.Checked = false;
                        this._200_400_Energy_Button.Checked = false;
                        this._400_1200_Energy_Button.Checked = false;
                        this._1200_Energy_Button.Checked = true;
                        return;
                    default:
                        this._100_Energy_Button.Checked = false;
                        this._100_200_Energy_Button.Checked = false;
                        this._200_400_Energy_Button.Checked = false;
                        this._400_1200_Energy_Button.Checked = false;
                        this._1200_Energy_Button.Checked = false;
                        return;

                }
            }
        }
        private RadionuclideFamily findSourceFamily(Radioactive_Source src)
        {
            return (this.ListOfValidFamily.Find(x => (x.GetFamilyID() == src.GetFamilyID())));
        }
        private void SetSourceHalfLifeAndCurrentlyAppliedActivity()
        {
            DateTime Today = DateTime.Now;
            ulong famhalflife;
            foreach (Radioactive_Source rs in this.ListOfValidSources)
            {
                famhalflife = (this.ListOfValidFamily.Find(x => (x.GetFamilyID() == rs.GetFamilyID()))).GetHalfLife();

                //Computing the disintigration decay value
                TimeSpan ElapsedTime = Today.Subtract(DateTime.Parse(rs.GetCertificationDate()));
                double ElapsedHalfLives = Convert.ToDouble(ElapsedTime.TotalSeconds) / Convert.ToDouble(famhalflife);
                double HalfLifeMultiplier = Math.Pow(0.5, ElapsedHalfLives);

                rs.SetCurrentlyAppliedActivity(Convert.ToInt32((HalfLifeMultiplier * rs.GetCertifiedActivity())));
                rs.SetHalfLife(famhalflife);
            }
        }
        private void initBackgroundTable()
        {
            this.Background_Results_GridView.Columns.Add("Sample number", "Sample#");
            this.Background_Results_GridView.Columns.Add("Background_Results_Acq_Time", "Acq Time");
            this.Background_Results_GridView.Columns.Add("Background_Results_Alpha_Count", "Alpha Count");
            this.Background_Results_GridView.Columns.Add("Background_Results_Alpha_CPM", "Alpha CPM");
            this.Background_Results_GridView.Columns.Add("Background_Results_Beta_Count", "Beta Count");
            this.Background_Results_GridView.Columns.Add("Background_Results_Beta_CPM", "Beta CPM");

            for (int col_i = 0; col_i < this.Background_Results_GridView.Columns.Count; col_i++)
            {
                DataGridViewColumn column = this.Background_Results_GridView.Columns[col_i];
                column.ValueType = typeof(int);
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            int bgRowIterator = 0;
            try
            {
                bgRowIterator = Convert.ToInt32(this.BG_NumCounts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Non-integral value of counts. Exiting...");
                return;
            }

            //Add one row for each count
            for (int i = 0; i < bgRowIterator; i++)
            {
                this.Background_Results_GridView.Rows.Add(new object[] { i + 1, "", "", "", "", "", "", "" });
            }

            this.BackgroudFootDGV.Columns.Add("Sample number", "Sample#");
            this.BackgroudFootDGV.Columns.Add("Background_Results_Acq_Time", "Acq Time");
            this.BackgroudFootDGV.Columns.Add("Background_Results_Alpha_Count", "Alpha Count");
            this.BackgroudFootDGV.Columns.Add("Background_Results_Alpha_CPM", "Alpha CPM");
            this.BackgroudFootDGV.Columns.Add("Background_Results_Beta_Count", "Beta Count");
            this.BackgroudFootDGV.Columns.Add("Background_Results_Beta_CPM", "Beta CPM");
            this.BackgroudFootDGV.Rows.Add(new object[] { "Average", "", "", "", "", ""});
            this.BackgroudFootDGV.Rows.Add(new object[] { "StDev", "", "", "", "", "" });
        }
        private void initEfficiencyTable()
        {
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Source", "Sample#");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Acq_Time", "Acq Time");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Alpha_Count", "Alpha Count");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Alpha_CPM", "Alpha NCPM");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Beta_Count", "Beta Count");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Beta_CPM", "Beta NCPM");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Alpha_Eff", "Alpha Efficiency");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Beta_Eff", "Beta Efficiency");

            for (int col_i = 0; col_i < this.Calibration_Results_GridView.Columns.Count; col_i++)
            {
                DataGridViewColumn column = this.Calibration_Results_GridView.Columns[col_i];
                if (column.Index == 6 || column.Index == 7)
                    column.ValueType = typeof(double);
                else
                    column.ValueType = typeof(int);
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            int RowIterator = 0;
            try
            {
                RowIterator = Convert.ToInt32(this.EFF_NumCounts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Non-integral value of counts. Exiting...");
                return;
            }

            //Add one row for each count
            for (int i = 0; i < RowIterator; i++)
            {
                Calibration_Results_GridView.Rows.Add(new object[] { i + 1, "", "", "", "", "", "", "" });
            }

            this.EfficiencyFootDGV.Columns.Add("Calibration_Results_Source", "Sample#");
            this.EfficiencyFootDGV.Columns.Add("Calibration_Results_Acq_Time", "Acq Time");
            this.EfficiencyFootDGV.Columns.Add("Calibration_Results_Alpha_Count", "Alpha Count");
            this.EfficiencyFootDGV.Columns.Add("Calibration_Results_Alpha_CPM", "Alpha NCPM");
            this.EfficiencyFootDGV.Columns.Add("Calibration_Results_Beta_Count", "Beta Count");
            this.EfficiencyFootDGV.Columns.Add("Calibration_Results_Beta_CPM", "Beta NCPM");
            this.EfficiencyFootDGV.Columns.Add("Calibration_Results_Alpha_Eff", "Alpha Efficiency");
            this.EfficiencyFootDGV.Columns.Add("Calibration_Results_Beta_Eff", "Beta Efficiency");

            this.EfficiencyFootDGV.Rows.Add(new object[] { "Average", "", "", "", "", "", "", ""});
            this.EfficiencyFootDGV.Rows.Add(new object[] { "StDev", "", "", "", "", "", "", ""});

            //Add two more rows-one for the forced 1 minute count results and one for % deviation from the averaged counts
            this.EfficiencyFootDGV.Rows.Add(new object[] { "Extra 1 minute", "", "", "", "", "", "", "" });
            this.EfficiencyFootDGV.Rows.Add(new object[] { "%Difference", "", "", "", "", "", "", "" });
        }
        private void initButtonStates(bool bval)
        {
            this.Stop_Count_Button.Enabled = bval;
            this.SaveBackgroundCalibrationButton.Enabled = bval;
            this.SaveEfficiencyDataButton.Enabled = bval;
            this.ImageSaveButton.Enabled = bval;
        }
        private void SetGUI(bool testRunning)
        {
            this.Determine_BG_Button.Enabled = !testRunning;
            this.Determine_Efficiency_Button.Enabled = !testRunning;

            this.SaveBackgroundCalibrationButton.Enabled = !testRunning;
            this.SaveEfficiencyDataButton.Enabled = !testRunning;
            this.ImageSaveButton.Enabled = !testRunning;

            this.Stop_Count_Button.Enabled = testRunning; 

            this.Source_ComboBox.Enabled = !testRunning;
            this.EFF_NumCounts_TB.Enabled = !testRunning;
            this.Min_EFF_TB.Enabled = !testRunning;
            this.Sec_EFF_TB.Enabled = !testRunning;

            this.BG_NumCounts_TB.Enabled = !testRunning;
            this.Min_BG_TB.Enabled = !testRunning;
            this.Sec_BG_TB.Enabled = !testRunning;

            return;
        }
        private void Add_Or_Subtract_Rows(int FinalNumberOfRows, DataGridView DG)
        {
            if ((DG.RowCount) == FinalNumberOfRows)
            {
                return;
            }

            if ((DG.RowCount) > FinalNumberOfRows)
            {
                //Too many get rid of a few
                while ((DG.RowCount) > FinalNumberOfRows)
                {
                    DG.Rows.RemoveAt(0);
                }
            }
            else
            {
                while ((DG.RowCount) < FinalNumberOfRows)
                {
                    DG.Rows.Insert(0, "", "", "", "", "", "", "", "");
                }
            }

            //Re-number the rows
            for (int i = 0; i < DG.RowCount; i++)
            {
                DG[0, i].Value = i + 1;
            }
        }
        private void ClearDataGridView(DataGridView DG)
        {
            for (int i = 1; i < DG.ColumnCount; i++)
            {
                for (int j = 0; j < DG.RowCount; j++)
                {
                    DG[i, j].Value = "";
                }
            }

            return;
        }
        private void fillDGVsWithPreviousResults()
        {
            Radioactive_Source BackgroundSource = ListOfValidSources.Find(x => x.GetSerialNumber() == "Background");
            this.lbl_BGcalDate.Text = (BackgroundSource.GetAnnualCalibratedTime()).ToShortDateString();
            this.BackgroudFootDGV.Rows[0].Cells[3].Value = BackgroundSource.GetAnnualAlphaCPM();
            this.BackgroudFootDGV.Rows[0].Cells[5].Value = BackgroundSource.GetAnnualBetaCPM();

            Radioactive_Source CalibratedSource = ListOfValidSources.Find(x => x.GetSerialNumber() == this.Source_ComboBox.Text);
            this.lbl_EFFcalDate.Text = (CalibratedSource.GetAnnualCalibratedTime()).ToShortDateString();
            this.EfficiencyFootDGV.Rows[0].Cells[3].Value = CalibratedSource.GetAnnualAlphaCPM();
            this.EfficiencyFootDGV.Rows[0].Cells[5].Value = CalibratedSource.GetAnnualBetaCPM();
            this.EfficiencyFootDGV.Rows[0].Cells[6].Value = CalibratedSource.GetAlphaEfficiency();
            this.EfficiencyFootDGV.Rows[0].Cells[7].Value = CalibratedSource.GetBetaEfficiency();
        }
        public void AbortAll()
        {
            if (EL != null)
            {
                EL.RequestStop();
                while (EL.IsRunning())
                {
                    Thread.Sleep(1);
                }
            }

            if (BL != null)
            {
                BL.RequestStop();
                while (BL.IsRunning())
                {
                    Thread.Sleep(1);
                }
            }

            this.DialogResult = DialogResult.Abort;
            this.Close();
            return;
        }
        #endregion

        #region auto-Calibration Controller Functions
        public bool AutoCalibrateSourceEfficiency(object sender, List<Radioactive_Source> ListOfCalibratedSources, string SourceName)
        {
            /*Check for correct usage
            if (!(sender is QuickCalibrationController))
            {
                return false;
            }*/

            /*Accept newly calibrated source*/
            this.ListOfValidSources = ListOfCalibratedSources;

            this.Calibrating = true;

            /*Select source*/
            try
            {
                this.Source_ComboBox.Text = SourceName;
            }

            catch
            {
                return false;
            }

            /*Set counting parameters*/
            this.EFF_NumCounts_TB.Text = "10";
            this.Min_EFF_TB.Text = "1";
            this.Sec_EFF_TB.Text = "0";

            /*Click buttons*/
            Determine_Efficiency_Button_Click(this, null);
            return true;
        }

        #endregion
    }

    #region listeners
    public class BackgroundListener
    {
        #region Data Members
        //All data members are private - we don't want other classes to have the ability to modify the test once it's been started
        private bool Done;
        private DataGridView Background_Table;
        private DataGridView BackgroundFoot_Table;
        private int SampleTime;
        private DABRAS DABRAS;
        private bool ShouldStop = false;
        private bool WasBackgroundFinishedSuccessfully = false;
        private DateTime BackgroundFinished;
        private int AlphaBackground;
        private int BetaBackground;
        private int NumSamples;
        private Radioactive_Source BG;

        private int AlphaCPM;
        private int BetaCPM;

        private double AlphaDFactor;
        private double BetaDFactor;

        private bool Running = false;

        public event EventHandler BackgroundSampleThreadFinished;

        #endregion

        #region Constructor
        public BackgroundListener(DABRAS _DABRAS, int _SampleTime, int _NumSamples, DataGridView _CalTable, DataGridView _FootTable, Radioactive_Source _BGsrc)
        {
            this.DABRAS = _DABRAS;
            this.Background_Table = _CalTable;
            this.BackgroundFoot_Table = _FootTable;
            this.SampleTime = _SampleTime;
            this.NumSamples = _NumSamples;
            this.BG = _BGsrc;
        }
        #endregion

        #region Control Functions
        /*RequestStop()
         * this function will ask the thread to stop, safely terimating it as soon as possible (usually < 200ms).
         * If it is desired to halt program flow until the thread stops, call this method, then wait use thread.join() to block
         */
        public void RequestStop()
        {
            ShouldStop = true;
        }

        #endregion

        #region Getters
        /*Allow people to make copy of test results, but NOT to modify them - otherwise, we would just make everything public!*/
        public double GetBetaDecayFactor()
        {
            return this.BetaDFactor;
        }

        public double GetAlphaDecayFactor()
        {
            return this.AlphaDFactor;
        }

        public bool IsRunning()
        {
            return this.Running;
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
        #endregion

        #region Main Background Thread
        private void prepare4Start()
        {
            //Set flag for running
            this.Running = true;
            //Pause any background monitors
            this.DABRAS.Cut();

            //Set aquisition time
            //See the DABRAS Serial protocol for information about how to use this command
            this.DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            this.DABRAS.Write_To_Serial_Port(Convert.ToString(this.SampleTime));
            Thread.Sleep(500); //needed for stability

            //Clear any data left in the buffer
            this.DABRAS.ClearSerialPacket();
            this.DABRAS.ClearPacketFlag();

            //Start the watchdog - Because the serialport API doesn't give us an event for a hardware disconnection, we will need to manually watch for it
            this.DABRAS.EnableWatchdog();
        }
        private void wait4IncomingDataPacket()
        {
            //Whenever pausing for a point in time, we need to try-catch it. If the watchdog times out, then we will need to throw an exception
            try
            {
                while (!this.DABRAS.IsDataReady() && !ShouldStop)
                {
                    Thread.Sleep(100);//Allow other threads to access CPU, but keep on relatively tight leash.
                    if (!this.DABRAS.IsConnected())//this is what happens if the watchdog times out.
                    {
                        throw new TimeoutException(); //Throw an exception if the hardware detatches.
                    }
                }
            }
            catch
            {//The hardware has detatched...exit abnormally.
                MessageBox.Show("Error: Connection lost. Please re-connect communication port." + Convert.ToString(this.SampleTime));
                this.DABRAS.DisableWatchdog();
                if(this.BackgroundSampleThreadFinished != null)
                    this.BackgroundSampleThreadFinished(this, null);
                return;
            }
        }
        private void readFirstPacket()
        {//Check for the first good packet
            while (!ShouldStop)
            {              
                this.wait4IncomingDataPacket();
                
                //We got something! Read in the packet
                SerialPacket IncomingData = this.DABRAS.ReadSerialPacket();

                //Reset the watchdog timer
                this.DABRAS.KickWatchdog();

                //Break the while loop if we get a packet with the correctly set sample time, and a zero start time.
                if (IncomingData != null && IncomingData.ElTime == 0 && IncomingData.TargetTime == this.SampleTime)
                {
                    break;
                }

                if (IncomingData != null && IncomingData.ElTime > 5)
                {
                    this.DABRAS.Write_To_Serial_Port("t");
                    Thread.Sleep(250);
                    this.DABRAS.Write_To_Serial_Port(Convert.ToString(this.SampleTime));
                    this.DABRAS.Write_To_Serial_Port("g");
                }
            }
        }
        private void fillTableWithSampleData()
        {
            //Do not increment the row index until the current sample time has elapsed
            for (int i = 0; i < this.NumSamples; i++)
            {
                bool RowComplete = false;
                DABRAS.Write_To_Serial_Port("g");

                this.readFirstPacket();

                while (!RowComplete && !ShouldStop)
                {
                    this.wait4IncomingDataPacket();

                    //We made it! Read in packet and kick watchdog
                    SerialPacket IncomingData = this.DABRAS.ReadSerialPacket();
                    this.DABRAS.KickWatchdog();

                    //Grab handles to form
                    if (!ShouldStop)
                    {
                        DataGridViewCell TimeElapsed, AlphaTot, AlphaCPM, BetaTot, BetaCPM;
                        //Update the i-th row
                        TimeElapsed = Background_Table[1, i];
                        AlphaTot = Background_Table[2, i];
                        AlphaCPM = Background_Table[3, i];
                        BetaTot = Background_Table[4, i];
                        BetaCPM = Background_Table[5, i];

                        //Parse data to form
                        TimeElapsed.Value = IncomingData.ElTime;

                        if (IncomingData != null && IncomingData.ElTime != 0)
                        {
                            //Standard computations
                            AlphaTot.Value = IncomingData.AlphaTot;
                            AlphaCPM.Value = StaticMethods.RoundToDecimal(Convert.ToInt32(IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)), 1);
                            BetaTot.Value = IncomingData.BetaTot;
                            BetaCPM.Value = StaticMethods.RoundToDecimal(Convert.ToInt32(IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)), 1);

                            this.AlphaBackground = Convert.ToInt32(IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60));
                            this.BetaBackground = Convert.ToInt32(IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60));
                        }

                        /*Luckily, the DataGridView class is inherently thread-safe, so we don't need to deal with GUI callbacks as much. However, we can't force an update without risking a meltdown
                         * Use Invalidate() to ask for an update. It may take a few ms to happen, but it's fast enough that the user won't notice.
                         */
                        this.Background_Table.Invalidate();

                        //If the sample time has elapsed, increment the row.
                        if (IncomingData != null && IncomingData.ElTime >= this.SampleTime)
                        {
                            RowComplete = true;
                        }
                    }
                }
            }
        }
        private void computeResults()
        {
            if (!ShouldStop)
            {
                //Compute averages
                double AverageAlphaCPM = 0;
                double AverageBetaCPM = 0;

                var ListOfAlphaCPM = new List<double>();
                var ListOfBetaCPM = new List<double>();

                for (int i = 0; i < NumSamples; i++)
                {
                    AverageAlphaCPM += Convert.ToDouble(Background_Table[3, i].Value);
                    ListOfAlphaCPM.Add(Convert.ToDouble(Background_Table[3, i].Value));
                    AverageBetaCPM += Convert.ToDouble(Background_Table[5, i].Value);
                    ListOfBetaCPM.Add(Convert.ToDouble(Background_Table[5, i].Value));
                }

                AverageAlphaCPM /= NumSamples;
                AverageBetaCPM /= NumSamples;

                //Grab references to the datagridview and update
                DataGridViewCell AverageAlphaCell = BackgroundFoot_Table[3, 0];
                DataGridViewCell AverageBetaCell = BackgroundFoot_Table[5, 0];
                DataGridViewCell StdDevAlphaCell = BackgroundFoot_Table[3, 1];
                DataGridViewCell StdDevBetaCell = BackgroundFoot_Table[5, 1];

                //StaticMethods.RoundToSigFigs() is an efficient method to round as described in tech note 100
                //3 sig figs for a value > 100, 2 for <100
                AverageAlphaCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaCPM);
                AverageBetaCell.Value = StaticMethods.RoundToSigFigs(AverageBetaCPM);

                this.AlphaCPM = Convert.ToInt32(AverageAlphaCPM);
                this.BetaCPM = Convert.ToInt32(AverageBetaCPM);

                //Compute Standard Deviations
                double StdDevAlpha = 0;
                double StdDevBeta = 0;

                for (int i = 0; i < NumSamples; i++)
                {
                    StdDevAlpha += (AverageAlphaCPM - Convert.ToDouble(Background_Table[3, i].Value)) * (AverageAlphaCPM - Convert.ToDouble(Background_Table[3, i].Value));
                    StdDevBeta += (AverageBetaCPM - Convert.ToDouble(Background_Table[5, i].Value)) * (AverageBetaCPM - Convert.ToDouble(Background_Table[5, i].Value));
                }

                if (NumSamples > 1)
                {
                    StdDevAlpha /= (NumSamples - 1);
                    StdDevBeta /= (NumSamples - 1);
                }

                StdDevAlpha = Math.Sqrt(StdDevAlpha);
                StdDevBeta = Math.Sqrt(StdDevBeta);

                //Sub in Poisson Statistics if necessary
                if (AverageAlphaCPM < 20)
                {
                    StdDevAlpha = Math.Sqrt(Math.Abs(AverageAlphaCPM));
                }

                if (AverageBetaCPM < 20)
                {
                    StdDevBeta = Math.Sqrt(Math.Abs(AverageBetaCPM));
                }

                StdDevAlphaCell.Value = StaticMethods.RoundToSigFigs(StdDevAlpha);
                StdDevBetaCell.Value = StaticMethods.RoundToSigFigs(StdDevBeta);

                BG.SetAnnualCalibratedTimespan(this.NumSamples * this.SampleTime);

                //Compute decay factor for the daily QC chart, as per MARLAP
                this.AlphaDFactor = BG.ComputeDecayFactor(ListOfAlphaCPM, this.SampleTime);
                this.BetaDFactor = BG.ComputeDecayFactor(ListOfBetaCPM, this.SampleTime);

                //Set time finished
                this.BackgroundFinished = DateTime.Now;
                this.WasBackgroundFinishedSuccessfully = true;
            }
        }
        public void Get_Background()
        {
            this.prepare4Start();

            this.fillTableWithSampleData();

            //We are now done with the hardware. Disable the timer.
            this.DABRAS.DisableWatchdog();

            this.computeResults();

            //Play an alert sound to signal the completion
            using (SoundPlayer S = new SoundPlayer(DABRAS_Software.Properties.Resources.DoneSound))
            {
                S.Play();
            }

            /*Fire an event if someone is listening
             * The null check is needed - if the form is closed and (somehow) we get here, BackgroundSampleThreadFinished will be null.
             * This will throw an exception if we fire the event.
             */
            if (this.BackgroundSampleThreadFinished != null)
                this.BackgroundSampleThreadFinished(this, null);

            //Reinstate background monitors, if they existed
            this.DABRAS.ResumeBackgroundMonitors();
            this.Running = false;

            return;
        }
        #endregion
    }

    public class EfficiencyListener
    {
        #region Data Members
        //All data members private - We don't want other classes modifying the test after it has been created
        private bool Done;
        private DataGridView Calibration_Table;
        private DataGridView CalibrationFoot_Table;
        private DataGridView BackgroundAvg_Table;
        private int SampleTime;
        private int NumSamples;
        private DABRAS DABRAS;
        private bool ShouldStop = false;
        private Radioactive_Source R;
        private bool WasEfficiencyFinishedSuccessfully = false;
        private DateTime EfficiencyFinished;

        private double AlphaEfficiency;
        private double BetaEfficiency;
        private double AlphaNCPM;
        private double BetaNCPM;

        private double AlphaBG;
        private double BetaBG;

        private double AlphaDFactor;
        private double BetaDFactor;

        public event EventHandler EfficiencyBackgroundThreadFinished;

        private bool Running = false;
        private string radioType;
        #endregion

        #region Constructor
        public EfficiencyListener(DABRAS _DABRAS, int _SampleTime, int _NumSamples, DataGridView _CalTable, DataGridView _FootTable, DataGridView _BkgAvgTable, Radioactive_Source _R, string _Rtype)
        {
            this.DABRAS = _DABRAS;
            this.Calibration_Table = _CalTable;
            this.CalibrationFoot_Table = _FootTable;
            this.BackgroundAvg_Table = _BkgAvgTable;
            this.SampleTime = _SampleTime;
            this.NumSamples = _NumSamples;
            this.R = _R;
            this.radioType = _Rtype;
            this.SetAlphaBetaBG();
        }
        private void SetAlphaBetaBG()
        {
            int bkgRows = this.BackgroundAvg_Table.RowCount;
            this.AlphaBG = Convert.ToDouble(this.BackgroundAvg_Table.Rows[bkgRows - 2].Cells[3].Value);
            this.BetaBG = Convert.ToDouble(this.BackgroundAvg_Table.Rows[bkgRows - 2].Cells[5].Value);
        }
        #endregion

        #region Control Functions
        /*RequestStop()
         * this function will ask the thread to stop, safely terimating it as soon as possible (usually < 200ms).
         * If it is desired to halt program flow until the thread stops, call this method, then wait use thread.join() to block
         */
        public void RequestStop()
        {
            ShouldStop = true;
        }
        #endregion

        #region Getters
        //Allow people to make copy of test results, but NOT to modify them - otherwise, we would just make everything public!
        public double GetAlphaDecayFactor()
        {
            return this.AlphaDFactor;
        }

        public double GetBetaDecayFactor()
        {
            return this.BetaDFactor;
        }

        public bool IsRunning()
        {
            return this.Running;
        }

        public Radioactive_Source GetCalibratedSource()
        {
            return this.R;
        }

        public double GetAlphaEfficiency()
        {
            return this.AlphaEfficiency;
        }

        public double GetBetaEfficiency()
        {
            return this.BetaEfficiency;
        }

        public double GetAlphaNCPM()
        {
            return this.AlphaNCPM;
        }

        public double GetBetaNCPM()
        {
            return this.BetaNCPM;
        }

        public DateTime GetDateTimeCompleted()
        {
            return this.EfficiencyFinished;
        }

        public bool WasTestCompleted()
        {
            return this.WasEfficiencyFinishedSuccessfully;
        }
        #endregion

        #region Main Efficiency Thread
        private void prepare4Start()
        {
            //Set flag for running
            this.Running = true;

            //Clear any background threads, if they exist
            this.DABRAS.Cut();

            //Set aquisition time
            //See the DABRAS Serial protocol for information about how to use this command
            this.DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            this.DABRAS.Write_To_Serial_Port(Convert.ToString(this.SampleTime));
            Thread.Sleep(500); //needed for stability

            //Clear any data left in the buffer
            this.DABRAS.ClearSerialPacket();
            this.DABRAS.ClearPacketFlag();

            //Start the watchdog - Because the serialport API doesn't give us an event for a hardware disconnection, we will need to manually watch for it
            this.DABRAS.EnableWatchdog();
        }
        private void wait4IncomingDataPacket()
        {
            //Whenever pausing for a point in time, we need to try-catch it. If the watchdog times out, then we will need to throw an exception
            try
            {
                while (!this.DABRAS.IsDataReady() && !ShouldStop)
                {
                    Thread.Sleep(100);//Allow other threads to access CPU, but keep on relatively tight leash.
                    if (!this.DABRAS.IsConnected())//this is what happens if the watchdog times out.
                    {
                        throw new TimeoutException(); //Throw an exception if the hardware detatches.
                    }
                }
            }
            catch
            {//The hardware has detatched...exit abnormally.
                MessageBox.Show("Error: Connection lost. Please re-connect communication port." + Convert.ToString(this.SampleTime));
                this.DABRAS.DisableWatchdog();
                if (this.EfficiencyBackgroundThreadFinished != null)
                    this.EfficiencyBackgroundThreadFinished(this, null);
                return;
            }
        }
        private void readFirstPacket(int sampleT)
        {//Check for the first good packet
            while (!ShouldStop)
            {
                this.wait4IncomingDataPacket();

                //We got a packet! Read in it.
                SerialPacket IncomingData = this.DABRAS.ReadSerialPacket();

                //Reset the watchdog timer
                this.DABRAS.KickWatchdog();

                //Break the while loop (we are good to go) if we get a packet with the correctly set sample time, and a zero start time (the elapsed time is zero).
                if (IncomingData != null && IncomingData.ElTime == 0 && IncomingData.TargetTime == sampleT)
                {
                    break;
                }

                if (IncomingData != null && IncomingData.ElTime > 5)
                {
                    this.DABRAS.Write_To_Serial_Port("t");
                    Thread.Sleep(250);
                    this.DABRAS.Write_To_Serial_Port(Convert.ToString(sampleT));
                }
            }
        }
        private void fillTableWithSampleData()
        {
            //local variables
            int sample_time;
            double ExpectedDPM = Convert.ToDouble(R.GetCertifiedActivity()) * this.R.GetDecayFactor(DateTime.Now);

            for (int i = 0; i <= this.NumSamples; i++)//changed from i < NumSamples so that the last one counting can be used for the mandatory 1 minute counting
            {
                if (i < this.NumSamples)
                    sample_time = this.SampleTime;
                else
                {
                    sample_time = 60;//60 seconds

                    //Re-set aquisition time
                    //See the DABRAS Serial protocol for information about how to use this command
                    this.DABRAS.Write_To_Serial_Port("t");
                    Thread.Sleep(250);
                    this.DABRAS.Write_To_Serial_Port(Convert.ToString(sample_time));
                    Thread.Sleep(500); //needed for stability

                    //Clear any data left in the buffer
                    this.DABRAS.ClearSerialPacket();
                    this.DABRAS.ClearPacketFlag();
                }

                bool RowComplete = false;
                this.DABRAS.Write_To_Serial_Port("g");

                //Check for the first good packet
                this.readFirstPacket(sample_time);

                //Do not increment the row index until the current sample time has elapsed
                while (!RowComplete && !ShouldStop)
                {
                    this.wait4IncomingDataPacket();
                    
                    //We made it! Read in packet and kick watchdog
                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();
                    this.DABRAS.KickWatchdog();

                    //Grab handles to form
                    if (!ShouldStop)
                    {
                        DataGridViewCell TimeElapsed, AlphaTot, AlphaNCPM, BetaTot, BetaNCPM, AlphaEff, BetaEff;
                        if (i == NumSamples)
                        {//Update the i+2-th row, skip the Average and the StdDev rows
                            TimeElapsed = CalibrationFoot_Table[1, 2];
                            AlphaTot = CalibrationFoot_Table[2, 2];
                            AlphaNCPM = CalibrationFoot_Table[3, 2];
                            BetaTot = CalibrationFoot_Table[4, 2];
                            BetaNCPM = CalibrationFoot_Table[5, 2];
                            AlphaEff = CalibrationFoot_Table[6, 2];
                            BetaEff = CalibrationFoot_Table[7, 2];
                        }
                        else
                        {//Update the i-th row
                            TimeElapsed = Calibration_Table[1, i];
                            AlphaTot = Calibration_Table[2, i];
                            AlphaNCPM = Calibration_Table[3, i];
                            BetaTot = Calibration_Table[4, i];
                            BetaNCPM = Calibration_Table[5, i];
                            AlphaEff = Calibration_Table[6, i];
                            BetaEff = Calibration_Table[7, i];
                        }

                        //Parse data to form
                        TimeElapsed.Value = IncomingData.ElTime;
                        if (IncomingData != null && IncomingData.ElTime != 0)
                        {
                            //Standard computations
                            AlphaTot.Value = IncomingData.AlphaTot;
                            AlphaNCPM.Value = StaticMethods.RoundToDecimal(IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60) - AlphaBG, 1); //Convert raw counts into CPM. +1 to avoid DBZ, PIC uses zero based counting.
                            BetaTot.Value = IncomingData.BetaTot;
                            BetaNCPM.Value = StaticMethods.RoundToDecimal(IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60) - BetaBG, 1);
                            //efficiencies are ALWAYS three sig figs!
                            AlphaEff.Value = StaticMethods.RoundToSpecificSigFigs((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60) - AlphaBG) * 100 / ExpectedDPM, 3);
                            BetaEff.Value = StaticMethods.RoundToSpecificSigFigs((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60) - BetaBG) * 100 / ExpectedDPM, 3);

                        }
                        //Re-draw table
                        this.Calibration_Table.Invalidate();

                        //Kick the dog - reset the watchdog timer
                        this.DABRAS.KickWatchdog();

                        if (IncomingData != null && (IncomingData.ElTime >= sample_time))
                        {
                            RowComplete = true; //move to the next row when this test finishes.
                        }
                    }
                }
            }
        }
        private void computeResults()
        {
            if (!ShouldStop)
            {
                //Compute averages
                double AverageAlphaNCPM = 0;
                double AverageBetaNCPM = 0;
                double AverageAlphaEff = 0;
                double AverageBetaEff = 0;

                //Make lists for decay factor calculation
                var ListOfAlphaNCPM = new List<double>();
                var ListOfBetaNCPM = new List<double>();

                for (int i = 0; i < NumSamples; i++)
                {
                    AverageAlphaNCPM += Convert.ToDouble(Calibration_Table[3, i].Value);
                    ListOfAlphaNCPM.Add(Convert.ToDouble(Calibration_Table[3, i].Value));

                    AverageBetaNCPM += Convert.ToDouble(Calibration_Table[5, i].Value);
                    ListOfBetaNCPM.Add(Convert.ToDouble(Calibration_Table[5, i].Value));

                    AverageAlphaEff += Convert.ToDouble(Calibration_Table[6, i].Value);
                    AverageBetaEff += Convert.ToDouble(Calibration_Table[7, i].Value);
                }

                AverageAlphaNCPM /= NumSamples;
                AverageBetaNCPM /= NumSamples;
                AverageAlphaEff /= NumSamples;
                AverageBetaEff /= NumSamples;

                DataGridViewCell AverageAlphaCell = CalibrationFoot_Table[3, 0];
                DataGridViewCell AverageBetaCell = CalibrationFoot_Table[5, 0];
                DataGridViewCell AverageAlphaEffCell = CalibrationFoot_Table[6, 0];
                DataGridViewCell AverageBetaEffCell = CalibrationFoot_Table[7, 0];
                DataGridViewCell StdDevAlphaCell = CalibrationFoot_Table[6, 1];
                DataGridViewCell StdDevBetaCell = CalibrationFoot_Table[7, 1];

                AverageAlphaCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaNCPM);
                AverageBetaCell.Value = StaticMethods.RoundToSigFigs(AverageBetaNCPM);
                AverageAlphaEffCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaEff);
                AverageBetaEffCell.Value = StaticMethods.RoundToSigFigs(AverageBetaEff);

                this.AlphaEfficiency = AverageAlphaEff;
                this.BetaEfficiency = AverageBetaEff;
                this.AlphaNCPM = AverageAlphaNCPM;
                this.BetaNCPM = AverageBetaNCPM;

                //Compute Standard Deviations
                double StdDevAlpha = 0;
                double StdDevBeta = 0;
                for (int i = 0; i < NumSamples; i++)
                {
                    StdDevAlpha += (AverageAlphaEff - Convert.ToDouble(Calibration_Table[6, i].Value)) * (AverageAlphaEff - Convert.ToDouble(Calibration_Table[6, i].Value));
                    StdDevBeta += (AverageBetaEff - Convert.ToDouble(Calibration_Table[7, i].Value)) * (AverageBetaEff - Convert.ToDouble(Calibration_Table[7, i].Value));
                }

                if (NumSamples > 1)
                {
                    StdDevAlpha /= (NumSamples - 1);
                    StdDevBeta /= (NumSamples - 1);
                    StdDevAlpha = Math.Sqrt(StdDevAlpha);
                    StdDevBeta = Math.Sqrt(StdDevBeta);
                }
                else
                {
                    StdDevAlpha = Math.Sqrt(AverageAlphaNCPM);
                    StdDevBeta = Math.Sqrt(AverageBetaNCPM);
                }
                /*
                //Sub in Poisson Statistics if necessary
                if (AverageAlphaNCPM < 20)
                {
                    StdDevAlpha = Math.Sqrt(Math.Abs(AverageAlphaNCPM));
                }

                if (AverageBetaNCPM < 20)
                {
                    StdDevBeta = Math.Sqrt(Math.Abs(AverageBetaNCPM));
                }
                */
                StdDevAlphaCell.Value = StaticMethods.RoundToSigFigs(StdDevAlpha);
                StdDevBetaCell.Value = StaticMethods.RoundToSigFigs(StdDevBeta);

                //Compute Decay Value
                this.AlphaDFactor = this.R.ComputeDecayFactor(ListOfAlphaNCPM, this.SampleTime);
                this.BetaDFactor = this.R.ComputeDecayFactor(ListOfBetaNCPM, this.SampleTime);

                //Fill the last rows with the mandatory 1 minute count differences from the average
                double OneMinuteA = 0.05, OneMinuteB = 0.05, OneMinuteAeff = 0.05, OneMinuteBeff = 0.05;
                if (AverageAlphaNCPM != 0)
                {
                    OneMinuteA = (Convert.ToDouble(this.CalibrationFoot_Table[3, 2].Value) - AverageAlphaNCPM) / AverageAlphaNCPM;
                    this.CalibrationFoot_Table[3, 3].Value = OneMinuteA * 100;
                }
                if (AverageBetaNCPM != 0)
                {
                    OneMinuteB = (Convert.ToDouble(this.CalibrationFoot_Table[5, 2].Value) - AverageBetaNCPM) / AverageBetaNCPM;
                    this.CalibrationFoot_Table[5, 3].Value = OneMinuteB * 100;
                }
                if (AverageAlphaEff != 0)
                {
                    OneMinuteAeff = (Convert.ToDouble(this.CalibrationFoot_Table[6, 2].Value) - AverageAlphaEff) / AverageAlphaEff;
                    this.CalibrationFoot_Table[6, 3].Value = OneMinuteAeff * 100;
                }
                if (AverageBetaEff != 0)
                {
                    OneMinuteBeff = (Convert.ToDouble(this.CalibrationFoot_Table[7, 2].Value) - AverageBetaEff) / AverageBetaEff;
                    this.CalibrationFoot_Table[7, 3].Value = OneMinuteBeff * 100;
                }

                if (((this.radioType == "Alpha" && Math.Abs(OneMinuteA * 100) < 5.0 && Math.Abs(OneMinuteAeff * 100) < 5.0))
                    || ((this.radioType == "Beta" && Math.Abs(OneMinuteB * 100) < 5.0 && Math.Abs(OneMinuteBeff * 100) < 5.0)))
                {
                    //Set efficiency finished field
                    this.EfficiencyFinished = DateTime.Now;

                    //Set the finished property
                    this.WasEfficiencyFinishedSuccessfully = true;
                }
                else
                    this.WasEfficiencyFinishedSuccessfully = false;
            }
        }
        public void Get_Efficiency()
        {
            this.prepare4Start();

            this.fillTableWithSampleData();

            //We are now done with the hardware, diable the timer
            this.DABRAS.DisableWatchdog();

            this.computeResults();

            /*Play an alert sound to signal the completion.
             * Note the use of the using() block - avoid memory leaks!*/
            using (SoundPlayer S = new SoundPlayer(DABRAS_Software.Properties.Resources.DoneSound))
            {
                S.Play();
            }

            /*Fire an event if someone is listening
             * The null check is needed - if the form is closed and (somehow) we get here, BackgroundSampleThreadFinished will be null.
             * This will throw an exception if we fire the event.
             */
            if (this.EfficiencyBackgroundThreadFinished != null)
                this.EfficiencyBackgroundThreadFinished(this, null);

            //Reinstate background threads, if they existed
            this.DABRAS.ResumeBackgroundMonitors();
            this.Running = false;
            return;
        }
        #endregion
    }

    #endregion
}
