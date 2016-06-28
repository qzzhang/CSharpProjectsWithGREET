using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DABRAS_Software
{
    public partial class CalibrationInfo : Form
    {

        private List<Radioactive_Source> ListOfSources;


        public CalibrationInfo(List<Radioactive_Source> _List)
        {
            InitializeComponent();

            this.ListOfSources = _List;

            Source_ComboBox.Items.Clear();

            foreach (Radioactive_Source R in ListOfSources)
            {
                Source_ComboBox.Items.Add(R.GetName());
            }

            if (ListOfSources.Count != 0)
            {
                Source_ComboBox.SelectedIndex = 0;
                FillData(ListOfSources[0]);
            }

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);

        }

        private void Source_ComboBox_TextUpdate(object sender, EventArgs e)
        {
            Radioactive_Source SourceSelected = ListOfSources.Find(x => x.GetName() == Source_ComboBox.Text);
            if (SourceSelected != null)
            {
                FillData(SourceSelected);
            }
        }

        private void FillData(Radioactive_Source R)
        {
            this.Alpha_Eff_TB.Text = (R.GetSourceType() == Radioactive_Source.RadiationType.Alpha ? Convert.ToString(R.GetAlphaEfficiency()) : "N/A");
            this.Beta_Eff_TB.Text = (R.GetSourceType() == Radioactive_Source.RadiationType.Beta ? Convert.ToString(R.GetBetaEfficiency()) : "N/A");

            this.Beta_KEV_TB.Text = (R.GetSourceType() == Radioactive_Source.RadiationType.Beta ? MakeEnergyBandString(R.GetEnergyBand()) : "N/A");
           
            this.Alpha_Avg_TB.Text = Convert.ToString(R.GetAlphaHiLoAvg());
            this.Beta_Avg_TB.Text = Convert.ToString(R.GetBetaHiLoAvg());

            if (R.GetSourceType() != Radioactive_Source.RadiationType.Beta)
            {
                this.Alpha_UL_TB.Text = Convert.ToString(R.GetAlphaHi());
                this.Alpha_LL_TB.Text = Convert.ToString(R.GetAlphaLo());
            }

            else
            {
                this.Alpha_UL_TB.Text = "N/A";
                this.Alpha_LL_TB.Text = "N/A";
            }

            if (R.GetSourceType() != Radioactive_Source.RadiationType.Alpha)
            {
                this.Beta_UL_TB.Text = Convert.ToString(R.GetBetaHi());
                this.Beta_LL_TB.Text = Convert.ToString(R.GetBetaLo());
            }

            else
            {
                this.Beta_UL_TB.Text = "N/A";
                this.Beta_LL_TB.Text = "N/A";
            }

            return;
        }

        private string MakeEnergyBandString(Radioactive_Source.EnergyBand Band)
        {
            switch (Band)
            {
                case Radioactive_Source.EnergyBand.Beta_100_200KeV:
                    return "100-200 KeV";
                case Radioactive_Source.EnergyBand.Beta_200_400KeV:
                    return "200-400 KeV";
                case Radioactive_Source.EnergyBand.Beta_400_1200KeV:
                    return "400-1200 KeV";
                case Radioactive_Source.EnergyBand.Beta_More_1200KeV:
                    return ">1200KeV";
                default:
                    return "N/A";
            }
        }

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

        private void closeCtrlQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
