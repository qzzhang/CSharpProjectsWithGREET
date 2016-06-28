using System.IO;
using System.Xml;
using Greet.UnitLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Greet.UnitLibTest
{
    
    
    /// <summary>
    ///This is a test class for UnitTest and is intended
    ///to contain all UnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UnitTest
    {
        private TestContext testContextInstance;
        public Unit u1;
        public Unit u2;
        XmlDocument doc;
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
            u1 = new Unit("test", "ts", 2, 3, "base");
            doc = new XmlDocument();
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual("ts", u1.ToString());
        }


        /// <summary>
        ///A test for Unit Constructor
        ///</summary>
        [TestMethod()]
        public void UnitConstructorTest()
        {
            XmlNode node = u1.ToXmlNode(doc);
            Unit target = new Unit(node);
            Assert.AreEqual(target.Name, u1.Name);
            Assert.AreEqual(target.notes, u1.notes);
            Assert.AreEqual(target.prefixes, u1.prefixes);
            Assert.AreEqual(target.Si_slope, u1.Si_slope);
            Assert.AreEqual(target.Si_intercept, u1.Si_intercept);
        }

        /// <summary>
        ///A test for ToXmlNode
        ///</summary>
        [TestMethod()]
        public void ToXmlNodeTest()
        {
            XmlNode node = u1.ToXmlNode(doc);
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            node.WriteTo(xw);
            Assert.AreEqual(sw.ToString(), "<unit name=\"test\" display_name=\"\" abbrev=\"ts\" si_slope=\"2\" si_intercept=\"3\" fromDefault=\"\" toDefault=\"\" group=\"base\" customUnit=\"True\" />");
        }
    }
}
