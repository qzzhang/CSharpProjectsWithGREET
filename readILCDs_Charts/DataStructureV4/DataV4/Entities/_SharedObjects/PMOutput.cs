using System;
using System.Drawing;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// <para>PMOutputs are used to define what upstreams are accessible from a pathway or a mix, this simplifies usage as it's uniform between path, proc and mix, they all have outputs with unique GUID</para>
    /// </summary>
    [Serializable]
    public class PMOutput : IIO
    {
        /// <summary>
        /// Resource for the output, theorically should match the outptut to which it is connected too, so may be considered redundent information
        /// but for now we'll do that for ease of programming. In a sense this could also be seen as double checking for the edge connecting the vertex to that output.
        /// </summary>
        private int resourceID;
        /// <summary>
        /// Unique ID for the output, similar reasoning as to a process output
        /// </summary>
        private Guid id; 
        /// <summary>
        /// Notes associated with that output, helps the user to choose an output or another in case multiple outputs are defined with the same resource
        /// </summary>
        private string notes;
        /// <summary>
        /// Location if defined for representation on a graph, could be null if the user never place that item on the graph manually
        /// </summary>
        public PointF Location { get; set; }

        /// <summary>
        /// Creates a new instance of a PMoutput,
        /// sets the resourceID to -1 and the ID to a new and probably unique Guid
        /// </summary>
        public PMOutput()
        {
            this.resourceID = -1;
            this.id = Guid.NewGuid();
            this.notes = "";
        }

        /// <summary>
        /// Creates an XML node representation of that instance of a PMOutput
        /// Typically used when saving the current object in memory to an XML data file
        /// </summary>
        /// <param name="xmlDoc">xmlDocument in which the node is going to be inserted for namespace reasons</param>
        /// <returns>XmlNode containing the data from the attributes of that object, can be then used with FromXmlNode to reload</returns>
        internal System.Xml.XmlNode ToXmlNode(System.Xml.XmlDocument xmlDoc)
        {
            XmlNode vertexNode = xmlDoc.CreateNode("output",
               xmlDoc.CreateAttr("id", id),
               xmlDoc.CreateAttr("resource", resourceID),
               xmlDoc.CreateAttr("location", Location.X.ToString(Constants.USCI) + "," + Location.Y.ToString(Constants.USCI)),
               xmlDoc.CreateAttr("notes", Notes));
            return vertexNode;
        }

        /// <summary>
        /// Instantiate all attributes for that object using data contained into an XML node
        /// Typically used when loading data from XML file at startup
        /// </summary>
        /// <param name="pathwayOutputNode">XML node containing the data to populate attributes of that object</param>
        internal void FromXmlNode(XmlNode pathwayOutputNode)
        {
            this.resourceID = Convert.ToInt32(pathwayOutputNode.Attributes["resource"].Value);
            string[] location = pathwayOutputNode.Attributes["location"].Value.Split(',');
            this.Location = new PointF((float)Convert.ToDouble(location[0], Constants.USCI), (float)Convert.ToDouble(location[1], Constants.USCI));
            this.id = new Guid(pathwayOutputNode.Attributes["id"].Value);
            this.Notes = pathwayOutputNode.Attributes["notes"].Value;
        }

       

        /// <summary>
        /// Resource for the output, theorically should match the outptut to which it is connected too, so may be considered redundent information
        /// but for now we'll do that for ease of programming. In a sense this could also be seen as double checking for the edge connecting the vertex to that output.
        /// </summary>
        public int ResourceId
        {
            get { return this.resourceID; }
            set { this.resourceID = value; }
        }

        /// <summary>
        /// Unique ID for the PMOutput, similar reasoning as to a process output
        /// </summary>
        public Guid Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// Notes associated with that output, helps the user to choose an output or another in case multiple outputs are defined with the same resource
        /// </summary>
        public string Notes
        {
            get { return this.notes; }
            set { this.notes = value; }
        }
    }
}
