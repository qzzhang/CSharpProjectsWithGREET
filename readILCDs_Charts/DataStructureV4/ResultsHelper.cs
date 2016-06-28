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

using System.Collections.Generic;
using Greet.DataStructureV4.Entities;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.UnitLib3;

namespace Greet.DataStructureV4
{
    public static class ResultsHelper
    {
        public static string TOTAL_ENERGY = "Total Energy";
        public static string URBAN_SUFFIX = " Urban";

        #region Members

        /// <summary>
        /// This method sums up all the fuels in their groups and returns a DVDict which contains all the groupped values
        /// </summary>
        /// <returns></returns>
        public static DVDict GenerateGroups(Resources resourcesData, ResourceAmounts amounts)
        {
            DVDict groups = new DVDict();
            //taking care of addint the amount in the groups list
            foreach (KeyValuePair<int, LightValue> val in amounts)
            {
                LightValue amount = val.Value;
                foreach (int membership in resourcesData[val.Key].Memberships)
                {
                    if (resourcesData.Groups.ContainsKey(membership))
                    {
                        Group group = resourcesData.Groups[membership];
                        List<int> group_id_and_includes = new List<int>();
                        foreach (int sub_group in group.IncludeInGroups)
                            if (resourcesData[val.Key].Memberships.Contains(sub_group) == false)
                                group_id_and_includes.Add(sub_group);
                        group_id_and_includes.Add(group.Id);

                        foreach (int gid in group_id_and_includes)
                        {
                            if (groups.ContainsKey(gid))
                            {
                                if (groups[gid].Dim == amount.Dim)
                                    groups[gid] += amount.Value;
                                else
                                    groups[gid] += resourcesData[val.Key].ConvertTo(groups[gid].Dim, amount);
                            }
                            else
                            {
                                if (resourcesData[val.Key].CanConvertTo(DimensionUtils.ENERGY, amount)) //hardcoded
                                    groups.Add(gid, resourcesData[val.Key].ConvertTo(DimensionUtils.ENERGY, amount)); //hardcoded
                            }
                        }

                    }
                }
            }
            return groups;
        }

        /// <summary>
        /// Get the total amount of energy and the groups necessary for the vehicles results display
        /// This is added in a dictionary from where we know the order of things
        /// </summary>
        /// <param name="amounts"></param>
        /// <returns></returns>
        public static Dictionary<string, LightValue> GetSelectiveResourcesResults(Resources resourcesData, ResourceAmounts amounts)
        {
            Dictionary<string, LightValue> results = new Dictionary<string, LightValue>();
            DVDict groups = GenerateGroups(resourcesData, amounts);
            results.Add("Total Energy", amounts.TotalEnergy());

            // Add the groups that we need to show in the vehicle results
            if (groups.ContainsKey(4))
                results.Add(resourcesData.Groups[4].Name, groups[4]);
            else
                results.Add(resourcesData.Groups[4].Name, new LightValue(0.0, DimensionUtils.ENERGY));

            if (groups.ContainsKey(3))
                results.Add(resourcesData.Groups[3].Name, groups[3]);
            else
                results.Add(resourcesData.Groups[3].Name, new LightValue(0.0, DimensionUtils.ENERGY));


            if (groups.ContainsKey(2))
                results.Add(resourcesData.Groups[2].Name, groups[2]);
            else
                results.Add(resourcesData.Groups[2].Name, new LightValue(0.0, DimensionUtils.ENERGY));


            if (groups.ContainsKey(1))
                results.Add(resourcesData.Groups[1].Name, groups[1]);
            else
                results.Add(resourcesData.Groups[1].Name, new LightValue(0.0, DimensionUtils.ENERGY));

            return results;
        }

