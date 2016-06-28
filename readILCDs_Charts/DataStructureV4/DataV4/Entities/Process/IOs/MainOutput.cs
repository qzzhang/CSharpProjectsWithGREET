using System;
using System.Xml;

namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public class MainOutput : AOutput
    {
        #region constructors
        public MainOutput(GData data, XmlNode output_node, string optionalParamPrefix)
            : base(data, output_node, optionalParamPrefix)
        {

        }

        public MainOutput()
            : base()
        {

        }

        public MainOutput(GData data, int resourceId, ParameterTS designAmount)
            : base(resourceId, designAmount)
        {
            
        }

        #endregion
       
        public override bool CheckSpecificIntegrity(GData data, bool showIds, bool fixFixableIssues, out string errorMessage)
        {
            errorMessage = "";
            return true;
        }
    }
}
