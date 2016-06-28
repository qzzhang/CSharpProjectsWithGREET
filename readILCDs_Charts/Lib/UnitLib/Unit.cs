using System;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.UnitLib
{
    /// <summary>
    /// Stores information about a unit.
    /// </summary>
    [Serializable]
    public class Unit : IUnit
    {
        #region properties
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Abbrev { get; set; }
        public double Si_slope {get; set;}
        public double Si_intercept { get; set; }

        //the following methods are being replaced with Si_Slope and Si_Intercept properties
        [Obsolete("Will be replaced by Si_slope and Si_intecept assuming all unit conversion are following a linear equation")]
        private string toDefaultStr;

        [Obsolete("Will be replaced by Si_slope and Si_intecept assuming all unit conversion are following a linear equation")]
        private string fromDefaultStr;

        [Obsolete("Will be replaced by Si_slope and Si_intecept assuming all unit conversion are following a linear equation")]
        public string ToDefaultStr
        {
            get { return toDefaultStr; }
            set
            {
                this.toDefaultStr = value;
            }
        }

        [Obsolete("Will be replaced by Si_slope and Si_intecept assuming all unit conversion are following a linear equation")]
        public string FromDefaultStr
        {
            get { return this.fromDefaultStr; }
            set
            {
                fromDefaultStr = value;
            }
        }


        public bool CustomUnit { get; set; }
        public string BaseGroupName { get; set; }
        public int prefixes { get; set; }
        public string AboveUnit { get; set; }
        public string BelowUnit { get; set; }
        public string notes { get; set; }
        #endregion
        #region methods
        public override string ToString()
        {
            if (String.IsNullOrEmpty(Abbrev))
                return DisplayName;
            else
                return Abbrev;
        }
        internal XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateNode("unit", doc.CreateAttr("name", Name), doc.CreateAttr("display_name", DisplayName), doc.CreateAttr("abbrev", Abbrev),
#pragma warning disable 618
                 doc.CreateAttr("si_slope", Si_slope), doc.CreateAttr("si_intercept", Si_intercept), doc.CreateAttr("fromDefault", FromDefaultStr), doc.CreateAttr("toDefault", ToDefaultStr), doc.CreateAttr("group", BaseGroupName));
#pragma warning restore 618
            if (this.CustomUnit)
                node.Attributes.Append(doc.CreateAttr("customUnit", true));
            if (prefixes != -1)
                node.Attributes.Append(doc.CreateAttr("prefix_serie", prefixes));
            if (String.IsNullOrEmpty(AboveUnit) == false)
                node.Attributes.Append(doc.CreateAttr("above_unit", this.AboveUnit));
            if (String.IsNullOrEmpty(BelowUnit) == false)
                node.Attributes.Append(doc.CreateAttr("below_unit", this.BelowUnit));
            if (String.IsNullOrEmpty(notes) == false)
                node.Attributes.Append(doc.CreateAttr("notes", this.notes));
            return node;
        }
        #endregion
        #region constructors
        public Unit(XmlNode node)
        {
            Name = node.Attributes["name"].Value;
            DisplayName = node.Attributes["display_name"].Value;
            Abbrev = node.Attributes["abbrev"].Value;
            if(node.Attributes["si_slope"] != null)
                Si_slope = Convert.ToDouble(node.Attributes["si_slope"].Value, Units.USCI);
            if(node.Attributes["si_intercept"] != null)
                Si_intercept = Convert.ToDouble(node.Attributes["si_intercept"].Value, Units.USCI);
            if (node.Attributes["toDefault"] != null)
#pragma warning disable 618
                ToDefaultStr = node.Attributes["toDefault"].Value;
#pragma warning restore 618
            if (node.Attributes["fromDefault"] != null)
#pragma warning disable 618
                FromDefaultStr = node.Attributes["fromDefault"].Value;
#pragma warning restore 618
            if (node.Attributes["customUnit"] != null && node.Attributes["customUnit"].Value == "True")
                CustomUnit = true;
            BaseGroupName = node.Attributes["group"].Value;
            if (node.Attributes["prefix_serie"] != null)
                this.prefixes = Convert.ToInt32(node.Attributes["prefix_serie"].Value);
            else
                this.prefixes = -1;
            if (node.Attributes["above_unit"] != null)
                this.AboveUnit = node.Attributes["above_unit"].Value;
            if (node.Attributes["below_unit"] != null)
                this.BelowUnit = node.Attributes["below_unit"].Value;
            if (node.Attributes["notes"] != null)
                this.notes = node.Attributes["notes"].Value;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitName"></param>
        /// <param name="unitAbbrev"></param>
        /// <param name="a">to si conversion equation slope</param>
        /// <param name="b">to si conversion equation intercept</param>
        /// <param name="baseGroupName">Name of the group this unit belongs to</param>
        public Unit(string unitName, string unitAbbrev, double a, double b, string baseGroupName)
        {
            try
            {
                if (unitName == "")
                    throw new Exception();
                this.Name = unitName;
                this.Abbrev = unitAbbrev;
                this.CustomUnit = true;
                this.BaseGroupName = baseGroupName;
                this.prefixes = -1;
            }
            catch
            {
                throw new Exception("Field cannot be empty.");
            }
            this.Si_slope = a;
            this.Si_intercept = b;
            //this.ToDefaultStr = toDef;
            //this.FromDefaultStr = MathParse.Invert(toDef);
        }

        #endregion
    }
}
