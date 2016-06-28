using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.UnitLib
{
    /// <summary>
    /// This is a group which contains one or more BaseGroups, this allows combining BaseGroups into more complicated groups
    /// such as HeatingValue = Energy/Mass. The references to the BaseGroups are stored in a list of Bases which contains
    /// the default and override unit and whether it is in the numerator or denominator.
    /// </summary>
    [Serializable]
    public class DerivedQuantity : BaseQuantity
    {
        #region attributes
        public string defaultUnitStringBuffer;
        public bool createdByTheCalculations;
        #endregion

        #region accessors
        public List<DerivedQuantityBase> BaseGroups = new List<DerivedQuantityBase>();
        public override string Abbrev
        {
            get
            {
                string abbrev = "";
                foreach (DerivedQuantityBase b in BaseGroups)
                {
                    if (abbrev != "")
                    {
                        if (b.Numerator) abbrev += "*";
                        else abbrev += "/";
                    }
                    abbrev += b.OverrideUnit.Abbrev;
                }
                return abbrev;
            }
        }
        public override string DisplayUnitStr
        {
            get
            {
                string name = "";
                foreach (DerivedQuantityBase b in BaseGroups)
                {
                    if (name != "")
                    {
                        if (b.Numerator) name += "*";
                        else name += "/";
                    }
                    name += b.OverrideUnit.Name;
                }
                return name;
            }
        }
        public override string SIUnitStr
        {
            get { return this.defaultUnitStringBuffer; }
        }

        public override string DefaultUnitAbbrev
        {
            get
            {
                string name = "";

                //reorders the bases into numerator first then denominator
                //avoid display 1/Btus*Grams for display Grams/Btus
                foreach (DerivedQuantityBase b in BaseGroups.OrderBy(item => item.Numerator == false))
                {
                    if (name != "")
                    {
                        if (b.Numerator) name += "*";
                        else name += "/";
                    }
                    else if (!b.Numerator) name = "1/";
                    if (String.IsNullOrEmpty(b.DefaultUnit.Abbrev))
                        name += b.DefaultUnit.DisplayName;
                    else
                        name += b.DefaultUnit.Abbrev;
                }
                return name;
            }
        }

        /// <summary>
        /// Returns the group in Utils.groups that matches this DerivedGroup. Matching criteria includes
        /// containing a match for each bases (regardless of order) on the numerator and denominator.
        /// To be a matching base the group, defaultunit, and numerator must be the same (override is ignored).
        /// </summary>
        public BaseQuantity MatchedGroup
        {
            get
            {
                foreach (BaseQuantity dg in Units.QuantityList.Values)
                {
                    if (dg.Equals(this))
                        return dg;
                }

                return this;
            }
        }

        /// <summary>
        /// This method tries to find if this is present in  Units.QuantityList. If yes, returns the reference to the item from the list if not returns this
        /// </summary>
        public BaseQuantity DefaultOnlyMatchedGroup
        {
            get
            {
                foreach (BaseQuantity dg in Units.QuantityList.Values)
                {
                    if (dg.DefaultOnlyEquals(this))
                        return dg;
                }

                return this;
            }
        }

        /// <summary>
        /// Return the first of the bottom units, use with care, we never know if there are many bottom units
        /// </summary>
        public string TopUnit
        {
            get
            {
                var v = from bg in BaseGroups
                        where bg.Numerator == true
                        select bg.Quantity.SIUnitStr;
                if (v.Count() == 0)
                    return "";
                else
                {
                    StringBuilder concat = new StringBuilder();
                    foreach (string str in v)
                    {
                        concat.Append(str);
                        concat.Append("*");
                    }
                    return concat.ToString().TrimEnd('*');
                }
            }
        }
        /// <summary>
        /// Return the first of the bottom units, use with care, we never know if there are many bottom units
        /// </summary>
        public string BottomUnit
        {
            get
            {
                var v = from bg in BaseGroups
                        where bg.Numerator == false
                        select bg.Quantity.SIUnitStr;
                if (v.Count() == 0)
                    return "";
                else
                {
                    StringBuilder concat = new StringBuilder();
                    foreach (string str in v)
                    {
                        concat.Append(str);
                        concat.Append("/");
                    }
                    return concat.ToString().TrimEnd('/');
                }
            }
        }
        #endregion
        #region methods
        public string BasesToString(bool forXmlAttibute)
        {
            string name = "";

            //reorders the bases into numerator first then denominator
            //avoid display 1/Btus*Grams for display Grams/Btus
            foreach (DerivedQuantityBase b in BaseGroups.OrderBy(item => item.Numerator == false))
            {
                if (name != "")
                {
                    if (b.Numerator) name += "*";
                    else name += "/";
                }
                else if (!b.Numerator) name = "1/";
                if (forXmlAttibute) name += b.Quantity.Name + ":" + b.OverrideUnit.Name;
                else name += b.DefaultUnit.Name;
            }
            return name;
        }
        internal List<DerivedQuantityBase> StringToBases(string unitExpression, bool invert)
        {
            List<DerivedQuantityBase> bases = new List<DerivedQuantityBase>();
            List<List<string>> temp = Units.SplitString(unitExpression, invert);
            List<string> top = temp[0];
            List<string> bottom = temp[1];

            foreach (string part in top)
            {
                Unit unit = null;
                if (Units.UnitsList.ContainsKey(part))
                    unit = Units.UnitsList[part];
                else if (Units.UnitsList.ContainsKey(part.ToLower()))
                    unit = Units.UnitsList[part.ToLower()];
                else if (Units.UnitsList.ContainsKey(part.Substring(0, part.Length - 1).ToLower()))
                    unit = Units.UnitsList[part.Substring(0, part.Length - 1).ToLower()];
                else if (Units.UnitsList.Values.Any(item => item.Abbrev == part))
                    unit = Units.UnitsList.Values.First(item => item.Abbrev == part);
                else if (Units.UnitsList.ContainsKey(part.ToLower() + "s"))
                    unit = Units.UnitsList[part.ToLower() + "s"];
                if (unit == null)
                    throw new System.ArgumentException("The unit specified in the input string was not found in the UnitList", "unitExpression");

                Quantity bg = Units.QuantityList[unit.BaseGroupName] as Quantity;
                if(bases.Any(b => b.Numerator && b.DefaultUnit.Abbrev == "") && bg.DefaultUnit.Abbrev != "")
                    continue;
                bases.Add(new DerivedQuantityBase(bg, unit, true, this));
            }
            foreach (string part in bottom)
            {
                Unit unit = null;
                if (Units.UnitsList.ContainsKey(part))
                    unit = Units.UnitsList[part];
                else if (Units.UnitsList.ContainsKey(part.ToLower()))
                    unit = Units.UnitsList[part.ToLower()];
                else if (Units.UnitsList.ContainsKey(part.Substring(0, part.Length - 1).ToLower()))
                    unit = Units.UnitsList[part.Substring(0, part.Length - 1).ToLower()];
                else if (Units.UnitsList.Values.Any(item => item.Abbrev == part))
                    unit = Units.UnitsList.Values.First(item => item.Abbrev == part);
                else if (Units.UnitsList.ContainsKey(part.ToLower() + "s"))
                    unit = Units.UnitsList[part.ToLower() + "s"];
                if (unit == null)
                    throw new System.ArgumentException("The unit specified in the input string was not found in the UnitList", "unitExpression");

                Quantity bg = Units.QuantityList[unit.BaseGroupName] as Quantity;
                if (bg.DefaultUnit.Abbrev != "")
                    bases.Add(new DerivedQuantityBase(bg, unit, false, this));
            }

            return bases;
        }
        internal void AddBasesFromString(string unitExpression, bool invert)
        {
            this.BaseGroups.AddRange(StringToBases(unitExpression, invert));
        }
        public override double ConvertFromDefaultToOverride(double valueToConvert)
        {
            double convertedVal = valueToConvert;
            foreach (DerivedQuantityBase b in BaseGroups)
                if (b.OverrideUnit != b.DefaultUnit)
                {
                    if (b.Numerator)
                        convertedVal = Units.Conversion(convertedVal, b.DefaultUnit, b.OverrideUnit);
                    else    //The different unit is in the denominator so take the reciprocal
                        convertedVal = Units.Conversion(convertedVal, b.OverrideUnit, b.DefaultUnit);
                }
            return convertedVal;
        }
        public override double ConvertFromDefaultToSpecific(double valueToConvert, string unit)
        {
            double convertedVal = valueToConvert;
            foreach (DerivedQuantityBase b in BaseGroups)
                if (Units.UnitsList[unit] != b.DefaultUnit)
                {
                    if (b.Numerator)
                        convertedVal = Units.Conversion(convertedVal, b.DefaultUnit, Units.UnitsList[unit]);
                    else    //The different unit is in the denominator so take the reciprocal
                        convertedVal = Units.Conversion(convertedVal, b.OverrideUnit, Units.UnitsList[unit]);
                }
            return convertedVal;
        }
        public override double ConvertFromOverrideToDefault(double valueToConvert)
        {
            double convertedVal = valueToConvert;
            foreach (DerivedQuantityBase b in BaseGroups)
                if (b.OverrideUnit != b.DefaultUnit)
                {
                    if (b.Numerator)
                        convertedVal = Units.Conversion(convertedVal, b.OverrideUnit, b.DefaultUnit);
                    else    //The different unit is in the denominator so take the reciprocal
                        convertedVal = Units.Conversion(convertedVal, b.DefaultUnit, b.OverrideUnit);
                }
            return convertedVal;
        }
        internal override XmlNode ToXmlNode(XmlDocument doc)
        {
            return doc.CreateNode("derived", doc.CreateAttr("name", Name), doc.CreateAttr("display_name", DisplayName), doc.CreateAttr("format", this.format), doc.CreateAttr("base", BasesToString(true)));
        }

        public override bool Equals(object obj)
        {
            //Check group, default unit, and numerator for equality of two derived groups
            DerivedQuantity dg = obj as DerivedQuantity;
            if (dg == null && this != null)
                return false;
            if (dg == null && this == null)
                return true;
            int n = dg.BaseGroups.Count;
            bool[] matched_flags = new bool[n];
            DerivedQuantityBase b2;
            for (int i = 0; i < n; i++)
                matched_flags[i] = false;

            if (dg == null)
                return false;
            if (this.BaseGroups.Count != dg.BaseGroups.Count)   //Make sure there are no extra bases in dg than we just checked in this
                return false;

            foreach (DerivedQuantityBase b in this.BaseGroups)     //Check if each base has a match
            {
                bool matched = false;
                for (int i = 0; i < n; i++)
                {
                    if (matched_flags[i])
                        continue;
                    b2 = dg.BaseGroups[i];
                    if (b.Quantity == b2.Quantity && b.DefaultUnit == b2.DefaultUnit && b.OverrideUnit == b2.OverrideUnit && b.Numerator == b2.Numerator)
                    {
                        matched = true;
                        matched_flags[i] = true;
                        break;
                    }
                }
                if (!matched)
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
        #region constructors

        public DerivedQuantity(XmlNode node)
            : base(node)
        {
            string[] baseSplit = node.Attributes["base"].Value.Split("/".ToCharArray());

            //Add numerator units
            foreach (string str in baseSplit[0].Split("*".ToCharArray()))
            {
                string[] unitSplit = str.Split(":".ToCharArray());
                BaseGroups.Add(new DerivedQuantityBase(unitSplit[0], unitSplit[1], true, this));
            }
            //Add denominator units
            for (int i = 1; i < baseSplit.Length; i++)
            {
                string[] unitSplit = baseSplit[i].Split(":".ToCharArray());
                BaseGroups.Add(new DerivedQuantityBase(unitSplit[0], unitSplit[1], false, this));
            }

            this.createdByTheCalculations = false;

            this.defaultUnitStringBuffer = BasesToString(false);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <param name="format"></param>
        /// <param name="baseString">String that contains information about base groups of this derived group, example: "volume:gallons/distance:miles"</param>
        public DerivedQuantity(string name, string displayName, string format, string baseString)
        {

            this.Name = name;
            this.DisplayName = displayName;
            this.format = format;

            string[] baseSplit = baseString.Split("/".ToCharArray());

            //Add numerator units
            foreach (string str in baseSplit[0].Split("*".ToCharArray()))
            {
                string[] unitSplit = str.Split(":".ToCharArray());
                BaseGroups.Add(new DerivedQuantityBase(unitSplit[0], unitSplit[1], true, this));
            }
            //Add denominator units
            for (int i = 1; i < baseSplit.Length; i++)
            {
                string[] unitSplit = baseSplit[i].Split(":".ToCharArray());
                BaseGroups.Add(new DerivedQuantityBase(unitSplit[0], unitSplit[1], false, this));
            }

            this.createdByTheCalculations = false;

            this.defaultUnitStringBuffer = BasesToString(false);

        }

        /// <summary>
        /// This constructor is for the DoubleValue operators, it takes the defaultUnit expressions and the operation 
        /// that is being done as inputs. It parses the expressions, simplifies, and adds the appropriate groups.
        /// </summary>
        /// <param name="d1">ValueInDefaultUnit of parameter d1</param>
        /// <param name="d1_unitgroupname">UnitGroupName of parameter d1</param>
        /// <param name="d2">ValueInDefaultUnit of parameter d2</param>
        /// <param name="d2_unitgroupname">UnitGroupName of parameter d2</param> 
        /// <param name="operation"></param>
        public DerivedQuantity(ref double d1, string d1_unitgroupname, ref double d2, string d2_unitgroupname, char operation)
            : base()
        {
            if (operation == '*' || operation == '/')
                DoMultDiv(ref d1, d1_unitgroupname, ref d2, d2_unitgroupname, operation);
            else
                DoAddSub(Units.QuantityList[d1_unitgroupname], Units.QuantityList[d2_unitgroupname], operation);

            this.defaultUnitStringBuffer = BasesToString(false);
            this.Name = "automatic_" + this.SIUnitStr;

            if (this.BaseGroups.Count == 0)
                if (Units.QuantityList[d1_unitgroupname].SIUnitStr == "ratio" && Units.QuantityList[d2_unitgroupname].SIUnitStr == "ratio")
                    this.Name = "percentage";
                else
                    this.Name = "unitless";

            this.createdByTheCalculations = true;

            BaseQuantity temps = this.DefaultOnlyMatchedGroup;
            if (temps == this && !Units.QuantityList.ContainsKey(temps.Name))
                Units.QuantityList.Add(this.Name, this);
        }

        public DerivedQuantity(BaseQuantity g1, BaseQuantity g2, char operation, string forcedName = "")
            : base()
        {
            if (operation == '*' || operation == '/')
                DoMultDiv(g1, g2, operation);
            else
                DoAddSub(g1, g2, operation);

            this.defaultUnitStringBuffer = BasesToString(false);
            if (forcedName != "")
                this.Name = forcedName;
            else
                this.Name = "automatic_" + this.SIUnitStr;

            if (this.BaseGroups.Count == 0)
                this.Name = "unitless";

            this.createdByTheCalculations = true;

            BaseQuantity temps = this.DefaultOnlyMatchedGroup;
            if (temps == this && !Units.QuantityList.ContainsKey(temps.Name))
                Units.QuantityList.Add(this.Name, this);

        }
        /// <summary>
        /// Used for generating the derived groups "on the fly"
        /// </summary>
        /// <param name="unitExpression">Contains name of a unit or unit group, example: meters/grams</param>
        /// <param name="name"></param>
        public DerivedQuantity(string unitExpression)
            : base()
        {
            if (unitExpression == "")
                unitExpression = "unitless";
            AddBasesFromString(unitExpression.Replace("automatic_","").Replace("1/*","").Replace("ratio*",""), false);
            this.Name = "automatic_" + this.BasesToString(false);

            this.createdByTheCalculations = true;

            this.defaultUnitStringBuffer = BasesToString(false);

            BaseQuantity temps = this.DefaultOnlyMatchedGroup;
            if (temps == this && !Units.QuantityList.ContainsKey(temps.Name))
                Units.QuantityList.Add(this.Name, this);
        }

        public DerivedQuantity()
        {
            
        }
        #endregion constructros
        #region methods

        private void DoMultDiv(ref double d1, string d1_unitgroupname, ref double d2, string d2_unitgroupname, char operation)
        {
            string g1_default_unit_str = Units.QuantityList[d1_unitgroupname].SIUnitStr;
            string g2_default_unit_str = Units.QuantityList[d2_unitgroupname].SIUnitStr;

            if (g1_default_unit_str == "unitless" && g2_default_unit_str != "unitless")
                AddBasesFromString(g2_default_unit_str, operation == '/');
            else if (g2_default_unit_str == "unitless")
                AddBasesFromString(g1_default_unit_str, false);
            else if (g1_default_unit_str == "ratio" && g2_default_unit_str != "ratio")
                AddBasesFromString(g2_default_unit_str, operation == '/');
            else if (g2_default_unit_str == "ratio" && g1_default_unit_str != "ratio")
                AddBasesFromString(g1_default_unit_str, false);
            else
            {
                List<DerivedQuantityBase> d1Bases = StringToBases(g1_default_unit_str, false);
                List<DerivedQuantityBase> d2Bases = StringToBases(operation == '*' ? g2_default_unit_str : ("1/" + Units.InvertExpression(g2_default_unit_str)), false);
                for (int b1 = 0; b1 < d1Bases.Count; b1++)
                {
                    for (int b2 = 0; b2 < d2Bases.Count && b1 >= 0; b2++)
                    {
                        //Check if it is the same group on top and bottom
                        if (d1Bases[b1].Quantity == d2Bases[b2].Quantity && d1Bases[b1].Numerator != d2Bases[b2].Numerator)
                        {
                            //Check if they are in the same unit, if not convert to the baseGroup's default
                            if (d1Bases[b1].DefaultUnit != d2Bases[b2].DefaultUnit)
                            {
                                if (d1Bases[b1].Numerator)
                                    d1 = Units.Conversion(d1, d1Bases[b1].DefaultUnit, d1Bases[b1].Quantity.DefaultUnit);
                                else
                                    d1 = Units.Conversion(d1, d1Bases[b1].Quantity.DefaultUnit, d1Bases[b1].DefaultUnit);
                                if (d2Bases[b2].Numerator)
                                    d2 = Units.Conversion(d1, d2Bases[b2].DefaultUnit, d2Bases[b2].Quantity.DefaultUnit);
                                else
                                    d2 = Units.Conversion(d2, d2Bases[b2].Quantity.DefaultUnit, d2Bases[b2].DefaultUnit);
                            }

                            d1Bases.RemoveAt(b1);
                            d2Bases.RemoveAt(b2);
                            b1--; b2--;     //Removed the bases from the individual lists, prevent increment
                        }
                    }
                }

                BaseGroups.Clear();
                foreach (DerivedQuantityBase b in d1Bases)
                    BaseGroups.Add(b);
                foreach (DerivedQuantityBase b in d2Bases)
                    BaseGroups.Add(b);
            }
        }
        private void DoMultDiv(BaseQuantity g1, BaseQuantity g2, char operation)
        {
            string g1_default_unit_str = g1.SIUnitStr;
            string g2_default_unit_str = g2.SIUnitStr;

            if (g1_default_unit_str == "unitless" && g2_default_unit_str != "unitless")
                AddBasesFromString(g2_default_unit_str, operation == '/');
            else if (g2_default_unit_str == "unitless")
                AddBasesFromString(g1_default_unit_str, false);
            else if (g1_default_unit_str == "ratio" && g2_default_unit_str != "ratio")
                AddBasesFromString(g2_default_unit_str, operation == '/');
            else if (g2_default_unit_str == "ratio" && g1_default_unit_str != "ratio")
                AddBasesFromString(g1_default_unit_str, false);
            else if (g1_default_unit_str == g2_default_unit_str && (g1_default_unit_str == "ratio" || g1_default_unit_str == "unitless"))
                AddBasesFromString(g1_default_unit_str, false);
            else
            {
                List<DerivedQuantityBase> d1Bases = StringToBases(g1_default_unit_str, false);
                List<DerivedQuantityBase> d2Bases = StringToBases(operation == '*' ? g2_default_unit_str : ("1/" + Units.InvertExpression(g2_default_unit_str)), false);
                for (int b1 = 0; b1 < d1Bases.Count; b1++)
                {
                    for (int b2 = 0; b2 < d2Bases.Count && b1 >= 0; b2++)
                    {
                        //Check if it is the same group on top and bottom
                        if (d1Bases[b1].Quantity == d2Bases[b2].Quantity && d1Bases[b1].Numerator != d2Bases[b2].Numerator)
                        {
                            d1Bases.RemoveAt(b1);
                            d2Bases.RemoveAt(b2);
                            b1--; b2--;     //Removed the bases from the individual lists, prevent increment
                        }
                    }
                }
                foreach (DerivedQuantityBase b in d1Bases)
                    BaseGroups.Add(b);
                foreach (DerivedQuantityBase b in d2Bases)
                    BaseGroups.Add(b);
            }
        }
        private void DoAddSub(BaseQuantity g1, BaseQuantity g2, char operation)
        {
            if (g1.DefaultOnlyEquals(g2))
                AddBasesFromString(g1.SIUnitStr, false);
            else if (g1.Name == "unitless" && g2.Name != "unitless")
                AddBasesFromString(g2.SIUnitStr, false);
            else if (g2.Name == "unitless" && g1.Name != "unitless")
                AddBasesFromString(g1.SIUnitStr, false);
            else if (g1.SIUnitStr == "ratio" && g2.SIUnitStr != "ratio")
                AddBasesFromString(g2.SIUnitStr, false);
            else if (g2.SIUnitStr == "ratio" && g1.SIUnitStr != "ratio")
                AddBasesFromString(g1.SIUnitStr, false);
            else
                throw new Exception("It's irresponsible to " + (operation == '+' ? "add " : "subtract ") + g1.SIUnitStr + " and " + g2.SIUnitStr);
        }

        #endregion
    }
}
