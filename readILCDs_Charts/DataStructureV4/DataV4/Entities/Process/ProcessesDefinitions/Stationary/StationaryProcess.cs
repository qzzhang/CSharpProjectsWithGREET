/*********************************************************************** 
COPYRIGHT NOTIFICATION 

Email contact: greet@anl.gov 
Copyright (c) 2012, UChicago Argonne, LLC 
All Rights Reserved

THIS SOFTWARE AND MANUAL DISCLOSE MATERIAL PROTECTED UNDER COPYRIGHT 
LAW, AND FURTHER DISSEMINATION IS PROHIBITED WITHOUT PRIOR WRITTEN 
CONSENT OF THE PATENT COUNSEL OF ARGONNE NATIONAL LABORATORY, EXCEPT AS 
NOTED IN THE “LICENSING TERMS AND CONDITIONS” NOTED BELOW. 

************************************************************************ 
ARGONNE NATIONAL LABORATORY, WITH A FACILITY IN THE STATE OF ILLINOIS, 
IS OWNED BY THE UNITED STATES GOVERNMENT, AND OPERATED BY UCHICAGO 
ARGONNE, LLC UNDER PROVISION OF A CONTRACT WITH THE DEPARTMENT OF 
ENERGY. 
************************************************************************
 
***********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.ResultsStorage;
using Greet.LoggerLib;
using System.IO;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// The class which holds the specific attributes and method for stationary processes
    /// </summary>
    [Serializable]
    public class StationaryProcess : AProcess, IHaveAPicture, IStationaryProcess
    {
        #region attributes
        //begin stationary-process-attributes
        //latex A stationary process has tree attributes:
        //latex \textit{groups}, 
        private StationaryProcessGroup _group;

        //latex \textit{inputs} and 
        private List<Input> _otherInputs = new List<Input>();

        public List<Input> OtherInputs
        {
            get { return _otherInputs; }
            set { _otherInputs = value; }
        }

        private bool _automaticCarbonMatrix = true;

       
        //end latex
        #endregion attributes

        #region constructors

         /// <summary>
        /// Constructor reads XML node and creates the process.
        /// </summary>
        /// <param name="node"></param>
        public StationaryProcess(GData data, XmlNode node, string optionalParamPrefix)
        {
            this.FromXmlNode(data, node, optionalParamPrefix);
        }


        public StationaryProcess(GData data)
        {
            this.Id = Convenience.IDs.GetIdUnusedFromTimeStamp((int[])data.ProcessesData.Values.Select(item => item.Id).ToArray());
            this._name = "New Stationary Process " + this.Id;
            UrbanShare = data.ParametersData.CreateRegisteredParameter("%", 0, 0, "proc_" + this.Id + "_urbshare");
        }

        #endregion constructors

        #region accessors
        [Browsable(false)]
        public StationaryProcessGroup Group
        {
            get { return _group; }
            set { _group = value; }
        }
        [Browsable(true)]
        public new string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        /// <summary>
        /// Inserts the Input id to the list if the input is coming from previous and if the list do not already contains this id.        /// 
        /// Method used by PreviousMaterialId method in order to avoid copy pasted code.
        /// </summary>
        /// <param name="list">The list of input ids we are trying to populate</param>
        /// <param name="inp">The input that we try to insert to the list</param>
        private void AppendInt(List<int> list, Input inp)
        {
            if (inp.SourceType == Enumerators.SourceType.Previous && !list.Contains(inp.ResourceId))
                list.Add(inp.ResourceId);
        }       

        public override int Id
        {
            get { return base._id; }
            set { base._id = value; }
        }

        public bool AutomaticCarbonMatrix
        {
            get { return _automaticCarbonMatrix; }
            set { _automaticCarbonMatrix = value; }
        }
        #endregion accessors

        #region methods
        /// <summary>
        /// For saving a new process, transfers all the DoubleValues override values to default, in order to save the process with all the values set as defaults
        /// </summary>
        public void TransfersOverridesAsDefaults()
        {

        }
        public override XmlNode ToXmlNode(XmlDocument doc)
        {

            XmlNode process = doc.CreateNode("stationary");
            base.ToXmlNodeCommon(process, doc);

            process.Attributes.Append(doc.CreateAttr("autoCMatrix", _automaticCarbonMatrix));

            if(this._group!=null)
            {
                process.AppendChild(this._group.ToXmlNode(doc));
            }

            //inputs
            foreach (Input pf in this.OtherInputs)
                process.AppendChild(pf.ToXmlNode(doc));

            //output
            if (MainOutput != null)
                process.AppendChild(MainOutput.ToXmlNode(doc));

            // doc.AppendChild(otherEmissions);

            return process;
        }

        public override void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            base.FromXmlNode(data, node, optionalParamPrefix);
            string status = "readin urban share";
            
            if (node.Attributes["autoCMatrix"] != null)
                _automaticCarbonMatrix = Convert.ToBoolean(node.Attributes["autoCMatrix"].Value);

            try
            {
                //output
                XmlNode output_node = node.SelectSingleNode("output");
                this.MainOutput = new MainOutput(data, output_node, "sproc_" + this.Id + "_output");

                //coproducts
                if (node.SelectSingleNode("coproducts") != null)
                {
                    this.CoProducts = new CoProductsElements(data, node.SelectSingleNode("coproducts"), this.Id, "sproc_" + this.Id + "_copr");
                }

                //other inputs
                status = "reading other inputs";
                int count = 0;
                foreach (XmlNode input_node in node.SelectNodes("input"))
                {
                    Input inp = new Input(data, input_node, "sproc_" + this.Id + "_otherinps_" + count);
                  
                    OtherInputs.Add(inp);
                    count++;
                }

                //stationary process group
                status = "reading group";
                foreach (XmlNode group_node in node.SelectNodes("group"))
                {

                    this._group = new StationaryProcessGroup(data, group_node, this.Id, "sproc_" + this.Id + "_group");
                }

                //trying to assign an automatic transfer matrix if not existing
                if (this.CarbonTransMatrix == null) //was not read by the base constructor in AProcess
                {
                    this.CompleteMatrix(this.CarbonTransMatrix);
                }

            }
            catch (Exception e)
            {
                LogFile.Write("Error 22:" + node.OwnerDocument.BaseURI + "\r\n" + node.OuterXml + "\r\n" + e.Message + "\r\n" + status + "\r\n");
                throw new Exception("Process (" + this.Id + ") was not fully read from the database");
            }


        }

        /// <summary>
        /// Inputs from previous, MainOutput and co products have been checked in Aprocess.CheckIntegrity
        /// This methods is more specific to stationary processes inputs
        /// </summary>
        /// <returns></returns>
        public override bool CheckSpecificIntegrity(GData data, bool showIds, bool fixFixableIssues, out string errorMessage)
        {
            StringBuilder problems = new StringBuilder();
            bool canbeHandledByGreet = true;

            List<Input> inputs = new List<Input>();
            foreach(IInput inp in this.FlattenInputList)
                inputs.Add(inp as Input);

            if(inputs.Count == 0)
                problems.AppendLine("WARNING: No inputs are defined");

            #region check inputs integrity
            foreach (Input inp in inputs)
            {
                string temp = "";
                inp.CheckIntegrity(data, showIds, fixFixableIssues, out temp, (fixFixableIssues ? this._id : -1));

                using (StringReader reader = new StringReader(temp))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = "  " + line;
                    }
                }

                if (String.IsNullOrEmpty(temp) == false)
                    problems.Append(temp);
            }
            #endregion

            List<AOutput> alloutputs = new List<AOutput>();
            if(this.MainOutput != null)
                alloutputs.Add(this.MainOutput);
            foreach (AOutput output in this.CoProducts)
                alloutputs.Add(output);

            #region check outputs integrity
            foreach (AOutput output in alloutputs)
            {
                string temp = "";
                output.CheckIntegrity(data, showIds, fixFixableIssues, out temp);

                using (StringReader reader = new StringReader(temp))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = "  " + line;
                    }
                }

                if (String.IsNullOrEmpty(temp) == false)
                    problems.Append(temp);
            }
            #endregion

            #region checkTransferMatrix
            List<AOutput> outputs = new List<AOutput>();
            if(this.MainOutput != null)
                outputs.Add(this.MainOutput);
            foreach (AOutput output in this.CoProducts.Where(item => item.method == CoProductsElements.TreatmentMethod.allocation))
                outputs.Add(output);
            Dictionary<Guid, Dictionary<Guid, double>> transferMatric = this.CarbonTransMatrix;
            
            bool transfertMatrixIssue = false;
            foreach (AOutput output in outputs)
            {
                foreach (IInput input in inputs)
                {
                    bool contains = transferMatric.ContainsKey(output.id);
                    if(contains)
                        contains &= transferMatric[output.id].ContainsKey(input.Id);
                    transfertMatrixIssue |= !contains;
                }
            }

            if (transfertMatrixIssue && fixFixableIssues) 
            {
                this.CompleteMatrix(this.CarbonTransMatrix);
                transfertMatrixIssue = false;
                problems.AppendLine("FIXED: Carbon transfer matrix dimensions have been updated, however you should check the transfer ratios manually.");
            }
            if (transfertMatrixIssue)
                problems.AppendLine("ERROR: Carbon transfer matrix dimensions do not match numbers of inputs or outputs.");

            #endregion

            errorMessage = problems.ToString();
            return canbeHandledByGreet;
        }

        public override List<IInput> FlattenInputList
        {
            get
            {
                List<IInput> flattenList = new List<IInput>();

                //Add the inputs which are not part of any groups, used for transmission and distribution of electricity and storages mostly
                if (this.OtherInputs != null && this.OtherInputs.Count > 0)
                {
                    foreach (Input inp in this.OtherInputs)
                        flattenList.Add(inp);
                }

                if ((this as StationaryProcess).Group != null)
                {
                    foreach (Input pf in this.Group.Group_amount_inputs)
                        flattenList.Add(pf);

                    //foreach process fuel we assign a amount, based on the share, and the remaining energy of the group
                    foreach (InputWithShare pf in this.Group.Shares)
                        flattenList.Add(pf as IInput);
                }

                return flattenList;
            }
        }

        /// <summary>
        /// Creates the transfer matrix if non existing, resize and add new IOs if needed
        /// new IOs will be created using a transfer of 0 except if they are input from previous
        /// thus considered "Main Inputs"
        /// </summary>
        /// <param name="matrix">A reference to the matrix that needs to be completed, usually the process carbon matrix itself</param>
        /// <returns>0 if the matrix is up to date, -1 if there is no main output and therefore no matrix can be calculated</returns>
        public int CompleteMatrix(Dictionary<Guid, Dictionary<Guid, double>> matrix)
        {
            List<Guid> inputsIDs = new List<Guid>();
            List<Guid> outputsIDs = new List<Guid>();

            //loop though inputs, get ids and assign new ones if necessary
            if (this.OtherInputs != null && this.OtherInputs.Count > 0)
                foreach (Input inp in this.OtherInputs)
                    inputsIDs.Add(inp.id);

            if (this._group != null)
            {
                foreach (Input inp in this._group.Shares)
                    inputsIDs.Add(inp.id);
                
                foreach (Input inp in this._group.Group_amount_inputs)
                    inputsIDs.Add(inp.id);
            }

            ////loop though outputs, get ids and assign new ones if necessary
            foreach (AOutput outp in this.CoProducts)
                outputsIDs.Add(outp.id);

            //find main output ID
            if (this.MainOutput != null)
                outputsIDs.Add(this.MainOutput.id);

            if(matrix == null)
            {
                matrix = new Dictionary<Guid, Dictionary<Guid, double>>();
            }
            else
            {
                //clean up potential IOs that are not in the process anymore but still in the carbon transfer matrix
                List<Guid> outToRemove = new List<Guid>();
                List<Guid> inToRemove = new List<Guid>();
                foreach(KeyValuePair<Guid, Dictionary<Guid, double>> outID in this.CarbonTransMatrix)
                {
                    if(!outputsIDs.Contains(outID.Key))
                        outToRemove.Add(outID.Key);
                    foreach(Guid inpID in outID.Value.Keys)
                    {
                        if(!inputsIDs.Contains(inpID) && !inToRemove.Contains(inpID))
                            inToRemove.Add(inpID);
                    }
                }

                foreach(Guid outRem in outToRemove)
                    matrix.Remove(outRem);
                foreach(Guid inRem in inToRemove)
                    foreach(KeyValuePair<Guid, Dictionary<Guid, double>> outID in this.CarbonTransMatrix)
                        outID.Value.Remove(inRem);
            }
            
            foreach (Guid outID in outputsIDs)
            {
                if (!matrix.ContainsKey(outID))
                    matrix.Add(outID, new Dictionary<Guid, double>());
                foreach (Guid inpID in inputsIDs)
                {
                    if (!matrix[outID].ContainsKey(inpID))
                        matrix[outID].Add(inpID, 0);
                }
            }

            return 0;
        }

        private void ExpandMatrixToSize(DVMatrix dVMatrix, List<int> inputsIDs, List<int> outputsIDs)
        {
            dVMatrix.RowsCount = outputsIDs.Count;
            dVMatrix.ColsCount = inputsIDs.Count;
        }

        /// <summary>
        /// <para>Tries to guess the optimal mass carbon transfert ratio for the process.</para>
        /// <para>Throws an exception if the main output of the process is not set to an instance of an object</para>
        /// </summary>
        /// <param name="data">Current dataset and results from the last previous calculations</param>
        /// <param name="stopWhenOutputMassReached">Stops selecting inputs when the mass of the main output is reached by summing up the biggest selected inputs</param>
        public Dictionary<Guid, Dictionary<Guid, double>> CarbonMatrixDefaults(GData data, bool stopWhenOutputMassReached = true)
        {
            Dictionary<Guid, Dictionary<Guid, double>> matrix = new Dictionary<Guid, Dictionary<Guid, double>>();

            this.CompleteMatrix(matrix);

            if (this.MainOutput == null)
                throw new Exceptions.MainOutputNullException("Main output is not set to an instance of an object");

            //Finds mass of the main output
            MainOutput mo = this.MainOutput;
            int moResId = mo.ResourceId;
            LightValue mainOutputMass = data.ResourcesData[moResId].ConvertTo(DimensionUtils.MASS, new LightValue(mo.DesignAmount.CurrentValue.ValueInDefaultUnit, mo.DesignAmount.CurrentValue.Dim));

            var flattendList = this.FlattenInputList;

            List<Guid> discarded = this.DiscardedInputsForCarbonMatrix(data);

            //Converts all inputs to a mass amount
            Dictionary<Guid, double> inputsByCarbonMass = new Dictionary<Guid,double>();
            LightValue groupAmount = null;

            if (this.Group != null)
            {
                if (this.Group.Amount != null && this.Group.Amount.CurrentValue != null)
                {
                    groupAmount = new LightValue(this.Group.Amount.CurrentValue.ValueInDefaultUnit, this.Group.Amount.CurrentValue.Dim);
                }
                else if (this.Group.Efficiency != null && this.MainOutput != null && this.MainOutput.DesignAmount != null && this.MainOutput.DesignAmount.CurrentValue != null)
                    groupAmount = this.MainOutput.DesignAmount.CurrentValue / this.Group.Efficiency.CurrentValue;
            }
            foreach (Input inp in flattendList)
            {
                if (!discarded.Contains(inp.Id))
                {
                    try
                    {
                        //This is a good idea but causes issues in the case where non combustion emissions are defined in a tehcnology and the carbon is not accounted as "used"
                        //double sumUsedByTechnology = 0;
                        //foreach (TechnologyRef techRef in inp.Technologies)
                        //    sumUsedByTechnology += techRef.ShareValueInDefaultUnit;

                        if (inp.DesignAmount != null && !(inp is InputWithShare))
                        {
                            LightValue inCarbonMass = data.ResourcesData[inp.ResourceId].CarbonContent(inp.DesignAmount.CurrentValue);
                            //inCarbonMass *= (1 - sumUsedByTechnology);
                            inputsByCarbonMass.Add(inp.Id, inCarbonMass.Value);
                        }
                        else if (groupAmount != null && inp is InputWithShare && (inp as InputWithShare).Share != null)
                        {
                            LightValue inCarbonMass = data.ResourcesData[inp.ResourceId].ConvertTo(DimensionUtils.MASS, (inp as InputWithShare).Share.CurrentValue * groupAmount);
                            inCarbonMass = inCarbonMass * data.ResourcesData[inp.ResourceId].CRatio.ValueInDefaultUnit;
                            //inCarbonMass *= (1 - sumUsedByTechnology);
                            inputsByCarbonMass.Add(inp.Id, inCarbonMass.Value);
                        }
                        else
                            inputsByCarbonMass.Add(inp.Id, 0);
                    }
                    catch { }
                }
            }

            //Selects the largest inputs that makes most or more than the mass of the output
            List<Guid> selected = new List<Guid>();
            double inputCarbonMass = 0;
            foreach (KeyValuePair<Guid, double> inId in inputsByCarbonMass.OrderByDescending(item => item.Value))
            {
                if (!discarded.Contains(inId.Key))
                {
                    Input inp = flattendList.Single(item => item.Id == inId.Key) as Input;
                    if (!stopWhenOutputMassReached || inputCarbonMass < mainOutputMass.Value)
                    {
                        selected.Add(inp.Id);
                        inputCarbonMass += inputsByCarbonMass[inp.Id];
                    }
                }
            }

            //calculate shares for each of the inputs
            foreach (IInput input in flattendList)
            {
                if (selected.Contains(input.Id))
                {
                    Input inp = flattendList.Single(item => item.Id == input.Id) as Input;
                    if (inputsByCarbonMass.ContainsKey(input.Id))
                    {
                        double inputlMass = inputsByCarbonMass[input.Id];
                        matrix[this.MainOutput.Id][input.Id] = inputlMass / inputCarbonMass;
                    }
                    else
                        matrix[this.MainOutput.Id][input.Id] = 0;
                }
                else
                    matrix[this.MainOutput.Id][input.Id] = 0;
            }

            return matrix;
        }

        private void AddIfFromPreviousOrWell(Input inp, List<Guid> inputsConsideredAsMain)
        {
            if (inp.SourceType == Enumerators.SourceType.Previous
                || inp.SourceType == Enumerators.SourceType.Well)
                inputsConsideredAsMain.Add(inp.Id);
        }

        #endregion methods

        /// <summary>
        /// <para>Returns a list of GUID of the inputs for that process that should be discarded from use
        /// in the carbon matrix.</para>
        /// <para>These inputs are rejected based on their state, carbon ratio and biomass membership</para>
        /// </summary>
        /// <param name="data">Dataset containing the resources data</param>
        /// <returns>List of input IDs to be discarded</returns>
        public List<Guid> DiscardedInputsForCarbonMatrix(GData data)
        {
            var flattendList = this.FlattenInputList;

            //Discards the stateless inputs (electrciticy...)
            //Discards the inputs with a carbon ratio of zero (water...)
            //Discards the inputs from well with non biomass membership (water...)
            List<Guid> discarded = new List<Guid>();
            foreach (Input inp in flattendList)
            {
                ResourceData resource = data.ResourcesData[inp.ResourceId];
                if (resource.State == Resources.PhysicalState.energy)
                    discarded.Add(inp.Id);
                else if (resource.CarbonRatio != null
                    && ((resource.CarbonRatio.UseOriginal && resource.CarbonRatio.GreetValue == 0)
                    || (!resource.CarbonRatio.UseOriginal && resource.CarbonRatio.UserValue == 0))
                    && !discarded.Contains(inp.Id))
                    discarded.Add(inp.Id);
                else if (inp.SourceType == Enumerators.SourceType.Well
                    && !resource.Memberships.Contains(6)
                    && !discarded.Contains(inp.Id))
                    discarded.Add(inp.Id);
                else if (inp.InternalProduct
                    && !discarded.Contains(inp.Id))
                    discarded.Add(inp.Id);
            }
            return discarded;
        }

        /// <summary>
        /// <para>Returns a list of GUID of the outputs for that process that should be discarded from use
        /// in the carbon matrix.</para>
        /// <para>These outputs are rejected based on their state, carbon ratio and biomass membership</para>
        /// </summary>
        /// <param name="data">Dataset containing the resources data</param>
        /// <returns>List of outputs IDs to be discarded</returns>
        public List<Guid> DiscardedOutputsForCarbonMatrix(GData data)
        {
            var flattendList = this.FlattenAllocatedOutputList;

            //Discards the stateless inputs (electrciticy...)
            //Discards the inputs with a carbon ratio of zero (water...)
            //Discards the inputs from well with non biomass membership (water...)
            List<Guid> discarded = new List<Guid>();
            foreach (IIO inp in flattendList)
            {
                ResourceData resource = data.ResourcesData[inp.ResourceId];
                if (resource.State == Resources.PhysicalState.energy)
                    discarded.Add(inp.Id);
                if (resource.CarbonRatio != null
                    && ((resource.CarbonRatio.UseOriginal && resource.CarbonRatio.GreetValue == 0)
                    || (!resource.CarbonRatio.UseOriginal && resource.CarbonRatio.UserValue == 0))
                    && !discarded.Contains(inp.Id))
                    discarded.Add(inp.Id);
            }
            return discarded;
        }
    
    }
}
