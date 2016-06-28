using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;


namespace DABRAS_Software
{
    public partial class DailyQCGraph : Form
    {
        #region Data Members
        private QCListKeeper QC_List;
        private List<Radioactive_Source> ListOfSources;
        private DefaultConfigurations DC;

        private Logger Logger;

        private Form LaunchedFrom;

        #endregion
              
        #region Constructor
        public DailyQCGraph(Form Parent, QCListKeeper _List, Logger _Logger, List<Radioactive_Source> _Sources, DefaultConfigurations _DC)
        {
            InitializeComponent();

            this.LaunchedFrom = Parent;
            this.QC_List = _List;
            this.Logger = _Logger;
            this.ListOfSources = _Sources;

            this.DC = _DC;

            Chart.SuppressExceptions = false;

            Load += DailyQCGraph_Load;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }

        private void DailyQCGraph_Load(object sender, EventArgs e)
        {
            RedrawGraph();
        }
        #endregion

        #region Save button Handlers
        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog S = new SaveFileDialog();
            S.Filter = "Comma Separated Value|*.csv";
            S.ShowDialog();

            if (S.FileName != "")
            {
                string[,] FileToWrite = QC_List.GetCurrentCSVRepresentation();
                FileStream F = (FileStream)S.OpenFile();
                string FilePath = S.FileName;
                switch (S.FilterIndex)
                {
                    case 1:
                        Logger.WriteCSV(F, FileToWrite);
                        MessageBox.Show("File Written.");
                        break;
                    default:
                        break;
                }
            }
        }

        private void SaveAllButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog S = new SaveFileDialog();
            S.Filter = "Comma Separated Value|*.csv";
            S.ShowDialog();

            if (S.FileName != "")
            {
                string[,] FileToWrite = QC_List.GetFullCSVRepresentation();
                FileStream F = (FileStream)S.OpenFile();
                string FilePath = S.FileName;
                switch (S.FilterIndex)
                {
                    case 1:
                        Logger.WriteCSV(F, FileToWrite);
                        MessageBox.Show("File Written.");
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Save Image Handler
        private void ImageSaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog SD = new SaveFileDialog();
            SD.Filter = "Portable Network Graphics (*.png)|*.png";
            SD.ShowDialog();

            if (SD.FileName != "")
            {
                /*Add Building and Set Number*/
                TextAnnotation BuildingAnnotation = new TextAnnotation();
                BuildingAnnotation.X = Chart.Legends[0].Position.X;
                BuildingAnnotation.Y = Chart.Legends[0].Position.Bottom;
                BuildingAnnotation.IsMultiline = true;
                BuildingAnnotation.Text = String.Format("Building Number: {0}\nSet Number: {1}", DC.GetBuildingNo(), DC.GetSetNo());
                
                BuildingAnnotation.ForeColor = Color.Red;
                Chart.Annotations.Add(BuildingAnnotation);

                using (FileStream F = (FileStream)SD.OpenFile())
                {
                    Chart.SaveImage(F, ChartImageFormat.Png);
                }
                Chart.Annotations.Remove(BuildingAnnotation);
                MessageBox.Show("Image Saved.");
            }

        }
        #endregion

        #region Misc GUI Functions
        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm NewForm = new AboutForm();
            NewForm.ShowDialog();
        }
        #endregion

