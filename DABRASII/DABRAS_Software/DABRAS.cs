using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Timers;


namespace DABRAS_Software
{
    [Serializable]
    public sealed class DABRAS
    {
        #region Data Members
        public string VCP_Instance;     //Holds the VCP string it connects to
        public string Firmware_Version; //Holds the firmware version of the chip
        public string Serial_Number;

        private SerialPort SP;
        private bool Connected;

        private bool PacketReady = false;
        private SerialPacket SerialPacket;
        private string RawData = null;

        
        private Thread ContinuousBackgroundMonitorThread = null;
        private const int DefaultBackgroundMonitorTime = 600;

        private DefaultConfigurations DC;

        private Thread DailyBackgroundMonitorThread = null;
        private const int DefaultDailyBackgroundMonitorTime = 14400;

        private Logger Logger;

        private QCListKeeper QC_List;

        private ContinuousBackgroundMonitor CBM = null;
        private DailyBackgroundMonitor DBM = null;
         
        
        /*Interrupted State Variables*/
        private bool InterruptedContinuousLogger = false;
        private bool InterruptedDailyLogger = false;

        private System.Timers.Timer Watchdog;

        #endregion

        #region Constructors
        /*Null constructor*/
        public DABRAS()
        {
            VCP_Instance = null;
            Firmware_Version = null;
            SP = null;
            Connected = false;
            this.Watchdog = new System.Timers.Timer(3000);
            this.Watchdog.Elapsed += new ElapsedEventHandler(Watchdog_Bark);
            return;
        }

        public DABRAS(DefaultConfigurations _DC, Logger _L)
        {
            VCP_Instance = _DC.GetDefaultVCP();
            this.DC = _DC;
            Firmware_Version = null;
            SP = null;
            Connected = false;
            this.Logger = _L;
            this.Watchdog = new System.Timers.Timer(3000);
            this.Watchdog.Elapsed += new ElapsedEventHandler(Watchdog_Bark);
            return;
        }

        void Watchdog_Bark(object sender, ElapsedEventArgs e)
        {
            /*Communication timed out.*/
            this.Connected = false;
        }

        public void KickWatchdog()
        {
            this.Watchdog.Stop();
            this.Watchdog.Start();
        }

        public void EnableWatchdog()
        {
            this.Watchdog.Start();
        }

        public void DisableWatchdog()
        {
            this.Watchdog.Stop();
        }

        #endregion
  
        #region Continuous Background Monitor Methods
      
        public void StartMonitoringBackgroundContinuously(int AlphaBGLo, int BetaBGLo, int AlphaBGHi, int BetaBGHi)
        {
            StartMonitoringBackgroundContinuously(AlphaBGLo, BetaBGLo, AlphaBGHi, BetaBGHi, DefaultBackgroundMonitorTime);
        }

        public void StartMonitoringBackgroundContinuously(int AlphaBGLo, int BetaBGLo, int AlphaBGHi, int BetaBGHi, int TimeInterval)
        {
            CBM = new ContinuousBackgroundMonitor(this, Logger, QC_List, AlphaBGHi, BetaBGHi, AlphaBGLo, BetaBGLo, TimeInterval);
            ContinuousBackgroundMonitorThread = new Thread(() => CBM.RunInBackground());
            ContinuousBackgroundMonitorThread.Start();
        }

        public void PauseContinuousBackgroundMonitorAsync()
        {
            if (ContinuousBackgroundMonitorThread != null)
            {
                this.InterruptedContinuousLogger = true;
                CBM.Interrupt();
                return;
            }

            this.InterruptedContinuousLogger = false;
            return;
        }

        public void PauseContinuousBackgroundMonitor()
        {
            if (ContinuousBackgroundMonitorThread != null)
            {
                CBM.Interrupt();
                this.InterruptedContinuousLogger = true;
                while (!CBM.Interrupted())
                {
                    Thread.Sleep(1);
                }
                return;
            }

            this.InterruptedContinuousLogger = false;
            return;
        }

        public bool ContinuousBackgroundMonitorAlive()
        {
            return (ContinuousBackgroundMonitorThread != null);
        }
        
