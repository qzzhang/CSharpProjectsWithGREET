using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace DABRAS_Software
{
    public partial class mainFramework : Form
    {
        #region Enums
        public enum BackgroundType { Annual, Daily };
        #endregion

        #region Data Members
        //Communication Instance with PIC
        private DABRAS DABRAS;

        //These 4 are all from RSC, to be changed to private with setters/getters
        public bool DABRASModified = false;
        public BackgroundType BGType = BackgroundType.Annual;
        public bool DefaultConfigModified = false;

        private Logger Logger = null;
        private List<User> ListOfUsers;
        private List<RadionuclideFamily> ListOfFamily;
        private List<Radioactive_Source> ListOfSources;
        private DefaultConfigurations DefaultConfig = null;
        private FormCLB clbForm = null;
        private FormRSC rscForm = null;
        private FormQC qcForm = null;

        #endregion

        #region Constructor
        public mainFramework(int BadgeNo)
        {
            InitializeComponent();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            //Initialize the writer
            this.Logger = new Logger();
            this.Logger.InitializeLogger();

            if (!this.CreateConfigFromFile())
                this.createDefaultConfig();

            this.DABRAS = new DABRAS(this.DefaultConfig, this.Logger);
            this.ListOfUsers = this.DefaultConfig.GetListOfUsers();
            this.ListOfFamily = this.DefaultConfig.GetListOfFamily();
            this.ListOfSources = this.DefaultConfig.GetListOfSources();

            if (this.ComPortConnected())
            {
                this.setComPortDetails();
            }
        }
        #endregion

        #region startups
        private bool ComPortConnected()
        {
            if (this.DABRAS.VCP_Instance != "")
            {
                try
                {
                    if (this.DABRAS.Get_Coms(DABRAS.VCP_Instance))
                    {
                        this.refreshConnectStatus();
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("ERROR: Failed to connect on default port. Please manually configure the connection.");
                    }
                }
                catch
                {
                    MessageBox.Show("ERROR: Unable to connect on default port. Please manually configure the connection.");
                }
            }
            this.disconnectCleanup("No port connected");
            return false;
        }

        private void setComPortDetails()
        {
            this.Logger.CreateDirectoryTree();

            this.DefaultConfig.ClearModifiedFlag();

            this.Logger.WriteLineToStatusLog(String.Format("Program started at time {0}", DateTime.Now));

            this.createConfgPathFile();

            this.addSubforms();//insert form objects onto TabPages

            this.onStartQCForm();
        }

        private void addSubforms()
        {
            if (this.clbForm == null)
            {
                this.clbForm = new FormCLB(this);
                this.clbForm.TopLevel = false;
                this.clbForm.Enabled = false;
                this.clbForm.Visible = true;
                this.clbForm.FormBorderStyle = FormBorderStyle.None;
                this.clbForm.Dock = DockStyle.Fill;
                this.mainTab.TabPages[2].Controls.Add(clbForm);
            }

            if (this.qcForm == null)
            {
                this.qcForm = new FormQC(this, FormQC.TypeOfQC.Background);
                this.qcForm.TopLevel = false;
                this.qcForm.Enabled = false;//the qcForm cannot be enabled until Calibration is completed
                this.qcForm.Visible = true;
                this.qcForm.FormBorderStyle = FormBorderStyle.None;
                this.qcForm.Dock = DockStyle.Fill;
                this.mainTab.TabPages[0].Controls.Add(qcForm);
            }

            if (this.rscForm == null)
            {
                this.rscForm = new FormRSC(this);
                this.rscForm.TopLevel = false;
                this.rscForm.Enabled = false;//the rscForm can not be enabled until both Calibration and QC Testing are passed
                this.rscForm.Visible = true;
                this.rscForm.FormBorderStyle = FormBorderStyle.None;
                this.rscForm.Dock = DockStyle.Fill;
                this.mainTab.TabPages[1].Controls.Add(rscForm);
            }
        }
        #endregion

        #region untility functions
        public void resetDABRAS2Default()
        {
            this.createDefaultConfig();
            this.DABRAS = new DABRAS(this.DefaultConfig, this.Logger);
            this.ListOfFamily = this.DefaultConfig.GetListOfFamily();
            this.ListOfSources = this.DefaultConfig.GetListOfSources();

            this.updateConfgPathFile();
        }
        private void createDefaultConfig()
        {
            //Config not found, create it
            this.DefaultConfig = new DefaultConfigurations();
            MessageBox.Show("Default Config not found, creating new one");
        }
        private bool CreateConfigFromFile()
        {
            //Attempt to load the Default configurations from a file
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
                        this.DefaultConfig = (DefaultConfigurations)BF.Deserialize(S);
                    }
                    S.Close();
                    return true;
                }
            }
            catch
            {
                MessageBox.Show("Error: This program needs hard disk read/write access to perform. Undefined results will occour when it cannot write to disk.");
            }
            return false;
        }
        public string[,] MakeDataWritable(DataGridView DG)
        {
            int Rows = DG.RowCount;
            int Cols = DG.ColumnCount;
            string tmpstr = "";

            string[,] ReturnString = new string[Rows, Cols];

            foreach (DataGridViewColumn col in DG.Columns)
            {
                ReturnString[0, col.Index] = col.HeaderText;
            }

            for (int i = 1; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    ReturnString[i, j] = "";

                    tmpstr = Convert.ToString(DG[j, i - 1].ToolTipText);
                    if (tmpstr != "")
                    {
                        tmpstr = tmpstr.Replace("\r\n", "; ");//replace the Environment.NewLine
                        tmpstr = tmpstr.Remove(tmpstr.Length - 2);//remove the last ";"
                        tmpstr = Convert.ToString(DG[j, i - 1].Value) + "[" + tmpstr + "]";
                    }
                    else
                        tmpstr = Convert.ToString(DG[j, i - 1].Value);

                    ReturnString[i, j] = tmpstr;
                }
            }

            return ReturnString;
        }

        private void createConfgPathFile()
        {
            //Serialize the data at the start of the mainFramework form
            string CurrentDir = Environment.CurrentDirectory;
            string DataPath = String.Concat(CurrentDir, "\\conf\\");

            //Create directory if it doesn't exist
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

            //Create the file if it doesn't exist
            DataPath = String.Concat(DataPath, "DABRAS_conf.dat");

            IFormatter BF = new BinaryFormatter();
            using (Stream FileStream = new FileStream(DataPath, FileMode.Create, FileAccess.Write))
            {
                BF.Serialize(FileStream, this.DefaultConfig);
            }
        }

        public void updateConfgPathFile()
        {
            this.createConfgPathFile();
        }
        #endregion

        #region enable/disable child forms
        public void EnableCLBForm(bool clbActive)
        {
            this.clbForm.Enabled = clbActive;
        }

        public void EnableQCForm(bool qcActive)
        {
            this.qcForm.Enabled = qcActive;
        }

        public void EnableRSCForm(bool rscActive)
        {
            this.rscForm.Enabled = rscActive;
        }
        #endregion

        #region Connect Handler
        public void disconnectCleanup(string prtStatus)
        {
            this.DABRAS_Firmware_Label.Text = "Firmware Version: ??";
            this.DABRAS_Serial_Label.Text = "Serial Number: ??";
            this.DABRAS_Status_Label.Text = "STATUS: " + prtStatus;

            this.DABRAS.VCP_Instance = "";
            this.DABRASModified = true;

            this.connectDisconnectPortCtrlPToolStripMenuItem.Text = "Connect Port";

            return;
        }
        public void refreshConnectStatus()
        {
            if (this.DABRAS.IsConnected())
            {
                this.DABRAS.Initialize();
                this.DABRAS_Firmware_Label.Text = "Firmware Version: " + this.DABRAS.Firmware_Version;
                this.DABRAS_Serial_Label.Text = "Serial Number: " + this.DABRAS.Serial_Number;
                this.DABRAS_Status_Label.Text = "STATUS: Connected to " + this.DABRAS.VCP_Instance;
                this.connectDisconnectPortCtrlPToolStripMenuItem.Text = "Disconnect Port";
            }
            else
                this.disconnectCleanup("No port connected");
        }
        #endregion

        #region WebForm Handler
        private void openWebBasedSurveySystemF12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(this.DefaultConfig.GetWebSurvey());
            NewForm.Show();
        }

        private void openRSOSharepointF11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(this.DefaultConfig.GetRSOLink());
            NewForm.Show();
        }

        private void openRSOHomeF10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebForm NewForm = new WebForm(this.DefaultConfig.GetRSOHome());
            NewForm.Show();
        }
        #endregion

        #region Set Background Type Handler
        private bool checkPassword()
        {
            //First, authenticate
            ChallengePassword CP = new ChallengePassword();
            if (CP.ShowDialog() != DialogResult.OK)
            {
                //User did not submit password
                if (CP.DialogResult == DialogResult.Abort)
                {
                    //AbortAll();//*******************************TODO
                }

                CP.Dispose();
                return false;
            }

            //Check password. Check this awesome security!
            if (String.Compare(CP.GetUserEnteredPassword(), this.DefaultConfig.GetPassword()) != 0)
            {
                MessageBox.Show("Incorrect Password.");
                CP.Dispose();
                return false;
            }
            return true;
        }
        private void setBackgroundTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.checkPassword())
            {
                BackgroundTypeForm NewForm = new BackgroundTypeForm(this.BGType);

                NewForm.ShowDialog();

                if (NewForm.DialogResult == DialogResult.OK)
                {
                    this.BGType = NewForm.GetBackgroundType();

                    this.DefaultConfig.SetRoutineCalibrationBackgroundType(this.BGType);
                    this.DefaultConfigModified = true;
                }

                if (NewForm.DialogResult == DialogResult.Abort)
                {
                    //AbortAll();//******************TODO
                }
            }
            else
                this.mainTab.SelectedIndex = 0;
        }
        #endregion

        #region About Handler
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm NewForm = new AboutForm();
            NewForm.ShowDialog();

            if (NewForm.DialogResult == DialogResult.Abort)
            {
                //AbortAll();//***********************TODO
            }
        }
        #endregion

        #region Toolbar Functions
        private void connectDisconnectPortCtrlPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Show the dialogue
            string connCmd = (this.connectDisconnectPortCtrlPToolStripMenuItem.Text).Contains("Dis") ? "Disconnect" : "Connect";
            VCPConnect vcpPopup = new VCPConnect(connCmd);
            if (vcpPopup.ShowDialog() == DialogResult.OK)
            {
                //The user clicked OK! Attempt to connect to the DABRAS
                bool portConnected = this.DABRAS.Get_Coms(vcpPopup.VCP_Port);
                if (!portConnected)
                {
                    //MessageBox.Show("Communication disconnected!");
                    this.Logger.WriteLineToStatusLog(String.Format("Communication to port {0} lost at time {1}", vcpPopup.VCP_Port, DateTime.Now));

                    this.disconnectCleanup(vcpPopup.VCP_Port + " Disconnected");

                    //MessageBox.Show(String.Format("Communication to port {0} lost at time {1}", vcpPopup.VCP_Port, DateTime.Now));//disconnect the current VCP_Instance
                }
                else
                {
                    MessageBox.Show("Communication to " + vcpPopup.VCP_Port + " Established.");

                    this.refreshConnectStatus();

                    this.setComPortDetails();

                    //Write to constants
                    this.DABRASModified = true;
                }

                //Write to default constants
                DefaultConfig.WriteDefaultVCP(vcpPopup.VCP_Port);
            }
            else if (vcpPopup.DialogResult == DialogResult.Abort)
            {
                //AbortAll();//**************************TODO
            }
        }

        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.rscForm != null)
                this.rscForm.endFormActivities();
            if(this.qcForm != null)
                this.qcForm.endFormActivities();
            if (this.clbForm != null)
                this.clbForm.endFormActivities();

            /*
            do
            {
                this.connectDisconnectPortCtrlPToolStripMenuItem.Text = "Disconnect Port";
                this.connectDisconnectPortCtrlPToolStripMenuItem_Click(this, null);
            } while (this.DABRAS.IsConnected());
            */
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }
        #endregion

        private void onStartQCForm()
        {
            if (this.clbForm.IsCalibrationCompleted())
                this.EnableQCForm(true);
            else
            {
                if (MessageBox.Show("The Calibration is out of date. Please perform the Calbration under the Calbration tab first. UNLESS, you have administrator privilege to bypass?", "Verify", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (this.checkPassword())
                        this.EnableQCForm(true);
                    else
                        this.EnableQCForm(false);
                }
                else
                    this.EnableQCForm(false);
            }
        }
        private void mainTab_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if(this.mainTab.SelectedIndex == 0)//The QC tab is selected
            {
                this.onStartQCForm();
            }
            else if (mainTab.SelectedIndex == 1)//The RSC tab is selected
            {
                if (this.qcForm.QCbackgroundPassed() && this.qcForm.QCAlphaBetaPassed())
                    this.EnableRSCForm(true);
                else
                {
                    MessageBox.Show("The QC testing did not pass. Please perform the QC testing under the QC testing tab first.");
                    this.EnableRSCForm(false);
                    this.EnableQCForm(true);
                    this.mainTab.SelectedIndex = 0;
                }
            }
            else if (mainTab.SelectedIndex == 2)//The CLB tab is selected
            {
                if (MessageBox.Show("To perform the Calbration, you need the administrator privilege. Log in?", "Verify", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (this.checkPassword())
                        this.EnableCLBForm(true);
                    else
                    {
                        this.EnableCLBForm(false);
                        this.mainTab.SelectedIndex = 0;
                    }
                }
                else
                {
                    this.EnableCLBForm(false);
                    this.mainTab.SelectedIndex = 0;
                }
            }
        }

        public void updFamilyAndSource(List<RadionuclideFamily> famList, List<Radioactive_Source> srcList)
        {
            this.ListOfFamily = famList;
            this.DefaultConfig.SetRadioFamilyList(famList);

            this.ListOfSources = srcList;
            this.DefaultConfig.SetRadioactiveSourceList(srcList);

            this.updateConfgPathFile();
            this.rscForm.updateRSCCombos();
        }
        public void updUsers(List<User> usrList)
        {
            this.ListOfUsers = usrList;
            this.DefaultConfig.SetListOfUsers(usrList);

            this.updateConfgPathFile();
        }

        #region Getters
        public DABRAS GetDABRAS()
        {
            return this.DABRAS;
        }
        //public List<QCCalResultNode> GetListOfQCResults()
        //{
        //    return this.DefaultConfig.GetListOfCalResults();
        //}
        public Logger GetLogger()
        {
            return this.Logger;
        }
        public List<User> GetListOfUsers()
        {
            return this.DefaultConfig.GetListOfUsers();
        }
        public List<RadionuclideFamily> GetListOfFamily()
        {
            return this.DefaultConfig.GetListOfFamily();
        }
        public List<Radioactive_Source> GetListOfSources()
        {
            return this.DefaultConfig.GetListOfSources();
        }
        public List<Radioactive_Source> GetNewSources()
        {
            return this.ListOfSources;
        }

        public DefaultConfigurations GetDefaultConfig()
        {
            return this.DefaultConfig;
        }

        public FormRSC GetRscForm()
        {
            return this.rscForm;
        }
        public FormQC GetQcForm()
        {
            return this.qcForm;
        }

        #endregion

        #region Finalization

        #endregion
    }
}