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
    public partial class RemoveSource : Form
    {
        #region Data Members
        private List<Radioactive_Source> ListOfSources;
        private bool ListChanged;

        private string[] ListOfProtectedSources = { "Background", "Am-241", "Sr-90" };
        private Form LaunchedFrom;
        #endregion

        #region Constructor
        public RemoveSource(Form Parent, List<Radioactive_Source> _List)
        {
            InitializeComponent();
            ListChanged = false;
            ListOfSources = _List;

            Radioactive_Source R = null;
            foreach (Radioactive_Source PotentialSource in ListOfSources)
            {
                bool Protected = false;
                for (int j = 0; j < ListOfProtectedSources.Length; j++)
                {
                    if (PotentialSource.GetName() == ListOfProtectedSources[j])
                    {
                        Protected = true;
                        break;
                    }
                }

                if (!Protected)
                {
                    R = PotentialSource;
                    break;
                }
            }

            FillData(R);
            this.LaunchedFrom = Parent;

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(KeyPressed);
        }
        #endregion

        #region Delete Button Handler
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete this source from the calibration list? This cannot be undone.", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int Index = ListOfSources.FindIndex(x => (x.GetName() == Source_ComboBox.Text));
                ListOfSources.RemoveAt(Index);
                ListChanged = true;
            }

            Source_ComboBox_SelectedIndexChanged(this, null);
        }
        #endregion

        #region Private Utility Functions
        private void FillData(Radioactive_Source R)
        {
            if (R == null)
            {
                return;
            }

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
                this.Gamma_Button.Checked = true;
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

            else if ((HalfLife >= 3600) && (HalfLife < 86400))
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
        #endregion

        #region Misc. GUI Functions
        private void Source_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Source_ComboBox.Items.Clear();

            foreach (Radioactive_Source i in ListOfSources)
            {
                bool Protected = false;
                string Name = i.GetName();

                for (int j = 0; j < ListOfProtectedSources.Length; j++)
                {
                    if (Name == ListOfProtectedSources[j])
                    {
                        Protected = true;
                        break;
                    }
                }

                if (!Protected)
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

        #region Getters
        public bool WasListChanged()
        {
            return this.ListChanged;
        }

        public List<Radioactive_Source> GetNewList()
        {
            return this.ListOfSources;
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
        private void RemoveSource_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.LaunchedFrom.Show();
        }
        #endregion
    }
}
