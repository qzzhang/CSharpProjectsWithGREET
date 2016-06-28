using Greet.LoggerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// Represents a Payload that is going to be used in the transportation mode calculations
    /// in order to calculate the energy intensities. Typically a payload contains a mass, and a
    /// reference to the resource being transported.
    /// </summary>
    [Serializable]
    public class MaterialTransportedPayload
    {
        #region globalDefinitions
        /// <summary>
        /// The ResourceData ID associated with that payload
        /// </summary>
        private int reference;
        /// <summary>
        /// Mass of resource being transported by the transportation mode
        /// </summary>
        private Parameter payload;
        private string notes;
        #endregion globalDefinitions

        #region constructors

        public MaterialTransportedPayload(GData data, XmlNode materialPayload, string optionalParamPrefix = "")
        {
            string status = "";
            try
            {
                reference = Convert.ToInt32(materialPayload.Attributes["ref"].Value);
                if (materialPayload.Attributes["notes"] != null)
                    this.notes = materialPayload.Attributes["notes"].Value;
                status = "reading payload";
                if (materialPayload.Attributes["payload"].Value != "")
                    this.payload = data.ParametersData.CreateRegisteredParameter(materialPayload.Attributes["payload"], optionalParamPrefix + "payload_" + reference);
                else
                    this.payload.GreetValue = 0;
                status = "reading reference";

            }
            catch (Exception e)
            {
                LogFile.Write("Error 80:" + materialPayload.OwnerDocument.BaseURI + "\r\n" + materialPayload.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
                throw e;
            }
        }

        public MaterialTransportedPayload(GData data, int resId)
        {
            this.reference = resId;
            this.payload = data.ParametersData.CreateRegisteredParameter("kg", 0);
        }

        #endregion constructors

        #region accessors
        public Parameter Payload
        {
            get { return payload; }
            set { payload = value; }
        }
        public int Reference
        {
            get { return reference; }
            set { reference = value; }
        }
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }
        #endregion accessors

        #region methods

        internal XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode mt = xmlDoc.CreateNode("material_transported");
            mt.Attributes.Append(xmlDoc.CreateAttr("notes", this.notes));
            mt.Attributes.Append(xmlDoc.CreateAttr("payload", this.payload));
            mt.Attributes.Append(xmlDoc.CreateAttr("ref", this.reference));
            return mt;
        }

        #endregion methods
    }
}
