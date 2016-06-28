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
        #region Data Members
        private mainFramework frmParent;
        private DABRAS DABRAS = null;
        private Logger Logger = null;
        protected string[] ValidSourceNames = {};
        protected string[,] ValidSourceRadiationTypes = {{},{}};
        private List<RadionuclideFamily> ListOfValidFamily;
        private List<Radioactive_Source> ListOfValidSources = null;
        
        private EfficiencyListener EL;
        private BackgroundListener BL;

        private bool NewSources = false;
        private bool NewDABRAS = false;

        private bool NewPasswordSet = false;
        private string NewPassword = "";

        private double AlphaEff;
        private double BetaEff;

        private string[,] bkgrdData, eff_Data;

        private Thread BackgroundBackgroundThread;
        private Thread EfficiencyBackgroundThread;

        private DefaultConfigurations DC;
        private bool DCModified = false;

        private bool Calibrating = false;
        private bool BackgroundCompleted = false;
        private bool EfficiencyCompleted = false;
        private bool CalibrationCompleted = false;

        private Form ChildDialogue = null;

        public delegate void OnBackgroundThreadFinished();
        #endregion
        
        #region Constructor
        public FormCLB(mainFramework Parent)
        {
            InitializeComponent();
            frmParent = Parent;
            this.ListOfValidFamily = frmParent.GetListOfFamily();
            this.ListOfValidSources = frmParent.GetListOfSources();
            this.DABRAS = frmParent.GetDABRAS();
            this.Logger = frmParent.GetLogger();
            this.DC = frmParent.GetDefaultConfig();

            this.initBackgroundTable();
            this.initEfficiencyTable();

            this.updFamilyAndSource(this.ListOfValidFamily, this.ListOfValidSources);
            this.updateSourceTypeLabels();
            
            this.initButtonStates(false);
            //this.fillDGVsWithPreviousResults();
        }
        #endregion

        public void updFamilyAndSource(List<RadionuclideFamily> famList, List<Radioactive_Source> srcList)
        {
            this.ListOfValidSources = srcList;

            //Initialize/update source combobox 
            this.Source_ComboBox.Items.Clear();
            foreach (Radioactive_Source i in srcList)
            {
                if (i.GetSerialNumber() != "Background")
                {
                    this.Source_ComboBox.Items.Add(i.GetSerialNumber());
                }
            }
            this.Source_ComboBox.SelectedIndex = 0;

            //and update the family's current_efficiency with the efficiency of the source which has the most cureent calibration date.
            DateTime dt = new DateTime(2006, 8, 1, 0, 0, 0);
            Radioactive_Source curr_src = null;
            foreach (RadionuclideFamily rf in famList)
            {
                foreach(Radioactive_Source src in srcList)
                {
                    if(src.GetFamilyID() == rf.GetFamilyID())
                    {
                        if (DateTime.Compare(dt, src.GetAnnualCalibratedTime()) < 0)
                        {
                            dt = src.GetAnnualCalibratedTime();
                            curr_src = src;
                        }
                    }    
                }
                if(curr_src != null)
                    rf.SetCurrentSource(curr_src);
            }

            this.ListOfValidFamily = famList;

            this.SetSourceHalfLifeAndCurrentlyAppliedActivity();
        }
        public void updParentFamilyAndSource(List<RadionuclideFamily> famList, List<Radioactive_Source> srcList)
        {
            this.frmParent.updFamilyAndSource(famList, srcList);
        }

        #region Efficiency Start Button Handler
        private void Determine_Efficiency_Button_Click(object sender, EventArgs e)
        {
            int NumSamples;
            int SampleTime;

            try
            {
                NumSamples = Convert.ToInt32(this.EFF_NumCounts_TB.Text);
                SampleTime = (Convert.ToInt32(this.Min_EFF_TB.Text) * 60) + Convert.ToInt32(this.Sec_EFF_TB.Text);
                Math.Sqrt(SampleTime);
                if(NumSamples <= 0 || SampleTime <= 0)
                {
                    MessageBox.Show("Please enter positive numbers for samples and sampling time.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Error: Bad parameters. Please enter valid numerical parameters for the Efficiency Scan and try again.");
                return;
            }

            if (!this.DABRAS.IsConnected())
            {
                MessageBox.Show("Error: Must be connected to DABRAS to determine its efficiency. Please make a connection, and try again.");
                this.frmParent.refreshConnectStatus();
                this.endFormActivities();
                this.SetGUI(false);
                return;
            }

            if (!Calibrating)
            {
                if ((MessageBox.Show(String.Format("Source will be counted {0} times for {1} seconds each. Proceed?", NumSamples, SampleTime), "Verify", MessageBoxButtons.YesNo)) != DialogResult.Yes)
                {
                    MessageBox.Show("Aborted.");
                    this.CalibrationCompleted = false;
                    this.SetGUI(false);
                    return;
                }
            }

            this.ClearDataGridView(this.Calibration_Results_GridView);
            this.ClearDataGridView(this.EfficiencyFootDGV);

            //Clear any data left in the buffer
            this.DABRAS.ClearSerialPacket();
            this.DABRAS.ClearPacketFlag();

            Radioactive_Source effSrc = ListOfValidSources.Find(x => (String.Compare(x.GetSerialNumber(), this.Source_ComboBox.Text) == 0));

            Radioactive_Source BG = ListOfValidSources.Find(x => (String.Compare(x.GetSerialNumber(), "Background") == 0));

            this.EL = new EfficiencyListener(this.DABRAS, SampleTime, NumSamples, this.Calibration_Results_GridView, this.EfficiencyFootDGV, this.BackgroudFootDGV, effSrc, this.TypeOfRadiationLabel1.Text);
            this.EfficiencyBackgroundThread = new Thread(() => EL.Get_Efficiency());

            this.EL.EfficiencyBackgroundThreadFinished += (s, args) => { this.InvokeThreadCallback(); };

            this.EfficiencyBackgroundThread.Start();

            this.Stop_Count_Button.Text = "Stop Efficiency Scanning";
            this.SetGUI(true);    
        }
        #endregion

        #region Efficiency Save Button Handler
        private void SaveEfficiencyDataButton_Click(object sender, EventArgs e)
        {
            string[,] StringToWrite = this.frmParent.MakeDataWritable(this.Calibration_Results_GridView);

            SaveFileDialog S = new SaveFileDialog();
            S.Filter = "Comma Separated Value|*.csv";
            S.ShowDialog();
            if (S.FileName != "")
            {
                FileStream F = (FileStream)S.OpenFile();
                string FilePath = S.FileName;
                Logger.WriteCSV(F, StringToWrite);
            }
            this.updFamilyAndSource(this.ListOfValidFamily, this.ListOfValidSources);
            this.updParentFamilyAndSource(this.ListOfValidFamily, this.ListOfValidSources);
            return;
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
                if(NumSamples <= 0 || SampleTime <= 0)
                {
                    MessageBox.Show("Please enter positive numbers for samples and sampling time.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Error: Invalid Parameters. Enter numerial values only.");
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
                    this.SetGUI(false);
                    return;
                }
            }

            this.ClearDataGridView(this.Background_Results_GridView);
            this.ClearDataGridView(this.BackgroudFootDGV);

            //Clear any data left in the buffer
            this.DABRAS.ClearSerialPacket();
            this.DABRAS.ClearPacketFlag();

            Radioactive_Source BGsrc = ListOfValidSources.Find(x => x.GetSerialNumber() == "Background");

            this.BL = new BackgroundListener(this.DABRAS, SampleTime, NumSamples, this.Background_Results_GridView, this.BackgroudFootDGV, BGsrc);
            this.BackgroundBackgroundThread = new Thread(() => BL.Get_Background());
            this.BL.BackgroundSampleThreadFinished += (s, args) => { this.InvokeThreadCallback(); };
            this.BackgroundBackgroundThread.Start();

            this.Stop_Count_Button.Text = "Stop Background Counting";
            this.SetGUI(true);
        }
        #endregion

        #region Background Save Handler
        private void SaveBackgroundCalibrationButton_Click(object sender, EventArgs e)
        {
            string[,] StringToWrite = this.frmParent.MakeDataWritable(this.Background_Results_GridView);

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
            //MessageBox.Show("Done!");
            return;
        }
        #endregion

        #region Misc GUI
        private void Num_Counts_TB_TextChanged(object sender, EventArgs e)
        {
            int NewNumberOfRows = 0;
            try
            {
                NewNumberOfRows = Convert.ToInt32(EFF_NumCounts_TB.Text);
            }
            catch
            {
                MessageBox.Show("Error: Bad Value.");
                return;
            }

            this.Add_Or_Subtract_Rows(NewNumberOfRows, this.Calibration_Results_GridView);

            this.resizeDataGridview(this.Calibration_Results_GridView);

            this.repositionDataGridview(this.Calibration_Results_GridView, this.EfficiencyFootDGV);
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

            this.Add_Or_Subtract_Rows(NewNumberOfRows, this.Background_Results_GridView);

            this.resizeDataGridview(this.Background_Results_GridView);

            this.repositionDataGridview(this.Background_Results_GridView, this.BackgroudFootDGV);
        }

        private void Source_ComboBox_Click(object sender, EventArgs e)
        {
            Source_ComboBox.Items.Clear();
            foreach (Radioactive_Source i in ListOfValidSources)
            {
                if (String.Compare(i.GetSerialNumber(), "Background") != 0)
                {
                    Source_ComboBox.Items.Add(i.GetSerialNumber());
                }
            }
        }

        private void updateSourceTypeLabels()
        {
            Radioactive_Source srcSelected = this.ListOfValidSources.Find(x => x.GetSerialNumber() == this.Source_ComboBox.Text);
            if (srcSelected != null)
            {
                this.TypeOfRadiationLabel1.Text = this.findSourceFamily(srcSelected).GetSourceType_String();
                this.TypeOfRadiationLabel2.Text = this.findSourceFamily(srcSelected).GetSourceType_String();
            }
        }
        private void Source_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {           
            this.fillDGVsWithPreviousResults();
            this.updateSourceTypeLabels();
            this.SelectCorrectBeta();
            this.Calibration_Results_GridView.Columns[0].HeaderText = this.Source_ComboBox.Text;
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
            this.endFormActivities();
            this.Stop_Count_Button.Text = "Stop Counting";
            this.SetGUI(false);
        }
        #endregion

        #region Close Button Handler
        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region View/Edit Calibration Sources Handler
        private void viewEditCtrlOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManageSources NewForm = new ManageSources(this, this.ListOfValidFamily, this.ListOfValidSources);
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

        /**/
        #region HV Plateau Handler
        /*TODO: Pass back data?!*/
        private void highVoltagePlateauCtrlLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHighVoltage NewForm = new FormHighVoltage(this);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region Change Password Handler

        #endregion

        #region Background Type Handler
        private void setBackgroundTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Mod Factors Handler
        private void setModFactorRequirementsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Building Number Handler
        private void setBuildingNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region About Handler
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

        }

        private void saveBackgroundDataCtrlJToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveImageCtrlIToolStripMenuItem_Click(object sender, EventArgs e)
        {

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

        #region Getters
        public mainFramework GetParentForm()
        {
            return this.frmParent;
        }
        public double GetAlphaEff()
        {
            return this.AlphaEff;
        }

        public double GetBetaEff()
        {
            return this.BetaEff;
        }

        public DefaultConfigurations GetDefaultConfig()
        {
            return this.DC;
        }

        public bool IsCalibrationCompleted()
        {/*At calibration time the calibrator is required to calibrate the 3 required families, 
          * which is Am-241, Sr-90, & Tc-99.
          * Without these 3 completed the calibration should not be considered complete.*/
            int newAlphaCal = 0;
            int newBetaCal1 = 0;
            int newBetaCal2 = 0;
            int famAlpha = this.ListOfValidFamily.Find(x=>(x.GetName()=="Am-241")).GetFamilyID();
            int famBeta1 = this.ListOfValidFamily.Find(x=>(x.GetName()=="Sr-90")).GetFamilyID();
            int famBeta2 = this.ListOfValidFamily.Find(x=>(x.GetName()=="Tc-99")).GetFamilyID();
            foreach(Radioactive_Source rs in this.ListOfValidSources)
            {
                TimeSpan T = DateTime.Now.Subtract(rs.GetAnnualCalibratedTime());
                if (rs.GetFamilyID() == famAlpha && TimeSpan.Compare(T, new TimeSpan(365, 0, 0, 0)) < 0)
                    newAlphaCal += 1;
                if (rs.GetFamilyID() == famBeta1 && TimeSpan.Compare(T, new TimeSpan(365, 0, 0, 0)) < 0)
                    newBetaCal1 += 1;
                if (rs.GetFamilyID() == famBeta2 && TimeSpan.Compare(T, new TimeSpan(365, 0, 0, 0)) < 0)
                    newBetaCal2 += 1;
            }
            this.CalibrationCompleted = (newAlphaCal >= 1 && newBetaCal1 >= 1 && newBetaCal2 >= 1);
            return this.CalibrationCompleted;
        }

        public bool WereSourcesModified()
        {
            return this.NewSources;
        }
        public List<RadionuclideFamily> GetFamilyList()
        {
            return this.ListOfValidFamily;
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
        }
        #endregion

        #region Finalization
        private void FormCLB_FormClosing(object sender, FormClosingEventArgs e)
        {
            endFormActivities();
        }

        public void endFormActivities()
        {
            //Stop the BG listener, if it exists
            if (this.EL != null)
            {
                EL.RequestStop();
                //EfficiencyBackgroundThread.Join();//Wait for the efficiency thread to terminate
            }

            if (this.BL != null)
            {
                BL.RequestStop();
                //BackgroundBackgroundThread.Join();//Wait for the background thread to terminate
            }
        }
        #endregion

        #region handle DataGridview sizing and positioning
        private int GetDataGridViewHeight(DataGridView dataGridView)
        {
            var sum = (dataGridView.ColumnHeadersVisible ? dataGridView.ColumnHeadersHeight : 0) +
                      dataGridView.Rows.OfType<DataGridViewRow>().Where(r => r.Visible).Sum(r => r.Height);

            return sum;
        }
        private void resizeDataGridview(DataGridView dgv)
        {//This is to resize the DataGridview when the actual number of rows changes.
            dgv.Height = this.GetDataGridViewHeight(dgv) + dgv.Padding.Top + dgv.Padding.Bottom;
        }
        private void repositionDataGridview(DataGridView dgv1, DataGridView dgv2)
        {
            Point p1 = dgv1.Location;
            Point p2 = new Point(p1.X, p1.Y + dgv1.Height + dgv1.Padding.Bottom);
            dgv2.Location = p2;
        }
        #endregion 

        private void btnManageSources_Click(object sender, EventArgs e)
        {
            ManageSources frmSrcMng = new ManageSources(this, this.ListOfValidFamily, this.ListOfValidSources);
            this.ChildDialogue = frmSrcMng;
            frmSrcMng.Show(this);
            this.Enabled = false;
        }

        private void btnEstablishHiLoLimits_Click(object sender, EventArgs e)
        {
            FormHiLo frmHiLo = new FormHiLo(this);
            if (!frmHiLo.IsDisposed)
            {
                this.ChildDialogue = frmHiLo;
                frmHiLo.Show();
                this.Enabled = false;//Hide();
            }
        }

        private void btnHighVoltage_Click(object sender, EventArgs e)
        {
            FormHighVoltage frmHighVoltage = new FormHighVoltage(this);
            if (!frmHighVoltage.IsDisposed)
            {
                this.ChildDialogue = frmHighVoltage;
                frmHighVoltage.Show();
                this.Enabled = false;// Hide();
            }
        }

        private void btnSetHVCtrl_Click(object sender, EventArgs e)
        {
            if (this.DABRAS.IsConnected())
            {
                FormHVCsetting frmHVC = new FormHVCsetting(this.DABRAS);
                frmHVC.ShowDialog();
                this.Enabled = false;

                if(frmHVC.DialogResult == DialogResult.Abort)
                {
                    AbortAll();
                }
            }
            else
            {
                MessageBox.Show("Error: A valid connection the DABRAS is needed to set the High Voltage Control. Please re-connect and try again.");
            }

            this.Enabled = true;
            return;
        }
    }
}