using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace DABRAS_Software
{
    public partial class QCMain : Form
    {
        #region Data Members
        private readonly mainFramework LaunchedFrom;
        private DABRAS DABRAS;
        private Logger Logger;
        private DefaultConfigurations DC;

        private List<Radioactive_Source> ListOfSources;

        private QCListKeeper QC_List;

        private bool SourcesModified = false;

        private bool DABRASModified = false;
        private bool DCModified = false;

        private Form ChildDialogue;

        #endregion

        #region Constructor
        public QCMain(Form Parent)
        {
            InitializeComponent();

            this.LaunchedFrom = (mainFramework)Parent;
            this.DABRAS = LaunchedFrom.GetDABRAS();
            this.Logger = LaunchedFrom.GetLogger();
            this.ListOfSources = LaunchedFrom.GetListOfSources();
            this.DC = LaunchedFrom.GetDefaultConfig();
            //this.ListOfCalibrationResults = HF.GetListOfQCResults();

            if (DABRAS.IsConnected())
            {
                this.DABRAS_Firmware_Label.Text = "Firmware Version: " + DABRAS.Firmware_Version;
                this.DABRAS_Serial_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                this.VCP_Status_Label.Text = "STATUS: Connected!";
            }

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
                            QC_List = (QCListKeeper)BF.Deserialize(S);
                        }
                    }
                }
                catch
                {
                    QC_List = null;
                }
            }

            if (QC_List == null)
            {
                QC_List = new QCListKeeper();
            }

            DABRAS.SetQCList(QC_List);

            /*Add keypress event handlers*/
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Startup Events
        private void QCMain_Shown(object sender, EventArgs e)
        {
            /*Only activated the FIRST time form is shown*/
            Radioactive_Source BG = ListOfSources.Find(x => x.GetName() == "Background");

            /*Check calibration dates*/
            DateTime RecalibrationDate = BG.GetAnnualCalibratedTime().AddYears(1);
            if (DateTime.Now.Month == RecalibrationDate.Month && DateTime.Now.Year == RecalibrationDate.Year)
            {
                MessageBox.Show("Warning: Annual calibration due at the end of this month. This machine will not be capable of counting routine smears without a calibration.");
            }

        }
        #endregion

        #region Background Calibration Handler
        private void Background_Button_Click(object sender, EventArgs e)
        {
            QC NewForm = new QC(this, QC.TypeOfQC.Background);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.WereSourcesModified())
            //{
            //    this.ListOfSources = NewForm.GetModifiedSources();
            //    this.SourcesModified = true;
            //}

            //if (NewForm.WasDCModified())
            //{
            //    this.DC = NewForm.GetDefaultConfig();
            //}

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region Alpha Calibration Handler
        private void AlphaButton_Click(object sender, EventArgs e)
        {
            QC NewForm = new QC(this, QC.TypeOfQC.Alpha);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.WereSourcesModified())
            //{
            //    this.ListOfSources = NewForm.GetModifiedSources();
            //    this.SourcesModified = true;
            //}

            //if (NewForm.WasDCModified())
            //{
            //    this.DC = NewForm.GetDefaultConfig();
            //}

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region Beta Calibration Handler
        private void BetaButton_Click(object sender, EventArgs e)
        {
            QC NewForm = new QC(this, QC.TypeOfQC.Beta);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.WereSourcesModified())
            //{
            //    this.ListOfSources = NewForm.GetModifiedSources();
            //    this.SourcesModified = true;
            //}

            //if (NewForm.WasDCModified())
            //{
            //    this.DC = NewForm.GetDefaultConfig();
            //}

            //if (NewForm.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region Close Handler
        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }
        #endregion

        #region Automation Handler
        private void automationControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutomationControls Newform = new AutomationControls(this);
            this.ChildDialogue = Newform;
            Newform.Show();
            this.Hide();

            //if (Newform.WasDABRASModified())
            //{
            //    this.DABRAS = Newform.GetDABRAS();
            //    this.DABRASModified = true;
            //}

            //if (Newform.WasDefaultConfigurationsModified())
            //{
            //    this.DC = Newform.GetDefaultConfig();
            //    this.DCModified = true;
            //}

            //if (Newform.DialogResult == DialogResult.Abort)
            //{
            //    AbortAll();
            //}
        }
        #endregion

        #region Control Graphs Handler
        private void viewControlGraphsF9ToolStripMenuItem_Click(object sender, EventArgs e)
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

        #region Beta Efficiency Graphs
        private void viewBetaEfficiencyGraphsF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BetaEfficiencyChart NewForm = new BetaEfficiencyChart(this, this.ListOfSources);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            if (NewForm.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }
        #endregion

        #region ChiSquared Handler
        private void ChiSquaredButton_Click(object sender, EventArgs e)
        {
            ChiSquared NewForm = new ChiSquared(this);
            this.ChildDialogue = NewForm;
            NewForm.Show();
            this.Hide();

            //if (NewForm.DialogResult != DialogResult.Abort)
            //{
            //    if (NewForm.WasDABRASModified())
            //    {
            //        this.DABRAS = NewForm.GetDABRAS();
            //        this.DABRASModified = true;
            //    }

            //    if (NewForm.WasDCModified())
            //    {
            //        this.DC = NewForm.GetDefaultConfigurations();
            //        this.DCModified = true;
            //    }
            //    return;
            //}

            //else
            //{
            //    AbortAll();
            //}
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

        #region Port Connect Handler
        private void connectDisconnectToAPortCtrlPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*Show the dialogue*/
            VCPConnect NewPopup = new VCPConnect("Connect");
            if (NewPopup.ShowDialog() == DialogResult.OK)
            {
                /*The user clicked OK! Attempt to connect to the DABRAS*/
                bool Successful = DABRAS.Get_Coms(NewPopup.VCP_Port);
                if (!(Successful))
                {
                    MessageBox.Show("Error: Communication could not be established.");
                    Logger.WriteLineToStatusLog(String.Format("Communication attempt failed at time {0}", DateTime.Now));

                    this.DABRAS_Firmware_Label.Text = "Firmware Version: ??";
                    this.DABRAS_Serial_Label.Text = "Serial Number: ??";
                    this.VCP_Status_Label.Text = "STATUS: Disconnected";
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
                this.DABRAS_Serial_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                this.VCP_Status_Label.Text = "STATUS: Connected!";

                /*Set DABRAS Flag*/
                this.DABRASModified = true;
            }

            if (NewPopup.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }
        #endregion

        #region Getters
        public QCListKeeper GetQCList()
        {
            return this.QC_List;
        }

        public bool WasDABRASModified()
        {
            return this.DABRASModified;
        }

        public DABRAS GetDABRAS()
        {
            return this.DABRAS;
        }

        public Logger GetLogger()
        {
            return this.Logger;
        }

        public List<Radioactive_Source> GetListOfSources()
        {
            return this.ListOfSources;
        }

        public bool WereSourcesModified()
        {
            return this.SourcesModified;
        }

        public List<Radioactive_Source> GetNewSources()
        {
            return this.ListOfSources;
        }

        public DefaultConfigurations GetDefaultConfigurations()
        {
            return this.DC;
        }

        public bool WasDCModified()
        {
            return this.DCModified;
        }
        #endregion

        #region Dummy Overloads
        private void dailyBackgroundCheckCtrlBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Background_Button_Click(this, null);
        }

        private void dailyAm241SourceCheckCtrlAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AlphaButton_Click(this, null);
        }

        private void dailySr90SourceCheckCtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BetaButton_Click(this, null);
        }

        private void writeDailyQCResultsToFileCtrlWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WriteToFileButton_Click(this, null);
        }

        private void performChiSquaredTestCtrlXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChiSquaredButton_Click(this, null);
        }
        #endregion

        #region Abort Handler
        private void AbortAll()
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
            return;
        }
        #endregion

        #region Test functions
        private void DummyButton_Click(object sender, EventArgs e)
        {
            QC_List.ForceClear();
            QC_List.MakeDummyList();
            MessageBox.Show("List created!");
            return;
        }

        private void ForceClearButton_Click(object sender, EventArgs e)
        {
            QC_List.ForceClear();
            MessageBox.Show("List destroyed!");
            return;
        }
        #endregion

        #region Write to File Handler
        private void WriteToFileButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog SD = new SaveFileDialog();
            SD.Filter = "Text File|*.txt";
            SD.ShowDialog();

            if (SD.FileName != "")
            {
                FileStream F = (FileStream)SD.OpenFile();
                TextWriter TW = new StreamWriter(F);

                TW.WriteLine("Daily QC Results");
                Write_Spacer(TW);

                Radioactive_Source BG = ListOfSources.Find(x => x.GetName() == "Background");
                Radioactive_Source Am = ListOfSources.Find(x => x.GetName() == "Am-241");
                Radioactive_Source Sr = ListOfSources.Find(x => x.GetName() == "Sr-90");

                TimeSpan ONE_DAY = new TimeSpan(24,0,0);
                

                #region Background


                TW.WriteLine("Background Results");
                if (DateTime.Now.Subtract(BG.GetDailyCalibratedTime()) < ONE_DAY)
                {
                    TW.WriteLine(String.Format("The background was last successfully calibrated on {0}, with an Alpha NCPM of {1} and a Beta NCPM of {2}", BG.GetDailyCalibratedTime(), BG.GetDailyAlphaCPM(), BG.GetDailyBetaCPM()));
                }

                else
                {
                    TW.WriteLine("Background not calibrated within 24 hours.");
                }

                Write_Spacer(TW);

                #endregion

                #region Alpha

                TW.WriteLine("Alpha Results");
                if (DateTime.Now.Subtract(Am.GetDailyCalibratedTime()) < ONE_DAY)
                {
                    TW.WriteLine(String.Format("The Alpha source was last successfully calibrated at time {0} with a NCPM of {1}", Am.GetDailyCalibratedTime(), Am.GetDailyAlphaCPM()));
                }

                else
                {
                    TW.WriteLine("Alpha Source not calibrated within 24 hours.");
                }

                Write_Spacer(TW);

                #endregion

                #region Beta

                TW.WriteLine("Beta Results");
                if (DateTime.Now.Subtract(Sr.GetDailyCalibratedTime()) < ONE_DAY)
                {
                    TW.WriteLine(String.Format("The Beta source was last successfully calibrated at time {0} with a NCPM of {1}", Sr.GetDailyCalibratedTime(), Sr.GetDailyBetaCPM()));
                }

                else
                {
                    TW.WriteLine("Beta Source not calibrated within 24 hours.");
                }

                Write_Spacer(TW);

                #endregion

                #region Chi Squared

                TW.WriteLine(String.Format("The Chi Squared test was last performed on {0}", DC.GetChiSquaredDate()));

                #endregion

                TW.Close();
                F.Close();

                MessageBox.Show("File written.");
            }
        }

        private void Write_Spacer(TextWriter T)
        {
            T.WriteLine();
            T.WriteLine("****************************************************************");
            T.WriteLine();
            return;
        }

        #endregion

        #region KeyPresses
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
                    closeCtrlQToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.B)
                {
                    Background_Button_Click(this, null);
                }

                if (Key.KeyCode == Keys.A)
                {
                    AlphaButton_Click(this, null);
                }

                if (Key.KeyCode == Keys.S)
                {
                    BetaButton_Click(this, null);
                }

                if (Key.KeyCode == Keys.P)
                {
                    connectDisconnectToAPortCtrlPToolStripMenuItem_Click(this, null);
                }

                if (Key.KeyCode == Keys.X)
                {
                    ChiSquaredButton_Click(this, null);
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

            if (Key.KeyCode == Keys.F9)
            {
                viewControlGraphsF9ToolStripMenuItem_Click(this, null);
            }

            if (Key.KeyCode == Keys.F8)
            {
                viewBetaEfficiencyGraphsF8ToolStripMenuItem_Click(this, null);
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            /*Get sources*/
            Radioactive_Source Am241 = ListOfSources.Find(x => x.GetName() == "Am-241");
            Radioactive_Source Sr90 = ListOfSources.Find(x => x.GetName() == "Sr-90");
            Radioactive_Source Background = ListOfSources.Find(x => x.GetName() == "Background");

            Am241.SetAlphaEfficiency(23.8);
            Sr90.SetBetaEfficiency(48.5);
            Background.SetAlphaDisintigrationConstant(1);
            Background.SetBetaDisintigrationFactor(192);


            MessageBox.Show("Slap.");

            return;
        }

        #region Finalization
        private void QCMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*Check to see if there is anything of value to write*/
            if (QC_List.IsNew())
            {
                string CurrentDir = Environment.CurrentDirectory;
                string DataPath = String.Concat(CurrentDir, "\\data\\QC\\Master\\QC_Data.dat");
                try
                {
                    
                    /*Create the file if it doesn't exist*/

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
                    MessageBox.Show("Error in Master List Serialization.");
                }
            }

            this.LaunchedFrom.Show();

        }
        #endregion

        #region Show/Hide Handler
        private void QCMain_VisibleChanged(object sender, EventArgs e)
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
                    this.DABRAS_Serial_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                    this.VCP_Status_Label.Text = "STATUS: Connected!";
                }
            }
        }
        #endregion

        private void showCalibrationDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CalibrationInfo NewForm = new CalibrationInfo(this.ListOfSources);
            NewForm.ShowDialog();

            if (NewForm.DialogResult == DialogResult.Abort)
            {
                AbortAll();
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

    }
}
