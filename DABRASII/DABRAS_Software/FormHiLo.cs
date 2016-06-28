using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Media;
using System.Drawing.Imaging;

namespace DABRAS_Software
{
    public partial class FormHiLo : Form
    {
        #region Enums
        public enum TypeOfHiLo { BACKGROUND, ALPHA, BETA };
        public enum LimitType { STDEV, PERCENT };
        #endregion

        #region Data Members
        private LimitType TypeOfLimit;
        private double LimitValue;
        private TypeOfHiLo Type;
        private DABRAS DABRAS;
        private Logger Logger;
        private readonly FormCLB LaunchedFrom;
        private bool NewDABRAS = false;
        private DefaultConfigurations DC;

        private HiLoListener HL = null;
        private List<Radioactive_Source> ListOfSources;
        private List<RadionuclideFamily> ListOfFamily;
        private bool ModifiedSourceList = false;

        private double AlphaLL;
        private double AlphaUL;
        private double BetaLL;
        private double BetaUL;
        private double AlphaAvg;
        private double BetaAvg;

        private int TotalSampleTime = 1;

        private bool AskSave = false;

        private bool Calibrating = false;

        private int NumSamples = 0;

        private string[,] bkgrdData, alphaData, betaData;

        public delegate void UpdateLimitsCallback();
        #endregion

