using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace DABRAS_Software
{
    [Serializable]
    public class Radioactive_Source
    {
        #region Data members
        private int RadionuclideFamilyID;//added by QZ
        private string SerialNumber;
        private string Description;
        private ulong HalfLife;
        private string CertificationDate;//fanufacturer's certification
        private int CertifiedActivity;
        private int CurrentlyAppliedActivity;

        private double Beta_Energy = -1;

        private double AlphaHi = -1;
        private double AlphaLo = -1;
        private double BetaHi = -1;
        private double BetaLo = -1;

        private double AlphaHiLoAvg = -1;
        private double BetaHiLoAvg = -1;

        private double AnnualAlphaCPM = -1;
        private double AnnualBetaCPM = -1;
        private double DailyAlphaCPM = -1;
        private double DailyBetaCPM = -1;

        private double AlphaEfficiency = -1;
        private double BetaEfficiency = -1;

        private int AnnualCalibrationSeconds;
        private double AlphaDisintigrationFactor;
        private double BetaDisintigrationFactor;
        private DateTime DailyCalibrationDate;
        private int DailyCalibrationSeconds;
        private DateTime HiLoCalibrationDate;
        private DateTime AnnualCalibrationDate;
        #endregion

        #region Constructor
        public Radioactive_Source(int _rdFamilyID, string _SerialNumber, string _Description, string _CertDate, int _CertActivity)
        {
            this.RadionuclideFamilyID = _rdFamilyID;
            this.SerialNumber = _SerialNumber;
            this.Description = _Description;
            this.CertificationDate = _CertDate;
            this.CertifiedActivity = _CertActivity;
        }
        #endregion

        #region Utility functions

        public double ComputeDecayFactor(List<double> _CPM, int SampleTime)
        {
            //Implement rhat patten from Marlap - Procedure 18.3
            double RHat = 0;
            
            foreach (double i in _CPM)
            {
                RHat += i;
            }

            //Because the time intervals are constant, and each Di is approximately constant, we compute the constant product
            double Denom = Convert.ToDouble(_CPM.Count) * GetDecayFactor(DateTime.Now) * Convert.ToDouble(SampleTime) / 60;

            return (RHat / Denom);
        }

        #endregion

        #region Getters
        public int GetFamilyID()
        {
            return this.RadionuclideFamilyID;
        }

        public double GetAlphaHiLoAvg()
        {
            return this.AlphaHiLoAvg;
        }

        public double GetBetaHiLoAvg()
        {
            return this.BetaHiLoAvg;
        }

        public double GetBetaEnergyLevel()
        {
            return this.Beta_Energy;
        }

        public ulong GetHalfLife()
        {
            return this.HalfLife;
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

        public double GetAlphaDistigrationFactor()
        {
            return this.AlphaDisintigrationFactor;
        }

        public double GetBetaDisintigrationFactor()
        {
            return this.BetaDisintigrationFactor;
        }

        public double GetAlphaEfficiency()
        {
            return this.AlphaEfficiency;
        }

        public double GetBetaEfficiency()
        {
            return this.BetaEfficiency;
        }

        public string GetSerialNumber()
        {
            return SerialNumber;
        }

        public string GetDescription()
        {
            return Description;
        }

        public int GetCertifiedActivity()
        {
            return CertifiedActivity;
        }

        public string GetCertificationDate()
        {
            return this.CertificationDate;
        }

        public int GetCurrentlyAppliedActivity()
        {
            //Computing the disintigration decay value
            TimeSpan ElapsedTime = (DateTime.Now).Subtract(DateTime.Parse(this.GetCertificationDate()));
            double ElapsedHalfLives = Convert.ToDouble(ElapsedTime.TotalSeconds) / Convert.ToDouble(this.GetHalfLife());
            double HalfLifeMultiplier = Math.Pow(0.5, ElapsedHalfLives);

            this.SetCurrentlyAppliedActivity(Convert.ToInt32((HalfLifeMultiplier * this.GetCertifiedActivity())));
            return this.CurrentlyAppliedActivity;
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

        public double GetAnnualAlphaCPM()
        {
            return this.AnnualAlphaCPM;
        }

        public double GetAnnualBetaCPM()
        {
            return this.AnnualBetaCPM;
        }

        public double GetDailyAlphaCPM()
        {
            return this.DailyAlphaCPM;
        }

        public double GetDailyBetaCPM()
        {
            return this.DailyBetaCPM;
        }

        public DateTime GetHiLoCalibratedTime()
        {
            return this.HiLoCalibrationDate;
        }

        public DateTime GetDailyCalibratedTime()
        {
            return this.DailyCalibrationDate;
        }

        public DateTime GetAnnualCalibratedTime()
        {
            return this.AnnualCalibrationDate;
        }

        public int GetDailyCalibratedTimespan()
        {
            return this.DailyCalibrationSeconds;
        }

        public int GetAnnualCalibratedTimespan()
        {
             return this.AnnualCalibrationSeconds;
        }

        #endregion

        #region Custom Setters
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

        public bool SetBetaEnergy(double _E)
        {
            this.Beta_Energy = _E;
            return true;
        }
        public bool SetAlphaDisintigrationConstant(double _Value)
        {
            this.AlphaDisintigrationFactor = _Value;
            return true;
        }

        public bool SetBetaDisintigrationFactor(double _Value)
        {
            this.BetaDisintigrationFactor = _Value;
            return true;
        }

        public bool SetAlphaEfficiency(double _AE)
        {
            this.AlphaEfficiency = _AE;
            return true;
        }

        public bool SetBetaEfficiency(double _BE)
        {
            this.BetaEfficiency = _BE;
            return true;
        }

        public bool SetSerialNumber(string _SN)
        {
            this.SerialNumber = _SN;
            return true;
        }

        public bool SetDescription(string _D)
        {
            this.Description = _D;
            return true;
        }

        public bool SetHalfLife(ulong _T)
        {
            this.HalfLife = _T;
            return true;
        }

        public bool SetCertifiedActivity(int _A)
        {
            this.CertifiedActivity = _A;
            return true;
        }

        public bool SetCertificationDate(string _D)
        {
            this.CertificationDate = _D;
            return true;
        }

        public bool SetCurrentlyAppliedActivity(int _A)
        {
            this.CurrentlyAppliedActivity = _A;
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
            this.DailyCalibrationDate= _D;
            return true;
        }
        public bool SetAnnualCalibratedDate(DateTime _D)
        {
            this.AnnualCalibrationDate = _D;
            return true;
        }

        public bool SetDailyCalibratedTimespan(int _T)
        {
            this.DailyCalibrationSeconds = _T;
            return true;
        }

        public bool SetAnnualCalibratedTimespan(int _T)
        {
            this.AnnualCalibrationSeconds = _T;
            return true;
        }

        #endregion
    }
}
