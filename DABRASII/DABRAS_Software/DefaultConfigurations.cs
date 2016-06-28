using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DABRAS_Software
{
    /* DefaultConfigurations.cs
     * This class library will represent all of the default values for the 
     * user-definable fields in the whole project.
     * 
     * It will be written to a stream on destruction of the form, and will attempt
     * to be loaded on startup.
     */
    
    [Serializable]
    public class DefaultConfigurations
    {
        #region Data Members
        private string Default_VCP;
        private bool ConfModified;
        private List<Radioactive_Source> ListOfSources;
        private List<RadionuclideFamily> ListOfFamily;
        private List<User> UserList;
        private ModFactors ModFactor;

        private string BuildingNo;
        private string SetNo;

        private string WebBasedSurveyString = "https://webapps.inside.anl.gov/rwp/";
        private string RsoSharePointString = "https://sharepoint.anl.gov/Divisions/esq/rso/HealthPhysicsProcedures/HPP%203.0%20Rev.%209.pdf";
        private string RSOHome = "https://sharepoint.anl.gov/Divisions/esq/rso/HealthPhysicsProcedures/Forms/AllItems.aspx";

        //private List<QCCalResultNode> ListOfDailyCalibrationResults;

        private mainFramework.BackgroundType DefaultRoutineBackgroundType = mainFramework.BackgroundType.Annual;

        private DateTime ChiSquaredExpires;
        private TimeSpan ChiSquaredRenewalTimespan;

        private string Password;
        #endregion

        #region Constructor
        public DefaultConfigurations()
        {
            this.Default_VCP = "COM6";
            ConfModified = false;

            this.Password = "Admin";

            ListOfFamily = new List<RadionuclideFamily>();
            ListOfFamily.Add(new RadionuclideFamily(1, "Am-241", RadionuclideFamily.RadiationType.Alpha, RadionuclideFamily.EnergyBand.Alpha, YearCvt(432.2)));
            ListOfFamily.Add(new RadionuclideFamily(2, "Sr-90", RadionuclideFamily.RadiationType.Beta, RadionuclideFamily.EnergyBand.Beta_More_1200KeV, YearCvt(28.6)));
            ListOfFamily.Add(new RadionuclideFamily(3, "Tc-99", RadionuclideFamily.RadiationType.Beta, RadionuclideFamily.EnergyBand.Beta_200_400KeV, YearCvt(213000)));
            ListOfFamily.Add(new RadionuclideFamily(4, "Cs-137",RadionuclideFamily.RadiationType.Beta, RadionuclideFamily.EnergyBand.Beta_400_1200KeV, YearCvt(30.17)));
            ListOfFamily.Add(new RadionuclideFamily(5, "C-14", RadionuclideFamily.RadiationType.Beta,RadionuclideFamily.EnergyBand.Beta_100_200KeV, YearCvt(5730)));
            ListOfFamily.Add(new RadionuclideFamily(0, "Background", RadionuclideFamily.RadiationType.AlphaBeta, RadionuclideFamily.EnergyBand.Alpha, 0));

            ListOfSources = new List<Radioactive_Source>();
            ListOfSources.Add(new Radioactive_Source(1, "RE218", "", "5/22/2008", 61200));
            ListOfSources.Add(new Radioactive_Source(2, "RE215", "","5/22/2008", 102240));
            ListOfSources.Add(new Radioactive_Source(3, "NR873", "", "8/22/2005", 72000));
            ListOfSources.Add(new Radioactive_Source(4, "***", "", "6/03/2013", 1));
            ListOfSources.Add(new Radioactive_Source(5, "???", "", "6/10/2013", 1));
            ListOfSources.Add(new Radioactive_Source(0, "Background", "", "5/22/2008", 0));

            UserList = new List<User>();
            UserList.Add(new User(12345, "QZ"));

            ChiSquaredExpires = DateTime.Parse("2099/01/01");
            ChiSquaredRenewalTimespan = new TimeSpan();
            ChiSquaredRenewalTimespan.Add(TimeSpan.MaxValue);
        }
        #endregion

        #region VCP Functions
        public void WriteDefaultVCP(string Input)
        {
            if (Input.IndexOf("COM") != -1)
            {
                this.Default_VCP = Input;
                this.ConfModified = true;
            }
        }
        #endregion

        #region Setters
        public void SetSetNo(string _SN)
        {
            this.SetNo = _SN;
            this.ConfModified = true;
            return;
        }

        public void SetBuildingNo(string _BN)
        {
            this.BuildingNo = _BN;
            this.ConfModified = true;
            return;
        }

        public void SetListOfUsers(List<User> _UL)
        {
            this.UserList = _UL;
            this.ConfModified = true;
            return;
        }

        public void SetModFactors(ModFactors _MF)
        {
            this.ModFactor = _MF;
            this.ConfModified = true;
            return;
        }
        public void SetWebSurvey(string URL)
        {
            this.WebBasedSurveyString = URL;
            this.ConfModified = true;
            return;
        }

        public void SetRSOLink(string URL)
        {
            this.RsoSharePointString = URL;
            this.ConfModified = true;
            return;
        }

        public void SetRSOHome(string URL)
        {
            this.RSOHome = URL;
            this.ConfModified = true;
            return;
        }
        
        public void ClearModifiedFlag()
        {
            this.ConfModified = false;
        }

        public void SetPassword(string PW)
        {
            this.Password = PW;
            this.ConfModified = true;
            return;
        }

        public bool SetChiSquaredDateTime(DateTime _D)
        {
            this.ChiSquaredExpires = _D;
            this.ConfModified = true;
            return true;
        }

        public bool SetChiSquaredTimespan(TimeSpan _T)
        {
            this.ChiSquaredRenewalTimespan = _T;
            this.ConfModified = true;
            return true;
        }

        public bool SetRadioFamilyList(List<RadionuclideFamily> RF)
        {
            this.ListOfFamily = RF;
            this.ConfModified = true;
            return true;
        }

        public bool SetRadioactiveSourceList(List<Radioactive_Source> R)
        {
            this.ListOfSources = R;
            this.ConfModified = true;
            return true;
        }

        public bool SetRoutineCalibrationBackgroundType(mainFramework.BackgroundType _type)
        {
            this.DefaultRoutineBackgroundType = _type;
            this.ConfModified = true;
            return true;
        }
        
        #endregion

        #region Getters
        public string GetSetNo()
        {
            return this.SetNo;
        }

        public string GetBuildingNo()
        {
            return this.BuildingNo;
        }

        public List<User> GetListOfUsers()
        {
            return this.UserList;
        }

        public ModFactors GetModFactors()
        {
            return this.ModFactor;
        }

        public string GetWebSurvey()
        {
            return this.WebBasedSurveyString;
        }

        public string GetRSOLink()
        {
            return this.RsoSharePointString;
        }

        public string GetRSOHome()
        {
            return this.RSOHome;
        }
        
        public string GetPassword()
        {
            return this.Password;
        }

        public bool IsNew()
        {
            return this.ConfModified;
        }

        public string GetDefaultVCP()
        {
            return this.Default_VCP;
        }

        public DateTime GetChiSquaredDate()
        {
            return this.ChiSquaredExpires;
        }

        public TimeSpan GetChiSquaredTimespan()
        {
            return this.ChiSquaredRenewalTimespan;
        }

        public List<RadionuclideFamily> GetListOfFamily()
        {
            return this.ListOfFamily;
        }
        public List<Radioactive_Source> GetListOfSources()
        {
            return ListOfSources;
        }
        public mainFramework.BackgroundType GetRoutineCalibrationBackgroundType()
        {
            return this.DefaultRoutineBackgroundType;
        }
        #endregion

        #region Additional Radioactive Source List Methods

        public bool EditRadioactiveSource(int index, Radioactive_Source R)
        {
            ListOfSources.RemoveAt(index);
            ListOfSources.Insert(index, R);
            this.ConfModified = true;
            return true;
        }
        
        public bool AddRadioactiveSource(int index, Radioactive_Source R)
        {
            ListOfSources.Add(R);
            this.ConfModified = true;
            return true;
        }

        public bool DeleteRadioActiveSource(int index)
        {
            ListOfSources.RemoveAt(index);
            this.ConfModified = true;
            return true;
        }
        #endregion

        #region Private Utility Functions
        private ulong YearCvt(double Years)
        {
            return (ulong)(Years * 31556000);
        }
        #endregion
    }

}
