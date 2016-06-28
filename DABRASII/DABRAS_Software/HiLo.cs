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
    public partial class HiLo : Form
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
        private readonly CalibrationForm LaunchedFrom;
        private bool NewDABRAS = false;
        private DefaultConfigurations DC;

        private HiLoListener HL = null;
        private List<Radioactive_Source> ListOfSources;
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


        public delegate void UpdateLimitsCallback();
        #endregion

        #region Constructor
        public HiLo(CalibrationForm _CF, TypeOfHiLo _Type)
        {
            InitializeComponent();

            this.Type = _Type;
            this.LaunchedFrom = _CF;
            this.DABRAS = LaunchedFrom.GetDABRAS();
            this.Logger = LaunchedFrom.GetLogger();
            this.ListOfSources = LaunchedFrom.GetSourceList();
            this.DC = LaunchedFrom.GetDefaultConfig();

            if (DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = DABRAS.Firmware_Version;
                this.DABRAS_SN_Label.Text = DABRAS.Serial_Number;
                this.DABRAS_Status_Label.Text = "STATUS: Connected!";
            }

            switch (Type)
            {
                case TypeOfHiLo.BACKGROUND:
                    this.TypeOfHiLoLabel.Text = "Background Alpha and Beta Radiation";
                    HiLo_Results_DataGridView.Columns.Add("Calibration_Source_Name", "Bkgd");
                    this.Min_TB.Text = "10"; //account for different counting time
                    this.Net_CPM_Label.Text = "Gross CPM";
                    this.Net_CPM_Label2.Text = "Gross CPM";
                    break;
                case TypeOfHiLo.ALPHA:
                    this.TypeOfHiLoLabel.Text = "Alpha: Am-241";
                    HiLo_Results_DataGridView.Columns.Add("Calibration_Source_Name", "Am-241");
                    this.HL_Beta_Label.Visible = false;
                    this.HL_Beta_TB.Visible = false;
                    this.LL_Beta_TB.Visible = false;
                    this.LL_Beta_Label.Visible = false;

                    break;
                case TypeOfHiLo.BETA:
                    this.TypeOfHiLoLabel.Text = "Beta: Sr-90";
                    HiLo_Results_DataGridView.Columns.Add("Calibration_Source_Name", "Sr-90");
                    this.HL_Alpha_Label.Visible = false;
                    this.HL_Alpha_TB.Visible = false;
                    this.LL_Alpha_TB.Visible = false;
                    this.LL_Alpha_Label.Visible = false;

                    /*Move label*/
                    this.Net_CPM_Label.Location = new Point(Net_CPM_Label.Location.X, Net_CPM_Label.Location.Y + 25);
                    this.Net_CPM_Label2.Location = new Point(Net_CPM_Label2.Location.X, Net_CPM_Label2.Location.Y + 25);

                    break;
            }

            int NumSamples = 0;
            
            try
            {
                NumSamples = Convert.ToInt32(this.Num_Counts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Bad sample count.");
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }

            this.HiLo_Results_DataGridView = Make_HiLo_GridView(NumSamples);

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);


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

            if (!DABRAS.IsConnected())
            {
                MessageBox.Show("Error: Must be connected to the DABRAS to perform a Hi/Lo calibration. Please re-connect and try again.");
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

            ClearDataGridView(this.HiLo_Results_DataGridView);

            Radioactive_Source BG = ListOfSources.Find(x => x.GetName() == "Background");

            HL = new HiLoListener(this.DABRAS, SampleTime, NumberOfSamples, this.HiLo_Results_DataGridView, this.Type, this.TypeOfLimit, this.LimitValue, BG.GetAnnualAlphaCPM(), BG.GetAnnualBetaCPM());

            Thread BackgroundThread = new Thread(() => HL.Get_HiLo());
            BackgroundThread.Start();

            HL.BackgroundThreadFinished += (s, args) => { InvokeCallback(); };

            ToggleGUI(true);
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

        #region Save Limits Button Handler
        private void Save_Limits_Button_Click(object sender, EventArgs e)
        {
            /*Find radioactive source and update its values*/
            string NameOfSource = GetGridViewString();

            Radioactive_Source SourceToModify = null;

            SourceToModify = ListOfSources.Find(x => x.GetName() == NameOfSource);

            double AlphaHi = Convert.ToDouble(this.HL_Alpha_TB.Text);
            double AlphaLo = Convert.ToDouble(this.LL_Alpha_TB.Text);
            double BetaHi = Convert.ToDouble(HL_Beta_TB.Text);
            double BetaLo = Convert.ToDouble(LL_Beta_TB.Text);


            /*Swap the values such that the hi is always greater than the low*/
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

            if (SourceToModify != null)
            {
                if ((SourceToModify.GetSourceType() == Radioactive_Source.RadiationType.Alpha) || (SourceToModify.GetSourceType() == Radioactive_Source.RadiationType.AlphaBeta))
                {
                    if ((this.HL_Alpha_TB.Text != "") && (this.LL_Alpha_TB.Text != ""))
                    {
                        try
                        {
                            SourceToModify.SetAlphaHi(AlphaHi);
                            SourceToModify.SetAlphaLo(AlphaLo);
                            SourceToModify.SetAlphaHiLoAvg(this.AlphaAvg);
                            SourceToModify.SetAnnualAlphaCPM(this.AlphaAvg);
                            SourceToModify.SetHiLoCalibratedDate(DateTime.Now);
                            SourceToModify.SetAnnualCalibratedTimespan(this.TotalSampleTime);
                            this.ModifiedSourceList = true;
                        }
                        catch
                        {
                            MessageBox.Show("Error: Bad values");
                            return;
                        }
                    }
                }

                if ((SourceToModify.GetSourceType() == Radioactive_Source.RadiationType.Beta) || (SourceToModify.GetSourceType() == Radioactive_Source.RadiationType.AlphaBeta))
                {
                    if ((this.HL_Beta_TB.Text != "") && (this.LL_Beta_TB.Text != ""))
                    {
                        try
                        {
                            SourceToModify.SetBetaHi(BetaHi);
                            SourceToModify.SetBetaLo(BetaLo);
                            SourceToModify.SetBetaHiLoAvg(this.BetaAvg);
                            SourceToModify.SetAnnualBetaCPM(this.BetaAvg);
                            SourceToModify.SetHiLoCalibratedDate(DateTime.Now);
                            SourceToModify.SetAnnualCalibratedTimespan(this.TotalSampleTime);
                            this.ModifiedSourceList = true;
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
            
            /*Compute averages*/
            double AverageAlphaCPM = 0;
            double AverageBetaCPM = 0;

            try
            {
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


                /*Compute Standard Deviations*/
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

                /*Allow for the usage of Poisson statistics*/
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

                /*Compute HiLo limits*/
                double AlphaUpperLimit = 0;
                double AlphaLowerLimit = 0;
                double BetaUpperLimit = 0;
                double BetaLowerLimit = 0;

                if (this.TypeOfLimit == HiLo.LimitType.STDEV)
                {
                    /*Compute Betas for all except the Alpha*/


                    if (this.Type != HiLo.TypeOfHiLo.ALPHA)
                    {
                        BetaLowerLimit = AverageBetaCPM - (this.LimitValue * StdDevBeta);
                        BetaUpperLimit = AverageBetaCPM + (this.LimitValue * StdDevBeta);
                    }

                    /*Compute Alphas for all except the Beta*/
                    if (this.Type != HiLo.TypeOfHiLo.BETA)
                    {
                        AlphaLowerLimit = AverageAlphaCPM - (this.LimitValue * StdDevAlpha);
                        AlphaUpperLimit = AverageAlphaCPM + (this.LimitValue * StdDevAlpha);
                    }

                }

                else
                {
                    /*Compute Betas for all except the Alpha*/
                    if (this.Type != HiLo.TypeOfHiLo.ALPHA)
                    {
                        BetaLowerLimit = AverageBetaCPM - (this.LimitValue * 0.01 * AverageBetaCPM);
                        BetaUpperLimit = AverageBetaCPM + (this.LimitValue * 0.01 * AverageBetaCPM);
                    }

                    /*Compute Alphas for all except the Beta*/
                    if (this.Type != HiLo.TypeOfHiLo.BETA)
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

            }
            catch
            {
                ;
            }
            MessageBox.Show("Done!");
            return;
        }
        #endregion

        #region Misc. GUI Functions
        private void Num_Counts_TB_TextChanged(object sender, EventArgs e)
        {
            int NumSamples = 0;
            try
            {
                NumSamples = Convert.ToInt32(this.Num_Counts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Counts must be an integer greater than zero. Please try again.");
                return;
            }

            Add_Or_Subtract_Rows(NumSamples, this.HiLo_Results_DataGridView);
            HiLo_Results_DataGridView.Invalidate();
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
            this.aquireCtrlAToolStripMenuItem.Enabled = !TestRunning;
            this.Save_Limits_Button.Enabled = !TestRunning;
            this.SaveButton.Enabled = !TestRunning;
            this.writeResultsToFileCtrlVToolStripMenuItem.Enabled = !TestRunning;
            this.Recompute_Limits_Button.Enabled = !TestRunning;
            this.Override_CB.Enabled = !TestRunning;
            this.saveImageCtrlIToolStripMenuItem.Enabled = !TestRunning;
            this.ImageSaveButton.Enabled = !TestRunning;
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem.Enabled = !TestRunning;

            this.StopButton.Enabled = TestRunning;
            this.stopAquiringCtrlSToolStripMenuItem.Enabled = TestRunning;

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

        #region About Handler
        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm NewForm = new AboutForm();
            NewForm.ShowDialog();
        }
        #endregion

        #region WebForm Handler
        private void openWebBasedSurveyF12ToolStripMenuItem_Click(object sender, EventArgs e)
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
        private void Add_Or_Subtract_Rows(int FinalNumberOfRows, DataGridView DG)
        {
            FinalNumberOfRows += 2;
            if ((DG.RowCount) == FinalNumberOfRows)
            {
                return;
            }

            if ((DG.RowCount) > FinalNumberOfRows)
            {
                /*Too many get rid of a few*/
                while ((DG.RowCount) > (FinalNumberOfRows + 1))
                {
                    DG.Rows.RemoveAt(0);
                }

            }

            else
            {
                while ((DG.RowCount) < (FinalNumberOfRows + 1))
                {
                    DG.Rows.Insert(0, "", "", "", "", "", "", "0", "0");
                }

            }

            /*Re-number the rows*/
            for (int i = 0; i < DG.RowCount - 3; i++)
            {
                DG[0, i].Value = i + 1;
            }
        }
        
        private DataGridView Make_HiLo_GridView(int NumSamples)
        {
            DataGridView HiLo_Results_Gridview = new DataGridView();

            //HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Source", this.GetGridViewString());
            HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Acq_Time", "Acq Time");
            HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Alpha_Count", "Alpha Count");
            if (this.Type != TypeOfHiLo.BACKGROUND)
            {
                HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Alpha_CPM", "Alpha NCPM");
            }
            else
            {
                HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Alpha_CPM", "Alpha GCPM");
            }
            HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Beta_Count", "Beta Count");

            if (this.Type != TypeOfHiLo.BACKGROUND)
            {
                HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Beta_CPM", "Beta NCPM");
            }
            else
            {
                HiLo_Results_DataGridView.Columns.Add("Calibration_Results_Beta_CPM", "Beta GCPM");

            }
            /*Add one row for each count*/
            for (int i = 0; i < NumSamples; i++)
            {
                HiLo_Results_DataGridView.Rows.Add(new object[] { Convert.ToString((i + 1)), "", "", "", "", "", "", "" });

            }

            HiLo_Results_DataGridView.Rows.Add(new object[] { "Average", "", "", "", "", "", "", "" });
            HiLo_Results_DataGridView.Rows.Add(new object[] { "StDev", "", "", "", "", "", "", "" });

            return HiLo_Results_DataGridView;

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

        #endregion

        #region GUI Callbacks
        private void InvokeCallback()
        {
            this.Invoke(new UpdateLimitsCallback(this.Update_Limits));
        }

        private void Update_Limits()
        {
            if (HL.WasTestCompleted())
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

                /*Write to file*/
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
                        Toast T = new Toast("File Written.");
                        T.Show();
                    }
                }
                catch
                {
                    MessageBox.Show("Automatic write failed.");
                }

                HL.RequestStop();

                this.AskSave = true;
            }

            if (!DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = "Firmware Version: XX";
                this.DABRAS_SN_Label.Text = "Serial Number: XX";
                this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
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

        #region Port Connection Handler
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

                    this.DABRAS_Firmware_Label.Text = "Firmware Version: ??";
                    this.DABRAS_SN_Label.Text = "Serial Number: ??";
                    this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
                    DABRAS.VCP_Instance = "";
                    this.NewDABRAS = true;

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
                this.NewDABRAS = true;
            }
        }
        #endregion      

        #region Dummy Overloads
        private void writeResultsToFileCtrlVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (StopButton.Enabled)
            {
                StopButton_Click(this, null);
            }
        }

        private void aquireCtrlAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AquireButton_Click(this, null);
        }

        private void stopAquiringCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopButton_Click(this, null);
        }

        private void closeWindowCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            //this.Close();
        }

        private void saveImageCtrlIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageSaveButton_Click(this, null);
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
                if (Key.KeyCode == Keys.Q)
                {
                    closeWindowCtrlQToolStripMenuItem_Click(this, null);
                    return;
                }

                if (Key.KeyCode == Keys.A)
                {
                    if (AquireButton.Enabled)
                    {
                        aquireCtrlAToolStripMenuItem_Click(this, null);
                    }

                    return;
                }

                if (Key.KeyCode == Keys.S)
                {
                    if (StopButton.Enabled)
                    {
                        StopButton_Click(this, null);
                    }

                    return;
                }

                if (Key.KeyCode == Keys.P)
                {
                    if (connectOrDisconnectAPortCtrlPToolStripMenuItem.Enabled)
                    {
                        connectOrDisconnectAPortCtrlPToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.V)
                {
                    if (Save_Limits_Button.Enabled)
                    {
                        Save_Limits_Button_Click(this, null);
                    }
                }
            }

            if (Key.KeyCode == Keys.F12)
            {
                openWebBasedSurveyF12ToolStripMenuItem_Click(this, null);
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
        private void HiLo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HL != null)
            {
                HL.RequestStop();
            }

            if (AskSave)
            {
                if (MessageBox.Show("Save limits?", "Confirm Save", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Save_Limits_Button_Click(this, null);
                }
            }

            if (!Calibrating)
            {
                this.LaunchedFrom.Show();
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
            /*Guard against improper calls*/
            if (!(Sender is QuickCalibrationController))
            {
                return;
            }

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


    public class HiLoListener
    {
        #region Data Members
        private bool Done;
        private DataGridView Calibration_Table;
        private int SampleTime;
        private int NumSamples;
        private DABRAS DABRAS;
        private bool ShouldStop;
        private HiLo.TypeOfHiLo HiLo_Base_Type;
        private HiLo.LimitType HiLo_Limit_Type;
        private double LimitValue;

        private double AlphaBackground = 0;
        private double BetaBackground = 0;

        private double[] ListOfReturnedValues = {0,0,0,0,0,0};

        private bool TestComplete = false;

        public event EventHandler BackgroundThreadFinished;
        #endregion

        #region Constructor
        public HiLoListener(DABRAS _DABRAS, int _SampleTime, int _NumSamples, DataGridView _CalTable, HiLo.TypeOfHiLo _Type, HiLo.LimitType _LimitType, double _LimitValue, double _AlphaBG, double _BetaBG)
        {
            this.DABRAS = _DABRAS;
            this.Calibration_Table = _CalTable;
            this.SampleTime = _SampleTime;
            this.NumSamples = _NumSamples;
            this.HiLo_Base_Type = _Type;
            this.HiLo_Limit_Type = _LimitType;
            this.LimitValue = _LimitValue;

            if (this.HiLo_Base_Type != HiLo.TypeOfHiLo.BACKGROUND)
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
            /*Stop any background threads, if they exist*/
            DABRAS.Cut();

            /*Set aquisition time*/
            DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
            Thread.Sleep(250);

            /*Clear any data left in the buffer*/
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();
            DABRAS.EnableWatchdog();

            for (int i = 0; i < NumSamples; i++)
            {

                bool RowComplete = false;
                DABRAS.Write_To_Serial_Port("g");

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

                /*Do not increment the row index until the current sample time has elapsed*/
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

                    /*Grab handles to form*/
                    if (!ShouldStop)
                    {
                        DataGridViewCell TimeElapsed = Calibration_Table[1, i];
                        DataGridViewCell AlphaTot = Calibration_Table[2, i];
                        DataGridViewCell AlphaNCPM = Calibration_Table[3, i];
                        DataGridViewCell BetaTot = Calibration_Table[4, i];
                        DataGridViewCell BetaNCPM = Calibration_Table[5, i];

                        /*Parse data to form*/
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

                            /*Re-draw table*/
                            Calibration_Table.Invalidate();

                            /*If the sample time has elapsed, increment the row.*/
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

                /*Compute averages*/
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


                /*Compute Standard Deviations*/
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

                /*Allow for the usage of Poisson statistics*/
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

                /*Compute HiLo limits*/
                double AlphaUpperLimit = 0;
                double AlphaLowerLimit = 0;
                double BetaUpperLimit = 0;
                double BetaLowerLimit = 0;
                
                if (this.HiLo_Limit_Type == HiLo.LimitType.STDEV)
                {
                    /*Compute Betas for all except the Alpha*/  
                    
                    if (this.HiLo_Base_Type != HiLo.TypeOfHiLo.ALPHA)
                    {
                        BetaLowerLimit = AverageBetaCPM - (this.LimitValue * StdDevBeta);
                        BetaUpperLimit = AverageBetaCPM + (this.LimitValue * StdDevBeta);
                    }

                    /*Compute Alphas for all except the Beta*/
                    if (this.HiLo_Base_Type != HiLo.TypeOfHiLo.BETA)
                    {
                        AlphaLowerLimit = AverageAlphaCPM - (this.LimitValue * StdDevAlpha);
                        AlphaUpperLimit = AverageAlphaCPM + (this.LimitValue * StdDevAlpha);
                    }

                }

                else
                {
                    /*Compute Betas for all except the Alpha*/
                    if (this.HiLo_Base_Type != HiLo.TypeOfHiLo.ALPHA)
                    {
                        BetaLowerLimit = AverageBetaCPM - (this.LimitValue * 0.01 * AverageBetaCPM);
                        BetaUpperLimit = AverageBetaCPM + (this.LimitValue * 0.01 * AverageBetaCPM);
                    }

                    /*Compute Alphas for all except the Beta*/
                    if (this.HiLo_Base_Type != HiLo.TypeOfHiLo.BETA)
                    {
                        AlphaLowerLimit = AverageAlphaCPM - (this.LimitValue * 0.01 * AverageAlphaCPM);
                        AlphaUpperLimit = AverageAlphaCPM + (this.LimitValue * 0.01 * AverageAlphaCPM);
                    }

                }

                

                /*Write to return packet in specified format*/
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

            /*Reinstate background threads*/
            DABRAS.ResumeBackgroundMonitors();

            return;
        }
        #endregion

    }
}
