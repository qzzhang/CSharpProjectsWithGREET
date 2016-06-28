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

namespace CreateGoogleCharts
{
    public partial class createCharts : Form
    {
        private string[] checkedColumns;

        public createCharts()
        {
            InitializeComponent();
            PopulateTreeView();
            this.treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
        }
        private void LoadCSVData(string fnm, ListViewItem itm)
        {
            string line;
            string[] rowData;
            int lineCount = 0;
            string[] colHeaders = null;

            //clear the DatagridView and CheckListBox
            //this.rawDataDGV.Rows.Clear();
            //this.chkListBox_cols.Items.Clear();
            using (Stream s = new FileStream(fnm, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    while ((line = sr.ReadLine()) != null)
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
                            this.chkListBox_cols.SetItemCheckState(0, CheckState.Indeterminate);//set the first item to have an Intermediate CheckState
                            this.chkListBox_cols.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkListBoxItem_Check);
                            continue;
                        }

                        //split the CSV line using commas, yet keep the comma embedded in between double quotes
                        rowData = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                        this.rawDataDGV.Rows.Add(rowData);
                    }
                }
            }
        }
        private void chkListBoxItem_Check(object sender, ItemCheckEventArgs e)
        {
            if (e.CurrentValue == CheckState.Indeterminate)
            {//to ensure that the first item is always checked
                e.NewValue = CheckState.Indeterminate;//ignore any clicking on the item with an Intermediate CheckState
            }

            CheckedListBox clb = (CheckedListBox)sender;
            // Switch off event handler
            clb.ItemCheck -= chkListBoxItem_Check;
            clb.SetItemCheckState(e.Index, e.NewValue);
            // Switch on event handler
            clb.ItemCheck += chkListBoxItem_Check;

            // Now you can go further
            this.SetCheckedColumns();
        }

        private void SetCheckedColumns()
        {
            int i = 0;
            this.checkedColumns = new string[this.chkListBox_cols.CheckedItems.Count];
            foreach (string col in this.chkListBox_cols.CheckedItems)
            {
                this.checkedColumns[i] = col.ToString();
                i++;
            }
        }
        public string[] GetCheckedColumns()
        {
            return this.checkedColumns;
        }
        private string BuildDatastring()
        {
            string dString = "[";
            
            string[] colHeaders = new string[this.rawDataDGV.Columns.Count];
            int col_i = 0;
            foreach (DataGridViewColumn col in this.rawDataDGV.Columns)
            {
                colHeaders[col_i] = col.HeaderText;
                col_i++;
            }

            //First, fill the data string with the captions of the columns that are checked.
            dString += "[";
            foreach (string hd in this.GetCheckedColumns())
            {
                dString += "'" + hd.Replace("\"", "") + "',";
            }
            dString = dString.TrimEnd(',');
            dString += "],";

            //Second, fill the data string with the row data.
            for (int i = 0; i < this.rawDataDGV.RowCount; i++)
            {
                dString += "[";
                for (int j = 0; j < this.rawDataDGV.ColumnCount; j++)
                {
                    if (this.GetCheckedColumns().Contains(colHeaders[j]))
                    {
                        if (j == 0)
                            dString += "'" + this.rawDataDGV.Rows[i].Cells[j].Value.ToString().Replace("\"", "") + "',";
                        else
                            dString += this.rawDataDGV.Rows[i].Cells[j].Value.ToString().Replace("\"", "") + ",";
                    }
                }
                dString = dString.TrimEnd(',');
                dString += "],";
            }
            //after reading all the lines, finish up the dataString for building the DataTable for Google charting purpose
            dString = dString.TrimEnd(',');
            dString += "]";
            return dString;
        }

        private void WriteHTML(string dataString, string csvFN)
        {
            string htmlfile = "";
            string cpath = Environment.CurrentDirectory;
            string gcdir = String.Concat(cpath, "\\greet_chart_htmls\\");
            string colNM = this.chkListBox_cols.Items[0].ToString();

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

            htmlfile = String.Concat(gcdir, csvFN);
            // Delete the file if it exists.
            if (File.Exists(htmlfile))
                File.Delete(htmlfile);

            string forWrite = this.writeHeadPortion();
            forWrite += this.writeGoogleDashboardJS();
            forWrite += this.writeDrawFunction();
            forWrite += this.writeDataTable(dataString);

            for (int i = 1; i < this.GetCheckedColumns().GetLength(0); i++)
            {
                forWrite += this.writeGoogleSlider("slider" + i, "NumberRangeFilter", "slider_div" + i, i, "Range Filter for" + this.GetCheckedColumns()[i].ToString());
                int[] colArr = new int[2] { 0, i };
                forWrite += this.writeGoogleBar("bar" + i, "ColumnChart", "barchart_div" + i, "Emission,g/kg*km", colNM, colArr);
                forWrite += this.writeGooglePie("pie" + i, "PieChart", "piechart_div" + i, colNM, colArr);
                if (i == 1)
                {
                    forWrite += this.writeGoogleStringfilter(colNM, "stringFilter" + i, "stringFilter_div" + i);
                    forWrite += this.writeGoogleTable("table" + i, "table_div" + i);
                }
            }
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
        }

        private void CSVfile2Dashboard(string fnm, ListViewItem itm)
        {
            string dataString;

            //Read the data in the datagridview and checklistbox, build the data string for Google DataTable,
            dataString = this.BuildDatastring();

            //and then write the HTML5 file
            this.WriteHTML(dataString, itm.Text.Replace(".csv", "") + ".html");
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

        private string writeDataTable(string dataStr)
        {
            string retStr = "";
            retStr +=
"         var data = google.visualization.arrayToDataTable(" + dataStr + ",false);// 'false' means that the first row contains labels, not data.\n"
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
//"            hAxis: {title:'" + hAxisTitle + "', minValue: 0, maxValue: 1}\n" +
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

        private string writeGoogleStringfilter(string colHeader, string filtrNM, string divId)
        {
            string retStr = "";
            retStr +=
"         var " + filtrNM + " = new google.visualization.ControlWrapper({\n" +
"         controlType: 'StringFilter',\n" +
"         containerId: '" + divId + "',\n" +
"         options: {\n" +
"            filterColumnLabel: '" + colHeader + "',//select the data to filter by matching column name exactly\n" +
"            ui: {labelStacking: 'vertical'}\n" +
"         }\n" +
"     });\n";
            return retStr;
        }

        private string writeGoogleFormmater()
        {
            string retStr ="         var formatter = new google.visualization.NumberFormat({pattern: '0.###E+000'});\n";
            for (int i = 1; i < this.GetCheckedColumns().GetLength(0); i++)
                retStr += "         formatter.format(data, " + i + "); // Apply formatter to the column\n"; 
            return retStr;
        }

        private string writeGoogleDashboardJSbind()
        {
            string retStr = "";
            for (int i = 1; i < this.GetCheckedColumns().GetLength(0); i++)
            {
                if( i == 1)
                    retStr +=
"         dashboard.bind([stringFilter" + i + "], [table" + i + "]); //Bind the StringFilter to the Table if you want the StringFilter to control only the table.\n";
                retStr +=

"         dashboard.bind([slider" + i + "], [bar" + i + ", pie" + i + "]); // Bind the NumberRangeFilter and CategoryFilter to the BarChart, PieChart and Table\n";
            }
                retStr += "         dashboard.draw(data);//Draw the entire dashboard\n" +
    "}\n" +
    "</script>\n";
            return retStr;
        }

        private string writeHtmlBody()
        {
            string retStr =
" </head>\n" +
" <body>\n" +
"   <div id='dashboard_div' style='border: 1px solid #ccc; margin-top: 1em'>\n" +
"    <p style='padding-left: 1em'><strong>Jet emissions</strong></p>\n"; ;

            for (int i = 1; i < this.GetCheckedColumns().GetLength(0); i++)
            {
                retStr +=
"    <table class='columns'>\n" +
"      <tr>\n" +
"         <td>\n" +
"           <div id='stringFilter_div" + i + "' style='padding-left: 15px'></div>\n" +
"         </td><td>\n" +
"           <div id='slider_div" + i + "' style='padding-left: 15px'></div>\n" +
"         </td>\n" +
"      </tr><tr>\n" +
"         <td>\n" +
"           <div id='table_div" + i + "' style='padding-top: 15px;width: 400px'></div>\n" +
"         </td><td>\n" +
"           <div id='barchart_div" + i + "' style='padding-top: 15px'></div>\n" +
"         </td><td>\n" +
"           <div id='piechart_div" + i + "' style='padding-top: 30px'></div>\n" +
"         </td>\n" +
"      </tr>\n" +
"    </table>\n";
            }

    retStr +=
"   </div>\n" +
" </body>\n" +
"</html>"
;
            return retStr;
        }
        #endregion

        private void CreateDashboards()
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
            if (this.chkListBox_cols.CheckedItems.Count > 1)
            {
                if (this.rawDataDGV.RowCount > 0 && this.rawDataDGV.ColumnCount > 0)
                    this.CreateDashboards();
            }
            else
                MessageBox.Show("Please select at least one column for the data you want to plot!");
        }

        private void btnCloseCharting_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem itm in this.listView1.SelectedItems)
            {
                if (itm != null)
                {
                    this.LoadCSVData(GetSelectedItemFullPath(itm), itm);
                }
            }
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
