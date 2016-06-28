using System.IO;
using System.Xml;
using Greet.UnitLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Greet.UnitLibTest
{
    
    
    /// <summary>
    ///This is a test class for UnitBaseGroupTest and is intended
    ///to contain all UnitBaseGroupTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UnitBaseGroupTest
    {


        private TestContext testContextInstance;
        Quantity ubg;
        private XmlDocument doc;
        Unit u, u1;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            u = new Unit("defunit", "ts", 2, 3, "base");
            u1 = new Unit("overrideunit", "ts", 5, 0, "base");
            Units.UnitsList.Add("defunit", u);
            Units.UnitsList.Add("overrideunit", u1);
            ubg = new Quantity("basegroup", "dispname", "0.000", "defunit", "overrideunit");
            doc = new XmlDocument();

        }
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Units.UnitsList.Clear();
        }
        //
        #endregion


        /// <summary>
        ///A test for ToXmlNode
        ///</summary>
        [TestMethod()]
        public void ToXmlNodeTest()
        {
            XmlNode node = ubg.ToXmlNode(doc);
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            node.WriteTo(xw);
            Assert.AreEqual(sw.ToString(), "<group name=\"basegroup\" display_name=\"dispname\" format=\"0.000\" unit=\"defunit:overrideunit\" />");
        }

        /// <summary>
        ///A test for ConvertFromOverrideToDefault
        ///</summary>
        [TestMethod()]
        public void ConvertFromOverrideToDefaultTest()
        {
            double valueToConvert = 15; 
            double expected = 36; //15 -> 15*5 = 75 75 -> (75-3)/2 = 36
            double actual;
            actual = ubg.ConvertFromOverrideToDefault(valueToConvert);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ConvertFromDefaultToSpecific
        ///</summary>
        [TestMethod()]
        public void ConvertFromDefaultToSpecificTest()
        {
            double valueToConvert = 36; 
            string unit = "overrideunit";
            double expected = 15;
            double actual;
            actual = ubg.ConvertFromDefaultToSpecific(valueToConvert, unit);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ConvertFromDefaultToOverride
        ///</summary>
        [TestMethod()]
        public void ConvertFromDefaultToOverrideTest()
        {
            double valueToConvert = 36;
            double expected = 15;
            double actual;
            actual = ubg.ConvertFromDefaultToOverride(valueToConvert);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UnitBaseGroup Constructor
        ///</summary>
        [TestMethod()]
        public void UnitBaseGroupConstructorTest1()
        {
            XmlNode node = ubg.ToXmlNode(doc);
            Quantity target = new Quantity(node);
            Assert.AreEqual(ubg.Name, target.Name);
        }
    }
}
