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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Media;
using System.Drawing.Imaging;

namespace DABRAS_Software
{
    public partial class QC : Form
    {
        #region Public Enums
        public enum TypeOfQC { Background, Alpha, Beta }
        #endregion

        #region Data Members
        private readonly QCMain QCMain;

        private DABRAS DABRAS;
        private bool DABRASModified = false;
        private Logger Logger;

        private QCListKeeper QC_List;

        private DefaultConfigurations DC;
        private bool DCModified = false;

        private double AlphaHi = -1;
        private double AlphaLo = -1;
        private double BetaHi = -1;
        private double BetaLo = -1;

        private bool UsingAnnual = true;

        private bool SourcesModified = false;
        

        private TypeOfQC Type;

        QCBackgroundListener QCBG;
        QCAlphaBetaListener QCAB;

        private Radioactive_Source SelectedSource;

        private List<Radioactive_Source> ListOfSources;

        private List<QCCalResultNode> ListOfQCResults;

        private Thread OperationThread;

        private Form LaunchedFrom;
        private Form ChildDialogue;

        public delegate void BG_GUI_Callback();
        public delegate void AB_GUI_Callback();

        #endregion

        #region Constructor
        public QC(QCMain Parent, TypeOfQC _T)
        {
            InitializeComponent();

            this.LaunchedFrom = Parent;

            this.QCMain = Parent;
            this.DABRAS = QCMain.GetDABRAS();
            this.Logger = QCMain.GetLogger();
            this.Type = _T;
            this.DC = QCMain.GetDefaultConfigurations();
            this.ListOfSources = QCMain.GetListOfSources();
            this.QC_List = QCMain.GetQCList();
            
            /*Find source and set labels correctly*/
            if (Type == TypeOfQC.Background)
            {
                SelectedSource = ListOfSources.Find(x => x.GetName() == "Background");

                this.AlphaHi = SelectedSource.GetAlphaHi();
                this.AlphaLo = SelectedSource.GetAlphaLo();
                this.BetaHi = SelectedSource.GetBetaHi();
                this.BetaLo = SelectedSource.GetBetaLo();

                this.Alpha_GCPM_Label.Text = String.Format("Alpha GCPM must be between {0} and {1} (Average of {2}, Determined {3})", AlphaHi, AlphaLo, StaticMethods.RoundToSigFigs(SelectedSource.GetAlphaHiLoAvg()), SelectedSource.GetHiLoCalibratedTime());
                this.Beta_GCPM_Label.Text = String.Format("Beta GCPM must be between {0} and {1} (Average of {2}, Determined {3})", BetaHi, BetaLo, StaticMethods.RoundToSigFigs(SelectedSource.GetBetaHiLoAvg()), SelectedSource.GetHiLoCalibratedTime());

                /*Change the default to the correct value*/
                this.Min_TB.Text = "10";

            }

            if (Type == TypeOfQC.Alpha)
            {
                SelectedSource = ListOfSources.Find(x => x.GetName() == "Am-241");

                this.AlphaHi = SelectedSource.GetAlphaHi();
                this.AlphaLo = SelectedSource.GetAlphaLo();


                this.Alpha_GCPM_Label.Text = String.Format("Alpha NCPM must be between {0} and {1} (Average of {2}, Determined {3})", AlphaHi, AlphaLo, StaticMethods.RoundToSigFigs(SelectedSource.GetAlphaHiLoAvg()), SelectedSource.GetHiLoCalibratedTime());
                this.Beta_GCPM_Label.Visible = false;
            }

            if (Type == TypeOfQC.Beta)
            {
                SelectedSource = ListOfSources.Find(x => x.GetName() == "Sr-90");


                this.BetaHi = Convert.ToDouble(StaticMethods.RoundToSigFigs(SelectedSource.GetBetaHi() * SelectedSource.GetDecayFactor(DateTime.Now, SelectedSource.GetHiLoCalibratedTime())));
                this.BetaLo = Convert.ToDouble(StaticMethods.RoundToSigFigs(SelectedSource.GetBetaLo() * SelectedSource.GetDecayFactor(DateTime.Now, SelectedSource.GetHiLoCalibratedTime())));

                this.Alpha_GCPM_Label.Visible = false;
                this.Beta_GCPM_Label.Text = String.Format("Beta NCPM must be between {0} and {1} (Average of {2}, Determined {3})", BetaHi, BetaLo, StaticMethods.RoundToSigFigs(SelectedSource.GetBetaHiLoAvg()), SelectedSource.GetHiLoCalibratedTime());

            }

            ShortDataGridView.Columns.Add("ShortDataGridView_Time", "Sample No");
            ShortDataGridView.Columns.Add("ShortDataGridView_Time", "Acq Time");
            ShortDataGridView.Columns.Add("ShortDataGridView_AGCPM", "Alpha GCPM");
            ShortDataGridView.Columns.Add("ShortDataGridView_ANCPM", "Alpha NCPM");
            ShortDataGridView.Columns.Add("ShortDataGridView_BGCPM", "Beta GCPM");
            ShortDataGridView.Columns.Add("ShortDataGridView_BNCPM", "Beta NCPM");
            ShortDataGridView.Columns.Add("ShortDataGridView_PassFail", "Pass/Fail");

            Add_Or_Subtract_Rows(Convert.ToInt32(this.Counts_TB.Text), ShortDataGridView);

            if (DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = "Firmware Version: " + DABRAS.Firmware_Version;
                this.DABRAS_SN_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                this.DABRAS_Status_Label.Text = "STATUS: Connected!";
            }

            Badge_CB.Items.Clear();

            foreach (User U in DC.GetListOfUsers())
            {
                Badge_CB.Items.Add(Convert.ToString(U.GetBadgeNo()));
            }

            /*Add keypress event handlers*/
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

        }
        #endregion        

