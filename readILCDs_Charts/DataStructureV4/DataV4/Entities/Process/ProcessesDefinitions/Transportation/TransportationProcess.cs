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
using System.Reflection;
using System.Text;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;
using Greet.UnitLib3;


namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// A transportation process in GREET, contains an architecture of steps which define the transportation and how modes are used
    /// the fuel share for each modes are defined in the TransportationFuelShares
    /// </summary>
    [Serializable]
    public class TransportationProcess : AProcess, IHaveAPicture, IDurationProcess, ITransportationProcess
    {
        #region attributes
        /// <summary>
        /// This is a nested list in which the transportation steps are stored
        /// </summary>
        Dictionary<Guid, TransportationStep> _transportationSteps = new Dictionary<Guid, TransportationStep>();

        public Input MainInput = new Input();

        //moisture content
        private Parameter _mainOutputMoistureContent;

        public Parameter MainOutputMoistureContent
        {
            get { return _mainOutputMoistureContent; }
            private set { _mainOutputMoistureContent = value; }
        }

        #endregion attributes

        #region constructors

        public TransportationProcess(GData data)
            : base()
        {
            this.Id = Convenience.IDs.GetIdUnusedFromTimeStamp((int[])data.ProcessesData.Values.Select(item => item.Id).ToArray());
            this.Name = "New Transportation Process " + this.Id;
            this._mainOutputMoistureContent = data.ParametersData.CreateRegisteredParameter("%", 0,0,"tp_" + this._id + "_moisture");
            this.MainOutput = new MainOutput();
            this.MainOutput.DesignAmount = new ParameterTS(data, "kg", 1, 0, "tp_" + this._id + "_output");
            this.MainInput = new Input();
            this.MainInput.DesignAmount = new ParameterTS(data, "kg", 1, 0, "tp_" + this._id + "_input");
            this.MainInput.SourceType = Enumerators.SourceType.Previous;
            this.MainInput.SourceMixOrPathwayID = -1;
            this.UrbanShare = data.ParametersData.CreateRegisteredParameter("%", 0, 0, "proc_" + this.Id + "_urbshare");
            this.AssignDefaultsForMatrix();
        }

        public TransportationProcess(GData data, XmlNode node, string optionalPramaPrefix) : base()
        {
            this.FromXmlNode(data, node, optionalPramaPrefix);
        }
       
        #endregion constructors

        #region accessors

        [Browsable(false)]
        public new string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        [Browsable(false)]
        public Dictionary<Guid,TransportationStep> TransportationSteps
        {
            get { return _transportationSteps; }
            set { _transportationSteps = value; }
        }

        public override int Id
        {
            get { return base._id; }
            set 
            {
                base._id = value;
                foreach (TransportationStep step in _transportationSteps.Values)
                    step.TransportationProcessReferenceId = Id;
            
            }
        }

        #endregion accessors

        #region ITransportationProcess

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<ITransportationStep> Steps
        {
            get
            {
                List<ITransportationStep> transportationSteps = new List<ITransportationStep>();
                foreach (TransportationStep transporationStep in this.TransportationSteps.Values)
                {
                    transportationSteps.Add(transporationStep as ITransportationStep);
                }
                return transportationSteps;
            }
        }

        #endregion

        #region calculations


        /// <summary>
        /// Calculate the storage duration in seconds depending of the steps distances and average speeds for this transportation process
        /// </summary>
        /// <returns></returns>
        public double CalculateStorageDuration(GData data)
        {
            double duration = 0;
            return CalculateStorageDurationFromSteps(data, this.TransportationSteps.Values, ref duration);
        }

        /// <summary>
        /// Calculate the storage duration in seconds for a defined list of steps
        /// </summary>
        /// <param name="steps"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private double CalculateStorageDurationFromSteps(GData data, IEnumerable<TransportationStep> steps, ref double duration)
        {
            foreach (TransportationStep step in steps)
            {
                duration += (step.Distance.CurrentValue.ValueInDefaultUnit * step.Share.CurrentValue.ValueInDefaultUnit) / ((data.ModesData[step.Reference] as IHaveAverageSpeed).AverageSpeed.CurrentValue.ValueInDefaultUnit * 24);
            }
            return duration;
        }


        #endregion calculations

        #region methods

        public override XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode process = doc.CreateNode("transportation");
            base.ToXmlNodeCommon(process, doc);

            foreach (TransportationStep step in this._transportationSteps.Values)
            {
                process.AppendChild(step.ToXmlNode(doc));
            }

            process.Attributes.Append(doc.CreateAttr("moisture", this.MainOutputMoistureContent));

            process.AppendChild(this.MainOutput.ToXmlNode(doc));
            process.AppendChild(this.MainInput.ToXmlNode(doc));

            return process;
        }

        public override void FromXmlNode(GData data, XmlNode node, string optionalParamPrefix)
        {
            base.FromXmlNode(data, node, optionalParamPrefix);
            this._transportationSteps = new Dictionary<Guid, TransportationStep>();
            string status = "";

            try
            {
                status = "reading Output";
                this.MainOutput = new MainOutput(data, node.SelectSingleNode("output"), "tp_" + this.Id + "_output");
                this.MainOutput.urbanShare = new LightValue(0, "%");

                status = "reading Main Input";
                if(node.SelectSingleNode("input") != null)
                    this.MainInput = new Input(data, node.SelectSingleNode("input"), "tp_" + this.Id + "_input");

                status = "creating simple transfer matrix for transportation process";
                if (this.CarbonTransMatrix == null)//was not read by the base constructor in AProcess
                {
                    this.CarbonTransMatrix = new Dictionary<Guid, Dictionary<Guid, double>>();
                    throw new NotImplementedException(); //also check for the same issue in the constructor of that class, we now need an IO IDs for mapping the transfer matrix
                }

                status = "reading moistureContent";
                if (node.Attributes["moisture"] != null)
                    this.MainOutputMoistureContent = data.ParametersData.CreateRegisteredParameter(node.Attributes["moisture"], "tp_" + this.Id + "_moisture");
                
                status = "reading steps";
                foreach (XmlNode transportationStep in node.ChildNodes)
                {
                    try
                    {
                        if (transportationStep.Name == "step")
                        {
                            TransportationStep tps = new TransportationStep(data, transportationStep, this.Id, "tp_" + this.Id + "_st");
                            this._transportationSteps.Add(tps.Id, tps);
                        }
                    }
                    catch (Exception e) { LogFile.Write("Error 36:" + e.Message); }
                }
            }
            catch (Exception e)
            {
                LogFile.Write("Error 40:" + node.OwnerDocument.BaseURI + "\r\n" +
                    node.OuterXml + "\r\n" +
                    e.Message + "\r\n" +
                    status + "\r\n");
                throw e;
            }
        }

        /// <summary>
        /// Checks the integrity of a transportation process
        /// </summary>
        /// <param name="data">Database used to test constraints such as resource ids, processes upstreams</param>
        /// <param name="showIds"></param>
        /// <param name="fixFixableIssues"></param>
        /// <param name="errorMessage"></param>
        /// <returns>True if Valid</returns>
        public override bool CheckSpecificIntegrity(GData data, bool showIds, bool fixFixableIssues, out string errorMessage)
        {
            bool canCalculationsHandle = true;
            StringBuilder problems = new StringBuilder();
            if (this.MainOutput == null || this.MainOutput.DesignAmount == null)
            {
                problems.AppendLine(" - Please set the amount of resource you want to transported");
                canCalculationsHandle = false;
            }
            string outputErrorMessage;
            if (!this.MainOutput.CheckIntegrity(data, showIds, fixFixableIssues, out outputErrorMessage))
            {
                problems.AppendLine(" - Errors in the process thoughput:");
                problems.AppendLine(" -  " + outputErrorMessage);
                canCalculationsHandle = false;
            }
            #region check steps connections
            foreach (TransportationStep step in _transportationSteps.Values)
            {
                string stepIssue;
                canCalculationsHandle &= step.CheckIntegrity(data, showIds, fixFixableIssues, MainOutput.ResourceId, out stepIssue);
                if (!string.IsNullOrEmpty(stepIssue))
                    problems.AppendLine(stepIssue);
            }
            #endregion check steps connection

            #region checkTransferMatrix

            if (!this.CarbonTransMatrix.ContainsKey(this.MainOutput.Id)
                || !this.CarbonTransMatrix[this.MainOutput.Id].ContainsKey(this.MainInput.Id))
            {
                this.CarbonTransMatrix = new Dictionary<Guid, Dictionary<Guid, double>>();
                this.AssignDefaultsForMatrix();
                problems.AppendLine(" - Carbon transfer matrix has been updated according to inputs and outputs");
            }

            #endregion

            errorMessage = problems.ToString();
            return canCalculationsHandle;
        }

        public override List<IInput> FlattenInputList
        {
            get
            {
                List<IInput> flattenList = new List<IInput>();

                flattenList.Add(MainInput);

                return flattenList;
            }
        }
       
        /// <summary>
        /// This method checks if the any of the steps of the transportation process uses mode with id = mode_id
        /// </summary>
        /// <param name="mode_id">Id of a transportation mode</param>
        /// <returns></returns>
        public bool UsesMode(int mode_id)
        { 
            foreach (TransportationStep step in this.TransportationSteps.Values)
            {
                if (step.modeReference == mode_id)
                    return true;
            }
            return false;

        }
        
        public bool UsesFuelShare(int mode_id, int fuel_share_ref)
        {
            foreach (TransportationStep step in this.TransportationSteps.Values)
            {
                if (step.modeReference == mode_id && step.FuelShareRef == fuel_share_ref)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a new Instance of a CarbonTransferMatrix
        /// Uses the MainInput.Id and MainOutput.Id in order to create a singleton transfer matrix with a single element set to 1
        /// </summary>
        public void AssignDefaultsForMatrix()
        {
            this.CarbonTransMatrix = new Dictionary<Guid, Dictionary<Guid, double>>();

            if (!this.CarbonTransMatrix.ContainsKey(this.MainOutput.Id))
                this.CarbonTransMatrix.Add(this.MainOutput.Id, new Dictionary<Guid, double>());

            if (!this.CarbonTransMatrix[this.MainOutput.Id].ContainsKey(this.MainInput.Id))
                this.CarbonTransMatrix[this.MainOutput.Id].Add(this.MainInput.Id, 1);
        }

        /// <summary>
        /// This method fill is the transportationOrganization parameter by recursevlely 
        /// reading the structure from the transportation process XmlNode
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="node"></param>
        public void RecursiveSearch(GData data, IEnumerable<TransportationStep> structure, XmlNode node, string optionalParamPrefix)
        {
           
        }


        #endregion methods

    }
}
