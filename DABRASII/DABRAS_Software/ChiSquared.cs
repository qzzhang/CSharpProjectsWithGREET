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
    public partial class ChiSquared : Form
    {
        #region Data Members
        private DABRAS DABRAS;
        private bool NewDABRAS = false;
        private Logger Logger;
        private DefaultConfigurations DC;
        private bool NewDC = false;
        private List<Radioactive_Source> ListOfSources;


        private ChiSquaredListener CL;
        private Thread BackgroundThread;

        private readonly mainFramework frmParent;

        public delegate void UpdateGUICallback();
        #endregion

        #region Constructor
        public ChiSquared(mainFramework Parent)
        {
            InitializeComponent();

            this.frmParent = Parent;

            this.DABRAS = Parent.GetDABRAS();
            this.Logger = Parent.GetLogger();
            this.DC = Parent.GetDefaultConfigurations();
            this.ListOfSources = Parent.GetListOfSources();

            #region DataGridView Initialization

            ChiSquaredDataGridView.Columns.Add("Null_Corner", "");
            ChiSquaredDataGridView.Columns.Add("Background_Results_Acq_Time", "Acq Time");
            ChiSquaredDataGridView.Columns.Add("Background_Results_Alpha_Count", "Alpha Count");
            ChiSquaredDataGridView.Columns.Add("Background_Results_Alpha_CPM", "Alpha CPM");
            ChiSquaredDataGridView.Columns.Add("Background_Results_Beta_Count", "Beta Count");
            ChiSquaredDataGridView.Columns.Add("Background_Results_Beta_CPM", "Beta CPM");

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
                ChiSquaredDataGridView.Rows.Add(new object[] { Convert.ToString((i + 1)), "", "", "", "", "", "", "" });

            }

            ChiSquaredDataGridView.Rows.Add(new object[] { "Average", "", "", "", "", "", "", "" });
            ChiSquaredDataGridView.Rows.Add(new object[] { "StDev", "", "", "", "", "", "", "" });
            ChiSquaredDataGridView.Rows.Add(new object[] { "Chi²", "", "", "", "", "", "", "" });
            ChiSquaredDataGridView.Rows.Add(new object[] { "P Value", "", "", "", "", "", "", "" });


             #endregion

            if (DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = "Firmware Version: " + DABRAS.Firmware_Version;
                this.DABRAS_SN_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                this.DABRAS_Status_Label.Text = "STATUS: Connected!";
            }

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Private Utility Functions
        private void Add_Or_Subtract_Rows(int FinalNumberOfRows, DataGridView DG)
        {
            FinalNumberOfRows += 5;
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
            for (int i = 0; i < DG.RowCount - 5; i++)
            {
                DG[0, i].Value = i + 1;
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

        #endregion

        #region Misc GUI Functions
        private void Num_Counts_TB_TextChanged(object sender, EventArgs e)
        {
            int NewNumberOfRows = 0;
            try
            {
                NewNumberOfRows = Convert.ToInt32(this.Num_Counts_TB.Text);
            }

            catch
            {
                MessageBox.Show("Error: Bad value.");
                return;
            }

            Add_Or_Subtract_Rows(NewNumberOfRows, this.ChiSquaredDataGridView);
            ChiSquaredDataGridView.Invalidate();

            return;
        }

        private void Set_GUI(bool Running)
        {
            this.AquireButton.Enabled = !Running;
            this.aquireCtrlAToolStripMenuItem.Enabled = !Running;
            this.SaveButton.Enabled = !Running;
            this.saveDataToFileCtrlVToolStripMenuItem.Enabled = !Running;
            this.ImageSaveButton.Enabled = !Running;
            this.saveImageCtrlIToolStripMenuItem.Enabled = !Running;

            this.StopButton.Enabled = Running;
            this.stopCtrlSToolStripMenuItem.Enabled = Running;

            this.Num_Counts_TB.Enabled = !Running;
            this.Min_TB.Enabled = !Running;
            this.Sec_TB.Enabled = !Running;
            this.Alpha_ChiSq_TB.Enabled = !Running;
            this.Beta_ChiSq_TB.Enabled = !Running;
            this.AlphaPValue_TB.Enabled = !Running;
            this.BetaPValue_TB.Enabled = !Running;
            this.AlphaSourceCB.Enabled = !Running;
            this.BetaSourceCB.Enabled = !Running;

            return;
        }
        #endregion

        #region Aquire Button Handler
        private void AquireButton_Click(object sender, EventArgs e)
        {
            int SampleTime = 0;
            int NumberOfSamples = 0;

            try
            {
                NumberOfSamples = Convert.ToInt32(this.Num_Counts_TB.Text);
                SampleTime = (60 * Convert.ToInt32(this.Min_TB.Text)) + Convert.ToInt32(this.Sec_TB.Text);
            }

            catch
            {
                MessageBox.Show("Error: Bad values.");
                return;
            }

            if (!DABRAS.IsConnected())
            {
                MessageBox.Show("Error: Must be connected to the DABRAS to perform Chi Squared test. Please re-connect and try again.");
                return;
            }

            ClearDataGridView(this.ChiSquaredDataGridView);
            this.Alpha_ChiSq_TB.Text = "";
            this.Beta_ChiSq_TB.Text = "";

            Radioactive_Source BG = ListOfSources.Find(x => x.GetName() == "Background");

            CL = new ChiSquaredListener(this.DABRAS, SampleTime, NumberOfSamples, this.ChiSquaredDataGridView, BG, this.AlphaSourceCB.Checked, this.BetaSourceCB.Checked);
            BackgroundThread = new Thread(() => CL.Get_ChiSq());

            CL.ChiSqThreadFinished += (s, args) => { InvokeCallback(); };
            
            Set_GUI(true);

            BackgroundThread.Start();
        }
        #endregion

        #region GUI Callbacks
        private void InvokeCallback()
        {
            this.Invoke(new UpdateGUICallback(Update_GUI));
            return;
        }

        private void Update_GUI()
        {
            if (CL.WasTestCompleted())
            {
                this.Alpha_ChiSq_TB.Text = StaticMethods.RoundToSigFigs(CL.GetAlphaChiSq());
                this.Beta_ChiSq_TB.Text = StaticMethods.RoundToSigFigs(CL.GetBetaChiSq());

                this.AlphaPValue_TB.Text = StaticMethods.RoundToSigFigs(CL.GetAlphaPValue());
                this.BetaPValue_TB.Text = StaticMethods.RoundToSigFigs(CL.GetBetaPValue());

                if (CL.WasTestPassed())
                {
                    DC.SetChiSquaredDateTime(DateTime.Now);
                    this.NewDC = true;

                    if (DC.GetChiSquaredTimespan() != TimeSpan.MaxValue)
                    {
                        DateTime FinalTime = DateTime.Now.Add(DC.GetChiSquaredTimespan());
                        MessageBox.Show(String.Format("Chi Squared Calibration successfully completed! Next calibration due at {0}", FinalTime));
                    }

                    else
                    {
                        MessageBox.Show("Chi Squared Calibration successfully completed!");
                    }
                }

                else
                {
                    MessageBox.Show("Chi Squared calibration failed.");
                }
            }

            Set_GUI(false);

            /*Write to file*/
            string FilePath = String.Format("{0}\\data\\QC\\ChiSq\\{1}_{2}_{3}_{4}_ChiSq.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DABRAS.Serial_Number);
            string[,] DataToWrite = MakeDataWritable(ChiSquaredDataGridView);
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

            if (!DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = "Firmware Version: XX";
                this.DABRAS_SN_Label.Text = "Serial Number: XX";
                this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
            }

            return;
        }
        #endregion

        #region Port Connect Handler
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
        }
        #endregion

        #region Stop Button Handler
        private void StopButton_Click(object sender, EventArgs e)
        {
            if (CL != null)
            {
                CL.RequestStop();
                BackgroundThread.Join();
            }

            Set_GUI(false);
        }
        #endregion

        #region Save Button Handler
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog SD = new SaveFileDialog();
            SD.Filter = "Comma Separated Value|*.csv";
            SD.ShowDialog();

            if (SD.FileName!= "")
            {
                string [,] DataToWrite = MakeDataWritable(this.ChiSquaredDataGridView);
                FileStream F = (FileStream)SD.OpenFile();

                Logger.WriteCSV(F, DataToWrite);
                MessageBox.Show("File Written.");
                return;
            }

            MessageBox.Show("Aborted.");
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

        #region WebForm Handlers
        private void openWebBasedSurveyF12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DC.GetWebSurvey());
            NewForm.Show();
        }

        private void openRSOSharePointF11ToolStripMenuItem_Click(object sender, EventArgs e)
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
        private void aquireCtrlAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AquireButton.Enabled)
            {
                AquireButton_Click(this, null);
            }

            return;
        }

        private void stopCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (StopButton.Enabled)
            {
                StopButton_Click(this, null);
            }

            return;
        }


        private void saveDataToFileCtrlVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveButton.Enabled)
            {
                SaveButton_Click(this, null);
            }

            return;
        }

        private void saveImageCtrlIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageSaveButton_Click(this, null);
        }
        #endregion

        #region Close form handler
        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }
        #endregion

        #region AboutForm Handler
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm AF = new AboutForm();
            AF.ShowDialog();
            return;
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

            if (Key.Control)
            {
                if (Key.KeyCode == Keys.Q)
                {
                    closeCtrlQToolStripMenuItem_Click(this, null);
                    return;
                }

                if (Key.KeyCode == Keys.A)
                {
                    if (AquireButton.Enabled)
                    {
                        AquireButton_Click(this, null);
                        return;
                    }
                }

                if (Key.KeyCode == Keys.S)
                {
                    if (StopButton.Enabled)
                    {
                        StopButton_Click(this, null);
                    }

                    return;
                }

                if (Key.KeyCode == Keys.V)
                {
                    if (SaveButton.Enabled)
                    {
                        SaveButton_Click(this, null);
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
            }
        }
        #endregion

        #region Getters
        public bool WasDABRASModified()
        {
            return this.NewDABRAS;
        }

        public DABRAS GetDABRAS()
        {
            return this.DABRAS;
        }

        public bool WasDCModified()
        {
            return this.NewDC;
        }

        public DefaultConfigurations GetDefaultConfigurations()
        {
            return this.DC;
        }

        #endregion

        #region Finalization
        private void ChiSquared_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CL != null)
            {
                CL.RequestStop();
                BackgroundThread.Join();
            }

            this.frmParent.Show();
        }
        #endregion

        #region Show/Hide Handler
        private void ChiSquared_VisibleChanged(object sender, EventArgs e)
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

    public class ChiSquaredListener
    {
        #region Data Members
        private bool Done;
        private DataGridView ChiSq_Table;
        private int SampleTime;
        private DABRAS DABRAS;
        private bool ShouldStop;
        private bool WasBackgroundFinishedSuccessfully;
        private DateTime BackgroundFinished;
        private int AlphaBackground;
        private int BetaBackground;
        private int NumSamples;
        private Radioactive_Source BG;

        private int AlphaCPM;
        private int BetaCPM;

        private double AlphaChiSq;
        private double BetaChiSq;

        private double AlphaP;
        private double BetaP;

        private bool Running = false;

        private bool IsAlphaTest = false;
        private bool IsBetaTest = false;
        private bool TestPassed = false;

        public event EventHandler ChiSqThreadFinished;

        #endregion

        #region Constructor
        public ChiSquaredListener(DABRAS _DABRAS, int _SampleTime, int _NumSamples, DataGridView _CalTable, Radioactive_Source _BG, bool _IsAlphaTest, bool _IsBetaTest)
        {
            this.DABRAS = _DABRAS;
            this.ChiSq_Table = _CalTable;
            this.SampleTime = _SampleTime;
            WasBackgroundFinishedSuccessfully = false;
            this.NumSamples = _NumSamples;
            this.BG = _BG;
            this.IsAlphaTest = _IsAlphaTest;
            this.IsBetaTest = _IsBetaTest;
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
        public bool WasTestPassed()
        {
            return this.TestPassed;
        }

        public double GetAlphaPValue()
        {
            return this.AlphaP;
        }

        public double GetBetaPValue()
        {
            return this.BetaP;
        }

        public double GetAlphaChiSq()
        {
            return this.AlphaChiSq;
        }

        public double GetBetaChiSq()
        {
            return this.BetaChiSq;
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

        #region Private Utility Functions

        /*This came from AlgLib*/
        private double Gamma(double x)
        {
            double result = 0;
            double p = 0;
            double pp = 0;
            double q = 0;
            double qq = 0;
            double z = 0;
            int i = 0;
            double sgngam = 0;

            sgngam = 1;
            q = Math.Abs(x);
            if ((double)(q) > (double)(33.0))
            {
                if ((double)(x) < (double)(0.0))
                {
                    p = (int)Math.Floor(q);
                    i = (int)Math.Round(p);
                    if (i % 2 == 0)
                    {
                        sgngam = -1;
                    }
                    z = q - p;
                    if ((double)(z) > (double)(0.5))
                    {
                        p = p + 1;
                        z = q - p;
                    }
                    z = q * Math.Sin(Math.PI * z);
                    z = Math.Abs(z);
                    z = Math.PI / (z * gammastirf(q));
                }
                else
                {
                    z = gammastirf(x);
                }
                result = sgngam * z;
                return result;
            }
            z = 1;
            while ((double)(x) >= (double)(3))
            {
                x = x - 1;
                z = z * x;
            }
            while ((double)(x) < (double)(0))
            {
                if ((double)(x) > (double)(-0.000000001))
                {
                    result = z / ((1 + 0.5772156649015329 * x) * x);
                    return result;
                }
                z = z / x;
                x = x + 1;
            }
            while ((double)(x) < (double)(2))
            {
                if ((double)(x) < (double)(0.000000001))
                {
                    result = z / ((1 + 0.5772156649015329 * x) * x);
                    return result;
                }
                z = z / x;
                x = x + 1.0;
            }
            if ((double)(x) == (double)(2))
            {
                result = z;
                return result;
            }
            x = x - 2.0;
            pp = 1.60119522476751861407E-4;
            pp = 1.19135147006586384913E-3 + x * pp;
            pp = 1.04213797561761569935E-2 + x * pp;
            pp = 4.76367800457137231464E-2 + x * pp;
            pp = 2.07448227648435975150E-1 + x * pp;
            pp = 4.94214826801497100753E-1 + x * pp;
            pp = 9.99999999999999996796E-1 + x * pp;
            qq = -2.31581873324120129819E-5;
            qq = 5.39605580493303397842E-4 + x * qq;
            qq = -4.45641913851797240494E-3 + x * qq;
            qq = 1.18139785222060435552E-2 + x * qq;
            qq = 3.58236398605498653373E-2 + x * qq;
            qq = -2.34591795718243348568E-1 + x * qq;
            qq = 7.14304917030273074085E-2 + x * qq;
            qq = 1.00000000000000000320 + x * qq;
            result = z * pp / qq;
            return result;
        }

        /*This came from AlgLib*/
        private double gammastirf(double x)
        {
            double result = 0;
            double y = 0;
            double w = 0;
            double v = 0;
            double stir = 0;

            w = 1 / x;
            stir = 7.87311395793093628397E-4;
            stir = -2.29549961613378126380E-4 + w * stir;
            stir = -2.68132617805781232825E-3 + w * stir;
            stir = 3.47222221605458667310E-3 + w * stir;
            stir = 8.33333333333482257126E-2 + w * stir;
            w = 1 + w * stir;
            y = Math.Exp(x);
            if ((double)(x) > (double)(143.01608))
            {
                v = Math.Pow(x, 0.5 * x - 0.25);
                y = v * (v / y);
            }
            else
            {
                y = Math.Pow(x, x - 0.5) / y;
            }
            result = 2.50662827463100050242 * y * w;
            return result;
        }

        /*From Alglib*/
        public static double incompletegamma(double a,
            double x)
        {
            double result = 0;
            double igammaepsilon = 0;
            double ans = 0;
            double ax = 0;
            double c = 0;
            double r = 0;
            double tmp = 0;

            igammaepsilon = 0.000000000000001;
            if ((double)(x) <= (double)(0) || (double)(a) <= (double)(0))
            {
                result = 0;
                return result;
            }
            if ((double)(x) > (double)(1) && (double)(x) > (double)(a))
            {
                result = 1 - incompletegammac(a, x);
                return result;
            }
            ax = a * Math.Log(x) - x - lngamma(a, ref tmp);
            if ((double)(ax) < (double)(-709.78271289338399))
            {
                result = 0;
                return result;
            }
            ax = Math.Exp(ax);
            r = a;
            c = 1;
            ans = 1;
            do
            {
                r = r + 1;
                c = c * x / r;
                ans = ans + c;
            }
            while ((double)(c / ans) > (double)(igammaepsilon));
            result = ans * ax / a;
            return result;
        }

        /*From Alglib*/
        public static double incompletegammac(double a,
            double x)
        {
            double result = 0;
            double igammaepsilon = 0;
            double igammabignumber = 0;
            double igammabignumberinv = 0;
            double ans = 0;
            double ax = 0;
            double c = 0;
            double yc = 0;
            double r = 0;
            double t = 0;
            double y = 0;
            double z = 0;
            double pk = 0;
            double pkm1 = 0;
            double pkm2 = 0;
            double qk = 0;
            double qkm1 = 0;
            double qkm2 = 0;
            double tmp = 0;

            igammaepsilon = 0.000000000000001;
            igammabignumber = 4503599627370496.0;
            igammabignumberinv = 2.22044604925031308085 * 0.0000000000000001;
            if ((double)(x) <= (double)(0) || (double)(a) <= (double)(0))
            {
                result = 1;
                return result;
            }
            if ((double)(x) < (double)(1) || (double)(x) < (double)(a))
            {
                result = 1 - incompletegamma(a, x);
                return result;
            }
            ax = a * Math.Log(x) - x - lngamma(a, ref tmp);
            if ((double)(ax) < (double)(-709.78271289338399))
            {
                result = 0;
                return result;
            }
            ax = Math.Exp(ax);
            y = 1 - a;
            z = x + y + 1;
            c = 0;
            pkm2 = 1;
            qkm2 = x;
            pkm1 = x + 1;
            qkm1 = z * x;
            ans = pkm1 / qkm1;
            do
            {
                c = c + 1;
                y = y + 1;
                z = z + 2;
                yc = y * c;
                pk = pkm1 * z - pkm2 * yc;
                qk = qkm1 * z - qkm2 * yc;
                if ((double)(qk) != (double)(0))
                {
                    r = pk / qk;
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                {
                    t = 1;
                }
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if ((double)(Math.Abs(pk)) > (double)(igammabignumber))
                {
                    pkm2 = pkm2 * igammabignumberinv;
                    pkm1 = pkm1 * igammabignumberinv;
                    qkm2 = qkm2 * igammabignumberinv;
                    qkm1 = qkm1 * igammabignumberinv;
                }
            }
            while ((double)(t) > (double)(igammaepsilon));
            result = ans * ax;
            return result;
        }

        /*From Alglib*/
        public static double lngamma(double x,
            ref double sgngam)
        {
            double result = 0;
            double a = 0;
            double b = 0;
            double c = 0;
            double p = 0;
            double q = 0;
            double u = 0;
            double w = 0;
            double z = 0;
            int i = 0;
            double logpi = 0;
            double ls2pi = 0;
            double tmp = 0;

            sgngam = 0;

            sgngam = 1;
            logpi = 1.14472988584940017414;
            ls2pi = 0.91893853320467274178;
            if ((double)(x) < (double)(-34.0))
            {
                q = -x;
                w = lngamma(q, ref tmp);
                p = (int)Math.Floor(q);
                i = (int)Math.Round(p);
                if (i % 2 == 0)
                {
                    sgngam = -1;
                }
                else
                {
                    sgngam = 1;
                }
                z = q - p;
                if ((double)(z) > (double)(0.5))
                {
                    p = p + 1;
                    z = p - q;
                }
                z = q * Math.Sin(Math.PI * z);
                result = logpi - Math.Log(z) - w;
                return result;
            }
            if ((double)(x) < (double)(13))
            {
                z = 1;
                p = 0;
                u = x;
                while ((double)(u) >= (double)(3))
                {
                    p = p - 1;
                    u = x + p;
                    z = z * u;
                }
                while ((double)(u) < (double)(2))
                {
                    z = z / u;
                    p = p + 1;
                    u = x + p;
                }
                if ((double)(z) < (double)(0))
                {
                    sgngam = -1;
                    z = -z;
                }
                else
                {
                    sgngam = 1;
                }
                if ((double)(u) == (double)(2))
                {
                    result = Math.Log(z);
                    return result;
                }
                p = p - 2;
                x = x + p;
                b = -1378.25152569120859100;
                b = -38801.6315134637840924 + x * b;
                b = -331612.992738871184744 + x * b;
                b = -1162370.97492762307383 + x * b;
                b = -1721737.00820839662146 + x * b;
                b = -853555.664245765465627 + x * b;
                c = 1;
                c = -351.815701436523470549 + x * c;
                c = -17064.2106651881159223 + x * c;
                c = -220528.590553854454839 + x * c;
                c = -1139334.44367982507207 + x * c;
                c = -2532523.07177582951285 + x * c;
                c = -2018891.41433532773231 + x * c;
                p = x * b / c;
                result = Math.Log(z) + p;
                return result;
            }
            q = (x - 0.5) * Math.Log(x) - x + ls2pi;
            if ((double)(x) > (double)(100000000))
            {
                result = q;
                return result;
            }
            p = 1 / (x * x);
            if ((double)(x) >= (double)(1000.0))
            {
                q = q + ((7.9365079365079365079365 * 0.0001 * p - 2.7777777777777777777778 * 0.001) * p + 0.0833333333333333333333) / x;
            }
            else
            {
                a = 8.11614167470508450300 * 0.0001;
                a = -(5.95061904284301438324 * 0.0001) + p * a;
                a = 7.93650340457716943945 * 0.0001 + p * a;
                a = -(2.77777777730099687205 * 0.001) + p * a;
                a = 8.33333333333331927722 * 0.01 + p * a;
                q = q + a / x;
            }
            result = q;
            return result;
        }

        #endregion

        #region Main Background Thread
        public void Get_ChiSq()
        {
            Running = true;
            /*Pause any background monitors*/
            DABRAS.Cut();

            /*Set aquisition time*/
            DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
            Thread.Sleep(500);

            /*Clear any data left in the buffer*/
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();
            DABRAS.EnableWatchdog();

            /*Do not increment the row index until the current sample time has elapsed*/
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
                        ChiSqThreadFinished(this, null);
                        return;
                    }

                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                    DABRAS.KickWatchdog();

                    if (!ShouldStop)
                    {
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
                        ChiSqThreadFinished(this, null);
                        return;
                    }

                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                    /*Grab handles to form*/
                    if (!ShouldStop)
                    {
                        DataGridViewCell TimeElapsed = ChiSq_Table[1, i];
                        DataGridViewCell AlphaTot = ChiSq_Table[2, i];
                        DataGridViewCell AlphaCPM = ChiSq_Table[3, i];
                        DataGridViewCell BetaTot = ChiSq_Table[4, i];
                        DataGridViewCell BetaCPM = ChiSq_Table[5, i];


                        /*Parse data to form*/
                        TimeElapsed.Value = IncomingData.ElTime;

                        if (IncomingData != null && IncomingData.ElTime != 0)
                        {
                            AlphaTot.Value = IncomingData.AlphaTot;
                            AlphaCPM.Value = StaticMethods.RoundToDecimal(Convert.ToDouble(IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)), 1); //Convert raw counts into CPM. +1 to avoid DBZ, PIC uses zero based counting.
                            BetaTot.Value = IncomingData.BetaTot;
                            BetaCPM.Value = StaticMethods.RoundToDecimal(Convert.ToDouble(IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)), 1);

                            this.AlphaBackground = Convert.ToInt32(IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60));
                            this.BetaBackground = Convert.ToInt32(IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60));

                        }
                        /*Re-draw table*/
                        ChiSq_Table.Invalidate();
                        DABRAS.KickWatchdog();

                        /*If the sample time has elapsed, increment the row.*/
                        if (IncomingData != null && IncomingData.ElTime >= SampleTime)
                        {
                            RowComplete = true;
                        }
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
                    AverageAlphaCPM += Convert.ToDouble(ChiSq_Table[2, i].Value);
                    AverageBetaCPM += Convert.ToDouble(ChiSq_Table[4, i].Value);
                }

                AverageAlphaCPM /= NumSamples;
                AverageBetaCPM /= NumSamples;

                DataGridViewCell AverageAlphaCell = ChiSq_Table[2, NumSamples];
                DataGridViewCell AverageBetaCell = ChiSq_Table[4, NumSamples];
                DataGridViewCell StdDevAlphaCell = ChiSq_Table[2, NumSamples + 1];
                DataGridViewCell StdDevBetaCell = ChiSq_Table[4, NumSamples + 1];
                DataGridViewCell ChiSqAlphaCell = ChiSq_Table[2, NumSamples + 2];
                DataGridViewCell ChiSqBetaCell = ChiSq_Table[4, NumSamples + 2];
                DataGridViewCell AlphaPCell = ChiSq_Table[2, NumSamples + 3];
                DataGridViewCell BetaPCell = ChiSq_Table[4, NumSamples + 3];


                AverageAlphaCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaCPM);
                AverageBetaCell.Value = StaticMethods.RoundToSigFigs(AverageBetaCPM);

                this.AlphaCPM = Convert.ToInt32(AverageAlphaCPM);
                this.BetaCPM = Convert.ToInt32(AverageBetaCPM);

                /*Compute Standard Deviations*/
                double StdDevAlpha = 0;
                double StdDevBeta = 0;

                for (int i = 0; i < NumSamples; i++)
                {
                    StdDevAlpha += (AverageAlphaCPM - Convert.ToDouble(ChiSq_Table[2, i].Value)) * (AverageAlphaCPM - Convert.ToDouble(ChiSq_Table[2, i].Value));
                    StdDevBeta += (AverageBetaCPM - Convert.ToDouble(ChiSq_Table[4, i].Value)) * (AverageBetaCPM - Convert.ToDouble(ChiSq_Table[4, i].Value));
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

                /*Compute Chi^2*/

                this.AlphaChiSq = 0;
                this.BetaChiSq = 0;

                /*TODO: Yates Correction?*/
                for (int i = 0; i < NumSamples; i++)
                {
                    double TempAlpha = Convert.ToDouble(ChiSq_Table[2, i].Value);
                    double TempBeta = Convert.ToDouble(ChiSq_Table[4, i].Value);
                    AlphaChiSq += (TempAlpha - AverageAlphaCPM) * (TempAlpha - AverageAlphaCPM);
                    BetaChiSq += (TempBeta - AverageBetaCPM) * (TempBeta - AverageBetaCPM);
                }

                AlphaChiSq /= AverageAlphaCPM;
                BetaChiSq /= AverageBetaCPM;

                /*Check for NAN*/
                /*
                if (AlphaChiSq != AlphaChiSq)
                {
                    AlphaChiSq = 0;
                }

                if (BetaChiSq != BetaChiSq)
                {
                    BetaChiSq = 0;
                }
                */
                ChiSqAlphaCell.Value = StaticMethods.RoundToSigFigs(AlphaChiSq);
                ChiSqBetaCell.Value = StaticMethods.RoundToSigFigs(BetaChiSq);

                /*Compute P value.*/
                /*This value comes from Stegun's Handbook of Mathematical Functions*/
                double DegOfFreedom = NumSamples - 1;

                ///*For Testing*/
                //DegOfFreedom = 23;
                //AlphaChiSq = 44.18;
                //BetaChiSq = 9.26;

                this.AlphaP = 1 - incompletegamma((DegOfFreedom / 2), (AlphaChiSq / 2));  // *(Gamma(DegOfFreedom) / (Gamma(DegOfFreedom / 2))); <- Not needed b/c we are computing the regularlized incomplete gamma function
                this.BetaP = 1 - incompletegamma((DegOfFreedom / 2), (BetaChiSq / 2));  // *(Gamma(DegOfFreedom) / (Gamma(DegOfFreedom / 2))); <- Not needed b/c we are computing the regularlized incomplete gamma function

                AlphaPCell.Value = StaticMethods.RoundToSigFigs(AlphaP);
                BetaPCell.Value = StaticMethods.RoundToSigFigs(BetaP);

                if (IsAlphaTest)
                {
                    TestPassed = ((AlphaP <= 0.95) && (AlphaP >= 0.05));
                }

                if (IsBetaTest && !TestPassed)
                {
                    TestPassed = ((BetaP <= 0.95) && (BetaP >= 0.05));
                }

                this.BackgroundFinished = DateTime.Now;
                WasBackgroundFinishedSuccessfully = true;

                using (SoundPlayer S = new SoundPlayer(DABRAS_Software.Properties.Resources.DoneSound))
                {
                    S.Play();
                }
                
                if (ChiSqThreadFinished != null)
                {
                    ChiSqThreadFinished(this, null);
                }
            }

            /*Reinstate background monitors, if they existed*/
            DABRAS.ResumeBackgroundMonitors();
            Running = false;

        }
        #endregion
    }
}

