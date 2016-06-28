using Greet.UnitLib3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using System.IO;

namespace Greet.UnitLib3Test
{
    
    
    /// <summary>
    ///This is a test class for LightValueTest and is intended
    ///to contain all LightValueTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LightValueTest
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
        ///A test for op_Addition
        ///</summary>
        [TestMethod()]
        public void op_AdditionTest()
        {
            LightValue a = GuiUtils.CreateLightValue("10 Btu/lb");
            double b = 10;
            LightValue expected = GuiUtils.CreateLightValue("23270 J/kg");
            LightValue actual;
            actual = (a + b);
            Assert.AreEqual(expected.Value, actual.Value,1);
        }

        /// <summary>
        ///A test for op_Addition
        ///</summary>
        [TestMethod()]
        public void op_AdditionTest1()
        {
            LightValue a = GuiUtils.CreateLightValue("10 Btu/lb");
            LightValue b = GuiUtils.CreateLightValue("15 Btu/lb");
            LightValue expected = GuiUtils.CreateLightValue("25 Btu/lb");
            LightValue actual;
            actual = (a + b);
            Assert.AreEqual(expected.Value, actual.Value, 1);
            Assert.AreEqual(expected.Dim, actual.Dim);
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod()]
        public void op_DivisionTest()
        {
            LightValue a = GuiUtils.CreateLightValue("15 Btu/lb");
            LightValue b = GuiUtils.CreateLightValue("5 Btu/lb");
            LightValue expected = GuiUtils.CreateLightValue("3");
            LightValue actual;
            actual = (a / b);
            Assert.AreEqual(expected.Value, actual.Value, 1);
            Assert.AreEqual(expected.Dim, actual.Dim);
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod()]
        public void op_DivisionTest1()
        {
            LightValue a = GuiUtils.CreateLightValue("15 Btu/lb");
            double b = 5;
            LightValue expected = GuiUtils.CreateLightValue("3 Btu/lb");
            LightValue actual;
            actual = (a / b);
            Assert.AreEqual(expected.Value, actual.Value, 1);
            Assert.AreEqual(expected.Dim, actual.Dim);
        }

        /// <summary>
        ///A test for op_Division
        ///</summary>
        [TestMethod()]
        public void op_DivisionTest2()
        {
            LightValue a = GuiUtils.CreateLightValue("5 Btu/lb");
            double b = 15;
            LightValue expected = GuiUtils.CreateLightValue("3 lb/Btu");
            LightValue actual;
            actual = (b / a);
            Assert.AreEqual(expected.Value, actual.Value, 1);
            Assert.AreEqual(expected.Dim, actual.Dim);
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod()]
        public void op_MultiplyTest()
        {
            LightValue a = GuiUtils.CreateLightValue("15 Btu/lb");
            LightValue b = GuiUtils.CreateLightValue("3 lb/km");
            LightValue expected = GuiUtils.CreateLightValue("45 Btu/km");
            LightValue actual;
            actual = (a * b);
            Assert.AreEqual(expected.Value, actual.Value, 1);
            Assert.AreEqual(expected.Dim, actual.Dim);
        }

        /// <summary>
        ///A test for op_Multiply
        ///</summary>
        [TestMethod()]
        public void op_MultiplyTest1()
        {
            LightValue a = GuiUtils.CreateLightValue("15 Btu/lb");
            double b = 3.0;
            LightValue expected = GuiUtils.CreateLightValue("45 Btu/lb");
            LightValue actual;
            actual = (a * b);
            Assert.AreEqual(expected.Value, actual.Value, 1);
            Assert.AreEqual(expected.Dim, actual.Dim);
        }

        /// <summary>
        ///A test for op_Subtraction
        ///</summary>
        [TestMethod()]
        public void op_SubtractionTest()
        {
            LightValue a = GuiUtils.CreateLightValue("10 Btu/lb");
            LightValue b = GuiUtils.CreateLightValue("15 Btu/lb");
            LightValue expected = GuiUtils.CreateLightValue("-5 Btu/lb");
            LightValue actual;
            actual = (a - b);
            Assert.AreEqual(expected.Value, actual.Value, 1);
            Assert.AreEqual(expected.Dim, actual.Dim);
        }

        /// <summary>
        ///A test for op_UnaryNegation
        ///</summary>
        [TestMethod()]
        public void op_UnaryNegationTest()
        {
            LightValue a = GuiUtils.CreateLightValue("10 Btu/lb");
            LightValue expected = GuiUtils.CreateLightValue("-10 Btu/lb");
            LightValue actual;
            actual = -(a);
            Assert.AreEqual(expected.Value, actual.Value);
            Assert.AreEqual(expected.Dim, actual.Dim);
        }
    }
}