        public bool ValidateContinuousAlphaBackground()
        {
            if (CBM != null)
            {
                return CBM.IsAlphaOK();
            }

            else
            {
                return true;
            }
        }

        public bool ValidateContinuousBetaBackground()
        {
            if (CBM != null)
            {
                return CBM.IsBetaOK();
            }

            else
            {
                return true;
            }
        }
        
        public DateTime GetValidationDate()
        {
            if (CBM != null)
            {
                return CBM.GetLastSafelyCompleted();
            }

            return DateTime.Parse("1900/01/01");
        }

        public void RestartContinuousBackgroundMonitor()
        {
            this.InterruptedContinuousLogger = false;
            if (ContinuousBackgroundMonitorThread != null && this.InterruptedContinuousLogger)
            {
                CBM.Resume();
            }

            return;
        }

        public void KillContinuousBackgroundMonitor()
        {
            if (ContinuousBackgroundMonitorThread != null)
            {
                CBM.Kill();
                ContinuousBackgroundMonitorThread.Join();
                ContinuousBackgroundMonitorThread = null;
                CBM = null;
            }

        }

        #endregion
        
        #region Daily Background Monitor Methods

        public void StartMonitoringBackgroundDaily(int AlphaBGLo, int BetaBGLo, int AlphaBGHi, int BetaBGHi, DateTime StartTime)
        {
            StartMonitoringBackgroundDaily(AlphaBGLo, BetaBGLo, AlphaBGHi, BetaBGHi, DefaultBackgroundMonitorTime, StartTime);
        }

        public void StartMonitoringBackgroundDaily(int AlphaBGLo, int BetaBGLo, int AlphaBGHi, int BetaBGHi, int TimeInterval, DateTime StartTime)
        {
            DBM = new DailyBackgroundMonitor(this, Logger, QC_List, AlphaBGHi, AlphaBGLo, BetaBGHi, BetaBGLo, DC.GetListOfSources().Find(x => x.GetSerialNumber() == "Background"), StartTime, TimeInterval);
            DailyBackgroundMonitorThread = new Thread(() => DBM.MonitorDaily());
            DailyBackgroundMonitorThread.Start();
        }

        public void PauseDailyBackgroundMonitorAsync()
        {
            if (DailyBackgroundMonitorThread != null)
            {
                DBM.Interrupt();
                this.InterruptedDailyLogger = true;
            }
        }

        public void PauseDailyBackgroundMonitor()
        {
            if (DailyBackgroundMonitorThread != null)
            {
                DBM.Interrupt();
                while (!DBM.Interrupted())
                {
                    Thread.Sleep(1);
                }
                this.InterruptedDailyLogger = true;
            }
        }

        public bool DailyBackgroundMonitorAlive()
        {
            return (DailyBackgroundMonitorThread != null);
        }

        public void RestartDailyBackgroundMonitor()
        {
            this.InterruptedDailyLogger = false;
            if (ContinuousBackgroundMonitorThread != null)
            {
                DBM.Resume();
            }

        }

        //public void KillDailyBackgroundMonitor()
        //{
        //    if (DailyBackgroundMonitorThread != null)
        //    {
        //        DBM.Kill();
        //        DailyBackgroundMonitorThread.Join();
        //        DailyBackgroundMonitorThread = null;
        //        DBM = null;
        //    }

        //}

        #endregion

        #region Misc Background Monitoring
        public void Cut()
        {
            if (this.CBM != null)
            {
                this.InterruptedContinuousLogger = true;
                this.CBM.Interrupt();
                while (!CBM.Interrupted())
                {
                    Thread.Sleep(1);
                }
            }

            if (this.DBM != null)
            {
                this.InterruptedDailyLogger = true;
                this.DBM.Interrupt();
                while (!CBM.Interrupted())
                {
                    Thread.Sleep(1);
                }
            }

            return;
        }

        public void ResumeBackgroundMonitors()
        {
            if (this.InterruptedContinuousLogger)
            {
                this.InterruptedContinuousLogger = false;
                this.CBM.Resume();
            }

            if (this.InterruptedDailyLogger)
            {
                this.InterruptedDailyLogger = false;
                this.DBM.Resume();
            }
        }

