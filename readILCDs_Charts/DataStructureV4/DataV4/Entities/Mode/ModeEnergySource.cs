using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;


namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class ModeEnergySource : IModeEnergySource
    {
        #region attributes

        private Parameter share;
        private InputResourceReference reference;
        private int technologyTo = -1;
        private int technologyFrom = -1;

        string notes = "";

        #endregion attributes

        #region constructors

        public ModeEnergySource(GData data)
        {
            this.share = data.ParametersData.CreateRegisteredParameter("%", 0);
        }

        public ModeEnergySource(GData data, XmlNode node, string optionalParamPrefix)
        {
            string status = "";
            try
            {
                status = "reading resource reference";
                int material_id = Convert.ToInt32(node.Attributes["fuel_ref"].Value);
                status = "reading share";
                this.share = data.ParametersData.CreateRegisteredParameter(node.Attributes["share"], optionalParamPrefix + "_fuel_" + material_id + "_share");

                if (node.Attributes["mix"] != null)
                {
                    int material_mix = Convert.ToInt32(node.Attributes["mix"].Value);
                    this.reference = new InputResourceReference(material_id, material_mix, Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix);
                }
                else if (node.Attributes["pathway"] != null)
                {
                    int material_pathway = Convert.ToInt32(node.Attributes["pathway"].Value);
                    this.reference = new InputResourceReference(material_id, material_pathway, Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway);
                }

                if (node.Attributes["notes"] != null)
                    notes = node.Attributes["notes"].Value;

                //addded relatively to bug #922 issue 1
                if (node.Attributes["tech_to"] != null)
                    this.technologyTo = Convert.ToInt32(node.Attributes["tech_to"].Value);
                if (node.Attributes["tech_from"] != null)
                    this.technologyFrom = Convert.ToInt32(node.Attributes["tech_from"].Value);

            }
            catch (Exception e)
            {
                LogFile.Write("Error 93:" + node.OwnerDocument.BaseURI + "/r/n" +
                    node.OuterXml + "\r\n" +
                    e.Message + "\r\n" +
                    status + "\r\n"
                    );
            }
        }

        #endregion constructors

        #region accessors

        [Browsable(false)]
        public InputResourceReference ResourceReference
        {
            get { return reference; }
            set { reference = value; }
        }
        [Browsable(true), DisplayName("Share"), ReadOnly(false)]
        public Parameter Share
        {
            get { return share; }
            set { share = value; }
        }

        //addded relatively to bug #922 issue 1
        [Browsable(false)]
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int TechnologyTo
        {
            get { return technologyTo; }
            set { technologyTo = value; }
        }

        //addded relatively to bug #922 issue 1
        [Browsable(false)]
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int TechnologyFrom
        {
            get { return technologyFrom; }
            set { technologyFrom = value; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IInputResourceReference ResourceRef
        {
            get
            {
                return reference as IInputResourceReference;
            }
        }

        #endregion accessors

        #region methods

        internal XmlNode ToXmlNode(XmlDocument doc)
        {
            if (reference.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix)
                return doc.CreateNode("fuel", doc.CreateAttr("fuel_ref", reference.ResourceId),
                    doc.CreateAttr("mix", reference.SourceMixOrPathwayID), doc.CreateAttr("share", share), doc.CreateAttr("notes", this.notes), doc.CreateAttr("tech_to", this.technologyTo), doc.CreateAttr("tech_from", this.technologyFrom));
            else if (reference.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway)
                return doc.CreateNode("fuel", doc.CreateAttr("fuel_ref", reference.ResourceId),
                    doc.CreateAttr("pathway", reference.SourceMixOrPathwayID), doc.CreateAttr("share", share), doc.CreateAttr("notes", this.notes), doc.CreateAttr("tech_to", this.technologyTo), doc.CreateAttr("tech_from", this.technologyFrom));
            else
                return null;
        }

        #endregion methods
    }
}
