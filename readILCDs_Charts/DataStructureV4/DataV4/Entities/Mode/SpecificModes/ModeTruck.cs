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
using System.Reflection;
using Greet.ConvenienceLib;
using Greet.LoggerLib;


namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    /// <summary>
    /// mode type 2 can be a HD or MD truck, a mode defined by a payload and a fuel economy
    /// </summary>
    public class ModeTruck : AMode, IGraphRepresented, INeedPayload
    {
        #region attributes

        private Dictionary<int, MaterialTransportedPayload> payload;

        private ParameterTS fuelEconomyTo;
        private ParameterTS fuelEconomyFrom;

        #endregion attributes

        #region constructors

        public ModeTruck(GData data, XmlNode node, string optionalParamPrefix)
            : base(data)
        {
            FromXmlNode(data, node, optionalParamPrefix);
        }

        public ModeTruck(GData data)
            : base(data)
        {
            Type = Modes.ModeType.Truck;
            fuelEconomyTo = new ParameterTS(data, "m/L", 0, 0, "truck_" + this.Id + "_fe_to");
            fuelEconomyFrom = new ParameterTS(data, "m/L", 0, 0, "truck_" + this.Id + "_fe_from");
            payload = new Dictionary<int, MaterialTransportedPayload>();
        }

        #endregion constructors

        #region accessors
        [Browsable(false)]
        public Dictionary<int, MaterialTransportedPayload> Payload
        {
            get { return payload; }
            set { payload = value; }
        }
        [Browsable(false)]
        public ParameterTS FuelEconomyTo
        {
            get { return fuelEconomyTo; }
            set { fuelEconomyTo = value; }
        }
        [Browsable(false)]
        public ParameterTS FuelEconomyFrom
        {
            get { return fuelEconomyFrom; }
            set { fuelEconomyFrom = value; }
        }

        [Browsable(true), DisplayName("Fuel Economy From"), CategoryAttribute("Fuel Economy")]
        public string FuelEconomyNiceFrom
        {
            get
            {
                return "Obsolete Replaced with DataStructureV4 entities";
                //return this.fuelEconomyFrom.NiceValueInOverridePrefixedWithAttribute; 
            }
        }

        [Browsable(true), DisplayName("Fuel Economy To"), CategoryAttribute("Fuel Economy")]
        public string FuelEconomyNiceTo
        {
            get
            {
                return "Obsolete Replaced with DataStructureV4 entities";
                //return this.fuelEconomyTo.NiceValueInOverridePrefixedWithAttribute; 
            }
        }

        public override bool CanBackHaul
        {
            get { return true; }
        }

        #endregion accessors

        #region methods
        internal override void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            string status = "";
            try
            {
                status = "reading id";
                this.Id = Convert.ToInt32(node.Attributes["id"].Value);
                status = "reading name";
                this.Name = node.Attributes["name"].Value;
                status = "reading type";
                this.Type = (Modes.ModeType)Enum.ToObject(typeof(Modes.ModeType), Convert.ToInt32(node.Attributes["type"].Value));
                status = "reading fuel economy from";
                this.fuelEconomyFrom = new ParameterTS(data, node.SelectSingleNode("fuel_economy_from"), "truck_" + this.Id + "_fe_from");
                status = "reading fuel economy to";
                this.fuelEconomyTo = new ParameterTS(data, node.SelectSingleNode("fuel_economy_to"), "truck_" + this.Id + "_fe_to");

                base.FromXmlNode(data, node, "truck_" + this.Id);

                status = "reading picture";
                if (node.Attributes["picture"].NotNullNOrEmpty())
                    this.PictureName = node.Attributes["picture"].Value;

                payload = new Dictionary<int, MaterialTransportedPayload>();
                XmlNodeList payloads = node.SelectNodes("payload/material_transported");
                foreach (XmlNode payloadNode in payloads)
                {
                    try
                    {
                        MaterialTransportedPayload pay = new MaterialTransportedPayload(data, payloadNode, "truck_" + this.Id + "_payload");
                        this.payload.Add(pay.Reference, pay);
                    }
                    catch (Exception e)
                    {
                        LogFile.Write
                          ("Error 103: " + e.Message);
                    }
                }

            }
            catch (Exception e)
            {
                LogFile.Write
                       ("Error 102: " + node.OwnerDocument.BaseURI + Environment.NewLine +
                       node.OuterXml + Environment.NewLine +
                       e.Message + Environment.NewLine +
                       status + Environment.NewLine);
            }
        }
        /// <summary>
        /// Returns a string containing all the errors if any are detected
        /// </summary>
        /// <returns></returns>
        public override bool CheckIntegrity(GData data, bool showIds, out string errorMessage)
        {
            errorMessage = "";
            //turns out baseline fuel is set to null if the ID does not exist... checking needs to be done elsewhere
            foreach (MaterialTransportedPayload MTP in this.Payload.Values)
            {
                if (!data.ResourcesData.Keys.Contains(MTP.Reference) && !data.ResourcesData.Groups.Keys.Contains(MTP.Reference))
                    errorMessage += " - Contains a payload reference resource (" + MTP.Reference + ") that does not exist" + Environment.NewLine;
                if (MTP.Payload == null || MTP.Payload.ValueInDefaultUnit == 0)
                    errorMessage += " - Contains a payload of value 0" + Environment.NewLine;
            }
            foreach (ModeFuelShares MFS in this.FuelSharesData.Values)
            {
                foreach (ModeEnergySource PFS in MFS.ProcessFuels.Values)
                {
                    if (!data.ResourcesData.ContainsKey(PFS.ResourceReference.ResourceId))
                        errorMessage += " - Contains a fuel share (" + MFS.Name + ") that references a resource that does not exist" + Environment.NewLine;
                    else if (PFS.ResourceReference.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix && !data.MixesData.ContainsKey(PFS.ResourceReference.SourceMixOrPathwayID))
                        errorMessage += " - Contains a fuel share (" + MFS.Name + ") that references a " + "Pathway Mix" + " that does not exist" + Environment.NewLine;
                }
            }
            if (errorMessage != "")
                errorMessage = "Mode: " + this.Name + (showIds ? "(" + this.Id + ")" : "") + errorMessage;
            return true;
        }

        public override XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode t2 = base.ToXmlNode(xmlDoc);
            t2.Attributes.Append(xmlDoc.CreateAttr("type", (int)this.Type));
            t2.Attributes.Append(xmlDoc.CreateAttr("id", this.Id));
            t2.Attributes.Append(xmlDoc.CreateAttr("name", this.Name));
            t2.Attributes.Append(xmlDoc.CreateAttr("picture", this.PictureName));
            t2.AppendChild(this.fuelEconomyTo.ToXmlNode(xmlDoc, "fuel_economy_to"));
            t2.AppendChild(this.fuelEconomyFrom.ToXmlNode(xmlDoc, "fuel_economy_from"));
            base.XMLFuelSharesAndER(t2, xmlDoc);
            XmlNode payload = xmlDoc.CreateNode("payload");
            foreach (KeyValuePair<int, MaterialTransportedPayload> payl in this.payload)
                payload.AppendChild(payl.Value.ToXmlNode(xmlDoc));
            t2.AppendChild(payload);

            return t2;
        }

        /// <summary>
        /// convert emission vactors for the Trucks from g/mi (as it is given in DB) to g/MMBtu
        /// </summary>
        public void Update()
        {
        }

        #endregion methods

    }

}