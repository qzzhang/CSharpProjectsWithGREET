    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.LoggerLib;
using System.Text;


namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// A single step operated by a defined mode in the overall transportation process
    /// </summary>
    [Serializable]
    public class TransportationStep : ITransportationStep
    {
        #region Constants
        public static KeyValuePair<string, bool> EmissionWithFuelsUpstream = new KeyValuePair<string, bool>("With Partial Upstream", true);
        public static KeyValuePair<string, bool> StepOnSiteEmissionsOnly = new KeyValuePair<string, bool>("On Site", true);
        public static KeyValuePair<string, bool> EnergyWithFuelsUpstream = new KeyValuePair<string, bool>("With Partial Upstream", true);
        public static KeyValuePair<string, bool> StepOnSiteEnergyOnly = new KeyValuePair<string, bool>("On Site", true);
        public static KeyValuePair<string, bool> EIFor = new KeyValuePair<string, bool>("EI for ", true);
        public static KeyValuePair<string, bool> HeaderEmissions = new KeyValuePair<string, bool>("Emissions", true);
        public static KeyValuePair<string, bool> HeaderEnergy = new KeyValuePair<string, bool>("Energy", true);
        public static KeyValuePair<string, bool> HeaderGeneral = new KeyValuePair<string, bool>("General", true);
        public static KeyValuePair<string, bool> HeaderEnergyIntensity = new KeyValuePair<string, bool>("Energy Intensity", true);

        #endregion

        #region attributes

        public Guid Id = Guid.NewGuid();
        private int originReference = -1;
        private int destinationReference = -1;
        private ParameterTS distance;
        private ParameterTS share;
        private ParameterTS urbanShare;
        public int modeReference = -1;
        private int transportationProcessReferenceId = -1;
        private string notes = "";
        private int fuelShareRef = -1;
        public bool backHaul;
        public Enem lossesEnem = new Enem();
        private nLoss loss = null;

        #endregion attributes

        #region constructors

        public TransportationStep(GData data)
        {
            this.fuelShareRef = 1; //default fuel share id for all of  the modes
            this.backHaul = true;
            this.distance = new ParameterTS(data, "m", 0);
            this.share = new ParameterTS(data, "%", 0);
            this.urbanShare = new ParameterTS(data, "%", 0);
        }

        public TransportationStep(GData data, XmlNode node, int process_ref, string optionalParamPrefix)
        {
            string status = "";
            //this.origin = new List<string>();
            try
            {
                status = "reading step guid";
                if (node.Attributes["id"] != null)
                    this.Id = new Guid(node.Attributes["id"].Value);
                status = "reading fuel share reference";
                if (node.Attributes["fuel_share_ref"] != null)
                    this.fuelShareRef = Convert.ToInt32(node.Attributes["fuel_share_ref"].Value);
                else
                    this.fuelShareRef = 1;
                status = "reading origin reference";
                this.originReference = Convert.ToInt32(node.Attributes["origin_ref"].Value);
                status = "reading destination reference";
                this.destinationReference = Convert.ToInt32(node.Attributes["dest_ref"].Value);
                status = "reading distance";
                this.distance = new ParameterTS(data, node.SelectSingleNode("distance"), optionalParamPrefix + "_step_" + this.originReference + this.destinationReference + this.modeReference + "_dist");
                status = "reading share";
                this.share = new ParameterTS(data, node.SelectSingleNode("share"), optionalParamPrefix + "_step_" + this.originReference + this.destinationReference + this.modeReference + "_share");
                status = "reading reference";
                this.modeReference = Convert.ToInt32(node.Attributes["ref"].Value);
                status = "assigning tranpsortation process";
                this.transportationProcessReferenceId = process_ref;
                status = "reading back haul";
                if (node.Attributes["back_haul"] != null)
                    this.backHaul = Convert.ToBoolean(node.Attributes["back_haul"].Value);
                else
                    this.backHaul = true;
                status = "reading to destination attribute";
                this.urbanShare = new ParameterTS(data, node.SelectSingleNode("urban_share"), optionalParamPrefix + "_step_" + this.originReference + this.destinationReference + this.modeReference + "_urban");
                XmlNode loss_node = node.SelectSingleNode("nloss");
                if (loss_node != null)
                {
                    this.loss = new nLoss(data, loss_node, optionalParamPrefix + "_loss");
                }
            }
            catch (Exception e)
            {
                LogFile.Write("Error 35:" + node.OwnerDocument.BaseURI + "\r\n" +
                    node.OuterXml + "\r\n" +
                    e.Message + "\r\n" +
                    status + "\r\n"
                    );
                throw e;
            }
        }

        #endregion constructors

        #region accessors
        [Browsable(false)]
        public int FuelShareRef
        {
            get { return fuelShareRef; }
            set { fuelShareRef = value; }
        }
        [Browsable(false)]
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }
        [Browsable(false)]
        public int TransportationProcessReferenceId
        {
            get { return transportationProcessReferenceId; }
            set { transportationProcessReferenceId = value; }
        }
        [Browsable(false)]
        public ParameterTS Distance
        {
            get { return distance; }
            set { distance = value; }
        }
        [Browsable(false)]
        public int Reference
        {
            get { return modeReference; }
            set { modeReference = value; }
        }
        [Browsable(false)]
        public ParameterTS Share
        {
            get { return share; }
            set { share = value; }
        }
        [Browsable(false)]
        public ParameterTS UrbanShare
        {
            get { return urbanShare; }
            set { urbanShare = value; }
        }
        [Browsable(false)]
        public int DestinationRef
        {
            get { return destinationReference; }
            set { destinationReference = value; }
        }
        [Browsable(false)]
        public int OriginRef
        {
            get { return originReference; }
            set { originReference = value; }
        }
        [Browsable(true), DisplayName("Share"), CategoryAttribute("Distance / Share")]
        public string NiceShare
        { get { return "Obsolete Replaced with DataStructureV4 entities";
            //return this.share.NiceValueInOverridePrefixedWithAttribute;
        } }
        [Browsable(true), DisplayName("m"), CategoryAttribute("Distance / Share")]
        public string NiceDistance
        { get {return "Obsolete Replaced with DataStructureV4 entities";
            //return this.distance.NiceValueInOverridePrefixedWithAttribute;
        } }

        public nLoss Loss
        {
            get { return loss; }
            set { loss = value; }
        }
        #endregion accessors

        #region ITransportationProcess

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int ModeReference
        {
            get { return modeReference; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int OriginReference
        {
            get { return originReference; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int DestinationReference
        {
            get { return destinationReference; }
        }

        #endregion

        #region methods

        /// <summary>
        /// Checks the integrity of the step for a given transported resource ID
        /// </summary>
        /// <param name="data">Dataset containing resources, modes,...</param>
        /// <param name="showIds">If true IDs will be visible in the error messages</param>
        /// <param name="fixFixableIssues">If true, the algorithm will attempt to fix some fixable issues</param>
        /// <param name="transportedResourceID">The ID of the resource being transpored by that step</param>
        /// <param name="stepIssue">String containing human readable issues concerning that step</param>
        /// <returns>True if all is good, false otherwise</returns>
        internal bool CheckIntegrity(GData data, bool showIds, bool fixFixableIssues, int transportedResourceID, out string stepIssue)
        {
            bool canHandleCalculations = true;
            StringBuilder problems = new StringBuilder();
            if (data.ModesData.ContainsKey(this.modeReference))
            {
                if (data.ModesData[this.modeReference] is ModeTankerBarge)
                    if ((data.ModesData[this.modeReference] as ModeTankerBarge).Payload.ContainsKey(transportedResourceID) == false)
                    {
                        problems.AppendLine(" - Mode: " + data.ModesData[this.modeReference].Name + " Payload for the resource Id:" + transportedResourceID + " is not defined for the Mode: " + (data.ModesData[this.modeReference] as ModeTankerBarge).ToString());
                        canHandleCalculations = false;
                    }

                if (data.ModesData[this.modeReference] is ModeTruck)
                    if ((data.ModesData[this.modeReference] as ModeTruck).Payload.ContainsKey(transportedResourceID) == false)
                    {
                        problems.AppendLine(" - Mode: " + data.ModesData[this.modeReference].Name + " Payload for the resource Id:" + transportedResourceID + " is not defined for the Mode: " + (data.ModesData[this.modeReference] as ModeTruck).ToString());
                        canHandleCalculations = false;
                    }

                if (data.ModesData[this.modeReference].FuelSharesData.ContainsKey(this.FuelShareRef))
                {
                    foreach (KeyValuePair<InputResourceReference, ModeEnergySource> fuel_id in data.ModesData[this.modeReference].FuelSharesData[this.FuelShareRef].ProcessFuels)
                    {
                        string fuelShareName = " - Mode: " + data.ModesData[this.modeReference].Name +
                                " selected fuel share: " + data.ModesData[this.modeReference].FuelSharesData[this.FuelShareRef].Name;

                        if (!data.TechnologiesData.ContainsKey(fuel_id.Value.TechnologyFrom))
                            problems.AppendLine(fuelShareName
                                + " has an undefined reference for technology from");
                        if (!data.TechnologiesData.ContainsKey(fuel_id.Value.TechnologyTo))
                            problems.AppendLine(fuelShareName
                                + " has an undefined reference for technology to");
                        if (fuel_id.Value.ResourceReference == null
                            || !data.ResourcesData.ContainsKey(fuel_id.Value.ResourceReference.ResourceId))
                        {
                            problems.AppendLine(fuelShareName
                                + " does not refers to an existing resource");
                            canHandleCalculations = false;
                        }
                        else
                        {
                            if (fuel_id.Value.ResourceReference.SourceType == Enumerators.SourceType.Mix
                                && !data.MixesData.ContainsKey(fuel_id.Value.ResourceReference.SourceMixOrPathwayID))
                            {
                                problems.AppendLine(fuelShareName
                                    + " refers to a non existing pathway for");
                                canHandleCalculations = false;
                            }
                            else if (fuel_id.Value.ResourceReference.SourceType == Enumerators.SourceType.Pathway
                                && !data.PathwaysData.ContainsKey(fuel_id.Value.ResourceReference.SourceMixOrPathwayID))
                            {
                                problems.AppendLine(fuelShareName
                                       + " refers to a non existing mix for selected resource");
                                canHandleCalculations = false;
                            }
                        }
                    }
                }
                else
                {
                    problems.AppendLine(" - Mode: " + data.ModesData[this.modeReference].Name + " Refers to a non existing fuel share, this transportation step cannot be calculated");
                    canHandleCalculations = false;
                }
            }
            else
            {
                problems.AppendLine(" - Reference to non existing mode ID Mode : " + this.modeReference + " for one of the transportation steps");
                canHandleCalculations = false;
            }

            stepIssue = problems.ToString();
            return canHandleCalculations;
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode step = xmlDoc.CreateNode("step");

            step.Attributes.Append(xmlDoc.CreateAttr("dest_ref", this.destinationReference));
            
            step.Attributes.Append(xmlDoc.CreateAttr("origin_ref", this.originReference));
            step.Attributes.Append(xmlDoc.CreateAttr("ref", this.modeReference));        
            step.Attributes.Append(xmlDoc.CreateAttr("fuel_share_ref", this.fuelShareRef));
            step.Attributes.Append(xmlDoc.CreateAttr("back_haul", this.backHaul));         
            step.Attributes.Append(xmlDoc.CreateAttr("id", this.Id));

            if (this.Loss != null)
                step.AppendChild(this.Loss.ToXmlNode(xmlDoc));
            step.AppendChild(this.distance.ToXmlNode(xmlDoc, "distance"));
            step.AppendChild(this.share.ToXmlNode(xmlDoc, "share"));
            step.AppendChild(this.urbanShare.ToXmlNode(xmlDoc, "urban_share"));

            return step;
        }

        #endregion methods

    }
}
