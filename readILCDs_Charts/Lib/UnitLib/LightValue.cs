/*********************************************************************** 
COPYRIGHT NOTIFICATION 

Email contact: greet@anl.gov 
Copyright (c) 2012, UChicago Argonne, LLC 
All Rights Reserved

THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 

************************************************************************ 
ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
ENERGY. 
************************************************************************
 
***********************************************************************/
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Greet.ConvenienceLib;

namespace Greet.UnitLib
{
    [Serializable]
    public class LightValue : ISerializable, IComparable
    {

        #region properties 

        /// <summary>
        /// stores the actual original value for the object
        /// </summary>
        public double ValueInDefaultUnit { get; set; }

        /// <summary>
        /// The unit parameter that this doublevalue corresponds to for doing unit conversions. This unit is stored internally as the default unit for this parameter
        /// and the OverrideUnit is the user defined unit for display
        /// </summary>
        public string QuantityName { get; set; }

        #endregion

        #region constructors

        public LightValue()
        {

        }

        public LightValue(string quantity_or_unit_name, double value)
        {
            //first of all we try to detect the unit group, or the units used for those values
            if (Units.QuantityList.ContainsKey(quantity_or_unit_name))
                this.QuantityName = quantity_or_unit_name;
            else
                this.QuantityName = null;

            //we check if the values are in default units or not
            if (this.QuantityName == null)
            {
                //get the derived group
                DerivedQuantity dg = new DerivedQuantity(quantity_or_unit_name);
                this.ValueInDefaultUnit = dg.ConvertFromOverrideToDefault(value);
                BaseQuantity bg = dg.DefaultOnlyMatchedGroup;
                this.QuantityName = bg.Name;
            }
            else
            {
                //the unit is default unit
                this.ValueInDefaultUnit = value;
            }
        }

        public LightValue(SerializationInfo info, StreamingContext text)
            : this()
        {

            this.ValueInDefaultUnit = info.GetDouble("original_buffer");
            string temp_unit_group_name = info.GetString("unitGroup");
            this.QuantityName = temp_unit_group_name;

        }

        #endregion

        #region methods
        /// <summary>
        /// Serializer, serialise everything we do that because we want to override the deserializer which clones the unitgroup
        /// we dont want to clone the unit group so basically we specify how to serialize, and how to deserialize
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("original_buffer", this.ValueInDefaultUnit);
            info.AddValue("unitGroup", this.QuantityName);

        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            LightValue secondValue = obj as LightValue;
            if (secondValue == null)
                throw new ArgumentException("Object is not a LightValue");

            //Comparing Same Units
            else if (Units.QuantityList[this.QuantityName].SIUnitStr == Units.QuantityList[secondValue.QuantityName].SIUnitStr)
                return this.ValueInDefaultUnit.CompareTo(secondValue.ValueInDefaultUnit);

            // Comparing if different Units n Unit preference order Joules, Grams, Litres
            else if (Units.QuantityList[this.QuantityName].SIUnitStr != Units.QuantityList[secondValue.QuantityName].SIUnitStr)
            {
                // When Comparing two values of different units with one being Joules, The value of the with 0.0 should follow the other

                if (Units.QuantityList[this.QuantityName].SIUnitStr == "joules" || Units.QuantityList[secondValue.QuantityName].SIUnitStr == "joules")
                    if (this.ValueInDefaultUnit == 0.0)
                        return -1;
                    else if (secondValue.ValueInDefaultUnit == 0.0)
                        return 1;
                    else
                        return this.ValueInDefaultUnit.CompareTo(secondValue.ValueInDefaultUnit);


                else if (Units.QuantityList[this.QuantityName].SIUnitStr == "kilograms" || Units.QuantityList[secondValue.QuantityName].SIUnitStr == "kilograms")
                    if (this.ValueInDefaultUnit == 0.0)
                        return -1;
                    else if (secondValue.ValueInDefaultUnit == 0.0)
                        return 1;
                    else
                        return this.ValueInDefaultUnit.CompareTo(secondValue.ValueInDefaultUnit);

                else if (Units.QuantityList[this.QuantityName].SIUnitStr == "cu_meters" || Units.QuantityList[secondValue.QuantityName].SIUnitStr == "cu_meters")
                    if (this.ValueInDefaultUnit == 0.0)
                        return -1;
                    else if (secondValue.ValueInDefaultUnit == 0.0)
                        return 1;
                    else
                        return this.ValueInDefaultUnit.CompareTo(secondValue.ValueInDefaultUnit);

                else
                    return 0;
            }
            else
                return 0;
        }

