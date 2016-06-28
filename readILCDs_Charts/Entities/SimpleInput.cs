using Greet.DataStructureV4.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greet.Plugins.EcoSpold01.Entities
{
    class InputFlow
    {
        private string _flowCategory;
        private string _flowName;
        private string _flowUnit;
        private double _flowMeanValue;
        private int _inputGroup;
        private int _greetResourceID;
        private IParameter _greetParameter;
        private IInputResourceReference _greetResourceReference;

        public InputFlow(string flowCategory, string flowName, string flowUnit, double flowMeanValue, int inputGroup)
        {
            this._flowCategory = flowCategory;
            this._flowName = flowName;
            this._flowUnit = flowUnit;
            this._flowMeanValue = flowMeanValue;
            this._inputGroup = inputGroup;
        }

        public IParameter GreetParameter
        {
            get { return _greetParameter; }
            set { _greetParameter = value; }
        }


        public int GreetResourceID
        {
            get { return _greetResourceID; }
            set { _greetResourceID = value; }
        }

        public IInputResourceReference GreetResourceReference
        {
            get { return _greetResourceReference; }
            set { _greetResourceReference = value; }
        }
    }
}
