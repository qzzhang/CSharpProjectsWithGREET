using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greet.UnitLib2;
using System.Runtime.Serialization;
using Greet.DataStructureV3.Entities;

namespace Greet.DataStructureV3.ResultsStorage
{
    [Serializable]
    internal class DVDictNewUnit : Dictionary<int, LightValue>
    {
        #region attributes

        private string bottomUnitName = "";
        private uint dim;

        #endregion attributes

        #region constructors
        public DVDictNewUnit()
            : base()
        { }
        public DVDictNewUnit(string bottom)
            : base()
        {
            this.bottomUnitName = bottom;
        }
        /// <summary>
        /// This constructor is for copying purpose
        /// </summary>
        /// <param name="d"></param>
        private DVDictNewUnit(DVDictNewUnit d)
            : base(d)
        {
            this.bottomUnitName = d.bottomUnitName;
        }

        protected DVDictNewUnit(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            bottomUnitName = info.GetString("bottom_unit_name");
        }
        #endregion constructors

        #region methods

        public void Replace(int i, LightValue val)
        {
            this[i] = val;
        }

        /// <summary>
        /// As DVDict1 can contain amount not convertible to energy amounts
        /// this method can only return the total energy amount of the dictionary.
        /// </summary>
        /// <returns></returns>
        public virtual LightValue TotalEnergy()
        {
            double somme = 0;
            uint edim = UnitLib2.DimensionUtils.FromMLT(1, 2, -2);
            foreach (KeyValuePair<int, LightValue> val in this)
            {

                if (val.Value.Dim.Dim == edim)
                    somme += val.Value.Value;
            }
            LightValue sum = new LightValue(somme, edim);
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
            foreach (KeyValuePair<int, LightValue> val in this)
            {
                sum += val.Value.Value;
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
            info.AddValue("dim", this.dim);
        }

        #endregion methods

        #region operators

        public static bool EverythingIsOKForAdditionSubstraction(DVDictNewUnit e1, DVDictNewUnit e2)
        {
            return
                   ((e1.BottomUnitName != "" && e1.BottomUnitName != "unitless" && e1.BottomUnitName != "ratio") && (e2.BottomUnitName == "" || e2.BottomUnitName == "unitless" || e2.BottomUnitName == "ratio"))
                   ||
                   ((e2.BottomUnitName != "" && e2.BottomUnitName != "unitless" && e2.BottomUnitName != "ratio") && (e1.BottomUnitName == "" || e1.BottomUnitName == "unitless" || e1.BottomUnitName == "ratio"))
                   ||
                   e1.bottomUnitName == e2.bottomUnitName;
        }

        public static DVDictNewUnit operator *(DVDictNewUnit e1, double e2)
        {
            DVDictNewUnit result = new DVDictNewUnit(e1);

            foreach (int key in e1.Keys)
                result[key] *= e2;
            return result;
        }

        public static DVDictNewUnit operator *(DVDictNewUnit e1, Parameter e2)
        {
            DVDictNewUnit result = new DVDictNewUnit(e1);
            result.dim = UnitLib2.DimensionUtils.Plus(e1.dim, 0);

            result *= e2.ValueInDefaultUnit;

            return result;

        }
        public static DVDictNewUnit operator *(DVDictNewUnit e1, LightValue e2)
        {
            DVDictNewUnit result = new DVDictNewUnit(e1);

            result.dim = UnitLib2.DimensionUtils.Plus(e1.dim, e2.Dim.Dim);

            result *= e2.Value;

            return result;
        }
        public static DVDictNewUnit operator *(Parameter e2, DVDictNewUnit e1)
        {
            DVDictNewUnit result = new DVDictNewUnit(e1);
            result.dim = UnitLib2.DimensionUtils.Plus(e1.dim, 0);

            result *= e2.ValueInDefaultUnit;

            return result;
        }
        public static DVDictNewUnit operator *(LightValue e2, DVDictNewUnit e1)
        {
            DVDictNewUnit result = new DVDictNewUnit(e1);

            result.dim = UnitLib2.DimensionUtils.Plus(e1.dim, e2.Dim.Dim);

            result *= e2.Value;

            return result;
        }
        public static DVDictNewUnit operator *(DVDictNewUnit e1, Dictionary<int, double> e2)
        {
            DVDictNewUnit result = new DVDictNewUnit(e1);

            foreach (int key in e1.Keys) //add e1 to the result
            {
                if (e2.Keys.Contains(key))
                    result[key] = e1[key] * e2[key];
            }
            return result;
        }
        public static DVDictNewUnit operator *(DVDictNewUnit e1, Dictionary<int, Parameter> e2)
        {
            DVDictNewUnit result = new DVDictNewUnit(e1);

            foreach (int key in e1.Keys) //add e1 to the result
            {
                if (e2.Keys.Contains(key))
                    result[key] = e1[key] * e2[key].ValueInDefaultUnit;
            }
            return result;
        }
        public static DVDictNewUnit operator /(DVDictNewUnit e1, Parameter e2)
        {
            DVDictNewUnit result = new DVDictNewUnit(e1);
            result.dim = UnitLib2.DimensionUtils.Minus(e1.dim, 0);

            foreach (int key in e1.Keys)
                result[key] = result[key] / e2.ValueInDefaultUnit;

            return result;
        }
        public static DVDictNewUnit operator /(DVDictNewUnit e1, LightValue e2)
        {
            DVDictNewUnit result = new DVDictNewUnit(e1);

            result.dim = UnitLib2.DimensionUtils.Minus(e1.dim, 0);

            foreach (int key in e1.Keys)
                result[key] = result[key] / e2.Value;

            return result;
        }
        public static DVDictNewUnit operator /(DVDictNewUnit e1, double e2)
        {
            DVDictNewUnit result = new DVDictNewUnit(e1);
            foreach (int key in e1.Keys)
                result[key] = result[key] / e2;

            return result;
        }
        public static DVDictNewUnit operator +(DVDictNewUnit e1, DVDictNewUnit e2)
        {
#if DEBUG
            if (EverythingIsOKForAdditionSubstraction(e1, e2) == false)
                throw new System.InvalidOperationException("Summands must have the same dimentions");
#endif
            DVDictNewUnit result = new DVDictNewUnit(e1);

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
        /// adds values of a second DVDict1 to the current one, replaces += and enhance performance because less new objects are created
        /// </summary>
        /// <param name="e2"></param>
        internal void Addition(DVDictNewUnit e2)
        {
            LightValue value;
            foreach (int key in e2.Keys) //add e2 to the result
            {
                if (this.TryGetValue(key, out value))
                    this[key].Value += e2[key].Value; //much faster but loses unit check on the addition with this[key] = value + e2[key];
                else
                    this.Add(key, new LightValue(e2[key].Value, e2[key].Dim));
            }
        }
        /// <summary>
        /// In-placve addition
        /// adds values of a second Dictionary to the current one
        /// </summary>
        /// <param name="e2"></param>
        internal void Addition(Dictionary<int, Parameter> e2)
        {
            LightValue value;
            foreach (int key in e2.Keys) //add e2 to the result
            {
                if (this.TryGetValue(key, out value))
                    this[key] = value + e2[key].ToLightValue2();
                else
                    this.Add(key, e2[key].ToLightValue2());
            }
        }

        /// <summary>
        /// In place multiplication and addition this = this + p*e2
        /// Adds the values to the current object after mutiplying them by the double p
        /// Saves time comparing to results = results + (values * p) as this single operation do not create any new objects for the results
        /// </summary>
        /// <param name="p"></param>
        /// <param name="e2"></param>
        internal void MulAdd(double p, DVDictNewUnit e2)
        {
            LightValue value;
            foreach (int key in e2.Keys) //add e2 to the result
            {
                if (this.TryGetValue(key, out value))
                    this[key] = value + (e2[key] * p);
                else
                    this.Add(key, e2[key] * p);
            }
        }

        public static DVDictNewUnit operator -(DVDictNewUnit e)
        {
            DVDictNewUnit result = new DVDictNewUnit(e);

            foreach (int key in e.Keys)
            {
                result[key] = -result[key];
            }

            return result;
        }
        public static DVDictNewUnit operator -(DVDictNewUnit e1, DVDictNewUnit e2)
        {
#if DEBUG
            if (EverythingIsOKForAdditionSubstraction(e1, e2) == false)
                throw new System.InvalidOperationException("Summands must have the same dimentions");
#endif
            DVDictNewUnit result = new DVDictNewUnit();

            result = e1 + (-e2);

            return result;
        }

        #endregion operators

        #region accessors
        internal string BottomUnitName
        {
            get { return bottomUnitName; }
            set { bottomUnitName = value; }
        }
        #endregion accessors
    }
}
