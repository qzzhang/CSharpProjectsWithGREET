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
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class represents an entitiy saved into the database for representing a technology attached to a input of a process<br/>
    /// It is not intended to be used for "calculated" technologies which are created while converting the process to a canonical form<br/>
    /// as doing so will create unnecessary registered paramters for the Share memember<br/>
    /// </summary>
    [Serializable]
    public class EntityTechnologyRef : TechnologyRef
    {
        #region attributes

        private ParameterTS share;

        #endregion attributes

        #region constructors

        public EntityTechnologyRef()
        {
            
        }

        public EntityTechnologyRef(GData data, int technologyId, double share)
            : this()
        {
            this.share = new ParameterTS(data, "%", 0, share, "techref_" + technologyId + "_share");
            this.technologyRef = technologyId;
        }

        public EntityTechnologyRef(GData data, XmlNode technoNode, string optionalParamPrefix)
            : this()
        {
            string status = "";

            try
            {
                status = "reading reference";
                this.technologyRef = Convert.ToInt32(technoNode.Attributes["ref"].Value);
                status = "reading share";
                this.share = new ParameterTS(data, technoNode.SelectSingleNode("share"), optionalParamPrefix + this.technologyRef + "_share");

                if (technoNode.Attributes["account_in_balance"] != null)
                    this.accountInBalance = Convert.ToBoolean(technoNode.Attributes["account_in_balance"].Value);
            }
            catch (Exception e)
            {
                LogFile.Write("Error 18:" + technoNode.OwnerDocument.BaseURI + "\r\n" + technoNode.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
                throw e;
            }
        }

        #endregion constructors

        #region methods

        internal XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode techno = doc.CreateNode("technology", doc.CreateAttr("ref", technologyRef), doc.CreateAttr("account_in_balance", this.accountInBalance));
            techno.AppendChild(share.ToXmlNode(doc, "share"));
            return techno;
        }
        #endregion methods

        public override double ShareValueInDefaultUnit
        {
            get { return Share.CurrentValue.ValueInDefaultUnit; }
        }
        
        public ParameterTS Share
        {
            get { return share; }
            set { share = value; }
        }

        public override string ToString()
        {
            return "Technology: " + this.technologyName;
        }
    }
}
