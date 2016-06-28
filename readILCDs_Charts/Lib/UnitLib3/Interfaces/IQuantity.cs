using System;
namespace Greet.UnitLib3
{
    public interface IQuantity
    { 
        double ConvertFromSI(int to_unit_index, double value);
        double ConvertToSI(int from_unit_index, double value);
        uint Dim { get; }
        System.Collections.Generic.List<string> MemberUnits { get; }
        string Name { get; }
        int PreferedUnitIdx { get; }
        Unit SiUnit { get; }  
        string Symbol { get; set; }
        System.Xml.XmlNode ToXML(System.Xml.XmlDocument doc);
        System.Collections.Generic.List<Unit> Units { get; }

        #region old unit API discarded but kept in the code for compatibility with UnitLib1
        [Obsolete("OLD UnitLib API")]
        string Abbrev { get; }
        [Obsolete("OLD UnitLib API, use IQantity.ConvertFromSI(string expression, double value)")]
        double ConvertFromDefaultToOverride(double valueToConvert);
        [Obsolete("OLD UnitLib API, use static AQantity.ConvertToSI(int unitIndex, double value)")]
        double ConvertFromOverrideToDefault(double value);
        [Obsolete("OLD UnitLib API")]
        bool createdByTheCalculations { get; }
        [Obsolete("OLD UnitLib API")]
        bool DefaultOnlyEquals(AQuantity baseQuantity);
        [Obsolete("OLD UnitLib API")]
        string DefaultUnitAbbrev { get; }
        [Obsolete("OLD UnitLib API")]
        string DisplayName { get; set; }
        [Obsolete("OLD UnitLib API, use IQantity.Units[IQantity.PreferedUnitIdx].Expression")]
        string DisplayUnitStr { get; }
        [Obsolete("OLD UnitLib API")]
        string format { get; }
        [Obsolete("OLD UnitLib API")]
        Unit OverrideUnit { get; set; }
        [Obsolete("OLD UnitLib API")]
        string SIUnitStr { get; }
        [Obsolete("OLD UnitLib API")]
        event AQuantity.UnitChangedDelegate UnitChangedEvent;
        #endregion
    }
}
