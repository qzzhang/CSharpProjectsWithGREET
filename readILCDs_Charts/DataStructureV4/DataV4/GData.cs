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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Entities;
using Greet.DataStructureV4.Interfaces;

namespace Greet.DataStructureV4
{
    [Serializable]
    public class GData : IData
    {
        public static CultureInfo Nfi = new CultureInfo("en-US");

        #region eventDelegates

        public delegate void ReadingXmlFilesInProgressEvent(int progress, string message);
        [field: NonSerialized]
        public event ReadingXmlFilesInProgressEvent ReadingDataInProgressEvent;

        public delegate void ReadingXmlFilesDoneEvent();
        [field: NonSerialized]
        public event ReadingXmlFilesDoneEvent ReadingDone;

        public delegate void MaterialParameterHasBeenChanged();
        [field: NonSerialized]
        public MaterialParameterHasBeenChanged PathwayParameterHasBeenChangedEvent;

        public delegate void EiHasBeenChanged();
        [field: NonSerialized]
        public EiHasBeenChanged EiHasBeenChangedEvent;

        public delegate void VehicleParameterHasBeenChanged();
        [field: NonSerialized]
        public VehicleParameterHasBeenChanged VehicleParameterHasBeenChangedEvent;

        #endregion eventDelegates

        #region attributes

        private EmissionGases _emissionGases;
        private Resources _resources;
        private Technologies _technologies;
        private Locations _locations;
        private Modes _modes;
        private Pathways _pathways;
        private InputTables _inputs;
        private Vehicles _vehicles;
        private Pictures _pictures;
        private Mixes _mixes;
        private Parameters _parameters;
        /// <summary>
        /// this dictionary holds both stationary and transportation processes
        /// </summary>            
        private Processes _processes;

        private bool _fuelCalculationNeeded = true;
        private bool _vehicleCalculationNeeded = true;
       
        public bool Loaded;
        public bool FullyLoaded;
        public bool ChangeDbFormulaCalculateOnTheFly = true;
        public bool AllRelationshipsFine;
        public Exception ExceptionToThrow;

        private DataHelper _helper;

        #endregion attributes

        #region constructors
        /// <summary>
        /// Build the data holder for greet, that object will store all the dataset 
        /// ( working and user copy ) to allow calculations
        /// </summary>
        public GData()
        {
            _inputs = new InputTables();
            _resources = new Resources();
            _pathways = new Pathways();
            _technologies = new Technologies();
            _modes = new Modes();
            _locations = new Locations();
            _emissionGases = new EmissionGases();
            _processes = new Processes();
            _vehicles = new Vehicles();
            _pictures = new Pictures();
            _mixes = new Mixes();
            _parameters = new Parameters();
            _helper = new DataHelper(this);
        }
       
        #endregion constructors

        #region methods

        #region RWdata

