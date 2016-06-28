using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class LocationData : IGroupAvailable, IHaveAPicture, ILocation, IXmlObj, IGREETEntity
    {
        #region attributes

        private string name;
        private int id;
        private string picture;
        string notes;
        private List<int> memberships = new List<int>();

        public List<int> Memberships
        {
            get { return memberships; }
            set { memberships = value; }
        }

        public bool Discarded { get; set; }
        public string DiscardedReason { get; set; }
        public DateTime DiscardedOn { get; set; }
        public string DiscarededBy { get; set; }

        #endregion attributes

        #region constructors

        public LocationData(GData data)
        {
            this.id = Convenience.IDs.GetIdUnusedFromTimeStamp(data.LocationsData.Keys.ToArray());
            this.name = "New Location " + id.ToString();
            this.picture = "empty.png";
        }

        public LocationData(GData data, XmlNode node, string optionalParamPrefix)
            : this(data)
        {
            FromXmlNode(data, node, optionalParamPrefix);
        }



        #endregion constructors

        #region methods

        /// <summary>
        /// Populates the attributes of the object from an XML node
        /// </summary>
        /// <param name="node"></param>
        private void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            string status = "";
            try
            {
                if (node.Attributes["discarded"] != null)
                {
                    Discarded = Convert.ToBoolean(node.Attributes["discarded"].Value);
                    DiscardedOn = Convert.ToDateTime(node.Attributes["discardedOn"].Value, GData.Nfi);
                    DiscarededBy = node.Attributes["discardedBy"].Value;
                    DiscardedReason = node.Attributes["discardedReason"].Value;
                }

                status = "READING ID";
                this.id = Convert.ToInt32(node.Attributes["id"].Value);
                status = "reading name";
                this.name = node.Attributes["name"].Value;
                status = "reading picture Name";
                this.picture = node.Attributes["picture"].Value;
                if (node.Attributes["notes"] != null)
                    this.notes = node.Attributes["notes"].Value;

                status = "reading memberships";
                foreach (XmlNode node2 in node.SelectNodes("membership"))
                {
                    try
                    {
                        this.memberships.Add(Convert.ToInt32(node2.Attributes["group_id"].Value));
                    }
                    catch (Exception e)
                    {
                        LogFile.Write("Error 450:" + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                LogFile.Write("Error 55:" + node.OwnerDocument.BaseURI + "\r\n" + node.OuterXml + "\r\n" +
                    e.Message + "\r\n" + status + "\r\n");
                throw e;
            }
        }


        public XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode locationNode = doc.CreateNode("location");

            if (this.Discarded)
            {
                locationNode.Attributes.Append(doc.CreateAttr("discarded", Discarded));
                locationNode.Attributes.Append(doc.CreateAttr("discardedReason", DiscardedReason));
                locationNode.Attributes.Append(doc.CreateAttr("discardedOn", DiscardedOn));
                locationNode.Attributes.Append(doc.CreateAttr("discardedBy", DiscarededBy));
            }

            locationNode.Attributes.Append(doc.CreateAttr("name", name));
            locationNode.Attributes.Append(doc.CreateAttr("picture", picture));
            locationNode.Attributes.Append(doc.CreateAttr("id", id));
            locationNode.Attributes.Append(doc.CreateAttr("notes", notes));
            foreach (int member in this.memberships)
                locationNode.AppendChild(doc.CreateNode("membership", doc.CreateAttr("group_id", member)));
            return locationNode;
        }


        #endregion methods

        #region accessors

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string PictureName
        {
            get { return picture; }
            set { picture = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public override String ToString()
        {
            return this.name;
        }
        #endregion accessors

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public void FromXmlNode(IData data, XmlNode node)
        {
            this.FromXmlNode(data as GData, node, "");
        }
    }
}
