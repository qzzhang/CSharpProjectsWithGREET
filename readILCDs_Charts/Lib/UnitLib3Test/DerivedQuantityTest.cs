using Greet.UnitLib3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml;

namespace Greet.UnitLib3Test
{
    
    
    /// <summary>
    ///This is a test class for DerivedQuantityTest and is intended
    ///to contain all DerivedQuantityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DerivedQuantityTest
    {


        private TestContext testContextInstance;
        private XmlDocument doc;

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
            string result = string.Empty;
            using (Stream stream = typeof(GuiUtilsTest).Assembly.
                       GetManifestResourceStream("Greet.UnitLib3Test.data.xml"))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    this.doc = new XmlDocument();
                    doc.Load(sr);
                }
            }
            Greet.UnitLib3.Units.BuildContext(doc);
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
        ///A test for DerivedQuantity Constructor
        ///</summary>
        [TestMethod()]
        public void DerivedQuantityConstructorTest()
        {

            BaseQuantity top_ = Greet.UnitLib3.Units.QName2Q["energy"] as BaseQuantity;
            BaseQuantity bottom_ = Greet.UnitLib3.Units.QName2Q["mass"] as BaseQuantity;
            string symbol_ = "HV";
            DerivedQuantity target = new DerivedQuantity(top_, bottom_, symbol_, 0);
            Assert.AreEqual(DimensionUtils.FromMLT(0, 2, -2, 0), target.Dim);
        }
    }
}