        /// <summary>
        /// reads all the files defined in the project
        /// </summary>
        /// <returns></returns>
        internal bool ReadDb(XmlNode dataNode, String dbFilePath)
        {
            WarnPathwayValueHasChangedNeedRecalculation();
            Loaded = false;
            FullyLoaded = false;

            bool correclyRead = true;

            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(1, "Loading input..."); }
            correclyRead &= _inputs.ReadDB(this, dataNode.SelectSingleNode("inputs"));
            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(9, "Loading gases..."); }
            correclyRead &= GasesData.ReadDB(this, dataNode.SelectSingleNode("gases"));
            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(18, "Loading resources..."); }
            correclyRead &= _resources.ReadDB(this, dataNode.SelectSingleNode("resources"));
            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(27, "Loading locations..."); }
            correclyRead &= _locations.ReadDB(this, dataNode.SelectSingleNode("locations"));
            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(36, "Loading modes..."); }
            correclyRead &= _modes.ReadDB(this, dataNode.SelectSingleNode("modes"));
            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(45, "Loading technologies..."); }
            correclyRead &= _technologies.ReadDB(this, dataNode.SelectSingleNode("technologies"));
            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(54, "Loading processes..."); }
            correclyRead &= _processes.ReadDB(this, dataNode.SelectSingleNode("processes"));
            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(81, "Loading pathways..."); }
            correclyRead &= _pathways.ReadDB(this, dataNode.SelectSingleNode("pathways"));
            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(90, "Loading vehicles..."); }
            correclyRead &= _vehicles.ReadDB(this, dataNode.SelectSingleNode("vehicles"));
            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(90, "Loading mixes..."); }
            correclyRead &= _mixes.ReadDB(this, dataNode.SelectSingleNode("mixes"));
            if (ReadingDataInProgressEvent != null) { ReadingDataInProgressEvent(100, "Done"); }

            if (ReadingDone != null) { ReadingDone(); }
            Loaded = correclyRead;
            FullyLoaded = _inputs.FullyLoaded && GasesData.fullyLoaded && _resources.fullyLoaded
                && _locations.fullyLoaded && _modes.fullyLoaded && _technologies.fullyLoaded
                && _processes.fullyLoaded && _pathways.fullyLoaded && _vehicles.fullyLoaded;

            ResourcesData.UpdateFrequencies(_processes);
            return Loaded && FullyLoaded;
        }

        /// <summary>
        /// Save all the data and return a single data node
        /// </summary>
        /// <param name="xmlDoc">Parent XmlDocuent for context</param>
        /// <param name="resetModificationFlag">Reset Modification flag to know if any data has been changed</param>
        /// <returns>True is saved successfully all the data</returns>
        internal XmlNode ToXmlNode(XmlDocument xmlDoc, bool resetModificationFlag)
        {
            XmlNode dataNode = xmlDoc.CreateNode("data");

            dataNode.AppendChild(_pathways.ToXmlNode(xmlDoc));
            dataNode.AppendChild(_resources.ToXmlNode(xmlDoc));
            dataNode.AppendChild(_processes.ToXmlNode(xmlDoc));
            dataNode.AppendChild(_technologies.ToXmlNode(xmlDoc));
            dataNode.AppendChild(_modes.ToXmlNode(xmlDoc));
            dataNode.AppendChild(_locations.ToXmlNode(xmlDoc));
            dataNode.AppendChild(_emissionGases.ToXmlNode(xmlDoc));
            dataNode.AppendChild(_inputs.ToXmlNode(xmlDoc));
            dataNode.AppendChild(_vehicles.ToXmlNode(xmlDoc));
            dataNode.AppendChild(_mixes.ToXmlNode(xmlDoc));

            return dataNode;
        }

        /// <summary>
        /// This method checks all the relationships in the data just after the data has been loaded
        /// </summary>
        /// <returns>Errors found in the database</returns>
        public string CheckIntegrity(bool showIds, bool fixFixableIssues)
        {
            StringBuilder problems = new StringBuilder();

            //checks parameters integrity and force evaluation of all formulas
            //refresh all buffers for evaluated formulas and parameter strings
            foreach (Parameter param in ParametersData.Values)
            {
                try
                {
                    param.UpdateBuffers(this);
                }
                catch (Exception e)
                {
                    problems.AppendLine("Parameter [" + (param != null && !String.IsNullOrEmpty(param.Name) ? param.Name : param.Id) + "]:" + Environment.NewLine + e.Message + "" + Environment.NewLine);
                }
            }

            //Checks that all the pathways and materials used in mix are existing in the pathway and material data.
            foreach (ResourceData mat in _resources.Values.Where(item => item.Discarded == false))
            {
                string errors = "";
                bool canBeHandled = mat.CheckIntegrity(this, showIds, out errors);
                if (string.IsNullOrEmpty(errors) == false)
                    problems.AppendLine("Resource \"" + mat.Name + "\"" + (showIds ? "(Id:" + mat.Id + ")" : "") + ":" + Environment.NewLine + errors);
            }

            //Checks that all the processes used in pathways are existing in the processes data
            foreach (Pathway path in _pathways.Values.Where(item => item.Discarded == false))
            {
                string errors = "";
                bool canBeHandled = path.CheckIntegrity(this, showIds, fixFixableIssues, out errors);
                if (string.IsNullOrEmpty(errors) == false)
                    problems.Append("Pathway \"" + path.Name + "\"" + (showIds ? "(Id:" + path.Id + ")" : "") + ":" + Environment.NewLine + errors);
            }

            foreach (Mix mix in _mixes.Values.Where(item => item.Discarded == false))
            {
                string errors = "";
                bool canBeHandled = mix.CheckIntegrity(this, showIds, out errors);
                if (string.IsNullOrEmpty(errors) == false)
                    problems.AppendLine("Mix \"" + mix.Name + "\"" + (showIds ? "(Id:" + mix.Id + ")" : "") + ":" + Environment.NewLine + errors);
            }

            //Check the processes integrity
            List<Guid> guids = new List<Guid>();
            foreach (AProcess proc in _processes.Values.Where(item => item.Discarded == false))
            {
                try
                {
                    string errors = "";
                    bool canHandleErrors = proc.CheckIntegrity(this, showIds, fixFixableIssues, out errors);
                    if (string.IsNullOrEmpty(errors) == false)
                        problems.AppendLine("Process \"" + proc.Name + "\"" + (showIds ? "(Id:" + proc.Id + ")" : "") + ":" + Environment.NewLine + errors);

                    List<IIO> outputs = proc.FlattenAllocatedOutputList;
                    foreach (IIO o in outputs)
                    {
                        if (!guids.Contains(o.Id))
                            guids.Add(o.Id);
                        else
                            problems.AppendLine("\n\n\"" + proc.Name + "\"" + (showIds ? "(Id:" + proc.Id + ")" : "") + ":" + Environment.NewLine + "GUID " + o.Id + " Already exists!!");
                    }
                    List<IInput> inputs = proc.FlattenInputList;
                    foreach (IInput i in inputs)
                    {
                        if (!guids.Contains(i.Id))
                            guids.Add(i.Id);
                        else
                            problems.AppendLine("\n\n\"" + proc.Name + "\"" + (showIds ? "(Id:" + proc.Id + ")" : "") + ":" + Environment.NewLine + "GUID" + i.Id + " Already exists!!");
                    }
                }
                catch (Exception pe)
                {
                    problems.AppendLine("Process \"" + proc.Name + "\"" + (showIds ? "(Id:" + proc.Id + ")" : "") + ":" + Environment.NewLine + pe.Message);
                }
            }

            //check gas memberships
            foreach (Gas gas in GasesData.Values.Where(item => item.Discarded == false))
            {
                string errors = "";
                bool canHandleErrors = gas.CheckIntegrity(this, showIds, out errors);
                if (String.IsNullOrEmpty(errors) == false)
                    problems.AppendLine("Emission \"" + gas.Name + "\"" + (showIds ? "(Id:" + gas.Id + ")" : ":" + Environment.NewLine) + errors);
            }


            //modes
            foreach (AMode mode in ModesData.Values.Where(item => item.Discarded == false))
            {
                string errors = "";
                bool canHandleErrors = mode.CheckIntegrity(this, showIds, out errors);
                if (String.IsNullOrEmpty(errors) == false)
                    problems.AppendLine("Mode \"" + mode.Name + "\"" + (showIds ? "(Id:" + mode.Id + ")" : ":" + Environment.NewLine) + errors);
            }

            //technologies
            foreach (TechnologyData technology in TechnologiesData.Values.Where(item => item.Discarded == false))
            {
                string errors = "";
                bool canHandleErrors = technology.CheckIntegrity(this, showIds, out errors);
                if (string.IsNullOrEmpty(errors) == false)
                    problems.AppendLine("Technology \"" + technology.Name + "\"" + (showIds ? "(Id:" + technology.Id + ")" : ":" + Environment.NewLine) + errors);
            }
           
            //check vehicles
            foreach (Vehicle vehicle in VehiclesData.Values.Where(item => item.Discarded == false))
            {
                string errors = "";
                bool canHandleErrors = vehicle.CheckIntegrity(this, showIds, out errors);
                if (string.IsNullOrEmpty(errors) == false)
                    problems.AppendLine("Vehicle \"" + vehicle.Name + "\"" + (showIds ? "(Id:" + vehicle.Id + ")" : ":" + Environment.NewLine) + errors);
            }
        
            if (problems.Length > 0)
                AllRelationshipsFine = false;
            else
                AllRelationshipsFine = true;
            return problems.ToString();
        }

        #endregion RWdata

        public void WarnPathwayValueHasChangedNeedRecalculation()
        
        {
            FuelCalculationNeeded = true;
            VehicleCalculationNeeded = true;

            if (PathwayParameterHasBeenChangedEvent != null)
                PathwayParameterHasBeenChangedEvent();
        }

        public void WarnVehicleValueHasChangedNeedRecalculation()
        {
            VehicleCalculationNeeded = true;

            if (VehicleParameterHasBeenChangedEvent != null)
                VehicleParameterHasBeenChangedEvent();
        }

        /// <summary>
        /// This is a list of all mixes and pathways that output a given resource.
        /// </summary>
        /// <param name="resourceId">The resource id whose list of possible ways is needed</param>
        /// <returns>The return type is a list of InputResourceReference's because this object can represent a pathway or a mix depending on the source attribute of the object</returns>
        internal List<InputResourceReference> GetListOfMixesAndPathways(int resourceId)
        {
            //our list we will be returning 
            List<InputResourceReference> rrList = new List<InputResourceReference>();

            //get mixes and add them to the list 
            Dictionary<int, Mix>.ValueCollection allMixes = MixesData.Values;
            if (allMixes != null)
            {
                foreach (Mix m in allMixes.Where(item => item.MainOutputResourceID == resourceId || ResourcesData[resourceId].CompatibilityIds.Contains(item.MainOutputResourceID)).OrderBy(item => item.Name))
                {
                    InputResourceReference rr = new InputResourceReference(resourceId, m.Id, Enumerators.SourceType.Mix);
                    rrList.Add(rr);
                }
            }

            //get pathways and add them to the list
            foreach (Pathway pw in PathwaysData.Values.OrderBy(item => item.Name))
            {
                foreach(PMOutput outp in pw.OutputsData)
                {
                    if(outp.ResourceId == resourceId)
                    {
                         InputResourceReference rr = new InputResourceReference(resourceId, pw.Id, Enumerators.SourceType.Pathway);
                         rrList.Add(rr);
                    }
                }
            }

            return rrList;
        }

        /// <summary>
        /// Method for convenience that uses a pathway ID in order to grab the last process in that pathway
        /// and figure out the main output resource ID from the last process
        /// </summary>
        /// <param name="pathwayId">The pathway ID for which we want to know the main output resource ID</param>
        /// <returns>The main output ID or -1 if not possible to find either the pathway or the process</returns>
        public int PathwayMainOutputResouce(int pathwayId)
        {
            if (PathwaysData.ContainsKey(pathwayId))
            {
                Pathway pw = PathwaysData[pathwayId];
                foreach (PMOutput pout in pw.OutputsData)
                {
                    if (pout.Id == pw.MainOutput)
                        return pout.ResourceId;
                }
                return -1;
            }
            return -1;
        }

        /// <summary>
        /// Extensive checks to make sure that changes to an existing process model will not break all edges in pathways using it
        /// If that proces isn't in the database or used anywhere, it can be safely added without any checks
        /// </summary>
        /// <param name="project"></param>
        /// <param name="updatedProcess">Updated process to be checked for replacing an existing one</param>
        /// <param name="msg">Message containing possible errors if any for insertion in the database</param>
        public List<string> CheckProcessModelModification(GProject project, AProcess updatedProcess, out string msg)
        {
            msg = "";
            List<string> actions = new List<string>();

            //Checks if the database already contains a process with the same ID
            //this needs to be checked in case the user edits a process, and want to save it to the
            //database in order to overwrite the old copy
            AProcess originalProcess = null;
            if (project.Dataset.ProcessesData.ContainsKey(updatedProcess.Id))
                originalProcess = project.Dataset.ProcessesData[updatedProcess.Id];
            else
                return null; //the process is not yet in the database, we can include it without any risks

            //Checks if the process is used in a pathway and create a list of the pathways containing a vertex refereing to that process
            IEnumerable<Pathway> pathways =
                from path in project.Dataset.PathwaysData.Values where path.Discarded == false
                where (from pr in path.VerticesData.Values where pr.Type == 0 select pr.ModelID).Contains(updatedProcess.Id)
                select path;

            if (pathways.Count() == 0)
                return actions; //if the process is not used anywhere it dosen't matter if we make modifications to it
            foreach (Pathway path in pathways)
            {
                string msgPath = "";//messages for this pathway only

                //locate the process in the pathway if the pathway is using it.
                IEnumerable<Vertex> originalVertices =
                    from vertx in path.VerticesData.Values
                    where vertx.Type == 0 && vertx.ModelID == updatedProcess.Id
                    select vertx;

                foreach(Vertex originalVertex in originalVertices)
                {
                    //find all edges associated with that vertex in the existing pathway
                    List<Edge> originalInputEdges = new List<Edge>();
                    foreach (Edge e in path.EdgesData)
                        if (e.InputVertexID.Equals(originalVertex.ID))
                            originalInputEdges.Add(e);
                    List<Edge> originalOutputEdges = new List<Edge>();
                    foreach (Edge e in path.EdgesData)
                        if (e.OutputVertexID.Equals(originalVertex.ID))
                            originalOutputEdges.Add(e);

                    //check that all new inputs are connected with an edge
                    foreach (IInput updatedInput in updatedProcess.FlattenInputList)
                    {
                        if (updatedInput.SourceType == Enumerators.SourceType.Previous && updatedInput.InternalProduct == false
                            && !path.EdgesData.Any(item => item.InputVertexID == originalVertex.ID
                                                           && item.InputID == updatedInput.Id))
                        {//no edge found for that input
                            msgPath += "The input for " + ResourcesData[updatedInput.ResourceId].Name + " as a source " + Constants.sourceNames[(int)updatedInput.SourceType] + " is not connected to anything" + Environment.NewLine;
                        }
                    }

                    //check that all new outputs are connected with an edge
                    foreach(IIO updatedOutput in updatedProcess.FlattenAllocatedOutputList)
                    {
                        if (updatedOutput is MainOutput)
                        {
                            if (!path.EdgesData.Any(item => item.OutputVertexID == originalVertex.ID
                                                            && item.OutputID == updatedOutput.Id))
                            {//no edge found for that output, however as it is allocated we should connect it to something
                                msgPath += "The " + (updatedOutput is MainOutput ? "main output" : "co-product") + " for " + ResourcesData[updatedOutput.ResourceId].Name + " is not connected to anything" + Environment.NewLine;
                            }
                        }
                    }

                    //check that all existing edges are valid for the new process (maybe the user change the source of an input, deleted an input...)
                    foreach(Edge ed in originalInputEdges)
                    {
                        if (!updatedProcess.FlattenInputList.Any(item => item.Id == ed.InputID))
                        {
                            msgPath += "A incoming connection to that process is going to be left unconnected as an input has been removed or replaced" + Environment.NewLine;
                            string action = "DELETE FROM PATHWAY " + path.Id + " WHERE INPUTVERTEX " + ed.InputVertexID + " INPUT " + ed.InputID + " OUTPUTVERTEX " + ed.OutputVertexID + " OUTPUT " + ed.OutputID;
                            actions.Add(action);
                        }
                        else if (!updatedProcess.FlattenInputList.Any(item => item.Id == ed.InputID && item.SourceType == Enumerators.SourceType.Previous && item.InternalProduct == false))
                        {
                            msgPath += "A incoming connection to that process is going to be left unconnected as an input source has been modified or the inptut is set as internal product" + Environment.NewLine;
                            string action = "DELETE FROM PATHWAY " + path.Id + " WHERE INPUTVERTEX " + ed.InputVertexID + " INPUT " + ed.InputID + " OUTPUTVERTEX " + ed.OutputVertexID + " OUTPUT " + ed.OutputID;
                            actions.Add(action);
                        }
                    }

                    //check that all existing edges are valid for the new process (maybe the user changed the treatment method from allocation to displacement for a co-product or removed one of the outputs)
                    foreach (Edge ed in originalOutputEdges)
                    {
                        if (!updatedProcess.FlattenAllocatedOutputList.Any(item => item.Id == ed.OutputID))
                        {
                            msgPath += "A output connection to that process is going to be left unconnected as the output has been removed or replaced" + Environment.NewLine;
                            string action = "DELETE FROM PATHWAY " + path.Id + " WHERE INPUTVERTEX " + ed.InputVertexID + " INPUT " + ed.InputID + " OUTPUTVERTEX " + ed.OutputVertexID + " OUTPUT " + ed.OutputID;
                            actions.Add(action);
                        }
                    }
                }

                //creat message specific for this pathway
                if (msgPath != "")
                    msg += "The pathway: " + path.Name + " uses this process:" + Environment.NewLine + msgPath;
            }
            return actions;
        }

        public void PerformActions(List<string> actions)
        {
            string[] split;
            foreach (string action in actions)
            {
                split = action.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length >= 1 && split[0] == "DELETE")
                {
                    if (split.Length >= 2 && split[1] == "FROM")
                    {
                        if (split.Length >= 3 && split[2] == "PATHWAY")
                        { 
                            int pathId;
                            if (split.Length >= 4 && int.TryParse(split[3], out pathId))
                            {
                                Pathway p = PathwaysData.Values.SingleOrDefault(item => item.Id == pathId);
                                if (p != null && split.Length >= 5 && split[4] == "WHERE")
                                {
                                    if (split.Length >= 12 && split[5] == "INPUTVERTEX" && split[7] == "INPUT"
                                        && split[9] == "OUTPUTVERTEX" && split[11] == "OUTPUT") 
                                    {//we're deleteing an edge from the pathway
                                        Guid inVId, inId, oVId, oId;
                                        if (Guid.TryParse(split[6], out inVId) && Guid.TryParse(split[8], out inId) && Guid.TryParse(split[10], out oVId) && Guid.TryParse(split[12], out oId))
                                        {
                                            Edge e = p.EdgesData.SingleOrDefault(item => item.InputID.Equals(inId) && item.InputVertexID.Equals(inVId)
                                                && item.OutputID.Equals(oId) && item.OutputVertexID.Equals(oVId));
                                            if (e != null && p.EdgesData.Contains(e))
                                                p.EdgesData.Remove(e);
                                            int c = p.EdgesData.Count;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool SafeRemoveFuelShare(int modeId, int modeFuelShareId, out string msg)
        {
            msg = "";
            var processes =
                from process in ProcessesData.Transportation().Values
                where process.UsesFuelShare(modeId, modeFuelShareId)
                select process;
            if (processes.Count() > 0)
            {
                msg += "It is impossible to delete the fuel share item.\n";
                msg += "The following transportation processes reference it:\n";
                foreach (TransportationProcess p in processes)
                    msg += String.Format("- {0}\n", p.Name);
                return false;
            }
            if (ModesData.ContainsKey(modeId))
                ModesData[modeId].FuelSharesData.Remove(modeFuelShareId);
            return true;
        }

        /// <summary>
        /// This method removes an entry from list of energy intecities. Before removeing, it checks wheter the ei is used anywhere.
        /// </summary>
        /// <param name="materialId">The id of the materil/material group to be removed from the energyIntensity</param>
        /// <param name="msg">If remove fails, the msg contains meaningful explanation of whay it happened.</param>
        /// <returns>The true is returned if remove operation was success, false otherwise</returns>
        public bool SafePipelineEiRemove(int modeId, int materialId, out string msg)
        {
            msg = "";
            //before we can delete an ei, we need to make sure that none of the transportation processes depends on it
            var processes =
                from process in ProcessesData.Transportation().Values
                where process.UsesMode(5) &&
                (process.MainOutputResourceID == materialId ||
                      ResourcesData[process.MainOutputResourceID].Memberships.Contains(materialId))
                select process;
            if (processes.Count() > 0)
            {
                msg += "It is impossible to delete the energy intensity item.\n";
                msg += "The following transportation processes reference it:\n";
                foreach (TransportationProcess p in processes)
                    msg += String.Format("- {0}\n", p.Name);
                return false;
            }

            (ModesData[modeId] as ModePipeline).EnergyIntensity.Remove(materialId);

            return true;
        }

        public List<MixListItem> CreateMixListItems(IEnumerable<Mix> mixes)
        {
            List<MixListItem> list = new List<MixListItem>();

            foreach (Mix m in mixes)
            {
                MixListItem ml = new MixListItem();
                if(ResourcesData.ContainsKey(m.MainOutputResourceID))
                    ml.MaterialName = ResourcesData[m.MainOutputResourceID].Name;
                ml.MixName = m.Name;
                ml.MixTag = m;
                ml.MaterialId = m.output.ResourceId;
                list.Add(ml);
            }

            return list;
        }

        public List<TechnologyListItem> CreateTechnologyListItems(IEnumerable<TechnologyData> techs)
        {
            List<TechnologyListItem> list = new List<TechnologyListItem>();

            foreach (TechnologyData te in techs)
            {
                TechnologyListItem tl = new TechnologyListItem();
                tl.MaterialId = te.InputResourceRef;
                tl.MaterialName = ResourcesData[te.InputResourceRef].Name;
                tl.TechologyName = te.Name;
                tl.TechnologyTag = te;
                list.Add(tl);
            }

            return list;
        }

        public List<VehicleListItem> CreateVehicleListItems(IEnumerable<Vehicle> vehicles)
        {
            List<VehicleListItem> list = new List<VehicleListItem>();

            foreach (Vehicle vehicle in vehicles)
            {
                VehicleListItem item = new VehicleListItem();
                item.VehicleName = vehicle.Name;
                item.VehicleID = vehicle.Id;
                StringBuilder sb = new StringBuilder();
                foreach (VehicleOperationalMode mode in vehicle.Modes)
                {
                    foreach (VehicleModePowerPlant plant in mode.Plants)
                    {
                        foreach (InputResourceReference fuel in plant.FuelUsed)
                        {
                            sb.Append(ResourcesData[fuel.ResourceId].Name + ", ");
                        }
                    }
                }
                if (sb.Length > 2)
                    sb.Remove(sb.Length - 2, 2);
                item.FuelsUsed = sb.ToString();
                list.Add(item);
            }

            return list;
        }

        // lzf: Add VehicleModeList
        public List<VehicleOperationalModeListItem> CreateVehicleOperationalModeListItems(IEnumerable<Vehicle> vehicles)
        {
            List<VehicleOperationalModeListItem> list = new List<VehicleOperationalModeListItem>();
            foreach (Vehicle vehicle in vehicles)
            {
                VehicleOperationalModeListItem item = new VehicleOperationalModeListItem();
                StringBuilder sb = new StringBuilder();
                foreach(VehicleOperationalMode mode in vehicle.Modes)
                {
                    item.VehicleName = vehicle.Name;
                    item.VehicleID = vehicle.Id;
                    item.VehicleModeName=mode.Name;
                    item.VehicleModeID = mode.UniqueId.ToString();
                    foreach (VehicleModePowerPlant plant in mode.Plants)
                        foreach(InputResourceReference vf in plant.FuelUsed)
                            sb.Append(ResourcesData[vf.ResourceId].Name + ", ");
                    if (sb.Length > 2)
                        sb.Remove(sb.Length - 2, 2);
                    item.FuelsUsed = sb.ToString();
                    list.Add(item);
                }                
            }
            return list;  
        }

        // lzf: Add VehicleModePlantList
        public List<VehicleModePowerPlantListItem> CreateVehicleModePowerPlantListItems(IEnumerable<Vehicle> vehicles)
        {
            List<VehicleModePowerPlantListItem> list = new List<VehicleModePowerPlantListItem>();
            foreach (Vehicle vehicle in vehicles)
            { 
                foreach (VehicleOperationalMode mode in vehicle.Modes)
                {
                    foreach (VehicleModePowerPlant plant in mode.Plants)
                    {
                        VehicleModePowerPlantListItem item = new VehicleModePowerPlantListItem();
                        StringBuilder sb = new StringBuilder();
                        item.VehicleName = vehicle.Name;
                        item.VehicleID = vehicle.Id;
                        item.VehicleModeName = mode.Name;
                        item.VehicleModeID = mode.UniqueId.ToString();
                        item.VehicleModePlantName = plant.Name;
                        item.VehicleModePlantID = plant.UniqueID.ToString();                        
                        foreach (InputResourceReference vf in plant.FuelUsed)
                            sb.Append(ResourcesData[vf.ResourceId].Name + ", ");
                        if (sb.Length > 2)
                            sb.Remove(sb.Length - 2, 2);
                        item.FuelsUsed = sb.ToString();
                        list.Add(item);
                    }  
                }
            }
            return list;
        }

        /// <summary>
        /// Removes unconnected edges in pathways.
        /// </summary>
        /// <param name="pathways">List of pathways to be checked. If null, all pathways in the dataset will be checked</param>
        /// <returns>True if error was detected in a pathway</returns>
        public bool CleanPathways(List<Pathway> pathways = null)
        {
            if (pathways == null)
                pathways = PathwaysData.Values.ToList();

            bool issueFound = true;
            foreach (Pathway p in pathways.Where(p => p.Discarded == false))
            { 
                while (issueFound)
                {
                    issueFound = false;

                    //check that all edges are connected on both sides, otherwise delete them
                    List<Edge> toRemoveE = new List<Edge>();
                    foreach (Edge e in p.EdgesData)
                    {
                        if (p.VerticesData.ContainsKey(e.OutputVertexID) && (p.VerticesData.ContainsKey(e.InputVertexID) || p.OutputsData.Any(item => item.Id == e.InputVertexID)))
                            continue; //this edge is connected on both sides
                        toRemoveE.Add(e);
                    }

                    foreach (Edge e in toRemoveE)
                    {
                        p.EdgesData.Remove(e);
                        issueFound = true;
                    }

                    //check that each vertex is connected for all i/o that needs a connection otherwise delete them
                    List<Vertex> toRemoveV = new List<Vertex>();
                    foreach (Vertex v in p.VerticesData.Values)
                    {
                        if (p.EdgesData.Any(e => e.OutputVertexID == v.ID))
                            continue; //this vertex output is connected to something
                        toRemoveV.Add(v);
                    }

                    foreach (Vertex v in toRemoveV)
                    {
                        p.VerticesData.Remove(v.ID);
                        issueFound = true;
                    }

                    //check that each output is connected to something
                    List<PMOutput> toRemoveO = new List<PMOutput>();
                    foreach (PMOutput o in p.OutputsData)
                    {
                        if (p.EdgesData.Any(e => e.InputVertexID == o.Id))
                            continue; //there is a connection to that output
                        toRemoveO.Add(o);
                    }

                    foreach (PMOutput o in toRemoveO)
                    {
                        p.OutputsData.Remove(o);
                        issueFound = true;
                    }
                }
            }
            return issueFound;
        }

        #endregion methods

        #region accessors

        public Mixes MixesData
        {
            get { return _mixes; }
        }

        public Processes ProcessesData
        {
            get { return _processes; }
        }

        public EmissionGases GasesData
        {
            get { return _emissionGases; }
        }

        public Locations LocationsData
        {
            get { return _locations; }
        }

        public Resources ResourcesData
        {
            get { return _resources; }
        }

        public Pathways PathwaysData
        {
            get { return _pathways; }
        }

        public Technologies TechnologiesData
        {
            get { return _technologies; }
        }

        public Modes ModesData
        {
            get { return _modes; }
        }

        public InputTables InputsData
        {
            get { return _inputs; }
        }

        public Vehicles VehiclesData
        {
            get { return _vehicles; }
        }

        public Pictures PicturesData
        {
            get { return _pictures; }
            set { _pictures = value; }
        }

        public Parameters ParametersData
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public bool FuelCalculationNeeded
        {
            get { return _fuelCalculationNeeded; }
            set
            {
                _fuelCalculationNeeded = value;
            }
        }

        public bool VehicleCalculationNeeded
        {
            get { return _vehicleCalculationNeeded; }
            set
            {
                _vehicleCalculationNeeded = value;
            }
        }

        public IDataHelper Helper
        {
            get { return _helper; }
        }

        #endregion accessors

        #region IData dictionaries

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<int, IResource> Resources
        {
            get { return _resources; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<int, IMix> Mixes
        {
            get { return _mixes; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<int, ITechnology> Technologies
        {
            get { return _technologies; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<int, IPathway> Pathways
        {
            get { return _pathways; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<int, IProcess> Processes
        {
            get { return _processes; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<int, ILocation> Locations
        {
            get { return _locations; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<int, IGas> Gases
        {
            get { return GasesData; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<int, IAMode> Modes
        {
            get { return _modes; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<int, IVehicle> Vehicles
        {
            get { return _vehicles; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<string, IInputTable> InputTables
        {
            get { return _inputs.Tables; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<string, IPicture> Pictures
        {
            get { return _pictures; }
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IGDataDictionary<string, IParameter> Parameters
        {
            get { return _parameters; }
        }
       
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IGroup> ResourceGroups
        {
            get
            {
                return ResourcesData.Groups.Values.ToList<IGroup>();
            }
        }
        
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IGroup> GasGroups
        {
            get
            {
                return GasesData.Groups.Values.ToList<IGroup>();
            }
        }

        #endregion    
    
       
    }

 
}