        #region Graph Init
        private void RedrawGraph()
        {
            /*Clear chart*/
            Chart.Series.Clear();
            QC_List.Lock();

            List<QCCalResultNode> NodesToPlot;
            if (this.FullDataSet_CB.Checked)
            {
                NodesToPlot = QC_List.GetFullList();
            }
            else
            {
                NodesToPlot = QC_List.GetCurrentList();
            }
            /*Start by creating the series*/
            #region Background
            #region BGAlpha

            /*Main BGAlpha*/
            Chart.Series.Add(new Series(name: "Background Alpha"));
            Chart.Series["Background Alpha"].ChartType = SeriesChartType.FastPoint;
            Chart.Series["Background Alpha"].XValueType = ChartValueType.Date;
            Chart.Series["Background Alpha"].Enabled = this.BG_Alpha_CheckBox.Checked;
            Chart.Series["Background Alpha"].MarkerSize = 5;
            Chart.Series["Background Alpha"].Color = Color.Red;


            /*2sig BGAlpha*/
            Chart.Series.Add("BG_Alpha_2sig+");
            Chart.Series["BG_Alpha_2sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Alpha_2sig+"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Alpha_2sig+"].Enabled = this.BG_Alpha_2Sig_CB.Checked && this.BG_Alpha_2Sig_CB.Enabled;
            Chart.Series["BG_Alpha_2sig+"].BorderWidth = 3;
            Chart.Series["BG_Alpha_2sig+"].Color = Color.Purple;


            Chart.Series.Add("BG_Alpha_2sig-");
            Chart.Series["BG_Alpha_2sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Alpha_2sig-"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Alpha_2sig-"].Enabled = this.BG_Alpha_2Sig_CB.Checked && this.BG_Alpha_2Sig_CB.Enabled;
            Chart.Series["BG_Alpha_2sig-"].BorderWidth = 3;
            Chart.Series["BG_Alpha_2sig-"].Color = Color.Purple;


            /*3sig BGAlpha*/
            Chart.Series.Add("BG_Alpha_3sig+");
            Chart.Series["BG_Alpha_3sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Alpha_3sig+"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Alpha_3sig+"].Enabled = this.BG_Alpha_3Sig_CB.Checked && this.BG_Alpha_3Sig_CB.Enabled;
            Chart.Series["BG_Alpha_3sig+"].BorderWidth = 3;
            Chart.Series["BG_Alpha_3sig+"].Color = Color.SteelBlue;

            Chart.Series.Add("BG_Alpha_3sig-");
            Chart.Series["BG_Alpha_3sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Alpha_3sig-"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Alpha_3sig-"].Enabled = this.BG_Alpha_3Sig_CB.Checked && this.BG_Alpha_3Sig_CB.Enabled;
            Chart.Series["BG_Alpha_3sig-"].BorderWidth = 3;
            Chart.Series["BG_Alpha_3sig-"].Color = Color.SteelBlue;

            /*10% BGAlpha*/
            Chart.Series.Add("BG_Alpha_10p-");
            Chart.Series["BG_Alpha_10p-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Alpha_10p-"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Alpha_10p-"].Enabled = this.BG_Alpha_10p_CB.Checked && this.BG_Alpha_10p_CB.Enabled;
            Chart.Series["BG_Alpha_10p-"].BorderWidth = 3;
            Chart.Series["BG_Alpha_10p-"].Color = Color.DarkOliveGreen;

            Chart.Series.Add("BG_Alpha_10p+");
            Chart.Series["BG_Alpha_10p+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Alpha_10p+"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Alpha_10p+"].Enabled = this.BG_Alpha_10p_CB.Checked && this.BG_Alpha_10p_CB.Enabled;
            Chart.Series["BG_Alpha_10p+"].BorderWidth = 3;
            Chart.Series["BG_Alpha_10p+"].Color = Color.DarkOliveGreen;


            /*Avg BGAlpha*/
            Chart.Series.Add("BG_Alpha_Avg");
            Chart.Series["BG_Alpha_Avg"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Alpha_Avg"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Alpha_Avg"].Enabled = this.BG_Alpha_Avg_CB.Checked && this.BG_Alpha_Avg_CB.Enabled;
            Chart.Series["BG_Alpha_Avg"].BorderWidth = 3;
            Chart.Series["BG_Alpha_Avg"].Color = Color.SaddleBrown;


            #endregion

            #region BGBeta

            /*Main BGBeta*/
            Chart.Series.Add("Background Beta");
            Chart.Series["Background Beta"].ChartType = SeriesChartType.FastPoint;
            Chart.Series["Background Beta"].XValueType = ChartValueType.Date;
            Chart.Series["Background Beta"].Enabled = this.BG_Beta_CheckBox.Checked;
            Chart.Series["Background Beta"].MarkerSize = 10;
            Chart.Series["Background Beta"].Color = Color.Blue;



            /*2sig BGBeta*/
            Chart.Series.Add("BG_Beta_2sig+");
            Chart.Series["BG_Beta_2sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Beta_2sig+"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Beta_2sig+"].Enabled = this.BG_Beta_2Sig_CB.Checked && this.BG_Beta_2Sig_CB.Enabled;
            Chart.Series["BG_Beta_2sig+"].BorderWidth = 3;
            Chart.Series["BG_Beta_2sig+"].Color = Color.Purple;

            Chart.Series.Add("BG_Beta_2sig-");
            Chart.Series["BG_Beta_2sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Beta_2sig-"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Beta_2sig-"].Enabled = this.BG_Beta_2Sig_CB.Checked && this.BG_Beta_2Sig_CB.Enabled;
            Chart.Series["BG_Beta_2sig-"].BorderWidth = 3;
            Chart.Series["BG_Beta_2sig-"].Color = Color.Purple;



            /*3sig BGBeta*/
            Chart.Series.Add("BG_Beta_3sig+");
            Chart.Series["BG_Beta_3sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Beta_3sig+"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Beta_3sig+"].Enabled = this.BG_Beta_3Sig_CB.Checked && this.BG_Beta_3Sig_CB.Enabled;
            Chart.Series["BG_Beta_3sig+"].BorderWidth = 3;
            Chart.Series["BG_Beta_3sig+"].Color = Color.SteelBlue;


            Chart.Series.Add("BG_Beta_3sig-");
            Chart.Series["BG_Beta_3sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Beta_3sig-"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Beta_3sig-"].Enabled = this.BG_Beta_3Sig_CB.Checked && this.BG_Beta_3Sig_CB.Enabled;
            Chart.Series["BG_Beta_3sig-"].BorderWidth = 3;
            Chart.Series["BG_Beta_3sig-"].Color = Color.SteelBlue;

            /*10% BGBeta*/
            Chart.Series.Add("BG_Beta_10p-");
            Chart.Series["BG_Beta_10p-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Beta_10p-"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Beta_10p-"].Enabled = this.BG_Beta_10p_CB.Checked && this.BG_Beta_10p_CB.Enabled;
            Chart.Series["BG_Beta_10p-"].BorderWidth = 3;
            Chart.Series["BG_Beta_10p-"].Color = Color.DarkOliveGreen;

            Chart.Series.Add("BG_Beta_10p+");
            Chart.Series["BG_Beta_10p+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Beta_10p+"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Beta_10p+"].Enabled = this.BG_Beta_10p_CB.Checked && this.BG_Beta_10p_CB.Enabled;
            Chart.Series["BG_Beta_10p+"].BorderWidth = 3;
            Chart.Series["BG_Beta_10p+"].Color = Color.DarkOliveGreen;


            /*Average BGBeta*/
            Chart.Series.Add("BG_Beta_Avg");
            Chart.Series["BG_Beta_Avg"].ChartType = SeriesChartType.FastLine;
            Chart.Series["BG_Beta_Avg"].XValueType = ChartValueType.Date;
            Chart.Series["BG_Beta_Avg"].Enabled = this.BG_Beta_Avg_CB.Checked && this.BG_Beta_Avg_CB.Enabled;
            Chart.Series["BG_Beta_Avg"].BorderWidth = 3;
            Chart.Series["BG_Beta_Avg"].Color = Color.SaddleBrown;


            #endregion
            #endregion

            #region Alpha

            #region AlphaResults

            /*Main Alpha_Alpha*/
            Chart.Series.Add("Alpha Source Alpha");
            Chart.Series["Alpha Source Alpha"].ChartType = SeriesChartType.FastPoint;
            Chart.Series["Alpha Source Alpha"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha Source Alpha"].Enabled = this.Alpha_Alpha_CheckBox.Checked;
            Chart.Series["Alpha Source Alpha"].MarkerSize = 10;
            Chart.Series["Alpha Source Alpha"].Color = Color.Indigo;


            /*2sig Alpha_Alpha*/
            Chart.Series.Add("Alpha_Alpha_2sig+");
            Chart.Series["Alpha_Alpha_2sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Alpha_2sig+"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Alpha_2sig+"].Enabled = this.Alpha_Alpha_2Sig_CB.Checked && this.Alpha_Alpha_2Sig_CB.Enabled;
            Chart.Series["Alpha_Alpha_2sig+"].BorderWidth = 3;
            Chart.Series["Alpha_Alpha_2sig+"].Color = Color.Purple;


            Chart.Series.Add("Alpha_Alpha_2sig-");
            Chart.Series["Alpha_Alpha_2sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Alpha_2sig-"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Alpha_2sig-"].Enabled = this.Alpha_Alpha_2Sig_CB.Checked && this.Alpha_Alpha_2Sig_CB.Enabled;
            Chart.Series["Alpha_Alpha_2sig-"].BorderWidth = 3;
            Chart.Series["Alpha_Alpha_2sig-"].Color = Color.Purple;


            /*3sig Alpha_Alpha*/
            Chart.Series.Add("Alpha_Alpha_3sig+");
            Chart.Series["Alpha_Alpha_3sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Alpha_3sig+"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Alpha_3sig+"].Enabled = this.Alpha_Alpha_3Sig_CB.Checked && this.Alpha_Alpha_3Sig_CB.Enabled;
            Chart.Series["Alpha_Alpha_3sig+"].BorderWidth = 3;
            Chart.Series["Alpha_Alpha_3sig+"].Color = Color.SteelBlue;

            Chart.Series.Add("Alpha_Alpha_3sig-");
            Chart.Series["Alpha_Alpha_3sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Alpha_3sig-"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Alpha_3sig-"].Enabled = this.Alpha_Alpha_3Sig_CB.Checked && this.Alpha_Alpha_3Sig_CB.Enabled;
            Chart.Series["Alpha_Alpha_3sig-"].BorderWidth = 3;
            Chart.Series["Alpha_Alpha_3sig-"].Color = Color.SteelBlue;

            /*10% AlphaAlpha*/
            Chart.Series.Add("Alpha_Alpha_10p-");
            Chart.Series["Alpha_Alpha_10p-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Alpha_10p-"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Alpha_10p-"].Enabled = this.Alpha_Alpha_10p_CB.Checked && this.Alpha_Alpha_10p_CB.Enabled;
            Chart.Series["Alpha_Alpha_10p-"].BorderWidth = 3;
            Chart.Series["Alpha_Alpha_10p-"].Color = Color.DarkOliveGreen;

            Chart.Series.Add("Alpha_Alpha_10p+");
            Chart.Series["Alpha_Alpha_10p+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Alpha_10p+"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Alpha_10p+"].Enabled = this.Alpha_Alpha_10p_CB.Checked && this.Alpha_Alpha_10p_CB.Enabled;
            Chart.Series["Alpha_Alpha_10p+"].BorderWidth = 3;
            Chart.Series["Alpha_Alpha_10p+"].Color = Color.DarkOliveGreen;


            /*Avg Alpha_Alpha*/
            Chart.Series.Add("Alpha_Alpha_Avg");
            Chart.Series["Alpha_Alpha_Avg"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Alpha_Avg"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Alpha_Avg"].Enabled = this.Alpha_Alpha_Avg_CB.Checked && Alpha_Alpha_Avg_CB.Enabled;
            Chart.Series["Alpha_Alpha_Avg"].BorderWidth = 3;
            Chart.Series["Alpha_Alpha_Avg"].Color = Color.SaddleBrown;


            #endregion

            #region BetaResults

            /*Main Alpha_Beta*/
            Chart.Series.Add("Alpha Source Beta");
            Chart.Series["Alpha Source Beta"].ChartType = SeriesChartType.FastPoint;
            Chart.Series["Alpha Source Beta"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha Source Beta"].Enabled = this.Alpha_Beta_CheckBox.Checked;
            Chart.Series["Alpha Source Beta"].MarkerSize = 10;
            Chart.Series["Alpha Source Beta"].Color = Color.SeaGreen;


            /*2sig Alpha_Beta*/
            Chart.Series.Add("Alpha_Beta_2sig+");
            Chart.Series["Alpha_Beta_2sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Beta_2sig+"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Beta_2sig+"].Enabled = this.Alpha_Beta_2Sig_CB.Checked && this.Alpha_Beta_2Sig_CB.Enabled;
            Chart.Series["Alpha_Beta_2sig+"].BorderWidth = 3;
            Chart.Series["Alpha_Beta_2sig+"].Color = Color.Purple;

            Chart.Series.Add("Alpha_Beta_2sig-");
            Chart.Series["Alpha_Beta_2sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Beta_2sig-"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Beta_2sig-"].Enabled = this.Alpha_Beta_2Sig_CB.Checked && this.Alpha_Beta_2Sig_CB.Enabled;
            Chart.Series["Alpha_Beta_2sig-"].BorderWidth = 3;
            Chart.Series["Alpha_Beta_2sig-"].Color = Color.Purple;

            /*3sig Alpha_Beta*/
            Chart.Series.Add("Alpha_Beta_3sig+");
            Chart.Series["Alpha_Beta_3sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Beta_3sig+"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Beta_3sig+"].Enabled = this.Alpha_Beta_3Sig_CB.Checked && this.Alpha_Beta_3Sig_CB.Enabled;
            Chart.Series["Alpha_Beta_3sig+"].BorderWidth = 3;
            Chart.Series["Alpha_Beta_3sig+"].Color = Color.SteelBlue;


            Chart.Series.Add("Alpha_Beta_3sig-");
            Chart.Series["Alpha_Beta_3sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Beta_3sig-"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Beta_3sig-"].Enabled = this.Alpha_Beta_3Sig_CB.Checked && this.Alpha_Beta_3Sig_CB.Enabled;
            Chart.Series["Alpha_Beta_3sig-"].BorderWidth = 3;
            Chart.Series["Alpha_Beta_3sig-"].Color = Color.SteelBlue;

            /*10% Alpha Beta*/
            Chart.Series.Add("Alpha_Beta_10p-");
            Chart.Series["Alpha_Beta_10p-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Beta_10p-"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Beta_10p-"].Enabled = this.Alpha_Beta_10p_CB.Checked && this.Alpha_Beta_10p_CB.Enabled;
            Chart.Series["Alpha_Beta_10p-"].BorderWidth = 3;
            Chart.Series["Alpha_Beta_10p-"].Color = Color.DarkOliveGreen;

            Chart.Series.Add("Alpha_Beta_10p+");
            Chart.Series["Alpha_Beta_10p+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Beta_10p+"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Beta_10p+"].Enabled = this.Alpha_Beta_10p_CB.Checked && this.Alpha_Beta_10p_CB.Enabled;
            Chart.Series["Alpha_Beta_10p+"].BorderWidth = 3;
            Chart.Series["Alpha_Beta_10p+"].Color = Color.DarkOliveGreen;

            /*Avg Alpha_Beta*/
            Chart.Series.Add("Alpha_Beta_Avg");
            Chart.Series["Alpha_Beta_Avg"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Alpha_Beta_Avg"].XValueType = ChartValueType.Date;
            Chart.Series["Alpha_Beta_Avg"].Enabled = this.Alpha_Beta_Avg_CB.Checked && this.Alpha_Beta_Avg_CB.Enabled;
            Chart.Series["Alpha_Beta_Avg"].BorderWidth = 3;
            Chart.Series["Alpha_Beta_Avg"].Color = Color.SaddleBrown;


            #endregion
            #endregion

            #region Beta

            #region Alpha Results

            /*Main Beta_Alpha*/
            Chart.Series.Add("Beta Source Alpha");
            Chart.Series["Beta Source Alpha"].ChartType = SeriesChartType.FastPoint;
            Chart.Series["Beta Source Alpha"].XValueType = ChartValueType.Date;
            Chart.Series["Beta Source Alpha"].Enabled = this.Beta_Alpha_CheckBox.Checked;
            Chart.Series["Beta Source Alpha"].MarkerSize = 10;
            Chart.Series["Beta Source Alpha"].Color = Color.Chocolate;

            /*2sig Beta_Alpha*/
            Chart.Series.Add("Beta_Alpha_2sig+");
            Chart.Series["Beta_Alpha_2sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Alpha_2sig+"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Alpha_2sig+"].Enabled = this.Beta_Alpha_2Sig_CB.Checked && this.Beta_Alpha_2Sig_CB.Enabled;
            Chart.Series["Beta_Alpha_2sig+"].BorderWidth = 3;
            Chart.Series["Beta_Alpha_2sig+"].Color = Color.Purple;

            Chart.Series.Add("Beta_Alpha_2sig-");
            Chart.Series["Beta_Alpha_2sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Alpha_2sig-"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Alpha_2sig-"].Enabled = this.Beta_Alpha_2Sig_CB.Checked && this.Beta_Alpha_2Sig_CB.Enabled;
            Chart.Series["Beta_Alpha_2sig-"].BorderWidth = 3;
            Chart.Series["Beta_Alpha_2sig-"].Color = Color.Purple;


            /*3sig Beta_Alpha*/
            Chart.Series.Add("Beta_Alpha_3sig+");
            Chart.Series["Beta_Alpha_3sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Alpha_3sig+"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Alpha_3sig+"].Enabled = this.Beta_Alpha_3Sig_CB.Checked && this.Beta_Alpha_3Sig_CB.Enabled;
            Chart.Series["Beta_Alpha_3sig+"].BorderWidth = 3;
            Chart.Series["Beta_Alpha_3sig+"].Color = Color.SteelBlue;

            Chart.Series.Add("Beta_Alpha_3sig-");
            Chart.Series["Beta_Alpha_3sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Alpha_3sig-"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Alpha_3sig-"].Enabled = this.Beta_Alpha_3Sig_CB.Checked && this.Beta_Alpha_3Sig_CB.Enabled;
            Chart.Series["Beta_Alpha_3sig-"].BorderWidth = 3;
            Chart.Series["Beta_Alpha_3sig-"].Color = Color.SteelBlue;

            /*10% BGAlpha*/
            Chart.Series.Add("Beta_Alpha_10p-");
            Chart.Series["Beta_Alpha_10p-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Alpha_10p-"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Alpha_10p-"].Enabled = this.Beta_Alpha_10p_CB.Checked && this.Beta_Alpha_10p_CB.Enabled;
            Chart.Series["Beta_Alpha_10p-"].BorderWidth = 3;
            Chart.Series["Beta_Alpha_10p-"].Color = Color.DarkOliveGreen;

            Chart.Series.Add("Beta_Alpha_10p+");
            Chart.Series["Beta_Alpha_10p+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Alpha_10p+"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Alpha_10p+"].Enabled = this.Beta_Alpha_10p_CB.Checked && this.Beta_Alpha_10p_CB.Enabled;
            Chart.Series["Beta_Alpha_10p+"].BorderWidth = 3;
            Chart.Series["Beta_Alpha_10p+"].Color = Color.DarkOliveGreen;

            /*Avg Beta_Alpha*/
            Chart.Series.Add("Beta_Alpha_Avg");
            Chart.Series["Beta_Alpha_Avg"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Alpha_Avg"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Alpha_Avg"].Enabled = this.Beta_Alpha_Avg_CB.Checked && this.Beta_Alpha_Avg_CB.Enabled;
            Chart.Series["Beta_Alpha_Avg"].BorderWidth = 3;
            Chart.Series["Beta_Alpha_Avg"].Color = Color.SaddleBrown;


            #endregion

            #region Beta Results

            /*Main Beta_Beta*/
            Chart.Series.Add("Beta Source Beta");
            Chart.Series["Beta Source Beta"].ChartType = SeriesChartType.FastPoint;
            Chart.Series["Beta Source Beta"].XValueType = ChartValueType.Date;
            Chart.Series["Beta Source Beta"].Enabled = this.Beta_Beta_CheckBox.Checked;
            Chart.Series["Beta Source Beta"].MarkerSize = 10;
            Chart.Series["Beta Source Beta"].Color = Color.Black;

            /*2sig Beta_Beta*/
            Chart.Series.Add("Beta_Beta_2sig+");
            Chart.Series["Beta_Beta_2sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Beta_2sig+"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Beta_2sig+"].Enabled = this.Beta_Beta_2Sig_CB.Checked && this.Beta_Beta_2Sig_CB.Enabled;
            Chart.Series["Beta_Beta_2sig+"].BorderWidth = 3;
            Chart.Series["Beta_Beta_2sig+"].Color = Color.Purple;

            Chart.Series.Add("Beta_Beta_2sig-");
            Chart.Series["Beta_Beta_2sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Beta_2sig-"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Beta_2sig-"].Enabled = this.Beta_Beta_2Sig_CB.Checked && this.Beta_Beta_2Sig_CB.Enabled;
            Chart.Series["Beta_Beta_2sig-"].BorderWidth = 3;
            Chart.Series["Beta_Beta_2sig-"].Color = Color.Purple;


            /*3sig Beta_Beta*/
            Chart.Series.Add("Beta_Beta_3sig+");
            Chart.Series["Beta_Beta_3sig+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Beta_3sig+"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Beta_3sig+"].Enabled = this.Beta_Beta_3Sig_CB.Checked && this.Beta_Beta_3Sig_CB.Enabled;
            Chart.Series["Beta_Beta_3sig+"].BorderWidth = 3;
            Chart.Series["Beta_Beta_3sig+"].Color = Color.SteelBlue;

            Chart.Series.Add("Beta_Beta_3sig-");
            Chart.Series["Beta_Beta_3sig-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Beta_3sig-"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Beta_3sig-"].Enabled = this.Beta_Beta_3Sig_CB.Checked && this.Beta_Beta_3Sig_CB.Enabled;
            Chart.Series["Beta_Beta_3sig-"].BorderWidth = 3;
            Chart.Series["Beta_Beta_3sig-"].Color = Color.SteelBlue;

            /*10% BGBeta*/
            Chart.Series.Add("Beta_Beta_10p-");
            Chart.Series["Beta_Beta_10p-"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Beta_10p-"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Beta_10p-"].Enabled = this.Beta_Beta_10p_CB.Checked && this.Beta_Beta_10p_CB.Enabled;
            Chart.Series["Beta_Beta_10p-"].BorderWidth = 3;
            Chart.Series["Beta_Beta_10p-"].Color = Color.DarkOliveGreen;

            Chart.Series.Add("Beta_Beta_10p+");
            Chart.Series["Beta_Beta_10p+"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Beta_10p+"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Beta_10p+"].Enabled = this.Beta_Beta_10p_CB.Checked && this.Beta_Beta_10p_CB.Enabled;
            Chart.Series["Beta_Beta_10p+"].BorderWidth = 3;
            Chart.Series["Beta_Beta_10p+"].Color = Color.DarkOliveGreen;

            /*Avg Beta_Beta*/
            Chart.Series.Add("Beta_Beta_Avg");
            Chart.Series["Beta_Beta_Avg"].ChartType = SeriesChartType.FastLine;
            Chart.Series["Beta_Beta_Avg"].XValueType = ChartValueType.Date;
            Chart.Series["Beta_Beta_Avg"].Enabled = this.Beta_Beta_Avg_CB.Checked && this.Beta_Beta_Avg_CB.Checked;
            Chart.Series["Beta_Beta_Avg"].BorderWidth = 3;
            Chart.Series["Beta_Beta_Avg"].Color = Color.SaddleBrown;


            #endregion
            #endregion
            
            Chart.ChartAreas[0].AxisY.LabelStyle.Format = "#.";

            List<double> Background_Alpha_Nodes = new List<double>();
            List<double> Background_Beta_Nodes = new List<double>();
            List<double> Alpha_Alpha_Nodes = new List<double>();
            List<double> Alpha_Beta_Nodes = new List<double>();
            List<double> Beta_Alpha_Nodes = new List<double>();
            List<double> Beta_Beta_Nodes = new List<double>();

            DateTime Earliest = DateTime.Now;
            DateTime Latest = DateTime.Parse("1900/01/01");

            Radioactive_Source Am241 = ListOfSources.Find(x => x.GetSerialNumber() == "Am-241");
            Radioactive_Source Sr90 = ListOfSources.Find(x => x.GetSerialNumber() == "Sr-90");
            Radioactive_Source Bg = ListOfSources.Find(x => x.GetSerialNumber() == "Background");

            if (!FullDataSet_CB.Checked)
            {
                for (int i = 0; i < NodesToPlot.Count; i++)
                {
                    if (NodesToPlot.ElementAt(i).GetTypeOfQC() == FormQC.TypeOfQC.Background)
                    {
                        if (DateTime.Compare(NodesToPlot.ElementAt(i).GetDateTimeCompleted(), Bg.GetAnnualCalibratedTime()) <= 0)
                        {
                            QC_List.MoveToDefunctList(NodesToPlot.ElementAt(i));
                            i--;
                            //NodesToPlot.RemoveAt(i);
                            continue;
                        }
                    }

                    else if (NodesToPlot.ElementAt(i).GetTypeOfQC() == FormQC.TypeOfQC.Alpha)
                    {
                        if (DateTime.Compare(NodesToPlot.ElementAt(i).GetDateTimeCompleted(), Am241.GetAnnualCalibratedTime()) <= 0)
                        {
                            QC_List.MoveToDefunctList(NodesToPlot.ElementAt(i));
                            i--;
                           // NodesToPlot.RemoveAt(i);
                            continue;
                        }
                    }

                    else if (NodesToPlot.ElementAt(i).GetTypeOfQC() == FormQC.TypeOfQC.Beta)
                    {
                        if (DateTime.Compare(NodesToPlot.ElementAt(i).GetDateTimeCompleted(), Sr90.GetAnnualCalibratedTime()) <= 0)
                        {
                            QC_List.MoveToDefunctList(NodesToPlot.ElementAt(i));
                            i--;
                            //NodesToPlot.RemoveAt(i);
                            continue;
                        }
                    }
                }
            }
            
            /*Plot the main points, save the statistics*/
            foreach (QCCalResultNode i in NodesToPlot)
            {
                if (i.IsPlottable() && (!(i.GetName() == ("CBM") || i.GetName() == ("DBM")) || this.AutoCalibration_CB.Checked)) //make sure point is plottable and that we are plotting background points.
                {
                    FormQC.TypeOfQC iType = i.GetTypeOfQC();

                    if (IsBefore(Earliest, i.GetDateTimeCompleted()))
                    {
                        Earliest = i.GetDateTimeCompleted();
                    }

                    if (IsAfter(Latest, i.GetDateTimeCompleted()))
                    {
                        Latest = i.GetDateTimeCompleted();
                    }

                    #region Background
                    #region BGAlpha

                    /*Main BGAlpha*/
                    if (iType == FormQC.TypeOfQC.Background)
                    {
                        Chart.Series["Background Alpha"].Points.AddXY(i.GetDateTimeCompleted().ToOADate(), i.GetNetAlphaCPM());
                        Background_Alpha_Nodes.Add(i.GetNetAlphaCPM());                //CPM, as specified in marlap
                        //Background_Alpha_Nodes.Add(i.GetNetAlphaCPM() * 10);         //Counts, for comparison
                    }

                    #endregion

                    #region BGBeta

                    /*Main BGBeta*/
                    if (iType == FormQC.TypeOfQC.Background)
                    {
                        Chart.Series["Background Beta"].Points.AddXY(i.GetDateTimeCompleted().ToOADate(), i.GetNetBetaCPM());
                        Background_Beta_Nodes.Add(i.GetNetBetaCPM());
                        //Background_Beta_Nodes.Add(i.GetNetBetaCPM() * 10);
                    }

                    #endregion
                    #endregion


                    #region Alpha

                    #region AlphaResults

                    /*Main Alpha_Alpha*/
                    if (iType == FormQC.TypeOfQC.Alpha)
                    {
                        Chart.Series["Alpha Source Alpha"].Points.AddXY(i.GetDateTimeCompleted().ToOADate(), i.GetNetAlphaCPM());
                        Alpha_Alpha_Nodes.Add(i.GetNetAlphaCPM());
                    }

                    #endregion

                    #region BetaResults

                    /*Main Alpha_Beta*/
                    if (iType == FormQC.TypeOfQC.Alpha)
                    {
                        Chart.Series["Alpha Source Beta"].Points.AddXY(i.GetDateTimeCompleted().ToOADate(), i.GetNetBetaCPM());
                        Alpha_Beta_Nodes.Add(i.GetNetBetaCPM());
                    }

                    #endregion
                    #endregion


                    #region Beta

                    #region Alpha Results

                    /*Main Beta_Alpha*/
                    if (iType == FormQC.TypeOfQC.Beta)
                    {
                        Chart.Series["Beta Source Alpha"].Points.AddXY(i.GetDateTimeCompleted().ToOADate(), i.GetNetAlphaCPM());
                        Beta_Alpha_Nodes.Add(i.GetNetAlphaCPM());
                    }


                    #endregion

                    #region Beta Results

                    /*Main Beta_Beta*/
                    if (iType == FormQC.TypeOfQC.Beta)
                    {
                        Chart.Series["Beta Source Beta"].Points.AddXY(i.GetDateTimeCompleted().ToOADate(), i.GetNetBetaCPM());
                        Beta_Beta_Nodes.Add(i.GetNetBetaCPM());
                    }
                }
                #endregion
                #endregion

            }

            QC_List.UnLock();

            if (DateTime.Compare(Earliest, Latest) == 0)
            {
                Earliest = Earliest.Subtract(new TimeSpan(5, 0, 0, 0));
            }

            /*Other statistics*/
            if (NodesToPlot.Count != 0)
            {
                #region Background
                #region BGAlpha


                double[] Statistics = Compute_Statistics(Background_Alpha_Nodes, Earliest, Latest, ListOfSources.Find(x => x.GetSerialNumber() == "Background"), RadionuclideFamily.RadiationType.Alpha);


                if (Statistics != null)
                {
                    Chart.Series["BG_Alpha_3sig-"].Points.AddXY(Earliest, Statistics[0]);
                    Chart.Series["BG_Alpha_3sig-"].Points.AddXY(Latest, Statistics[1]);


                    Chart.Series["BG_Alpha_3sig+"].Points.AddXY(Earliest, Statistics[8]);
                    Chart.Series["BG_Alpha_3sig+"].Points.AddXY(Latest, Statistics[9]);



                    Chart.Series["BG_Alpha_2sig-"].Points.AddXY(Earliest, Statistics[2]);
                    Chart.Series["BG_Alpha_2sig-"].Points.AddXY(Latest, Statistics[3]);


                    Chart.Series["BG_Alpha_2sig+"].Points.AddXY(Earliest, Statistics[6]);
                    Chart.Series["BG_Alpha_2sig+"].Points.AddXY(Latest, Statistics[7]);

                    Chart.Series["BG_Alpha_10p-"].Points.AddXY(Earliest, Statistics[10]);
                    Chart.Series["BG_Alpha_10p-"].Points.AddXY(Latest, Statistics[11]);

                    Chart.Series["BG_Alpha_10p+"].Points.AddXY(Earliest, Statistics[12]);
                    Chart.Series["BG_Alpha_10p+"].Points.AddXY(Latest, Statistics[13]);

                    Chart.Series["BG_Alpha_Avg"].Points.AddXY(Earliest, Statistics[4]);
                    Chart.Series["BG_Alpha_Avg"].Points.AddXY(Latest, Statistics[5]);
                }
                #endregion

                #region BGBeta

                Statistics = Compute_Statistics(Background_Beta_Nodes, Earliest, Latest, ListOfSources.Find(x => x.GetSerialNumber() == "Background"), RadionuclideFamily.RadiationType.Beta);

                if (Statistics != null)
                {
                    Chart.Series["BG_Beta_3sig-"].Points.AddXY(Earliest, Statistics[0]);
                    Chart.Series["BG_Beta_3sig-"].Points.AddXY(Latest, Statistics[1]);


                    Chart.Series["BG_Beta_3sig+"].Points.AddXY(Earliest, Statistics[8]);
                    Chart.Series["BG_Beta_3sig+"].Points.AddXY(Latest, Statistics[9]);



                    Chart.Series["BG_Beta_2sig-"].Points.AddXY(Earliest, Statistics[2]);
                    Chart.Series["BG_Beta_2sig-"].Points.AddXY(Latest, Statistics[3]);


                    Chart.Series["BG_Beta_2sig+"].Points.AddXY(Earliest, Statistics[6]);
                    Chart.Series["BG_Beta_2sig+"].Points.AddXY(Latest, Statistics[7]);

                    Chart.Series["BG_Beta_10p-"].Points.AddXY(Earliest, Statistics[10]);
                    Chart.Series["BG_Beta_10p-"].Points.AddXY(Latest, Statistics[11]);

                    Chart.Series["BG_Beta_10p+"].Points.AddXY(Earliest, Statistics[12]);
                    Chart.Series["BG_Beta_10p+"].Points.AddXY(Latest, Statistics[13]);

                    Chart.Series["BG_Beta_Avg"].Points.AddXY(Earliest, Statistics[4]);
                    Chart.Series["BG_Beta_Avg"].Points.AddXY(Latest, Statistics[5]);
                }
                #endregion
                #endregion

                #region Alpha

                #region AlphaResults

                Statistics = Compute_Statistics(Alpha_Alpha_Nodes, Earliest, Latest, ListOfSources.Find(x => x.GetSerialNumber() == "Am-241"), RadionuclideFamily.RadiationType.Alpha);


                if (Statistics != null)
                {
                    Chart.Series["Alpha_Alpha_3sig-"].Points.AddXY(Earliest, Statistics[0]);
                    Chart.Series["Alpha_Alpha_3sig-"].Points.AddXY(Latest, Statistics[1]);


                    Chart.Series["Alpha_Alpha_3sig+"].Points.AddXY(Earliest, Statistics[8]);
                    Chart.Series["Alpha_Alpha_3sig+"].Points.AddXY(Latest, Statistics[9]);



                    Chart.Series["Alpha_Alpha_2sig-"].Points.AddXY(Earliest, Statistics[2]);
                    Chart.Series["Alpha_Alpha_2sig-"].Points.AddXY(Latest, Statistics[3]);


                    Chart.Series["Alpha_Alpha_2sig+"].Points.AddXY(Earliest, Statistics[6]);
                    Chart.Series["Alpha_Alpha_2sig+"].Points.AddXY(Latest, Statistics[7]);

                    Chart.Series["Alpha_Alpha_10p-"].Points.AddXY(Earliest, Statistics[10]);
                    Chart.Series["Alpha_Alpha_10p-"].Points.AddXY(Latest, Statistics[11]);

                    Chart.Series["Alpha_Alpha_10p+"].Points.AddXY(Earliest, Statistics[12]);
                    Chart.Series["Alpha_Alpha_10p+"].Points.AddXY(Latest, Statistics[13]);

                    Chart.Series["Alpha_Alpha_Avg"].Points.AddXY(Earliest, Statistics[4]);
                    Chart.Series["Alpha_Alpha_Avg"].Points.AddXY(Latest, Statistics[5]);
                }
                #endregion

                #region BetaResults
                Statistics = Compute_Statistics(Alpha_Beta_Nodes, Earliest, Latest, ListOfSources.Find(x => x.GetSerialNumber() == "Am-241"), RadionuclideFamily.RadiationType.Beta);

                if (Statistics != null)
                {
                    Chart.Series["Alpha_Beta_3sig-"].Points.AddXY(Earliest, Statistics[0]);
                    Chart.Series["Alpha_Beta_3sig-"].Points.AddXY(Latest, Statistics[1]);


                    Chart.Series["Alpha_Beta_3sig+"].Points.AddXY(Earliest, Statistics[8]);
                    Chart.Series["Alpha_Beta_3sig+"].Points.AddXY(Latest, Statistics[9]);



                    Chart.Series["Alpha_Beta_2sig-"].Points.AddXY(Earliest, Statistics[2]);
                    Chart.Series["Alpha_Beta_2sig-"].Points.AddXY(Latest, Statistics[3]);


                    Chart.Series["Alpha_Beta_2sig+"].Points.AddXY(Earliest, Statistics[6]);
                    Chart.Series["Alpha_Beta_2sig+"].Points.AddXY(Latest, Statistics[7]);

                    Chart.Series["Alpha_Beta_10p-"].Points.AddXY(Earliest, Statistics[10]);
                    Chart.Series["Alpha_Beta_10p-"].Points.AddXY(Latest, Statistics[11]);

                    Chart.Series["Alpha_Beta_10p+"].Points.AddXY(Earliest, Statistics[12]);
                    Chart.Series["Alpha_Beta_10p+"].Points.AddXY(Latest, Statistics[13]);

                    Chart.Series["Alpha_Beta_Avg"].Points.AddXY(Earliest, Statistics[4]);
                    Chart.Series["Alpha_Beta_Avg"].Points.AddXY(Latest, Statistics[5]);
                }
                #endregion
                #endregion

                #region Beta

                #region AlphaResults

                Statistics = Compute_Statistics(Beta_Alpha_Nodes, Earliest, Latest, ListOfSources.Find(x => x.GetSerialNumber() == "Sr-90"), RadionuclideFamily.RadiationType.Alpha);

                if (Statistics != null)
                {
                    Chart.Series["Beta_Alpha_3sig-"].Points.AddXY(Earliest, Statistics[0]);
                    Chart.Series["Beta_Alpha_3sig-"].Points.AddXY(Latest, Statistics[1]);


                    Chart.Series["Beta_Alpha_3sig+"].Points.AddXY(Earliest, Statistics[8]);
                    Chart.Series["Beta_Alpha_3sig+"].Points.AddXY(Latest, Statistics[9]);

                    Chart.Series["Beta_Alpha_2sig-"].Points.AddXY(Earliest, Statistics[2]);
                    Chart.Series["Beta_Alpha_2sig-"].Points.AddXY(Latest, Statistics[3]);


                    Chart.Series["Beta_Alpha_2sig+"].Points.AddXY(Earliest, Statistics[6]);
                    Chart.Series["Beta_Alpha_2sig+"].Points.AddXY(Latest, Statistics[7]);

                    Chart.Series["Beta_Alpha_10p-"].Points.AddXY(Earliest, Statistics[10]);
                    Chart.Series["Beta_Alpha_10p-"].Points.AddXY(Latest, Statistics[11]);

                    Chart.Series["Beta_Alpha_10p+"].Points.AddXY(Earliest, Statistics[12]);
                    Chart.Series["Beta_Alpha_10p+"].Points.AddXY(Latest, Statistics[13]);

                    Chart.Series["Beta_Alpha_Avg"].Points.AddXY(Earliest, Statistics[4]);
                    Chart.Series["Beta_Alpha_Avg"].Points.AddXY(Latest, Statistics[5]);
                }
                #endregion

                #region BetaResults
                Statistics = Compute_Statistics(Beta_Beta_Nodes, Earliest, Latest, ListOfSources.Find(x => x.GetSerialNumber() == "Sr-90"), RadionuclideFamily.RadiationType.Beta);

                if (Statistics != null)
                {
                    Chart.Series["Beta_Beta_3sig-"].Points.AddXY(Earliest, Statistics[0]);
                    Chart.Series["Beta_Beta_3sig-"].Points.AddXY(Latest, Statistics[1]);


                    Chart.Series["Beta_Beta_3sig+"].Points.AddXY(Earliest, Statistics[8]);
                    Chart.Series["Beta_Beta_3sig+"].Points.AddXY(Latest, Statistics[9]);



                    Chart.Series["Beta_Beta_2sig-"].Points.AddXY(Earliest, Statistics[2]);
                    Chart.Series["Beta_Beta_2sig-"].Points.AddXY(Latest, Statistics[3]);


                    Chart.Series["Beta_Beta_2sig+"].Points.AddXY(Earliest, Statistics[6]);
                    Chart.Series["Beta_Beta_2sig+"].Points.AddXY(Latest, Statistics[7]);

                    Chart.Series["Beta_Beta_10p-"].Points.AddXY(Earliest, Statistics[10]);
                    Chart.Series["Beta_Beta_10p-"].Points.AddXY(Latest, Statistics[11]);

                    Chart.Series["Beta_Beta_10p+"].Points.AddXY(Earliest, Statistics[12]);
                    Chart.Series["Beta_Beta_10p+"].Points.AddXY(Latest, Statistics[13]);

                    Chart.Series["Beta_Beta_Avg"].Points.AddXY(Earliest, Statistics[4]);
                    Chart.Series["Beta_Beta_Avg"].Points.AddXY(Latest, Statistics[5]);
                }
                #endregion
                #endregion

            }
            ScaleGraph();

            Chart.Invalidate();

            return;
        }

        #endregion

        #region Private Utility Functions
        /*Compute std.dev and average*/
        /*Returns:
         * <-3 sig_0, -3 sig_1, -2 sig_0, -2 sig_1, average_0, average_1, +2 sig_0, +2sig_1, +3 sig_0, +3sig_1, -10%_0, -10%_1, +10%_0, +10%_1>
         */
        private double[] Compute_Statistics(List<double> InData, DateTime Earliest, DateTime Latest, Radioactive_Source R, RadionuclideFamily.RadiationType TypeToCompute)
        {
            /*Decay corrected*/
            if ((this.CorrectForDecayCheckBox.Checked) && (R.GetSerialNumber() != "Background"))
            {
                double EarlyCenterLine = 0;
                double LateCenterLine = 0;
                double CalibratedDisintigrationFactor = 0;

                if (TypeToCompute == RadionuclideFamily.RadiationType.Alpha)
                {
                    CalibratedDisintigrationFactor = R.GetCertifiedActivity() * R.GetAlphaEfficiency() / 100;
                }
                else
                {
                    CalibratedDisintigrationFactor = R.GetCertifiedActivity() * R.GetBetaEfficiency() / 100;
                }

                EarlyCenterLine = R.GetDecayFactor(Earliest) * CalibratedDisintigrationFactor;
                LateCenterLine = R.GetDecayFactor(Latest) * CalibratedDisintigrationFactor ;

                double Sigma_Early = 0;
                double Sigma_Late = 0;

                /*For gaussian statistics*/
                if (EarlyCenterLine> 20)
                {
                    foreach (double i in InData)
                    {
                        Sigma_Early += (i - EarlyCenterLine) * (i - EarlyCenterLine);
                    }

                    if (InData.Count > 1)
                    {
                        Sigma_Early /= (InData.Count - 1);
                    }

                    Sigma_Early = Math.Sqrt(Sigma_Early);

                    /*Add in DOF correction*/
                    double n = Convert.ToDouble(InData.Count);
                    Sigma_Early /= Math.Sqrt(2 / (n - 1)) * (Gamma((n / 2)) / (Gamma((n - 1) / 2)));

                }

                /*For Poisson statistics*/
                else
                {
                    Sigma_Early = Math.Sqrt(EarlyCenterLine);
                }

                if (LateCenterLine > 20)
                {
                    foreach (double i in InData)
                    {
                        Sigma_Late += (i - LateCenterLine) * (i - LateCenterLine);
                    }

                    if (InData.Count > 1)
                    {
                        Sigma_Late /= (InData.Count - 1);
                    }

                    Sigma_Late = Math.Sqrt(Sigma_Late);

                    /*Add in DOF correction*/
                    double n = Convert.ToDouble(InData.Count);
                    Sigma_Late /= Math.Sqrt(2 / (n - 1)) * (Gamma((n / 2)) / (Gamma((n - 1) / 2)));

                }

                /*For Poisson statistics*/
                else
                {
                    Sigma_Late = Math.Sqrt(LateCenterLine);
                }

                //double Sigma_Early = Math.Sqrt(EarlyCenterLine);
                //double Sigma_Late = Math.Sqrt(LateCenterLine);

                /*Construct Return Matrix*/
                double[] ReturnData = new double[14];
                
                ReturnData[0] = EarlyCenterLine - (3 * Sigma_Early);
                ReturnData[1] = LateCenterLine - (3 * Sigma_Late);
                ReturnData[2] = EarlyCenterLine - (2 * Sigma_Early);
                ReturnData[3] = LateCenterLine - (2 * Sigma_Late);
                ReturnData[4] = EarlyCenterLine;
                ReturnData[5] = LateCenterLine;
                ReturnData[6] = EarlyCenterLine + (2 * Sigma_Early);
                ReturnData[7] = LateCenterLine + (2 * Sigma_Late);
                ReturnData[8] = EarlyCenterLine + (3 * Sigma_Early);
                ReturnData[9] = LateCenterLine + (3 * Sigma_Late);
                ReturnData[10] = (9 * EarlyCenterLine) / 10;
                ReturnData[11] = (9 * LateCenterLine) / 10;
                ReturnData[12] = (11 * EarlyCenterLine) / 10;
                ReturnData[13] = (11 * LateCenterLine) / 10;

                return ReturnData;
                
            }
            
            /*Standard*/
            else
            {
                if (InData.Count == 0)
                {
                    return null;
                }

                double Sigma = 0;
                double Average = 0;

                foreach (double i in InData)
                {
                    Average += i;

                }

                Average /= InData.Count;

                /*For gaussian statistics*/
                if (Average > 20)
                {
                    foreach (double i in InData)
                    {
                        Sigma += (i - Average) * (i - Average);
                    }

                    if (InData.Count > 1)
                    {
                        Sigma /= (InData.Count - 1);
                    }

                    Sigma = Math.Sqrt(Sigma);

                    /*Add in DOF correction*/
                    double n = Convert.ToDouble(InData.Count);
                    Sigma /= Math.Sqrt(2 / (n - 1)) * (Gamma((n / 2)) / (Gamma((n - 1) / 2)));

                }

                /*For Poisson statistics*/
                else
                {
                    Sigma = Math.Sqrt(Average);
                }

                double[] ReturnData = new double[14];

                ReturnData[0] = Average - (3 * Sigma);
                ReturnData[1] = ReturnData[0];
                ReturnData[2] = Average - (2 * Sigma);
                ReturnData[3] = ReturnData[2];
                ReturnData[4] = Average;
                ReturnData[5] = Average;
                ReturnData[6] = Average + (2 * Sigma);
                ReturnData[7] = ReturnData[6];
                ReturnData[8] = Average + (3 * Sigma);
                ReturnData[9] = ReturnData[8];
                ReturnData[10] = (9 * Average) / 10;
                ReturnData[11] = ReturnData[10];
                ReturnData[12] = (11 * Average) / 10;
                ReturnData[13] = ReturnData[12];
                
                return ReturnData;
            }

            
        }

        /*This came from AlgLib*/
        private double Gamma(double x)
        {
            double result = 0;
            double p = 0;
            double pp = 0;
            double q = 0;
            double qq = 0;
            double z = 0;
            int i = 0;
            double sgngam = 0;

            sgngam = 1;
            q = Math.Abs(x);
            if ((double)(q) > (double)(33.0))
            {
                if ((double)(x) < (double)(0.0))
                {
                    p = (int)Math.Floor(q);
                    i = (int)Math.Round(p);
                    if (i % 2 == 0)
                    {
                        sgngam = -1;
                    }
                    z = q - p;
                    if ((double)(z) > (double)(0.5))
                    {
                        p = p + 1;
                        z = q - p;
                    }
                    z = q * Math.Sin(Math.PI * z);
                    z = Math.Abs(z);
                    z = Math.PI / (z * gammastirf(q));
                }
                else
                {
                    z = gammastirf(x);
                }
                result = sgngam * z;
                return result;
            }
            z = 1;
            while ((double)(x) >= (double)(3))
            {
                x = x - 1;
                z = z * x;
            }
            while ((double)(x) < (double)(0))
            {
                if ((double)(x) > (double)(-0.000000001))
                {
                    result = z / ((1 + 0.5772156649015329 * x) * x);
                    return result;
                }
                z = z / x;
                x = x + 1;
            }
            while ((double)(x) < (double)(2))
            {
                if ((double)(x) < (double)(0.000000001))
                {
                    result = z / ((1 + 0.5772156649015329 * x) * x);
                    return result;
                }
                z = z / x;
                x = x + 1.0;
            }
            if ((double)(x) == (double)(2))
            {
                result = z;
                return result;
            }
            x = x - 2.0;
            pp = 1.60119522476751861407E-4;
            pp = 1.19135147006586384913E-3 + x * pp;
            pp = 1.04213797561761569935E-2 + x * pp;
            pp = 4.76367800457137231464E-2 + x * pp;
            pp = 2.07448227648435975150E-1 + x * pp;
            pp = 4.94214826801497100753E-1 + x * pp;
            pp = 9.99999999999999996796E-1 + x * pp;
            qq = -2.31581873324120129819E-5;
            qq = 5.39605580493303397842E-4 + x * qq;
            qq = -4.45641913851797240494E-3 + x * qq;
            qq = 1.18139785222060435552E-2 + x * qq;
            qq = 3.58236398605498653373E-2 + x * qq;
            qq = -2.34591795718243348568E-1 + x * qq;
            qq = 7.14304917030273074085E-2 + x * qq;
            qq = 1.00000000000000000320 + x * qq;
            result = z * pp / qq;
            return result;
        }

        /*This came from AlgLib*/
        private double gammastirf(double x)
        {
            double result = 0;
            double y = 0;
            double w = 0;
            double v = 0;
            double stir = 0;

            w = 1 / x;
            stir = 7.87311395793093628397E-4;
            stir = -2.29549961613378126380E-4 + w * stir;
            stir = -2.68132617805781232825E-3 + w * stir;
            stir = 3.47222221605458667310E-3 + w * stir;
            stir = 8.33333333333482257126E-2 + w * stir;
            w = 1 + w * stir;
            y = Math.Exp(x);
            if ((double)(x) > (double)(143.01608))
            {
                v = Math.Pow(x, 0.5 * x - 0.25);
                y = v * (v / y);
            }
            else
            {
                y = Math.Pow(x, x - 0.5) / y;
            }
            result = 2.50662827463100050242 * y * w;
            return result;
        }

        private bool IsBefore(DateTime Benchmark, DateTime Challenger)
        {
            return ( (DateTime.Compare(Benchmark, Challenger) > 0) ? true : false);
        }

        private bool IsAfter(DateTime Benchmark, DateTime Challenger)
        {
            return ((DateTime.Compare(Benchmark, Challenger) < 0) ? true : false);
        }

        private void ScaleGraph()
        {
            double MaxY = 0;
            double MinY = 9999999999;

            foreach (Series s in Chart.Series)
            {
                if (s.Enabled)
                {
                    for (int i = 0; i < s.Points.Count; i++)
                    {
                        if (MaxY < s.Points[i].YValues[0])
                        {
                            MaxY = s.Points[i].YValues[0];
                        }

                        if (MinY > s.Points[i].YValues[0])
                        {
                            MinY = s.Points[i].YValues[0];
                        }
                    }
                }
            }

            Chart.ChartAreas[0].AxisY.Maximum = MaxY + 10;
            Chart.ChartAreas[0].AxisY.Minimum = MinY - 10;
            return;

        }

        

        private void AbortAll()
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
            return;
        }

        #endregion

        #region Background Checkbox Handlers

        #region Background Alpha CB Handlers
        private void BG_Alpha_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BG_Alpha_CheckBox.Checked)
            {
                /*Enable minor checkboxes*/
                this.BG_Alpha_2Sig_CB.Enabled = true;
                this.BG_Alpha_3Sig_CB.Enabled = true;
                this.BG_Alpha_Avg_CB.Enabled = true;
                this.BG_Alpha_10p_CB.Enabled = true;

                /*Enable chart*/
                Chart.Series["Background Alpha"].Enabled = true;

            }

            else
            {
                /*Disable minor checkboxes*/
                this.BG_Alpha_2Sig_CB.Enabled = false;
                this.BG_Alpha_3Sig_CB.Enabled = false;
                this.BG_Alpha_Avg_CB.Enabled = false;
                this.BG_Alpha_10p_CB.Enabled = false;

                /*Enable chart*/
                Chart.Series["Background Alpha"].Enabled = false;

            }

            BG_Alpha_2Sig_CB_CheckedChanged(this, null);
            BG_Alpha_3Sig_CB_CheckedChanged(this, null);
            BG_Alpha_Avg_CB_CheckedChanged(this, null);
            BG_Alpha_10p_CB_CheckedChanged(this, null);

            return;
        }

        private void BG_Alpha_2Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["BG_Alpha_2sig+"].Enabled = (this.BG_Alpha_2Sig_CB.Enabled && this.BG_Alpha_2Sig_CB.Checked);
            Chart.Series["BG_Alpha_2sig-"].Enabled = (this.BG_Alpha_2Sig_CB.Enabled && this.BG_Alpha_2Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void BG_Alpha_3Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["BG_Alpha_3sig+"].Enabled = (this.BG_Alpha_3Sig_CB.Enabled && this.BG_Alpha_3Sig_CB.Checked);
            Chart.Series["BG_Alpha_3sig-"].Enabled = (this.BG_Alpha_3Sig_CB.Enabled && this.BG_Alpha_3Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void BG_Alpha_Avg_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["BG_Alpha_Avg"].Enabled = (this.BG_Alpha_Avg_CB.Enabled && this.BG_Alpha_Avg_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void BG_Alpha_10p_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["BG_Alpha_10p-"].Enabled = (this.BG_Alpha_10p_CB.Checked && this.BG_Alpha_10p_CB.Enabled);
            Chart.Series["BG_Alpha_10p+"].Enabled = (this.BG_Alpha_10p_CB.Checked && this.BG_Alpha_10p_CB.Enabled);
            ScaleGraph();
            Chart.Invalidate();
        }
        #endregion

        #region Background Beta CB Handlers
        private void BG_Beta_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.BG_Beta_CheckBox.Checked)
            {
                /*Enable minor checkboxes*/
                this.BG_Beta_2Sig_CB.Enabled = true;
                this.BG_Beta_3Sig_CB.Enabled = true;
                this.BG_Beta_Avg_CB.Enabled = true;
                this.BG_Beta_10p_CB.Enabled = true;

                /*Disable chart*/
                Chart.Series["Background Beta"].Enabled = true;

            }

            else
            {
                /*Disable minor checkboxes*/
                this.BG_Beta_2Sig_CB.Enabled = false;
                this.BG_Beta_3Sig_CB.Enabled = false;
                this.BG_Beta_Avg_CB.Enabled = false;
                this.BG_Beta_10p_CB.Enabled = false;

                /*Enable chart*/
                Chart.Series["Background Beta"].Enabled = false;

            }

            BG_Beta_2Sig_CB_CheckedChanged(this, null);
            BG_Beta_3Sig_CB_CheckedChanged(this, null);
            BG_Beta_Avg_CB_CheckedChanged(this, null);
            BG_Beta_10p_CB_CheckedChanged(this, null);

            return;
        }

        private void BG_Beta_2Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["BG_Beta_2sig+"].Enabled = (this.BG_Beta_2Sig_CB.Enabled && this.BG_Beta_2Sig_CB.Checked);
            Chart.Series["BG_Beta_2sig-"].Enabled = (this.BG_Beta_2Sig_CB.Enabled && this.BG_Beta_2Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void BG_Beta_3Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["BG_Beta_3sig+"].Enabled = (this.BG_Beta_3Sig_CB.Enabled && this.BG_Beta_3Sig_CB.Checked);
            Chart.Series["BG_Beta_3sig-"].Enabled = (this.BG_Beta_3Sig_CB.Enabled && this.BG_Beta_3Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void BG_Beta_Avg_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["BG_Beta_Avg"].Enabled = (this.BG_Beta_Avg_CB.Enabled && this.BG_Beta_Avg_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void BG_Beta_10p_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["BG_Beta_10p-"].Enabled = (this.BG_Beta_10p_CB.Checked && this.BG_Beta_10p_CB.Enabled);
            Chart.Series["BG_Beta_10p+"].Enabled = (this.BG_Beta_10p_CB.Checked && this.BG_Beta_10p_CB.Enabled);
            ScaleGraph();
            Chart.Invalidate();
        }
        #endregion

        #endregion

        #region Alpha Checkbox Handlers

        #region Alpha Results Handler
        private void Alpha_Alpha_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Alpha_Alpha_CheckBox.Checked)
            {
                /*Enable minor checkboxes*/
                this.Alpha_Alpha_2Sig_CB.Enabled = true;
                this.Alpha_Alpha_3Sig_CB.Enabled = true;
                this.Alpha_Alpha_Avg_CB.Enabled = true;
                this.Alpha_Alpha_10p_CB.Enabled = true;

                /*Enable chart*/
                Chart.Series["Alpha Source Alpha"].Enabled = true;

            }

            else
            {
                /*Disable minor checkboxes*/
                this.Alpha_Alpha_2Sig_CB.Enabled = false;
                this.Alpha_Alpha_3Sig_CB.Enabled = false;
                this.Alpha_Alpha_Avg_CB.Enabled = false;
                this.Alpha_Alpha_10p_CB.Enabled = false;

                /*Enable chart*/
                Chart.Series["Alpha Source Alpha"].Enabled = false;

            }

            /*Flush checkboxes*/
            Alpha_Alpha_2Sig_CB_CheckedChanged(this, null);
            Alpha_Alpha_3Sig_CB_CheckedChanged(this, null);
            Alpha_Alpha_Avg_CB_CheckedChanged(this, null);
            Alpha_Alpha_10p_CB_CheckedChanged(this, null);

            return;
        }

        

        private void Alpha_Alpha_2Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Alpha_Alpha_2sig-"].Enabled = (this.Alpha_Alpha_2Sig_CB.Enabled && this.Alpha_Alpha_2Sig_CB.Checked);
            Chart.Series["Alpha_Alpha_2sig+"].Enabled = (this.Alpha_Alpha_2Sig_CB.Enabled && this.Alpha_Alpha_2Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Alpha_Alpha_3Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Alpha_Alpha_3sig-"].Enabled = (this.Alpha_Alpha_3Sig_CB.Enabled && this.Alpha_Alpha_3Sig_CB.Checked);
            Chart.Series["Alpha_Alpha_3sig+"].Enabled = (this.Alpha_Alpha_3Sig_CB.Enabled && this.Alpha_Alpha_3Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Alpha_Alpha_Avg_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Alpha_Alpha_Avg"].Enabled = (this.Alpha_Alpha_Avg_CB.Enabled && this.Alpha_Alpha_Avg_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Alpha_Alpha_10p_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Alpha_Alpha_10p-"].Enabled = (this.Alpha_Alpha_10p_CB.Checked && this.Alpha_Alpha_10p_CB.Enabled);
            Chart.Series["Alpha_Alpha_10p+"].Enabled = (this.Alpha_Alpha_10p_CB.Checked && this.Alpha_Alpha_10p_CB.Enabled);
            ScaleGraph();
            Chart.Invalidate();
        }
        #endregion

        #region Beta Results Handler
        private void Alpha_Beta_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Alpha_Beta_CheckBox.Checked)
            {
                /*Enable minor checkboxes*/
                this.Alpha_Beta_2Sig_CB.Enabled = true;
                this.Alpha_Beta_3Sig_CB.Enabled = true;
                this.Alpha_Beta_Avg_CB.Enabled = true;
                this.Alpha_Beta_10p_CB.Enabled = true;

                /*Enable chart*/
                Chart.Series["Alpha Source Beta"].Enabled = true;

            }

            else
            {
                /*Disable minor checkboxes*/
                this.Alpha_Beta_2Sig_CB.Enabled = false;
                this.Alpha_Beta_3Sig_CB.Enabled = false;
                this.Alpha_Beta_Avg_CB.Enabled = false;
                this.Alpha_Beta_10p_CB.Enabled = false;

                /*Enable chart*/
                Chart.Series["Alpha Source Beta"].Enabled = false;

            }

            /*Flush checkboxes*/
            Alpha_Beta_2Sig_CB_CheckedChanged(this, null);
            Alpha_Beta_3Sig_CB_CheckedChanged(this, null);
            Alpha_Beta_Avg_CB_CheckedChanged(this, null);
            Alpha_Beta_10p_CB_CheckedChanged(this, null);

            return;
        }

        private void Alpha_Beta_2Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Alpha_Beta_2sig-"].Enabled = (this.Alpha_Beta_2Sig_CB.Enabled && this.Alpha_Beta_2Sig_CB.Checked);
            Chart.Series["Alpha_Beta_2sig+"].Enabled = (this.Alpha_Beta_2Sig_CB.Enabled && this.Alpha_Beta_2Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Alpha_Beta_3Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Alpha_Beta_3sig-"].Enabled = (this.Alpha_Beta_3Sig_CB.Enabled && this.Alpha_Beta_3Sig_CB.Checked);
            Chart.Series["Alpha_Beta_3sig+"].Enabled = (this.Alpha_Beta_3Sig_CB.Enabled && this.Alpha_Beta_3Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Alpha_Beta_Avg_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Alpha_Beta_Avg"].Enabled = (this.Alpha_Beta_Avg_CB.Enabled && this.Alpha_Beta_Avg_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Alpha_Beta_10p_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Alpha_Beta_10p-"].Enabled = (this.Alpha_Beta_10p_CB.Checked && this.Alpha_Beta_10p_CB.Enabled);
            Chart.Series["Alpha_Beta_10p+"].Enabled = (this.Alpha_Beta_10p_CB.Checked && this.Alpha_Beta_10p_CB.Enabled);
            ScaleGraph();
            Chart.Invalidate();
        }

        #endregion

        #endregion

        #region Beta Checkbox Handlers

        #region Alpha
        private void Beta_Alpha_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Beta_Alpha_CheckBox.Checked)
            {
                /*Enable minor checkboxes*/
                this.Beta_Alpha_2Sig_CB.Enabled = true;
                this.Beta_Alpha_3Sig_CB.Enabled = true;
                this.Beta_Alpha_Avg_CB.Enabled = true;
                this.Beta_Alpha_10p_CB.Enabled = true;

                /*Enable chart*/
                Chart.Series["Beta Source Alpha"].Enabled = true;

            }

            else
            {
                /*Disable minor checkboxes*/
                this.Beta_Alpha_2Sig_CB.Enabled = false;
                this.Beta_Alpha_3Sig_CB.Enabled = false;
                this.Beta_Alpha_Avg_CB.Enabled = false;
                this.Beta_Alpha_10p_CB.Enabled = false;

                /*Enable chart*/
                Chart.Series["Beta Source Alpha"].Enabled = false;

            }

            /*Flush checkboxes*/
            Beta_Alpha_2Sig_CB_CheckedChanged(this, null);
            Beta_Alpha_3Sig_CB_CheckedChanged(this, null);
            Beta_Alpha_Avg_CB_CheckedChanged(this, null);
            Beta_Alpha_10p_CB_CheckedChanged(this, null);

            return;
        }


        private void Beta_Alpha_2Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Beta_Alpha_2sig-"].Enabled = (this.Beta_Alpha_2Sig_CB.Enabled && this.Beta_Alpha_2Sig_CB.Checked);
            Chart.Series["Beta_Alpha_2sig+"].Enabled = (this.Beta_Alpha_2Sig_CB.Enabled && this.Beta_Alpha_2Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Beta_Alpha_3Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Beta_Alpha_3sig-"].Enabled = (this.Beta_Alpha_3Sig_CB.Enabled && this.Beta_Alpha_3Sig_CB.Checked);
            Chart.Series["Beta_Alpha_3sig+"].Enabled = (this.Beta_Alpha_3Sig_CB.Enabled && this.Beta_Alpha_3Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Beta_Alpha_Avg_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Beta_Alpha_Avg"].Enabled = (this.Beta_Alpha_Avg_CB.Enabled && this.Beta_Alpha_Avg_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Beta_Alpha_10p_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Beta_Alpha_10p-"].Enabled = (this.Beta_Alpha_10p_CB.Checked && this.Beta_Alpha_10p_CB.Enabled);
            Chart.Series["Beta_Alpha_10p+"].Enabled = (this.Beta_Alpha_10p_CB.Checked && this.Beta_Alpha_10p_CB.Enabled);
            ScaleGraph();
            Chart.Invalidate();
        }
        #endregion

        #region Beta
        private void Beta_Beta_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Beta_Beta_CheckBox.Checked)
            {
                /*Enable minor checkboxes*/
                this.Beta_Beta_2Sig_CB.Enabled = true;
                this.Beta_Beta_3Sig_CB.Enabled = true;
                this.Beta_Beta_Avg_CB.Enabled = true;
                this.Beta_Beta_10p_CB.Enabled = true;

                /*Enable chart*/
                Chart.Series["Beta Source Beta"].Enabled = true;

            }

            else
            {
                /*Disable minor checkboxes*/
                this.Beta_Beta_2Sig_CB.Enabled = false;
                this.Beta_Beta_3Sig_CB.Enabled = false;
                this.Beta_Beta_Avg_CB.Enabled = false;
                this.Beta_Beta_10p_CB.Enabled = false;

                /*Enable chart*/
                Chart.Series["Beta Source Beta"].Enabled = false;

            }

            /*Flush checkboxes*/
            Beta_Beta_2Sig_CB_CheckedChanged(this, null);
            Beta_Beta_3Sig_CB_CheckedChanged(this, null);
            Beta_Beta_Avg_CB_CheckedChanged(this, null);
            Beta_Beta_10p_CB_CheckedChanged(this, null);

            return;
        }

        private void Beta_Beta_2Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Beta_Beta_2sig-"].Enabled = (this.Beta_Beta_2Sig_CB.Enabled && this.Beta_Beta_2Sig_CB.Checked);
            Chart.Series["Beta_Beta_2sig+"].Enabled = (this.Beta_Beta_2Sig_CB.Enabled && this.Beta_Beta_2Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Beta_Beta_3Sig_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Beta_Beta_3sig-"].Enabled = (this.Beta_Beta_3Sig_CB.Enabled && this.Beta_Beta_3Sig_CB.Checked);
            Chart.Series["Beta_Beta_3sig+"].Enabled = (this.Beta_Beta_3Sig_CB.Enabled && this.Beta_Beta_3Sig_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Beta_Beta_Avg_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Beta_Beta_Avg"].Enabled = (this.Beta_Beta_Avg_CB.Enabled && this.Beta_Beta_Avg_CB.Checked);
            ScaleGraph();
            Chart.Invalidate();
            return;
        }

        private void Beta_Beta_10p_CB_CheckedChanged(object sender, EventArgs e)
        {
            Chart.Series["Beta_Beta_10p-"].Enabled = (this.Beta_Beta_10p_CB.Checked && this.Beta_Beta_10p_CB.Enabled);
            Chart.Series["Beta_Beta_10p+"].Enabled = (this.Beta_Beta_10p_CB.Checked && this.Beta_Beta_10p_CB.Enabled);
            ScaleGraph();
            Chart.Invalidate();
        }
        #endregion

        #endregion

        #region Show SuppressedNodes Handler
        private void showSuppressedNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SuppressedNodeViewer NewForm = new SuppressedNodeViewer(this.QC_List);
            if (NewForm.ShowDialog() != DialogResult.Abort)
            {
                this.QC_List = NewForm.GetListKeeper();

                RedrawGraph();
            }

            else
            {
                AbortAll();
            }

        }
        #endregion

        #region Correct For Decay Handler
        private void CorrectForDecayCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            RedrawGraph();
            Chart.Invalidate();
        }
        #endregion

        /*TODO: Refactor this to remove some of the for loops?*/
        #region Click Handler
        private void Chart_MouseClick(object sender, MouseEventArgs e)
        {
            /*Get position of the click*/
            Point pos = e.Location;

            /*Iterate over all of the points, seeing if we clicked on something*/
            HitTestResult[] results = Chart.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint);
            foreach (HitTestResult result in results)
            {
                if (result.ChartElementType == ChartElementType.DataPoint)
                {
                    /*We clicked on something! Find the point.*/
                    DataPoint ClickedPoint = result.Object as DataPoint;

                    foreach (Series S in Chart.Series)
                    {
                        foreach (DataPoint P in S.Points)
                        {
                            if ((Math.Abs(ClickedPoint.XValue - P.XValue) < 10) && (Math.Abs(ClickedPoint.YValues[0] - P.YValues[0]) < 10))
                            {
                                /*Found point! Now, diplay it to the user*/
                                DateTime DateOfCurrentPoint = DateTime.FromOADate(P.XValue);
                                double YValue = P.YValues[0];

                                TimeSpan ONE_SECOND = new TimeSpan(0, 0, 1);

                                QC_List.Lock();
                                foreach (QCCalResultNode Q in QC_List.GetFullList())
                                {
                                    if ((Q.GetDateTimeCompleted().Subtract(DateOfCurrentPoint) < ONE_SECOND) && (DateOfCurrentPoint.Subtract(Q.GetDateTimeCompleted()) < ONE_SECOND))
                                    {
                                        switch (S.Name)
                                        {
                                            case "Background Alpha":
                                                if ((Q.GetTypeOfQC() == FormQC.TypeOfQC.Background) && (Q.GetNetAlphaCPM() == YValue))
                                                {
                                                    
                                                    if (MessageBox.Show(String.Format("Point generated on {0} by {1} with a badge number of {2}\nNet Alpha CPM = {3}\nNet Beta CPM = {4}\nThe type of test was {5}\nThe Comment left was \"{6}\"\n\nDeclare Point Unplottable?", Q.GetDateTimeCompleted(), Q.GetName(), Q.GetBadgeNo(), Q.GetNetAlphaCPM(), Q.GetNetBetaCPM(), Q.GetTypeOfQC(), Q.GetComment()), "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                    {
                                                        LongPrompt LP = new LongPrompt("Enter the reason for declaring the point unplottable.");
                                                        LP.ShowDialog();
                                                        if (LP.GetResponse() != "")
                                                        {
                                                            MessageBox.Show("Point is now unplottable.");
                                                            Q.SetComment(LP.GetResponse());
                                                            Q.SetPlottable(false);
                                                            RedrawGraph();
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Error: Reason cannot be null. Point will still be plotted.");
                                                        }
                                                    }
                                                    QC_List.UnLock();
                                                    return;
                                                }
                                                break;

                                            case "Background Beta":
                                                if ((Q.GetTypeOfQC() == FormQC.TypeOfQC.Background) && (Q.GetNetBetaCPM() == YValue))
                                                {

                                                    if (MessageBox.Show(String.Format("Point generated on {0} by {1} with a badge number of {2}\nNet Alpha CPM = {3}\nNet Beta CPM = {4}\nThe type of test was {5}\nThe Comment left was \"{6}\"\n\nDeclare Point Unplottable?", Q.GetDateTimeCompleted(), Q.GetName(), Q.GetBadgeNo(), Q.GetNetAlphaCPM(), Q.GetNetBetaCPM(), Q.GetTypeOfQC(), Q.GetComment()), "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                    {
                                                        LongPrompt LP = new LongPrompt("Enter the reason for declaring the point unplottable.");
                                                        LP.ShowDialog();
                                                        if (LP.GetResponse() != "")
                                                        {
                                                            MessageBox.Show("Point is now unplottable.");
                                                            Q.SetComment(LP.GetResponse());
                                                            Q.SetPlottable(false);
                                                            RedrawGraph();
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Error: Reason cannot be null. Point will still be plotted.");
                                                        }
                                                    }
                                                    QC_List.UnLock();
                                                    return;
                                                }
                                                break;

                                            case "Alpha Source Alpha":
                                                if ((Q.GetTypeOfQC() == FormQC.TypeOfQC.Alpha) && (Q.GetNetAlphaCPM() == YValue))
                                                {

                                                    if (MessageBox.Show(String.Format("Point generated on {0} by {1} with a badge number of {2}\nNet Alpha CPM = {3}\nNet Beta CPM = {4}\nThe type of test was {5}\nThe Comment left was \"{6}\"\n\nDeclare Point Unplottable?", Q.GetDateTimeCompleted(), Q.GetName(), Q.GetBadgeNo(), Q.GetNetAlphaCPM(), Q.GetNetBetaCPM(), Q.GetTypeOfQC(), Q.GetComment()), "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                    {
                                                        LongPrompt LP = new LongPrompt("Enter the reason for declaring the point unplottable.");
                                                        LP.ShowDialog();
                                                        if (LP.GetResponse() != "")
                                                        {
                                                            MessageBox.Show("Point is now unplottable.");
                                                            Q.SetComment(LP.GetResponse());
                                                            Q.SetPlottable(false);
                                                            RedrawGraph();
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Error: Reason cannot be null. Point will still be plotted.");
                                                        }
                                                    }
                                                    QC_List.UnLock();
                                                    return;
                                                }
                                                break;

                                            case "Alpha Source Beta":
                                                if ((Q.GetTypeOfQC() == FormQC.TypeOfQC.Alpha) && (Q.GetNetBetaCPM() == YValue))
                                                {

                                                    if (MessageBox.Show(String.Format("Point generated on {0} by {1} with a badge number of {2}\nNet Alpha CPM = {3}\nNet Beta CPM = {4}\nThe type of test was {5}\nThe Comment left was \"{6}\"\n\nDeclare Point Unplottable?", Q.GetDateTimeCompleted(), Q.GetName(), Q.GetBadgeNo(), Q.GetNetAlphaCPM(), Q.GetNetBetaCPM(), Q.GetTypeOfQC(), Q.GetComment()), "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                    {
                                                        LongPrompt LP = new LongPrompt("Enter the reason for declaring the point unplottable.");
                                                        LP.ShowDialog();
                                                        if (LP.GetResponse() != "")
                                                        {
                                                            MessageBox.Show("Point is now unplottable.");
                                                            Q.SetComment(LP.GetResponse());
                                                            Q.SetPlottable(false);
                                                            RedrawGraph();
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Error: Reason cannot be null. Point will still be plotted.");
                                                        }
                                                    }
                                                    QC_List.UnLock();
                                                    return;
                                                }
                                                break;

                                            case "Beta Source Alpha":
                                                if ((Q.GetTypeOfQC() == FormQC.TypeOfQC.Beta) && (Q.GetNetAlphaCPM() == YValue))
                                                {

                                                    if (MessageBox.Show(String.Format("Point generated on {0} by {1} with a badge number of {2}\nNet Alpha CPM = {3}\nNet Beta CPM = {4}\nThe type of test was {5}\nThe Comment left was \"{6}\"\n\nDeclare Point Unplottable?", Q.GetDateTimeCompleted(), Q.GetName(), Q.GetBadgeNo(), Q.GetNetAlphaCPM(), Q.GetNetBetaCPM(), Q.GetTypeOfQC(), Q.GetComment()), "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                    {
                                                        LongPrompt LP = new LongPrompt("Enter the reason for declaring the point unplottable.");
                                                        LP.ShowDialog();
                                                        if (LP.GetResponse() != "")
                                                        {
                                                            MessageBox.Show("Point is now unplottable.");
                                                            Q.SetComment(LP.GetResponse());
                                                            Q.SetPlottable(false);
                                                            RedrawGraph();
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Error: Reason cannot be null. Point will still be plotted.");
                                                        }
                                                    }
                                                    QC_List.UnLock();
                                                    return;
                                                }
                                                break;

                                            case "Beta Source Beta":
                                                if ((Q.GetTypeOfQC() == FormQC.TypeOfQC.Beta) && (Q.GetNetBetaCPM() == YValue))
                                                {

                                                    if (MessageBox.Show(String.Format("Point generated on {0} by {1} with a badge number of {2}\nNet Alpha CPM = {3}\nNet Beta CPM = {4}\nThe type of test was {5}\nThe Comment left was \"{6}\"\n\nDeclare Point Unplottable?", Q.GetDateTimeCompleted(), Q.GetName(), Q.GetBadgeNo(), Q.GetNetAlphaCPM(), Q.GetNetBetaCPM(), Q.GetTypeOfQC(), Q.GetComment()), "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                                    {
                                                        LongPrompt LP = new LongPrompt("Enter the reason for declaring the point unplottable.");
                                                        LP.ShowDialog();
                                                        if (LP.GetResponse() != "")
                                                        {
                                                            MessageBox.Show("Point is now unplottable.");
                                                            Q.SetComment(LP.GetResponse());
                                                            Q.SetPlottable(false);
                                                            RedrawGraph();
                                                        }
                                                        else
                                                        {
                                                            MessageBox.Show("Error: Reason cannot be null. Point will still be plotted.");
                                                        }
                                                    }
                                                    QC_List.UnLock();
                                                    return;
                                                }
                                                break;

                                            default:
                                                break;
                                        }
                                    }
                                }
                                QC_List.UnLock();
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Keypress Handler
        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                AbortAll();
                return;
            }
            return;
        }
        #endregion

        #region Control Checkbox Handlers
        private void LastCalButton_CheckedChanged(object sender, EventArgs e)
        {
            RedrawGraph();
        }

        private void AutoCalibration_CB_CheckedChanged(object sender, EventArgs e)
        {
            RedrawGraph();
        }
        #endregion

        #region Suppressed Node Click Handler
        private void ViewSuppressedNodeButton_Click(object sender, EventArgs e)
        {
            SuppressedNodeViewer S = new SuppressedNodeViewer(QC_List);
            S.ShowDialog();
            this.QC_List = S.GetListKeeper();
        }
        #endregion

        #region Finalization
        private void DailyQCGraph_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.LaunchedFrom.Show();
        }
        #endregion

    }
}
