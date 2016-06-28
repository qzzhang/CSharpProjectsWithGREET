using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace readXMLs
{
    public partial class createCharts : Form
    {
        public createCharts()
        {
            InitializeComponent();
            PopulateTreeView();
            this.treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
        }

        private void CSVfile2Dashboard(string fnm, ListViewItem itm)
        {
            string htmlfile = "";
            string line;
            string[] colHeaders = null;
            string[] rowData;
            string dataString = "[";

            // Read the file line by line and create processes from flows
            using (Stream s = new FileStream(fnm, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader fileStream = new StreamReader(s))
                {
                    //clear the DatagridView and CheckListBox
                    this.rawDataDGV.Rows.Clear();
                    this.chkListBox_cols.Items.Clear();
                    int lineCount = 0;
                    while ((line = fileStream.ReadLine()) != null)
                    {
                        //read each line
                        lineCount++;
                        if (lineCount == 1)
                        {//read the column headers by splitting the CSV line using commas, yet keep the comma embedded in between double quotes
                            colHeaders = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                            for (int i = 0; i < colHeaders.Count(); i++)
                            {
                                this.chkListBox_cols.Items.Add(colHeaders[i]);
                                this.rawDataDGV.Columns.Add(colHeaders[i], colHeaders[i]);
                            }
                            dataString += "[";
                            foreach (string hd in colHeaders)
                            {
                                dataString += "'" + hd.Replace("\"", "") + "',";
                            }
                            dataString = dataString.TrimEnd(',');
                            dataString += "],";
                            continue;
                        }

                        //split the CSV line using commas, yet keep the comma embedded in between double quotes
                        rowData = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        this.rawDataDGV.Rows.Add(rowData);

                        dataString += "[";
                        //foreach (string item in rowData)
                        for (int j = 0; j < rowData.Count(); j++)
                        {
                            //if (this.chkListBox_cols.SelectedItems[i].ToString() == colHeaders[i].ToString())
                            foreach (int indexChecked in this.chkListBox_cols.CheckedIndices)
                            {
                                if (this.chkListBox_cols.GetItemCheckState(indexChecked) == CheckState.Checked 
                                    && this.chkListBox_cols.Items[indexChecked].ToString() == colHeaders[j].ToString())
                                {
                                    if (j == 0)
                                        dataString += "'" + rowData[j].Replace("\"", "") + "',";
                                    else
                                        dataString += rowData[j].Replace("\"", "") + ",";
                                }
                            }
                        }
                        dataString = dataString.TrimEnd(',');
                        dataString += "],";
                    }
                    //after reading all the lines, finish up the dataString for building the DataTable for Google charting purpose
                    dataString = dataString.TrimEnd(',');
                    dataString += "]";
                    /*
                    #region writing the html file
                    string cpath = Environment.CurrentDirectory;
                    string gcdir = String.Concat(cpath, "\\greet_chart_htmls\\");
                    // Create directory if it doesn't exist 
                    try
                    {
                        if (!Directory.Exists(gcdir))
                            Directory.CreateDirectory(gcdir);
                    }
                    catch
                    {
                        MessageBox.Show("Error writing charting data: No access!");
                    }

                    htmlfile = String.Concat(gcdir, itm.Text.Replace(".csv", "") + ".html");
                    // Delete the file if it exists.
                    if (File.Exists(htmlfile))
                        File.Delete(htmlfile);

                    string forWrite = this.writeHeadPortion();
                    forWrite += this.writeGoogleDashboardJS();
                    forWrite += this.writeDrawFunction();
                    forWrite += this.writeDataTable(dataString);
                    forWrite += this.writeGoogleSlider("slider1", "NumberRangeFilter", "slider1_div", 1, "Range Filter for table and bar chart");
                    forWrite += this.writeGoogleSlider("slider2", "NumberRangeFilter", "slider2_div", 2, "Range Filter for table and pie chart");
                    forWrite += this.writeGoogleStringfilter(colHeaders[0], "control3_div");
                    int[] colArr1 = new int[3] { 0, 1, 2 };
                    int[] colArr2 = new int[2] { 0, 2 };
                    forWrite += this.writeGoogleBar("bar", "ColumnChart", "barchart_div", "Emission,g/kg*km", colHeaders[0], colArr1);
                    forWrite += this.writeGooglePie("pie", "PieChart", "piechart_div", colHeaders[0], colArr2);
                    forWrite += this.writeGoogleTable("table", "table_div");
                    forWrite += this.writeGoogleFormmater();
                    forWrite += this.writeGoogleDashboardJSbind();
                    forWrite += this.writeHtmlBody();

                    //save the html file to disk
                    try
                    {
                        using (Stream fps = new FileStream(htmlfile, FileMode.Create, FileAccess.Write))
                        {
                            byte[] info = new UTF8Encoding(true).GetBytes(forWrite);
                            fps.Write(info, 0, info.Length);
                        }
                        MessageBox.Show("File written to: " + htmlfile);
                    }
                    catch (Exception fwex)
                    {
                        MessageBox.Show(htmlfile + ":\n" + fwex.Message + "----explanation: " + fwex.InnerException.Message);
                    }
                    #endregion
                    */
                }
            }
        }
        #region writing the html5 page portions
        private string writeHeadPortion()
        {
            string retStr = "";
            retStr +=
"<!DOCTYPE html>\n" +
"<html>\n" +
" <head>\n" +
"    <title>GREET Results with Google Charts</title>\n" +
"    <script type='text/javascript' src='https://www.google.com/jsapi'></script>\n" +
"    <script type='text/javascript' src='https://www.gstatic.com/charts/loader.js'></script>\n" +
"    <script type='text/javascript' src='https://google-developers.appspot.com/_static/a1c29df6f1/js/prettify-bundle.js'></script>\n" +
"    <script type='text/javascript' src='//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js'></script>\n" +
"    <script type='text/javascript' src='https://google-developers.appspot.com/_static/a1c29df6f1/js/jquery_ui-bundle.js'></script>\n" +
"    <script type='text/javascript' src='//www.google.com/jsapi?key=AIzaSyCZfHRnq7tigC-COeQRmoa9Cxr0vbrK6xw'></script>\n" +
"    <script type='text/javascript' src='https://google-developers.appspot.com/_static/a1c29df6f1/js/framebox.js'></script>\n" +
"    <link rel='stylesheet' href='//fonts.googleapis.com/css?family=Roboto:300,400,400italic,500,500italic,700,700italic|Roboto+Mono:400,700|Material+Icons'>\n" +
"    <link rel='stylesheet' href='https://google-developers.appspot.com/_static/a1c29df6f1/css/devsite-cyan.css'>\n"
;
            return retStr;
        }

        private string writeGoogleDashboardJS()
        {
            string retStr = "";
            retStr +=
"    <script type='text/javascript'>\n" +
"       google.charts.load('current', { 'packages':['corechart', 'table', 'gauge', 'controls']});\n" +
"       google.charts.setOnLoadCallback(drawMainDashboard);\n";
            return retStr;
        }

        private string writeDrawFunction()
        {
            string retStr = "";
            retStr +=
"       function drawMainDashboard() {\n" +
"         var dashboard = new google.visualization.Dashboard(document.getElementById('dashboard_div'));\n"
;
            return retStr;
        }

        private string writeGooglePie(string pieNm, string ctlType, string divId, string pieTitle, int[] cols)
        {
            string retStr = "";
            string colStr = "[";
            for (int i = 0; i < cols.Count(); i++)
                colStr += cols[i] + ",";
            colStr = colStr.TrimEnd(',') + "]";

            retStr +=
"         var " + pieNm + " = new google.visualization.ChartWrapper({\n" +
"         chartType: '" + ctlType + "',\n" +
"         containerId: '" + divId + "',\n" +
"         options: {\n" +
"            width: 300,\n" +
"            height: 300,\n" +
"            legend: 'left',\n" +
"            chartArea: {'left': 15, 'top': 15, 'right': 0, 'bottom': 0},\n" +
"            pieSliceText: 'label',\n" +
"            title: '" + pieTitle + "',\n" +
//"            is3D: true,\n" +//We cannot combine is3D and pieHole. If we do, the pieHole option will be ignored.
"            pieHole: 0.4 \n" +
"         },\n" +
"         view: {columns: " + colStr + "}\n" +
"     }); \n";
            return retStr;
        }

        private string writeGoogleBar(string barNm, string ctlType, string divId, string vAxisTitle, string hAxisTitle, int[] cols)
        {
            string retStr = "";
            string colStr = "[";
            for (int i = 0; i < cols.Count(); i++)
                colStr += cols[i] + ",";
            colStr = colStr.TrimEnd(',') + "]";

            retStr +=
"         var " + barNm + " = new google.visualization.ChartWrapper({\n" +
"         chartType: '" + ctlType + "',\n" +
"         containerId: '" + divId + "',\n" +
"         options: {\n" +
"            width: 300,\n" +
"            height: 300,\n" +
"            legend: 'bottom',\n" +
"            bar: {groupWidth: '95%'},\n" +
"            vAxis: {title:'" + vAxisTitle + "', gridlines: { count: 6 }, format: '0.###E+000'},\n" +
"            hAxis: {title:'" + hAxisTitle + "', minValue: 0, maxValue: 1}\n" +
"         },\n" +
"         view: {columns: " + colStr + "}\n" +
"     }); \n";
            return retStr;
        }

        private string writeGoogleTable(string tabNm, string divId)
        {
            string retStr = "";
            retStr +=
"         var " + tabNm + " = new google.visualization.ChartWrapper({\n" +
"         chartType: 'Table',\n" +
"         containerId: '" + divId + "',\n" +
"         options: {\n" +
"            width: 400\n" +
"         }\n" +
"     }); \n"
;
            return retStr;
        }

        private string writeGoogleSlider(string sldNm, string ctlType, string divId, int filterCol, string lblStr)// string opts)
        {
            string retStr = "";
            retStr +=
"         var " + sldNm + " = new google.visualization.ControlWrapper({\n" +
"         controlType: '" + ctlType + "',\n" + // NumberRangeFilter',\n" +
"         containerId: '" + divId + "',\n" + //slider1_div',\n" +
"         options: {\n" +
"            filterColumnIndex: " + filterCol + ",//select the data to filter by matching column index exactly\n" +
"            //showRangeValues: true,\n" +
"            //minValue: 0.0,\n" +
"            //maxValue: 1.0,\n" +
"            ui: {\n" +
"                labelStacking: 'vertical',\n" +
"                label: '" + lblStr + "',\n" +
"                unitIncrement: 0.1\n" +
"         }\n" +
"      }\n" +
"       //state: {lowValue: 0.005, highValue: 0.5}\n" +
"     });\n"
;
            return retStr;
        }

        private string writeGoogleStringfilter(string colHeader, string divId)
        {
            string retStr = "";
            retStr +=
"         var stringFilter = new google.visualization.ControlWrapper({\n" +
"         controlType: 'StringFilter',\n" +
"         containerId: '" + divId + "',\n" +
"         options: {\n" +
"            filterColumnLabel: '" + colHeader + "',//select the data to filter by matching column name exactly\n" +
"            ui: {labelStacking: 'vertical'}\n" +
"         }\n" +
"     });\n";
            return retStr;
        }

        private string writeDataTable(string dataStr)
        {
            string retStr = "";
            retStr +=
"         var data = google.visualization.arrayToDataTable(" + dataStr + ",false);// 'false' means that the first row contains labels, not data.\n"
;
            return retStr;
        }

        private string writeGoogleFormmater()
        {
            string retStr = "";
            retStr +=
"         var formatter = new google.visualization.NumberFormat({pattern: '0.###E+000'});\n" +
"         formatter.format(data, 1); // Apply formatter to second column\n" +
"         formatter.format(data, 2); // Apply formatter to second column\n"
;
            return retStr;
        }

        private string writeGoogleDashboardJSbind()
        {
            string retStr = "";
            retStr +=
"         dashboard.bind([stringFilter], [table]); //Bind the StringFilter to the Table if you want the StringFilter to control only the table.\n" +
"         dashboard.bind([slider1], [table, bar]); // Bind the NumberRangeFilter and CategoryFilter to the BarChart, PieChart and Table\n" +
"         dashboard.bind([slider2], [table, pie]); // dashboard.bind([slider1, slider1, slider2], [table, pie, bar])\n" +
"         dashboard.draw(data);//Draw the entire dashboard\n" +
"}\n" +
"</script>\n";
            return retStr;
        }

        private string writeHtmlBody()
        {
            string retStr = "";
            retStr +=
" </head>\n" +
" <body>\n" +
"   <div id='dashboard_div' style='border: 1px solid #ccc; margin-top: 1em'>\n" +
"    <p style='padding-left: 1em'><strong>Jet emissions</strong></p>\n" +
"    <table class='columns'>\n" +
"      <tr>\n" +
"         <td>\n" +
"           <div id='slider1_div' style='padding-left: 15px'></div><br />\n" +
"           <div id='control3_div' style='padding-left: 15px'></div>\n" +
"         </td><td>\n" +
"           <div id='slider2_div' style='padding-left: 15px'></div>\n" +
"         </td><td>\n" +
"           <div id='categoryPicker_div' style='padding-left: 15px'></div>\n" +
"         </td>\n" +
"      </tr><tr>\n" +
"         <td>\n" +
"           <div id='table_div' style='padding-top: 15px'></div>\n" +
"         </td><td>\n" +
"           <div id='barchart_div' style='padding-top: 15px'></div>\n" +
"         </td><td>\n" +
"           <div id='piechart_div' style='padding-top: 30px'></div>\n" +
"         </td>\n" +
"      </tr>\n" +
"    </table>\n" +
"   </div>\n" +
" </body>\n" +
"</html>"
;
            return retStr;
        }
        #endregion

        private void CreateDashboard()
        {
            foreach (ListViewItem itm in this.listView1.SelectedItems)
            {
                if (itm != null)
                {
                    this.CSVfile2Dashboard(GetSelectedItemFullPath(itm), itm);
                }
            }
        }

        private void btnChartDashboard_Click(object sender, EventArgs e)
        {
            this.CreateDashboard();
        }

        private void btnCloseCharting_Click(object sender, EventArgs e)
        {
            ReadXML frmReadXML = new ReadXML();
            frmReadXML.Show();
            this.Hide();
        }

        #region organizing the files for reading
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
    }
}
