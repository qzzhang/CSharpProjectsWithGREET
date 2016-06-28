using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Greet.ConvenienceLib;
using System.Reflection;
using System.Globalization;

namespace Greet.UnitLib3
{
    public static class Units
    {
        public static Dictionary<string, Unit> UnitsList;
        /// <summary>
        /// Maps old group name to the dimenstion represented by uint in a string
        /// </summary>
        public static Q2Dim OLDGroup2Dims;
        public static Dim2Q Dim2OldGroup;
        public static Quantities QuantityList;

        #region datacontext
        internal static Dictionary<uint, List<AQuantity>> Dim2Quantities = new Dictionary<uint, List<AQuantity>>();
        //low case unit expression --> Quantity
        internal static Dictionary<string, AQuantity> UELow2Q = new Dictionary<string, AQuantity>();
        internal static Dictionary<Unit, AQuantity> U2Q = new Dictionary<Unit, AQuantity>();
        internal static Dictionary<string, AQuantity> QName2Q = new Dictionary<string, AQuantity>();
        internal static List<AQuantity> Q = new List<AQuantity>();
        /// <summary>
        /// low case unit formula --> Unit
        /// </summary>
        internal static Dictionary<string, Unit> UELow2U = new Dictionary<string, Unit>();
        //low case unit expression --> index in quantity list
        internal static Dictionary<string, int> UELow2Ind = new Dictionary<string, int>();
        /// <summary>
        /// US Culture info to use whenever loading saving using the Convert.ToDouble or ToString methods to data files
        /// </summary>
        public static CultureInfo USCI = new CultureInfo("en-US");
        #endregion

        #region constructors
        //need to populate all of the look up dictionaries
        internal static void RegisterQuantity(AQuantity q)
        {
            if (!Dim2Quantities.ContainsKey(q.Dim))
                Dim2Quantities[q.Dim] = new List<AQuantity>();
            Dim2Quantities[q.Dim].Add(q);
            for (int i = 0; i < q.Units.Count; i++)
            {
                Unit u = q.Units[i];
                UELow2U.Add(u.Expression, u);
                UELow2Q.Add(u.Expression, q);
                UELow2Ind.Add(u.Expression, i);
                U2Q.Add(u, q);
            }
            QName2Q.Add(q.Name, q);
            Q.Add(q);
        }

        /// <summary>
        /// Loads the definitions from the units, quantities and conversion formulas from the data file
        /// </summary>
        public static void BuildContext()
        {
            XmlDocument doc = new XmlDocument();

            string path = Path.Combine(AssemblyDirectory, "unitsdata.xml");
            if(!File.Exists(path)){
                //The following line of code is a neat way to read a file that is embeded as a resource to the compiled libraray, for reference see http://msdn.microsoft.com/en-us/library/xc4235zt(v=vs.110).aspx
                using (Stream stream = typeof(Greet.UnitLib3.Units).Assembly.
                           GetManifestResourceStream(typeof(Units).Namespace + ".unitsdata.xml"))
                {
                    if (stream != null)
                    {
                        using (Stream output = File.Create(path))
                        {
                            CopyStream(stream, output);
                        }
                    }
                }
            }

            doc.Load(path);
              
            BuildContext(doc);
            ConversionFromOLDUnitLib.BuildConversionContext(doc);//Old UnitLib
            
            
            InitUnitLibObjects();
        }
        
        private static void CopyStream(Stream input, Stream output)
        {
            // Insert null checking here for production
            byte[] buffer = new byte[8192];

            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static void SaveContext()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode headerNode = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(headerNode);
            XmlNode root = xmlDoc.CreateNode("root");
            xmlDoc.AppendChild(root);
            foreach (AQuantity qty in QuantityList.Values)
            {
                XmlNode qtyNode = qty.ToXML(xmlDoc);
                root.AppendChild(qtyNode);
            }

            XmlNode oldNode = ConversionFromOLDUnitLib.SaveConversionContext(xmlDoc);
            root.AppendChild(oldNode);

            string path = Path.Combine(AssemblyDirectory, "unitsdata.xml");
            xmlDoc.Save(path);
        }

        public static void BuildContext(XmlDocument doc)
        {
            Dim2Quantities.Clear();
            UELow2Q.Clear();
            QName2Q.Clear();
            Q.Clear();
            UELow2U.Clear();
            UELow2Ind.Clear();
            U2Q.Clear();
            AQuantity q;
            foreach (XmlNode qnode in doc.GetElementsByTagName("quantity"))
            {
                q = new BaseQuantity(qnode);
                RegisterQuantity(q);
            }
            foreach (XmlNode qnode in doc.GetElementsByTagName("derived_quantity"))
            {
                q = new DerivedQuantity(qnode);
                RegisterQuantity(q);
            }

        }
        #endregion

        #region UnitLib
        public static void InitUnitLibObjects()
        {
            Units.QuantityList = new Quantities();
            Units.OLDGroup2Dims = new Q2Dim();
            Units.Dim2OldGroup = new UnitLib3.Dim2Q();
            Units.UnitsList = new Dictionary<string, Unit>();
            AQuantity qq;
            foreach (string g in ConversionFromOLDUnitLib.OLDGroupName2NEWQuantityName.Keys)
            {
                qq = QName2Q[ConversionFromOLDUnitLib.OLDGroupName2NEWQuantityName[g]];
                Units.OLDGroup2Dims.Add(g, qq.Dim.ToString());
            }
            foreach (var q in Q)
            {
                if (!Units.QuantityList.ContainsKey(q.Name))
                    Units.QuantityList.Add(q.Name, q);
                foreach (Unit u in q.Units)
                    Units.UnitsList.Add(u.Name, u);
            }
        }
        #endregion

        public static List<Unit> UnitsFromQName(string quantity_name)
        {
            return QName2Q[quantity_name].Units;
        }
    }
}
