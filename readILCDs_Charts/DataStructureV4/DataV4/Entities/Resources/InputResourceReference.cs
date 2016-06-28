using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;
using Greet.DataStructureV4.ResultsStorage;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Defines the origin for an input resource. 
    /// It can be from well, mix, pathway, feed or main output of the previous process
    /// </summary>
    [Serializable]
    public class InputResourceReference : IInputResourceReference
    {
        #region attributes

        private int _resourceId = -1;
        private int _mixOrPathwayId = -1;
        private string _notes = "";
        private Greet.DataStructureV4.Interfaces.Enumerators.SourceType source;

        #endregion attributes

        #region constructors
        public InputResourceReference()
        {
        }

        public InputResourceReference(int material_id) :
            this(material_id, -1, Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix)
        {
        }

        public InputResourceReference(int resourceId, int mixOrPathId, Greet.DataStructureV4.Interfaces.Enumerators.SourceType srcType)
        {
            this._resourceId = resourceId;
            this._mixOrPathwayId = mixOrPathId;
            this.source = srcType;
        }

        public InputResourceReference(XmlNode node)
        {
            if (node.Attributes["id"] != null && String.IsNullOrEmpty(node.Attributes["id"].Value) == false)
                this._resourceId = Convert.ToInt32(node.Attributes["id"].Value);
            if (node.Attributes["ref"] != null && String.IsNullOrEmpty(node.Attributes["ref"].Value) == false)
                this._resourceId = Convert.ToInt32(node.Attributes["ref"].Value);
            if (node.Attributes["mix"] != null && String.IsNullOrEmpty(node.Attributes["mix"].Value) == false)
            {
                this._mixOrPathwayId = Convert.ToInt32(node.Attributes["mix"].Value);
                this.source = Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix;
            }
            if (node.Attributes["pathway"] != null && String.IsNullOrEmpty(node.Attributes["pathway"].Value) == false)
            {
                this._mixOrPathwayId = Convert.ToInt32(node.Attributes["pathway"].Value);
                this.source = Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway;
            }
            if (node.Attributes["notes"] != null && String.IsNullOrEmpty(node.Attributes["notes"].Value) == false)
                this._notes = node.Attributes["notes"].Value;
        }

        public InputResourceReference(XmlAttribute attribute)
        {
            string[] split = attribute.Value.Split(";".ToCharArray());
            if (split.Length >= 1)
                this._resourceId = Convert.ToInt32(split[0]);
            if (split.Length >= 2)
                if (split.Length >= 3)
                    this._mixOrPathwayId = Convert.ToInt32(split[2]);
            this.source = Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix;
        }

        public InputResourceReference(XmlAttribute xmlAttribute, XmlAttribute xmlAttribute_3, Greet.DataStructureV4.Interfaces.Enumerators.SourceType src)
        {
            this._resourceId = Convert.ToInt32(xmlAttribute.Value);
            this._mixOrPathwayId = Convert.ToInt32(xmlAttribute_3.Value);
            this.source = src;
        }
        #endregion constructors

        #region methods

        /// <summary>
        /// Append the attributes of the materials key to a existing node
        /// </summary>
        /// <param name="xmlDoc">The xml document where this attributes will be created</param>
        /// <param name="node">Existing node where the attributes will be append</param>
        /// <returns></returns>
        internal XmlNode ToXmlNode(XmlDocument xmlDoc, XmlNode node)
        {
            node.Attributes.Append(xmlDoc.CreateAttr("ref", _resourceId));
            if (this.source == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix)
                node.Attributes.Append(xmlDoc.CreateAttr("mix", _mixOrPathwayId));
            else if (this.source == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway)
                node.Attributes.Append(xmlDoc.CreateAttr("pathway", _mixOrPathwayId));
            return node;
        }

        public ResourceAmounts GetUpstreamResources(GProject project)
        {
            if (this.SourceType == Enumerators.SourceType.Mix)
                return project.Dataset.MixesData[this.SourceMixOrPathwayID].getMainOutputResults().Results.wellToProductEnem.materialsAmounts;
            else if (this.SourceType == Enumerators.SourceType.Pathway)
                return project.Dataset.PathwaysData[this.SourceMixOrPathwayID].getMainOutputResults().Results.wellToProductEnem.materialsAmounts;
            else
                return null;
        }

        public EmissionAmounts GetUpstreamEmissions(GProject project)
        {
            if (this.SourceType == Enumerators.SourceType.Mix)
                return project.Dataset.MixesData[this.SourceMixOrPathwayID].getMainOutputResults().Results.wellToProductEnem.emissions;
            else if (this.SourceType == Enumerators.SourceType.Pathway)
                return project.Dataset.PathwaysData[this.SourceMixOrPathwayID].getMainOutputResults().Results.wellToProductEnem.emissions;
            else
                return null;
        }

        public EmissionAmounts GetUpstreamUrbanEmissions(GProject project)
        {
            if (this.SourceType == Enumerators.SourceType.Mix)
                return project.Dataset.MixesData[this.SourceMixOrPathwayID].getMainOutputResults().Results.wellToProductUrbanEmission;
            else if (this.SourceType == Enumerators.SourceType.Pathway)
                return project.Dataset.PathwaysData[this.SourceMixOrPathwayID].getMainOutputResults().Results.wellToProductUrbanEmission;
            else
                return null;
        }
        #endregion methods

        #region accessors

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int ResourceId
        {
            get { return _resourceId; }
            set { _resourceId = value; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int SourceMixOrPathwayID
        {
            get { return this._mixOrPathwayId; }
            set { this._mixOrPathwayId = value; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public Greet.DataStructureV4.Interfaces.Enumerators.SourceType SourceType
        {
            get { return this.source; }
            set { this.source = value; }
        }
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        #endregion accessors

        #region nested_classes
        [Serializable]
        private class EqualityComparer : IEqualityComparer<InputResourceReference> //this is required if instances of the MaterialKey class are used as dicionary keys
        {
            public bool Equals(InputResourceReference x, InputResourceReference y)
            {
                return x.ResourceId == y.ResourceId && x._mixOrPathwayId == y._mixOrPathwayId && x.SourceType == y.SourceType;
            }
            public int GetHashCode(InputResourceReference x)
            {
                int purpose_key = -1;

                return purpose_key * 1000000 + x._mixOrPathwayId * 100000 + x.ResourceId * 10000;
            }
        }

        public bool Equals(InputResourceReference x)
        {
            return x._resourceId == this._resourceId && x._mixOrPathwayId == this._mixOrPathwayId && this.source == x.SourceType;
        }

        #endregion nested_classes

        #region methods

        public override string ToString()
        {
            return "Res:"+-_resourceId+"Src:"+source.ToString()+",MPId:"+_mixOrPathwayId;
        }
        #endregion methods

        public KeyValuePair<bool, string> IsEverythingAlrightWithMe(GData gData)
        {
            bool returnValue = true;
            string message = "";
            if (!gData.ResourcesData.ContainsKey(this.ResourceId))
            {
                message ="The resource specified an Input does not exists in the database" + "-" + this.ResourceId;
                returnValue = false;
            }
            else if(this.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix && !gData.MixesData.ContainsKey(this.SourceMixOrPathwayID))
            {
                message = "The Pathway Mix specified as Input does not exists in the database" + "-" + this.SourceMixOrPathwayID;
                returnValue = false;
            }
            else if (this.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway && !gData.PathwaysData.ContainsKey(this.SourceMixOrPathwayID))
            {
                message = "The Pathway specified as Input does not exists in the database" + "-" + this.SourceMixOrPathwayID;
                returnValue = false;
            }
            LogFile.Write(message);
            return new KeyValuePair<bool, string>(returnValue, message);
        }
    }
}
