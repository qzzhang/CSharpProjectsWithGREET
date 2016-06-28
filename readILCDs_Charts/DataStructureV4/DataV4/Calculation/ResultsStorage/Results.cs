using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greet.UnitLib3;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4.ResultsStorage
{
    [Serializable]
    public class Results : IResults
    {
        /// <summary>
        /// This attriobute stores the energy and emissions associated with all the inputs fuels production as well as with combustion emissions.
        /// It means that it contains the upstream part for the process fuel and the main input of a stationary process
        /// </summary>
        public Enem wellToProductEnem = new Enem();
        /// <summary>
        /// Emissions associated with cumbustion processes of the process fuels and non-combustion emissions
        /// </summary>
        public EmissionAmounts onsiteEmissions = new EmissionAmounts();
        /// <summary>
        /// This attribute is used for showing list of inputs in the GUI
        /// it stores the energy amount of the main input and the process fuels
        /// </summary>
        public ResourceAmounts onsiteResources = new ResourceAmounts();
        /// <summary>
        /// This attribute store the losses emissions for this process
        /// </summary>
        public EmissionAmounts lossesEmissions = new EmissionAmounts();
        /// <summary>
        /// This attributes stores the losses energy only for this process
        /// </summary>
        public ResourceAmounts lossesAmounts = new ResourceAmounts();
        /// <summary>
        /// Other emissions, or group emissions are putted here
        /// Those emissions are not coming neither from the technologies, nor the losses
        /// They are just random emissions added to the process or the group
        /// </summary>
        public EmissionAmounts staticEmissions = new EmissionAmounts();
        /// <summary>
        /// Total urban emissions
        /// </summary>
        public EmissionAmounts wellToProductUrbanEmission = new EmissionAmounts();
        /// <summary>
        /// Urban emissions that are produced locally
        /// </summary>
        public EmissionAmounts onsiteUrbanEmissions = new EmissionAmounts();

        #region Constructors

        public Results()
        {
            
        }

        #endregion

        #region operators

        public static Results operator *(double e1, Results e2)
        {
            Results opRes = new Results();
            opRes.wellToProductEnem = e1 * e2.wellToProductEnem;
            opRes.onsiteEmissions = e1 * e2.onsiteEmissions;
            opRes.onsiteResources = e1 * e2.onsiteResources;
            opRes.lossesEmissions = e1 * e2.lossesEmissions;
            opRes.lossesAmounts = e1 * e2.lossesAmounts;
            opRes.staticEmissions = e1 * e2.staticEmissions;
            opRes.wellToProductUrbanEmission = e1 * e2.wellToProductUrbanEmission;
            opRes.onsiteUrbanEmissions = e1 * e2.onsiteUrbanEmissions;
            return opRes;          
        }

        public static Results operator *(Results e2, double e1)
        {
            Results opRes = new Results();
            opRes.wellToProductEnem = e1 * e2.wellToProductEnem;
            opRes.onsiteEmissions = e1 * e2.onsiteEmissions;
            opRes.onsiteResources = e1 * e2.onsiteResources;
            opRes.lossesEmissions = e1 * e2.lossesEmissions;
            opRes.lossesAmounts = e1 * e2.lossesAmounts;
            opRes.staticEmissions = e1 * e2.staticEmissions;
            opRes.wellToProductUrbanEmission = e1 * e2.wellToProductUrbanEmission;
            opRes.onsiteUrbanEmissions = e1 * e2.onsiteUrbanEmissions;
            return opRes;
        }

        public static Results operator / (Results e1, LightValue e2)
        {
            Results opRes = new Results();
            opRes.wellToProductEnem = e1.wellToProductEnem / e2;
            opRes.onsiteEmissions = e1.onsiteEmissions / e2;
            opRes.onsiteResources = e1.onsiteResources / e2;
            opRes.lossesEmissions = e1.lossesEmissions/ e2;
            opRes.lossesAmounts = e1.lossesAmounts/ e2;
            opRes.staticEmissions = e1.staticEmissions/ e2;
            opRes.wellToProductUrbanEmission = e1.wellToProductUrbanEmission/ e2;
            opRes.onsiteUrbanEmissions = e1.onsiteUrbanEmissions/ e2;
            return opRes;  
        }

        public static Results operator /(Results e1, double e2)
        {
            Results opRes = new Results();
            opRes.wellToProductEnem = e1.wellToProductEnem / e2;
            opRes.onsiteEmissions = e1.onsiteEmissions / e2;
            opRes.onsiteResources = e1.onsiteResources / e2;
            opRes.lossesEmissions = e1.lossesEmissions / e2;
            opRes.lossesAmounts = e1.lossesAmounts / e2;
            opRes.staticEmissions = e1.staticEmissions / e2;
            opRes.wellToProductUrbanEmission = e1.wellToProductUrbanEmission / e2;
            opRes.onsiteUrbanEmissions = e1.onsiteUrbanEmissions / e2;
            return opRes;
        }

        public static Results operator - (Results e1, Results e2)
        {
            Results opRes = new Results();
            opRes.wellToProductEnem = e1.wellToProductEnem - e2.wellToProductEnem;
            opRes.onsiteEmissions = e1.onsiteEmissions - e2.onsiteEmissions;
            opRes.onsiteResources = e1.onsiteResources - e2.onsiteResources;
            opRes.lossesEmissions = e1.lossesEmissions - e2.lossesEmissions;
            opRes.lossesAmounts = e1.lossesAmounts - e2.lossesAmounts;
            opRes.staticEmissions = e1.staticEmissions - e2.staticEmissions;
            opRes.wellToProductUrbanEmission = e1.wellToProductUrbanEmission - e2.wellToProductUrbanEmission;
            opRes.onsiteUrbanEmissions = e1.onsiteUrbanEmissions - e2.onsiteUrbanEmissions;
            return opRes;
        }

        public static Results operator + (Results e1, Results e2)
        {
            Results opRes = new Results();
            opRes.wellToProductEnem = e1.wellToProductEnem + e2.wellToProductEnem;
            opRes.onsiteEmissions = e1.onsiteEmissions + e2.onsiteEmissions;
            opRes.onsiteResources = e1.onsiteResources + e2.onsiteResources;
            opRes.lossesEmissions = e1.lossesEmissions + e2.lossesEmissions;
            opRes.lossesAmounts = e1.lossesAmounts + e2.lossesAmounts;
            opRes.staticEmissions = e1.staticEmissions + e2.staticEmissions;
            opRes.wellToProductUrbanEmission = e1.wellToProductUrbanEmission + e2.wellToProductUrbanEmission;
            opRes.onsiteUrbanEmissions = e1.onsiteUrbanEmissions + e2.onsiteUrbanEmissions;
            return opRes;
        }

        #endregion

        /// <summary>
        /// Get or set the common bottom unit property for all results variables
        /// When Getting the common bottom, and if the bottom is not homogenous, throw an exception : Different bottoms detected
        /// </summary>
        [Obsolete("For compatibility with old 2014 API, please use BottomDim instead")]
        public string FunctionalUnit 
        {
            get 
            {
                if (this.wellToProductEnem.BottomDim == this.wellToProductUrbanEmission.BottomDim
                    && this.wellToProductUrbanEmission.BottomDim == this.lossesAmounts.BottomDim//Check to compensate for the 3 lines commented above
                    && this.lossesAmounts.BottomDim == this.lossesEmissions.BottomDim
                    && this.lossesEmissions.BottomDim == this.onsiteResources.BottomDim
                    && this.onsiteResources.BottomDim == this.onsiteEmissions.BottomDim
                    && this.onsiteEmissions.BottomDim == onsiteUrbanEmissions.BottomDim
                    && onsiteUrbanEmissions.BottomDim == this.staticEmissions.BottomDim)
                {
                    AQuantity qty = Units.QuantityList.ByDim(this.wellToProductEnem.BottomDim);
                    return qty.SiUnit.Expression;
                }
                else
                    throw new Exception("Different bottom units detected for different results");

            }
            set 
            {
                Unit unit = Units.UnitsList.Values.FirstOrDefault(item => item.Expression == value);
                if (unit == null)
                {
                    Units.UnitsList.TryGetValue(value, out unit);
                    if (unit == null)
                        throw new Exception("Unrecognized unit, you must use a unit from the UnitLib3 UnitList. Please check available units, or report to developing team");
                }

                AQuantity qty = Units.QuantityList.Values.FirstOrDefault(item => item.Units.Contains(unit));
                if (qty == null)
                    throw new Exception("Failed to find a quantity associated with the unit " + unit.Name + ". Please check available units, or report to developing team");

                this.wellToProductEnem.BottomDim = qty.Dim;
                this.wellToProductUrbanEmission.BottomDim = qty.Dim;
                this.lossesAmounts.BottomDim = qty.Dim;
                this.lossesEmissions.BottomDim = qty.Dim;
                this.onsiteResources.BottomDim = qty.Dim;
                this.onsiteEmissions.BottomDim = qty.Dim;
                this.onsiteUrbanEmissions.BottomDim = qty.Dim;
                this.staticEmissions.BottomDim = qty.Dim;
            }
        }

        /// <summary>
        /// Get or set the common bottom unit property for all results variables
        /// When Getting the common bottom, and if the bottom is not homogenous, throw an exception : Different bottoms detected
        /// </summary>
        public uint BottomDim
        {
            get
            {
                if (this.wellToProductEnem.BottomDim == this.wellToProductUrbanEmission.BottomDim
                    && this.wellToProductUrbanEmission.BottomDim == this.lossesAmounts.BottomDim //Check to compensate for the 3 lines commented above
                    && this.lossesAmounts.BottomDim == this.lossesEmissions.BottomDim
                    && this.lossesEmissions.BottomDim == this.onsiteResources.BottomDim
                    && this.onsiteResources.BottomDim == this.onsiteEmissions.BottomDim
                    && this.onsiteEmissions.BottomDim == onsiteUrbanEmissions.BottomDim
                    && onsiteUrbanEmissions.BottomDim == this.staticEmissions.BottomDim)
                {
                    return this.wellToProductEnem.BottomDim;
                }
                else
                    throw new Exception("Different bottom units detected for different results");

            }
            set
            {
                this.wellToProductEnem.BottomDim = value;
                this.wellToProductUrbanEmission.BottomDim = value;
                this.lossesAmounts.BottomDim = value;
                this.lossesEmissions.BottomDim = value;
                this.onsiteResources.BottomDim = value;
                this.onsiteEmissions.BottomDim = value;
                this.onsiteUrbanEmissions.BottomDim = value;
                this.staticEmissions.BottomDim = value;
            }
        }

        #region IResults

        #region private attributes

        /// <summary>
        /// string representing the kind of result object we have, the type can be
        /// mix, pathway, process, vehicle, emissions, resources, lifecycle, onsite, group
        /// </summary>
        private Enumerators.ItemType _ObjectType;

        /// <summary>
        /// The ID associated with the objectType if any is available, otherwise this value can be set as -1
        /// </summary>
        private int _ObjectID;

        /// <summary>
        /// The preferred displayed amount and unit preference associated with the results for use in the model
        /// This object should not be revealed through IResults to avoid coupling the Model and interface objects
        /// </summary>
        private FunctionalUnitPreference _customUnitPreference = new FunctionalUnitPreference();

        /// <summary>
        /// Biogenic Carbon ratio for the resource produced and associated with that upstream
        /// </summary>
        private double massBiongenicCarbonRatio = 0;

      
        
        #endregion

        #region public Accessors/Methods

        public Enumerators.ItemType ObjectType
        {
            get { return _ObjectType; }
            set { _ObjectType = value; }
        }

        public int ObjectID
        {
            get { return _ObjectID; }
            set { _ObjectID = value; }
        }

        public FunctionalUnitPreference CustomFunctionalUnitPreference
        {
            get { return _customUnitPreference; }
            set { _customUnitPreference = value; }
        }

        public double BiongenicCarbonRatio
        {
            get { return massBiongenicCarbonRatio; }
            set { massBiongenicCarbonRatio = value; }
        }

        /// <summary>
        /// Emissions that were produced on site for a process or a vehicle, basically it includes anything relevant
        /// to the current object except the upstream values
        /// </summary>
        public Dictionary<int, IValue> OnSiteEmissions()
        {
            return this.onsiteEmissions.ToInterfaceDictionary();
        }

        /// <summary>
        /// Resources that were used on site for a process or a vehicle, basically it includes anything relevant
        /// to the current object except the upstream values
        /// </summary>
        public Dictionary<int, IValue> OnSiteResources()
        {
           return this.onsiteResources.ToInterfaceDictionary();
        }

        /// <summary>
        /// Urban emissions that were produced on site for a process or a vehicle, basically it includes anything relevant
        /// to the current object except the upstream values
        /// </summary
        public Dictionary<int, IValue> OnSiteUrbanEmissions()
        {
            return this.onsiteUrbanEmissions.ToInterfaceDictionary();
        }

        /// <summary>
        /// Emissions that were produced on site and upstream, these are live cycle values
        /// </summary
        public Dictionary<int, IValue> WellToProductEmissions()
        {
            return this.wellToProductEnem.emissions.ToInterfaceDictionary();
        }

        /// <summary>
        /// Resources that were used on site and upstream, these are live cycle values
        /// </summary
        public Dictionary<int, IValue> WellToProductResources()
        {
            return this.wellToProductEnem.materialsAmounts.ToInterfaceDictionary();
        }

        /// <summary>
        /// Urban emissions that were produced on site and upstream, these are live cycle values
        /// </summary
        public Dictionary<int, IValue> WellToProductUrbanEmissions()
        {
            return this.wellToProductUrbanEmission.ToInterfaceDictionary();
        }

        /// <summary>
        /// Emissions that were produced on site for a process or a vehicle, basically it includes anything relevant
        /// to the current object except the upstream values
        /// </summary>
        public Dictionary<int, IValue> OnSiteEmissionsGroups(IData data)
        {
            return this.onsiteEmissions.GroupsToInterfaceDictionary(data as GData);
        }

        /// <summary>
        /// Resources that were used on site for a process or a vehicle, basically it includes anything relevant
        /// to the current object except the upstream values
        /// </summary>
        public Dictionary<int, IValue> OnSiteResourcesGroups(IData data)
        {
            return this.onsiteResources.GroupsToInterfaceDictionary(data as GData);
        }

        /// <summary>
        /// Urban emissions that were produced on site for a process or a vehicle, basically it includes anything relevant
        /// to the current object except the upstream values
        /// </summary
        public Dictionary<int, IValue> OnSiteUrbanEmissionsGroups(IData data)
        {
            return this.onsiteUrbanEmissions.GroupsToInterfaceDictionary(data as GData); 
        }

        /// <summary>
        /// Emissions that were produced on site and upstream, these are live cycle values
        /// </summary
        public Dictionary<int, IValue> WellToProductEmissionsGroups(IData data)
        {
            return this.wellToProductEnem.emissions.GroupsToInterfaceDictionary(data as GData); 
        }

        /// <summary>
        /// Resources that were used on site and upstream, these are live cycle values
        /// </summary
        public Dictionary<int, IValue> WellToProductResourcesGroups(IData data)
        {
            return this.wellToProductEnem.materialsAmounts.GroupsToInterfaceDictionary(data as GData); 
        }

        /// <summary>
        /// Urban emissions that were produced on site and upstream, these are live cycle values
        /// </summary
        public Dictionary<int, IValue> WellToProductUrbanEmissionsGroups(IData data)
        {
            return this.wellToProductUrbanEmission.GroupsToInterfaceDictionary(data as GData);
        }


        //public Dictionary<int, IValue> LifeCycleEmissions(GData gData, int _producedResourceId, LightValue functionalUnit)
        //{
        //    if (biongenicCarbonRatio == -1)
        //        throw new Exception("Cannot calculate the CO2 if that product is used with a biogenic carbon ratio of -1, this must be set to a zero or positive ratio");

        //    return this.wellToProductEnem.emissions.ToInterfaceDictionaryLifeCycle(gData, _producedResourceId, functionalUnit, this.biongenicCarbonRatio);
        //}

        //public Dictionary<int, IValue> LifeCycleEmissionsGroups(GData gData, int _producedResourceId, LightValue functionalUnit)
        //{
        //    if (biongenicCarbonRatio == -1)
        //        throw new Exception("Cannot calculate the CO2 if that product is used with a biogenic carbon ratio of -1, this must be set to a zero or positive ratio");

        //    return this.wellToProductEnem.emissions.GroupsToInterfaceDictionaryLifeCycle(gData, _producedResourceId, functionalUnit, this.biongenicCarbonRatio);
        //}

        #endregion

        #endregion

    }
}
