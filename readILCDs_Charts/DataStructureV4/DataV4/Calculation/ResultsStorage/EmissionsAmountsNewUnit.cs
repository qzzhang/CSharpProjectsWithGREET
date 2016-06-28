using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using Greet.UnitLib2;
using Greet.ConvenienceLib;
using Greet.DataStructureV3.Interfaces;
using Greet.DataStructureV3.Entities;


namespace Greet.DataStructureV3.ResultsStorage
{
    /// <summary>
    /// This class is used to store the results of the emissions. We use int,double to speed up the calculation as the emission are always in the unit group mass
    /// </summary>
    [Serializable]
    internal class EmissionAmountsNewUnit : DictNewUnit
    {
        #region constructors

        public EmissionAmountsNewUnit()
            : base()
        {
        }

        private EmissionAmountsNewUnit(DictNewUnit d)
            : base(d)
        {
        }

        protected EmissionAmountsNewUnit(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }


        public override void GetObjectData(SerializationInfo info,
                                    StreamingContext context)
        {
            base.GetObjectData(info, context);

        }
        #endregion constructors

        #region methods
        internal void AppendToXmlNode(System.Xml.XmlDocument processDoc, XmlNode parent)
        {
            foreach (KeyValuePair<int, double> pair in this)
            {
                XmlNode gas = processDoc.CreateNode("emission", processDoc.CreateAttr("ref", pair.Key), processDoc.CreateAttr("amount", pair.Value));
                parent.AppendChild(gas);
            }
        }
        /// <summary>
        /// This is called when we copy to clipboard all results
        /// We do not want to see the name of the class there so we return an empty string.
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            return "";
        }
        #endregion methods

        #region operators

        public static EmissionAmountsNewUnit operator *(EmissionAmountsNewUnit e1, Parameter e2)
        {
            return new EmissionAmountsNewUnit((e1 as DictNewUnit) * e2);
        }
        public static EmissionAmountsNewUnit operator *(EmissionAmountsNewUnit e1, LightValue e2)
        {
            return new EmissionAmountsNewUnit((e1 as DictNewUnit) * e2);
        }
        public static EmissionAmountsNewUnit operator *(Parameter e2, EmissionAmountsNewUnit e1)
        {
            return new EmissionAmountsNewUnit(e2 * (e1 as DictNewUnit));
        }
        public static EmissionAmountsNewUnit operator *(LightValue e2, EmissionAmountsNewUnit e1)
        {
            return new EmissionAmountsNewUnit(e2 * (e1 as DictNewUnit));
        }
        public static EmissionAmountsNewUnit operator *(double e1, EmissionAmountsNewUnit e2)
        {
            return new EmissionAmountsNewUnit((e2 as DictNewUnit) * e1);
        }
        public static EmissionAmountsNewUnit operator *(EmissionAmountsNewUnit e1, double e2)
        {
            return new EmissionAmountsNewUnit((e1 as DictNewUnit) * e2);
        }
        public static EmissionAmountsNewUnit operator /(EmissionAmountsNewUnit e1, Parameter e2)
        {
            return new EmissionAmountsNewUnit((e1 as DictNewUnit) / e2);
        }
        public static EmissionAmountsNewUnit operator /(EmissionAmountsNewUnit e1, LightValue e2)
        {
            return new EmissionAmountsNewUnit((e1 as DictNewUnit) / e2);
        }
        public static EmissionAmountsNewUnit operator /(EmissionAmountsNewUnit e1, double e2)
        {
            return new EmissionAmountsNewUnit((e1 as DictNewUnit) / e2);
        }

        public static EmissionAmountsNewUnit operator +(EmissionAmountsNewUnit e1, EmissionAmountsNewUnit e2)
        {
            return new EmissionAmountsNewUnit((e1 as DictNewUnit) + (e2 as DictNewUnit));
        }

