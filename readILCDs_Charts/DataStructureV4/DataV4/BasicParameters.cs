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
using System.Xml;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4
{
    [Serializable]
    public class BasicParameters
    {
        #region attributes

        /// <summary>
        /// STATIC VARIABLE
        /// Stores the selected year for the simulation
        /// </summary>
        public static Parameter SelectedYear;

        /// <summary>
        /// STATIC VARIABLE
        /// Uses lhv of hhv for the simulations
        /// </summary>
        public static bool UseLhv = true;

        #endregion attributes

        #region constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BasicParameters()
        {
        }

        /// <summary>
        /// Constructor from XML file
        /// </summary>
        /// <param name="data"></param>
        /// <param name="yearNode"></param>
        public BasicParameters(GData data, XmlNode yearNode)
        {
            SelectedYear = data.ParametersData.CreateRegisteredParameter(yearNode.Attributes["year_selected"],"bas_year");
            if (yearNode.Attributes["lhv"] != null)
                UseLhv = Boolean.Parse(yearNode.Attributes["lhv"].Value);
            else
                UseLhv = true;
        }

        #endregion constructors
    }
}
