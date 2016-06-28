using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is read from the technologies file. 
    /// It holds the parameters of the current technology (name, id, ...) and inherits from EmissionsTimeSerie which holds the emission factors for each years
    /// </summary>
    [Serializable]
    public class TechnologyData : TimeSeries<EmissionsFactors>, IComparable, IHaveAPicture, ITechnology, IXmlObj, IHaveMetadata, IGREETEntity
    {
        #region constants
        const string xmlAttrModifiedOn = "modified-on";
        const string xmlAttrModifiedBy = "modified-by";
        #endregion

        #region attributes
        private int _id;
        private string _name = "";
        private string _pictureName = "empty.png";
        private int _outputResource = -1;
        private Parameter _massTransfer;
        /// <summary>
        /// each emission factors series for a technology is modeled for a specific fuel
        /// </summary>
        private int _inputResourceRef;
        /// <summary>
        /// If this technology defines it's emission factors as a ratio of another technology this value should be different than -1 and all EmissionsFactors will be BasedEmissionFactors
        /// </summary>
        private int _baseTechnologyData = -1;
        
        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string _modifiedOn = "";
        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string _modifiedBy = "";

        public bool Discarded { get; set; }
        public string DiscardedReason { get; set; }
        public DateTime DiscardedOn { get; set; }
        public string DiscarededBy { get; set; }
        #endregion

        #region constructors

        /// <summary>
        /// Creates a new empty technology for a specific resource ID
        /// Years are added without any emission factors.
        /// </summary>
        /// <param name="resourceId"></param>
        public TechnologyData(GData data)
        {
            this.Id = Convenience.IDs.GetIdUnusedFromTimeStamp(data.MixesData.Keys);
            this.InputResourceRef = -1;
            this._name = "New Technology " + this._id.ToString();
            this._pictureName = "empty.png";

            List<int> years = new List<int>();
        }

        /// <summary>
        /// Creates a new instance of a technology from an XML node
        /// Supports old technology format where resource ID was read from another XMLNode
        /// </summary>
        /// <param name="technology">XmlNode to read data from</param>
        /// <param name="resourceId">Old format input resource ID if known from another XMLNode</param>
        public TechnologyData(GData data, XmlNode technology, string optionalParamPrefix)
            : this(data)
        {
            this.FromXmlNode(data, technology, optionalParamPrefix);
        }

        protected TechnologyData(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _id = (int)info.GetValue("id", typeof(int));
            _name = (string)info.GetValue("name", typeof(string));
            _notes = (string)info.GetValue("notes", typeof(string));
            _pictureName = (string)info.GetValue("pictureName", typeof(string));
            _outputResource = (int)info.GetValue("outputResource", typeof(int));
            _massTransfer = (Parameter)info.GetValue("massTransfer", typeof(Parameter));
            _inputResourceRef = (int)info.GetValue("inputResource", typeof(int));
            _baseTechnologyData = (int)info.GetValue("baseTechnology", typeof(int));
            _mostRecentData = (int)info.GetValue("mostRecent", typeof(int));
            ModifiedBy = (string)info.GetValue(xmlAttrModifiedBy, typeof(string));
            ModifiedOn = (string)info.GetValue(xmlAttrModifiedOn, typeof(string));
        }

        #endregion

        #region accessors
        
        /// <summary>
        /// Each emission factors series for a technology is modeled for a specific fuel
        /// </summary>
        [Browsable(false)]
        public int InputResourceRef
        {
            get { return _inputResourceRef; }
            set { _inputResourceRef = value; }
        }
        [Browsable(false)]
        public string PictureName
        {
            get { return _pictureName; }
            set { _pictureName = value; }
        }
        /// <summary>
        /// Technology Id
        /// </summary>
        [Browsable(false)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        [Browsable(false)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [Browsable(false)]
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public override string ToString()
        {
            return this._name;
        }
        public int BaseTechnology
        {
            get { return _baseTechnologyData; }
            set { _baseTechnologyData = value; }
        }
        public bool IsRealEmissionFactors
        {
            get
            {
                bool isRealOnly = true;
                foreach (EmissionsFactors ts in this.Values)
                    isRealOnly &= ts is RealEmissionsFactors;
                return isRealOnly;
            }
        }
        #endregion

        #region methods

        public override void GetObjectData(SerializationInfo info,
                        StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("id", _id, typeof(int));
            info.AddValue("name", _name, typeof(string));
            info.AddValue("notes", _notes, typeof(string));
            info.AddValue("pictureName", _pictureName, typeof(string));
            info.AddValue("outputResource", _outputResource, typeof(int));
            info.AddValue("massTransfer", _massTransfer, typeof(Parameter));
            info.AddValue("inputResource", _inputResourceRef, typeof(int));
            info.AddValue("baseTechnology", _baseTechnologyData, typeof(int));
            info.AddValue(xmlAttrModifiedOn, ModifiedOn, typeof(string));
            info.AddValue(xmlAttrModifiedBy, ModifiedBy, typeof(string));
        }

        private void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            string status = "";

            try
            {
                if (node.Attributes["discarded"] != null)
                {
                    Discarded = Convert.ToBoolean(node.Attributes["discarded"].Value);
                    DiscardedOn = Convert.ToDateTime(node.Attributes["discardedOn"].Value, GData.Nfi);
                    DiscarededBy = node.Attributes["discardedBy"].Value;
                    DiscardedReason = node.Attributes["discardedReason"].Value;
                }

                status = "reading id";
                this._id = Convert.ToInt32(node.Attributes["id"].Value);
                status = "reading name";
                this._name = node.Attributes["name"].Value;
                status = "reading notes";
                if (node.Attributes["notes"] != null)
                    _notes = node.Attributes["notes"].Value;
                status = "reading input resource id";
                if (node.Attributes["inputRef"] != null)
                    this.InputResourceRef = Convert.ToInt32(node.Attributes["inputRef"].Value);
                status = "reading output resource id";
                if (node.Attributes["outputRef"] != null)
                    this._outputResource = Convert.ToInt32(node.Attributes["outputRef"].Value);
                status = "reading mass transfer";
                if (node.Attributes["massTransfert"] != null)
                    this._massTransfer = data.ParametersData.CreateRegisteredParameter(node.Attributes["massTransfer"]);
                status = "reading picture name";
                if (node.Attributes["picture"].NotNullNOrEmpty())
                    this._pictureName = node.Attributes["picture"].Value;
                else
                    this._pictureName = "empty.png";
                if (node.Attributes["basetech"].NotNullNOrEmpty())
                    this._baseTechnologyData = Convert.ToInt32(node.Attributes["basetech"].Value);
                status = "reading modified on";
                if (node.Attributes[xmlAttrModifiedOn] != null)
                    this.ModifiedOn = node.Attributes[xmlAttrModifiedOn].Value;
                status = "reading modified by";
                if (node.Attributes[xmlAttrModifiedBy] != null)
                    this.ModifiedBy = node.Attributes[xmlAttrModifiedBy].Value;
                if (node.Attributes["mostRecent"] != null)
                    _mostRecentData = Convert.ToInt32(node.Attributes["mostRecent"].Value);

                status = "creating new year dictionary";
                foreach (XmlNode year in node.SelectNodes("year"))
                {
                    try
                    {
                        EmissionsFactors yearD;
                        status = "reading value";

                        if (year.SelectSingleNode("base") == null && this._baseTechnologyData == -1)
                            yearD = new RealEmissionsFactors(data, year, optionalParamPrefix + "tech_" + this._id);
                        else
                        {
                            yearD = new BasedEmissionFactors(data, year, optionalParamPrefix + "tech_" + this._id);
                            if (this._baseTechnologyData == -1)//the old format when base was store for each of the year
                                this._baseTechnologyData = Convert.ToInt32(year.SelectSingleNode("base").Attributes["techno"].Value);
                        }
                                     
                        this.Add(yearD.Year, yearD);
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Error 6:" + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                LogFile.Write("Error 5:" + node.OwnerDocument.BaseURI + "\r\n" + node.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
                throw e;
            }
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode technoNode = xmlDoc.CreateNode("technology");
                
            if (this.Discarded)
            {
                technoNode.Attributes.Append(xmlDoc.CreateAttr("discarded", Discarded));
                technoNode.Attributes.Append(xmlDoc.CreateAttr("discardedReason", DiscardedReason));
                technoNode.Attributes.Append(xmlDoc.CreateAttr("discardedOn", DiscardedOn));
                technoNode.Attributes.Append(xmlDoc.CreateAttr("discardedBy", DiscarededBy));
            }
                
            technoNode.Attributes.Append(xmlDoc.CreateAttr("id", _id));
            technoNode.Attributes.Append(xmlDoc.CreateAttr("name", _name));
            technoNode.Attributes.Append(xmlDoc.CreateAttr("notes", _notes));
            technoNode.Attributes.Append(xmlDoc.CreateAttr("picture", this._pictureName));
            technoNode.Attributes.Append(xmlDoc.CreateAttr("inputRef", this.InputResourceRef));
            technoNode.Attributes.Append(xmlDoc.CreateAttr("outputRef", this._outputResource));
            technoNode.Attributes.Append(xmlDoc.CreateAttr("massTransfer", this._massTransfer));
            technoNode.Attributes.Append(xmlDoc.CreateAttr("basetech", this.BaseTechnology));
            technoNode.Attributes.Append(xmlDoc.CreateAttr("mostRecent", _mostRecentData));
            technoNode.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedOn, this.ModifiedOn.ToString(GData.Nfi)));
            technoNode.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedBy, this.ModifiedBy));

            foreach (KeyValuePair<int, EmissionsFactors> year in this.OrderBy(temp => temp.Key))
                technoNode.AppendChild(year.Value.ToXmlNode(xmlDoc, "year"));

            return technoNode;
        }

        public int CompareTo(object obj)
        {
            if (obj is TechnologyData)
            {
                TechnologyData value = (TechnologyData)obj;
                return this._name.CompareTo(value._name);
            }
            else

                throw new ArgumentException("Object is not a TechnologyEmissionsTSData");
        }

        /// <summary>
        /// Checks that the ID of the input resource exists in the database
        /// </summary>
        /// <returns>Meaningfull error message</returns>
        public bool CheckIntegrity(GData data, bool showIds, out string errorMessage)
        {
            errorMessage = "";
            bool canHandle = true;
            if (!data.ResourcesData.ContainsKey(this.InputResourceRef))
            {
                errorMessage += "Technology Input resource References does not exits/r/n";
                canHandle = false;
            }
            if (this._baseTechnologyData != -1 && !data.TechnologiesData.ContainsKey(this._baseTechnologyData))
            {
                errorMessage += "Technology used as a base for this technology does not exists in the database\r\n";
                canHandle = false;
            }
            foreach (EmissionsFactors ef in this.Values)
            {
                string efErrMsg = "";
                canHandle &= ef.CheckIntegrity(data, showIds, out efErrMsg);
                errorMessage += efErrMsg;             
            }

            if (!String.IsNullOrEmpty(errorMessage))
                errorMessage += "\r\n";

            return canHandle;
        }
        
        #endregion

        #region ITechnology

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IYearEmissionFactors> YearEmissionFactors
        {
            get
            {
                List<IYearEmissionFactors> emissionFactors = new List<IYearEmissionFactors>();
                foreach (EmissionsFactors emissionFactor in this.Values)
                {
                    emissionFactors.Add(emissionFactor as IYearEmissionFactors);
                }
                return emissionFactors;
            }
        }
        
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void FromXmlNode(IData data, XmlNode node)
        {
            this.FromXmlNode(data as GData, node, "");
        }
        #endregion

        #region IHaveMetadata Members

        public string ModifiedBy { get { return this._modifiedOn; } set { this._modifiedOn = value; } }

        public string ModifiedOn { get { return this._modifiedBy; } set { this._modifiedBy = value; } }

        #endregion

        /// <summary>
        /// Tests if all the EmissionFactors are defined as ratios to another technology or as RealEmissionFactors
        /// </summary>
       
    }
}