        /// <summary>
        /// Access the results of a manufacturing group for GUI display
        /// Sums up all the different upstreams from the vehicle components and other used within that manufacturing category
        /// </summary>
        /// <param name="gData"></param>
        /// <param name="vehicleManufacturing"></param>
        /// <param name="format"><para>0: Per MJ</para>
        /// <para>1: Per 100km</para>
        /// <para>2: Per mile</para>
        /// <para>3: Per ton*mile</para>
        /// <para>4: Per tonne*km</para>
        /// <para>5: Per passenger mile</para>
        /// <para>6: Per passenger kilometer</para></param>
        /// <returns></returns>
        public static Dictionary<string, LightValue> ResultDictionaryManufacturingCategory(GData data, Vehicle vehicle, VehicleManufacturing vehicleManufacturing, ParameterTS payload, ParameterTS passengers, int format = 0, List<int> optionalResourceIDs = null)
        {
            #region Adding items to Result Dictionary
            Dictionary<string, LightValue> resultDictionary = new Dictionary<string, LightValue>();

            ResourceAmounts manufacturingCalculatedOperationENERGY = new ResourceAmounts();
            manufacturingCalculatedOperationENERGY.BottomDim = DimensionUtils.LENGTH;
            EmissionAmounts manufacturingCalculatedOperationEMISSIONS = new EmissionAmounts();
            manufacturingCalculatedOperationEMISSIONS.BottomDim = DimensionUtils.LENGTH;
            EmissionAmounts manufacturingCalculatedOperationEMISSIONSUrban = new EmissionAmounts();
            manufacturingCalculatedOperationEMISSIONSUrban.BottomDim = DimensionUtils.LENGTH;

            for (int i = 0; i < vehicleManufacturing.Materials.Count; i++)
            {
                manufacturingCalculatedOperationENERGY.Addition(vehicleManufacturing.CalculatedMaterialsEnergy[i]);
                manufacturingCalculatedOperationEMISSIONS.Addition(vehicleManufacturing.CalculatedMaterialsEmissions[i]);
                manufacturingCalculatedOperationEMISSIONSUrban.Addition(vehicleManufacturing.CalculatedMaterialsEmissionsUrban[i]);
            }

            #region ENERGY
            foreach (KeyValuePair<string, LightValue> pair in GetSelectiveResourcesResults(data.ResourcesData, manufacturingCalculatedOperationENERGY))
            {
                LightValue lv = new LightValue(pair.Value.Value, DimensionUtils.Minus(pair.Value.Dim, manufacturingCalculatedOperationENERGY.BottomDim));
                resultDictionary.Add(pair.Key, lv);
            }
            if (optionalResourceIDs != null)
            {
                foreach (int resID in optionalResourceIDs)
                {
                    //Adding the Water Resource Values
                    if (manufacturingCalculatedOperationENERGY.ContainsKey(resID))
                    {
                        LightValue unitMeter = new LightValue(1.0, DimensionUtils.LENGTH);
                        resultDictionary.Add(data.ResourcesData[resID].Name,
                            manufacturingCalculatedOperationENERGY[resID]/unitMeter);
                    }
                }
            }

            #endregion

            #region Emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (manufacturingCalculatedOperationEMISSIONS.ContainsKey(i))
                {
                    LightValue lv = new LightValue(manufacturingCalculatedOperationEMISSIONS[i], DimensionUtils.Minus(DimensionUtils.MASS, manufacturingCalculatedOperationEMISSIONS.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name, lv);
                }
            }

            //Adding Gas group value to the results
            Dictionary<int, IValue> groups = manufacturingCalculatedOperationEMISSIONS.GroupsToInterfaceDictionary(data);
            for (int i = 0; i < data.GasGroups.Count; i++)
                if (data.GasGroups[i].ShowInResults)
                {
                    if (groups.ContainsKey(data.GasGroups[i].Id))
                    {
                        LightValue lv1 = new LightValue(groups[data.GasGroups[i].Id].Value, DimensionUtils.Minus(DimensionUtils.MASS, manufacturingCalculatedOperationEMISSIONS.BottomDim));
                        resultDictionary.Add(data.GasGroups[i].Name, lv1);
                    }
                }

            #endregion

            #region Urban Emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (manufacturingCalculatedOperationEMISSIONSUrban.ContainsKey(i))
                {
                    LightValue lv = new LightValue(manufacturingCalculatedOperationEMISSIONSUrban[i], DimensionUtils.Minus(DimensionUtils.MASS, manufacturingCalculatedOperationEMISSIONSUrban.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name + " Urban", lv);
                }
            }
            #endregion
            #endregion

            #region Convert the functional unit of all items in the Operational Energy Result Dictionary

            LightValue conversionFactor = ResultsDictionaryWTWConversionFactor(vehicle, format);
            Dictionary<string, LightValue> convertedValues = new Dictionary<string, LightValue>();
            foreach (KeyValuePair<string, LightValue> original in resultDictionary)
            {
                LightValue converted = original.Value * conversionFactor;
                convertedValues.Add(original.Key, converted);
            }

            #endregion

            return convertedValues;
        }

        /// <summary>
        /// Access the results of a manufacturing material item for GUI display
        /// </summary>
        /// <param name="gData"></param>
        /// <param name="vehicleManufacturing"></param>
        /// <param name="j">Index of the item in the materials for the vehicleManufacturing object</param>
        /// <param name="format"><para>0: Per MJ</para>
        /// <para>1: Per 100km</para>
        /// <para>2: Per mile</para>
        /// <para>3: Per ton*mile</para>
        /// <para>4: Per tonne*km</para>
        /// <para>5: Per passenger mile</para>
        /// <para>6: Per passenger kilometer</para></param>
        /// <returns></returns>
        public static Dictionary<string, LightValue> ResultDictionaryManufacturingItem(GData data, Vehicle vehicle, VehicleManufacturing vehicleManufacturing, int j, ParameterTS payload, ParameterTS passengers, int format = 0, List<int> optionalResourceIDs = null)
        {
            #region Adding items to Result Dictionary
            Dictionary<string, LightValue> resultDictionary = new Dictionary<string, LightValue>();

            ResourceAmounts manufacturingCalculatedOperationENERGY = vehicleManufacturing.CalculatedMaterialsEnergy[j];
            EmissionAmounts manufacturingCalculatedOperationEMISSIONS = vehicleManufacturing.CalculatedMaterialsEmissions[j];
            EmissionAmounts manufacturingCalculatedOperationEMISSIONSUrban = new EmissionAmounts(vehicleManufacturing.CalculatedMaterialsEmissionsUrban[j]);

            #region ENERGY
            foreach (KeyValuePair<string, LightValue> pair in GetSelectiveResourcesResults(data.ResourcesData, manufacturingCalculatedOperationENERGY))
            {
                LightValue lv = new LightValue(pair.Value.Value, DimensionUtils.Minus(pair.Value.Dim, manufacturingCalculatedOperationENERGY.BottomDim));
                resultDictionary.Add(pair.Key, lv);
            }
            //Adding the Water Resource Values
            if (optionalResourceIDs != null)
            {
                foreach (int resID in optionalResourceIDs)
                {
                    //Adding the Water Resource Values
                    if (manufacturingCalculatedOperationENERGY.ContainsKey(resID))
                    {
                        LightValue unitMeter = new LightValue(1.0, DimensionUtils.LENGTH);
                        resultDictionary.Add(data.ResourcesData[resID].Name,
                            manufacturingCalculatedOperationENERGY[resID]/unitMeter);
                    }
                }
            }

            #endregion

            #region Emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (manufacturingCalculatedOperationEMISSIONS.ContainsKey(i))
                {
                    LightValue lv = new LightValue(manufacturingCalculatedOperationEMISSIONS[i], DimensionUtils.Minus(DimensionUtils.MASS, manufacturingCalculatedOperationEMISSIONS.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name, lv);
                }
            }

            //Adding Gas group value to the results
            Dictionary<int, IValue> groups = manufacturingCalculatedOperationEMISSIONS.GroupsToInterfaceDictionary(data);
            for (int i = 0; i < data.GasGroups.Count; i++)
                if (data.GasGroups[i].ShowInResults)
                {
                    if (groups.ContainsKey(data.GasGroups[i].Id))
                    {
                        LightValue lv1 = new LightValue(groups[data.GasGroups[i].Id].Value, DimensionUtils.Minus(DimensionUtils.MASS, manufacturingCalculatedOperationEMISSIONS.BottomDim));
                        resultDictionary.Add(data.GasGroups[i].Name, lv1);
                    }
                }

            #endregion

            #region Urban Emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (manufacturingCalculatedOperationEMISSIONSUrban.ContainsKey(i))
                {
                    LightValue lv = new LightValue(manufacturingCalculatedOperationEMISSIONSUrban[i], DimensionUtils.Minus(DimensionUtils.MASS, manufacturingCalculatedOperationEMISSIONSUrban.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name + " Urban", lv);
                }
            }
            #endregion
            #endregion

            #region Convert the functional unit of all items in the Operational Energy Result Dictionary

            LightValue conversionFactor = ResultsDictionaryWTWConversionFactor(vehicle, format);
            Dictionary<string, LightValue> convertedValues = new Dictionary<string, LightValue>();
            foreach (KeyValuePair<string, LightValue> original in resultDictionary)
            {
                LightValue converted = original.Value * conversionFactor;
                convertedValues.Add(original.Key, converted);
            }

            #endregion

            return convertedValues;
        }