        public void KillAllMonitors()
        {
            if (this.CBM != null)
            {
                this.CBM.Kill();
                this.ContinuousBackgroundMonitorThread.Join();
                this.CBM = null;
                this.ContinuousBackgroundMonitorThread = null;
                this.InterruptedContinuousLogger = false;
            }

            if (this.DBM != null)
            {
                this.DBM.Kill();
                this.DailyBackgroundMonitorThread.Join();
                this.DBM = null;
                this.DailyBackgroundMonitorThread = null;
                this.InterruptedDailyLogger = false;
            }

            return;
        }
        #endregion

        #region VCP Methods
        /*Get_Coms
         * This will attempt to connect on the COM port with the parameters passed to the function.
         * It will return a bool relating to the result.
         * 
         * @author Mitchell Spryn
         * @version 0.1
         * @return Bool True if communication is established, False if not.
         */
        public bool Get_Coms(string PortToConnect) //Overload with default communication instance.
        {
            return Get_Coms(PortToConnect, 8, Parity.None, 115200, Handshake.None, StopBits.One);
        }
        
        public bool Get_Coms(string PortToConnect, int DataBits, Parity parity, int baudrate, Handshake handshake, StopBits stopbits)
        {
            if (this.Connected)
            {
                //MessageBox.Show(String.Format("Attempting to disconnect communication on port {0}", this.VCP_Instance));//disconnect the current VCP_Instance

                if (SP.IsOpen)
                {
                    SP.Write("h");

                    ClearRaw();
                    ClearSerialPacket();
                    //Throw exception if port isn't opened
                    string Handshake = SP.ReadLine();

                    //Throw exception if DABRAS is not authentic
                    //if (Handshake.Replace("\r", "") != "ARGONNE")//QZ: currently we only allow one com port connection, so this statement will always evaluate to true
                    {
                        this.VCP_Instance = "";
                        this.Connected = false;
                        //add the follow two lines to clear the serial port--QZ
                        SP.Close();
                        SP = null;

                        return false;
                    }

                    //If here, we are still connected. Don't touch anything!
                    //this.Connected = true;

                    //return true;
                }

                this.VCP_Instance = "";
                this.Connected = false;
                return false;
            }

            if (PortToConnect != "" && !this.Connected)//trying to establish a new connection
            {
                SP = new SerialPort();// Create a new SerialPort object with default settings.

                // Set the appropriate properties
                SP.PortName = PortToConnect;
                SP.DataBits = DataBits;
                SP.Parity = parity;
                SP.BaudRate = baudrate;
                SP.Handshake = handshake;
                SP.StopBits = stopbits;

                // Set the read/write timeouts
                SP.WriteTimeout = 2000;
                SP.ReadTimeout = 2000;
                SP.DataReceived += new SerialDataReceivedEventHandler(DataPacketReceived);

                try
                {
                    SP.Open();
                    SP.Write("h");

                    ClearRaw();
                    ClearSerialPacket();
                    //Throw exception if port isn't opened
                    string _Handshake = SP.ReadLine();

                    //Throw exception if DABRAS is not authentic
                    //TODO: This seems to be throwing errors...
                    //if (_Handshake.Replace("\r", "") != "ARGONNE")
                    //{
                    //    DivideByZeroException e = new DivideByZeroException(); //LOLOL
                    //    throw e;
                    //}

                    this.VCP_Instance = PortToConnect;
                    this.Connected = true;
                    //MessageBox.Show("Port Opened.");
                }
                catch
                {
                    MessageBox.Show("ERROR: Port did not open successfully.");
                    if (SP.IsOpen)
                    {
                        SP.Close();
                    }
                    SP = null;
                }

                if (SP != null)
                {
                    return true;
                }
            }

            return false;
        }

        /*Write_To_Serial_Port
         * This function will take in a string and write it to the serial port of the DABRAS.
         * It will automatically append the "0x0D,0x0A,0x00" sequence to the end.
         * 
         * 0x0D represents a carriage return(\t), 0x0A represents a linefeed (\n) 0x00 represents null
         * 
         * @author Mitchell Spryn
         * @version 0.1
         * @return Bool True if data was successfully sent, False if it failed.
         * @todo Add checks for valid data?
         */

