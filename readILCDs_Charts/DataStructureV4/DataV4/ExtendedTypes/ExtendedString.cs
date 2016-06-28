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

namespace Greet.DataStructureV4
{
    public static class ExtendedString
    {
        #region methods


        public static String LineBreakLongStrings(this String to_be_cutted)
        {
            if (String.IsNullOrEmpty(to_be_cutted))
                return "";
            else
                return LineBreakLongStrings(to_be_cutted, 16);
        }

        private static String LineBreakLongStrings(this String to_be_cutted, int char_count_per_line)
        {
            String breaked = "";

            String[] splitted = to_be_cutted.Split(' ');

            int char_counter = 0;
            foreach (String str in splitted)
            {
                char_counter += (str.Length + 1);

                if (char_counter > char_count_per_line)
                {
                    breaked += "\r\n";
                    char_counter = (str.Length + 1);
                }
                breaked += str + " ";
            }

            return breaked;
        }




        #endregion methods
    }
}
