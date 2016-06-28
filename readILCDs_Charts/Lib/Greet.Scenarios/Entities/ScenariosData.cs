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
using Greet.Lib.Scenarios.Entities;

namespace Greet.Lib.Scenarios
{
    /// <summary>
    /// A class to store a list of scenarios and results
    /// </summary>
    public class ScenariosData
    {
        #region Fields and Constants

        public List<RecordedEntityResults> Results = new List<RecordedEntityResults>();
        public List<Scenario> Scenarios = new List<Scenario>();
        public List<ShapeFile> Shapefiles = new List<ShapeFile>();

        #endregion

        #region Members

        /// <summary>
        /// Returns a new instance of Scenario data which has been cloned from the current one.
        /// DOES NOT clone the results
        /// </summary>
        /// <returns>Cloned scenario</returns>
        public ScenariosData Clone()
        {
            ScenariosData clone = new ScenariosData();
            foreach (ShapeFile shp in Shapefiles)
                clone.Shapefiles.Add(shp.Clone());
            foreach (Scenario scn in Scenarios)
                clone.Scenarios.Add(scn.Clone());
            return clone;
        }

        #endregion
    }
}
