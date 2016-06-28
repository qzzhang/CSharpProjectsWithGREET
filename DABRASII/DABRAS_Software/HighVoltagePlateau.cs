using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Media;
using System.Drawing.Imaging;

namespace DABRAS_Software
{
    public partial class HighVoltagePlateau : Form
    {
        #region Data Members
        private DABRAS DABRAS;
        private Logger Logger;
        private readonly FormCLB LaunchedFrom;
        private DefaultConfigurations DC;
        private bool NewDABRAS;

        HighVoltagePlateauListener HV;

        public delegate void UpdateHVRowCallback();
        #endregion

        #region Constructor
        public HighVoltagePlateau(FormCLB _CF)
        {
            InitializeComponent();

            this.LaunchedFrom = _CF;
            this.DABRAS = LaunchedFrom.GetDABRAS();
            this.Logger = LaunchedFrom.GetLogger();
            this.NewDABRAS = false;
            this.DC = LaunchedFrom.GetDefaultConfig();

            if (this.DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = "Firmware Version: " + DABRAS.Firmware_Version;
                this.DABRAS_SN_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                this.DABRAS_Status_Label.Text = "STATUS: Connected!";
            }

            int NumSamples = 0;
            try
            {
                NumSamples = Convert.ToInt32((Convert.ToDouble(this.EndV_TB.Text) - Convert.ToDouble(this.StartV_TB.Text)) / Convert.ToDouble(this.StepSize_TB.Text));
                Math.Sqrt(NumSamples);
            }
            catch
            {
                MessageBox.Show("Error: Bad Sample Step Size");
                return;
            }

            VoltagePlateauDataGridView = Make_Volage_Plateau_GridView(NumSamples);
            VoltagePlateauDataGridView.Invalidate();
            
            DataChart.Series["Alpha"].ChartType = SeriesChartType.FastLine;
            DataChart.Series["Alpha"].Color = Color.Red;


            DataChart.Series["Beta"].ChartType = SeriesChartType.FastLine;
            DataChart.Series["Beta"].Color = Color.Blue;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

        }
        #endregion

        #region Private Utility Functions
        private DataGridView Make_Volage_Plateau_GridView(int NumSamples)
        {
            DataGridView VolagePlateauDataGridView = new DataGridView();
            VoltagePlateauDataGridView.Columns.Add("Null", "");
            VoltagePlateauDataGridView.Columns.Add("Voltage_Column", "Voltage");
            VoltagePlateauDataGridView.Columns.Add("Acq_Time_Column", "Acq Time");
            VoltagePlateauDataGridView.Columns.Add("Alpha_Count_Column", "Alpha Count");
            VoltagePlateauDataGridView.Columns.Add("Alpha_CPM_Column", "Alpha CPM");
            VoltagePlateauDataGridView.Columns.Add("Beta_Count_Column", "Beta Count");
            VoltagePlateauDataGridView.Columns.Add("Beta_CPM_Column", "Beta CPM");

            /*Add one row for each count*/
            for (int i = 0; i < NumSamples; i++)
            {
                VoltagePlateauDataGridView.Rows.Add(new object[] { Convert.ToString((i + 1)), "", "", "", "", "", "0", "0" });

            }

            return VoltagePlateauDataGridView;

        }

        private bool Verify_Values(double Start, double End, double StepSize)
        {
            if (Start >= End)
            {
                return false;
            }

            /*TODO: Add other parameters*/

            return true;
        }

