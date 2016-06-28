using System;
using System.Linq;
using System.Xml;

using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// Charge sustaining mode, a regular mode where it is a assumed that no exernal source of power is used from the V3OLDVehicle. The ICE or equivalent and
    /// battery can be used together, but only the fuel consumption is considered to be consumed from outside of the V3OLDVehicle system
    /// </summary>
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDRegularMode : V3OLDVehicleOperationalMode
    {
    
        #region constructors

        public V3OLDRegularMode(int V3OLDVehicle_reference)
            : base(V3OLDVehicle_reference)
        { }

        public V3OLDRegularMode(GData data, XmlNode xmlNode, int V3OLDVehicle_reference, string optionalParamPrefix)
            : base(data, xmlNode, V3OLDVehicle_reference, optionalParamPrefix)
        {
            this._mpg = new V3OLDMPGsTS(data, xmlNode, optionalParamPrefix);
       
        }
        #endregion

        #region methods

        internal override XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode modeNode = xmlDoc.CreateNode("mode", xmlDoc.CreateAttr("mpgge_ref", _mpggeRef), xmlDoc.CreateAttr("mode", _name), xmlDoc.CreateAttr("notes", this.Notes));

            foreach (V3OLDVehicleFuel pair in this.FuelsUsedWithoutBaseFuels)
            {
                modeNode.AppendChild(pair.ToXmlNode(xmlDoc));
            }
            foreach (V3OLDCarYearEmissionsFactors year in this._technologies.Values)
            {
                XmlNode yearNode = year.ToXmlNode(xmlDoc);
                if (this._mpg.Keys.Contains(year.Year))
                    yearNode.AppendChild(xmlDoc.CreateNode("mpg", _mpg[year.Year].ToXmlAttribute(xmlDoc, "mpg"), xmlDoc.CreateAttr("notes", _mpg.notes[year.Year])));
               
                modeNode.AppendChild(yearNode);
            }
            return modeNode;
        }

        #endregion methods
    }
}
