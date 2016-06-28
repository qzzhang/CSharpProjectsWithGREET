using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.LoggerLib;


namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// <para>The pathway class stores vertices and edges that are resprectively representing processes and flows</para>
    /// <para>Before the calculations all processes and flows are converted to a canonical format for the calculations as the same process 
    /// can be reused in multiple pathways with different upstreams.</para>
    /// <para>The pathway also defines output, these outputs will be available for usage outside of the pathway, one of them must be declared
    /// MainOutput so the user dosn't need to choose the output everytime he desires to use this pathway as upstream for something</para>
    /// </summary>
    [Serializable]
    public class Pathway : IPathwayForDataGridViewDisplay, IComparable, IPathway, IHaveResults, IHaveMetadata, IHaveAPicture, IGREETEntity
    {
        #region constants
        const string xmlAttrModifiedOn = "modified-on";
        const string xmlAttrModifiedBy = "modified-by";
        #endregion

        #region attributes

        private int _id = -1;
        private string _name = "";
        private string _notes = "";
        /// <summary>
        /// Canonical processes by VertexID
        /// </summary>
        private Dictionary<Guid, CanonicalProcess> _canonicalProcesses = new Dictionary<Guid, CanonicalProcess>();
        private string _pictureName = Constants.EmptyPicture;
        /// <summary>
        /// Edges for connections in between the vertices
        /// </summary>
        private List<Edge> _edges = new List<Edge>();
        /// <summary>
        /// Vertices for that Pathway, can represent a feed or a process
        /// </summary>
        private Dictionary<Guid, Vertex> _vertices = new Dictionary<Guid, Vertex>();
        /// <summary>
        /// Defined outputs for the pathway that will be usable by others processes and pathways
        /// </summary>
        private List<PMOutput> _outputs = new List<PMOutput>();
        /// <summary>
        /// <para>User-defined main output for the pathway (only used for GUI simplification by automatically using the main output 
        /// when a pathway is defined as a feed for an input)</para>
        /// </summary>
        private Guid _mainOutput = new Guid();
        /// <summary>
        /// Stores username and email of the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string _modifiedOn = "";
        /// <summary>
        /// Stores date and time at which the user that modified this entity for the last time accoding to IHaveMetadata interface
        /// </summary>
        private string _modifiedBy = "";

        public bool Discarded { get; set; }
        public string DiscardedReason { get; set; }
        public DateTime DiscardedOn { get; set; }
        public string DiscarededBy { get; set; }
        #endregion attributes

        #region constructors

        /// <summary>
        /// <para>This constructor instantiate the name to "New Pathway " + id</para>
        /// <para>It also finds a new ID for the pathway based on the list of existing pathway IDs passed as a parameter</para>
        /// </summary>
        /// <param name="existingPathwayIDs">Existing pathway IDs to check that we're not creating a new instance with an ID that already exists</param>
        public Pathway(int[] existingPathwayIDs)
        {
            this._id = Convenience.IDs.GetIdUnusedFromTimeStamp(existingPathwayIDs);
            this._name = "New Pathway " + _id.ToString();
        }

        /// <summary>
        /// This constructor instantiate all Pathway attributes from the data provided in the XML node
        /// </summary>
        /// <param name="data">The GData object in which the pathway is going to be inserted</param>
        /// <param name="pathwayNode">XML node containing the information for the pathway</param>
        public Pathway(GData data, XmlNode pathwayNode)
        {
            this.FromXmlNode(data, pathwayNode);
        }

        #endregion constructors

        #region accessors

        /// <summary>
        /// Returns the resource ID associated with the main output of the pathway
        /// This accessor finds the Output for which the ID match the MainOutput guid and reutrns the resource defined for that output
        /// </summary>
        public int MainOutputResourceID
        {
            get
            {
                IIO output = this.OutputsData.SingleOrDefault(o => o.Id == this.MainOutput);
                if (output != null)
                    return output.ResourceId;
                else
                    return -1;
            }
        }

        /// <summary>
        /// An id to define the pathway
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The name of the  pathway
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Some notes associated to that pathway
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        [Browsable(false)]
        public string PictureName
        {
            get { return this._pictureName; }
            set { this._pictureName = value; }

        }

        /// <summary>
        /// The list of processes in that category
        /// </summary>
        [Browsable(false)]
        public Dictionary<Guid, CanonicalProcess> CanonicalProcesses
        {
            get { return _canonicalProcesses; }
            set { _canonicalProcesses = value; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IProcessReference> Processes
        {
            get
            {
                List<IProcessReference> procs = new List<IProcessReference>();
                foreach (CanonicalProcess procRef in this._canonicalProcesses.Values)
                    procs.Add(procRef as IProcessReference);
                return procs;
            }
        }

        [Browsable(true), DisplayName("Energy by Process Fuel")]
        public DVDict InputsByProcessFuel
        {
            get 
            {
                //TODO 6/12/2013
                return new DVDict();
                //return this.outputResults.materialsAmounts;
            
            }
        }

        [Browsable(true), DisplayName("Total Emissions")]
        public EmissionAmounts TotalEmissions
        {
            get 
            { 
                //TODO 6/12/2013
                return new EmissionAmounts();
                //return this.outputResults.emissions;
            }
        }

        public Boolean Valid(GData data)
        {
            return this.CheckPathwayGlobal(data).Key == 0;
        }

        /// <summary>
        /// Returns the output defined as the MainOutput of the pathway, only used for simplification reasons
        /// thus the user can choose the patwhay as an upstream and the MainOutput will be automatically selected
        /// as an Upstream for this pathway. Saves the user from selecting which of the outputs he desires to use
        /// </summary>
        public Guid MainOutput
        {
            get { return _mainOutput; }
            set { _mainOutput = value; }
        }
        
        /// <summary>
        /// <para>Lists outputs that are proper to the pathway</para>
        /// <para>This are not the outputs of the processes. The output of the processes are linked to these outputs using Edges but are not the same instances</para>
        /// </summary>
        public List<IIO> Outputs
        {
            get 
            {
                List<IIO> returned = new List<IIO>();
                foreach (PMOutput po in this._outputs)
                    returned.Add(po as IIO);
                return returned;
            }
        }

        public Dictionary<Guid, Vertex> VerticesData
        {
            get { return _vertices; }
            set { _vertices = value; }
        }

        public List<PMOutput> OutputsData
        {
            get { return _outputs; }
            set { _outputs = value; }
        }

        public List<Edge> EdgesData
        {
            get { return _edges; }
            set { _edges = value; }
        }

        public List<IEdge> Edges
        {
            get
            {
                List<IEdge> edges = new List<IEdge>();
                foreach (Edge e in _edges)
                    edges.Add(e);
                return edges;
            }
        }

        public List<IVertex> Vertices
        {
            get
            {
                List<IVertex> vertices = new List<IVertex>();
                foreach (Vertex v in _vertices.Values)
                    vertices.Add(v);
                return vertices;
            }
        }
        
        #endregion accessors

        #region internal and public methods

        /// <summary>
        /// Instantiate all properties of the pathway from the data contained into an XML node
        /// </summary>
        /// <param name="data">Never actually used in the method but necessary to respect the interface</param>
        /// <param name="pathwayNode">The XML node for the pathway</param>
        public void FromXmlNode(IData data, XmlNode pathwayNode)
        {
            string status = "";
            try
            {
                if (pathwayNode.Attributes["discarded"] != null)
                {
                    Discarded = Convert.ToBoolean(pathwayNode.Attributes["discarded"].Value);
                    DiscardedOn = Convert.ToDateTime(pathwayNode.Attributes["discardedOn"].Value, GData.Nfi);
                    DiscarededBy = pathwayNode.Attributes["discardedBy"].Value;
                    DiscardedReason = pathwayNode.Attributes["discardedReason"].Value;
                }

                status = "reading id";
                this._id = Convert.ToInt32(pathwayNode.Attributes["id"].Value);
                status = "reading notes";
                if (pathwayNode.Attributes["notes"] != null)
                    this._notes = pathwayNode.Attributes["notes"].Value;
                status = "reading picture";
                if (pathwayNode.Attributes["picture"].NotNullNOrEmpty())
                    this._pictureName = pathwayNode.Attributes["picture"].Value;
                status = "reading name";
                this._name = pathwayNode.Attributes["name"].Value;
                status = "reading main output";
                this._mainOutput = new Guid(pathwayNode.Attributes["main-output"].Value);
                status = "reading modified on";
                if (pathwayNode.Attributes[xmlAttrModifiedOn] != null)
                    this.ModifiedOn = pathwayNode.Attributes[xmlAttrModifiedOn].Value;
                status = "reading modified by";
                if (pathwayNode.Attributes[xmlAttrModifiedBy] != null)
                    this.ModifiedBy = pathwayNode.Attributes[xmlAttrModifiedBy].Value;

                foreach (XmlNode vertexNode in pathwayNode.SelectNodes("vertex"))
                {
                    Vertex vertex = new Vertex();
                    vertex.FromXmlNode(vertexNode);
                    this._vertices.Add(vertex.ID, vertex);
                }

                foreach (XmlNode edgeNode in pathwayNode.SelectNodes("edge"))
                {
                    Edge edge = new Edge();
                    edge.FromXmlNode(edgeNode);
                    this._edges.Add(edge);
                }

                foreach (XmlNode pathwayOutput in pathwayNode.SelectNodes("output"))
                {
                    PMOutput pout = new PMOutput();
                    pout.FromXmlNode(pathwayOutput);
                    this._outputs.Add(pout);
                }
            }
            catch (Exception e)
            {
                LogFile.Write("Error 88:" + pathwayNode.OwnerDocument.BaseURI + "\r\n" + pathwayNode.OuterXml + "\r\n" +
                        e.Message + "\r\n" + status + "\r\n");
                throw e;
            }
        }

        /// <summary>
        /// Check integrity of the pathway and returns a string containing the list of errors detected
        /// </summary>
        /// <param name="data">Instance of the dataset containing all processes, mixes and pathways</param>
        /// <param name="showIds">If true IDs will be shown in the error message</param>
        /// <param name="fixFixableIssues">If true some issues will be automatically fixed</param>
        /// <param name="errorMessage">Output containing human readable errors</param>
        /// <returns>True</returns>
        internal bool CheckIntegrity(GData data, bool showIds, bool fixFixableIssues, out string errorMessage)
        {
            errorMessage = "";
            
            KeyValuePair<int, string> errors = this.CheckPathwayGlobal(data, showIds, fixFixableIssues);
            if (errors.Key != 0)
                errorMessage += errors.Value;

            if (!String.IsNullOrEmpty(errorMessage))
                errorMessage += "\r\n";

            return true;
        }

        /// <summary>
        /// Returns a node containing the results process by process for the pathway
        /// </summary>
        /// <param name="doc">The XmlDocument for Namespace URI</param>
        /// <returns>The XML node containin results data for this pathway</returns>
        public XmlNode ToXmlResultsNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateNode("pathway", doc.CreateAttr("id", this._id), doc.CreateAttr("name", this._name), doc.CreateAttr("notes", this._notes));

            foreach (CanonicalProcess pRef in this.CanonicalProcesses.Values)
            {
                XmlNode proc_node = pRef.ResultsToXml(doc);
                node.AppendChild(proc_node);
            }
            return node;
        }

        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            //main node
            XmlNode pathway_node = xmlDoc.CreateNode("pathway");

            if (this.Discarded)
            {
                pathway_node.Attributes.Append(xmlDoc.CreateAttr("discarded", Discarded));
                pathway_node.Attributes.Append(xmlDoc.CreateAttr("discardedReason", DiscardedReason));
                pathway_node.Attributes.Append(xmlDoc.CreateAttr("discardedOn", DiscardedOn));
                pathway_node.Attributes.Append(xmlDoc.CreateAttr("discardedBy", DiscarededBy));
            }

            pathway_node.Attributes.Append(xmlDoc.CreateAttr("id", _id));
            pathway_node.Attributes.Append(xmlDoc.CreateAttr("name", _name));
            pathway_node.Attributes.Append(xmlDoc.CreateAttr("notes", _notes));
            pathway_node.Attributes.Append(xmlDoc.CreateAttr("main-output", _mainOutput));
            pathway_node.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedOn, this.ModifiedOn.ToString(GData.Nfi)));
            pathway_node.Attributes.Append(xmlDoc.CreateAttr(xmlAttrModifiedBy, this.ModifiedBy));

            //vertices
            
            foreach (Vertex vertex in this._vertices.Values)
            {
                Dictionary<Guid, FunctionalUnitPreference> preferences = new Dictionary<Guid,FunctionalUnitPreference>();
                if (_canonicalProcesses.ContainsKey(vertex.ID))
                {
                    foreach (KeyValuePair<Guid, CanonicalOutput> coPair in _canonicalProcesses[vertex.ID].OutputsResults)
                        preferences.Add(coPair.Key, coPair.Value.Results.CustomFunctionalUnitPreference);//<-- This is where we save the functional unit fron the results instance to the vertex instance, we store functional units for each outputs of the vertex by their GUID
                }
                pathway_node.AppendChild(vertex.ToXmlNode(xmlDoc, preferences));
            }

            //edges
            foreach (Edge edge in this._edges)
                pathway_node.AppendChild(edge.ToXmlNode(xmlDoc));

            //pathway outputs
            foreach (PMOutput output in this._outputs)
                pathway_node.AppendChild(output.ToXmlNode(xmlDoc));

            return pathway_node;
        }

        public override string ToString()
        {
            return "Pathway: " + this._name;
        }

        /// <summary>
        /// Check the properties of a pathway and return error number and human readable sentence to errors if any is detected
        /// </summary>
        /// <param name="data">The GData object for other entities references checks</param>
        /// <returns>KeyValuePair containing the number of errors, and a string explaining the detected issues</returns>
        public KeyValuePair<int, string> CheckPathwayGlobal(GData data, bool showIds = true, bool fixFixableIssues = true)
        {
            int errors_detected = 0;

            try
            {
                string message = "";

                //check that pathway have vertices
                if (this._vertices.Count == 0)
                {
                    errors_detected++;
                    message += " - There are no processes nor feedstocks in the pathway\r\n";
                }

                //check main outptut
                if(!this._outputs.Any(item => item.Id == this._mainOutput))
                {
                    errors_detected++;
                    message += "Main output of the pathway is not defined properly\r\n";
                }

                //Check if all the vertices model references exists in the database
                #region check vertices reference models existance 
                foreach (Vertex vertex in this._vertices.Values)
                {
                    if (vertex.Type == 0)
                    {//check for process reference
                        if (!data.ProcessesData.ContainsKey(vertex.ModelID))
                        {
                            errors_detected++;
                            message += " - The process " + vertex.ModelID + " does not exist\r\n";
                        }
                    }
                    else if (vertex.Type == 1)
                    {//check for pathway reference
                        if (!data.PathwaysData.ContainsKey(vertex.ModelID))
                        {
                            errors_detected++;
                            message += " - The pathway feed " + vertex.ModelID + " does not exist\r\n";
                        }
                    }
                    else if (vertex.Type == 2)
                    {//check for feed reference
                        if (!data.MixesData.ContainsKey(vertex.ModelID))
                        {
                            errors_detected++;
                            message += " - The mix feed " + vertex.ModelID + " does not exist\r\n";
                        }
                    }
                }
                #endregion

                //the pathway should have a name
                #region check pathway name
                if (string.IsNullOrEmpty(this.Name))
                {
                    errors_detected++;
                    message += " - The pathway has not been given a name\r\n";
                }
                #endregion

                //check that vertices refering to processes have inputs from previous correctly connected with an edge
                #region check inputs from previous
                foreach (Vertex vertex in this._vertices.Values)
                {
                    if (vertex.Type == 0)
                    {//check for process reference
                        if (data.ProcessesData.ContainsKey(vertex.ModelID))
                        {//get process and check inputs sources
                            AProcess process = data.ProcessesData[vertex.ModelID];
                            foreach (Input input in process.FlattenInputList)
                            {
                                if (input.SourceType == Enumerators.SourceType.Previous && !input.InternalProduct)
                                {//check all edges and find the one for connecting this input 
                                    bool connectionFound = false;
                                    foreach (Edge edge in this._edges)
                                    {
                                        if (edge.InputVertexID == vertex.ID
                                            && edge.InputID == input.Id)
                                        { //found the connection
                                            connectionFound = true;
                                            //here we could do more check and see if the edge is valid which means that the resource
                                            //is the same at the output and at the input. However this will also be tested in the
                                            //test for edges links below, so we do not care much about it here.
                                            break;
                                        }
                                    }
                                    if (!connectionFound) 
                                    {
                                        errors_detected++;
                                        message += "One of the input for the process " + process.Name 
                                            + " has an input from previous which is not properly connected\r\n";
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                //Check that pathway outputs are correctly connected with an edge
                #region check outputs connections
                foreach (PMOutput output in this._outputs)
                {
                    bool connected = false;
                    foreach (Edge edge in this._edges)
                    {
                        if (edge.InputID == output.Id)
                        {
                            connected = true;
                            break;
                        }
                    }

                    if (!connected) 
                    {
                        errors_detected++;
                        message += "A pathway output for resource " + output.ResourceId + " is not connected\r\n";
                    }
                }
                #endregion

                //Check edges links
                #region check edges validity
                foreach (Edge edge in this._edges)
                {
                    uint ErrorCode = this.CheckEdge(edge, data);
                    BitArray bits = new BitArray(BitConverter.GetBytes(ErrorCode).ToArray());
                    if (bits.Count > 0 && bits[0]){
                        errors_detected++;
                        string fromName = Pathway.NameFrom(data, this, edge);
                        string toName = Pathway.NameTo(data, this, edge);
                        message += "Left and Right resources are different for the connection between " + fromName + " and " + toName + "\r\n";
                    }
                    if (bits.Count > 1 && bits[1])
                    {
                        errors_detected++;
                        string fromName = Pathway.NameFrom(data, this, edge);
                        message += "Input vertex does not exists for a connexion from " + fromName + "\r\n";
                    }
                    if (bits.Count > 2 && bits[2])
                    {
                        errors_detected++;
                        string toName = Pathway.NameTo(data, this, edge);
                        message += "Output vertex does not exists for a connexion to " + toName + "\r\n";
                    }
                    if (bits.Count > 3 && bits[3])
                    {
                        errors_detected++;
                        string toName = Pathway.NameTo(data, this, edge);
                        message += "Input does not exists in "+ toName + " (Issue in the process IOs IDs or in the connection definition)\r\n";
                    }
                    if (bits.Count > 4 && bits[4])
                    {
                        errors_detected++;
                        string fromName = Pathway.NameFrom(data, this, edge);
                        message += "Output Id does not exists in " + fromName + " (Issue in the process IOs IDs or in the connection definition)\r\n";
                    }
                    if (bits.Count > 5 && bits[5])
                    {
                        errors_detected++;
                        string toName = Pathway.NameTo(data, this, edge);
                        message += toName + " model does not accepts an input from previous therefore cannot be linked with a connection\r\n";
                    }
                    if (bits.Count > 6 && bits[6])
                    {
                        errors_detected++;
                        string nameFrom = Pathway.NameFrom(data, this, edge);
                        message += "Model ID does not exists for the the input connection starting from " + nameFrom + "\r\n";
                    }
                    if (bits.Count > 7 && bits[7])
                    {
                        errors_detected++;
                        string nameTo = Pathway.NameTo(data, this, edge);
                        message += "Model ID does not exists for the output connection incoming to " + nameTo + "\r\n";
                    }
                    if (bits.Count > 8 && bits[8])
                    {
                        errors_detected++;
                        string nameFrom = Pathway.NameFrom(data, this, edge);
                        string nameTo = Pathway.NameTo(data, this, edge);
                        message += "Connection from a " + nameFrom + " displaced co-product to " + nameTo + " is impossible\r\n";
                    }
                }
                #endregion

                return new KeyValuePair<int, string>(errors_detected, message);
            }
            catch (Exception ex)
            {
                LogFile.Write(ex.Message);
                return new KeyValuePair<int, string>(errors_detected, "Multiple errors are found in the pathway.");
            }
        }

        /// <summary>
        /// Using the edge OutputVertexId finds out a Name for that vertex, it could be the name of a process, pathway or mix
        /// </summary>
        /// <param name="data">Data necessary to retreive instances of refered objects</param>
        /// <param name="path">Pathway in which the edge is used</param>
        /// <param name="edge">Edge from which we'll use properties to find out the name of the Output Vertex</param>
        /// <returns></returns>
        private static string NameFrom(GData data, Pathway path, Edge edge)
        {
            string fromName = "";
            if (path._vertices.ContainsKey(edge.OutputVertexID))
            {
                Vertex vertex = path._vertices[edge.OutputVertexID];

                if (vertex.Type == 0)
                {//looking for a process name 
                    if (data.ProcessesData.ContainsKey(vertex.ModelID))
                        fromName = data.ProcessesData[vertex.ModelID].Name;
                }
                else if (vertex.Type == 1)
                {//looking for a pathway name
                    if (data.PathwaysData.ContainsKey(vertex.ModelID))
                        fromName = data.PathwaysData[vertex.ModelID].Name;
                }
                else if (vertex.Type == 2)
                {//looking for a mix name
                    if (data.MixesData.ContainsKey(vertex.ModelID))
                        fromName = data.MixesData[vertex.ModelID].Name;
                }
            }
            return fromName;
        }

        /// <summary>
        /// Using the edge InptVertexId finds out a Name for that vertex, it could be the name of a process, pathway or mix
        /// </summary>
        /// <param name="data">Data necessary to retreive instances of refered objects</param>
        /// <param name="path">Pathway in which the edge is used</param>
        /// <param name="edge">Edge from which we'll use properties to find out the name of the Input Vertex</param>
        /// <returns></returns>
        private static string NameTo(GData data, Pathway path, Edge edge)
        {
            string toName = "";
            if (path._vertices.ContainsKey(edge.InputVertexID))
            {
                Vertex vertex = path._vertices[edge.InputVertexID];

                if (vertex.Type == 0)
                {//looking for a process name 
                    if (data.ProcessesData.ContainsKey(vertex.ModelID))
                        toName = data.ProcessesData[vertex.ModelID].Name;
                }
                else if (vertex.Type == 1)
                {//looking for a pathway name
                    if (data.PathwaysData.ContainsKey(vertex.ModelID))
                        toName = data.PathwaysData[vertex.ModelID].Name;
                }
                else if (vertex.Type == 2)
                {//looking for a mix name
                    if (data.MixesData.ContainsKey(vertex.ModelID))
                        toName = data.MixesData[vertex.ModelID].Name;
                }
            }
            else
            { 
                if(path._outputs.Any(item => item.Id == edge.InputID))
                {
                    PMOutput pathOutput = path._outputs.Single(item => item.Id == edge.InputID);
                    if (data.ResourcesData.ContainsKey(pathOutput.ResourceId))
                        toName = "pathway output " + data.ResourcesData[pathOutput.ResourceId].Name;
                }
            }
            return toName;
        }

        /// <summary>
        /// Check the edge connections at boths ends and see if this is valid or not
        /// Returns an error code describing the issue, the error can be checked bit by bit to know see if one or more issues are detected
        /// </summary>
        /// <param name="edge">
        /// 0 - Everything is fine
        /// 1 - Left and Right resources are different
        /// 2 - Input vertex does not exists in the pathway
        /// 4 - Output vertex does not exists in the pathway
        /// 8 - Input Id does not exists in the refered object (probably an issue in the process IOs IDs)
        /// 16 - Output Id does not exists in the refered object (output does not exists for pathway, mix, or process?)
        /// 32 - Vertex model does not accepts an input from previous therefore cannot be linked with an edge
        /// 64 - Input model ID does not exists
        /// 128 - Output model ID does not exists
        /// 256 - Edge connected to a displaced co-product
        /// <returns></returns>
        public uint CheckEdge(Edge edge, GData data)
        {
            uint errorsDetected = 0;

            bool inputIsPathwayOutput = false;

            foreach (PMOutput outp in this._outputs)
            {
                if (edge.InputID == outp.Id) 
                {
                    inputIsPathwayOutput = true;
                    break;
                }
            }

            //check that both vertices exists in the pathway
            bool inputVertexIDExists = inputIsPathwayOutput || this._vertices.ContainsKey(edge.InputVertexID);
            if (!inputVertexIDExists)
                errorsDetected += 2;
            bool outputVertexIDExists = this._vertices.ContainsKey(edge.OutputVertexID);
            if (!outputVertexIDExists)
                errorsDetected += 4;

            if (inputVertexIDExists && outputVertexIDExists)
            {
                Vertex inputVertex = null;
                PMOutput pathwayOutput = null;
                if (!inputIsPathwayOutput)
                    inputVertex = this._vertices[edge.InputVertexID];
                else
                    pathwayOutput = this._outputs.SingleOrDefault(item => item.Id == edge.InputID);

                Vertex outputVertex = this._vertices[edge.OutputVertexID];

                bool inputVertexModelExists = (inputIsPathwayOutput && pathwayOutput != null) || CheckVertexModelID(inputVertex, data);//order of operands matter here
                if (!inputVertexModelExists)
                    errorsDetected += 64;
                bool outputVertexModelExists = CheckVertexModelID(outputVertex, data);
                if (!outputVertexModelExists)
                    errorsDetected += 128;

                int commonResourceId = -1;

                if (inputVertexModelExists && !inputIsPathwayOutput)
                {
                    if (inputVertex.Type == 0)
                    {
                        AProcess process = data.ProcessesData[inputVertex.ModelID];
                        foreach (Input input in process.FlattenInputList)
                        {
                            if (input.Id == edge.InputID)
                            {
                                commonResourceId = input.ResourceId;
                            }
                        }
                    }
                    else if (inputVertex.Type == 1)
                        errorsDetected += 32;
                    else if (inputVertex.Type == 2)
                        errorsDetected += 32;
                }
                else if (inputVertexModelExists && inputIsPathwayOutput)
                {
                    commonResourceId = pathwayOutput.ResourceId;
                }

                if (outputVertexModelExists)
                {
                    if (outputVertex.Type == 0)
                    {
                        AProcess process = data.ProcessesData[outputVertex.ModelID];
                        bool outputFound = false;
                        foreach (CoProduct output in process.CoProducts)
                        {
                            if (output.id == edge.OutputID)
                            {
                                outputFound = true;
                                if (output.method == CoProductsElements.TreatmentMethod.allocation)
                                {

                                    if (commonResourceId != output.ResourceId)
                                    {
                                        commonResourceId = -1;
                                        errorsDetected += 1;
                                    }
                                    break;
                                }
                                else
                                {
                                    errorsDetected += 256;
                                }
                                break;
                            }
                        }
                        if (process.MainOutput.id == edge.OutputID)
                        {
                            outputFound = true;
                            if (commonResourceId != process.MainOutput.ResourceId)
                            {
                                commonResourceId = -1;
                                errorsDetected += 1;
                            }
                        }
                        if(!outputFound)
                        {
                            errorsDetected += 16;
                        }
                    }
                    else if (outputVertex.Type == 1)
                    {
                        Pathway pathway = data.PathwaysData[outputVertex.ModelID];
                        bool outputFound = false;
                        foreach (PMOutput output in pathway._outputs)
                        {
                            if (output.Id == edge.OutputID)
                            {
                                outputFound = true;
                                if (output.ResourceId != commonResourceId)
                                {
                                    commonResourceId = -1;
                                    errorsDetected += 1;
                                }
                                break;
                            }
                        }
                        if(!outputFound)
                        {
                            errorsDetected += 16;
                        }
                    }
                    else if (outputVertex.Type == 2)
                    {
                        Mix mix = data.MixesData[outputVertex.ModelID];
                        if (mix.output.ResourceId != commonResourceId)
                        {
                            commonResourceId = -1;
                            errorsDetected += 1;
                        }
                        if(mix.output.Id != edge.OutputID)
                        {
                            errorsDetected += 16;
                        }
                    }
                }
            }

            return errorsDetected;
        }

        /// <summary>
        /// Updates the cloned pathway class member vertices edges and outputs from the pathway returned from the PathwayViewer
        /// We only wants to update these specific attributes of the pathway as the pathway viewer does not know anything about name, id and other attributes
        /// </summary>
        /// <param name="source">The instance that will serve to update (source)</param>
        /// <param name="destination">The instance that will be updated (destination)</param>
        public static void UpdateVerticesAndEdges(Pathway source, Pathway destination)
        {
            List<Guid> toDeletedVertices = destination._vertices.Keys.ToList<Guid>();
            foreach (Vertex v in source._vertices.Values)
            {
                if (!destination._vertices.ContainsKey(v.ID))
                    destination._vertices.Add(v.ID, v);
                else if (toDeletedVertices.Contains(v.ID))
                    toDeletedVertices.Remove(v.ID);

                foreach (Vertex v2 in destination._vertices.Values)
                {
                    if (v.ID == v2.ID)
                    {
                        v2.Location = v.Location;
                    }
                }
            }

            foreach (Guid id in toDeletedVertices)
            {
                destination._vertices.Remove(id);
                IEnumerable<Edge> connectingToDeleted = destination._edges.Where(item => item.InputVertexID == id || item.OutputVertexID == id);
                foreach (Edge e in connectingToDeleted.ToArray())
                    destination._edges.Remove(e);
            }

            List<Edge> toDeleteEdges = destination._edges.ToList();
            foreach (Edge e in source._edges)
            {
                if (!destination._edges.Any(item => item.InputID == e.InputID && item.InputVertexID == e.InputVertexID
                    && item.OutputID == e.OutputID && item.OutputVertexID == e.OutputVertexID))
                    destination._edges.Add(e);
                else if (toDeleteEdges.Any(item => item.Equals(e)))
                    toDeleteEdges.Remove(toDeleteEdges.Single(item => item.Equals(e)));
            }

            foreach (Edge toDelete in toDeleteEdges)
                destination._edges.Remove(toDelete);

            List<PMOutput> toDeleteOutputs = destination._outputs.ToList();
            foreach (PMOutput o in source._outputs)
            {
                if (!destination._outputs.Any(item => item.Id == o.Id))
                    destination._outputs.Add(o);
                else if (toDeleteOutputs.Any(item => item.Id == o.Id))
                    toDeleteOutputs.Remove(toDeleteOutputs.Single(item => item.Id == o.Id));

                foreach (PMOutput output in destination._outputs)
                {
                    if (output.Id == o.Id)
                    {
                        output.Location = o.Location;
                    }
                }
            }
            if(source.MainOutput != Guid.Empty)
                destination.MainOutput = source.MainOutput;

            foreach (PMOutput toDelete in toDeleteOutputs)
                destination._outputs.Remove(toDelete);
        }

        /// <summary>
        /// Checks if the ModelId for a vertex exists in the database
        /// </summary>
        /// <param name="vertex">Vertex we want to be checked</param>
        /// <returns>False if the model ID does not exits</returns>
        private bool CheckVertexModelID(Vertex vertex, GData data)
        {
            if (vertex.Type == 0)
            {
                if (data == null
                    || data.ProcessesData == null
                    || !data.ProcessesData.ContainsKey(vertex.ModelID))
                    return false;
            }
            else if (vertex.Type == 1)
            {
                if (data == null
                    || data.PathwaysData == null
                    || !data.PathwaysData.ContainsKey(vertex.ModelID))
                    return false;
            }
            else if (vertex.Type == 2)
            {
                if (data == null
                   || data.MixesData == null
                   || !data.MixesData.ContainsKey(vertex.ModelID))
                    return false;
            }

            return true;
        }

        public int CompareTo(object obj)
        {
            if (obj is Pathway)
            {
                Pathway otherPathway = obj as Pathway;
                return this._name.CompareTo(otherPathway._name);
            }
            return 0;
        }

        /// <summary>
        /// Returns results for all outputs of this pathway
        /// Keys are PathwayOutput.Id members
        /// </summary>
        /// <param name="data">Data isn't used here and could ignored</param>
        /// <returns>Dictionary where Keys are PathwayOutput.Id and values are Results associated with these outputs</returns>
        public Dictionary<IIO, Results> GetResults(GData data)
        {
            Dictionary<IIO, Results> pathwayResults = new Dictionary<IIO, Results>();

            foreach (PMOutput output in this._outputs)
            {
                CanonicalOutput results = this.getOutputResults(output.Id);
                if (results != null)
                {
                    results.Results.ObjectID = this.Id;
                    results.Results.ObjectType = Enumerators.ItemType.Pathway;
                    results.Results.BiongenicCarbonRatio = results.MassBiogenicCarbonRatio;
                    pathwayResults.Add(output, results.Results);
                }
            }

            return pathwayResults;
        }

        /// <summary>
        /// Clears the results of all the processreference thus clearing the results of the pathway.
        /// </summary>
        public void ClearResults()
        {
            foreach (CanonicalProcess pRef in this._canonicalProcesses.Values)
                pRef.ClearAllResults();
        }

        /// <summary>
        /// <para>Assuming that the vertices that refers to a process have been converted to canonical process result storage instance</para>
        /// <para>Returns an instance of the CanonicalOutput object if the pathway has been converted to canonical processes</para>
        /// <para>May throw exceptions if the request cannot be satisfied</para>
        /// </summary>
        /// <returns>Results for the outptut defined as the main outptut of the pathway</returns>
        public CanonicalOutput getMainOutputResults()
        {
            if (this._mainOutput != new Guid())
            {
                //get reference to the pathway main output
                PMOutput pathMainOutput = this._outputs.SingleOrDefault(item => item.Id == this._mainOutput);

                if (pathMainOutput != null)
                {
                    return this.getOutputResults(pathMainOutput.Id);
                }
                else
                    throw new Exception("MainOutput attribute defined but does not corresponds to any of the outputs for this Pathway, check Pathway outputs");
            }
            else
                throw new Exception("MainOutput attribute not defined, therefore we can't know which canonical process OutputResults is to be used as main outptut for that pathway, define a main output");
        }

        /// <summary>
        /// Get output results from a Pathway output Guid
        /// </summary>
        /// <param name="guid">GUID of the pathway output</param>
        /// <returns>Canonical output of the process connected to that pathway output</returns>
        public CanonicalOutput getOutputResults(Guid guid)
        {
            //finds the edge that is used for that pathway output and connects to a vertex
            Edge mainOutputEdge = this._edges.SingleOrDefault(item => item.InputID == guid);

            if (mainOutputEdge != null)
            {
                //finds the vertex and associated canonical process output that connects this edge
                if (this._canonicalProcesses.ContainsKey(mainOutputEdge.OutputVertexID))
                {
                    CanonicalProcess cp = this._canonicalProcesses[mainOutputEdge.OutputVertexID];

                    if (cp.OutputsResults.ContainsKey(mainOutputEdge.OutputID))
                        return cp.OutputsResults[mainOutputEdge.OutputID];
                    else
                        throw new Exception("The edge defines a connection to an existing process reference, however this process does not have any output with the ID defined in the edge, check edge definition");
                }
                else
                    throw new Exception("The edge connecting to the main output is refering to a canonical process result storage that cannot be found in this Pathway.CanonicalProcesses, check edge definition or creation of CanonicalProcesses");
            }
            else
                throw new Exception("No edge can be found to connect the PathwayOutput to the output of a canonical process, check connections");
        }

        internal AOutput getOutputFromProcess(GData data, Guid outputId)
        {
            if (outputId != new Guid())
            {
                //get reference to the pathway main output
                PMOutput pathOutput = this._outputs.SingleOrDefault(item => item.Id == outputId);

                if (pathOutput != null)
                {
                    //finds the edge that is used for that pathway main output and connects to a vertex
                    Edge outputEdge = this._edges.SingleOrDefault(item => item.InputID == pathOutput.Id);

                    if (outputEdge != null)
                    {
                        //finds the vertex and associated canonical process output that connects this edge
                        if (this._vertices.ContainsKey(outputEdge.OutputVertexID))
                        {
                            Vertex vertexOutptut = this._vertices[outputEdge.OutputVertexID];
                            if (data.ProcessesData.ContainsKey(vertexOutptut.ModelID))
                            {
                                AProcess processOutput = data.ProcessesData[vertexOutptut.ModelID];

                                if (processOutput.MainOutput.id == outputEdge.OutputID)
                                    return processOutput.MainOutput;
                                CoProduct coprod = processOutput.CoProducts.SingleOrDefault(item => item.id == outputEdge.OutputID && item.method == CoProductsElements.TreatmentMethod.allocation);
                                if (coprod != null)
                                    return coprod;
                                else
                                    throw new Exception("The edge defines a connection to an existing process, however this process does not have any output with the ID defined in the edge, check edge definition");
                            }
                            else
                                throw new Exception("The database does not contains a process model which has the same id as the one refered in the Vertex.ModelId");
                        }
                        else
                            throw new Exception("The edge connecting to the main output is refering to a Vertex that cannot be found in this Pathway.Vertices, check edge definition or vertices");
                    }
                    else
                        throw new Exception("No edge can be found to connect the PathwayOutput to the output of a Vertex, check connections");
                }
                else
                    throw new Exception("Pathway.MainOutput attribute defined but does not corresponds to any of the outputs in Pathway.Outputs, check pathway outputs");
            }
            else
                throw new Exception("Pathway.MainOutput attribute not defined, therefore we can't know which canonical process OutputResults is to be used as main outptut for that pathway, define a main output");

        }

        /// <summary>
        /// <para>Uses the vertices, edges, pathways outptut and pathway main output definition to find the instance of an process model output</para>
        /// <para>The returned reference to instance corresponds to the output of a process model used in this pathway</para>
        /// <para>May throw exceptions if the request cannot be satisfied</para>
        /// </summary>
        /// <returns>Reference to the instance of an output for a process model</returns>
        public AOutput getMainOutputFromProcess(GData data)
        {
            return getOutputFromProcess(data, this._mainOutput);
        }

        /// <summary>
        /// Returns the process name from which one of the allocated outputs is connected to the desired pathway output
        /// </summary>
        /// <param name="guid">Guid of the desired pathway output</param>
        /// <returns>Name or exception if the pathway output guid does not exists or if the connection or the vertex is missing</returns>
        public string FindOutputName(Guid guid, GData data)
        {
            try
            {
                PMOutput outp = this._outputs.Single(p => p.Id == guid);
                Edge edge = this._edges.Single(e => e.InputVertexID == guid);
                Vertex ver = this._vertices[edge.OutputVertexID];
                switch (ver.Type)
                {
                    case 0:
                        return data.ProcessesData[ver.ModelID].Name;
                    case 1:
                        return data.PathwaysData[ver.ModelID].Name;
                    case 2:
                        return data.MixesData[ver.ModelID].Name;
                    default:
                        throw new Exception("Error in vertex type");
                }
            }
            catch (Exception e)
            {
                LogFile.Write(e.Message);
                return "Reference Error";
            }
        }

        /// <summary>
        /// Performs a breadth-first search and return the index of the output in the graph representaiton of the pathway
        /// </summary>
        /// <param name="guid">Guid of the output to be searched</param>
        /// <returns>Number of node probed before the output was found</returns>
        public int FindOutputDepth(Guid guid)
        {
            List<Vertex> vertices = new List<Vertex>();
            vertices.AddRange(this._vertices.Values);
            foreach (PMOutput op in this.Outputs)
            {
                Vertex vop = new Vertex();
                vop.ID = op.Id;
                vop.Type = 3;
                vertices.Add(vop);
            }

            int depth = 0;
            Queue<Vertex> queue = new Queue<Vertex>();
            List<Vertex> set = new List<Vertex>();

            foreach (Vertex v in vertices.Where(v => !this._edges.Any(e => e.InputVertexID == v.ID)))
            {
                set.Add(v);
                queue.Enqueue(v);
            }

            while (queue.Count > 0)
            {
                Vertex t = queue.Dequeue();
                if (t.ID == guid)
                    return depth;
                depth++;

                foreach (Edge e in this._edges.Where(e => e.OutputVertexID == t.ID))
                {
                    Vertex u = vertices.SingleOrDefault(v => v.ID == e.InputVertexID && v.ID != t.ID);
                    if (u != null && !set.Contains(u))
                    {

                        set.Add(u);
                        queue.Enqueue(u);
                    }
                }
            }
            return depth;
        }

        /// <summary>
        /// Performs a depth-first search in order to detect cycles in the pathway
        /// </summary>
        /// <returns></returns>
        public bool hasCycle()
        {
            if (this.VerticesData==null || this.VerticesData.Count == 0)
                return false;
            List<Guid> discovered = new List<Guid>();
            
            //select randomly a root vertex
            Random rnd = new Random();
            int index = rnd.Next(0, this.VerticesData.Count);
            Guid root = this.VerticesData.Keys.ElementAt(index);
            return this.DepthFirstSearch(this.VerticesData[root], discovered);

        }

        /// <summary>
        /// Recursive depth-first search algorithm
        /// </summary>
        /// <param name="v">Root vertex to search from</param>
        /// <param name="discovered">List of all discovered vertices recursively</param>
        /// <returns>True if cycle detected, False otherwise</returns>
        private bool DepthFirstSearch(Vertex v, List<Guid> discovered)
        {
            if(!discovered.Contains(v.ID))
                discovered.Add(v.ID);
            bool hasCycle = false;
            foreach(Edge e in this.EdgesData.Where(e => e.OutputVertexID == v.ID))
            {
                if (!discovered.Contains(e.InputVertexID))
                {
                    if(this.VerticesData.ContainsKey(e.InputVertexID))
                        hasCycle |= DepthFirstSearch(this.VerticesData[e.InputVertexID], discovered);
                }
                else
                    hasCycle = true;
            }
            return hasCycle;
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public Dictionary<IIO, IResults> GetUpstreamResults(IData data)
        {
            Dictionary<IIO, IResults> toReturn = new Dictionary<IIO, IResults>();
            Dictionary<IIO, Results> currentResults = this.GetResults(data as GData);
            foreach (KeyValuePair<IIO, Results> pair in currentResults)
                toReturn.Add(pair.Key, pair.Value as IResults);
            return toReturn;
        }

        #endregion methods

        #region IHaveMetadata Members

        public string ModifiedBy { get { return this._modifiedOn; } set { this._modifiedOn = value; } }

        public string ModifiedOn { get { return this._modifiedBy; } set { this._modifiedBy = value; } }

        #endregion

    }
}
