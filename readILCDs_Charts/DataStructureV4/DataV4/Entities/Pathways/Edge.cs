using System;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class Edge : IEdge
    {
        #region members
        /// <summary>
        /// "Source" end of the connection
        /// </summary>
        private Guid _outputVertexID;
        /// <summary>
        /// "Source" end of the connection
        /// </summary>
        private Guid _outputID;
        /// <summary>
        /// "Destination" end of the connection
        /// </summary>
        private Guid _inputVertexID;
        /// <summary>
        /// "Destination" end of the connection
        /// </summary>
        private Guid _inputID;
        #endregion

        #region constructors
        /// <summary>
        /// Creates an empty edge where Guid are all zeros
        /// </summary>
        public Edge()
        { }

        /// <summary>
        /// Creates an edge and defines references to vertices and IOs
        /// </summary>
        /// <param name="outputVertexGuid">Vertex "Source" for the edge</param>
        /// <param name="outputGUID">Output of the vertex acting as "Source" for the edge</param>
        /// <param name="inputVertexGuid">Vertex "Destination" for the edge</param>
        /// <param name="inputGUID">Input of the vertex acting as "Destination" for the edge</param>
        public Edge(Guid outputVertexGuid, Guid outputGUID, Guid inputVertexGuid, Guid inputGUID)
        {
            this._outputVertexID = outputVertexGuid;
            this._outputID = outputGUID;
            this._inputVertexID = inputVertexGuid;
            this._inputID = inputGUID;
        }
        #endregion

        #region public and internal methods
        /// <summary>
        /// Saves the edges as an XML representation
        /// </summary>
        /// <param name="xmlDoc">XMLDocument for namespace, attributes and nodes creation</param>
        /// <returns>Edge XML representation respecting our V3 schema</returns>
        internal System.Xml.XmlNode ToXmlNode(System.Xml.XmlDocument xmlDoc)
        {
            XmlNode vertexNode = xmlDoc.CreateNode("edge",
               xmlDoc.CreateAttr("output-vertex", _outputVertexID),
               xmlDoc.CreateAttr("output-id", _outputID),
               xmlDoc.CreateAttr("input-vertex", _inputVertexID),
               xmlDoc.CreateAttr("input-id",_inputID));
            return vertexNode;
        }

        /// <summary>
        /// Loads an edge from an XML representation
        /// </summary>
        /// <param name="edgeNode">Edge XML node from an XML document respecting our V3 schema</param>
        internal void FromXmlNode(XmlNode edgeNode)
        {
            this._outputVertexID = new Guid(edgeNode.Attributes["output-vertex"].Value);
            this._outputID = new Guid(edgeNode.Attributes["output-id"].Value);
            this._inputVertexID = new Guid(edgeNode.Attributes["input-vertex"].Value);
            this._inputID = new Guid(edgeNode.Attributes["input-id"].Value);
        }

        /// <summary>
        /// Returns true if two instances of an Edge have the same attribute values
        /// If inputId, inputVertexId, outputId, outputVertexId are the same returns true.
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if attribute values are the same</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Edge))
                return false;
            else
            {
                Edge e = obj as Edge;
                return e._inputID == this._inputID
                    && e._inputVertexID == this._inputVertexID
                    && e._outputID == this._outputID
                    && e._outputVertexID == this._outputVertexID;
            }
        }

        /// <summary>
        /// Sourceforge:
        /// It s important to implement both equals and gethashcode, due to collisions, in particular while using dictionaries. 
        /// If two object returns same hashcode, they are inserted in the dictionary with chaining. While accessing the item equals method is used.
        /// </summary>
        /// <returns>base.GetHashCode()</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region public accessors
        public Guid OutputVertexID
        {
            get { return _outputVertexID; }
            set { _outputVertexID = value; }
        }
        public Guid OutputID
        {
            get { return _outputID; }
            set { _outputID = value; }
        }
        public Guid InputVertexID
        {
            get { return _inputVertexID; }
            set { _inputVertexID = value; }
        }
        public Guid InputID
        {
            get { return _inputID; }
            set { _inputID = value; }
        }
        #endregion
    }
}
