using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// Time series emissions data per unit of distance for a certain mode.
    /// </summary>
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDVehicleOperatingModeEmissionsTSData : V3OLDCarEmissionsTimeSeries
    {
        int V3OLDVehicle_id;
        string mode_name;

        //sometimes for a blend of fuels like for V3OLDVehicles which are using a blend of gasoline and reformulated gasoline
        //we store here the list of the fuel used and their share to establish the carbon balance and caclulate the CO2 facor and SOx factor
        public List<KeyValuePair<InputResourceReference, Parameter>> fuelsBurned = new List<KeyValuePair<InputResourceReference, Parameter>>();

        #region constructors

        public V3OLDVehicleOperatingModeEmissionsTSData(GData data, XmlNode xmlNode, int V3OLDVehicleId, string modeName, string optionalParamPrefix)
        {
            this.V3OLDVehicle_id = V3OLDVehicleId;
            this.mode_name = modeName;

            string status = "";
            try
            {
                status = "attributing fuel id";
                List<KeyValuePair<InputResourceReference, Parameter>> fuels = new List<KeyValuePair<InputResourceReference, Parameter>>();
                this.fuelsBurned = fuels;

                status = "creating new year dictionary";
                foreach (XmlNode year in xmlNode.SelectNodes("year"))
                {
                    try
                    {
                        V3OLDCarYearEmissionsFactors yearD = new V3OLDCarYearEmissionsFactors(data, year, optionalParamPrefix);
                        this.Add(yearD.Year, yearD);
                        if (year.Attributes["notes"] != null)
                            yearD.notes = year.Attributes["notes"].Value;
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Error 6:" + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                LogFile.Write("Error 5:" + xmlNode.OwnerDocument.BaseURI + "\r\n" + xmlNode.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
                throw e;
            }
        }

        protected V3OLDVehicleOperatingModeEmissionsTSData(SerializationInfo information, StreamingContext context) :
            base(information, context)
        {

        }

        public V3OLDVehicleOperatingModeEmissionsTSData(int V3OLDVehicleId, string modeName)
        {
            this.V3OLDVehicle_id = V3OLDVehicleId;
            this.mode_name = modeName;
        }
        #endregion
    }
}
