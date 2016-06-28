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
    public partial class FormQC : Form
    {
        #region Public Enums
        public enum TypeOfQC { Background, Alpha, Beta }
        #endregion

        #region Data Members
        private mainFramework frmParent;

        private DABRAS DABRAS;
        private bool DABRASModified = false;
        private QCListKeeper QC_List;

        private DefaultConfigurations DC;
        private bool DCModified = false;

        private double AlphaHi = -1;
        private double AlphaLo = -1;
        private double BetaHi = -1;
        private double BetaLo = -1;

        private bool UsingAnnual = true;

        private bool SourcesModified = false;
       
        private TypeOfQC _typ;

        QCBackgroundListener QCBG = null;
        QCAlphaBetaListener QCAB = null;

        private Thread OperationThread_QCBG = null;
        private Thread OperationThread_QCAB = null;

        private Radioactive_Source SelectedSource;
        private RadionuclideFamily SelectedFamily;

        private List<User> ListOfUsers;
        private List<Radioactive_Source> ListOfSources;
        private List<RadionuclideFamily> ListOfFamily;

        private Form ChildDialogue;

        public delegate void BG_GUI_Callback();
        public delegate void AB_GUI_Callback();
        private bool bgPassed, abPassed;

        private int NumSamples = 0;
        private int Sampletime = 0;
        private int BadgeNo = 0;

        #endregion

        #region Constructor
        public FormQC(mainFramework Parent, TypeOfQC _T)
        {
            InitializeComponent();

            this.frmParent = Parent;

            this.DABRAS = frmParent.GetDABRAS();
            this.Type = _T;
            this.DC = frmParent.GetDefaultConfig();
            this.ListOfUsers = frmParent.GetListOfUsers();
            this.ListOfSources = frmParent.GetListOfSources();
            this.ListOfFamily = frmParent.GetListOfFamily();

            this.updUserCombo();

            this.initQCList();

            this.initDataGrid();

            this.setSourceDefault();
        }
        #endregion        

        #region QCList initialization created by QZ by copying the QC initialization portion from QCMain.cs
        private void initQCList()
        {
            string CurrentDir = Environment.CurrentDirectory;
            string DataPath = String.Concat(CurrentDir, "\\data\\QC\\Master\\QC_Data.dat");
            if (File.Exists(DataPath))
            {
                try
                {
                    using (Stream S = File.Open(DataPath, FileMode.Open, FileAccess.Read))
                    {
                        IFormatter BF = new BinaryFormatter(); //Open for reading
                        if (S.Length != 0)
                        {
                            this.QC_List = (QCListKeeper)BF.Deserialize(S);
                        }
                    }
                }
                catch
                {
                    this.QC_List = null;
                }
            }

            if (this.QC_List == null)
            {
                this.QC_List = new QCListKeeper();
            }

            this.DABRAS.SetQCList(this.QC_List);
        }
        #endregion

        #region initialize the GridView
        private void initDataGrid()
        {
            this.ShortDataGridView.DataSource = null;
            this.ShortDataGridView.Rows.Clear();
            this.ShortDataGridView.Columns.Clear();

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

            if (this.ShortDataGridView.Columns.Count == 0)//adding columns only at the first time
            {
                ShortDataGridView.Columns.Add("ShortDataGridView_Time", "Sample No");
                ShortDataGridView.Columns.Add("ShortDataGridView_Time", "Acq Time");
                ShortDataGridView.Columns.Add("ShortDataGridView_AGCPM", "Alpha GCPM");
                if (this.Type != TypeOfQC.Background)
                    ShortDataGridView.Columns.Add("ShortDataGridView_ANCPM", "Alpha NCPM");
                ShortDataGridView.Columns.Add("ShortDataGridView_BGCPM", "Beta GCPM");
                if (this.Type != TypeOfQC.Background)
                    ShortDataGridView.Columns.Add("ShortDataGridView_BNCPM", "Beta NCPM");
                ShortDataGridView.Columns.Add("ShortDataGridView_PassFail", "Pass/Fail");
            }

            //Add keypress event handlers
            //this.KeyPreview = true;
            //this.KeyDown += new KeyEventHandler(KeyPressed);

            Add_Or_Subtract_Rows(NumberOfRows, this.ShortDataGridView);

            for (int col_i = 0; col_i < this.ShortDataGridView.Columns.Count; col_i++)
            {
                DataGridViewColumn column = this.ShortDataGridView.Columns[col_i];
                if (column.Index == 0 || column.Index == (this.ShortDataGridView.ColumnCount - 1))
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                else
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }
        #endregion

        #region user management
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
                try
                {
                    FoundUser = this.ListOfUsers.Find(x => x.GetBadgeNo() == BadgeNo);
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
        private void Badge_CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillEmployeeData(this, null);
        }
        private void updUserCombo()
        {
            this.Badge_CB.Items.Clear();

            foreach (User U in this.ListOfUsers)
            {
                this.Badge_CB.Items.Add(Convert.ToString(U.GetBadgeNo()));
            }
            this.Badge_CB.SelectedIndex = this.Badge_CB.Items.Count - 1;
        }
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            string userNm;
            int bdgNum;

            try
            {
                bdgNum = Convert.ToInt32(this.tbBadge.Text);
            }
            catch (Exception eNum)
            {
                MessageBox.Show("Please enter numbers only for Badge number! " + eNum.Message);
                return;
            }
            userNm = Convert.ToString(this.tbUserName.Text);

            User tgtUser = this.ListOfUsers.Find(x => (x.GetBadgeNo() == bdgNum));
            if (tgtUser == null)
            {
                this.ListOfUsers.Add(new User(bdgNum, userNm));
            }
            else
                tgtUser.SetName(userNm);

            this.updUserCombo();

            this.frmParent.updUsers(this.ListOfUsers);

            return;
        }
        private void btnRemoveUser_Click(object sender, EventArgs e)
        {
            User tgtUser = this.ListOfUsers.Find(x => (x.GetName() == Convert.ToString(this.tbUserName.Text) && x.GetBadgeNo() == Convert.ToInt32(this.tbBadge.Text)));
            if (tgtUser != null)
            {
                this.ListOfUsers.Remove(tgtUser);
                this.updUserCombo();
                this.frmParent.updUsers(this.ListOfUsers);
            }
            return;
        }
        #endregion

        #region Find source and set labels correctly
        private void setSourceDefault() //QZ added this function
        {
            if (this.Type == TypeOfQC.Background)
            {
                this.SelectedFamily = this.ListOfFamily.Find(x => x.GetName() == "Background");

                this.AlphaHi = this.SelectedFamily.GetAlphaHi();
                this.AlphaLo = this.SelectedFamily.GetAlphaLo();
                this.BetaHi = this.SelectedFamily.GetBetaHi();
                this.BetaLo = this.SelectedFamily.GetBetaLo();

                this.Alpha_GCPM_Label.Text = String.Format("Alpha GCPM must be between {0} and {1} (Average of {2}, Determined {3})", AlphaHi, AlphaLo, StaticMethods.RoundToSigFigs(this.SelectedFamily.GetAlphaHiLoAvg()), this.SelectedFamily.GetHiLoCalibratedTime());
                this.Beta_GCPM_Label.Text = String.Format("Beta GCPM must be between {0} and {1} (Average of {2}, Determined {3})", BetaHi, BetaLo, StaticMethods.RoundToSigFigs(this.SelectedFamily.GetBetaHiLoAvg()), this.SelectedFamily.GetHiLoCalibratedTime());

                //Change the default to the correct value
                this.Min_TB.Text = "10";
                this.Beta_GCPM_Label.Visible = true;
                this.Alpha_GCPM_Label.Visible = true;
            }

            if (this.Type == TypeOfQC.Alpha)
            {
                this.SelectedFamily = this.ListOfFamily.Find(x => x.GetName() == "Am-241");

                this.AlphaHi = this.SelectedFamily.GetAlphaHi();
                this.AlphaLo = this.SelectedFamily.GetAlphaLo();

                this.Alpha_GCPM_Label.Text = String.Format("Alpha NCPM must be between {0} and {1} (Average of {2}, Determined {3})", AlphaHi, AlphaLo, StaticMethods.RoundToSigFigs(this.SelectedFamily.GetAlphaHiLoAvg()), this.SelectedFamily.GetHiLoCalibratedTime());
                this.Beta_GCPM_Label.Visible = false;
                this.Alpha_GCPM_Label.Visible = true;
            }

            if (this.Type == TypeOfQC.Beta)
            {
                this.SelectedFamily = this.ListOfFamily.Find(x => x.GetName() == "Sr-90");

                this.BetaHi = Convert.ToDouble(StaticMethods.RoundToSigFigs(SelectedFamily.GetBetaHi() * this.SelectedFamily.GetDecayFactor(DateTime.Now, this.SelectedFamily.GetHiLoCalibratedTime())));
                this.BetaLo = Convert.ToDouble(StaticMethods.RoundToSigFigs(SelectedFamily.GetBetaLo() * this.SelectedFamily.GetDecayFactor(DateTime.Now, this.SelectedFamily.GetHiLoCalibratedTime())));

                this.Alpha_GCPM_Label.Visible = false;
                this.Beta_GCPM_Label.Visible = true;
                this.Beta_GCPM_Label.Text = String.Format("Beta NCPM must be between {0} and {1} (Average of {2}, Determined {3})", BetaHi, BetaLo, StaticMethods.RoundToSigFigs(this.SelectedFamily.GetBetaHiLoAvg()), this.SelectedFamily.GetHiLoCalibratedTime());
            }
        }

        #endregion

        #region ResetQCType and the form content respectively
        private void ResetQCType(TypeOfQC _T)//added by QZ
        {
            this.Type = _T;
            this.initDataGrid();
            this.setSourceDefault();
        }
        #endregion

        #region Start Count Button Handler
        private bool Prepare4Start() //added by QZ
        {
            try
            {
                this.BadgeNo = Convert.ToInt32(this.Badge_CB.Text);
                if (String.Compare(Name_TB.Text, "") == 0)
                {
                    //Throw random exception
                    Exception k = new Exception();
                    throw k;
                }
            }
            catch
            {
                MessageBox.Show("Error: Please input your name and badge number.");
                return false;
            }

            try
            {
                this.NumSamples = Math.Abs(Convert.ToInt32(this.Counts_TB.Text));
                this.Sampletime = Math.Abs((60 * Convert.ToInt32(this.Min_TB.Text)) + Convert.ToInt32(this.Sec_TB.Text));
                if(this.NumSamples <= 0 || this.Sampletime <= 0)
                {
                    MessageBox.Show("Please enter positive numbers for samples and sampling time.");
                    return false;
                }
            }
            catch
            {
                MessageBox.Show("Error: Bad Sample Parameters");
                return false;
            }

            if (!DABRAS.IsConnected())
            {
                MessageBox.Show("Error: Connection to DABRAS needed. Please re-connect and try again.");
                this.frmParent.refreshConnectStatus();
                this.endFormActivities();
                return false;
            }

            this.initDataGrid();
            this.ClearDataGridView(this.ShortDataGridView);

            SetGUI(true);
            return true;
        }

        private void btnStartCount_Click(object sender, EventArgs e)
        {
            if( !this.Prepare4Start())
                return;
            if (this.Type == TypeOfQC.Background)
            {
                QCBG = new QCBackgroundListener(this.DABRAS, Sampletime, NumSamples, this.ShortDataGridView, this.AlphaHi, this.AlphaLo, this.BetaHi, this.BetaLo, DateTime.Now, BadgeNo, this.Name_TB.Text);
                QCBG.setCallerForm(this.frmParent);

                OperationThread_QCBG = new Thread(() => QCBG.Get_Background());
                OperationThread_QCBG.Start();

                QCBG.BackgroundThreadFinished += (s, args) => { InvokeBGCallback(); };
            }
            else
            {
                Radioactive_Source R = ListOfSources.Find(x => x.GetSerialNumber() == "Background");

                bool IsAlpha = (this.Type == TypeOfQC.Alpha);

                if (IsAlpha)
                {
                    if (this.UsingAnnual)
                    {
                        QCAB = new QCAlphaBetaListener(this.DABRAS, Sampletime, NumSamples, this.ShortDataGridView, this.AlphaHi, this.AlphaLo, IsAlpha, Convert.ToInt32(R.GetAnnualAlphaCPM()), Convert.ToInt32(R.GetAnnualBetaCPM()), DateTime.Now, BadgeNo, this.Name_TB.Text);
                        QCAB.setCallerForm(this.frmParent);
                    }
                    else
                    {
                        QCAB = new QCAlphaBetaListener(this.DABRAS, Sampletime, NumSamples, this.ShortDataGridView, this.AlphaHi, this.AlphaLo, IsAlpha, Convert.ToInt32(R.GetDailyAlphaCPM()), Convert.ToInt32(R.GetDailyBetaCPM()), DateTime.Now, BadgeNo, this.Name_TB.Text);
                        QCAB.setCallerForm(this.frmParent);
                    }
                }
                else
                {
                    if (this.UsingAnnual)
                    {
                        QCAB = new QCAlphaBetaListener(this.DABRAS, Sampletime, NumSamples, this.ShortDataGridView, this.BetaHi, this.BetaLo, IsAlpha, Convert.ToInt32(R.GetAnnualAlphaCPM()), Convert.ToInt32(R.GetAnnualBetaCPM()), DateTime.Now, BadgeNo, this.Name_TB.Text);
                        QCAB.setCallerForm(this.frmParent);
                    }
                    else
                    {
                        QCAB = new QCAlphaBetaListener(this.DABRAS, Sampletime, NumSamples, this.ShortDataGridView, this.BetaHi, this.BetaLo, IsAlpha, Convert.ToInt32(R.GetDailyAlphaCPM()), Convert.ToInt32(R.GetDailyBetaCPM()), DateTime.Now, BadgeNo, this.Name_TB.Text);
                        QCAB.setCallerForm(this.frmParent);
                    }
                }

                OperationThread_QCAB = new Thread(() => QCAB.Get_Background());
                OperationThread_QCAB.Start();

                QCAB.BackgroundThreadFinished += (s, args) => { InvokeABCallback(); };
            }
        }
        #endregion

        #region set check buttons
        private void setCheckButtons()
        {
            this.btnStartCount.Enabled = true;
            this.StopButton.Enabled = false;

            if (Type == TypeOfQC.Background)
            {
                this.btnAlphaCheck.Enabled = true;
                this.btnBetaCheck.Enabled = false;
                this.btnAlphaCheck.Visible = true;
                this.btnBetaCheck.Visible = false;
            }
            if (Type == TypeOfQC.Alpha)
            {
                this.btnBackgroundCheck.Enabled = true;
                this.btnBackgroundCheck.Visible = true;
                this.btnBetaCheck.Enabled = true;
                this.btnBetaCheck.Visible = true;
            }
            if (Type == TypeOfQC.Beta)
            {
                this.btnBackgroundCheck.Enabled = true;
                this.btnBackgroundCheck.Visible = true;
                this.btnAlphaCheck.Enabled = true;
                this.btnAlphaCheck.Visible = true;
            }
        }
        #endregion

        #region check buttons' handlers
        private void btnBackgroundCheck_Click(object sender, EventArgs e)
        {
            this.btnAlphaCheck.Enabled = false;
            this.btnBetaCheck.Enabled = false;
            this.btnAlphaCheck.Visible = false;
            this.btnBetaCheck.Visible = false;
            this.ResetQCType(TypeOfQC.Background);
        }

        private void btnAlphaCheck_Click(object sender, EventArgs e)
        {
            this.btnBackgroundCheck.Enabled = true;
            this.btnBetaCheck.Enabled = false;
            this.btnBetaCheck.Visible = false;
            this.ResetQCType(TypeOfQC.Alpha);
        }

        private void btnBetaCheck_Click(object sender, EventArgs e)
        {
            this.btnAlphaCheck.Enabled = true;
            this.btnBackgroundCheck.Enabled = true;
            this.ResetQCType(TypeOfQC.Beta);
        }
        #endregion

        #region Show Full Data Set Handler
        private void ShowFullDataSetButton_Click(object sender, EventArgs e)
        {
            FullDataSetPopup NewForm = new FullDataSetPopup(QCAB, QCBG, this.frmParent.GetLogger());
            NewForm.ShowDialog();
        }
        #endregion

        #region Background Calibration Handlers
        private void SetButton_Click(object sender, EventArgs e)
        {
            this.initDataGrid();
        }
        #endregion

        #region Show Graph Handler
        private void ShowGraphButton_Click(object sender, EventArgs e)
        {
            DailyQCGraph NewForm = new DailyQCGraph(this, this.QC_List, this.frmParent.GetLogger(), this.ListOfSources, this.DC);
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

        private void SetGUI(bool Running)
        {
            btnStartCount.Enabled = !Running;
            CVSSaveButton.Enabled = !Running;
            ShowGraphButton.Enabled = !Running;
            ShowFullDataSetButton.Enabled = !Running;
            ImageSaveButton.Enabled = !Running;     
            ShowLogButton.Enabled = !Running;
            StopButton.Enabled = Running;

            this.Counts_TB.Enabled = !Running;
            this.Min_TB.Enabled = !Running;
            this.Sec_TB.Enabled = !Running;
            this.Badge_CB.Enabled = !Running;
            this.Name_TB.Enabled = !Running;

            /*removed...
            aquireDataCtrlAToolStripMenuItem.Enabled = !Running;
            saveToFileCtrlVToolStripMenuItem.Enabled = !Running;
            saveImageCtrlIToolStripMenuItem.Enabled = !Running;
            stopAquisitionCtrlSToolStripMenuItem.Enabled = Running;
             * */
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

        #region ShowLog Button Handler
        private void ShowLogButton_Click(object sender, EventArgs e)
        {
            if (QC_List.GetFullList().Count > 0)
            {
                DailyQCLog NewForm = new DailyQCLog(this, QC_List.GetFullList(), DABRAS, this.frmParent.GetLogger());
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
            string[,] DataToWrite = this.frmParent.MakeDataWritable(ShortDataGridView);
            SaveFileDialog SD = new SaveFileDialog();
            SD.Filter = "Comma Separated Value|*.csv";
            SD.ShowDialog();
            if (SD.FileName != "")
            {
                FileStream F = (FileStream)SD.OpenFile();
                this.frmParent.GetLogger().WriteCSV(F, DataToWrite);
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

        #region Stop Handler
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

            this.SetGUI(false);
        }
        #endregion

        #region Getters Setters
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

        public QCListKeeper GetQCList()
        {
            return this.QC_List;
        }

        public TypeOfQC Type
        {
            get { return _typ; }
            set { _typ = value; }
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
            }

            if (Key.KeyCode == Keys.Enter)
            {
                FillEmployeeData(this, null);
            }
        }
        #endregion

        #region Finalization
        public void QC_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.endFormActivities();
        }

        public void endFormActivities()
        {
            //Stop the BG listener, if it exists
            if (QCBG != null)
            {
                QCBG.RequestStop();
            }

            //Stop the AB listener, if it exists
            if (QCAB != null)
            {
                QCAB.RequestStop();
            }

            //Wait for the background thread to terminate
            if (OperationThread_QCBG != null)
            {
                OperationThread_QCBG.Join();//added by QZ to block the current thread until the QCAB's thread terminates
            }
            if (OperationThread_QCAB != null)
            {
                OperationThread_QCAB.Join();//added by QZ to block the current thread until the QCAB's thread terminates
            }
        }

        private void writeQCdata()
        {
            //Check to see if there is anything of value to write
            if (QC_List.IsNew())
            {
                string CurrentDir = Environment.CurrentDirectory;
                string DataPath = String.Concat(CurrentDir, "\\data\\QC\\Master\\QC_Data.dat");
                try
                {
                    //Create the file if it doesn't exist
                    if (!File.Exists(DataPath))
                    {
                        File.Create(DataPath).Dispose();
                    }

                    IFormatter BF = new BinaryFormatter();
                    using (Stream FileStream = new FileStream(DataPath, FileMode.Create, FileAccess.Write))
                    {

                        BF.Serialize(FileStream, QC_List); //hangs...
                    }

                }
                catch
                {
                    MessageBox.Show("Error in QC Master List Serialization.");
                }
            }
        }
        #endregion

        private void Counts_TB_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.NumSamples = Convert.ToInt32(this.Counts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Counts must be an integer greater than zero. Please try again.");
                return;
            }
            this.ClearDataGridView(this.ShortDataGridView);
            this.Add_Or_Subtract_Rows(this.NumSamples, this.ShortDataGridView);
            this.ShortDataGridView.Invalidate();
        }
    }
}