        #region Aquire Button Handler
        private void AquireButton_Click(object sender, EventArgs e)
        {
            if (this.Type == TypeOfQC.Background)
            {
                int NumSamples = 0;
                int Sampletime = 0;
                int BadgeNo = 0;

                try
                {
                    BadgeNo = Convert.ToInt32(this.Badge_CB.Text);
                    if (String.Compare(Name_TB.Text, "") == 0)
                    {
                        /*Throw random exception*/
                        Exception k = new Exception();
                        throw k;
                    }
                }
                catch
                {
                    MessageBox.Show("Error: Please input your name and badge number.");
                    return;
                }

                try
                {
                    NumSamples = Math.Abs(Convert.ToInt32(this.Counts_TB.Text));
                    Sampletime = Math.Abs((60 * Convert.ToInt32(this.Min_TB.Text)) + Convert.ToInt32(this.Sec_TB.Text));
                }
                catch
                {
                    MessageBox.Show("Error: Bad Sample Parameters");
                    return;
                }

                if (!DABRAS.IsConnected())
                {
                    MessageBox.Show("Error: Connection to DABRAS needed. Please re-connect and try again.");
                    return;
                }

                SetButton_Click(this, null);

                ClearDataGridView(this.ShortDataGridView);

                SetGUI(true);

                QCBG = new QCBackgroundListener(this.DABRAS, Sampletime, NumSamples, this.ShortDataGridView, this.AlphaHi, this.AlphaLo, this.BetaHi, this.BetaLo, DateTime.Now, BadgeNo, this.Name_TB.Text);
                OperationThread = new Thread(() => QCBG.Get_Background());
                OperationThread.Start();

                QCBG.BackgroundThreadFinished += (s, args) => { InvokeBGCallback(); };

            }

            else
            {
                int NumSamples = 0;
                int Sampletime = 0;
                int BadgeNo = 0;

                try
                {
                    BadgeNo = Convert.ToInt32(this.Badge_CB.Text);
                    if (String.Compare(Name_TB.Text, "") == 0)
                    {
                        /*Throw random exception*/
                        Exception k = new Exception();
                        throw k;
                    }
                }
                catch
                {
                    MessageBox.Show("Error: Please input your name and badge number.");
                    return;
                }

                try
                {
                    NumSamples = Math.Abs(Convert.ToInt32(this.Counts_TB.Text));
                    Sampletime = Math.Abs((60 * Convert.ToInt32(this.Min_TB.Text)) + Convert.ToInt32(this.Sec_TB.Text));
                }
                catch
                {
                    MessageBox.Show("Error: Bad Values");
                    return;
                }

                if (!DABRAS.IsConnected())
                {
                    MessageBox.Show("Error: Connection to DABRAS needed. Please re-connect and try again.");
                    return;
                }

                ClearDataGridView(this.ShortDataGridView);

                SetGUI(true);

                Radioactive_Source R = ListOfSources.Find(x => x.GetName() == "Background");

                SetButton_Click(this, null);

                bool IsAlpha = (this.Type == TypeOfQC.Alpha);
                
                if (this.Type == TypeOfQC.Alpha)
                {
                    if (this.UsingAnnual)
                    {
                        QCAB = new QCAlphaBetaListener(this.DABRAS, Sampletime, NumSamples, this.ShortDataGridView, this.AlphaHi, this.AlphaLo, IsAlpha, Convert.ToInt32(R.GetAnnualAlphaCPM()), Convert.ToInt32(R.GetAnnualBetaCPM()), DateTime.Now, BadgeNo, this.Name_TB.Text);
                    }
                    else
                    {
                        QCAB = new QCAlphaBetaListener(this.DABRAS, Sampletime, NumSamples, this.ShortDataGridView, this.AlphaHi, this.AlphaLo, IsAlpha, Convert.ToInt32(R.GetDailyAlphaCPM()), Convert.ToInt32(R.GetDailyBetaCPM()), DateTime.Now, BadgeNo, this.Name_TB.Text);
                    }
                }

                else
                {
                    if (this.UsingAnnual)
                    {
                        QCAB = new QCAlphaBetaListener(this.DABRAS, Sampletime, NumSamples, this.ShortDataGridView, this.BetaHi, this.BetaLo, IsAlpha, Convert.ToInt32(R.GetAnnualAlphaCPM()), Convert.ToInt32(R.GetAnnualBetaCPM()), DateTime.Now, BadgeNo, this.Name_TB.Text);
                    }
                    else
                    {
                        QCAB = new QCAlphaBetaListener(this.DABRAS, Sampletime, NumSamples, this.ShortDataGridView, this.BetaHi, this.BetaLo, IsAlpha, Convert.ToInt32(R.GetDailyAlphaCPM()), Convert.ToInt32(R.GetDailyBetaCPM()), DateTime.Now, BadgeNo, this.Name_TB.Text);
                    }
                }

                OperationThread= new Thread(() => QCAB.Get_Background());
                OperationThread.Start();

                QCAB.BackgroundThreadFinished += (s, args) => { InvokeABCallback(); };
            }
        }
        #endregion

        #region Set Rows Button Handler
        private void SetButton_Click(object sender, EventArgs e)
        {
            int NumberOfRows = 0;

            try
            {
                NumberOfRows = Convert.ToInt32(this.Counts_TB.Text);
                Math.Sqrt(NumberOfRows);
            }
            catch
            {
                MessageBox.Show("Bad values");
            }

            Add_Or_Subtract_Rows(NumberOfRows, this.ShortDataGridView);

        }
        #endregion

        #region GUI Callbacks

