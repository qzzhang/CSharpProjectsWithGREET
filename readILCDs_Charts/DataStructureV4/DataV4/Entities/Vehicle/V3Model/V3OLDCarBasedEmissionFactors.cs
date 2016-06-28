using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities.Legacy
{
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    internal class V3OLDCarBasedEmissionFactors : V3OLDCarEmissionsFactors, IEnumerable<KeyValuePair<int, V3OLDCarEmissionValue>>
    {
        #region attributes

        public int baseTechno;
        Dictionary<int, V3OLDCarEmissionValue> ratios = new Dictionary<int, V3OLDCarEmissionValue>();
        List<V3OLDCarEmissionNode> nodes = new List<V3OLDCarEmissionNode>();

        #endregion attributes

        #region constructor

        internal V3OLDCarBasedEmissionFactors(int baseTechnology)
        {
            this.baseTechno = baseTechnology;
        }

        internal V3OLDCarBasedEmissionFactors(GData data, XmlNode node, string optionalParamPrefix)
        {
            string notes;
            XmlNode base_node = node.SelectSingleNode("base");
            this.baseTechno = Convert.ToInt32(base_node.Attributes["techno"].Value);

            foreach (XmlNode gas in node.SelectNodes("gas_ratio"))
            {
                int gasId = Convert.ToInt32(gas.Attributes["ref"].Value);
                V3OLDCarEmissionValue dfactor;
                if (gas.Attributes["calculated"] != null)
                {
                    gas.Attributes["calculated"].Value = "0;" + gas.Attributes["calculated"].Value;
                    dfactor = new V3OLDCarEmissionValue(data, gas.Attributes["calculated"], false, optionalParamPrefix + "_calc_" + gasId);
                    dfactor.useCalculated = dfactor.EmParameter.UseOriginal;
                    dfactor.CanCalculated = true;
                }
                else
                {
                    dfactor = new V3OLDCarEmissionValue(data, gas.Attributes["factor"], false, optionalParamPrefix + "_fact_" + gasId);
                }

                dfactor.isBased = true;

                if (gas.Attributes["notes"] != null)
                    notes = gas.Attributes["notes"].Value;
                else
                    notes = "";
                this.Add(gasId, dfactor, notes);

            }
        }

        protected V3OLDCarBasedEmissionFactors(SerializationInfo info, StreamingContext context)
        {
            nodes = (List<V3OLDCarEmissionNode>)info.GetValue("nodes", typeof(List<V3OLDCarEmissionNode>));

        }

        #endregion constuctor

        #region accessors

        internal V3OLDCarEmissionValue RatioValue(int gasIndex)
        {
            return this.ratios[gasIndex];
        }

        public override V3OLDCarEmissionValue this[int index]
        {
            get
            {
                return this.ratios[index];
            }
            set
            {
                this.ratios[index] = value;
            }
        }

        internal Dictionary<int, V3OLDCarEmissionValue> Ratios
        {
            get { return this.ratios; }
            set { this.ratios = value; }
        }

        public override Dictionary<int, V3OLDCarEmissionValue>.KeyCollection Keys { get { return this.ratios.Keys; } }

        #endregion accessors

        #region methods

        /// <summary>
        /// Adds the given item to the list
        /// </summary>
        /// <param name="key">gas ID</param>
        /// <param name="value">Double value - Factor</param>
        /// <param name="notes">Notes associated with the emission</param>
        public override void Add(int key, V3OLDCarEmissionValue value, string notes)
        {
            this.ratios.Add(key, value);
            this.nodes.Add(new V3OLDCarEmissionNode(key, value, notes));
        }

        public override void Remove(int key)
        {
            this.ratios.Remove(key);
            this.nodes.Remove(this.nodes.Find(temp => temp.gasId == key));
        }

        public override IEnumerator<KeyValuePair<int, V3OLDCarEmissionValue>> GetEnumerator()
        {
            foreach (int i in this.Keys)
                yield return new KeyValuePair<int, V3OLDCarEmissionValue>(i, this[i]);
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
            XmlNode base_node = doc.CreateNode("base", doc.CreateAttr("techno", this.baseTechno));
            yearNode.AppendChild(base_node);
            foreach (V3OLDCarEmissionNode node in this.nodes)
            {
                XmlNode gasFactor = doc.CreateNode("gas_ratio", doc.CreateAttr("ref", node.gasId));
                XmlAttribute factor;

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

        public override bool ContainsKey(int key)
        {
            return this.ratios.ContainsKey(key);
        }

        #endregion
    }
}
