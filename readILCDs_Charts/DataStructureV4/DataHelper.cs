using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.Entities;
using System.Threading.Tasks;
using Greet.UnitLib3;

namespace Greet.DataStructureV4
{
    /// <summary>
    /// <para>A wrapper class for the data class. Simplify the usage of the data class by providing easier entry point to manage data.</para>
    /// <para>The main idea is that by using this wrapper class, the data will be managed and in a consitance state at any time.</para>
    /// <para>It is recomended to use the wrapper class for any operations that are done from External libraries such as Model or Gui. If functionalities
    /// are missing, send a request or inplement a new method here and respect the naming convention and conventional interfaces</para>
    /// </summary>
    public class DataHelper : IDataHelper
    {
        //Reference to the data object
        GData _data = null;

        /// <summary>
        /// Default construsctor, needs a reference to an instance of a GData object
        /// </summary>
        /// <param name="data"></param>
        public DataHelper(GData data)
        {
            _data = data;
        }

        #region factories
        /// <summary>
        /// Creates a new instance of a Pathway WITHOUT adding it to the dataset
        /// </summary>
        /// <param name="name">Optional name for the created pathway</param>
        /// <param name="notes">Notes for the created pathway</param>
        /// <returns>New instance of pathway</returns>
        public IPathway CreateNewPathway(string name, string notes = "")
        {
            Pathway path = new Pathway(_data.PathwaysData.Keys.ToArray<int>());
            if (name != "")
                path.Name = name;
            return path as IPathway;
        }
        /// <summary>
        /// Creates a new instance of a process either stationary or transportation
        /// </summary>
        /// <param name="type">0 for stationary, 1 for transportation</param>
        /// <param name="name">Optional name for the newly created process</param>
        /// /// <param name="notes">Notes to be set for that item</param>
        /// <returns>Null if type is not 0 nor 1, otherwise an instance of a new Process</returns>
        public IProcess CreateNewProcess(int type, string name, string notes = "")
        {
            AProcess process = null;
            if (type == 0)
                process = new StationaryProcess(_data);
            else if (type == 1)
                process = new TransportationProcess(_data);
            else
                return null;

            if (name != "")
                process.Name = name;
            process.Notes = notes;

            return process as IProcess;
        }
        /// <summary>
        /// Creates a new input if resource ID exists in the database, returns null otherwise
        /// </summary>
        /// <param name="resourceId">ID of an existing resource</param>
        /// <param name="errorMessage">Errors or warning message, can be used to diagnose issues</param>
        /// <param name="amount">Design amount for the input</param>
        /// <param name="quantity">Quantity for the design amount</param>
        /// <param name="source">Source of the input, 0=Well, 1=Previous, 2=Pathway, 3=Mix</param>
        /// <param name="pathOrMix">ID of the pathway of mix if source is set to Pathway or Mix</param>
        /// <returns>New instance of Input or null</returns>
        public IInput CreateNewInput(int resourceId, double amount = 0, string quantity = "joules", int source = 0, int pathOrMix = -1)
        {
            Input inp = new Input();

            if (!_data.ResourcesData.ContainsKey(resourceId))
                throw new Exceptions.IDNotFoundInDatabase("The resource ID does not exists in the database");
            inp.resourceId = resourceId;

            ParameterTS param = new ParameterTS(_data, quantity, amount);
            if (param.CurrentValue == null)
                throw new Exception("Error during parameter creation");
            if (Units.QuantityList.ByDim(param.CurrentValue.Dim) == null)
                throw new Exception("The given unit was not part of the current unit system");
            inp.DesignAmount = param;

            switch (source)
            {
                case 0:
                    inp.SourceType = Enumerators.SourceType.Well;
                    break;
                case 1:
                    inp.SourceType = Enumerators.SourceType.Previous;
                    break;
                case 2:
                    inp.SourceType = Enumerators.SourceType.Pathway;
                    if (!_data.PathwaysData.ContainsKey(pathOrMix))
                        throw new Exceptions.IDNotFoundInDatabase("The given pathOrMix ID does not exists in the Pathways dataset");
                    inp.SourceMixOrPathwayID = pathOrMix;
                    break;
                case 3:
                    inp.SourceType = Enumerators.SourceType.Mix;
                    if (!_data.MixesData.ContainsKey(pathOrMix))
                        throw new Exceptions.IDNotFoundInDatabase("The given pathOrMix ID does not exists in the Mix dataset");
                    inp.SourceMixOrPathwayID = pathOrMix;
                    break;
            }
            return inp;
        }
        /// <summary>
        /// Creates a new output if the resource ID exists in the database, returns null otherwise
        /// </summary>
        /// <param name="resourceId">ID of an existing resource</param>
        /// <param name="amount">Design amount for the output</param>
        /// <param name="preferedUnitExpression">Prefered unit expression like J for joules</param>
        /// <returns>New instance of Output or null</returns>
        public IIO CreateNewMainOutput(int resourceId, double amount = 0, string preferedUnitExpression = "J")
        {
            if (!_data.ResourcesData.ContainsKey(resourceId))
                return null;

            MainOutput outp = new MainOutput();
            outp.DesignAmount = new ParameterTS(_data, preferedUnitExpression, amount);
            outp.ResourceId = resourceId;

            return outp as IIO;
        }
        /// <summary>
        /// Creates a new output if the resource ID exists in the database, returns null otherwise
        /// </summary>
        /// <param name="resourceId">ID of an existing resource</param>
        /// <param name="amount">Design amount for the output</param>
        /// <param name="preferedUnitExpression">Prefered unit expression like J for joules</param>
        /// <returns>New instance of Output or null</returns>
        public IIO CreateNewCoProduct(int resourceId, double amount = 0, string preferedUnitExpression = "J")
        {
            if (!_data.ResourcesData.ContainsKey(resourceId))
                return null;

            CoProduct outp = new CoProduct(_data, resourceId, new ParameterTS(_data, preferedUnitExpression, amount));

            return outp as IIO;
        }
        /// <summary>
        /// Creates a new instance of a resource given a name for that resource
        /// </summary>
        /// <param name="name">Optional name for the resource</param>
        /// <param name="notes">Notes to be set for that item</param>
        /// <returns>New instance of the created resource</returns>
        public IResource CreateNewResource(string name, string notes = "")
        {
            ResourceData resource = new ResourceData(_data);
            if (name != "")
                resource.Name = name;
            else
                resource.Name = "Resouce " + resource.Id;
            resource.Notes = notes;
            return resource as IResource;
        }
        /// <summary>
        /// <para>Creates a new instance of a transportation step for use in a transportation process.</para>
        /// <para>May throw exceptions in case of unknown mode ID</para>
        /// </summary>
        /// <param name="modeId">The ID of the mode to be used for that step</param>
        /// <param name="share">0-1 The mass share of the thoughput transported resource</param>
        /// <param name="m">In meters, the distance for that step</param>
        /// <param name="errorMessage">Potential problems during the construction of the new step</param>
        /// <param name="urbanShare">The urban share for the emissions generated by that step</param>
        /// <param name="processFuel">The process fuel selection, 1 will use the default fuel share</param>
        /// <returns>Newly created transportation step or null</returns>
        public ITransportationStep CreateNewTStep(int modeId, double share, double distance, double urbanShare = 0, int processFuel = 1)
        {
            TransportationStep step = new TransportationStep(_data);
            if (!_data.ModesData.ContainsKey(modeId))
                throw new Exceptions.IDNotFoundInDatabase("The mode ID provided does not exists in the database");
            step.modeReference = modeId;
            if (share < 0 || share > 1)
                throw new Exceptions.ValueIncorrect("The mode share must be in the 0-1 range");
            step.Share.CurrentValue.ValueInDefaultUnit = share;
            if (distance <= 0)
                throw new Exceptions.ValueIncorrect("The distance must be superior than 0");
            step.Distance.CurrentValue.ValueInDefaultUnit = distance;
            if (urbanShare < 0 || urbanShare > 1)
                throw new Exceptions.ValueIncorrect("The mode urban share must be in the 0-1 range");
            step.UrbanShare.CurrentValue.ValueInDefaultUnit = urbanShare;
            if(!_data.ModesData[modeId].FuelSharesData.Any(item => item.Key == processFuel))
                throw new Exceptions.IDNotFoundInDatabase("The process fuel ID given for the mode fuel share is not in the database for the given mode");
            step.TransportationProcessReferenceId = processFuel;

            return step as ITransportationStep;          
        }
        /// <summary>
        /// Creates a new instance of a mix with a given name;
        /// </summary>
        /// <param name="name">Name for the mix to be created</param>
        /// <param name="notes">Notes to be set for that item</param>
        /// <returns>Instance of a mix</returns>
        public IMix CreateNewMix(string name, string notes = "")
        {
            Mix mix = new Mix(_data);
            mix.Name = name;
            mix.Notes = notes;
            return mix as IMix;
        }   
        /// <summary>
        /// Creates a new instance of a technology for use in a process inptut or transportation mode
        /// </summary>
        /// <param name="name">Desired name for the new technology</param>
        /// <param name="notes">Notes to be set for that item</param>
        /// <returns>Newly created technology</returns>
        public ITechnology CreateNewTechnology(string name, string notes = "")
        {
            TechnologyData tech = new TechnologyData(_data);
            tech.Name = name;
            tech.Notes = notes;
            return tech as ITechnology;
        }
        /// <summary>
        /// Creates a new pollutant that can be used for technology emission factors or other emissions
        /// </summary>
        /// <param name="name">Name of the pollutant</param>
        /// <param name="notes">Notes associated with that pollutant</param>
        /// <returns>Instance of a new pollutant</returns>
        public IGas CreateNewPollutant(string name, string notes = "")
        {
            Gas pol = new Gas(_data);
            pol.Name = name;
            pol.Notes = notes;
            return pol as IGas;
        }
        /// <summary>
        /// Creates a new instance of a transportation mode based on a certain type
        /// </summary>
        /// <param name="type">1=Tanker, 2=Truck, 3=Pipeline, 4=Rail, 5=Magic</param>
        /// <param name="name">Name for the new mode</param>
        /// <param name="notes">Notes associated with the mode</param>
        /// <returns>The newly created mode</returns>
        public IAMode CreateNewMode(int type, string name, string notes = "")
        {
            AMode mode;
            if (type == 1)
                mode = new ModeTankerBarge(_data);
            else if (type == 2)
                mode = new ModeTruck(_data);
            else if (type == 3)
                mode = new ModePipeline(_data);
            else if (type == 4)
                mode = new ModeRail(_data);
            else if (type == 5)
                mode = new ModeConnector(_data);
            else
                throw new ArgumentException("Unkown type, cannot create a new Mode");
            mode.Name = name;
            mode.Notes = notes;
            return mode as IAMode;
        }
        #endregion

