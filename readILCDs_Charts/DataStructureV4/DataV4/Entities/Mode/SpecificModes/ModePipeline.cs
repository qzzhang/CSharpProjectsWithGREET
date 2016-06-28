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
using Greet.LoggerLib;
using Greet.ConvenienceLib;


namespace Greet.DataStructureV4.Entities
{

    [Serializable]
    /// <summary>
    /// Mode type 3 can is Pipline. A mode is defined by the group of the fuel transported and the associated technologies
    /// A Pipeline is different from the other modes because it does not contain a list of payloads. 
    /// </summary>
    public class ModePipeline : AMode, IGraphRepresented
    {
        #region attributes

        /** This dictionaty maps an id of a resoure or resource group to the energy intensity */
        private Dictionary<int, PipelineMaterialTransported> energyIntensity = new Dictionary<int, PipelineMaterialTransported>();

        #endregion attributes

        #region constructors

        public ModePipeline(GData data, XmlNode node, string optionalParamPrefix) 
            : base(data)
        {
            FromXmlNode(data, node, optionalParamPrefix);
        }

        public ModePipeline(GData data)
            : base(data)
        {
            Type = Modes.ModeType.Pipeline;
        }

        #endregion constructors

        #region accessors

        /** This dictionaty maps an id of the material group to the energy intensity */
        [Browsable(false)]
        public Dictionary<int, PipelineMaterialTransported> EnergyIntensity
        {
            get { return energyIntensity; }
            set { energyIntensity = value; }
        }

        public override bool CanBackHaul
        {
            get { return false; }
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
                status = "reading picture";
                if (node.Attributes["picture"].NotNullNOrEmpty())
                    this.PictureName = node.Attributes["picture"].Value;

                base.FromXmlNode(data, node, "pipeline_" + this.Id);


                XmlNodeList eis = node.SelectNodes("energy_intensity/material_transported");
                foreach (XmlNode ei in eis)
                {
                    try
                    {
                        PipelineMaterialTransported ei_for_material_transported = new PipelineMaterialTransported(data, ei);
                        this.energyIntensity.Add(ei_for_material_transported.Reference, ei_for_material_transported);
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Error 83:" + e.Message + "\r\n");
                    }
                }


            }
            catch (Exception e)
            {
                LogFile.Write
                   ("Error 84:" + node.OwnerDocument.BaseURI + "\r\n" +
                   node.OuterXml + "\r\n" +
                   e.Message + "\r\n" +
                   status + "\r\n");
                throw e;
            } 
        }

        public override XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode t3 = base.ToXmlNode(xmlDoc);
            t3.Attributes.Append(xmlDoc.CreateAttr("type", (int)this.Type));
            t3.Attributes.Append(xmlDoc.CreateAttr("id", this.Id));
            t3.Attributes.Append(xmlDoc.CreateAttr("name", this.Name));
            t3.Attributes.Append(xmlDoc.CreateAttr("picture", this.PictureName));
            base.XMLFuelSharesAndER(t3, xmlDoc);
            XmlNode eiandpowershare = xmlDoc.CreateNode("energy_intensity");
            foreach (PipelineMaterialTransported ei in this.energyIntensity.Values)
                eiandpowershare.AppendChild(ei.ToXmlNode(xmlDoc));
            t3.AppendChild(eiandpowershare);
            return t3;
        }
        public override bool CheckIntegrity(GData data, bool showIds, out string errorMessage)
        {
            errorMessage = "";
            foreach (PipelineMaterialTransported PMT in this.energyIntensity.Values)
            {
                if (!data.ResourcesData.Keys.Contains(PMT.Reference) && !data.ResourcesData.Groups.Keys.Contains(PMT.Reference))
                    errorMessage += " - Contains a transported resource (" + PMT.Reference + ") that does not exist\r\n";
            }
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
           
            return true;
        }
       
        #endregion methods



    }

}