using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace readILCDs
{
    public class UnitProcessFile
    {
        public string Name { get; set; }
        public string FileName { get; set; }
    }

    #region dataset class--The following classes are for testing loading data of the US_LCI database.--QZ
    [XmlRoot("dataset", Namespace="http://www.EcoInvent.org/EcoSpold01"), Serializable]
    public class UnitProcess
    {
        [XmlAttribute("validCompanyCodes")]
        public string validCompanyCodes { get; set; }
        [XmlAttribute("validRegionalCodes")]
        public string validRegionalCodes { get; set; }
        [XmlAttribute("validCategories")]
        public string validCategories { get; set; }
        [XmlAttribute("validUnits")]
        public string validUnits { get; set; }
        [XmlAttribute("number")]
        public int number { get; set; }
        [XmlAttribute("generator")]
        public string generator { get; set; }
        [XmlAttribute("timestamp")]
        public string timestamp { get; set; }
        [XmlAttribute("internalSchemaVersion")]
        public string internalSchemaVersion { get; set; }

        [XmlElement("metaInformation")]
        public metaInformation metaInformation { get; set; }
        
        [XmlElement("flowData")]
        public flowData flowData { get; set; }
    }

    public class metaInformation
    {
        [XmlElement("processInformation")]
        public processInformation processInformation { get; set; }
        [XmlElement("modellingAndValidation")]
        public modellingAndValidation modellingAndValidation { get; set; }
        [XmlElement("administrativeInformation")]
        public administrativeInformation administrativeInformation { get; set; }
    }

    public class processInformation
    {
        [XmlElement("referenceFunction")]
        public referenceFunction referenceFunction { get; set; }
        [XmlElement("geography")]
        public geography geography { get; set; }
        [XmlElement("technology")]
        public technology technology { get; set; }
        [XmlElement("timePeriod")]
        public timePeriod timePeriod { get; set; }
        [XmlElement("dataSetInformation")]
        public dataSetInformation dataSetInformation { get; set; }
    }

    public class referenceFunction
    {
        [XmlAttribute("datasetRelatesToProduct")]
        public bool datasetRelatesToProduct { get; set; }
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("localName")]
        public string localName { get; set; }
        [XmlAttribute("infrastructureProcess")]
        public bool infrastructureProcess { get; set; }
        [XmlAttribute("amount")]
        public double amount { get; set; }
        [XmlAttribute("unit")]
        public string unit { get; set; }
        [XmlAttribute("category")]
        public string category { get; set; }
        [XmlAttribute("subCategory")]
        public string subCategory { get; set; }
        [XmlAttribute("localCategory")]
        public string localCategory { get; set; }
        [XmlAttribute("localSubCategory")]
        public string localSubCategory { get; set; }
        [XmlAttribute("includedProcesses")]
        public string includedProcesses { get; set; }
        [XmlAttribute("generalComment")]
        public string generalComment { get; set; }
        [XmlAttribute("infrastructureIncluded")]
        public string infrastructureIncluded { get; set; }
    }

    public class geography
    {
        [XmlAttribute("location")]
        public string location { get; set; }
        [XmlAttribute("text")]
        public string text { get; set; }
    }

    public class technology
    {
        [XmlAttribute("text")]
        public string text { get; set; }
    }

    public class timePeriod
    {
        [XmlAttribute("dataValidForEntirePeriod")]
        public bool dataValidForEntirePeriod { get; set; }
        [XmlElement("startYear")]
        public int startYear { get; set; }
        [XmlElement("endYear")]
        public int endYear { get; set; }
    }

    public class dataSetInformation
    {
        [XmlAttribute("timestamp")]
        public string timestamp { get; set; }
        [XmlAttribute("type")]
        public int type { get; set; }
        [XmlAttribute("impactAssessmentResult")]
        public string impactAssessmentResult { get; set; }
        [XmlAttribute("version")]
        public string version { get; set; }
        [XmlAttribute("internalVersion")]
        public string internalVersion { get; set; }
        [XmlAttribute("energyValues")]
        public int energyValues { get; set; }
        [XmlAttribute("languageCode")]
        public string languageCode { get; set; }
        [XmlAttribute("localLanguageCode")]
        public string localLanguageCode { get; set; }
    }

    public class modellingAndValidation
    {
        [XmlElement("representativeness")]
        public representativeness representativeness { get; set; }
        [XmlElement("source")]
        public source[] source { get; set; }
    }

    public class representativeness
    {
        [XmlAttribute("percent")]
        public double percent { get; set; }
        [XmlAttribute("productionVolume")]
        public string productionVolume { get; set; }//Because sometimes the value is given in the form of "approximately 25%", this attribute cannot be number types.
        [XmlAttribute("samplingProcedure")]
        public string samplingProcedure { get; set; }
    }

    public class source
    {
        [XmlAttribute("number")]
        public int number { get; set; }
        [XmlAttribute("sourceType")]
        public int sourceType { get; set; }
        [XmlAttribute("firstAuthor")]
        public string firstAuthor { get; set; }
        [XmlAttribute("year")]
        public int year { get; set; }
        [XmlAttribute("title")]
        public string title { get; set; }
        [XmlAttribute("placeOfPublications")]
        public string placeOfPublications { get; set; }
        [XmlAttribute("publisher")]
        public string publisher { get; set; }
        [XmlAttribute("volumeNo")]
        public int volumeNo { get; set; }
    }

    /**
      * Staff or entity, that documented the generated data set, entering the
      * information into the database; plus administrative information linked to the
      * data entry activity.
      * 
      * @Element dataEntryBy
      */
    public class administrativeInformation
    {
        [XmlElement("dataEntryBy")]
        public dataEntryBy dataEntryBy { get; set; }
        [XmlElement("dataGeneratorAndPublication")]
        public dataGeneratorAndPublication dataGeneratorAndPublication { get; set; }
        [XmlElement("person")]
        public person person { get; set; }
    }

    public class dataEntryBy
    {
        [XmlAttribute("person")]
        public int person { get; set; }
        [XmlAttribute("qualityNetwork")]
        public int qualityNetwork { get; set; }
    }

    public class dataGeneratorAndPublication
    {
        [XmlAttribute("person")]
        public int person { get; set; }
        [XmlAttribute("dataPublishedIn")]
        public int dataPublishedIn { get; set; }
        [XmlAttribute("referenceToPublishedSource")]
        public int referenceToPublishedSource { get; set; }
        [XmlAttribute("copyright")]
        public bool copyright { get; set; }
        [XmlAttribute("accessRestrictedTo")]
        public string accessRestrictedTo { get; set; }
    }

    /**
     * the responsible person or entity that has
	 * documented this data set, i.e. entered the data and the descriptive
	 * information.
     */ 
    public class person
    {
        [XmlAttribute("number")]
        public int number { get; set; }
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("address")]
        public string address { get; set; }
        [XmlAttribute("telephone")]
        public string telephone { get; set; }
        [XmlAttribute("email")]
        public string email { get; set; }
        [XmlAttribute("companyCode")]
        public string companyCode { get; set; }
        [XmlAttribute("countryCode")]
        public string countryCode { get; set; }
    }

    public class flowData
    {
        [XmlElement("exchange")]
        public exchange[] exchange { get; set; }
    }

    public class exchange
    {
        [XmlAttribute("number")]
        public int number { get; set; }
        [XmlAttribute("infrastructureProcess")]
        public bool infrastructureProcess { get; set; }
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("location")]
        public string location { get; set; }
        [XmlAttribute("category")]
        public string category { get; set; }
        [XmlAttribute("subCategory")]
        public string subCategory { get; set; }
        [XmlAttribute("unit")]
        public string unit { get; set; }
        [XmlAttribute("meanValue")]
        public double meanValue { get; set; }
        [XmlAttribute("uncertaintyType")]
        public int uncertaintyType { get; set; }
        [XmlAttribute("generalComment")]
        public string generalComment { get; set; }

        [XmlElement("inputGroup")]
        public string inputGroup { get; set; }

        [XmlElement("outputGroup")]
        public string outputGroup { get; set; }
    }

    #endregion
}
