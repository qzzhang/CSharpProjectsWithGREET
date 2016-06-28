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
    public class V3OLDConsumptionsTS : TimeSeries<V3OLDConsumptions>
    {
        #region constructors

        public V3OLDConsumptionsTS(GData data, XmlNode node, string optionalParamPrefix)
        {
            foreach (XmlNode yearNode in node.SelectNodes("year"))
            {
                int year_read = Convert.ToInt32(yearNode.Attributes["value"].Value);

                V3OLDConsumptions consump = new V3OLDConsumptions(year_read);
                this.Add(consump.year, consump);

                if (yearNode.SelectSingleNode("fuel_consumption") != null)
                {
                    consump.tables.Add("fuel_consumption", new V3OLDConsumption(data, "consumption_fuel", yearNode.SelectSingleNode("fuel_consumption"), optionalParamPrefix + "_fc_year_" + year_read));

                }
                if (yearNode.SelectSingleNode("electricity_consumption") != null)
                {
                    consump.tables.Add("electricity_consumption", new V3OLDConsumption(data, "consumption_electricity", yearNode.SelectSingleNode("electricity_consumption"), optionalParamPrefix + "_ec_year_" + year_read));
                }
                if (yearNode.SelectSingleNode("electric_range") != null)
                {
                    consump.tables.Add("electric_range", new V3OLDConsumption(data, "m", yearNode.SelectSingleNode("electric_range"), optionalParamPrefix + "_er_year_" + year_read));
                }
            }
        }

        public V3OLDConsumptionsTS(int year)
        {
            V3OLDConsumptions consump = new V3OLDConsumptions(year);
            this.Add(consump.year, consump);

        }

        protected V3OLDConsumptionsTS(SerializationInfo information, StreamingContext context) :
            base(information, context)
        {

        }
        #endregion
    }
}
