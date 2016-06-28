using Greet.DataStructureV4.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Greet.DataStructureV4.Entities.Legacy
{
    [Serializable]
    [Obsolete("Has been replaced with a newer version or discarded")]
    public class V3OLDMPGsTS : TimeSeries<Parameter>
    {
        #region attributes

        public TimeSeries<string> notes = new TimeSeries<string>();

        #endregion

        #region constructors

        public V3OLDMPGsTS(GData data, String unit, int year)
        {
            this.notes.Add(year, "");
            if (unit.Equals("%"))
                this.Add(year, data.ParametersData.CreateRegisteredParameter(unit, 1));
            else
                this.Add(year, data.ParametersData.CreateRegisteredParameter("m/L", 19131505.09737243));
        }


        public V3OLDMPGsTS(GData data, XmlNode node, string optionalParamPrefix = "")
        {
            foreach (XmlNode year_node in node.SelectNodes("year"))
            {
                int year = Convert.ToInt32(year_node.Attributes["value"].Value);
                XmlNode mpg_node = year_node.SelectSingleNode("mpg");
                if (mpg_node.Attributes["notes"] != null)
                    this.notes.Add(year, mpg_node.Attributes["notes"].Value);
                else
                    this.notes.Add(year, "");
                this.Add(year, data.ParametersData.CreateRegisteredParameter(mpg_node.Attributes["mpg"], optionalParamPrefix + "_" + year));
            }
        }

        protected V3OLDMPGsTS(SerializationInfo information, StreamingContext context) :
            base(information, context)
        {
            notes = (TimeSeries<string>)information.GetValue("notes", typeof(TimeSeries<string>));
        }
        #endregion

        #region methods
        public override void GetObjectData(SerializationInfo info,
                            StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("notes", notes, typeof(TimeSeries<string>));
        }
        #endregion
    }
}
