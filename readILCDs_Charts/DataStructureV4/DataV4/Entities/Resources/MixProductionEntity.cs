using System;
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Stores a reference to a mix for a mix
    /// </summary>
    [Serializable]
    public class MixProductionEntity : FuelProductionEntity, ISerializable
    {
        #region attributes

        int _mixReference;
        
        #endregion attributes

        #region constructors

        public MixProductionEntity(GData data, XmlNode node, string optionalParamPrefix)
            : base(data, node, optionalParamPrefix)
        {
            if (node.Attributes["mix"] != null && String.IsNullOrEmpty(node.Attributes["mix"].Value) == false)
                this._mixReference = Convert.ToInt32(node.Attributes["mix"].Value);
        }

        public MixProductionEntity(GData data, int mixId, double share)
            : base(data, share)
        {
            _mixReference = mixId;
        }

        public MixProductionEntity(int mixId, ParameterTS share)
        {
            _mixReference = mixId;
            _share = share;
        }

        protected MixProductionEntity(SerializationInfo information, StreamingContext context) :
            base(information, context)
        {
            _mixReference = information.GetInt32("mixref");
        }

        #endregion constructors

        #region accessors

        public int MixReference
        {
            get { return _mixReference; }
            set { _mixReference = value; }
        }

        #endregion accessors

        #region methods

        /// <summary>
        /// Converts that object to his representation as an XML node
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        public override XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode material_ref_node = xmlDoc.CreateNode("resource", xmlDoc.CreateAttr("mix", _mixReference));
            base.ToXmlNodeCommon(material_ref_node, xmlDoc);
            return material_ref_node;
        }

        /// <summary>
        /// Checks if the reference exists in the database, if we refer a pathway does this pathway exists in the database
        /// and is valid
        /// </summary>
        public override bool Exists(GData data)
        {
            string errors = "";
            data.MixesData[_mixReference].CheckIntegrity(data, false, out errors);
            return data.MixesData.ContainsKey(_mixReference)
                   && errors == "";
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mixref", _mixReference);
            base.GetObjectData(info, context);
        }

        #endregion methods 
    }
}