using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DABRAS_Software
{
    static class StaticMethods
    {
        #region Static Rounding
        /*Implement rounding rule as discussed in tech note TBD-003*/
        public static string RoundToSigFigs(double NumberToRound)
        {            
            if (NumberToRound > 100)
            {
                return RoundToSpecificSigFigs(NumberToRound, 3);
            }

            return RoundToSpecificSigFigs(NumberToRound, 2);
        }

        /*Overload for integers*/
        public static string RoundToSigFigs(int NumberToRound)
        {
            return RoundToSigFigs(Convert.ToDouble(NumberToRound));
        }

        public static string RoundToSpecificSigFigs(double NumberToRound, int NumFigs)
        {
            /*Catch zero*/
            if (NumberToRound == 0)
            {
                return "0";
            }

            double LargestDigitPlace = Math.Ceiling(Math.Log10(Math.Abs(NumberToRound)));

            double Magnitude = Math.Pow(10, (NumFigs - LargestDigitPlace));
            double Shift = Math.Round(Magnitude * NumberToRound);

            return String.Format("{0}", (Shift / Magnitude));
        }

        /*Actually rounding the numbers breaks the standard deviation calculators*/
        public static string RoundToDecimal(double NumberToRound, int NumPlaces)
        {
        //    double IncreasedNumber_Double = NumberToRound * (Math.Pow(10, NumPlaces));
        //    long IncreasedNumber_Int = Convert.ToInt64(IncreasedNumber_Double);
            
        //    if (IncreasedNumber_Double - Convert.ToDouble(IncreasedNumber_Int) >= 0.5)
        //    {
        //        IncreasedNumber_Int++;
        //    }

        //    return Convert.ToString(Convert.ToDouble(IncreasedNumber_Int) / (Math.Pow(10, NumPlaces)));
            string retstr;

            if (NumberToRound == 0)
            {
                return "0";
            }
            retstr = String.Format("{0:.#}", NumberToRound);
            return retstr;       
        }
        #endregion

    }
}
