using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;
using System.Linq;

namespace Greet.UnitLib3
{
    [DataContract]
    public abstract class AQuantity : Greet.UnitLib3.IQuantity
    {
        #region private members
        protected string _symbol;
        protected string _name;
        protected List<Unit> _units;
        protected int _preferredUnitIdx;
        protected uint _dim;
        protected double _epsilon = 0;
        #endregion

        #region constructor
        protected AQuantity() { }
        protected AQuantity(XmlNode node)
        {
            this._name = node.Attributes["name"].Value;
            this._symbol = node.Attributes["common_symbol"].Value;
            this._preferredUnitIdx = System.Convert.ToInt32(node.Attributes["preferred_unit"].Value);
            if (node.Attributes["epsilon"] != null)
                _epsilon = Convert.ToDouble(node.Attributes["epsilon"].Value, UnitLib3.Units.USCI);
        }
        #endregion

        #region public accessors
        /// <summary>
        /// The default unit for the quantity
        /// </summary>
        public abstract Unit SiUnit { get; }
        /// <summary>
        /// List of all of the units defined for the quantity
        /// </summary>
        public abstract List<Unit> Units { get; }
        /// <summary>
        /// Refers to index in Units list. To be used to display the quantity in GUi/Reports.
        /// </summary>
        public abstract int PreferedUnitIdx { get; set; }
        /// <summary>
        /// Basic dimensions of the quantity represented in a single integer, use Dimension.Dimension to calculate the integer
        /// </summary>
        public abstract uint Dim { get; }
        /// <summary>
        /// Name of the quantity, for example "energy" or "mass density"
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Common SI symbol used for the quantity, for example L for length, E for energy, $ for currency, Rho for mass density,...
        /// </summary>
        public abstract string Symbol { get; set; }

        abstract public XmlNode ToXML(XmlDocument doc);
        /// <summary>
        /// Converts the value to value in SI units, given the index of the Unit as ddefined in the associated list
        /// </summary>
        /// <param name="from_unit_index">Index of the Unit from which to be converted</param>
        /// <param name="value">value in the non-SI unnits</param>
        /// <returns></returns>
        public double ConvertToSI(int from_unit_index, double value)
        {
            if (Units.Count == 1)
                throw new NoSIUnitDefinedException();
            if (Units.Count - 1 < from_unit_index)
                throw new System.ArgumentException("The index is out of range", "from_unit_index");
            double res = value;
            Unit from = this.Units[from_unit_index];
            res = from.ToSI(value);
            return res;
        }

        /// <summary>
        /// Converts the value to value in SI units, given the index of the Unit as ddefined in the associated list
        /// </summary>
        /// <param name="to_unit_index">Index of the Unit from which to be converted</param>
        /// <param name="value">value in the non-SI unnits</param>
        /// <returns></returns>
        public double ConvertFromSI(int to_unit_index, double value)
        {
            if (Units.Count == 1)
                throw new NoSIUnitDefinedException();
            if (Units.Count - 1 < to_unit_index)
                throw new System.ArgumentException("The index is out of range", "to_unit_index");
            double res = value;
            Unit from = this.Units[to_unit_index];
            res = from.FromSI(value);
            return res;
        }
        #endregion

        class NoSIUnitDefinedException : Exception
        {
            public NoSIUnitDefinedException() :
                base("There are no units defined for the Quantity, not even SI.") { }
        }

        public override string ToString()
        {
            return this.Name;
        }

