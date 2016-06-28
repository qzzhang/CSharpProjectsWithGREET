using System;
using System.ComponentModel;
using System.Text;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.UnitLib3;


namespace Greet.DataStructureV4.Entities
{
    [Serializable]
    public abstract class AOutput : AIO, IIO
    {
        #region Attirbutes
        /// <summary>
        /// new loss according to specifications 001-02
        /// Used only for main output, we trend to remove completely the old losses from the database
        /// </summary>
        private nLoss loss;

        //losses variables
        public LightValue lostAmountBuffer;
        public EmissionAmounts lostEmissionsBuffer;
        public LightValue AmountAfterLossesBufffer;

        #endregion Attirbutes

        #region constructors
        protected AOutput()
            : base()
        { }

        protected AOutput(int resourceId, ParameterTS designAmount)
            : base(resourceId, designAmount)
        {
            
        }

        protected AOutput(GData data, XmlNode node, string optionalParamPrefix)
            : base(data, node, optionalParamPrefix)
        {
            XmlNode loss_node = node.SelectSingleNode("nloss");
            if (loss_node != null)
            {
                this.loss = new nLoss(data, loss_node, optionalParamPrefix + "_loss");
            }
        }
        #endregion constructors

        #region methods

        /// Calculates all the necessay informations about the losses and populate the buffer attributes to avoid recalculated the losses everytime
        /// </summary>
        private void CalculateLossesBuffers(Resources resources)
        {
            this.lostAmountBuffer = new LightValue(0.0, this.DesignAmount.CurrentValue.Dim);
            this.lostEmissionsBuffer = new EmissionAmounts();

            if (loss != null)
            {
                Enem LostAmounts = this.CalculateLosses(this.DesignAmount.CurrentValue.ToLightValue(), resources[this.resourceId], loss);
                this.lostEmissionsBuffer.Addition(LostAmounts.emissions);
                if (LostAmounts.materialsAmounts.ContainsKey(this.resourceId)) //if the calculate loss returns noting in case the rate or some paramaters are null
                {
                    this.lostAmountBuffer += resources[this.resourceId].ConvertTo(this.lostAmountBuffer.Dim, LostAmounts.materialsAmounts[this.resourceId]);
                }
            }
        }

        /// <summary>
        /// Calculates and returns the quantities of material lost and the emissions associated with that loss in an Enem format
        /// </summary>
        /// <param name="doubleValue">The amount before losses which is going to be used for the calculations</param>
        /// <returns>Contains the resources lost and the emissions associated with that loss</returns>
        private Enem CalculateLosses(LightValue doubleValue, ResourceData rd, nLoss loss)
        {
            LightValue amount_lost = loss.Rate.ValueInDefaultUnit * doubleValue;
            Enem resultsStorage = new Enem();
            if (amount_lost.Value != 0)
            {
                #region leakage

                LightValue mass_lost = rd.ConvertToMass(amount_lost);
                resultsStorage.materialsAmounts.AddFuel(rd.Id, amount_lost, rd);

                //get the id of which gas gets created when we loose some of the current material
                foreach (EvaporatedGas pair in rd.evaporatedGases)
                {
                    //add emissiosn to the loss object itself ( the emissions associated to that specific loss, used for display only )
                    if (resultsStorage.emissions.ContainsKey(pair.GasIdReference))
                        resultsStorage.emissions[pair.GasIdReference] += mass_lost.Value * pair.MassRatio.ValueInDefaultUnit;
                    else
                        resultsStorage.emissions.Add(pair.GasIdReference, mass_lost.Value * pair.MassRatio.ValueInDefaultUnit);
                }
                #endregion
            }

            return resultsStorage;
        }

        internal XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode output_node = doc.CreateNode("output");
            base.ToXmlNode(output_node, doc);

            if (this.loss != null)
                output_node.AppendChild(this.loss.ToXmlNode(doc));

            return output_node;
        }

        public abstract bool CheckSpecificIntegrity(GData data, bool showIds, bool fixFixableIssues, out string errorMessage);

        #endregion methods

        #region accessors

        [Browsable(false)]
        private LightValue LostAmount(Resources resources)
        {
            //    //watch out this accessor is tricky because it stores the first calculated value 
            //    //and do not return an updated one until the "buffer" have been nulled !
            if (this.lostAmountBuffer != null)
                return this.lostAmountBuffer;
            else
            {
                this.CalculateLossesBuffers(resources);
                return this.lostAmountBuffer;
            }
        }