        /// <summary>
        /// Access the results of the mode for GUI Display
        /// </summary>
        /// <param name="data">Dataset containing resources and emissions in order to map item IDs and their names</param>
        /// <param name="vehicle">The vehicle from which results are being extracted</param>
        /// <param name="passengers">The number of passengers, only necessary if results are asked per passenger mile or passenger kilometer</param>
        /// <param name="format"><para>0: Per MJ</para>
        /// <para>1: Per 100km</para>
        /// <para>2: Per mile</para>
        /// <para>3: Per ton*mile</para>
        /// <para>4: Per tonne*km</para>
        /// <para>5: Per passenger mile</para>
        /// <para>6: Per passenger kilometer</para></param>
        /// <param name="mode">The mode from which results are being extracted</param>
        /// <param name="payload">The payload, only necessary if the results are asked per unit of mass*distance</param>
        /// <param name="optionalResourceIDs">List of individual resources IDs that are added to the tables in the vehicle results in order to extract results.</param>
        /// <returns></returns>
        public static Dictionary<string, LightValue> ResultDictionaryMode(GData data, Vehicle vehicle, VehicleOperationalMode mode, ParameterTS payload, ParameterTS passengers, int format = 0, List<int> optionalResourceIDs = null)
        {
            #region Adding items to Result Dictionary
            Dictionary<string, LightValue> resultDictionary = new Dictionary<string, LightValue>();

            ResourceAmounts modeCalculatedOperationENERGY = mode.CalculatedOperationEnergy();
            EmissionAmounts modeCalculatedOperationEMISSIONS = mode.CalculatedOperationEmissions();
            EmissionAmounts modeCalculatedOperationEMISSIONSUrban = mode.CalculatedOperationEmissionsUrban();

            #region ENERGY
            foreach (KeyValuePair<string, LightValue> pair in GetSelectiveResourcesResults(data.ResourcesData, modeCalculatedOperationENERGY))
            {
                LightValue lv = new LightValue(pair.Value.Value, DimensionUtils.Minus(pair.Value.Dim, modeCalculatedOperationENERGY.BottomDim));
                resultDictionary.Add(pair.Key, lv);
            }

            //Adding the Water Resource Values
            if (optionalResourceIDs != null)
            {
                foreach (int resID in optionalResourceIDs)
                {
                    if (modeCalculatedOperationENERGY.ContainsKey(resID))
                    {
                        LightValue unitMeter = new LightValue(1.0, DimensionUtils.LENGTH);
                        resultDictionary.Add(data.ResourcesData[resID].Name,
                            modeCalculatedOperationENERGY[resID]/unitMeter);
                            //HORRIBLE ID HARDCODING HARDCODED for water
                    }
                }
            }

            #endregion

            #region Emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (modeCalculatedOperationEMISSIONS.ContainsKey(i))
                {
                    LightValue lv = new LightValue(modeCalculatedOperationEMISSIONS[i], DimensionUtils.Minus(DimensionUtils.MASS, modeCalculatedOperationEMISSIONS.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name, lv);
                }
            }

            //Adding Gas group value to the results
            Dictionary<int, IValue> groupResults = modeCalculatedOperationEMISSIONS.GroupsToInterfaceDictionary(data);
            for (int i = 0; i < data.GasGroups.Count; i++)
                if (data.GasGroups[i].ShowInResults)
                {
                    if (groupResults.ContainsKey(data.GasGroups[i].Id))
                    {
                        LightValue lv1 = new LightValue(groupResults[data.GasGroups[i].Id].Value, DimensionUtils.Minus(DimensionUtils.MASS, modeCalculatedOperationEMISSIONS.BottomDim));
                        resultDictionary.Add(data.GasGroups[i].Name, lv1);
                    }
                }

            #endregion

            #region Urban Emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (modeCalculatedOperationEMISSIONSUrban.ContainsKey(i))
                {
                    LightValue lv = new LightValue(modeCalculatedOperationEMISSIONSUrban[i], DimensionUtils.Minus(DimensionUtils.MASS, modeCalculatedOperationEMISSIONSUrban.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name + " Urban", lv);
                }
            }
            #endregion
            #endregion

            #region Convert the functional unit of all items in the Operational Energy Result Dictionary

            LightValue conversionFactor = ResultsDictionaryWTWConversionFactor(vehicle, format);
            Dictionary<string, LightValue> convertedValues = new Dictionary<string, LightValue>();
            foreach (KeyValuePair<string, LightValue> original in resultDictionary)
            {
                LightValue converted = original.Value * conversionFactor;
                convertedValues.Add(original.Key, converted);
            }

            #endregion

            return convertedValues;
        }

