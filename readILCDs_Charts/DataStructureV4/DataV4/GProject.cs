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
using System.IO;
using System.Text;
using System.Xml;
using Greet.ConvenienceLib;
using Greet.DataStructureV4.Interfaces;
using Greet.LoggerLib;
using System.Threading;

namespace Greet.DataStructureV4
{
    [Serializable]
    public class GProject : IProject
    {
        #region attributes

        private GData data;
        private MonitorValues monitorList = new MonitorValues();
        private bool someValuesHaveBeenUpdatedSinceProjectSaved = false;
        private XmlDocument LoadedDocument = null;

        public FileInfo fileInformation = null;
        public BasicParameters basicParameters = new BasicParameters();
        public XmlNode PluginsNode = null;

        /// <summary>
        /// For the Main Form, everytime the SomeValuesHaveBeenUpdatedSinceProjectSaved accessor is set
        /// we display a little red label on the bottom left of the main form indicating that we need to save
        /// </summary>
        /// <param name="changed"></param>
        public delegate void ValueHasChanged(bool changed);
        [field: NonSerialized]
        public ValueHasChanged ValueHasChangedEvent;

        public delegate void DisplayPreferencesChanged();
        [field: NonSerialized]
        public DisplayPreferencesChanged DisplayPreferencesChangedEvent;

        private int updateCount = 0;

        public int version = -1;

        /// <summary>
        ///  0 : everything is fine
        ///  1 : data is too old and cannot be loaded
        ///  2 : Corrupted Xml structure and cannot be loaded.
        /// </summary>
        public int loadingResult = 0;

        /// <summary>
        /// true: Data is currently be loaded. Not all data has been loaded at this point. 
        /// </summary>
        public bool isLoading = false;
        #endregion attributes

        #region accessors

        /// <summary>
        /// A Collection of the monitor values for fuels in WTP
        /// </summary>
        public MonitorValues MonitorList
        {
            get { return monitorList; }
            set
            {
                monitorList = value;
                if (this.monitorList != null)
                {
                    foreach (Monitor mnt in monitorList.Values)
                    {
                        mnt.CalculationResultsValues.Clear();
                    }
                }
            }
        }

        public List<IMonitor> MonitorValues
        {
            get 
            {
                List<IMonitor> mntVals = new List<IMonitor>();
                foreach (AMonitor mnt in MonitorList.Values)
                    mntVals.Add(mnt);
                return mntVals;
            }
        }

        public GData Dataset
        {
            get { return data; }
            private set { data = value as GData; }
        }

        public IData Data
        {
            get { return data; }
            set { data = value as GData; }
        }

        public BasicParameters BasicParameters
        {
            get { return basicParameters; }
            set { basicParameters = value; }
        }

        public bool SomeValuesHaveBeenUpdatedSinceProjectSaved
        {
            get { return someValuesHaveBeenUpdatedSinceProjectSaved; }
            set
            {
                someValuesHaveBeenUpdatedSinceProjectSaved = value;
                if (this.ValueHasChangedEvent != null)
                    ValueHasChangedEvent(value);
            }
        }

        public int UpdateCount
        {
            get { return updateCount; }
            set { updateCount = value; }
        }


        public int Version
        {
            get { return this.version; }
            set { this.version = value; }
        }


        #endregion accessors

        #region constructors

        /// <summary>
        /// Creates an empty project, used if nothing else is loaded over
        /// </summary>
        /// <param name="projectFileName"></param>
        public GProject()
        {
            this.Dataset = new GData();
        }

        /// <summary>
        /// Loads the data from a file name after the class have been instantiated
        /// Developer Note: This method should only be used by the execution service, if needed load from execution service and not from here
        /// otherwise preprocessing will be missing as well as calls to the plugins
        /// </summary>
        /// <param name="projectFileName"></param>
        public void Load(string projectFileName)
        {
            string status = "";

            XmlDocument projectDoc = new XmlDocument();

            this.fileInformation = new FileInfo(projectFileName);

            try
            {
                this.clearData();
                status = "Reading project file";

                try
                {//try to open as a regular XML file
                    //we could try to validate the XML document if we had an schematic for it
                    //but for now the try catch is good enough
                    projectDoc.Load(projectFileName);
                }
                catch
                {//try to open a compressed stream
                    FileStream fs = new FileStream(projectFileName, FileMode.Open);

                    MemoryStream readableStream = new MemoryStream();
                    CompDecomp.Decompress(fs, readableStream);

                    //Saving the decompressed memory stream to a file then open the file apprears to work fine
                    readableStream.Position = 0;
                    projectDoc.Load(readableStream);
                    readableStream.Close();
                }

                status = "reading the project version";
                if (projectDoc.SelectSingleNode("greet") != null
                    && projectDoc.SelectSingleNode("greet").Attributes["version"] != null)
                    this.version = Convert.ToInt32(projectDoc.SelectSingleNode("greet").Attributes["version"].Value);
                
                if (this.version < 7783)
                {
                    this.loadingResult = 1;
                }
                else
                {
                    //updating version from SVN revision if possible
                    if (projectDoc.SelectSingleNode("greet") != null
                        && projectDoc.SelectSingleNode("greet").Attributes["rev"] != null)
                    {
                        string revValue = projectDoc.SelectSingleNode("greet").Attributes["rev"].Value;
                        revValue = revValue.Replace("$Rev: ", "").Replace("$","").TrimEnd();
                        int temp;
                        if (int.TryParse(revValue, out temp))
                        {
                            this.version = temp;
                        }
                    }

                    status = "reading pictures";
                    this.Dataset.PicturesData = new Pictures(data, projectDoc.SelectSingleNode("greet/pictures"), "");

                    status = "reading data";
                    this.Dataset.ReadDb(projectDoc.SelectSingleNode("greet/data"), projectFileName);

                    status = "reading basic parameters";
                    this.basicParameters = new BasicParameters(data, projectDoc.SelectSingleNode("greet/basic_parameters"));

                    status = "reading monitored items";
                    XmlNode monitored_node = projectDoc.SelectSingleNode("greet/monitored");
                    if (monitored_node != null)
                        this.MonitorList = new MonitorValues(data, monitored_node);
                    else
                        this.MonitorList = new MonitorValues();

                    this.PluginsNode = projectDoc.SelectSingleNode("greet/plugins");
                }

                fileInformation = new FileInfo(projectFileName);
                this.LoadedDocument = projectDoc;

            }
            catch (Exception e)
            {
                this.loadingResult = 2;
                LogFile.Write("Project file error \r\n" + status + "\r\n");
                throw e;
            }

        }

