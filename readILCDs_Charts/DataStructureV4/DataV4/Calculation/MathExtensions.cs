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

namespace Greet.DataStructureV4
{
    public static class MathExtensions
    {

        /// <summary>
        /// Compute a polynom equation
        /// The factors are ordered from the smallest power to the highest
        /// return = a + bx + cx^2 + dx^3... List = a,b,c,d
        /// </summary>
        /// <param name="factors"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static double PloyVal(List<double> factors, double variable)
        {
            double returned_value = 0;
            for (int i = 0; i <= factors.Count - 1; i++)
                returned_value += Math.Pow(variable, i) * factors[i];

            return returned_value;
        }
    }
}
