using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Greet.ConvenienceLib;
using System.Globalization;

namespace Greet.UnitLib
{
    /// <summary>
    /// This class manages units, unit groups, and parameters
    /// and provides Conversion between 
    /// </summary>
    [Serializable]
    public static class Units
    {
        #region attributes
        internal static Dictionary<int, Prefixes> prefixes = new Dictionary<int, Prefixes>();
        /// <summary>
        /// DO NOT ACCESS DIRECTLY, USE THE ACCESSOR, OR YOU MAY CREATE THREADING ISSUES
        /// </summary>
        private static QuantityList groups = new QuantityList();
        /// <summary>
        /// DO NOT ACCESS DIRECTLY, USE THE ACCESSOR, OR YOU MAY CREATE THREADING ISSUES
        /// </summary>
        private static Dictionary<string, Unit> units = new Dictionary<string, Unit>();
        /// <summary>
        /// US Culture info to use whenever loading saving using the Convert.ToDouble or ToString methods to data files
        /// </summary>
        public static CultureInfo USCI = new CultureInfo("en-US");
        
        #endregion

        #region constructor
        static Units()
        {
            //Default units loaded ( might be overrided by the database units )
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(UnitLib.Properties.Settings.Default.Units);

            XmlNode rootNode = doc.DocumentElement;
            ReadDB(rootNode);
        }
        #endregion

        #region accessors
        /// <summary>
        /// Contains all of the units
        /// </summary>
        /// 
        public static Dictionary<string, Unit> UnitsList
        {
            get
            {
                lock (units)
                { return units; }
            }
            set
            {
                lock (units)
                { units = value; }
            }
        }
        /// <summary>
        /// Contains all of the unit groups.
        /// </summary>
        public static QuantityList QuantityList
        {
            get
            {
                lock (groups)
                { return groups; }
            }
            set
            {
                lock (groups)
                { groups = value; }
            }
        }
        #endregion
       
