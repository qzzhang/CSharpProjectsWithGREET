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
using Greet.DataStructureV4.ResultsStorage;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public abstract class AProcess : IGraphRepresented, IProcess, IHaveMetadata, IGREETEntity
    {
        #region constants
        const string xmlAttrModifiedOn = "modified-on";
        const string xmlAttrModifiedBy = "modified-by";
        #endregion

        #region commonAttributes
        //begin process-common-attributes
        /// <summary>
        /// Picture Name
        /// </summary>
        private string _pictureName = Constants.EmptyPicture;
        /// <summary>
        /// Do not use this as a public accessor, when the ID is changed, all the references to the parent process id for the transportation steps needs to be updated
        /// This is going to be done in the accessors of the transportation process Id
        /// </summary>
        protected int _id;
        /// <summary>
        /// Process name
        /// //latex Name
        /// </summary>
        protected string _name;
        /// <summary>
        /// Process Notes
        /// </summary>
        protected string _notes;
        /// <summary>
        /// The transfert matrix is used to know which inputs are propagated into which outputs.
        /// It is a way to know the main input but also to associate an input with the co products, mostly used to trace biogenic carbon when burned into technologies
        /// </summary>
        private Dictionary<Guid, Dictionary<Guid, double>> _carbonTransMatrix = null;
        /// <summary>
        /// List of all inputs of the process. 
        /// In stationary process. It contains both the indivdual inputs and group inputs.
        /// In Transportation process. It contains all the steps which are converted as inputs.
        /// </summary>
        private List<Input> _converted_inputs_for_aprocess_calculations;
        /// <summary>
        /// The Main putput of the process.
        /// </summary>
        private MainOutput _mainOutput;
        /// <summary>
        /// List of all the Co-Products.
        /// </summary>
        private CoProductsElements _coProducts;
        /// <summary>
        /// Other Emissions of the process
        /// </summary>
        private ProcessStaticEmissionList _otherStaticEmissions;
        /// <summary>
        /// Static Emissions are calculated on the fly but does not need to be save in XML Data files.
        /// </summary>
        private EmissionAmounts _otherStaticEmissionsCalculatedAndNotSaved;
        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string _modifiedOn = "";
        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string _modifiedBy = "";
        /// <summary>
        /// Urban share, used by all inputs and outputs losses for stationary processes
        /// Use only by output losses for transportation processes
        /// </summary>
        private Parameter _urbanShare;

        public bool Discarded { get; set; }
        public string DiscardedReason { get; set; }
        public DateTime DiscardedOn { get; set; }
        public string DiscarededBy { get; set; }

        #endregion commonAttributes

        #region abstractFields

        /// <summary>
        /// Method to save the process to Xml.
        /// </summary>
        /// <param name="processDoc">XML Document</param>
        /// <returns>Xml Node with the process information</returns>
        public abstract XmlNode ToXmlNode(XmlDocument processDoc);
        /// <summary>
        /// Checks the integrity of the process 
        /// </summary>
        /// <param name="data">Data for checking references</param>
        /// <param name="showIds">True: To return detailed information about the errors</param>
        /// <param name="fixFixableIssues">If set to true some issues will be fixed automatically</param>
        /// <param name="errorMessage">Error message filled up with human readable issues and fixes</param>
        /// <returns></returns>
        public abstract bool CheckSpecificIntegrity(GData data, bool showIds, bool fixFixableIssues, out string errorMessage);

        #endregion abstractFields

        #region constructors

        /// <summary>
        /// General Constructors
        /// </summary>
        protected AProcess()
        {
            _id = 0;
            _name = "";
            _notes = "";
            this._coProducts = new CoProductsElements();
            this._carbonTransMatrix = new Dictionary<Guid, Dictionary<Guid, double>>();
        }

        #endregion construcotrs

        #region methods

        /// <summary>
        /// Reads the content of the node to populate the members of this AProcess instance
        /// </summary>
        /// <param name="node"></param>
        public virtual void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            if (node.Attributes["discarded"] != null)
            {
                Discarded = Convert.ToBoolean(node.Attributes["discarded"].Value);
                DiscardedOn = Convert.ToDateTime(node.Attributes["discardedOn"].Value, GData.Nfi);
                DiscarededBy = node.Attributes["discardedBy"].Value;
                DiscardedReason = node.Attributes["discardedReason"].Value;
            }

            //common attributes ( they are read first so we have the id of that process while debugging )
            this._id = Convert.ToInt32(node.Attributes["id"].Value);
            this._name = node.Attributes["name"].Value;
            if (node.Attributes["notes"] != null)
                this._notes = node.Attributes["notes"].Value;
            if (node.Attributes["picture"].NotNullNOrEmpty())
                this._pictureName = node.Attributes["picture"].Value;
            if (node.SelectSingleNode("io-carbon-map") != null)
                this.CarbonTransMatrix = TransMatrixFromXmlNode(node.SelectSingleNode("io-carbon-map"));
            if (node.Attributes[xmlAttrModifiedOn] != null)
                this.ModifiedOn = node.Attributes[xmlAttrModifiedOn].Value;
            if (node.Attributes[xmlAttrModifiedBy] != null)
                this.ModifiedBy = node.Attributes[xmlAttrModifiedBy].Value;
            if (node.Attributes["urban_share"] != null)
                _urbanShare = data.ParametersData.CreateRegisteredParameter(node.Attributes["urban_share"], "sproc_" + this.Id + "_urban");
            else
                _urbanShare = data.ParametersData.CreateRegisteredParameter("%", 0, 0);

            //other emissions 
            XmlNode other_emissions = node.SelectSingleNode("other_emissions");
            if (other_emissions != null)
                this.OtherStaticEmissions = new ProcessStaticEmissionList(data, other_emissions, false, "sproc_" + this.Id + "_static");
        }

        /// <summary>
        /// This function checks wheather both main input and main output were assigned. It needs to be done after ConvertToGeneralInOut is called.
        /// Well main input can be null for a few processes so we don't need to check it (david)
        /// </summary>
        /// <param name="showMoreInfo"></param>
        /// <param name="showIds"></param>
        /// <param name="fixFixableIssues">If set to true, some issues will be automatically fixed</param>
        /// <param name="msg">String used to add various messages if errors are detected</param>
        /// <returns>True if this process can be handled by the calculations false otherwise</returns>
        public bool CheckIntegrity(GData data, bool showIds, bool fixFixableIssues, out string msg)
        {
            bool cabBeHandledByGreet = true;
            msg = "";
            if (this.MainOutput == null || this.MainOutput.resourceId == 0 )
            {
                cabBeHandledByGreet &= false;
                msg += " - Missing main output" + Environment.NewLine;
            }
            else if (!data.ResourcesData.ContainsKey(this._mainOutput.resourceId))
            {
                cabBeHandledByGreet &= false;
                msg += " - Invalid Main output, No resource found with Id " + MainOutput.ResourceId + Environment.NewLine;

                //06/10/2014 CAB: Moved foreach loop inside if statement. mainOutput was null causing GREET to crash when user tried to save empty process.
                foreach (Parameter param in this._mainOutput.DesignAmount.Values)
                {
                    if (param.ValueInDefaultUnit <= 0.0)
                    {
                        cabBeHandledByGreet &= false;
                        msg += " - Invalid Main output amount, amount should be greater than 0." + Environment.NewLine;
                    }
                }
            }
            
            string temp ="";
            cabBeHandledByGreet &= this.CheckSpecificIntegrity(data, showIds, fixFixableIssues, out temp);
            if (msg.Length > 0)
                msg += "\n";
            msg += temp;

            //check that the same resource is not defined more than once as a main output or coproduct
            HashSet<int> hashSet = new HashSet<int>();
            foreach (CoProduct coProduct in this.CoProducts)
            {
                if (!data.ResourcesData.ContainsKey(coProduct.resourceId))
                {
                    msg += " - Invalid coproduct, No resource found with Id: " + coProduct.ResourceId + Environment.NewLine;
                    continue;
                }
                else
                {
                    if (!hashSet.Add(coProduct.ResourceId))
                    {
                        msg += " - " + data.ResourcesData[coProduct.ResourceId].Name + (showIds ? "(" + coProduct.ResourceId + ")" : "") + " cannot be defined as coproduct more than once" + Environment.NewLine;
                        continue;
                    }
                }
                string outputMsg;
                coProduct.CheckSpecificIntegrity(data, false, true, out outputMsg);
                if(!String.IsNullOrEmpty(outputMsg))
                    msg += outputMsg + Environment.NewLine;
            }
            if (this._mainOutput != null && !hashSet.Add(this.MainOutput.ResourceId))
            {
                msg += " - Resource " + data.ResourcesData[MainOutput.ResourceId].Name + (showIds ? "(" + MainOutput.ResourceId + ")" : "") + " cannot be defined as coproduct and Main output at the same time" + Environment.NewLine;
            }

            return cabBeHandledByGreet;
        }

        /// <summary>
        /// Override method to return the Name of the process.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._name;
        }
       
        /// <summary>
        /// Adds the common process attributes to the XML node.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="doc"></param>
        internal void ToXmlNodeCommon(XmlNode process, XmlDocument doc)
        {
            //common attributes ( they are read first so we have the id of that process while debugging )
            if (this.Discarded)
            {
                process.Attributes.Append(doc.CreateAttr("discarded", Discarded));
                process.Attributes.Append(doc.CreateAttr("discardedReason", DiscardedReason));
                process.Attributes.Append(doc.CreateAttr("discardedOn", DiscardedOn));
                process.Attributes.Append(doc.CreateAttr("discardedBy", DiscarededBy));
            }

            process.Attributes.Append(doc.CreateAttr("id", this._id));
            process.Attributes.Append(doc.CreateAttr("name", this._name));
            process.Attributes.Append(doc.CreateAttr("notes", this._notes));
            process.Attributes.Append(doc.CreateAttr("picture", this._pictureName));
            process.Attributes.Append(doc.CreateAttr(xmlAttrModifiedOn, this.ModifiedOn.ToString(GData.Nfi)));
            process.Attributes.Append(doc.CreateAttr(xmlAttrModifiedBy, this.ModifiedBy));
            process.Attributes.Append(this.UrbanShare.ToXmlAttribute(doc, "urban_share"));

            //coproducts
            if (this.CoProducts.Count > 0)
                process.AppendChild(this.CoProducts.toXmlNode(doc));

            process.AppendChild(this.TransMatrixToXML(doc));

            if (OtherStaticEmissions != null && OtherStaticEmissions.Count > 0)
                process.AppendChild(OtherStaticEmissions.toXmlNode(doc, "other_emissions"));
        }

        /// <summary>
        /// Returns an XmlNode containing the TransferMatrix elements for a transportation process.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument for XmlContext</param>
        /// <returns>XmlNode representation of the TransferMatrix</returns>
        private XmlNode TransMatrixToXML(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateNode("io-carbon-map");

            foreach (KeyValuePair<Guid, Dictionary<Guid, double>> outputRow in this.CarbonTransMatrix)
            {
                XmlNode outputNode = xmlDoc.CreateNode("output", xmlDoc.CreateAttr("id", outputRow.Key));
                node.AppendChild(outputNode);
                foreach (KeyValuePair<Guid, double> inputCol in outputRow.Value)
                {
                    XmlNode inputNode = xmlDoc.CreateNode("input", xmlDoc.CreateAttr("id", inputCol.Key), xmlDoc.CreateAttr("ratio", inputCol.Value));
                    outputNode.AppendChild(inputNode);
                }
            }

            return node;
        }

        /// <summary>
        /// Creates the IO Carbon Transfer Matrix from an XML node assuming it respects the format used when 
        /// saving with the method TransMatrixToXML in the AProcessClass
        /// </summary>
        /// <param name="node">XmlNode to be converted to dictionaries</param>
        /// <returns>Instance of dictionaries from the XmlNode</returns>
        private Dictionary<Guid, Dictionary<Guid, double>> TransMatrixFromXmlNode(XmlNode node)
        {
            Dictionary<Guid, Dictionary<Guid, double>> transferMatrix = new Dictionary<Guid,Dictionary<Guid,double>>();
            
            foreach(XmlNode rowNode in node.SelectNodes("output"))
            {
                Guid outputGuid = new Guid(rowNode.Attributes["id"].Value);
                transferMatrix.Add(outputGuid, new Dictionary<Guid,double>());
                foreach(XmlNode colNode in rowNode.SelectNodes("input"))
                    transferMatrix[outputGuid].Add(new Guid(colNode.Attributes["id"].Value), Convert.ToDouble(colNode.Attributes["ratio"].Value, Constants.USCI));
            }


            return transferMatrix;
        }
        
       
        /// <summary>
        /// Checks if the resource can come from a well and returns
        /// </summary>
        /// <param name="resourceId">resource Id</param>
        /// <returns>True: if resource is coming from Well else False</returns>
        public bool HasInputFromWellFor(int resourceId)
        {
            foreach (var inp in this.FlattenInputList)
                if (inp.ResourceId == resourceId && inp.SourceType == Enumerators.SourceType.Well)
                    return true;
            return false;
        }

        public void FromXmlNode(IData data, XmlNode node)
        {
            this.FromXmlNode(data as GData, node, "");
        }
        #endregion methods

        #region accessors

        #region generalParameters

        /// <summary>
        /// Picture Name
        /// </summary>
        [Browsable(true)]
        public string PictureName
        {
            get { return this._pictureName; }
            set { this._pictureName = value; }
        }

        /// <summary>
        /// Unique Process ID, this accessor is abstract because changing the ID of a stationary process needs some processing done to the children members
        /// of the process. This processing is done in the accessors of the clas
        /// </summary>
        [Browsable(true), CategoryAttribute("General Parameters"), ReadOnly(true), Obfuscation(Feature = "renaming", Exclude = true)]
        public abstract int Id { get; set; }

        /// <summary>
        /// Name of the process
        /// </summary>
        [Browsable(true), CategoryAttribute("General Parameters"), ReadOnly(true), Obfuscation(Feature = "renaming", Exclude = true)]
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// Returns the type of the Process. Stationary or Transportation.
        /// </summary>
        [Browsable(true), CategoryAttribute("General Parameters"), ReadOnly(true), Obfuscation(Feature = "renaming", Exclude = true)]
        public string Type
        {
            get
            {
                if (this is StationaryProcess)
                    return "Stationary";
                else if (this is TransportationProcess)
                    return "Transportation";
                else
                    return "";
            }
        }


        /// <summary>
        /// <para>The transfert matrix is used to know which inputs are propagated into which outputs.</para>
        /// <para>It is a way to know the main input but also to associate an input with the co products, mostly used to trace biogenic carbon when burned into technologies</para>
        /// <para>Used by indexing in this specific order : [OutputId][InputId]</para>
        /// </summary>
        public Dictionary<Guid, Dictionary<Guid, double>> CarbonTransMatrix
        {
            get { return _carbonTransMatrix; }
            set { _carbonTransMatrix = value; }
        }
      
        /// <summary>
        /// Main Output of the Process
        /// </summary>
        [BrowsableAttribute(false)]
        public MainOutput MainOutput 
        {
            get
            {
                return this._mainOutput;
            }
            set
            {
                this._mainOutput = value;
            }
        }
       
        /// <summary>
        /// List of all inputs of the process. 
        /// In stationary process. It contains both the indivdual inputs and group inputs.
        /// In Transportation process. It contains all the steps which are converted as inputs.
        /// </summary>
        public List<Input> Converted_inputs_for_aprocess_calculations
        {
            get { return _converted_inputs_for_aprocess_calculations; }
            set { _converted_inputs_for_aprocess_calculations = value; }
        }

        /// <summary>
        /// List of all the Co-Products.
        /// </summary>
        public CoProductsElements CoProducts
        {
            get { return _coProducts; }
            set { _coProducts = value; }
        }

        /// <summary>
        /// Other Emissions of the process
        /// </summary>
        public ProcessStaticEmissionList OtherStaticEmissions
        {
            get { return _otherStaticEmissions; }
            set { _otherStaticEmissions = value; }
        }

        /// <summary>
        /// Static Emissions are calculated on the fly but does not need to be save in XML Data files.
        /// </summary>
        public EmissionAmounts OtherStaticEmissionsCalculatedAndNotSaved
        {
            get { return _otherStaticEmissionsCalculatedAndNotSaved; }
            set { _otherStaticEmissionsCalculatedAndNotSaved = value; }
        }

        public Parameter UrbanShare
        {
            get { return _urbanShare; }
            set { _urbanShare = value; }
        }
        #endregion generalParameters

        #region For ICanHaveFunctionalUnitPreference

        /// <summary>
        /// Returns the main output material id for this process
        /// </summary>
        public int MainOutputResourceID
        {
            get { return this.MainOutput.ResourceId; }
        }

        #endregion

        #region For IProcess

        public List<int> OutputEmissionsIds
        {
            get
            {
                List<int> emissionIds = new List<int>();
                if(this.OtherStaticEmissions!=null)
                foreach (ProcessStaticEmissionItem eg in this.OtherStaticEmissions.StaticEmissions)
                    emissionIds.Add(eg.GasId);
                return emissionIds;
            }
        }

        /// <summary>
        /// This methods flattens all the available inputs from a process without doing any calculations or operations on the process
        /// Warning for transportation processes, it only returns the MainOutput amount in a new MainInputObject as the others are dependent on the Modes and Transportation Steps and Process Fuels used for each step
        /// </summary>
        public abstract List<IInput> FlattenInputList {get;}
       

        /// <summary>
        /// This should be renamed CoProductsResourcesIds
        /// </summary>
        public List<int> CoProductIds
        {
            get
            {
                List<int> coProductIdList = new List<int>();
                foreach (CoProduct cp in this.CoProducts)
                    coProductIdList.Add(cp.ResourceId);
                return coProductIdList;
            }
        }

        
        #endregion
        #endregion accessors

        #region IHaveMetadata Members

        /// <summary>
        /// Process Notes
        /// </summary>
        public string Notes
        {
            get { return this._notes; }
            set { this._notes = value; }
        }

        public string ModifiedBy { get { return this._modifiedOn; } set { this._modifiedOn = value; } }

        public string ModifiedOn { get { return this._modifiedBy; } set { this._modifiedBy = value; } }

        #endregion

        /// <summary>
        /// Returns as a list, all of allocated outputs (main output and allocated co-products)
        /// </summary>
        public List<IIO> FlattenAllocatedOutputList
        {
            get 
            {
                List<IIO> returned = new List<IIO>();
                if (this._mainOutput != null)
                    returned.Add(this._mainOutput as IIO);
                foreach (CoProduct copr in this.CoProducts.Where(c => c.method == CoProductsElements.TreatmentMethod.allocation))
                    returned.Add(copr as IIO);
                return returned;
            }
        }

    } //end of AProcess
}