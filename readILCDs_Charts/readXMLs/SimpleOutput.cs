using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace readXMLs
{
    class OutputFlow
    {
        private string _flowCategory;
        private string _flowName;
        private string _flowUnit;
        private double _flowMeanValue;
        private int _outputGroup;
        private string _greetFlowType;
        

        public OutputFlow(string flowCategory, string flowName, string flowUnit, double flowMeanValue, int outputGroup)
        {
            // TODO: Complete member initialization
            this._flowCategory = flowCategory;
            this._flowName = flowName;
            this._flowUnit = flowUnit;
            this._flowMeanValue = flowMeanValue;
            this._outputGroup = outputGroup;
        }

        public string GreetFlowType
        {
            get { return _greetFlowType; }
            set { _greetFlowType = value; }
        }
    }
}
