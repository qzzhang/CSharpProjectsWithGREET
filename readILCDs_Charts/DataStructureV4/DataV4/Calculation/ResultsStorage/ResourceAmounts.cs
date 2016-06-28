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
using System.Xml;
using Greet.UnitLib3;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.Entities;
using System.Runtime.Serialization;

namespace Greet.DataStructureV4.ResultsStorage
{
    /// <summary>
    /// This class is used to store amount of different resources
    /// which might be represented using different units. We might have 
    /// values in joules, cubic meters, or kilograms in this dictionary.
    /// </summary>
    [Serializable]
    public class ResourceAmounts : DVDict
    {
        #region constructors
        public ResourceAmounts()
        {
           
        }

        public ResourceAmounts(DVDict d)
            : base(d)
        {
        }


        protected ResourceAmounts(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion constructors

        #region methods

        /// <summary>
        /// Adds a quantity of material to the fuels/groups dictionaries
        /// Tries to convert to energy whenever it is possible for the material
        /// </summary>
        /// <param name="material_id"></param>
        /// <param name="amount"></param>
        public void AddFuel(ResourceData rd, LightValue amount)
        {

            //taking care of adding the amount in the fuels list
            if (this.ContainsKey(rd.Id))
            {
                if (this[rd.Id].Dim == amount.Dim)
                    this[rd.Id] += amount.Value;
                else
                    this[rd.Id] += rd.ConvertTo(this[rd.Id].Dim, amount);
            }
            else
            {
                if (rd.CanConvertTo(DimensionUtils.ENERGY, amount)) //hardcoded
                    this.Add(rd.Id, rd.ConvertTo(DimensionUtils.ENERGY, amount)); //hardcoded
                else
                    this.Add(rd.Id, amount);
            }
        }

        /// <summary>
        /// Adds a quantity of material to the fuels/groups dictionaries
        /// Tries to convert to energy whenever it is possible for the material id given as a parameter for conversion
        /// </summary>
        /// <param name="material_id"></param>
        /// <param name="amount"></param>
        public void AddFuel(int material_id, LightValue amount, ResourceData material_id_for_conversion)
        {

            //taking care of adding the amount in the fuels list
            if (this.ContainsKey(material_id))
            {
                if (this[material_id].Dim == amount.Dim)
                    this[material_id] += amount.Value;
                else
                    this[material_id] += material_id_for_conversion.ConvertTo(this[material_id].Dim, amount);
            }
            else
            {
                if (material_id_for_conversion.CanConvertTo(DimensionUtils.ENERGY, amount)) //hardcoded
                    this.Add(material_id, material_id_for_conversion.ConvertTo(DimensionUtils.ENERGY, amount)); //hardcoded
                else
                    this.Add(material_id, amount);
            }
        }

        public string GetCommonValueAbbrev()
        {
            return "";
        }

        /// <summary>
        /// Returns an XML node containing the amount used by each resource, used to export results to XML node
        /// </summary>
        /// <param name="processDoc">The document for namespace URI</param>
        /// <param name="parent">The parent node to which individual results are going to be appened to</param>
        internal void AppendToXmlNode(System.Xml.XmlDocument processDoc, XmlNode parent)
        {
            foreach (KeyValuePair<int, LightValue> pair in this)
            {
                XmlNode gas = processDoc.CreateNode("resource", processDoc.CreateAttr("ref", pair.Key), processDoc.CreateAttr("amount", pair.Value));
                parent.AppendChild(gas);
            }
        }

        #endregion methods

        #region operators

        public static ResourceAmounts operator *(ResourceAmounts e1, Parameter e2)
        {
            return new ResourceAmounts((e1 as DVDict) * e2);
        }
        public static ResourceAmounts operator *(ResourceAmounts e1, LightValue e2)
        {
            return new ResourceAmounts((e1 as DVDict) * e2);
        }
        public static ResourceAmounts operator *(Parameter e2, ResourceAmounts e1)
        {
            return new ResourceAmounts((e1 as DVDict) * e2);
        }
        public static ResourceAmounts operator *(LightValue e2, ResourceAmounts e1)
        {
            return new ResourceAmounts((e1 as DVDict) * e2);
        }
        public static ResourceAmounts operator *(ResourceAmounts e1, double e2)
        {
            return new ResourceAmounts((e1 as DVDict) * e2);
        }
        public static ResourceAmounts operator *(double e2, ResourceAmounts e1)
        {
            return new ResourceAmounts((e1 as DVDict) * e2);
        }
        public static ResourceAmounts operator /(ResourceAmounts e1, Parameter e2)
        {
            return new ResourceAmounts((e1 as DVDict) / e2);
        }
        public static ResourceAmounts operator /(ResourceAmounts e1, LightValue e2)
        {
            return new ResourceAmounts((e1 as DVDict) / e2);
        }
        public static ResourceAmounts operator /(ResourceAmounts e1, double e2)
        {
            return new ResourceAmounts((e1 as DVDict) / e2);
        }
        public static ResourceAmounts operator +(ResourceAmounts e1, ResourceAmounts e2)
        {
            return new ResourceAmounts((e1 as DVDict) + e2);
        }
        public static ResourceAmounts operator -(ResourceAmounts e1, ResourceAmounts e2)
        {
            return new ResourceAmounts((e1 as DVDict) - e2);
        }
        #endregion operators

        #region accessors

        [Obsolete("For compatibility with old 2014 API, please use BottomDim instead")]
        public string BottomUnitName
        {
            get 
            {
                AQuantity qty = Units.QuantityList.ByDim(this.BottomDim);
                return qty.SiUnit.Expression;
            }
            set 
            {
                Unit unit = Units.UnitsList.Values.FirstOrDefault(item => item.Expression == value);
                if (unit == null)
                {
                    Units.UnitsList.TryGetValue(value, out unit);
                    if (unit == null)
                        throw new Exception("Unrecognized unit, you must use a unit from the UnitLib3 UnitList. Please check available units, or report to developing team");
                }

                AQuantity qty = Units.QuantityList.Values.FirstOrDefault(item => item.Units.Contains(unit));
                if (qty == null)
                    throw new Exception("Failed to find a quantity associated with the unit " + unit.Name + ". Please check available units, or report to developing team");

                this.BottomDim = qty.Dim;
            }
        
        }
        #endregion

        internal Dictionary<int, IValue> ToInterfaceDictionary()
        {
            Dictionary<int, IValue> results = new Dictionary<int, IValue>();
            foreach (KeyValuePair<int, LightValue> pair in this)
            {
                ResultValue resVal = new ResultValue();
                resVal.UnitExpression = Units.QuantityList.ByDim(pair.Value.Dim).SiUnit.Expression;
                resVal.Value = pair.Value.Value;
                resVal.ValueSpecie = Greet.DataStructureV4.Interfaces.Enumerators.ResultType.resource;
                resVal.SpecieId = pair.Key;
                results.Add(pair.Key, resVal);
            }
            return results;
        }

        public Dictionary<int, IValue> GroupsToInterfaceDictionary(GData data)
        {
            Dictionary<int, IValue> groups = new Dictionary<int, IValue>();
            
            foreach (KeyValuePair<int, LightValue> pair in this)
            {
                LightValue amount = pair.Value;
                foreach (int membership in data.ResourcesData[pair.Key].Memberships)
                {
                    if (data.ResourcesData.Groups.ContainsKey(membership))
                    {
                        Group group = data.ResourcesData.Groups[membership];
                        List<int> groupIdAndIncludes = new List<int>();
                        foreach (int subGroup in group.IncludeInGroups)
                        {
                            if (data.ResourcesData[pair.Key].Memberships.Contains(subGroup) == false)
                                groupIdAndIncludes.Add(subGroup);
                        }
                        groupIdAndIncludes.Add(group.Id);

                        foreach (int groupId in groupIdAndIncludes)
                        {
                            if (groups.ContainsKey(groupId))
                            {
                                (groups[groupId] as ResultValue).Value += pair.Value.Value;
                            }
                            else
                            {
                                ResultValue value = new ResultValue();
                                value.Value = pair.Value.Value;
                                value.UnitExpression = Units.QuantityList.ByDim(pair.Value.Dim).SiUnit.Expression;
                                value.ValueSpecie = Greet.DataStructureV4.Interfaces.Enumerators.ResultType.resourceGroup;
                                value.SpecieId = pair.Key;

                                groups.Add(groupId, value);
                            }
                        }

                    }
                }
            }
            return groups;
        }

    }
}