        #region Background Callback
        private void BG_BackgroundThreadFinished()
        {
            string Comment = "";
            if (QCBG.WasTestPassed() && QCBG.WasTestCompleted())
            {
                if (MessageBox.Show("Test passed! The machine is functioning properly. Would you like to add a comment?", "Success!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    QuickComment NewCommentForm = new QuickComment();
                    if ((NewCommentForm.ShowDialog()) == DialogResult.OK)
                    {
                        Comment = NewCommentForm.GetComment();
                    }
                }

                /*Write daily limits to background source*/
                Radioactive_Source R = ListOfSources.Find(x => x.GetName() == "Background");

                R.SetDailyAlphaCPM(Convert.ToInt32(QCBG.GetAlphaGCPM()));
                R.SetDailyBetaCPM(Convert.ToInt32(QCBG.GetBetaGCPM()));
                R.SetDailyCalibratedDate(DateTime.Now);
                R.SetDailyCalibratedTimespan(QCBG.GetSampleTime() * QCBG.GetNumSamples());

                /*Add QC object to List*/
                QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Background, QCBG.GetAlphaNCPM(), QCBG.GetBetaNCPM(),QCBG.GetBadgeNo(), true, Comment, QCBG.GetName(), QCBG.GetSampleTime());
                QC_List.Add(NewResult);
                
                R.SetDailyCalibratedTimespan(QCBG.GetSampleTime() * QCBG.GetNumSamples());

                DCModified = true;
                SourcesModified = true;

                this.AquireButton.Enabled = true;
                this.aquireDataCtrlAToolStripMenuItem.Enabled = true;
                this.StopButton.Enabled = false;
                this.stopAquisitionCtrlSToolStripMenuItem.Enabled = false;
            }

            else if ((!(QCBG.WasTestPassed())) && QCBG.WasTestCompleted())
            {
                if (MessageBox.Show("Calibration test failed. Add a comment? Note that to declare a point unplottable, you must add a comment.", "Test failed", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    /*Write daily limits to background source*/
                    Radioactive_Source R = ListOfSources.Find(x => x.GetName() == "Background");

                    R.SetDailyAlphaCPM(Convert.ToInt32(QCBG.GetAlphaGCPM()));
                    R.SetDailyBetaCPM(Convert.ToInt32(QCBG.GetBetaGCPM()));
                    R.SetDailyCalibratedDate(DateTime.Now);

                    bool Plottable = true;
                    QuickComment NewCommentForm = new QuickComment();
                    if ((NewCommentForm.ShowDialog()) == DialogResult.OK)
                    {
                        Comment = NewCommentForm.GetComment();
                        if ((Comment != "") && (MessageBox.Show("Declare point as unplottable?", "Plottable?", MessageBoxButtons.YesNo) == DialogResult.No))
                        {
                            Plottable = false;
                        }
                    }

                    /*Add QC object to List*/
                    QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Background, QCBG.GetAlphaNCPM(), QCBG.GetBetaNCPM(), QCBG.GetBadgeNo(), Plottable, Comment, QCBG.GetName(), QCBG.GetSampleTime());
                    QC_List.Add(NewResult);
                    //R.SetDailyCalibratedTimespan(QCBG.GetSampleTime() * QCBG.GetNumSamples());

                    DCModified = true;
                    SourcesModified = true;

                }
                
            }

            /*Write to file*/
            string FilePath = String.Format("{0}\\data\\QC\\Bkgd\\{1}_{2}_{3}_{4}_{5}_{6}_BkgdQC.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, QCBG.GetBadgeNo(), QCBG.GetName(), DABRAS.Serial_Number);
            string[,] DataToWrite = MakeDataWritable(ShortDataGridView);
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

            SetGUI(false);

            QCBG.RequestStop();
            //QCBG = null;
        }

        #endregion

        public void InvokeBGCallback()
        {
            this.Invoke(new BG_GUI_Callback(this.BG_BackgroundThreadFinished));
        }

        #region Alpha or Beta Callback
        private void AB_BackgroundThreadFinished()
        {
            string Comment = "";
            if (QCAB.WasTestPassed() && QCAB.WasTestCompleted())
            {
                if (MessageBox.Show("Test passed! Would you like to add a comment?", "Success!", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    QuickComment NewComment = new QuickComment();
                    if (NewComment.ShowDialog() == DialogResult.OK)
                    {
                        Comment = NewComment.GetComment();
                    }
                }

                /*Write daily limits to calibrated source*/
                if (this.Type == TypeOfQC.Alpha)
                {
                    Radioactive_Source R = ListOfSources.Find(x => x.GetName() == "Am-241");

                    R.SetDailyAlphaCPM(Convert.ToInt32(QCAB.GetAlphaNCPM()));
                    R.SetDailyBetaCPM(Convert.ToInt32(QCAB.GetBetaNCPM()));
                    R.SetDailyCalibratedDate(DateTime.Now);
                    R.SetDailyCalibratedTimespan(QCAB.GetNumSamples() * QCAB.GetSampleTime());

                    /*Add QC object to List*/
                    QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Alpha, QCAB.GetAlphaNCPM(), QCAB.GetBetaNCPM(), QCAB.GetBadgeNo(), true, Comment, QCAB.GetName(), QCAB.GetSampleTime());
                    QC_List.Add(NewResult);

                    R.SetDailyCalibratedTimespan(QCAB.GetSampleTime() * QCAB.GetNumSamples());

                    DCModified = true;
                    SourcesModified = true;
                }

                else
                {
                    Radioactive_Source R = ListOfSources.Find(x => x.GetName() == "Sr-90");

                    R.SetDailyAlphaCPM(Convert.ToInt32(QCAB.GetAlphaNCPM()));
                    R.SetDailyBetaCPM(Convert.ToInt32(QCAB.GetBetaNCPM()));
                    R.SetDailyCalibratedDate(DateTime.Now);

                    /*Add QC object to List*/
                    QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Beta, QCAB.GetAlphaNCPM(), QCAB.GetBetaNCPM(), QCAB.GetBadgeNo(), true, Comment, QCAB.GetName(), QCAB.GetSampleTime());
                    QC_List.Add(NewResult);

                    DCModified = true;
                    SourcesModified = true;
                }
 
            }

            else if ((!(QCAB.WasTestPassed())) && QCAB.WasTestCompleted())
            {
                bool Plottable = true;
                if (MessageBox.Show("Calibration test failed. Would you like to add a comment? Note that in order to declare a point unplottable, you must add a comment.", "Test failed", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                   
                    QuickComment QC = new QuickComment();
                    if (QC.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Comment = QC.GetComment();
                        if ((Comment != "") && (MessageBox.Show("Declare point unplottable?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes))
                        {
                            Plottable = false;
                        }
                    }
                }
                    /*Write daily limits to calibrated source*/
                if (this.Type == TypeOfQC.Alpha)
                {
                    Radioactive_Source R = ListOfSources.Find(x => x.GetName() == "Am-241");

                    R.SetDailyAlphaCPM(Convert.ToInt32(QCAB.GetAlphaNCPM()));
                    R.SetDailyBetaCPM(Convert.ToInt32(QCAB.GetBetaNCPM()));
                    R.SetDailyCalibratedDate(DateTime.Now);

                    /*Add QC object to List*/
                    QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Alpha, QCAB.GetAlphaNCPM(), QCAB.GetBetaNCPM(), QCAB.GetBadgeNo(), Plottable, Comment, QCAB.GetName());
                    QC_List.Add(NewResult);

                    R.SetDailyCalibratedTimespan(QCAB.GetSampleTime() * QCAB.GetNumSamples());

                    DCModified = true;
                    SourcesModified = true;
                }

                else
                {
                    Radioactive_Source R = ListOfSources.Find(x => x.GetName() == "Sr-90");

                    R.SetDailyAlphaCPM(Convert.ToInt32(QCAB.GetAlphaNCPM()));
                    R.SetDailyBetaCPM(Convert.ToInt32(QCAB.GetBetaNCPM()));
                    R.SetDailyCalibratedDate(DateTime.Now);

                    /*Add QC object to List*/
                    QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Beta, QCAB.GetAlphaNCPM(), QCAB.GetBetaNCPM(), QCAB.GetBadgeNo(), Plottable, Comment, QCAB.GetName());
                    QC_List.Add(NewResult);

                    DCModified = true;
                    SourcesModified = true;
                }

            }

            if (!DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = "Firmware Version: XX";
                this.DABRAS_SN_Label.Text = "Serial Number: XX";
                this.DABRAS_Status_Label.Text = "STATUS: Disconnected";
            }

            SetGUI(false);

            /*Write to file*/
            string FilePath;
            if (QCAB.AlphaTest())
            {
                FilePath = String.Format("{0}\\data\\QC\\Alpha\\{1}_{2}_{3}_{4}_{5}_{6}_AlphaQC.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, QCAB.GetBadgeNo(), QCAB.GetName(), DABRAS.Serial_Number);
            }

            else
            {
                FilePath = String.Format("{0}\\data\\QC\\Beta\\{1}_{2}_{3}_{4}_{5}_{6}_BetaQC.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, QCAB.GetBadgeNo(), QCAB.GetName(), DABRAS.Serial_Number);
            }

            string[,] DataToWrite = MakeDataWritable(ShortDataGridView);
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

            QCAB.RequestStop(); //to be safe
            //QCAB = null;
        }
        #endregion

        public void InvokeABCallback()
        {
            this.Invoke(new AB_GUI_Callback(this.AB_BackgroundThreadFinished));
        }

        #endregion

        #region Private Utility Functions
        private void FillEmployeeData(object sender, EventArgs e)
        {
            int BadgeNo = -1;
            string Name = "";
            try
            {
                Name = Convert.ToString(this.Name_TB.Text);
                BadgeNo = Convert.ToInt32(this.Badge_CB.Text);
            }

            catch
            {
                return;
            }

            if (BadgeNo != -1)
            {
                User FoundUser;
                List<User> ListOfUsers = DC.GetListOfUsers();
                try
                {
                    FoundUser = ListOfUsers.Find(x => x.GetBadgeNo() == BadgeNo);
                }

                catch
                {
                    return;
                }

                if (FoundUser != null)
                {
                    this.Name_TB.Text = Convert.ToString(FoundUser.GetName());
                    this.Badge_CB.Text = Convert.ToString(FoundUser.GetBadgeNo());
                }
            }
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
                while ((DG.RowCount) > (FinalNumberOfRows))
                {
                    DG.Rows.RemoveAt(0);
                }

            }

            else
            {
                while ((DG.RowCount) < (FinalNumberOfRows))
                {
                    DG.Rows.Insert(0, "", "", "", "", "", "", "", "");
                }

            }

            /*Re-number the rows*/
            for (int i = 0; i < DG.RowCount - 2; i++)
            {
                DG[0, i].Value = i + 1;
            }

            DG[0, DG.RowCount - 2].Value = "Avg";
            DG[0, DG.RowCount - 1].Value = "StdDev";
        }

        private void ClearDataGridView(DataGridView DG)
        {
            for (int i = 0; i < DG.RowCount; i++)
            {
                for (int j = 1; j < DG.ColumnCount; j++)
                {
                    DG[j, i].Value = "";
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
        #endregion

        #region Show Full Data Set Handler
        private void ShowFullDataSetButton_Click(object sender, EventArgs e)
        {
            FullDataSetPopup NewForm = new FullDataSetPopup(QCAB, QCBG, this.Logger);
            NewForm.ShowDialog();
        }
        #endregion

        #region Show Graph Handler
        private void ShowGraphButton_Click(object sender, EventArgs e)
        {
            DailyQCGraph NewForm = new DailyQCGraph(this, this.QC_List, this.Logger, this.ListOfSources, this.DC);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region Misc GUI Functions

        private void Badge_CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillEmployeeData(this, null);
        }

        private void SetGUI(bool Running)
        {
            AquireButton.Enabled = !Running;
            aquireDataCtrlAToolStripMenuItem.Enabled = !Running;
            SetButton.Enabled = !Running;
            ShowGraphButton.Enabled = !Running;
            ShowFullDataSetButton.Enabled = !Running;
            ImageSaveButton.Enabled = !Running;
            saveImageCtrlIToolStripMenuItem.Enabled = !Running;
            SaveButton.Enabled = !Running;
            saveToFileCtrlVToolStripMenuItem.Enabled = !Running;
            connectOrDisconnectToAPortCtrlPToolStripMenuItem.Enabled = !Running;
            LogButton.Enabled = !Running;

            StopButton.Enabled = Running;
            stopAquisitionCtrlSToolStripMenuItem.Enabled = Running;

            this.Counts_TB.Enabled = !Running;
            this.Min_TB.Enabled = !Running;
            this.Sec_TB.Enabled = !Running;
            this.Badge_CB.Enabled = !Running;
            this.Name_TB.Enabled = !Running;
            return;
        }

        #endregion

        #region Close Button Handler
        private void closeWindowCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }
        #endregion

        #region Log Button Handler
        private void LogButton_Click(object sender, EventArgs e)
        {
            if (QC_List.GetFullList().Count > 0)
            {
                DailyQCLog NewForm = new DailyQCLog(this, QC_List.GetFullList(), DABRAS, Logger);
                NewForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("No results to display.");
            }            
        }
        #endregion

        #region CSV Save Handler
        private void SaveButton_Click(object sender, EventArgs e)
        {
            string[,] DataToWrite = MakeDataWritable(ShortDataGridView);
            SaveFileDialog SD = new SaveFileDialog();
            SD.Filter = "Comma Separated Value|*.csv";
            SD.ShowDialog();
            if (SD.FileName != "")
            {
                FileStream F = (FileStream)SD.OpenFile();
                Logger.WriteCSV(F, DataToWrite);
                MessageBox.Show("Data Written.");
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
                    MessageBox.Show("Done!");
                }
            }
            return;
        }
        #endregion

        #region Abort Handler
        private void AbortAll()
        {
            if (QCBG != null)
            {
                QCBG.RequestStop();
                while (QCBG.IsRunning())
                {
                    Thread.Sleep(1);
                }
            }

            if (QCAB != null)
            {
                QCAB.RequestStop();
                while (QCAB.IsRunning())
                {
                    Thread.Sleep(1);
                }
            }

            this.DialogResult = DialogResult.Abort;
            this.Close();
            return;
        }
        #endregion

        #region Port Connect Handler
        private void connectOrDisconnectToAPortCtrlPToolStripMenuItem_Click(object sender, EventArgs e)
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

                /*Set DABRAS Flag*/
                this.DABRASModified = true;
            }

            if (NewPopup.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }
        #endregion

        #region About Form Handler
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

        #region Getters
        public bool WasDABRASModified()
        {
            return this.DABRASModified;
        }

        public DABRAS GetDABRAS()
        {
            return this.DABRAS;
        }

        public List<Radioactive_Source> GetModifiedSources()
        {
            return this.ListOfSources;
        }

        public bool WereSourcesModified()
        {
            return this.SourcesModified;
        }

        public bool WasDCModified()
        {
            return this.DCModified;
        }

        public DefaultConfigurations GetDefaultConfig()
        {
            return this.DC;
        }

        #endregion

        #region Dummy Overloads
        private void aquireDataCtrlAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AquireButton.Enabled)
            {
                AquireButton_Click(this, null);
            }
        }

        private void stopAquisitionCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (StopButton.Enabled)
            {
                StopButton_Click(this, null);
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (QCAB != null)
            {
                QCAB.RequestStop();
            }

            if (QCBG != null)
            {
                QCBG.RequestStop();
            }

            SetGUI(false);
        }

        private void saveImageCtrlIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageSaveButton_Click(this, null);
        }
        #endregion

