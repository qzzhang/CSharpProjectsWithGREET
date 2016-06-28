using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Greet.UnitLib
{
    [Serializable]
    internal class Prefixes : Dictionary<double, string>
    {
        public double maxPower = 0;
        public double minPower = 0;

        public Prefixes()
        { }

        internal Prefixes(XmlNode node)
        {

            foreach (XmlNode pref in node.SelectNodes("prefix"))
            {
                this.Add(Convert.ToDouble(pref.Attributes["power"].Value), pref.Attributes["abbrev"].Value);
            }

            foreach (double d in this.Keys)
            {
                maxPower = Math.Max(d, maxPower);
                minPower = Math.Min(d, minPower);
            }
        }

        internal double PowerJustAbove(int p)
        {
            foreach (KeyValuePair<double, string> powers in this.OrderBy(item => item.Key))
            {
                if (powers.Key > p)
                    return powers.Key;
            }
            return double.MaxValue;
        }
    }
}
