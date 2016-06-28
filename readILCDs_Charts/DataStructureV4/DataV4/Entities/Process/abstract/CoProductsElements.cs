/*********************************************************************** 
COPYRIGHT NOTIFICATION 

Email contact: greet@anl.gov 
Copyright (c) 2012, UChicago Argonne, LLC 
All Rights Reserved

THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 

************************************************************************ 
ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
ENERGY. 
************************************************************************
 
***********************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class holds the allocation method for all the coProducts listed
    /// as the allocationMethod is common to all the allocated coProducts
    /// </summary>
    [Serializable]
    public class CoProductsElements : List<CoProduct>
    {
        #region ennumerators
        /// <summary>
        /// Those are possible options for treating co-products
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum TreatmentMethod { displacement, allocation, unused };
        /// <summary>
        /// Those are possible options for claculating credits when allocartion method is used to treat a coproduct
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum AllocationMethod { Energy, Mass, Market, Volume };
        #endregion

        #region attributes
        public AllocationMethod? commonAllocationMethod;
        #endregion

        #region consturctors
        public CoProductsElements()
        {
        }

        /// <summary>
        /// Create the Coproduct elements from the root node containing all the products
        /// Need to be done last as we do not return the current id reference integer for assigning new ids to other inputs
        /// </summary>
        /// <param name="node">xmlNode of the coproducts object</param>
        /// <param name="process_reference">process reference to assign to the output</param>
        public CoProductsElements(GData data, XmlNode node, int process_reference, string optionalParamPrefix)
        {
            foreach (XmlNode coproduct_node in node.SelectNodes("coproduct"))
            {
                CoProduct coproduct = new CoProduct(data, coproduct_node, optionalParamPrefix);
                
                this.Add(coproduct);
            }
            if (node.Attributes["allocation_method"] != null && node.Attributes["allocation_method"].Value != "")
                this.commonAllocationMethod = (AllocationMethod)Enum.Parse(typeof(AllocationMethod), node.Attributes["allocation_method"].Value, true);
        }
        #endregion

        #region methods

        internal XmlNode toXmlNode(XmlDocument doc)
        {
            XmlNode coproductsNode = doc.CreateNode("coproducts", doc.CreateAttr("allocation_method", commonAllocationMethod));
            foreach (CoProduct coproduct in this)
                coproductsNode.AppendChild(coproduct.ToXmlNode(doc));
            return coproductsNode;
        }
        #endregion


    }
}
