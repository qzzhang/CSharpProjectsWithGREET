using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DABRAS_Software
{
    class DailyBackgroundMonitor
    {
        #region Data Members
        private DateTime StartTime;
        private int SampleTime;

        private DABRAS DABRAS;
        private Logger Logger;

        private bool ShouldStop = false;
        private bool ShouldPause = false;

        private Radioactive_Source BGSource;

        private int AlphaHi;
        private int AlphaLo;
        private int BetaHi;
        private int BetaLo;

        private int AlphaBG = 0;
        private int BetaBG = 0;

        private QCListKeeper QC_List;

        private int SleepValue = 5;
        #endregion

        #region Constructor
        public DailyBackgroundMonitor(DABRAS _D, Logger _L, QCListKeeper _QL, int _AGB_HI, int _ABG_LO, int _BBG_HI, int _BBG_LO, Radioactive_Source _BGSRC, DateTime _STARTTIME, int _SAMPLETIME)
        {
            this.DABRAS = _D;
            this.Logger = _L;
            this.QC_List = _QL;
            this.AlphaHi = _AGB_HI;
            this.AlphaLo = _ABG_LO;
            this.BetaHi = _BBG_HI;
            this.BetaLo = _BBG_LO;
            this.StartTime = _STARTTIME;
            this.SampleTime = _SAMPLETIME;
            this.BGSource = _BGSRC;
        }
        #endregion

        #region Control Functions
        public void Interrupt()
        {
            this.ShouldPause = true;
            this.ShouldStop = true;
        }

        public void Kill()
        {
            this.ShouldPause = false;
            this.ShouldStop = true;
        }

        public void Resume()
        {
            this.ShouldPause = false;
            this.ShouldStop = false;
        }

        #endregion

        #region Getters
        public bool Interrupted()
        {
            return (this.ShouldPause && this.ShouldStop);
        }

        public int GetAlphaBG()
        {
            return this.AlphaBG;
        }

        public int GetBetaBG()
        {
            return this.BetaBG;
        }
        #endregion

        #region Public Function Looper
        public void MonitorDaily()
        {
            while (!ShouldStop)
            {
                int DifferenceInSeconds = Convert.ToInt32(DateTime.Now.Subtract(StartTime).Seconds);

                if ((DifferenceInSeconds > 0) && (DifferenceInSeconds < (1050 * SleepValue))) //not 1000 due to execution times
                {
                    DABRAS.PauseContinuousBackgroundMonitor();

                    Logger.WriteLineToStatusLog(String.Format("Starting Auto-background configuration at {0}", DateTime.Now));
                    StartTime.AddDays(1);

                    RunTest();

                    bool PointAdded = false;
                    while (!ShouldStop && !PointAdded)
                    {
                        PointAdded = QC_List.Add(new QCCalResultNode(DateTime.Now, FormQC.TypeOfQC.Background, BGSource.GetDailyAlphaCPM(), BGSource.GetDailyBetaCPM(), -1, true, "Derived from the Automatic Daily Background Check.", "DBM"));
                    }

                    DABRAS.RestartContinuousBackgroundMonitor();
                    
                }

                else
                {
                    Thread.Sleep(SleepValue * 1000);
                }

                if (ShouldPause)
                {
                    while (ShouldPause)
                    {
                        Thread.Sleep(SleepValue * 1000);
                    }

                    ShouldStop = false;
                }
            }

            return;
        }
        #endregion

        #region Private background thread
        private void RunTest()
        {
            /*Set aquisition time*/
            DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(60)); //Divide up into 1-minute samples to avoid buffer overflow

            int NumberOfMinutes = SampleTime / 60;
            NumberOfMinutes += ((SampleTime % 60) == 0 ? 0 : 1); //Add one minute for odd time intervals.
            int NumberOfMinutesSampled = 0;
            List<int> NumberOfAlphaCPM = new List<int>();
            List<int> NumberOfBetaCPM = new List<int>();

            /*Clear any data left in the buffer*/
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();

            DABRAS.Write_To_Serial_Port("g");

            /*Do not increment the row index until the current sample time has elapsed*/
            while ((!ShouldStop) && (NumberOfMinutesSampled < NumberOfMinutes))
            {

                bool RowComplete = false;
                /*Check for first good incoming packet*/
                while (!ShouldStop)
                {
                    while (!DABRAS.IsDataReady() && !ShouldStop)
                    {
                        Thread.Sleep(100);
                    }

                    if (!ShouldStop)
                    {
                        SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                        if ((IncomingData.ElTime <= 2) && IncomingData.TargetTime == SampleTime)
                        {
                            break;
                        }
                    }
                }

                while (!ShouldStop && !RowComplete)
                {
                    /*Wait for incoming data packet*/
                    while (!DABRAS.IsDataReady() && !ShouldStop)
                    {
                        Thread.Sleep(100);
                    }

                    if (!ShouldStop)
                    {
                        SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                        /*Read in time*/
                        int TimeElapsed = IncomingData.ElTime + 1;

                        /*If the sample time has elapsed, increment the row.*/
                        if (IncomingData.ElTime >= 60)
                        {
                            NumberOfAlphaCPM.Add(Convert.ToInt32(IncomingData.AlphaTot));
                            NumberOfBetaCPM.Add(Convert.ToInt32(IncomingData.BetaTot));
                            NumberOfMinutesSampled++;
                            RowComplete = true;
                            DABRAS.Write_To_Serial_Port("g");
                        }
                    }
                }

            }

            if (!ShouldStop)
            {
                DateTime BackgroundFinished = DateTime.Now;

                double AlphaAverage = 0;
                double BetaAverage = 0;

                for (int i = 0; i < NumberOfAlphaCPM.Count; i++)
                {
                    AlphaAverage += NumberOfAlphaCPM.ElementAt(i);
                    BetaAverage += NumberOfBetaCPM.ElementAt(i);
                }

                AlphaAverage /= NumberOfAlphaCPM.Count;
                BetaAverage /= NumberOfBetaCPM.Count;

                if ((AlphaAverage < AlphaHi) && (AlphaAverage > AlphaLo) && (BetaAverage < BetaHi) && (BetaAverage > BetaLo))
                {
                    /*Good results. Log*/
                    BGSource.SetDailyAlphaCPM(Convert.ToInt32(AlphaAverage));
                    BGSource.SetDailyBetaCPM(Convert.ToInt32(BetaAverage));

                    BGSource.SetDailyCalibratedDate(DateTime.Now);

                    this.AlphaBG = Convert.ToInt32(AlphaAverage);
                    this.BetaBG = Convert.ToInt32(BetaAverage);

                    Logger.WriteLineToStatusLog(String.Format("Auto Background Calibration Passed.Finished at {0}. Alpha CPM = {1}, Beta CPM = {2}", DateTime.Now, AlphaAverage, BetaAverage));
                }


            }

            return;
        }
        #endregion
    }
}
