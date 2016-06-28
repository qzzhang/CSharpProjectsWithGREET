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
    public partial class AddSource : Form
    {
        #region Data Members
        Radioactive_Source R = null;
        List<Radioactive_Source> ListOfSources;
        private bool NewSourceWritten = false;
        private Form LaunchedFrom;
        #endregion

        #region Constructor
        public AddSource(Form Parent, List<Radioactive_Source> _List)
        {
            InitializeComponent();
            this.ListOfSources = _List;
            this.LaunchedFrom = Parent;

            HalfLife_Combobox.Items.Clear();

            HalfLife_Combobox.Items.Add("Seconds");
            HalfLife_Combobox.Items.Add("Minutes");
            HalfLife_Combobox.Items.Add("Days");
            HalfLife_Combobox.Items.Add("Months");
            HalfLife_Combobox.Items.Add("Years");

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Save Handler
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Save Source?", "Confirm Action", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                
                R = new Radioactive_Source(this.Source_TB.Text, this.Serial_TB.Text, this.Description_TB.Text, GetCurrentSourceType(), GetBetaEnergyLevel(), GetHalfLife(), this.CertDate_DTP.Text, Convert.ToInt32(this.CertAct_TB.Text), Convert.ToInt32(this.CurAct_TB.Text));

                ListOfSources.Add(R);
                NewSourceWritten = true;
                MessageBox.Show("Source Saved.");
                this.Close();
            }
        }
        #endregion

        #region Private Utility Functions
        private ulong GetHalfLife()
        {
            if (String.Compare(this.HalfLife_Combobox.Text, "Seconds") == 0)
            {
                return (ulong)Convert.ToInt64(this.HalfLife_TB.Text);
            }

            if (String.Compare(this.HalfLife_Combobox.Text, "Minutes") == 0)
            {
                ulong Temp = (ulong)Convert.ToInt64(this.HalfLife_TB.Text);
                return Temp * 60;
            }

            if (String.Compare(this.HalfLife_Combobox.Text, "Hours") == 0)
            {
                ulong Temp = (ulong)Convert.ToInt64(this.HalfLife_TB.Text);
                return Temp * 3600;
            }

            if (String.Compare(this.HalfLife_Combobox.Text, "Days") == 0)
            {
                ulong Temp = (ulong)Convert.ToInt64(this.HalfLife_TB.Text);
                return Temp * 86400;
            }

            if (String.Compare(this.HalfLife_Combobox.Text, "Months") == 0)
            {
                ulong Temp = (ulong)Convert.ToInt64(this.HalfLife_TB.Text);
                return Temp * 2678400;
            }

            if (String.Compare(this.HalfLife_Combobox.Text, "Years") == 0)
            {
                ulong Temp = (ulong)Convert.ToInt64(this.HalfLife_TB.Text);
                return Temp * 31556000;
            }

            return 0;

        }

        private Radioactive_Source.EnergyBand GetBetaEnergyLevel()
        {
            if (this.Alpha_Button.Checked)
            {
                return Radioactive_Source.EnergyBand.Alpha;
            }

            if (this.Beta_Button.Checked)
            {
                if (this._100_200KeV_Button.Checked)
                {
                    return Radioactive_Source.EnergyBand.Beta_100_200KeV;
                }

                if (this._200_400_KeV_Button.Checked)
                {
                    return Radioactive_Source.EnergyBand.Beta_200_400KeV;
                }

                if (this._400_1200_KeV_Button.Checked)
                {
                    return Radioactive_Source.EnergyBand.Beta_400_1200KeV;
                }

                if (this._1200_KeV_Button.Checked)
                {
                    return Radioactive_Source.EnergyBand.Beta_More_1200KeV;
                }
            }

            if (this.Alpha_Beta_Button.Checked)
            {
                return Radioactive_Source.EnergyBand.AlphaBeta;
            }

            return Radioactive_Source.EnergyBand.Unknown;
        }

        private Radioactive_Source.RadiationType GetCurrentSourceType()
        {
            if (this.Alpha_Button.Checked)
            {
                return Radioactive_Source.RadiationType.Alpha;
            }
            if (this.Beta_Button.Checked)
            {
                return Radioactive_Source.RadiationType.Beta;
            }
            if (this.Alpha_Beta_Button.Checked)
            {
                return Radioactive_Source.RadiationType.Gamma;
            }
            else
            {
                return Radioactive_Source.RadiationType.Unknown;
            }
        }

        private void Set_Beta_Modifiers_Enabled(bool Status)
        {
            this._100_200KeV_Button.Enabled = Status;
            this._200_400_KeV_Button.Enabled = Status;
            this._400_1200_KeV_Button.Enabled = Status;
            this._1200_KeV_Button.Enabled = Status;
            return;
        }

        #endregion

        #region Getters
        public bool WasSourceWritten()
        {
            return this.NewSourceWritten;
        }

        public List<Radioactive_Source> GetNewList()
        {
            return this.ListOfSources;
        }
        #endregion

        #region RadioButton Handlers
        private void Alpha_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (Beta_Button.Checked)
            {
                Set_Beta_Modifiers_Enabled(true);
            }

            else
            {
                Set_Beta_Modifiers_Enabled(false);
            }
        }

        private void Beta_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (Beta_Button.Checked)
            {
                Set_Beta_Modifiers_Enabled(true);
            }

            else
            {
                Set_Beta_Modifiers_Enabled(false);
            }
        }

        private void AlphaBeta_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (Beta_Button.Checked)
            {
                Set_Beta_Modifiers_Enabled(true);
            }

            else
            {
                Set_Beta_Modifiers_Enabled(false);
            }
        }

        private void Unknown_Button_CheckedChanged(object sender, EventArgs e)
        {
            if (Beta_Button.Checked)
            {
                Set_Beta_Modifiers_Enabled(true);
            }

            else
            {
                Set_Beta_Modifiers_Enabled(false);
            }
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
        }
        #endregion

        #region Finalization
        private void AddSource_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.LaunchedFrom.Show();
        }
        #endregion

    }
}