        #region KeyPress Handler
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
                    closeWindowCtrlQToolStripMenuItem_Click(this, null);
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

                if (Key.KeyCode == Keys.A)
                {
                    if (aquireDataCtrlAToolStripMenuItem.Enabled)
                    {
                        aquireDataCtrlAToolStripMenuItem_Click(this, null);
                    }
                    return;
                }

                if (Key.KeyCode == Keys.P)
                {
                    if (connectOrDisconnectToAPortCtrlPToolStripMenuItem.Enabled)
                    {
                        connectOrDisconnectToAPortCtrlPToolStripMenuItem_Click(this, null);
                    }
                    return;
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

            if (Key.KeyCode == Keys.Enter)
            {
                FillEmployeeData(this, null);
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

        #region Finalization
        private void QC_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*Stop the BG listener, if it exists*/
            if (QCBG != null)
            {
                QCBG.RequestStop();
            }

            /*Stop the AB listener, if it exists*/
            if (QCAB != null)
            {
                QCAB.RequestStop();
            }

            /*Wait for the background thread to terminate*/
            if (OperationThread != null)
            {
                OperationThread.Join();
            }

            this.LaunchedFrom.Show();
        }
        #endregion

        #region Show/Hide Handler
        private void QC_VisibleChanged(object sender, EventArgs e)
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

    }

