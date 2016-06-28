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
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Greet.UnitLib3;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4.ResultsStorage
{
    /// <summary>
    /// This class is used to store the results of the emissions. We use int,double to speed up the calculation as the emission are always in the unit group mass
    /// </summary>
    [Serializable]
    public class EmissionAmounts : Dict
    {
        #region constructors

        public EmissionAmounts()
            : base()
        {
        }
        public EmissionAmounts(Dict d)
            : base(d)
        {
        }

        protected EmissionAmounts(SerializationInfo info, StreamingContext context)
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

        public static EmissionAmounts operator *(EmissionAmounts e1, Parameter e2)
        {
            return new EmissionAmounts((e1 as Dict) * e2);
        }
        public static EmissionAmounts operator *(EmissionAmounts e1, LightValue e2)
        {
            return new EmissionAmounts((e1 as Dict) * e2);
        }
        public static EmissionAmounts operator *(Parameter e2, EmissionAmounts e1)
        {
            return new EmissionAmounts(e2 * (e1 as Dict));
        }
        public static EmissionAmounts operator *(LightValue e2, EmissionAmounts e1)
        {
            return new EmissionAmounts(e2 * (e1 as Dict));
        }
        public static EmissionAmounts operator *(double e1, EmissionAmounts e2)
        {
            return new EmissionAmounts((e2 as Dict) * e1);
        }
        public static EmissionAmounts operator *(EmissionAmounts e1, double e2)
        {
            return new EmissionAmounts((e1 as Dict) * e2);
        }
        public static EmissionAmounts operator /(EmissionAmounts e1, Parameter e2)
        {
            return new EmissionAmounts((e1 as Dict) / e2);
        }
        public static EmissionAmounts operator /(EmissionAmounts e1, LightValue e2)
        {
            return new EmissionAmounts((e1 as Dict) / e2);
        }
        public static EmissionAmounts operator /(EmissionAmounts e1, double e2)
        {
            return new EmissionAmounts((e1 as Dict) / e2);
        }

        public static EmissionAmounts operator +(EmissionAmounts e1, EmissionAmounts e2)
        {
            return new EmissionAmounts((e1 as Dict) + (e2 as Dict));
        }

        /// <summary>
        /// adds values of a second EmissionResults to the current one, replaces += and enhance performance because less new objects are created
        /// </summary>
        /// <param name="emissionResults"></param>
        public new void Addition(Dict emissionResults)
        {
            (this as Dict).Addition(emissionResults);
        }
        /// <summary>
        /// Adds the values to the current object after mutiplying them by the double p
        /// Saves time comparing to results = results + (values * p) as this single operation do not create any new objects for the results
        /// </summary>
        /// <param name="p"></param>
        /// <param name="values"></param>
        public void MulAdd(double p, EmissionAmounts values)
        {
            (this as Dict).MulAdd(p, values as Dict);
        }
        public static EmissionAmounts operator -(EmissionAmounts e1)
        {
            return new EmissionAmounts(-(e1 as Dict));
        }
        public static EmissionAmounts operator -(EmissionAmounts e1, EmissionAmounts e2)
        {
            return new EmissionAmounts((e1 as Dict) - (e2 as Dict));
        }

        #endregion operators

        internal Dictionary<int, IValue> ToInterfaceDictionary()
        {
            Dictionary<int, IValue> results = new Dictionary<int, IValue>();
            foreach (KeyValuePair<int, double> pair in this)
            {
                ResultValue resVal = new ResultValue();
                resVal.UnitExpression = "kg"; //HARDCODED
                resVal.Value = pair.Value;
                resVal.ValueSpecie = Greet.DataStructureV4.Interfaces.Enumerators.ResultType.emission;
                resVal.SpecieId = pair.Key;
                results.Add(pair.Key, resVal);
            }
            return results;
        }

        /// <summary>
        /// Create groups for emissions and calculate the GHG value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Dictionary<int, IValue> GroupsToInterfaceDictionary(GData data)
        {
            Dictionary<int, IValue> groups = new Dictionary<int, IValue>();

            foreach (KeyValuePair<int, double> pair in this)
            {
                Gas gas = data.GasesData[pair.Key];
                List<int> memberships = new List<int>();
                memberships.AddRange(data.GasesData[pair.Key].Memberships);
                if (!memberships.Contains(1) && gas.GlobalWarmingPotential100 != null && gas.GlobalWarmingPotential100.ValueInDefaultUnit != 0)
                    memberships.Add(1); //hardcoded greenhouse gas group if there is a GWP associated with the resource
                if (!memberships.Contains(9) && gas.GlobalWarmingPotential20 != null && gas.GlobalWarmingPotential20.ValueInDefaultUnit != 0
                    && data.GasGroups.Any(item => item.Id == 9))
                    memberships.Add(9); //hardcoded greenhouse gas group if there is a GWP associated with the resource

                foreach (int groupId in memberships)
                {
                    double factor = 0;
                    if (groupId == 1)
                        factor = gas.GlobalWarmingPotential100.ValueInDefaultUnit;
                    if (groupId == 9)
                        factor = gas.GlobalWarmingPotential20.ValueInDefaultUnit;

                    if (gas.AccountDisociationCO2 && gas.CarbonRatio != null)
                    {
                        int co2Id = data.GasesData.BalancesIds[supportedBalanceTypes.carbon].GasRef;
                        Gas co2Gas = data.GasesData[co2Id];
                        if (co2Gas.CarbonRatio != null)
                        {
                            factor += gas.CarbonRatio.ValueInDefaultUnit / co2Gas.CarbonRatio.ValueInDefaultUnit;
                        }
                    }

                    if (groups.ContainsKey(groupId))
                    {
                        (groups[groupId] as ResultValue).Value += pair.Value * factor;
                    }
                    else
                    {
                        ResultValue resVal = new ResultValue();
                        resVal.UnitExpression = "kg"; //HARDCODED
                        resVal.Value = pair.Value * factor;
                        resVal.ValueSpecie = Greet.DataStructureV4.Interfaces.Enumerators.ResultType.emissionGroup;
                        resVal.SpecieId = pair.Key;
                        groups.Add(groupId, resVal);
                    }

                    
                }
            }

            return groups;
        }
    }
}

