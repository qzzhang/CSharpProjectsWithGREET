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
    public static class ExtendedList
    {
        #region methods

        public static List<T> Clone<T>(this IEnumerable<T> list)
        {
            List<T> CloneList = new List<T>();

            foreach (T item in list)
            {
                CloneList.Add(item);
            }

            return CloneList;
        }

        public static int findObjectAndReturnIndex<T>(this IEnumerable<T> list, Object toFind)
        {
            int i = 0;
            foreach (Object item in list)
            {
                if (item == toFind)
                {
                    return i;
                }
                else
                {
                    i++;
                }
            }

            return -1;
        }

        public static void AddRangeWithoutDuplicates<T>(this IList<T> list, IEnumerable<T> objects)
        {
            foreach (T obj in objects)
            {
                if (list.Contains(obj) == false)
                    list.Add(obj);
            }


        }

        #endregion methods
    }
}
