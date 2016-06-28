using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities
{
    //this class stores the attributes of the coproduct and contains constructors and attributes. The actual calculation function is a member of AProcess (CalculateCoProductsCredit). 
    [Serializable]
    public class CoProduct : AOutput
    {
        #region attributes

        public CoProductsElements.TreatmentMethod method;
        private List<ConventionalProducts> conventionalDisplacedResourcesList = new List<ConventionalProducts>();

        #endregion attributes

        #region Accessors
        public List<ConventionalProducts> ConventionalDisplacedResourcesList
        {
            get { return conventionalDisplacedResourcesList; }
            set { conventionalDisplacedResourcesList = value; }
        }

        #endregion

        #region constructors
        public CoProduct(GData data, XmlNode node, string optionalParamPrefix)
            : base(data, node, optionalParamPrefix)
        {
            resourceId = Convert.ToInt32(node.Attributes["ref"].Value);
            method = (CoProductsElements.TreatmentMethod)Enum.Parse(typeof(CoProductsElements.TreatmentMethod), node.Attributes["method"].Value);

            XmlNode con_pr = node.SelectSingleNode("conventional_products");
            if (con_pr != null)
            {
                int count = 0;
                foreach (XmlNode pr_node in con_pr.SelectNodes("product"))
                {
                    Parameter temp = data.ParametersData.CreateRegisteredParameter(pr_node.Attributes["ratio"], optionalParamPrefix +"_cp_" + count + "_cop_" + resourceId + "_ratio");
                    if (pr_node.Attributes["mix"] != null)
                        conventionalDisplacedResourcesList.Add(
                            new ConventionalProducts(data,
                                new InputResourceReference(pr_node.Attributes["ref"], pr_node.Attributes["mix"], Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix), temp));
                    else if (pr_node.Attributes["pathway"] != null)
                        conventionalDisplacedResourcesList.Add(
                            new ConventionalProducts(data,
                                new InputResourceReference(pr_node.Attributes["ref"], pr_node.Attributes["pathway"], Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway), temp));
                    count++;
                }
            }
        }

        /// <summary>
        /// Creates a new CoProduct and register the new parameter to the given database.
        /// The default treatement method is Displacement
        /// </summary>
        /// <param name="data">Database to register parameters</param>
        /// <param name="resourceId">Resource ID of the CoProduct</param>
        /// <param name="unitGroup">The unitGroup for the default amount which is going to be 0</param>
        public CoProduct(GData data, int resourceId, ParameterTS designAmount)
            : base()
        {
            this.resourceId = resourceId;
            this.method = CoProductsElements.TreatmentMethod.displacement;
            if (designAmount != null)
                this.DesignAmount = designAmount;
            else
                this.DesignAmount = new ParameterTS(data, data.ResourcesData[resourceId].DefaultQuantityExpression(), 0);
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Returns an XML node which represents a coproduct object
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        internal new XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode coprod_node = doc.CreateNode("coproduct", doc.CreateAttr("ref", this.resourceId), doc.CreateAttr("method", this.method));

            XmlNode amount_node = doc.CreateNode("amount");
            foreach (KeyValuePair<int, Parameter> pair in this.DesignAmount)
            {
                XmlNode yearValue = doc.CreateNode("year", doc.CreateAttr("value", pair.Value), doc.CreateAttr("year", pair.Key));
                amount_node.AppendChild(yearValue);
            }
            coprod_node.AppendChild(amount_node);

            XmlNode con_pr_node = doc.CreateNode("conventional_products");
            coprod_node.AppendChild(con_pr_node);


            foreach (ConventionalProducts conventionalProducts in ConventionalDisplacedResourcesList)
            {
                XmlNode pr_node = doc.CreateNode("product");
                conventionalProducts.MaterialKey.ToXmlNode(doc, pr_node);
                pr_node.Attributes.Append(doc.CreateAttr("ratio", conventionalProducts.DispRatio));
                con_pr_node.AppendChild(pr_node);
            }

            coprod_node.Attributes.Append(doc.CreateAttr("id", this.id));

            coprod_node.Attributes.Append(doc.CreateAttr("notes", this.Notes));

            return coprod_node;
        }

        #endregion methods

        public override bool CheckSpecificIntegrity(GData data, bool showIds, bool fixFixableIssues, out string errorMessage)
        {
            StringBuilder problems = new StringBuilder();
            if (this.method == CoProductsElements.TreatmentMethod.displacement)
            {
                foreach (ConventionalProducts cp in conventionalDisplacedResourcesList)
                {
                    if (!data.ResourcesData.ContainsKey(cp.MaterialKey.ResourceId))
                        problems.Append("The co-products displaces a resource that does not exists. " + (showIds ? "The non existing resource ID is " + cp.MaterialKey.ResourceId : ""));
                    else
                    {
                        if (cp.MaterialKey.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Mix
                            && !data.MixesData.ContainsKey(cp.MaterialKey.SourceMixOrPathwayID))
                            problems.Append("The co-products displaces a resource using as an upstream a mix that does not exists. " + (showIds ? "The non existing mix ID is " + cp.MaterialKey.SourceMixOrPathwayID : ""));
                        else if (cp.MaterialKey.SourceType == Greet.DataStructureV4.Interfaces.Enumerators.SourceType.Pathway
                            && !data.PathwaysData.ContainsKey(cp.MaterialKey.SourceMixOrPathwayID))
                            problems.Append("The co-products displaces a resource using as an upstream a pathway that does not exists. " + (showIds ? "The non existing pathway ID is " + cp.MaterialKey.SourceMixOrPathwayID : ""));
                        else if (cp.MaterialKey.SourceType == 0 || cp.MaterialKey.SourceMixOrPathwayID == -1 )
                            problems.Append("The co-products displaces a resource without defining which upstream to use.");
                    }
                }
            }
            else if (this.method == CoProductsElements.TreatmentMethod.allocation)
            {
            
            }

            errorMessage = problems.ToString();
            return true;
        }
        
    }
}
