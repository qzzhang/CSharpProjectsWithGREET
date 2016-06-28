using Greet.DataStructureV4.Entities;
using Greet.DataStructureV4.Interfaces;
using Greet.Gui;
using Greet.Gui.DataEditors.ProcessesEditors.StationaryProcessEditor;
using Greet.Model;
using Greet.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Greet.Plugins.EcoSpold01
{
    class EcoSpoldImport : APlugin
    {
        IGREETController _controller;
        DataEditor _dataEditorControl;

        public override bool InitializePlugin(IGREETController controler)
        {
            _controller = controler;
            Buisness.Controller = controler;
            Buisness.LoadRelationsFile();

            return true;
        }

        public override void onDataEditorInitialization(object sender)
        {
            _dataEditorControl = sender as DataEditor;
        }

        public override void onDataEditorMenuItemsInitialization(ToolStripItemCollection toolStripItemCollection)
        {
            foreach (ToolStripMenuItem tsi in toolStripItemCollection)
            {
                if (tsi.Text == "Processes")
                {
                    ToolStripItem item = new ToolStripMenuItem();
                    item.Text = "Import ILCD file";//"Import EcoSpoldV1";
                    item.Name = "Import ILCD file";//"Import EcoSpoldV1";
                    item.Click += new EventHandler(ImportMenuDataEditorClick);
                    tsi.DropDownItems.Add(item);
                }
            }
        }

        private void ImportMenuDataEditorClick(object sender, EventArgs e)
        {

            Stream myStream = null;

            string _allInfoInXML = "";


            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open an EcoSpoldV1 .xml file";
            //dialog.InitialDirectory = Holder.Paths.PathDefaultDataLocation.FullName;
            dialog.Filter = "xml files (*.xml)|*.xml";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = dialog.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // the path to the .xml file
                            string path = dialog.FileName;
                            // set up the filestream (READER)
                            FileStream READER = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            // Set up the XmlDocument fileInfo
                            XmlDocument fileInfo = new XmlDocument();
                            //Load the data from the file into the XmlDocument fileInfo
                            fileInfo.Load(READER);


                            //_allInfoInXML += fileInfo.DocumentElement.ChildNodes[0].Name + "\r\n";


                            //    for (int i = 0; i < fileInfo.DocumentElement.ChildNodes[0].Attributes.Count; i++)
                            //    {

                            //        _allInfoInXML += "   "+fileInfo.DocumentElement.ChildNodes[0].Attributes[i].Name + ": " + fileInfo.DocumentElement.ChildNodes[0].Attributes[i].Value + "\r\n";

                            //    }

                            _allInfoInXML = addChildNodeInfo(fileInfo.DocumentElement.ChildNodes[0], _allInfoInXML,0);

                            _allInfoInXML=_allInfoInXML.Replace("&#xA;","");






                            XmlNodeList NodeList = fileInfo.GetElementsByTagName("processInformation");


                            string text;
                            //text = NodeList[0].ChildNodes[0].ChildNodes[0].ChildNodes[1].Attributes.Count.FirstChild.Name;





                            // Insert code to read the stream here.
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
                // OpenProjectFile(dialog.FileName);
            }







            //Prompt the user to select a file (use OpenFileDialog)

            //Load the XML file in a structure using XmlDocument and XmlSchema

            //Iterate over the metadata of the file and read process name, notes and other things that we may want to put in the notes

            //Iterate over all the input flows and create stationary process inputs
            //Use the resource names and match with our names using FuzzyString or HammingDistance

            //Iterate over all the output flows
            //If they are resource, then create main output and co-products for the process
            //If they are emissions, then create 'other emissions' for the process

            //Show the process editor to the user with imported data and let him choose weather or not he wants to use this process


            //The code below shows a dummy example of how to create a process, add inputs, outputs and emissions
            //and show it in the editor. The values, ID, and everything are here to demonstrate how to use the greet API
            StationaryProcessEditorControl spe = null;
            if (_dataEditorControl != null)
            {
                spe = _dataEditorControl.showNewStationaryEditor(this as object);

                //Hardcoded crude oil resource ID, could be fetched from the IData class instead
                int CRUDE_RES_ID = 23;
                //Hardcoded mix for crude oil production, could be fectched from the IData class instead
                int CRUDE_FOR_US_MIX = 0;
                //Hardcoded pathway id for conventional crude recovery, could be fetched from the IData class instead
                int CRUDE_RECOVERY_PROCESS = 34;
                //Approximation of a BTU International in Joules
                double BTU = 1055.5;
                //Technology
                int TECH_BOIL = 230106;

                IDataHelper _dataHelper = _controller.CurrentProject.Data.Helper;

                //Creates an instance of a process
                IProcess process = _dataHelper.CreateNewProcess(0, "Example4 Stationary Process", _allInfoInXML);

                bool success = true;
                //Creates instances of inputs and outputs to be used in that stationary process
                IInput input = _dataHelper.CreateNewInput(CRUDE_RES_ID, BTU / 2, "joules", 3, CRUDE_FOR_US_MIX);
                //_dataHelper.InputAddTechnology(input, TECH_BOIL, 1);
                IInput input2 = _dataHelper.CreateNewInput(CRUDE_RES_ID, BTU / 2, "joules", 2, CRUDE_RECOVERY_PROCESS);
                IIO mainOutput = _dataHelper.CreateNewMainOutput(CRUDE_RES_ID, 1055, "joules");
                IIO coProduct = _dataHelper.CreateNewCoProduct(CRUDE_RES_ID, 0, "joules");
                success &= _dataHelper.ProcessAddOtherEmission(process, 1, 2.12);
                success &= _dataHelper.ProcessAddOtherEmission(process, 2, 3.14);
                success &= _dataHelper.ProcessAddOtherEmission(process, 3, 5.65);

                //Adds all inputs and outputs to the process
                success &= _dataHelper.ProcessAddInput(process, input, false);
                success &= _dataHelper.ProcessAddInput(process, input2, false);
                success &= _dataHelper.ProcessAddOrUpdateOutput(process, coProduct);
                success &= _dataHelper.ProcessAddOrUpdateOutput(process, mainOutput);

                spe.InitializeWithProcess(process as AProcess);


            }
        }

        private string addChildNodeInfo(XmlNode xmlNode, string allInfo, int level)
        {
            XmlNode xNode;
            XmlNodeList xNodeList;
            int xlevel;

            if (xmlNode.Attributes != null)
            {
                // Add child node name
                allInfo += "\r\n"+xmlNode.Name.PadLeft(xmlNode.Name.Length+level*5,' ');
                // Add child node attributes
                for (int i = 0; i < xmlNode.Attributes.Count; i++)
                    allInfo +="\r\n"+ (xmlNode.Attributes[i].Name + ": " + xmlNode.Attributes[i].Value).Replace("\n","").PadLeft((xmlNode.Attributes[i].Name + ": " + xmlNode.Attributes[i].Value).Replace("\n","").Length+(level+1)*5,' ') ;
            }
            else
                allInfo += ": "+xmlNode.OuterXml.Trim();

            // if the current node has children
            if (xmlNode.HasChildNodes)
            {
                xNodeList = xmlNode.ChildNodes;
                xlevel = level + 1;
                // loop through the child nodes
                for (int j = 0; j < xNodeList.Count; j++)
                {
                    xNode = xmlNode.ChildNodes[j];
                    allInfo = addChildNodeInfo(xNode, allInfo,xlevel);

                }
            }
            return allInfo;
        }




        public override string GetPluginName
        {
            get { return "ILCD Import"; }//"EcospoldV1 Import"; }
        }

        public override string GetPluginDescription
        {
            get { return "Import unit process into the GREET model"; }
        }

        public override string GetPluginVersion
        {
            get { return "1.0"; }
        }

        public override System.Drawing.Image GetPluginIcon
        {
            get { return null; }
        }

        public override System.Windows.Forms.ToolStripMenuItem[] GetMainMenuItems()
        {
            ToolStripMenuItem importILCD = new ToolStripMenuItem("Import ILCD Data");//("Import EcoSpold V1 Unit");
            importILCD.Click += importILCD_Click;
            return new ToolStripMenuItem[1] { importILCD };
        }

        void importILCD_Click(object sender, EventArgs e)
        {
            Form form = new Form();
            form.Text = "Select files to import into GREET";// as GREET processes";
            form.Icon = null;
            ILCD_FileSelector ilcdi = new ILCD_FileSelector(_controller);
            form.Controls.Add(ilcdi);
            ilcdi.Dock = DockStyle.Fill;
            form.Size = new System.Drawing.Size(700, 500);
            form.ShowDialog();
        }
    }
}
