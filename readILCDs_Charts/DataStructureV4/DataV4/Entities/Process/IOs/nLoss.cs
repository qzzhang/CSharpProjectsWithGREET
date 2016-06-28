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
using System.Xml;
using Greet.ConvenienceLib;


namespace Greet.DataStructureV4.Entities
{

    /// <summary>
    /// The new loss class implements the specification 001-02
    /// This is the unique model to be used for the losses instead of using all the different cases that were done in the excel version
    /// </summary>
    [Serializable]
    public class nLoss
    {

        #region attributes
        /// <summary>
        /// This parameter is valid for any of the new losses, we always set the rate as a certain ratio of the output per unit of time per unit of distance
        /// Then the duration and distance are calculated if they are appropriate and given to the loss. An example of that would be a transportation step where 
        /// the user wants to give the rate as function of the distance or the time. For process output, this will be possible only with time
        /// </summary>
        private Parameter rate;
        /// <summary>
        /// Defines if the distance or speed or none of them needs to be set for calculating this loss
        /// This will only be done during the conversion of the transportation steps to general in out
        /// </summary>
        private Greet.DataStructureV4.Interfaces.Enumerators.LossDependency dependency = Greet.DataStructureV4.Interfaces.Enumerators.LossDependency.none;
        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a loss from an XML node in the database
        /// </summary>
        /// <param name="node"></param>
        public nLoss(GData data, XmlNode node, string optionalParamPrefix)
        {
            this.rate = data.ParametersData.CreateRegisteredParameter(node.Attributes["rate"], optionalParamPrefix + "_rate");
            if (node.Attributes["dependency"] != null)
                this.Dependency = (Greet.DataStructureV4.Interfaces.Enumerators.LossDependency)Enum.Parse(typeof(Greet.DataStructureV4.Interfaces.Enumerators.LossDependency), node.Attributes["dependency"].Value, true);
        }
        /// <summary>
        /// Creates a new loss with 
        /// Rate = 0
        /// Distance = 1
        /// Duration = 1
        /// RecoveryRate = 0
        /// </summary>
        public nLoss(Parameters parameters)
        {
            this.rate = parameters.CreateRegisteredParameter("%", 0);
        }

        #endregion

        #region methods
        /// <summary>
        /// Converts the information of the loss into an XML node to be stored in the database
        /// </summary>
        /// <param name="xmlDoc">The XmlDocument reference in which the node is going to be added (for culture info only)</param>
        /// <returns>The representation of that loss as an XmlNode</returns>
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateNode("nloss", xmlDoc.CreateAttr("rate", this.rate), xmlDoc.CreateAttr("dependency",this.Dependency));
            return node;
        }

       
        #endregion

        #region accessors
        /// <summary>
        /// The rate at which the loss is leaked or boilled off, this is a parameter direclty set by the user from the GUI
        /// </summary>
        public Parameter Rate
        {
            get { return rate; }
            set { rate = value; }
        }
        /// <summary>
        /// Access the dependency value for this loss, defines if the conversion should set the distance and time while converting
        /// to general in out
        /// </summary>
        public Greet.DataStructureV4.Interfaces.Enumerators.LossDependency Dependency
        {
            get { return dependency; }
            set { dependency = value; }
        }
        
        #endregion

    }
}
