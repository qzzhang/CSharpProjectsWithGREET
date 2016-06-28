using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class InputTableRow : Dictionary<int, InputTableObject>, ISerializable
    {
        #region attributes
        string name;
        int id;
        internal static int idCounter = 1;
        #endregion
        #region constructors
        internal InputTableRow(string rowName) { name = rowName; id = idCounter; idCounter++; }
        private InputTableRow(SerializationInfo info, StreamingContext text)
            : base(info, text)
        {
            this.name = info.GetString("name");
            this.id = info.GetInt32("id");
        }
        internal InputTableRow(XmlNode rowNode)
        {
            if (rowNode.Attributes["name"] != null)
                name = rowNode.Attributes["name"].Value;
            if (rowNode.Attributes["id"] != null)
                id = Convert.ToInt32(rowNode.Attributes["id"].Value);
            else { id = idCounter; idCounter++; }
        }
        #endregion
        #region accessors
        public string Name { get { return name; } set { name = value; } }
        public int Id { get { return id; } set { id = value; } }
        #endregion
        #region methods
        public override string ToString() { return name + "  Count = " + Count; }
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("name", this.name);
            info.AddValue("id", this.id);
        }
        #endregion
    }
}
