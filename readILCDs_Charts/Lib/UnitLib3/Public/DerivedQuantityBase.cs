using System;

namespace Greet.UnitLib3
{
    /// <summary>
    /// This class is used to store references to Quantity inside DerivedQuantity that can contain their own default/override and whether
    /// the Quantity is in the numerator or the denominator of the expression
    /// </summary>
    [Serializable]
    public class DerivedQuantityBase
    {
        #region accessors
        internal DerivedQuantity dGroupRef;
        private Unit overrideUnit;
        public Quantity Quantity { get; set; }
        public Unit DefaultUnit { get { return Quantity.DefaultUnit; } }
        public Unit OverrideUnit
        {
            get { return overrideUnit; }
            set
            {
                overrideUnit = value;
                dGroupRef.OnEvent();
            }
        }
        public bool Numerator { get; set; }
        #endregion
        #region constructors
        public DerivedQuantityBase(string groupName, string overrideUnitStr, bool inNumerator, DerivedQuantity derivedGroup) :
            this(Units.QuantityList[groupName] as Quantity, Units.UnitsList[overrideUnitStr], inNumerator, derivedGroup) { }
        internal DerivedQuantityBase(Quantity group, Unit overrideUnit, bool inNumerator, DerivedQuantity derivedGroup)
        {
            Quantity = group;
            this.overrideUnit = overrideUnit;
            Numerator = inNumerator;
            dGroupRef = derivedGroup;
        }
        #endregion
        #region methods
        public override string ToString()
        {
            return Quantity.Name;
        }

        public override bool Equals(object obj)
        {
            if (obj is DerivedQuantityBase)
            {
                DerivedQuantityBase b = obj as DerivedQuantityBase;
                return this.Quantity == b.Quantity && this.Numerator == b.Numerator;
            }
            else
                return false;
        }

        public bool DefaultOnlyEquals(object obj)
        {
            if (obj is DerivedQuantityBase)
            {
                DerivedQuantityBase b = obj as DerivedQuantityBase;
                return this.Quantity == b.Quantity && this.Numerator == b.Numerator;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