        /// <summary>
        /// Access the results of the vehcles for GUI Display of the vehicle operation without upstream
        /// </summary>
        /// <param name="data"></param>
        /// <param name="vehicle"></param>
        /// <param name="format"><para>0: Per MJ</para>
        /// <para>1: Per 100km</para>
        /// <para>2: Per mile</para>
        /// <para>3: Per ton*mile</para>
        /// <para>4: Per tonne*km</para>
        /// <para>5: Per passenger mile</para>
        /// <para>6: Per passenger kilometer</para></param>
        /// <returns></returns>
        public static Dictionary<string, LightValue> ResultDictionaryOperationNoUpstream(GData data, Vehicle vehicle, int format = 0, List<int> optionalResourceIDs = null)
        {
            Dictionary<string, LightValue> resultDictionary = new Dictionary<string, LightValue>();

            #region Adding items to Result Dictionary
            //Operational Energy
            foreach (KeyValuePair<string, LightValue> pair in GetSelectiveResourcesResults(data.ResourcesData, vehicle.VehicleOperationEnergy))
            {
                LightValue lv = new LightValue(pair.Value.Value, DimensionUtils.Minus(pair.Value.Dim, vehicle.VehicleOperationEnergy.BottomDim));
                resultDictionary.Add(pair.Key, lv);
            }

            //Adding the Water Resource Values
            if (optionalResourceIDs != null)
            {
                foreach (int resID in optionalResourceIDs)
                {
                    if (vehicle.VehicleOperationEnergy.ContainsKey(resID))
                    {
                        LightValue unitMeter = new LightValue(1.0, DimensionUtils.LENGTH);
                        resultDictionary.Add(data.ResourcesData[resID].Name,
                            vehicle.VehicleOperationEnergy[resID]/unitMeter);
                        //HORRIBLE ID HARDCODING HARDCODED for water
                    }
                }
            }

            //Operational Emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (vehicle.VehicleOperationEmissions.ContainsKey(i))
                {
                    LightValue lv = new LightValue(vehicle.VehicleOperationEmissions[i], DimensionUtils.Minus(DimensionUtils.MASS, vehicle.VehicleOperationEmissions.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name, lv);
                }
            }


            //Adding Gas group value to the results
            Dictionary<int, IValue> groups = vehicle.VehicleOperationEmissions.GroupsToInterfaceDictionary(data);
            foreach (IGroup group in data.GasGroups)
            {
                if (group.ShowInResults)
                {
                    if (groups.ContainsKey(group.Id))
                    {
                        LightValue lv1 = new LightValue(groups[group.Id].Value, DimensionUtils.Minus(DimensionUtils.MASS, vehicle.VehicleOperationEmissions.BottomDim));
                        resultDictionary.Add(group.Name, lv1);
                    }
                }
            }
            //Operational Urban Emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (vehicle.VehicleOperationEmissionsUrban.ContainsKey(i))
                {
                    LightValue lv = new LightValue(vehicle.VehicleOperationEmissionsUrban[i], DimensionUtils.Minus(DimensionUtils.MASS, vehicle.VehicleOperationEmissionsUrban.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name + " Urban", lv);
                }
            }
            #endregion

            #region Convert the functional unit of all items in the Operational Energy Result Dictionary

            LightValue conversionFactor = ResultsDictionaryWTWConversionFactor(vehicle, format);
            Dictionary<string, LightValue> convertedValues = new Dictionary<string, LightValue>();
            foreach (KeyValuePair<string, LightValue> original in resultDictionary)
            {
                LightValue converted = original.Value * conversionFactor;
                convertedValues.Add(original.Key, converted);
            }

            #endregion

            return convertedValues;
        }

        /// <summary>
        /// Access the results of the vehcles for GUI Display
        /// </summary>
        /// <param name="data"></param>
        /// <param name="vehicle"></param>
        /// <param name="format"><para>0: Per MJ</para>
        /// <para>1: Per 100km</para>
        /// <para>2: Per mile</para>
        /// <para>3: Per ton*mile</para>
        /// <para>4: Per tonne*km</para>
        /// <para>5: Per passenger mile</para>
        /// <para>6: Per passenger kilometer</para></param>
        /// <returns></returns>
        public static Dictionary<string, LightValue> ResultDictionaryTiresBrakesWear(GData data, Vehicle vehicle, int format = 0, List<int> optionalResourceIDs = null)
        {
            Dictionary<string, LightValue> resultDictionary = new Dictionary<string, LightValue>();

            #region Adding items to Result Dictionary

            //Tires and brake wear emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (vehicle.TireBreakWearEmissions.ContainsKey(i))
                {
                    LightValue lv = new LightValue(vehicle.TireBreakWearEmissions[i], DimensionUtils.Minus(DimensionUtils.MASS, vehicle.TireBreakWearEmissions.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name, lv);
                }
            }

            #endregion

            #region Convert the functional unit of all items in the Operational Energy Result Dictionary

            LightValue conversionFactor = ResultsDictionaryWTWConversionFactor(vehicle, format);
            Dictionary<string, LightValue> convertedValues = new Dictionary<string, LightValue>();
            foreach (KeyValuePair<string, LightValue> original in resultDictionary)
            {
                LightValue converted = original.Value * conversionFactor;
                convertedValues.Add(original.Key, converted);
            }

            #endregion

            return convertedValues;
        }

