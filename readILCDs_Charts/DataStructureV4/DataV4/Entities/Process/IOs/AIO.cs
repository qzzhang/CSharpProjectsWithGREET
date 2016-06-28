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
using System.ComponentModel;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
   
    [Serializable]
    /// <summary>
    /// This is the base class for Input and Output classes
    /// </summary>
    public abstract class AIO : IHaveMetadata
    {
        #region constants
        const string xmlAttrModifiedOn = "modified-on";
        const string xmlAttrModifiedBy = "modified-by";
        #endregion

        #region attributes
        /// <summary>
        /// The resource ID for that IO
        /// </summary>
        public int resourceId;
        /// <summary>
        /// The design amount represents the amount that is read and saved to the data file.
        /// This is not the amount used during the calculations. The amount used during the calculations depends on what happened during the ConvertToGeneralIO methods
        /// </summary>
        private ParameterTS designAmount;
        /// <summary>
        /// Notes associated to that IO for user information
        /// </summary>
        private string notes = "";
        /// <summary>
        /// The IDs are used by the IOTrace matrix which links which inputs are transfered to which outputs
        /// It's a way of tracing which are the main inputs by using scalar values instead of a boolean
        /// </summary>
        public Guid id = Guid.NewGuid();
        /// <summary>
        /// Urban share for that input, we assume that for the IO object the urban share is a calculated value which is set during the ConvertToGeneralIO methods
        /// therefore there are not Parameters but LightValues
        /// </summary>
        public LightValue urbanShare = new LightValue(0.0, "%");
        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedOn = "";
        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string modifiedBy = "";
        #endregion attributes

        #region constructors

        protected AIO()
        {
        }

        protected AIO(int resourceId, ParameterTS designAmount)
        {
            this.resourceId = resourceId;
            this.designAmount = designAmount;
        }

        protected AIO(GData data, XmlNode node, string optionalParamPrefix)
        {
            //required attributes
            this.resourceId = Convert.ToInt32(node.Attributes["ref"].Value);

            //optional attributes
            if (node.SelectSingleNode("amount") != null)
                this.designAmount = new ParameterTS(data, node.SelectSingleNode("amount"), optionalParamPrefix + "_res_"+resourceId + "_amt");
            else if (node.Attributes["amount"] != null)
            {
                this.designAmount = new ParameterTS();
                this.designAmount.Add(0, data.ParametersData.CreateRegisteredParameter(node.Attributes["amount"], optionalParamPrefix + "_res_" + this.resourceId + "_amount"));
            }
            
            if (node.Attributes["notes"] != null)
                this.notes = node.Attributes["notes"].Value;
            if (node.Attributes["id"] != null)
                Guid.TryParse(node.Attributes["id"].Value, out this.id);
            if (node.Attributes[xmlAttrModifiedOn] != null)
                this.ModifiedOn = node.Attributes[xmlAttrModifiedOn].Value;
            if (node.Attributes[xmlAttrModifiedBy] != null)
                this.ModifiedBy = node.Attributes[xmlAttrModifiedBy].Value;
        }

        #endregion constructors

        #region methods

        internal void ToXmlNode(XmlNode input, XmlDocument doc)
        {
            input.Attributes.Append(doc.CreateAttr("ref", this.resourceId));

            input.Attributes.Append(doc.CreateAttr("id", this.id));

            if (this.DesignAmount != null)
                input.AppendChild(this.DesignAmount.ToXmlNode(doc, "amount"));

            input.Attributes.Append(doc.CreateAttr("notes", this.notes));
            input.Attributes.Append(doc.CreateAttr(xmlAttrModifiedOn, this.ModifiedOn.ToString(GData.Nfi)));
            input.Attributes.Append(doc.CreateAttr(xmlAttrModifiedBy, this.ModifiedBy));
        }

        #endregion

        #region accessors

        [Browsable(false)]
        public int ResourceId
        {
            get { return this.resourceId; }
            set { this.resourceId = value; }
        }

        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }

        public string ModifiedBy { get { return this.modifiedOn; } set { this.modifiedOn = value; } }

        public string ModifiedOn { get { return this.modifiedBy; } set { this.modifiedBy = value; } }

        public ParameterTS DesignAmount
        {
            get { return designAmount; }
            set { designAmount = value; }
        }
        #endregion accessors

    }//end IO

}
