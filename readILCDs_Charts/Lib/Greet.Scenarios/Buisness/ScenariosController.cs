// *********************************************************************** 
//  COPYRIGHT NOTIFICATION 
// 
//  Email contact: greet@anl.gov 
//  Copyright (c) 2012, UChicago Argonne, LLC 
//  All Rights Reserved
//  
//  THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
//  LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
//  CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
//  NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 
//  
//  ************************************************************************ 
//  ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
//  IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
//  ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
//  ENERGY. 
//  ************************************************************************
//   
//  ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4;
using Greet.DataStructureV4.Entities;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.Lib.Scenarios.Entities;
using Greet.Model.Interfaces;
using Greet.UnitLib3;

namespace Greet.Lib.Scenarios.Buisness
{
    public static class ScenariosController
    {
        #region Public Delegates And Enum

        public delegate void MessageEventHandler(string message);

        public enum ScenarioRunType { All, Modified }

        #endregion

        #region Fields and Constants

        private const int FIRST_SCENARIO_COL = 6;

        private const int FU_ROW_NBR = 2;
        private const int MIX_ID_COL = 3;
        private const int NOTE_COL = 0;

        private const int PARAMETER_ID_COL = 1;
        private const int PARAMETER_UNIT_COL = 5;
        private const int PATHWAY_ID_COL = 2;
        private const int RECORD_FU_COL = 5;
        private const int RECORD_ITEM_ID_COL = 1;
        private const int RECORD_ITEM_TYPE_COL = 0;
        private const int VEHICLE_ID_COL = 4;

        // Property:
        private static bool? _consolePresent;

        private static readonly List<string> _polGroups = new List<string> {"GHG-100"};

        private static readonly List<string> _pollutants = new List<string>
        {
            "VOC",
            "CO",
            "NOx",
            "PM10",
            "PM2.5",
            "SOx",
            "BC",
            "POC",
            "CH4",
            "N2O",
            "CO2",
            "CO2_Biogenic"
        };

        private static readonly List<string> _resGroups = new List<string>
        {
            "Total Energy",
            "Fossil Fuel",
            "Coal Fuel",
            "Natural Gas Fuel",
            "Petroleum Fuel",
            "Water"
        };

        private static readonly List<string> _urbanPoll = new List<string> {"VOC", "CO", "NOx", "PM10", "PM2.5", "SOx", "BC", "POC"};
        private static readonly string _preferedEnergy = "J";

        private static readonly string _preferedMass = "kg";
        private static readonly string _preferedVolume = "m^3";

        #endregion

        #region Properties and Indexers

        private static bool ConsolePresent
        {
            get
            {
                if (_consolePresent == null)
                {
                    _consolePresent = true;
                    try { int windowHeight = Console.WindowHeight; }
                    catch { _consolePresent = false; }
                }
                return _consolePresent.Value;
            }
        }

        #endregion

        #region Members