        #region Constructor
        public FormHiLo(FormCLB _CF)
        {
            this.LaunchedFrom = _CF;
            this.DABRAS = LaunchedFrom.GetDABRAS();

            if (!this.DABRAS.IsConnected())
            {
                this.endFormActivities();
                this.LaunchedFrom.endFormActivities();
                this.notifyParentForm();

                MessageBox.Show("Error: Must be connected to the DABRAS to perform a Hi/Lo calibration. Please re-connect and try again.");
                this.Calibrating = false;
                this.Dispose();
                return;
            }

            InitializeComponent();

            this.Type = TypeOfHiLo.BACKGROUND;
            this.rdBtnBackground.Checked = true;
            this.Logger = LaunchedFrom.GetLogger();
            this.ListOfSources = LaunchedFrom.GetSourceList();
            this.ListOfFamily = LaunchedFrom.GetFamilyList();
            this.DC = LaunchedFrom.GetDefaultConfig();
            this.bkgrdData = null;
            this.betaData = null;
            this.alphaData = null;

            try
            {
                this.NumSamples = Convert.ToInt32(this.Num_Counts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Bad sample count.");
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }

            this.setFormByType();

            //.KeyPreview = true;
            //this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Aquire Button Handler
        private void AquireButton_Click(object sender, EventArgs e)
        {
            int NumberOfSamples = 0;
            int SampleTime = 0;
            try
            {
                NumberOfSamples = Convert.ToInt32(this.Num_Counts_TB.Text);
                SampleTime = (Convert.ToInt32(this.Min_TB.Text) * 60) + Convert.ToInt32(this.Sec_TB.Text);
                Math.Sqrt(SampleTime);
                if(NumSamples <= 0 || SampleTime <= 0)
                {
                    MessageBox.Show("Please enter positive numbers for samples and sampling time.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Error: Bad Time Values.");
                return;
            }

            if (this.StdDevButton.Checked)
            {
                this.TypeOfLimit = LimitType.STDEV;
                try
                {
                    this.LimitValue = Convert.ToDouble(this.StdDev_TB.Text);
                }
                catch
                {
                    MessageBox.Show("Error: Bad STDEV value");
                    return;
                }
            }
            else
            {
                this.TypeOfLimit = LimitType.PERCENT;
                try
                {
                    this.LimitValue = Convert.ToDouble(this.Percent_TB.Text);
                }
                catch
                {
                    MessageBox.Show("Error: Bad Percentage Value");
                    return;
                }
            }

            if (!this.DABRAS.IsConnected())
            {
                this.endFormActivities();
                this.LaunchedFrom.endFormActivities();
                this.notifyParentForm();

                MessageBox.Show("Error: Must be connected to the DABRAS to perform a Hi/Lo calibration. Please re-connect and try again.");
                this.Calibrating = false;
                return;
            }

            if (!Calibrating)
            {
                if (MessageBox.Show(String.Format("Count {0} times for {1}:{2:00}?", NumberOfSamples, (SampleTime / 60), (SampleTime % 60)), "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    MessageBox.Show("Aborted.");
                    return;
                }
            }

            this.ClearDataGridView(this.HiLo_Results_DataGridView);

            Radioactive_Source BG = ListOfSources.Find(x => x.GetSerialNumber() == "Background");

            this.HL = new HiLoListener(this.DABRAS, SampleTime, NumberOfSamples, this.HiLo_Results_DataGridView, this.Type, this.TypeOfLimit, this.LimitValue, BG.GetAnnualAlphaCPM(), BG.GetAnnualBetaCPM());

            Thread BackgroundThread = new Thread(() => this.HL.Get_HiLo());
            BackgroundThread.Start();

            this.HL.BackgroundThreadFinished += (s, args) => { InvokeCallback(); };

            this.ToggleGUI(true);
        }
        #endregion

        #region Stop Button Handler
        private void StopButton_Click(object sender, EventArgs e)
        {
            if (HL != null)
            {
                HL.RequestStop();
            }

            ToggleGUI(false);

        }
        #endregion   

        #region SaveHiLoLimits
        private void SaveHiLoLimits()
        {
            //Find radionuclide family and update its values
            string NameOfFamily = this.GetGridViewString();

            RadionuclideFamily rFamily = ListOfFamily.Find(x => x.GetName() == NameOfFamily);

            double AlphaHi = Convert.ToDouble(this.HL_Alpha_TB.Text);
            double AlphaLo = Convert.ToDouble(this.LL_Alpha_TB.Text);
            double BetaHi = Convert.ToDouble(HL_Beta_TB.Text);
            double BetaLo = Convert.ToDouble(LL_Beta_TB.Text);

            //Swap the values such that the hi is always greater than the low
            if (AlphaLo > AlphaHi)
            {
                double T = AlphaLo;
                AlphaLo = AlphaHi;
                AlphaHi = T;
            }

            if (BetaLo > BetaHi)
            {
                double T = BetaLo;
                BetaLo = BetaHi;
                BetaHi = T;
            }

            if (rFamily != null)
            {
                RadionuclideFamily.RadiationType RF_type = rFamily.GetSourceType();
                if ((RF_type == RadionuclideFamily.RadiationType.Alpha) || (RF_type == RadionuclideFamily.RadiationType.AlphaBeta))
                {
                    if ((this.HL_Alpha_TB.Text != "") && (this.LL_Alpha_TB.Text != ""))
                    {
                        try
                        {
                            rFamily.SetAlphaHi(AlphaHi);
                            rFamily.SetAlphaLo(AlphaLo);
                            rFamily.SetAlphaHiLoAvg(this.AlphaAvg);
                            rFamily.SetAnnualAlphaCPM(this.AlphaAvg);
                            rFamily.SetHiLoCalibratedDate(DateTime.Now);
                            rFamily.SetAnnualCalibratedTimespan(this.TotalSampleTime);
                        }
                        catch
                        {
                            MessageBox.Show("Error: Bad values");
                            return;
                        }
                    }
                }

                if ((RF_type == RadionuclideFamily.RadiationType.Beta) || (RF_type == RadionuclideFamily.RadiationType.AlphaBeta))
                {
                    if ((this.HL_Beta_TB.Text != "") && (this.LL_Beta_TB.Text != ""))
                    {
                        try
                        {
                            rFamily.SetBetaHi(BetaHi);
                            rFamily.SetBetaLo(BetaLo);
                            rFamily.SetBetaHiLoAvg(this.BetaAvg);
                            rFamily.SetAnnualBetaCPM(this.BetaAvg);
                            rFamily.SetHiLoCalibratedDate(DateTime.Now);
                            rFamily.SetAnnualCalibratedTimespan(this.TotalSampleTime);
                        }
                        catch
                        {
                            MessageBox.Show("Error: Bad values.");
                            return;
                        }
                    }
                }

                this.AlphaUL = AlphaHi;
                this.AlphaLL = AlphaLo;
                this.BetaUL = BetaHi;
                this.BetaLL = BetaLo;

                this.LaunchedFrom.updFamilyAndSource(this.ListOfFamily, this.ListOfSources);
                this.LaunchedFrom.updParentFamilyAndSource(this.ListOfFamily, this.ListOfSources);
                this.lbl_HiLocalDate.Text = (DateTime.Now).ToShortDateString();
                MessageBox.Show("Limits saved.");
                this.AskSave = false;
            }
        }
        #endregion

        #region Save Image Handler
        private void ImageSaveButton_Click(object sender, EventArgs e)
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

        #region Recompute Limits Handler
        private void Recompute_Limits_Button_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.StdDevButton.Checked)
                {
                    this.LimitValue = Convert.ToDouble(this.StdDev_TB.Text);
                }
                if (this.PercentButton.Checked)
                {
                    this.LimitValue = Convert.ToDouble(this.Percent_TB.Text);
                }
            }
            catch
            {
                MessageBox.Show("Error: Bad values.");
                return;
            }

            //Compute Averages, Standard Deviations and HiLo limits
            double AverageAlphaCPM = 0;
            double AverageBetaCPM = 0;

            try
            {
                //1)Averages
                for (int i = 0; i < HiLo_Results_DataGridView.RowCount - 3; i++)
                {
                    AverageAlphaCPM += Convert.ToDouble(HiLo_Results_DataGridView[3, i].Value);
                    AverageBetaCPM += Convert.ToDouble(HiLo_Results_DataGridView[5, i].Value);
                }

                AverageAlphaCPM /= (HiLo_Results_DataGridView.RowCount - 3);
                AverageBetaCPM /= (HiLo_Results_DataGridView.RowCount - 3);

                DataGridViewCell AverageAlphaCell = HiLo_Results_DataGridView[3, (HiLo_Results_DataGridView.RowCount - 3)];
                DataGridViewCell AverageBetaCell = HiLo_Results_DataGridView[5, (HiLo_Results_DataGridView.RowCount - 3)];
                DataGridViewCell StdDevAlphaCell = HiLo_Results_DataGridView[3, (HiLo_Results_DataGridView.RowCount - 3) + 1];
                DataGridViewCell StdDevBetaCell = HiLo_Results_DataGridView[5, (HiLo_Results_DataGridView.RowCount - 3) + 1];


                AverageAlphaCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaCPM);
                AverageBetaCell.Value = StaticMethods.RoundToSigFigs(AverageBetaCPM);

                //2)Standard Deviations
                double StdDevAlpha = 0;
                double StdDevBeta = 0;
                for (int i = 0; i < (HiLo_Results_DataGridView.RowCount - 3); i++)
                {
                    StdDevAlpha += (AverageAlphaCPM - Convert.ToDouble(HiLo_Results_DataGridView[3, i].Value)) * (AverageAlphaCPM - Convert.ToDouble(HiLo_Results_DataGridView[3, i].Value));
                    StdDevBeta += (AverageBetaCPM - Convert.ToDouble(HiLo_Results_DataGridView[5, i].Value)) * (AverageBetaCPM - Convert.ToDouble(HiLo_Results_DataGridView[5, i].Value));
                }

                if ((HiLo_Results_DataGridView.RowCount - 3) > 1)
                {
                    StdDevAlpha /= ((HiLo_Results_DataGridView.RowCount - 2) - 1);
                    StdDevBeta /= ((HiLo_Results_DataGridView.RowCount - 2) - 1);
                    StdDevAlpha = Math.Sqrt(StdDevAlpha);
                    StdDevBeta = Math.Sqrt(StdDevBeta);
                }
                else
                {
                    StdDevAlpha = Math.Sqrt(AverageAlphaCPM);
                    StdDevBeta = Math.Sqrt(AverageBetaCPM);
                }

                //Allow for the usage of Poisson statistics
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

                //3)HiLo limits
                double AlphaUpperLimit = 0;
                double AlphaLowerLimit = 0;
                double BetaUpperLimit = 0;
                double BetaLowerLimit = 0;

                if (this.TypeOfLimit == FormHiLo.LimitType.STDEV)
                {
                    //Compute Betas for all except the Alpha
                    if (this.Type != FormHiLo.TypeOfHiLo.ALPHA)
                    {
                        BetaLowerLimit = AverageBetaCPM - (this.LimitValue * StdDevBeta);
                        BetaUpperLimit = AverageBetaCPM + (this.LimitValue * StdDevBeta);
                    }

                    //Compute Alphas for all except the Beta
                    if (this.Type != FormHiLo.TypeOfHiLo.BETA)
                    {
                        AlphaLowerLimit = AverageAlphaCPM - (this.LimitValue * StdDevAlpha);
                        AlphaUpperLimit = AverageAlphaCPM + (this.LimitValue * StdDevAlpha);
                    }
                }
                else
                {
                    //Compute Betas for all except the Alpha
                    if (this.Type != FormHiLo.TypeOfHiLo.ALPHA)
                    {
                        BetaLowerLimit = AverageBetaCPM - (this.LimitValue * 0.01 * AverageBetaCPM);
                        BetaUpperLimit = AverageBetaCPM + (this.LimitValue * 0.01 * AverageBetaCPM);
                    }

                    //Compute Alphas for all except the Beta
                    if (this.Type != FormHiLo.TypeOfHiLo.BETA)
                    {
                        AlphaLowerLimit = AverageAlphaCPM - (this.LimitValue * 0.01 * AverageAlphaCPM);
                        AlphaUpperLimit = AverageAlphaCPM + (this.LimitValue * 0.01 * AverageAlphaCPM);
                    }
                }

                this.AlphaAvg = AverageAlphaCPM;
                this.BetaAvg = AverageBetaCPM;

                this.HL_Alpha_TB.Text = StaticMethods.RoundToSigFigs(AlphaUpperLimit);
                this.AlphaUL = Convert.ToDouble(StaticMethods.RoundToSigFigs(AlphaUpperLimit));
                this.LL_Alpha_TB.Text = StaticMethods.RoundToSigFigs(AlphaLowerLimit);
                this.AlphaLL = Convert.ToDouble(StaticMethods.RoundToSigFigs(AlphaLowerLimit));
                this.HL_Beta_TB.Text = StaticMethods.RoundToSigFigs(BetaUpperLimit);
                this.BetaUL = Convert.ToDouble(StaticMethods.RoundToSigFigs(BetaUpperLimit));
                this.LL_Beta_TB.Text = StaticMethods.RoundToSigFigs(BetaLowerLimit);
                this.BetaLL = Convert.ToDouble(StaticMethods.RoundToSigFigs(BetaLowerLimit));

                this.SaveHiLoLimits();
            }
            catch(Exception le)
            {
                MessageBox.Show("Error occurred: " + le.Message);
            }
            MessageBox.Show("Done!");
            return;
        }
        #endregion

