using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greet.UnitLib3;
using System.Runtime.Serialization;
using Greet.DataStructureV4.Entities;

namespace Greet.DataStructureV4.ResultsStorage
{
    [Serializable]
    public class DVDict : Dictionary<int, LightValue>
    {
        #region attributes

        private uint bottomDim = 0;

        #endregion attributes

        #region constructors
        public DVDict()
            : base()
        { }
        public DVDict(uint bottom)
            : base()
        {
            this.bottomDim = bottom;
        }
        /// <summary>
        /// This constructor is for copying purpose
        /// </summary>
        /// <param name="d"></param>
        public DVDict(DVDict d)
            : base(d)
        {
            this.bottomDim = d.bottomDim;
        }

        protected DVDict(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            bottomDim = info.GetUInt32("bottom_dim");
        }
        #endregion constructors

        #region methods

        public void Replace(int i, LightValue val)
        {
            this[i] = val;
        }

        /// <summary>
        /// As DVDict can contain amount not convertible to energy amounts
        /// this method can only return the total energy amount of the dictionary.
        /// </summary>
        /// <returns></returns>
        public virtual LightValue TotalEnergy()
        {
            double somme = 0;
            foreach (KeyValuePair<int, LightValue> pair in this)
            {
                if (pair.Value.Dim == DimensionUtils.ENERGY)
                    somme += pair.Value.Value;
            }
            LightValue sum = new LightValue(somme, DimensionUtils.ENERGY);
            return sum;
        }

        /// <summary>
        /// This method can will return the total of all the contents of the dictionary
        /// It will ignore all units  
        /// </summary>
        /// <returns></returns>
        public virtual double Total()
        {
            double sum = 0;
            foreach (KeyValuePair<int, LightValue> pair in this)
            {
                sum += pair.Value.Value;
            }

            return sum;
        }

        public new string ToString()
        {
            return TotalEnergy().ToString();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("bottom_dim", bottomDim);
        }

        #endregion methods

        #region operators

      
        public static DVDict operator *(DVDict e1, double e2)
        {
            DVDict result = new DVDict(e1);

            foreach (int key in e1.Keys)
                result[key] *= e2;
            return result;
        }

        public static DVDict operator *(DVDict e1, Parameter e2)
        {
            DVDict result = new DVDict(e1);

            result.bottomDim = DimensionUtils.Minus(e1.bottomDim, e2.Dim);

            result *= e2.ValueInDefaultUnit;

            return result;

        }
        public static DVDict operator *(DVDict e1, LightValue e2)
        {
            DVDict result = new DVDict(e1);

            result.bottomDim = DimensionUtils.Minus(e1.bottomDim, e2.Dim);

            result *= e2.Value;

            return result;
        }
        public static DVDict operator *(Parameter e2, DVDict e1)
        {
            DVDict result = new DVDict(e1);

            result.bottomDim = DimensionUtils.Minus(e1.bottomDim, e2.Dim);

            result *= e2.ValueInDefaultUnit;

            return result;
        }
        public static DVDict operator *(LightValue e2, DVDict e1)
        {
            DVDict result = new DVDict(e1);

            result.bottomDim = DimensionUtils.Minus(e1.bottomDim, e2.Dim);

            result *= e2.Value;

            return result;
        }
        public static DVDict operator *(DVDict e1, Dictionary<int, double> e2)
        {
            DVDict result = new DVDict(e1);

            foreach (int key in e1.Keys) //add e1 to the result
            {
                if (e2.Keys.Contains(key))
                    result[key] = e1[key] * e2[key];
            }
            return result;
        }
        public static DVDict operator *(DVDict e1, Dictionary<int, Parameter> e2)
        {
            DVDict result = new DVDict(e1);

            foreach (int key in e1.Keys) //add e1 to the result
            {
                if (e2.Keys.Contains(key))
                    result[key] = e1[key] * e2[key].ValueInDefaultUnit;
            }
            return result;
        }
        public static DVDict operator /(DVDict e1, Parameter e2)
        {
            DVDict result = new DVDict(e1);

            result.bottomDim = DimensionUtils.Plus(e2.Dim, e1.bottomDim);

            foreach (int key in e1.Keys)
                result[key] = result[key] / e2.ValueInDefaultUnit;

            return result;
        }
        public static DVDict operator /(DVDict e1, LightValue e2)
        {
            DVDict result = new DVDict(e1);

            result.bottomDim = DimensionUtils.Plus(e2.Dim, e1.bottomDim);

            foreach (int key in e1.Keys)
                result[key] = result[key] / e2.Value;

            return result;
        }
        public static DVDict operator /(DVDict e1, double e2)
        {
            DVDict result = new DVDict(e1);
            foreach (int key in e1.Keys)
                result[key] = result[key] / e2;

            return result;
        }
        public static DVDict operator +(DVDict e1, DVDict e2)
        {
            if (e1.BottomDim != e2.BottomDim)
                throw new System.InvalidOperationException("Summands must have the same dimentions");
            DVDict result = new DVDict(e1);

            LightValue value;
            foreach (int key in e2.Keys) //add e2 to the result
            {
                if (result.TryGetValue(key, out value))
                    result[key] = value + e2[key];
                else
                    result.Add(key, new LightValue(e2[key].Value, e2[key].Dim));
            }

            return result;
        }
        /// <summary>
        /// adds values of a second DVDict to the current one, replaces += and enhance performance because less new objects are created
        /// </summary>
        /// <param name="e2"></param>
        public void Addition(DVDict e2)
        {
            LightValue value;
            foreach (KeyValuePair<int, LightValue> pair in e2) //add e2 to the result
            {
                if (this.TryGetValue(pair.Key, out value))
                    this[pair.Key].Value += pair.Value.Value; //much faster but loses unit check on the addition with this[key] = value + e2[key];
                else
                    this.Add(pair.Key, new LightValue(pair.Value.Value, pair.Value.Dim));
            }
        }
        /// <summary>
        /// adds values of a second Dictionary to the current one
        /// </summary>
        /// <param name="e2"></param>
        public void Addition(Dictionary<int, Parameter> e2)
        {
            LightValue value;
            foreach (int key in e2.Keys) //add e2 to the result
            {
                if (this.TryGetValue(key, out value))
                    this[key] = value + e2[key].ToLightValue();
                else
                    this.Add(key, e2[key].ToLightValue());
            }
        }

        /// <summary>
        /// Adds the values to the current object after mutiplying them by the double p
        /// Saves time comparing to results = results + (values * p) as this single operation do not create any new objects for the results
        /// </summary>
        /// <param name="p"></param>
        /// <param name="e2"></param>
        public void MulAdd(double p, DVDict e2)
        {
            LightValue value;
            foreach (KeyValuePair<int, LightValue> pair in e2) //add e2 to the result
            {
                if (this.TryGetValue(pair.Key, out value))
                    this[pair.Key] = value + (pair.Value * p);
                else
                    this.Add(pair.Key, pair.Value * p);
            }
        }

        public static DVDict operator -(DVDict e)
        {
            DVDict result = new DVDict(e);
            foreach (int key in e.Keys)
            {
                result[key] = -result[key];
            }

            return result;
        }
        public static DVDict operator -(DVDict e1, DVDict e2)
        {
            if(e1.BottomDim != e2.BottomDim)
                throw new System.InvalidOperationException("Summands must have the same dimentions");
            
            DVDict result = new DVDict();
            result = e1 + (-e2);
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
