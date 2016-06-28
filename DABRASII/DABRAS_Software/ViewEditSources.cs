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
    public partial class ViewEditSources : Form
    {
        #region Data Members
        private List<Radioactive_Source> ListOfSources;
        private bool WriteSource = false;
        private Form LaunchedFrom;
        #endregion

        #region Constructor
        public ViewEditSources(Form _Parent, List<Radioactive_Source> _ListOfSources)
        {
            InitializeComponent();
            WriteSource = false;
            this.ListOfSources = _ListOfSources;
            this.LaunchedFrom = _Parent;

            /*Initialize Textbox data with the first source on the list*/
            Radioactive_Source R = ListOfSources.First();
            FillData(R);

            this.HalfLife_Combobox.Items.Clear();
            this.HalfLife_Combobox.Items.Add("Seconds");
            this.HalfLife_Combobox.Items.Add("Minutes");
            this.HalfLife_Combobox.Items.Add("Days");
            this.HalfLife_Combobox.Items.Add("Months");
            this.HalfLife_Combobox.Items.Add("Years");


            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Private Utility Functions
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
            if (this.Unknown_Button.Checked)
            {
                return Radioactive_Source.RadiationType.Gamma;
            }
            else
            {
                return Radioactive_Source.RadiationType.Unknown;
            }
        }

        private void Set_Beta_Modifier_Buttons(bool EnabledValue)
        {
            this._100_200KeV_Button.Enabled = EnabledValue;
            this._200_400_KeV_Button.Enabled = EnabledValue;
            this._400_1200_KeV_Button.Enabled = EnabledValue;
            this._1200_KeV_Button.Enabled = EnabledValue;
            return;
        }

        private void FillData(Radioactive_Source R)
        {
            this.Source_ComboBox.Text = R.GetName();
            this.Serial_TB.Text = R.GetSerialNumber();
            this.Description_TB.Text = R.GetDescription();
            this.CertAct_TB.Text = Convert.ToString(R.GetCertifiedActivity());
            this.CurAct_TB.Text = Convert.ToString(R.GetCurrentlyAppliedActivity());

            if (String.Compare(R.GetSourceType_String(), "Alpha") == 0)
            {
                this.Alpha_Button.Checked = true;
            }

            if (String.Compare(R.GetSourceType_String(), "Beta") == 0)
            {
                this.Beta_Button.Checked = true;
            }

            if (String.Compare(R.GetSourceType_String(), "Gamma") == 0)
            {
                this.Unknown_Button.Checked = true;
            }

            if (this.Beta_Button.Checked)
            {
                Set_Beta_Modifier_Buttons(this.EditCheckBox.Checked);

                Radioactive_Source.EnergyBand CurrentBetaEnergy = R.GetEnergyBand();

                if (CurrentBetaEnergy == Radioactive_Source.EnergyBand.Beta_100_200KeV)
                {
                    this._100_200KeV_Button.Checked = true;
                }

                if (CurrentBetaEnergy == Radioactive_Source.EnergyBand.Beta_200_400KeV)
                {
                    this._200_400_KeV_Button.Checked = true;
                }

                if (CurrentBetaEnergy == Radioactive_Source.EnergyBand.Beta_400_1200KeV)
                {
                    this._400_1200_KeV_Button.Checked = true;
                }

                if (CurrentBetaEnergy == Radioactive_Source.EnergyBand.Beta_More_1200KeV)
                {
                    this._1200_KeV_Button.Checked = true;
                }

                this.Beta_KEV_TB.Text = Convert.ToString(R.GetBetaEnergyLevel());
            }

            else
            {
                Set_Beta_Modifier_Buttons(false);
                this.Beta_KEV_TB.Enabled = false;
            }

            /*Convert from seconds to a more user-friendly format*/
            ulong HalfLife = R.GetHalfLife();
            double FinalValue = 0;
            string Identifier = "Seconds";

            if ((HalfLife > 60) && (HalfLife < 3600))
            {
                Identifier = "Minutes";
                FinalValue = HalfLife / 60;
            }

            else if ((HalfLife >= 3600) && (HalfLife < 2678400))
            {
                Identifier = "Days";
                FinalValue /= 3600;
            }

            else if ((HalfLife > 2678400) && (HalfLife < 31556000))
            {
                Identifier = "Months";
                FinalValue = HalfLife / 2678400;
            }

            else if (HalfLife >= 31556000)
            {
                Identifier = "Years";
                FinalValue = HalfLife / 31556000;
            }

            this.HalfLife_TB.Text = Convert.ToString(FinalValue);
            this.HalfLife_Combobox.Text = Identifier;

            DateTime CertificationDate = DateTime.Parse(R.GetCertificaitonDate());
            CertDate_DTP.Value = CertificationDate;
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

        #endregion

        #region Edit Checkbox Handler
        private void EditCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool BetaSource = false;
            try
            {
                BetaSource = (ListOfSources.Find(x => x.GetName() == Source_ComboBox.Text).GetSourceType() == Radioactive_Source.RadiationType.Beta);
            }
            catch
            {
                ;
            }
            
            /*Swap the disabled and enabled GUI elements*/
            if (EditCheckBox.Checked)
            {
                /*Allow user to edit, block the rest of the form*/
                this.SaveButton.Enabled = true;
                this.SaveButton.Visible = true;
                this.Serial_TB.Enabled = true;
                this.Description_TB.Enabled = true;
                this.Alpha_Button.Enabled = true;
                this.Beta_Button.Enabled = true;
                this.Unknown_Button.Enabled = true;
                this.CertDate_DTP.Enabled = true;
                this.CertAct_TB.Enabled = true;

                this.HalfLife_TB.Enabled = true;
                this.HalfLife_Combobox.Enabled = true;

                this.Source_ComboBox.Enabled = false;

                if (BetaSource)
                {
                    Set_Beta_Modifier_Buttons(true);
                    Beta_KEV_TB.Enabled = true;
                }
            }

            else
            {
                /*Don't Allow user to edit, Enable the rest of the form*/
                this.SaveButton.Enabled = false;
                this.SaveButton.Visible = false;
                this.Serial_TB.Enabled = false;
                this.Description_TB.Enabled = false;
                this.Alpha_Button.Enabled = false;
                this.Beta_Button.Enabled = false;
                this.Unknown_Button.Enabled = false;
                this.CertDate_DTP.Enabled = false;
                this.CertAct_TB.Enabled = false;

                this.HalfLife_Combobox.Enabled = false;
                this.HalfLife_TB.Enabled = false;

                Set_Beta_Modifier_Buttons(false);
                Beta_KEV_TB.Enabled = false;

                this.Source_ComboBox.Enabled = true;
            }
        }
        #endregion

        #region Save Handler
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Write Data? This will overwrite the previous source data.", "Confirm Write", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                WriteSource = true;

                ulong HalfLife = 0;

                /*Set half-life*/
                if (this.HalfLife_Combobox.Text == "Seconds")
                {
                    HalfLife = Convert.ToUInt64(HalfLife_TB.Text);
                }

                if (this.HalfLife_Combobox.Text == "Minutes")
                {
                    HalfLife = Convert.ToUInt64(Math.Floor(Convert.ToDouble(HalfLife_TB.Text) * 60));
                }

                if (this.HalfLife_Combobox.Text == "Days")
                {
                    HalfLife = Convert.ToUInt64(Math.Floor(Convert.ToDouble(HalfLife_TB.Text) * 86400));
                }

                if (this.HalfLife_Combobox.Text == "Months")
                {
                    HalfLife = Convert.ToUInt64(Math.Floor(Convert.ToDouble(HalfLife_TB.Text) * 2678400));
                }

                if (this.HalfLife_Combobox.Text == "Years")
                {
                    HalfLife = Convert.ToUInt64(Math.Floor(Convert.ToDouble(HalfLife_TB.Text) * 31556000));
                }

                Radioactive_Source CurrentSource = ListOfSources.Find(x => (x.GetName() == Source_ComboBox.Text));
                CurrentSource.SetSourceType(GetCurrentSourceType());

                if (CurrentSource.GetSourceType() == Radioactive_Source.RadiationType.Beta)
                {
                    try
                    {
                        CurrentSource.SetBetaEnergy(Convert.ToDouble(this.Beta_KEV_TB.Text));
                    }

                    catch
                    {
                        MessageBox.Show("Error: Bad values.");
                        return;
                    }
                }
                


                CurrentSource.SetName(this.Source_ComboBox.Text);
                CurrentSource.SetSerialNumber(this.Serial_TB.Text);
                CurrentSource.SetDescription(this.Description_TB.Text);
                CurrentSource.SetEnergyBand(GetBetaEnergyLevel());
                CurrentSource.SetHalfLife(HalfLife);
                CurrentSource.SetCertificationDate(this.CertDate_DTP.Text);
                CurrentSource.SetCertifiedActivity(Convert.ToInt32(this.CertAct_TB.Text));
                CurrentSource.SetCurrentlyAppliedActivity(Convert.ToInt32(this.CertAct_TB.Text));

                MessageBox.Show("Source written.");
            }
        }
        #endregion

        #region Getters
        public bool WereSourcesModified()
        {
            return WriteSource;
        }

        public List<Radioactive_Source> GetRadioActiveSourceList()
        {
            return ListOfSources;
        }
        #endregion

        #region Misc GUI Handlers
        private void Source_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Source_ComboBox.Items.Clear();

            foreach (Radioactive_Source i in ListOfSources)
            {
                if (i.GetName() != "Background")
                {
                    Source_ComboBox.Items.Add(i.GetName());
                }
            }
        }

        private void Source_ComboBox_TextUpdate(object sender, EventArgs e)
        {
            Radioactive_Source R = ListOfSources.Find(x => (x.GetName() == Source_ComboBox.Text));

            FillData(R);
        }
        #endregion

        #region Keypress Handler
        private void KeyPressed(object sender, KeyEventArgs Key)
        {
            if (Key.Control && Key.Alt && Key.Shift)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }

            if (Key.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }
        }
        #endregion

        #region Finalization
        private void ViewEditSources_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.LaunchedFrom.Show();
        }
        #endregion
    }
}
