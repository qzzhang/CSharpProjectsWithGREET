using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Greet.Plugins.EcoSpold01.Entities
{
    public class UnitProcessFile
    {
        public string Name { get; set; }
        public string FileName { get; set; }
    }

    #region UnitProcess class--The following classes are for testing loading Crude Oil Production data of US_LCI database.--QZ
    /*
    [XmlRoot("ecoSpold"), Serializable]
    public class ecoSpold
    {
        [XmlElement("dataset")]
        public dataset dataset { get; set; }
    }
    */

    [XmlRoot("dataset"), Serializable]
    public class dataset
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
        public metaInformation metaInfo { get; set; }
        /*
        [XmlElement("flowData")]
        public flowData flowData { get; set; }
         * */
    }

    public class metaInformation//done
    {
        [XmlElement("processInformation")]
        public processInformation processInformation { get; set; }
        [XmlElement("modellingAndValidation")]
        public modellingAndValidation modellingAndValidation { get; set; }
        [XmlElement("administrativeInformation")]
        public administrativeInformation administrativeInformation { get; set; }
    }

    public class processInformation//done
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

    public class referenceFunction//done
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

    public class geography//done
    {
        [XmlAttribute("location")]
        public string location { get; set; }
        [XmlAttribute("text")]
        public string text { get; set; }
    }

    public class technology//done
    {
        [XmlAttribute("text")]
        public string text { get; set; }
    }

    public class timePeriod//done
    {
        [XmlAttribute("dataValidForEntirePeriod")]
        public bool dataValidForEntirePeriod { get; set; }
        [XmlElement("startYear")]
        public startYear startYear { get; set; }
        [XmlElement("endYear")]
        public endYear endYear { get; set; }
    }

    public class startYear//done
    {
        [XmlText]
        public int theStartYear { get; set; }
    }

    public class endYear//done
    {
        [XmlText]
        public int theEndYear { get; set; }
    }

    public class dataSetInformation//done
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

    public class modellingAndValidation//check for multiple "source" entries before done
    {
        [XmlElement("representativeness")]
        public representativeness representativeness { get; set; }
        [XmlElement("source")]
        public source source { get; set; }
    }

    public class representativeness//done
    {
        [XmlAttribute("percent")]
        public double percent { get; set; }
        [XmlAttribute("productionVolume")]
        public int productionVolume { get; set; }
        [XmlAttribute("samplingProcedure")]
        public string samplingProcedure { get; set; }
    }

    public class source//done
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

    public class administrativeInformation//done
    {
        [XmlElement("dataEntryBy")]
        public dataEntryBy dataEntryBy { get; set; }
        [XmlElement("dataGeneratorAndPublication")]
        public dataGeneratorAndPublication dataGeneratorAndPublication { get; set; }
        [XmlElement("person")]
        public person person { get; set; }
    }

    public class dataEntryBy//done
    {
        [XmlAttribute("person")]
        public int person { get; set; }
        [XmlAttribute("qualityNetwork")]
        public int qualityNetwork { get; set; }
    }

    public class dataGeneratorAndPublication//done
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

    public class person//done
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
/*
    public class flowData
    {
        [XmlAttribute("City")]
        public string City { get; set; }
        [XmlAttribute("Pin")]
        public string Pin { get; set; }
        [XmlText]
        public string AddressValue { get; set; }
    }
*/
    #endregion

    #region Testing class--The following classes are for testing purpose only and will be modified as the ILCD data format goes.--QZ
    [XmlRoot("Datatable"), Serializable]
    public class Datatable
    {
        [XmlElement("Employees")]
        public Employees Employees { get; set; }
    }

    public class Employees
    {
        [XmlAttribute("Count")]
        public int Count { get; set; }
        [XmlElement("Employee")]
        public List<Employee> Employee { get; set; }
    }

    public class Employee
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlElement("Address")]
        public Address Address { get; set; }
        [XmlElement("Contact")]
        public Contact Contact { get; set; }
    }

    public class Address
    {
        [XmlAttribute("City")]
        public string City { get; set; }
        [XmlAttribute("Pin")]
        public string Pin { get; set; }
        [XmlText]
        public string AddressValue { get; set; }
    }

    public class Contact
    {
        [XmlAttribute("Mob")]
        public string Mob { get; set; }
        [XmlAttribute("Phone")]
        public string Phone { get; set; }
        [XmlAttribute("Email")]
        public string Email { get; set; }
        [XmlText]
        public string ContactValue { get; set; }
    }
    #endregion

}
