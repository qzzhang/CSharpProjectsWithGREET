
using System;
using System.Xml;
using Greet.ConvenienceLib;

namespace Greet.UnitLib3
{
    public class Unit : Greet.UnitLib3.IUnit
    {
        #region private members
        private double _si_slope;
        private double _si_intercept;
        private string _expression;
        private string _name;
        private string _aboveName = "";
        private string _belowName = "";
        private bool _showFormula = true;
        #endregion

        #region constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formula">represnetaion of the unit i.e. g</param>
        /// <param name="name">name of the unit, i.e. gram</param>
        /// <param name="si_slope">slope used to convert to SI, i.e 1000</param>
        /// <param name="si_intercept"></param>		
        public Unit(string formula, string name, double si_slope, double si_intercept)
        {
            _expression = formula;
            _name = name;
            _si_slope = si_slope;
            _si_intercept = si_intercept;
        }
        public Unit(System.Xml.XmlNode node)
        {
            _expression = node.Attributes["formula"].Value;
            if (node.Attributes["showFormula"] != null)
                _showFormula = Convert.ToBoolean(node.Attributes["showFormula"].Value);
            _name = node.Attributes["name"].Value;
            if (node.Attributes["si_slope"] != null)
                _si_slope = System.Convert.ToDouble(node.Attributes["si_slope"].Value, UnitLib3.Units.USCI);
            if(node.Attributes["si_intercept"] != null)
                _si_intercept = System.Convert.ToDouble(node.Attributes["si_intercept"].Value, UnitLib3.Units.USCI);
            if (node.Attributes["above"] != null)
                _aboveName = node.Attributes["above"].Value;
            if (node.Attributes["below"] != null)
                _belowName = node.Attributes["below"].Value;
        }
        #endregion 
         
        #region public conversion methods
        /// <summary>
        /// Converts from SI to the given unit
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public double FromSI(double val)
        {

            return val * Si_slope + Si_intercept;
        }
        /// <summary>
        /// Converts from this unit to an SI value
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>		
        public double ToSI(double val)
        {
            return (val - Si_intercept) / Si_slope;
        }
        #endregion

        public override string ToString()
        {
            return this.Expression;
        }

        #region accessors
        /// <summary>
        /// Represents units a formula-like string, for example "kg/m^3" or "lb/ft^3" for mass density
        /// </summary>
	    public string Expression
	    {
		    get { return _expression; }
	    }
        /// <summary>
        /// Represents units a sentence-like string, for example "kilograms per cubic meter" or "pounds per cubic feet" for mass density 
        /// </summary>
	    public string Name
	    {
		    get { return _name; }
	    }
        /// <summary>
        /// Intercept of a linear equation for converting to SI units, i.e. b coefficient of SI_Value=a*This_Value + b
        /// </summary>
        public double Si_intercept
        {
            get { return _si_intercept; }
        }
        /// <summary>
        /// Slope of a linear equation for converting to SI units, i.e. a coefficient of SI_Value = SI_SLope * non_si_value + SI_intercept
        /// </summary>
        public double Si_slope
        {
            get { return _si_slope; }
        }
        /// <summary>
        /// Name of the unit that represents a larger amount
        /// for example the unit above a Joule is a Megajoule
        /// </summary>
        public string AboveName
        {
            get { return _aboveName; }
            set { _aboveName = value; }
        }
        /// <summary>
        /// Name of the unit that represents a smaller amount
        /// for example the unit below a Megajoule is a Joule
        /// </summary>
        public string BelowName
        {
            get { return _belowName; }
            set { _belowName = value; }
        }
        /// <summary>
        /// If set to false the expression will not be shown in the user interface
        /// </summary>
        public bool ShowFormula
        {
            get { return _showFormula; }
            set { _showFormula = value; }
        }
        #endregion
    
        #region UnitLib API
        [Obsolete("OLD UnitLib API")]
        public string Abbrev { get; set; }
        [Obsolete("OLD UnitLib API")]
        public string BaseGroupName { get; set; }
        #endregion

        internal System.Xml.XmlNode ToXmlNode(System.Xml.XmlDocument doc)
        {
            XmlNode node = doc.CreateNode("unit", doc.CreateAttr("name", _name)
                , doc.CreateAttr("formula", _expression), doc.CreateAttr("showFormula", _showFormula)
                , doc.CreateAttr("si_slope", _si_slope), doc.CreateAttr("si_intercept", _si_intercept)
                , doc.CreateAttr("above", _aboveName), doc.CreateAttr("below", _belowName));
            return node;
        }
    }
}
