using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DABRAS_Software
{
    public class ContinuousBackgroundMonitor
    {
        #region Data Members
        private DABRAS DABRAS;
        private Logger Logger;
        private QCListKeeper QC_List;

        private double AlphaBGHi;
        private double BetaBGHi;
        private double AlphaBGLo;
        private double BetaBGLo;


        private bool ShouldStop = false;
        private bool ShouldPause = false;
        private bool InInterruptedState = false;


        bool BadAlpha = false;
        bool BadBeta = false;

        private int SampleTime;

        private double AlphaAverage;
        private double BetaAverage;

        private DateTime LastCompleted = DateTime.Parse("1900/01/01");
        #endregion

        #region Constructor
        public ContinuousBackgroundMonitor(DABRAS _D, Logger _L, QCListKeeper _QL, double _ABG_HI, double _BBG_HI, double _ABG_LO, double _BBG_LO, int _ST)
        {
            this.DABRAS = _D;
            this.Logger = _L;
            this.QC_List = _QL;
            this.AlphaBGHi = _ABG_HI;
            this.BetaBGHi = _BBG_HI;
            this.AlphaBGLo = _ABG_LO;
            this.BetaBGLo = _BBG_LO;
            this.SampleTime = _ST;
        }
        #endregion

        #region Publicly avaliable methods
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

        public bool Interrupted()
        {
            return (this.ShouldStop && this.InInterruptedState); 
        }

        public bool IsAlphaOK()
        {
            return (!BadAlpha);
        }

        public bool IsBetaOK()
        {
            return (!BadBeta);
        }

        public DateTime GetLastSafelyCompleted()
        {
            return this.LastCompleted;
        }
       
        public void RunInBackground()
        {

            while (!ShouldStop)
            {
                RunTest();

                /*This looks screwy because of the thread sync going on*/
                bool ResultsWritten = false;
                while (!ShouldStop && !ResultsWritten)
                {
                    ResultsWritten = QC_List.Add(new QCCalResultNode(DateTime.Now, FormQC.TypeOfQC.Background, this.AlphaAverage, this.BetaAverage, -1, true, "Derived from Continuous Background Monitor.", "CBM"));
                }

                if (ShouldPause)
                {
                    /*For test*/
                    //MessageBox.Show("Interrupted!");

                    InInterruptedState = true;
                    while (ShouldPause)
                    {
                        Thread.Sleep(1000);
                    }

                    InInterruptedState = false;
                    ShouldStop = false;

                    
                }
            }
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
            NumberOfMinutes += ( (SampleTime % 60) == 0 ? 0 : 1); //Add one minute for odd time intervals.
            int NumberOfMinutesSampled = 0;
            List<int>NumberOfAlphaCPM = new List<int>();
            List<int>NumberOfBetaCPM = new List<int>();


            
            /*Do not increment the row index until the current sample time has elapsed*/
            while ((!ShouldStop) && (NumberOfMinutesSampled < NumberOfMinutes))
            {

                bool MinuteComplete = false;
                /*Clear any data left in the buffer*/
                DABRAS.ClearSerialPacket();
                DABRAS.ClearPacketFlag();

                DABRAS.Write_To_Serial_Port("g");

                /*Check for the first good packet*/
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

                while (!ShouldStop && !MinuteComplete)
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
                            MinuteComplete = true;
                            DABRAS.Write_To_Serial_Port("g");
                        }
                    }
                }

            }

            if (!ShouldStop)
            {
                DateTime BackgroundFinished = DateTime.Now;

                this.AlphaAverage = 0;
                this.BetaAverage = 0;

                for (int i = 0; i < NumberOfAlphaCPM.Count; i++)
                {
                    AlphaAverage += NumberOfAlphaCPM.ElementAt(i);
                    BetaAverage += NumberOfBetaCPM.ElementAt(i);
                }

                AlphaAverage /= NumberOfAlphaCPM.Count;
                BetaAverage /= NumberOfBetaCPM.Count;

                /*Check for high background levels*/
                BadAlpha = ((AlphaAverage > AlphaBGLo) && (AlphaAverage < AlphaBGHi));
                BadBeta = ((BetaAverage > BetaBGLo) && (BetaAverage < BetaBGHi));

                if (BadAlpha && BadBeta)
                {
                    Logger.WriteLineToStatusLog(String.Format("Automatic background check failed at {0} with Alpha levels of {1} (expected range of {2} to {3}) and Beta levels of {4} (expected range of {5} to {6})", DateTime.Now, AlphaAverage, AlphaBGLo, AlphaBGHi, BetaAverage, BetaBGLo, BetaBGHi));
                }

                else if (BadAlpha && !BadBeta)
                {
                    Logger.WriteLineToStatusLog(String.Format("Automatic background check failed at {0} with Alpha levels of {1} (expected range of {2} to {3}) and Beta levels of {4} (expected range of {5} to {6})", DateTime.Now, AlphaAverage, AlphaBGLo, AlphaBGHi, BetaAverage, BetaBGLo, BetaBGHi));
                }

                else if (!BadAlpha && BadBeta)
                {
                    Logger.WriteLineToStatusLog(String.Format("Automatic background check failed at {0} with Alpha levels of {1} (expected range of {2} to {3}) and Beta levels of {4} (expected range of {5} to {6})", DateTime.Now, AlphaAverage, AlphaBGLo, AlphaBGHi, BetaAverage, BetaBGLo, BetaBGHi));
                }

                else
                {
                    Logger.WriteLineToStatusLog(String.Format("Automatic background check passed at {0} with Alpha levels of {1} (expected range of {2} to {3}) and Beta levels of {4} (expected range of {5} to {6})", DateTime.Now, AlphaAverage, AlphaBGLo, AlphaBGHi, BetaAverage, BetaBGLo, BetaBGHi));
                }

                this.LastCompleted = DateTime.Now;

            }

            return;
        }
        #endregion
    }
}
