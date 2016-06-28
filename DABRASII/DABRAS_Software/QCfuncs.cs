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
    public partial class FormQC
    {
        #region Background Callback
        public void InvokeBGCallback()
        {
            this.Invoke(new BG_GUI_Callback(this.BG_BackgroundThreadFinished));
        }

        private void BG_BackgroundThreadFinished()
        {
            if (!this.DABRAS.IsConnected())
            {
                this.frmParent.refreshConnectStatus();
                this.endFormActivities();
                return;
            }

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

                //Write daily limits to background source
                Radioactive_Source R = ListOfSources.Find(x => x.GetSerialNumber() == "Background");

                R.SetDailyAlphaCPM(Convert.ToInt32(QCBG.GetAlphaGCPM()));
                R.SetDailyBetaCPM(Convert.ToInt32(QCBG.GetBetaGCPM()));
                R.SetDailyCalibratedDate(DateTime.Now);
                R.SetDailyCalibratedTimespan(QCBG.GetSampleTime() * QCBG.GetNumSamples());

                //Add QC object to List
                QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Background, 0,0,QCBG.GetBadgeNo(), true, Comment, QCBG.GetName(), QCBG.GetSampleTime());
                QC_List.Add(NewResult);
                
                R.SetDailyCalibratedTimespan(QCBG.GetSampleTime() * QCBG.GetNumSamples());

                DCModified = true;
                SourcesModified = true;

                this.setCheckButtons();

                this.bgPassed = true;
                this.frmParent.EnableRSCForm(true);
            }
            else if ((!(QCBG.WasTestPassed())) && QCBG.WasTestCompleted())
            {
                if (MessageBox.Show("Calibration test failed. Add a comment? Note that to declare a point unplottable, you must add a comment.", "Test failed", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //Write daily limits to background source
                    Radioactive_Source R = ListOfSources.Find(x => x.GetSerialNumber() == "Background");

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

                    //Add QC object to List
                    QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Background, 0, 0, QCBG.GetBadgeNo(), Plottable, Comment, QCBG.GetName(), QCBG.GetSampleTime());
                    QC_List.Add(NewResult);
                    R.SetDailyCalibratedTimespan(QCBG.GetSampleTime() * QCBG.GetNumSamples());

                    DCModified = true;
                    SourcesModified = true;

                }

                this.setCheckButtons();//later should be removed when stricter checking on pass is reinforced

                this.bgPassed = false;
                this.frmParent.EnableRSCForm(false);
            }

            this.OperationThread_QCBG = null;

            //Write results to a file
            this.autoWriteBG2Files();

            SetGUI(false);

            QCBG.RequestStop();
            QCBG = null;
        }

        public bool QCbackgroundPassed()
        {
            return this.bgPassed;
        }
        #endregion

        #region Alpha or Beta Callback

        public void InvokeABCallback()
        {
            this.Invoke(new AB_GUI_Callback(this.AB_BackgroundThreadFinished));
        }

        private void AB_BackgroundThreadFinished()
        {
            if (!this.DABRAS.IsConnected())
            {
                this.frmParent.refreshConnectStatus();
                this.endFormActivities();
                return;
            }

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

                //Write daily limits to calibrated source
                if (this.Type == TypeOfQC.Alpha)
                {
                    RadionuclideFamily R = ListOfFamily.Find(x => x.GetName() == "Am-241");

                    R.SetDailyAlphaCPM(Convert.ToInt32(QCAB.GetAlphaNCPM()));
                    R.SetDailyBetaCPM(Convert.ToInt32(QCAB.GetBetaNCPM()));
                    R.SetDailyCalibratedDate(DateTime.Now);
                    R.SetDailyCalibratedTimespan(QCAB.GetNumSamples() * QCAB.GetSampleTime());

                    //Add QC object to List
                    QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Alpha, QCAB.GetAlphaNCPM(), QCAB.GetBetaNCPM(), QCAB.GetBadgeNo(), true, Comment, QCAB.GetName(), QCAB.GetSampleTime());
                    QC_List.Add(NewResult);

                    R.SetDailyCalibratedTimespan(QCAB.GetSampleTime() * QCAB.GetNumSamples());

                    DCModified = true;
                    SourcesModified = true;
                }
                else
                {
                    RadionuclideFamily R = ListOfFamily.Find(x => x.GetName() == "Sr-90");

                    R.SetDailyAlphaCPM(Convert.ToInt32(QCAB.GetAlphaNCPM()));
                    R.SetDailyBetaCPM(Convert.ToInt32(QCAB.GetBetaNCPM()));
                    R.SetDailyCalibratedDate(DateTime.Now);

                    //Add QC object to List
                    QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Beta, QCAB.GetAlphaNCPM(), QCAB.GetBetaNCPM(), QCAB.GetBadgeNo(), true, Comment, QCAB.GetName(), QCAB.GetSampleTime());
                    QC_List.Add(NewResult);

                    DCModified = true;
                    SourcesModified = true;
                }

                this.setCheckButtons();

                this.abPassed = true;
                this.frmParent.EnableRSCForm(true);
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
                //Write daily limits to calibrated source
                if (this.Type == TypeOfQC.Alpha)
                {
                    RadionuclideFamily R = ListOfFamily.Find(x => x.GetName() == "Am-241");

                    R.SetDailyAlphaCPM(Convert.ToInt32(QCAB.GetAlphaNCPM()));
                    R.SetDailyBetaCPM(Convert.ToInt32(QCAB.GetBetaNCPM()));
                    R.SetDailyCalibratedDate(DateTime.Now);

                    //Add QC object to List
                    QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Alpha, QCAB.GetAlphaNCPM(), QCAB.GetBetaNCPM(), QCAB.GetBadgeNo(), Plottable, Comment, QCAB.GetName());
                    QC_List.Add(NewResult);

                    R.SetDailyCalibratedTimespan(QCAB.GetSampleTime() * QCAB.GetNumSamples());

                    DCModified = true;
                    SourcesModified = true;
                }
                else
                {
                    RadionuclideFamily R = ListOfFamily.Find(x => x.GetName() == "Sr-90");

                    R.SetDailyAlphaCPM(Convert.ToInt32(QCAB.GetAlphaNCPM()));
                    R.SetDailyBetaCPM(Convert.ToInt32(QCAB.GetBetaNCPM()));
                    R.SetDailyCalibratedDate(DateTime.Now);

                    //Add QC object to List
                    QCCalResultNode NewResult = new QCCalResultNode(DateTime.Now, TypeOfQC.Beta, QCAB.GetAlphaNCPM(), QCAB.GetBetaNCPM(), QCAB.GetBadgeNo(), Plottable, Comment, QCAB.GetName());
                    QC_List.Add(NewResult);

                    DCModified = true;
                    SourcesModified = true;
                }
                this.setCheckButtons();//for testing only, later should be removed when stricter checking on pass is reinforce

                this.abPassed = false;
                this.frmParent.EnableRSCForm(false);
            }
            this.OperationThread_QCAB = null;

            SetGUI(false);

            //Write results to a file
            this.autoWriteAB2Files();

            QCAB.RequestStop(); //to be safe
            QCAB = null;
        }
        public bool QCAlphaBetaPassed()
        {
            return this.abPassed;
        }
        #endregion

        #region Private Utility Functions
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
                    DG.Rows.Insert(0, "", "", "", "", "", "", "", "", "");//QZ added one more column for a button column
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

        private void autoWriteBG2Files()
        {
            //Write to two CSV files
            string BkgdDir = String.Format("{0}\\data\\QC\\Bkgd", Environment.CurrentDirectory);
            string BkgdMasterDir = String.Format("{0}\\data\\QC\\Bkgd\\Master", Environment.CurrentDirectory);
            string BkgdFilePath = String.Format("{0}\\data\\QC\\Bkgd\\{1}_{2}_{3}_{4}_{5}_{6}_BkgdQC.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, QCBG.GetBadgeNo(), QCBG.GetName(), DABRAS.Serial_Number);
            string BkgdMasterPath = String.Format("{0}\\data\\QC\\Bkgd\\Master\\BkgdMaster.csv", Environment.CurrentDirectory);
            string[,] DataToWrite = this.frmParent.MakeDataWritable(ShortDataGridView);
            try
            {
                if (!Directory.Exists(BkgdDir))
                {
                    Directory.CreateDirectory(BkgdDir);
                }
                if (!Directory.Exists(BkgdMasterDir))
                {
                    Directory.CreateDirectory(BkgdMasterDir);
                }

                if (!File.Exists(BkgdFilePath))
                {
                    File.Create(BkgdFilePath).Dispose();
                }
                using (FileStream F = new FileStream(BkgdFilePath, FileMode.Create))
                {
                    this.frmParent.GetLogger().WriteCSV(F, DataToWrite);
                }

                if (!File.Exists(BkgdMasterPath))
                {
                    File.Create(BkgdMasterPath).Dispose();
                }
                using (FileStream F = new FileStream(BkgdMasterPath, FileMode.Append))
                {
                    this.frmParent.GetLogger().WriteCSV(F, DataToWrite);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show("Automatic write background data failed--" + e.Message);
            }
        }

        private void autoWriteAB2Files()
        {
            string abFilePath, abMasterPath;

            string AlphaDir = String.Format("{0}\\data\\QC\\Alpha", Environment.CurrentDirectory);
            string AlphaMasterDir = String.Format("{0}\\data\\QC\\Alpha\\Master", Environment.CurrentDirectory);
            string BetaDir = String.Format("{0}\\data\\QC\\Beta", Environment.CurrentDirectory);
            string BetaMasterDir = String.Format("{0}\\data\\QC\\Beta\\Master", Environment.CurrentDirectory);
            string[,] DataToWrite = this.frmParent.MakeDataWritable(ShortDataGridView);

            if (QCAB.AlphaTest())
            {
                abFilePath = String.Format("{0}\\data\\QC\\Alpha\\{1}_{2}_{3}_{4}_{5}_{6}_AlphaQC.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, QCAB.GetBadgeNo(), QCAB.GetName(), DABRAS.Serial_Number);
                abMasterPath = String.Format("{0}\\data\\QC\\Alpha\\Master\\alphaMaster.csv", Environment.CurrentDirectory);
            }
            else
            {
                abFilePath = String.Format("{0}\\data\\QC\\Beta\\{1}_{2}_{3}_{4}_{5}_{6}_BetaQC.csv", Environment.CurrentDirectory, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, QCAB.GetBadgeNo(), QCAB.GetName(), DABRAS.Serial_Number);
                abMasterPath = String.Format("{0}\\data\\QC\\Beta\\Master\\betaMaster.csv", Environment.CurrentDirectory);
            }

            try
            {
                if (!Directory.Exists(AlphaDir))
                {
                    Directory.CreateDirectory(AlphaDir);
                }
                if (!Directory.Exists(AlphaMasterDir))
                {
                    Directory.CreateDirectory(AlphaMasterDir);
                }
                if (!Directory.Exists(BetaDir))
                {
                    Directory.CreateDirectory(BetaDir);
                }
                if (!Directory.Exists(BetaMasterDir))
                {
                    Directory.CreateDirectory(BetaMasterDir);
                }

                if (!File.Exists(abFilePath))
                {
                    File.Create(abFilePath).Dispose();
                }
                using (FileStream F = new FileStream(abFilePath, FileMode.Create))
                {
                    this.frmParent.GetLogger().WriteCSV(F, DataToWrite);
                }

                if (!File.Exists(abMasterPath))
                {
                    File.Create(abMasterPath).Dispose();
                }
                using (FileStream F = new FileStream(abMasterPath, FileMode.Append))
                {
                    this.frmParent.GetLogger().WriteCSV(F, DataToWrite);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Automatic write alpha/beta data failed--" + e.Message);
            }
        }
        #endregion
    }

    #region listeners
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
        //private double AlphaNCPM;
        //private double BetaNCPM;

        private bool Running = false;

        private int BadgeNo;
        private string Name;

        public event EventHandler BackgroundThreadFinished;
        public mainFramework frmCaller;
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
        public void setCallerForm(mainFramework clr)
        {
            this.frmCaller = clr;
        }
        public void RequestStop()
        {
            ShouldStop = true;
        }
        private string BuildToolTipText(double avga, double avgb,bool lessAlphaLo, bool greaterAlphaHi, bool lessBetaLo, bool greaterBetaHi)
        {
	        StringBuilder resnBuilder = new StringBuilder();

            if (lessAlphaLo)
            {
                resnBuilder.AppendFormat("Average Alpha background = {0} is less than the alpha lower limit {1}", StaticMethods.RoundToSigFigs(avga), AlphaLo);
                resnBuilder.Append(Environment.NewLine);
            }
            if (greaterAlphaHi)
            {
                resnBuilder.AppendFormat("Average Alpha background = {0} is higher than the alpha higher limit {1}", StaticMethods.RoundToSigFigs(avga), AlphaHi);
                resnBuilder.Append(Environment.NewLine);
            }
            if (lessBetaLo)
            {
                resnBuilder.AppendFormat("Average Beta background = {0} is less than the beta lower limit {1}", StaticMethods.RoundToSigFigs(avgb), BetaLo);
                resnBuilder.Append(Environment.NewLine);
            }
            if (greaterBetaHi)
            {
                resnBuilder.AppendFormat("Average Beta background = {0} is higher than the beta higher limit {1}", StaticMethods.RoundToSigFigs(avgb), BetaHi);
                resnBuilder.Append(Environment.NewLine);
            }
            return resnBuilder.ToString();
}
        private void Wait4IncomingData()
        {
            //Wait for incoming data packet
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
                MessageBox.Show("Error: Connection lost by QC.");
                //this.frmCaller.refreshConnectStatus();
                DABRAS.DisableWatchdog();
                BackgroundThreadFinished(this, null);
                return;
            }
        }
        private bool ComputeAvgsAndStdDevs()
        {
            //Compute averages
            double AverageAlphaGCPM = 0;
            double AverageBetaGCPM = 0;
            //double AverageAlphaNCPM = 0;
            //double AverageBetaNCPM = 0;

            for (int i = 0; i < NumSamples; i++)
            {
                AverageAlphaGCPM += Convert.ToDouble(QC_Table[2, i].Value);  
                //AverageAlphaNCPM += Convert.ToDouble(QC_Table[3, i].Value);
                AverageBetaGCPM += Convert.ToDouble(QC_Table[3, i].Value);
                //AverageBetaNCPM += Convert.ToDouble(QC_Table[5, i].Value);
            }

            AverageAlphaGCPM /= Convert.ToDouble(NumSamples);
            AverageBetaGCPM /= Convert.ToDouble( NumSamples);
            //AverageAlphaNCPM /= Convert.ToDouble( NumSamples);
            //AverageBetaNCPM /= Convert.ToDouble(NumSamples);

            DataGridViewCell AverageAlphaGCPMCell = QC_Table[2, NumSamples];
            //DataGridViewCell AverageAlphaNCell = QC_Table[3, NumSamples];
            DataGridViewCell AverageBetaGCPMCell = QC_Table[3, NumSamples];
            //DataGridViewCell AverageBetaNCell = QC_Table[5, NumSamples];
            DataGridViewCell FinalPassFail = QC_Table[4, NumSamples];

            DataGridViewCell StdDevAlphaGCell = QC_Table[2, NumSamples + 1];
            //DataGridViewCell StdDevAlphaNCell = QC_Table[3, NumSamples + 1];
            DataGridViewCell StdDevBetaGCell = QC_Table[3, NumSamples + 1];
            //DataGridViewCell StdDevBetaNCell = QC_Table[5, NumSamples + 1];

            AverageAlphaGCPMCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaGCPM);
            //AverageAlphaNCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaNCPM);
            AverageBetaGCPMCell.Value = StaticMethods.RoundToSigFigs(AverageBetaGCPM);
            //AverageBetaNCell.Value = StaticMethods.RoundToSigFigs(AverageBetaNCPM);

            this.AlphaGCPM = AverageAlphaGCPM;
            //this.AlphaNCPM = AverageAlphaNCPM;
            this.BetaGCPM = AverageBetaGCPM;
            //this.BetaNCPM = AverageBetaNCPM;

            //Compute Standard Deviations
            double StdDevGAlpha = 0;
            double StdDevNAlpha = 0;
            double StdDevGBeta = 0;
            double StdDevNBeta = 0;

            for (int i = 0; i < NumSamples; i++)
            {
                StdDevGAlpha += (AverageAlphaGCPM - Convert.ToDouble(QC_Table[2, i].Value)) * (AverageAlphaGCPM - Convert.ToDouble(QC_Table[2, i].Value));
                //StdDevNAlpha += (AverageAlphaNCPM - Convert.ToDouble(QC_Table[3, i].Value)) * (AverageAlphaNCPM - Convert.ToDouble(QC_Table[3, i].Value));
                StdDevGBeta += (AverageBetaGCPM - Convert.ToDouble(QC_Table[3, i].Value)) * (AverageBetaGCPM - Convert.ToDouble(QC_Table[3, i].Value));
                //StdDevNBeta += (AverageBetaNCPM - Convert.ToDouble(QC_Table[5, i].Value)) * (AverageBetaNCPM - Convert.ToDouble(QC_Table[5, i].Value));
            }

            if (NumSamples > 1)
            {
                StdDevGAlpha /= Convert.ToDouble((NumSamples - 1));
                //StdDevNAlpha /= Convert.ToDouble((NumSamples - 1));
                StdDevGBeta /= Convert.ToDouble((NumSamples - 1));
                //StdDevNBeta /= Convert.ToDouble((NumSamples - 1));

                StdDevGAlpha = Math.Sqrt(StdDevGAlpha);
                //StdDevNAlpha = Math.Sqrt(StdDevNAlpha);
                StdDevGBeta = Math.Sqrt(StdDevGBeta);
                //StdDevNBeta = Math.Sqrt(StdDevNBeta);
            }
            else
            {
                StdDevGAlpha = Math.Sqrt(StdDevGAlpha);
                StdDevNAlpha = Math.Sqrt(StdDevNAlpha);
                StdDevGBeta = Math.Sqrt(StdDevGBeta);
                StdDevNBeta = Math.Sqrt(StdDevNBeta);
            }

            StdDevAlphaGCell.Value = StaticMethods.RoundToSigFigs(StdDevGAlpha);
            //StdDevAlphaNCell.Value = StaticMethods.RoundToSigFigs(StdDevNAlpha);
            StdDevBetaGCell.Value = StaticMethods.RoundToSigFigs(StdDevGBeta);
            //StdDevBetaNCell.Value = StaticMethods.RoundToSigFigs(StdDevNBeta);

            //Display overall pass/fail--Test fails if any one of the tests fails.
            bool lessAlphaLo = !(this.AlphaGCPM >= AlphaLo);
            bool greaterAlphaHi = !(this.AlphaGCPM <= AlphaHi);
            bool lessBetaLo = !(this.BetaGCPM >= BetaLo);
            bool greaterBetaHi = !(this.BetaGCPM <= BetaHi);
            if (!lessAlphaLo && !greaterAlphaHi && !lessBetaLo && !greaterBetaHi)
            {
                FinalPassFail.Value = "PASS";
                FinalPassFail.Style = new DataGridViewCellStyle { ForeColor = Color.Green, Font = new Font(this.QC_Table.Font, FontStyle.Bold) };
                return true;
            }
            else
            {
                FinalPassFail.Value = "FAIL (?)";
                //FinalPassFail.ToolTipText = "Test fails if any one of the tests fails.";
                FinalPassFail.ToolTipText = BuildToolTipText(this.AlphaGCPM, this.BetaGCPM, lessAlphaLo, greaterAlphaHi, lessBetaLo, greaterBetaHi);
                FinalPassFail.Style = new DataGridViewCellStyle { ForeColor = Color.Red, Font = new Font(this.QC_Table.Font, FontStyle.Bold) };
                return false;
            }
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
        /*
        public double GetAlphaNCPM()
        {
            return this.AlphaNCPM;
        }

        public double GetBetaNCPM()
        {
            return this.BetaNCPM;
        }
        */
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
            //Stop all background threads
            this.DABRAS.Cut();

            //Set aquisition time
            this.DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            this.DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));

            //Clear any data left in the buffer
            this.DABRAS.ClearSerialPacket();
            this.DABRAS.ClearPacketFlag();
            this.DABRAS.EnableWatchdog();

            //Set test passed flag
            this.TestPassed = true;

            for (int i = 0; i < NumSamples; i++)
            {
                bool RowComplete = false;
                this.DABRAS.Write_To_Serial_Port("g");
                Thread.Sleep(150);
                this.DABRAS.ClearSerialPacket();
                this.DABRAS.ClearPacketFlag();

                //Check for the first good packet
                while (!ShouldStop)
                {
                    Wait4IncomingData();

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
                }//first good packet of data

                //Do not increment the row index until the current sample time has elapsed
                while (!RowComplete && !ShouldStop)
                {
                    Wait4IncomingData();

                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                    //Grab handles to form
                    DataGridViewCell TimeElapsed = this.QC_Table[1, i];
                    DataGridViewCell AlphaGCPM = this.QC_Table[2, i];
                    //DataGridViewCell AlphaNCPM = this.QC_Table[3, i];
                    DataGridViewCell BetaGCPM = this.QC_Table[3, i];
                    //DataGridViewCell BetaNCPM = this.QC_Table[5, i];
                    DataGridViewCell PassFail = this.QC_Table[4, i];

                    //Parse data to form
                    TimeElapsed.Value = IncomingData.ElTime;

                    if (IncomingData != null && IncomingData.ElTime != 0)
                    {
                        AlphaGCPM.Value = StaticMethods.RoundToDecimal((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)), 1); //Convert raw counts into CPM. +1 to avoid DBZ, PIC uses zero based counting.
                        //AlphaNCPM.Value = StaticMethods.RoundToDecimal((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)), 1); //Should prolly be N/A
                        BetaGCPM.Value = StaticMethods.RoundToDecimal((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)), 1);
                        //BetaNCPM.Value = StaticMethods.RoundToDecimal((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)), 1); //Should prolly be N/A

                        //Re-draw table
                        QC_Table.Invalidate();

                        //If the sample time has elapsed, increment the row //and compute Pass/Fail.
                        if (IncomingData.ElTime >= SampleTime)
                        {
                            RowComplete = true;
                        }
                    }
                    this.DABRAS.KickWatchdog();
                }
            }//finished all samples

            this.DABRAS.DisableWatchdog();

            if (!ShouldStop)
            {
                this.TestPassed = ComputeAvgsAndStdDevs();

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

            //Resume background threads, if they existed
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

        public mainFramework frmCaller;
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
        public void setCallerForm(mainFramework clr)
        {
            this.frmCaller = clr;
        }
        public void RequestStop()
        {
            ShouldStop = true;
        }
        private void Wait4IncomingData()
        {
            //Wait for incoming data packet
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
                MessageBox.Show("Error: Connection lost by QC.");
                //this.frmCaller.refreshConnectStatus();
                DABRAS.DisableWatchdog();
                BackgroundThreadFinished(this, null);
                return;
            }
        }
        private string BuildToolTipText(double avga, double avgb,bool lessAlphaLo, bool greaterAlphaHi, bool lessBetaLo, bool greaterBetaHi)
        {
            StringBuilder resnBuilder = new StringBuilder();

            if (lessAlphaLo)
            {
                resnBuilder.AppendFormat("Average Alpha - AlphaBackground = {0} is less than the alpha lower limit {1}", StaticMethods.RoundToSigFigs(avga), Lo);
                resnBuilder.Append(Environment.NewLine);
            }
            if (greaterAlphaHi)
            {
                resnBuilder.AppendFormat("Average Alpha - AlphaBackground = {0} is higher than the alpha higher limit {1}", StaticMethods.RoundToSigFigs(avga), Hi);
                resnBuilder.Append(Environment.NewLine);
            }
            if (lessBetaLo)
            {
                resnBuilder.AppendFormat("Average Beta - BetaBackground = {0} is less than the beta lower limit {1}", StaticMethods.RoundToSigFigs(avgb), Lo);
                resnBuilder.Append(Environment.NewLine);
            }
            if (greaterBetaHi)
            {
                resnBuilder.AppendFormat("Average Beta - BetaBackground = {0} is higher than the beta higher limit {1}", StaticMethods.RoundToSigFigs(avgb), Hi);
                resnBuilder.Append(Environment.NewLine);
            }
            return resnBuilder.ToString();
        }
        private bool ComputeAvgsAndStdDevs()//Compute Averages and Standard Deviations
        {
            double AverageAlphaGCPM = 0;
            double AverageBetaGCPM = 0;
            double AverageAlphaNCPM = 0;
            double AverageBetaNCPM = 0;

            double StdDevGAlpha = 0;
            double StdDevNAlpha = 0;
            double StdDevGBeta = 0;
            double StdDevNBeta = 0;

            //Compute averages
            for (int i = 0; i < NumSamples; i++)
            {
                if (this.IsAlphaTest)
                {
                    AverageAlphaGCPM += Convert.ToDouble(QC_Table[2, i].Value);
                    AverageAlphaNCPM += Convert.ToDouble(QC_Table[3, i].Value);
                }
                else
                {//Beta test
                    AverageBetaGCPM += Convert.ToDouble(QC_Table[4, i].Value);
                    AverageBetaNCPM += Convert.ToDouble(QC_Table[5, i].Value);
                }
            }

            if (this.IsAlphaTest)
            {
                AverageAlphaGCPM /= Convert.ToDouble(NumSamples);
                AverageAlphaNCPM /= Convert.ToDouble(NumSamples);
                DataGridViewCell AverageAlphaGCPMCell = QC_Table[2, NumSamples];
                DataGridViewCell AverageAlphaNCPMCell = QC_Table[3, NumSamples];
                AverageAlphaGCPMCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaGCPM);
                AverageAlphaNCPMCell.Value = StaticMethods.RoundToSigFigs(AverageAlphaNCPM);

                this.AlphaGCPM = AverageAlphaGCPM;
                this.AlphaNCPM = AverageAlphaNCPM;
            }
            else
            {//Beta test
                AverageBetaGCPM /= Convert.ToDouble(NumSamples);
                AverageBetaNCPM /= Convert.ToDouble(NumSamples);
                DataGridViewCell AverageBetaGCPMCell = QC_Table[4, NumSamples];
                DataGridViewCell AverageBetaNCPMCell = QC_Table[5, NumSamples];
                AverageBetaGCPMCell.Value = StaticMethods.RoundToSigFigs(AverageBetaGCPM);
                AverageBetaNCPMCell.Value = StaticMethods.RoundToSigFigs(AverageBetaNCPM);

                this.BetaGCPM = AverageBetaGCPM;
                this.BetaNCPM = AverageBetaNCPM;
            }

            //Compute standard deviations
            for (int i = 0; i < NumSamples; i++)
            {
                if (this.IsAlphaTest)
                {
                    StdDevGAlpha += (AverageAlphaGCPM - Convert.ToDouble(QC_Table[2, i].Value)) * (AverageAlphaGCPM - Convert.ToDouble(QC_Table[2, i].Value));
                    StdDevNAlpha += (AverageAlphaNCPM - Convert.ToDouble(QC_Table[3, i].Value)) * (AverageAlphaNCPM - Convert.ToDouble(QC_Table[3, i].Value));
                }
                else
                {//Beta test
                    StdDevGBeta += (AverageBetaGCPM - Convert.ToDouble(QC_Table[4, i].Value)) * (AverageBetaGCPM - Convert.ToDouble(QC_Table[4, i].Value));
                    StdDevNBeta += (AverageBetaNCPM - Convert.ToDouble(QC_Table[5, i].Value)) * (AverageBetaNCPM - Convert.ToDouble(QC_Table[5, i].Value));
                }
            }

            if (this.IsAlphaTest)
            {
                DataGridViewCell StdDevAlphaGCell = QC_Table[2, NumSamples + 1];
                DataGridViewCell StdDevAlphaNCell = QC_Table[3, NumSamples + 1];
                if (NumSamples > 1)
                {
                    StdDevGAlpha /= Convert.ToDouble((NumSamples - 1));
                    StdDevNAlpha /= Convert.ToDouble((NumSamples - 1));
                    StdDevGAlpha = Math.Sqrt(StdDevGAlpha);
                    StdDevNAlpha = Math.Sqrt(StdDevNAlpha);
                }
                else
                {
                    StdDevGAlpha = Math.Sqrt(AverageAlphaGCPM);
                    StdDevNAlpha = Math.Sqrt(AverageAlphaNCPM);               
                }
                StdDevAlphaGCell.Value = StaticMethods.RoundToSigFigs(StdDevGAlpha);
                StdDevAlphaNCell.Value = StaticMethods.RoundToSigFigs(StdDevNAlpha);
           }
           else
            {//Beta test
                DataGridViewCell StdDevBetaGCell = QC_Table[4, NumSamples + 1];
                DataGridViewCell StdDevBetaNCell = QC_Table[5, NumSamples + 1];
                if (NumSamples > 1)
                {
                    StdDevGBeta /= Convert.ToDouble((NumSamples - 1));
                    StdDevNBeta /= Convert.ToDouble((NumSamples - 1));
                    StdDevGBeta = Math.Sqrt(StdDevGBeta);
                    StdDevNBeta = Math.Sqrt(StdDevNBeta);
                }
                else
                {
                    StdDevGBeta = Math.Sqrt(AverageBetaGCPM);
                    StdDevNBeta = Math.Sqrt(AverageBetaNCPM);
                }
               StdDevBetaGCell.Value = StaticMethods.RoundToSigFigs(StdDevGBeta);
               StdDevBetaNCell.Value = StaticMethods.RoundToSigFigs(StdDevNBeta);
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
                       
            //Display overall pass/fail--Test fails if any one of the tests fails.
            bool lessAlphaLo = !(this.GetAlphaNCPM() >= Lo);
            bool greaterAlphaHi = !(this.GetAlphaNCPM() <= Hi);
            bool lessBetaLo = !(this.GetBetaNCPM() >= Lo);
            bool greaterBetaHi = !(this.GetBetaNCPM() <= Hi);

            DataGridViewCell FinalPassFail = QC_Table[6, NumSamples];

            if (this.IsAlphaTest)
            {
                if (!lessAlphaLo && !greaterAlphaHi)// && !lessBetaLo && !greaterBetaHi)
                {
                    FinalPassFail.Value = "PASS";
                    FinalPassFail.Style = new DataGridViewCellStyle { ForeColor = Color.Green, Font = new Font(this.QC_Table.Font, FontStyle.Bold) };
                    return true;
                }
                else
                {
                    FinalPassFail.Value = "FAIL (?)";
                    FinalPassFail.Style = new DataGridViewCellStyle { ForeColor = Color.Red, Font = new Font(this.QC_Table.Font, FontStyle.Bold) };
                    FinalPassFail.ToolTipText = BuildToolTipText(this.AlphaNCPM, this.BetaNCPM, lessAlphaLo, greaterAlphaHi, false, false);
                    return false; 
                }
            }
            else
            {//Beta test
                if (!lessBetaLo && !greaterBetaHi)
                {
                    FinalPassFail.Value = "PASS";
                    FinalPassFail.Style = new DataGridViewCellStyle { ForeColor = Color.Green, Font = new Font(this.QC_Table.Font, FontStyle.Bold) };
                    return true;
                }
                else
                {
                    FinalPassFail.Value = "FAIL (?)";
                    FinalPassFail.Style = new DataGridViewCellStyle { ForeColor = Color.Red, Font = new Font(this.QC_Table.Font, FontStyle.Bold) };
                    FinalPassFail.ToolTipText = BuildToolTipText(this.AlphaNCPM, this.BetaNCPM, false, false, lessBetaLo, greaterBetaHi);
                    return false; 
                }
            }

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
            //Stop all background threads
            DABRAS.Cut();

            //Set aquisition time
            DABRAS.Write_To_Serial_Port("t");
            Thread.Sleep(250);
            DABRAS.Write_To_Serial_Port(Convert.ToString(SampleTime));
            Thread.Sleep(500);

            //Clear any data left in the buffer
            DABRAS.ClearSerialPacket();
            DABRAS.ClearPacketFlag();
            DABRAS.EnableWatchdog();

            //Set test passed flag
            this.TestPassed = true;

            for (int i = 0; i < NumSamples; i++)
            {
                bool RowComplete = false;
                DABRAS.Write_To_Serial_Port("g");
                DABRAS.ClearSerialPacket();
                DABRAS.ClearPacketFlag();
                //Thread.Sleep(100);

                //Check for the first good packet
                while (!ShouldStop)
                {
                    Wait4IncomingData();

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
                }//first good packet of data

                //Do not increment the row index until the current sample time has elapsed
                while (!RowComplete && !ShouldStop)
                {
                    Wait4IncomingData();

                    SerialPacket IncomingData = DABRAS.ReadSerialPacket();

                    //Grab handles to form
                    DataGridViewCell TimeElapsed = QC_Table[1, i];
                    DataGridViewCell AlphaGCPM = QC_Table[2, i];
                    DataGridViewCell AlphaNCPM = QC_Table[3, i];
                    DataGridViewCell BetaGCPM = QC_Table[4, i];
                    DataGridViewCell BetaNCPM = QC_Table[5, i];
                    DataGridViewCell PassFail = QC_Table[6, i];

                    //Parse incoming data to form
                    TimeElapsed.Value = IncomingData.ElTime;

                    if (IncomingData != null && IncomingData.ElTime != 0)
                    {
                        if (this.IsAlphaTest)
                        {
                            AlphaGCPM.Value = StaticMethods.RoundToDecimal((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)), 1);//gross
                            AlphaNCPM.Value = StaticMethods.RoundToDecimal((IncomingData.AlphaTot / (((double)IncomingData.ElTime) / 60)) - AlphaBackground, 1);//net
                        }
                        else
                        {//Beta test

                            BetaGCPM.Value = StaticMethods.RoundToSigFigs((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)));//gross
                            BetaNCPM.Value = StaticMethods.RoundToDecimal((IncomingData.BetaTot / (((double)IncomingData.ElTime) / 60)) - BetaBackground, 1);//net
                        }
                        //Re-draw table
                        QC_Table.Invalidate();

                        //If the sample time has elapsed, increment the row
                        if (IncomingData.ElTime >= SampleTime)
                        {
                            RowComplete = true;
                        }//the sample time has elapsed
                    }
                    DABRAS.KickWatchdog();
                }
            }//finish all the samples

            DABRAS.DisableWatchdog();

            if (!ShouldStop)
            {
                this.TestPassed = this.ComputeAvgsAndStdDevs();

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

            //Resume any background threads, if they existed
            DABRAS.ResumeBackgroundMonitors();
            this.Running = false;

            return;
        }
        #endregion
    }

    #endregion
}