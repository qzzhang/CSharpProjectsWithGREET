using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// Charge depleting mode, a mode used by PulgIn Hybrids V3OLDVehicles. In this mode we're depleting the energy from an electric battery
    /// using the assistance of the ICE or equivalent in peak power regime.
    /// </summary>
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDCDMode : V3OLDVehicleOperationalMode
    {
        #region attributes

        public V3OLDVehicleFuel electricityUsed;
        private V3OLDCarEmissionsTimeSeries technologieRatiosForAllElectricOperation;
        internal V3OLDConsumptionsTS _consumptions;
        public LightValue calculatedEquivalentMpg;

        #endregion

        #region constructors

        public V3OLDCDMode(int V3OLDVehicle_reference)
            : base(V3OLDVehicle_reference)
        {

        }

        public V3OLDCDMode(GData data, XmlNode xmlNode, int V3OLDVehicle_reference, string optionalParamPrefix)
            : base(data, xmlNode, V3OLDVehicle_reference, optionalParamPrefix)
        {
            try
            {

                this.electricityUsed = new V3OLDVehicleFuel(data, xmlNode.SelectSingleNode("electricity"), optionalParamPrefix +  "_CDMode_elec_");

                this.Consumptions = new V3OLDConsumptionsTS(data, xmlNode, optionalParamPrefix + "_cons");

                this.technologieRatiosForAllElectricOperation = new V3OLDCarEmissionsTimeSeries();
                foreach (XmlNode year_node in xmlNode.SelectNodes("year"))
                {
                    int year = Convert.ToInt32(year_node.Attributes["value"].Value);
                    V3OLDCarYearEmissionsFactors t_e_f = new V3OLDCarYearEmissionsFactors(year);//I dont understand
                    V3OLDCarRealEmissionsFactors r_e_f = new V3OLDCarRealEmissionsFactors(data, year_node.SelectSingleNode("all_electric_emissions"), optionalParamPrefix + "_elec_" + year) ;
                    t_e_f.EmissionsFactors = r_e_f;
                    this.technologieRatiosForAllElectricOperation.Add(year, t_e_f);
                }
            }
            catch (Exception e)
            {
                LogFile.Write("Error while reading CD mode");
                throw e;
            }

        }
        #endregion

        #region methods

        internal override XmlNode ToXmlNode(XmlDocument xmlDoc)
        {

            XmlNode modeNode = xmlDoc.CreateNode("mode", xmlDoc.CreateAttr("mpgge_ref", _mpggeRef), xmlDoc.CreateAttr("mode", _name), xmlDoc.CreateAttr("notes", Notes));


            foreach (V3OLDVehicleFuel pair in this.FuelsUsedWithoutBaseFuels)
            {
                modeNode.AppendChild(pair.ToXmlNode(xmlDoc));
            }

            XmlNode temp = this.electricityUsed.ToXmlNode(xmlDoc, "electricity");
            modeNode.AppendChild(temp);


            foreach (V3OLDCarYearEmissionsFactors year in this._technologies.Values)
            {
                XmlNode yearNode = year.ToXmlNode(xmlDoc);

                if (_mpg != null && this._mpg.Keys.Contains(year.Year))
                    yearNode.AppendChild(xmlDoc.CreateNode("mpg", _mpg[year.Year].ToXmlAttribute(xmlDoc, "mpg"), xmlDoc.CreateAttr("notes", _mpg.notes[year.Year])));
                if (this.technologieRatiosForAllElectricOperation != null)
                {
                    XmlNode all_electric_emissions_node = xmlDoc.CreateNode("all_electric_emissions");
                    this.technologieRatiosForAllElectricOperation[year.Year].EmissionsFactors.ToXmlNode(xmlDoc, ref all_electric_emissions_node);
                    yearNode.AppendChild(all_electric_emissions_node);
                }
                if (this.Consumptions.Keys.Contains(year.Year))
                {
                    List<XmlNode> nodes = this.Consumptions[year.Year].ToXmlNode(xmlDoc);
                    foreach (XmlNode consump in nodes)
                        yearNode.AppendChild(consump);
                }
                modeNode.AppendChild(yearNode);
            }
            return modeNode;
        }


        override public void AddNewYear(GData data, int year, bool baseMpg, bool baseEmission)
        {
            base.AddNewYear(data, year, baseMpg, baseEmission);
            if (this is V3OLDCDMode)
            {
                if (this.Consumptions != null)
                    this.Consumptions.Add(year, new V3OLDConsumptions(Convert.ToInt32(year)));
                else
                    this.Consumptions = new V3OLDConsumptionsTS(Convert.ToInt32(year));

                this.Consumptions[year].tables.Add("fuel_consumption", new V3OLDConsumption(data, "fuel_consumption"));
                this.Consumptions[year].tables.Add("electricity_consumption", new V3OLDConsumption(data, "electricity_consumption"));
                this.Consumptions[year].tables.Add("electric_range", new V3OLDConsumption(data, "electric_range"));

                if (this.TechnologieRatiosAllElectricOperation == null)
                    this.TechnologieRatiosAllElectricOperation = new V3OLDCarEmissionsTimeSeries(); 

                V3OLDCarYearEmissionsFactors t_e_f = new V3OLDCarYearEmissionsFactors(data, year, this.TechnologieRatiosAllElectricOperation, false);
                if (data.Gases.KeyExists(12))
                    t_e_f.EmissionsFactors.Add(12, new V3OLDCarEmissionValue(data, "unitless", 1), "");//HARDCODED CD MODE ONLY NEEDS THESE TWO EMISSIONS
                if (data.Gases.KeyExists(13))
                    t_e_f.EmissionsFactors.Add(13, new V3OLDCarEmissionValue(data, "unitless", 1), "");
                this.TechnologieRatiosAllElectricOperation.Add(year, t_e_f);

            }
        }
       
        #endregion

        #region accessors

        /// <summary>
        /// The energy consumption tables usually used for plugin hybrids
        /// </summary>
        public V3OLDConsumptionsTS Consumptions
        {
            get { return _consumptions; }
            set { _consumptions = value; }
        }

        public LightValue CalculatedEquivalentMpg
        {
            get { return calculatedEquivalentMpg; }
            set { calculatedEquivalentMpg = value; }
        }

        public V3OLDVehicleFuel ElectricityUsed
        {
            get { return electricityUsed; }
            set { electricityUsed = value; }
        }

        public V3OLDCarEmissionsTimeSeries TechnologieRatiosAllElectricOperation
        {
            get { return technologieRatiosForAllElectricOperation; }
            set { technologieRatiosForAllElectricOperation = value; }
        }

        #endregion

        #region IV3OLDCDMode
        public IInputResourceReference ElectricityUsage
        {
            get
            {
                return this.ElectricityUsed.InputResourceRef;
            }
        }
        #endregion
    }
}