        private void clearData()
        {
            this.loadingResult = 0;

            data.InputsData.Clear();
            data.GasesData.Clear();
            data.GasesData.Groups.Clear();
            data.ResourcesData.Clear();
            data.ResourcesData.Groups.Clear();
            data.LocationsData.Clear();
            data.LocationsData.groups.Clear();
            data.ModesData.Clear();
            data.TechnologiesData.Clear();
            data.ProcessesData.Clear();
            data.PathwaysData.Clear();
            data.VehiclesData.Clear();
        }

        #endregion constructors

        #region methods

        /// <summary>
        /// This methods acts kind of the same way as  the save function, however it does the job in another folder
        /// so the current copy of the project and the datafiles are not overwritten by the autosaves.
        /// When a Save() or SaveAs() is called the autosaves are deleted if the saving was done correctly.
        /// </summary>
        public void AutoSave(bool compress = true)
        {
            bool saved = this.SaveProjectAs(
                //10/06/2014 CAB: Added 'Temp' folder name to the path
                this.fileInformation.DirectoryName + @"\Temp\" + this.fileInformation.Name.Replace(".greet", "") + "-autosave.greet"
                , true
                , compress);
        }

        /// <summary>
        /// Deletes the autosaved file, this shouldbe exectued after the user saved his project and the saving method retured true
        /// </summary>
        public void DeleteAutosaves()
        {
            if (this.fileInformation != null
                && this.fileInformation.DirectoryName != null)
            {
                //10/06/2014 CAB: Added 'Temp' folder name to the path
                string FilePath = this.fileInformation.DirectoryName + @"\Temp\" + this.fileInformation.Name.Replace(".greet", "") + "-autosave.greet";
                //string FilePath = this.fileInformation.DirectoryName + @"\" + this.fileInformation.Name.Replace(".greet", "") + "-autosave.greet";

                if (File.Exists(FilePath))
                    File.Delete(FilePath);
            }
        }

        public bool Save(bool compress)
        {
            bool saveResult = this.SaveProjectAs(this.fileInformation.FullName, false, compress);
            if (saveResult)
                this.DeleteAutosaves();
            return saveResult;
        }

