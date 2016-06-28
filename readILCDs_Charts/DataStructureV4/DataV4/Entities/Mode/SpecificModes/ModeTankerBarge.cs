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
    /// mode type 1 can be a Ocean Tanker or a Barge, something defined by an average speed, a load factor from, load factor to, a payload  an energy intensity ratio
    /// </summary>
    public class ModeTankerBarge : AMode, IGraphRepresented, INeedPayload, IHaveAverageSpeed
    {
        #region attributes

        private ParameterTS _averageSpeed;
        private ParameterTS _loadFactorFrom;
        private ParameterTS _loadFactorTo;
        private Dictionary<int, MaterialTransportedPayload> _payload;
        private ParameterTS _brakeSpecificFuelConsumption;
        private ParameterTS _hpPayloadFactor;
        private ParameterTS _typicalHpRequirement;
        private ParameterTS _loadFactorAdjustmentBsfc;

        #endregion attributes

        #region constructors

        public ModeTankerBarge(GData data, XmlNode node, string optionalParamPrefix)
            : base(data)
        {
            FromXmlNode(data, node, optionalParamPrefix);
        }
        public ModeTankerBarge(GData data)
            : base(data)
        {
            Type = Modes.ModeType.TankerBarge;
            _payload = new Dictionary<int, MaterialTransportedPayload>();
            _averageSpeed = new ParameterTS(data, "m/s", 0, 0, "vessel_" + this.Id + "_AverageSpeed");
            _loadFactorFrom = new ParameterTS(data, "%", 0, 0, "vessel_" + this.Id + "_loadfrom");
            _loadFactorTo = new ParameterTS(data, "%", 0, 0, "vessel_" + this.Id + "loadto");
            _brakeSpecificFuelConsumption = new ParameterTS(data, "g/Wh", 150.0 / 1000.0, 0, "vessel_" + this.Id + "_fc"); //350 default for barge, 150 default for ocean tanker
            _hpPayloadFactor = new ParameterTS(data, "hp/ton", 0.101, 0, "vessel_" + this.Id + "_hpf"); //5600.0/22500.0 default for barge, 0.101 for ocean tanker
            _typicalHpRequirement = new ParameterTS(data, "hp", 9070, 0, "vessel_" + this.Id + "_hp"); //0 default for barge, 9070 default for ocean tanker
            _loadFactorAdjustmentBsfc = new ParameterTS(data, "g/kWh", 0, 0, "vessel_" + this.Id + "_adjustBsfc"); //that number is 14.42 for barges and zero for ocean tanker
        }

        #endregion constructors

        #region accessors

        [Browsable(false)]
        public ParameterTS AverageSpeed
        {
            get { return _averageSpeed; }
            set { _averageSpeed = value; }
        }

        [Browsable(false)]
        public ParameterTS LoadFactorAdjustmentBsfc
        {
            get { return _loadFactorAdjustmentBsfc; }
            set { _loadFactorAdjustmentBsfc = value; }
        }

        public ParameterTS LoadFactorFrom
        {
            get { return _loadFactorFrom; }
            set { _loadFactorFrom = value; }
        }

        public ParameterTS LoadFactorTo
        {
            get { return _loadFactorTo; }
            set { _loadFactorTo = value; }
        }

        [Browsable(false)]
        public Dictionary<int, MaterialTransportedPayload> Payload
        {
            get { return _payload; }
            set { _payload = value; }
        }

        public override bool CanBackHaul
        {
            get { return true; }
        }

        public ParameterTS BrakeSpecificFuelConsumption
        {
            get { return _brakeSpecificFuelConsumption; }
            set { _brakeSpecificFuelConsumption = value; }
        }


        public ParameterTS TypicalHPRequirement
        {
            get { return _typicalHpRequirement; }
            set { _typicalHpRequirement = value; }
        }


        public ParameterTS HpPayloadFactor
        {
            get { return _hpPayloadFactor; }
            set { _hpPayloadFactor = value; }
        }

        #endregion accessors

        #region methods
        internal override void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            try
            {
                this._payload = new Dictionary<int, MaterialTransportedPayload>();
                this.Id = Convert.ToInt32(node.Attributes["id"].Value);
                this.Name = node.Attributes["name"].Value;
                this.Type = (Modes.ModeType)Enum.ToObject(typeof(Modes.ModeType), Convert.ToInt32(node.Attributes["type"].Value));
                this._averageSpeed = new ParameterTS(data, node.SelectSingleNode("average_speed"), "vessel_" + this.Id + "_avgspd");
                this._loadFactorFrom = new ParameterTS(data, node.SelectSingleNode("load_factor_from"), "vessel_" + this.Id + "_loadffrom");
                this._loadFactorTo = new ParameterTS(data, node.SelectSingleNode("load_factor_to"), "vessel_" + this.Id + "_loadfto");
                this._brakeSpecificFuelConsumption = new ParameterTS(data, node.SelectSingleNode("typical_fc"), "vessel_" + this.Id + "_fc");
                this._typicalHpRequirement = new ParameterTS(data, node.SelectSingleNode("typical_hp"), "vessel_" + this.Id + "_hp");
                this._hpPayloadFactor = new ParameterTS(data, node.SelectSingleNode("hp_factor"), "vessel_" + this.Id + "_hpf");
                this._loadFactorAdjustmentBsfc = new ParameterTS(data, node.SelectSingleNode("bsfc_adjustment"),
                    "vessel_" + this.Id + "_bsfc_adjustement");
                if (node.Attributes["picture"].NotNullNOrEmpty())
                    this.PictureName = node.Attributes["picture"].Value;

                base.FromXmlNode(data, node, "vessel_" + this.Id);

                XmlNodeList payloads = node.SelectNodes("payload/material_transported");
                foreach (XmlNode payload in payloads)
                {
                    try
                    {
                        MaterialTransportedPayload pay = new MaterialTransportedPayload(data, payload, "vessel_" + this.Id + "_payload");
                        this._payload.Add(pay.Reference, pay);
                    }
                    catch (Exception e)
                    {
                        LogFile.Write
                        ("Error 101: " + e.Message);
                    }
                }

                if (node.SelectSingleNode("emissions_ratios") != null)
                {
                    this.ratiosBaselineFuel = Convert.ToInt32(node.SelectSingleNode("emissions_ratios").Attributes["baseline_fuel"].Value);
                    foreach (XmlNode derived in node.SelectNodes("emissions_ratios/derived_fuel"))
                    {
                        this.ratios.Add(Convert.ToInt32(derived.Attributes["fuel_ref"].Value), new EmissionRatios(data, derived));
                    }
                }
            }
            catch (Exception e)
            {
                LogFile.Write
                    ("Error 100: " + node.OwnerDocument.BaseURI + Environment.NewLine +
                    node.OuterXml + Environment.NewLine +
                    e.Message + Environment.NewLine);

                throw e;
            }
        }
        public override XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode t1 = base.ToXmlNode(xmlDoc);

            t1.Attributes.Append(xmlDoc.CreateAttr("type", (int)this.Type));
            t1.Attributes.Append(xmlDoc.CreateAttr("id", this.Id));
            t1.Attributes.Append(xmlDoc.CreateAttr("name", this.Name));
            t1.Attributes.Append(xmlDoc.CreateAttr("picture", this.PictureName));

            t1.AppendChild(this._hpPayloadFactor.ToXmlNode(xmlDoc, "hp_factor"));
            t1.AppendChild(this._typicalHpRequirement.ToXmlNode(xmlDoc, "typical_hp"));
            t1.AppendChild(this._brakeSpecificFuelConsumption.ToXmlNode(xmlDoc, "typical_fc"));
            t1.AppendChild(this._loadFactorTo.ToXmlNode(xmlDoc, "load_factor_to"));
            t1.AppendChild(this._loadFactorFrom.ToXmlNode(xmlDoc, "load_factor_from"));
            t1.AppendChild(this._averageSpeed.ToXmlNode(xmlDoc, "average_speed"));
            t1.AppendChild(this._loadFactorAdjustmentBsfc.ToXmlNode(xmlDoc, "bsfc_adjustment"));

            base.XMLFuelSharesAndER(t1, xmlDoc);

            XmlNode payload = xmlDoc.CreateNode("payload");
            foreach (KeyValuePair<int, MaterialTransportedPayload> payl in this._payload)
                payload.AppendChild(payl.Value.ToXmlNode(xmlDoc));

            t1.AppendChild(payload);

            return t1;
        }

        public override bool CheckIntegrity(GData data, bool showIds, out string errorMessage)
        {
            errorMessage = "";

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
        #endregion methods

    }


}