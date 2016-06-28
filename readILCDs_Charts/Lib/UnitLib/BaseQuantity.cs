using System;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Greet.UnitLib
{
    /// <summary>
    /// This is the generic class that contains common accessors and methods.
    /// </summary>
    [Serializable]
    public abstract class BaseQuantity : IQuantity
    {
        #region event
        public delegate void UnitChangedDelegate();
        /// <summary>
        /// Occurs when the overrideUnit changes.
        /// </summary>
        [NonSerialized]
        public UnitChangedDelegate UnitChanged;

        public event UnitChangedDelegate UnitChangedEvent
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
            {
                UnitChanged = (UnitChangedDelegate)Delegate.Combine(UnitChanged, value);
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
            {
                UnitChanged = (UnitChangedDelegate)Delegate.Remove(UnitChanged, value);
            }
        }
        public virtual void OnEvent()
        {
            if (UnitChanged != null) UnitChanged();
        }
        #endregion

        #region properties
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public abstract string Abbrev { get; }
        public abstract string DisplayUnitStr { get; }
        public abstract string SIUnitStr { get; }
        public abstract string DefaultUnitAbbrev { get; }
        #endregion
        
        #region attributes
        /// <summary>
        /// Used to visualize parameters of this group with the number of significant digits specified. This attribute is used with ToString methods. Possible value=0.000
        /// </summary>
        public string format = "";

        #endregion
        #region methods
        public abstract double ConvertFromOverrideToDefault(double valueToConvert);
        public abstract double ConvertFromDefaultToOverride(double valueToConvert);
        public abstract double ConvertFromDefaultToSpecific(double valueToConvert, string unit);
        internal abstract XmlNode ToXmlNode(XmlDocument doc);
        public override string ToString()
        {
            return Name;
        }
        public override bool Equals(object obj)
        {
            if (obj is Quantity)
                return obj == this;
            else if (this is DerivedQuantity)
            {
                DerivedQuantity dg1 = this as DerivedQuantity;
                DerivedQuantity dg2 = obj as DerivedQuantity;

                if (dg1.SIUnitStr == dg2.SIUnitStr
                    && dg1.DisplayUnitStr == dg2.DisplayUnitStr)
                    return true;
                else
                    return false;
            }
            else if (this is Quantity && obj is DerivedQuantity)
            {
                if (((DerivedQuantity)obj).BaseGroups.Count == 0 && this.SIUnitStr == "unitless")
                    return true;
                else if (((DerivedQuantity)obj).BaseGroups.Count == 1
                    && this.SIUnitStr == ((DerivedQuantity)obj).BaseGroups[0].DefaultUnit.Name
                    && this.DisplayUnitStr == ((DerivedQuantity)obj).BaseGroups[0].OverrideUnit.Name
                    && ((DerivedQuantity)obj).BaseGroups[0].Numerator)
                    return true;
                else if (this.Name == (obj as DerivedQuantity).Name)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public bool DefaultOnlyEquals(BaseQuantity obj)
        {
            return String.Equals(obj.SIUnitStr, this.SIUnitStr, StringComparison.OrdinalIgnoreCase);
            // == operator performs case sensitive comparaison, operations is twice faster is we ignore case in the strings
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
        #region constructor
        protected BaseQuantity()
        { }
        protected BaseQuantity(XmlNode node)
        {
            //reads group name
            this.Name = node.Attributes["name"].Value;

            //reads displayed name
            this.DisplayName = node.Attributes["display_name"].Value;

            //read format for display
            if (node.Attributes["format"] != null)
                this.format = node.Attributes["format"].Value;
        }
        #endregion

    }
}