using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greet.UnitLib2;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV3.Interfaces;
using Greet.DataStructureV3.Entities;

namespace Greet.DataStructureV3.ResultsStorage
{
    /// <summary>
    /// This class is used to store amount of different resources
    /// which might be represented using different units. We might have 
    /// values in joules, cubic meters, or kilograms in this dictionary.
    /// </summary>
    [Serializable]
    internal class ResourceAmountsNewUnit
    {
        public DVDictNewUnit resources;

        #region constructors
        public ResourceAmountsNewUnit()
        {
            resources = new DVDictNewUnit();
        }

        public ResourceAmountsNewUnit(LightValue val)
        {
            resources.Add(0, val);
        }
        #endregion constructors

        #region methods
        public void Clear()
        {
            resources.Clear();
        }

        /// <summary>
        /// Adds a quantity of material to the fuels/groups dictionaries
        /// Tries to convert to energy whenever it is possible for the material
        /// </summary>
        /// <param name="material_id"></param>
        /// <param name="amount"></param>
        public void AddFuel(ResourceData rd, LightValue amount)
        {

            //taking care of adding the amount in the fuels list
            if (this.resources.ContainsKey(rd.Id))
            {
                if (this.resources[rd.Id].Dim == amount.Dim)
                    this.resources[rd.Id].Value += amount.Value;
                else
                    this.resources[rd.Id] += rd.ConvertTo(this.resources[rd.Id].Dim.Dim, amount);
            }
            else
            {
                if (rd.CanConvertTo(DimensionUtils.ENERGY, amount)) //hardcoded
                    this.resources.Add(rd.Id, rd.ConvertToEnergy(amount)); //hardcoded
                else
                    this.resources.Add(rd.Id, amount);
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
            if (this.resources.ContainsKey(material_id))
            {
                if (this.resources[material_id].Dim == amount.Dim)
                    this.resources[material_id].Value += amount.Value;
                else
                    this.resources[material_id] += material_id_for_conversion.ConvertTo(this.resources[material_id].Dim.Dim, amount);
            }
            else
            {
                if (material_id_for_conversion.CanConvertTo(DimensionUtils.ENERGY, amount)) //hardcoded
                    this.resources.Add(material_id, material_id_for_conversion.ConvertToEnergy(amount)); //hardcoded
                else
                    this.resources.Add(material_id, amount);
            }
        }

        public string GetCommonValueAbbrev()
        {
            return "";
        }

        public override string ToString()
        {
            return this.resources.TotalEnergy().ToString();
        }

        /// <summary>
        /// Returns an XML node containing the amount used by each resource, used to export results to XML node
        /// </summary>
        /// <param name="processDoc">The document for namespace URI</param>
        /// <param name="parent">The parent node to which individual results are going to be appened to</param>
        internal void AppendToXmlNode(System.Xml.XmlDocument processDoc, XmlNode parent)
        {
            foreach (KeyValuePair<int, LightValue> pair in this.resources)
            {
                XmlNode gas = processDoc.CreateNode("resource", processDoc.CreateAttr("ref", pair.Key), processDoc.CreateAttr("amount", pair.Value));
                parent.AppendChild(gas);
            }
        }



        #endregion methods

        #region operators

        public static ResourceAmountsNewUnit operator *(ResourceAmountsNewUnit e1, Parameter e2)
        {
            ResourceAmountsNewUnit result = new ResourceAmountsNewUnit();
            result.resources = e1.resources * e2;
            return result;

        }
        public static ResourceAmountsNewUnit operator *(ResourceAmountsNewUnit e1, LightValue e2)
        {
            ResourceAmountsNewUnit result = new ResourceAmountsNewUnit();
            result.resources = e1.resources * e2;
            return result;
        }
        public static ResourceAmountsNewUnit operator *(Parameter e2, ResourceAmountsNewUnit e1)
        {
            ResourceAmountsNewUnit result = new ResourceAmountsNewUnit();
            result.resources = e1.resources * e2;
            return result;
        }
        public static ResourceAmountsNewUnit operator *(LightValue e2, ResourceAmountsNewUnit e1)
        {
            ResourceAmountsNewUnit result = new ResourceAmountsNewUnit();
            result.resources = e1.resources * e2;
            return result;
        }
        public static ResourceAmountsNewUnit operator *(ResourceAmountsNewUnit e1, double e2)
        {
            ResourceAmountsNewUnit result = new ResourceAmountsNewUnit();
            result.resources = e1.resources * e2;
            return result;
        }
        public static ResourceAmountsNewUnit operator *(double e1, ResourceAmountsNewUnit e2)
        {
            return e2 * e1;
        }
        public static ResourceAmountsNewUnit operator /(ResourceAmountsNewUnit e1, Parameter e2)
        {
            ResourceAmountsNewUnit result = new ResourceAmountsNewUnit();
            result.resources = e1.resources / e2;
            return result;
        }
        public static ResourceAmountsNewUnit operator /(ResourceAmountsNewUnit e1, LightValue e2)
        {
            ResourceAmountsNewUnit result = new ResourceAmountsNewUnit();
            result.resources = e1.resources / e2;
            return result;
        }
        public static ResourceAmountsNewUnit operator /(ResourceAmountsNewUnit e1, double e2)
        {
            ResourceAmountsNewUnit result = new ResourceAmountsNewUnit();
            result.resources = e1.resources / e2;
            return result;
        }
        public static ResourceAmountsNewUnit operator +(ResourceAmountsNewUnit e1, ResourceAmountsNewUnit e2)
        {
            ResourceAmountsNewUnit result = new ResourceAmountsNewUnit();
            result.resources = e1.resources + e2.resources;
            return result;
        }
        /// <summary>
        /// adds values of a second MaterialEnem to the current one, replaces += and enhance performance because less new objects are created
        /// </summary>
        /// <param name="materialAmounts"></param>
        internal void Addition(ResourceAmountsNewUnit materialAmounts)
        {
            this.resources.Addition(materialAmounts.resources);
        }

        /// <summary>
        /// adds values of a second Dictionary to the current one
        /// </summary>
        /// <param name="e2"></param>
        internal void Addition(Dictionary<int, Parameter> e2)
        {
            this.resources.Addition(e2);

        }

        /// <summary>
        /// Adds the values to the current object after mutiplying them by the double p
        /// Saves time comparing to results = results + (values * p) as this single operation do not create any new objects for the results
        /// </summary>
        /// <param name="p"></param>
        /// <param name="values"></param>
        internal void MulAdd(double p, ResourceAmountsNewUnit values)
        {
            this.resources.MulAdd(p, values.resources);
        }

        public static ResourceAmountsNewUnit operator -(ResourceAmountsNewUnit e1, ResourceAmountsNewUnit e2)
        {
            ResourceAmountsNewUnit result = new ResourceAmountsNewUnit();
            result.resources = e1.resources - e2.resources;
            return result;
        }
        #endregion operators

        #region accessors

        public string BottomUnitName
        {
            set
            {
                this.resources.BottomUnitName = value;
            }
            get
            {
                return this.resources.BottomUnitName;
            }
        }

        #endregion

        internal Dictionary<int, IValue> ToInterfaceDictionary()
        {
            Dictionary<int, IValue> results = new Dictionary<int, IValue>();
            foreach (KeyValuePair<int, LightValue> pair in this.resources)
            {
                ResultValue resVal = new ResultValue();
                resVal.Unit = pair.Value.Dim.PreferedExpression;
                resVal.Value = pair.Value.Value;
                resVal.ValueSpecie = Greet.DataStructureV3.Interfaces.Enumerators.ResultType.resource;
                resVal.SpecieID = pair.Key;
                results.Add(pair.Key, resVal);
            }
            return results;
        }

        internal Dictionary<int, IValue> GroupsToInterfaceDictionary(GData data)
        {
            Dictionary<int, IValue> groups = new Dictionary<int, IValue>();

            foreach (KeyValuePair<int, LightValue> pair in this.resources)
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
                                value.Unit = pair.Value.Dim.PreferedExpression;
                                value.ValueSpecie = Greet.DataStructureV3.Interfaces.Enumerators.ResultType.resourceGroup;
                                value.SpecieID = pair.Key;

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
