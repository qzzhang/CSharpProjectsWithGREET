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
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;


namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public abstract class AMode : IHaveAPicture, IAMode, IHaveMetadata, IGREETEntity
    {
        #region constants
        const string xmlAttrModifiedOn = "modified-on";
        const string xmlAttrModifiedBy = "modified-by";
        #endregion

        #region private attriutes
        private int id;
        private string name;
        /// <summary>
        /// The filename of picture to use for this mode
        /// </summary>
        private string pictureName = Constants.EmptyPicture;
        private Dictionary<int, ModeFuelShares> fuelShares = new Dictionary<int, ModeFuelShares>();
        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedOn = "";
        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedBy = "";
        /// <summary>
        /// Stores notes related to that mode as specified in the IHaveMetadata interface
        /// </summary>
        private string notes = "";
        #endregion

        #region public attributes

        /// <summary>
        /// Type defines how to convert that mode to a generic mode
        /// Type 1 : Ocean Tanker, Barge
        /// Type 2 : Heavy Duty Truck, Medium Heavy Duty Truck
        /// Type 3 : Pipeline
        /// Type 4 : Rail
        /// Type 5 : Magic Move
        /// </summary>
        public Modes.ModeType Type;

        internal Dictionary<int, EmissionRatios> ratios = new Dictionary<int, EmissionRatios>();
        public int ratiosBaselineFuel;

        public bool Discarded { get; set; }
        public string DiscardedReason { get; set; }
        public DateTime DiscardedOn { get; set; }
        public string DiscarededBy { get; set; }

        #endregion

        #region accessors
        [Browsable(true)]
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [Browsable(true)]
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        [Browsable(false)]
        public string PictureName
        {
            get { return pictureName; }
            set { pictureName = value; }
        }
        public abstract bool CanBackHaul { get; }
        [Browsable(false)]
        public Dictionary<int, ModeFuelShares> FuelSharesData
        {
            get { return fuelShares; }
            set { fuelShares = value; }
        }

        #endregion accessors

        #region constructor
        /// <summary>
        /// Basic constructor
        /// </summary>
        protected AMode(GData data)
        {
            string newModeName = "Mode";
            int number = 1;
            //Checks if a mode with the same name exist. If yes, it adds a number at the end and loops until the name is unique.
            while (data.ModesData.Values.Any(item => item.name == newModeName + " - " + number))
                number++;
            name = newModeName + " - " + number;

            id = Convenience.IDs.GetIdUnusedFromTimeStamp(data.ModesData.Keys.ToArray());
            PictureName = "empty.png";

            ModeFuelShares share = new ModeFuelShares();
            share.Id = 1;
            share.Name = "Default";
            FuelSharesData.Add(share.Id, share);
        }
        #endregion

        #region methods

        internal virtual void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            this.FuelSharesData.Clear();

            if (node.Attributes["discarded"] != null)
            {
                Discarded = Convert.ToBoolean(node.Attributes["discarded"].Value);
                DiscardedOn = Convert.ToDateTime(node.Attributes["discardedOn"].Value, GData.Nfi);
                DiscarededBy = node.Attributes["discardedBy"].Value;
                DiscardedReason = node.Attributes["discardedReason"].Value;
            }

            XmlNodeList fuel_shares_nodes = node.SelectNodes("fuel_shares/share");

            this.id = Convert.ToInt32(node.Attributes["id"].Value);
            this.notes = node.Attributes["notes"].Value;
            if (node.Attributes[xmlAttrModifiedOn] != null)
                this.ModifiedOn = node.Attributes[xmlAttrModifiedOn].Value;
            if (node.Attributes[xmlAttrModifiedBy] != null)
                this.ModifiedBy = node.Attributes[xmlAttrModifiedBy].Value;

            foreach (XmlNode fs in fuel_shares_nodes)
            {
                ModeFuelShares mfs = new ModeFuelShares(data, fs, optionalParamPrefix + "_fuelshare");
                this.FuelSharesData.Add(mfs.Id, mfs);

                foreach (ModeEnergySource fref in mfs.ProcessFuels.Values)
                {
                    //addded relatively to bug #922 issue 1
                    if (fref.TechnologyFrom == -1)
                        fref.TechnologyFrom = 2 * this.Id;
                    if (fref.TechnologyTo == -1)
                        fref.TechnologyTo = 2 * this.Id - 1;
                }
            }
        }
        public virtual XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode t4 = xmlDoc.CreateNode("mode");

            if (this.Discarded)
            {
                t4.Attributes.Append(xmlDoc.CreateAttr("discarded", Discarded));
                t4.Attributes.Append(xmlDoc.CreateAttr("discardedReason", DiscardedReason));
                t4.Attributes.Append(xmlDoc.CreateAttr("discardedOn", DiscardedOn));
                t4.Attributes.Append(xmlDoc.CreateAttr("discardedBy", DiscarededBy));
            }

            t4.Attributes.Append(xmlDoc.CreateAttr("id", id));
            t4.Attributes.Append(xmlDoc.CreateAttr("notes", notes));
            t4.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedOn, this.ModifiedOn.ToString(GData.Nfi)));
            t4.Attributes.Append( xmlDoc.CreateAttr(xmlAttrModifiedBy, this.ModifiedBy));

            return t4;
        }

        public virtual void XMLFuelSharesAndER(XmlNode node, XmlDocument xmlDoc)
        {
            XmlNode shares_node = xmlDoc.CreateNode("fuel_shares");
            foreach (ModeFuelShares mfs in this.FuelSharesData.Values)
                shares_node.AppendChild(mfs.ToXmlNode(xmlDoc));
            if (this.FuelSharesData.Count > 0)
                node.AppendChild(shares_node);
            if (this.ratiosBaselineFuel != 0)
            {
                XmlNode emissions_ratios = xmlDoc.CreateNode("emissions_ratios", xmlDoc.CreateAttr("baseline_fuel", this.ratiosBaselineFuel));
                foreach (EmissionRatios ratio in this.ratios.Values)
                    emissions_ratios.AppendChild(ratio.ToXmlNode(xmlDoc));
                node.AppendChild(emissions_ratios);
            }

        }

        public override string ToString()
        {
            return this.name;
        }
     
        /// <summary>
        /// Each type of mode has to be able to check it's structural integrity and return a string of errors if any are detected
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckIntegrity(GData data, bool showIds, out string errorMessage);

        #endregion methods

        #region IAMode

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IModeFuelShares> FuelShares
        {
            get
            {
                List<IModeFuelShares> fuelSharesList = new List<IModeFuelShares>();
                foreach (ModeFuelShares modeFuelShares in this.FuelSharesData.Values)
                    fuelSharesList.Add(modeFuelShares as IModeFuelShares);
                return fuelSharesList;
            }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> TransportedResources
        {
            get
            {
                List<int> transportedResources = new List<int>();
                if (this is INeedPayload)
                {
                    foreach (MaterialTransportedPayload materialPayload in (this as INeedPayload).Payload.Values)
                    {
                        transportedResources.Add(materialPayload.Reference);
                    }
                }
                if (this is ModePipeline)
                {
                    foreach (PipelineMaterialTransported pipelineMaterial in (this as ModePipeline).EnergyIntensity.Values)
                    {
                        transportedResources.Add(pipelineMaterial.Reference);
                    }
                }
                return transportedResources;
            }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void FromXmlNode(IData data, XmlNode node)
        {
            this.FromXmlNode(data as GData, node, "");
        }

        #endregion

        #region IHaveMetadata Members

        public string Notes { get { return this.notes; } set { this.notes = value; } }

        public string ModifiedBy { get { return this.modifiedOn; } set { this.modifiedOn = value; } }

        public string ModifiedOn { get { return this.modifiedBy; } set { this.modifiedBy = value; } }

        #endregion

     
    }
   
}
