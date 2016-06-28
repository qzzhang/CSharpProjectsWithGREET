using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.ResultsStorage;
using Greet.UnitLib3;

namespace Greet.DataStructureV4.Entities
{
    /// <summary>
    /// A stationary process can contain groups, it can be efficiency group or amount group
    /// </summary>
    [Serializable]
    public class StationaryProcessGroup
    {
        #region enumerators
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public enum GroupType { efficiency, amount };
        #endregion

        #region attributes

        private StationaryProcessGroup.GroupType type;
        private ParameterTS efficiency;
        private ParameterTS amount;
        private List<InputWithShare> shares = new List<InputWithShare>();
        private List<Input> group_amount_inputs = new List<Input>();
        public int processRefId;
        private RealEmissionsFactors groupEmissionsFactors;
        private EmissionAmounts groupConvertedEmissions;
        #endregion attributes

        #region constructors

        public StationaryProcessGroup()
        {
            this.group_amount_inputs = new List<Input>();
            this.shares = new List<InputWithShare>();
        }

        public StationaryProcessGroup(GData data, GroupType type)
            : this()
        {
            this.type = type;
            if (this.type == GroupType.amount)
                this.amount = new ParameterTS(data, "J", 0);
            else if (this.type == GroupType.efficiency)
                this.efficiency = new ParameterTS(data, "%", 100);
        }

        /// <summary>
        /// Creates a stationary process group, where an efficiency or a specified energy amount is shared among multiple inputs
        /// Need to be done last as we do not return the current id reference integer for assigning new ids to other inputs
        /// </summary>
        /// <param name="node">The node to be used to create this object</param>
        /// <param name="process_ref_id">Process reference id is used by the inputs created</param>
        public StationaryProcessGroup(GData data, XmlNode node, int process_ref_id, string optionalParamPrefix)
            : this()
        {
            //sets the process reference where that group is in, used to refer the main input, if the main input is in a group
            this.processRefId = process_ref_id;

            //read in type of the group
            this.type = (GroupType)Enum.Parse(typeof(GroupType), node.Attributes["type"].Value);
            if (this.type == GroupType.efficiency)
                this.efficiency = new ParameterTS(data, node.SelectSingleNode("efficiency"), optionalParamPrefix + "_eff");
            else
                this.amount = new ParameterTS(data, node.SelectSingleNode("amount"), optionalParamPrefix + "_eff");

            //read in shares node if exists
            XmlNode shares_node = node.SelectSingleNode("shares");
            this.shares.Clear();
            if (shares_node != null)
            {
                int count = 0;
                foreach (XmlNode share_node in shares_node.SelectNodes("input"))
                {
                    InputWithShare pf = new InputWithShare(data, share_node, optionalParamPrefix + "_ishare" + count);
                    this.shares.Add(pf);
                    count++;
                }
            }

            //read in other inputs in the group
            foreach (XmlNode input_node in node.SelectNodes("input"))
            {
                Input inp = new Input(data, input_node, optionalParamPrefix + "_inpamt");
                this.group_amount_inputs.Add(inp);
            }
            //read the group emissions which are kind of other emissions adjusted by the efficiency of the group
            XmlNode group_emissions = node.SelectSingleNode("technologies");
            if (group_emissions != null)
                this.groupEmissionsFactors = new RealEmissionsFactors(data, group_emissions, "proc_" + this.processRefId + "_gef");
        }

        #endregion constructors

        #region conversion

        /// <summary>
        /// This function clears and then populates converted_inputs attribute
        /// </summary>
        /// <param name="process"></param>
        public List<Input> GroupToInputs(StationaryProcess process, Resources resources)
        {
            List<Input> converted_inputs = new List<Input>();
            try
            {
                //initialization of the values
                Parameter output_amount = process.MainOutput.DesignAmount.CurrentValue;

                //calculating the amount to share within the group
                LightValue group_input_to_share = new LightValue(0.0, DimensionUtils.ENERGY);
                if (Type == GroupType.amount)
                    group_input_to_share = Amount.CurrentValue.ToLightValue();
                else if (Type == GroupType.efficiency)
                    group_input_to_share = (output_amount / Efficiency.CurrentValue);

                //if the main input is in the group ( usually when the group is efficiency ) we remove the main input amount from the amount of the group
                //do somthing like the -1 in 1/eff-1
                //removing from the amount to share in the group, the amount of the fixed other inputs of the group
                //adding all the other inputs in that group which are not shares but fixed values
                foreach (Input pf in this.Group_amount_inputs)
                {
                    if (pf.NotAnInternalProduct)
                    {
                        pf.AmountForCalculations = pf.DesignAmount.CurrentValue.ToLightValue();
                        pf.urbanShare = process.UrbanShare.ToLightValue();
                        group_input_to_share = group_input_to_share - resources[pf.resourceId].ConvertTo(group_input_to_share.Dim, pf.AmountForCalculations);
                    }
                    converted_inputs.Add(pf);
                }

                //foreach process fuel we assign a amount, based on the share, and the remaining energy of the group
                foreach (InputWithShare pf in Shares)
                {
                    pf.AmountForCalculations = group_input_to_share * pf.Share.CurrentValue;
                    pf.urbanShare = process.UrbanShare.ToLightValue();
                    converted_inputs.Add(pf as Input);
                }

                //populate the group emissions which are based on the group amount to share 
                //and the emissions factors placed within the group
                if (this.GroupEmissionsFactors != null)
                {
                    this.GroupConvertedEmissions = new EmissionAmounts();

                    foreach (KeyValuePair<int, EmissionValue> ef_pair in this.GroupEmissionsFactors.EmissionFactors)
                    {
                        if (this.GroupConvertedEmissions.ContainsKey(ef_pair.Key))
                            this.GroupConvertedEmissions[ef_pair.Key] += (ef_pair.Value.Value * group_input_to_share).Value;
                        else
                            this.GroupConvertedEmissions.Add(ef_pair.Key, (ef_pair.Value.Value * group_input_to_share).Value);
                    }
                }
            }
            catch { }

            return converted_inputs;
        }

