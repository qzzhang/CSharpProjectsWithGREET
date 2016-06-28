using System;
using System.Linq;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Entities;
using Greet.DataStructureV4;

namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// A fuel used by a V3OLDVehicle, ca be a reference to a pathway or a mix of pathways. Each fuel aslo have a share which might be calculated.
    /// from the baseline V3OLDVehicle share or not
    /// </summary>
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDVehicleFuel
    {
        #region attributes
        private InputResourceReference _inputResourceRef;

        private Parameter _volumeShare;

        private bool usedForCarbonBalance;
       
        private bool isFuelFromBaseVehicle;
       
        private string notes = "";
        #endregion

        #region constructors

        public V3OLDVehicleFuel(GData data, XmlNode node, string optionalParamPrefix)
        {
            this._inputResourceRef = new InputResourceReference(node);
            this._volumeShare = data.ParametersData.CreateRegisteredParameter(node.Attributes["share"], optionalParamPrefix + "_fuel_" + this._inputResourceRef.ResourceId + "_share");
           
            if (node.Attributes["used_in_carbon_balance"] != null)
                this.usedForCarbonBalance = Convert.ToBoolean(node.Attributes["used_in_carbon_balance"].Value);
            else
                this.usedForCarbonBalance = true;
            
            if (node.Attributes["notes"] != null)
                this.notes = node.Attributes["notes"].Value;
        }

        /// <summary>
        /// <para>Creates a new V3OLDVehicle fuel based on a resource ID</para>
        /// <para>The upstream assigned will be either a mix or a pathway and need to be checked!</para>
        /// <para>We'll use the first mix or pathway available as an upstream!</para>
        /// </summary>
        /// <param name="data">Database containing all processes, mixes and resources</param>
        /// <param name="resourceID">Resource ID for the upstream</param>
        public V3OLDVehicleFuel(GData data, int resourceID)
        {
            this._inputResourceRef = this.FindFirstAvailableMixOrPathway(data, resourceID);
            _volumeShare = data.ParametersData.CreateRegisteredParameter("%", 0, 1);

            usedForCarbonBalance = true;
            notes = "";
        }

        #endregion
        
        #region methods

        /// <summary>
        /// This method has to be moved to a higher class or deleted
        /// </summary>
        /// <param name="data">Database containing all processes, mixes and resources</param>
        /// <param name="resourceID">Resource ID for the upstream</param>
        /// <returns>The first mix or pathway found in the database to produce the resource given as an ID</returns>
        private InputResourceReference FindFirstAvailableMixOrPathway(GData data, int resourceID)
        {
            InputResourceReference rk = new InputResourceReference();
            rk.ResourceId = resourceID;
            rk.SourceMixOrPathwayID = -1;

            //Try to look if their is a mix we can use
            foreach (Mix mix in data.MixesData.Values.Where(item => item.output.ResourceId == resourceID))
            {
                rk.SourceMixOrPathwayID = mix.Id;
                rk.SourceType = Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix;
                break;//no need to keep looping after we find the first mix available. 
            }

            //if we could not find a mix we can look at pathways 
            if (rk.SourceMixOrPathwayID == -1)
            {
                foreach (Pathway pw in data.PathwaysData.Values)
                {
                    if (data.PathwayMainOutputResouce(pw.Id) == resourceID)
                    {
                        rk.SourceMixOrPathwayID = pw.Id;
                        rk.SourceType = Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway;
                        break;
                    }
                }
            }
            return rk;
        }
        /// <summary>
        /// Returns a fuel xml node
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        internal XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            return this.ToXmlNode(xmlDoc, "fuel");
        }
        /// <summary>
        /// The same as ToXmlNode but we can specify the node name
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal XmlNode ToXmlNode(XmlDocument xmlDoc, string name)
        {
            XmlNode fuel_node = xmlDoc.CreateNode(name, xmlDoc.CreateAttr("share", this.VolumeShare), xmlDoc.CreateAttr("notes", this.Notes)); //hardcoded group name
            this.InputResourceRef.ToXmlNode(xmlDoc, fuel_node);
            /*fuel_node.Attributes.Append(xmlDoc.CreateAttr("ref", this.material.Id));
            if (this.material.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix)
            {
                if (Holder.Project.Data.Resources[this.material.Id].Mixes[this.material.MixOrPathwayID].Entities.Count == 1 && Holder.Project.Data.Resources[this.material.Id].Mixes[this.material.MixOrPathwayID].Entities[0].share.CurrentValue.ValueInDefaultUnit == 1
                    && Holder.Project.Data.Resources[this.material.Id].Mixes[this.material.MixOrPathwayID].Entities[0] is PathwayRef)
                {
                    PathwayRef pw = Holder.Project.Data.Resources[this.material.Id].Mixes[this.material.MixOrPathwayID].Entities[0] as PathwayRef;
                    fuel_node.Attributes.Append(xmlDoc.CreateAttr("pathway", pw.Reference));
                }
                else
                    fuel_node.Attributes.Append(xmlDoc.CreateAttr("mix", this.material.MixOrPathwayID));
            }
            else if (this.material.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway)
                fuel_node.Attributes.Append(xmlDoc.CreateAttr("pathway", this.material.MixOrPathwayID));*/


            return fuel_node;
        }
        /// <summary>
        /// Public ToXmlNode to be used outside of the Data Project
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public XmlNode ConvertToXmlNode(XmlDocument xmlDoc)
        {
            return this.ToXmlNode(xmlDoc);
        }

        public bool EqualAtrributes(V3OLDVehicleFuel other)
        {
            if (this.InputResourceRef.Equals(other.InputResourceRef) && this.VolumeShare.ValueInDefaultUnit == other.VolumeShare.ValueInDefaultUnit &&
                 this.UsedForCarbonBalance == other.UsedForCarbonBalance && this.IsFuelFromBaseVehicle == other.IsFuelFromBaseVehicle && this.Notes == other.Notes)
            {
                return true;
            }
            else
                return false;
        }
        
        #endregion

        #region Accessors

        /// <summary>
        /// Defines the origin for an input resource for the Fuel.
        /// It can be from well, mix, pathway, feed or main output of the previous process
        /// </summary>
        public InputResourceReference InputResourceRef
        {
            get { return _inputResourceRef; }
            set { _inputResourceRef = value; }
        }
        /// <summary>
        /// Defines the share of this specific fuel
        /// </summary>
        public Parameter VolumeShare
        {
            get { return _volumeShare; }
            set { _volumeShare = value; }
        }

        /// <summary>
        /// To check if the fuel is a carbon based one. True: If it is carbon based fuel. False: If it is not carbon based fuel. 
        /// </summary>
        public bool UsedForCarbonBalance
        {
            get { return usedForCarbonBalance; }
            set { usedForCarbonBalance = value; }
        }

        /// <summary>
        /// Determines if this fuel is from Base V3OLDVehicle.
        /// </summary>
        public bool IsFuelFromBaseVehicle
        {
            get { return isFuelFromBaseVehicle; }
            set { isFuelFromBaseVehicle = value; }
        }
        /// <summary>
        /// Notes about this fuel.
        /// </summary>
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }
        #endregion
    }
}
