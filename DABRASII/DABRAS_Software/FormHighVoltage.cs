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
    public partial class FormHighVoltage : Form
    {
        #region Data Members
        private DABRAS DABRAS;
        private Logger Logger;
        private readonly FormCLB LaunchedFrom;
        private DefaultConfigurations DC;
        private bool NewDABRAS;

        private bool Calibrating = false;

        HighVoltagePlateauListener HVListener;

        public delegate void UpdateHVRowCallback();
        double StartValue = 0;
        double StopValue = 0;
        double StepSize = 0;
        int SampleTime = 0;

        #endregion

        #region Constructor
        public FormHighVoltage(FormCLB _CF)
        {
            this.LaunchedFrom = _CF;
            this.DABRAS = LaunchedFrom.GetDABRAS();

            if (!this.DABRAS.IsConnected())
            {
                this.endFormActivities();
                this.LaunchedFrom.endFormActivities();
                this.notifyParentForm();
                MessageBox.Show("Error: Must be connected to the DABRAS to enable this feature. Please re-connect and try again.");
                this.Calibrating = false;
                this.Dispose();
                return;
            }

            InitializeComponent();

            this.Logger = LaunchedFrom.GetLogger();
            this.NewDABRAS = false;
            this.DC = LaunchedFrom.GetDefaultConfig();
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

            this.VoltagePlateauDataGridView = Make_Volage_Plateau_GridView(NumSamples);
            this.VoltagePlateauDataGridView.Invalidate();
            
            DataChart.Series["Alpha"].ChartType = SeriesChartType.FastLine;
            DataChart.Series["Alpha"].Color = Color.Red;


            DataChart.Series["Beta"].ChartType = SeriesChartType.FastLine;
            DataChart.Series["Beta"].Color = Color.Blue;

            this.SetGUI(false);
        }
        #endregion

        #region Private Utility Functions
        private DataGridView Make_Volage_Plateau_GridView(int NumSamples)
        {
            DataGridView VolagePlateauDataGridView = new DataGridView();
            VoltagePlateauDataGridView.Columns.Add("SampleNo_Column", "Sample#");
            VoltagePlateauDataGridView.Columns.Add("Voltage_Column", "Voltage");
            VoltagePlateauDataGridView.Columns.Add("Acq_Time_Column", "Acq Time");
            VoltagePlateauDataGridView.Columns.Add("Alpha_Count_Column", "Alpha Count");
            VoltagePlateauDataGridView.Columns.Add("Alpha_CPM_Column", "Alpha CPM");
            VoltagePlateauDataGridView.Columns.Add("Beta_Count_Column", "Beta Count");
            VoltagePlateauDataGridView.Columns.Add("Beta_CPM_Column", "Beta CPM");

            //Add one row for each count
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
                //To get rid of extra rows
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
            this.AquireButton.Enabled = !TestRunning;
            this.SetButton.Enabled = !TestRunning;
            this.StartV_TB.Enabled = !TestRunning;
            this.EndV_TB.Enabled = !TestRunning;
            this.StepSize_TB.Enabled = !TestRunning;
            this.Min_TB.Enabled = !TestRunning;
            this.Sec_TB.Enabled = !TestRunning;
            this.WriteFileButton.Enabled = !TestRunning;
            this.ImageWriteButton.Enabled = !TestRunning;

            this.StopButton.Enabled = TestRunning;
            return;
        }

        private void endFormActivities()
        {
            //Stop the HL listener, if it exists
            if (this.HVListener != null)
            {
                this.HVListener.RequestStop();
            }
            this.DABRAS.Cut();
        }

        private void notifyParentForm()
        {
            this.LaunchedFrom.GetParentForm().refreshConnectStatus();
        }
        #endregion

        #region Aquire Handler
        private void AquireButton_Click(object sender, EventArgs e)
        {
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

            this.SetButton_Click(this, null);

            this.ClearDataGridView(this.VoltagePlateauDataGridView);

            this.DataChart.Series["Alpha"].Points.Clear();
            this.DataChart.Series["Beta"].Points.Clear();
            this.DataChart.Series["Alpha"].ChartType = SeriesChartType.Line;
            this.DataChart.Series["Beta"].ChartType = SeriesChartType.Line;

            this.ImposeWait();
        }
        private void StartBackgroundThread(int smpT, double startV, double stopV, double stepV, DataGridView dgv)
        {
            this.HVListener = new HighVoltagePlateauListener(this.DABRAS, smpT, startV, stopV, stepV, dgv);
            Thread BackgroundThread = new Thread(() => (this.HVListener).Get_Voltages());

            BackgroundThread.Start();

            this.HVListener.BackgroundThreadUpdate += (s, args) => { InvokeCallBack(); };

            this.formatCharts();
            this.SetGUI(true);
        }
        #endregion

        #region Stop Button Handler
        private void StopButton_Click(object sender, EventArgs e)
        {
            if (this.HVListener != null)
            {
                this.HVListener.RequestStop();
            }

            SetGUI(false);
        }
        #endregion

        #region Set Button Handler
        private void SetButton_Click(object sender, EventArgs e)
        {//to set the voltage range and then the datagrid rows
            double vstep;
            try
            {
                vstep = Convert.ToDouble(this.StepSize_TB.Text);
                if(vstep == 0.0)
                {
                    MessageBox.Show("Error: The step size has to be non-zero. Please enter a valid number!");
                    return;
                }
                if (!this.Verify_Values(Convert.ToDouble(this.StartV_TB.Text), Convert.ToDouble(this.EndV_TB.Text), vstep))
                {
                    MessageBox.Show("Error: Bad Values.");
                    return;
                }
            }
            catch(Exception de)
            {
                MessageBox.Show("Error: "+ de.Message);
                return;
            }

            int NumberOfRows = Convert.ToInt32((Convert.ToDouble(EndV_TB.Text) - Convert.ToDouble(StartV_TB.Text)) / vstep);
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
            //MessageBox.Show("Done!");
            return;
        }
        #endregion

        #region Callbacks
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
            if (!this.DABRAS.IsConnected())
            {
                this.endFormActivities();
                this.LaunchedFrom.endFormActivities();
                this.notifyParentForm();
                MessageBox.Show("Error: Must be connected to the DABRAS to perform a Hi/Lo calibration. Please re-connect and try again.");
                return;
            }
            if (this.HVListener != null)
            {
                if (!this.HVListener.IsDone())
                {
                    double[] AlphaPoint = this.HVListener.GetAlphaPoint();
                    double[] BetaPoint = this.HVListener.GetBetaPoint();

                    DataChart.Series["Alpha"].Points.AddXY(AlphaPoint[0], AlphaPoint[1]);
                    DataChart.Series["Beta"].Points.AddXY(BetaPoint[0], BetaPoint[1]);
                }
                else
                {
                    SetGUI(false);

                    if (this.HVListener.IsSuccessful())
                    {
                        //Write to file
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
                                //Toast T = new Toast("File Written.");
                                //T.Show();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Automatic write failed.");
                        }
                        MessageBox.Show("High Voltage Plateau completed.");
                    }
                }
            }
        }

        //added by QZ
        private void formatCharts()
        {
            //DataChart.Series["Alpha"].IsValueShownAsLabel = true;
            //DataChart.Series["Alpha"]["LabelStyle"] = "Top";
            DataChart.Series["Alpha"].MarkerStyle = MarkerStyle.Circle;
            DataChart.Series["Alpha"].MarkerSize = 10;
            DataChart.Series["Alpha"].MarkerColor = Color.Transparent;
            DataChart.Series["Alpha"].MarkerBorderColor = Color.Red;
            //DataChart.Series["Alpha"].LabelBackColor = Color.LightCyan;
            foreach (DataPoint dp in DataChart.Series["Alpha"].Points)
                dp.Font = new System.Drawing.Font("Arial", 18, FontStyle.Bold);

            //DataChart.Series["Beta"].IsValueShownAsLabel = true;
            //DataChart.Series["Beta"]["LabelStyle"] = "Top";
            DataChart.Series["Beta"].MarkerStyle = MarkerStyle.Diamond;
            DataChart.Series["Beta"].MarkerSize = 10;
            DataChart.Series["Beta"].MarkerColor = Color.Transparent;
            DataChart.Series["Beta"].MarkerBorderColor = Color.Blue;
            //DataChart.Series["Beta"].LabelBackColor = Color.LightGray;
            foreach (DataPoint dp in DataChart.Series["Beta"].Points)
                dp.Font = new System.Drawing.Font("Arial", 18, FontStyle.Bold);
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
                if (Key.KeyCode == Keys.S)
                {
                    if (StopButton.Enabled)
                    {
                        StopButton_Click(this, null);
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
        }
        #endregion

        #region Finalization
        private void HighVoltagePlateau_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.endFormActivities();

            if (this.DialogResult != DialogResult.Abort && this.DABRAS.IsConnected())
            {
                //Show HV return form if not brutally murdered and we have a valid connection
                FormHVCsetting hvcForm;
                try
                {
                    hvcForm = new FormHVCsetting(Convert.ToDouble(this.StartV_TB.Text) * 1000, Convert.ToDouble(this.EndV_TB.Text) * 1000, this.DABRAS);
                }
                catch
                {
                    hvcForm = new FormHVCsetting(0, 5000, this.DABRAS);
                }

                hvcForm.ShowDialog();

                this.DABRAS.ResumeBackgroundMonitors();
            }
            else
            {
                this.DABRAS.SetHVC(2500);

                MessageBox.Show("Killed. Set HVC to 2500 mV");

                this.DABRAS.ResumeBackgroundMonitors();
            }

            this.LaunchedFrom.Enabled = true; 
        }
        #endregion

        #region Show/Hide Handler
        private void HighVoltagePlateau_VisibleChanged(object sender, EventArgs e)
        {
            if (this.DABRAS != null)
            {
                if (this.DABRAS.IsConnected())
                {
                   
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

        private void StepSize_TB_TextChanged(object sender, EventArgs e)
        {
            //this.SetButton_Click(this, null);
        }

        private void EndV_TB_TextChanged(object sender, EventArgs e)
        {
            //this.SetButton_Click(this, null);
        }

        private void StartV_TB_TextChanged(object sender, EventArgs e)
        {
            //this.SetButton_Click(this, null);
        }

        #region progress bar
        private void ImposeWait()
        {
            this.progressBar1.Value = 0;
            this.progressBar1.Enabled = true;
            this.progressBar1.Visible = true;
            this.lbl_progress.Visible = true;

            // Start the timer.
            this.timer1.Interval = 200;
            this.timer1.Enabled = true;
            this.timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.progressBar1.Value < 100) // Should be less than progress bar max value
            {
                this.progressBar1.Increment(1);
                this.progressBar1.Value++;
                if (this.progressBar1.Value == 100) //The maximum value of the progress bar
                {
                    this.progressBar1.Enabled = false;
                    this.progressBar1.Visible = false;
                    this.lbl_progress.Visible = false;
                    this.timer1.Stop();
                    this.timer1.Enabled = false;
                    this.StartBackgroundThread(SampleTime, StartValue, StopValue, StepSize, this.VoltagePlateauDataGridView);
                }
            }
            else
            {
                this.progressBar1.Enabled = false;
                this.progressBar1.Visible = false;
                this.timer1.Stop();
                this.timer1.Enabled = false;
                this.lbl_progress.Visible = false;
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
            //Compute the number of samples needed
            int NumSamples = Convert.ToInt32((StopValue - StartValue) / StepSize);
            NumSamples++; //Account for round-down tendancies

            //Interrupt any background threads
            DABRAS.Cut();

            //Set aquisition time
            DABRAS.Write_To_Serial_Port("t");//Must be followed within one second by an ASCII decimal value 
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));//the decimal value representing the number of seconds to which the acquisition time should be set

            //Clear any data left in the buffer
            this.DABRAS.ClearAll();

            this.DABRAS.EnableWatchdog();

            while ((StartValue <= StopValue) && (!ShouldStop))
            {
                //Wait for Voltage to go into regulation
                double d_HVC = 0.0;
                do
                {
                    this.DABRAS.SetHVC(1000 * StartValue);//Set HV Control
                    Thread.Sleep(400);
                    this.DABRAS.KickWatchdog();
                    d_HVC = this.DABRAS.GetHVC();
                } while (((Math.Abs((d_HVC * 5000 / 16383) - (StartValue * 1000)) / (StartValue * 1000)) > .05) && (!ShouldStop));

                //Start Aquisition
                DABRAS.Write_To_Serial_Port("g");// clears the buffers containing accumulated beta and alpha counts as well as the elapsed time and sets bit 0 of the status byte to 1
                //Clear any data left in the buffer
                this.DABRAS.ClearAll();
                
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
                            DABRAS.Write_To_Serial_Port("t");//Set the Acquisition Time
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
                    //Wait for incoming packet
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

                    //Parse packet
                    if (!ShouldStop)
                    {
                        S = DABRAS.ReadSerialPacket();

                        DataGridViewCell VoltageCell = Plat_Table[1, CurrentStep];
                        DataGridViewCell TimeCell = Plat_Table[2, CurrentStep];
                        DataGridViewCell AlphaCell = Plat_Table[3, CurrentStep];
                        DataGridViewCell AlphaCPMCell = Plat_Table[4, CurrentStep];
                        DataGridViewCell BetaCell = Plat_Table[5, CurrentStep];
                        DataGridViewCell BetaCPMCell = Plat_Table[6, CurrentStep];

                        //QZ_todo: add (or uncomment) the if-else statement back
                        //if (this.DABRAS.SetHVC(this.StartValue * 1000))
                        {
                            this.DABRAS.SetHVC(this.StartValue * 1000);
                            VoltageCell.Value = Convert.ToDecimal(string.Format("{0:0.0}", this.StartValue));
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
                        }
                        /*else
                        {
                            MessageBox.Show("Error: Failed to set the HV DAC voltage.");
                            return;
                        }*/
                        this.DABRAS.KickWatchdog();
                    }

                } while ((!Done) && (!ShouldStop));
                //Fire event for the end of the row
                
                this.DABRAS.DisableWatchdog();
                
                if (!ShouldStop)
                {
                    this.AlphaPoint[0] = this.StartValue;
                    this.AlphaPoint[1] = S.AlphaTot;

                    this.BetaPoint[0] = this.StartValue;
                    this.BetaPoint[1] = S.BetaTot;

                    BackgroundThreadUpdate(this, null);
                    this.StartValue += StepSize;
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

            //Reinstate background threads, if they existed
            this.DABRAS.ResumeBackgroundMonitors();
            
            this.Done = true;
            BackgroundThreadUpdate(this, null);

            return;
        }
        #endregion
    }
}
