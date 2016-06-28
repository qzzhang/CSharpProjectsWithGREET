using Greet.UnitLib3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Greet.UnitLib3Test
{
    
    
    /// <summary>
    ///This is a test class for GuiUtilsTest and is intended
    ///to contain all GuiUtilsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GuiUtilsTest
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
        #endregion

        /// <summary>
        ///A test for Convert
        ///</summary>
        [TestMethod()]
        public void ConvertFromSITest()
        {
            string to = "Btu lb^(-1)";
            double value = 1000;
            double expected = 0.429922614;
            double actual;
            actual = GuiUtils.ConvertFromSI(to, value);
            Assert.AreEqual(expected, actual, 0.01);
        }
        [TestMethod()]
        public void ConvertFromSITest1()
        {
            string to = "Btu/lb^(1)";
            double value = 1000;
            double expected = 0.429922614;
            double actual;
            actual = GuiUtils.ConvertFromSI(to, value);
            Assert.AreEqual(expected, actual, 0.01);
        }
        [TestMethod()]
        [ExpectedException(typeof(ExpressionStringParingException))]
        public void ConvertFromSITest2()
        {

            string to = "Btu/bababa";
            double value = 1000;
            double actual;
            actual = GuiUtils.ConvertFromSI(to, value);
            //Assert.AreEqual(963, actual, 0.5);
        }

        /// <summary>
        ///A test for ConvertToSI
        ///</summary>
        [TestMethod()]
        public void ConvertToSITest()
        {
            string from = "Btu/lgtn";
            double value = 963;
            double expected = 1000;
            double actual;
            actual = GuiUtils.ConvertToSI(from, value);
            Assert.AreEqual(expected, actual,0.2);
        }

        /// <summary>
        ///A test for CreateDim
        ///</summary>
        [TestMethod()]
        public void CreateDimTest()
        {
            string p = "Btu/lgtn";
            uint expected = UnitLib3.DimensionUtils.FromMLT(0, 2, -2);
            uint actual;
            actual = GuiUtils.CreateDim(p);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CreateLightValue
        ///</summary>
        [TestMethod()]
        public void CreateLightValueTest()
        {
            double val = 10;
            string units_formula = "Btu/lb";
            LightValue expected = new LightValue(23260.0, UnitLib3.DimensionUtils.FromMLT(0, 2, -2));
            LightValue actual;
            actual = GuiUtils.CreateLightValue(val, units_formula);
            Assert.AreEqual(expected.Value, actual.Value,1);
            Assert.AreEqual(expected.Dim, actual.Dim);
        }
        [TestMethod()]
        public void CreateLightValueTest1()
        {
            LightValue expected = new LightValue(23260.0, UnitLib3.DimensionUtils.FromMLT(0, 2, -2));
            LightValue actual;
            actual = GuiUtils.CreateLightValue("10 Btu/lb");
            Assert.AreEqual(expected.Value, actual.Value,1);
            Assert.AreEqual(expected.Dim, actual.Dim);
        }

        /// <summary>
        ///A test for LightValueInUnits
        ///</summary>
        [TestMethod()]
        public void LightValueInUnitsTest()
        {
            LightValue lv = new LightValue(23260.0, UnitLib3.DimensionUtils.FromMLT(0, 2, -2));
            string units_formula = "Btu/lb";
            string expected = "10 Btu/lb";
            string actual;
            actual = GuiUtils.LightValueInUnits(lv, units_formula);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for UpdateLightValue
        ///</summary>
        [TestMethod()]
        public void UpdateLightValueTest()
        {
            LightValue lv = new LightValue(23260.0, UnitLib3.DimensionUtils.FromMLT(0, 2, -2));
            string expression = "20 Btu/lb";
            GuiUtils.UpdateLightValue(lv, expression);
            Assert.AreEqual(23260.0 * 2, lv.Value, 1);
        }
        [TestMethod()]
        public void UpdateLightValueTest1()
        {
            LightValue lv = new LightValue(23260.0, UnitLib3.DimensionUtils.FromMLT(0, 2, -2));
            string expression = "20 Btu/km";
            GuiUtils.UpdateLightValue(lv, expression);
            Assert.AreEqual(23260.0, lv.Value, 1);
        }
        [TestMethod()]
        //[ExpectedException(typeof(Exception))]
        public void UpdateLightValueTest2()
        {
            LightValue lv = new LightValue(23260, UnitLib3.DimensionUtils.FromMLT(0, 2, -2));
            string expression = "20 Btu/sfjnrh ";
            GuiUtils.UpdateLightValue(lv, expression);
            Assert.AreEqual(23260.0, lv.Value, 1);
        }


        /// <summary>
        ///A test for CreateDim
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(Greet.UnitLib3.ExpressionStringParingException))]
        public void CreateDimTest1()
        {
            string p = "meters/seconds";
            uint expected = UnitLib3.DimensionUtils.FromMLT(0, 1, -1);
            uint actual;
            actual = GuiUtils.CreateDim(p);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BottomQuantityName
        ///</summary>
        [TestMethod()]
        public void BottomQuantityNameTest()
        {
            uint dim = UnitLib3.DimensionUtils.FromMLT(0, 2, -2);
            string expected = "mass";
            string actual;
            actual = GuiUtils.BottomQuantityName(dim);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TopQuantityName
        ///</summary>
        [TestMethod()]
        public void TopQuantityNameTest()
        {
            uint dim = UnitLib3.DimensionUtils.FromMLT(0, 2, -2);
            string expected = "energy";
            string actual;
            actual = GuiUtils.TopQuantityName(dim);
            Assert.AreEqual(expected, actual);
        }
    }
}
