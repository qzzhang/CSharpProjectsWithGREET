using System;
using System.ComponentModel;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// This class is inhitited from ProcessFuel class and adds share attribute to it. 
    /// </summary>
    [Serializable]
    public class InputWithShare : Input
    {
        #region attributes

        private ParameterTS share;

        #endregion

        #region constructors
        public InputWithShare(GData data, string preferedUnitExpression, double share, int material_id)
            : base(data,new ParameterTS(data, preferedUnitExpression, share), material_id)
        {
            this.share = new ParameterTS(data, preferedUnitExpression,0, share);
            this.DesignAmount = null; //there is no design amount for an input with share, we only use the share member and calculated input amount for calculations
        }

        public InputWithShare(GData data, XmlNode node, string optionalParamPrefix)
            : base(data, node, optionalParamPrefix)
        {
            share = new ParameterTS(data, node.SelectSingleNode("share"), optionalParamPrefix + "_share_" + this.resourceId);
        }
        #endregion

        #region methods
        public new XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode input = doc.CreateNode("input");
            input.AppendChild(share.ToXmlNode(doc, "share"));
            input.Attributes.Append(doc.CreateAttr("source", this.SourceType));
            base.ToXmlNode(input, doc);
            input.Attributes.RemoveNamedItem("amount");
            return input;
        }
        #endregion

        #region accessors
        [Browsable(true), CategoryAttribute("Shared Process Fuel"), DisplayName("Share")]
        public ParameterTS Share
        {
            get { return this.share; }
            set { this.share = value; }

        }
        #endregion
    }
}
