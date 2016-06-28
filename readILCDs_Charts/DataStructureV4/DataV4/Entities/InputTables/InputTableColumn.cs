using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public sealed class InputTableColumn
    {
        #region attributes
        string name;
        int id;
        public static int idCounter = 1;
        #endregion
        #region constructors
        internal InputTableColumn(string colName) { name = colName; id = idCounter; idCounter++; }
        internal InputTableColumn(XmlNode columnNode)
        {
            if (columnNode.Attributes["name"] != null)
                name = columnNode.Attributes["name"].Value;
            if (columnNode.Attributes["id"] != null)
                id = Convert.ToInt32(columnNode.Attributes["id"].Value);
            else { id = idCounter; idCounter++; }
        }
        #endregion
        #region accessors
        public String Name { get { return name; } set { name = value; } }
        public int Id { get { return id; } set { id = value; } }
        #endregion
        #region methods
        public override string ToString() { return name + ", " + id; }
        #endregion
    }
}
