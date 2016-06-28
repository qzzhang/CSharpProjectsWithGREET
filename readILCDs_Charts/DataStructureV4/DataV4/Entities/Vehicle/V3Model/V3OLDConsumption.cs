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
    public class V3OLDConsumption : Series<Parameter, Parameter>
    {
        #region attributes

        public Series<Parameter, string> consumptionNotes = new Series<Parameter, string>();

        #endregion

        #region constructors
        public V3OLDConsumption(GData data, String type, XmlNode node, string optionalParamPrefix = "")
        {
            if (node != null)
            {
                int count = 0;
                foreach (XmlNode child in node.ChildNodes)
                {
                    Parameter base_range = data.ParametersData.CreateRegisteredParameter(child.Attributes["range"], optionalParamPrefix + "_range_" + count);
                    this.Add(base_range, data.ParametersData.CreateRegisteredParameter(child.Attributes["value"], optionalParamPrefix + "_val_" + count));
                    if (child.Attributes["notes"] != null)
                        consumptionNotes.Add(base_range, child.Attributes["notes"].Value);
                    else
                        consumptionNotes.Add(base_range, "");

                    count++;
                }
            }
        }

        public V3OLDConsumption(GData data, String type)
        {
            //HARDCODED Excel always specififes exactly 12 value pairs, so I followed their example. We may need to take another look at this. 

            if (type.Equals("fuel_consumption"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    Parameter base_range = data.ParametersData.CreateRegisteredParameter("m", 1609.344 * i);
                    this.Add(base_range, data.ParametersData.CreateRegisteredParameter("J/m", 0));
                    consumptionNotes.Add(base_range, "");
                }
            }

            if (type.Equals("electricity_consumption"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    Parameter base_range = data.ParametersData.CreateRegisteredParameter("m", 1609.344 * i);
                    this.Add(base_range, data.ParametersData.CreateRegisteredParameter("J/m", 0));
                    consumptionNotes.Add(base_range, "");
                }
            }

            if (type.Equals("electric_range"))
            {
                for (int i = 0; i <= 11; i++)
                {
                    Parameter base_range = data.ParametersData.CreateRegisteredParameter("m", 1609.344 * i);
                    this.Add(base_range, data.ParametersData.CreateRegisteredParameter("m", 0));
                    consumptionNotes.Add(base_range, "");
                }
            }
        }


        protected V3OLDConsumption(SerializationInfo information, StreamingContext context) :
            base(information, context)
        {
            consumptionNotes = (Series<Parameter, string>)information.GetValue("notes", typeof(Series<Parameter, string>));
        }
        #endregion

        #region accessors
        public new Parameter this[Parameter index]
        {
            get
            {
                foreach (KeyValuePair<Parameter, Parameter> pair in this)
                {
                    if (pair.Key.ValueInDefaultUnit == index.ValueInDefaultUnit && pair.Key.UnitGroupName == index.UnitGroupName)
                        return pair.Value;
                }
                return null;
            }
        }
        #endregion

        #region methods
        public new bool ContainsKey(Parameter index)
        {
            if (this[index] != null)
                return true;
            else
                return false;

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("notes", consumptionNotes, typeof(Series<Parameter, string>));
        }
        #endregion
    }
}