        /// <summary>
        /// Saves the project file and the associated data in a subfolder created with the same name as the project
        /// </summary>
        /// <param name="fileName">The FileInfo of the project file, defines the location and name where the project will be saved</param>
        /// <param name="autoSaving">If true the Working File location is unchanged as it is saving to Autosave location</param>
        /// <returns>Returns true if the project have been saved correctly, false if there was an issue</returns>
        public bool SaveProjectAs(string fileName, bool autoSaving = false, bool compress = true)
        {
            try
            {
                FileInfo saveFileInformation;
                if (autoSaving)
                    saveFileInformation = new FileInfo(fileName);
                else
                {
                    this.fileInformation = new FileInfo(fileName);
                    saveFileInformation = this.fileInformation;
                }
                string directoryName = saveFileInformation.DirectoryName;
                DirectoryInfo dataFolder = new DirectoryInfo(directoryName + "\\" + saveFileInformation.Name);
                if (dataFolder.Exists == false)
                    FilesFolders.CreateFolder(directoryName);

                XmlDocument doc = new XmlDocument();
                DateTime date = DateTime.Now;

                XmlNode greetNode = doc.CreateNode("greet", doc.CreateAttr("version", this.version), doc.CreateAttr("rev", "$Rev$"));
                doc.AppendChild(greetNode);

                XmlNode baseParamNode = doc.CreateNode("basic_parameters", doc.CreateAttr("year_selected", BasicParameters.SelectedYear), doc.CreateAttr("lhv", BasicParameters.UseLhv));
                greetNode.AppendChild(baseParamNode);

                XmlNode monitored = this.MonitorList.ToXmlNode(doc);
                greetNode.AppendChild(monitored);

                XmlNode dataNode = this.Dataset.ToXmlNode(doc, true);
                greetNode.AppendChild(dataNode);

                XmlNode pictureNode = this.Dataset.PicturesData.ToXmlNode(doc);
                greetNode.AppendChild(pictureNode);

                if (this.PluginsNode != null)
                {
                    XmlNode pluginsNode = doc.CreateNode("plugins");
                    pluginsNode.InnerXml = this.PluginsNode.InnerXml;
                    greetNode.AppendChild(pluginsNode);
                }

                MemoryStream savingStream = new MemoryStream();
                if (compress)
                {
                    MemoryStream dataStream = new MemoryStream();
                    doc.Save(dataStream);
                    CompDecomp.Compress(dataStream, savingStream);
                }
                else
                {
                    doc.Save(savingStream);

                    byte[] jsonBytes = new byte[savingStream.Length];
                    savingStream.Read(jsonBytes, 0, (int)savingStream.Length);
                    string jsonString = Encoding.Default.GetString(jsonBytes);
                }

                //doc.Save(fileName);
                FileStream fs = new FileStream(fileName, FileMode.Create);
                savingStream.Position = 0;
                savingStream.WriteTo(fs);
                fs.Close();
                savingStream.Close();

                int safetyInt = 100;
                while (true && safetyInt > 0)
                {
                    safetyInt -= 1;

                    try
                    {
                        // Attempt to open the file exclusively.
                        using (FileStream ffs = new FileStream(fileName,
                            FileMode.Open, FileAccess.ReadWrite,
                            FileShare.None, 100))
                        {
                            ffs.ReadByte();

                            // If we got this far the file is ready
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogFile.Write("Error 463:" + ex.Message);
                        Thread.Sleep(100);
                    }
                }

                SomeValuesHaveBeenUpdatedSinceProjectSaved = false;

                return true;
            }
            catch (Exception e)
            {
                LogFile.Write(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Allows the user to push a node to the PluginsNode collection.
        /// This node will be saved when the project is saved
        /// </summary>
        /// <param name="pluginName">A plugin instance</param>
        /// <param name="node">The node that we want to push to the database</param>
        /// <returns><para>-1 if the name of the node, 0 if the node has been inserted, +1 if the node replaces an existing one</para>
        /// <para>May throw an exceltion if the XmlNode is not created using the same Owner document or contains XMLDeclarations that should no be inserted, see Inner Exception</para>
        /// </returns>
        public int PushPluginXML(string pluginName, XmlNode node)
        {
            int retVal = 0;

            if (this.PluginsNode == null)
                this.PluginsNode = this.LoadedDocument.CreateNode("plugins");

            if (node.Name != pluginName)
            {
                retVal = -1; //XML Node must be named as the plugin is
                return retVal;
            }
            else
            {
                if (this.PluginsNode[node.Name] != null)
                {
                    if (this.PluginsNode[node.Name].OuterXml != node.OuterXml)
                        this.someValuesHaveBeenUpdatedSinceProjectSaved |= true;
                    this.PluginsNode.RemoveChild(this.PluginsNode[node.Name]);
                    retVal = 1;
                    XmlNode toBeInserted = this.LoadedDocument.CreateNode(pluginName);
                    toBeInserted.InnerXml = node.InnerXml;
                    this.PluginsNode.AppendChild(toBeInserted);
                }
                else
                {
                    XmlNode toBeInserted = this.LoadedDocument.CreateNode(pluginName);
                    toBeInserted.InnerXml = node.InnerXml;
                    this.PluginsNode.AppendChild(toBeInserted);
                    this.someValuesHaveBeenUpdatedSinceProjectSaved |= true;
                }

                return retVal;
            }
        }

        /// <summary>
        /// Returns the XmlDocument loaded for this project. Changes to it will not affect the saved data
        /// </summary>
        /// <returns>XmlDocument loaded</returns>
        public XmlDocument GetProjectDocument()
        {
            return this.LoadedDocument;
        }

        /// <summary>
        /// Returns existing XmlNode or a new one for the plugin desiring it.
        /// Modifications to this object will not be saved
        /// </summary>
        /// <param name="pluginName">A plugin instance</param>
        /// <returns>XMLNode for the plugin</returns>
        public XmlNode GetPluginXML (string pluginName)
        {
            if (this.PluginsNode == null && this.LoadedDocument != null)
                this.PluginsNode = this.LoadedDocument.CreateNode("plugins");
            if(this.PluginsNode != null && this.PluginsNode[pluginName] == null && this.LoadedDocument != null)
                this.PluginsNode.AppendChild(this.LoadedDocument.CreateNode(pluginName));
            if(this.PluginsNode != null && this.PluginsNode[pluginName] != null)
                return this.PluginsNode[pluginName].Clone();
            else
                return null;
        }

        #endregion
    }
}
