using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing.Imaging;

namespace DABRAS_Software
{
    public partial class BetaEfficiencyChart : Form
    {
        #region Data Members
        private List<RadionuclideFamily> ListOfFamily;
        private List<Radioactive_Source> ListOfSources;

        private double Slope;
        private double Y_Int;
        private Form LaunchedFrom;
        #endregion

        #region Constructor
        public BetaEfficiencyChart(Form Parent, List<RadionuclideFamily> _List1, List<Radioactive_Source> _List2)
        {
            InitializeComponent();

            this.LaunchedFrom = Parent;
            this.ListOfFamily = _List1;
            this.ListOfSources = _List2;

            Load += BetaEfficiencyChart_Load;

            //Add keypress event handlers
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Load Handler
        private void BetaEfficiencyChart_Load(object sender, EventArgs e)
        {
            DrawGraph();   
        }
        #endregion

        #region Graph Paint Function
        private void DrawGraph()
        {
            //Add series
            Chart.Series.Add(new Series(name: "Beta Efficiencies"));
            Chart.Series["Beta Efficiencies"].ChartType = SeriesChartType.FastPoint;
            Chart.Series["Beta Efficiencies"].MarkerSize = 10;
            Chart.Series["Beta Efficiencies"].Color = Color.Red;

            Chart.Series.Add(new Series(name: "Beta Line Of Best Fit"));
            Chart.Series["Beta Line Of Best Fit"].ChartType = SeriesChartType.Line;
            Chart.Series["Beta Line Of Best Fit"].MarkerSize = 5;
            Chart.Series["Beta Line Of Best Fit"].Color = Color.Blue;

            double MaxX = 0;
            double MinX = 999999;

            foreach (Radioactive_Source R in ListOfSources)
            {
                int famID = R.GetFamilyID();
                RadionuclideFamily RF = this.ListOfFamily[famID];
                if ((RF.GetSourceType() == RadionuclideFamily.RadiationType.Beta))
                {
                    //plot point
                    double EnergyLevel = R.GetBetaEnergyLevel();
                    switch (RF.GetEnergyBand())
                    {
                        case RadionuclideFamily.EnergyBand.Beta_100_200KeV:
                            Chart.Series["Beta Efficiencies"].Points.AddXY(EnergyLevel, R.GetBetaEfficiency());
                            break;

                        case RadionuclideFamily.EnergyBand.Beta_200_400KeV:
                            Chart.Series["Beta Efficiencies"].Points.AddXY(EnergyLevel, R.GetBetaEfficiency());
                            break;

                        case RadionuclideFamily.EnergyBand.Beta_400_1200KeV:
                            Chart.Series["Beta Efficiencies"].Points.AddXY(EnergyLevel, R.GetBetaEfficiency());
                            break;

                        case RadionuclideFamily.EnergyBand.Beta_More_1200KeV:
                            Chart.Series["Beta Efficiencies"].Points.AddXY(EnergyLevel, R.GetBetaEfficiency());
                            break;

                        default:
                            break;
                    }

                    if (MaxX < EnergyLevel)
                    {
                        MaxX = EnergyLevel;
                    }

                    if (MinX > EnergyLevel)
                    {
                        MinX = EnergyLevel;
                    }

                }
            }

            /*Compute line of best fit*/
            /*Slope computation*/

            double xyProdSum = 0;
            double SumX = 0;
            double SumXSq = 0;
            double SumY = 0;

            double NumPoints = Chart.Series["Beta Efficiencies"].Points.Count;

            foreach (DataPoint P in Chart.Series["Beta Efficiencies"].Points)
            {
                double x = P.XValue;
                double y = P.YValues[0];
                xyProdSum += x*y;
                SumX += x;
                SumXSq += x * x;
                SumY += y;
            }

            this.Slope = (xyProdSum - ((SumX * SumY) / (NumPoints))) / (SumXSq - ((SumX * SumX) / NumPoints));

            this.Y_Int = (SumY / NumPoints) - (Slope * (SumX / NumPoints));

            /*Draw line*/
            Chart.Series["Beta Line Of Best Fit"].Points.AddXY(MinX, (Y_Int + (Slope * MinX)));
            Chart.Series["Beta Line Of Best Fit"].Points.AddXY(MaxX, (Y_Int + (Slope * MaxX)));

            /*Invalidate chart*/
            Chart.Invalidate();
        }
        #endregion

        #region Compute Button Handler
        private void ComputeButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Efficiency_TB.Text = Convert.ToString(Y_Int + (Slope * (Convert.ToDouble(this.Energy_Level_TB.Text))));
            }
            catch
            {
                MessageBox.Show("Error: Bad values.");
            }

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
                if (Key.KeyCode == Keys.K)
                {
                    ComputeButton_Click(this, null);
                    return;
                }

                if (Key.KeyCode == Keys.I)
                {
                    ImageSaveButton_Click(this, null);
                }
            }
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
            MessageBox.Show("Done!");
            return;
        }
        #endregion

        #region Finalization
        private void BetaEfficiencyChart_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.LaunchedFrom.Show();
        }
        #endregion

    }
}
