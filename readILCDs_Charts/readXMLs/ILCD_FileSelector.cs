using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;

namespace readXMLs
{
    public partial class ILCD_FileSelector : UserControl
    {
        //IGREETController _controller;

        public ILCD_FileSelector()//(IGREETController controller)
        {
            InitializeComponent();
            PopulateTreeView();
            //_controller = controller;
            this.treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.listView1.MouseDoubleClick += new MouseEventHandler(this.listView1_MouseDoubleClick);
        }

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

        private void FetchDirectories(DirectoryInfo[] subDirs,TreeNode nodeToAddTo)
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

        void treeView1_NodeMouseClick(object sender,TreeNodeMouseClickEventArgs e)
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
                if (file.Extension == ".xml")
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
        private void CreateGREETObjects()
        {
            foreach (ListViewItem itm in this.listView1.SelectedItems)
            {
                if (itm != null)
                {
                    //MessageBox.Show(itm.Text.ToString());
                    this.OpenILCDfile(GetSelectedItemFullPath(itm));
                }
            }
        }

        private void OpenILCDfile(string fnm)
        {
            /*
            XmlSerializer serializer = new XmlSerializer(typeof(dataset));

            string xmlString = string.Empty;
            using (StreamReader loadStream = new StreamReader(fnm))
            {//read ONLY the legitimate portions
                xmlString = loadStream.ReadToEnd();
            }

            // if the serializer needs a TextReader then you can wrap it here
            StringReader strReader = new StringReader(xmlString);
            Datatable loadedObject = (Datatable)serializer.Deserialize(strReader);
            */
            
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (XmlReader reader = XmlReader.Create(fnm, settings))
            {
                //reader.MoveToContent();
                //reader.ReadStartElement(); 

                // Position the reader on the desired node
                reader.ReadToFollowing("dataset");
                //reader.Skip();

                // Create another reader that contains just the desired node.
                XmlReader inner = reader.ReadSubtree();

                inner.ReadToDescendant("processInformation");
                while (inner.Read() && inner.NodeType != XmlNodeType.EndElement)
                {
                    //this.tb_reads.Text += inner.ReadElementContentAsString();
                    this.tb_reads.Text += inner.NodeType + ":" + inner.Name;
                }
                //Console.WriteLine(inner.Name);

                // Do additional processing on the inner reader. After you 
                // are done, call Close on the inner reader and 
                // continue processing using the original reader.
                inner.Close();

                //reader.ReadEndElement();
            }
            

            /*var rootString = XElement.Parse(fnm).Element("dataset").ToString();
            using (StringReader rdr = new StringReader(rootString))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(metaInformation));
                dataset loadedObject = (dataset)serializer.Deserialize(rdr);
            }
             * */

            /*
            using (TextReader reader = File.OpenText(@"C:\perl.txt"))//The StreamReader class is derived from the TextReader class
            {
                char[] block = new char[3];
                char first = (char)reader.Peek();// Peek at first character in file with TextReader.
                string line = reader.ReadLine();// Read one line with TextReader.
                reader.ReadBlock(block, 0, 3);// Read three text characters with TextReader.
                string text = reader.ReadToEnd();// Read entire text file with TextReader.
                Console.WriteLine(block);
            }
            */
            /**The following portion works as well
            XmlSerializer serializer = new XmlSerializer(typeof(Datatable));
            using (FileStream loadStream = new FileStream(fnm, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Datatable loadedObject = (Datatable)serializer.Deserialize(loadStream);
                //ecoSpold loadedObject = (ecoSpold)serializer.Deserialize(loadStream);
            }
             * */
        }

        #region event handlers
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CreateGREETObjects();
        }
        private void btnCreateGREETobjs_Click(object sender, EventArgs e)
        {
            CreateGREETObjects();
        }
        #endregion
    }
}
