using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.UnitLib3
{
    [DataContract]
    public class BaseQuantity : AQuantity
    {
        #region attributes
        [DataMember]
        public override string Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }
        public override string Name
        {
            get { return _name; }
        }
        public override Unit SiUnit
        {
            get { return (_units.Count > 0) ? _units[0] : null; }
        }
        public override List<Unit> Units
        {
            get { return _units; }
        }
        public override uint Dim
        {
            get { return _dim; }
        }
        public override int PreferedUnitIdx
        {
            get { return _preferredUnitIdx; }
            set { _preferredUnitIdx = value; }
        }
        #endregion

        /// <summary>
        /// This constructor designed to be used during the initialization of the Context
        /// </summary>
        /// <param name="node"></param>
        internal BaseQuantity(XmlNode node)
        {
            this._name = node.Attributes["name"].Value;
            this.Symbol = node.Attributes["common_symbol"].Value;
            string dims = node.Attributes["dimension"].Value;
            int.TryParse(node.Attributes["preferred_unit"].Value, out _preferredUnitIdx);
            string[] dimss = dims.Split(':');
            this._units = new List<Unit>();
            if (dimss.Count() < 4)
                throw new System.ArgumentException(String.Format("XML Node for Quantity {0} does not have all 4 basic dimensions specified. Check dimension attribute", this.Name));
            this._dim = DimensionUtils.FromMLT(System.Convert.ToInt32(dimss[0]), System.Convert.ToInt32(dimss[1]), System.Convert.ToInt32(dimss[2]), System.Convert.ToInt32(dimss[3]));
            Unit u;
            foreach (XmlNode unode in node.SelectNodes("unit"))
            {
                u = new Unit(unode);
                this._units.Add(u);
            }
        }

        public BaseQuantity(uint dim_)
        {
            this._dim = dim_;
        }

        public override XmlNode ToXML(XmlDocument doc)
        {
            int m,l,t,c;
            DimensionUtils.ToMLT(_dim, out m, out l, out t, out c);

            XmlNode node = doc.CreateNode("quantity", doc.CreateAttr("name", _name), doc.CreateAttr("common_symbol", _symbol), 
                doc.CreateAttr("dimension",m+":"+l+":"+t+":"+c), doc.CreateAttr("preferred_unit", _preferredUnitIdx), doc.CreateAttr("epsilon", _epsilon));
            foreach (Unit u in _units)
            {
                XmlNode unode = u.ToXmlNode(doc);
                node.AppendChild(unode);
            }

            return node;
        }

        #region UnitLib API
        public  Unit DefaultUnit { get; set; }
        #endregion


        
    }
}
