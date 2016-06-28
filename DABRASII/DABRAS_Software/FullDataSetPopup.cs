using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DABRAS_Software
{
    public partial class FullDataSetPopup : Form
    {
        #region Data Members
        private QCAlphaBetaListener AB_Listener = null;
        private QCBackgroundListener BG_Listener = null;
        private Logger Logger;
        #endregion

        #region Constructor
        public FullDataSetPopup(QCAlphaBetaListener _A, QCBackgroundListener _B, Logger _L)
        {
            InitializeComponent();

            this.Logger = _L;
            this.AB_Listener = _A;
            this.BG_Listener = _B;
            
            FullDataSet.Columns.Add("FullDataSet_Date", "Date");
            FullDataSet.Columns.Add("FullDataSet_Time", "Acq Time");
            FullDataSet.Columns.Add("FullDataSet_AUL", "Alpha UL");
            FullDataSet.Columns.Add("FullDataSet_ALL", "Alpha LL");
            FullDataSet.Columns.Add("FullDataSet_AGCPM", "Alpha GCPM");
            FullDataSet.Columns.Add("FullDataSet_ANCPM", "Alpha NCPM");
            FullDataSet.Columns.Add("FullDataSet_BUL", "Beta UL");
            FullDataSet.Columns.Add("FullDataSet_BLL", "Beta LL");
            FullDataSet.Columns.Add("FullDataSet_BGCPM", "Beta GCPM");
            FullDataSet.Columns.Add("FullDataSet_BNCPM", "Beta NCPM");
            FullDataSet.Columns.Add("FullDataSet_PassFail", "Pass/Fail");

            if (_A != null)
            {
                if (_A.AlphaTest())
                {
                    FullDataSet.Rows.Add(Convert.ToString(AB_Listener.GetDateTimeStarted()), String.Format("{0}:{1}", (((AB_Listener.GetNumSamples() * AB_Listener.GetSampleTime()) + 1) / 60) , (((AB_Listener.GetNumSamples() * AB_Listener.GetSampleTime()) + 1) % 60)), Convert.ToString(AB_Listener.GetHi()), Convert.ToString(AB_Listener.GetLo()), Convert.ToString(AB_Listener.GetAlphaGCPM()), Convert.ToString(AB_Listener.GetAlphaNCPM()), "N/A", "N/A", "N/A", "N/A", (AB_Listener.WasTestPassed() ? "PASS" : "FAIL"));
                }
                else
                {
                    FullDataSet.Rows.Add(AB_Listener.GetDateTimeStarted(), String.Format("{0}:{1}", (((AB_Listener.GetNumSamples() * AB_Listener.GetSampleTime()) + 1) / 60), (((AB_Listener.GetNumSamples() * AB_Listener.GetSampleTime()) + 1) % 60)), "N/A", "N/A", "N/A", "N/A", AB_Listener.GetHi(), AB_Listener.GetLo(), AB_Listener.GetBetaGCPM(), AB_Listener.GetBetaNCPM(), (AB_Listener.WasTestPassed() ? "PASS" : "FAIL"));
                }
            }

            else if (_B != null)
            {
                FullDataSet.Rows.Add(BG_Listener.GetDateTimeStarted(), String.Format("{0}:{1}", (((BG_Listener.GetNumSamples() * BG_Listener.GetSampleTime()) + 1) / 60), (((BG_Listener.GetNumSamples() * BG_Listener.GetSampleTime()) + 1) % 60)), BG_Listener.GetAlphaHi(), BG_Listener.GetAlphaLo(), BG_Listener.GetAlphaGCPM(), BG_Listener.GetBetaHi(), BG_Listener.GetBetaLo(), BG_Listener.GetBetaGCPM(), (BG_Listener.WasTestPassed() ? "PASS" : "FAIL"));
            }

            /*Add keypress event handlers*/
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

        }
        #endregion

        #region Private Utility Functions
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

        #region Write Handler
        private void writeToFileCtrlVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[,] DataToWrite = MakeDataWritable(this.FullDataSet);

            SaveFileDialog S = new SaveFileDialog();
            S.Filter = "Comma Separated Value|*.csv";
            S.ShowDialog();
            if (S.FileName != "")
            {
                FileStream F = (FileStream)S.OpenFile();
                string FilePath = S.FileName;
                Logger.WriteCSV(F, DataToWrite);
            }

            return;
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

        #region KeyPress Handler
        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }

            if (Key.Control)
            {
                if (Key.KeyCode == Keys.Q)
                {
                    closeCtrlQToolStripMenuItem_Click(this, null);
                }
            }
        }
        #endregion
    }
}
