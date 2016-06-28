using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace DABRAS_Software
{
    /*HOMEFORM.CS
     * This form will be used for all general-purpose data-logging, data-receiving, and data display.
     * 
     * @author Mitchell Spryn
     * @version 0.1
     */
    
    public partial class HomeForm : Form
    {

        #region Data Members
        /*Communication Instance with PIC*/
        private DABRAS DABRAS;

        private DefaultConfigurations DefaultConfig = null;
        private Logger Logger = null;

        private bool TestingVariable = false;
        #endregion

        #region Constructor
        public HomeForm()
        {
            InitializeComponent();

            /*Initialize the writers*/
            Logger = new Logger();
            Logger.InitializeLogger();
            
            /*Attempt to load the Default Class*/
            string CurrentDir = Environment.CurrentDirectory;
            string DataPath = String.Concat(CurrentDir, "\\conf\\DABRAS_conf.dat");
            try
            {
                if (File.Exists(DataPath))
                {
                    Stream S = File.Open(DataPath, FileMode.Open, FileAccess.Read);
                    IFormatter BF = new BinaryFormatter(); //Open for reading
                    if (S.Length != 0)
                    {
                        DefaultConfig = (DefaultConfigurations)BF.Deserialize(S);
                    }
                    S.Close();
                }
            }
            catch
            {
                MessageBox.Show("Error: This program needs hard disk read/write access to perform. Undefined results will occour when it cannot write to disk.");
            }

            DABRAS = new DABRAS();
            if (DefaultConfig == null)
            {
                /*Config not found, create it*/
                DefaultConfig = new DefaultConfigurations();
                MessageBox.Show("Default Config not found, creating new one");
            }
            else
            {
                DABRAS = new DABRAS(DefaultConfig, Logger);
                if (DABRAS.VCP_Instance != "")
                {
                    try
                    {
                        if (DABRAS.Get_Coms(DABRAS.VCP_Instance))
                        {
                            DABRAS.Initialize();

                            this.DABRAS_Firmware_Label.Text = "Firmware Version: " + DABRAS.Firmware_Version;
                            this.DABRAS_Serial_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                            this.VCP_Status_Label.Text = "STATUS: Connected!";

                            /*TEST: Start background monitor thread with really short timeout*/
                            //DABRAS.StartMonitoringBackground(0, 0, 10, 10, 10);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("ERROR: Unable to connect on default port. Please manually configure the connection.");
                    }
                }
            }

            Logger.CreateDirectoryTree();

            this.DefaultConfig.ClearModifiedFlag();

            /*Add keypress event handlers*/
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

            Logger.WriteLineToStatusLog(String.Format("Program started at time {0}", DateTime.Now));

        }
        #endregion

        #region PortConnectHandler

        private void connectToAPortCtrlPToolStripMenuItem_Click(object sender, EventArgs e)
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
                    this.DABRAS_Serial_Label.Text = "Serial Number: ??" ;
                    this.VCP_Status_Label.Text = "STATUS: Disconnected";
                    DABRAS.VCP_Instance = "";

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

                /*Write to constants*/
                DefaultConfig.WriteDefaultVCP(NewPopup.VCP_Port);
            }

        }

        #endregion

        #region Routine_Sample Handler
        private void Routine_Sample_Button_Click(object sender, EventArgs e)
        {
            
            BatchNumber NewForm = new BatchNumber();
            try
            {
                
                if (NewForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(String.Format("Caught {0}", E.Message));
                return;
            }
            
            /*User pushed OK, get data*/
            mainFramework RoutineForm = new mainFramework(NewForm.Get_Group_ID());
            RoutineForm.Show();
            this.Hide();

            if (RoutineForm.WasDABRASModified())
            {
                this.DABRAS = RoutineForm.GetDABRAS();
                this.DefaultConfig.WriteDefaultVCP(DABRAS.VCP_Instance);
            }

            if (RoutineForm.WasDCModified())
            {
                this.DefaultConfig = RoutineForm.GetDefaultConfig();
                
            }
            
            return;
        }
        #endregion

        #region QC_Testing Handler
        private void QC_Testing_Button_Click(object sender, EventArgs e)
        {
            QCMain NewForm = new QCMain(this);

            NewForm.Show();
            this.Hide();

            if (NewForm.WasDABRASModified())
            {
                this.DABRAS = NewForm.GetDABRAS();
                this.DefaultConfig.WriteDefaultVCP(DABRAS.VCP_Instance);
            }

            if (NewForm.WereSourcesModified())
            {
                DefaultConfig.SetRadioactiveSourceList(NewForm.GetNewSources());
            }

            if (NewForm.WasDCModified())
            {
                this.DefaultConfig = NewForm.GetDefaultConfigurations();
            }
        }
        #endregion

        #region Parameter Button Handler
        private void Parameter_Summary_Button_Click(object sender, EventArgs e)
        {
            ParamSummaryForm NewForm = new ParamSummaryForm(this);
            NewForm.Show();
            this.Hide();

            if (NewForm.New_Instance_Of_DABRAS())
            {
                /*If the user made changes to the DABRAS object, write it to the current one*/
               this.DABRAS = NewForm.DABRAS;

                /*Change the labels*/
                this.DABRAS_Firmware_Label.Text = "Firmware Version: " + DABRAS.Firmware_Version;
                this.DABRAS_Serial_Label.Text = "Serial Number: " + DABRAS.Serial_Number;
                if (DABRAS.IsConnected())
                {
                    this.VCP_Status_Label.Text = "STATUS: Connected!";
                }
                else
                {
                    this.VCP_Status_Label.Text = "STATUS: Disconnected!";
                }
            }
        }
        #endregion

        #region CalibrationHandler
        private void calibrationF5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*First, authenticate*/
            ChallengePassword CP = new ChallengePassword();
            if (CP.ShowDialog() != DialogResult.OK)
            {
                /*User did not submit password*/
                CP.Dispose();
                return;
            }

            /*Check password. Check this awesome security!*/
            if (String.Compare(CP.GetUserEnteredPassword(), DefaultConfig.GetPassword()) != 0)
            {
                MessageBox.Show("Incorrect Password.");
                CP.Dispose();
                return;
            }
            
            CalibrationForm CF = new CalibrationForm(this);

            CF.Show(this);

            this.Hide();

            //this.Show();

            ///*Grab pointers again*/
            //if (CF.WasDABRASModified())
            //{
            //    this.DABRAS = CF.GetDABRAS();
            //}

            //if (CF.WasDCModified())
            //{
            //    this.DefaultConfig = CF.GetDefaultConfig();
            //}

            //if (CF.WereSourcesModified())
            //{
            //    DefaultConfig.SetRadioactiveSourceList(CF.GetSourceList());
            //}

        }
        #endregion

        #region QuickCal Handler
        private void quickCalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*First, authenticate*/
            ChallengePassword CP = new ChallengePassword();
            if (CP.ShowDialog() != DialogResult.OK)
            {
                /*User did not submit password*/
                CP.Dispose();
                return;
            }

            /*Check password. Check this awesome security!*/
            if (String.Compare(CP.GetUserEnteredPassword(), DefaultConfig.GetPassword()) != 0)
            {
                MessageBox.Show("Incorrect Password.");
                CP.Dispose();
                return;
            }
            
            QuickCalibrationController QC = new QuickCalibrationController(this);
            QC.RunCalibrationProc();

            /*Read in list of calibrated sources*/
            DefaultConfig.SetRadioactiveSourceList(QC.GetListOfSources());
        }
        #endregion

        #region Webform Handler
        private void openWebBasedSurveyF12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DefaultConfig.GetWebSurvey());
            NewForm.Show();
        }

        private void openRSOSharepointF11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DefaultConfig.GetRSOLink());
            NewForm.Show();
        }

        private void openRSOHomeF10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(DefaultConfig.GetRSOHome());
            NewForm.Show();
        }
        #endregion

        #region Edit Link Handler
        private void editEmbeddedLinkTargetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkTargets NewForm = new LinkTargets(this.DefaultConfig);
            NewForm.ShowDialog();

            if (NewForm.DialogResult == DialogResult.OK)
            {
                this.DefaultConfig = NewForm.GetDefaultConfig();
            }
        }
        #endregion

        #region KeyHandler
        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Alt)
            {
                if (Key.KeyCode == Keys.F4)
                {
                    exitCtrlQToolStripMenuItem_Click(this, null);
                    return;
                }
            }
            
            if (Key.Control)
            {
                if (Key.KeyCode == Keys.Q)
                {
                    this.TestingVariable = false;
                    exitCtrlQToolStripMenuItem_Click(this, null);
                    return;
                }

                if (Key.KeyCode == Keys.P)
                {
                    this.TestingVariable = false;
                    connectToAPortCtrlPToolStripMenuItem_Click(this, null);
                    return;
                }

                if (Key.KeyCode == Keys.U)
                {
                    this.TestingVariable = true;
                    return;
                }

                if ((Key.KeyCode == Keys.A) && (this.TestingVariable))
                {
                    RollTide RTR = new RollTide();
                    RTR.ShowDialog();
                    this.TestingVariable = false;
                    return;
                }

                if (Key.KeyCode == Keys.K)
                {
                    editUserListCtrlKToolStripMenuItem_Click(this, null);
                }
            }

            /*Check for F2 for Routine Sample Counting*/
            if (Key.KeyCode == Keys.F2)
            {
                this.TestingVariable = false;
                Routine_Sample_Button_Click(this, null);
                return;
            }

            /*Check F3 for QC test*/
            if (Key.KeyCode == Keys.F3)
            {
                this.TestingVariable = false;
                QC_Testing_Button_Click(this, null);
                return;
            }

            /*Check F4 for Param Summary button*/
            //if (Key.KeyCode == Keys.F4)
            //{
            //    this.TestingVariable = false;
            //    Parameter_Summary_Button_Click(this, null);
            //    return;
            //}

            /*Check F5 for Calibration*/
            if (Key.KeyCode == Keys.F5)
            {
                this.TestingVariable = false;
                calibrationF5ToolStripMenuItem_Click(this, null);
                return;
            }

            if (Key.KeyCode == Keys.F12)
            {
                this.TestingVariable = false;
                openWebBasedSurveyF12ToolStripMenuItem_Click(this, null);
                return;
            }

            if (Key.KeyCode == Keys.F11)
            {
                this.TestingVariable = false;
                openRSOSharepointF11ToolStripMenuItem_Click(this, null);
                return;
            }

            if (Key.KeyCode == Keys.F10)
            {
                this.TestingVariable = false;
                openRSOHomeF10ToolStripMenuItem_Click(this, null);
                return;
            }

            if (Key.KeyCode == Keys.F6)
            {
                this.TestingVariable = false;
                quickCalToolStripMenuItem_Click(this, null);
                return;
            }

            this.TestingVariable = false;

            return;
        }
        #endregion

        #region About Button 
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm NewForm = new AboutForm();
            NewForm.ShowDialog();
        }
        #endregion

        #region Dummy Overloads

        private void exitCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            FinalStuffToDo();
            this.Dispose();
        }

        private void routineSampleCountingF2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Routine_Sample_Button_Click(this, null);
        }

        private void qCTestingF3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QC_Testing_Button_Click(this, null);
        }

        private void parameterSummaryF4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Parameter_Summary_Button_Click(this, null);
        }

        #endregion

        #region Getters

        public DABRAS GetDABRAS()
        {
            return this.DABRAS;
        }

        public List<Radioactive_Source> GetListOfSources()
        {
            return this.DefaultConfig.GetListOfSources();
        }

        //public List<QCCalResultNode> GetListOfQCResults()
        //{
        //    return this.DefaultConfig.GetListOfCalResults();
        //}

        public Logger GetLogger()
        {
            return this.Logger;
        }

        public DefaultConfigurations GetDefaultConfig()
        {
            return this.DefaultConfig;
        }

        public string GetCalibrationPassword()
        {
            return DefaultConfig.GetPassword();
        }

        #endregion

        #region Finalization
        private void FinalStuffToDo()
        {
            /*Kill the background monitoring threads, if it exists*/
            DABRAS.KillAllMonitors();
            
            /*Check to see if there is anything of value to write*/
            //if (DefaultConfig.IsNew())
            if (true)
            {
                string CurrentDir = Environment.CurrentDirectory;
                string DataPath = String.Concat(CurrentDir, "\\conf\\");
                
                /*Create directory if it doesn't exist*/
                try
                {

                    if (!Directory.Exists(DataPath))
                    {
                        Directory.CreateDirectory(DataPath);
                    }
                }

                catch
                {
                    MessageBox.Show("Error writing default config data: No access! No calibration data from today saved...");
                }

                /*Create the file if it doesn't exist*/
                DataPath = String.Concat(DataPath, "DABRAS_conf.dat");
                
                IFormatter BF = new BinaryFormatter();
                using (Stream FileStream = new FileStream(DataPath, FileMode.Create, FileAccess.Write))
                {
                    BF.Serialize(FileStream, DefaultConfig); 
                }
            }


            /*He's dead, Jim!*/
        }
        #endregion

        #region Show/Hide Handler
        private void HomeForm_VisibleChanged(object sender, EventArgs e)
        {
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

        private void editUserListCtrlKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifyUserList NewForm = new ModifyUserList(this, this.DefaultConfig, this.DABRAS, this.Logger);
            NewForm.Show();
            this.Hide();
        }

        private void HomeForm_Activated(object sender, EventArgs e)
        {
            /*Serialize the data every time the home form is shown, as crash protection*/
            string CurrentDir = Environment.CurrentDirectory;
            string DataPath = String.Concat(CurrentDir, "\\conf\\");

            /*Create directory if it doesn't exist*/
            try
            {

                if (!Directory.Exists(DataPath))
                {
                    Directory.CreateDirectory(DataPath);
                }
            }

            catch
            {
                MessageBox.Show("Error writing default config data: No access! No calibration data from today saved...");
            }

            /*Create the file if it doesn't exist*/
            DataPath = String.Concat(DataPath, "DABRAS_conf.dat");

            IFormatter BF = new BinaryFormatter();
            using (Stream FileStream = new FileStream(DataPath, FileMode.Create, FileAccess.Write))
            {
                BF.Serialize(FileStream, DefaultConfig);
            }
            
        }

    }
}
