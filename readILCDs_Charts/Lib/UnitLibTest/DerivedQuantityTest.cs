using System.Collections.Generic;
using System.Xml;
using Greet.UnitLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Greet.UnitLibTest
{
    
    
    /// <summary>
    ///This is a test class for DerivedQuantityTest and is intended
    ///to contain all DerivedQuantityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DerivedQuantityTest
    {


        private TestContext testContextInstance;
        Quantity ubg1, ubg2;
        private XmlDocument doc;
        Unit u, u1, u2, u3;
        private DerivedQuantity udg1;

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
            u2 = new Unit("defunit1", "ts1", 2, 3, "base1");
            u3 = new Unit("overrideunit1", "ts1", 2, 0, "base1");
            Units.UnitsList.Add("defunit", u);
            Units.UnitsList.Add("overrideunit", u1);
            Units.UnitsList.Add("defunit1", u2);
            Units.UnitsList.Add("overrideunit1", u3);
            ubg1 = new Quantity("base", "dispname", "0.000", "defunit", "overrideunit");
            ubg2 = new Quantity("base1", "dispname1", "0.000", "defunit1", "overrideunit1");
            Units.QuantityList.Add("base", ubg1);
            Units.QuantityList.Add("base1",ubg2);
            doc = new XmlDocument();
            udg1 = new DerivedQuantity(ubg1, ubg2, '/');
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            Units.UnitsList.Remove("defunit");
            Units.UnitsList.Remove("overrideunit");
            Units.UnitsList.Remove("defunit1");
            Units.UnitsList.Remove("overrideunit1");
            Units.QuantityList.Remove("base");
            Units.QuantityList.Remove("base1");
        }
        //
        #endregion


        /// <summary>
        ///A test for StringToBases
        ///</summary>
        [TestMethod()]
        public void StringToBasesTest()
        {
            string unitExpression = "gallons/miles";
            bool invert = false;
            List<DerivedQuantityBase> actual = udg1.StringToBases(unitExpression, invert);
            Assert.AreEqual("volume", actual[0].Quantity.ToString());
            Assert.AreEqual("distance", actual[1].Quantity.ToString());
       }
    }
}
