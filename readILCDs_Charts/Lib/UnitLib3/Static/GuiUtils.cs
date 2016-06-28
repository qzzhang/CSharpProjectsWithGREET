using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Globalization;

namespace Greet.UnitLib3
{
    public static class GuiUtils
    {

        #region properties
        public static string DefaultFormatting
        { get { return "E3"; } }
        #endregion
    
        #region internal methods

        static string pattern = @"\(?([a-zA-Z\$\%]+\^?\(?\-?\d{0,3})\)?\s*\*?\s*\)?"; //(kg shtn m^3 s^-3) -> [kg, shtn, m^3, s^-3]
        static Regex rgx = new Regex(pattern);
        /// <summary>
        /// Parse an expression and returns a list of references to Units and their exponent
        /// Filters out plural 's' at the end of units
        /// </summary>
        /// <param name="expression">A string that contains the expression for the unit, i.e. (kg *   lb m^3 s^-3)</param>
        /// <param name="units">List of atomic units , i.e [kg,lb,m,s]</param>
        /// <param name="exponents">List of exponents associated with atomic units, i.e. [1,1,3,-3]</param>
        internal static void ParseSimpleExpression(string expression, out  List<Unit> units, out List<int> exponents)
        {
            MatchCollection matches = rgx.Matches(expression);
            units = new List<Unit>();
            exponents = new List<int>();
            string unit_exponenta, unit;
            string[] unit_exponenta_splitted;
            int exponenta;
            foreach (Match match in matches)
            {
                unit_exponenta = match.Groups[1].Value;
                unit_exponenta_splitted = unit_exponenta.Split('^');
                unit = unit_exponenta_splitted[0];
                if (unit_exponenta_splitted.Count() > 1)
                    exponenta = System.Convert.ToInt32(unit_exponenta_splitted[1].Trim('(').Trim(')'));
                else
                    exponenta = 1;
                Unit u;
                if ((u = Units.UnitsList.Values.FirstOrDefault(item => item.Expression == unit)) != null
                    || (u = Units.UnitsList.Values.FirstOrDefault(item => item.Expression == unit.TrimEnd('s'))) != null
                    || (u = Units.UnitsList.Values.FirstOrDefault(item => item.Name == unit)) != null
                    || (u = Units.UnitsList.Values.FirstOrDefault(item => item.Name == unit.TrimEnd('s'))) != null)
                {//we found a unit name or expression that matches the user entered string
                    if (!units.Contains(u))
                    {
                        units.Add(u);
                        exponents.Add(exponenta);
                    }
                    else
                    {
                        int idx = units.IndexOf(u);
                        exponents[idx]++;
                    }
                }
                else if(Units.UnitsList.Values.Count(item => item.Expression.ToLower() == unit.ToLower()) <= 1
                    && Units.UnitsList.Values.Count(item => item.Expression.ToLower() == unit.TrimEnd('s').ToLower()) <= 1)
                {//we'll try to see if we can find a single unit that matches with lowercase, if more than one match we throw an exception
                    //this will for example allow mmBtu MMBtu mmBTU to be matched, but will prevent the confusion of MJ (mega joule) and mJ (milli joule)
                    u = Units.UnitsList.Values.FirstOrDefault(item => item.Expression.ToLower() == unit.ToLower());
                    if(u == null)
                        u = Units.UnitsList.Values.FirstOrDefault(item => item.Expression.ToLower() == unit.TrimEnd('s').ToLower());

                    if (!units.Contains(u))
                    {
                        units.Add(u);
                        exponents.Add(exponenta);
                    }
                    else
                    {
                        int idx = units.IndexOf(u);
                        exponents[idx]++;
                    }
                }
                else
                    throw new ExpressionStringParingException();
            }

        }
        /// <summary>
        /// Parse a properly formated string of units with a single or none '/' character
        /// </summary>
        /// <param name="expression">Expression to parse</param>
        /// <param name="units">Units used</param>
        /// <param name="exponents">Exponents of units used</param>
        internal static void ParseExpression(string expression, out  List<Unit> units, out List<int> exponents)
        {
            string[] to_splitted = expression.Trim().Split('/');
            units = new List<Unit>();
            exponents = new List<int>();
            if (to_splitted[0].Trim() != "1")
            {
                ParseSimpleExpression(to_splitted[0], out units, out exponents);
            }
            if (to_splitted.Count() > 1)
            {
                List<Unit> bottom_units;
                List<int> bottom_exponents;
                ParseSimpleExpression(to_splitted[1].Trim(), out bottom_units, out bottom_exponents);
                for (int i = 0; i < bottom_units.Count; i++)
                {
                    if (units.Contains(bottom_units[i]))
                    {
                        int idx = units.IndexOf(bottom_units[i]);
                        exponents[idx] -= bottom_exponents[i];
                    }
                    else
                    {
                        units.Add(bottom_units[i]);
                        exponents.Add(-bottom_exponents[i]);
                    }
                }
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Formats a value according to the options chosen by the user in the main form
        /// </summary>
        /// <param name="valueSIUnit">The numerical value to be formated</param>
        /// <param name="format">format: 0 unitGroupFormat, 1 scientific notation, 2 all digits</param>
        /// <param name="autoSelectedExpression">Returns the unit and prefixes if necessary</param>
        /// <param name="slope">Returns the slope used in the unit conversion</param>
        /// <param name="automaticScaling">If scaling is selected in the options the method will use prefixes like k for kilo, M for mega...
        /// <param name="scientificFigures">Number of decimal points to be displayed when scientific format is used (format = 1)
        /// <param name="dim">Dimension of the valueSI</param>
        /// <param name="preferedUnit">Prefered unit if something else than the default is desired</param>
        /// Setting this option to false will disable that automatic scaling feature whatever the user selection is</param>
        /// <returns>Formated value</returns>
        public static string FormatSIValue(double valueSIUnit,
            int format,
            out string autoSelectedExpression,
            out double slope,
            bool automaticScaling,
            int scientificFigures,
            uint dim,
            string preferedUnit)
        {
            string formatedValue = "";
            slope = 1;

            AQuantity qty = Units.QuantityList.ByDim(dim);
            autoSelectedExpression = "";
            if (String.IsNullOrEmpty(preferedUnit) && qty != null)
            { //if no unit preference and a group is found
                autoSelectedExpression = qty.Units[qty.PreferedUnitIdx].Expression;
            }
            else if (!String.IsNullOrEmpty(preferedUnit))  //if we have a unit preference
                autoSelectedExpression = preferedUnit;
            else
                throw new Exception("Either the dimension of a known quantity must be defined, or a prefered unit string. Without that we can't guess the way this value should be represented");

            double valueInOverrideUnit = AQuantity.ConvertFromSIToSpecific(valueSIUnit, autoSelectedExpression);
            slope = AQuantity.ConvertFromSIToSpecific(1, autoSelectedExpression);
            if (format == 1)  //scientific notation format
            {
                string pounds = new string('0', Math.Max(0,scientificFigures-1));
                formatedValue = valueInOverrideUnit.ToString("0."+ pounds + "e-0", CultureInfo.InvariantCulture);
                slope = AQuantity.ConvertFromSIToSpecific(1, autoSelectedExpression);
            }
            else if (format == 0) //default format
            {
                double prefixPower = 1;
                double valueInOverrideBeforePrefixing = valueInOverrideUnit;
                if (valueInOverrideUnit != 0 && automaticScaling)
                    valueInOverrideUnit = FormatPrefix(valueInOverrideUnit, autoSelectedExpression, out autoSelectedExpression, out prefixPower);
                if (valueInOverrideUnit == 0)
                    formatedValue = valueInOverrideUnit.ToString("G");
                else if (Math.Abs(valueInOverrideBeforePrefixing) > 1E6) //to remove unecessary digits for large numbers
                    formatedValue = valueInOverrideUnit.ToString("F0");
                else if (Math.Abs(valueInOverrideUnit) > 1E-3) //to display "regular" values using de default OS settings
                    formatedValue = valueInOverrideUnit.ToString("F2");
                else//to display very small values as scientific notation
                    formatedValue = valueInOverrideUnit.ToString("0.##e-0", CultureInfo.InvariantCulture);
                slope /= prefixPower;
            }
            else if (format == 2) //all digits format
            {
                double prefixPower = 1;
                if (valueInOverrideUnit != 0 && automaticScaling)
                    valueInOverrideUnit = FormatPrefix(valueInOverrideUnit, autoSelectedExpression, out autoSelectedExpression, out prefixPower);
                formatedValue = valueInOverrideUnit.ToString("G");
                slope /= prefixPower;
            }

            return formatedValue;
            
        }

        /// <summary>
        /// Takes as an input a value in override unit and tries to multiply or divide by 10 until a prefix 
        /// can be found for the top and or bottom unit. Returns the values and outptus the new unit to be shown
        /// and power of 10 used to convert from the prefixed value to the value in override unit.
        /// 
        /// Example: Calling the method with FormatPrefix(1250000, "energy", unit, pow);
        /// will return 1.25, unit = mmBtu, pow = 6
        /// </summary>
        /// <param name="valueInOverrideUnit">Value in override unit to be converted</param>
        /// <param name="unit_group">Unit group in which the value is defined</param>
        /// <param name="prefixedUnit">Returns the unit and prefixes if necessary</param>
        /// <param name="power">Returns the power of 10 used for the conversion</param>
        /// <returns>Returns the value multiplied by the power of 10 returned</returns>
        private static double FormatPrefix(double valueInOverrideUnit, string expression, out string autoExpression, out double slope)
        {
            //storing temporary the factors used to change between the unit prefixes or units
            double value = valueInOverrideUnit;
            autoExpression = "";
            slope = 1;
            List<Unit> units;          
            List<int> exponents;
            ParseExpression(expression, out units, out exponents);
            List<Unit> replacements = new List<Unit>(units.ToArray());
            string valueString = value.ToString("F15");

            //try to make acceptable by changing numerator
            for (int i = 0; i < replacements.Count; i++)
            {         
                int exponent = exponents[i];
                if (exponent > 0)
                {
                    int safety = 0;
                    bool keepTrying = true; //indicates we can keep going with a higher unit if we have one
                    int direction = 0;//indicates weather we're going towards larger units or smaller units and prevent oscillations
                    while (keepTrying && safety < 10000)
                    {
                        Unit u = replacements[i];
                        safety++;
                        int lz = leadingZeros(valueString);
                        int ld = leadingDigits(valueString);
                        if (lz > 1 && !String.IsNullOrEmpty(u.BelowName) && direction <=0)
                        {
                            Unit belowUnit = Units.UnitsList[u.BelowName];
                            double factor = Math.Pow(belowUnit.Si_slope / u.Si_slope, exponents[i]);
                            value *= factor;
                            slope *= factor;
                            replacements[i] = belowUnit;
                            valueString = value.ToString("F15");
                            direction = -1;
                        }
                        else if (ld > 4 && !String.IsNullOrEmpty(u.AboveName) && direction >= 0)
                        {
                            Unit aboveUnit = Units.UnitsList[u.AboveName];
                            double factor = Math.Pow(aboveUnit.Si_slope / u.Si_slope, exponents[i]);
                            value *= factor;
                            slope *= factor;
                            replacements[i] = aboveUnit;
                            valueString = value.ToString("F15");
                        }
                        else
                            keepTrying = false;
                    }
                }
            }

            //build a new expression from the replacements
            List<String> top = new List<string>();
            List<String> bottom = new List<string>();



            if (replacements.Count == 1 && exponents.Count == 1 && exponents[0] == 0)
            {//if we have only one unit with exponent 0 it means that we have a case such as J/J or kg/kg
                //as this method is used for displaying unit to the user, we do not want to over simplify and will keep that as a special case
                top.Add(units[0].Expression);
                bottom.Add(units[0].Expression);
            }
            else
            {//if we have any other case we assign units to the top and the bottom parts
                for (int i = 0; i < replacements.Count; i++)
                {
                    for (int j = 0; j < Math.Abs(exponents[i]); j++)
                    {
                        if (exponents[i] > 0)
                            top.Add(replacements[i].Expression);
                        else if (exponents[i] < 0)
                            bottom.Add(replacements[i].Expression);
                    }
                }
            }

            string replacementExpression = "";
            //Find equivalent dim and SI slope and intercept
            foreach (string unit in top)
                replacementExpression += "*" + unit;
            if (bottom.Count > 0)
            {
                replacementExpression += "/";
                if(bottom.Count > 1)    
                    replacementExpression += "(";
                foreach (string unit in bottom)
                    replacementExpression += unit + " ";

                replacementExpression = replacementExpression.TrimEnd(' ');
                if(bottom.Count > 1)
                    replacementExpression += ")";
            }

            autoExpression = CombineUnits(replacementExpression.TrimStart('*'));
            if (String.IsNullOrEmpty(autoExpression) && !String.IsNullOrEmpty(replacementExpression.TrimStart('*')))
                autoExpression = replacementExpression.TrimStart('*');//there was some simplification
            return value;
        }

        private static int leadingZeros(string value)
        {
            bool dot = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != '0' && value[i] != '.')
                    return dot ? i-1 : i;
                else if (value[i] != '.')
                    dot = true;
            }
            return dot ? value.Length - 1 : value.Length;
        }