        private DataGridView ReDrawDataGridView(double StartVal, double EndVal, double StepSize)
        {
            int NumRows = Convert.ToInt32((EndVal - StartVal) / StepSize);
            NumRows++;

            return Make_Volage_Plateau_GridView(NumRows);
        }

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
                    DG.Rows.Insert(0, "", "", "", "", "", "", "0", "0");
                }

            }

            /*Re-number the rows*/
            for (int i = 0; i < DG.RowCount - 1; i++)
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

        private void SetGUI(bool TestRunning)
        {
            this.aquireCtrlAToolStripMenuItem.Enabled = !TestRunning;
            this.AquireButton.Enabled = !TestRunning;

            this.WriteFileButton.Enabled = !TestRunning;
            this.writeDataToFileCtrlWToolStripMenuItem.Enabled = !TestRunning;

            this.ImageWriteButton.Enabled = !TestRunning;
            this.writeImageCtrlIToolStripMenuItem.Enabled = !TestRunning;

            this.SetButton.Enabled = !TestRunning;

            this.connectDisconnectToAPortCtrlPToolStripMenuItem.Enabled = !TestRunning;

            this.stopCtrlSToolStripMenuItem.Enabled = TestRunning;
            this.StopButton.Enabled = TestRunning;

            this.StartV_TB.Enabled = !TestRunning;
            this.EndV_TB.Enabled = !TestRunning;
            this.StepSize_TB.Enabled = !TestRunning;
            this.Min_TB.Enabled = !TestRunning;
            this.Sec_TB.Enabled = !TestRunning;

            return;
        }
        #endregion

        #region Aquire Handler
        private void AquireButton_Click(object sender, EventArgs e)
        {
            double StartValue = 0;
            double StopValue = 0;
            double StepSize = 0;

            int SampleTime = 0;

            try
            {
                StartValue = Convert.ToDouble(this.StartV_TB.Text);
                Math.Sqrt(StartValue);
                StopValue = Convert.ToDouble(this.EndV_TB.Text);
                Math.Sqrt(StopValue);
                StepSize = Convert.ToDouble(this.StepSize_TB.Text);
                Math.Sqrt(StepSize);

                SampleTime = (Convert.ToInt32(this.Min_TB.Text) * 60) + Convert.ToInt32(this.Sec_TB.Text);
                Math.Sqrt(SampleTime);
            }

            catch
            {
                MessageBox.Show("Error: Bad Parameters");
                return;
            }

            if (StartValue >= StopValue)
            {
                MessageBox.Show("Error: StartValue must be less than StopValue");
                return;
            }

            if (!DABRAS.IsConnected())
            {
                MessageBox.Show("Error: Must be connected to the DABRAS to enable this feature. Please re-connect and try again.");
                /*
                this.frmParent.refreshConnectStatus();
                this.endFormActivities();
                */
                return;
            }

            SetButton_Click(this, null);

            ClearDataGridView(this.VoltagePlateauDataGridView);

            DataChart.Series["Alpha"].Points.Clear();
            DataChart.Series["Beta"].Points.Clear();

            HV = new HighVoltagePlateauListener(DABRAS, SampleTime, StartValue, StopValue, StepSize, this.VoltagePlateauDataGridView);
            Thread BackgroundThread = new Thread(() => HV.Get_Voltages());

            BackgroundThread.Start();

            HV.BackgroundThreadUpdate += (s, args) => { InvokeCallBack(); };

            SetGUI(true);

        }
        #endregion

        #region Stop Button Handler
        private void StopButton_Click(object sender, EventArgs e)
        {
            if (HV != null)
            {
                HV.RequestStop();
            }

            SetGUI(false);
        }
        #endregion

        #region Set Button Handler
        private void SetButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Verify_Values(Convert.ToDouble(this.StartV_TB.Text), Convert.ToDouble(this.EndV_TB.Text), Convert.ToDouble(this.StepSize_TB.Text)))
                {
                    MessageBox.Show("Error: Bad Values.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Error: Bad value.");
            }

            int NumberOfRows = Convert.ToInt32((Convert.ToDouble(EndV_TB.Text) - Convert.ToDouble(StartV_TB.Text)) / Convert.ToDouble(this.StepSize_TB.Text));
            //NumberOfRows++; //Account for truncation

            Add_Or_Subtract_Rows(NumberOfRows, this.VoltagePlateauDataGridView);

            return;
        }
        #endregion

        #region Save Button Handler
        private void WriteFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog S = new SaveFileDialog();
            S.Filter = "Comma Separated Value|*.csv|Microsoft Excel Tab Delimited File|*.xls";
            S.ShowDialog();
            if (S.FileName != "")
            {
                FileStream F = (FileStream)S.OpenFile();
                string FilePath = S.FileName;
                switch (S.FilterIndex)
                {
                    case 1:
                        Logger.WriteCSV(F, MakeDataWritable(this.VoltagePlateauDataGridView));
                        break;
                    case 2:
                        MessageBox.Show("Not Implemented. Writing CSV.");
                        Logger.WriteCSV(F, MakeDataWritable(this.VoltagePlateauDataGridView));
                        break;
                    //Logger.WriteExcel(FilePath, MakeDataWritable());//TODO: Fix
                    //break;
                    default:
                        break;
                }

            }
        }
        #endregion

        #region Image Save Handler
        private void ImageWriteButton_Click(object sender, EventArgs e)
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

        private void openRSOHomeF10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DC.GetRSOHome());
            NewForm.Show();
        }
        #endregion

        #region About Handler
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm NewForm = new AboutForm();
            NewForm.ShowDialog();
        }
        #endregion

        #region GUI Callbacks
        private void InvokeCallBack()
        {
            try
            {
                this.Invoke(new UpdateHVRowCallback(this.UpdateChart));
            }
            catch
            {
                return;
            }
        }

        private void UpdateChart()
        {
            if (HV != null)
            {
                if (!HV.IsDone())
                {
                    double[] AlphaPoint = HV.GetAlphaPoint();
                    double[] BetaPoint = HV.GetBetaPoint();

                    DataChart.Series["Alpha"].Points.AddXY(AlphaPoint[0], AlphaPoint[1]);
                    DataChart.Series["Beta"].Points.AddXY(BetaPoint[0], BetaPoint[1]);
                }

                else
                {
                    SetGUI(false);

                    if (HV.IsSuccessful())
                    {
                        /*Write to file*/
                        string FilePath = String.Format("{0}\\data\\Cal\\HVP\\{1}_{2}_{3}_{4}_HVP.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DABRAS.Serial_Number);
                        string[,] DataToWrite = MakeDataWritable(VoltagePlateauDataGridView);
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

                    if (!DABRAS.IsConnected())
                    {
                        this.DABRAS_Firmware_Label.Text = "Firmware Version: XX";
                        this.DABRAS_SN_Label.Text = "Serial Number: XX";
                        this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
                    }
                }
            }

        }
        #endregion

        #region Port Connect Handler
        private void connectDisconnectToAPortCtrlPToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void writeImageCtrlIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ImageWriteButton.Enabled)
            {
                this.ImageWriteButton_Click(this, null);
            }
        }

        private void aquireCtrlAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AquireButton_Click(this, null);
        }

        private void stopCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopButton_Click(this, null);
        }

        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }

        private void writeDataToFileCtrlWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteFileButton_Click(this, null);
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
                    if (connectDisconnectToAPortCtrlPToolStripMenuItem.Enabled)
                    {
                        connectDisconnectToAPortCtrlPToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.W)
                {
                    if (WriteFileButton.Enabled)
                    {
                        WriteFileButton_Click(this, null);
                    }
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
                openRSOHomeF10ToolStripMenuItem_Click(this, null);
            }
        }
        #endregion

        #region Finalization
        private void HighVoltagePlateau_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (HV != null)
            {
                HV.RequestStop();
            }

            if (this.DialogResult != DialogResult.Abort && this.DABRAS.IsConnected())
            {
                /*Show HV return form if not brutally murdered and we have a valid connection*/
                HV_Exiting NewForm;
                try
                {
                    NewForm = new HV_Exiting(Convert.ToDouble(this.StartV_TB.Text) * 1000, Convert.ToDouble(this.EndV_TB.Text) * 1000);
                }

                catch
                {
                    NewForm = new HV_Exiting(0, 5000);
                }

                NewForm.ShowDialog();

                DABRAS.Cut();

                DABRAS.SetHVC(NewForm.FinalValue);

                DABRAS.ResumeBackgroundMonitors();
            }

            else
            {
                DABRAS.Cut();

                DABRAS.SetHVC(2500);

                MessageBox.Show("Killed. Set HVC to 2500 mV");

                DABRAS.ResumeBackgroundMonitors();
            }

            this.LaunchedFrom.Show();
        }
        #endregion

        #region Show/Hide Handler
        private void HighVoltagePlateau_VisibleChanged(object sender, EventArgs e)
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

        #region Click Handler
        private void DataChart_MouseClick(object sender, MouseEventArgs e)
        {
            HitTestResult[] Results = DataChart.HitTest(e.X, e.Y, false, ChartElementType.DataPoint);

            foreach (HitTestResult R in Results)
            {
                if (R.ChartElementType == ChartElementType.DataPoint)
                {
                    try
                    {
                        foreach (DataGridViewRow Row in VoltagePlateauDataGridView.Rows)
                        {
                            Row.Selected = false;
                        }
                        VoltagePlateauDataGridView.Rows[R.PointIndex].Selected = true;
                    }

                    catch
                    {
                        ;
                    }
                }
            }
        }
        #endregion

    }

    public class HighVoltagePlateauListener
    {
        #region Data Members
        private bool Done = false;
        private DataGridView Plat_Table;
        private int SampleTime;
        private DABRAS DABRAS;
        private bool ShouldStop;
        private bool Successful = false;

        private double StartValue;
        private double StopValue;
        private double StepSize;
        private int CurrentStep;

        private double[] AlphaPoint = { 0, 0 };
        private double[] BetaPoint = { 0, 0 };

        public event EventHandler BackgroundThreadUpdate;
        #endregion

        #region Constructor
        public HighVoltagePlateauListener(DABRAS _DABRAS, int _SampleTime, double _StartValue, double _StopValue, double _StepSize, DataGridView _PlatTable)
        {
            this.DABRAS = _DABRAS;
            this.Plat_Table= _PlatTable;
            this.SampleTime = _SampleTime;
            this.StartValue = _StartValue;
            this.StopValue = _StopValue;
            this.StepSize = _StepSize;

            this.CurrentStep = 0;

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
        public bool IsSuccessful()
        {
            return this.Successful;
        }

        public double[] GetAlphaPoint()
        {
            return this.AlphaPoint;
        }

        public double[] GetBetaPoint()
        {
            return this.BetaPoint;
        }

        public bool IsDone()
        {
            return this.Done;
        }
        #endregion

        #region Main Background Thread
        public void Get_Voltages()
        {
            /*Compute the number of samples needed*/
            int NumSamples = Convert.ToInt32((StopValue - StartValue) / StepSize);
            NumSamples++; //Account for round-down tendancies

            /*Interrupt any background threads*/
            DABRAS.Cut();

            /*Set aquisition time*/
            DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));

            /*Clear any data left in the buffer*/
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();
            DABRAS.EnableWatchdog();

            while ((StartValue < StopValue) && (!ShouldStop))
            {
                /*Wait for Voltage to go into regulation*/
                do
                {
                    /*Set HV Control*/
                    DABRAS.SetHVC(1000 * StartValue);
                    Thread.Sleep(400);
                    DABRAS.KickWatchdog();
                } while (((Math.Abs((DABRAS.GetHVC() * 5000 / 16383) - (StartValue * 1000)) / (StartValue * 1000)) > .1) && (!ShouldStop));

                DABRAS.Write_To_Serial_Port("g"); //Start Aquisition
                DABRAS.ClearSerialPacket();
                DABRAS.ClearRaw();
                DABRAS.ClearPacketFlag();
                

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
                        BackgroundThreadUpdate(this, null);
                        return;
                    }

                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                    DABRAS.KickWatchdog();

                    if (!ShouldStop)
                    {
                        if (IncomingData.ElTime == 0 && IncomingData.TargetTime == SampleTime)
                        {
                            break;
                        }

                        if (IncomingData.ElTime > 5)
                        {
                            DABRAS.Write_To_Serial_Port("t");
                            Thread.Sleep(250);
                            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
                            DABRAS.Write_To_Serial_Port("g");
                        }
                    }
                }

                bool Done = false;
                SerialPacket S = null;
                do
                {
                    /*Wait for incoming packet*/
                    try
                    {
                        while (!DABRAS.IsDataReady() && (!ShouldStop))
                        {
                            Thread.Sleep(50); //augmented to avoid getting stuck
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
                        BackgroundThreadUpdate(this, null);
                        return;
                    }

                    /*Parse packet*/
                    if (!ShouldStop)
                    {
                        S = DABRAS.ReadSerialPacket();

                        DataGridViewCell VoltageCell = Plat_Table[1, CurrentStep];
                        DataGridViewCell TimeCell = Plat_Table[2, CurrentStep];
                        DataGridViewCell AlphaCell = Plat_Table[3, CurrentStep];
                        DataGridViewCell AlphaCPMCell = Plat_Table[4, CurrentStep];
                        DataGridViewCell BetaCell = Plat_Table[5, CurrentStep];
                        DataGridViewCell BetaCPMCell = Plat_Table[6, CurrentStep];

                        VoltageCell.Value = StaticMethods.RoundToSigFigs(Convert.ToDouble(((DABRAS.GetHVC() * 5) / 16383)));
                        TimeCell.Value = S.ElTime;

                        if (S.ElTime != 0)
                        {
                            AlphaCell.Value = S.AlphaTot;
                            AlphaCPMCell.Value = StaticMethods.RoundToDecimal((Convert.ToDouble(S.AlphaTot) / ((Convert.ToDouble(S.ElTime)) / 60)), 1);
                            BetaCell.Value = S.BetaTot;
                            BetaCPMCell.Value = StaticMethods.RoundToDecimal((Convert.ToDouble(S.BetaTot) / ((Convert.ToDouble(S.ElTime)) / 60)), 1);

                            Plat_Table.Invalidate();

                            if (S.ElTime >= SampleTime)
                            {
                                Done = true;
                            }
                        }
                        DABRAS.KickWatchdog();
                    }

                } while ((!Done) && (!ShouldStop));
                /*Fire event for the end of the row*/
                
                DABRAS.DisableWatchdog();
                
                if (!ShouldStop)
                {
                    this.AlphaPoint[0] = this.StartValue;
                    this.AlphaPoint[1] = S.AlphaTot;

                    this.BetaPoint[0] = this.StartValue;
                    this.BetaPoint[1] = S.BetaTot;

                    BackgroundThreadUpdate(this, null);
                    StartValue += StepSize;
                    CurrentStep++;
                }
            }

            if (!ShouldStop)
            {
                this.Successful = true;
                using (SoundPlayer S = new SoundPlayer(DABRAS_Software.Properties.Resources.DoneSound))
                {
                    S.Play();
                }
            }

            /*Reinstate background threads, if they existed*/
            DABRAS.ResumeBackgroundMonitors();
            
            this.Done = true;
            BackgroundThreadUpdate(this, null);

            return;
        }
        #endregion
    }
}
