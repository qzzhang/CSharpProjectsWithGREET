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
using Greet.UnitLib3;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4.ResultsStorage
{
    [Serializable]
    public class Dict : Dictionary<int, double>
    {
        #region attributes

        private uint bottomDim = 0;

        #endregion attributes

        #region constructors
        public Dict()
            : base()
        { }
        public Dict(uint bottomDim)
            : base()
        {
            this.bottomDim = bottomDim;
        }
        /// <summary>
        /// This constructor is for copying purpose
        /// </summary>
        /// <param name="d"></param>
        public Dict(Dict d)
            : base(d)
        {
            this.bottomDim = d.bottomDim;
        }

        protected Dict(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            bottomDim = info.GetUInt32("bottomDim"); //Hardcoded 
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
            info.AddValue("bottomDim", bottomDim);
        }
        #endregion methods

        #region operators

        public static Dict operator *(Dict e1, double e2)
        {
            Dict result = new Dict(e1);

            foreach (int key in e1.Keys)
                result[key] *= e2;
            return result;
        }
        public static Dict operator *(Dict e1, Parameter e2)
        {
            Dict result = new Dict(e1);

            result.bottomDim = DimensionUtils.Minus(e1.bottomDim, e2.Dim);

            result *= e2.ValueInDefaultUnit;

            return result;
        }
        public static Dict operator *(Dict e1, LightValue e2)
        {
            Dict result = new Dict(e1);
            
            result.bottomDim = DimensionUtils.Minus(e1.bottomDim, e2.Dim);

            result *= e2.Value;

            return result;
        }
        public static Dict operator *(Parameter e2, Dict e1)
        {
            Dict result = new Dict(e1);

            result.bottomDim = DimensionUtils.Minus(e1.bottomDim, e2.Dim);

            result *= e2.ValueInDefaultUnit;

            return result;
        }
        public static Dict operator *(LightValue e2, Dict e1)
        {
            Dict result = new Dict(e1);

            result.bottomDim = DimensionUtils.Minus(e1.bottomDim, e2.Dim);

            result *= e2.Value;

            return result;
        }
     
        public static Dict operator /(Dict e1, Parameter e2)
        {
            Dict result = new Dict(e1);

            result.bottomDim = DimensionUtils.Plus(e2.Dim, e1.bottomDim);

            foreach (int key in e1.Keys)
                result[key] = result[key] / e2.ValueInDefaultUnit;

            return result;
        }
        public static Dict operator /(Dict e1, LightValue e2)
        {
            Dict result = new Dict(e1);

            result.bottomDim = DimensionUtils.Plus(e2.Dim, e1.bottomDim);

            foreach (int key in e1.Keys)
                result[key] = result[key] / e2.Value;

            return result;
        }
        public static Dict operator /(Dict e1, double e2)
        {
            Dict result = new Dict(e1);
            foreach (int key in e1.Keys)
                result[key] = result[key] / e2;

            return result;
        }

        public static Dict operator +(Dict e1, Dict e2)
        {

            if (e1.BottomDim != e2.BottomDim)
                throw new System.InvalidOperationException("Summands must have the same dimentions");

            Dict result = new Dict(e1);

            double value;
            foreach (int key in e2.Keys) //add e2 to the result
            {
                if (result.TryGetValue(key,out value))
                    result[key] = value + e2[key];
                else
                    result.Add(key, e2[key]);
            }

            return result;
        }
        /// <summary>
        /// adds values of a second dict to the current one, replaces += and enhance performance because less new objects are created
        /// </summary>
        /// <param name="e2"></param>
        internal void Addition(Dict e2)
        {
            double value;
            foreach (KeyValuePair<int, double> pair in e2) //add e2 to the result
            {
                if (this.TryGetValue(pair.Key, out value))
                    this[pair.Key] = value + pair.Value;
                else
                    this.Add(pair.Key, pair.Value);
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
        /// In place multiplication and addition this = this + p*e2
        /// Adds the values to the current object after mutiplying them by the double p
        /// Saves time comparing to results = results + (values * p) as this single operation do not create any new objects for the results
        /// </summary>
        /// <param name="p"></param>
        /// <param name="e2"></param>
        internal void MulAdd(double p, Dict e2)
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

        public static Dict operator -(Dict e)
        {
            Dict result = new Dict(e);

            foreach (int key in e.Keys)
            {
                result[key] = -result[key];
            }

            return result;
        }
        public static Dict operator -(Dict e1, Dict e2)
        {

            if (e1.BottomDim != e2.BottomDim)
                throw new System.InvalidOperationException("Summands must have the same dimentions");

            Dict result = new Dict(e1);

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
        public uint BottomDim
        {
            get { return bottomDim; }
            set { bottomDim = value; }
        }
        #endregion accessors
    }

   
}
