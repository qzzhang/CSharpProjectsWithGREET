using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace readXMLs
{
    class InputFlow
    {
        private string _flowCategory;
        private string _flowName;
        private string _flowUnit;
        private double _flowMeanValue;
        private int _inputGroup;

        public InputFlow(string flowCategory, string flowName, string flowUnit, double flowMeanValue, int inputGroup)
        {
            this._flowCategory = flowCategory;
            this._flowName = flowName;
            this._flowUnit = flowUnit;
            this._flowMeanValue = flowMeanValue;
            this._inputGroup = inputGroup;
        }

    }
}