        #region Data manipulation
        /// <summary>
        /// Tries to insert a pathway in the database, checks for validity before insertion
        /// If a pathway with the same ID is already inserted, then overwrites the existing one with the new pathway given as a parameter
        /// </summary>
        /// <param name="pathway">Pathway to be inserted</param>
        /// <param name="errorMessage">Error messages if any when checking the integrity of that pathway</param>
        /// <returns>True if pathway inserted correctly without any issues</returns>
        public bool DataInsertOrUpdatePathway(IPathway pathway)
        {
            if(pathway == null)
                throw new ArgumentNullException("Given pathway is a null reference");
            if(!(pathway is Pathway))
                throw new Exception("The given pathway is not of type Pathway");

            Pathway path = (Pathway)pathway;
            KeyValuePair<int, string> errors = path.CheckPathwayGlobal(_data);
            string errorMessage = errors.Value;
            if (errors.Key == 0 && errors.Value == "")
            {
                if (!_data.PathwaysData.ContainsKey(path.Id))
                    _data.PathwaysData.Add(path.Id, path);
                else
                    _data.PathwaysData[path.Id] = path;
                return true;
            }
            else
            {
                if (!String.IsNullOrEmpty(errorMessage))
                    throw new Exception(errorMessage);
                return false;
            }
        }
        /// <summary>
        /// Tries to insert a process in the database, checks for validity before insertion
        /// If a process with the same ID is already inserted, then overwrites the existing one with the new process given as a parameter
        /// </summary>
        /// <param name="process">Process to be inserted</param>
        /// <param name="errorMessage">Error messages if any when checking the integrity of that process</param>
        /// <returns></returns>
        public bool DataInsertOrUpdateProcess(IProcess process)
        {
            if(process == null)
                throw new ArgumentNullException("Given process is a null reference");
            if (!(process is AProcess))
                throw new Exception("Given process is not of type AProcess");

            AProcess proc = (AProcess)process;
            string errorMessage;
            if (proc.CheckSpecificIntegrity(_data, true, true, out errorMessage))
            {
                if (!_data.ProcessesData.ContainsKey(proc.Id))
                    _data.ProcessesData.Add(proc.Id, proc);
                else
                    _data.ProcessesData[proc.Id] = proc;
                return true;
            }
            else
            {
                if (!String.IsNullOrEmpty(errorMessage))
                    throw new Exception(errorMessage);
                return false;
            }
        }
        /// <summary>
        /// Inserts the technology to the database or update an existing one that has the same ID
        /// </summary>
        /// <param name="technology">The technology to be inserted</param>
        /// <returns>True if successfully inserted, false otherwise</returns>
        public bool DataInsertOrUpdateTechnology(ITechnology technology)
        {
            if (technology == null)
                throw new ArgumentNullException("Given mix is a null reference");
            if (!(technology is TechnologyData))
                throw new Exception("The given mix is not of type Mix");
            TechnologyData tech = technology as TechnologyData;

            string errorMessage;
            bool isOk = tech.CheckIntegrity(_data, true, out errorMessage);
            if (!isOk || !String.IsNullOrEmpty(errorMessage))
                throw new Exception(errorMessage);
            else
            {
                if (_data.TechnologiesData.ContainsKey(tech.Id))
                    _data.TechnologiesData[tech.Id] = tech;
                else
                    _data.TechnologiesData.Add(tech.Id, tech);
                return true;
            }
        }
        /// <summary>
        /// Removes a process from the database. Will throw an exception if the process is used in any pathway
        /// </summary>
        /// <param name="process">The process to be removed from the database</param>
        /// <param name="force">If true, will simply remove it and will not throw exceptions</param>
        /// <returns>True if removed, false if wasn't in the database</returns>
        public bool DataRemoveProcess(IProcess process, bool force = false)
        {
            if (process == null)
                throw new ArgumentNullException("Given process is a null reference");
            if (!(process is AProcess))
                throw new Exception("The given process is not of type AProcess");
            AProcess proc = process as AProcess;
            if (!_data.ProcessesData.ContainsKey(process.Id))
                return false;
            List<IDependentItem> dependentItems = ProcessDependentItems(new DependentItem(proc));
            if (dependentItems.Count > 0 && !force)
                throw new Exception("Items depend on that process, therefore it cannot be deleted");
            _data.ProcessesData.Remove(process.Id);
            return true;
        }
        /// <summary>
        /// Removes a pathway from the database. Will throw an exception if the pathway is used in any pathway or mix
        /// </summary>
        /// <param name="process">The pathway to be removed from the database</param>
        /// <param name="force">If true, will simply remove it and will not throw exceptions</param>
        /// <returns>True if removed, false if wasn't in the database</returns>
        public bool DataRemovePathway(IPathway pathway, bool force = false)
        {
            if (pathway == null)
                throw new ArgumentNullException("Given pathway is a null reference");
            if (!(pathway is Pathway))
                throw new Exception("The given pathway is not of type Pathway");
            Pathway path = pathway as Pathway;
            if (!_data.PathwaysData.ContainsKey(pathway.Id))
                return false;
            List<IDependentItem> dependentItems = PathwayDependentItems(new DependentItem(path));
            if (dependentItems.Count > 0 && !force)
                throw new Exception("Items depend on that pathway, therefore it cannot be deleted");
            _data.PathwaysData.Remove(pathway.Id);
            return true;
        }
        /// <summary>
        /// Removes a resource from the database. Will throw an exception if the resource is used anywhere
        /// </summary>
        /// <param name="process">The resource to be removed from the database</param>
        /// <param name="force">If true, will simply remove it and will not throw exceptions</param>
        /// <returns>True if removed, false if wasn't in the database</returns>
        public bool DataremoveResource(IResource resource, bool force = false)
        {
            if (resource == null)
                throw new ArgumentNullException("Given resource is a null reference");
            if (!(resource is ResourceData))
                throw new Exception("The given resource is not of type Resource");
            ResourceData res = resource as ResourceData;
            if (!_data.ResourcesData.ContainsKey(res.Id))
                return false;
            List<IDependentItem> dependentItems = ResourceDependentItems(new DependentItem(res));
            if (dependentItems.Count > 0 && !force)
                throw new Exception("Items depend on that resource, therefore it cannot be deleted");
            _data.ResourcesData.Remove(res.Id);
            return true;
        }
        /// <summary>
        /// Removes a mix from the database. Will throw an exception if the mix is used in any pathway or mix
        /// </summary>
        /// <param name="process">The mix to be removed from the database</param>
        /// <param name="force">If true, will simply remove it and will not throw exceptions</param>
        /// <returns>True if removed, false if wasn't in the database</returns>
        public bool DataRemoveMix(IMix mix, bool force = false)
        {
            if (mix == null)
                throw new ArgumentNullException("Given mix is a null reference");
            if (!(mix is Mix))
                throw new Exception("The given mix is not of type Mix");
            Mix mixx = mix as Mix;
            if (!_data.MixesData.ContainsKey(mixx.Id))
                return false;
            List<IDependentItem> dependentItems = MixDependentItems(new DependentItem(mixx));
            if (dependentItems.Count > 0 && !force)
                throw new Exception("Items depend on that mix, therefore it cannot be deleted");
            _data.MixesData.Remove(mixx.Id);
            return true;
        }
        /// <summary>
        /// Removes a technology from the database. Will throw an exception if the technology is used in any process or transportation mode
        /// </summary>
        /// <param name="process">The technology to be removed from the database</param>
        /// <param name="force">If true, will simply remove it and will not throw exceptions</param>
        /// <returns>True if removed, false if wasn't in the database</returns>
        public bool DataRemoveTechnology(ITechnology technology, bool force = false)
        {
            if (technology == null)
                throw new ArgumentNullException("Given technology is a null reference");
            if (!(technology is TechnologyData))
                throw new Exception("The given technology is not of type Technology");
            TechnologyData tech = technology as TechnologyData;
            if (!_data.TechnologiesData.ContainsKey(tech.Id))
                return false;
            List<IDependentItem> dependentItems = TechnologyDependentItems(new DependentItem(tech));
            if (dependentItems.Count > 0 && !force)
                throw new Exception("Items depend on that technology, therefore it cannot be deleted");
            _data.TechnologiesData.Remove(tech.Id);
            return true;
        }
        /// <summary>
        /// Inserts a new resource in the database or replace the one with the same ID if it already exists
        /// </summary>
        /// <param name="resource">The new resource to be added or used as an update</param>
        /// <returns>True if insertion was done</returns>
        public bool DataInsertOrUpdateResource(IResource resource)
        {
            if (resource == null)
                throw new ArgumentNullException("The resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("The given resource is not an instance of type Resource");
            ResourceData rd = resource as ResourceData;
            string errorMessage;
            bool isOk = rd.CheckIntegrity(_data, true, out errorMessage);
            if (!isOk || !String.IsNullOrEmpty(errorMessage))
                throw new Exception(errorMessage);
            else
            {
                if (_data.ResourcesData.ContainsKey(resource.Id))
                    _data.ResourcesData[resource.Id] = resource as ResourceData;
                else
                    _data.ResourcesData.Add(resource.Id, resource as ResourceData);

                return true;
            }
        }
        /// <summary>
        /// Inserts a mix to the database after checking it's integrity
        /// </summary>
        /// <param name="mix">The mix to be inserted to the database</param>
        /// <returns>True if successfully added to the database</returns>
        public bool DataInsertOrUpdateMix(IMix mix)
        {
            if (mix == null)
                throw new ArgumentNullException("Given mix is a null reference");
            if (!(mix is Mix))
                throw new Exception("The given mix is not of type Mix");
            Mix mixx = mix as Mix;

            string errorMessage;
            bool isOk = mixx.CheckIntegrity(_data, true, out errorMessage);
            if (!isOk || !String.IsNullOrEmpty(errorMessage))
                throw new Exception(errorMessage);
            else
            {
                if (_data.MixesData.ContainsKey(mixx.Id))
                    _data.MixesData[mixx.Id] = mixx;
                else
                    _data.MixesData.Add(mixx.Id, mixx);
                return true;
            }
        }
        /// <summary>
        /// Inserts a pollutant to the database after checking it's integrity
        /// </summary>
        /// <param name="pollutant">The pollutant to be inserted</param>
        /// <returns>True if inserted correctly</returns>
        public bool DataInsertOrUpdatePollutant(IGas pollutant)
        {
            if (pollutant == null)
                throw new ArgumentNullException("Given pollutant is a null reference");
            if (!(pollutant is Gas))
                throw new Exception("The given pollutant is not an instance of Gas");

            Gas gas = pollutant as Gas;

            string errorMessage;
            bool isOk = gas.CheckIntegrity(_data, true, out errorMessage);
            if (!isOk || !String.IsNullOrEmpty(errorMessage))
                throw new Exception(errorMessage);
            else
            {
                if (_data.MixesData.ContainsKey(gas.Id))
                    _data.GasesData[gas.Id] = gas;
                else
                    _data.GasesData.Add(gas.Id, gas);
                return true;
            }
        }
        /// <summary>
        /// Inserts a mode in the dataset after checking it's integrity
        /// </summary>
        /// <param name="mode">The mode to be added to the database</param>
        /// <returns>True if added correctly</returns>
        public bool DataInsertOrUpdateMode(IAMode mode)
        {
            if (mode == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(mode is AMode))
                throw new Exception("The given pathway is not of type AMode");

            AMode m = mode as AMode;
            string errors;
            bool pass = m.CheckIntegrity(_data, true, out errors);
            if (String.IsNullOrEmpty(errors))
            {
                if (!_data.ModesData.ContainsKey(m.Id))
                    _data.ModesData.Add(m.Id, m);
                else
                    _data.ModesData[m.Id] = m;
                return true;
            }
            else
                throw new Exception(errors);
        }
        /// <summary>
        /// Adds a new energy source to a fuel share given by the fuelshare id
        /// </summary>
        /// <param name="mode">The mode being modified</param>
        /// <param name="fsId">The fuel share Id beeing modified. Should be set to 1 for the default fuel share</param>
        /// <param name="resourceId">The resource ID of the fuel to be added to that fuel share</param>
        /// <param name="share">The energy share 0-1 to be assigned to that fuel</param>
        /// <param name="pathOrMixId">The pathway or mix ID to be used for the production of the fuel/resource</param>
        /// <param name="source">Source for the pathOrMixID: 2=pathway, 3=mix</param>
        /// <param name="techToId">Technology used for the trip TO destination</param>
        /// <param name="techFromId">Technology used for the trip FROM destination</param>
        /// <returns>True if the resource has been added to the fuel share</returns>
        public bool ModeAddEnergySource(IAMode mode, int fsId, int resourceId, double share, int pathOrMixId, int source, int techToId, int techFromId)
        {
            if (mode == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(mode is AMode))
                throw new Exception("The given mode is not of type AMode");
            AMode m = mode as AMode;

            if (!m.FuelSharesData.ContainsKey(fsId))
                throw new Exception("Mode does not contains a fuel share with the same ID");
            else
            {
                ModeEnergySource mfs = new ModeEnergySource(_data);
                Enumerators.SourceType srcType;
                if (source == 2)
                    srcType = Enumerators.SourceType.Pathway;
                else if (source == 3)
                    srcType = Enumerators.SourceType.Mix;
                else
                    throw new Exception("Source type must be 2 for pathway or 3 for mix");
                mfs.ResourceReference = new InputResourceReference(resourceId, pathOrMixId, srcType);
                mfs.Share.ValueInDefaultUnit = share;
                mfs.TechnologyFrom = techFromId;
                mfs.TechnologyTo = techToId;
                m.FuelSharesData[fsId].ProcessFuels.Add(mfs.ResourceReference, mfs);
                return true;
            }
        }
        /// <summary>
        /// Inserts a new fuel share for a mode. Throws an exceptions with a fuel share already exists with the same ID
        /// </summary>
        /// <param name="mode">Mode being modified</param>
        /// <param name="fs">Fuel share to insert</param>
        /// <returns>True if inserted to the mode</returns>
        public bool ModeInsertFuelShare(IAMode mode, IModeFuelShares fs)
        {
            if (mode == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(mode is AMode))
                throw new Exception("The given pathway is not of type AMode");

            AMode m = mode as AMode;
            if (m.FuelSharesData.ContainsKey(fs.Id))
                throw new Exception("Another fuel share with the same ID already exists");
            else
            {
                m.FuelSharesData.Add(fs.Id, fs as ModeFuelShares);
                return true;
            }
        }
        /// <summary>
        /// <para>Creates or updates a specific EI for a given resource ID.</para>
        /// <para>For pipeline, resource groups (such as liquid) or individual resources, can have a unique EI for the pipelines.</para>
        /// </summary>
        /// <param name="pipeBased">Pipeline being modified</param>
        /// <param name="id">Resource or Resource Group ID</param>
        /// <param name="unit">Unit for the EO, typically energy/distance/mass</param>
        /// <param name="value">Value of the EI in the given energy</param>
        /// <returns>True if the EI has been set for the given resource or resource group ID</returns>
        public bool ModePipelineInsertOrUpdateEI(IAMode pipeBased, int id, string unit, int value)
        {
            if (pipeBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(pipeBased is ModePipeline))
                throw new Exception("The given mode is not of type Pipeline");
            ModePipeline pipe = pipeBased as ModePipeline;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            if (pipe.EnergyIntensity.ContainsKey(id))
            {
                pipe.EnergyIntensity[id].EnergyIntensity.CurrentValue.UserValue = temp.ValueInDefaultUnit;
                pipe.EnergyIntensity[id].EnergyIntensity.CurrentValue.UserDim = temp.Dim;
                pipe.EnergyIntensity[id].EnergyIntensity.CurrentValue.UseOriginal = false;
            }
            else
            {
                PipelineMaterialTransported matT = new PipelineMaterialTransported(_data, id);
                matT.EnergyIntensity.CurrentValue.UserValue = temp.ValueInDefaultUnit;
                matT.EnergyIntensity.CurrentValue.UserDim = temp.Dim;
                matT.EnergyIntensity.CurrentValue.UseOriginal = false;
                pipe.EnergyIntensity.Add(id, matT);
            }

            return true;
        }
        /// <summary>
        /// Sets the average speed for a mode based on the tanker/barge model
        /// </summary>
        /// <param name="tankerBased">Tanker based mode being modified</param>
        /// <param name="unit">Unit for the average speed</param>
        /// <param name="p2">Value of the average speed in the given unit</param>
        /// <returns>True if the load factor has been set correctly</returns>
        public bool ModeTankerSetAvgSpd(IAMode tankerBased, string unit, double value)
        {
            if (tankerBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(tankerBased is ModeTankerBarge))
                throw new Exception("The given mode is not of type TankerBarge");
            ModeTankerBarge boat = tankerBased as ModeTankerBarge;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            boat.AverageSpeed.CurrentValue.UserDim = temp.Dim;
            boat.AverageSpeed.CurrentValue.UserValue = temp.ValueInDefaultUnit;
            boat.AverageSpeed.CurrentValue.UseOriginal = false;
            return true;
        }
        /// <summary>
        /// Sets the engine power load factor from for a mode based on the tanker/barge
        /// </summary>
        /// <param name="tankerBased">Tanker based mode being modified</param>
        /// <param name="factor">Engine power load factor 0-1</param>
        /// <returns>True if the load factor has been set correctly</returns>
        public bool ModeTankerSetLoadFactorFrom(IAMode tankerBased, double factor)
        {
            if (tankerBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(tankerBased is ModeTankerBarge))
                throw new Exception("The given mode is not of type TankerBarge");
            ModeTankerBarge boat = tankerBased as ModeTankerBarge;

            boat.LoadFactorFrom.CurrentValue.ValueInDefaultUnit = factor;

            return true;
        }
        /// <summary>
        /// Sets the fuel consumption TO destination for a truck based mode
        /// </summary>
        /// <param name="truckBased">Mode being modified</param>
        /// <param name="unit">Unit of the fuel consumption</param>
        /// <param name="value">Value of the fuel consumption</param>
        /// <returns>True if the fuel consumption TO has been set for the truck based mode</returns>
        public bool ModeTruckSetFuelConsumptionTo(IAMode truckBased, string unit, double value)
        {
            if (truckBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(truckBased is ModeTruck))
                throw new Exception("The given mode is not of type Truck");
            ModeTruck truck = truckBased as ModeTruck;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            truck.FuelEconomyTo.CurrentValue.UserDim = temp.Dim;
            truck.FuelEconomyTo.CurrentValue.UserValue = temp.ValueInDefaultUnit;
            truck.FuelEconomyTo.CurrentValue.UseOriginal = false;

            return true;
        }
        /// <summary>
        /// Inserts or update a payload for the truck. If a payload for the given resource ID is not defined, a new one will be created.
        /// Otherwise the payload associated with the given resource ID will be updated
        /// </summary>
        /// <param name="truckBased">Mode being modified</param>
        /// <param name="resId">Resource ID of transported payload</param>
        /// <param name="unit">Unit for the payload value</param>
        /// <param name="value">Value in the given unit for the payload mass</param>
        /// <returns>True if the payload has been added or updated</returns>
        public bool ModeTruckInsertOrUpdatePayload(IAMode truckBased, int resId, string unit, double value)
        {
            if (truckBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(truckBased is ModeTruck))
                throw new Exception("The given mode is not of type Truck");
            ModeTruck truck = truckBased as ModeTruck;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            truck.FuelEconomyTo.CurrentValue.UserDim = temp.Dim;
            truck.FuelEconomyTo.CurrentValue.UserValue = temp.ValueInDefaultUnit;
            truck.FuelEconomyTo.CurrentValue.UseOriginal = false;

            return true;
        }
        /// <summary>
        /// Set the engine power load factor TO destination for a tanker/barge based mode
        /// </summary>
        /// <param name="tankerBased">The mode being modified</param>
        /// <param name="factor">The power engine load factor</param>
        /// <returns>True if the load factor has been set</returns>
        public bool ModeTankerSetLoadFactorTo(IAMode tankerBased, double factor)
        {
            if (tankerBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(tankerBased is ModeTankerBarge))
                throw new Exception("The given mode is not of type TankerBarge");
            ModeTankerBarge boat = tankerBased as ModeTankerBarge;

            boat.LoadFactorTo.CurrentValue.ValueInDefaultUnit = factor;

            return true;
        }
        /// <summary>
        /// Sets the engine typical fuel consumption for a tanker/barge based mode
        /// </summary>
        /// <param name="tankerBased">The mode being modified</param>
        /// <param name="unit">Unit for the typical fuel consumption</param>
        /// <param name="value">Value of the typical fuel consumption</param>
        /// <returns>True if the fuel consumption has been set</returns>
        public bool ModeTankerSetTypicalFuelConsumption(IAMode tankerBased, string unit, double value)
        {
            if (tankerBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(tankerBased is ModeTankerBarge))
                throw new Exception("The given mode is not of type TankerBarge");
            ModeTankerBarge boat = tankerBased as ModeTankerBarge;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            boat.BrakeSpecificFuelConsumption.CurrentValue.UserDim = temp.Dim;
            boat.BrakeSpecificFuelConsumption.CurrentValue.UserValue = temp.ValueInDefaultUnit;
            boat.BrakeSpecificFuelConsumption.CurrentValue.UseOriginal = false;

            return true;
        }
        /// <summary>
        /// Sets the engine typical horsepower
        /// </summary>
        /// <param name="tankerBased">The mode being modified</param>
        /// <param name="unit">Unit for the engine power</param>
        /// <param name="value">Value for the engine power</param>
        /// <returns>True if the engine power has been set</returns>
        public bool ModeTankerSetTypicalHP(IAMode tankerBased, string unit, double value)
        {
            if (tankerBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(tankerBased is ModeTankerBarge))
                throw new Exception("The given mode is not of type TankerBarge");
            ModeTankerBarge boat = tankerBased as ModeTankerBarge;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            boat.TypicalHPRequirement.CurrentValue.UserDim = temp.Dim;
            boat.TypicalHPRequirement.CurrentValue.UserValue = temp.ValueInDefaultUnit;
            boat.TypicalHPRequirement.CurrentValue.UseOriginal = false;

            return true;
        }
        /// <summary>
        /// Sets the HP/Payload factor for the ocean tanker which adjusts the engine power requirement according to the payload
        /// </summary>
        /// <param name="tankerBased">The mode being modified</param>
        /// <param name="unit">Unit of the HP/Payload factor</param>
        /// <param name="value">Value of the factor</param>
        /// <returns>True if the HP/Payload factor has been set</returns>
        public bool ModeTankerSetHPPayloadFactor(IAMode tankerBased, string unit, double value)
        {
            if (tankerBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(tankerBased is ModeTankerBarge))
                throw new Exception("The given mode is not of type TankerBarge");
            ModeTankerBarge boat = tankerBased as ModeTankerBarge;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            boat.HpPayloadFactor.CurrentValue.UserDim = temp.Dim;
            boat.HpPayloadFactor.CurrentValue.UserValue = temp.ValueInDefaultUnit;
            boat.HpPayloadFactor.CurrentValue.UseOriginal = false;

            return true;
        }
        /// <summary>
        /// Inserts or Update a payload for the Tanker/Barge mode. Creates a new one if the resource id isn't there alread
        /// or update an existing payload for a given resource Id
        /// </summary>
        /// <param name="tankerBased">The mode being modified</param>
        /// <param name="resId">Resource ID for the transported payload</param>
        /// <param name="unit">Unit of mass for the payload</param>
        /// <param name="value">Value of the payload in the given unit</param>
        /// <returns>True if the payload has been set or added</returns>
        public bool ModeTankerInsertOrUpdatePayload(IAMode tankerBased, int resId, string unit, double value)
        {
            if (tankerBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(tankerBased is ModeTankerBarge))
                throw new Exception("The given mode is not of type TankerBarge");
            ModeTankerBarge boat = tankerBased as ModeTankerBarge;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            if (boat.Payload.ContainsKey(resId))
            {
                boat.Payload[resId].Payload.UserValue = temp.ValueInDefaultUnit;
                boat.Payload[resId].Payload.UserDim = temp.Dim;
                boat.Payload[resId].Payload.UseOriginal = false;
            }
            else
            {
                MaterialTransportedPayload p = new MaterialTransportedPayload(_data, resId);
                p.Payload.UserValue = temp.ValueInDefaultUnit;
                p.Payload.UserDim = temp.Dim;
                p.Payload.UseOriginal = false;
                boat.Payload.Add(resId, p);
                return true;
            }

            return true;
        }
        /// <summary>
        /// Sets fuel consumption from destination
        /// </summary>
        /// <param name="truckBased">The mode being modified</param>
        /// <param name="unit">Unit of the fuel consumption value</param>
        /// <param name="value">Value of the fuel consumption</param>
        /// <returns>True if the fuel consumption has been set</returns>
        public bool ModeTruckSetFuelConsumptionFrom(IAMode truckBased, string unit, double value)
        {
            if (truckBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(truckBased is ModeTruck))
                throw new Exception("The given mode is not of type Truck");
            ModeTruck truck = truckBased as ModeTruck;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            truck.FuelEconomyFrom.CurrentValue.UserDim = temp.Dim;
            truck.FuelEconomyFrom.CurrentValue.UserValue = temp.ValueInDefaultUnit;
            truck.FuelEconomyFrom.CurrentValue.UseOriginal = false;

            return true;
        }
        /// <summary>
        /// Sets the average speed for a Rail-based mode
        /// </summary>
        /// <param name="railBased">The mode being modified</param>
        /// <param name="unit">Unit of the average speed value</param>
        /// <param name="value">Average speed value for the rail-based mode</param>
        /// <returns>True if the average speed has been set</returns>
        public bool ModeRailSetAvgSpd(IAMode railBased, string unit, double value)
        {
            if (railBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(railBased is ModeRail))
                throw new Exception("The given mode is not of type ModeRail");
            ModeRail rail = railBased as ModeRail;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            rail.AverageSpeed.CurrentValue.UserDim = temp.Dim;
            rail.AverageSpeed.CurrentValue.UserValue = temp.ValueInDefaultUnit;
            rail.AverageSpeed.CurrentValue.UseOriginal = false;

            return true;
        }
        /// <summary>
        /// Sets the EI for a Rail-based mode. If set with a resource ID, 
        /// </summary>
        /// <param name="railBased">The mode being modified</param>
        /// <param name="resOrGrpId">Resource Id or Resource Group Id</param>
        /// <param name="unit">The unit for the average speed value</param>
        /// <param name="value">The average speed value in the given unit</param>
        /// <returns>True if the average speed has been set</returns>
        public bool ModeRailSetEI(IAMode railBased, int resOrGrpId, string unit, double value)
        {
            if (railBased == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(railBased is ModeRail))
                throw new Exception("The given mode is not of type ModeRail");
            ModeRail rail = railBased as ModeRail;

            Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
            rail.Ei.CurrentValue.UserDim = temp.Dim;
            rail.Ei.CurrentValue.UserValue = temp.ValueInDefaultUnit;
            rail.Ei.CurrentValue.UseOriginal = false;

            return true;
        }
        /// <summary>
        /// Creates a new instance of a mode fuel share for use in a mode
        /// </summary>
        /// <param name="mode">The mode to which we desire to add a new fuel share</param>
        /// <param name="name">The name for the new fuel share</param>
        /// <param name="notes">Notes associated with the new fuel share</param>
        /// <returns>A reference to the newly created instance of a mode fuel share</returns>
        public IModeFuelShares ModeCreateNewFuelShare(IAMode mode, string name, string notes = "")
        {
            if (mode == null)
                throw new ArgumentNullException("Given mode is a null reference");
            if (!(mode is AMode))
                throw new Exception("The given pathway is not of type AMode");

            AMode m = mode as AMode;

            List<int> ids = new List<int>();
            foreach (ModeFuelShares shr in m.FuelShares)
                ids.Add(shr.Id);
            int newId = Convenience.IDs.GetIdUnusedFromTimeStamp(ids);

            ModeFuelShares mfs = new ModeFuelShares();
            mfs.Id = newId;
            mfs.Name = name;
            return mfs as IModeFuelShares;
        }
        /// <summary>
        /// Creates a new vertex within a pathway to use a process model
        /// </summary>
        /// <param name="path">The pathway in which we want to create a new vertex</param>
        /// <param name="proc">The process model that is going to be used for that vertex</param>
        public IVertex PathwayAddModel(IPathway path, IProcess proc)
        {
            if (path == null)
                throw new ArgumentNullException("Given pathway is a null reference");
            if (!(path is Pathway))
                throw new Exception("The given pathway is not of type Pathway");
            if (proc == null)
                throw new ArgumentNullException("Given process is a null reference");
            if (!(proc is AProcess))
                throw new Exception("Given process is not of type AProcess");

            Pathway pathway = path as Pathway;
            Vertex v = new Vertex(proc.Id);
            pathway.VerticesData.Add(v.ID, v);
            return v as Vertex;
        }
        /// <summary>
        /// Creates a connector between an output of a process and an input of a process
        /// </summary>
        /// <param name="path">The pathway that contains the process </param>
        /// <param name="origin">The vertex that contains the output (origin of the connector)</param>
        /// <param name="outp">The outptut ID in the model represented by the vertex</param>
        /// <param name="destination">The vertex that contains the input (destination of the connector)</param>
        /// <param name="inp">The input ID in the model represented by the vertex</param>
        /// <returns>True if successfully added, false if a connector with the same connection was already defined in the pathway</returns>
        public bool PathwayAddConnector(IPathway path, Guid origin, Guid outp, Guid destination, Guid inp)
        {
            if (path == null)
                throw new ArgumentNullException("Given pathway is a null reference");
            if (!(path is Pathway))
                throw new Exception("The given pathway is not of type Pathway");
            Pathway pathway = path as Pathway;
            if (!pathway.VerticesData.ContainsKey(origin))
                throw new Exceptions.InvalidParameterReferenceException("The GUID given for the origin process is not a vertex in the given pathway");
            if (!pathway.VerticesData.ContainsKey(destination))
                throw new Exceptions.InvalidParameterReferenceException("The GUID given for the destination process is not a vertex in the given pathway");

            foreach (Edge e in path.Edges)
                if (e.InputID == inp && e.InputVertexID == destination && e.OutputID == outp && e.OutputVertexID == origin)
                    return false; //prevent adding twice the same edge

            Edge ed = new Edge(origin, outp, destination, inp);
            pathway.EdgesData.Add(ed);
            return true;
        }
        /// <summary>
        /// Creates an output for a pathway that can be connected to the output of a process and expose an upstream outside of the pathway
        /// </summary>
        /// <param name="pathway">The pathway that we want to modify by adding an output</param>
        /// <param name="resourceId">The resource ID for the output that will be created</param>
        /// <returns>Instance of the PMOutput created for this pathway</returns>
        public IIO PathwayCreateOutput(IPathway pathway, int resourceId)
        {
            if (pathway == null)
                throw new ArgumentNullException("Given pathway is a null reference");
            if (!(pathway is Pathway))
                throw new Exception("The given pathway is not of type Pathway");
            if (!_data.ResourcesData.ContainsKey(resourceId))
                throw new Exceptions.IDNotFoundInDatabase("The given resource ID cannot be found in the database");
            Pathway path = pathway as Pathway;

            PMOutput pmout = new PMOutput();
            pmout.ResourceId = resourceId;
            path.OutputsData.Add(pmout);
            return pmout as IIO;
        }
        /// <summary>
        /// Creates a connector between the output of a process and the output of a pathway
        /// </summary>
        /// <param name="pathway">The pathway being modified</param>
        /// <param name="origin">>The vertex ID that contains the output (origin of the connector)</param>
        /// <param name="outp">The outptut ID in the model represented by the vertex</param>
        /// <param name="pathOut">The pathway output to which we want to connect the process output (destination of the connector)</param>
        /// <returns>True if added successfully</returns>
        public bool PathwayAddConnector(IPathway pathway, Guid origin, Guid outp, Guid pathOut)
        {
            if (pathway == null)
                throw new ArgumentNullException("Given pathway is a null reference");
            if (!(pathway is Pathway))
                throw new Exception("The given pathway is not of type Pathway");
            Pathway path = pathway as Pathway;
            if (!path.OutputsData.Any(item => item.Id == pathOut))
                throw new Exceptions.InvalidParameterReferenceException("The given pathway output ID cannot be found in the pathway");

            foreach (Edge e in path.Edges)
                if (e.InputID == pathOut && e.InputVertexID == pathOut && e.OutputID == outp && e.OutputVertexID == origin)
                    return false; //prevent adding twice the same edge

            Edge ed = new Edge(origin, outp, pathOut, pathOut);
            path.EdgesData.Add(ed);
            return true;
        }
        /// <summary>
        /// Sets which output is the main output of the pathway
        /// </summary>
        /// <param name="pathway">The pathway to be modified</param>
        /// <param name="pathOut">The pathway output to be used as main output</param>
        public bool PathwaySetMainOutput(IPathway pathway, Guid pathOut)
        {
            if (pathway == null)
                throw new ArgumentNullException("Given pathway is a null reference");
            if (!(pathway is Pathway))
                throw new Exception("The given pathway is not of type Pathway");
            Pathway path = pathway as Pathway;
            List<IDependentItem> dependentItems = PathwayDependentItems(new DependentItem(path));
            if (dependentItems.Count > 0)
                throw new Exception("Items depend on that pathway, therefore the main output cannot be changed");
            if (!path.OutputsData.Any(item => item.Id == pathOut))
                throw new Exceptions.InvalidParameterReferenceException("The given pathway output ID cannot be found in the pathway");

            path.MainOutput = pathOut;
            return true;
        }
        /// <summary>
        /// Sets the carbon ratio of a pollutant to a certain value
        /// </summary>
        /// <param name="pollutant">The pollutant being modified</param>
        /// <param name="value">The mass ratio of Carbon in the pollutant</param>
        /// <returns>True if the value has been set properly</returns>
        public bool PollutantSetCRatio(IGas pollutant, double value)
        {
            if (pollutant == null)
                throw new ArgumentNullException("Given pollutant is a null reference");
            if (!(pollutant is Gas))
                throw new Exception("The given pollutant is not an instance of Gas");
            Gas gas = pollutant as Gas;
            gas.CarbonRatio.ValueInDefaultUnit = value;
            return true;
        }
        /// <summary>
        /// Sets the sulfur ratio of a pollutant to a certain value
        /// </summary>
        /// <param name="pollutant">The pollutant being modified</param>
        /// <param name="value">The mass ratio of Sulfur in the pollutant</param>
        /// <returns>True if the value has been set properly</returns>
        public bool PollutantSetSRatio(IGas pollutant, double value)
        {
            if (pollutant == null)
                throw new ArgumentNullException("Given pollutant is a null reference");
            if (!(pollutant is Gas))
                throw new Exception("The given pollutant is not an instance of Gas");
            Gas gas = pollutant as Gas;
            gas.SulfurRatio.ValueInDefaultUnit = value;
            return true;
        }
        /// <summary>
        /// Sets the global warming potential of a pollutant to a certain value
        /// </summary>
        /// <param name="pollutant">The pollutant being modified</param>
        /// <param name="value">The global warming potential relatively to CO2</param>
        /// <returns>True if the value has been set properly</returns>
        public bool PollutantSetGWP100(IGas pollutant, double value)
        {
            if (pollutant == null)
                throw new ArgumentNullException("Given pollutant is a null reference");
            if (!(pollutant is Gas))
                throw new Exception("The given pollutant is not an instance of Gas");
            Gas gas = pollutant as Gas;
            gas.GlobalWarmingPotential100.ValueInDefaultUnit = value;
            return true;
        }
        /// <summary>
        /// Inserts an input to a StationaryProcess only if the Input is either and InputWithShare
        /// or if the input is an Input.
        /// </summary>
        /// <param name="process">Stationary process to which the input is going to be added</param>
        /// <param name="input">The instance of an input to add to the process</param>
        /// <param name="toGroup">If input is an instance on Input, setting to true will add in process Group</param>
        /// <param name="errorMessage">Error message for insertion or errors detected in the process integrity after insertion</param>
        /// <param name="autoCarbonEstimate">If true the carbon relations between inputs and outputs will be automatically adjusted</param>
        /// <returns>True if added to the process, false otherwise</returns>
        public bool ProcessAddInput(IProcess process, IInput input, bool toGroup, bool autoCarbonEstimate = true)
        {
            if(process == null)
                throw new ArgumentNullException("given process is a null reference");
            if(input == null)
                throw new ArgumentNullException("given input is a null reference");

            if (process is StationaryProcess)
            {
                StationaryProcess proc = (StationaryProcess)process;
                string errorMessage;
                if (proc.CheckSpecificIntegrity(_data, true, true, out errorMessage))
                {//if integrity is ok we try to add
                    if (input is InputWithShare)
                    {
                        if (proc.Group == null)
                            proc.Group = new StationaryProcessGroup();

                        proc.Group.Shares.Add(input as InputWithShare);
                    }
                    else if (input is Input)
                    {
                        if (!toGroup)
                        {
                            if (proc.OtherInputs == null)
                                proc.OtherInputs = new List<Input>();

                            proc.OtherInputs.Add(input as Input);
                        }
                        else
                        {
                            if (proc.Group == null)
                                proc.Group = new StationaryProcessGroup();

                            proc.Group.Inputs.Add(input as Input);
                        }
                    }
                    else
                        throw new Exception("Input isn't an instance of the class InputWithShare nor Input");
                }
                else
                {
                    if (!string.IsNullOrEmpty(errorMessage))
                        throw new Exception("Specific Integrity of the process is not valid, therefore nothing was modified", new Exception(errorMessage));
                    return false;
                }

                if (autoCarbonEstimate)
                    if ((process as StationaryProcess).MainOutput != null)//avoid throwing the exception in the carbon matrix defaults
                        (process as StationaryProcess).CarbonTransMatrix = (process as StationaryProcess).CarbonMatrixDefaults(_data);
                else
                    (process as StationaryProcess).CompleteMatrix((process as StationaryProcess).CarbonTransMatrix);
                return true;
            }
            else
                throw new Exception("Process isn't an instance of the class StationaryProcess");
        }
        /// <summary>
        /// <para>If the output is a MainOutput, sets or update the existing main output of the process</para>
        /// <para>If the output is a CoProduct, adds or update an existing co-product that outputs the same resource ID</para>
        /// </summary>
        /// <param name="process">Process to which we want to add an output(Main output or co-product)</param>
        /// <param name="output">Output to be added to the process</param>
        /// <param name="errorMessage">Error messages if any errors</param>
        /// <param name="autoCarbonEstimate">If true the carbon relations between inputs and outputs will be automatically adjusted</param>
        /// <returns>True if inserted or updated, false otherwise</returns>
        public bool ProcessAddOrUpdateOutput(IProcess process, IIO output, bool autoCarbonEstimate = true)
        {
            if(process == null)
                throw new ArgumentNullException("process is a null reference");
            if (!(process is AProcess))
                throw new Exception("Given process is not of type AProcess");
            if(output == null)
                throw new ArgumentNullException("output is a null reference");

            AProcess proc = (AProcess)process;
            string errorMessage;
            if (proc.CheckSpecificIntegrity(_data, true, true, out errorMessage))
            {//if integrity is ok we try to add
                if (output is MainOutput)
                {
                    if (proc.CheckSpecificIntegrity(_data, true, true, out errorMessage))
                    {
                        proc.MainOutput = output as MainOutput;
                    }
                    else
                        throw new Exception("Specific Integrity of the process is not valid, therefore nothing was modified", new Exception(errorMessage));
                }
                else if (output is CoProduct)
                {
                    if (proc.CheckSpecificIntegrity(_data, true, true, out errorMessage))
                    {
                        CoProduct existingWithSameResource = proc.CoProducts.SingleOrDefault(item => item.ResourceId == (output as CoProduct).ResourceId);
                        if (existingWithSameResource != null)
                            proc.CoProducts.Remove(existingWithSameResource);
                        proc.CoProducts.Add(output as CoProduct);
                    }
                    else
                        throw new Exception("Specific Integrity of the process is not valid, therefore nothing was modified", new Exception(errorMessage));
                }
                else
                    throw new Exception("Unknown output instance class type, the type should be " + typeof(MainOutput) + " or " + typeof(CoProduct));

                if (autoCarbonEstimate && proc is StationaryProcess)//no need to update matrix for transportation process as the relation is 1 for 1
                    if ((process as StationaryProcess).MainOutput != null)//avoid throwing the exception in the carbon matrix defaults
                        (process as StationaryProcess).CarbonTransMatrix = (process as StationaryProcess).CarbonMatrixDefaults(_data);
                else
                    (process as StationaryProcess).CompleteMatrix((process as StationaryProcess).CarbonTransMatrix);
                return true;
            }
            else
                throw new Exception("Integrity of the process is not valid, therefore nothing was modified", new Exception(errorMessage));
        }
        /// <summary>
        /// Removes an input from a stationary process
        /// </summary>
        /// <param name="process">The process to be modified</param>
        /// <param name="input">The input to be removed</param>
        /// <returns>True if removed, false if the input is not part of that process</returns>
        public bool ProcessRemoveInput(IProcess process, IInput input)
        {
            if (process == null)
                throw new ArgumentNullException("process is a null reference");
            if (!(process is StationaryProcess))
                throw new Exception("Given process is not of type StationaryProcess");
            if (input == null)
                throw new ArgumentNullException("given input is a null reference");
            if (!(input is Input))
                throw new ArgumentNullException("given input is of type Input");
            StationaryProcess proc = process as StationaryProcess;

            List<IDependentItem> items = ProcessDependentItems(new DependentItem(proc));
            foreach (IDependentItem i in items)
            {
                if (i.Type == Enumerators.ItemType.Pathway && _data.PathwaysData.ContainsKey(i.Id))
                {
                    Pathway path = _data.PathwaysData[i.Id];
                    foreach (Vertex v in path.VerticesData.Values.Where(item => item.ModelID == proc.Id))
                    {
                        foreach (Edge e in path.EdgesData)
                        {
                            if (e.InputID == input.Id || e.OutputID == input.Id)
                                throw new Exception("Input cannot be removed because the process is used", new Exception("That input is connected to other itens in the pathway " + path.Name));
                        }
                    }
                }
            }

            foreach (Input i in proc.OtherInputs)
            {
                if (i == input)
                {
                    proc.OtherInputs.Remove(i);
                    return true;
                }
            }
            if (proc.Group != null)
            {
                foreach (Input i in proc.Group.Inputs)
                {
                    if (i == input)
                    {
                        proc.Group.Inputs.Remove(i);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Removes an output from a stationary process
        /// </summary>
        /// <param name="process"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool ProcessRemoveOutput(IProcess process, IIO output)
        {
            if (process == null)
                throw new ArgumentNullException("process is a null reference");
            if (!(process is AProcess))
                throw new Exception("Given process is not of type AProcess");
            if (!(process is StationaryProcess))
                throw new Exception("Given process is not of type StationaryProcess");
            if (output == null)
                throw new ArgumentNullException("given input is a null reference");
            if (!(output is AOutput))
                throw new ArgumentNullException("given input is of type Input");
            StationaryProcess proc = process as StationaryProcess;

            List<IDependentItem> items = ProcessDependentItems(new DependentItem(proc));
            foreach (IDependentItem i in items)
            {
                if (i.Type == Enumerators.ItemType.Pathway && _data.PathwaysData.ContainsKey(i.Id))
                {
                    Pathway path = _data.PathwaysData[i.Id];
                    foreach (Vertex v in path.VerticesData.Values.Where(item => item.ModelID == proc.Id))
                    {
                        foreach (Edge e in path.EdgesData)
                        {
                            if (e.InputID == output.Id || e.OutputID == output.Id)
                                throw new Exception("Output cannot be removed because the process is used", new Exception("That input is connected to other itens in the pathway " + path.Name));
                        }
                    }
                }
            }

            if (proc.MainOutput == output)
            {
                proc.MainOutput = null;
                return true;
            }
            foreach (CoProduct cop in proc.CoProducts)
            {
                if (cop == output)
                {
                    proc.CoProducts.Remove(cop);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Adds another emission item to a stationary process, this can be used to add emissions that are not related to technologies.
        /// </summary>
        /// <param name="process">The stationary process to which we desire to add other emissions</param>
        /// <param name="gasId">The ID of the emission we desire to add</param>
        /// <param name="quantity">The amount in kilograms we desire to add</param>
        /// <returns>True if added as a new emission, false if an emission with the same gasId already exists</returns>
        public bool ProcessAddOtherEmission(IProcess process, int gasId, double quantity)
        {
            if (process == null)
                throw new ArgumentNullException("process is a null reference");
            if (!(process is StationaryProcess))
                throw new Exception("Given process is not of type StationaryProcess");
            StationaryProcess proc = process as StationaryProcess;

            if (proc.OtherStaticEmissions == null)
                proc.OtherStaticEmissions = new ProcessStaticEmissionList(false);

            if (!proc.OtherStaticEmissions.ContainsKey(gasId))
            {
                ProcessStaticEmissionItem pse = new ProcessStaticEmissionItem(_data, gasId);
                pse.EmParameter.CurrentValue.UserValue = quantity;
                pse.EmParameter.CurrentValue.UseOriginal = false;
                proc.OtherStaticEmissions.StaticEmissions.Add(pse);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Sets the density of a resource by specifying the unit and quantiy
        /// </summary>
        /// <param name="resource">The resource being modified</param>
        /// <param name="preferedUnitExpression">The unit expression for boths values that is prefered for user representation of the density, values will be automatically converted to the SI units of that unit expression</param>
        /// <param name="userValue">The desired value for the density in the given prefered unit expression, will be set as the user value for the parameter</param>
        /// <returns>True if the density has been updated</returns>
        public bool ResourceSetDensity(IResource resource, string preferedUnitExpression, double userValue)
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            if (rd.DensityAsParameter == null)
            {
                rd.DensityAsParameter = _data.ParametersData.CreateRegisteredParameter(preferedUnitExpression, 0, userValue);
                rd.DensityAsParameter.UseOriginal = false;
                return true;
            }
            else
            {
                Parameter param = _data.ParametersData.CreateUnregisteredParameter(_data, preferedUnitExpression, 0, userValue, false);
                rd.DensityAsParameter.UserValue = param.UserValue;
                rd.DensityAsParameter.UserDim = param.Dim;
                rd.DensityAsParameter.UseOriginal = false;

                return true;
            }
        }
        /// <summary>
        /// Sets the market value for a resource
        /// </summary>
        /// <param name="resource">The resource being modified</param>
        /// <param name="preferedUnitExpression">The unit expression for boths values that is prefered for user representation of the market value, values will be automatically converted to the SI units of that unit expression</param>
        /// <param name="userValue">The desired value for the market value in the given prefered unit expression, will be set as the user value for the parameter/param>
        /// <returns>True if the market value is correctly initialized</returns>
        public bool ResourceSetMarketValue(IResource resource, string preferedUnitExpression, double userValue)
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            if (rd.MarketValue == null)
            {
                rd.MarketValue = _data.ParametersData.CreateRegisteredParameter(preferedUnitExpression, 0, userValue);
                rd.MarketValue.UseOriginal = false;
                return true;
            }
            else
            {
                Parameter param = _data.ParametersData.CreateUnregisteredParameter(_data, preferedUnitExpression, 0, userValue, false);
                rd.MarketValue.UserValue = param.UserValue;
                rd.MarketValue.UserDim = param.Dim;
                rd.MarketValue.UseOriginal = false;
                return true;
            }
        }
        /// <summary>
        /// Set the notes for a resource
        /// </summary>
        /// <param name="resource">The resource being modified</param>
        /// <param name="notes">Notes to be set</param>
        /// <returns>True if notes have been set correctly</returns>
        public bool ResourceSetNotes(IResource resource, string notes) 
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            rd.Notes = notes;
            return true;
        }
        /// <summary>
        /// Sets the Is Primary flag of a resource
        /// </summary>
        /// <param name="resource">The resource being modified</param>
        /// <param name="value">The value of that flag</param>
        /// <returns>True if the flag has been set correctly</returns>
        public bool ResourceSetIsPrimary(IResource resource, bool value)
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            rd.canBePrimaryResource = value;
            return true;
        }
        /// <summary>
        /// <para>Sets the sulfur ratio on a scale from 0 to 1</para>
        /// <para>Sulfur ratio is a time series. If the year argument is left to 0 the default year will be updated.</para>
        /// <para>If the year argument given exists in the time series, the corresponding sulfur ratio will be updated.</para>
        /// <para>If the year argument given does not exists in the time seres, a new sulfur ratio will be created for that year</para>
        /// </summary>
        /// <param name="resource">The resource being modified</param>
        /// <param name="userValue">The 0-1 value of the sulfur ratio</param>
        /// <param name="year">Optional year value to be added as Sulfur Ratio is a time series parameter, leave at 0 for default year</param>
        /// <returns>True if the value has been set correctly</returns>
        public bool ResourceSetSRatio(IResource resource, double userValue, int year = 0)
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            if (rd.SRatio == null)
            {
                rd.SRatio = new ParameterTS(_data, "%", 0, userValue * 100);
                rd.SRatio.CurrentValue.UseOriginal = false;
                return true;
            }
            else
            {
                if (rd.SRatio.ContainsKey(year))
                {
                    rd.SRatio.Value(year).ValueInDefaultUnit = userValue;
                    return true;
                }
                else
                {
                    Parameter newValue = _data.ParametersData.CreateRegisteredParameter("%", 0, userValue * 100);
                    newValue.UseOriginal = false;
                    rd.SRatio.Add(year, newValue);
                    return true;
                }
            }
        }
        /// <summary>
        /// Sets the Higher heating value for a resource
        /// </summary>
        /// <param name="resource">The resource beeing modified</param>
        /// <param name="preferedUnitExpression">The unit expression for boths values that is prefered for user representation of the heating value, values will be automatically converted to the SI units of that unit expression</param>
        /// <param name="userValue">The desired value for the market value in the given prefered unit expression, will be set as the user value for the parameter</param>
        /// <returns>True if the heating value has been set correctly</returns>
        public bool ResourceSetHHV(IResource resource, string preferedUnitExpression, double userValue) 
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            if (rd.HeatingValueHhv == null)
            {
                rd.HeatingValueHhv = _data.ParametersData.CreateRegisteredParameter(preferedUnitExpression, 0, userValue);
                rd.HeatingValueHhv.UseOriginal = false;
                return true;
            }
            else
            {
                Parameter param = _data.ParametersData.CreateUnregisteredParameter(_data, preferedUnitExpression, 0, userValue, false);
                rd.HeatingValueHhv.UserValue = param.UserValue;
                rd.HeatingValueHhv.UserDim = param.Dim;
                rd.HeatingValueHhv.UseOriginal = false;

                return true;
            }
        }
        /// <summary>
        /// Sets the physical state of the resource
        /// </summary>
        /// <param name="resource">The resource being modified</param>
        /// <param name="physicalState">The desired physical state for that resource</param>
        /// <returns>True if the physical state has been set correctly</returns>
        public bool ResourceSetState(IResource resource, Resources.PhysicalState physicalState)
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            rd.State = physicalState;
            return true;
        }
        /// <summary>
        /// Sets the carbon ratio for a resource using a 0 to 1 value
        /// </summary>
        /// <param name="resource">The resource being modified</param>
        /// <param name="userValue">The 0-1 value for the carbon ratio</param>
        /// <returns>True if the value has been set correctly</returns>
        public bool ResourceSetCRatio(IResource resource, double userValue) 
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            if (rd.CRatio == null)
            {
                rd.CRatio = _data.ParametersData.CreateRegisteredParameter("%", 0, userValue * 100);
                rd.CRatio.UseOriginal = false;
                return true;
            }
            else
            {
                Parameter param = _data.ParametersData.CreateUnregisteredParameter(_data, "%", 0, userValue * 100, false);
                rd.CRatio.UserValue = param.UserValue;
                rd.CRatio.UserDim = param.Dim;
                rd.CRatio.UseOriginal = false;

                return true;
            }
        }
        /// <summary>
        /// Sets the lower heating value for the resource
        /// </summary>
        /// <param name="resource">The resource being modified</param>
        /// <param name="preferedUnitExpression">The unit expression for boths values that is prefered for user representation of the heating value, values will be automatically converted to the SI units of that unit expression</param>
        /// <param name="userValue">The desired value for the market value in the given prefered unit expression, will be set as the user value for the parameter</param>
        /// <returns>True if the LHV has been set correctly</returns>
        public bool ResourceSetLHV(IResource resource, string preferedUnitExpression, int userValue)
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            if (rd.HeatingValueLhv == null)
            {
                rd.HeatingValueLhv = _data.ParametersData.CreateRegisteredParameter(preferedUnitExpression, 0, userValue);
                rd.HeatingValueLhv.UseOriginal = false;
                return true;
            }
            else
            {
                Parameter param = _data.ParametersData.CreateUnregisteredParameter(_data, preferedUnitExpression, 0, userValue, false);
                rd.HeatingValueLhv.UserValue = param.UserValue;
                rd.HeatingValueLhv.UserDim = param.Dim;
                rd.HeatingValueLhv.UseOriginal = false;

                return true;
            }
        }
        /// <summary>
        /// Sets to null the lower heating value. This is necessary if the resource has no heating value.
        /// If the user sets a heating value of 0, GREET will use this to convert all quantities to zero.
        /// If the lower heating value is unset or null, GREET will not perform any conversion and will keep the original quantities
        /// </summary>
        /// <param name="resource">The resource being modified</param>
        /// <returns>True if successfully set to null</returns>
        public bool ResourceUnsetLHV(IResource resource)
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            rd.HeatingValueLhv = null;
            return true;
        }
        /// <summary>
        /// Sets to null the higher heating value. This is necessary if the resource has no heating value.
        /// If the user sets a heating value of 0, GREET will use this to convert all quantities to zero.
        /// If the higher heating value is unset or null, GREET will not perform any conversion and will keep the original quantities
        /// </summary>
        /// <param name="resource">The resource being modified<param>
        /// <returns>True if successfully set to null</returns>
        public bool ResourceUnsetHVV(IResource resource)
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            rd.HeatingValueHhv = null;
            return true;
        }
        /// <summary>
        /// Sets to null the density. This is necessary if the resource has no density.
        /// If the users sets a density of 0, GREET will use it to perform conversion and may end up with zeros.
        /// If the density is unset or null, GREET will not perform any conversion and will keep the original quantities
        /// </summary>
        /// <param name="resource">The resource being modified</param>
        /// <returns>True if successfully set to null</returns>
        public bool ResourceUnsetDensity(IResource resource)
        {
            if (resource == null)
                throw new ArgumentNullException("Resource cannot be null");
            if (!(resource is ResourceData))
                throw new Exception("Resource is not of type ResourceData");
            ResourceData rd = resource as ResourceData;
            rd.DensityAsParameter = null;
            return true;
        }
        /// <summary>
        /// Adds a new year to the technology for all pollutants
        /// </summary>
        /// <param name="technology">The technology object to be modified</param>
        /// <param name="year">The new year to be added i.e. 2023</param>
        /// <returns>True if the year has been added, false ifthe year is already present for that technology</returns>
        public bool TechnologyAddYear(ITechnology technology, int year)
        {
            if (technology == null)
                throw new ArgumentNullException("Given technology is a null reference");
            if (!(technology is TechnologyData))
                throw new Exception("The given technology is not of type Technology");
            TechnologyData tech = technology as TechnologyData;

            if (tech.ContainsKey(year))
                return false;
            else
            {
                List<int> gases = new List<int>();

                if (tech.Count > 0)
                    foreach (int gasId in tech.First().Value.EmissionFactorsForCalculations.Keys)
                        gases.Add(gasId);

                RealEmissionsFactors ef = new RealEmissionsFactors(year);
                tech.Add(year, ef as EmissionsFactors);

                foreach (int gasId in gases)
                {
                    Parameter param = _data.ParametersData.CreateRegisteredParameter("kg/J", 0);
                    param.UseOriginal = false;
                    ef.EmissionFactors.Add(gasId, new EmissionValue(param, false));
                }

                return true;
            }
        }
        /// <summary>
        /// Sets the resource being combusted/used by the technology
        /// </summary>
        /// <param name="technology">The technology to be modified</param>
        /// <param name="resource">The resource ID to be assigned to that technology</param>
        /// <returns></returns>
        public bool TechnologySetResource(ITechnology technology, int resource)
        {
            if (technology == null)
                throw new ArgumentNullException("Given technology is a null reference");
            if (!(technology is TechnologyData))
                throw new Exception("The given technology is not of type Technology");
            TechnologyData tech = technology as TechnologyData;
            List<IDependentItem> dependentItems = TechnologyDependentItems(new DependentItem(tech));
            if (dependentItems.Count > 0)
                throw new Exception("Items depend on that pathway, therefore the main output cannot be changed");
            tech.InputResourceRef = resource;
            return true;
        }
        /// <summary>
        /// Adds an emission factor to every years in the current technology
        /// </summary>
        /// <param name="technology">Technology being modified</param>
        /// <param name="gas">Gas being added as emission factors</param>
        /// <returns>True if emission factors have been added, false if the emission factor is already defined for that gas</returns>
        public bool TechnologyAddEmission(ITechnology technology, IGas gas)
        {
            if (technology == null)
                throw new ArgumentNullException("Given technology is a null reference");
            if (!(technology is TechnologyData))
                throw new Exception("The given technology is not of type Technology");
            if (gas == null)
                throw new ArgumentNullException("Given gas is a null reference");
            if (!(gas is Gas))
                throw new Exception("The given gas is not of type Gas");

            TechnologyData tech = technology as TechnologyData;

            if (tech.Count > 0 && tech.First().Value.EmissionFactorsForCalculations.ContainsKey(gas.Id))
                return false;//already contains a year with this gas so we assume all the years are already containing this gas
            else
            {
                List<int> years = new List<int>();
                if (tech.Count > 0)
                    foreach (int year in tech.Keys)
                        years.Add(year);
                else
                    years.Add(0);

                foreach (int year in years)
                {
                    if (!tech.ContainsKey(year))
                    {
                        RealEmissionsFactors ef = new RealEmissionsFactors(year);
                        tech.Add(year, ef);
                    }
                    EmissionsFactors efs = tech[year];
                    if (efs is RealEmissionsFactors)
                    {
                        Parameter param = _data.ParametersData.CreateRegisteredParameter("kg/J", 0);
                        param.UseOriginal = false;
                        (efs as RealEmissionsFactors).EmissionFactors.Add(gas.Id, new EmissionValue(param, false));
                    }
                    else
                        throw new Exception("Cannot add an emission factor to this technology because it's refereing another technology with ratios");
                }
                return true;
            }
        }
        /// <summary>
        /// Retrieves an emission factor from a technology given a year and pollutant id
        /// </summary>
        /// <param name="technology">The technology from which we want to retrive this emission factor</param>
        /// <param name="yearValue">The Year for which we want to get the emission factor</param>
        /// <param name="pollutantId">The ID of the pollutant for which we want to get the emission factor</param>
        /// <returns>The emission factor if found, null if no emission factor exists for the given year and pollutant id</returns>
        public IParameter TechnologyGetEF(ITechnology technology, int yearValue, int pollutantId)
        { 
            if (technology == null)
                throw new ArgumentNullException("Given technology is a null reference");
            if (!(technology is TechnologyData))
                throw new Exception("The given technology is not of type Technology");

            TechnologyData tech = technology as TechnologyData;

            if(tech.ContainsKey(yearValue))
            {
                EmissionsFactors efs = tech[yearValue];
                if (efs is RealEmissionsFactors)
                {
                    RealEmissionsFactors real = efs as RealEmissionsFactors;
                    if (real.EmissionFactors.ContainsKey(pollutantId))
                        return real.EmissionFactors[pollutantId].Value as IParameter;
                }
                else
                {
                    BasedEmissionFactors based = efs as BasedEmissionFactors;
                    if(based.Ratios.ContainsKey(pollutantId))
                        return based.Ratios[pollutantId].Value as IParameter;
                }
            }
            return null;
        }
        /// <summary>
        /// Sets the value of an emission factor for a given year and pollutant id
        /// Creates a new emission factor if it was not previously an instance of an object
        /// </summary>
        /// <param name="technology">The technology being modified</param>
        /// <param name="yearValue">The Year for which we want to get the emission factor</param>
        /// <param name="pollutantId">The ID of the pollutant for which we want to get the emission factor</param>
        /// <param name="unit">The unit for the emission factor</param>
        /// <param name="value">The value for the emission factor</param>
        /// <returns>True if successfully modified</returns>
        public bool TechnologySetEF(ITechnology technology, int yearValue, int pollutantId, string unit, double value)
        { 
            if (technology == null)
                throw new ArgumentNullException("Given technology is a null reference");
            if (!(technology is TechnologyData))
                throw new Exception("The given technology is not of type Technology");

            TechnologyData tech = technology as TechnologyData;

            if(tech.ContainsKey(yearValue))
            {
                EmissionsFactors efs = tech[yearValue];
                if (efs is RealEmissionsFactors)
                {
                    RealEmissionsFactors real = efs as RealEmissionsFactors;
                    if (real.EmissionFactors.ContainsKey(pollutantId))
                    {
                        Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
                        Parameter ef = real.EmissionFactors[pollutantId].Value;
                        ef.UserDim = temp.Dim;
                        ef.ValueInDefaultUnit = temp.GreetValue;
                        ef.UseOriginal = false;
                        return true;
                    }
                }
                else
                {
                    BasedEmissionFactors based = efs as BasedEmissionFactors;
                    if (based.Ratios.ContainsKey(pollutantId))
                    {
                        Parameter temp = _data.ParametersData.CreateUnregisteredParameter(_data, unit, value);
                        Parameter ef = based.Ratios[pollutantId].Value;
                        ef.UserDim = temp.Dim;
                        ef.ValueInDefaultUnit = temp.GreetValue;
                        ef.UseOriginal = false;
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Adds a transportation step to a transportation process
        /// </summary>
        /// <param name="process">The transportation process to be manipulated</param>
        /// <param name="step">The step to be added to the transportation process</param>
        public bool TransportationAddTStep(IProcess process, ITransportationStep step)
        {
            if (process == null)
                throw new ArgumentNullException("process cannot be null");
            if (step == null)
                throw new ArgumentNullException("step cannot be null");
            if (!(process is TransportationProcess))
                throw new Exception("the process must be an instance of a transportation process, steps cannot be added to any other kind of processes");
            else
            {
                TransportationProcess tp = process as TransportationProcess;
                TransportationStep ts = step as TransportationStep;
                string stepError;
                ts.CheckIntegrity(_data, true, false, tp.MainOutput.ResourceId, out stepError);
                if (!String.IsNullOrEmpty(stepError))
                    throw new Exception("The given step has errors", new Exception(stepError));

                if (!tp.TransportationSteps.ContainsKey(ts.Id))
                {
                    tp.TransportationSteps.Add(ts.Id, ts);
                    return true;
                }
                else
                    throw new ArgumentException("A transportation step with the same ID is already added to that transportation process");
            }
        }

        /// <summary>
        /// Add connector between a process and a location in a transportation step
        /// </summary>
        /// <param name="start">The origin for the connection</param>
        /// <param name="end">The end for the connection</param>
        public bool TransportationAddConnector(ILocation start, ITransportationStep end) 
        {
            if (start == null)
                throw new ArgumentNullException("strart connection location cannot be null");
            if (end == null)
                throw new ArgumentNullException("end connection transportation step cannot be null");
            if (!(end is TransportationStep))
                throw new Exception("the end must be an instance of a transportation step");
            if (!_data.LocationsData.ContainsKey(start.Id))
                throw new Exceptions.IDNotFoundInDatabase("The given location cannot be found in the current dataset, please insert this location first");
            else
            {
                TransportationStep ts = end as TransportationStep;
                ts.OriginRef = start.Id;
                return true;
            }
        }
        /// <summary>
        /// Add connector between a process and a location in a transportation step
        /// </summary>
        /// <param name="process">The transportation process to be manipulated</param>
        /// <param name="start">The origin for the connection</param>
        /// <param name="end">The end for the connection</param>
        public bool TransportationAddConnector(ITransportationStep start, ILocation end) 
        {
            if (start == null)
                throw new ArgumentNullException("start connection transportation step cannot be null");
            if (end == null)
                throw new ArgumentNullException("end connection location cannot be null");
            if (!(start is TransportationStep))
                throw new Exception("the start must be an instance of a transportation step");
            if (!_data.LocationsData.ContainsKey(end.Id))
                throw new Exceptions.IDNotFoundInDatabase("The given location cannot be found in the current dataset, please insert this location first");
            else
            {
                TransportationStep ts = start as TransportationStep;
                ts.DestinationRef = end.Id;
                return true;
            }
        }
        /// <summary>
        /// Sets the resource being transported by a transportation process, returns true if succeeded, false otherwise
        /// </summary>
        /// <param name="process">The process for which we want to change the transported resource</param>
        /// <param name="resourceId">The transported resource ID</param>
        /// <returns>True if successfully changed</returns>
        public bool TransportationSetResource(IProcess process, int resourceId)
        {
            if(!_data.ResourcesData.ContainsKey(resourceId))
                throw new Exceptions.IDNotFoundInDatabase("The given resource ID cannot be found in the current dataset, please insert this resource first");
            if (process == null)
                throw new ArgumentNullException("process cannot be null");
            else
            { 
                TransportationProcess tp = process as TransportationProcess;

                List<IDependentItem> dependentItems = ProcessDependentItems(new DependentItem(tp));
                if (dependentItems.Count > 0)
                    throw new Exception("Items depend on that process, therefore the transported resource cannot be changed");

                string stepIssues = "";
                foreach (TransportationStep ts in tp.TransportationSteps.Values)
                {
                    string stepIssue;
                    ts.CheckIntegrity(_data, true, false, resourceId, out stepIssue);
                    stepIssues += stepIssue + Environment.NewLine;
                }

                if(!String.IsNullOrEmpty(stepIssues))
                    throw new Exception("Some of the transportation steps cannot handle the new resource ID", new Exception(stepIssues));
                else
                {
                    tp.MainInput.ResourceId = resourceId;
                    tp.MainOutput.ResourceId = resourceId;

                    tp.MainInput.id = Guid.NewGuid();
                    tp.MainOutput.id = Guid.NewGuid();
                    return true;
                }
            }
        }
        /// <summary>
        /// <para>Removes a transportation step from a transportation process</para>
        /// <para>This also removes the associated connectors</para>
        /// </summary>
        /// <param name="process">The process being modified</param>
        /// <param name="step">The step being removed</param>
        /// <returns>True if the step was removed, false if the step cannot be found in that process</returns>
        public bool TransportationRemoveStep(IProcess process, ITransportationStep step) 
        {
            if (process == null)
                throw new ArgumentNullException("Process cannot be null");
            if (step == null)
                throw new ArgumentNullException("Step cannot be null");
            if (!(step is TransportationStep))
                throw new Exception("Step is not of type TransportationStep");
            if (!(process is TransportationProcess))
                throw new Exception("the process must be an instance of a transportation process, steps cannot be added to any other kind of processes");
            else
            {
                TransportationProcess tp = process as TransportationProcess;
                TransportationStep ts = step as TransportationStep;
                if (!tp.TransportationSteps.ContainsKey(ts.Id))
                {
                    tp.TransportationSteps.Remove(ts.Id);
                    return true;
                }
                else
                    throw new ArgumentException("A transportation step with the same ID is already added to that transportation process");
            }
        }
        /// <summary>
        /// <para>Removes a location from a transportation process</para>
        /// <para>This also removes the associated connectors</para>
        /// </summary>
        /// <param name="process">The process being modified</param>
        /// <param name="location">The location being removed</param>
        /// <returns>True if the step was removed, false if the location cannot be found in that process</returns>
        public bool TransportationRemoveLocation(IProcess process, ILocation location)
        {
            if (process == null)
                throw new ArgumentNullException("Process cannot be null");
            if (location == null)
                throw new ArgumentNullException("Location cannot be null");
            if (!(location is LocationData))
                throw new Exception("Location must be of type LocationData");
            if (!(process is TransportationProcess))
                throw new Exception("the process must be an instance of a transportation process, steps cannot be added to any other kind of processes");
            else
            {
                TransportationProcess tp = process as TransportationProcess;
                LocationData loc = location as LocationData;

                bool removed = false;
                foreach(TransportationStep ts in tp.TransportationSteps.Values)
                {
                    if (ts.DestinationRef == loc.Id) {
                        ts.DestinationRef = -1;
                        removed = true;
                    }
                    if (ts.OriginRef == loc.Id) { 
                        ts.OriginRef = -1;
                        removed = true;
                    }
                }
                return removed;
            }
        }
        /// <summary>
        /// Removes a connector from a transportation process
        /// </summary>
        /// <param name="process">The process being modifed</param>
        /// <param name="start">The start of the connector as a transportation step</param>
        /// <param name="end">The end of the connector as a location</param>
        /// <returns>True if the connector was removed, false if the end is not connected to the start</returns>
        public bool TransportationRemoveConnector(IProcess process, ITransportationStep start, ILocation end)
        {
            if (process == null)
                throw new ArgumentNullException("Process cannot be null");
            if (start == null)
                throw new ArgumentNullException("Start cannot be null");
            if(end == null)
                throw new ArgumentNullException("End cannot be null");
            if (!(start is TransportationStep))
                throw new Exception("Start is not of type TransportationStep");
            if (!(end is LocationData))
                throw new Exception("End must be of type LocationData");
            if (!(start is TransportationProcess))
                throw new Exception("the process must be an instance of a transportation process, steps cannot be added to any other kind of processes");
            else
            {
                TransportationStep ts = start as TransportationStep;
                LocationData loc = end as LocationData;
                if (ts.DestinationRef == loc.Id)
                {
                    ts.DestinationRef = -1;
                    return true;
                }
                return false;
            }

        }
        /// <summary>
        /// Removes a connector from a transportation process
        /// </summary>
        /// <param name="process">The process being modifed</param>
        /// <param name="start">The start of the connector as a location</param>
        /// <param name="end">The end of the connector as a transportation step</param>
        /// <returns>True if the connector was removed, false otherwise</returns>
        public bool TransportationRemoveConnector(IProcess process, ILocation start, ITransportationStep end)
        {
            if (process == null)
                throw new ArgumentNullException("Process cannot be null");
            if (start == null)
                throw new ArgumentNullException("Start cannot be null");
            if (end == null)
                throw new ArgumentNullException("End cannot be null");
            if (!(start is ILocation))
                throw new Exception("Start is not of type LocationData");
            if (!(end is TransportationStep))
                throw new Exception("End must be of type TransportationStep");
            if (!(start is TransportationProcess))
                throw new Exception("the process must be an instance of a transportation process, steps cannot be added to any other kind of processes");
            else
            {
                TransportationStep ts = end as TransportationStep;
                LocationData loc = start as LocationData;
                if (ts.OriginRef == loc.Id)
                {
                    ts.OriginRef = -1;
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Modifies a mix by adding to it a pathway and a share
        /// </summary>
        /// <param name="mix">The mix to be modified</param>
        /// <param name="pathway">The source for the upstream added to that mix</param>
        /// <param name="share">The share for the source added to that mix</param>
        /// <returns>True if success, false if the pathways is already there</returns>
        public bool MixAddFeed(IMix mix, IPathway pathway, double share = 0)
        {
            if (mix == null)
                throw new ArgumentNullException("Given mix is a null reference");
            if (!(mix is Mix))
                throw new Exception("The given mix is not of type Mix");
            Mix mixx = mix as Mix;
            if (pathway == null)
                throw new ArgumentNullException("Given pathway is a null reference");
            if (!(pathway is Pathway))
                throw new Exception("The given pathway is not of type Pathway");
            Pathway path = pathway as Pathway;
            if(!(path.MainOutput != Guid.Empty && path.OutputsData.Any(item => item.Id == path.MainOutput)))
                throw new Exception("The main output of the pathway is not properly defined");

            if (mix.FuelProductionEntities.Any(item => item.MixOrPathwayId == path.Id && item.SourceType == Enumerators.SourceType.Pathway))
                return false;

            ParameterTS pmts = new ParameterTS(_data, "%", 0, share);
            pmts.CurrentValue.UseOriginal = false;
            PathwayProductionEntity mpe = new PathwayProductionEntity(pathway.Id, path.MainOutput, pmts);
            mixx.Entities.Add(mpe as FuelProductionEntity);
            return true;
        }
        /// <summary>
        /// Modifies a mix by adding to it a mix and a share
        /// </summary>
        /// <param name="mix">The mix to be modified</param>
        /// <param name="mix">The source for the upstream added to that mix</param>
        /// <param name="share">The share for the source added to that mix</param>
        /// <returns>True if success, false if the mix is already there</returns>
        public bool MixAddFeed(IMix mix, IMix upstream, double share)
        {
            if (mix == null)
                throw new ArgumentNullException("Given mix is a null reference");
            if (!(mix is Mix))
                throw new Exception("The given mix is not of type Mix");
            Mix mixx = mix as Mix;
            if (upstream == null)
                throw new ArgumentNullException("Given mix is a null reference");
            if (!(upstream is Mix))
                throw new Exception("The given mix is not of type Mix");
            Mix upstreamMix = upstream as Mix;
            if (!(upstreamMix.output != null))
                throw new Exception("The main output of the mix is not properly defined");

            if (mix.FuelProductionEntities.Any(item => item.MixOrPathwayId == upstreamMix.Id && item.SourceType == Enumerators.SourceType.Pathway))
                return false;

            ParameterTS pmts = new ParameterTS(_data, "%", 0, share);
            pmts.CurrentValue.UseOriginal = false;
            MixProductionEntity mpe = new MixProductionEntity(upstreamMix.Id, pmts);
            mixx.Entities.Add(mpe as FuelProductionEntity);
            return true;
        }
        /// <summary>
        /// Sets the output resource ID for the mix
        /// </summary>
        /// <param name="mix">The mix to be modified</param>
        /// <param name="resourceId">The resource id to be set as the main output</param>
        /// <returns>True if success</returns>
        public bool MixSetResource(IMix mix, int resourceId)
        {
            if (mix == null)
                throw new ArgumentNullException("Given mix is a null reference");
            if (!(mix is Mix))
                throw new Exception("The given mix is not of type Mix");
            Mix mixx = mix as Mix;

            List<IDependentItem> dependentItems = MixDependentItems(new DependentItem(mixx));
            if (dependentItems.Count > 0) 
                throw new Exception("Items depend on that mix, therefore the resource output cannot be changed");

            if (mixx.output != null)
                mixx.output.ResourceId = resourceId;
            else
            {
                mixx.output = new PMOutput();
                mixx.output.ResourceId = resourceId;
            }
            return true;
        }
        /// <summary>
        /// Adds a technology to the input of a stationary process
        /// </summary>
        /// <param name="input">The input being modified</param>
        /// <param name="techId">The ID of the technology to add</param>
        /// <param name="share">The energy share 0-1 for that technology</param>
        /// <returns>True if the technology has been added, false if the same technology is already there</returns>
        public bool InputAddTechnology(IInput input, int techId, double share = 0)
        {
            if (input == null)
                throw new ArgumentNullException("given input is a null reference");
            if (!(input is Input))
                throw new ArgumentNullException("given input is of type Input");

            Input inp = input as Input;
            if (inp.Technologies.Any(item => item.Reference == techId))
                return false;
            
            TechnologyRef tref = new EntityTechnologyRef(_data, techId, share);
            inp.Technologies.Add(tref);
            return true;
        }
        /// <summary>
        /// Removes a technology from an input of a stationary process
        /// </summary>
        /// <param name="input">The input being modified</param>
        /// <param name="techId">The ID of the technology to be removed</param>
        /// <returns>True if the technology has been removed, false if a technology with that ID couldn't be found</returns>
        public bool InputRemoveTechnology(IInput input, int techId) 
        {
            if (input == null)
                throw new ArgumentNullException("given input is a null reference");
            if (!(input is Input))
                throw new ArgumentNullException("given input is of type Input");

            Input inp = input as Input;
            if (!inp.Technologies.Any(item => item.Reference == techId))
                return false;
            else
            {
                TechnologyRef tref = inp.Technologies.Single(item => item.Reference == techId);
                inp.Technologies.Remove(tref);
                return true;
            }
        }
       
        #endregion

        #region Entities Duplicators
        /// <summary>
        /// Creates a copy of a pathway with an identical structure but a whole new set of IDs
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public IPathway DuplicatePathway(IPathway o)
        {
            Pathway duplicated = new Pathway(_data.PathwaysData.Keys.ToArray<int>());
            duplicated.Name = o.Name;
            duplicated.PictureName = o.PictureName;
            while (_data.PathwaysData.Values.Any(item => item.Name == duplicated.Name))
                duplicated.Name = duplicated.Name + "-Copy";
            duplicated.Notes = (o as Pathway).Notes;
            duplicated.ModifiedBy = (o as Pathway).ModifiedBy;
            duplicated.ModifiedOn = (o as Pathway).ModifiedOn;
            
            Dictionary<Guid,Guid> map = new Dictionary<Guid,Guid>();
            foreach (Vertex vertex in (o as Pathway).VerticesData.Values)
            {
                map.Add(vertex.ID, Guid.NewGuid());
                Vertex dv = new Vertex(vertex.ModelID);
                dv.ID = map[vertex.ID];
                dv.Location = vertex.Location;
                dv.Type = vertex.Type;
                duplicated.VerticesData.Add(dv.ID, dv);
            }

            foreach (PMOutput output in (o as Pathway).OutputsData)
            {
                map.Add(output.Id, Guid.NewGuid());
                PMOutput dp = new PMOutput();
                dp.Id = map[output.Id];
                dp.Location = output.Location;
                dp.Notes = output.Notes;
                dp.ResourceId = output.ResourceId;
                duplicated.OutputsData.Add(dp);
            }

            if(o != null && map.ContainsKey(o.MainOutput))
                duplicated.MainOutput = map[o.MainOutput];

            foreach (Edge edge in (o as Pathway).EdgesData)
            {
                Edge de = new Edge(map[edge.OutputVertexID]
                    , edge.OutputID
                    , map[edge.InputVertexID]
                    , ((o as Pathway).OutputsData.Any(outp => outp.Id == edge.InputID) ? map[edge.InputID] : edge.InputID));
                duplicated.EdgesData.Add(de);
            }

            return duplicated; 
        }
        /// <summary>
        /// Creates a copy of a stationary process with an identical structure but new IDs and parameters
        /// </summary>
        /// <param name="stationaryProcess">The process to be duplicated</param>
        /// <returns>The new instance of a duplicated process</returns>
        public IStationaryProcess DuplicateStationaryProcess(IStationaryProcess stationaryProcess)
        {
            StationaryProcess duplicated = Convenience.Clone(stationaryProcess as StationaryProcess);
            duplicated.Id = Convenience.IDs.GetIdUnusedFromTimeStamp(_data.ProcessesData.Keys);
            while (_data.ProcessesData.Values.Any(item => item.Name == duplicated.Name))
                duplicated.Name = duplicated.Name + "-Copy";

            Dictionary<Guid, Guid> inputOld2New = new Dictionary<Guid, Guid>();
            foreach (Input inp in duplicated.OtherInputs)
            {
                inputOld2New.Add(inp.id, Guid.NewGuid());
                inp.id = inputOld2New[inp.id];

            }

            if (duplicated.Group != null)
            {
                foreach (Input inp in duplicated.Group.Inputs)
                {
                    inputOld2New.Add(inp.id, Guid.NewGuid());
                    inp.id = inputOld2New[inp.id];
                }
            }

            Dictionary<Guid, Guid> outputOld2New = new Dictionary<Guid, Guid>();
            if (duplicated.MainOutput != null)
            {
                outputOld2New.Add(duplicated.MainOutput.id, Guid.NewGuid());
                duplicated.MainOutput.id = outputOld2New[duplicated.MainOutput.id];
            }

            foreach (CoProduct cop in duplicated.CoProducts)
            {
                outputOld2New.Add(cop.id, Guid.NewGuid());
                cop.id = outputOld2New[cop.id];
            }

            duplicated.CarbonTransMatrix = new Dictionary<Guid, Dictionary<Guid, double>>();
            duplicated.CompleteMatrix(duplicated.CarbonTransMatrix);

            foreach (KeyValuePair<Guid, Guid> inpId in inputOld2New)
            {
                foreach (KeyValuePair<Guid, Guid> outId in outputOld2New)
                {
                    duplicated.CarbonTransMatrix[outId.Value][inpId.Value] = (stationaryProcess as StationaryProcess).CarbonTransMatrix[outId.Key][inpId.Key];
                }
            }

            ToolsDataStructure.RenameAllParameters(_data.ParametersData, duplicated);

            return duplicated;
        }
        /// <summary>
        /// Creates a new instance of a mix with a new ID and new output ID
        /// Takes care of registering all parameters and assiging new IDs
        /// </summary>
        /// <param name="mix">The mix to be duplicated</param>
        /// <returns>A new instance of the mix</returns>
        public IMix DuplicateMix(IMix mix)
        {
            Mix duplicated = Convenience.Clone(mix as Mix);
            while (_data.MixesData.Values.Any(item => item.Name == duplicated.Name))
                duplicated.Name = duplicated.Name + "-Copy";
            duplicated.Id = Convenience.IDs.GetIdUnusedFromTimeStamp(_data.MixesData.Keys);
            duplicated.output.Id = Guid.NewGuid();

            ToolsDataStructure.RenameAllParameters(_data.ParametersData, duplicated);

            return duplicated;
        }
        /// <summary>
        /// Creates a copy of a transportation process with an identical structure but new IDs and parameters
        /// </summary>
        /// <param name="transportationProcess">The process to be duplicated</param>
        /// <returns>The new instance of a duplicated process</returns>
        public ITransportationProcess DuplicateTransportationProcess(ITransportationProcess transportationProcess)
        {
            TransportationProcess duplicated = Convenience.Clone(transportationProcess as TransportationProcess);
            duplicated.Id = Convenience.IDs.GetIdUnusedFromTimeStamp(_data.ProcessesData.Keys);
            while (_data.ProcessesData.Values.Any(item => item.Name == duplicated.Name))
                duplicated.Name = duplicated.Name + "-Copy";

            duplicated.MainInput.id = Guid.NewGuid();
            duplicated.MainOutput.id = Guid.NewGuid();

            duplicated.AssignDefaultsForMatrix();

            ToolsDataStructure.RenameAllParameters(_data.ParametersData, duplicated);

            return duplicated;
        }
        #endregion

        #region Dependencies Finder and Used/Unused methods

        #region Pathways
        /// <summary>
        /// This method returns a list of pathways that are un-used by other parts of the sofware.
        /// In other words there is no other entitiy in Greet that is dependent on these pathways.
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> unUsedPathways()
        {
            List<int> unUsedPathways = new List<int>();
            List<int> usedPathways = this.usedPathways();

            foreach (Pathway pw in _data.PathwaysData.Values)
            {
                if (pw.Discarded == false)
                {
                    if (usedPathways.Contains(pw.Id) == false && unUsedPathways.Contains(pw.Id) == false)//if the pathway is not used and not already in the list of not used pathways
                        unUsedPathways.Add(pw.Id);
                }
            }
            return unUsedPathways;
        }
        /// <summary>
        /// This method returns a list of pathways that are used by other parts of the software.
        /// In other words there are entities in Greet dependent on the exisitence of these pathways.
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> usedPathways(bool excludeVehicles = false, bool excludeMix = false)
        {
            List<int> usedPathways = new List<int>();

            foreach (Pathway pw in _data.PathwaysData.Values)
            {
                if (pw.Discarded == false)
                {
                    List<IDependentItem> dependentData = PathwayDependentItems(new DependentItem(pw), new Guid(), excludeVehicles, excludeMix, true);
                    if (dependentData.Count > 0 && usedPathways.Contains(pw.Id) == false)
                        usedPathways.Add(pw.Id);
                }
            }

            return usedPathways;
        }
        /// <summary>
        /// List all instances that are dependent of the Pathway passed as a argument
        /// </summary>
        /// <param name="pathway">The pathway for which we want to know all the dependencies</param>
        /// <param name="output">Optional GUID for the output that we want to test. If empty all outputs are considered</param>
        /// <param name="excludeVehicles">If set to true, this method will nto account pathway used when only used within a vehicle (transportation modes and anythnig else is still accounted as used)</param>
        /// <param name="excludeMixes">If set to true, this method will not account pathways used only withing a mix with a share of zero<s/param>
        /// <param name="returnAsap">If set to TRUE, the method will return when it hits the first dependency</param>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IDependentItem> PathwayDependentItems(IDependentItem pathway, Guid output = new Guid(), bool excludeVehicles = false, bool excludeMixes = false, bool returnAsap = false)
        {
            if (pathway.Type == Enumerators.ItemType.Pathway && _data.PathwaysData.ContainsKey(pathway.Id))
            {
                Pathway pw = _data.PathwaysData[pathway.Id];
                Guid mainOutput = pw.MainOutput;
                List<IDependentItem> dependentData = new List<IDependentItem>();

                if (!excludeVehicles)
                {
                    #region Dependent Vehicles fuels and materials
                    foreach (Vehicle vehicle in _data.VehiclesData.Values)
                    {
                        if (vehicle.Discarded == false)
                        {
                            bool breakOuter = false;
                            foreach (VehicleOperationalMode mode in vehicle.Modes)
                            {
                                foreach (VehicleModePowerPlant plant in mode.Plants)
                                {
                                    foreach (InputResourceReference fuel in plant.FuelUsed)
                                    {
                                        if (fuel.SourceType == Enumerators.SourceType.Pathway && pw.Id == fuel.SourceMixOrPathwayID
                                            && (output == Guid.Empty || mainOutput == output))
                                        {
                                            DependentItem vDi = new DependentItem(vehicle);
                                            if (!dependentData.Contains(vDi))
                                            {
                                                dependentData.Add(vDi);
                                                if (returnAsap)
                                                    return dependentData;
                                                breakOuter = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (breakOuter)
                                    break;
                            }

                            if (!breakOuter)
                            {
                                foreach (VehicleManufacturing mf in vehicle.Manufacturing)
                                {
                                    foreach (InputResourceReference inputRef in mf.Materials)
                                    {
                                        if (inputRef.SourceType == Enumerators.SourceType.Pathway
                                            && inputRef.SourceMixOrPathwayID == pw.Id)
                                        {
                                            DependentItem vDi = new DependentItem(vehicle);
                                            if (!dependentData.Contains(vDi))
                                            {
                                                dependentData.Add(vDi);
                                                if (returnAsap)
                                                    return dependentData;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }

                #region Dependent Mixes
                foreach (Mix m in _data.MixesData.Values)
                {
                    if (m.Discarded == false)
                    {
                        foreach (FuelProductionEntity fpe in m.Entities)
                            if (fpe is PathwayProductionEntity && (fpe as PathwayProductionEntity).PathwayReference == pw.Id
                                && (output == Guid.Empty || (fpe as PathwayProductionEntity).OutputReference == output))
                            {
                                if (excludeMixes)
                                {
                                    bool allZeros = true;
                                    foreach (Parameter p in fpe.Share.Values)
                                    {
                                        allZeros &= p.ValueInDefaultUnit == 0;
                                    }
                                    if (allZeros)
                                        continue;
                                }
                                DependentItem diMix = new DependentItem(m);
                                if (!dependentData.Contains(diMix))
                                {
                                    dependentData.Add(diMix);
                                    if (returnAsap)
                                        return dependentData;
                                    break;
                                }
                            }
                    }
                }
                #endregion

                #region Dependent Pathways
                foreach (Pathway pathVal in _data.PathwaysData.Values)
                {
                    if (pathVal.Discarded == false)
                    {
                        foreach (Vertex vertex in pathVal.VerticesData.Values)
                        {
                            if (vertex.Type == 1 && vertex.ModelID == pw.Id
                                    && (output == Guid.Empty || pathVal.EdgesData.Any(e => e.OutputID == output)))
                            {
                                DependentItem dp = new DependentItem(pathVal);
                                if (!dependentData.Contains(dp))
                                    dependentData.Add(dp);
                                if (returnAsap)
                                    return dependentData;
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region Dependent Prcoess Inputs, Displaced resources, sequestration energy
                foreach (AProcess proc in _data.ProcessesData.Values)
                {
                    if (proc.Discarded == false)
                    {
                        bool keepLookingOutputs = true;
                        foreach (Input inp in proc.FlattenInputList)
                        {
                            if (inp.SourceType == Enumerators.SourceType.Pathway
                                && inp.SourceMixOrPathwayID == pw.Id
                                && (output == Guid.Empty || mainOutput == output))
                            {
                                DependentItem diP = new DependentItem(proc);
                                if (!dependentData.Contains(diP))
                                {
                                    dependentData.Add(diP);
                                    keepLookingOutputs = false;
                                    if (returnAsap)
                                        return dependentData;
                                    break;
                                }
                            }
                            else if (inp.sequestrationFlag = true
                                && inp.sequestration != null
                                && inp.sequestration.SourceType == Enumerators.SourceType.Pathway
                                && inp.sequestration.PathwayOrMix == pw.Id)
                            {
                                DependentItem diP = new DependentItem(proc);
                                if (!dependentData.Contains(diP))
                                {
                                    dependentData.Add(diP);
                                    keepLookingOutputs = false;
                                    if (returnAsap)
                                        return dependentData;
                                    break;
                                }
                            }
                        }
                        if (keepLookingOutputs)
                        {
                            foreach (IIO allocatedOutput in proc.FlattenAllocatedOutputList)
                            {
                                if (allocatedOutput is CoProduct)
                                {
                                    CoProduct cop = allocatedOutput as CoProduct;
                                    if (cop.method == CoProductsElements.TreatmentMethod.displacement)
                                    {
                                        bool breakOuter = false;
                                        foreach (ConventionalProducts convProd in cop.ConventionalDisplacedResourcesList)
                                        {
                                            if (convProd.MaterialKey.SourceType == Enumerators.SourceType.Pathway
                                                && convProd.MaterialKey.SourceMixOrPathwayID == pw.Id)
                                            {
                                                DependentItem diP = new DependentItem(proc);
                                                if (!dependentData.Contains(diP))
                                                {
                                                    dependentData.Add(diP);
                                                    breakOuter = true;
                                                    if (returnAsap)
                                                        return dependentData;
                                                    break;
                                                }
                                            }
                                        }
                                        if (breakOuter)
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Dependent Transportation modes
                
                foreach (AMode mode in _data.ModesData.Values)
                {
                    if (mode.Discarded == false)
                    {
                        bool breakOuter = false;
                        foreach (ModeFuelShares mfs in mode.FuelSharesData.Values)
                        {
                            foreach (KeyValuePair<InputResourceReference, ModeEnergySource> pair in mfs.ProcessFuels)
                            {
                                InputResourceReference upstreamRef = pair.Key;
                                if (upstreamRef.SourceType == Enumerators.SourceType.Pathway
                                    && upstreamRef.SourceMixOrPathwayID == pw.Id)
                                {
                                    DependentItem diP = new DependentItem(mode);
                                    if (!dependentData.Contains(diP))
                                    {
                                        dependentData.Add(diP);
                                        if (returnAsap)
                                            return dependentData;
                                        breakOuter = true;
                                        break;
                                    }
                                }
                            }
                            if (breakOuter)
                                break;
                        }
                    }
                }

                #endregion

                #region Dependent Process Displaced-CoProducts
                foreach (AProcess proc in _data.ProcessesData.Values)
                {
                    if (proc.Discarded == false)
                    {
                        foreach (CoProduct cop in proc.CoProducts)
                        {
                            if (cop.method == CoProductsElements.TreatmentMethod.displacement)
                            {
                                foreach (ConventionalProducts displaced in cop.ConventionalDisplacedResourcesList)
                                {
                                    InputResourceReference inp = displaced.MaterialKey;

                                    if (inp.SourceType == Enumerators.SourceType.Pathway && inp.SourceMixOrPathwayID == pw.Id
                                        && (output == Guid.Empty || mainOutput == output))
                                    {
                                        DependentItem diP = new DependentItem(proc);
                                        if (!dependentData.Contains(diP))
                                        {
                                            dependentData.Add(diP);
                                            if (returnAsap)
                                                return dependentData;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Dependent Modes

                foreach (AMode mode in _data.ModesData.Values)
                {
                    if (mode.Discarded == false)
                    {
                        foreach (ModeFuelShares mfs in mode.FuelSharesData.Values)
                        {
                            foreach (ModeEnergySource fs in mfs.ProcessFuels.Values)
                            {
                                if (fs.ResourceReference.SourceType == Enumerators.SourceType.Pathway && fs.ResourceReference.SourceMixOrPathwayID == pw.Id)
                                {
                                    DependentItem dp = new DependentItem(mode);
                                    if (!dependentData.Contains(dp))
                                    {
                                        dependentData.Add(dp);
                                        if (returnAsap)
                                            return dependentData;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                return dependentData;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// List of all data that is discarded.
        /// </summary>
        /// <returns>List of the discarded items IDs</returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> discardedPathways()
        {
            List<int> discardedPathways = new List<int>();
            foreach (Pathway pw in _data.PathwaysData.Values)
            {
                if (pw.Discarded && discardedPathways.Contains(pw.Id) == false)
                    discardedPathways.Add(pw.Id);
            }
            return discardedPathways;
        }
        #endregion

        #region Technology
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> unUsedTechnologies()
        {
            List<int> usedTechnologies = this.usedTechnologies();
            List<int> unUsedTechnologies = new List<int>();

            foreach (TechnologyData techno in _data.TechnologiesData.Values)
            {
                if (techno.Discarded == false)
                {
                    if (usedTechnologies.Contains(techno.Id) == false && unUsedTechnologies.Contains(techno.Id) == false)
                        unUsedTechnologies.Add(techno.Id);
                }
            }

            return unUsedTechnologies;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> usedTechnologies()
        {
            List<int> usedTechnologies = new List<int>();

            foreach (TechnologyData td in _data.TechnologiesData.Values)
            {
                if (td.Discarded == false)
                {
                    List<IDependentItem> dependentData = TechnologyDependentItems(new DependentItem(td), true);
                    if (dependentData.Count > 0 && usedTechnologies.Contains(td.Id) == false)
                        usedTechnologies.Add(td.Id);
                }
            }

            return usedTechnologies;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idi"></param>
        /// <param name="returnAsap">If set to TRUE, the method will return when it hits the first dependency</param>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IDependentItem> TechnologyDependentItems(IDependentItem idi, bool returnAsap = false)
        {
            if (idi.Type == Enumerators.ItemType.Technology && _data.TechnologiesData.ContainsKey(idi.Id))
            {
                TechnologyData td = _data.TechnologiesData[idi.Id];
                List<IDependentItem> dependentData = new List<IDependentItem>();

                #region Dependent Processes
                foreach (AProcess process in _data.ProcessesData.Values)
                {
                    if (process.Discarded == false)
                    {
                        foreach (Input input in process.FlattenInputList)
                        {
                            foreach (TechnologyRef techno in input.Technologies)
                            {
                                if (techno.Reference == td.Id)
                                {
                                    DependentItem dp = new DependentItem(process);
                                    if (!dependentData.Contains(dp))
                                    {
                                        dependentData.Add(dp);
                                        if (returnAsap)
                                            return dependentData;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Dependent Technologies

                foreach (TechnologyData techno in _data.TechnologiesData.Values)
                {
                    if (techno.Discarded == false)
                    {
                        if (techno.BaseTechnology != -1 && techno.BaseTechnology == td.Id)
                        {
                            DependentItem dp = new DependentItem(techno);
                            if (!dependentData.Contains(dp))
                            {
                                dependentData.Add(dp);
                                if (returnAsap)
                                    return dependentData;
                            }
                        }
                    }
                }

                #endregion

                #region Dependent Modes

                foreach (AMode mode in _data.ModesData.Values)
                {
                    if (mode.Discarded == false)
                    {
                        foreach (ModeFuelShares mfs in mode.FuelSharesData.Values)
                        {
                            foreach (ModeEnergySource fs in mfs.ProcessFuels.Values)
                            {
                                if (fs.TechnologyFrom == td.Id
                                    || fs.TechnologyTo == td.Id)
                                {
                                    DependentItem dp = new DependentItem(mode);
                                    if (!dependentData.Contains(dp))
                                    {
                                        dependentData.Add(dp);
                                        if (returnAsap)
                                            return dependentData;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Dependent parameters

                Dictionary<string, Parameter> parameterList = new Dictionary<string, Parameter>();
                ToolsDataStructure.FindAllParameters(ref parameterList, new List<object>(), td);

                var parms = from p in _data.ParametersData.Values.AsParallel()
                            where !String.IsNullOrEmpty(p.CurrentFormula)
                            select p;
                if (parms.Count() < parameterList.Count)
                {
                    Parallel.ForEach(parameterList.Values, (dep, state) =>
                    {
                        bool broken = false;
                        foreach (Parameter p in parms)
                        {
                            if (!parameterList.Values.Contains(dep))//test that the parameter is not from the process itself
                                if (p.CurrentFormula.Contains("[" + dep.Id + "]") ||
                                    p.CurrentFormula.Contains("[" + dep.Name + "]"))
                                {
                                    dependentData.Add(new DependentItem(p));
                                    if (returnAsap)
                                    {
                                        broken = true;
                                        break;
                                    }
                                }
                        }
                        if (broken && returnAsap)
                            state.Break();
                    });
                }
                else
                {
                    Parallel.ForEach(parms, (dep, state) =>
                    {
                        bool broken = false;
                        foreach (Parameter p in parameterList.Values)
                        {
                            if (!parameterList.Values.Contains(dep))//test that the parameter is not from the process itself
                                if (dep.CurrentFormula.Contains("[" + p.Id + "]") ||
                                    dep.CurrentFormula.Contains("[" + p.Name + "]"))
                                {
                                    dependentData.Add(new DependentItem(p));
                                    if (returnAsap)
                                    {
                                        broken = true;
                                        break;
                                    }
                                }
                        }
                        if (broken && returnAsap)
                            state.Break();
                    });
                }

                #endregion

                return dependentData;
            }
            else
                return new List<IDependentItem>();
        }
        /// <summary>
        /// List of all data that is discarded.
        /// </summary>
        /// <returns>List of the discarded items IDs</returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> discardedTechnologies()
        {
            List<int> discardedTechnologies = new List<int>();
            foreach (TechnologyData pw in _data.TechnologiesData.Values)
            {
                if (pw.Discarded && discardedTechnologies.Contains(pw.Id) == false)
                    discardedTechnologies.Add(pw.Id);
            }
            return discardedTechnologies;
        }
        #endregion

        #region Process
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> usedProcesses()
        {
            List<int> usedProcesses = new List<int>();
            foreach (AProcess proc in _data.ProcessesData.Values)
            {
                if (proc.Discarded == false)
                {
                    List<IDependentItem> dependentItems = ProcessDependentItems(new DependentItem(proc), new Guid(), true);
                    if (dependentItems.Count > 0 && usedProcesses.Contains(proc.Id) == false)
                        usedProcesses.Add(proc.Id);
                }
            }
            return usedProcesses;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> unUsedProcesses()
        {
            List<int> unUsedProcesses = new List<int>();
            List<int> usedProcesses = this.usedProcesses();
            foreach (AProcess proc in _data.ProcessesData.Values)
            {
                if (proc.Discarded == false)
                {
                    if (usedProcesses.Contains(proc.Id) == false && unUsedProcesses.Contains(proc.Id) == false)
                        unUsedProcesses.Add(proc.Id);
                }

            }
            return unUsedProcesses;
        }
        /// <summary>
        /// Checks if a process is used in pathways, if the io Guid is provided we'll only check for the specified io
        /// otherwise we'll check for any of the input or outputs.
        /// </summary>
        /// <param name="idi">Process IDependentItem</param>
        /// <param name="io">Specific IO GUID if necessary to check only for a specific edge, keep new Guid() if not deisred to specify</param>
        /// <param name="returnAsap">If set to TRUE, the method will return when it hits the first dependency</param>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IDependentItem> ProcessDependentItems(IDependentItem idi, Guid io = new Guid(), bool returnAsap = false)
        {
            List<IDependentItem> dependentItems = new List<IDependentItem>();

            if (idi.Type == Enumerators.ItemType.Process && _data.ProcessesData.ContainsKey(idi.Id))
            {
                AProcess proc = _data.ProcessesData[idi.Id];

                #region Pathways
                foreach (Pathway pathway in _data.PathwaysData.Values)
                {
                    if (pathway.Discarded == false)
                    {
                        foreach (Vertex processRef in pathway.VerticesData.Values)
                        {
                            if (processRef.Type == 0 && processRef.ModelID == proc.Id)
                            {
                                if (io == Guid.Empty
                                    || pathway.EdgesData.Any(e => e.InputID == io || e.OutputID == io))
                                {
                                    DependentItem dp = new DependentItem(pathway);
                                    if (!dependentItems.Contains(dp))
                                    {
                                        dependentItems.Add(dp);
                                        if(returnAsap)
                                            return dependentItems;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Dependent parameters

                Dictionary<string, Parameter> parameterList = new Dictionary<string, Parameter>();
                ToolsDataStructure.FindAllParameters(ref parameterList, new List<object>(), proc);

                var parms = from p in _data.ParametersData.Values.AsParallel()
                            where !String.IsNullOrEmpty(p.CurrentFormula)
                            select p;
                if (parms.Count() < parameterList.Count)
                {
                    Parallel.ForEach(parameterList.Values, (dep, state) =>
                    {
                        bool broken = false;
                        foreach (Parameter p in parms)
                        {
                            if (!parameterList.Values.Contains(dep))//test that the parameter is not from the process itself
                                if (p.CurrentFormula.Contains("[" + dep.Id + "]") ||
                                    p.CurrentFormula.Contains("[" + dep.Name + "]"))
                                {
                                    dependentItems.Add(new DependentItem(p));
                                    if (returnAsap)
                                    {
                                        broken = true;
                                        break;
                                    }
                                }
                        }
                        if (broken && returnAsap)
                            state.Break();

                    });
                }
                else
                {
                    Parallel.ForEach(parms, (dep, state) =>
                    {
                        bool broken = false;
                        foreach (Parameter p in parameterList.Values)
                        {
                            if (!parameterList.Values.Contains(dep))//test that the parameter is not from the process itself
                                if (dep.CurrentFormula.Contains("[" + p.Id + "]") ||
                                    dep.CurrentFormula.Contains("[" + p.Name + "]"))
                                {
                                    dependentItems.Add(new DependentItem(p));
                                    if (returnAsap)
                                    {
                                        broken = true;
                                        break;
                                    }
                                }
                        }
                        if (broken && returnAsap)
                            state.Break();
                    });
                }

                #endregion
            }

            return dependentItems;

        }
        /// <summary>
        /// List of all data that is discarded.
        /// </summary>
        /// <returns>List of the discarded items IDs</returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> discardedProcesses()
        {
            List<int> discardedProcesses = new List<int>();
            foreach (AProcess pw in _data.ProcessesData.Values)
            {
                if (pw.Discarded && discardedProcesses.Contains(pw.Id) == false)
                    discardedProcesses.Add(pw.Id);
            }
            return discardedProcesses;
        }
        #endregion

        #region Mixes
        /// <summary>
        /// This method returns a list of mixes that are un-used by other parts of the sofware.
        /// In other words there is no other entitiy in Greet that is dependent on these mixes.
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> unUsedMixes()
        {
            List<int> unUsedMixes = new List<int>();
            List<int> usedMixes = this.usedMixes();

            foreach (Mix m in _data.MixesData.Values)
            {
                if (m.Discarded == false)
                {
                    if (usedMixes.Contains(m.Id) == false && unUsedMixes.Contains(m.Id) == false)//if the mixes is not used and not already in the list of not used mixes
                        unUsedMixes.Add(m.Id);
                }
            }

            return unUsedMixes;
        }
        /// <summary>
        /// This method returns a list of mixes that are used by other parts of the software.
        /// In other words there are entities in Greet dependent on the exisitence of these mixes.
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> usedMixes(bool excludeParameters = false)
        {
            List<int> usedMixes = new List<int>();

            foreach (Mix m in _data.MixesData.Values)
            {
                if (m.Discarded == false)
                {
                    List<IDependentItem> dependentData = MixDependentItems(new DependentItem(m), excludeParameters, true);
                    if (dependentData.Count > 0 && usedMixes.Contains(m.Id) == false)
                        usedMixes.Add(m.Id);
                }
            }

            return usedMixes;
        }
        /// <summary>
        /// List of all data that is dependent on the inputted object.
        /// </summary>
        /// <param name="idi"></param>
        /// <param name="excludeParameters">If set to true, does not performs a parameter exploration</param>
        /// <param name="returnAsap">If set to TRUE, the method will return when it hits the first dependency</param>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IDependentItem> MixDependentItems(IDependentItem idi, bool excludeParameters = false, bool returnAsap = true)
        {
            List<IDependentItem> dependentItems = new List<IDependentItem>();

            if (idi.Type == Enumerators.ItemType.Pathway_Mix && _data.MixesData.ContainsKey(idi.Id))
            {
                Mix m = _data.MixesData[idi.Id];

                #region Dependent Processes
                foreach (AProcess process in _data.ProcessesData.Values)
                {
                    if (process.Discarded == false)
                    {
                        bool isDependent = false;

                        #region inputs
                        if (isDependent == false)
                        {
                            foreach (Input input in process.FlattenInputList)
                            {
                                if (input.SourceMixOrPathwayID == m.Id && input.SourceType == Enumerators.SourceType.Mix
                                    && input.ResourceId == m.output.ResourceId)
                                {
                                    isDependent = true;
                                    break;
                                }

                            }
                        }

                        #endregion

                        #region coProducts
                        if (isDependent == false)
                        {
                            foreach (CoProduct cp in process.CoProducts)
                            {
                                if (cp.method == CoProductsElements.TreatmentMethod.displacement)
                                {
                                    foreach (ConventionalProducts convProd in cp.ConventionalDisplacedResourcesList)
                                    {
                                        if (convProd.MaterialKey.SourceType == Enumerators.SourceType.Mix && convProd.MaterialKey.ResourceId == m.output.ResourceId && convProd.MaterialKey.SourceMixOrPathwayID == m.Id)
                                        {
                                            isDependent = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        if (isDependent == true)
                        {
                            DependentItem dp = new DependentItem(process);
                            if (!dependentItems.Contains(dp))
                                dependentItems.Add(dp);
                            if (returnAsap)
                                return dependentItems;
                        }
                    }
                }
                #endregion

                #region Dependent Pathways

                //searches for all pathways that are using this mix as a feed and which are therefore dependent of it
                foreach (Pathway pathway in _data.PathwaysData.Values)
                {
                    if (pathway.Discarded == false)
                    {
                        foreach (Vertex vertex in pathway.VerticesData.Values)
                        {
                            if (vertex.Type == 2 && vertex.ModelID == m.Id)
                            {
                                DependentItem dp = new DependentItem(pathway);
                                if (!dependentItems.Contains(dp))
                                    dependentItems.Add(dp);
                                if (returnAsap)
                                    return dependentItems;
                                break;
                            }
                        }
                    }
                }
                #endregion

                #region Dependent Mixes

                foreach (Mix mix in _data.MixesData.Values)
                {
                    if (mix.Discarded == false)
                    {
                        foreach (FuelProductionEntity fpe in mix.Entities)
                        {
                            if (fpe is MixProductionEntity)
                            {
                                MixProductionEntity mpe = fpe as MixProductionEntity;
                                if (mpe.MixReference == m.Id)
                                {
                                    DependentItem dm = new DependentItem(mix);
                                    if (!dependentItems.Contains(dm))
                                        dependentItems.Add(dm);
                                    if (returnAsap)
                                        return dependentItems;
                                    break;
                                }
                            }
                        }
                    }
                }

                #endregion

                #region Dependent Vehicles
                foreach (Vehicle vehicle in _data.VehiclesData.Values)
                {
                    if (vehicle.Discarded == false)
                    {
                        foreach (VehicleOperationalMode mode in vehicle.Modes)
                        {
                            foreach (VehicleModePowerPlant plant in mode.Plants)
                            {
                                foreach (InputResourceReference fuel in plant.FuelUsed)
                                {
                                    if (fuel.SourceType == Enumerators.SourceType.Mix && fuel.ResourceId == m.output.ResourceId && fuel.SourceMixOrPathwayID == m.Id)
                                    {
                                        DependentItem dv = new DependentItem(vehicle);
                                        if (!dependentItems.Contains(dv))
                                            dependentItems.Add(dv);
                                        if (returnAsap)
                                            return dependentItems;
                                        break;
                                    }
                                }
                            }
                        }
                        foreach (VehicleManufacturing mf in vehicle.Manufacturing)
                        {
                            foreach (InputResourceReference fuel in mf.Materials)
                            {
                                if (fuel.SourceType == Enumerators.SourceType.Mix && fuel.ResourceId == m.output.ResourceId && fuel.SourceMixOrPathwayID == m.Id)
                                {
                                    DependentItem dv = new DependentItem(vehicle);
                                    if (!dependentItems.Contains(dv))
                                        dependentItems.Add(dv);
                                    if (returnAsap)
                                        return dependentItems;
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Dependent Modes

                foreach (AMode mode in _data.ModesData.Values)
                {
                    if (mode.Discarded == false)
                    {
                        foreach (ModeFuelShares mfs in mode.FuelSharesData.Values)
                        {
                            foreach (ModeEnergySource fs in mfs.ProcessFuels.Values)
                            {
                                if (fs.ResourceReference.SourceType == Enumerators.SourceType.Mix && fs.ResourceReference.SourceMixOrPathwayID == m.Id)
                                {
                                    DependentItem dp = new DependentItem(mode);
                                    if (!dependentItems.Contains(dp))
                                    {
                                        dependentItems.Add(dp);
                                        if (returnAsap)
                                            return dependentItems;
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                if (!excludeParameters)
                {
                    #region Dependent parameters

                    Dictionary<string, Parameter> parameterList = new Dictionary<string, Parameter>();
                    ToolsDataStructure.FindAllParameters(ref parameterList, new List<object>(), m);

                    var parms = from p in _data.ParametersData.Values.AsParallel()
                                where !String.IsNullOrEmpty(p.CurrentFormula)
                                select p;
                    if (parms.Count() < parameterList.Count)
                    {
                        Parallel.ForEach(parameterList.Values, (dep, state) =>
                        {
                            bool broken = false;
                            foreach (Parameter p in parms)
                            {
                                if (!parameterList.Values.Contains(dep))//test that the parameter is not from the process itself
                                    if (p.CurrentFormula.Contains("[" + dep.Id + "]") ||
                                        p.CurrentFormula.Contains("[" + dep.Name + "]"))
                                    {
                                        dependentItems.Add(new DependentItem(p));
                                        if (returnAsap)
                                        {
                                            broken = true;
                                            break;
                                        }
                                    }
                            }
                            if (broken && returnAsap)
                                state.Break();

                        });
                    }
                    else
                    {
                        Parallel.ForEach(parms, (dep, state) =>
                        {
                            bool broken = false;
                            foreach (Parameter p in parameterList.Values)
                            {
                                if (!parameterList.Values.Contains(dep))//test that the parameter is not from the process itself
                                    if (dep.CurrentFormula.Contains("[" + p.Id + "]") ||
                                        dep.CurrentFormula.Contains("[" + p.Name + "]"))
                                    {
                                        dependentItems.Add(new DependentItem(p));
                                        if (returnAsap)
                                        {
                                            broken = true;
                                            break;
                                        }
                                    }
                            }
                            if (broken && returnAsap)
                                state.Break();
                        });
                    }

                    #endregion
                }
            }

            return dependentItems;
        }
        /// <summary>
        /// List of all data that is discarded.
        /// </summary>
        /// <returns>List of the discarded items IDs</returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> discardedMixes()
        {
            List<int> discardedMixes = new List<int>();
            foreach (Mix pw in _data.MixesData.Values)
            {
                if (pw.Discarded && discardedMixes.Contains(pw.Id) == false)
                    discardedMixes.Add(pw.Id);
            }
            return discardedMixes;
        }
        #endregion

        #region Resources
        /// <summary>
        /// This method returns a list of resources that are un-used by other parts of the sofware.
        /// In other words there is no other entitiy in Greet that is dependent on these resources.
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> unUsedResources()
        {
            List<int> unUsedResources = new List<int>();
            List<int> usedResources = this.usedResources();

            foreach (ResourceData r in _data.ResourcesData.Values)
            {
                if (r.Discarded == false)
                {
                    if (usedResources.Contains(r.Id) == false && unUsedResources.Contains(r.Id) == false)//if the mixes is not used and not already in the list of not used mixes
                        unUsedResources.Add(r.Id);
                }
            }

            return unUsedResources;
        }
        /// <summary>
        /// This method returns a list of resources that are used by other parts of the software.
        /// In other words there are entities in Greet dependent on the exisitence of these resources.
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> usedResources()
        {
            List<int> usedResources = new List<int>();

            foreach (ResourceData r in _data.ResourcesData.Values)
            {
                if (r.Discarded == false)
                {
                    List<IDependentItem> dependentData = ResourceDependentItems(new DependentItem(r), true);
                    if (dependentData.Count > 0 && usedResources.Contains(r.Id) == false)
                        usedResources.Add(r.Id);
                }
            }

            return usedResources;
        }
        /// <summary>
        /// List of all data that is dependent on the inputted object.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="returnAsap">If set to TRUE, the method will return when it hits the first dependency</param>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IDependentItem> ResourceDependentItems(IDependentItem r, bool returnAsap = false)
        {
            List<IDependentItem> dependentItems = new List<IDependentItem>();

            ResourceData rd = _data.ResourcesData[r.Id];

            #region Dependent Processes
            foreach (AProcess process in _data.ProcessesData.Values)
            {
                if (process.Discarded == false)
                {
                    bool isDependent = false;

                    #region inputs
                    if (isDependent == false)
                        foreach (Input input in process.FlattenInputList)
                            if (input.resourceId == r.Id)
                            {
                                isDependent = true;
                                break;
                            }

                    #endregion

                    #region coProducts
                    if (isDependent == false)
                        foreach (CoProduct cp in process.CoProducts)
                            if (cp.ResourceId == r.Id)
                            {
                                isDependent = true;
                                break;
                            }

                    #endregion

                    if (isDependent == false && process.MainOutput.ResourceId == r.Id)
                        isDependent = true;

                    if (isDependent == true)
                    {
                        DependentItem dp = new DependentItem(process);
                        if (!dependentItems.Contains(dp))
                            dependentItems.Add(dp);
                        if (returnAsap)
                            return dependentItems;
                    }
                }
            }
            #endregion Processes

            #region Dependent Pathways
            foreach (Pathway pathway in _data.PathwaysData.Values)
            {
                if (pathway.Discarded == false)
                {
                    foreach (PMOutput output in pathway.OutputsData)
                    {
                        if (output.ResourceId == r.Id)
                        {
                            DependentItem dp = new DependentItem(pathway);
                            if (!dependentItems.Contains(dp))
                                dependentItems.Add(dp);
                            if (returnAsap)
                                return dependentItems;
                            break;
                        }
                    }
                }
            }

            #endregion

            #region Dependent Technologies

            foreach (TechnologyData td in _data.TechnologiesData.Values.Where(item => item.InputResourceRef == r.Id))
            {
                if (td.Discarded == false)
                {
                    DependentItem dt = new DependentItem(td);
                    if (dependentItems.Contains(dt) == false)
                        dependentItems.Add(dt);
                    if (returnAsap)
                        return dependentItems;
                }
            }

            #endregion

            #region Dependent Vehicles
            foreach (Vehicle vehicle in _data.VehiclesData.Values)
            {
                if (vehicle.Discarded == false)
                {
                    foreach (VehicleOperationalMode mode in vehicle.Modes)
                    {
                        foreach (VehicleModePowerPlant plant in mode.Plants)
                        {
                            foreach (InputResourceReference fuel in plant.FuelUsed)
                            {
                                if (fuel.ResourceId == r.Id)
                                {
                                    DependentItem dv = new DependentItem(vehicle);
                                    if (!dependentItems.Contains(dv))
                                        dependentItems.Add(dv);
                                    if (returnAsap)
                                        return dependentItems;
                                    break;
                                }
                            }
                        }
                    }
                    foreach (VehicleManufacturing mf in vehicle.Manufacturing)
                    {
                        foreach (InputResourceReference fuel in mf.Materials)
                        {
                            if (fuel.ResourceId == r.Id)
                            {
                                DependentItem dv = new DependentItem(vehicle);
                                if (!dependentItems.Contains(dv))
                                    dependentItems.Add(dv);
                                if (returnAsap)
                                    return dependentItems;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Dependent Modes
            foreach (AMode m in _data.ModesData.Values)
            {
                if (m.Discarded == false)
                {
                    bool isDependent = false;
                    if (m is ModeTruck)
                    {
                        if ((m as ModeTruck).Payload.Keys.Any(item => item == r.Id))
                            isDependent = true;
                    }
                    else if (m is ModeTankerBarge)
                    {
                        if ((m as ModeTankerBarge).Payload.Keys.Any(item => item == r.Id))
                            isDependent = true;
                    }
                    else if (m is ModePipeline)
                    {
                        if ((m as ModePipeline).EnergyIntensity.Keys.Any(item => item == r.Id))
                            isDependent = true;
                    }

                    if (isDependent == true)
                    {
                        DependentItem di = new DependentItem(m);
                        if (!dependentItems.Contains(di))
                            dependentItems.Add(di);
                        if (returnAsap)
                            return dependentItems;
                    }
                }
            }
            #endregion

            #region Dependent parameters

            Dictionary<string, Parameter> parameterList = new Dictionary<string, Parameter>();
            ToolsDataStructure.FindAllParameters(ref parameterList, new List<object>(), rd);

            var parms = from p in _data.ParametersData.Values.AsParallel()
                        where !String.IsNullOrEmpty(p.CurrentFormula)
                        select p;
            if (parms.Count() < parameterList.Count)
            {
                Parallel.ForEach(parameterList.Values, (dep, state) =>
                {
                    bool broken = false;
                    foreach (Parameter p in parms)
                    {
                        if (!parameterList.Values.Contains(dep))//test that the parameter is not from the process itself
                            if (p.CurrentFormula.Contains("[" + dep.Id + "]") ||
                                p.CurrentFormula.Contains("[" + dep.Name + "]"))
                            {
                                dependentItems.Add(new DependentItem(p));
                                if (returnAsap)
                                {
                                    broken = true;
                                    break;
                                }
                            }
                    }
                    if (broken && returnAsap)
                        state.Break();

                });
            }
            else
            {
                Parallel.ForEach(parms, (dep, state) =>
                {
                    bool broken = false;
                    foreach (Parameter p in parameterList.Values)
                    {
                        if (!parameterList.Values.Contains(dep))//test that the parameter is not from the process itself
                            if (dep.CurrentFormula.Contains("[" + p.Id + "]") ||
                                dep.CurrentFormula.Contains("[" + p.Name + "]"))
                            {
                                dependentItems.Add(new DependentItem(p));
                                if (returnAsap)
                                {
                                    broken = true;
                                    break;
                                }
                            }
                    }
                    if (broken && returnAsap)
                        state.Break();
                });
            }

            #endregion

            #region dependent mixes

            foreach (Mix mix in _data.MixesData.Values)
            {
                if (mix.Discarded == false)
                {
                    if (mix.output != null)
                    {
                        if (mix.output.ResourceId == r.Id)
                        {
                            DependentItem dp = new DependentItem(mix);
                            if (!dependentItems.Contains(dp))
                                dependentItems.Add(dp);
                            if (returnAsap)
                                return dependentItems;
                            break;
                        }
                    }
                }
            }
            #endregion

            #region dependent compatible resources

            foreach (ResourceData res in _data.ResourcesData.Values)
            {
                if (res.Discarded == false)
                {
                    foreach (int compat in res.CompatibilityIds)
                    {
                        if (compat == res.Id)
                        {
                            DependentItem dp = new DependentItem(res);
                            if (!dependentItems.Contains(dp))
                                dependentItems.Add(dp);
                            if (returnAsap)
                                return dependentItems;
                            break;
                        }
                    }
                }
            }

            #endregion

            return dependentItems;
        }
        /// <summary>
        /// List of all data that is discarded.
        /// </summary>
        /// <returns>List of the discarded items IDs</returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> discardedResources()
        {
            List<int> discardedResources = new List<int>();
            foreach (ResourceData pw in _data.ResourcesData.Values)
            {
                if (pw.Discarded && discardedResources.Contains(pw.Id) == false)
                    discardedResources.Add(pw.Id);
            }
            return discardedResources;
        }
        #endregion

        #region Vehicle
        /// <summary>
        /// This method returns a list of vehicles that are un-used by other parts of the sofware.
        /// In other words there is no other entitiy in Greet that is dependent on these vehicles.
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> unUsedVehicles()
        {
            List<int> unUsedVehiclesIds = new List<int>();
            List<int> usedVehiclesIds = usedVehicles();

            foreach (Vehicle v in _data.VehiclesData.Values)
            {
                if (v.Discarded == false)
                {
                    if (usedVehiclesIds.Contains(v.Id) == false)
                        unUsedVehiclesIds.Add(v.Id);
                }
            }

            return unUsedVehiclesIds;
        }
        /// <summary>
        /// This method returns a list of vehicles that are used by other parts of the software.
        /// In other words there are entities in Greet dependent on the exisitence of these vehicles.
        /// </summary>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> usedVehicles()
        {
            List<int> usedVehicles = new List<int>();
            foreach (Vehicle v1 in _data.VehiclesData.Values)
            {
                if (v1.Discarded == false)
                {
                    List<IDependentItem> dependentData = VehicleDependentItems(new DependentItem(v1), true);
                    if (dependentData.Count > 0 && usedVehicles.Contains(v1.Id) == false)
                        usedVehicles.Add(v1.Id);
                }
            }

            return usedVehicles;
        }
        /// <summary>
        /// List of all data that is dependent on the inputted object.
        /// </summary>
        /// <param name="idi"></param>
        /// <param name="returnAsap">If set to TRUE, the method will return when it hits the first dependency</param>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IDependentItem> VehicleDependentItems(IDependentItem idi, bool returnAsap = false)
        {
            if (idi.Type == Enumerators.ItemType.Vehicle && _data.VehiclesData.ContainsKey(idi.Id))
            {
                Vehicle veh = _data.VehiclesData[idi.Id];
                List<IDependentItem> dependentItems = new List<IDependentItem>();

                #region Dependent parameters

                Dictionary<string, Parameter> parameterList = new Dictionary<string, Parameter>();
                ToolsDataStructure.FindAllParameters(ref parameterList, new List<object>(), veh);

                var parms = from p in _data.ParametersData.Values.AsParallel()
                            where !String.IsNullOrEmpty(p.CurrentFormula)
                            select p;
                if (parms.Count() < parameterList.Count)
                {
                    Parallel.ForEach(parameterList.Values, (dep, state) =>
                    {
                        bool broken = false;
                        foreach (Parameter p in parms)
                        {
                            if (!parameterList.Values.Contains(dep))//test that the parameter is not from the process itself
                                if (p.CurrentFormula.Contains("[" + dep.Id + "]") ||
                                    p.CurrentFormula.Contains("[" + dep.Name + "]"))
                                {
                                    dependentItems.Add(new DependentItem(p));
                                    if (returnAsap)
                                    {
                                        broken = true;
                                        break;
                                    }
                                }
                        }
                        if (broken && returnAsap)
                            state.Break();

                    });
                }
                else
                {
                    Parallel.ForEach(parms, (dep, state) =>
                    {
                        bool broken = false;
                        foreach (Parameter p in parameterList.Values)
                        {
                            if (!parameterList.Values.Contains(dep))//test that the parameter is not from the process itself
                                if (dep.CurrentFormula.Contains("[" + p.Id + "]") ||
                                    dep.CurrentFormula.Contains("[" + p.Name + "]"))
                                {
                                    dependentItems.Add(new DependentItem(p));
                                    if (returnAsap)
                                    {
                                        broken = true;
                                        break;
                                    }
                                }
                        }
                        if (broken && returnAsap)
                            state.Break();
                    });
                }

                #endregion

                return dependentItems;
            }
            else
                return null;
        }
        /// <summary>
        /// List of all data that is discarded.
        /// </summary>
        /// <returns>List of the discarded items IDs</returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<int> discardedVehicles()
        {
            List<int> discardedVehicles = new List<int>();
            foreach (Vehicle pw in _data.VehiclesData.Values)
            {
                if (pw.Discarded && discardedVehicles.Contains(pw.Id) == false)
                    discardedVehicles.Add(pw.Id);
            }
            return discardedVehicles;
        }
        #endregion

        #endregion

        #region Specific methods
        /// <summary>
        /// This is a list of all mixes and pathways that output a given resource.
        /// </summary>
        /// <param name="resourceId">The resource id whose list of possible ways is needed</param>
        /// <returns>The return type is a list of InputResourceReference's because this object can represent a pathway or a mix depending on the source attribute of the object</returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<IInputResourceReference> ProduceResource(int resourceId)
        {
            return _data.GetListOfMixesAndPathways(resourceId).ToList<IInputResourceReference>();
        }
        /// <summary>
        /// Returns the resource ID of the main output for a given pathway
        /// </summary>
        /// <param name="pathwayId"></param>
        /// <returns></returns>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public int PathwayMainOutputResouce(int pathwayId)
        {
            if (_data.PathwaysData.ContainsKey(pathwayId))
            {
                Pathway pw = _data.PathwaysData[pathwayId];
                return pw.MainOutputResourceID;
            }
            else
                return -1;
        }
        #endregion

    }
}
