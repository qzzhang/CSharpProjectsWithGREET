using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Xml;
using System.Linq;
using Greet.ConvenienceLib;

namespace Greet.UnitLib3
{
    [DataContract]
    public class DerivedQuantity : AQuantity
    {
        #region attributes

        BaseQuantity _top;
        BaseQuantity _bottom;

        #endregion

        #region public accessors
		/// <summary>
        /// Gets the name if the top quantity
        /// </summary>
        public string Top
        {
            get { return _top.Name; }
        }
        public string Bottom
        {
            get { return _bottom.Name; }
        }

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

        #region constructors
        /// <summary>
        /// Creates new quantity based on the denominator and numerator. i.e energy/mass
        /// </summary>
        /// <param name="top_">energy</param>
        /// <param name="bottom_">mass</param>
        /// <param name="symbol_">energy per mass</param>
        /// <param name="preferred_unit_">Index in the list of units to be used as preferred for the quantity</param>		
        public DerivedQuantity(BaseQuantity top_, BaseQuantity bottom_, string symbol_, int preferred_unit_)
        {
            this._top = top_;
            this._bottom = bottom_;
            this._name = this._top.Name + " per " + this._bottom.Name;
            this._units = GenerateUnits(top_, bottom_);
            this._dim = DimensionUtils.Minus(this._top.Dim, this._bottom.Dim);
            this.Symbol = this._top.Symbol + " per " + this._bottom.Symbol;
            this._preferredUnitIdx = preferred_unit_;
            if (this._name == "energy per separative_work")
                ;
        }

        public DerivedQuantity(System.Xml.XmlNode node)
        {
            string topname, bottomname;
            topname = node.Attributes["numerator"].Value;
            bottomname = node.Attributes["denominator"].Value;
            this._top = Greet.UnitLib3.Units.QName2Q[topname] as BaseQuantity;
            this._bottom = Greet.UnitLib3.Units.QName2Q[bottomname] as BaseQuantity;
            this._name = this._top.Name + " per " + this._bottom.Name;
            this._units = GenerateUnits(this._top, this._bottom);
            this._dim = DimensionUtils.Minus(this._top.Dim, this._bottom.Dim);
            this.Symbol = this._top.Symbol + " per " + this._bottom.Symbol;
            if (node.Attributes["name"] != null)
                this._name = node.Attributes["name"].Value;
            else
                this._name = _top.Name + " per" + _bottom.Name;
        }
        #endregion
      
        #region private methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="top_"></param>
        /// <param name="bottom_"></param>
        /// <returns></returns>
        private List<Unit> GenerateUnits(BaseQuantity top_, BaseQuantity bottom_)
        {
            List<Unit> res = new List<Unit>();
            double slope, intercept;
            Unit u;
            //u = new Unit(top.Si_unit.Formula + "/(" + this.bottom.Si_unit.Formula + ")", top.Si_unit.Name + " per " + bottom.Si_unit.Name, 1, 0);
            ////Add SI unit for the new quantity
            //res.Add(u);
            if (this._top.Units != null && this._bottom.Units != null)
            {
                foreach (Unit topu in this._top.Units)
                {
                    foreach (Unit botu in this._bottom.Units)
                    {
                        slope = topu.Si_slope / botu.Si_slope;
                        intercept = (topu.Si_intercept - topu.Si_intercept) / botu.Si_slope;
                        string bottomExpression = (botu.Expression.Contains(" ") ? "(" + botu.Expression + ")" : botu.Expression);
                        u = new Unit(topu.Expression + "/" + bottomExpression, topu.Name + " per " + botu.Name, slope, intercept);
                        res.Add(u);
                    }
                }
            }
            return res;

        }
        #endregion

        #region public methods
        public override XmlNode ToXML(XmlDocument doc)
        {
            XmlNode node = doc.CreateNode("derived_quantity", doc.CreateAttr("name", this._name), doc.CreateAttr("numerator", _top.Name), doc.CreateAttr("denominator", _bottom.Name), doc.CreateAttr("prefered_unit", _preferredUnitIdx));
            return node;
        }
        #endregion

        #region UnitLib API

        private void Clone(AQuantity q)
        {
            this._dim = q.Dim;
            this._name = q.Name;
            this._units = q.Units;
            this._symbol = q.Symbol;
            this._preferredUnitIdx = q.PreferedUnitIdx;
            if (q is DerivedQuantity)
            {
                this._top = (q as DerivedQuantity)._top;
                this._bottom = (q as DerivedQuantity)._bottom;
            }
        }
        /// <summary>
        /// Legaci API: uses string of the folling format 1/joules*kilograms
        /// </summary>
        /// <param name="p"></param>
        public DerivedQuantity(string p)
        {
            this._dim = DimensionUtils.FromString(p);
            uint topDim = DimensionUtils.Numerator(this._dim);
            uint bottomDim = DimensionUtils.Denominator(this._dim);

            if (Greet.UnitLib3.Units.Dim2Quantities.ContainsKey(topDim))
                this._top = (BaseQuantity)Greet.UnitLib3.Units.Dim2Quantities[topDim][0];
            else
                this._top = new BaseQuantity(topDim);
            if (Greet.UnitLib3.Units.Dim2Quantities.ContainsKey(bottomDim))
                this._bottom = (BaseQuantity)Greet.UnitLib3.Units.Dim2Quantities[bottomDim][0];
            else
                this._bottom = new BaseQuantity(bottomDim);
 
            this._units = GenerateUnits(this._top, this._bottom);
            
            this._name = _dim.ToString();
            if(this._top.Symbol != null && this._bottom.Symbol != null)
               this.Symbol = this._top.Symbol + " per " + this._bottom.Symbol;
            this._preferredUnitIdx = 0;
        }
        /// <summary>
        /// Legaci API: GQuantity is created as a result of an operation on two other quantities. 
        /// </summary>
        /// <param name="q1"></param>
        /// <param name="q2"></param>
        /// <param name="operation">* or + or / or -</param>
        public DerivedQuantity(AQuantity q1, AQuantity q2, char operation)
        {
            uint dim = 0;
            if (operation == '/')
                dim = DimensionUtils.Minus(q1.Dim, q2.Dim);
            else if (operation == '*')
                dim = DimensionUtils.Plus(q1.Dim, q2.Dim);
            else if (operation == '-' || operation == '+')
                dim = q1.Dim;
            else
                Debug.Assert(false);
            //if (Units.Dim2Q.ContainsKey(dim))//find the corresponding quantity
            //{
            //    BaseQuantity q = Units.Dim2Q[dim][0];
            //    this.Clone(q);
            //}
            //else
            //{
            this._dim = dim;
            this._name = dim.ToString();
            //}
        }


