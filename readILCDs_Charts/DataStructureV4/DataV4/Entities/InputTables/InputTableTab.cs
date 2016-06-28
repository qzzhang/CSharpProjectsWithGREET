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
    /// Input tables tabs represents a tab into which one or multiple tables can be represented.
    /// Each tab help to classify tables.
    /// </summary>
    public class InputTableTab
    {
        #region attributes
        private string _text = "New Table";
        private int _id = -1;
        private string _help = "";
        #endregion

        #region constructor
        /// <summary>
        /// This constructor creates a new table with the name New Table, the Idset as -1 and the help string set as an empty string
        /// </summary>
        public InputTableTab()
        { }
        /// <summary>
        /// This contstructor creates a new table with all parameters set 
        /// </summary>
        /// <param name="name">The desired name for this new table object</param>
        /// <param name="id">The desired id for this new table object</param>
        /// <param name="help">The desired help string for this new table object</param>
        public InputTableTab(string name, int id, string help)
        {
            _text = name;
            _id = id;
            _help = help;
        }
        /// <summary>
        /// Build an Input table tab from an XMLnode 
        /// </summary>
        /// <param name="node"></param>
        public InputTableTab(XmlNode node)
        {
            _id = Convert.ToInt32(node.Attributes["id"].Value);
            _text = node.Attributes["display_name"].Value;
            _help = node.Attributes["help"].Value;
        }
        #endregion

        #region public accessors
        /// <summary>
        /// Name associated with that Tab, the name is going to be used for the user interface.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        /// <summary>
        /// Id associated with that tab which is going to be used as a reference.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// Help associated with that tab in case a link needs to be embeded there.
        /// </summary>
        public string Help
        {
            get { return _help; }
            set { _help = value; }
        }
        #endregion

        #region methods
        public XmlNode ToXmlNode(XmlDocument xmlDoc)
        {
            XmlNode tabNode = xmlDoc.CreateNode("tab", xmlDoc.CreateAttr("id", _id), xmlDoc.CreateAttr("display_name", _text), xmlDoc.CreateAttr("help", _help));
            return tabNode;
        }
        #endregion

        #region overrides
        public override string ToString()
        {
            return this.Text;
        }
        #endregion
    }
}