    public class QCBackgroundListener
    {
        #region Data Members
        private bool Done;
        private DataGridView QC_Table;
        private int SampleTime;
        private int NumSamples;
        private DABRAS DABRAS;
        private bool ShouldStop;
        private bool WasBackgroundFinishedSuccessfully;
        private DateTime BackgroundFinished;
        private DateTime TestStarted;
        private bool TestPassed = false;

        private double AlphaHi;
        private double AlphaLo;
        private double BetaHi;
        private double BetaLo;

        private double AlphaGCPM;
        private double BetaGCPM;
        private double AlphaNCPM;
        private double BetaNCPM;

        private bool Running = false;

        private int BadgeNo;
        private string Name;

        public event EventHandler BackgroundThreadFinished;
        #endregion

        #region Constructor
        public QCBackgroundListener(DABRAS _DABRAS, int _SampleTime, int _NumSamples, DataGridView _QC_Table, double _AH, double _AL, double _BH, double _BL, DateTime _Today, int _BadgeNo, string _Name)
        {
            this.DABRAS = _DABRAS;
            this.QC_Table = _QC_Table;
            this.SampleTime = _SampleTime;
            this.NumSamples = _NumSamples;
            this.TestStarted = _Today;
            WasBackgroundFinishedSuccessfully = false;
            ShouldStop = false;

            this.AlphaHi = _AH;
            this.AlphaLo = _AL;
            this.BetaHi = _BH;
            this.BetaLo = _BL;
            this.BadgeNo = _BadgeNo;
            this.Name = _Name;
        }
        #endregion

        #region Control Functions
        public void RequestStop()
        {
            ShouldStop = true;
        }
        #endregion

        #region Getters
        public string GetName()
        {
            return this.Name;
        }

        public int GetBadgeNo()
        {
            return this.BadgeNo;
        }

        public DateTime GetDateTimeStarted()
        {
            return this.TestStarted;
        }

        public double GetAlphaHi()
        {
            return this.AlphaHi;
        }

        public double GetAlphaLo()
        {
            return this.AlphaLo;
        }

        public double GetBetaHi()
        {
            return this.BetaHi;
        }

        public int GetNumSamples()
        {
            return this.NumSamples;
        }

        public int GetSampleTime()
        {
            return this.SampleTime;
        }

        public DateTime GetStartTime()
        {
            return this.TestStarted;
        }

        public double GetBetaLo()
        {
            return this.BetaLo;
        }
        
        public bool WasTestPassed()
        {
            return this.TestPassed;
        }

        public double GetAlphaGCPM()
        {
            return this.AlphaGCPM;
        }

        public double GetBetaGCPM()
        {
            return this.BetaGCPM;
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
            return this.BackgroundFinished;
        }

        public bool WasTestCompleted()
        {
            return this.WasBackgroundFinishedSuccessfully;
        }

        public bool IsRunning()
        {
            return this.Running;
        }
        #endregion        

        #region Main Background Thread
        public void Get_Background()
        {
            this.Running = true;
            /*Stop all background threads*/
            this.DABRAS.Cut();
            
            /*Set aquisition time*/
            this.DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            this.DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));

            /*Clear any data left in the buffer*/
            this.DABRAS.ClearSerialPacket();
            this.DABRAS.ClearPacketFlag();
            this.DABRAS.EnableWatchdog();

            /*Set test passed flag*/
            this.TestPassed = true;

