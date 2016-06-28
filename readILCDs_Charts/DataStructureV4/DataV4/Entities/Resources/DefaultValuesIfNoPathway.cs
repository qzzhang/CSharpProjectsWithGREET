using System;
using System.Collections.Generic;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.ResultsStorage;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class DefaultValuesIfNoPathway
    {
        #region Attributes
        Parameter perOutputAmount = null;
        Dictionary<int, Parameter> energies = new Dictionary<int, Parameter>();
        Dictionary<int, Parameter> emissions = new Dictionary<int, Parameter>();

        #endregion

        #region Accessor
        public Dictionary<int, Parameter> Energies
        {
            get { return energies; }
            set { energies = value; }
        }

        public Dictionary<int, Parameter> Emissions
        {
            get { return emissions; }
            set { emissions = value; }
        }

        public Parameter PerOutputAmount
        {
            get { return perOutputAmount; }
            set { perOutputAmount = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new set of fixed values for the mix
        /// </summary>
        /// <param name="unit">Default unit for the perOutputAmount, based on the pysical properties of the parent material</param>
        public DefaultValuesIfNoPathway(GData data, string unit)
        {
            perOutputAmount = data.ParametersData.CreateRegisteredParameter(unit, 0);
        }

        public DefaultValuesIfNoPathway(GData data, XmlNode node)
        {
            perOutputAmount = data.ParametersData.CreateRegisteredParameter(node.Attributes["per_amount_of"]);
            if (node.SelectSingleNode("Resources") != null)
                foreach (XmlNode resNode in node.SelectSingleNode("Resources").ChildNodes)
                {
                    this.energies.Add(Convert.ToInt32(resNode.Attributes["id"].Value), data.ParametersData.CreateRegisteredParameter(resNode.Attributes["Amount"]));
                }

            if (node.SelectSingleNode("Emissions") != null)
                foreach (XmlNode resNode in node.SelectSingleNode("Emissions").ChildNodes)
                {
                    this.emissions.Add(Convert.ToInt32(resNode.Attributes["id"].Value), data.ParametersData.CreateRegisteredParameter(resNode.Attributes["Amount"]));
                }
        }
        #endregion

        #region Methods
        public XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode default_values_node = doc.CreateNode("Fixed_Values", doc.CreateAttr("per_amount_of", this.perOutputAmount));

            XmlNode resNode = doc.CreateNode("Resources");

            foreach (KeyValuePair<int, Parameter> pair in this.energies)
            {
                resNode.AppendChild(doc.CreateNode("Fuel", doc.CreateAttr("id", pair.Key), pair.Value.ToXmlAttribute(doc, "Amount")));
            }

            default_values_node.AppendChild(resNode);

            XmlNode emissionNode = doc.CreateNode("Emissions");

            foreach (KeyValuePair<int, Parameter> pair in this.emissions)
            {
                emissionNode.AppendChild(doc.CreateNode("Gas", doc.CreateAttr("id", pair.Key), pair.Value.ToXmlAttribute(doc, "Amount")));
            }
            default_values_node.AppendChild(emissionNode);

            return default_values_node;
        }


        public Results GetCurrentBalance()
        {
            Results total_ee = new Results();

            if (this.perOutputAmount.Dim == DimensionUtils.MASS 
                || this.perOutputAmount.Dim == DimensionUtils.ENERGY 
                || this.perOutputAmount.Dim == DimensionUtils.VOLUME)//hard_unit
            {
                double default_amount = this.perOutputAmount.ValueInDefaultUnit;

                total_ee.wellToProductEnem.emissions.Addition(this.emissions);
                total_ee.wellToProductEnem.materialsAmounts.Addition(this.energies);

                total_ee.wellToProductEnem = total_ee.wellToProductEnem / default_amount; //to get a balance per unit of product
                total_ee.wellToProductEnem.BottomDim = this.perOutputAmount.Dim;
            }
            else
                throw new Exception("the per unit of.. should be in one of our basic units, grams, liters or joules");

            return total_ee;
        }
        #endregion
    }
}
