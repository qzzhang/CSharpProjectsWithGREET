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

    public partial class CalibrationForm : Form
    {
        #region Data Members
        private readonly mainFramework frmParent;
        private DABRAS DABRAS = null;
        private Logger Logger = null;
        protected string[] ValidSourceNames = {};
        protected string[,] ValidSourceRadiationTypes = {{},{}};

        private List<Radioactive_Source> ListOfValidSources = null;
        
        private EfficiencyListener EL;
        private BackgroundListener BL;

        private bool NewSources = false;

        private bool NewDABRAS = false;

        private bool NewPasswordSet = false;
        private string NewPassword = "";

        private bool CalibrationCompleted = false;

        private double AlphaEff;
        private double BetaEff;

        private Thread BackgroundBackgroundThread;
        private Thread EfficiencyBackgroundThread;

        private DefaultConfigurations DC;
        private bool DCModified = false;

        private bool Calibrating = false;

        private Form ChildDialogue = null;

        public delegate void OnBackgroundThreadFinished();
        #endregion
        
        #region Constructor
        public CalibrationForm(mainFramework Parent)
        {
            InitializeComponent();
            frmParent = Parent;
            this.ListOfValidSources = frmParent.GetListOfSources();
            this.DABRAS = frmParent.GetDABRAS();
            this.Logger = frmParent.GetLogger();
            this.DC = frmParent.GetDefaultConfig();

            #region Calibration_Results Init
            /*Initialize the Gridview for the calibration*/
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Source", this.Source_ComboBox.Text);
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Acq_Time", "Acq Time");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Alpha_Count", "Alpha Count");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Alpha_CPM", "Alpha CPM");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Beta_Count", "Beta Count");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Beta_CPM", "Beta CPM");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Alpha_Eff", "Alpha Efficiency");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Beta_Eff", "Beta Efficiency");

            int RowIterator = 0;
            try
            {
                RowIterator = Convert.ToInt32(this.Num_Counts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Non-integral value of counts. Exiting...");
                return;
            }


            /*Add one row for each count*/
            for (int i = 0; i < RowIterator; i++)
            {
                Calibration_Results_GridView.Rows.Add(new object[] { Convert.ToString((i + 1)), "", "", "", "", "", "", "" });

            }

            Calibration_Results_GridView.Rows.Add(new object[] { "Average", "", "", "", "", "", "", "" });
            Calibration_Results_GridView.Rows.Add(new object[] { "StDev", "", "", "", "", "", "", "" });



            #endregion

            #region Background_Init
            Background_Results_GridView.Columns.Add("Null_Corner", "");
            Background_Results_GridView.Columns.Add("Background_Results_Acq_Time", "Acq Time");
            Background_Results_GridView.Columns.Add("Background_Results_Alpha_Count", "Alpha Count");
            Background_Results_GridView.Columns.Add("Background_Results_Alpha_CPM", "Alpha CPM");
            Background_Results_GridView.Columns.Add("Background_Results_Beta_Count", "Beta Count");
            Background_Results_GridView.Columns.Add("Background_Results_Beta_CPM", "Beta CPM");

            RowIterator = 0;
            try
            {
                RowIterator = Convert.ToInt32(this.BG_NumCounts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Non-integral value of counts. Exiting...");
                return;
            }


            /*Add one row for each count*/
            for (int i = 0; i < RowIterator; i++)
            {
                Background_Results_GridView.Rows.Add(new object[] { Convert.ToString((i + 1)), "", "", "", "", "", "", "" });

            }

            Background_Results_GridView.Rows.Add(new object[] { "Average", "", "", "", "", "", "", "" });
            Background_Results_GridView.Rows.Add(new object[] { "StDev", "", "", "", "", "", "", "" });


            #endregion

            /*Initialize source combobox*/

            this.Source_ComboBox.Items.Clear();
            foreach (Radioactive_Source i in ListOfValidSources)
            {
                if (i.GetName() != "Background")
                {
                    this.Source_ComboBox.Items.Add(i.GetName());
                }
            }

            this.Source_ComboBox.SelectedIndex = 0;

            Radioactive_Source FirstSelected = ListOfValidSources.Find(x => x.GetName() == Source_ComboBox.Text);

            this.TypeOfRadiationLabel1.Text = FirstSelected.GetSourceType_String();
            this.TypeOfRadiationLabel2.Text = FirstSelected.GetSourceType_String();

            if (DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = "Firmware Version: " + DABRAS.Firmware_Version;
                this.DABRAS_SN_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                this.DABRAS_Status_Label.Text = "STATUS: Connected!";
            }

            Logger = frmParent.GetLogger();

            

            /*Add keypress event handlers*/
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

        }
        #endregion

        #region Background Start Button Handler
        private void Determine_BG_Button_Click(object sender, EventArgs e)
        {
            int SampleTime = 0;
            int NumSamples = 0;
            try
            {
                SampleTime = (Convert.ToInt32(Min_BG_TB.Text) * 60) + Convert.ToInt32(Sec_BG_TB.Text);
                NumSamples = Convert.ToInt32(this.BG_NumCounts_TB.Text);
                Math.Sqrt(SampleTime); //Quick and dirty check for negative numbers
            }
            catch
            {
                MessageBox.Show("Error: Bad Parameters.");
                return;
            }

            if (!DABRAS.IsConnected())
            {
                MessageBox.Show("Error: Must be connected to DABRAS to perform background count. Please connect to the DABRAS.");
                return;
            }

            if (!Calibrating)
            {
                if ((MessageBox.Show(String.Format("Background will be counted {0} times for {1} seconds each. Proceed?", NumSamples, (SampleTime)), "Verify", MessageBoxButtons.YesNo)) != DialogResult.Yes)
                {
                    MessageBox.Show("Aborted.");
                    SetGUI(false);
                    return;
                }
            }
            
            ClearDataGridView(this.Background_Results_GridView);

            /*Clear any data left in the buffer*/
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();

            Radioactive_Source BG = ListOfValidSources.Find(x => x.GetName() == "Background");

            BL = new BackgroundListener(this.DABRAS, SampleTime, NumSamples, this.Background_Results_GridView, BG);
            BackgroundBackgroundThread = new Thread(() => BL.Get_Background());
            BL.BackgroundSampleThreadFinished += (s, args) => { InvokeThreadCallback(); };
            BackgroundBackgroundThread.Start();

            SetGUI(true);
        }
        #endregion

        #region Efficiency Start Button Handler
        private void Determine_Efficiency_Button_Click(object sender, EventArgs e)
        {
            int NumberOfSamples;
            int SampleTime;

            try
            {
                NumberOfSamples = Convert.ToInt32(this.Num_Counts_TB.Text);
                SampleTime = (Convert.ToInt32(this.Minutes_TB.Text) * 60) + Convert.ToInt32(this.Seconds_TB.Text);
                Math.Sqrt(SampleTime);
            }

            catch
            {
                MessageBox.Show("Error: Bad parameters. Please enter valid numerical parameters for the Efficiency Scan and try again.");
                return;
            }

            if (!DABRAS.IsConnected())
            {
                MessageBox.Show("Error: Must be connected to DABRAS to determine its efficiency. Please make a connection (Ctrl + P), and try again.");
                SetGUI(false);
                return;
            }

            if (!Calibrating)
            {
                if ((MessageBox.Show(String.Format("Source will be counted {0} times for {1} seconds each. Proceed?", NumberOfSamples, SampleTime), "Verify", MessageBoxButtons.YesNo)) != DialogResult.Yes)
                {
                    MessageBox.Show("Aborted.");
                    CalibrationCompleted = false;
                    SetGUI(false);
                    return;
                }
            }

            ClearDataGridView(this.Calibration_Results_GridView);

            /*Clear any data left in the buffer*/
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();

            Radioactive_Source R = ListOfValidSources.Find(x => (String.Compare(x.GetName() , this.Source_ComboBox.Text) == 0) );

            Radioactive_Source BG = ListOfValidSources.Find(x => (String.Compare(x.GetName(), "Background") == 0));
            
            EL = new EfficiencyListener(this.DABRAS, SampleTime, NumberOfSamples, this.Calibration_Results_GridView, R, BG.GetAnnualAlphaCPM(), BG.GetAnnualBetaCPM());
            EfficiencyBackgroundThread = new Thread(() => EL.Get_Efficiency());

            EL.EfficiencyBackgroundThreadFinished += (s, args) => { InvokeThreadCallback(); };

            EfficiencyBackgroundThread.Start();

            SetGUI(true);    
        }
        #endregion

        #region Efficiency Save Button Handler
        private void SaveEfficiencyDataButton_Click(object sender, EventArgs e)
        {
            string[,] StringToWrite = MakeDataWritable(this.Calibration_Results_GridView);

            SaveFileDialog S = new SaveFileDialog();
            S.Filter = "Comma Separated Value|*.csv";
            S.ShowDialog();
            if (S.FileName != "")
            {
                FileStream F = (FileStream)S.OpenFile();
                string FilePath = S.FileName;
                Logger.WriteCSV(F, StringToWrite);
            }

            return;
        }
        #endregion

        #region Background Save Handler
        private void SaveBackgroundCalibrationButton_Click(object sender, EventArgs e)
        {
            string[,] StringToWrite = MakeDataWritable(this.Background_Results_GridView);

            SaveFileDialog S = new SaveFileDialog();
            S.Filter = "Comma Separated Value|*.csv";
            S.ShowDialog();
            if (S.FileName != "")
            {
                FileStream F = (FileStream)S.OpenFile();
                string FilePath = S.FileName;
                Logger.WriteCSV(F, StringToWrite);
            }

            return;
        }
        #endregion

        #region Image Save Handler
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

        #region GUI Callbacks
        private void InvokeThreadCallback()
        {
            this.Invoke(new OnBackgroundThreadFinished(UpdateCalibratedSource));
        }

        private void UpdateCalibratedSource()
        {
            if (EL != null && EL.WasTestCompleted())
            {
                Radioactive_Source CalibratedSource = ListOfValidSources.Find(x => x.GetName() == EL.GetCalibratedSource().GetName());

                if ((CalibratedSource.GetSourceType() == Radioactive_Source.RadiationType.Alpha) || (CalibratedSource.GetSourceType() == Radioactive_Source.RadiationType.AlphaBeta))
                {
                    CalibratedSource.SetAnnualAlphaCPM(Convert.ToInt32(EL.GetAlphaCPM()));
                    this.AlphaEff = EL.GetAlphaEfficiency();
                    CalibratedSource.SetAlphaEfficiency(AlphaEff);

                    CalibratedSource.SetAlphaDisintigrationConstant(EL.GetAlphaDecayFactor());
                    
                }

                if ((CalibratedSource.GetSourceType() == Radioactive_Source.RadiationType.Beta) || (CalibratedSource.GetSourceType() == Radioactive_Source.RadiationType.AlphaBeta))
                {
                    CalibratedSource.SetAnnualBetaCPM(Convert.ToInt32(EL.GetBetaCPM()));
                    this.BetaEff = EL.GetBetaEfficiency();
                    CalibratedSource.SetBetaEfficiency(this.BetaEff);

                    CalibratedSource.SetBetaDisintigrationFactor(EL.GetBetaDecayFactor());
                }

                CalibratedSource.SetAnnualCalibratedDate(DateTime.Now);

                
                /*Write to file*/
                string FilePath = String.Format("{0}\\data\\Cal\\Eff\\{1}_{2}_{3}_{4}_{5}_Eff.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DABRAS.Serial_Number, EL.GetCalibratedSource().GetName());
                EL.RequestStop();
                EL = null;

                string[,] DataToWrite = MakeDataWritable(Calibration_Results_GridView);
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
            }

            else if (BL != null && BL.WasTestCompleted())
            {
                Radioactive_Source BackgroundSource = ListOfValidSources.Find(x =>  x.GetName() == "Background" );

                /*Type should always be AlphaBeta...*/
                if (BackgroundSource.GetSourceType() == Radioactive_Source.RadiationType.AlphaBeta)
                {
                    BackgroundSource.SetAnnualAlphaCPM(BL.GetAlphaBackground());
                    BackgroundSource.SetAnnualBetaCPM(BL.GetBetaBackground());
                    BackgroundSource.SetAlphaDisintigrationConstant(BL.GetAlphaDecayFactor());
                    BackgroundSource.SetBetaDisintigrationFactor(BL.GetBetaDecayFactor());
                }

                BL.RequestStop();
                BL = null;

                BackgroundSource.SetAnnualCalibratedDate(DateTime.Now);

                /*Write to file*/
                string FilePath = String.Format("{0}\\data\\Cal\\Bkgd\\{1}_{2}_{3}_{4}_Bkgd.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DABRAS.Serial_Number);
                string[,] DataToWrite = MakeDataWritable(Background_Results_GridView);
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

            }

            this.CalibrationCompleted = true;

            if (!DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = "Firmware Version: XX";
                this.DABRAS_SN_Label.Text = "Serial Number: XX";
                this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
            }

            SetGUI(false);

            this.NewSources = true;
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
                /*Too many get rid of a few*/
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

            /*Re-number the rows*/
            for (int i = 0; i < DG.RowCount - 2; i++)
            {
                DG[0, i].Value = i + 1;
            }
        }

        private DataGridView Make_Calibration_GridView(int NumSamples)
        {
            DataGridView Calibration_Results_Gridview = new DataGridView();

            Calibration_Results_GridView.Columns.Add("Calibration_Results_Source", this.Source_ComboBox.Text);
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Acq_Time", "Acq Time");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Alpha_Count", "Alpha Count");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Alpha_CPM", "Alpha CPM");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Beta_Count", "Beta Count");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Beta_CPM", "Beta CPM");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Alpha_Eff", "Alpha Efficiency");
            Calibration_Results_GridView.Columns.Add("Calibration_Results_Beta_Eff", "Beta Efficiency");

            /*Add one row for each count*/
            for (int i = 0; i < NumSamples; i++)
            {
                Calibration_Results_GridView.Rows.Add(new object[] { Convert.ToString((i + 1)), "", "", "", "", "", "0", "0" });

            }

            Calibration_Results_GridView.Rows.Add(new object[] { "Average", "", "", "0", "", "0", "0", "0" });
            Calibration_Results_GridView.Rows.Add(new object[] { "StDev", "", "", "", "", "", "0", "0" });

            return Calibration_Results_GridView;

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

        private void SelectCorrectBeta()
        {
            Radioactive_Source R = ListOfValidSources.Find(x => x.GetName() == Source_ComboBox.Text);

            if (R != null)
            {
                if (R.GetSourceType() != Radioactive_Source.RadiationType.Beta)
                {
                    this._100_200_Energy_Button.Enabled = false;
                    this._200_400_Energy_Button.Enabled = false;
                    this._400_1200_Energy_Button.Enabled = false;
                    this._1200_Energy_Button.Enabled = false;
                    return;
                }

                /*We have a beta source. Enable buttons*/

                this._100_200_Energy_Button.Enabled = true;
                this._200_400_Energy_Button.Enabled = true;
                this._400_1200_Energy_Button.Enabled = true;
                this._1200_Energy_Button.Enabled = true;

                Radioactive_Source.EnergyBand CurrentEnergyLevel = R.GetEnergyBand();

                switch (CurrentEnergyLevel)
                {
                    case Radioactive_Source.EnergyBand.Beta_100_200KeV:
                        this._100_200_Energy_Button.Checked = true;
                        this._200_400_Energy_Button.Checked = false;
                        this._400_1200_Energy_Button.Checked = false;
                        this._1200_Energy_Button.Checked = false;
                        return;
                    case Radioactive_Source.EnergyBand.Beta_200_400KeV:
                        this._100_200_Energy_Button.Checked = false;
                        this._200_400_Energy_Button.Checked = true;
                        this._400_1200_Energy_Button.Checked = false;
                        this._1200_Energy_Button.Checked = false;
                        return;
                    case Radioactive_Source.EnergyBand.Beta_400_1200KeV:
                        this._100_200_Energy_Button.Checked = false;
                        this._200_400_Energy_Button.Checked = false;
                        this._400_1200_Energy_Button.Checked = true;
                        this._1200_Energy_Button.Checked = false;
                        return;
                    case Radioactive_Source.EnergyBand.Beta_More_1200KeV:
                        this._100_200_Energy_Button.Checked = false;
                        this._200_400_Energy_Button.Checked = false;
                        this._400_1200_Energy_Button.Checked = false;
                        this._1200_Energy_Button.Checked = true;
                        return;
                    default:
                        this._100_200_Energy_Button.Checked = false;
                        this._200_400_Energy_Button.Checked = false;
                        this._400_1200_Energy_Button.Checked = false;
                        this._1200_Energy_Button.Checked = false;
                        return;
                    
                }
            }
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

        private void SetGUI(bool testRunning)
        {
            this.Determine_BG_Button.Enabled = !testRunning;
            this.Determine_Efficiency_Button.Enabled = !testRunning;
            this.backgroundCheckCtrlBToolStripMenuItem.Enabled = !testRunning;
            this.determineEfficiencyCtrlEToolStripMenuItem.Enabled = !testRunning;

            this.SaveBackgroundCalibrationButton.Enabled = !testRunning;
            this.SaveEfficiencyDataButton.Enabled = !testRunning;

            this.saveEfficiencyDataCtrlKToolStripMenuItem.Enabled = !testRunning;
            this.saveBackgroundDataCtrlJToolStripMenuItem.Enabled = !testRunning;

            this.ImageSaveButton.Enabled = !testRunning;
            this.saveImageCtrlIToolStripMenuItem.Enabled = !testRunning;

            this.stopAquisitionCtrlSToolStripMenuItem.Enabled = testRunning;
            this.Stop_Count_Button.Enabled = testRunning;

            this.highVoltagePlateauCtrlLToolStripMenuItem.Enabled = !testRunning;
            this.setHighVoltageCtrlHToolStripMenuItem.Enabled = !testRunning;
            this.connectOrDisconnectAPortCtrlPToolStripMenuItem.Enabled = !testRunning;

            this.establishAm241ToolStripMenuItem.Enabled = !testRunning;
            this.establishBackgroundHiLoLimitsToolStripMenuItem.Enabled = !testRunning;
            this.establishSrToolStripMenuItem.Enabled = !testRunning;

            this.viewEditCtrlOToolStripMenuItem.Enabled = !testRunning;
            this.addSourceCtrlAToolStripMenuItem.Enabled = !testRunning;
            this.deleteSourceCtrlDToolStripMenuItem.Enabled = !testRunning;

            this.Source_ComboBox.Enabled = !testRunning;
            this.Num_Counts_TB.Enabled = !testRunning;
            this.Minutes_TB.Enabled = !testRunning;
            this.Seconds_TB.Enabled = !testRunning;

            this.BG_NumCounts_TB.Enabled = !testRunning;
            this.Min_BG_TB.Enabled = !testRunning;
            this.Sec_BG_TB.Enabled = !testRunning;

            return;
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

        #region Misc GUI
        private void Num_Counts_TB_TextChanged(object sender, EventArgs e)
        {
            int NewNumberOfRows = 0;
            try
            {
                NewNumberOfRows = Convert.ToInt32(Num_Counts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Bad Value.");
                return;
            }

            Add_Or_Subtract_Rows(NewNumberOfRows, this.Calibration_Results_GridView);
        }

        private void BG_NumCounts_TB_TextChanged(object sender, EventArgs e)
        {
            int NewNumberOfRows = 0;
            try
            {
                NewNumberOfRows = Convert.ToInt32(this.BG_NumCounts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Bad Value.");
                return;
            }

            Add_Or_Subtract_Rows(NewNumberOfRows, this.Background_Results_GridView);
        }

        private void Source_ComboBox_Click(object sender, EventArgs e)
        {
            Source_ComboBox.Items.Clear();
            foreach (Radioactive_Source i in ListOfValidSources)
            {
                if (String.Compare(i.GetName(), "Background") != 0)
                {
                    Source_ComboBox.Items.Add(i.GetName());
                }
            }
        }

        private void Source_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            Radioactive_Source R = ListOfValidSources.Find(x => x.GetName() == Source_ComboBox.Text);

            /*Change labels to match radiation type*/
            if (R != null)
            {
                this.TypeOfRadiationLabel1.Text = R.GetSourceType_String();
                this.TypeOfRadiationLabel2.Text = R.GetSourceType_String();
            }

            SelectCorrectBeta();
            Calibration_Results_GridView.Columns[0].HeaderText = R.GetName();
        }

        private void _100_200_Energy_Button_CheckedChanged(object sender, EventArgs e)
        {
            SelectCorrectBeta();
        }

        private void _200_400_Energy_Button_CheckedChanged(object sender, EventArgs e)
        {
            SelectCorrectBeta();
        }

        private void _1200_Energy_Button_CheckedChanged(object sender, EventArgs e)
        {
            SelectCorrectBeta();
        }

        private void _400_1200_Energy_Button_CheckedChanged(object sender, EventArgs e)
        {
            SelectCorrectBeta();
        }
        #endregion

        #region Stop Button Handler
        private void Stop_Count_Button_Click(object sender, EventArgs e)
        {
            if (BL != null)
            {
                BL.RequestStop();
            }

            if (EL != null)
            {
                EL.RequestStop();
            }

            SetGUI(false);
        }
        #endregion

        #region Close Button Handler
        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion

        #region View/Edit Calibration Sources Handler
        private void viewEditCtrlOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewEditSources NewForm = new ViewEditSources(this, this.ListOfValidSources);
            this.ChildDialogue = NewForm;
            NewForm.Show(this);
            this.Hide();

            //if (NewForm.WereSourcesModified())
            //{
            //    this.ListOfValidSources = NewForm.GetRadioActiveSourceList();
            //    this.NewSources = true;
            //}

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region Add Source Handler
        private void addSourceCtrlAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSource NewForm = new AddSource(this, this.ListOfValidSources);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.WasSourceWritten())
            //{
            //    this.ListOfValidSources = NewForm.GetNewList();
            //    this.NewSources = true;
            //}

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region Delete Source Handler
        private void deleteSourceCtrlDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveSource NewForm = new RemoveSource(this, this.ListOfValidSources);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.WasListChanged())
            //{
            //    this.ListOfValidSources = NewForm.GetNewList();
            //    this.NewSources = true;
            //}

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region HiLo Handlers

        #region HiLo Background Handler
        private void establishBackgroundHiLoLimitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HiLo NewForm = new HiLo(this, HiLo.TypeOfHiLo.BACKGROUND);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.WasDABRASModified())
            //{
            //    this.DABRAS = NewForm.GetDABRAS();
            //    this.NewDABRAS = true;
            //}

            //if (NewForm.WasSourceListModified())
            //{
            //    this.ListOfValidSources = NewForm.GetModifiedSources();
            //    this.NewSources = true;
            //}

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region Am241 HiLo Handler
        private void establishAm241ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HiLo NewForm = new HiLo(this, HiLo.TypeOfHiLo.ALPHA);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.WasDABRASModified())
            //{
            //    this.DABRAS = NewForm.GetDABRAS();
            //}

            //if (NewForm.WasSourceListModified())
            //{
            //    this.ListOfValidSources = NewForm.GetModifiedSources();
            //    this.NewSources = true;
            //}

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region Sr90 HiLo Handler
        private void establishSrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HiLo NewForm = new HiLo(this, HiLo.TypeOfHiLo.BETA);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.WasDABRASModified())
            //{
            //    this.DABRAS = NewForm.GetDABRAS();
            //}

            //if (NewForm.WasSourceListModified())
            //{
            //    this.ListOfValidSources = NewForm.GetModifiedSources();
            //    this.NewSources = true;
            //}

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #endregion

        #region Port Connect Handlers
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
                return;
            }

            if (NewPopup.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }
        #endregion

        /**/
        #region HV Plateau Handler
        /*TODO: Pass back data?!*/
        private void highVoltagePlateauCtrlLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HighVoltagePlateau NewForm = new HighVoltagePlateau(this);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }


        #endregion

        #region Set HV Handler
        private void setHighVoltageCtrlHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DABRAS.IsConnected())
            {
                SetHighVoltageForm NewForm = new SetHighVoltageForm(this.DABRAS);
                NewForm.ShowDialog();

                if (NewForm.DialogResult == DialogResult.Abort)
                {
                    AbortAll();
                }
            }
            else
            {
                MessageBox.Show("Error: A valid connection the DABRAS is needed. Please re-connect and try again.");
                return;
            }
        }
        #endregion

        #region Chi Squared Handler
        private void setChiSquaredCalibrationRequirementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChiSquaredCalibrationRequirements NewForm = new ChiSquaredCalibrationRequirements(this.DC);
            NewForm.ShowDialog();

            if (NewForm.DialogResult == DialogResult.OK)
            {
                this.DC = NewForm.GetDefaultConfigurations();
                this.DCModified = true;
            }
        }
        #endregion

        #region Change Password Handler
        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangePasswordForm NewForm = new ChangePasswordForm(this.DC);
            NewForm.ShowDialog();

            if (NewForm.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }
        #endregion

        #region Background Type Handler
        private void setBackgroundTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackgroundTypeForm NewForm = new BackgroundTypeForm(mainFramework.BackgroundType.Annual);

            NewForm.ShowDialog();

            if (NewForm.DialogResult == DialogResult.OK)
            {
                this.DC.SetRoutineCalibrationBackgroundType(NewForm.GetBackgroundType());
                this.NewDABRAS = true;
            }

            if (NewForm.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }
        #endregion

        #region Mod Factors Handler
        private void setModFactorRequirementsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModFactorForm MF = new ModFactorForm(DC.GetModFactors());
            MF.ShowDialog();
            if (MF.DialogResult == DialogResult.OK)
            {
                DC.SetModFactors(MF.GetModFactors());
            }

            if (MF.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }
        #endregion

        #region Building Number Handler
        private void setBuildingNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuildingNumber BN = new BuildingNumber(this.DC);
            BN.ShowDialog();

            if (BN.DialogResult == DialogResult.Abort)
            {
                AbortAll();
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

        #region PDF Handler
        private void displayCalibrationProceduresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string CurrentEnv = Environment.CurrentDirectory;
            CurrentEnv = String.Concat(CurrentEnv, "\\PROCEDURE.pdf");

            /*Show non-modal!*/
            PDFViewer NewForm = new PDFViewer(CurrentEnv);
            NewForm.Show();
        }
        #endregion

        #region WebForm Handler
        private void openWebBasedSurveySystemF12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DC.GetWebSurvey());
            NewForm.Show();
        }

        private void openRSOSharepointToolStripMenuItem_Click(object sender, EventArgs e)
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

        #region Dummy Overloads
        private void determineEfficiencyCtrlEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Determine_Efficiency_Button_Click(this, null);
        }

        private void backgroundCheckCtrlBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Determine_BG_Button_Click(this, null);
        }

        private void stopAquisitionCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop_Count_Button_Click(this, null);
        }

        private void saveEfficiencyDataCtrlKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.SaveEfficiencyDataButton.Enabled)
            {
                this.SaveEfficiencyDataButton_Click(this, null);
            }
        }

        private void saveBackgroundDataCtrlJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.SaveBackgroundCalibrationButton.Enabled)
            {
                this.saveBackgroundDataCtrlJToolStripMenuItem_Click(this, null);
            }
        }

        private void saveImageCtrlIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ImageSaveButton.Enabled)
            {
                this.ImageSaveButton_Click(this, null);
            }
        }
        #endregion

        #region KeyPress Handler
        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                AbortAll();
                return;
            }
            
            if ((Key.Control && Key.Alt))
            {
                /*DELETE!...or not*/
                if (Key.KeyCode == Keys.B)
                {
                    if (establishBackgroundHiLoLimitsToolStripMenuItem.Enabled)
                    {
                        establishBackgroundHiLoLimitsToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.A)
                {
                    if (establishAm241ToolStripMenuItem.Enabled)
                    {
                        establishAm241ToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.S)
                {
                    if (establishSrToolStripMenuItem.Enabled)
                    {
                        establishSrToolStripMenuItem_Click(this, null);
                    }
                    return;
                }
            }
            
            if (Key.Control)
            {
                

                if (Key.KeyCode == Keys.Q)
                {
                    closeCtrlQToolStripMenuItem_Click(this, null);
                    return;
                }

                if (Key.KeyCode == Keys.E)
                {
                    if (Determine_Efficiency_Button.Enabled)
                    {
                        determineEfficiencyCtrlEToolStripMenuItem_Click(this, null);
                    }
                    
                    return;
                }

                if (Key.KeyCode == Keys.B)
                {
                    if (Determine_BG_Button.Enabled)
                    {
                        backgroundCheckCtrlBToolStripMenuItem_Click(this, null);
                    }

                    return;
                }

                if (Key.KeyCode == Keys.S)
                {
                    if (Stop_Count_Button.Enabled)
                    {
                        stopAquisitionCtrlSToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.O)
                {
                    if (viewEditCtrlOToolStripMenuItem.Enabled)
                    {
                        viewEditCtrlOToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.A)
                {
                    if (addSourceCtrlAToolStripMenuItem.Enabled)
                    {
                        addSourceCtrlAToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.D)
                {
                    if (deleteSourceCtrlDToolStripMenuItem.Enabled)
                    {
                        deleteSourceCtrlDToolStripMenuItem_Click(this, null);
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

                if (Key.KeyCode == Keys.L)
                {
                    if (highVoltagePlateauCtrlLToolStripMenuItem.Enabled)
                    {
                        highVoltagePlateauCtrlLToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.H)
                {
                    if (setHighVoltageCtrlHToolStripMenuItem.Enabled)
                    {
                        setHighVoltageCtrlHToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.K)
                {
                    if (SaveEfficiencyDataButton.Enabled)
                    {
                        SaveEfficiencyDataButton_Click(this, null);
                    }
                }

                if (Key.KeyCode == Keys.J)
                {
                    if (SaveBackgroundCalibrationButton.Enabled)
                    {
                        SaveBackgroundCalibrationButton_Click(this, null);
                    }
                }

                if (Key.KeyCode == Keys.I)
                {
                    if (ImageSaveButton.Enabled)
                    {
                        ImageSaveButton_Click(this, null);
                    }
                }

            }

            if (Key.KeyCode == Keys.F12)
            {
                openWebBasedSurveySystemF12ToolStripMenuItem_Click(this, null);
            }

            if (Key.KeyCode == Keys.F11)
            {
                openRSOSharepointToolStripMenuItem_Click(this, null);
            }

            if (Key.KeyCode == Keys.F10)
            {
                openRSOHomeF10ToolStripMenuItem_Click(this, null);
            }
            
        }
        #endregion

        #region Calibration Controller Functions
        public bool AutoCalibrateSourceEfficiency(object sender, List<Radioactive_Source> ListOfCalibratedSources, string SourceName)
        {
            /*Check for correct usage*/
            if (!(sender is QuickCalibrationController))
            {
                return false;
            }

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
            this.Num_Counts_TB.Text = "10";
            this.Minutes_TB.Text = "1";
            this.Seconds_TB.Text = "0";

            /*Click buttons*/
            Determine_Efficiency_Button_Click(this, null);
            return true;
        }

        #endregion

        #region Getters
        public double GetAlphaEff()
        {
            return this.AlphaEff;
        }

        public double GetBetaEff()
        {
            return this.BetaEff;
        }

        public bool WasCalCompleted()
        {
            return this.CalibrationCompleted;
        }

        public DefaultConfigurations GetDefaultConfig()
        {
            return this.DC;
        }

        public bool WasCalibrationCompleted()
        {
            return this.CalibrationCompleted;
        }

        private List<Radioactive_Source> GetListOfSources()
        {
            return this.ListOfValidSources;
        }

        public bool WereSourcesModified()
        {
            return this.NewSources;
        }

        public List<Radioactive_Source> GetSourceList()
        {
            return this.ListOfValidSources;
        }

        public bool WasDABRASModified()
        {
            return this.NewDABRAS;
        }

        public DABRAS GetDABRAS()
        {
            return DABRAS;
        }

        public Logger GetLogger()
        {
            return Logger;
        }

        public bool WasNewPasswordSet()
        {
            return this.NewPasswordSet;
        }

        public string GetNewPassword()
        {
            return this.NewPassword;
        }

        public bool WasDCModified()
        {
            return this.DCModified;
        }
        #endregion

        #region Show/Hide Handler
        private void CalibrationForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.ChildDialogue != null)
            {
                if (this.ChildDialogue.DialogResult == DialogResult.Abort)
                {
                    AbortAll();
                }

                if (this.ChildDialogue.DialogResult != DialogResult.None)
                {
                    this.ChildDialogue = null;
                }
            }

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

        #region Finalization
        private void CalibrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (EL != null)
            {
                EL.RequestStop();
                EfficiencyBackgroundThread.Join();
            }

            if (BL != null)
            {
                BL.RequestStop();
                BackgroundBackgroundThread.Join();
            }

            frmParent.Show();
        }
        #endregion

    }


    public class BackgroundListener
    {
        #region Data Members
        /*All data members are private - we don't want other classes to have the ability to modify the test once it's been started*/
        private bool Done;
        private DataGridView Background_Table;
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
        public BackgroundListener(DABRAS _DABRAS, int _SampleTime, int _NumSamples, DataGridView _CalTable, Radioactive_Source _BG)
        {
            this.DABRAS = _DABRAS;
            this.Background_Table = _CalTable;
            this.SampleTime = _SampleTime;
            this.NumSamples = _NumSamples;
            this.BG = _BG;
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
        public void Get_Background()
        {
            /*Set flag for running*/
            Running = true;
            /*Pause any background monitors*/
            DABRAS.Cut();

            /*Set aquisition time*/
            /*See the DABRAS Serial protocol for information about how to use this command*/
            DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
            Thread.Sleep(500); //needed for stability

            /*Clear any data left in the buffer*/
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();

            /*Start the watchdog - Because the serialport API doesn't give us an event for a hardware disconnection, we will need to manually watch for it*/
            DABRAS.EnableWatchdog();

            /*Do not increment the row index until the current sample time has elapsed*/
            for (int i = 0; i < NumSamples; i++)
            {
                bool RowComplete = false;
                DABRAS.Write_To_Serial_Port("g");

                /*Check for the first good packet*/
                while (!ShouldStop)
                {
                    /*Whenever pausing for a point in time, we need to try-catch it. If the watchdog times out, then we will need to throw an exception*/
                    try
                    {
                        while (!DABRAS.IsDataReady() && !ShouldStop)
                        {
                            Thread.Sleep(100); //Allow other threads to access CPU, but keep on relatively tight leash.
                            if (!DABRAS.IsConnected()) //this is what happens if the watchdog times out.
                            {
                                throw new TimeoutException(); //Throw an exception if the hardware detatches.
                            }
                        }
                    }
                    catch
                    {
                        /*The hardware has detatched...exit abnormally.*/
                        MessageBox.Show("Error: Connection Lost.");
                        DABRAS.DisableWatchdog();
                        BackgroundSampleThreadFinished(this, null);
                        return;
                    }

                    /*We got something! Read in the packet*/
                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();
                    
                    /*Reset the watchdog timer*/
                    DABRAS.KickWatchdog(); 

                    /*Break the while loop if we get a packet with the correctly set sample time, and a zero start time.*/
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
                
                while (!RowComplete && !ShouldStop)
                {
                    /*Wait for incoming data packet*/
                    /*Same Try-Catch pattern as before*/
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
                        BackgroundSampleThreadFinished(this, null);
                        return;
                    }

                    /*We made it! Read in packet and kick watchdog*/
                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();
                    DABRAS.KickWatchdog();

                    /*Grab handles to form*/
                    if (!ShouldStop)
                    {
                        /*Update the i-th row*/
                        DataGridViewCell TimeElapsed = Background_Table[1, i];
                        DataGridViewCell AlphaTot = Background_Table[2, i];
                        DataGridViewCell AlphaCPM = Background_Table[3, i];
                        DataGridViewCell BetaTot = Background_Table[4, i];
                        DataGridViewCell BetaCPM = Background_Table[5, i];


                        /*Parse data to form*/
                        TimeElapsed.Value = IncomingData.ElTime;

                        if (IncomingData != null && IncomingData.ElTime != 0)
                        {
                            /*Standard computations*/
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
                        Background_Table.Invalidate();

                        /*If the sample time has elapsed, increment the row.*/
                        if (IncomingData != null && IncomingData.ElTime >= SampleTime)
                        {
                            RowComplete = true;
                        }
                    }

                }
            }

            /*We are now done with the hardware. Disable the timer.*/
            DABRAS.DisableWatchdog();

            if (!ShouldStop)
            {
                /*Compute averages*/
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

                /*Grab references to the datagridview and update*/
                DataGridViewCell AverageAlphaCell = Background_Table[3, NumSamples];
                DataGridViewCell AverageBetaCell = Background_Table[5, NumSamples];
                DataGridViewCell StdDevAlphaCell = Background_Table[3, NumSamples + 1];
                DataGridViewCell StdDevBetaCell = Background_Table[5, NumSamples + 1];

                /*StaticMethods.RoundToSigFigs() is an efficient method to round as described in tech note 100*/
                /*3 sig figs for a value > 100, 2 for <100*/
                AverageAlphaCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaCPM);
                AverageBetaCell.Value = StaticMethods.RoundToSigFigs(AverageBetaCPM);

                this.AlphaCPM = Convert.ToInt32(AverageAlphaCPM);
                this.BetaCPM = Convert.ToInt32(AverageBetaCPM);

                /*Compute Standard Deviations*/
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
                
                /*Sub in Poisson Statistics if necessary*/
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
                
                BG.SetAnnualCalibratedTimespan(NumSamples * SampleTime);
                
                /*Compute decay factor for the daily QC chart, as per MARLAP*/
                this.AlphaDFactor = BG.ComputeDecayFactor(ListOfAlphaCPM, this.SampleTime);
                this.BetaDFactor = BG.ComputeDecayFactor(ListOfBetaCPM, this.SampleTime);
                
                /*Set time finished*/
                this.BackgroundFinished = DateTime.Now;
                WasBackgroundFinishedSuccessfully = true;

                /*Play an alert sound*/
                using (SoundPlayer S = new SoundPlayer(DABRAS_Software.Properties.Resources.DoneSound))
                {
                    S.Play();
                }

                /*Fire an event if someone is listening
                 * The null check is needed - if the form is closed and (somehow) we get here, BackgroundSampleThreadFinished will be null.
                 * This will throw an exception if we fire the event.
                 */
                if (BackgroundSampleThreadFinished != null)
                {
                    BackgroundSampleThreadFinished(this, null);
                }
            }

            /*Reinstate background monitors, if they existed*/
            DABRAS.ResumeBackgroundMonitors();
            Running = false;

            return;
        }
        #endregion
    }

    public class EfficiencyListener
    {
        #region Data Members
        /*All data members private - We don't want other classes modifying the test after it has been created*/
        private bool Done;
        private DataGridView Calibration_Table;
        private int SampleTime;
        private int NumSamples;
        private DABRAS DABRAS;
        private bool ShouldStop = false;
        private Radioactive_Source R;
        private bool WasBackgroundFinishedSuccessfully = false;
        private DateTime EfficiencyFinished;

        private double AlphaEfficiency;
        private double BetaEfficiency;
        private double AlphaCPM;
        private double BetaCPM;

        private double AlphaBG;
        private double BetaBG;

        private double AlphaDFactor;
        private double BetaDFactor;
        
        public event EventHandler EfficiencyBackgroundThreadFinished;

        private bool Running = false;
        #endregion

        #region Constructor
        public EfficiencyListener(DABRAS _DABRAS, int _SampleTime, int _NumSamples, DataGridView _CalTable, Radioactive_Source _R, double _AlphaBG, double _BetaBG)
        {
            this.DABRAS = _DABRAS;
            this.Calibration_Table = _CalTable;
            this.SampleTime = _SampleTime;
            this.NumSamples = _NumSamples;
            this.R = _R;
            this.AlphaBG = _AlphaBG;
            this.BetaBG = _BetaBG;
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

        public double GetAlphaCPM()
        {
            return this.AlphaCPM;
        }

        public double GetBetaCPM()
        {
            return this.BetaCPM;
        }

        public DateTime GetDateTimeCompleted()
        {
            return this.EfficiencyFinished;
        }

        public bool WasTestCompleted()
        {
            return this.WasBackgroundFinishedSuccessfully;
        }
        #endregion

        #region Main Background Thread
        public void Get_Efficiency()
        {
            /*Set the flag for running*/
            Running = true;
            /*Compute the number of half lives that have passed since certification*/
            DateTime Today = DateTime.Now;
            DateTime CertDate = Convert.ToDateTime(R.GetCertificaitonDate());

            /*Computing the disintigration decay value*/
            TimeSpan ElapsedTime = Today.Subtract(CertDate);
            ulong ElapsedSeconds = Convert.ToUInt64(ElapsedTime.TotalSeconds);

            double HalfLifeMultiplier = Math.Pow(0.5, Convert.ToDouble(ElapsedSeconds) / Convert.ToDouble(R.GetHalfLife()));

            double ExpectedDPM = Convert.ToDouble(R.GetCertifiedActivity()) * R.GetDecayFactor(DateTime.Now);

            /*Clear any background threads, if they exist*/
            DABRAS.Cut();

            /*Set aquisition time*/
            DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
            Thread.Sleep(500);

            /*Clear any data left in the buffer*/
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();

            /*Start the watchdog - Because the serialport API doesn't give us an event for a hardware disconnection, we will need to manually watch for it*/
            DABRAS.EnableWatchdog();

            for (int i = 0; i < NumSamples; i++)
            {

                bool RowComplete = false;
                DABRAS.Write_To_Serial_Port("g");

                /*Check for the first good packet*/
                while (!ShouldStop)
                {
                    /*Whenever pausing for a point in time, we need to try-catch it. If the watchdog times out, then we will need to throw an exception*/
                    try
                    {
                        while (!DABRAS.IsDataReady() && !ShouldStop)
                        {
                            Thread.Sleep(100); //Spin for awhile, allowing other threads to get processing time. Don't wait too long, however.
                            if (!DABRAS.IsConnected()) //This will be changed if the watchdog times out
                            {
                                throw new TimeoutException(); //If the hardware detatches, we want to throw an exception.
                            }
                        }
                    }
                    catch
                    {
                        /*the hardware detatched, terminate abnormally*/
                        MessageBox.Show("Error: Connection Lost.");
                        DABRAS.DisableWatchdog();
                        EfficiencyBackgroundThreadFinished(this, null);
                        return;
                    }

                    /*We got a packet! Read it*/
                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                    DABRAS.KickWatchdog();

                    if (!ShouldStop)
                    {
                        /*If we get a packet with the sample time set correctly, and the elapsed time at zero, we are good to go - continue!*/
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
                    /*Wait for incoming data packet*/
                    /*Use the same try-catch pattern as before*/
                    try
                    {
                        while (!DABRAS.IsDataReady() && !ShouldStop)
                        {
                            Thread.Sleep(100);
                            if (!DABRAS.IsConnected())
                            {
                                throw new TimeoutException(); //this gets thrown if the hardware detatches.
                            }
                        }
                    }
                    catch
                    {
                        /*The hardware detatched, terminate abnormally*/
                        MessageBox.Show("Error: Connection lost.");
                        DABRAS.DisableWatchdog();
                        EfficiencyBackgroundThreadFinished(this, null);
                        return;
                    }

                    /*Read in packet*/
                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                    /*Grab handles to form*/
                    if (!ShouldStop)
                    {
                        /*Update the i-th row*/
                        DataGridViewCell TimeElapsed = Calibration_Table[1, i];
                        DataGridViewCell AlphaTot = Calibration_Table[2, i];
                        DataGridViewCell AlphaCPM = Calibration_Table[3, i];
                        DataGridViewCell BetaTot = Calibration_Table[4, i];
                        DataGridViewCell BetaCPM = Calibration_Table[5, i];
                        DataGridViewCell AlphaEff = Calibration_Table[6, i];
                        DataGridViewCell BetaEff = Calibration_Table[7, i];

                        /*Parse data to form*/
                        TimeElapsed.Value = IncomingData.ElTime;
                        if (IncomingData != null && IncomingData.ElTime != 0)
                        {

                            AlphaTot.Value = IncomingData.AlphaTot;
                            AlphaCPM.Value = StaticMethods.RoundToDecimal(IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60) - AlphaBG, 1); //Convert raw counts into CPM. +1 to avoid DBZ, PIC uses zero based counting.
                            BetaTot.Value = IncomingData.BetaTot;
                            BetaCPM.Value = StaticMethods.RoundToDecimal(IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60) - BetaBG, 1);
                            /*Efficiencies are ALWAYS three sig figs!*/
                            AlphaEff.Value = StaticMethods.RoundToSpecificSigFigs((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60) - AlphaBG) * 100 / ExpectedDPM, 3);
                            BetaEff.Value = StaticMethods.RoundToSpecificSigFigs((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60) - BetaBG) * 100 / ExpectedDPM, 3);

                        }
                        /*Re-draw table*/
                        Calibration_Table.Invalidate();

                        /*Kick the dog - reset the watchdog timer*/
                        DABRAS.KickWatchdog();

                        if (IncomingData != null && (IncomingData.ElTime >= SampleTime))
                        {
                            RowComplete = true; //move to the next row when this test finishes.
                        }
                    }
                }

            }

            /*We are now done with the hardware, diable the timer*/
            DABRAS.DisableWatchdog();
            
            if (!ShouldStop)
            {
                /*Compute averages*/
                double AverageAlphaCPM = 0;
                double AverageBetaCPM = 0;
                double AverageAlphaEff = 0;
                double AverageBetaEff = 0;

                /*Make lists for decay factor calculation*/

                var ListOfAlphaCPM = new List<double>();
                var ListOfBetaCPM = new List<double>();

                for (int i = 0; i < NumSamples; i++)
                {
                    AverageAlphaCPM += Convert.ToDouble(Calibration_Table[3, i].Value);
                    ListOfAlphaCPM.Add(Convert.ToDouble(Calibration_Table[3, i].Value));

                    AverageBetaCPM += Convert.ToDouble(Calibration_Table[5, i].Value);
                    ListOfBetaCPM.Add(Convert.ToDouble(Calibration_Table[5, i].Value));
                    
                    AverageAlphaEff += Convert.ToDouble(Calibration_Table[6, i].Value);
                    AverageBetaEff += Convert.ToDouble(Calibration_Table[7, i].Value);
                }

                AverageAlphaCPM /= NumSamples;
                AverageBetaCPM /= NumSamples;
                AverageAlphaEff /= NumSamples;
                AverageBetaEff /= NumSamples;

                DataGridViewCell AverageAlphaCell = Calibration_Table[3, NumSamples];
                DataGridViewCell AverageBetaCell = Calibration_Table[5, NumSamples];
                DataGridViewCell AverageAlphaEffCell = Calibration_Table[6, NumSamples];
                DataGridViewCell AverageBetaEffCell = Calibration_Table[7, NumSamples];
                DataGridViewCell StdDevAlphaCell = Calibration_Table[6, NumSamples + 1];
                DataGridViewCell StdDevBetaCell = Calibration_Table[7, NumSamples + 1];


                AverageAlphaCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaCPM);
                AverageBetaCell.Value = StaticMethods.RoundToSigFigs(AverageBetaCPM);
                AverageAlphaEffCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaEff);
                AverageBetaEffCell.Value = StaticMethods.RoundToSigFigs(AverageBetaEff);

                this.AlphaEfficiency = AverageAlphaEff;
                this.BetaEfficiency = AverageBetaEff;
                this.AlphaCPM = AverageAlphaCPM;
                this.BetaCPM = AverageBetaCPM;

                /*Compute Standard Deviations*/
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
                    StdDevAlpha = Math.Sqrt(AverageAlphaCPM);
                    StdDevBeta = Math.Sqrt(AverageBetaCPM);
                }

                /*Sub in Poisson Statistics if necessary*/
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
                
                /*Compute Decay Value*/
                this.AlphaDFactor = R.ComputeDecayFactor(ListOfAlphaCPM, this.SampleTime);
                this.BetaDFactor = R.ComputeDecayFactor(ListOfBetaCPM, this.SampleTime);

                /*Set efficiency finished field*/
                this.EfficiencyFinished = DateTime.Now;

                /*Set the finished property*/
                WasBackgroundFinishedSuccessfully = true;

                /*Play a sound
                 * Note the use of the using() block - avoid memory leaks!*/
                using (SoundPlayer S = new SoundPlayer(DABRAS_Software.Properties.Resources.DoneSound))
                {
                    S.Play();
                }

                /*Fire an event if someone is listening
                 * The null check is needed - if the form is closed and (somehow) we get here, BackgroundSampleThreadFinished will be null.
                 * This will throw an exception if we fire the event.
                 */
                if (EfficiencyBackgroundThreadFinished != null)
                {
                    EfficiencyBackgroundThreadFinished(this, null);
                }

            }

            /*Reinstate background threads, if they existed*/
            DABRAS.ResumeBackgroundMonitors();
            Running = false;
            return;
        }
        #endregion
    }
}