        /// <summary>
        /// adds values of a second EmissionResults to the current one, replaces += and enhance performance because less new objects are created
        /// </summary>
        /// <param name="emissionResults"></param>
        public new void Addition(DictNewUnit emissionResults)
        {
            (this as DictNewUnit).Addition(emissionResults);
        }
        /// <summary>
        /// Adds the values to the current object after mutiplying them by the double p
        /// Saves time comparing to results = results + (values * p) as this single operation do not create any new objects for the results
        /// </summary>
        /// <param name="p"></param>
        /// <param name="values"></param>
        public void MulAdd(double p, EmissionAmountsNewUnit values)
        {
            (this as DictNewUnit).MulAdd(p, values as DictNewUnit);
        }
        public static EmissionAmountsNewUnit operator -(EmissionAmountsNewUnit e1)
        {
            return new EmissionAmountsNewUnit(-(e1 as DictNewUnit));
        }
        public static EmissionAmountsNewUnit operator -(EmissionAmountsNewUnit e1, EmissionAmountsNewUnit e2)
        {
            return new EmissionAmountsNewUnit((e1 as DictNewUnit) - (e2 as DictNewUnit));
        }

        #endregion operators

        internal Dictionary<int, IValue> ToInterfaceDictionary()
        {
            Dictionary<int, IValue> results = new Dictionary<int, IValue>();
            foreach (KeyValuePair<int, double> pair in this)
            {
                ResultValue resVal = new ResultValue();
                resVal.Unit = "kilograms";
                resVal.Value = pair.Value;
                resVal.ValueSpecie = Greet.DataStructureV3.Interfaces.Enumerators.ResultType.emission;
                resVal.SpecieID = pair.Key;
                results.Add(pair.Key, resVal);
            }
            return results;
        }

        /// <summary>
        /// Create groups for emissions and calculate the GHG value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal Dictionary<int, IValue> GroupsToInterfaceDictionary(GData data)
        {
            Dictionary<int, IValue> groups = new Dictionary<int, IValue>();

            foreach (KeyValuePair<int, double> pair in this)
            {
                foreach (int group in data.GasesData[pair.Key].Memberships)
                {
                    double factor = 0;
                    if (group == 1 && data.GasesData[pair.Key].GlobalWarmingPotential.ValueInDefaultUnit != 0)
                    {
                        factor = data.GasesData[pair.Key].GlobalWarmingPotential.ValueInDefaultUnit;
                    }

                    if (groups.ContainsKey(group))
                    {
                        (groups[group] as ResultValue).Value += pair.Value * factor;
                    }
                    else
                    {
                        ResultValue resVal = new ResultValue();
                        resVal.Unit = "kilograms";
                        resVal.Value = pair.Value * factor;
                        resVal.ValueSpecie = Greet.DataStructureV3.Interfaces.Enumerators.ResultType.emissionGroup;
                        resVal.SpecieID = pair.Key;
                        groups.Add(group, resVal);
                    }
                }
            }

            return groups;
        }

        //internal Dictionary<int, IValue> ToInterfaceDictionaryLifeCycle(GData data, int _producedResourceId, LightValue functionalUnit, double biogenicContent)
        //{
        //    Dictionary<int, IValue> results = new Dictionary<int, IValue>();

        //    #region simple object casting
        //    foreach (KeyValuePair<int, double> pair in this)
        //    {
        //        if(pair.Value != 0)
        //        {
        //            ResultValue resVal = new ResultValue();
        //            resVal.Unit = "kilograms";
        //            resVal.Value = pair.Value;
        //            resVal.ValueSpecie = PlugInsInterfaces.DataTypes.PluginEnums.ResultType.emission;
        //            resVal.SpecieID = pair.Key;
        //            results.Add(pair.Key, resVal);
        //        }
        //    }
        //    #endregion

        //    if (data.ResourcesData[_producedResourceId].CanBeAMass())
        //    {
        //        LightValue producedQuantity = data.ResourcesData[_producedResourceId].ConvertToMass(functionalUnit);

        //        #region carbon balance

        //        int co2_id = data.GasesData.Balances_ids[supportedBalanceTypes.carbon].GasRef;
        //        int biogenic_co2_id = data.GasesData.Balances_ids[supportedBalanceTypes.biogenic].GasRef;

        //        if (data.ResourcesData[_producedResourceId].CRatio != null)
        //        {
        //            LightValue cabonMass = data.ResourcesData[_producedResourceId].CarbonContent(producedQuantity);
        //            LightValue CO2Amount = cabonMass / data.GasesData[co2_id].CarbonRatio.ValueInDefaultUnit;
        //            if (CO2Amount.ValueInDefaultUnit != 0)
        //            {