        public override string ToString()
        {
            return this.ValueInDefaultUnit + " " + this.QuantityName;
        }

        #endregion

        #region operators

        public static LightValue operator +(LightValue d1, LightValue d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = d1.ValueInDefaultUnit + d2.ValueInDefaultUnit;
#if DEBUG
            if (Units.QuantityList[d1.QuantityName].DefaultOnlyEquals(Units.QuantityList[d2.QuantityName]))
                result.QuantityName = d1.QuantityName;
            else
                throw new Exception("It's irresponsible to add " + Units.QuantityList[d1.QuantityName].SIUnitStr + " and " + Units.QuantityList[d2.QuantityName].SIUnitStr);

            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#else
            result.QuantityName = d1.QuantityName;
#endif
            return result;
        }
        public static LightValue operator +(LightValue d1, double d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = d1.ValueInDefaultUnit + (double)d2;
            result.QuantityName = d1.QuantityName;
#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator +(double d1, LightValue d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = d2.ValueInDefaultUnit + (double)d1;
            result.QuantityName = d2.QuantityName;

#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator -(LightValue d1, LightValue d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = d1.ValueInDefaultUnit - d2.ValueInDefaultUnit;

            if (Units.QuantityList[d1.QuantityName].DefaultOnlyEquals(Units.QuantityList[d2.QuantityName]))
                result.QuantityName = d1.QuantityName;
            else
                throw new Exception("It's irresponsible to add " + Units.QuantityList[d1.QuantityName].SIUnitStr + " and " + Units.QuantityList[d2.QuantityName].SIUnitStr);


#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif

            return result;
        }
        public static LightValue operator -(LightValue d1, double d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = d1.ValueInDefaultUnit - (double)d2;

            result.QuantityName = d1.QuantityName;
#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator -(double d1, LightValue d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = (double)d1 - d2.ValueInDefaultUnit;

            result.QuantityName = d2.QuantityName;
#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator -(LightValue d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = -d2.ValueInDefaultUnit;

            result.QuantityName = d2.QuantityName;
#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator *(LightValue d1, LightValue d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = d1.ValueInDefaultUnit * d2.ValueInDefaultUnit;

            DerivedQuantity newGroup = new DerivedQuantity(Units.QuantityList[d1.QuantityName], Units.QuantityList[d2.QuantityName], '*');
            result.QuantityName = newGroup.DefaultOnlyMatchedGroup.Name;

#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator *(double d1, LightValue d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = d2.ValueInDefaultUnit * (double)d1;

            result.QuantityName = d2.QuantityName;
#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator *(LightValue d1, double d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = d1.ValueInDefaultUnit * d2;

            result.QuantityName = d1.QuantityName;
#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator /(LightValue d1, LightValue d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = d1.ValueInDefaultUnit / d2.ValueInDefaultUnit;

            DerivedQuantity newGroup = new DerivedQuantity(Units.QuantityList[d1.QuantityName], Units.QuantityList[d2.QuantityName], '/');
            result.QuantityName = newGroup.DefaultOnlyMatchedGroup.Name;
#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator /(LightValue d1, double d2)
        {
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = d1.ValueInDefaultUnit / (double)d2;

            result.QuantityName = d1.QuantityName;
#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
        public static LightValue operator /(double d1, LightValue d2)
        {

            //MathParse should not be references here in order to avoid recursive references between the Greet.UnitLib and the Greet.Model
            //Moving MathParse to the convenience library would probably work, except if the MathParser depends on the unit system too
            //we need to break all these dependecies
            LightValue result = new LightValue();

            result.ValueInDefaultUnit = (double)d1 / d2.ValueInDefaultUnit;

            DerivedQuantity newGroup = new DerivedQuantity(Units.QuantityList["unitless"], Units.QuantityList[d2.QuantityName], '/');
            result.QuantityName = newGroup.DefaultOnlyMatchedGroup.Name;
#if DEBUG
            double result_double = result.ValueInDefaultUnit;
            TestResult(result_double);
#endif
            return result;
        }
#if DEBUG
        private static void TestResult(double result)
        {
            if (Double.IsNaN(result))
                NotANumberOperation();
            if (Double.IsNegativeInfinity(result) || Double.IsPositiveInfinity(result))
                InfinityOperation();
        }

        /// <summary>
        /// Used for debugging to see which operations are outptuting a NaN value
        /// </summary>
        private static void NotANumberOperation()
        {
        }
        private static void InfinityOperation()
        {
        }
#endif
        #endregion operators

        /// <summary>
        /// Exorts a LightValue to an XML attribute for the export results function
        /// </summary>
        /// <param name="xmlDocument">The XML document used for namespace URI</param>
        /// <param name="attribute_name">The attribute name to be used</param>
        /// <returns></returns>
        internal System.Xml.XmlAttribute ToXmlAttribute(System.Xml.XmlDocument xmlDocument, string attribute_name)
        {
            return xmlDocument.CreateAttr(attribute_name, this.ValueInDefaultUnit.ToString() + ";" + this.QuantityName);
        }

       

        /// <summary>
        /// try to add a prfix for that unit and output a nice number
        /// the value has to come in the overriden unit of the group
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit_group"></param>
        private double AddPrefix(double value, BaseQuantity unit_group, ref string unitString)
        {
            //Cannot have a reference to Holder here we need to find another solution or move this method to a higher class

            if (value != 0)
            {
                //those are storing temporaly the factors used to change between the unit prefixes or units

                double original_value = value;
                value = original_value;

                //splitting the unit group in order to detect numerator and denominator units
                List<DerivedQuantityBase> base_groups_in_derived_unit_group;
                if (unit_group is DerivedQuantity)
                    base_groups_in_derived_unit_group = ((DerivedQuantity)unit_group).StringToBases(unit_group.DisplayUnitStr, false);
                else
                {
                    base_groups_in_derived_unit_group = new List<DerivedQuantityBase>();
                    base_groups_in_derived_unit_group.Add(new DerivedQuantityBase((Quantity)unit_group, Units.UnitsList[unit_group.DisplayUnitStr], true, null));
                }

                //create a dictionary to store the prefix per base
                Dictionary<DerivedQuantityBase, int> power_per_base = new Dictionary<DerivedQuantityBase, int>();
                foreach (DerivedQuantityBase baseunit in base_groups_in_derived_unit_group)
                    power_per_base.Add(baseunit, 0);

                bool prefix_used_denominator = false;
                bool prefix_used_numerator = false;

                //first we are trying to increase/decrease the denominator
                foreach (DerivedQuantityBase my_base in base_groups_in_derived_unit_group.FindAll(item => item.Numerator == false))
                {
                    //get the list of prefixes
                    Prefixes pref_serie;
                    if (Units.prefixes.ContainsKey(my_base.OverrideUnit.prefixes))
                        pref_serie = Units.prefixes[my_base.OverrideUnit.prefixes];
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
                                value = original_value * Math.Pow(10, power_per_base[my_base]);
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
                                value = original_value * Math.Pow(10, power_per_base[my_base]);
                                //value = original_value * Math.Pow(10, -power_per_base[my_base]);
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
                }

                //reset the original value so the changes to the denominator are taken into account
                original_value = value;

                //now we try to increase/decrease the numerator
                foreach (DerivedQuantityBase my_base in base_groups_in_derived_unit_group.FindAll(item => item.Numerator == true))
                {
                    //get the list of prefixes
                    Prefixes pref_serie;
                    if (Units.prefixes.ContainsKey(my_base.OverrideUnit.prefixes))
                        pref_serie = Units.prefixes[my_base.OverrideUnit.prefixes];
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
                                value = original_value * Math.Pow(10, -power_per_base[my_base]);
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
                                value = original_value * Math.Pow(10, -power_per_base[my_base]);
                                //value = original_value * Math.Pow(10, power_per_base[my_base]);
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
                }

                //we assign a value for the override abbrev of that double value
                unitString = "";
                foreach (KeyValuePair<DerivedQuantityBase, int> baseunit in power_per_base)
                {
                    string prefix_founded = "";
                    if (baseunit.Value != 0)
                    {
                        Prefixes pref_serie = Units.prefixes[baseunit.Key.OverrideUnit.prefixes];
                        prefix_founded = pref_serie[baseunit.Value];
                    }
                    if (baseunit.Key.Numerator == false)
                        unitString += "/";
                    unitString += prefix_founded + baseunit.Key.OverrideUnit.Abbrev;
                }
            }

            return value;
        }

    }
}
