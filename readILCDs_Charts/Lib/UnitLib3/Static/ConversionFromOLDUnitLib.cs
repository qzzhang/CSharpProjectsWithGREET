using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.UnitLib3
{
    /// <summary>
    /// is a static class that creates Dimension object as it is defined in UnitLib2 based on the unit or group name from the old unit system
    /// </summary>
    public static class ConversionFromOLDUnitLib
    {
        internal static Dictionary<string, string> OLDGroupName2NEWQuantityName = new Dictionary<string, string>();
        internal static Dictionary<string, string> NEWQuantityName2OLDGroupName = new Dictionary<string, string>();
        internal static Dictionary<string, string> OLDUnit2NewFormula = new Dictionary<string, string>();
        internal static Dictionary<string, string> NewFormula2OldUnit = new Dictionary<string, string>();


        public static void BuildConversionContext(XmlDocument doc)
        {
            OLDGroupName2NEWQuantityName.Clear();
            OLDUnit2NewFormula.Clear();
            NEWQuantityName2OLDGroupName.Clear();
            NewFormula2OldUnit.Clear();
            string key, val;
            foreach (XmlNode gnode in doc.GetElementsByTagName("group"))
            {
                key = gnode.Attributes["name"].Value;
                val = gnode.Attributes["quantity"].Value;
                OLDGroupName2NEWQuantityName.Add(key, val);
                if (NEWQuantityName2OLDGroupName.ContainsKey(val))
                    continue;
                NEWQuantityName2OLDGroupName.Add(val, key);
            }
            foreach (XmlNode unode in doc.GetElementsByTagName("oldunit"))
            {
                key = unode.Attributes["name"].Value;
                val = unode.Attributes["formula"].Value;
                OLDUnit2NewFormula.Add(key, val);
                if (NewFormula2OldUnit.ContainsKey(val))
                    continue;
                NewFormula2OldUnit.Add(val, key);
            }
        }

        internal static XmlNode SaveConversionContext(XmlDocument xmlDoc)
        {
            XmlNode node = xmlDoc.CreateNode("oldunitsystem");
            XmlNode groupsNode = xmlDoc.CreateNode("groups");
            node.AppendChild(groupsNode);
            foreach (KeyValuePair<string, string> pair in OLDGroupName2NEWQuantityName)
            {
                XmlNode u = xmlDoc.CreateNode("group", xmlDoc.CreateAttr("name", pair.Key), xmlDoc.CreateAttr("quantity", pair.Value));
                groupsNode.AppendChild(u);
            }
            XmlNode oldUnitsNode = xmlDoc.CreateNode("oldunits");
            node.AppendChild(oldUnitsNode);
            foreach (KeyValuePair<string, string> pair in OLDUnit2NewFormula)
            {
                XmlNode u = xmlDoc.CreateNode("oldunit", xmlDoc.CreateAttr("name", pair.Key), xmlDoc.CreateAttr("formula", pair.Value));
                oldUnitsNode.AppendChild(u);
            }
            return node;
        }



        public static string NewUnitFormula(string old_unit_name)
        {
            return OLDUnit2NewFormula[old_unit_name];
        }

        public static string NewQuantityFromGroup(string group_name)
        {
            return OLDGroupName2NEWQuantityName[group_name];
        }

        public static void GetDimensionAndConvertToSi(string OLDGroupOrUnit, double value, out uint dim, out double si_value)
        {

            string temp;
            si_value = 0;
            dim = 0;
            if (OLDGroupName2NEWQuantityName.ContainsKey(OLDGroupOrUnit))
            {
                temp = OLDGroupName2NEWQuantityName[OLDGroupOrUnit];
                if (Units.QName2Q.ContainsKey(temp))
                {
                    dim = Units.QName2Q[temp].Dim;
                    si_value = value;
                }
                else
                    throw new ArgumentException(String.Format("{0} is not defined in UnitLib2", temp));
            }
            else if (OLDUnit2NewFormula.ContainsKey(OLDGroupOrUnit))
            {
                temp = OLDUnit2NewFormula[OLDGroupOrUnit];
                si_value = GuiUtils.ConvertToSI(temp, value);
                dim = GuiUtils.CreateDim(temp);
            }
            else if (uint.TryParse(OLDGroupOrUnit, out dim))
            {
                si_value = value;
            }
            else
            {
                DerivedQuantity p = new DerivedQuantity(OLDGroupOrUnit);
                if(p == null)
                    throw new Exception("Need to add conversion to data.xml for: " + OLDGroupOrUnit);
                temp = DimensionUtils.ToMLTUnith(p.Dim);
                si_value = GuiUtils.ConvertToSI(temp, value);
                dim = p.Dim;
            }
        }
        public static uint GetDimension(string unit_group_name)
        {
            uint dim;
            string temp;
            if (OLDGroupName2NEWQuantityName.ContainsKey(unit_group_name))
            {
                temp = OLDGroupName2NEWQuantityName[unit_group_name];
                if (Units.QName2Q.ContainsKey(temp))
                {
                    dim = Units.QName2Q[temp].Dim;
                }
                else
                    throw new ArgumentException(String.Format("{0} is not defined in UnitLib2", temp));
            }
            else if (OLDUnit2NewFormula.ContainsKey(unit_group_name))
            {
                temp = OLDUnit2NewFormula[unit_group_name];
                dim = GuiUtils.CreateDim(temp);
            }
            else if (!uint.TryParse(unit_group_name, out dim))
            {
                dim = GuiUtils.CreateDim(SplitString(unit_group_name));
            }
                
            return dim;
        }

        public static string SplitString(string unitExpression)
        {
            string[] parts = unitExpression.Split(new char[] { '/', '*' }, StringSplitOptions.RemoveEmptyEntries);
            int char_number = 0;
            List<string> top = new List<string>();
            List<string> bottom = new List<string>();
            string res = "";
            bool first;
            foreach (string part in parts)
            {
                if (part != "1" && string.IsNullOrEmpty(part) == false)
                {
                    bool denom = (char_number != 0 && (unitExpression[char_number - 1] == '/'));
                    if (denom == true && top.Contains(part) == false)
                        bottom.Add(part);
                    else if (denom == false && bottom.Contains(part) == false)
                        top.Add(part);
                    else if (denom == true && top.Contains(part) == true)
                        top.Remove(part);
                    else if (denom == false && bottom.Contains(part) == true)
                        bottom.Remove(part);
                }
                char_number += part.Length + 1;
            }

            first = true;
            foreach (string t in top)
            {
                if (first)
                {
                    res += OLDUnit2NewFormula[t];
                    first = false;
                    continue;
                }
                res += "*";
                res += t;
            }
            if (top.Count == 0)
                res += "1";
            first = true;
            foreach (string b in bottom)
            {
                if (first)
                {
                    res += "/(";
                    res += OLDUnit2NewFormula[b];
                    first = false;
                    continue;
                }
                res += "*";
                res += OLDUnit2NewFormula[b];
            }
            if (bottom.Count > 0)
                res += ")";
            return res;
        }


      
    }
}