        #region static methods
        public static double ConvertFromSpecificToSI(double valueInSpecific, string specific)
        {
            List<Unit> specUnits = new List<Unit>();
            List<int> specExp = new List<int>();
            GuiUtils.ParseExpression(specific, out specUnits, out specExp);
            List<Unit> defUnits = new List<Unit>();
            foreach (Unit u in specUnits)
            {
                AQuantity qty = Greet.UnitLib3.Units.QuantityList.Values.FirstOrDefault(item => item.Units.Contains(u));
                defUnits.Add(qty.SiUnit);
            }

            List<List<string>> desiredUnits = ToUnitExpression(specUnits, specExp);
            List<List<string>> siUnits = ToUnitExpression(defUnits, specExp);

            //perform conversion unit by unit
            double convertedValue = valueInSpecific;
            List<string> desiredNumerator = desiredUnits[0];
            List<string> siNumerator = siUnits[0];
            List<string> desiredDenominator = desiredUnits[1];
            List<string> siDenominator = siUnits[1];

            int safetyInt = 0;
            while (desiredNumerator.Count > 0 && safetyInt < 10000)
            {//perform conversion for numerator units
                Unit desiredUnit = Greet.UnitLib3.Units.UnitsList.Values.FirstOrDefault(item => item.Expression == desiredNumerator[0]);
                if (desiredUnit == null)
                    throw new Exception("Unit unknown from the unit system, cannot perform the conversion");
                else
                {//try to find the corresponding SI unit
                    AQuantity qty = Greet.UnitLib3.Units.QuantityList.Values.FirstOrDefault(item => item.Units.Contains(desiredUnit));
                    Unit siUnitMatch = qty.SiUnit;//we expect here a simple unit such as J or kg, not a complex expression
                    if (siNumerator.Contains(siUnitMatch.Expression))
                    {//we have a match, perform the conversion
                        convertedValue *= desiredUnit.ToSI(1);
                        desiredNumerator.Remove(desiredUnit.Expression);
                        siNumerator.Remove(siUnitMatch.Expression);
                    }
                }
                safetyInt++;
            }

            safetyInt = 0;
            while (desiredDenominator.Count > 0 && safetyInt < 10000)
            {//perform conversion for denominator units
                Unit desiredUnit = Greet.UnitLib3.Units.UnitsList.Values.FirstOrDefault(item => item.Expression == desiredDenominator[0]);
                if (desiredUnit == null)
                    throw new Exception("Unit unknown from the unit system, cannot perform the conversion");
                else
                {//try to find the corresponding SI unit
                    AQuantity qty = Greet.UnitLib3.Units.QuantityList.Values.FirstOrDefault(item => item.Units.Contains(desiredUnit));
                    Unit siUnitMatch = qty.SiUnit;//we expect here a simple unit such as J or kg, not a complex expression
                    if (siDenominator.Contains(siUnitMatch.Expression))
                    {//we have a match, perform the conversion
                        convertedValue /= desiredUnit.ToSI(1);
                        desiredDenominator.Remove(desiredUnit.Expression);
                        siNumerator.Remove(siUnitMatch.Expression);
                    }
                }
                safetyInt++;
            }

            if (safetyInt == 10000)
                throw new Exception("Something went wrong during the conversion, we reached the maximum 10k iterations, please communicate the error to the development team");

            return convertedValue;

        }
        /// <summary>
        /// Uses a list of units and exponents to build two lists of top and bottom dimensions.
        /// This method is used for ConvertFromSpecificToSI and ConvertFromSIToSpecifi only
        /// </summary>
        /// <param name="units">List of units</param>
        /// <param name="exponents">List of exponents for each of the units</param>
        /// <returns>Two lists, Numerator units indexed at 0 and Denomintator unit indexed at 1</returns>
        private static List<List<string>> ToUnitExpression(List<Unit> units, List<int> exponents)
        {
            List<string> top = new List<string>();
            List<string> bottom = new List<string>();
            if (units.Count == 1 && exponents.Count == 1 && exponents[0] == 0)
            {//if we have only one unit with exponent 0 it means that we have a case such as J/J or kg/kg
                //as this method is used for displaying unit to the user, we do not want to over simplify and will keep that as a special case
                top.Add(units[0].Expression);
                bottom.Add(units[0].Expression);
            }
            else
            {
                for (int i = 0; i < units.Count; i++)
                {
                    for (int j = 0; j < Math.Abs(exponents[i]); j++)
                    {
                        if (exponents[i] > 0)
                            top.Add(units[i].Expression);
                        else if (exponents[i] < 0)
                            bottom.Add(units[i].Expression);
                    }
                }
            }
            return new List<List<string>>() { top, bottom };
        }

