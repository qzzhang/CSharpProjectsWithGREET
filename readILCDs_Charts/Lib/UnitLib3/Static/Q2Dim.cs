using System.Collections.Generic;

namespace Greet.UnitLib3
{
    public class Q2Dim : Dictionary<string, string>
    {
        new public string this[string s]
        {
            get
            {
                uint dim;
                if (uint.TryParse(s, out dim))
                    return s;
                else
                    return Units.QName2Q[ConversionFromOLDUnitLib.OLDGroupName2NEWQuantityName[s]].Dim.ToString();
            }
        }
    }
}