        #endregion

        #region methods

        internal XmlNode ToXmlNode(XmlDocument doc)
        {
            XmlNode groupnode = doc.CreateNode("group", doc.CreateAttr("type", this.Type));

            if (this.Type == GroupType.amount)
            {
                XmlNode amountnode = this.Amount.ToXmlNode(doc, "amount");
                groupnode.AppendChild(amountnode);
            }
            else if (this.Type == GroupType.efficiency)
            {
                XmlNode effnode = this.Efficiency.ToXmlNode(doc, "efficiency");
                groupnode.AppendChild(effnode);
            }

            XmlNode sharenode = doc.CreateNode("shares");
            foreach (InputWithShare share in this.Shares)
                sharenode.AppendChild(share.ToXmlNode(doc));
            groupnode.AppendChild(sharenode);

            foreach (Input input in this.Group_amount_inputs)
                groupnode.AppendChild(input.ToXmlNode(doc));

            if (this.GroupEmissionsFactors != null)
            {
                XmlNode group_tech = doc.CreateNode("technologies");
                this.GroupEmissionsFactors.AppendXmlNodes(doc, group_tech);
                groupnode.AppendChild(group_tech);
            }
            return groupnode;
        }

        public bool CheckIntegrity(GData data, bool showIds, bool fixFixableIssues, out string errorMessage)
        {
            StringBuilder problems = new StringBuilder();
            bool canBeHandled = true;
            foreach (Input input in this.Shares)
            {
                string temp = "";
                canBeHandled = canBeHandled || input.CheckIntegrity(data, showIds, fixFixableIssues, out temp);
                if (String.IsNullOrEmpty(temp) == false)
                    problems.Append(temp);
            }

            foreach (Input input in this.Group_amount_inputs)
            {
                string temp = "";
                canBeHandled = canBeHandled || input.CheckIntegrity(data, showIds, fixFixableIssues, out temp);
                if (String.IsNullOrEmpty(temp) == false)
                    problems.Append(temp);
            }

            errorMessage = problems.ToString();
            return canBeHandled;
        }

        #endregion

        #region accessors
        [Browsable(false)]
        public List<Input> Inputs
        {
            get
            {
                List<Input> res = new List<Input>();
                if (this.Group_amount_inputs != null)
                    res.AddRange(this.Group_amount_inputs);
                if (this.Shares != null)
                    res.AddRange(this.Shares);
                return res;
            }
        }

        /// <summary>
        /// Efficiency of the Group when the Group is defined as Efficiency.
        /// </summary>
        public ParameterTS Efficiency
        {
            get { return efficiency; }
            set { efficiency = value; }
        }

        /// <summary>
        /// Total Amount of the Group when the Group defined as Amount value
        /// </summary>
        public ParameterTS Amount
        {
            get { return amount; }
            set { 
                amount = value; 
            }
        }

        /// <summary>
        /// Inputs and corresponding shares
        /// </summary>
        public List<InputWithShare> Shares
        {
            get { return shares; }
            set { shares = value; }
        }

        /// <summary>
        /// Group Inputs 
        /// </summary>
        public List<Input> Group_amount_inputs
        {
            get { return group_amount_inputs; }
            set { group_amount_inputs = value; }
        }

        /// <summary>
        /// Defines whether the group is defined by amount or efficiency
        /// </summary>
        public StationaryProcessGroup.GroupType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Emission corresponding to a group
        /// </summary>
        public RealEmissionsFactors GroupEmissionsFactors
        {
            get { return groupEmissionsFactors; }
            set { groupEmissionsFactors = value; }
        }


        public EmissionAmounts GroupConvertedEmissions
        {
            get { return groupConvertedEmissions; }
            set { groupConvertedEmissions = value; }
        }
        #endregion
    }
}