        /// <summary>
        /// Legaci API: not sure what it does, use with caution 
        /// </summary>
        /// <param name="DGroupMassPerMegajoules1"></param>
        /// <param name="DGroupMassPerMegajoules2"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        public DerivedQuantity(string DGroupMassPerMegajoules1, string DGroupMassPerMegajoules2, string p2, string p3)
        {
            // TODO: Complete member initialization
            this.DGroupMassPerMegajoules1 = DGroupMassPerMegajoules1;
            this.DGroupMassPerMegajoules2 = DGroupMassPerMegajoules2;
            this.p2 = p2;
            this.p3 = p3;
        }

        private string DGroupMassPerMegajoules1;
        private string DGroupMassPerMegajoules2;
        private string p2;
        private string p3;
        internal void OnEvent()
        {
            throw new System.NotImplementedException();
        }
        public string BottomUnit
        {
            get
            {
                return ConversionFromOLDUnitLib.NewFormula2OldUnit[this._bottom.Units[0].Expression];
            }
        }
        /// <summary>
        /// Legaci API: returns old unit name for top quantity
        /// </summary>
        public string TopUnit
        {
            get
            {
                return ConversionFromOLDUnitLib.NewFormula2OldUnit[this._top.Units[0].Expression];
            }
        }
        /// <summary>
        /// Legaci API: do not use this
        /// </summary>		
        public AQuantity DefaultOnlyMatchedGroup
        {
            get
            {
                return this;
                //foreach (BaseQuantity dg in Units.QuantityList.Values)
                //{
                //    if (dg.DefaultOnlyEquals(this))
                //        return dg;
                //}

                //return this;

            }
        }

     

        /// <summary>
        /// Uses an expression to create a new Quantity or return an existing one if a match with the same base dimension already exists
        /// </summary>
        /// <param name="expression">Expression to be parsed, SI or other units are accepted, plural units are accepted</param>
        /// <param name="filteredExpression">The expression matched for the unit system. Removes plurals and returns the correct units to be used</param>
        /// <returns>New Quantity if we couldn't find one that matches the same dimension</returns>
        public static AQuantity FromString(string expression, out string filteredExpression)
        {
            string equivalentSIExpression, equivalentUSERExpression; uint equivalentDim; double equivalentSlope, equivalentIntercept;
            GuiUtils.FilterExpression(expression, out equivalentUSERExpression, out equivalentSIExpression, out equivalentDim, out equivalentSlope, out equivalentIntercept);
            filteredExpression = equivalentUSERExpression;

            //Match with potential existing quantity
            AQuantity equivalentMatch = Greet.UnitLib3.Units.QuantityList.Values.FirstOrDefault(item => item.Dim == equivalentDim);
            if (equivalentMatch != null)
            {//we already have a corresponding quantity
                if (equivalentMatch.Units.Any(item => item.Expression == equivalentUSERExpression || item.Expression == equivalentSIExpression))
                {//we already have the p expression nothing to do 

                }
                else
                {//we don't have the unit expression, we must add a new unit 
                    Unit u = new Unit(equivalentUSERExpression, "auto-" + equivalentUSERExpression, equivalentSlope, equivalentIntercept);
                    equivalentMatch.Units.Add(u);
                    Greet.UnitLib3.Units.UnitsList.Add(u.Name, u);
                }
                return equivalentMatch;
            }
            else
            {//we do not have a corresponding quantity 
                DerivedQuantity dq = new DerivedQuantity(equivalentSIExpression);
                Unit u = new Unit(equivalentUSERExpression, "auto-" + equivalentUSERExpression, equivalentSlope, equivalentIntercept);
#pragma warning disable 618
                u.BaseGroupName = dq._name;
#pragma warning restore 618
                dq.Units.Add(u);
                dq._preferredUnitIdx = 0;
                
                Greet.UnitLib3.Units.Dim2OldGroup.Add(dq.Dim, dq._name);
                Greet.UnitLib3.Units.Dim2Quantities.Add(dq.Dim, new List<AQuantity>() { dq });
                Greet.UnitLib3.Units.Q.Add(dq as AQuantity);
                Greet.UnitLib3.Units.OLDGroup2Dims.Add(dq.Name, dq.Dim.ToString());
                Greet.UnitLib3.Units.QName2Q.Add(dq.Name, dq as AQuantity);
                Greet.UnitLib3.Units.QuantityList.Add(dq.Name, dq);
                Greet.UnitLib3.Units.UELow2Ind.Add(equivalentSIExpression, 0);
                Greet.UnitLib3.Units.UELow2U.Add(equivalentSIExpression, u);
                Greet.UnitLib3.Units.UnitsList.Add(u.Name, u);

                return dq;
            }
        }

      
    }
        #endregion
}

