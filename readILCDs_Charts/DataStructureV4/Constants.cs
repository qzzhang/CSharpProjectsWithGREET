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
using System.Collections.Generic;
using System.Globalization;

namespace Greet.DataStructureV4
{
    public static class Constants
    {
        public static string EmptyPicture = "empty.png";

        public static Dictionary<int, string> sourceNames = new Dictionary<int, string>()
        {
            { 1, "Primary Resource"},
            { 2, "Pathway Mix"},
            { 3, "Output of a previous process"},
            { 5, "Single Pathway"}
        };

        public static CultureInfo USCI = new CultureInfo("en-US");
    }
}
