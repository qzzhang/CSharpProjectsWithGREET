using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities.Legacy
{
    /// <summary>
    /// This class holds the emission factors for a technology or a V3OLDVehicle mode
    /// The parents of this class would usually be : A time series object and a technology or V3OLDVehicle operation node object
    /// </summary>
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDCarYearEmissionsFactors : IYearEmissionFactors
    {
        #region attributes

        private int _year;
        private V3OLDCarEmissionsFactors gases;
        public string notes = "";

        #endregion attributes

        #region constructors

        public V3OLDCarYearEmissionsFactors(GData data, int year, V3OLDCarEmissionsTimeSeries TS_reference, bool create_emission_factors, string optionalParamPrefix = "") //if user adds new technology within GUI
        {
            this._year = year;
            this.gases = new V3OLDCarRealEmissionsFactors(data, 0, create_emission_factors, false, false, optionalParamPrefix + _year + "_");

        }

        public V3OLDCarYearEmissionsFactors(GData data, int year, V3OLDCarEmissionsTimeSeries TS_reference, bool create_emission_factors, bool isBaseV3OLDVehicle, string optionalParamPrefix = "") //if user adds new technology within GUI
        {
            this._year = year;
            if (isBaseV3OLDVehicle)
                this.gases = new V3OLDCarRealEmissionsFactors(data, 0, create_emission_factors, true, isBaseV3OLDVehicle, optionalParamPrefix + _year + "_");
            else
                this.gases = new V3OLDCarRealEmissionsFactors(data, 1, create_emission_factors, true, isBaseV3OLDVehicle, optionalParamPrefix + _year + "_");//default ratio 1 for V3OLDVehicles
             
        }


        /// <summary>
        /// A copy constructor for returning an object with the same values as the paramter object
        /// </summary>
        /// <param name="factorsToCopy"></param>
        public V3OLDCarYearEmissionsFactors(GData data, V3OLDCarYearEmissionsFactors factorsToCopy)
        {
            _year = factorsToCopy._year;
            gases = new V3OLDCarRealEmissionsFactors();
            notes = factorsToCopy.notes;
            foreach (KeyValuePair<int, V3OLDCarEmissionValue> pair in factorsToCopy.gases)
            {
                V3OLDCarEmissionValue cev = new V3OLDCarEmissionValue(data, pair.Value.EmParameter.UnitGroupName, pair.Value.EmParameter.ValueInDefaultUnit, pair.Value.CanCalculated);
                cev.useCalculated = pair.Value.useCalculated;
                cev.EmParameter.UserValue = pair.Value.EmParameter.UserValue;
                cev.EmParameter.UseOriginal = pair.Value.EmParameter.UseOriginal;

                gases.Add(pair.Key, cev, "");
            }
        }

        internal V3OLDCarYearEmissionsFactors(GData data, XmlNode yearNode, string optionalParamPrefix)
        {
            string status = "";

            try
            {
                status = "reading value";
                if (yearNode.Attributes["value"] != null)
                    this._year = Convert.ToInt32(yearNode.Attributes["value"].Value);
                else
                    this._year = 0;

                if (yearNode.SelectSingleNode("base") == null)
                    this.gases = new V3OLDCarRealEmissionsFactors(data, yearNode, optionalParamPrefix + "_real_" + this._year);
                else
                    this.gases = new V3OLDCarBasedEmissionFactors(data, yearNode, optionalParamPrefix + "_real_" + this._year);
            }
            catch (Exception e)
            {
                LogFile.Write("Error 4:" + yearNode.OwnerDocument.BaseURI + "\r\n" + yearNode.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
            }
        }

        public V3OLDCarYearEmissionsFactors(int year)
        {
            this._year = year;
        }

        #endregion constructors

        #region accessors
        /// <summary>
        /// The year of the emission factors, default value is 0.
        /// This property is set to public. It is used to identify the year to which the current emission facts belong to.
        /// </summary>
        [Browsable(false)]
        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }

        [Browsable(true),
        DisplayName("gases"),
        Description("The gases emissions ordered by years")]
        public V3OLDCarEmissionsFactors EmissionsFactors
        {
            get { return gases; }
            set { gases = value; }
        }

        #endregion accessors

        #region methods

     
        internal XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode yearNode = doc.CreateNode("year", doc.CreateAttr("value", _year), doc.CreateAttr("notes", this.notes));
            this.gases.ToXmlNode(doc, ref yearNode);
            return yearNode;
        }

       

        #endregion methods

        #region IYearEmissionFactors

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public IEmissionsFactors EmissionsFactorss
        {
            get
            {
                return this.EmissionsFactors as IEmissionsFactors;
            }
        }

        #endregion
    }
}