        //                ResultValue resVal = new ResultValue();
        //                resVal.Unit = "kilograms";
        //                resVal.Value = CO2Amount.ValueInDefaultUnit;
        //                resVal.ValueSpecie = PlugInsInterfaces.DataTypes.PluginEnums.ResultType.emission;
        //                resVal.SpecieID = co2_id;
        //                if (results.ContainsKey(co2_id))
        //                {
        //                    resVal.Value += results[co2_id].Value;
        //                    results[co2_id] = resVal;
        //                }
        //                else
        //                    results.Add(co2_id, resVal);

        //                if (biogenicContent != 0)
        //                {
        //                    ResultValue resBiogenicVal = new ResultValue();
        //                    resBiogenicVal.Unit = "kilograms";
        //                    resBiogenicVal.Value = - CO2Amount.ValueInDefaultUnit * biogenicContent;
        //                    resBiogenicVal.ValueSpecie = PlugInsInterfaces.DataTypes.PluginEnums.ResultType.emission;
        //                    resBiogenicVal.SpecieID = biogenic_co2_id;
        //                    if (results.ContainsKey(biogenic_co2_id))
        //                    {
        //                        resBiogenicVal.Value += results[biogenic_co2_id].Value;
        //                        results[biogenic_co2_id] = resBiogenicVal;
        //                    }
        //                    else
        //                        results.Add(biogenic_co2_id, resBiogenicVal);
        //                }
        //            }
        //        }
        //        #endregion

        //        #region sulfur balance
        //        int sox_id = data.GasesData.Values.Single(item => item.Name.ToLower() == "sox").Id;
        //        if (data.ResourcesData[_producedResourceId].SRatio != null)
        //        {
        //            LightValue sulfurMass = data.ResourcesData[_producedResourceId].SulfurContent(producedQuantity);
        //            LightValue SOXAmount = sulfurMass / data.GasesData[sox_id].SulfurRatio.ValueInDefaultUnit;
        //            if (SOXAmount.ValueInDefaultUnit != 0)
        //            {
        //                ResultValue resSVal = new ResultValue();
        //                resSVal.Unit = "kilograms";
        //                resSVal.Value = SOXAmount.ValueInDefaultUnit;
        //                resSVal.ValueSpecie = PlugInsInterfaces.DataTypes.PluginEnums.ResultType.emission;
        //                resSVal.SpecieID = sox_id;
        //                if (results.ContainsKey(sox_id))
        //                {
        //                    resSVal.Value += results[sox_id].Value;
        //                    results[sox_id] = resSVal;
        //                }
        //                else
        //                    results.Add(sox_id, resSVal);
        //            }
        //        }
        //        #endregion
        //    }
        //    return results;
        //}

        //internal Dictionary<int, IValue> GroupsToInterfaceDictionaryLifeCycle(GData data, int _producedResourceId, LightValue functionalUnit, double biogenicContent)
        //{
        //    Dictionary<int, IValue> groups = new Dictionary<int, IValue>();

        //    foreach (KeyValuePair<int, IValue> pair in ToInterfaceDictionaryLifeCycle(data, _producedResourceId, functionalUnit, biogenicContent))
        //    {
        //        foreach (int group in data.GasesData[pair.Key].Memberships)
        //        {
        //            double factor = 0;
        //            if (group == 1 && data.GasesData[pair.Key].GlobalWarmingPotential.ValueInDefaultUnit != 0)
        //            {
        //                factor = data.GasesData[pair.Key].GlobalWarmingPotential.ValueInDefaultUnit;
        //            }

        //            if (groups.ContainsKey(group))
        //            {
        //                (groups[group] as ResultValue).Value += pair.Value.Value * factor;
        //            }
        //            else
        //            {
        //                ResultValue resVal = new ResultValue();
        //                resVal.Unit = "kilograms";
        //                resVal.Value = pair.Value.Value * factor;
        //                resVal.ValueSpecie = PlugInsInterfaces.DataTypes.PluginEnums.ResultType.emissionGroup;
        //                resVal.SpecieID = pair.Key;
        //                groups.Add(group, resVal);
        //            }
        //        }
        //    }

        //    return groups;
        //}
    }
}
