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
using System.Xml;
using Greet.ConvenienceLib;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{

    [Serializable]
    /// <summary>
    /// mode type 5 can be a Magic Move, a mode defined by nothing, it's a kind of bypass mode.
    /// </summary>
    public class ModeConnector : AMode
    {

        #region constructors

        public ModeConnector(GData data,XmlNode modeNode, string optionalParamPrefix)
            : base(data)
        {
            FromXmlNode(data,modeNode, optionalParamPrefix);
        }

        public ModeConnector(GData data)
            :base(data)
        {
            Type = Modes.ModeType.MagicMove;
        }

        #endregion constructors

        #region accessors

        public override bool CanBackHaul
        {
            get { return false; }
        }

        #endregion accessors

        #region methods

        internal override void FromXmlNode(GData data, XmlNode modeNode, string optionalParamPrefix)
        {
            string status = "";
            try
            {
                status = "reading id";
                this.Id = Convert.ToInt32(modeNode.Attributes["id"].Value);
                status = "reading name";
                this.Name = modeNode.Attributes["name"].Value;
                status = "reading type";
                this.Type = (Modes.ModeType)Enum.ToObject(typeof(Modes.ModeType), Convert.ToInt32(modeNode.Attributes["type"].Value));
                status = "reading picture";
                if (modeNode.Attributes["picture"].NotNullNOrEmpty())
                    this.PictureName = modeNode.Attributes["picture"].Value;

                base.FromXmlNode(data, modeNode, "magicmove_" + this.Id);

            }
            catch (Exception e)
            {
                LogFile.Write("Error 81:" + modeNode.OwnerDocument.BaseURI + "\r\n" + modeNode.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
            }
        }

        public override bool CheckIntegrity(GData data, bool showIds, out string errorMessage)
        {
            errorMessage = "";

            return true;
        }

        public override XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode t5 = base.ToXmlNode(xmlDoc);
            t5.Attributes.Append(xmlDoc.CreateAttr("type", (int)this.Type));
            t5.Attributes.Append(xmlDoc.CreateAttr("id", this.Id));
            t5.Attributes.Append(xmlDoc.CreateAttr("name", this.Name));
            t5.Attributes.Append(xmlDoc.CreateAttr("picture", this.PictureName));
            base.XMLFuelSharesAndER(t5, xmlDoc);
            return t5;
        }

        #endregion methods

    }

}