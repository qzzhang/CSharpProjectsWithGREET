using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace readILCDs
{
    public partial class ReadXML : Form
    {
        #region constructor(s)
        public ReadXML()
        {
            InitializeComponent();
            PopulateTreeView();
            this.treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.listView1.MouseDoubleClick += new MouseEventHandler(this.listView1_MouseDoubleClick);
        }
        public const string resourceHeader = "NAME,AltNames,Notes,Families,Density,Density U,LHV,LHV U,HHV,HHV U,MarketVal,MarketVal U,Cratio(Mass),Sratio(Mass),IsPrimary,MemberGroups,OptionalID\n";
        public const string processHeader = "UnitProcess,Pathway,Stream,Quantiy,Unit,CoProdType,AllocationType,DisplacedPorM,DisplacedPorMId,DisplacementPorMShare,TechId,TechShare,flowNote\n";
        #endregion

        #region data mapping from xml to intermediate and then to GREET objects
        private void OpenILCDfile(string fnm)
        {
            string pfile = "";
            string rfile = "";

            //First, read the "dataset" of EcoSpold xml file into a string one at a time
            string xmlString = string.Empty;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (XmlReader reader = XmlReader.Create(fnm, settings))
            {
                // Moves the reader to the root element, i.e., <ecoSpold> in this case.
                reader.MoveToContent();

                // read all the inner xml including the markups
                xmlString += reader.ReadInnerXml();
            }
            //this.rTB_readResults.Text = fnm + ":\n" + xmlString;//for visually confirming what has been read

            //Second, read the xml into the corresponding object members.
            using (StringReader rdr = new StringReader(xmlString))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UnitProcess), "http://www.EcoInvent.org/EcoSpold01");
                try
                {
                    UnitProcess loadedProcess = (UnitProcess)serializer.Deserialize(rdr);

                    //read the xml info and create a list of record of [material_name, material_amount, unit]
                    string procName = loadedProcess.metaInformation.processInformation.referenceFunction.name;
                    string procCommt = loadedProcess.metaInformation.processInformation.referenceFunction.generalComment;
                    //write a csv file for each process
                    string cpath = Environment.CurrentDirectory;
                    string pdir = String.Concat(cpath, "\\process_csv\\");
                    // Create directory if it doesn't exist 
                    try
                    {
                        if (!Directory.Exists(pdir))
                            Directory.CreateDirectory(pdir);
                    }
                    catch
                    {
                        MessageBox.Show("Error writing csv data: No access!");
                    }

                    pfile = String.Concat(pdir, replaceSigns(procName) + ".csv");
                    rfile = String.Concat(pdir, replaceSigns(procName) + "_resources.csv");
                    // Delete the file if it exists.
                    if (File.Exists(pfile))
                        File.Delete(pfile);
                    if (File.Exists(rfile))
                        File.Delete(rfile);

                    procName = this.replaceNewLines(procName, "--");
                    procCommt = this.replaceNewLines(procCommt, "--");

                    string forWriteProcess = processHeader;
                    string forWriteResources = resourceHeader;
                    try
                    {
                        exchange[] fexs = loadedProcess.flowData.exchange;
                        //Create the file if it doesn't exist
                        foreach (exchange fex in fexs)
                        {
                            string mname = "";
                            string munit = "";
                            int inOrout = -1; //1 is output while -1 is input
                            double mamount = 0.0;
                            string mcommt = "";

                            if (fex.category != "" && fex.category != null)
                            {
                                mname = fex.category;
                                if (fex.subCategory != "" && fex.subCategory != null)
                                    mname += "->" + fex.subCategory;

                                mname += ":";
                            }
                            mname += fex.name;
                            mname = this.replaceNewLines(mname, "--");

                            munit = Regex.Replace(fex.unit, "(m|cm|mile|km|ft|inch|yard)(2|3)", "$1^$2");
                            mamount = fex.meanValue;
                            //assuming an elementary flow can be either input or output
                            inOrout = (fex.inputGroup != null) ? -1 : 1;
                            inOrout = (fex.outputGroup != null) ? 1 : -1;
                            mamount *= inOrout;
                            mcommt = fex.generalComment;
                            mcommt = this.replaceNewLines(mcommt, "--");
                            forWriteProcess += this.mustQuote(procName) + ",," + this.mustQuote(mname) + "," + mamount.ToString() + "," + munit + ",,,,,,," + "," + this.mustQuote(mcommt) + "\n";
                            forWriteResources += this.mustQuote(mname) + ",," + this.mustQuote(mcommt) + ",,,,,,,,,,,,,,\n";
                        }
                        forWriteProcess += "Process Note," + this.mustQuote(procCommt);
                        using (Stream fps = new FileStream(pfile, FileMode.Create, FileAccess.Write))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(forWriteProcess);
                            fps.Write(info, 0, info.Length);
                        }
                        using (Stream frs = new FileStream(rfile, FileMode.Create, FileAccess.Write))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(forWriteResources);
                            frs.Write(info, 0, info.Length);
                        }
                        //MessageBox.Show("File written to: " + pfile);
                    }
                    catch (Exception fwex)
                    {
                        MessageBox.Show(pfile + ":\n" + fwex.Message + "----explanation: " + fwex.InnerException.Message);
                    }
                }
                catch (Exception lex)
                {
                    MessageBox.Show(fnm + ":\n" + lex.Message + "----explanation: " + lex.InnerException.Message);
                }
            }
            //Open the stream and read it back.
            if (pfile != "")
            {
                string forReadProcess = "";
                var lines = File.ReadAllLines(pfile);
                foreach (var line in lines)
                {
                    forReadProcess += line;
                    string[] retStrAr = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                }
                //this.rTB_readResults.Text = pfile + ":\n" + forReadProcess;//for visually confirming what has been read
            }
        }
        private string replaceSigns(string src)
        {
            string retstr = "";
            if (src != null && src != "")
            {
                retstr = src.Replace(">", "&gt;");
                retstr = retstr.Replace("<", "&lt;");
                retstr = retstr.Replace("¢", "&cent;");
                retstr = retstr.Replace("£", "&pound;");
                retstr = retstr.Replace("¥", "&yen;");
                retstr = retstr.Replace("€", "&euro;");
                retstr = retstr.Replace("©", "&copy;");
                retstr = retstr.Replace("®", "&reg;");
                retstr = retstr.Replace("\"", "&quot;");
                retstr = retstr.Replace("'", "&quot;");
                retstr = retstr.Replace("`", "&apos;");
            }
            return retstr;
        }
        private string replaceNewLines(string src, string delmt)
        {
            string retstr = "";
            string rpl0 = "&#xD;&#xA;"; //\r\n
            string rpl1 = "&#xD;"; //\r
            string rpl2 = "&#xA;"; //\n
            string rpl3 = "&#xA;&#xD;"; //\n\r
            if (src != null && src != "")
            {
                retstr = src.Replace(rpl0, delmt);
                retstr = retstr.Replace(rpl3, delmt);
                retstr = retstr.Replace(rpl1, delmt);
                retstr = retstr.Replace(rpl2, delmt);
                retstr = retstr.Replace("\r\n", delmt);
                retstr = retstr.Replace("\n\r", delmt);
                retstr = retstr.Replace("\r", delmt);
                retstr = retstr.Replace("\n", delmt);
            }
            return retstr;
        }
        private string replaceComma(string src, string rpl)
        {
            string retstr = "";
            if (src != null && src != "")
                retstr = src.Replace(",", rpl);
            return retstr;
        }
        private string mustQuote(string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }
        private string putCommaBack(string src, string rpl)
        {
            string retstr = "";
            if (src != null && src != "")
                retstr = src.Replace(rpl, ",");
            return retstr;
        }
        #endregion

        #region organizing the xml files for reading
        private void PopulateTreeView()
        {
            TreeNode rootNode;
            string startPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Greet\";
            DirectoryInfo info = new DirectoryInfo(startPath);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                //rootNode = DirectoryToTreeView(null, @"../.."); //@"c:\temp");
                rootNode.Tag = info;
                this.FetchDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void FetchDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs != null && subSubDirs.Length != 0)
                {
                    this.FetchDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;
            /*ignore the sub directories
            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"), 
                     new ListViewItem.ListViewSubItem(item, dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            */
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                if (file.Extension == ".xml" || file.Extension == ".csv")
                {
                    item = new ListViewItem(file.Name, 1);
                    subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"), 
                     new ListViewItem.ListViewSubItem(item, file.LastAccessTime.ToShortDateString())};

                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                }
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            treeView1.SelectedNode = e.Node;
        }

        //Filter out any directory nodes that don't have any child nodes with a recursive call
        private TreeNode DirectoryToTreeView(TreeNode parentNode, string path, string extension = ".xml")
        {
            var result = new TreeNode(parentNode == null ? path : Path.GetFileName(path));
            foreach (var dir in Directory.GetDirectories(path))
            {
                TreeNode node = DirectoryToTreeView(result, dir);
                if (node.Nodes.Count > 0)
                {
                    result.Nodes.Add(node);
                }
            }
            foreach (var file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file).ToLower() == extension.ToLower())
                {
                    result.Nodes.Add(Path.GetFileName(file));
                }
            }
            return result;
        }
        private String GetSelectedItemFullPath(ListViewItem itm)
        {
            String path = String.Empty;

            // See if a node is selected in the TreeView
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null)
            {
                // Build the full path to the selected item.
                path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + treeView1.PathSeparator + selectedNode.FullPath + treeView1.PathSeparator + itm.Text;
            }

            return path;
        }

        #endregion

        #region event handlers
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CreateObjects();
        }
        private void btnCreateGREETobjs_Click(object sender, EventArgs e)
        {
            CreateObjects();
        }

        private void CreateObjects()
        {
            foreach (ListViewItem itm in this.listView1.SelectedItems)
            {
                if (itm != null)
                {
                    this.OpenILCDfile(GetSelectedItemFullPath(itm));
                }
            }
        }

        private void btnExitReadXML_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        #endregion
    }
}
