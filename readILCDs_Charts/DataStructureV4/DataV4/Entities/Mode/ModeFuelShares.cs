using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class ModeFuelShares : IModeFuelShares
    {
        #region attributes

        private int id;
        string notes = "";
        private string name;
        private ModeFuelSharesDictionary fuels = new ModeFuelSharesDictionary();

        #endregion attributes

        #region constructors

        public ModeFuelShares()
        {
        }

        public ModeFuelShares(GData data, XmlNode node, string optionalParamPrefix)
            : this()
        {
            string status = "";
            this.fuels.Clear();

            try
            {
                status = "reading id";
                this.id = Convert.ToInt32(node.Attributes["id"].Value);
                //status = "reading urban share";
                //this.urban_share = new DoubleValue(node.Attributes["urban_share"]);
                status = "reading name";
                this.name = node.Attributes["name"].Value;
                if (node.Attributes["notes"] != null)
                    this.notes = node.Attributes["notes"].Value;
                //status = "reading back and forth";
                //this.from_trip = Convert.ToBoolean(node.Attributes["from_trip"].Value);

                foreach (XmlNode fuel in node.SelectNodes("fuel"))
                {
                    ModeEnergySource fref = new ModeEnergySource(data, fuel, optionalParamPrefix + "_" + this.id);
                    this.fuels.Add(fref.ResourceReference, fref);


                }


            }
            catch (Exception e)
            {
                LogFile.Write("Error 94:" + node.OwnerDocument.BaseURI + "/r/n" +
                    node.OuterXml + "/r/n" +
                    e.Message + "/r/n" +
                    status + "/r/n");
            }
        }

        #endregion constructors

        #region accessors
        [Browsable(true), DisplayName("Process Fuels")]
        public ModeFuelSharesDictionary ProcessFuels
        {
            get { return fuels; }
            set { fuels = value; }
        }
        [Browsable(false)]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        [Browsable(false)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion accessors

        #region methods

        internal XmlNode ToXmlNode(XmlDocument txml)
        {
            XmlNode fs_node = txml.CreateNode("share");
            fs_node.Attributes.Append(txml.CreateAttr("name", this.name));
            fs_node.Attributes.Append(txml.CreateAttr("id", this.id));
            fs_node.Attributes.Append(txml.CreateAttr("notes", this.notes));

            foreach (KeyValuePair<InputResourceReference, ModeEnergySource> fuelRef in this.fuels)
                fs_node.AppendChild(fuelRef.Value.ToXmlNode(txml));
            return fs_node;
        }
        public override string ToString()
        {
            return this.Name;
        }
        public bool SafeProcessFuelRemove(InputResourceReference rk, out string msg)
        {
            msg = "";
            this.ProcessFuels.Remove(rk);
            return true;
        }

        #endregion methods
    }
}
