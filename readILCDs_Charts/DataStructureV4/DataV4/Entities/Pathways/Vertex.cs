using System;
using System.Drawing;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using System.Collections.Generic;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// <para>Represents either a feedstock (Pathway or Mix) or a process (Stationary or Transportation) in an instance of a Pathway</para>
    /// </summary>
    [Serializable]
    public class Vertex : IVertex
    {
        #region members
        /// <summary>
        /// Unique ID for that vertex among all vertices in the database
        /// </summary>
        private Guid _iD = Guid.NewGuid();
        /// <summary>
        /// Location if defined for representation on a graph, could be null if the user never place that item on the graph manually
        /// </summary>
        private PointF _location = new PointF();
        /// <summary>
        /// Reference to the process model unique ID used for this vertex
        /// </summary>
        private int _modelID = 0;
        /// <summary>
        /// 0 for a process, 1 for a pathway, 2 for a mix, 3 for an output (used in rare cases)
        /// </summary>
        private int _type = 0;
        /// <summary>
        /// Prefered functional units loaded from XML.
        /// We'll not modify the preferences in that object though the GUI, but we'll modify the Result object.
        /// When it is time to save a pathway, all the preferences will be passed as an argument of the ToXml method from the CanonicalProcesses and CanonicalOutput.Results stored in the pathway object
        /// </summary>
        private Dictionary<Guid, FunctionalUnitPreference> _xmlUnitPreferences = new Dictionary<Guid, FunctionalUnitPreference>();

       
        #endregion

        #region constructors
        /// <summary>
        /// Creates a new Vertex with all attribute set to their default values
        /// </summary>
        public Vertex()
        { }
       
        /// <summary>
        /// Instantiate the ModelID attribute to the model ID given as a parameter
        /// </summary>
        /// <param name="modelID"></param>
        public Vertex(int modelID)
        {
            this._modelID = modelID;
        }
        #endregion

        #region public and internal methods
        /// <summary>
        /// Creates an XML node representation of that instance of a Vertex
        /// Typiically used when saving the current object in memory to an XML data file
        /// </summary>
        /// <param name="xmlDoc">xmlDocument in which the node is going to be inserted for namespace reasons</param>
        /// <returns>XmlNode containing the data from the attributes of that object, can be then used with FromXmlNode to reload</returns>
        internal System.Xml.XmlNode ToXmlNode(System.Xml.XmlDocument xmlDoc, Dictionary<Guid, FunctionalUnitPreference> functionalUnitPreferences = null)
        {
            //creating the vertex node
            XmlNode vertexNode = xmlDoc.CreateNode("vertex",
                xmlDoc.CreateAttr("id", _iD),
                xmlDoc.CreateAttr("location", _location.X.ToString(Constants.USCI) + "," + _location.Y.ToString(Constants.USCI)),
                xmlDoc.CreateAttr("model-id", _modelID),
                xmlDoc.CreateAttr("type", _type));

            //saving the functional unit preference of each output of the vertex, will be used when loading the file later to assign preferences to the results objects
            if (functionalUnitPreferences != null)
            {
                foreach (KeyValuePair<Guid, FunctionalUnitPreference> prefPair in functionalUnitPreferences)
                {
                    if (prefPair.Value.enabled)
                    {
                        XmlNode fNode = prefPair.Value.ToXmlNode(xmlDoc);
                        fNode.Attributes.Append(xmlDoc.CreateAttr("id", prefPair.Key));
                        vertexNode.AppendChild(fNode);
                    }
                }
            }

            return vertexNode;
        }

        /// <summary>
        /// Instantiate all attributes for that object using data contained into an XML node
        /// Typically used when loading data from XML file at startup
        /// </summary>
        /// <param name="vertexNode">XML node containing the data to populate attributes of that object</param>
        internal void FromXmlNode(XmlNode vertexNode)
        {
            this._iD = new Guid(vertexNode.Attributes["id"].Value);
            string[] location = vertexNode.Attributes["location"].Value.Split(',');
            this._location = new PointF((float)Convert.ToDouble(location[0], Constants.USCI), (float)Convert.ToDouble(location[1], Constants.USCI));
            this._modelID = Convert.ToInt32(vertexNode.Attributes["model-id"].Value);
            this._type = Convert.ToInt16(vertexNode.Attributes["type"].Value);

            foreach (XmlNode n in vertexNode.SelectNodes("prefered_functional_unit"))
            {
                FunctionalUnitPreference fu = new FunctionalUnitPreference(n);
                Guid id = new Guid(n.Attributes["id"].Value);
                _xmlUnitPreferences.Add(id, fu);
            }
        }
        #endregion

        #region public accessors
        /// <summary>
        /// Read only, should not be modified.
        /// Used only for first initialization of the Results functional unit objects if nothing was there before.
        /// </summary>
        public Dictionary<Guid, FunctionalUnitPreference> XmlUnitPreferences
        {
            get { return _xmlUnitPreferences; }
        }
        /// <summary>
        /// Unique ID for that vertex among all vertices in the database
        /// </summary>
        public Guid ID
        {
            get { return _iD; }
            set { _iD = value; }
        }
        /// <summary>
        /// Location if defined for representation on a graph, could be null if the user never place that item on the graph manually
        /// </summary>
        public PointF Location
        {
            get { return _location; }
            set { _location = value; }
        }
        /// <summary>
        /// Reference to the process model unique ID used for this vertex
        /// </summary>
        public int ModelID
        {
            get { return _modelID; }
            set { _modelID = value; }
        }
        /// <summary>
        /// 0 for a process, 1 for a pathway, 2 for a mix, 3 for an output (used in rare cases)
        /// </summary>
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }
        #endregion

    }
}
