using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Abstract class for a fuel production entity of a mix. This class should only be used as such within a mix.
    /// holds ths shares and notes only, rest of the definition in PathwayProductionEntity and MixProductionEntity
    /// </summary>
    [Serializable]
    public abstract class FuelProductionEntity : IProductionItem, ISerializable
    {
        #region attributes
        /// <summary>
        /// Share of that entity within the mix. This share can be either
        /// volumetric, mass, or energy
        /// </summary>
        internal ParameterTS _share;
        /// <summary>
        /// Notes associated with that fuel production entity reference. This attribute might be removed
        /// and replace by the notes associated directly to the object we refer to the mix or the pathway
        /// </summary>
        internal string notes;
        #endregion attributes

        #region abstract definitions
        /// <summary>
        /// Defines how to save this enity as an xml node in the database
        /// </summary>
        /// <param name="doc">XmlDocument for the namespace URI</param>
        /// <returns>XmlNode containing the information for that fuel production entity</returns>
        public abstract XmlNode ToXmlNode(XmlDocument doc);
        /// <summary>
        /// Checks if the reference exists in the database, if we refer a pathway does this pathway exists
        /// in the database, if a mix does this mix exists in the database.
        /// </summary>
        public abstract bool Exists(GData data);
        #endregion

        #region constructors


        /// <summary>
        /// Populates the shares and nodes attributes of the fuel production enitiy from an XML node
        /// </summary>
        /// <param name="node"></param>
        protected FuelProductionEntity(GData data, XmlNode node, string optionalParamPrefix)
        {
            //trying to read a single share or a time series
            if (node.Attributes["share"] != null)
                this._share = new ParameterTS(data, node.Attributes["share"], optionalParamPrefix + "_fpe_shares");
            else if (node.SelectSingleNode("shares") != null)
                this._share = new ParameterTS(data, node.SelectSingleNode("shares"), optionalParamPrefix + "_fpe_shares");
            if (node.Attributes["notes"] != null)
                this.notes = node.Attributes["notes"].Value;
        }
        protected FuelProductionEntity()
        { }
        /// <summary>
        /// Creates a new Fuel Production Entity from the share only
        /// What's the use of that, can it be deleted ?
        /// </summary>
        /// <param name="share"></param>
        protected FuelProductionEntity(GData data, double share)
        {
            this._share = new ParameterTS(data, "%", share);
        }

        protected FuelProductionEntity(SerializationInfo information, StreamingContext context)
        {
            _share = (ParameterTS)information.GetValue("share", typeof(ParameterTS));
        }
        #endregion

        #region methoss
        /// <summary>
        /// Adds the shares and notes to a pathway production enity node or a mix production entity node
        /// </summary>
        /// <param name="entityNode"></param>
        /// <param name="xmlDoc"></param>
        internal void ToXmlNodeCommon(XmlNode entityNode, XmlDocument xmlDoc)
        {
            entityNode.AppendChild(this._share.ToXmlNode(xmlDoc, "shares"));
            entityNode.Attributes.Append(xmlDoc.CreateAttr("notes", this.notes));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("share", _share);
        }

        #endregion

        #region accessors
        /// <summary>
        /// returns the share for that fuel production entity
        /// </summary>
        public ParameterTS Share
        {
            get { return this._share; }
        }

        #endregion

        #region IProductionItem

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int MixOrPathwayId
        {
            get
            {
                if (this is PathwayProductionEntity)
                    return (this as PathwayProductionEntity).PathwayReference;
                else if (this is MixProductionEntity)
                    return (this as MixProductionEntity).MixReference;
                else
                    throw new InvalidOperationException("Unexpected abstract class implementation");
            }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public Greet.DataStructureV4.Interfaces.Enumerators.SourceType SourceType
        {
            get
            {
                if (this is PathwayProductionEntity)
                    return Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway;
                else if (this is MixProductionEntity)
                    return Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix;
                else
                    throw new InvalidOperationException("Unexpected abstract class implementation");
            }
        }

        double IProductionItem.Share
        {
            get
            {
                return _share.CurrentValue.ValueInDefaultUnit;
            }
        }

        #endregion
    }
}
