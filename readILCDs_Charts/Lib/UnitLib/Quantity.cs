using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using Greet.ConvenienceLib;

[assembly: InternalsVisibleTo("Greet.UnitLibTest")]

namespace Greet.UnitLib
{
    /// <summary>
    /// Extends BaseQuantity by including the default and override units and conversion methods between units
    /// </summary>
    [Serializable]
    public class Quantity : BaseQuantity
    {
        #region attributes
        private Unit defaultUnit;
        private Unit overrideUnit;

        #endregion
        #region accessors


        public List<string> MemberUnits = new List<string>();

        public Unit DefaultUnit
        {
            get { return defaultUnit; }
            set { defaultUnit = value; }
        }
        public Unit OverrideUnit
        {
            get
            {
                return overrideUnit;
            }
            set
            {
                if (overrideUnit != value)
                {
                    overrideUnit = value;
                    OnEvent();
                }
            }
        }

        public override string Abbrev { get { return OverrideUnit.Abbrev; } }
        public override string DisplayUnitStr { get { return OverrideUnit.Name; } }
        public override string SIUnitStr { get { return DefaultUnit.Name; } }
        public override string DefaultUnitAbbrev
        {
            get
            {
                if (String.IsNullOrEmpty(DefaultUnit.Abbrev))
                    return DefaultUnit.DisplayName;
                else
                    return DefaultUnit.Abbrev;
            }
        }
        #endregion
        #region methods
        /// <summary>
        /// Conversion from default unit of the group (not necessarily SI) to override unit of the group
        /// </summary>
        /// <param name="valueToConvert">quantity assumed to be in default units</param>
        /// <returns>Value in the override unit of the group</returns>
        public override double ConvertFromDefaultToOverride(double valueToConvert)
        {
            if (OverrideUnit != DefaultUnit)
                return Units.Conversion(valueToConvert, DefaultUnit, OverrideUnit);
            else
                return valueToConvert;
        }
        /// <summary>
        /// Conversion from default unit of the group (not necesseraly SI) to a different unit of the same group
        /// </summary>
        /// <param name="valueToConvert">quantity assumed to be in default units</param>
        /// <param name="unit">name of the unit to convert to</param>
        /// <returns>Value in the unit specified</returns>
        public override double ConvertFromDefaultToSpecific(double valueToConvert, string unit)
        {
            if (Units.UnitsList[unit] != DefaultUnit)
                return Units.Conversion(valueToConvert, DefaultUnit, Units.UnitsList[unit]);
            else
                return valueToConvert;
        }
        /// <summary>
        /// Conversion from override to default unit of the group (not necesseraly SI)
        /// </summary>
        /// <param name="valueToConvert">quantity assumed to be in override units</param>
        /// <returns>Value in the default unit of the group</returns>
        public override double ConvertFromOverrideToDefault(double valueToConvert)
        {
            if (OverrideUnit != DefaultUnit)
                return Units.Conversion(valueToConvert, OverrideUnit, DefaultUnit);
            else
                return valueToConvert;
        }
        internal override XmlNode ToXmlNode(XmlDocument doc)
        {
            return doc.CreateNode("group", doc.CreateAttr("name", Name), doc.CreateAttr("display_name", DisplayName), doc.CreateAttr("format", this.format), doc.CreateAttr("unit", DefaultUnit.Name + ":" + OverrideUnit.Name));
        }
        #endregion
        #region constructors
        public Quantity(XmlNode node)
            : base(node)
        {
            string[] unitSplit = node.Attributes["unit"].Value.Split(":".ToCharArray());
            defaultUnit = Units.UnitsList[unitSplit[0]];
            foreach (Unit unit in Units.UnitsList.Values.Where(item => item.BaseGroupName == Name))
                MemberUnits.Add(unit.Name);
            if (MemberUnits.Contains(unitSplit[1]))
                overrideUnit = Units.UnitsList[unitSplit[1]];//use specified override if it exists in units database
            else
                overrideUnit = Units.UnitsList[unitSplit[0]];//else use default 
        }

        public Quantity(string name, string displayName, string format, string defaultUnit, string overrideUnit)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.format = format;
            this.defaultUnit = Units.UnitsList[defaultUnit];
            this.overrideUnit = Units.UnitsList[overrideUnit];
            foreach (Unit unit in Units.UnitsList.Values.Where(item => item.BaseGroupName == Name))
                MemberUnits.Add(unit.Name);
        }

        #endregion constructors
    }
}
