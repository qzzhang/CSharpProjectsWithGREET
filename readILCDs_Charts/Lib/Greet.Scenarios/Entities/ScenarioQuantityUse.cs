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
namespace Greet.Lib.Scenarios
{
    /// <summary>
    /// This class holds a quantity (amount) of resources/item/product used within a certain scenario
    /// For example this can contain 4GWh of electricity from a certain mix, or 1 ton of product. 
    /// An advanced feature allows it to account for vehicles for example 1 million vehicles that travel 10000 miles per year
    /// </summary>
    class ScenarioQuantityUse
    {
        #region Members

        public ScenarioQuantityUse Clone()
        {
            return new ScenarioQuantityUse();
        }

        #endregion
    }
}
