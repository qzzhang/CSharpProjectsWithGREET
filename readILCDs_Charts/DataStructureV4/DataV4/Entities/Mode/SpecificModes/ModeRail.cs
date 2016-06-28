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
using Greet.LoggerLib;


namespace Greet.DataStructureV4.Entities
{


    [Serializable]
    /// <summary>
    /// mode type 4 can be a Train, a mode defined by the energy intensity from and the energy intensity to
    /// This mode does not contain a payload list like the others. 
    /// </summary>
    public class ModeRail : AMode, IGraphRepresented, IHaveAverageSpeed
    {
        #region attributes
        private ParameterTS _ei;
        private ParameterTS _averageSpeed;
        #endregion attributes

        #region constructors

        public ModeRail(GData data, XmlNode modeNode, string optionalParamPrefix)
            : base(data)
        {
            FromXmlNode(data, modeNode, optionalParamPrefix);
        }

        public ModeRail(GData data)
            : base(data)
        {
            Type = Modes.ModeType.Rail;
            _averageSpeed = new ParameterTS(data, "m/s", 0, 0, "rail_" + this.Id + "_AverageSpeed");

            _ei = new ParameterTS(data, "J/(kg m)", 0, 0, "rail_" + this.Id + "_ei");
        }

        #endregion constructors

        #region accessors

        [Browsable(false)]

        public ParameterTS AverageSpeed
        {
            get { return _averageSpeed; }
            set { _averageSpeed = value; }
        }
        
        public ParameterTS Ei
        {
            get { return _ei; }
            set { _ei = value; }
        }

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
                //ei to replace ei_to and ei_from
                status = "reading ei";
                this._ei = new ParameterTS(data, modeNode.SelectSingleNode("ei"), "railei_" + this.Id);
                this._averageSpeed = new ParameterTS(data, modeNode.SelectSingleNode("average_speed"), "rail_" + this.Id + "_spd");
                status = "redaing picture";
                if (modeNode.Attributes["picture"].NotNullNOrEmpty())
                    this.PictureName = modeNode.Attributes["picture"].Value;

                base.FromXmlNode(data, modeNode, "rail_" + this.Id);

            }
            catch (Exception e)
            {
                LogFile.Write("Error 82:" + modeNode.OwnerDocument.BaseURI + "\r\n" + modeNode.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
            } 
        }
        public override XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode t4 = base.ToXmlNode(xmlDoc);
            
            t4.Attributes.Append(xmlDoc.CreateAttr("type", (int)this.Type));
            t4.Attributes.Append(xmlDoc.CreateAttr("id", this.Id));
            t4.Attributes.Append(xmlDoc.CreateAttr("name", this.Name));
            t4.Attributes.Append(xmlDoc.CreateAttr("picture", this.PictureName));

            t4.AppendChild(this._ei.ToXmlNode(xmlDoc, "ei"));
            t4.AppendChild(this._averageSpeed.ToXmlNode(xmlDoc, "average_speed"));
            
            base.XMLFuelSharesAndER(t4, xmlDoc);
            return t4;
        }
        public override bool CheckIntegrity(GData data, bool showIds, out string errorMessage)     
        {

            errorMessage = "";
            foreach (ModeFuelShares MFS in this.FuelSharesData.Values)
            {
                foreach (ModeEnergySource PFS in MFS.ProcessFuels.Values)
                {
                    if (!data.ResourcesData.ContainsKey(PFS.ResourceReference.ResourceId))
                        errorMessage += " - Contains a fuel share (" + MFS.Name + ") that references a resource that does not exist\r\n";
                    else if (PFS.ResourceReference.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix && !data.MixesData.ContainsKey(PFS.ResourceReference.SourceMixOrPathwayID))
                        errorMessage += " - Contains a fuel share (" + MFS.Name + ") that references a " + "Pathway Mix" + " that does not exist\r\n";
                }
            }
            if (errorMessage != "")
                errorMessage = "Mode: " + this.Name + (showIds ? "(" + this.Id + ")" : "") + errorMessage;
            return true;
        }
        #endregion methods

        #region calculations



        #endregion calculations

      

    }

}