        public static double ConvertFromSIToSpecific(double valueInSI, string specific)
        {
            List<Unit> specUnits = new List<Unit>();
            List<int> specExp = new List<int>();
            GuiUtils.ParseExpression(specific, out specUnits, out specExp);
            List<Unit> defUnits = new List<Unit>();
            foreach (Unit u in specUnits)
            {
                AQuantity qty = Greet.UnitLib3.Units.QuantityList.Values.FirstOrDefault(item => item.Units.Contains(u));
                defUnits.Add(qty.SiUnit);
            }

            List<List<string>> desiredUnits = ToUnitExpression(specUnits, specExp);
            List<List<string>> siUnits = ToUnitExpression(defUnits, specExp);

            //perform conversion unit by unit
            double convertedValue = valueInSI;
            List<string> desiredNumerator = desiredUnits[0];
            List<string> siNumerator = siUnits[0];
            List<string> desiredDenominator = desiredUnits[1];
            List<string> siDenominator = siUnits[1];

            int safetyInt = 0;
            while (desiredNumerator.Count > 0 && safetyInt < 10000)
            {//perform conversion for numerator units
                Unit desiredUnit = Greet.UnitLib3.Units.UnitsList.Values.FirstOrDefault(item => item.Expression == desiredNumerator[0]);
                if (desiredUnit == null)
                    throw new Exception("Unit unknown from the unit system, cannot perform the conversion");
                else
                {//try to find the corresponding SI unit
                    AQuantity qty = Greet.UnitLib3.Units.QuantityList.Values.FirstOrDefault(item => item.Units.Contains(desiredUnit));
                    Unit siUnitMatch = qty.SiUnit;//we expect here a simple unit such as J or kg, not a complex expression
                    if (siNumerator.Contains(siUnitMatch.Expression))
                    {//we have a match, perform the conversion
                        convertedValue = convertedValue*desiredUnit.Si_slope + desiredUnit.Si_intercept;
                        desiredNumerator.Remove(desiredUnit.Expression);
                        siNumerator.Remove(siUnitMatch.Expression);
                    }
                }
                safetyInt++;
            }

            safetyInt = 0;
            while (desiredDenominator.Count > 0 && safetyInt < 10000)
            {//perform conversion for denominator units
                Unit desiredUnit = Greet.UnitLib3.Units.UnitsList.Values.FirstOrDefault(item => item.Expression == desiredDenominator[0]);
                if (desiredUnit == null)
                    throw new Exception("Unit unknown from the unit system, cannot perform the conversion");
                else
                {//try to find the corresponding SI unit
                    AQuantity qty = Greet.UnitLib3.Units.QuantityList.Values.FirstOrDefault(item => item.Units.Contains(desiredUnit));
                    Unit siUnitMatch = qty.SiUnit;//we expect here a simple unit such as J or kg, not a complex expression
                    if (siDenominator.Contains(siUnitMatch.Expression))
                    {//we have a match, perform the conversion
                        convertedValue *= desiredUnit.ToSI(1);
                        desiredDenominator.Remove(desiredUnit.Expression);
                        siNumerator.Remove(siUnitMatch.Expression);
                    }
                }
                safetyInt++;
            }

            if (safetyInt == 10000)
                throw new Exception("Something went wrong during the conversion, we reached the maximum 10k iterations, please communicate the error to the development team");

            return convertedValue;
        }

        #endregion static methods

