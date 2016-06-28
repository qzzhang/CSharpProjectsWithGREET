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
using System.Linq;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Store an object that is used for accounting the emissions associated with one specie in the model
    /// </summary>
    [Serializable]
    public class Gas : IGroupAvailable, IHaveAPicture, IGas, IHaveMetadata, IGREETEntity
    {
        #region constants
        const string xmlAttrModifiedOn = "modified-on";
        const string xmlAttrModifiedBy = "modified-by";
        #endregion

        #region attributes
        /// <summary>
        /// Unique atribute for this gas object in the gases database
        /// </summary>
        int _id = -1;
        /// <summary>
        /// Name for this gas object
        /// </summary>
        string _name;
        /// <summary>
        /// A picture name that can be associated with the gas object
        /// </summary>
        string _pictureName = Constants.EmptyPicture;
        /// <summary>
        /// If this gas is a child of another one, we want to know about it for agregating the results
        /// even if IDs are differents.
        /// </summary>
        int _childOf = -1;
        /// <summary>
        /// Global warming potential compared to CO2 for 100 years
        /// </summary>
        Parameter _globalWarmingPotential100 = null;
        /// <summary>
        /// Global warming potential compared to CO2 for 20 years
        /// </summary>
        Parameter _globalWarmingPotential20 = null;
        /// <summary>
        /// Mass sulfur ratio for this gas
        /// </summary>
        Parameter _sulfurRatio = null;
        /// <summary>
        /// Mass carbon ratio for this gas
        /// </summary>
        Parameter _carbonRatio = null;
        /// <summary>
        /// This gas can be member of other gases, if it does, results can be agregated per group
        /// </summary>
        List<int> _memberships = new List<int>();
        /// <summary>
        /// If true the gas object will be shown in the GUI in the results, otherwise it will still be calculated
        /// but we'll avoid showing it
        /// </summary>
        bool _showInResults = true;
        /// <summary>
        /// Notes associated with this gas item.
        /// </summary>
        string _notes;
        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedOn = "";
        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedBy = "";

        public bool Discarded { get; set; }
        public string DiscardedReason { get; set; }
        public DateTime DiscardedOn { get; set; }
        public string DiscarededBy { get; set; }

        /// <summary>
        /// Account for dissociation into CO2 for GHG purposes
        /// </summary>
        private bool _accountDisociationCO2 = false;

        #endregion attributes

        #region constructors

        /// <summary>
        /// Default consturctor, creates a gas with new properties and an Id that is unique to the gases database.
        /// </summary>
        public Gas(GData data)
        {
            this.Id = Convenience.IDs.GetIdUnusedFromTimeStamp(data.GasesData.Keys);
            this.Name = "New Gas " + this.Id;
            _pictureName = "empty.png";
            _notes = "";
            _sulfurRatio = data.ParametersData.CreateRegisteredParameter("%", 0, 0, "gas_" + this.Id + "_sratio_default");
            _carbonRatio = data.ParametersData.CreateRegisteredParameter("%", 0, 0, "gas_" + this.Id + "_cratio_default");
            _globalWarmingPotential100 = data.ParametersData.CreateRegisteredParameter("%", 0, 0, "gas_" + this.Id + "_gwp_default");
            _globalWarmingPotential20 = data.ParametersData.CreateRegisteredParameter("%", 0, 0, "gas_" + this.Id + "_gwp_default");
            _memberships = new List<int>();
        }

        /// <summary>
        /// Creates a new object from an XML node.
        /// </summary>
        /// <param name="gasNode">The XML node representation of this object</param>
        public Gas(GData data, XmlNode gasNode)
        {
            this.FromXmlNode(data, gasNode);
        }

        #endregion constructors

        #region accessors
        /// <summary>
        /// Get or Set the name of the picture associated to the gas object
        /// The picture name do not refer to a specific file, but to a picture name stored in the database
        /// images libraries
        /// </summary>
        public string PictureName
        {
            get { return this._pictureName; }
            set { this._pictureName = value; }
        }

        /// <summary>
        /// Strings that represents this object, here only the name is returned
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._name;
        }
        /// <summary>
        /// Get or Set the global warming potential value for this gas relative to CO2 for 100 years
        /// </summary>
        public Parameter GlobalWarmingPotential100
        {
            get { return _globalWarmingPotential100; }
            set { _globalWarmingPotential100 = value; }
        }
        /// <summary>
        /// Get or Set the global warming potential value for this gas relative to CO2 for 20 years
        /// </summary>
        public Parameter GlobalWarmingPotential20
        {
            get { return _globalWarmingPotential20; }
            set { _globalWarmingPotential20 = value; }
        }
        /// <summary>
        /// Get or Set the list of memeberships that this gas is a member of
        /// </summary>
        public List<int> Memberships
        {
            get { return _memberships; }
            set { _memberships = value; }
        } 
        /// <summary>
        /// Get or Sets the mass carbon ratio associated to that gas object
        /// </summary>
        public Parameter CarbonRatio
        {
            get { return _carbonRatio; }
            set { _carbonRatio = value; }
        }
        /// <summary>
        /// Get or Set the mass sulfur ratio associated with that gas object
        /// </summary>
        public Parameter SulfurRatio
        {
            get { return _sulfurRatio; }
            set { _sulfurRatio = value; }
        }
        /// <summary>
        /// Get or Sets the ID for this object
        /// The ID should be unique among the gases database
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// Gets or Sets the Name for this gas object
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// Get or Set the show in results property for that object
        /// If true, the emissions will show this gas as a results, if false the emissions 
        /// are still calculated but not shown in the GUI
        /// </summary>
        public bool ShowInResults
        {
            get { return _showInResults; }
            set { _showInResults = value; }
        }
        /// <summary>
        /// When set to TRUE the model will account for carbon in this gas, convert that to an equivalent amount of CO2 and use that for the GHG purposes
        /// </summary>
        public bool AccountDisociationCO2
        {
            get { return _accountDisociationCO2; }
            set { _accountDisociationCO2 = value; }
        }

        #endregion accessors

        #region methods
        /// <summary>
        /// Build the instance of that object from an XML node
        /// </summary>
        /// <param name="data"></param>
        /// <param name="gasNode"></param>
        /// <param name="optionalParamPrefix"></param>
        private void FromXmlNode(GData data, XmlNode gasNode, string optionalParamPrefix)
        {

            string status = "";
            try
            {
                if (gasNode.Attributes["discarded"] != null)
                {
                    Discarded = Convert.ToBoolean(gasNode.Attributes["discarded"].Value);
                    DiscardedOn = Convert.ToDateTime(gasNode.Attributes["discardedOn"].Value, GData.Nfi);
                    DiscarededBy = gasNode.Attributes["discardedBy"].Value;
                    DiscardedReason = gasNode.Attributes["discardedReason"].Value;
                }

                status = "reading id";
                this._id = Convert.ToInt32(gasNode.Attributes["id"].Value);
                status = "reading name";
                this._name = gasNode.Attributes["name"].Value;
                status = "reading picture name";
                if (gasNode.Attributes["picture"].NotNullNOrEmpty())
                    this._pictureName = gasNode.Attributes["picture"].Value;
                else
                    this._pictureName = "empty.png";
                status = "reading sulfur ratio";
                if (gasNode.Attributes["s_ratio"] != null && !string.IsNullOrEmpty(gasNode.Attributes["s_ratio"].Value))
                    this._sulfurRatio = data.ParametersData.CreateRegisteredParameter(gasNode.Attributes["s_ratio"], "res_"+this.Id+"sratio");
                else
                    this._sulfurRatio = data.ParametersData.CreateRegisteredParameter("%", 0, 0, "gas_" + this.Id + "_sratio_default");
                status = "reading carbon ratio";
                if (gasNode.Attributes["c_ratio"] != null && !string.IsNullOrEmpty(gasNode.Attributes["c_ratio"].Value))
                    this._carbonRatio = data.ParametersData.CreateRegisteredParameter(gasNode.Attributes["c_ratio"], "res_"+this.Id+"cratio");
                else
                    this._carbonRatio = data.ParametersData.CreateRegisteredParameter("%", 0, 0, "gas_" + this.Id + "_cratio_default");
                if (gasNode.Attributes["notes"] != null)
                    this._notes = gasNode.Attributes["notes"].Value;

                if (gasNode.Attributes[xmlAttrModifiedOn] != null)
                    this.ModifiedOn = gasNode.Attributes[xmlAttrModifiedOn].Value;
                if (gasNode.Attributes[xmlAttrModifiedBy] != null)
                    this.ModifiedBy = gasNode.Attributes[xmlAttrModifiedBy].Value;

                status = "reading global warming potential";
                if (gasNode.Attributes["global_warming_potential"] != null && !string.IsNullOrEmpty(gasNode.Attributes["global_warming_potential"].Value))
                    this._globalWarmingPotential100 = data.ParametersData.CreateRegisteredParameter(gasNode.Attributes["global_warming_potential"], "res_" + this.Id + "gwp");
                else
                    this._globalWarmingPotential100 = data.ParametersData.CreateRegisteredParameter("%", 0, 0, "gas_" + this.Id + "_gwp_default");

                if (gasNode.Attributes["global_warming_potential20"] != null && !string.IsNullOrEmpty(gasNode.Attributes["global_warming_potential20"].Value))
                    this._globalWarmingPotential20 = data.ParametersData.CreateRegisteredParameter(gasNode.Attributes["global_warming_potential20"], "res_" + this.Id + "gwp20");
                else
                    this._globalWarmingPotential20 = data.ParametersData.CreateRegisteredParameter("%", 0, 0, "gas_" + this.Id + "_gwp20_default");

                if (gasNode.Attributes["co2dissoc"] != null)
                    _accountDisociationCO2 = Convert.ToBoolean(gasNode.Attributes["co2dissoc"].Value);
                else
                    _accountDisociationCO2 = false;

                status = "reading child of";
                if (gasNode.Attributes["childof"] != null && !string.IsNullOrEmpty(gasNode.Attributes["global_warming_potential"].Value))
                    _childOf = Convert.ToInt32(gasNode.Attributes["childof"].Value);

                if (gasNode.Attributes["showInResults"] != null && gasNode.Attributes["showInResults"].Value != "")
                    _showInResults = bool.Parse(gasNode.Attributes["showInResults"].Value);

                this._memberships = new List<int>();
                foreach (XmlNode node in gasNode.SelectNodes("membership"))
                {
                    try
                    {
                        this._memberships.Add(Convert.ToInt32(node.Attributes["group_id"].Value));
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Error 45:" + e.Message);
                    }
                }

            }
            catch (Exception e)
            {
                LogFile.Write("Error 44:" + status + "\r\n" + e.Message);
            }
        }
        /// <summary>
        /// Creates an XML node object filled up with the data associated with that Gas object
        /// </summary>
        /// <param name="xmlDoc">XML Document object that will be used for the NamespaceURI</param>
        /// <returns>New XMLNode object that contains the data</returns>
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode gasNode = xmlDoc.CreateNode("gas");
            if (this.Discarded)
            {
                gasNode.Attributes.Append(xmlDoc.CreateAttr("discarded", Discarded));
                gasNode.Attributes.Append(xmlDoc.CreateAttr("discardedReason", DiscardedReason));
                gasNode.Attributes.Append(xmlDoc.CreateAttr("discardedOn", DiscardedOn));
                gasNode.Attributes.Append(xmlDoc.CreateAttr("discardedBy", DiscarededBy));
            }
            gasNode.Attributes.Append(xmlDoc.CreateAttr("name", this._name));
            gasNode.Attributes.Append(xmlDoc.CreateAttr("picture", this._pictureName));
            gasNode.Attributes.Append(xmlDoc.CreateAttr("id", this._id));
            gasNode.Attributes.Append(xmlDoc.CreateAttr("c_ratio", this._carbonRatio));
            gasNode.Attributes.Append(xmlDoc.CreateAttr("s_ratio", this._sulfurRatio));
            gasNode.Attributes.Append(xmlDoc.CreateAttr("notes", this._notes));
            gasNode.Attributes.Append(xmlDoc.CreateAttr("showInResults", false));
            gasNode.Attributes.Append(xmlDoc.CreateAttr("global_warming_potential", this._globalWarmingPotential100));
            gasNode.Attributes.Append(xmlDoc.CreateAttr("global_warming_potential20", this._globalWarmingPotential20));
            gasNode.Attributes.Append(xmlDoc.CreateAttr("co2dissoc", _accountDisociationCO2));
            gasNode.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedOn, this.ModifiedOn.ToString(GData.Nfi)));
            gasNode.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedBy, this.ModifiedBy));
            foreach (int member in this._memberships)
                gasNode.AppendChild(xmlDoc.CreateNode("membership", xmlDoc.CreateAttr("group_id", member)));
            return gasNode;
        }

        /// <summary>
        /// Checks integrity of the gas object and returns errors in a string if any
        /// </summary>
        /// <returns></returns>
        internal bool CheckIntegrity(GData data, bool showIds, out string errorMessage)
        {
            errorMessage = "";
            bool canHandleCalculations = true;
            foreach (int memb in this.Memberships)
            {
                if (!data.GasesData.Groups.Keys.Contains(memb))
                {
                    errorMessage += "Contains a group membership (" + memb + ") that does not exist\r\n";
                }
            }

            if (!String.IsNullOrEmpty(errorMessage))
                errorMessage += "\r\n";

            return canHandleCalculations;
        }

        #endregion methods

        /// <summary>
        /// Returns the ID stored in the
        /// childOf of attribute
        /// </summary>
        public int IsChildOf
        {
            get
            {
                return _childOf;
            }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void FromXmlNode(IData data, XmlNode node)
        {
            this.FromXmlNode(data as GData, node, "");
        }
        double IGas.GlobalWarmingPotential100
        {
            get {
                if (_globalWarmingPotential100 != null)
                    return _globalWarmingPotential100.ValueInDefaultUnit;
                else
                    return 0;
            }
        }

        double IGas.GlobalWarmingPotential20
        {
            get {
                if (_globalWarmingPotential20 != null)
                    return _globalWarmingPotential20.ValueInDefaultUnit;
                else
                    return 0;
            }
        }


        /// <summary>
        /// Get or Sets the mass carbon ratio associated to that gas object
        /// </summary>
        public IParameter Carbon_Ratio
        {
            get { return _carbonRatio; }
        }

        /// <summary>
        /// Get or Set the mass sulfur ratio associated with that gas object
        /// </summary>
        public IParameter Sulfur_Ratio
        {
            get { return _sulfurRatio; }
        }

        #region IHaveMetadata Members

        /// <summary>
        /// Get or Set notes associated to the gas object
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public string ModifiedBy { get { return this.modifiedOn; } set { this.modifiedOn = value; } }

        public string ModifiedOn { get { return this.modifiedBy; } set { this.modifiedBy = value; } }

        #endregion

    }
}