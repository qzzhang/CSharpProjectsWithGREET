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
using Greet.UnitLib3;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4.ResultsStorage
{
    /// <summary>
    /// This class contains Energy and Emission objects. Was created for convinience since those two come togeather in many places.
    /// </summary>
    [Serializable]
    public class Enem
    {
        #region attributes

        public ResourceAmounts materialsAmounts;
        public EmissionAmounts emissions;

        #endregion attributes

        #region constructors

        public Enem()
        {
            materialsAmounts = new ResourceAmounts();
            emissions = new EmissionAmounts();
        }

        public Enem(ResourceAmounts _en, EmissionAmounts _em)
        {
            this.materialsAmounts = _en;
            this.emissions = _em;
        }

        public Enem(Enem _enem)
            : this()
        {
            foreach (KeyValuePair<int, double> pair in _enem.emissions)
                this.emissions.Add(pair.Key, pair.Value);
            foreach (KeyValuePair<int, LightValue> pair in _enem.materialsAmounts)
                this.materialsAmounts.Add(pair.Key, pair.Value);
            this.BottomDim = _enem.BottomDim;
        }
        public Enem(uint bottomDim)
            : this()
        {
            this.emissions.BottomDim = bottomDim;
            this.materialsAmounts.BottomDim = bottomDim;
        }

        #endregion consrtructors

        #region methods

        public void Clear()
        {
            if (materialsAmounts != null)
                this.materialsAmounts.Clear();
            if (emissions != null)
                this.emissions.Clear();
        }

        #endregion methods

        #region operators

        public static Enem operator *(Enem e1, Parameter e2)
        {
            return new Enem(e1.materialsAmounts * e2, e1.emissions * e2);
        }
        public static Enem operator *(Enem e1, LightValue e2)
        {
            return new Enem(e1.materialsAmounts * e2, e1.emissions * e2);
        }
        public static Enem operator *(Parameter e2, Enem e1)
        {
            return new Enem(e2 * e1.materialsAmounts, e2 * e1.emissions);
        }
        public static Enem operator *(LightValue e2, Enem e1)
        {
            return new Enem(e2 * e1.materialsAmounts, e2 * e1.emissions);
        }
        public static Enem operator *(Enem e1, double e2)
        {
            return new Enem(e1.materialsAmounts * e2, e1.emissions * e2);
        }
        public static Enem operator *(double e2, Enem e1)
        {
            return new Enem(e1.materialsAmounts * e2, e1.emissions * e2);
        }
        public static Enem operator /(Enem e1, Parameter e2)
        {
            return new Enem(e1.materialsAmounts / e2, e1.emissions / e2);
        }
        public static Enem operator /(Enem e1, LightValue e2)
        {
            return new Enem(e1.materialsAmounts / e2, e1.emissions / e2);
        }
        public static Enem operator /(Enem e1, double e2)
        {
            return new Enem(e1.materialsAmounts / e2, e1.emissions / e2);
        }
        public static Enem operator +(Enem e1, Enem e2)
        {
            return new Enem(e1.materialsAmounts + e2.materialsAmounts, e1.emissions + e2.emissions);
        }

        /// <summary>
        /// adds values of a second enem to the current one, replaces += and enhance performance because less new objects are created
        /// </summary>
        /// <param name="e2"></param>
        /// <returns></returns>
        public void Addition(Enem e2)
        {
            this.emissions.Addition(e2.emissions);
            this.materialsAmounts.Addition(e2.materialsAmounts);
        }
        /// <summary>
        /// Adds the values to the current object after mutiplying them by the double p
        /// Saves time comparing to results = results + (values * p) as this single operation do not create any new objects for the results
        /// </summary>
        /// <param name="p"></param>
        /// <param name="values"></param>
        public void MulAdd(double p, Enem values)
        {
            this.emissions.MulAdd(p, values.emissions);
            this.materialsAmounts.MulAdd(p, values.materialsAmounts);
        }
        public static Enem operator -(Enem e1, Enem e2)
        {
            return new Enem(e1.materialsAmounts - e2.materialsAmounts, e1.emissions - e2.emissions);
        }

        #endregion operators

        #region accessors

        public uint BottomDim
        {
            set
            {
                this.emissions.BottomDim = value;
                this.materialsAmounts.BottomDim = value;
            }
            get
            {
                return this.emissions.BottomDim;
            }
        }

        #endregion

    }

   
}
