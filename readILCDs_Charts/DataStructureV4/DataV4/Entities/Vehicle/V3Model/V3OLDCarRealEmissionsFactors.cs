using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// To hold the emission factors read from the database, and stored as a double Value
    /// </summary>
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    internal class V3OLDCarRealEmissionsFactors : V3OLDCarEmissionsFactors, IEnumerable<KeyValuePair<int, V3OLDCarEmissionValue>>
    {

        #region attributes

        Dictionary<int, V3OLDCarEmissionValue> emissionFactors = new Dictionary<int, V3OLDCarEmissionValue>();
        List<V3OLDCarEmissionNode> nodes = new List<V3OLDCarEmissionNode>();
        #endregion

        #region constructors
        /// <summary>
        /// Create an empty dictionary of emission factors
        /// </summary>
        public V3OLDCarRealEmissionsFactors()
            : base()
        { }

        /// <summary>
        /// Initialize a specific value for each available gas in the database
        /// </summary>
        /// <param name="init_val"></param>
        internal V3OLDCarRealEmissionsFactors(GData data, double init_val, bool create_emission_factors, bool isV3OLDVehicle, bool isBaseV3OLDVehicle, string optionalParamPrefix = "")
            : base()
        {
            if (create_emission_factors)
            {
                foreach (Gas gas in data.GasesData.Values.Where(item => item.Memberships.Contains(3) || item.Memberships.Contains(5)))
                {
                    if (data.GasesData.ContainsKey(6) && isV3OLDVehicle)//if we are creating emission factors for a V3OLDVehicle
                    {
                        if (gas.Memberships.Contains(6) || gas.Memberships.Contains(5)) //HARDCODED
                        {
                            if (isBaseV3OLDVehicle)
                                this.Add(gas.Id, new V3OLDCarEmissionValue(data, "V3OLDVehicle_emission_factor", init_val, false, optionalParamPrefix + "em_real_" + gas.Id), "");
                            else
                                this.Add(gas.Id, new V3OLDCarEmissionValue(data, "%", init_val, false, optionalParamPrefix + "em_ratio_" + gas.Id), "");
                        }
                    }
                    else//if not creating emission factors for a V3OLDVehicle
                        this.Add(gas.Id, new V3OLDCarEmissionValue(data, "emission_factor", init_val, false, optionalParamPrefix + "em_real_" + gas.Id), "");
                }
            }
        }

        internal V3OLDCarRealEmissionsFactors(GData data, XmlNode node, string optionalParamPrefix)
        {
            string status, notes;
            status = "creating gases dictionary";
            foreach (XmlNode gas in node.SelectNodes("emission"))
            {
                try
                {
                    status = "reading gas id";
                    int gasId = Convert.ToInt32(gas.Attributes["ref"].Value);
                    status = "reading gas factor";
                    V3OLDCarEmissionValue dfactor;
                    if (gas.Attributes["calculated"] != null)
                    {
                        //calculated factors only store the override value, we "add" a fictive default value which will be later replaced by the calculated value
                        gas.Attributes["calculated"].Value = "0;" + gas.Attributes["calculated"].Value;
                        dfactor = new V3OLDCarEmissionValue(data, gas.Attributes["calculated"], true, optionalParamPrefix + "_calc_" + gasId);

                        dfactor.useCalculated = dfactor.EmParameter.UseOriginal;
                        dfactor.CanCalculated = true;
                    }
                    else
                    {
                        dfactor = new V3OLDCarEmissionValue(data, gas.Attributes["factor"], false, optionalParamPrefix + "_fact_" + gasId);
                    }
                    status = "add gas factor to the dictionary";
                    dfactor.isBased = false;
                    if (gas.Attributes["notes"] != null)
                        notes = gas.Attributes["notes"].Value;
                    else
                        notes = "";
                    this.Add(gasId, dfactor, notes);
                }
                catch (Exception e)
                {
                    LogFile.Write("Error 12:" + e.Message + "\r\n" + status + "\r\n" + gas.OuterXml + "\r\n" + gas.OwnerDocument.Name + "\r\n");
                }
            }
        }

        protected V3OLDCarRealEmissionsFactors(SerializationInfo info, StreamingContext context)
        {
            nodes = (List<V3OLDCarEmissionNode>)info.GetValue("nodes", typeof(List<V3OLDCarEmissionNode>));

        }

        #endregion

        #region methods

        public override IEnumerator<KeyValuePair<int, V3OLDCarEmissionValue>> GetEnumerator()
        {
            foreach (int i in this.Keys)
                yield return new KeyValuePair<int, V3OLDCarEmissionValue>(i, this.emissionFactors[i]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void GetObjectData(SerializationInfo info,
                                   StreamingContext context)
        {
            info.AddValue("nodes", nodes, typeof(List<V3OLDCarEmissionNode>));
        }
        public override XmlNode ToXmlNode(XmlDocument doc, ref XmlNode yearNode)
        {
            foreach (V3OLDCarEmissionNode node in nodes)
            {
                XmlNode gasFactor = doc.CreateNode("emission", doc.CreateAttr("ref", node.gasId));
                XmlAttribute factor;

                //TODO now we write the double back into the DoubleValue, here we loose the information contained in the Calculated_Value flag!!!! needs to be fixed

                V3OLDCarEmissionValue val = node.dfactor;
                if (node.dfactor.CanCalculated == false)
                {
                    factor = doc.CreateAttr("factor", val.EmParameter);
                    gasFactor.Attributes.Append(factor);
                }
                else
                {
                    gasFactor.Attributes.Append(doc.CreateAttr("calculated", val.EmParameter.UserValue.ToString(GData.Nfi) + ";" + val.useCalculated.ToString() + ";" + val.EmParameter.UnitGroupName));
                }

                yearNode.AppendChild(gasFactor);

                //if (node.notes != "")
                gasFactor.Attributes.Append(doc.CreateAttr("notes", node.notes));

            }
            return yearNode;
        }

        /// <summary>
        /// Adds the given item to the list
        /// </summary>
        /// <param name="key">gas ID</param>
        /// <param name="value">Double value - Factor</param>
        /// <param name="notes">Notes associated with the emission</param>
        public override void Add(int key, V3OLDCarEmissionValue value, string notes)
        {
            this.emissionFactors.Add(key, value);
            this.nodes.Add(new V3OLDCarEmissionNode(key, value, notes));
        }

        public override void Remove(int key)
        {
            this.emissionFactors.Remove(key);
            this.nodes.Remove(this.nodes.Find(temp => temp.gasId == key));
        }

        public override bool ContainsKey(int key)
        {
            return this.emissionFactors.ContainsKey(key);
        }

        #endregion

        #region accessors

        public override Dictionary<int, V3OLDCarEmissionValue>.KeyCollection Keys { get { return this.emissionFactors.Keys; } }

        public override V3OLDCarEmissionValue this[int index]
        {
            get
            {
                return this.emissionFactors[index];
            }
            set
            {
                this.emissionFactors[index] = value;
                this.nodes.Find(item => item.gasId == index).dfactor = value;
            }
        }

        #endregion

    }
}
