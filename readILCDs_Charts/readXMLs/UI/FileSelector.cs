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
using Greet.Model.Interfaces;
using Greet.DataStructureV4.Interfaces;
using Greet.DataStructureV4.Entities;
using Greet.Plugins.EcoSpold01.Entities;

namespace Greet.Plugins.EcoSpold01
{
    public partial class FileSelector : UserControl
    {
        IGREETController _controller;

        public FileSelector(IGREETController controller)
        {
            InitializeComponent();
            _controller = controller;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "XML Files (.xml)|*.xml|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            DialogResult result = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (result == DialogResult.OK)
            {
                // Open the selected file to read.
                List<UnitProcessFile> objects = new List<UnitProcessFile>();
                List<string> errors = new List<string>();
                foreach (string fileName in openFileDialog1.FileNames)
                {
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(fileName);

                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                        nsmgr.AddNamespace("es01", "http://www.EcoInvent.org/EcoSpold01");
                        XmlNode node = xmlDoc.SelectSingleNode("//es01:referenceFunction", nsmgr);
                        if (node != null && node.Attributes["name"] != null)
                        {
                            UnitProcessFile sv = new UnitProcessFile();
                            sv.Name = node.Attributes["name"].Value;
                            sv.FileName = fileName;
                            objects.Add(sv);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add("File: " + fileName + " ERROR: " + ex.Message);
                    }
                }
                this.dataGridView1.DataSource = objects;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<UnitProcessFile> files = new List<UnitProcessFile>();
            if (this.dataGridView1.DataSource == null)
            {
                MessageBox.Show("Please select files first using the Browse button");
                return;
            }
            files = this.dataGridView1.DataSource as List<UnitProcessFile>;
            Buisness.CreateProcesses(files);
        }
    }
}
