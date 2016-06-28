using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace DABRAS_Software
{
    [Serializable]
    public class RadionuclideFamily
    {
        #region Enums
        public enum RadiationType { Alpha, Beta, AlphaBeta, Gamma, Unknown };
        public enum EnergyBand { Alpha, AlphaBeta, Beta_Less_100KeV, Beta_100_200KeV, Beta_200_400KeV, Beta_400_1200KeV, Beta_More_1200KeV, Unknown };
        #endregion

        #region Data members
        private int FamilyID;
        private string Name;
        private RadiationType source_type;
        private EnergyBand energy_band;
        private ulong HalfLife;
        private Radioactive_Source current_source;//the most recently calibrated source in this family

        private double AlphaHi = -1;
        private double AlphaLo = -1;
        private double BetaHi = -1;
        private double BetaLo = -1;
        private double AlphaHiLoAvg = -1;
        private double BetaHiLoAvg = -1;

        private double DailyAlphaCPM = -1;
        private double DailyBetaCPM = -1;
        private double AnnualAlphaCPM = -1;
        private double AnnualBetaCPM = -1;

        private DateTime DailyCalibrationDate;
        private int DailyCalibrationSeconds;
        private int AnnualCalibrationSeconds;
        private DateTime HiLoCalibrationDate;

        private string CertificationDate; 
        #endregion

        #region Constructor
        public RadionuclideFamily(int _ID, string _Name, RadiationType _RT, EnergyBand _E, ulong _HalfLife)
        {
            this.FamilyID = _ID;
            this.Name = _Name;
            this.HalfLife = _HalfLife;
            this.source_type = _RT;
            this.energy_band = _E;
            if(_RT == RadiationType.Alpha)
                this.current_source = new Radioactive_Source(1, "RE218", "", "5/22/2008", 61200);
            else
                this.current_source = new Radioactive_Source(2, "RE215", "", "5/22/2008", 102240);
        }
        #endregion

        #region Getters
        public RadiationType GetSourceType()
        {
            return this.source_type; 
        }
        public string GetName()
        {
            return Name;
        }

        public ulong GetHalfLife()
        {
            return HalfLife;
        }
        public int GetFamilyID()
        {
             return this.FamilyID;
        }
        public EnergyBand GetEnergyBand()
        {
            return this.energy_band;
        }
        public string GetSourceType_String()
        {
            if (this.source_type == RadiationType.Alpha)
            {
                return "Alpha";
            }
            else if (this.source_type == RadiationType.Beta)
            {
                return "Beta";
            }
            else if (this.source_type == RadiationType.Gamma)
            {
                return "Gamma";
            }
            else if (this.source_type == RadiationType.AlphaBeta)
            {
                return "AlphaBeta";
            }
            else
            {
                return "Unknown";
            }
        }
        public Radioactive_Source GetCurrentSource()
        {
            return this.current_source ;
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

        public double GetBetaLo()
        {
            return this.BetaLo;
        }

        public double GetAlphaHiLoAvg()
        {
            return this.AlphaHiLoAvg;
        }

        public double GetBetaHiLoAvg()
        {
            return this.BetaHiLoAvg;
        }

        public DateTime GetHiLoCalibratedTime()
        {
            return this.HiLoCalibrationDate;
        }

        public string GetCertificationDate()
        {
            return this.current_source.GetCertificationDate();
        }

        public double GetDecayFactor(DateTime _EndTime)
        {
            //Computing the disintigration decay value
            TimeSpan ElapsedTime = _EndTime.Subtract(DateTime.Parse(this.CertificationDate));
            double ElapsedHalfLives = Convert.ToDouble(ElapsedTime.TotalSeconds) / Convert.ToDouble(this.HalfLife);

            double HalfLifeMultiplier = Math.Pow(0.5, ElapsedHalfLives);

            return HalfLifeMultiplier;
        }

        public double GetDecayFactor(DateTime _EndTime, DateTime _StartTime)
        {
            //Computing the disintigration decay value
            TimeSpan ElapsedTime = _EndTime.Subtract(_StartTime);
            double ElapsedHalfLives = Convert.ToDouble(ElapsedTime.TotalSeconds) / Convert.ToDouble(this.HalfLife);

            double HalfLifeMultiplier = Math.Pow(0.5, ElapsedHalfLives);

            return HalfLifeMultiplier;
        }
        #endregion

        #region Setters
        public bool SetSourceType(RadiationType rt)
        {
            this.source_type = rt;
            return true;
        }
        public bool SetName(string _Name)
        {
            this.Name = _Name;
            return true;
        }

        public bool SetHalfLife(ulong _T)
        {
            this.HalfLife = _T;
            return true;
        }
        public bool SetFamilyID(int _ID)
        {
            this.FamilyID = _ID;
            return true;
        }
        public bool SetEnergyBand(EnergyBand _E)
        {
            this.energy_band = _E;
            return true;
        }
        public bool SetCurrentSource(Radioactive_Source _CS)
        {
            this.current_source = _CS;
            return true;
        }

        public bool SetAlphaHiLoAvg(double _Val)
        {
            this.AlphaHiLoAvg = _Val;
            return true;
        }

        public bool SetBetaHiLoAvg(double _Val)
        {
            this.BetaHiLoAvg = _Val;
            return true;
        }

        public bool SetAlphaHi(double _AH)
        {
            this.AlphaHi = _AH;
            return true;
        }

        public bool SetAlphaLo(double _AL)
        {
            this.AlphaLo = _AL;
            return true;
        }

        public bool SetBetaHi(double _BH)
        {
            this.BetaHi = _BH;
            return true;
        }

        public bool SetBetaLo(double _BL)
        {
            this.BetaLo = _BL;
            return true;
        }
        public bool SetAnnualAlphaCPM(double _ACPM)
        {
            this.AnnualAlphaCPM = _ACPM;
            return true;
        }

        public bool SetAnnualBetaCPM(double _BCPM)
        {
            this.AnnualBetaCPM = _BCPM;
            return true;
        }

        public bool SetHiLoCalibratedDate(DateTime _D)
        {
            this.HiLoCalibrationDate = _D;
            return true;
        }

        public bool SetAnnualCalibratedTimespan(int _T)
        {
            this.AnnualCalibrationSeconds = _T;
            return true;
        }

        public bool SetCertificationDate(string _D)
        {
            this.CertificationDate = _D;
            return true;
        }

        public bool SetDailyAlphaCPM(int _DACPM)
        {
            this.DailyAlphaCPM = _DACPM;
            return true;
        }

        public bool SetDailyBetaCPM(int _DBCPM)
        {
            this.DailyBetaCPM = _DBCPM;
            return true;
        }
        public bool SetDailyCalibratedDate(DateTime _D)
        {
            this.DailyCalibrationDate = _D;
            return true;
        }

        public bool SetDailyCalibratedTimespan(int _T)
        {
            this.DailyCalibrationSeconds = _T;
            return true;
        }

        #endregion
    }
}
