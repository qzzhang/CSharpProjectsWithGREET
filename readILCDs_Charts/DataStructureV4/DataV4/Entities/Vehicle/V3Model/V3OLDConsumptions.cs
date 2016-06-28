using System;
using System.Collections.Generic;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// Energy V3OLDConsumptions for a V3OLDVehicle for a certain year for a certain mode.
    /// </summary>
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDConsumptions
    {
        #region attributes
        public int year;

        public Dictionary<string, V3OLDConsumption> tables;
        #endregion

        #region constructors
        public V3OLDConsumptions(int year)
        {
            this.year = year;
            this.tables = new Dictionary<string, V3OLDConsumption>();
        }
        #endregion

        #region methods
        internal List<XmlNode> ToXmlNode(System.Xml.XmlDocument xmlDoc)
        {
            List<XmlNode> tables = new List<XmlNode>();

            foreach (KeyValuePair<string, V3OLDConsumption> consp in this.tables)
            {
                if (consp.Value.Count > 0)
                {
                    XmlNode table_node = xmlDoc.CreateNode(consp.Key);
                    foreach (KeyValuePair<Parameter, Parameter> range in consp.Value)
                    {
                        table_node.AppendChild(xmlDoc.CreateNode("miles", range.Value.ToXmlAttribute(xmlDoc, "value"), range.Key.ToXmlAttribute(xmlDoc, "range"), xmlDoc.CreateAttr("notes", consp.Value.consumptionNotes[range.Key])));
                    }
                    tables.Add(table_node);
                }
            }

            return tables;
        }
        #endregion
    }
}