        #region UnitLib API
        [Obsolete("OLD UnitLib API")]
        public string SIUnitStr
        {
            get { return ConversionFromOLDUnitLib.NewFormula2OldUnit[this.Units[0].Expression]; }
        }
        [Obsolete("OLD UnitLib API")]
        public string Abbrev
        {
            get { return ConversionFromOLDUnitLib.NewFormula2OldUnit[this.Units[0].Expression]; }
        }
        [Obsolete("OLD UnitLib API")]
        public double ConvertFromDefaultToOverride(double valueToConvert)
        {
            return this.ConvertFromSI(this.PreferedUnitIdx, valueToConvert);
        }
        [Obsolete("OLD UnitLib API")]
        public double ConvertFromOverrideToDefault(double value)
        {
            throw new NotImplementedException();
        }
        [Obsolete("OLD UnitLib API")]
        public bool DefaultOnlyEquals(AQuantity baseQuantity)
        {
            return baseQuantity.Dim == this.Dim;
        }
        [Obsolete("OLD UnitLib API")]
        public delegate void UnitChangedDelegate();
        /// <summary>
        /// Occurs when the overrideUnit changes.
        /// </summary>
        [NonSerialized]
#pragma warning disable 618
        public UnitChangedDelegate UnitChanged;
#pragma warning restore 618
        [Obsolete("OLD UnitLib API")]
        public event UnitChangedDelegate UnitChangedEvent
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                UnitChanged = (UnitChangedDelegate)Delegate.Combine(UnitChanged, value);
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                UnitChanged = (UnitChangedDelegate)Delegate.Remove(UnitChanged, value);
            }
        }
        [Obsolete("OLD UnitLib API")]
        public string DisplayUnitStr
        {
            get { return this.Units[0].Expression; }
        }
        [Obsolete("OLD UnitLib API")]
        public string DefaultUnitAbbrev
        {
            get { return this.Units[0].Expression; }
        }
        [Obsolete("OLD UnitLib API")]
        public string DisplayName { get; set; }
        [Obsolete("OLD UnitLib API")]
        public string format
        {
            get
            { return GuiUtils.DefaultFormatting; }
        }
        [Obsolete("OLD UnitLib API")]
        public bool createdByTheCalculations
        {
            get
            { return false; }
        }
        [Obsolete("OLD UnitLib API")]
        public List<string> MemberUnits
        { 
            get {
                    return this.Units.Select(u => u.Expression).ToList();
            }
        }
        [Obsolete("OLD UnitLib API")]
        public Unit OverrideUnit
        {
            get {
                return this.Units[this.PreferedUnitIdx];
            }
            set
            {
                this._preferredUnitIdx = this.Units.IndexOf(value);
            }
        }
        #endregion

        /// <summary>
        /// <para>To be used when prefered unit is -1 or for derived quantities that do no store their prefered unit</para>
        /// <para>Decomposes the basic units that makes that derived quantity, find their quantities and respective prefered units</para>
        /// </summary>
        /// <returns>The combined respective prefered units combined</returns>
        public string DecomposePreferedExpression()
        {
            Unit preferedUnit = null;
            if (this.Units.Count >= _preferredUnitIdx)
                preferedUnit = this.Units[_preferredUnitIdx];
            else
                preferedUnit = this.Units[0];
            List<Unit> units = null;
            List<int> expo = null;
            GuiUtils.ParseExpression(preferedUnit.Expression, out units, out expo);

            List<Unit> preferedUnits = new List<Unit>();
            foreach (Unit u in units)
            {
                AQuantity qty = UnitLib3.Units.QuantityList.Values.FirstOrDefault(item => item.Units.Any(q => q.Expression == u.Expression));
                Unit prefered = qty.Units[qty.PreferedUnitIdx];
                preferedUnits.Add(prefered);
            }

            string filteredExpressionTop = "";
            string filteredExpressionBottom = "";
            for (int i = 0; i < preferedUnits.Count; i++)
            {
                string uStr = preferedUnits[i].Expression;
                int uExp = expo[i];
                if (uExp > 1)
                    filteredExpressionTop += " " + uStr + "^" + uExp;
                else if (uExp == 1)
                    filteredExpressionTop += " " + uStr;
                else if (uExp == -1)
                    filteredExpressionBottom += " " + uStr;
                else if (uExp < -1)
                    filteredExpressionBottom += " " + uStr + "^" + -uExp;

            }
            if (units.Count == 1 && expo.Count == 1 && expo[0] == 0)
            { //if we have only one unit with exponent 0 it means that we have a case such as J/J or kg/kg
                //as this method is used for displaying unit to the user, we do not want to over simplify and will keep that as a special case
                filteredExpressionTop += " " + units[0].Expression; ;
                filteredExpressionBottom += " " + units[0].Expression; ;
            }

            if (!String.IsNullOrEmpty(filteredExpressionBottom))
            {
                string returned = filteredExpressionTop.TrimStart(' ') + "/";
                if (filteredExpressionBottom.TrimStart(' ').Contains(' '))
                    returned += "(";
                returned += filteredExpressionBottom.TrimStart(' ');
                if (filteredExpressionBottom.TrimStart(' ').Contains(' '))
                    returned += ")";
                return returned;
            }
            else
                return filteredExpressionTop.Trim(' ');
            
        }
    }
    
}