        /// <summary>
        /// Access the results of the vehcles for GUI Display
        /// </summary>
        /// <param name="data"></param>
        /// <param name="vehicle"></param>
        /// <param name="format"><para>0: Per MJ</para>
        /// <para>1: Per 100km</para>
        /// <para>2: Per mile</para>
        /// <para>3: Per ton*mile</para>
        /// <para>4: Per tonne*km</para>
        /// <para>5: Per passenger mile</para>
        /// <para>6: Per passenger kilometer</para></param>
        /// <returns></returns>
        public static Dictionary<string, LightValue> ResultDictionaryTotal(GData data, Vehicle vehicle, int format = 0, List<int> optionalResourceIDs = null)
        {
            Dictionary<string, LightValue> resultDictionary = new Dictionary<string, LightValue>();

            #region Adding items to Result Dictionary
            //total energies
            foreach (KeyValuePair<string, LightValue> pair in GetSelectiveResourcesResults(data.ResourcesData, vehicle.TotalEnergy))
            {
                LightValue lv = new LightValue(pair.Value.Value, DimensionUtils.Minus(pair.Value.Dim, vehicle.TotalEnergy.BottomDim));
                resultDictionary.Add(pair.Key, lv);
            }

            //Adding the Water Resource Values
            if (optionalResourceIDs != null)
            {
                foreach (int resID in optionalResourceIDs)
                {
                    if (vehicle.TotalEnergy.ContainsKey(resID))
                    {
                        LightValue unitMeter = new LightValue(1.0, DimensionUtils.LENGTH);
                        resultDictionary.Add(data.ResourcesData[resID].Name, vehicle.TotalEnergy[resID]/unitMeter);
                    }
                }
            }

            #region total emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (vehicle.TotalEmissions.ContainsKey(i))
                {
                    LightValue lv = new LightValue(vehicle.TotalEmissions[i], DimensionUtils.Minus(DimensionUtils.MASS, vehicle.TotalEmissions.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name, lv);
                }
            }

            //Adding Gas group value to the results
            Dictionary<int, IValue> groupResults = vehicle.TotalEmissions.GroupsToInterfaceDictionary(data);
            for (int i = 0; i < data.GasGroups.Count; i++)
                if (data.GasGroups[i].ShowInResults)
                {
                    if (groupResults.ContainsKey(data.GasGroups[i].Id))
                    {
                        LightValue lv1 = new LightValue(groupResults[data.GasGroups[i].Id].Value, DimensionUtils.Minus(DimensionUtils.MASS, vehicle.TotalEmissions.BottomDim));
                        resultDictionary.Add(data.GasGroups[i].Name, lv1);
                    }
                }

            #endregion

            #region total urban emissions
            EmissionAmounts totalEmissionsUrban = vehicle.TotalEmissionsUrban;
            foreach (int i in data.GasesData.Keys)
            {
                if (vehicle.TotalEmissionsUrban.ContainsKey(i))
                {
                    LightValue lv = new LightValue(totalEmissionsUrban[i], DimensionUtils.Minus(DimensionUtils.MASS, vehicle.TotalEmissionsUrban.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name + " Urban", lv);
                }
            }
            #endregion
            #endregion

            #region Convert the functional unit of all items in the Operational Energy Result Dictionary

            LightValue conversionFactor = ResultsDictionaryWTWConversionFactor(vehicle, format);
            Dictionary<string, LightValue> convertedValues = new Dictionary<string, LightValue>();
            foreach (KeyValuePair<string, LightValue> original in resultDictionary)
            {
                LightValue converted = original.Value * conversionFactor;
                convertedValues.Add(original.Key, converted);
            }

            #endregion

            return convertedValues;
        }

        /// <summary>
        /// Access the results of the vehcles for GUI Display of the vehicle operation with upstream
        /// </summary>
        /// <param name="data"></param>
        /// <param name="vehicle"></param>
        /// <param name="format"><para>0: Per MJ</para>
        /// <para>1: Per 100km</para>
        /// <para>2: Per mile</para>
        /// <para>3: Per ton*mile</para>
        /// <para>4: Per tonne*km</para>
        /// <para>5: Per passenger mile</para>
        /// <para>6: Per passenger kilometer</para></param>
        /// <returns></returns>
        public static Dictionary<string, LightValue> ResultDictionaryWTP(GData data, Vehicle vehicle, int format = 0, List<int> optionalResourceIDs = null)
        {
            Dictionary<string, LightValue> resultDictionary = new Dictionary<string, LightValue>();
            foreach (VehicleOperationalMode mode in vehicle.Modes)
            {
                #region Adding items to Result Dictionary
                //Operational Energy with upstream
                ResourceAmounts resourcesWithUpstream = mode.CalculatedOperationEnergyWithUpstream();
                ResourceAmounts resourcesWithout = mode.CalculatedOperationEnergy();
                ResourceAmounts resourcesWTP = resourcesWithUpstream - resourcesWithout;
                foreach (KeyValuePair<string, LightValue> pair in GetSelectiveResourcesResults(data.ResourcesData, resourcesWTP))
                {
                    LightValue lv = new LightValue(pair.Value.Value, DimensionUtils.Minus(pair.Value.Dim, vehicle.VehicleOperationEnergy.BottomDim));
                    if (resultDictionary.ContainsKey(pair.Key))
                        resultDictionary[pair.Key] += lv;
                    else
                        resultDictionary.Add(pair.Key, lv);
                }

                //Adding the Water Resource Values
                if (optionalResourceIDs != null)
                {
                    foreach (int resID in optionalResourceIDs)
                    {
                        if (resourcesWTP.ContainsKey(resID))
                        {
                            LightValue unitMeter = new LightValue(1.0, DimensionUtils.LENGTH);

                            if (resultDictionary.ContainsKey(data.ResourcesData[resID].Name))
                                resultDictionary[data.ResourcesData[resID].Name] += resourcesWTP[resID]/unitMeter;
                            else
                                resultDictionary.Add(data.ResourcesData[resID].Name, resourcesWTP[resID]/unitMeter);
                        }
                    }
                }

                //Operational Emissions
                EmissionAmounts emissionsWithUpstream = mode.CalculatedOperationEmissionsWithUpstream();
                EmissionAmounts emissionsWithout = mode.CalculatedOperationEmissions();
                EmissionAmounts emissionsWTP = emissionsWithUpstream - emissionsWithout;
                foreach (int i in data.GasesData.Keys)
                {
                    if (emissionsWTP.ContainsKey(i))
                    {
                        LightValue lv = new LightValue(emissionsWTP[i], DimensionUtils.Minus(DimensionUtils.MASS, emissionsWTP.BottomDim));
                        if (resultDictionary.ContainsKey(data.GasesData[i].Name))
                            resultDictionary[data.GasesData[i].Name] += lv;
                        else
                            resultDictionary.Add(data.GasesData[i].Name, lv);
                    }
                }


                //Adding Gas group value to the results
                Dictionary<int, IValue> groups = emissionsWTP.GroupsToInterfaceDictionary(data);
                foreach (IGroup group in data.GasGroups)
                {
                    if (group.ShowInResults)
                    {
                        if (groups.ContainsKey(group.Id))
                        {
                            LightValue lv1 = new LightValue(groups[group.Id].Value, DimensionUtils.Minus(DimensionUtils.MASS, emissionsWTP.BottomDim));
                            if (resultDictionary.ContainsKey(group.Name))
                                resultDictionary[group.Name] += lv1;
                            else
                                resultDictionary.Add(group.Name, lv1);
                        }
                    }
                }

                //Operational Urban Emissions
                EmissionAmounts urbanEmissionsWithUpstream = mode.CalculatedOperationEmissionsUrbanWithUpstream();
                EmissionAmounts urbanEmissionsWithout = mode.CalculatedOperationEmissionsUrban();
                EmissionAmounts urbanEmissionsWTP = urbanEmissionsWithUpstream - urbanEmissionsWithout;
                foreach (int i in data.GasesData.Keys)
                {
                    if (urbanEmissionsWTP.ContainsKey(i))
                    {
                        LightValue lv = new LightValue(urbanEmissionsWTP[i], DimensionUtils.Minus(DimensionUtils.MASS, urbanEmissionsWTP.BottomDim));
                        if (resultDictionary.ContainsKey(data.GasesData[i].Name + " Urban"))
                            resultDictionary[data.GasesData[i].Name + " Urban"] += lv;
                        else
                            resultDictionary.Add(data.GasesData[i].Name + " Urban", lv);
                    }
                }
                #endregion
            }
            #region Convert the functional unit of all items in the Operational Energy Result Dictionary

            LightValue conversionFactor = ResultsDictionaryWTWConversionFactor(vehicle, format);
            Dictionary<string, LightValue> convertedValues = new Dictionary<string, LightValue>();
            foreach (KeyValuePair<string, LightValue> original in resultDictionary)
            {
                LightValue converted = original.Value * conversionFactor;
                convertedValues.Add(original.Key, converted);
            }

            #endregion

            return convertedValues;
        }

        /// <summary>
        /// Access the results of the vehcles for GUI Display of the vehicle operation with upstream
        /// </summary>
        /// <param name="format"><para>0: Per MJ</para>
        /// <para>1: Per 100km</para>
        /// <para>2: Per mile</para>
        /// <para>3: Per ton*mile</para>
        /// <para>4: Per tonne*km</para>
        /// <para>5: Per passenger mile</para>
        /// <para>6: Per passenger kilometer</para></param>
        /// <returns></returns>
        public static Dictionary<string, LightValue> ResultsDictionaryWTW(GData data, Vehicle vehicle, int format = 0, List<int> optionalResourceIDs = null)
        {
            Dictionary<string, LightValue> resultDictionary = new Dictionary<string, LightValue>();

            #region Adding items to Result Dictionary
            //Operational Energy
            foreach (KeyValuePair<string, LightValue> pair in GetSelectiveResourcesResults(data.ResourcesData, vehicle.CalculatedWTWEnergy))
            {
                LightValue lv = new LightValue(pair.Value.Value, DimensionUtils.Minus(pair.Value.Dim, vehicle.CalculatedWTWEnergy.BottomDim));
                resultDictionary.Add(pair.Key, lv);
            }

            //Adding the Water Resource Values
            if (optionalResourceIDs != null)
            {
                foreach (int resID in optionalResourceIDs)
                {
                    if (vehicle.CalculatedWTWEnergy.ContainsKey(resID))
                    {
                        LightValue unitMeter = new LightValue(1.0, DimensionUtils.LENGTH);
                        resultDictionary.Add(data.ResourcesData[resID].Name,
                            vehicle.CalculatedWTWEnergy[resID]/unitMeter); //HORRIBLE ID HARDCODING HARDCODED for water
                    }
                }
            }

            //Operational Emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (vehicle.CalculatedWTWEmissions.ContainsKey(i))
                {
                    LightValue lv = new LightValue(vehicle.CalculatedWTWEmissions[i], DimensionUtils.Minus(DimensionUtils.MASS, vehicle.CalculatedWTWEmissions.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name, lv);
                }
            }


            //Adding Gas group value to the results
            Dictionary<int, IValue> groups = vehicle.CalculatedWTWEmissions.GroupsToInterfaceDictionary(data);
            foreach (IGroup group in data.GasGroups)
            {
                if (group.ShowInResults)
                {
                    if (groups.ContainsKey(group.Id))
                    {
                        LightValue lv1 = new LightValue(groups[group.Id].Value, DimensionUtils.Minus(DimensionUtils.MASS, vehicle.CalculatedWTWEmissions.BottomDim));
                        resultDictionary.Add(group.Name, lv1);
                    }
                }
            }
            //Operational Urban Emissions
            foreach (int i in data.GasesData.Keys)
            {
                if (vehicle.CalculatedWTWEmissionsUrban.ContainsKey(i))
                {
                    LightValue lv = new LightValue(vehicle.CalculatedWTWEmissionsUrban[i], DimensionUtils.Minus(DimensionUtils.MASS, vehicle.CalculatedWTWEmissionsUrban.BottomDim));
                    resultDictionary.Add(data.GasesData[i].Name + " Urban", lv);
                }
            }
            #endregion

            #region Convert the functional unit of all items in the Operational Energy Result Dictionary

            LightValue conversionFactor = ResultsDictionaryWTWConversionFactor(vehicle, format);
            Dictionary<string, LightValue> convertedValues = new Dictionary<string, LightValue>();
            foreach (KeyValuePair<string, LightValue> original in resultDictionary)
            {
                LightValue converted = original.Value * conversionFactor;
                convertedValues.Add(original.Key, converted);
            }

            #endregion

            return convertedValues;
        }

        /// <summary>
        /// Returns the conversion ratio to convert results expressed on a per meter basis to values express per energy, distance, passenger or payload basis
        /// This method does not do the MJ, mi or 100 km conversion. It will return results in J, m and meter. Another step is necessary to obtain the desired functional unit
        /// </summary>
        /// <param name="format"><para>0: Per MJ</para>
        /// <para>1: Per 100km</para>
        /// <para>2: Per mile</para>
        /// <para>3: Per ton*mile</para>
        /// <para>4: Per tonne*km</para>
        /// <para>5: Per passenger mile</para>
        /// <para>6: Per passenger kilometer</para></param>
        /// <returns></returns>
        public static LightValue ResultsDictionaryWTWConversionFactor(Vehicle vehicle, int format)
        {
            if (format == 0)
            {//We want the results to be converted per energy
                LightValue totalModeEnergy = vehicle.VehicleOperationEnergy.TotalEnergy();
                LightValue totalModeEnergyPerDistance = new LightValue(totalModeEnergy.Value, DimensionUtils.Minus(totalModeEnergy.Dim, vehicle.VehicleOperationEnergy.BottomDim));
                return 1/totalModeEnergyPerDistance;
            }
            if (format == 1 || format == 2)
            {//results kept with a functional unit of distance
                return new LightValue(1, (uint)0);
            }
            if (format == 3 || format == 4)
            {//change functional unit to energy mass by dividing the results by the payload 
                LightValue payload = vehicle.Payload.CurrentValue.ToLightValue();
                return 1/payload;
            }
            if (format == 5 || format == 6)
            {//change functional unit to energy mass by dividing the results by the payload 
                LightValue passengers = vehicle.Passengers.CurrentValue.ToLightValue();
                return 1/ passengers;
            }
            return null;
        }

        /// <summary>
        /// Changes the text of a specified label
        /// </summary>
        /// <param name="vehicleResultValue"></param>
        /// <param name="format"><para>0: Per MJ</para>
        /// <para>1: Per 100km</para>
        /// <para>2: Per mile</para>
        /// <para>3: Per ton*mile</para>
        /// <para>4: Per tonne*km</para>
        /// <para>5: Per passenger mile</para>
        /// <para>6: Per passenger kilometer</para></param>
        /// <param name="greetModelNumericFormat"></param>
        /// <param name="greetModelAutoScaling"></param>
        /// <param name="greetModelScientificFigures"></param>
        /// <returns></returns>
        public static string FormatValue(LightValue vehicleResultValue, int format, int greetModelNumericFormat, bool greetModelAutoScaling, int greetModelScientificFigures)
        {
            string returnedValue = "";
            string autoSelectedUnit; double slope;
            if (vehicleResultValue.Dim == DimensionUtils.RATIO)//Joules/Joules -> Energy per MJ
            {
                AQuantity energyQty = Units.QuantityList.ByDim(DimensionUtils.ENERGY);
                string preferedEnergyExpression = energyQty.Units[energyQty.PreferedUnitIdx].Expression;

                if (format == 0)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedEnergyExpression + "/MJ") + " " + autoSelectedUnit;
                }
            }
            else if (vehicleResultValue.Dim == DimensionUtils.Minus(DimensionUtils.ENERGY, DimensionUtils.LENGTH))//Joules/Meters -> Energy per Mile or Meter
            {
                AQuantity energyQty = Units.QuantityList.ByDim(DimensionUtils.ENERGY);
                string preferedEnergyExpression = energyQty.Units[energyQty.PreferedUnitIdx].Expression;

                if (format == 1)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedEnergyExpression + "/hkm") + " " + autoSelectedUnit;
                }
                if (format == 2)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedEnergyExpression + "/mi") + " " + autoSelectedUnit;
                }
                if (format == 5)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedEnergyExpression + "/mi") + " " + autoSelectedUnit;
                    returnedValue.Replace("/mi", "/(passenger mi)");
                }
                if (format == 6)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedEnergyExpression + "/km") + " " + autoSelectedUnit;
                    returnedValue.Replace("/km", "/(passenger km)");
                }
            }

            else if (vehicleResultValue.Dim == DimensionUtils.Minus(DimensionUtils.ENERGY, DimensionUtils.Plus(DimensionUtils.MASS, DimensionUtils.LENGTH)))//Joules/(kg*meters) -> Energy per ton mile
            {
                AQuantity energyQty = Units.QuantityList.ByDim(DimensionUtils.ENERGY);
                string preferedEnergyExpression = energyQty.Units[energyQty.PreferedUnitIdx].Expression;

                if (format == 3)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                 , greetModelNumericFormat
                 , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                 , vehicleResultValue.Dim, preferedEnergyExpression + "/(ton mi)") + " " + autoSelectedUnit;
                }
                else if (format == 4)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                 , greetModelNumericFormat
                 , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                 , vehicleResultValue.Dim, preferedEnergyExpression + "/(t km)") + " " + autoSelectedUnit;
                }
            }
            else if (vehicleResultValue.Dim == DimensionUtils.Minus(DimensionUtils.MASS, DimensionUtils.ENERGY))//Kilograms/Joule -> Mass per MJ
            {
                AQuantity massQty = Units.QuantityList.ByDim(DimensionUtils.MASS);
                string preferedMassExpression = massQty.Units[massQty.PreferedUnitIdx].Expression;

                if (format == 0)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedMassExpression + "/MJ") + " " + autoSelectedUnit;
                }
            }
            else if (vehicleResultValue.Dim == DimensionUtils.Minus(DimensionUtils.MASS, DimensionUtils.LENGTH))//Kilograms/Meters -> Mass per Mile or Meter
            {
                AQuantity massQty = Units.QuantityList.ByDim(DimensionUtils.MASS);
                string preferedMassExpression = massQty.Units[massQty.PreferedUnitIdx].Expression;

                if (format == 1)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedMassExpression + "/hkm") + " " + autoSelectedUnit;
                }
                if (format == 2)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedMassExpression + "/mi") + " " + autoSelectedUnit;
                }
                if (format == 5)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedMassExpression + "/mi") + " " + autoSelectedUnit;
                    returnedValue.Replace("/mi", "/(passenger mi)");
                }
                if (format == 6)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedMassExpression + "/km") + " " + autoSelectedUnit;
                    returnedValue.Replace("/km", "/(passenger km)");
                }
            }
            else if (vehicleResultValue.Dim == DimensionUtils.Minus(0, DimensionUtils.LENGTH))//Kilograms/(kg*meters) -> Mass per ton mile
            {
                AQuantity massQty = Units.QuantityList.ByDim(DimensionUtils.MASS);
                string preferedMassExpression = massQty.Units[massQty.PreferedUnitIdx].Expression;

                if (format == 3)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                 , greetModelNumericFormat
                 , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                 , vehicleResultValue.Dim, preferedMassExpression + "/(ton mi)") + " " + autoSelectedUnit;
                }
                else if (format == 4)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                 , greetModelNumericFormat
                 , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                 , vehicleResultValue.Dim, preferedMassExpression + "/(t km)") + " " + autoSelectedUnit;
                }
            }
            else if (vehicleResultValue.Dim == DimensionUtils.Minus(DimensionUtils.VOLUME, DimensionUtils.ENERGY))//m^3/Joule -> Volume per MJ
            {
                AQuantity volumeQty = Units.QuantityList.ByDim(DimensionUtils.VOLUME);
                string preferedVolumeExpression = volumeQty.Units[volumeQty.PreferedUnitIdx].Expression;

                if (format == 0)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedVolumeExpression + "/MJ") + " " + autoSelectedUnit;
                }
            }
            else if (vehicleResultValue.Dim == DimensionUtils.Minus(DimensionUtils.VOLUME, DimensionUtils.LENGTH))//m^3/Meters -> Volume per Mile or Meter
            {
                AQuantity volumeQty = Units.QuantityList.ByDim(DimensionUtils.VOLUME);
                string preferedVolumeExpression = volumeQty.Units[volumeQty.PreferedUnitIdx].Expression;

                if (format == 1)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedVolumeExpression + "/hkm") + " " + autoSelectedUnit;
                }
                if (format == 2)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedVolumeExpression + "/mi") + " " + autoSelectedUnit;
                }
                if (format == 5)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedVolumeExpression + "/mi") + " " + autoSelectedUnit;
                    returnedValue.Replace("/mi", "/(passenger mi)");
                }
                if (format == 6)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedVolumeExpression + "/km") + " " + autoSelectedUnit;
                    returnedValue.Replace("/km", "/(passenger km)");
                }
            }
            else if (vehicleResultValue.Dim == DimensionUtils.Minus(DimensionUtils.VOLUME, DimensionUtils.Plus(DimensionUtils.MASS, DimensionUtils.LENGTH)))//m^3/(kg*meters) => Energy per Ton mile
            {
                AQuantity volumeQty = Units.QuantityList.ByDim(DimensionUtils.VOLUME);
                string preferedVolumeExpression = volumeQty.Units[volumeQty.PreferedUnitIdx].Expression;

                if (format == 3)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedVolumeExpression + "/(ton mi)") + " " + autoSelectedUnit;
                }
                else if (format == 4)
                {
                    returnedValue = GuiUtils.FormatSIValue(vehicleResultValue.Value
                , greetModelNumericFormat
                , out autoSelectedUnit, out slope, greetModelAutoScaling, greetModelScientificFigures
                , vehicleResultValue.Dim, preferedVolumeExpression + "/(t km)") + " " + autoSelectedUnit;
                }
            }

            return returnedValue;
        }

        #endregion
    }
}