        public bool Write_To_Serial_Port(string Data)
        {
            if (SP != null)
            {
                try
                {
                    Data = Data + "\r\n\0";
                    SP.Write(Data);
                    //for testing
                    //Thread.Sleep(100);
                    //MessageBox.Show(SP.ReadLine());
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        
        private void DataPacketReceived(object sender, SerialDataReceivedEventArgs Data)
        {
            if (SP != null)
            {
                string DataIn = "";
                try
                {
                //Read the Incoming Data
                DataIn = SP.ReadLine();

                ///Split it off into its separate parts
                //Declare holder variables
                int Status;
                int ElTime;
                int AlphaTot;
                int BetaTot;
                int TargetTime;

                //Split the string at the tabs
                string[] ParsedInputData;
                ParsedInputData = DataIn.Split('\t'); //Split at 0x09

                //Write each part to each holder variable             
                    ParsedInputData[0] = ParsedInputData[0].Replace("D", "");

                    //Chomp all whitespace
                    ParsedInputData[4] = ParsedInputData[4].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");

                    Status = Convert.ToInt32(ParsedInputData[0]);
                    ElTime = Convert.ToInt32(ParsedInputData[1]);
                    AlphaTot = Convert.ToInt32(ParsedInputData[2]);
                    BetaTot = Convert.ToInt32(ParsedInputData[3]);
                    TargetTime = Convert.ToInt32(ParsedInputData[4]);

                    this.SerialPacket = new SerialPacket(Status, ElTime, AlphaTot, BetaTot, TargetTime);
                    this.PacketReady = true;
                    this.RawData = DataIn;
                }
                catch
                {
                    this.PacketReady = true;
                    this.RawData = DataIn;
                    return;
                }
            }
        }
        #endregion

        #region Initialization
        public bool Initialize()
        {
            this.Write_To_Serial_Port("U");//enter the utility mode
            string Response = this.ReadRawSerialString().Replace("\r", "");
            while (String.Compare("OK", Response) != 0)//return "OK" if succeeded entering Utility mode, "ERR" otherwise
            {
                Thread.Sleep(1);
                Response = this.ReadRawSerialString().Replace("\r", "");
            }
            try
            {
                this.ClearRaw();
                this.ClearPacketFlag();
                this.Write_To_Serial_Port("F");//request firmware
                while (!this.PacketReady)
                {
                    Thread.Sleep(1);
                }
                this.Firmware_Version = this.ReadRawSerialString();
                this.Firmware_Version = this.Firmware_Version.TrimEnd('\r');

                this.ClearRaw();
                this.ClearPacketFlag();
                this.Write_To_Serial_Port("n");
                while (!this.PacketReady)
                {
                    Thread.Sleep(1);
                }
                this.Serial_Number = this.ReadRawSerialString().Replace("n", "").Replace("\r", "");
                this.Write_To_Serial_Port("u");//exit the utility mode
                return true;
            }
            catch
            {
                this.Write_To_Serial_Port("u");//exit the utility mode
                return false;
            }
        }
        #endregion

        #region HVC Functions
        public bool SetHVC(double VoltageToWrite)
        {
            VoltageToWrite *= 10;
            
            try
            {
                //Set HV Control
                if (!this.Write_To_Serial_Port("v"))//to set HV DAC control value, execute command "V" followed immediately by the voltage value appended with a carriage return
                {
                    return false;
                }

                Thread.Sleep(10);//not more than 100 ms before the following value is sent
                this.Write_To_Serial_Port(String.Format("{0}\r", Convert.ToString((VoltageToWrite))));

                this.ClearPacketFlag();
                this.ClearRaw();
                while (!this.IsDataReady())
                    Thread.Sleep(1);

                string Response = this.ReadRawSerialString().Replace("\r", "");
                if(String.Compare("OK", Response) == 0)//return "OK" if succeeded, "ERR" otherwise
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public double GetHVC()
        {
            string ReceivedHVC = "";
            double CurrentHVC = 0;
            /*working before
            this.Write_To_Serial_Port("U");//enter the utility mode
            while (!done)
            {
                string Response = this.ReadRawSerialString().Replace("\r", "");
                if (String.Compare("OK", Response) == 0)//return "OK" if succeeded entering Utility mode, "ERR" otherwise
                {
                    this.ClearRaw();
                    this.ClearPacketFlag();
                    this.Write_To_Serial_Port("s"); //to get HV DAC control setting, execute command "s" followed by a carriage return

                    while (!IsDataReady() && this.Connected)
                    {
                        Thread.Sleep(25);//changed from 250 ms by QZ
                    }

                    //Returns “S” followed by the ASCII decimal value of the raw value sent to the DAC and terminated with a carriage return and line feed
                    ReceivedHVC = this.ReadRawSerialString();
                    if (ReceivedHVC.Contains('S') || (!this.Connected))
                    {
                        ReceivedHVC = ReceivedHVC.Replace("S", "").Replace("\r", "");
                        done = true;
                    }
                }
            }
            this.Write_To_Serial_Port("u");//exit the utility mode
            */

            //The following first while loop is to ensure we enter the utility mode before reading the "S" (HVC) value
            /*
            this.Write_To_Serial_Port("U");//enter the utility mode
            string Response = this.ReadRawSerialString().Replace("\r", "");
            while (String.Compare("OK", Response) != 0)//return "OK" if succeeded entering Utility mode, "ERR" otherwise
            {
                this.Write_To_Serial_Port("u");//exit the utility mode
                this.Write_To_Serial_Port("R");//stop the acquisition
                this.ClearRaw();
                this.ClearPacketFlag();
                this.Write_To_Serial_Port("U");//enter the utility mode
                Response = this.ReadRawSerialString().Replace("\r", "");
            }
            */
            this.ClearRaw();
            this.ClearPacketFlag();
            this.Write_To_Serial_Port("s"); //to get HV DAC control setting, execute command "s" followed by a carriage return
            while (!IsDataReady() && this.Connected)
            {
                Thread.Sleep(25);//changed from 250 ms by QZ
            }

            //Returns “S” followed by the ASCII decimal value of the raw value sent to the DAC and terminated with a carriage return and line feed
            ReceivedHVC = this.ReadRawSerialString();
            while (this.Connected && !ReceivedHVC.Contains('S'))
            {
                Thread.Sleep(25);
                ReceivedHVC = this.ReadRawSerialString();
            }
            if (this.Connected)
            {
                ReceivedHVC = ReceivedHVC.Replace("S", "").Replace("\r", "");

                try
                {
                    CurrentHVC = Convert.ToDouble(ReceivedHVC);
                    return CurrentHVC;
                }
                catch
                {
                    MessageBox.Show(String.Format("Received bad string from DABRAS: {0}.\n", ReceivedHVC));
                    return -1;
                }
            }
            else
                return -1;
        }
        #endregion

        #region Getters
        public SerialPacket ReadSerialPacket()
        {
            this.PacketReady = false;
            return this.SerialPacket;
        }

        public string ReadRawSerialString()
        {
            this.PacketReady = false;
            return this.RawData;
        }

        #endregion

        #region Setters

        public void SetQCList(QCListKeeper _QL)
        {
            this.QC_List = _QL;
            return;
        }
        public void ClearPacketFlag()
        {
            this.PacketReady = false;
        }

        public bool IsDataReady()
        {
            return this.PacketReady;
        }

        public bool IsConnected()
        {
            return this.Connected;
        }

        public void ClearRaw()
        {
            this.RawData = "";
        }

        public void ClearSerialPacket()
        {
            this.SerialPacket = null;
        }

        public void ClearAll()//Clear any data left in the buffer
        {
            this.ClearSerialPacket();
            this.ClearPacketFlag();
            this.ClearRaw();
        }
        #endregion
    }

    public class SerialPacket
    {
        #region Data Members
        public int Status { get; set; }
        public int ElTime {get; set; }
        public int AlphaTot { get; set; }
        public int BetaTot { get; set; }
        public int TargetTime { get; set; }
        #endregion

        #region Constructor
        public SerialPacket(int _Stat, int _ElTime, int _AlphTot, int _BetTot, int _TarTime)
        {
            this.Status = _Stat;
            this.ElTime = _ElTime;
            this.AlphaTot = _AlphTot;
            this.BetaTot = _BetTot;
            this.TargetTime = _TarTime;
        }
        #endregion
    }
}
