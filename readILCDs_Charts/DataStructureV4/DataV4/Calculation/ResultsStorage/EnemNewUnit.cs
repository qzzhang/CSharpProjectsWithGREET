using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greet.UnitLib2;
using Greet.DataStructureV3.Entities;

namespace Greet.DataStructureV3.ResultsStorage
{
    /// <summary>
    /// This class contains Energy and Emission objects. Was created for convinience since those two come togeather in many places.
    /// </summary>
    [Serializable]
    internal class EnemNewUnit
    {
        #region attributes

        public ResourceAmountsNewUnit materialsAmounts;
        public EmissionAmountsNewUnit emissions;

        #endregion attributes

        #region constructors

        public EnemNewUnit()
        {
            materialsAmounts = new ResourceAmountsNewUnit();
            emissions = new EmissionAmountsNewUnit();
        }

        private EnemNewUnit(ResourceAmountsNewUnit _en, EmissionAmountsNewUnit _em)
        {
            this.materialsAmounts = _en;
            this.emissions = _em;
        }

        public EnemNewUnit(EnemNewUnit _enem)
            : this()
        {
            foreach (KeyValuePair<int, double> pair in _enem.emissions)
                this.emissions.Add(pair.Key, pair.Value);
            foreach (KeyValuePair<int, LightValue> pair in _enem.materialsAmounts.resources)
                this.materialsAmounts.resources.Add(pair.Key, pair.Value);
            this.BottomUnitName = _enem.BottomUnitName;
        }
        public EnemNewUnit(string bottom_normalize_unit)
            : this()
        {
            this.emissions.BottomUnitName = bottom_normalize_unit;
            this.materialsAmounts.BottomUnitName = bottom_normalize_unit;
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

        public static EnemNewUnit operator *(EnemNewUnit e1, Parameter e2)
        {
            return new EnemNewUnit(e1.materialsAmounts * e2, e1.emissions * e2);
        }
        public static EnemNewUnit operator *(EnemNewUnit e1, LightValue e2)
        {
            return new EnemNewUnit(e1.materialsAmounts * e2, e1.emissions * e2);
        }
        public static EnemNewUnit operator *(Parameter e2, EnemNewUnit e1)
        {
            return new EnemNewUnit(e2 * e1.materialsAmounts, e2 * e1.emissions);
        }
        public static EnemNewUnit operator *(LightValue e2, EnemNewUnit e1)
        {
            return new EnemNewUnit(e2 * e1.materialsAmounts, e2 * e1.emissions);
        }
        public static EnemNewUnit operator *(EnemNewUnit e1, double e2)
        {
            return new EnemNewUnit(e1.materialsAmounts * e2, e1.emissions * e2);
        }
        public static EnemNewUnit operator *(double e2, EnemNewUnit e1)
        {
            return new EnemNewUnit(e1.materialsAmounts * e2, e1.emissions * e2);
        }
        public static EnemNewUnit operator /(EnemNewUnit e1, Parameter e2)
        {
            return new EnemNewUnit(e1.materialsAmounts / e2, e1.emissions / e2);
        }
        public static EnemNewUnit operator /(EnemNewUnit e1, LightValue e2)
        {
            return new EnemNewUnit(e1.materialsAmounts / e2, e1.emissions / e2);
        }
        public static EnemNewUnit operator /(EnemNewUnit e1, double e2)
        {
            return new EnemNewUnit(e1.materialsAmounts / e2, e1.emissions / e2);
        }
        public static EnemNewUnit operator +(EnemNewUnit e1, EnemNewUnit e2)
        {
            return new EnemNewUnit(e1.materialsAmounts + e2.materialsAmounts, e1.emissions + e2.emissions);
        }

        /// <summary>
        /// adds values of a second enem to the current one, replaces += and enhance performance because less new objects are created
        /// </summary>
        /// <param name="e2"></param>
        /// <returns></returns>
        public void Addition(EnemNewUnit e2)
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
        public void MulAdd(double p, EnemNewUnit values)
        {
            this.emissions.MulAdd(p, values.emissions);
            this.materialsAmounts.MulAdd(p, values.materialsAmounts);
        }
        public static EnemNewUnit operator -(EnemNewUnit e1, EnemNewUnit e2)
        {
            return new EnemNewUnit(e1.materialsAmounts - e2.materialsAmounts, e1.emissions - e2.emissions);
        }

        #endregion operators

        #region accessors

        private string BottomUnitName
        {
            set
            {
                this.emissions.BottomUnitName = value;
                this.materialsAmounts.BottomUnitName = value;
            }
            get
            {
                return this.emissions.BottomUnitName;
            }
        }

        #endregion

    }
}