            for (int i = 0; i < NumSamples; i++)
            {

                bool RowComplete = false;
                this.DABRAS.Write_To_Serial_Port("g");
                Thread.Sleep(150);
                this.DABRAS.ClearSerialPacket();
                this.DABRAS.ClearPacketFlag();

                /*Check for the first good packet*/
                while (!ShouldStop)
                {
                    try
                    {
                        while (!this.DABRAS.IsDataReady() && !ShouldStop)
                        {
                            Thread.Sleep(100);
                            if (!this.DABRAS.IsConnected())
                            {
                                throw new TimeoutException();
                            }
                        }
                    }

                    catch
                    {
                        MessageBox.Show("Error: Connection lost.");
                        this.DABRAS.DisableWatchdog();
                        this.BackgroundThreadFinished(this, null);
                        return;
                    }

                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                    this.DABRAS.KickWatchdog();

                    if (IncomingData != null && IncomingData.ElTime == 0 && IncomingData.TargetTime == SampleTime)
                    {
                        break;
                    }

                    if (IncomingData != null && IncomingData.ElTime > 5)
                    {
                        this.DABRAS.Write_To_Serial_Port("t");
                        Thread.Sleep(250);
                        this.DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
                        this.DABRAS.Write_To_Serial_Port("g");
                    }

                }

                /*Do not increment the row index until the current sample time has elapsed*/
                while (!RowComplete && !ShouldStop)
                {
                    /*Wait for incoming data packet*/
                    try
                    {
                        while (!this.DABRAS.IsDataReady())
                        {
                            Thread.Sleep(100);
                            if (!this.DABRAS.IsConnected())
                            {
                                throw new TimeoutException();
                            }
                        }
                    }

                    catch
                    {
                        MessageBox.Show("Error: Connection Lost.");
                        this.DABRAS.DisableWatchdog();
                        BackgroundThreadFinished(this, null);
                        return;
                    }

                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();
                    
                    /*Grab handles to form*/
                    DataGridViewCell TimeElapsed = this.QC_Table[1, i];
                    DataGridViewCell AlphaGCPM = this.QC_Table[2, i];
                    DataGridViewCell AlphaNCPM = this.QC_Table[3, i];
                    DataGridViewCell BetaGCPM = this.QC_Table[4, i];
                    DataGridViewCell BetaNCPM = this.QC_Table[5, i];
                    DataGridViewCell PassFail = this.QC_Table[6, i];

                    /*Parse data to form*/
                    TimeElapsed.Value = IncomingData.ElTime;

                    if (IncomingData != null && IncomingData.ElTime != 0)
                    {

                        AlphaGCPM.Value = StaticMethods.RoundToDecimal((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)), 1); //Convert raw counts into CPM. +1 to avoid DBZ, PIC uses zero based counting.
                        AlphaNCPM.Value = StaticMethods.RoundToDecimal((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)), 1); //Should prolly be N/A
                        BetaGCPM.Value = StaticMethods.RoundToDecimal((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)), 1);
                        BetaNCPM.Value = StaticMethods.RoundToDecimal((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)), 1); //Should prolly be N/A

                        /*Re-draw table*/
                        QC_Table.Invalidate();