        public static DataTable DataTableFuntionalUnitConversion(DataTable dt, string exportFunctionalUnit, double quantity = 1)
        {
            //extract functional unit from the table top left corner
            if (dt.Rows.Count > 3 && dt.Rows[FU_ROW_NBR].ItemArray.Any())
            {
                string cel = dt.Rows[FU_ROW_NBR][0].ToString();
                cel = cel.Replace("Per ", "");
                string[] split = splitTextValueUnit(cel);

                // make it crash if the text is not convertible to double.
                double enteredDoubleValue = Convert.ToDouble(split[0]);

                if (split[1].Length > 0)
                {
                    //If the user enters the value along with the unit. Use the unit to determine the value to be displayed in the base unit of the DBTextbox.
                    // Used to temporarily store the value entered by user and to compare it with the existing unit of the DBTextBox. 
                    string siExp;
                    uint eqDim;
                    double slope, intercept;
                    var filteredUserExpression = "";
                    GuiUtils.FilterExpression(split[1], out siExp, out filteredUserExpression, out eqDim,
                        out slope, out intercept);
                    AQuantity.ConvertFromSpecificToSI(enteredDoubleValue, filteredUserExpression);
                }
            }

            double ratio;
            uint ratioDim;
            //find conversion ratio if possible to new desired unit
            string siExp2;
            uint eqDim2;
            double slope2, intercept2;
            string filteredUserExpression2;
            GuiUtils.FilterExpression(exportFunctionalUnit, out siExp2, out filteredUserExpression2, out eqDim2,
                out slope2, out intercept2);
            ratio = AQuantity.ConvertFromSpecificToSI(quantity, filteredUserExpression2);
            ratioDim = eqDim2;

            //if conversion ratio != 0 (exists) then convert the whole table numbers
            if (ratio != 0 && ratioDim != 0)
            {
                for (int row = FU_ROW_NBR; row < dt.Rows.Count; row++)
                {
                    for (int col = 1; col < dt.Columns.Count; col++)
                    {
                        string cell = dt.Rows[row][col].ToString();
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// Loads the scenario CSV file
        /// </summary>
        /// <param name="filename"></param>
        public static ScenariosData LoadFromFile(string filename, Parameters parameters = null)
        {
            ScenariosData scenarios = LoadCSVFile(filename, parameters);

            string advancedScenarioFileName = Path.GetDirectoryName(filename) + @"\" +
                                              Path.GetFileNameWithoutExtension(filename) + ".scnadv";
            if (!String.IsNullOrEmpty(advancedScenarioFileName))
                LoadAdvancedScenarioFile(advancedScenarioFileName, scenarios);

            string resultsScenarioFileName = Path.GetDirectoryName(filename) + @"\" +
                                             Path.GetFileNameWithoutExtension(filename) + ".scnres";
            if (!String.IsNullOrEmpty(resultsScenarioFileName))
                LoadResultsFile(resultsScenarioFileName, scenarios);

            return scenarios;
        }

        public static void Message(string text)
        {
            if (MessageEvent != null)
                MessageEvent(text);
        }

        public static event MessageEventHandler MessageEvent;

        /// <summary>
        /// For mix, pathway or vehicle
        /// </summary>
        /// <param name="record"></param>
        /// <param name="dataset"></param>
        /// <returns></returns>
        public static DataTable Record2DataTable(RecordedEntityResults record, ScenariosData scenarios, GData dataset)
        {
            if (record.Type == 'v')
                return VehicleRecord2DataTable(record, scenarios, dataset);
            return WTPRecord2DataTable(record, scenarios, dataset);
        }

        /// <summary>
        /// Creates tables to compare all WTP items recorded in the scenarios
        /// </summary>
        /// <param name="scnearios">Scenarios containing the data</param>
        /// <param name="dataset">GData to access diverse names and other things</param>
        /// <param name="scenarioId">List of the scenario ID to be compared</param>
        /// <returns>KeyValuePairs of Scenario Guid, WTP Items DataTable</returns>
        public static Dictionary<Guid, DataTable> Record2WTPDataTable(ScenariosData scnearios, GData dataset, List<Guid> scenarioId)
        {
            Dictionary<Guid, DataTable> results = new Dictionary<Guid, DataTable>();
            foreach (Scenario scn in scnearios.Scenarios)
            {//foreach scenario, we add as many columns as we have pathways

                if (!scenarioId.Contains(scn.Id)) continue;

                var commonFu = ""; var commonFuSet = false;
                DataTable sideBySideResults = new DataTable();
                sideBySideResults.Columns.Add("Items");
                var nameRow = new string[1];
                nameRow[0] = "";
                sideBySideResults.Rows.Add(nameRow);
                var idRow = new string[1];
                idRow[0] = "";
                sideBySideResults.Rows.Add(idRow);

                foreach (RecordedEntityResults record in scnearios.Results)
                {
                    if (record.Type == 'm' || record.Type == 'p')
                    {
                        #region taking first functional unit from the recorded result or from the pathway or mix itself
                        if (!String.IsNullOrEmpty(record.ExportFunctionalUnit) && !commonFuSet)
                        {
                            commonFu = record.ExportFunctionalUnit;

                            var fuRow = new string[1];
                            fuRow[0] = "Per " + commonFu;
                            sideBySideResults.Rows.Add(fuRow);
                            commonFuSet = true;
                        }
                        else
                        {
                            if (!commonFuSet)
                            {
                                double fuQty; //the '1' in 1 MJ
                                int outptutResourceId = -1;
                                if (record.Type == 'm')
                                    outptutResourceId = dataset.MixesData[record.Id].MainOutputResourceID;
                                else if (record.Type == 'p')
                                    outptutResourceId = dataset.PathwaysData[record.Id].MainOutputResourceID;
                                SimpleResultStorage resultsFu = record.Results.Values.FirstOrDefault();
                                string fuUnit; //the 'MJ' in 1 MJ
                                commonFu = GetPreferedVisualizationFunctionalUnitString(dataset, resultsFu, outptutResourceId, out fuQty, out fuUnit);

                                string[] fuRow = new string[1];
                                fuRow[0] = "Per " + commonFu;
                                sideBySideResults.Rows.Add(fuRow);
                                commonFuSet = true;
                            }
                        }

                        #endregion


                        if (record.Results.ContainsKey(scn.Id))
                        {
                            DataTable dt = WTPRecord2DataTable(record, scnearios, dataset, scn.Id, commonFu);
                            sideBySideResults = MergeDataTable(sideBySideResults, dt);
                        }
                    }
                }
                results.Add(scn.Id, sideBySideResults);
            }

            return results;
        }

        /// <summary>
        /// Creates tables to compare all WTW items recorded in the scenarios
        /// </summary>
        /// <param name="scneariosData">Scenarios containing the data</param>
        /// <param name="dataset">GData to access diverse names and other things</param>
        /// <param name="scenarioId">List of the scenario ID to be compared</param>
        /// <returns>KeyValuePairs of Scenario Guid, WTP Items DataTable</returns>
        public static Dictionary<Guid, DataTable> Record2WtwDataTable(ScenariosData scneariosData, GData dataset, List<Guid> scenarioId)
        {
            Dictionary<Guid, DataTable> results = new Dictionary<Guid, DataTable>();
            foreach (Scenario scn in scneariosData.Scenarios)
            {
                if (!scenarioId.Contains(scn.Id)) continue;

                var commonFu = ""; var commonFuSet = false;
                DataTable sideBySideResults = new DataTable();
                sideBySideResults.Columns.Add("Items");
                var nameRow = new string[1];
                nameRow[0] = "";
                sideBySideResults.Rows.Add(nameRow);
                var idRow = new string[1];
                idRow[0] = "";
                sideBySideResults.Rows.Add(idRow);
                foreach (RecordedEntityResults record in scneariosData.Results)
                {
                    if (record.Type == 'v')
                    {
                        #region taking first functional unit from the recorded result or from the pathway or mix itself
                        if (!String.IsNullOrEmpty(record.ExportFunctionalUnit) && !commonFuSet)
                        {
                            commonFu = record.ExportFunctionalUnit;

                            string[] fuRow = new string[1];
                            fuRow[0] = "Per " + commonFu;
                            sideBySideResults.Rows.Add(fuRow);
                            commonFuSet = true;
                        }
                        else
                        {
                            if (!commonFuSet)
                            {
                                double fuQty; //the '1' in 1 MJ
                                string fuUnit; //the 'MJ' in 1 MJ

                                SimpleResultStorage resultsFu = record.Results.Values.FirstOrDefault();
                                commonFu = GetPreferedVisualizationFunctionalUnitString(dataset, resultsFu, -1, out fuQty, out fuUnit);

                                string[] fuRow = new string[1];
                                fuRow[0] = "Per " + commonFu;
                                sideBySideResults.Rows.Add(fuRow);
                                commonFuSet = true;
                            }
                        }
                        #endregion


                        if (record.Results.ContainsKey(scn.Id))
                        {
                            DataTable dt = WTPRecord2DataTable(record, scneariosData, dataset, scn.Id, commonFu);
                            sideBySideResults = MergeDataTable(sideBySideResults, dt);
                        }
                    }
                }
                results.Add(scn.Id, sideBySideResults);
            }

            return results;
        }

        /// <summary>
        /// Runs calculations on the model for each scenario.
        /// Changes parameters user values iteratively, run model, record results
        /// </summary>
        /// <param name="controler">Greet controler which serves as a link to the model</param>
        /// <param name="scenarios">Data for values modifications and results storage</param>
        /// <param name="runType">All will run all scenarios, modified will only run the scenarios that have been modified since last calculations run</param>
        /// <returns></returns>
        public static int RunAllScenarios(IGREETController controler, ScenariosData scenarios, ScenarioRunType runType)
        {
            if (controler == null)
            {
                Message("Controller is null");
                return -1;
            }
            if (controler.CurrentProject == null)
            {
                Message("Current project is null");
                return -1;
            }
            if (controler.CurrentProject.Data == null)
            {
                Message("Data is null in the current project");
                return -1;
            }
            if (scenarios == null)
            {
                Message("Scenarios are null");
                return -1;
            }

            //finds scenarios for which results storage does not exists or for which SHA has changed which would mean that the parameter values have been modified
            List<Guid> scenarioIdsToRun = new List<Guid>();
            foreach (Scenario scn in scenarios.Scenarios)
            {
                foreach (RecordedEntityResults res in scenarios.Results)
                {
                    if (runType == ScenarioRunType.All)
                        scenarioIdsToRun.Add(scn.Id);
                    else
                    {
                        //The code below was used to detect which scenarios have been modified
                        if (!res.Results.ContainsKey(scn.Id) ||
                            res.Results[scn.Id].SHA256ScenarioState.CompareTo(scn.GetSHAState()) != 0)
                        {
                            scenarioIdsToRun.Add(scn.Id);
                            break;
                        }
                    }
                }
            }

            Dictionary<string, string> originalValues = new Dictionary<string, string>();
            int count = 0;
            int total = scenarioIdsToRun.Count;
            foreach (Scenario scn in scenarios.Scenarios)
            {
                if (!scenarioIdsToRun.Contains(scn.Id))
                    continue;

                if (scn.ValueModifications == null 
                    || scn.ValueModifications.Count == 0)
                    continue;

                Dictionary<string, string> original = SetParametersForScenario(scn, controler);
                foreach (KeyValuePair<string, string> pair in original)
                    if (!originalValues.ContainsKey(pair.Key))
                        originalValues.Add(pair.Key, pair.Value);

                Message(scn.Name + ": (" + count + "/" + total + ") Running simulations");
                #region run simulations

                string messages = controler.RunSimalation(false);
                if (!string.IsNullOrEmpty(messages))
                    Message("Calculation Messages:" + messages);
                #endregion

                Message(scn.Name + ": Storing results");
                #region store results

                foreach (RecordedEntityResults result in scenarios.Results)
                {
                    Results selectedResults = null;
                    if (result.Type == 'p')
                    {
                        try
                        {
                            IPathway path = controler.CurrentProject.Data.Pathways.ValueForKey(result.Id);
                            Dictionary<IIO, IResults> results = path.GetUpstreamResults(controler.CurrentProject.Data);
                            Guid mo = path.MainOutput;
                            selectedResults = results.SingleOrDefault(item => item.Key.Id == mo).Value as Results;
                        }
                        catch (Exception e)
                        {
                            Message("Exception with results recording for " + result + ": " + e.Message);
                        }
                    }
                    else if (result.Type == 'm')
                    {
                        try
                        {
                            IMix mix = controler.CurrentProject.Data.Mixes.ValueForKey(result.Id);
                            Dictionary<IIO, IResults> results = mix.GetUpstreamResults(controler.CurrentProject.Data);
                            selectedResults = results.FirstOrDefault().Value as Results;
                        }
                        catch (Exception e)
                        {
                            Message("Exception with results recording for " + result + ": " + e.Message);
                        }
                    }
                    else if (result.Type == 'v')
                    {
                        
                        IVehicle vehicle = controler.CurrentProject.Data.Vehicles.ValueForKey(result.Id);

                        string userSiExp; string userFilteredExpression; uint userEqDim; double userSlope; double userIntercept;
                        string desiredFunctionalUnit = "mi";
                        if (!String.IsNullOrEmpty(result.ExportFunctionalUnit))
                            desiredFunctionalUnit = result.ExportFunctionalUnit;

                        GuiUtils.FilterExpression(desiredFunctionalUnit, out userSiExp, out userFilteredExpression, out userEqDim, out userSlope, out userIntercept);

                        Enem enem = new Enem();
                        EmissionAmounts urban = new EmissionAmounts();
                        try
                        {
                            if (userEqDim == DimensionUtils.ENERGY)
                            {
                                enem = vehicle.GetTotalWTWResults(controler.CurrentProject.Data, 0);
                                urban = vehicle.GetTotalWTWUrbanEm(controler.CurrentProject.Data, 0);
                            }
                            else if (userEqDim == DimensionUtils.LENGTH && result.ExportFunctionalUnit.ToLower().Contains("passenger"))
                            {
                                enem = vehicle.GetTotalWTWResults(controler.CurrentProject.Data, 5);
                                urban = vehicle.GetTotalWTWUrbanEm(controler.CurrentProject.Data, 5);
                            }
                            else if (userEqDim == DimensionUtils.LENGTH)
                            {
                                enem = vehicle.GetTotalWTWResults(controler.CurrentProject.Data, 1);
                                urban = vehicle.GetTotalWTWUrbanEm(controler.CurrentProject.Data, 1);
                            }
                            else if (userEqDim == DimensionUtils.Plus(DimensionUtils.MASS, DimensionUtils.LENGTH))
                            {
                                enem = vehicle.GetTotalWTWResults(controler.CurrentProject.Data, 3);
                                urban = vehicle.GetTotalWTWUrbanEm(controler.CurrentProject.Data, 3);
                            }
                        }
                        catch (Exception e)
                        {
                            Message("Exception with results recording for " + result + ": " + e.Message);
                        }
                        Results results = new Results();
                        results.wellToProductEnem = enem;
                        results.wellToProductUrbanEmission = urban;
                        selectedResults = results;
                        results.BottomDim = enem.BottomDim;
                    }


                    if (!result.Results.ContainsKey(scn.Id))
                        result.Results.Add(scn.Id, new SimpleResultStorage());
                    if(selectedResults != null)
                        result.Results[scn.Id] = new SimpleResultStorage(selectedResults.wellToProductEnem
                            , selectedResults.wellToProductUrbanEmission
                            , selectedResults.BottomDim
                            , selectedResults.CustomFunctionalUnitPreference
                            , scn.GetSHAState());
                }

                #endregion

                count++;
            }

            Message("Restoring parameters to their original values");
            #region restoring all values to their original values
            foreach (KeyValuePair<string, string> pair in originalValues)
            {
                string paramID = pair.Key;
                string[] split = pair.Value.Split(',');
                bool paramUseOriginal = Convert.ToBoolean(split[0]); ;
                double paramUserValue = Convert.ToDouble(split[1]);
                string paramUserExpression = split[2];

                Parameter param = controler.CurrentProject.Data.Parameters.ValueForKey(paramID) as Parameter;
                param.UserValue = paramUserValue;
                param.UserValuePreferedExpression = paramUserExpression;
                param.UseOriginal = paramUseOriginal;
            }

            #endregion

            return 0;
        }

        /// <summary>
        /// Saves the scenarios to a csv formatted file when the user decides to save it with a new name
        /// Also create the advanced scenario file if any of the scenario contains a shapefile name or if any of the results contains consumptions/quantities used
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="filename"></param>
        /// <param name="scnearios"></param>
        public static void SaveAsCSV(ScenariosData scnearios, Parameters parameters, string filename)
        {
            string csv = ToCSVString(scnearios, parameters);
            try
            {
                using (StreamWriter file =
                    new StreamWriter(filename, false))
                {
                    file.Write(csv);
                }
            }
            catch (Exception ex)
            {

            }

            if (scnearios.Scenarios.Any(item => !String.IsNullOrEmpty(item.LayerName)))
            {
                string advFilename = Path.Combine(Path.GetDirectoryName(filename),
                    Path.GetFileNameWithoutExtension(filename) + ".scnadv");
                ToAdvancedScenariosFile(scnearios, advFilename);
            }

            if (scnearios.Results.Any(item => item.Results != null))
            {
                string rsltFilename = Path.Combine(Path.GetDirectoryName(filename),
                    Path.GetFileNameWithoutExtension(filename) + ".scnres");
                ToResultsFile(scnearios, rsltFilename);
            }
        }

        public static DataTable Scenarios2DataTable(ScenariosData scenariosData, GData gData)
        {
            List<string> parameterIDs = new List<string>();
            //Adds all the parameter ID to a list
            foreach (ValueModification valMod in scenariosData.Scenarios.SelectMany(scn => scn.ValueModifications.Where(valMod => !parameterIDs.Contains(valMod.ParameterID))))
                parameterIDs.Add(valMod.ParameterID);
            parameterIDs.Sort();

            DataTable dt = new DataTable();
            dt.Columns.Add("Scenario");
            foreach (string t in parameterIDs)
                dt.Columns.Add(t);

            //add row to store unit of each of the parameters
            dt.Rows.Add(new object[parameterIDs.Count + 1]);

            //add a new row for each scenario with values for the modified paramters
            foreach (Scenario scn in scenariosData.Scenarios)
            {
                var paramValues = new object[parameterIDs.Count + 1];
                paramValues[0] = scn.Name;
                foreach (ValueModification vmod in scn.ValueModifications)
                {
                    int position = parameterIDs.IndexOf(vmod.ParameterID);
                    string paraValue = vmod.NewUserValue + vmod.NewExpression;
                    paramValues[position+1] = paraValue; //+1 because we added a first column in the table for scenario names
                }

                DataRow dr = dt.NewRow();
                dr.ItemArray = paramValues;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static Dictionary<string, string> SetParametersForScenario(Scenario scn, IGREETController controler)
        {
            Dictionary<string, string> originalValues = new Dictionary<string, string>();
            Parameters parameters = controler.CurrentProject.Data.Parameters as Parameters;
            Message(scn.Name + ": Modifying values");
            #region store original values and modify using scenario values
            foreach (ValueModification valueModification in scn.ValueModifications)
            {
                Parameter param = null;
                if (parameters.ContainsKey(valueModification.ParameterID))
                    param = parameters[valueModification.ParameterID];
                else
                    param = parameters.Values.SingleOrDefault(item => item.Name == valueModification.ParameterID);

                if (param != null)
                {
                    if (!originalValues.ContainsKey(valueModification.ParameterID))
                        originalValues.Add(param.Id, param.UseOriginal + "," + param.UserValue + "," + param.UserValuePreferedExpression);

                    string siExp; string filteredUserExpression; uint eqDim; double slope, intercept;
                    GuiUtils.FilterExpression(valueModification.NewExpression, out siExp, out filteredUserExpression, out eqDim, out slope, out intercept);
                    param.UserValue = AQuantity.ConvertFromSpecificToSI(valueModification.NewUserValue, filteredUserExpression);
                    param.UserDim = eqDim;
                    param.UserValuePreferedExpression = filteredUserExpression;
                    param.UseOriginal = false;
                }
                else
                    Message("Cannot find parameter by ID or Name given as: " + valueModification.ParameterID);
            }
            return originalValues;
            #endregion
        }

        public static void SetParametersToGREETDefaults(GData data)
        {
            Parameters parameters = data.Parameters as Parameters;
            Message("Setting values to GREET Default");
            #region set all parameter flags to use greet defaults

            foreach (Parameter p in parameters.Values)
                p.UseOriginal = true;

            #endregion
        }

        public static string ToCSVString(ScenariosData scneariosData, Parameters parameters)
        {
            List<string> line = new List<string>();
            line.Add("Note");
            line.Add("Parameter");
            line.Add("Pathway");
            line.Add("Mix");
            line.Add("Vehicle");
            line.Add("FUnit");
            foreach (Scenario s in scneariosData.Scenarios)
                line.Add(s.Name);
            String csv = string.Join(",", line) + Environment.NewLine;

            //creating a new dictionary to reverse the tree structure to <ParameterID, <ScenarioID, ParameterNewValueObj>>
            Dictionary<string, Dictionary<string, ValueModification>> mods =
                new Dictionary<string, Dictionary<string, ValueModification>>();
            foreach (Scenario s in scneariosData.Scenarios)
            {
                //try to assign names instead of using parameter IDs so that the file is easier to read, internally this library uses IDs everywhere it can
                foreach (ValueModification v in s.ValueModifications)
                {
                    string visualName = v.ParameterID;
                    if (parameters.ContainsKey(visualName) && !String.IsNullOrEmpty(parameters[visualName].Name))
                        visualName = parameters[visualName].Name + ";" + v.ParentInfo;
                    else
                        visualName += ";" + v.ParentInfo;

                    if (!mods.ContainsKey(visualName))
                        mods.Add(visualName, new Dictionary<string, ValueModification>());
                    if (!mods[visualName].ContainsKey(s.Name))
                        mods[visualName].Add(s.Name, v);
                }
            }

            //creating a string out of all the value modification items
            foreach (KeyValuePair<string, Dictionary<string, ValueModification>> pair in mods)
            {
                line.Clear();
                line.Add(""); //placeholder for notes
                line.Add(pair.Key);
                line.Add(""); //placeholder for Pathway
                line.Add(""); //placeholder for Mix
                line.Add(""); //placeholder for Vehicle
                bool addExpression = true;
                foreach (KeyValuePair<string, ValueModification> md in pair.Value) //adding values for each scenario
                {
                    if (addExpression)
                    {
                        line.Add(md.Value.NewExpression); //FUnit or Expression
                        addExpression = false;
                    }
                    line.Add(md.Value.NewUserValue.ToString());
                }
                string csvline = string.Join(",", line) + Environment.NewLine;
                csv += csvline;
            }

            //adds recoreded entities and their quantities for each of the scenarios
            foreach (RecordedEntityResults res in scneariosData.Results)
            {
                string csvLine = "";
                switch (res.Type.ToString())
                {
                    case "p":
                        csvLine += ",," + res.Id + ",,," + res.ExportFunctionalUnit + ",";
                        break;

                    case "m":
                        csvLine += ",,," + res.Id + ",," + res.ExportFunctionalUnit + ",";
                        break;

                    case "v":
                        csvLine += ",,,," + res.Id + "," + res.ExportFunctionalUnit + ",";
                        break;
                }
                foreach (var amt in res.Amounts)
                {
                    csvLine += amt + ",";
                }
                csvLine = csvLine.TrimEnd(',');
                csv += csvLine + Environment.NewLine;
            }

            return csv;
        }

        /// <summary>
        /// Returns the coefficient to be used in order to display the results according to the preferences of the IResult
        /// object used to create the instance of this object. Returns the prefered functional unit divided by the functional unit.
        /// 
        /// In case of the FunctionalUnit is null or the PreferedUnit is null, this method returns a ratio of 1. This is usefull to
        /// display the Inputs results and the TransportationSteps results which are accounted for all outputs and not a specific one.
        /// In these cases instead of defining a PreferedUnit for each output we prefer to define none to make things simpler (see InputResult.GetResults)
        /// 
        /// Before display all results must be multiplied by this coefficient
        /// 
        /// May return a NullReferenceExeption if the IResult object does not define FunctinoalUnit, PreferedDisplayedUnit or PreferedDisplayedAmount
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private static double GetFunctionalRatio(GData data, SimpleResultStorage results, int producedResourceId)
        {
            if (results != null && results.CustomFunctionalUnit != null && results.CustomFunctionalUnit.PreferredUnitExpression != null)
            {
                LightValue functionalUnit = new LightValue(1.0, results.BottomDim);
                LightValue preferedFunctionalUnit = GetPreferedVisualizationFunctionalUnit(data, results, producedResourceId);

                switch (preferedFunctionalUnit.Dim)
                {
                    case DimensionUtils.MASS: // HARDCODED
                        {
                            if(data.ResourcesData.ContainsKey(producedResourceId))
                                functionalUnit = data.ResourcesData[producedResourceId].ConvertToMass(functionalUnit);
                        }
                        break;
                    case DimensionUtils.ENERGY: // HARDCODED
                        {
                            if (data.ResourcesData.ContainsKey(producedResourceId))
                                functionalUnit = data.ResourcesData[producedResourceId].ConvertToEnergy(functionalUnit);
                        }
                        break;
                    case DimensionUtils.VOLUME: // HARDCODED
                        {
                            if (data.ResourcesData.ContainsKey(producedResourceId))
                                functionalUnit = data.ResourcesData[producedResourceId].ConvertToVolume(functionalUnit);
                        }
                        break;
                }
                return preferedFunctionalUnit.Value / functionalUnit.Value;
            }
            return 1;
        }

        /// <summary>
        /// Returns the functional unit to be used on display: User prefered, default million btu or functional unit of the
        /// process in case of the database does not contains enough information to convert to one million btu
        /// </summary>
        /// <param name="data"></param>
        /// <param name="results"></param>
        /// <param name="producedResourceId"></param>
        /// <returns></returns>
        private static LightValue GetPreferedVisualizationFunctionalUnit(GData data, SimpleResultStorage results, int producedResourceId)
        {
            LightValue preferedFunctionalUnit;
            if (results.CustomFunctionalUnit.enabled)
                preferedFunctionalUnit = new LightValue(results.CustomFunctionalUnit.Amount, results.CustomFunctionalUnit.PreferredUnitExpression);
            else if (data.ResourcesData.ContainsKey(producedResourceId) && data.ResourcesData[producedResourceId].CanConvertTo(DimensionUtils.ENERGY, new LightValue(1.0, results.BottomDim)))
                preferedFunctionalUnit = new LightValue(1, "MJ");
            else
                preferedFunctionalUnit = new LightValue(1.0, results.BottomDim);
            return preferedFunctionalUnit;
        }

        /// <summary>
        /// Returns the functional unit to be used on display: User prefered, default million btu or functional unit of the
        /// process in case of the database does not contains enough information to convert to one million btu
        /// </summary>
        /// <param name="data"></param>
        /// <param name="results"></param>
        /// <param name="producedResourceId"></param>
        /// <returns></returns>
        private static string GetPreferedVisualizationFunctionalUnitString(GData data, SimpleResultStorage results, int producedResourceId, out double fu_qty, out string fu_unit)
        {
            fu_qty = 0;
            fu_unit = "";
            try
            {
                if (results.CustomFunctionalUnit != null && results.CustomFunctionalUnit.enabled)
                {
                    fu_qty = results.CustomFunctionalUnit.Amount;
                    fu_unit = results.CustomFunctionalUnit.PreferredUnitExpression;
                }
                else if (data.ResourcesData.ContainsKey(producedResourceId) && data.ResourcesData[producedResourceId].CanConvertTo(DimensionUtils.ENERGY, new LightValue(1.0, results.BottomDim)))
                {
                    fu_qty = 1;
                    fu_unit = "MJ";
                }
                else
                {
                    AQuantity qty = Units.QuantityList.ByDim(results.BottomDim);
                    fu_qty = 1;
                    fu_unit = qty.SiUnit.Expression;
                }
            }
            catch { }

            return fu_qty + " " + fu_unit;
        }

        /// <summary>
        /// Loads the advanced properties of scenarios from the advanced file into the scenarios
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="scenarios"></param>
        /// <returns>True if succeeded, false or exception otherwise</returns>
        private static bool LoadAdvancedScenarioFile(string filename, ScenariosData scenarios)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filename);
                string dirXmlLivesIn = Path.GetDirectoryName(filename);

                foreach (XmlNode layerNode in xmlDoc.SelectNodes("scenarios/shapefile"))
                {
                    ShapeFile shapeFileInstance = new ShapeFile();
                    if (layerNode.Attributes["filename"] != null)
                    {
                        string shapeFilePath = layerNode.Attributes["filename"].Value;
                        if (Path.IsPathRooted(shapeFilePath))
                            shapeFileInstance.Filename = shapeFilePath;
                        else
                            shapeFileInstance.Filename = Path.Combine(path1: dirXmlLivesIn, path2: shapeFilePath);
                    }
                    if (layerNode.Attributes["layerName"] != null)
                        shapeFileInstance.Name = layerNode.Attributes["layerName"].Value;

                    scenarios.Shapefiles.Add(shapeFileInstance);
                }


                foreach (XmlNode scenarioNode in xmlDoc.SelectNodes("scenarios/scenario"))
                {
                    string name = "";
                    if (scenarioNode.Attributes["name"] != null)
                        name = scenarioNode.Attributes["name"].Value;
                    else
                        continue;

                    Scenario secarioInstance = null;
                    secarioInstance = scenarios.Scenarios.SingleOrDefault(item => item.Name == name);
                    if (secarioInstance != null)
                    {
                        XmlNode shpNode = scenarioNode.SelectSingleNode("layer");
                        if (shpNode != null)
                        {
                            if (shpNode.Attributes["layerName"] != null)
                                secarioInstance.LayerName = shpNode.Attributes["layerName"].Value;
                            if (shpNode.Attributes["attributeName"] != null)
                                secarioInstance.ShapeFileFeatureAttributeName =
                                    shpNode.Attributes["attributeName"].Value;
                            if (shpNode.Attributes["attributeValue"] != null)
                                secarioInstance.ShapeFileFeatureAttributeValue =
                                    shpNode.Attributes["attributeValue"].Value;
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Message("Error in scenario file " + filename + Environment.NewLine + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Load scenario and parameter values for each scenarios from a CSV file
        /// Reads from the same file the items that needs to be recorded between each scenario runs
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static ScenariosData LoadCSVFile(string filename, Parameters parameters = null)
        {
            ScenariosData sl = new ScenariosData();
            StreamReader file = null;
            try
            {
                using (Stream s = new FileStream(filename,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    file = new StreamReader(s);
                    int lineNBr = 0;
                    string line = "";
                    sl.Scenarios.Clear();
                    sl.Results.Clear();
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] split = line.Split(',');
                        if (lineNBr == 0)
                        {
//reading the first line   
                            for (int i = FIRST_SCENARIO_COL; i < split.Length; i++)
                                sl.Scenarios.Add(new Scenario(split[i]));
                        }
                        else if (lineNBr > 0 && split[PARAMETER_ID_COL].ToLower() != "")
                        {
//reading lines containing parameter IDs and values
                            string parameterIDorNAME = ""; //parameter ID or NAME and optional parentInfo
                            string parameterPARENTINFO = "";
                            string[] parameterIDorNAMEandPARENTINFO = split[PARAMETER_ID_COL].Split(';');
                            if (parameterIDorNAMEandPARENTINFO.Length >= 1)
                                parameterIDorNAME = parameterIDorNAMEandPARENTINFO[0];
                            if (parameterIDorNAMEandPARENTINFO.Length >= 2)
                                parameterPARENTINFO = parameterIDorNAMEandPARENTINFO[1];
                            string parameterUnit = split[PARAMETER_UNIT_COL];

                            Parameter p;
                            if (parameters != null &&
                                (p = parameters.Values.SingleOrDefault(item => item.Name == parameterIDorNAME)) != null)
                                parameterIDorNAME = p.Id;

                            for (int i = FIRST_SCENARIO_COL; i < split.Length; i++)
                            {
                                string parameterValue = split[i];
                                sl.Scenarios[i - FIRST_SCENARIO_COL].ValueModifications.Add(
                                    new ValueModification(parameterIDorNAME, parameterUnit, parameterValue,
                                        parameterPARENTINFO));
                            }
                        }
                        else if (lineNBr > 0 && split[PATHWAY_ID_COL].ToLower() != "")
                        {
//reading line containing a pathway to save results for 
                            int itemId;
                            int.TryParse(split[PATHWAY_ID_COL], out itemId);
                            string fu = split[RECORD_FU_COL];

                            List<double> amounts = new List<double>();
                            double temp;
                            for (int i = RECORD_FU_COL + 1; i < split.Length; i++)
                            {
                                double.TryParse(split[i], out temp);
                                amounts.Add(temp);
                            }

                            sl.Results.Add(new RecordedEntityResults('p', itemId, amounts, fu));
                        }
                        else if (lineNBr > 0 && split[MIX_ID_COL].ToLower() != "")
                        {
//reading line containing a pathway to save results for 
                            int itemId;
                            int.TryParse(split[MIX_ID_COL], out itemId);
                            string fu = split[RECORD_FU_COL];

                            List<double> amounts = new List<double>();
                            double temp;
                            for (int i = RECORD_FU_COL + 1; i < split.Length; i++)
                            {
                                double.TryParse(split[i], out temp);
                                amounts.Add(temp);
                            }
                            sl.Results.Add(new RecordedEntityResults('m', itemId, amounts, fu));
                        }
                        else if (lineNBr > 0 && split[VEHICLE_ID_COL].ToLower() != "")
                        {
//reading line containing a pathway to save results for 
                            int itemId;
                            int.TryParse(split[VEHICLE_ID_COL], out itemId);
                            string fu = split[RECORD_FU_COL];

                            List<double> amounts = new List<double>();
                            double temp;
                            for (int i = RECORD_FU_COL + 1; i < split.Length; i++)
                            {
                                double.TryParse(split[i], out temp);
                                amounts.Add(temp);
                            }
                            sl.Results.Add(new RecordedEntityResults('v', itemId, amounts, fu));
                        }
                        lineNBr++;
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (file != null)
                    file.Close();
            }
            return sl;
        }

        /// <summary>
        /// Populates the recorded results of a scenarios entity from the stored results in a file
        /// </summary>
        /// <param name="filename">The filename to load from</param>
        /// <param name="scenarios">The scenario results that will be modified</param>
        /// <returns>True if succeeded, false or exception otherwise</returns>
        private static bool LoadResultsFile(string filename, ScenariosData scenarios)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filename);
                string dirXmlLivesIn = Path.GetDirectoryName(filename);
                foreach (XmlNode entityResultNode in xmlDoc.SelectNodes("results/recorded"))
                {
                    RecordedEntityResults entityResult = new RecordedEntityResults();
                    entityResult.FromXmlNode(entityResultNode, scenarios);

                    RecordedEntityResults res =
                        scenarios.Results.SingleOrDefault(
                            item => item.Id == entityResult.Id && item.Type == entityResult.Type);
                    if (res != null)
                        res.Results = entityResult.Results;
                    //scenarios.Results.Add(entityResult);
                }
                return true;
            }
            catch (Exception e)
            {

            }
            return false;
        }

        /// <summary>
        /// Merges the data tables by combining columns after the first one. Use to aggregate table when comparing multiple pathways, mix or vehicles.
        /// </summary>
        /// <param name="mergeTo"></param>
        /// <param name="mergeFrom"></param>
        /// <returns></returns>
        private static DataTable MergeDataTable(DataTable mergeTo, DataTable mergeFrom)
        {
            //completes with rows for gases resources...
            List<string> rows = new List<string> { "Header", "Name", "ID", "FU" };
            for (int i = 3; i < mergeFrom.Rows.Count; i++)
            {
                DataRow rowFrom = mergeFrom.Rows[i];
                if (mergeTo.Rows.Count > i)
                {
                    DataRow rowTo = mergeTo.Rows[i];
                    if (rowTo.ItemArray.Length > 0 && rowFrom.ItemArray.Length > 0
                        && rowTo[0].ToString() == rowFrom[0].ToString())
                    {
                        rows.Add(rowFrom[0].ToString());
                        continue;
                    }
                }
                DataRow added = mergeTo.Rows.Add(new string[mergeTo.Columns.Count]);
                if (rowFrom.ItemArray.Length > 0)
                    added[0] = rowFrom[0].ToString();
                rows.Add(rowFrom[0].ToString());
            }

            //Complete with the column values
            for (int i = 1; i < mergeFrom.Columns.Count; i++)
            {
                DataColumn dc = mergeTo.Columns.Add(mergeFrom.Rows[0][0].ToString(), typeof(string));
                for (int j = 3; j < mergeFrom.Rows.Count; j++)
                {
                    DataRow rowTo = mergeTo.Rows[j];
                    DataRow rowFrom = mergeFrom.Rows[j];
                    string value = rowFrom[i].ToString();
                    rowTo[dc.ColumnName] = value;
                }
            }

            return mergeTo;
        }

        private static string NiceValueWithAttribute(LightValue value, string preferedExpression = "")
        {
            double automaticScalingSlope = 1;
            string overrideUnitAttribute = "";
            return GuiUtils.FormatSIValue(value.Value
                , 2 //DEFINES THE FORMATTING FOR VALUES
                , out overrideUnitAttribute
                , out automaticScalingSlope
                , false
                , 16 //DEFINES HOW MANY DIGITS YOU WANT TO SEE
                , value.Dim
                , preferedExpression);// +" " + overrideUnitAttribute;
        }

        /// <summary>
        /// This method accepts a text and try to split the value from 
        /// the unit
        /// </summary>
        /// <param name="text">Text containing a number (value) and a unit</param>
        /// <returns>Value in the first part, Unit in the second part</returns>
        private static string[] splitTextValueUnit(string text)
        {
            string[] valueNumber = new string[2];
            int i = 0;
            foreach (char c in text)
            {
                if (Char.IsNumber(c) || c.ToString() == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                    i++;
                else if (c == 'E' || c == 'e' || c == '+' || c == '-')
                {
                    if (Char.IsNumber(text[i + 1]) || text[i + 1] == '+' || text[i + 1] == '-')
                        i++;
                }
                else
                    break;
            }
            valueNumber[0] = text.Substring(0, i).Trim();
            valueNumber[1] = text.Substring(i, text.Length - i).Trim();
            return valueNumber;
        }

        /// <summary>
        /// Saves the advanced properties of scenarios into a dedicated XML file in order to keep backward compatibility with the simpler CSV format
        /// </summary>
        /// <param name="scenariosData"></param>
        /// <param name="filename"></param>
        /// <returns>Filename if succeeded, null or empty otherwise</returns>
        private static string ToAdvancedScenariosFile(ScenariosData scenariosData, string filename)
        {
            try
            {
                //In order to calculate relative paths for the shapefiles if possible
                string filePath = Path.GetDirectoryName(filename);

                XmlDocument xmlDoc = new XmlDocument();
                XmlNode scenariosNode = xmlDoc.CreateNode("scenarios");
                xmlDoc.AppendChild(scenariosNode);

                foreach (ShapeFile shp in scenariosData.Shapefiles)
                {
                    //Calculating a relative path if possible
                    string shapeRelativeFileName = shp.Filename;
                    if (Path.IsPathRooted(shapeRelativeFileName) && Path.IsPathRooted(filePath) &&
                        shapeRelativeFileName.StartsWith(filePath))
                        shapeRelativeFileName = shapeRelativeFileName.Replace(filePath, "").TrimStart('\\');

                    XmlNode shpNode = xmlDoc.CreateNode("shapefile", xmlDoc.CreateAttr("layerName", shp.Name),
                        xmlDoc.CreateAttr("filename", shapeRelativeFileName));
                    scenariosNode.AppendChild(shpNode);
                }

                foreach (Scenario scn in scenariosData.Scenarios)
                {
                    XmlNode scnNode = xmlDoc.CreateNode("scenario", xmlDoc.CreateAttr("name", scn.Name));

                    scnNode.AppendChild(xmlDoc.CreateNode("layer", xmlDoc.CreateAttr("layerName", scn.LayerName)
                        , xmlDoc.CreateAttr("attributeName", scn.ShapeFileFeatureAttributeName)
                        , xmlDoc.CreateAttr("attributeValue", scn.ShapeFileFeatureAttributeValue)));

                    scenariosNode.AppendChild(scnNode);
                }
                xmlDoc.Save(filename);
                return filename;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        /// <summary>
        /// Saves all results per scenario and recorded items into a file
        /// </summary>
        /// <param name="_scenarios">Scenarios and results to be saved</param>
        /// <param name="filename">Filename for results to be saved</param>
        /// <returns>Filename if succeeded, null or empty otherwise</returns>
        private static string ToResultsFile(ScenariosData scenarios, string filename)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode root = xmlDoc.CreateNode("results");
            xmlDoc.AppendChild(root);
            foreach (RecordedEntityResults result in scenarios.Results)
            {
                XmlNode resNode = result.ToXmlNode(xmlDoc, scenarios);
                root.AppendChild(resNode);
            }
            xmlDoc.Save(filename);
            return filename;
        }

        /// <summary>
        /// Creates a data table that shows the results for a vehicle for multiple scenarios, each scenario beeing a column in the table
        /// </summary>
        /// <param name="record">Vehicle record</param>
        /// <param name="scenariosData">List of all scenarios</param>
        /// <param name="dataset">GREET Dataset</param>
        /// <returns>DataTable containing columns for each scenarios</returns>
        private static DataTable VehicleRecord2DataTable(RecordedEntityResults record, ScenariosData scenariosData, GData dataset, Guid forceSingleScenarioID = new Guid(), string forceSingleFunctionalUnit = "")
        {
            DataTable dt = new DataTable();

            if (record.Type != 'v')
                return dt;

            dt.Columns.Add("Items & Scenarios");
            foreach (KeyValuePair<Guid,SimpleResultStorage> pair in record.Results)
            {
                Guid scenarioGUID = pair.Key;
                SimpleResultStorage store = pair.Value;
                Scenario scn = scenariosData.Scenarios.SingleOrDefault(item => item.Id == scenarioGUID);
                string scenarioName = scenarioGUID.ToString();
                if (scn != null)
                    scenarioName = scn.Name;
                DataColumn column = dt.Columns.Add(scenarioName);
                column.ExtendedProperties.Add("ScenarioSHA", scn.GetSHAState());
                column.ExtendedProperties.Add("ResultsSHA", store.SHA256ScenarioState);
            }

            #region functional unit identification
            // for user defined unit
            string userFunctionUnit = !String.IsNullOrEmpty(forceSingleFunctionalUnit) ? forceSingleFunctionalUnit : record.ExportFunctionalUnit;
            string userSiExp; uint userEqDim; double userSlope, userIntercept; string userFilteredExpression = "";

            //the one that will be used for representation of the table
            string functionalUnit = "";
            double conversionFactor = 1;

            // for result object calculated functional unit
            double fu_qty = 1; //the '1' in 1 MJ
            string fu_unit = ""; //the 'MJ' in 1 MJ
            string deaultSiExp; uint defaultEqDim; double defaultSlope, defaultIntercept; string defaultFilteredExpression = "";

            //Get the results for the first scenario in order to indentify the Functional Unit for these results
            SimpleResultStorage resultsFU = record.Results.Values.FirstOrDefault();
            if (resultsFU != null)
                functionalUnit = GetPreferedVisualizationFunctionalUnitString(dataset, resultsFU, -1, out fu_qty, out fu_unit);
            if (userFunctionUnit != "")
            {
                try
                {
                    GuiUtils.FilterExpression(userFunctionUnit, out userSiExp, out userFilteredExpression, out userEqDim, out userSlope, out userIntercept);
                    GuiUtils.FilterExpression(fu_unit, out deaultSiExp, out defaultFilteredExpression, out defaultEqDim, out defaultSlope, out defaultIntercept);

                    if (userEqDim == defaultEqDim) //the results are calculated per joule in the results object and the user asks results per MJ from the scenario file
                    {
                        functionalUnit = "1 " + userFunctionUnit;
                        conversionFactor = 1 / defaultSlope / fu_qty * userSlope;
                    }
                    else
                    {
                        string message = "Error: User defined functional unit \"" + userFunctionUnit + "\" for " + record.Type +
                       " " + record.Id + " in Input.txt is not valid for the main output.\n\n";
                        Message(message);
                    }
                }
                catch
                {
                    string message = "Error: User defined functional unit \"" + userFunctionUnit + "\" for " + record.Type +
                         " " + record.Id + " in Input.txt cannot be identified.\n\n";
                    Message(message);
                }
            }
            #endregion

            functionalUnit = "Per " + functionalUnit;
            string name = "";
            if (record.Type == 'm')
                name = dataset.MixesData[record.Id].Name;
            else if (record.Type == 'p')
                name = dataset.PathwaysData[record.Id].Name;
            else if (record.Type == 'v')
                name = dataset.VehiclesData[record.Id].Name;

            string[] nameRow = new string[record.Results.Count + 1];
            nameRow[0] = name;
            dt.Rows.Add(nameRow);

            string[] IDRow = new string[record.Results.Count + 1];
            IDRow[0] = "ID=" + record.Id;
            dt.Rows.Add(IDRow);

            string[] fuRow = new string[record.Results.Count + 1];
            fuRow[0] = functionalUnit;
            dt.Rows.Add(fuRow);

            List<string> rowString = new List<string>(record.Results.Count + 1);
            #region total energy and energy groups
            foreach (string resGrp in _resGroups)
            {
                rowString = new List<string>();
                if (resGrp == "Water")
                    rowString.Add(resGrp + " (" + _preferedVolume + ")");
                else
                    rowString.Add(resGrp + " (" + _preferedEnergy + ")");

                foreach (Guid scenarioID in record.Results.Keys)
                {
                    if (forceSingleScenarioID == Guid.Empty || scenarioID == forceSingleScenarioID)
                    {
                        SimpleResultStorage results = record.Results[scenarioID];
                        if (results != null)
                        {
                            double amountRatio = GetFunctionalRatio(dataset, results, -1) * conversionFactor;

                            if (resGrp == "Total Energy")
                            {
                                LightValue totalE = results.FinalRe.TotalEnergy();
                                rowString.Add(NiceValueWithAttribute(totalE * amountRatio, _preferedEnergy));
                            }
                            else
                            {
                                Dictionary<int, IValue> resGroupes = results.FinalRe.GroupsToInterfaceDictionary(dataset);
                                int resGrpId = dataset.ResourcesData.Groups.Values.Single(item => item.Name == resGrp).Id;
                                if (resGroupes.ContainsKey(resGrpId))
                                {
                                    LightValue groupValue = new LightValue(resGroupes[resGrpId].Value, resGroupes[resGrpId].UnitExpression);
                                    if (groupValue.Dim == DimensionUtils.ENERGY)
                                        rowString.Add(NiceValueWithAttribute(groupValue * amountRatio, _preferedEnergy));
                                    else
                                        rowString.Add(NiceValueWithAttribute(groupValue * amountRatio, _preferedVolume));
                                }
                            }
                        }
                    }
                }
                dt.Rows.Add(rowString.ToArray());
            }
            #endregion

            #region wtp emissions
            foreach (string poll in _pollutants)
            {
                rowString = new List<string>();

                if (poll == "CO2")
                    rowString.Add("Total " + poll + " (" + _preferedMass + ")");
                else
                    rowString.Add(poll + " (" + _preferedMass + ")");

                foreach (Guid scenarioID in record.Results.Keys)
                {
                    if (forceSingleScenarioID == Guid.Empty || scenarioID == forceSingleScenarioID)
                    {
                        SimpleResultStorage results = record.Results[scenarioID];
                        if (results != null)
                        {
                            double amountRatio = GetFunctionalRatio(dataset, results, -1) * conversionFactor;

                            int polId = dataset.GasesData.Values.Single(item => item.Name == poll).Id;
                            if (results.FinalEm.ContainsKey(polId))
                            {
                                if (poll == "CO2")
                                {
                                    rowString.Add(NiceValueWithAttribute(
                                    new LightValue(results.FinalEm[polId] + results.FinalEm[dataset.GasesData.Values.Single(item => item.Name == "CO2_Biogenic").Id], DimensionUtils.MASS) * amountRatio
                                    , _preferedMass));

                                }
                                else
                                {
                                    rowString.Add(NiceValueWithAttribute(
                                    new LightValue(results.FinalEm[polId], DimensionUtils.MASS) * amountRatio
                                    , _preferedMass));
                                }
                            }
                        }
                    }
                }
                dt.Rows.Add(rowString.ToArray());
            }
            #endregion

            #region wtp Groups (here only GHG 100)
            foreach (string resGrp in _polGroups)
            {
                rowString = new List<string>();
                rowString.Add(resGrp + " (" + _preferedMass + ")");
                foreach (Guid scenarioID in record.Results.Keys)
                {
                    if (forceSingleScenarioID == Guid.Empty || scenarioID == forceSingleScenarioID)
                    {
                        SimpleResultStorage results = record.Results[scenarioID];
                        if (results != null)
                        {
                            double amountRatio = GetFunctionalRatio(dataset, results, -1) * conversionFactor;

                            Dictionary<int, IValue> emGroupes = results.FinalEm.GroupsToInterfaceDictionary(dataset);
                            int grpId = dataset.GasesData.Groups.Values.Single(item => item.Name == resGrp).Id;
                            if (emGroupes.ContainsKey(grpId))
                                rowString.Add(NiceValueWithAttribute(new LightValue(emGroupes[grpId].Value, emGroupes[grpId].UnitExpression) * amountRatio, _preferedMass));
                        }
                    }
                }
                dt.Rows.Add(rowString.ToArray());
            }
            #endregion

            dt.Rows.Add(new string[record.Results.Count + 1]);
            dt.Rows.Add(new string[record.Results.Count + 1]);

            #region urban emissions
            foreach (string poll in _urbanPoll)
            {
                rowString = new List<string>();
                rowString.Add("Urban " + poll + " (" + _preferedMass + ")");
                foreach (Guid scenarioID in record.Results.Keys)
                {
                    if (forceSingleScenarioID == Guid.Empty || scenarioID == forceSingleScenarioID)
                    {
                        SimpleResultStorage results = record.Results[scenarioID]; if (results != null)
                        {
                            double amountRatio = GetFunctionalRatio(dataset, results, -1) * conversionFactor;

                            int polId = dataset.GasesData.Values.Single(item => item.Name == poll).Id;
                            if (results.FinalEmUr.ContainsKey(polId))
                                rowString.Add(NiceValueWithAttribute(new LightValue(results.FinalEmUr[polId], DimensionUtils.MASS) * amountRatio, _preferedMass));
                        }
                    }
                }
                dt.Rows.Add(rowString.ToArray());
            }
            #endregion

            return dt;
        }

        /// <summary>
        /// Creates a data table that shows the results for a pathway or a mix for multiple scenarios, each scenario beeing a column in the table
        /// </summary>
        /// <param name="record">Vehicle record</param>
        /// <param name="scenariosData">List of all scenarios</param>
        /// <param name="dataset">GREET Dataset</param>
        /// <returns>DataTable containing columns for each scenarios</returns>
        private static DataTable WTPRecord2DataTable(RecordedEntityResults record, ScenariosData scenariosData, GData dataset, Guid forceSingleScenarioID = new Guid(), string forceSingleFunctionalUnit = "")
        {
            DataTable dt = new DataTable();

            int colCount = 0;
            dt.Columns.Add("Items & Scenarios");
            colCount++;
            foreach (KeyValuePair<Guid, SimpleResultStorage> pair in record.Results)
            {
                Guid scenarioGUID = pair.Key;
                SimpleResultStorage store = pair.Value;
                if (forceSingleScenarioID == Guid.Empty || forceSingleScenarioID == scenarioGUID)
                {
                    Scenario scn = scenariosData.Scenarios.SingleOrDefault(item => item.Id == scenarioGUID);
                    string scenarioName = scenarioGUID.ToString();
                    if (scn != null)
                        scenarioName = scn.Name;
                    DataColumn column = dt.Columns.Add(scenarioName);
                    column.ExtendedProperties.Add("ScenarioSHA", scn.GetSHAState());
                    column.ExtendedProperties.Add("ResultsSHA", store.SHA256ScenarioState);
                    colCount++;
                }
            }

            #region output product identification
            int outptutResourceId = -1;
            if (record.Type == 'm')
                outptutResourceId = dataset.MixesData[record.Id].MainOutputResourceID;
            else if (record.Type == 'p')
                outptutResourceId = dataset.PathwaysData[record.Id].MainOutputResourceID;
            #endregion

            #region functional unit identification
            //Get the results for the first scenario in order to indentify the Functional Unit for these results
            SimpleResultStorage resultsFu = record.Results.Values.FirstOrDefault();

            // for user defined unit
            string userFunctionUnit = !String.IsNullOrEmpty(forceSingleFunctionalUnit) ? forceSingleFunctionalUnit : record.ExportFunctionalUnit;

            // for data file default unit
            string functionalUnit = "";

            double fuQty = 1; //the '1' in 1 MJ
            string fuUnit = ""; //the 'MJ' in 1 MJ
            // conversion factor
            double conversionFactor = 1;

            if (resultsFu != null)
                functionalUnit = GetPreferedVisualizationFunctionalUnitString(dataset, resultsFu, outptutResourceId, out fuQty, out fuUnit);

            if (userFunctionUnit != "")
            {
                try
                {
                    string userSiExp;
                    uint userEqDim;
                    double userSlope;
                    var userFilteredExpression = "";
                    double userIntercept;
                    GuiUtils.FilterExpression(userFunctionUnit, out userSiExp, out userFilteredExpression, out userEqDim, out userSlope, out userIntercept);
                    string deaultSiExp;
                    uint defaultEqDim;
                    double defaultSlope;
                    var defaultFilteredExpression = "";
                    double defaultIntercept;
                    GuiUtils.FilterExpression(fuUnit, out deaultSiExp, out defaultFilteredExpression, out defaultEqDim, out defaultSlope, out defaultIntercept);

                    if (userEqDim != defaultEqDim)
                    {//TODO try to use Can Convert in order to attempt changing the functional unit using the physical properties of the resource
                        if (ConsolePresent)
                        {
                            Console.WriteLine("Error: User defined functional unit \"" + userFunctionUnit + "\" for " +
                                              record.Type +
                                              " " + record.Id + " in Input.txt is not valid for the main output.\n\n");
                            Console.WriteLine("Default functional unit of \"" + functionalUnit +
                                              "\" in the data file will  be used.\n\n");
                            Console.WriteLine("Press any key to continue...\n\n");
                            Console.ReadKey(true);
                        }
                        else
                            conversionFactor = 0;
                    }
                    else
                    {
                        functionalUnit = "1 " + userFunctionUnit;
                        conversionFactor = 1 / defaultSlope / fuQty * userSlope;
                    }
                }
                catch(Exception e)
                {
                    if (ConsolePresent)
                    {
                        Console.WriteLine("Error: User defined functional unit \"" + userFunctionUnit + "\" for " +
                                          record.Type +
                                          " " + record.Id + " in Input.txt cannot be identified.\n\n");
                        Console.WriteLine("Default functional unit of \"" + functionalUnit +
                                          "\" in the data file will be used.\n\n");
                        Console.WriteLine("Press any key to continue...\n\n");
                        Console.ReadKey(true);
                    }
                    else
                        conversionFactor = 0;
                }
            }
            #endregion

            functionalUnit = "Per " + functionalUnit;
            string name = "";
            if (record.Type == 'm')
            {
                if (dataset.MixesData.ContainsKey(record.Id))
                    name = dataset.MixesData[record.Id].Name;
                else
                    name = "Unknown Mix ID " + record.Id;
            }
            else if (record.Type == 'p')
            {
                if (dataset.PathwaysData.ContainsKey(record.Id))
                    name = dataset.PathwaysData[record.Id].Name;
                else
                    name = "Unknown Pathway ID " + record.Id;
            }
            else if (record.Type == 'v')
            {
                if (dataset.VehiclesData.ContainsKey(record.Id))
                    name = dataset.VehiclesData[record.Id].Name;
                else
                    name = "Unknown Vehicle ID " + record.Id;
            }

            string[] nameRow = new string[colCount];
            nameRow[0] = name;
            dt.Rows.Add(nameRow);

            string[] idRow = new string[colCount];
            idRow[0] = "ID=" + record.Id;
            dt.Rows.Add(idRow);

            string[] fuRow = new string[colCount];
            fuRow[0] = functionalUnit;
            dt.Rows.Add(fuRow);

            var rowString = new List<string>(colCount);
            #region total energy and energy groups
            foreach (string resGrp in _resGroups)
            {
                rowString = new List<string>();
                if (resGrp == "Water")
                    rowString.Add(resGrp + " (" + _preferedVolume + ")");
                else
                    rowString.Add(resGrp + " (" + _preferedEnergy + ")");

                foreach (Guid scenarioId in record.Results.Keys)
                {
                    if (forceSingleScenarioID == Guid.Empty || forceSingleScenarioID == scenarioId)
                    {
                        SimpleResultStorage results = record.Results[scenarioId];
                        if (results != null)
                        {
                            double amountRatio = 0;
                            try
                            {
                                amountRatio = GetFunctionalRatio(dataset, results, outptutResourceId) * conversionFactor;
                            }
                            catch (Exception e)
                            {//in case there are erroneous numbers such as functional unit is 0 if the pathway has not been calculated correctly
                                //amount ration will be zero and all the results will be as well
                            }

                            if (resGrp == "Total Energy")
                            {
                                LightValue totalE = results.FinalRe.TotalEnergy();
                                rowString.Add(NiceValueWithAttribute(totalE * amountRatio, _preferedEnergy));
                            }
                            else
                            {
                                Dictionary<int, IValue> resGroupes = results.FinalRe.GroupsToInterfaceDictionary(dataset);
                                int resGrpId = dataset.ResourcesData.Groups.Values.Single(item => item.Name == resGrp).Id;
                                if (resGroupes.ContainsKey(resGrpId))
                                {
                                    LightValue groupValue = new LightValue(resGroupes[resGrpId].Value, resGroupes[resGrpId].UnitExpression);
                                    if (groupValue.Dim == DimensionUtils.ENERGY)
                                        rowString.Add(NiceValueWithAttribute(groupValue * amountRatio, _preferedEnergy));
                                    else
                                        rowString.Add(NiceValueWithAttribute(groupValue * amountRatio, _preferedVolume));
                                }
                                else
                                    rowString.Add(NiceValueWithAttribute(new LightValue(0, _preferedVolume)));
                            }
                        }
                        else
                            rowString.Add(NiceValueWithAttribute(new LightValue(0, _preferedVolume)));
                    }
                }
                dt.Rows.Add(rowString.ToArray());
            }
            #endregion

            #region wtp emissions
            foreach (string poll in _pollutants)
            {
                rowString = new List<string>();

                if (poll == "CO2")
                    rowString.Add("Total " + poll + " (" + _preferedMass + ")");
                else
                    rowString.Add(poll + " (" + _preferedMass + ")");

                foreach (Guid scenarioID in record.Results.Keys)
                {
                    if (forceSingleScenarioID == Guid.Empty || forceSingleScenarioID == scenarioID)
                    {
                        SimpleResultStorage results = record.Results[scenarioID];
                        if (results != null)
                        {
                            double amountRatio = GetFunctionalRatio(dataset, results, outptutResourceId) * conversionFactor;

                            int polId = dataset.GasesData.Values.Single(item => item.Name == poll).Id;
                            if (results.FinalEm.ContainsKey(polId))
                            {
                                if (poll == "CO2")
                                {
                                    LightValue co2Sum = new LightValue(0, DimensionUtils.MASS);
                                    if (results.FinalEm.ContainsKey(polId))
                                        co2Sum += new LightValue(results.FinalEm[polId], DimensionUtils.MASS);
                                    if (results.FinalEm.ContainsKey(dataset.GasesData.Values.Single(item => item.Name == "CO2_Biogenic").Id))
                                        co2Sum += new LightValue(results.FinalEm[dataset.GasesData.Values.Single(item => item.Name == "CO2_Biogenic").Id], DimensionUtils.MASS);

                                    rowString.Add(NiceValueWithAttribute(
                                    co2Sum * amountRatio
                                    , _preferedMass));

                                }
                                else
                                {
                                    rowString.Add(NiceValueWithAttribute(
                                    new LightValue(results.FinalEm[polId], DimensionUtils.MASS) * amountRatio
                                    , _preferedMass));
                                }
                            }
                            else
                            {
                                rowString.Add(NiceValueWithAttribute(
                                    new LightValue(0, _preferedMass)));
                            }
                        }
                        else
                        {
                            rowString.Add(NiceValueWithAttribute(
                                    new LightValue(0, _preferedMass)));
                        }
                    }
                }
                dt.Rows.Add(rowString.ToArray());
            }
            #endregion

            #region wtp Groups (here only GHG 100)
            foreach (string resGrp in _polGroups)
            {
                rowString = new List<string>();
                rowString.Add(resGrp + " (" + _preferedMass + ")");
                foreach (Guid scenarioId in record.Results.Keys)
                {
                    if (forceSingleScenarioID == Guid.Empty || forceSingleScenarioID == scenarioId)
                    {
                        SimpleResultStorage results = record.Results[scenarioId];
                        if (results != null)
                        {
                            double amountRatio = GetFunctionalRatio(dataset, results, outptutResourceId) * conversionFactor;

                            Dictionary<int, IValue> emGroupes = results.FinalEm.GroupsToInterfaceDictionary(dataset);
                            int grpId = dataset.GasesData.Groups.Values.Single(item => item.Name == resGrp).Id;
                            if (emGroupes.ContainsKey(grpId))
                                rowString.Add(NiceValueWithAttribute(new LightValue(emGroupes[grpId].Value, emGroupes[grpId].UnitExpression) * amountRatio, _preferedMass));
                            else
                                rowString.Add(NiceValueWithAttribute(new LightValue(0, _preferedMass)));
                        }
                        else
                            rowString.Add(NiceValueWithAttribute(new LightValue(0, _preferedMass)));
                    }
                }
                dt.Rows.Add(rowString.ToArray());
            }
            #endregion

            dt.Rows.Add(new string[colCount]);
            dt.Rows.Add(new string[colCount]);

            #region urban emissions
            foreach (string poll in _urbanPoll)
            {
                rowString = new List<string> {"Urban " + poll + " (" + _preferedMass + ")"};
                foreach (Guid scenarioId in record.Results.Keys)
                {
                    if (forceSingleScenarioID == Guid.Empty || forceSingleScenarioID == scenarioId)
                    {
                        SimpleResultStorage results = record.Results[scenarioId]; if (results != null)
                        {
                            double amountRatio = GetFunctionalRatio(dataset, results, outptutResourceId) * conversionFactor;

                            int polId = dataset.GasesData.Values.Single(item => item.Name == poll).Id;
                            if (results.FinalEmUr.ContainsKey(polId))
                                rowString.Add(NiceValueWithAttribute(new LightValue(results.FinalEmUr[polId], DimensionUtils.MASS) * amountRatio, _preferedMass));
                            else
                                rowString.Add(NiceValueWithAttribute(new LightValue(0, _preferedMass)));
                        }
                        else
                            rowString.Add(NiceValueWithAttribute(new LightValue(0, _preferedMass)));
                    }
                }
                dt.Rows.Add(rowString.ToArray());
            }
            #endregion

            return dt;
        }

        #endregion
    }
}