        private static int leadingDigits(string value)
        {
            bool leadingZero = value.Length >= 1 && value[0] == '0';
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '.')
                    return leadingZero ? i-1 : i;
            }
            return value.Length;
        }


        /// <summary>
        /// Splits an expression into the top and bottom parts by finding divisions and multiplications characters
        /// </summary>
        /// <param name="unitExpression">Expression to be parsed</param>
        /// <param name="invert">If true the resulted split will be inverted (numerator goes denominoator)</param>
        /// <param name="simplify">If true, common numerator and denominator units will be simplified</param>
        /// <returns>Two lists, Numerator units indexed at 0 and Denomintator unit indexed at 1</returns>
        public static List<List<string>> SplitString(string unitExpression, bool invert, bool simplify = true)
        {
            string[] parts = unitExpression.Split(new char[] { '/', '*' }, StringSplitOptions.RemoveEmptyEntries);
            int char_number = 0;
            List<string> top = new List<string>();
            List<string> bottom = new List<string>();
            foreach (string part in parts)
            {
                if (part != "1" && string.IsNullOrEmpty(part) == false)
                {
                    bool denom = (char_number != 0 && (unitExpression[char_number - 1] == '/')) ^ invert;
                    if (denom == true && top.Contains(part) == false)
                        bottom.Add(part.TrimStart('(').TrimEnd(')'));
                    else if (denom == false && bottom.Contains(part) == false)
                        top.Add(part.TrimStart('(').TrimEnd(')'));
                    else if (denom == true && top.Contains(part) == true && simplify)
                        top.Remove(part);
                    else if (denom == false && bottom.Contains(part) == true && simplify)
                        bottom.Remove(part);
                }
                char_number += part.Length + 1;
            }
            return new List<List<string>>() { top, bottom };
        }

        /// <summary>
        /// Converts value in SI units to the dimensions specified by the expression string (to)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="value">in SI units</param>
        /// <returns></returns>
        public static double ConvertFromSI(string to, double value)
        {
            double res = value;
            string[] to_splitted = to.Split('/');
            List<Unit> units;
            List<int> exponents;
            ParseExpression(to, out units, out exponents);
            for (int i = 0; i < units.Count; i++)
            {
                if (exponents[i] == 0)
                    continue;
                if (exponents[i] > 0)
                {
                    for (int j = 1; j <= exponents[i]; j++)
                        res = units[i].FromSI(res);
                }
                else
                {
                    for (int j = 1; j <= -exponents[i]; j++)
                    {
                        res = units[i].ToSI(res);
                    }
                }
            }
            return res;
        }

        public static double ConvertToSI(string from, double value)
        {
            double res = value;
            string[] to_splitted = from.Split('/');
            List<Unit> units;
            List<int> exponents;
            ParseExpression(from, out units, out exponents);
            for (int i = 0; i < units.Count; i++)
            {
                if (exponents[i] == 0)
                    continue;
                if (exponents[i] > 0)
                {
                    for (int j = 1; j <= exponents[i]; j++)
                        res = units[i].ToSI(res);
                }
                else
                {
                    for (int j = 1; j <= -exponents[i]; j++)
                    {
                        res = units[i].FromSI(res);
                    }
                }
            }
            return res;
        }
        /// <summary>
        /// Returns an uint from a string in si units
        /// for example this method can take as an input m/s and return the corresponding uint
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static uint CreateDim(string p)
        {
            List<Unit> units;
            List<int> exponents;
            uint res = 0;
            ParseExpression(p, out units, out exponents);
            AQuantity q;
            for (int i = 0; i < units.Count; i++)
            {
                if (exponents[i] == 0)
                    continue;
                q = Units.U2Q[units[i]];
                res = DimensionUtils.Plus(res, DimensionUtils.Times(q.Dim, exponents[i]));
            }
            return res;
        }

        public static LightValue CreateLightValue(double val, string units_formula)
        {
            val = ConvertToSI(units_formula, val);
            return new LightValue(val, CreateDim(units_formula));
        }
        public static LightValue CreateLightValue(string value_units_formula)
        {
            double val;
            uint dim;
            ExtarctValueDimension(value_units_formula, out val, out dim);
            return new LightValue(val, dim);
        }
        /// <summary>
        /// Generates a string of this format: "10.234 Btu/lb". 
        /// </summary>
        /// <param name="lv"></param>
        /// <param name="units_formula">Units expressed as formula, for example "Btu/lb"</param>
        /// <returns></returns>
        public static string LightValueInUnits(LightValue lv, string units_formula)
        {
            string res;
            double non_si_value;
            non_si_value = ConvertFromSI(units_formula, lv.Value);
            res = Math.Round(non_si_value, 3).ToString() + " " + units_formula;
            return res;
        }

        /// <summary>
        /// This method takes a string such as 23 g/Btu and checks if it is the same quantity as currenntly stored.
        /// If it is it updates the Value member of the object, if not it does not do anything
        /// </summary>
        /// <param name="expression"></param>
        public static void UpdateLightValue(LightValue lv, string expression)
        {
            uint ldim;
            double value;
            bool flag;
            try
            {
                flag = ExtarctValueDimension(expression, out value, out ldim);
            }
            catch (Exception)
            {
                return;
            }
            if (!flag)
                return;
            if (ldim == lv.Dim)
                lv.Value = value;
        }
        /// <summary>
        /// Returns the name of the denominator quantity name of a DerivedQuantity
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        public static string BottomQuantityName(uint dim)
        {
            string res = "";
            if (Units.Dim2Quantities.ContainsKey(dim))
            {
                AQuantity q = Units.Dim2Quantities[dim][0];
                if (q is DerivedQuantity)
                {
                    res = ((DerivedQuantity)q).Bottom;
                }
            }
            return res;
        }
        /// <summary>
        /// Returns the name of the numerator quantity name of a DerivedQuantity
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        public static string TopQuantityName(uint dim)
        {
            string res = "";
            if (Units.Dim2Quantities.ContainsKey(dim))
            {
                AQuantity q = Units.Dim2Quantities[dim][0];
                if (q is DerivedQuantity)
                {
                    res = ((DerivedQuantity)q).Top;
                }
            }
            return res;
        }

        /// <summary>
        /// Returns the name of the numerator quantity name of a DerivedQuantity
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        public static string QuantityName(uint dim)
        {
            string res = "";
            if (Units.Dim2Quantities.ContainsKey(dim))
            {
                AQuantity q = Units.Dim2Quantities[dim][0];
                res = q.Name;
            }
            return res;
        }

        public static uint FromQuantityName(string qname)
        {
            Debug.Assert(Units.QName2Q.ContainsKey(qname), String.Format("Quantity {0} is not defined", qname));
            return Units.QName2Q[qname].Dim;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Exctract value and dimension by separating the numbers from the unit expression
        /// Uses the current computer locale to perform the conversion, not the en-US, so numbers passed as arguments can be represented as 1.234 or 1,234 depending on the user's windows settings
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="val_">Value is SI units</param>
        /// <param name="dim_">Corrsponding dimension</param>
        private static bool ExtarctValueDimension(string expression, out double val_, out uint dim_)
        {

            val_ = 0.0;
            dim_ = 0;
            if (expression.Trim() != string.Empty)
            {
                string pattern = @"\A\s*(\-?\s*\d+\.?\d*|NaN|Infinity)\s*(.*\z)"; //   0.258    (kWh/g^3) -> [0.258,(kWh/g^3)]
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                if (rgx.Matches(expression).Count == 0)
                    return false;
                Match match = rgx.Matches(expression)[0];
                try
                {
                    val_ = Convert.ToDouble(match.Groups[1].Value); //Uses the current computer locale to perform the conversion, not the en-US, so numbers passed as arguments can be represented as 1.234 or 1,234 depending on the user's windows settings
                    dim_ = GuiUtils.CreateDim(match.Groups[2].Value); 
                }
                catch (Exception)
                {
                    throw new Exception("ExtarctValueDimension was not able to parse the entered value");
                }
                val_ = GuiUtils.ConvertToSI(match.Groups[2].Value, val_);
            }
            return true;
        } 

        #endregion

        /// <summary>
        /// This methods parses an expression, tries sigular and plural expressions from the UnitLib3 system. And calculate attributes as if we were trying to determine a quantity from the string
        /// </summary>
        /// <param name="expression">An expression such as lbs/J</param>
        /// <param name="equivalentSIExpression">The expession with the same dimension in SI units</param>
        /// <param name="equivalentUSERExpression">The expression with the same dimension in the units given by the users, simplified, and corrected to singular instead of plural</param>
        /// <param name="equivalentDim">The dimension equivalent to that expression</param>
        /// <param name="equivalentSlope">The slope to convert to SI from the given expression to parse to the equivalentSIExpression</param>
        /// <param name="equivalentIntercept">The intercept to convert to SI from the given expression to parse to the equivalentSIExpression</param>
        public static void FilterExpression(string expression, out string equivalentSIExpression, out string equivalentUSERExpression, out uint equivalentDim, out double equivalentSlope, out double equivalentIntercept)
        {
            //Split the given formula in lists of units
            List<Unit> units = new List<Unit>();
            List<int> exp = new List<int>();
            GuiUtils.ParseExpression(expression, out units, out exp);
            List<String> top = new List<string>();
            List<String> bottom = new List<string>();

            if (units.Count == 1 && exp.Count == 1 && exp[0] == 0)
            {//if we have only one unit with exponent 0 it means that we have a case such as J/J or kg/kg
                //as this method is used for displaying unit to the user, we do not want to over simplify and will keep that as a special case
                top.Add(units[0].Expression);
                bottom.Add(units[0].Expression);
            }
            else
            {//if we have any other case we assign units to the top and the bottom parts
                for (int i = 0; i < units.Count; i++)
                {
                    for (int j = 0; j < Math.Abs(exp[i]); j++)
                    {
                        if (exp[i] > 0)
                            top.Add(units[i].Expression);
                        else if (exp[i] < 0)
                            bottom.Add(units[i].Expression);
                    }
                }
            }

            equivalentSIExpression = equivalentUSERExpression = "";
            equivalentSlope = 1;
            equivalentIntercept = 0;
            equivalentDim = 0;
            //Find equivalent dim and SI slope and intercept
            foreach (string unit in top)
            {
                AQuantity qtyMatch = Greet.UnitLib3.Units.QuantityList.Values.First(item => item.Units.Any(u => u.Expression == unit));
                equivalentDim = DimensionUtils.Plus(equivalentDim, qtyMatch.Dim);
                equivalentSIExpression += "*" + qtyMatch.SiUnit.Expression;
                equivalentUSERExpression += "*" + unit;
                equivalentSlope /= qtyMatch.Units.FirstOrDefault(item => item.Expression == unit).Si_slope;
            }
            if (bottom.Count > 0)
            {
                equivalentSIExpression += "/";
                equivalentUSERExpression += "/";
                if (bottom.Count > 1)
                {
                    equivalentSIExpression += "(";
                    equivalentUSERExpression += "(";
                }
                foreach (string unit in bottom)
                {
                    AQuantity qtyMatch = Greet.UnitLib3.Units.QuantityList.Values.First(item => item.Units.Any(u => u.Expression == unit));
                    equivalentDim = DimensionUtils.Minus(equivalentDim, qtyMatch.Dim);
                    equivalentSIExpression += qtyMatch.SiUnit.Expression + " ";
                    equivalentUSERExpression += unit + " ";
                    equivalentSlope *= qtyMatch.Units.FirstOrDefault(item => item.Expression == unit).Si_slope;
                }
                equivalentSIExpression = equivalentSIExpression.TrimEnd(' ');
                equivalentUSERExpression = equivalentUSERExpression.TrimEnd(' ');
                if (bottom.Count > 1)
                {
                    equivalentSIExpression += ")";
                    equivalentUSERExpression += ")";
                }
            }

            equivalentSIExpression = CombineUnits(equivalentSIExpression.TrimStart('*'));
            equivalentUSERExpression = CombineUnits(equivalentUSERExpression.TrimStart('*'));
        }

        /// <summary>
        /// Combines units and uses the notation x^y if a unit needs to be elevated to some power
        /// </summary>
        /// <param name="expression">An expression for example J/(m m m)</param>
        /// <returns>Elevate power expression for example J/(m^3) if J/(m m m) was entered</returns>
        private static string CombineUnits(string expression)
        {
            List<Unit> units = new List<Unit>();
            List<int> exp = new List<int>();
            string filteredExpressionTop = "";
            string filteredExpressionBottom = "";
            GuiUtils.ParseExpression(expression, out units, out exp);
            for (int i = 0; i < units.Count; i++)
            {
                string uStr = units[i].Expression;
                int uExp = exp[i];
                if (uExp > 1)
                    filteredExpressionTop += " " + uStr + "^" + uExp;
                else if (uExp == 1)
                    filteredExpressionTop += " " + uStr;
                else if (uExp == -1)
                    filteredExpressionBottom += " " + uStr;
                else if (uExp < -1)
                    filteredExpressionBottom += " " + uStr + "^" + -uExp;

            }
            if (units.Count == 1 && exp.Count == 1 && exp[0] == 0)
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