                        /*If the sample time has elapsed, increment the row and compute Pass/Fail.*/
                        if (IncomingData.ElTime >= SampleTime)
                        {
                            if ((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60) >= AlphaLo) && (IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60) <= AlphaHi) && (IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60) >= BetaLo) && (IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60) <= BetaHi))
                            {
                                PassFail.Value = "PASS";
                            }
                            else
                            {
                                PassFail.Value = "FAIL";
                                this.TestPassed = false; //Test fails if one of the test fails.
                            }

                            RowComplete = true;
                        }
                    }
                    this.DABRAS.KickWatchdog();

                }

            }

            this.DABRAS.DisableWatchdog();

            if (!ShouldStop)
            {

                /*Compute averages*/
                double AverageAlphaGCPM = 0;
                double AverageBetaGCPM = 0;
                double AverageAlphaNCPM = 0;
                double AverageBetaNCPM = 0;

                for (int i = 0; i < NumSamples; i++)
                {
                    AverageAlphaGCPM += Convert.ToDouble(QC_Table[2, i].Value);
                    AverageAlphaNCPM += Convert.ToDouble(QC_Table[3, i].Value);
                    AverageBetaGCPM += Convert.ToDouble(QC_Table[4, i].Value);
                    AverageBetaNCPM += Convert.ToDouble(QC_Table[5, i].Value);
                }

                AverageAlphaGCPM /= NumSamples;
                AverageBetaGCPM /= NumSamples;
                AverageAlphaNCPM /= NumSamples;
                AverageBetaNCPM /= NumSamples;

                DataGridViewCell AverageAlphaGCPMCell = QC_Table[2, NumSamples];
                DataGridViewCell AverageAlphaNCell = QC_Table[3, NumSamples];
                DataGridViewCell AverageBetaGCPMCell = QC_Table[4, NumSamples];
                DataGridViewCell AverageBetaNCell = QC_Table[5, NumSamples];
                DataGridViewCell StdDevAlphaNetCountCell = QC_Table[2, NumSamples + 1];
                DataGridViewCell StdDevAlphaNCell = QC_Table[3, NumSamples + 1];
                DataGridViewCell StdDevBetaNetCountCell = QC_Table[4, NumSamples + 1];
                DataGridViewCell StdDevBetaNCell = QC_Table[5, NumSamples + 1];
                DataGridViewCell FinalPassFail = QC_Table[6, NumSamples + 1];

                AverageAlphaGCPMCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaGCPM);
                AverageAlphaNCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaNCPM);
                AverageBetaGCPMCell.Value = StaticMethods.RoundToSigFigs(AverageBetaGCPM);
                AverageBetaNCell.Value = StaticMethods.RoundToSigFigs(AverageBetaNCPM);

                this.AlphaGCPM = AverageAlphaGCPM;
                this.AlphaNCPM = AverageAlphaNCPM;
                this.BetaGCPM = AverageBetaGCPM;
                this.BetaNCPM = AverageBetaNCPM;

                /*Compute Standard Deviations*/
                double StdDevNetCountAlpha = 0;
                double StdDevNAlpha = 0;
                double StdDevNetCountBeta = 0;
                double StdDevNBeta = 0;

                for (int i = 0; i < NumSamples; i++)
                {
                    StdDevNetCountAlpha += (AverageAlphaGCPM - Convert.ToDouble(QC_Table[2, i].Value)) * (AverageAlphaGCPM - Convert.ToDouble(QC_Table[2, i].Value));
                    StdDevNAlpha += (AverageAlphaNCPM - Convert.ToDouble(QC_Table[3, i].Value)) * (AverageAlphaNCPM - Convert.ToDouble(QC_Table[3, i].Value));
                    StdDevNetCountBeta += (AverageBetaGCPM - Convert.ToDouble(QC_Table[4, i].Value)) * (AverageBetaGCPM - Convert.ToDouble(QC_Table[4, i].Value));
                    StdDevNBeta += (AverageBetaNCPM - Convert.ToDouble(QC_Table[5, i].Value)) * (AverageBetaNCPM - Convert.ToDouble(QC_Table[5, i].Value));
                }

                if (NumSamples > 1)
                {
                    StdDevNetCountAlpha /= (NumSamples - 1);
                    StdDevNAlpha /= (NumSamples - 1);
                    StdDevNetCountBeta /= (NumSamples - 1);
                    StdDevNBeta /= (NumSamples - 1);

                    StdDevNetCountAlpha = Math.Sqrt(StdDevNetCountAlpha);
                    StdDevNAlpha = Math.Sqrt(AverageAlphaNCPM);
                    StdDevNetCountBeta = Math.Sqrt(StdDevNetCountBeta);
                    StdDevNBeta = Math.Sqrt(AverageBetaNCPM);
                }

                else
                {
                    StdDevNetCountAlpha = Math.Sqrt(StdDevNetCountAlpha);
                    StdDevNAlpha = Math.Sqrt(StdDevNAlpha);
                    StdDevNetCountBeta = Math.Sqrt(StdDevNetCountBeta);
                    StdDevNBeta = Math.Sqrt(StdDevNBeta);
                }

                StdDevAlphaNetCountCell.Value = StaticMethods.RoundToSigFigs(StdDevNetCountAlpha);
                StdDevAlphaNCell.Value = StaticMethods.RoundToSigFigs(StdDevNAlpha);
                StdDevBetaNetCountCell.Value = StaticMethods.RoundToSigFigs(StdDevNetCountBeta);
                StdDevBetaNCell.Value = StaticMethods.RoundToSigFigs(StdDevNBeta);

                
                /*Display overall pass/fail*/
                if (this.TestPassed)
                {
                    FinalPassFail.Value = "PASS";
                }

                else
                {
                    FinalPassFail.Value = "FAIL";
                }

                this.BackgroundFinished = DateTime.Now;
                WasBackgroundFinishedSuccessfully = true;

                if (BackgroundThreadFinished != null)
                {
                    using (SoundPlayer S = new SoundPlayer(DABRAS_Software.Properties.Resources.DoneSound))
                    {
                        S.Play();
                    }

                    BackgroundThreadFinished(this, null);
                }
                //MessageBox.Show("Done!");
            }

            /*Resume background threads, if they existed*/
            this.DABRAS.ResumeBackgroundMonitors();
            this.Running = false;

            return;
        }
        #endregion
    }

    public class QCAlphaBetaListener
    {
        #region Data Members

        private bool Done;
        private DataGridView QC_Table;
        private int SampleTime;
        private int NumSamples;
        private DABRAS DABRAS;
        private bool ShouldStop;
        private bool WasBackgroundFinishedSuccessfully;
        private DateTime BackgroundFinished;
        private bool TestPassed = false;

        private bool IsAlphaTest = false;

        private double Hi;
        private double Lo;

        private double AlphaGCPM;
        private double BetaGCPM;
        private double AlphaNCPM;
        private double BetaNCPM;

        private int AlphaBackground;
        private int BetaBackground;

        private DateTime TestStarted;

        public event EventHandler BackgroundThreadFinished;

        private bool Running = false;

        private int BadgeNo;
        private string Name;

        #endregion

        #region Constructor
        public QCAlphaBetaListener(DABRAS _DABRAS, int _SampleTime, int _NumSamples, DataGridView _QC_Table, double _Hi, double _Lo, bool _IsAlpha, int _AlphaBG, int _BetaBG, DateTime _Today, int _BadgeNo, string _Name)
        {
            this.DABRAS = _DABRAS;
            this.QC_Table = _QC_Table;
            this.SampleTime = _SampleTime;
            this.NumSamples = _NumSamples;
            WasBackgroundFinishedSuccessfully = false;
            ShouldStop = false;

            this.TestStarted = _Today;

            this.Hi = _Hi;
            this.Lo = _Lo;

            this.IsAlphaTest = _IsAlpha;

            this.AlphaBackground = _AlphaBG;
            this.BetaBackground = _BetaBG;
            this.BadgeNo = _BadgeNo;
            this.Name = _Name;
            return;
        }
        #endregion

        #region Control Functions
        public void RequestStop()
        {
            ShouldStop = true;
        }
        #endregion

        #region Getters
        public string GetName()
        {
            return this.Name;
        }

        public int GetBadgeNo()
        {
            return this.BadgeNo;
        }

        public int GetSampleTime()
        {
            return this.SampleTime;
        }

        public int GetNumSamples()
        {
            return this.NumSamples;
        }

        public bool WasTestPassed()
        {
            return this.TestPassed;
        }

        public double GetHi()
        {
            return Hi;
        }

        public double GetLo()
        {
            return Lo;
        }

        public bool AlphaTest()
        {
            return this.IsAlphaTest;
        }

        public double GetAlphaGCPM()
        {
            return this.AlphaGCPM;
        }

        public double GetBetaGCPM()
        {
            return this.BetaGCPM;
        }

        public double GetAlphaNCPM()
        {
            return this.AlphaNCPM;
        }

        public double GetBetaNCPM()
        {
            return this.BetaNCPM;
        }

        public DateTime GetDateTimeStarted()
        {
            return this.TestStarted;
        }

        public DateTime GetDateTimeCompleted()
        {
            return this.BackgroundFinished;
        }

        public bool WasTestCompleted()
        {
            return this.WasBackgroundFinishedSuccessfully;
        }

        public bool IsRunning()
        {
            return this.Running;
        }

        #endregion

        #region Main Background Thread
        public void Get_Background()
        {
            this.Running = true;
            /*Stop all background threads*/
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

            /*Set test passed flag*/
            this.TestPassed = true;

            for (int i = 0; i < NumSamples; i++)
            {

                bool RowComplete = false;
                DABRAS.Write_To_Serial_Port("g");
                DABRAS.ClearSerialPacket();
                DABRAS.ClearPacketFlag();
                //Thread.Sleep(100);

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
                        BackgroundThreadFinished(this, null);
                        return;
                    }

                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();
                    
                    DABRAS.KickWatchdog();

                    if (IncomingData != null)
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
                    else
                    {
                        return;
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

                    /*Grab handles to form*/
                    if (!ShouldStop)
                    {
                        DataGridViewCell TimeElapsed = QC_Table[1, i];
                        DataGridViewCell AlphaGCPM = QC_Table[2, i];
                        DataGridViewCell AlphaNCPM = QC_Table[3, i];
                        DataGridViewCell BetaGCPM = QC_Table[4, i];
                        DataGridViewCell BetaNCPM = QC_Table[5, i];
                        DataGridViewCell PassFail = QC_Table[6, i];

                        /*Parse data to form*/
                        TimeElapsed.Value = IncomingData.ElTime;

                        if (IncomingData != null && IncomingData.ElTime != 0)
                        {
                            AlphaGCPM.Value = StaticMethods.RoundToDecimal((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)), 1);
                            AlphaNCPM.Value = StaticMethods.RoundToDecimal((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)) - AlphaBackground, 1);
                            BetaGCPM.Value = StaticMethods.RoundToSigFigs((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)));
                            BetaNCPM.Value = StaticMethods.RoundToDecimal((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)) - BetaBackground, 1);

                            /*Re-draw table*/
                            QC_Table.Invalidate();

                            /*If the sample time has elapsed, increment the row and compute Pass/Fail.*/
                            if (IncomingData.ElTime >= SampleTime)
                            {
                                if (IsAlphaTest)
                                {
                                    if (((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)) - AlphaBackground >= Lo) && ((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)) - AlphaBackground <= Hi))
                                    {
                                        PassFail.Value = "PASS";
                                    }
                                    else
                                    {
                                        PassFail.Value = "FAIL";
                                        this.TestPassed = false; //Test fails if one of the test fails.
                                    }
                                }

                                else
                                {
                                    if (((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)) - BetaBackground >= Lo) && ((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)) - BetaBackground <= Hi))
                                    {
                                        PassFail.Value = "PASS";
                                    }
                                    else
                                    {
                                        PassFail.Value = "FAIL";
                                        this.TestPassed = false; //Test fails if one of the test fails.
                                    }
                                }

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
                double AverageAlphaGCPM = 0;
                double AverageBetaGCPM = 0;
                double AverageAlphaNCPM = 0;
                double AverageBetaNCPM = 0;

                for (int i = 0; i < NumSamples; i++)
                {
                    AverageAlphaGCPM += Convert.ToDouble(QC_Table[2, i].Value);
                    AverageAlphaNCPM += Convert.ToDouble(QC_Table[3, i].Value);
                    AverageBetaGCPM += Convert.ToDouble(QC_Table[4, i].Value);
                    AverageBetaNCPM += Convert.ToDouble(QC_Table[5, i].Value);
                }

                AverageAlphaGCPM /= NumSamples;
                AverageBetaGCPM /= NumSamples;
                AverageAlphaNCPM /= NumSamples;
                AverageBetaNCPM /= NumSamples;

                DataGridViewCell AverageAlphaGCPMCell = QC_Table[2, NumSamples];
                DataGridViewCell AverageAlphaNCPMCell = QC_Table[3, NumSamples];
                DataGridViewCell AverageBetaGCPMCell = QC_Table[4, NumSamples];
                DataGridViewCell AverageBetaNCPMCell = QC_Table[5, NumSamples];

                DataGridViewCell StdDevAlphaGCell = QC_Table[2, NumSamples + 1];
                DataGridViewCell StdDevAlphaNCell = QC_Table[3, NumSamples + 1];
                DataGridViewCell StdDevBetaGCell = QC_Table[4, NumSamples + 1];
                DataGridViewCell StdDevBetaNCell = QC_Table[5, NumSamples + 1];
                DataGridViewCell FinalPassFail = QC_Table[6, NumSamples + 1];


                AverageAlphaGCPMCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaGCPM);
                AverageAlphaNCPMCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaNCPM);
                AverageBetaGCPMCell.Value = StaticMethods.RoundToSigFigs(AverageBetaGCPM);
                AverageBetaNCPMCell.Value = StaticMethods.RoundToSigFigs(AverageBetaNCPM);

                this.AlphaGCPM = AverageAlphaGCPM;
                this.AlphaNCPM = AverageAlphaNCPM;
                this.BetaGCPM = AverageBetaGCPM;
                this.BetaNCPM = AverageBetaNCPM;

                /*Compute Standard Deviations*/
                double StdDevGAlpha = 0;
                double StdDevNAlpha = 0;
                double StdDevGBeta = 0;
                double StdDevNBeta = 0;

                for (int i = 0; i < NumSamples; i++)
                {
                    StdDevGAlpha += (AverageAlphaGCPM - Convert.ToDouble(QC_Table[2, i].Value)) * (AverageAlphaGCPM - Convert.ToDouble(QC_Table[2, i].Value));
                    StdDevNAlpha += (AverageAlphaNCPM - Convert.ToDouble(QC_Table[3, i].Value)) * (AverageAlphaNCPM - Convert.ToDouble(QC_Table[3, i].Value));
                    StdDevGBeta += (AverageBetaGCPM - Convert.ToDouble(QC_Table[4, i].Value)) * (AverageBetaGCPM - Convert.ToDouble(QC_Table[4, i].Value));
                    StdDevNBeta += (AverageBetaNCPM - Convert.ToDouble(QC_Table[5, i].Value)) * (AverageBetaNCPM - Convert.ToDouble(QC_Table[5, i].Value));
                }

                if (NumSamples > 1)
                {
                    StdDevGAlpha /= (NumSamples - 1);
                    StdDevNAlpha /= (NumSamples - 1);
                    StdDevGBeta /= (NumSamples - 1);
                    StdDevNBeta /= (NumSamples - 1);

                    StdDevGAlpha = Math.Sqrt(StdDevGAlpha);
                    StdDevNAlpha = Math.Sqrt(StdDevNAlpha);
                    StdDevGBeta = Math.Sqrt(StdDevGBeta);
                    StdDevNBeta = Math.Sqrt(StdDevNBeta);
                }

                else
                {
                    StdDevGAlpha = Math.Sqrt(AverageAlphaGCPM);
                    StdDevNAlpha = Math.Sqrt(AverageAlphaNCPM);
                    StdDevGBeta = Math.Sqrt(AverageBetaGCPM);
                    StdDevNBeta = Math.Sqrt(AverageBetaNCPM);

                }
                /*
                if (StdDevGAlpha != StdDevGAlpha)
                {
                    StdDevGAlpha = 0;
                }

                if (StdDevNAlpha != StdDevNAlpha)
                {
                    StdDevGAlpha = 0;
                }

                if (StdDevGBeta != StdDevGBeta)
                {
                    StdDevGBeta = 0;
                }

                if (StdDevNBeta != StdDevNBeta)
                {
                    StdDevNBeta = 0;
                }
                */
                StdDevAlphaGCell.Value = StaticMethods.RoundToSigFigs(StdDevGAlpha);
                StdDevAlphaNCell.Value = StaticMethods.RoundToSigFigs(StdDevNAlpha);
                StdDevBetaGCell.Value = StaticMethods.RoundToSigFigs(StdDevGBeta);
                StdDevBetaNCell.Value = StaticMethods.RoundToSigFigs(StdDevNBeta);
                
                /*Display overall pass/fail*/
                if (this.TestPassed)
                {
                    FinalPassFail.Value = "PASS";
                }

                else
                {
                    FinalPassFail.Value = "FAIL";
                }

                this.BackgroundFinished = DateTime.Now;
                WasBackgroundFinishedSuccessfully = true;
                if (BackgroundThreadFinished != null)
                {

                    using (SoundPlayer S = new SoundPlayer(DABRAS_Software.Properties.Resources.DoneSound))
                    {
                        S.Play();
                    }

                    BackgroundThreadFinished(this, null);
                }
                //MessageBox.Show("Done!");
            }

            /*Resume any background threads, if they existed*/
            DABRAS.ResumeBackgroundMonitors();
            this.Running = false;

            return;
        }
        #endregion
    }
}
