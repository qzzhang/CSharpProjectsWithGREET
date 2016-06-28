using Greet.DataStructureV4.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.EcoSpold01.Entities
{
    class OutputFlow
    {
        private string _flowCategory;
        private string _flowName;
        private string _flowUnit;
        private double _flowMeanValue;
        private int _outputGroup;
        private int _greetResourceID;
        private IParameter _greetParameter;
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

        public int GreetResourceID
        {
            get { return _greetResourceID; }
            set { _greetResourceID = value; }
        }

        public IParameter GreetParameter
        {
            get { return _greetParameter; }
            set { _greetParameter = value; }
        }


        public string GreetFlowType
        {
            get { return _greetFlowType; }
            set { _greetFlowType = value; }
        }
    }
}