        [Browsable(false)]
        public EmissionAmounts LostEmissions(Resources resources)
        {
                if (this.lostEmissionsBuffer != null)
                    return this.lostEmissionsBuffer;
                else
                {
                    this.CalculateLossesBuffers(resources);
                    return this.lostEmissionsBuffer;
                }
        }

        [Browsable(true)]
        public LightValue AmountAfterLosses(Resources resources)
        {
            if (this.AmountAfterLossesBufffer != null)
                return this.AmountAfterLossesBufffer;
            else
            {
                if (this.LostAmount(resources).Value != 0)//the next line depends on the call of this.LostAmount here to calculate the buffer
                    this.AmountAfterLossesBufffer = this.DesignAmount.CurrentValue - resources[this.resourceId].ConvertTo(this.DesignAmount.CurrentValue.Dim, this.lostAmountBuffer); //lost amount buffer can be used here as this.LostAmount has been called in the if statement of the line above, therefore the buffer is calculated.
                else
                    this.AmountAfterLossesBufffer = this.DesignAmount.CurrentValue.ToLightValue();
                return this.AmountAfterLossesBufffer;
            }
        }

        public nLoss Loss
        {
            get
            {
                return loss;
            }
            set
            {
                loss = value;
            }
        }
        #endregion

        internal bool CheckIntegrity(GData data, bool showIds, bool fixFixableIssues, out string errorMessage)
        {
            bool isValid = true;
            StringBuilder problems = new StringBuilder();

            //check resource ID is in our database
            if (data.ResourcesData.ContainsKey(this.resourceId) == false)
            {
                problems.AppendLine("ERROR: Unknown Resource Id -" + this.resourceId);
                isValid = false;
            }

            //check that stateless input cannot be defined in mass or volume units
            if (data.ResourcesData.ContainsKey(this.resourceId))
            {
                ResourceData rd = data.ResourcesData[this.resourceId];
                if (rd.State == Resources.PhysicalState.energy)
                {
                    foreach (Parameter param in this.DesignAmount.Values)
                    {
                        if (param.Dim == DimensionUtils.MASS || param.Dim == DimensionUtils.VOLUME)
                        {
                            problems.AppendLine("Input contains an amount defined with a unit of mass or volume, however the resource is stateless.");
                            isValid = false;
                        }
                    }
                }
            }

            //checks that we do not use a composed unit for the inputs (as it was done in some older versions of the database)
            foreach (Parameter param in this.DesignAmount.Values)
            {
                if (param.GreetValuePreferedExpression.Contains("/") || param.GreetValuePreferedExpression.Contains("*"))
                { //detects something such as joules/grams or any composed unit
                    problems.AppendLine("Input contains an amount defined with a composed unit: " + DimensionUtils.ToMLTh(param.Dim));
                    if (fixFixableIssues)
                    {
                        if (!param.CurrentFormula.Contains("["))
                        {
                            foreach (IQuantity unitGroup in Units.QuantityList.Values)
                            {
                                uint div = DimensionUtils.Minus(param.Dim, unitGroup.Dim);//perforns a substraction to see if only bottom units remains ==> meaning the top unit was the same : ex [J/kg]/[J] -> only kg^-1 remains, top was the same 
                                int mass, dist, time, currency;
                                DimensionUtils.ToMLT(div, out mass, out dist, out time, out currency);
                                if(mass <= 0 && dist <= 0 && time <=0 && currency<=0)
                                {
                                    param._greetValueDim = unitGroup.Dim;
                                    param._greetValuePreferedUnitExpression = unitGroup.Units[unitGroup.PreferedUnitIdx].Expression;
                                    problems.AppendLine("FIXED: This output has been fixed with a unit of " + unitGroup.Name);
                                }
                            }
                        }
                        else
                        {
                            problems.AppendLine("ERROR: The issue cannot be fixed automatically, please check the forumla for that parameter");
                            isValid = false;
                        }
                    }
                    else if (param.CurrentFormula.Contains("["))
                    {
                        problems.AppendLine("ERROR: The issue cannot be fixed automatically, please check the forumla for that parameter");
                        isValid = false;
                    }
                }
            }

            string specificErrors;
            this.CheckSpecificIntegrity(data, showIds, fixFixableIssues, out specificErrors);
            if (!String.IsNullOrEmpty(specificErrors))
                problems.AppendLine(specificErrors);

            errorMessage = problems.ToString();
            return isValid;
        }

        public Guid Id
        {
            get { return base.id; }
        }
    }
}
