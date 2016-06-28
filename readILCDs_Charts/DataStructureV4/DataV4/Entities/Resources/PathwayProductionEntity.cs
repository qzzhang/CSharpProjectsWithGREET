using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Stores a reference to a pathway for a mix
    /// </summary>
    [Serializable]
    public class PathwayProductionEntity : FuelProductionEntity, ISerializable
    {
        #region constants
        
        const string REF = "ref";
        const string OUT = "output";
        
        #endregion
        
        #region attributes
        
        /// <summary>
        /// Unique ID of the pathway
        /// </summary>
        int _pathwayReference;

        /// <summary>
        /// Unique Output ID within the pathway reference
        /// </summary>
        Guid _outputReference;

        #endregion attributes

        #region constructors

        /// <summary>
        /// Creates a new Pathway production entity instance for use in a Mix
        /// Assing pathway ID and share for this pathway in the mix
        /// </summary>
        /// <param name="data">The database to which the refered parameter will be added for the Share</param>
        /// <param name="pathwayId">The ID of the pathway to be used</param>
        /// <param name="share">The share in default unit => 1 for 100%</param>
        public PathwayProductionEntity(GData data, int pathwayId, Guid outputId, double share)
            : base(data, share)
        {
            this._pathwayReference = pathwayId;
            this._outputReference = outputId;
        }


        public PathwayProductionEntity(int pathwayId, Guid outputId, ParameterTS share)
        {
            this._outputReference = outputId;
            this._pathwayReference = pathwayId;
            this._share = share;
        }

        public PathwayProductionEntity(GData data, XmlNode node, string optionalParamPrefix)
            : base(data, node, optionalParamPrefix)
        {
            this._pathwayReference = Convert.ToInt32(node.Attributes[REF].Value);
            if(node.Attributes[OUT] != null)
                this._outputReference = Guid.Parse(node.Attributes[OUT].Value);
        }

        protected PathwayProductionEntity(SerializationInfo information, StreamingContext context) :
            base(information, context)
        {
            _pathwayReference = information.GetInt32(REF);
            _outputReference = Guid.Parse(information.GetString(OUT));
        }
        
        #endregion constructors

        #region accessors

        public int PathwayReference
        {
            get { return _pathwayReference; }
            set { _pathwayReference = value; }
        }

        public Guid OutputReference
        {
            get { return _outputReference; }
            set { _outputReference = value; }
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
            XmlNode node = xmlDoc.CreateNode("pathway", xmlDoc.CreateAttr(REF, this._pathwayReference), xmlDoc.CreateAttr(OUT, this._outputReference));
            base.ToXmlNodeCommon(node, xmlDoc);
            return node;
        }

        /// <summary>
        /// Checks if the reference exists in the database, if we refer a pathway does this pathway exists in the database
        /// and is valid
        /// </summary>
        public override bool Exists(GData data)
        {
            string errors = "";
            if (data.PathwaysData.ContainsKey(_pathwayReference))
            {
                data.PathwaysData[_pathwayReference].CheckIntegrity(data, false, false, out errors);
                Pathway path = data.PathwaysData[_pathwayReference];
                if (!path.Outputs.Any(o => o.Id == _outputReference))
                    errors += "Pathway output specified does not exists\r\n";
            }
            else
                errors += "Pathway does not exists\r\n";
            return errors == "";
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(REF, _pathwayReference);
            info.AddValue(OUT, _outputReference.ToString());
            base.GetObjectData(info, context);
        }

        #endregion methods
    }
}