        #region methods
        /// <summary>
        /// Formats a value according to the options chosen by the user in the main form
        /// </summary>
        /// <param name="valueSI">The numerical value to be formated</param>
        /// <param name="format">Optional format: 0 unitGroupFormat, 1 scientific notation, 2 all digits</param>
        /// <param name="groupName">The unit or unit group in which this value is represented</param>
        /// <param name="prefixedUnit">Returns the unit and prefixes if necessary</param>
        /// <param name="power">Returns the power of 10 used for the conversion</param>
        /// <param name="automaticScaling">If scaling is selected in the options the method will use prefixes like k for kilo, M for mega...
        /// Setting this option to false will disable that automatic scaling feature whatever the user selection is</param>
        /// <returns>Formated value</returns>
        public static string FormatSIValue(double valueSI, 
            int format, 
            string groupName, 
            out string prefixedUnit, 
            out int power, 
            bool automaticScaling, 
            int scientificFigures)
        {
            string formatedValue = "";
            power = 0;
            prefixedUnit = "";

            BaseQuantity selectedGroup = QuantityList[groupName];
            double value_in_override_unit = selectedGroup.ConvertFromDefaultToOverride(valueSI).RoundToSigFigs(14);
            prefixedUnit = selectedGroup.Abbrev;
            if (format == 1)
            {
                formatedValue = value_in_override_unit.ToString("e" + scientificFigures.ToString());
            }
            else if (format == 0)
            {
                if (value_in_override_unit != 0 && automaticScaling)
                    value_in_override_unit = FormatPrefix(value_in_override_unit, selectedGroup, out prefixedUnit, out power);
                formatedValue = value_in_override_unit.ToString(selectedGroup.format);
            }
            else if (format == 2)
            {
                if (value_in_override_unit != 0 && automaticScaling)
                    value_in_override_unit = FormatPrefix(value_in_override_unit, selectedGroup, out prefixedUnit, out power);
                formatedValue = value_in_override_unit.ToStringFull();
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
        public static double FormatPrefix(double valueInOverrideUnit, BaseQuantity unit_group, out string prefixedUnit, out int power)
        {
            //storing temporary the factors used to change between the unit prefixes or units
            double value = valueInOverrideUnit;

            //splitting the unit group in order to detect numerator and denominator units
            List<DerivedQuantityBase> base_groups_in_derived_unit_group;
            if (unit_group is DerivedQuantity)
                base_groups_in_derived_unit_group = ((DerivedQuantity)unit_group).StringToBases(unit_group.DisplayUnitStr, false);
            else
            {
                base_groups_in_derived_unit_group = new List<DerivedQuantityBase>();
                base_groups_in_derived_unit_group.Add(new DerivedQuantityBase((Quantity)unit_group, UnitsList[unit_group.DisplayUnitStr], true, null));
            }

            //create a dictionary to store the prefix per base
            Dictionary<DerivedQuantityBase, int> power_per_base = new Dictionary<DerivedQuantityBase, int>();
            foreach (DerivedQuantityBase baseunit in base_groups_in_derived_unit_group)
                power_per_base.Add(baseunit, 0);

            //we assign a value for the power of ten necessary to convert back to defaut
            power = 0;

            bool prefix_used_denominator = false;
            bool prefix_used_numerator = false;

            //first we are trying to increase/decrease the denominator
            foreach (DerivedQuantityBase my_base in base_groups_in_derived_unit_group.FindAll(item => item.Numerator == false))
            {
                //get the list of prefixes
                Prefixes pref_serie;
                if (Units.prefixes.ContainsKey(my_base.OverrideUnit.prefixes))
                    pref_serie = prefixes[my_base.OverrideUnit.prefixes];
                else
                    pref_serie = new Prefixes();

                //loop on the value unit we are in the "nice" range
                bool value_ok = false;
                bool increase_bootom = Math.Abs(value) > Math.Pow(10, pref_serie.PowerJustAbove(power_per_base[my_base]));
                while (value_ok == false && pref_serie.Count > 0)
                {
                    if (Math.Abs(value) < 1 && !increase_bootom)
                    {
                        power_per_base[my_base] += 1;
                        if (pref_serie.ContainsKey(power_per_base[my_base]))
                        {
                            value = valueInOverrideUnit * Math.Pow(10, power_per_base[my_base]);
                            prefix_used_denominator = true;
                        }
                        if (power_per_base[my_base] >= pref_serie.maxPower)
                            value_ok = true;
                    }
                    else if (Math.Abs(value) > Math.Pow(10, pref_serie.PowerJustAbove(power_per_base[my_base])) && increase_bootom)
                    {
                        power_per_base[my_base] -= 1;
                        if (pref_serie.ContainsKey(power_per_base[my_base]))
                        {
                            value = valueInOverrideUnit * Math.Pow(10, power_per_base[my_base]);
                            prefix_used_denominator = true;
                        }
                        if (power_per_base[my_base] <= pref_serie.minPower)
                            value_ok = true;
                    }
                    else
                        value_ok = true;
                }
                if (prefix_used_denominator == false)
                    power_per_base[my_base] = 0;

                power -= power_per_base[my_base];
            }

            //reset the original value so the changes to the denominator are taken into account
            valueInOverrideUnit = value;

            //now we try to increase/decrease the numerator
            foreach (DerivedQuantityBase my_base in base_groups_in_derived_unit_group.FindAll(item => item.Numerator == true))
            {
                //get the list of prefixes
                Prefixes pref_serie;
                if (Units.prefixes.ContainsKey(my_base.OverrideUnit.prefixes))
                    pref_serie = prefixes[my_base.OverrideUnit.prefixes];
                else
                    pref_serie = new Prefixes();

                //loop on the value unit we are in the "nice" range
                bool value_ok = false;
                bool increase_num = Math.Abs(value) >= Math.Pow(10, pref_serie.PowerJustAbove(power_per_base[my_base]));
                while (value_ok == false && pref_serie.Count > 0)
                {
                    if (Math.Abs(value) >= Math.Pow(10, pref_serie.PowerJustAbove(power_per_base[my_base])) && increase_num)
                    {
                        power_per_base[my_base] += 1;
                        if (pref_serie.ContainsKey(power_per_base[my_base]))
                        {
                            value = valueInOverrideUnit * Math.Pow(10, -power_per_base[my_base]);
                            prefix_used_numerator = true;
                        }
                        if (power_per_base[my_base] >= pref_serie.maxPower)
                            value_ok = true;
                    }
                    else if (Math.Abs(value) < 1 && !increase_num)
                    {
                        power_per_base[my_base] -= 1;
                        if (pref_serie.ContainsKey(power_per_base[my_base]))
                        {
                            value = valueInOverrideUnit * Math.Pow(10, -power_per_base[my_base]);
                            prefix_used_numerator = true;
                        }
                        if (power_per_base[my_base] <= pref_serie.minPower)
                            value_ok = true;
                    }
                    else
                        value_ok = true;
                }
                if (prefix_used_numerator == false)
                    power_per_base[my_base] = 0;

                power += power_per_base[my_base];
            }

            //we assign a value for the override abbrev of that double value
            prefixedUnit = "";
            foreach (KeyValuePair<DerivedQuantityBase, int> baseunit in power_per_base)
            {
                string prefix_founded = "";
                if (baseunit.Value != 0)
                {
                    Prefixes pref_serie = prefixes[baseunit.Key.OverrideUnit.prefixes];
                    prefix_founded = pref_serie[baseunit.Value];
                }
                if (baseunit.Key.Numerator == false)
                    prefixedUnit += "/";
                prefixedUnit += prefix_founded + baseunit.Key.OverrideUnit.Abbrev;
            }

            return value;
        }
        public static bool ReadDB(XmlNode rootNode)
        {
            Units.UnitsList.Clear();
            foreach (XmlNode node in rootNode.SelectNodes("units/unit"))
            {
                Unit u = new Unit(node);
                Units.UnitsList.Add(u.Name, u);
            }

            groups.Clear();
            foreach (XmlNode node in rootNode.SelectNodes("groups/group"))
            {
                Quantity base_temp = new Quantity(node);
                groups.Add(base_temp.Name, base_temp);
            }
            foreach (XmlNode node in rootNode.SelectNodes("groups/derived"))
            {
                DerivedQuantity derived_temp = new DerivedQuantity(node);
                groups.Add(derived_temp.Name, derived_temp);
            }

            prefixes.Clear();
            foreach (XmlNode node in rootNode.SelectNodes("prefixes/serie"))
            {
                prefixes.Add(Convert.ToInt32(node.Attributes["id"].Value), new Prefixes(node));
            }
            return true;
        }

        public static void Save()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = SaveDB(doc);
            UnitLib.Properties.Settings.Default.Units = node.OuterXml;
            UnitLib.Properties.Settings.Default.Save();
        }

        public static XmlNode SaveDB(XmlDocument xmlDoc)
        {
            try
            {

                XmlNode root = xmlDoc.CreateNode("unitsystem");
                XmlNode groupNode = xmlDoc.CreateNode("groups");
                XmlNode unitNode = xmlDoc.CreateNode("units");
                XmlNode prefixesNode = xmlDoc.CreateNode("prefixes");
                foreach (BaseQuantity g in groups.Values.Where<BaseQuantity>(item => item.Name.Contains("automatic") == false))
                    groupNode.AppendChild(g.ToXmlNode(xmlDoc));
                foreach (Unit u in UnitsList.Values)
                    unitNode.AppendChild(u.ToXmlNode(xmlDoc));
                foreach (KeyValuePair<int, Prefixes> pre in prefixes)
                {
                    XmlNode serieNode = xmlDoc.CreateNode("serie", xmlDoc.CreateAttr("id", pre.Key));
                    foreach (KeyValuePair<double, string> prefix in pre.Value)
                        serieNode.AppendChild(xmlDoc.CreateNode("prefix", xmlDoc.CreateAttr("abbrev", prefix.Value), xmlDoc.CreateAttr("power", prefix.Key)));

                    prefixesNode.AppendChild(serieNode);
                }
                root.AppendChild(groupNode);
                root.AppendChild(unitNode);
                root.AppendChild(prefixesNode);

                return root;
            }
            catch (Exception e)
            {
                //LogFile.Write("Error 70 : saving file \r\n" + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Converts a double value into the default unit for its group
        /// </summary>
        /// <param name="valueFrom"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        private static double ToDefault(double valueFrom, Unit from)
        {
            if(from.Si_slope != 0)
                return from.Si_slope * valueFrom + from.Si_intercept;
            else
#pragma warning disable 618
                return MathParse.Parse(from.ToDefaultStr.Replace("val", valueFrom.ToStringFull())).Value;
#pragma warning restore 618
        }
        /// <summary>
        /// Converts a double value into the desired unit for the user
        /// </summary>
        /// <param name="defValue"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static double FromDefault(double defValue, Unit to)
        {
            if (to.Si_slope != 0)
                return (defValue - to.Si_intercept) / to.Si_slope;
            else
#pragma warning disable 618
                return MathParse.Parse(to.FromDefaultStr.Replace("val", defValue.ToStringFull())).Value;
#pragma warning restore 618
        }
        /// <summary>
        /// Convert a value to a value in another unit
        /// </summary>
        /// <param name="valueFrom"></param>
        /// <param name="unitFrom"></param>
        /// <param name="unitTo"></param>
        /// <returns></returns>
        internal static double Conversion(double valueFrom, Unit unitFrom, Unit unitTo)
        {
            if (unitFrom.BaseGroupName == unitTo.BaseGroupName && unitFrom != unitTo)//WARNING somtimes here we are coming whith two groups which are clones, they have the same names but they are not the same objects, really needs to find why....
            {
                double defValue = ToDefault(valueFrom, unitFrom);
                double userValue = FromDefault(defValue, unitTo);

                return userValue;
            }
            else if (unitFrom != unitTo)
            {
                //LogFile.Write(string.Format("There is no conversion for {0} to {1}\n", unitFrom.Name, unitTo.Name));
                throw new Exception(string.Format("There is no conversion for {0} to {1}\n", unitFrom.Name, unitTo.Name));
            }
            else
                return valueFrom;
        }
        public static Unit AddUnit(string unitName, string unitAbbrev, double a, double b, string baseGroupName)
        {
            try
            {
                Unit newUnit = new Unit(unitName, unitAbbrev, a,b, baseGroupName);
                (groups[newUnit.BaseGroupName] as Quantity).MemberUnits.Add(newUnit.Name);
                Units.UnitsList.Add(newUnit.Name, newUnit);
                return newUnit;
            }
            catch (Exception e)
            {
                throw new Exception("Adding unit failed:\n" + e.Message);
            }
        }
        public static void RemoveUnit(Unit remUnit)
        {
            UnitsList.Remove(remUnit.Name);
            foreach (BaseQuantity g in groups.Values)
            {
                if (g is Quantity)
                {
                    Quantity b = g as Quantity;
                    b.MemberUnits.Remove(remUnit.Name);
                    if (b.OverrideUnit == remUnit)
                        b.OverrideUnit = b.DefaultUnit;
                }
                else
                    foreach (DerivedQuantityBase b in (g as DerivedQuantity).BaseGroups)
                        if (b.OverrideUnit == remUnit)
                            b.OverrideUnit = b.DefaultUnit;
            }
        }
        #endregion

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
                        bottom.Add(part);
                    else if (denom == false && bottom.Contains(part) == false)
                        top.Add(part);
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
        /// Removes parentheses and switches multiplications and divisions for the unit expressions
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static string InvertExpression(string expression)
        {
            string inverted = expression.Replace("(", "").Replace(")", "");
            inverted = inverted.Replace("*", "~").Replace("/", "*").Replace("~", "/");
            return inverted;
        }
        
        public static double RoundToSigFigs(this double value, int sigFigures)
        {
            // this method will return a rounded double value at a number of signifigant figures.
            // the sigFigures parameter must be between 0 and 15, exclusive.

            int roundingPosition;   // The rounding position of the value at a number of sig figures.
            double scale;           // Optionally used scaling value, for rounding whole numbers or decimals past 15 places

            // handle exceptional cases
            if (value == 0.0d) { return value; }
            if (double.IsNaN(value)) { return double.NaN; }
            if (double.IsPositiveInfinity(value)) { return double.PositiveInfinity; }
            if (double.IsNegativeInfinity(value)) { return double.NegativeInfinity; }
            if (sigFigures < 1 || sigFigures > 14) { throw new ArgumentOutOfRangeException("The sigFigures argument must be between 0 and 15 exclusive."); }

            // The resulting rounding position will be negative for rounding at whole numbers, and positive for decimal places.
            roundingPosition = sigFigures - 1 - (int)(Math.Round(Math.Log10(Math.Abs(value))));

            // try to use a rounding position directly, if no scale is needed.
            // this is because the scale mutliplication after the rounding can introduce error, although 
            // this only happens when you're dealing with really tiny numbers, i.e 9.9e-14.
            if (roundingPosition > 0 && roundingPosition < 15)
            {
                return Math.Round(value, roundingPosition, MidpointRounding.AwayFromZero);
            }
            else
            {
                scale = Math.Pow(10, Math.Ceiling(Math.Log10(Math.Abs(value))));
                return Math.Round(value / scale, sigFigures, MidpointRounding.AwayFromZero) * scale;
            }
        }
    }
}