        #region Misc. GUI Functions
        private void Num_Counts_TB_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.NumSamples = Convert.ToInt32(this.Num_Counts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Counts must be an integer greater than zero. Please try again.");
                return;
            }
            this.ClearDataGridView(this.HiLo_Results_DataGridView);
            this.Add_Or_Subtract_Rows(this.NumSamples, this.HiLo_Results_DataGridView);
            this.HiLo_Results_DataGridView.Invalidate();
        }

        private void StdDevButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.StdDevButton.Checked)
            {
                this.TypeOfLimit = LimitType.STDEV;
            }
        }

        private void PercentButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.PercentButton.Checked)
            {
                this.TypeOfLimit = LimitType.PERCENT;
            }
        }

        private void Override_CB_CheckedChanged(object sender, EventArgs e)
        {
            this.HL_Alpha_TB.Enabled = this.Override_CB.Checked;
            this.LL_Alpha_TB.Enabled = this.Override_CB.Checked;
            this.HL_Beta_TB.Enabled = this.Override_CB.Checked;
            this.LL_Beta_TB.Enabled = this.Override_CB.Checked;

            return;
        }

        private void ToggleGUI(bool TestRunning)
        {
            this.AquireButton.Enabled = !TestRunning;
            this.Recompute_Limits_Button.Enabled = !TestRunning;
            this.Override_CB.Enabled = !TestRunning;

            this.StopButton.Enabled = TestRunning;

            this.Num_Counts_TB.Enabled = !TestRunning;
            this.Min_TB.Enabled = !TestRunning;
            this.Sec_TB.Enabled = !TestRunning;
            this.StdDev_TB.Enabled = !TestRunning;
            this.Percent_TB.Enabled = !TestRunning;

            if (this.PercentButton.Checked)
            {
                this.StdDevButton.Enabled = !TestRunning;
                this.PercentButton.Enabled = !TestRunning;
            }
            else
            {
                this.PercentButton.Enabled = !TestRunning;
                this.StdDevButton.Enabled = !TestRunning;
            }

            this.Recompute_Limits_Button.Enabled = !TestRunning;

            return;
        }
        #endregion

        #region Private Utility Functions
        private void Add_Or_Subtract_Rows(int FinalNumberOfRows, DataGridView DG)
        {
            FinalNumberOfRows += 2;
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
                    DG.Rows.Insert(0, "", "", "", "", "", "", "0", "0");
                }
            }

            //Re-number the rows
            for (int i = 0; i < DG.RowCount - 2; i++)
            {
                DG[0, i].Value = i + 1;
            }
        }
        
        private void Make_HiLo_GridView(int numSamples)
        {
            this.HiLo_Results_DataGridView.DataSource = null;
            this.HiLo_Results_DataGridView.Columns.Clear();
            this.HiLo_Results_DataGridView.Rows.Clear();
            switch (this.Type)
            {
                case TypeOfHiLo.BACKGROUND:
                    //this.TypeOfHiLoLabel.Text = "Background Alpha and Beta Radiation";
                    this.HiLo_Results_DataGridView.Columns.Add("Calibration_Source_Name", "Background");
                    break;
                case TypeOfHiLo.ALPHA:
                    //this.TypeOfHiLoLabel.Text = "Alpha: Am-241";
                    this.HiLo_Results_DataGridView.Columns.Add("Calibration_Source_Name", "Am-241");
                    break;
                case TypeOfHiLo.BETA:
                    //this.TypeOfHiLoLabel.Text = "Beta: Sr-90";
                    this.HiLo_Results_DataGridView.Columns.Add("Calibration_Source_Name", "Sr-90");
                    break;
                default:
                    this.HiLo_Results_DataGridView.Columns.Add("Calibration_Source_Name", "Background");
                    break;
            }

            this.HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Acq_Time", "Acq Time");
            this.HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Alpha_Count", "Alpha Count");
            if (this.Type != TypeOfHiLo.BACKGROUND)
            {
                this.HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Alpha_CPM", "Alpha NCPM");
            }
            else
            {
                this.HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Alpha_CPM", "Alpha GCPM");
            }
            this.HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Beta_Count", "Beta Count");

            if (this.Type != TypeOfHiLo.BACKGROUND)
            {
                this.HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Beta_CPM", "Beta NCPM");
            }
            else
            {
                this.HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Beta_CPM", "Beta GCPM");

            }

            for (int col_i = 0; col_i < this.HiLo_Results_DataGridView.Columns.Count; col_i++)
            {
                DataGridViewColumn column = this.HiLo_Results_DataGridView.Columns[col_i];
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                column.ValueType = typeof(int);
            }

            //Add one row for each count
            for (int i = 0; i < numSamples; i++)
            {
                this.HiLo_Results_DataGridView.Rows.Add(new object[] { i + 1, "", "", "", "", "", "", "" });
            }

            this.HiLo_Results_DataGridView.Rows.Add(new object[] { "Average", "", "", "", "", "", "", "" });
            this.HiLo_Results_DataGridView.Rows.Add(new object[] { "StDev", "", "", "", "", "", "", "" });

            return; 
        }

        private void setFormByType()
        {
            switch (this.Type)
            {
                case TypeOfHiLo.BACKGROUND:
                    this.Min_TB.Text = "2"; //account for different counting time
                    RadionuclideFamily bkgF = ListOfFamily.Find(x => x.GetName() == "Background");

                    this.lbl_HiLocalDate.Text = (bkgF.GetHiLoCalibratedTime()).ToShortDateString();
                    this.Net_CPM_Label.Text = "Gross CPM";
                    this.Net_CPM_Label2.Text = "Gross CPM";

                    this.HL_Alpha_Label.Enabled = true;
                    this.LL_Alpha_Label.Enabled = true;
                    this.HL_Alpha_TB.Enabled = true;
                    this.HL_Alpha_TB.Text = bkgF.GetAlphaHi().ToString();
                    this.LL_Alpha_TB.Enabled = true;
                    this.LL_Alpha_TB.Text = bkgF.GetAlphaLo().ToString();

                    this.HL_Beta_Label.Enabled = true;
                    this.LL_Beta_Label.Enabled = true;
                    this.HL_Beta_TB.Enabled = true;
                    this.HL_Beta_TB.Text = bkgF.GetBetaHi().ToString();
                    this.LL_Beta_TB.Enabled = true;
                    this.LL_Beta_TB.Text = bkgF.GetBetaLo().ToString();
                    break;
                case TypeOfHiLo.ALPHA:
                    this.Min_TB.Text = "1";
                    RadionuclideFamily aF = ListOfFamily.Find(x => x.GetName() == "Am-241");

                    this.lbl_HiLocalDate.Text = (aF.GetHiLoCalibratedTime()).ToShortDateString();
                    this.HL_Alpha_Label.Enabled = true;
                    this.HL_Alpha_TB.Enabled = true;
                    this.HL_Alpha_TB.Text = aF.GetAlphaHi().ToString();
                    this.LL_Alpha_Label.Enabled = true;
                    this.LL_Alpha_TB.Enabled = true;
                    this.LL_Alpha_TB.Text = aF.GetAlphaLo().ToString();

                    this.HL_Beta_Label.Enabled = false;
                    this.HL_Beta_TB.Enabled = false;
                    this.LL_Beta_TB.Enabled = false;
                    this.LL_Beta_Label.Enabled = false;

                    this.Net_CPM_Label.Text = "Net CPM";
                    this.Net_CPM_Label2.Text = "Net CPM";
                    break;
                case TypeOfHiLo.BETA:
                    this.Min_TB.Text = "1";
                    RadionuclideFamily bF = ListOfFamily.Find(x => x.GetName() == "Sr-90");

                    this.lbl_HiLocalDate.Text = (bF.GetHiLoCalibratedTime()).ToShortDateString();
                    this.HL_Alpha_Label.Enabled = false;
                    this.HL_Alpha_TB.Enabled = false;
                    this.LL_Alpha_TB.Enabled = false;
                    this.LL_Alpha_Label.Enabled = false;

                    this.HL_Beta_Label.Enabled = true;
                    this.HL_Beta_TB.Enabled = true;
                    this.HL_Alpha_TB.Text = bF.GetAlphaHi().ToString();
                    this.LL_Beta_Label.Enabled = true;
                    this.LL_Alpha_TB.Enabled = true;
                    this.LL_Alpha_TB.Text = bF.GetAlphaLo().ToString();

                    this.Net_CPM_Label.Text = "Net CPM";
                    this.Net_CPM_Label2.Text = "Net CPM";
                    break;
            }

            if (this.Type == TypeOfHiLo.BACKGROUND && this.bkgrdData != null)
            {
                this.FillHiLoGridWithData(bkgrdData);
            }
            else if (this.Type == TypeOfHiLo.BETA && this.betaData != null)
            {
                this.FillHiLoGridWithData(betaData);
            }
            else if (this.Type == TypeOfHiLo.ALPHA && this.alphaData != null)
            {
                this.FillHiLoGridWithData(alphaData);
            }
            else
                this.Make_HiLo_GridView(this.NumSamples);
        }

        private void setTypeByRadioButtons()
        {
            if (this.rdBtnBackground.Checked)
                this.Type = TypeOfHiLo.BACKGROUND;
            else if (this.rdBtnAlpha.Checked)
                this.Type = TypeOfHiLo.ALPHA;
            else if (this.rdBtnBeta.Checked)
                this.Type = TypeOfHiLo.BETA;
            else
            {
                this.rdBtnBackground.Checked = true;
                this.Type = TypeOfHiLo.BACKGROUND;
            }
        }

        private string GetGridViewString()
        {
            if (this.Type == TypeOfHiLo.ALPHA)
            {
                return "Am-241";
            }
            if (this.Type == TypeOfHiLo.BETA)
            {
                return "Sr-90";
            }
            if (this.Type == TypeOfHiLo.BACKGROUND)
            {
                return "Background";
            }
            else
            {
                return "??";
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

        private string[,] MakeDataWritable(DataGridView DG)
        {
            int Rows = DG.RowCount + 3;
            int Cols = DG.ColumnCount;

            string[,] ReturnString = new string[Rows, Cols];

            foreach (DataGridViewColumn c in DG.Columns)
            {
                ReturnString[0, c.Index] = c.HeaderText;
            }

            for (int i = 1; i < DG.RowCount + 1; i++)
            {
                for (int j = 0; j < DG.ColumnCount; j++)
                {
                    ReturnString[i, j] = "";
                    ReturnString[i, j] = Convert.ToString(DG[j, i - 1].Value);
                }
            }

            ReturnString[Rows - 2, 0] = "Upper Alpha Limit:";
            ReturnString[Rows - 2, 2] = "Lower Alpha Limit:";

            if (this.Type != TypeOfHiLo.BETA)
            {
                ReturnString[Rows - 2, 1] = Convert.ToString(this.HL_Alpha_TB.Text);
                ReturnString[Rows - 2, 3] = Convert.ToString(this.LL_Alpha_TB.Text);
            }
            else
            {              
                ReturnString[Rows - 2, 1] = "N/A";
                ReturnString[Rows - 2, 3] = "N/A";
            }

            ReturnString[Rows - 1, 0] = "Upper Beta Limit:";
            ReturnString[Rows - 1, 2] = "Lower Alpha Limit:";

            if (this.Type != TypeOfHiLo.ALPHA)
            {
                ReturnString[Rows - 1, 1] = Convert.ToString(this.HL_Beta_TB.Text);
                ReturnString[Rows - 1, 3] = Convert.ToString(this.LL_Beta_TB.Text);
            }
            else
            {
                ReturnString[Rows - 1, 1] = "N/A";
                ReturnString[Rows - 1, 3] = "N/A";
            }

            return ReturnString;
        }

        private void FillHiLoGridWithData(string[,] arrData)
        {
            int Rows = arrData.GetLength(0);
            int Cols = arrData.GetLength(1);
            int smpTime = Convert.ToInt32(arrData[1, 1]);

            this.Num_Counts_TB.Text = Convert.ToString(Rows - 5);
            this.Min_TB.Text = Convert.ToString(smpTime / 60);
            this.Sec_TB.Text = Convert.ToString(smpTime % 60);

            this.NumSamples = Rows - 5;
            this.Add_Or_Subtract_Rows(this.NumSamples, this.HiLo_Results_DataGridView);

            for (int i = 1; i <= this.HiLo_Results_DataGridView.RowCount; i++)
            {
                for (int j = 0; j < this.HiLo_Results_DataGridView.ColumnCount; j++)
                {
                    this.HiLo_Results_DataGridView[j, i - 1].Value = arrData[i, j];
                }
            }

            if (this.Type != TypeOfHiLo.BETA)
            {
                this.HL_Alpha_TB.Text = arrData[Rows - 2, 1];
                this.LL_Alpha_TB.Text = arrData[Rows - 2, 3];
            }
            else
            {
                this.HL_Alpha_TB.Text = "N/A";
                this.LL_Alpha_TB.Text = "N/A";
            }

            if (this.Type != TypeOfHiLo.ALPHA)
            {
                this.HL_Beta_TB.Text = arrData[Rows - 1, 1];
                this.LL_Beta_TB.Text = arrData[Rows - 1, 3];
            }
            else
            {
                this.HL_Beta_TB.Text = "N/A";
                this.LL_Beta_TB.Text = "N/A";
            }
            this.lbl_HiLocalDate.Text = (DateTime.Now).ToShortDateString();
        }

        private void endFormActivities()
        {
            //Stop the HL listener, if it exists
            if (this.HL != null)
            {
                this.HL.RequestStop();
            }
        }

        private void notifyParentForm()
        {
            this.LaunchedFrom.GetParentForm().refreshConnectStatus();
        }
        #endregion

        #region Callbacks
        private void InvokeCallback()
        {
            this.Invoke(new UpdateLimitsCallback(this.Update_Limits));
        }

        private void Update_Limits()
        {
            if(!this.DABRAS.IsConnected())
            {
                this.endFormActivities();
                this.LaunchedFrom.endFormActivities();
                this.notifyParentForm();
                MessageBox.Show("Error: Must be connected to the DABRAS to perform a Hi/Lo calibration. Please re-connect and try again.");
                return;
            }
            if (this.HL.WasTestCompleted())
            {
                double[] ListOfReturnedValues = HL.GetReturnedValues();

                this.HL_Alpha_TB.Text = StaticMethods.RoundToSigFigs(ListOfReturnedValues[0]);
                this.AlphaUL = ListOfReturnedValues[0];

                this.LL_Alpha_TB.Text = StaticMethods.RoundToSigFigs(ListOfReturnedValues[1]);
                this.AlphaLL = ListOfReturnedValues[1];

                this.HL_Beta_TB.Text = StaticMethods.RoundToSigFigs(ListOfReturnedValues[2]);
                this.BetaUL = ListOfReturnedValues[2];

                this.LL_Beta_TB.Text = StaticMethods.RoundToSigFigs(ListOfReturnedValues[3]);
                this.BetaLL = ListOfReturnedValues[3];

                this.AlphaAvg = Convert.ToDouble(StaticMethods.RoundToSigFigs(ListOfReturnedValues[4]));
                this.BetaAvg = Convert.ToDouble(StaticMethods.RoundToSigFigs(ListOfReturnedValues[5]));

                this.TotalSampleTime = Convert.ToInt32(HL.GetTotalSampleTime());

                this.SaveHiLoLimits();

                //Write to file
                string FilePath = String.Format("{0}\\data\\cal\\HiLo\\{1}_{2}_{3}_{4}_{5}_HL.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DABRAS.Serial_Number, this.Type);
                string[,] DataToWrite = MakeDataWritable(HiLo_Results_DataGridView);
                try
                {
                    if (!File.Exists(FilePath))
                    {
                        File.Create(FilePath).Dispose();
                    }

                    using (FileStream F = new FileStream(FilePath, FileMode.Append))
                    {
                        Logger.WriteCSV(F, DataToWrite);
                        //Toast T = new Toast(String.Format("File written to {0}", FilePath));
                        //Toast T = new Toast("File Written.");
                        //T.Show();
                    }
                }
                catch
                {
                    MessageBox.Show("Automatic write failed.");
                }

                HL.RequestStop();

                this.AskSave = true;

                switch (this.Type)
                {
                    case TypeOfHiLo.BACKGROUND:
                        this.bkgrdData = DataToWrite;
                        break;
                    case TypeOfHiLo.ALPHA:
                        this.alphaData = DataToWrite;
                        break;
                    case TypeOfHiLo.BETA:
                        this.betaData = DataToWrite;
                        break;
                }
            }

            ToggleGUI(false);

            return;
        }       
        #endregion

        #region Getters
        public bool WasTestCompleted()
        {
            if (HL != null)
            {
                return HL.WasTestCompleted();
            }

            return false;
        }

        public double GetAlphaLL()
        {
            return this.AlphaLL;
        }

        public double GetAlphaUL()
        {
            return this.AlphaUL;
        }

        public double GetBetaLL()
        {
            return this.BetaLL;
        }

        public double GetBetaUL()
        {
            return this.BetaUL;
        }

        public double GetAlphaAvg()
        {
            return this.AlphaAvg;
        }

        public double GetBetaAvg()
        {
            return this.BetaAvg;
        }

        public bool WasSourceListModified()
        {
            return this.ModifiedSourceList;
        }

        public List<Radioactive_Source> GetModifiedSources()
        {
            return this.ListOfSources;
        }

        public bool WasDABRASModified()
        {
            return this.NewDABRAS;
        }

        public DABRAS GetDABRAS()
        {
            return this.DABRAS;
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
            
            if (Key.Control)
            {
                if (Key.KeyCode == Keys.S)
                {
                    if (StopButton.Enabled)
                    {
                        StopButton_Click(this, null);
                    }

                    return;
                }
            }
        }
        #endregion

        #region Finalization
        private void FormHiLo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HL != null)
            {
                HL.RequestStop();
            }
            if (!Calibrating)
            {
                this.LaunchedFrom.Enabled = true; //this.LaunchedFrom.Show();
            }
        }
        #endregion  

        #region Save Button Handler
        private void SaveButton_Click(object sender, EventArgs e)
        {
            string[,] DataToWrite = MakeDataWritable(this.HiLo_Results_DataGridView);
            SaveFileDialog SD = new SaveFileDialog();
            SD.Filter = "Comma Separated Value|*.csv";
            SD.ShowDialog();
            if (SD.FileName != "")
            {
                FileStream F = (FileStream)SD.OpenFile();
                string FilePath = SD.FileName;
                Logger.WriteCSV(F, DataToWrite);
            }

            return;
        }
        #endregion

        #region Quick Calibration Handlers

        public void StartBackgroundHiLo(object Sender, List<Radioactive_Source> ListOfCalibratedSources)
        {
            /*Guard against improper calls
            if (!(Sender is QuickCalibrationController))
            {
                return;
            }*/

            this.Calibrating = true;

            /*Set 10 counts*/
            //this.Num_Counts_TB.Text = "10";
            //Num_Counts_TB_TextChanged(this, null);

            /*Set Counting time to 10 minutes*/
            //this.Min_TB.Text = "10";
            //this.Sec_TB.Text = "0";

            this.ListOfSources = ListOfCalibratedSources;

            /*Start aquiring*/
            AquireButton_Click(this, null);

            return;
        }

        #endregion

        #region Show/Hide Handler
        private void HiLo_VisibleChanged(object sender, EventArgs e)
        {
            if (this.DABRAS != null)
            {
                if (!this.DABRAS.IsConnected())
                {
                    this.endFormActivities();
                    this.LaunchedFrom.endFormActivities();
                    this.notifyParentForm();
                }
            }
        }
        #endregion

        #region HiLo type radio button handlers
        private void rdBtnBackground_CheckedChanged(object sender, EventArgs e)
        {
            this.setTypeByRadioButtons();
            this.setFormByType();
        }

        private void rdBtnAlpha_CheckedChanged(object sender, EventArgs e)
        {
            this.setTypeByRadioButtons();
            this.setFormByType();
        }

        private void rdBtnBeta_CheckedChanged(object sender, EventArgs e)
        {
            this.setTypeByRadioButtons();
            this.setFormByType();
        }
        #endregion
    }


    public class HiLoListener
    {
        #region Data Members
        private bool Done;
        private DataGridView Calibration_Table;
        private int SampleTime;
        private int NumSamples;
        private DABRAS DABRAS;
        private bool ShouldStop;
        private FormHiLo.TypeOfHiLo HiLo_Base_Type;
        private FormHiLo.LimitType HiLo_Limit_Type;
        private double LimitValue;

        private double AlphaBackground = 0;
        private double BetaBackground = 0;

        private double[] ListOfReturnedValues = {0,0,0,0,0,0};

        private bool TestComplete = false;

        public event EventHandler BackgroundThreadFinished;
        #endregion

        #region Constructor
        public HiLoListener(DABRAS _DABRAS, int _SampleTime, int _NumSamples, DataGridView _CalTable, FormHiLo.TypeOfHiLo _Type, FormHiLo.LimitType _LimitType, double _LimitValue, double _AlphaBG, double _BetaBG)
        {
            this.DABRAS = _DABRAS;
            this.Calibration_Table = _CalTable;
            this.SampleTime = _SampleTime;
            this.NumSamples = _NumSamples;
            this.HiLo_Base_Type = _Type;
            this.HiLo_Limit_Type = _LimitType;
            this.LimitValue = _LimitValue;

            if (this.HiLo_Base_Type != FormHiLo.TypeOfHiLo.BACKGROUND)
            {
                this.AlphaBackground = _AlphaBG;
                this.BetaBackground = _BetaBG;
            }

            ShouldStop = false;
        }
        #endregion

        #region Control Functions
        public void RequestStop()
        {
            ShouldStop = true;
        }
        #endregion

        #region Getters
        /*Returns in the following format
         * <Alpha UL, Alpha LL, Beta UL, Beta LL, Alpha Avg, Beta Avg>
         */
        public double[] GetReturnedValues()
        {
            return this.ListOfReturnedValues;
        }

        public bool WasTestCompleted()
        {
            return this.TestComplete;
        }

        public double GetTotalSampleTime()
        {
            return this.NumSamples * this.SampleTime;
        }

        #endregion

        #region Main Background Thread
        public void Get_HiLo()
        {
            //Stop any background threads, if they exist
            DABRAS.Cut();

            //Set aquisition time
            DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
            Thread.Sleep(250);

            //Clear any data left in the buffer
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();
            DABRAS.EnableWatchdog();

            for (int i = 0; i < NumSamples; i++)
            {

                bool RowComplete = false;
                DABRAS.Write_To_Serial_Port("g");

                //Check for the first good packet
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
                        return;
                    }

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

                //Do not increment the row index until the current sample time has elapsed
                while (!RowComplete && !ShouldStop)
                {
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
                        BackgroundThreadFinished(this, null);
                        return;
                    }

                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                    /*TODO: Update efficiencies. Need formula*/

                    //Grab handles to form
                    if (!ShouldStop)
                    {
                        DataGridViewCell TimeElapsed = Calibration_Table[1, i];
                        DataGridViewCell AlphaTot = Calibration_Table[2, i];
                        DataGridViewCell AlphaNCPM = Calibration_Table[3, i];
                        DataGridViewCell BetaTot = Calibration_Table[4, i];
                        DataGridViewCell BetaNCPM = Calibration_Table[5, i];

                        //Parse data to form
                        TimeElapsed.Value = IncomingData.ElTime;

                        if (IncomingData != null && IncomingData.ElTime != 0)
                        {
                            AlphaTot.Value = IncomingData.AlphaTot;
                            //if (IncomingData.AlphaTot != 0)
                            //{
                            //    AlphaNCPM.Value = String.Format("{0:#####.#}", ((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)) - AlphaBackground)); //Convert raw counts into CPM. +1 to avoid DBZ, PIC uses zero based counting.
                            //}
                            //else
                            //{
                            //    AlphaNCPM.Value = "0";
                            //}
                                
                            
                            BetaTot.Value = IncomingData.BetaTot;
                            //if (IncomingData.BetaTot != 0)
                            //{
                            //    BetaNCPM.Value = String.Format("{0:#####.#}", (IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)) - BetaBackground);
                            //}
                            //else
                            //{
                            //    BetaNCPM.Value = "0";
                            //}

                            AlphaNCPM.Value = StaticMethods.RoundToDecimal((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)) - AlphaBackground, 1); //Convert raw counts into CPM. +1 to avoid DBZ, PIC uses zero based counting.
                            BetaNCPM.Value = StaticMethods.RoundToDecimal((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60) - BetaBackground), 1);
                            
                                
                            //BetaNCPM.Value = ((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)) - BetaBackground);

                            //Re-draw table
                            Calibration_Table.Invalidate();

                            //If the sample time has elapsed, increment the row.
                            if (IncomingData.ElTime >= SampleTime)
                            {
                                RowComplete = true;
                            }
                        }

                        DABRAS.KickWatchdog();
                    }
                }

            }

            DABRAS.DisableWatchdog();

            if (!ShouldStop)
            {
                //Compute averages
                double AverageAlphaCPM = 0;
                double AverageBetaCPM = 0;

                for (int i = 0; i < NumSamples; i++)
                {
                    AverageAlphaCPM += Convert.ToDouble(Calibration_Table[3, i].Value);
                    AverageBetaCPM += Convert.ToDouble(Calibration_Table[5, i].Value);
                }

                AverageAlphaCPM /= Convert.ToDouble(NumSamples);
                AverageBetaCPM /= Convert.ToDouble(NumSamples);

                DataGridViewCell AverageAlphaCell = Calibration_Table[3, NumSamples];
                DataGridViewCell AverageBetaCell = Calibration_Table[5, NumSamples];
                DataGridViewCell StdDevAlphaCell = Calibration_Table[3, NumSamples + 1];
                DataGridViewCell StdDevBetaCell = Calibration_Table[5, NumSamples + 1];

                AverageAlphaCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaCPM);
                AverageBetaCell.Value = StaticMethods.RoundToSigFigs(AverageBetaCPM);

                //Compute Standard Deviations
                double StdDevAlpha = 0;
                double StdDevBeta = 0;
                for (int i = 0; i < NumSamples; i++)
                {
                    StdDevAlpha += (AverageAlphaCPM - Convert.ToDouble(Calibration_Table[3, i].Value)) * (AverageAlphaCPM - Convert.ToDouble(Calibration_Table[3, i].Value));
                    StdDevBeta += (AverageBetaCPM - Convert.ToDouble(Calibration_Table[5, i].Value)) * (AverageBetaCPM - Convert.ToDouble(Calibration_Table[5, i].Value));
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
                    StdDevAlpha = Math.Sqrt(AverageAlphaCPM);
                    StdDevBeta = Math.Sqrt(AverageBetaCPM);
                }

                //Allow for the usage of Poisson statistics
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

                //Compute HiLo limits
                double AlphaUpperLimit = 0;
                double AlphaLowerLimit = 0;
                double BetaUpperLimit = 0;
                double BetaLowerLimit = 0;
                
                if (this.HiLo_Limit_Type == FormHiLo.LimitType.STDEV)
                {
                    //Compute Betas for all except the Alpha
                    
                    if (this.HiLo_Base_Type != FormHiLo.TypeOfHiLo.ALPHA)
                    {
                        BetaLowerLimit = AverageBetaCPM - (this.LimitValue * StdDevBeta);
                        BetaUpperLimit = AverageBetaCPM + (this.LimitValue * StdDevBeta);
                    }

                    //Compute Alphas for all except the Beta
                    if (this.HiLo_Base_Type != FormHiLo.TypeOfHiLo.BETA)
                    {
                        AlphaLowerLimit = AverageAlphaCPM - (this.LimitValue * StdDevAlpha);
                        AlphaUpperLimit = AverageAlphaCPM + (this.LimitValue * StdDevAlpha);
                    }

                }
                else
                {
                    //Compute Betas for all except the Alpha
                    if (this.HiLo_Base_Type != FormHiLo.TypeOfHiLo.ALPHA)
                    {
                        BetaLowerLimit = AverageBetaCPM - (this.LimitValue * 0.01 * AverageBetaCPM);
                        BetaUpperLimit = AverageBetaCPM + (this.LimitValue * 0.01 * AverageBetaCPM);
                    }

                    //Compute Alphas for all except the Beta
                    if (this.HiLo_Base_Type != FormHiLo.TypeOfHiLo.BETA)
                    {
                        AlphaLowerLimit = AverageAlphaCPM - (this.LimitValue * 0.01 * AverageAlphaCPM);
                        AlphaUpperLimit = AverageAlphaCPM + (this.LimitValue * 0.01 * AverageAlphaCPM);
                    }
                }

                //Write to return packet in specified format
                this.ListOfReturnedValues[0] = AlphaUpperLimit;
                this.ListOfReturnedValues[1] = AlphaLowerLimit;
                this.ListOfReturnedValues[2] = BetaUpperLimit;
                this.ListOfReturnedValues[3] = BetaLowerLimit;
                this.ListOfReturnedValues[4] = AverageAlphaCPM;
                this.ListOfReturnedValues[5] = AverageBetaCPM;

                using (SoundPlayer S = new SoundPlayer(DABRAS_Software.Properties.Resources.DoneSound))
                {
                    S.Play();
                }

                TestComplete = true;
                BackgroundThreadFinished(this, null);
            }

            //Reinstate background threads
            DABRAS.ResumeBackgroundMonitors();

            return;
        }
        #endregion

    }
}
