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
using System.Linq;
using System.Runtime.Serialization;
using Greet.UnitLib2;
using Greet.DataStructureV3.Entities;

namespace Greet.DataStructureV3.ResultsStorage
{
   
    /// <summary>
    /// Same as Dict but used UnitLib2 instead of UnitLib
    /// </summary>
    [Serializable]
    public class DictNewUnit : Dictionary<int, double>
    {
        private string bottomUnitName;

        private uint dim;

        #region constructors
        public DictNewUnit()
            : base()
        { this.dim = 0; }
        /// <summary>
        /// This constructor is for copying purpose
        /// </summary>
        /// <param name="d"></param>
        public DictNewUnit(DictNewUnit d)
            : base(d)
        {
            this.dim = d.dim;
        }

        protected DictNewUnit(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            dim = info.GetUInt32("dim");
        }
        #endregion constructors

        #region methods

        public virtual double Total()
        {
            double sum = 0;
            foreach (double val in this.Values)
            {
                sum += val;
            }
            return sum;
        }

        public new string ToString()
        {
            return Total().ToString();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("dim", this.dim);
        }
        #endregion methods

        #region operators

        public static DictNewUnit operator *(DictNewUnit e1, double e2)
        {
            DictNewUnit result = new DictNewUnit(e1);

            foreach (int key in e1.Keys)
                result[key] *= e2;
            return result;
        }
        public static DictNewUnit operator *(DictNewUnit e1, Parameter e2)
        {
            DictNewUnit result = new DictNewUnit(e1);
            //TODO: Need to switch Parameter to UnitLib2 in order for this to work
            result.dim = UnitLib2.DimensionUtils.Plus(e1.dim, 0);

            result *= e2.ValueInDefaultUnit;

            return result;
        }
        public static DictNewUnit operator *(DictNewUnit e1, LightValue e2)
        {
            DictNewUnit result = new DictNewUnit(e1);
            //TODO: Need to switch Parameter to UnitLib2 in order for this to work
            result.dim = UnitLib2.DimensionUtils.Plus(e1.dim, 0);

            result *= e2.Value;

            return result;
        }
        public static DictNewUnit operator *(Parameter e2, DictNewUnit e1)
        {
            DictNewUnit result = new DictNewUnit(e1);
            //TODO: Need to switch Parameter to UnitLib2 in order for this to work
            result.dim = UnitLib2.DimensionUtils.Plus(e1.dim, 0);

            result *= e2.ValueInDefaultUnit;

            return result;
        }
        public static DictNewUnit operator *(LightValue e2, DictNewUnit e1)
        {
            DictNewUnit result = new DictNewUnit(e1);
            //TODO: Need to switch LightValue to UnitLib2 in order for this to work
            result.dim = UnitLib2.DimensionUtils.Plus(e1.dim, 0);

            result *= e2.Value;

            return result;
        }
        public static DictNewUnit operator *(DictNewUnit e1, Dictionary<int, double> e2)
        {
            DictNewUnit result = new DictNewUnit(e1);

            foreach (int key in e1.Keys) //add e1 to the result
            {
                if (e2.Keys.Contains(key))
                    result[key] = e1[key] * e2[key];
            }
            return result;
        }
        public static DictNewUnit operator *(DictNewUnit e1, Dictionary<int, Parameter> e2)
        {
            DictNewUnit result = new DictNewUnit(e1);

            foreach (int key in e1.Keys) //add e1 to the result
            {
                if (e2.Keys.Contains(key))
                    result[key] = e1[key] * e2[key].ValueInDefaultUnit;
            }
            return result;
        }
        public static DictNewUnit operator /(DictNewUnit e1, Parameter e2)
        {
            DictNewUnit result = new DictNewUnit(e1);
            result.dim = UnitLib2.DimensionUtils.Minus(e1.dim, 0);

            foreach (int key in e1.Keys)
                result[key] = result[key] / e2.ValueInDefaultUnit;

            return result;
        }
        public static DictNewUnit operator /(DictNewUnit e1, LightValue e2)
        {
            DictNewUnit result = new DictNewUnit(e1);
            result.dim = UnitLib2.DimensionUtils.Minus(e1.dim, 0);

            foreach (int key in e1.Keys)
                result[key] = result[key] / e2.Value;

            return result;
        }
        public static DictNewUnit operator /(DictNewUnit e1, double e2)
        {
            DictNewUnit result = new DictNewUnit(e1);
            foreach (int key in e1.Keys)
                result[key] = result[key] / e2;

            return result;
        }


        public static DictNewUnit operator +(DictNewUnit e1, DictNewUnit e2)
        {
#if DEBUG
            if (e1.dim != e2.dim)
                throw new System.InvalidOperationException("Summands must have the same dimensions");
#endif
            DictNewUnit result = new DictNewUnit(e1);

            double value;
            foreach (int key in e2.Keys) //add e2 to the result
            {
                if (result.TryGetValue(key, out value))
                    result[key] = value + e2[key];
                else
                    result.Add(key, e2[key]);
            }

            return result;
        }
        /// <summary>
        /// In-place addition, replaces += and enhance performance because less new objects are created
        /// </summary>
        /// <param name="e2"></param>
        internal void Addition(DictNewUnit e2)
        {
            double value;
            foreach (int key in e2.Keys.ToArray()) //add e2 to the result
            {
                if (this.TryGetValue(key, out value))
                    this[key] = value + e2[key];
                else
                    this.Add(key, e2[key]);
            }
        }

        /// <summary>
        /// Add values in the second dictionary to the current one.
        /// </summary>
        /// <param name="e2"></param>
        internal void Addition(Dictionary<int, Parameter> e2)
        {
            double value;
            foreach (int key in e2.Keys) //add e2 to the result
            {
                if (this.TryGetValue(key, out value))
                    this[key] = value + e2[key].ValueInDefaultUnit;
                else
                    this.Add(key, e2[key].ValueInDefaultUnit);
            }

        }
        /// <summary>
        /// Adds the values to the current object after mutiplying them by the double p
        /// Saves time comparing to results = results + (values * p) as this single operation do not create any new objects for the results
        /// </summary>
        /// <param name="p"></param>
        /// <param name="e2"></param>
        internal void MulAdd(double p, DictNewUnit e2)
        {
            double value;
            foreach (int key in e2.Keys.ToArray()) //add e2 to the result
            {
                if (this.TryGetValue(key, out value))
                    this[key] = value + (e2[key] * p);
                else
                    this.Add(key, e2[key] * p);
            }
        }

        public static DictNewUnit operator -(DictNewUnit e)
        {
            DictNewUnit result = new DictNewUnit(e);

            foreach (int key in e.Keys)
            {
                result[key] = -result[key];
            }

            return result;
        }
        public static DictNewUnit operator -(DictNewUnit e1, DictNewUnit e2)
        {
#if DEBUG
            if (e1.dim!=e2.dim)
                throw new System.InvalidOperationException("Summands must have the same dimentions");
#endif
            DictNewUnit result = new DictNewUnit(e1);

            foreach (int key in e2.Keys) //add e2 to the result
            {
                if (result.Keys.Contains(key))
                    result[key] -= e2[key];
                else
                    result.Add(key, -e2[key]);
            }

            return result;
        }

        #endregion operators

        #region accessors
        public string BottomUnitName
        {
            get { return bottomUnitName; }
            set { bottomUnitName = value; }
        }
        #endregion accessors
    }


}
