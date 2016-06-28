using Greet.UnitLib3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Greet.UnitLib3Test
{
    
    
    /// <summary>
    ///This is a test class for DimensionUtilsTest and is intended
    ///to contain all DimensionUtilsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DimensionUtilsTest
    {


        private TestContext testContextInstance;

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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for FromMLT
        ///</summary>
        [TestMethod()]
        public void FromMLTTest()
        {
            uint actual;
            actual = DimensionUtils.FromMLT(1, 2, 3, 4);
            uint expected = 1060993; //000100 000011 000010 000001
            Assert.AreEqual(expected, actual);

            actual = DimensionUtils.FromMLT(-5, 0, 0, 0);
            expected = 27; //000000 000000 000000 011011 
            Assert.AreEqual(expected, actual);

            actual = DimensionUtils.FromMLT(-5, -2, 0, 0);
            expected = 1947; //000000 000000 011110 011011 
            Assert.AreEqual(expected, actual);

            actual = DimensionUtils.FromMLT(-5, -2, -3, -2);
            expected = 7985051; //011110011101011110011011 
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void TestPlus()
        {

            uint a = DimensionUtils.FromMLT(1, 2, -6, 4); //1155201   000100  011010  000010  000001
            uint b = DimensionUtils.FromMLT(2, 3, 5, -4);//7360706    011100  000101  000011  000010
            uint c = DimensionUtils.Plus(a, b);          //8515907    100000  011111  000101  000011
            int m, l, t, K;
            DimensionUtils.ToMLT(c, out m, out l, out t, out K);
            Assert.AreEqual(3, m);
            Assert.AreEqual(5, l);
            Assert.AreEqual(-1, t);
            Assert.AreEqual(0, K);
        }
        [TestMethod()]
        public void TestMinus()
        {

            uint a = DimensionUtils.FromMLT(1, 2, -6, 4);
            uint b = DimensionUtils.FromMLT(2, 3, 5, -4);
            uint c = DimensionUtils.Minus(a, b);
            int m, l, t, K;
            DimensionUtils.ToMLT(c, out m, out l, out t, out K);
            Assert.AreEqual(-1, m);
            Assert.AreEqual(-1, l);
            Assert.AreEqual(-11, t);
            Assert.AreEqual(8, K);
        }

        /// <summary>
        ///A test for ToMLT
        ///</summary>
        [TestMethod()]
        public void ToMLTTest()
        {
            int kg = 0;
            int m = 0;
            int s = 0;
            int K = 0;
            uint dim;
            dim = DimensionUtils.FromMLT(-5, 0, 0, 0);
            DimensionUtils.ToMLT(dim, out kg, out m, out s, out K);
            Assert.AreEqual(-5, kg);
            dim = DimensionUtils.FromMLT(-2, 3, -4, 1);

            DimensionUtils.ToMLT(dim, out kg, out m, out s, out K);
            Assert.AreEqual(-2, kg);
            Assert.AreEqual(3, m);
            Assert.AreEqual(-4, s);
            Assert.AreEqual(1, K);
        }

        /// <summary>
        ///A test for ToMLTh
        ///</summary>
        [TestMethod()]
        public void ToMLThTest()
        {
            uint dim = DimensionUtils.FromMLT(1, 2, -3);
            string expected = "[mass]^1[length]^2[time]^-3";
            string actual;
            actual = DimensionUtils.ToMLTh(dim);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void ToMLTUnithTest()
        {
            Assert.AreEqual("(kg m^2)/s^3", DimensionUtils.ToMLTUnith(DimensionUtils.FromMLT(1, 2, -3)));
            Assert.AreEqual("kg", DimensionUtils.ToMLTUnith(DimensionUtils.FromMLT(1, 0, 0)));
            Assert.AreEqual("1/s", DimensionUtils.ToMLTUnith(DimensionUtils.FromMLT(0, 0, -1)));
            Assert.AreEqual("s", DimensionUtils.ToMLTUnith(DimensionUtils.FromMLT(0, 0, 1)));
            Assert.AreEqual("", DimensionUtils.ToMLTUnith(DimensionUtils.FromMLT(0, 0, 0)));
        }

        /// <summary>
        ///A test for Times
        ///</summary>
        [TestMethod()]
        public void TimesTest()
        {
            uint a = DimensionUtils.FromMLT(1, -2, 3);
            int factor = 2;
            uint expected = DimensionUtils.FromMLT(2, -4, 6);
            uint actual;
            actual = DimensionUtils.Times(a, factor);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TimesTest1()
        {
            uint a = DimensionUtils.FromMLT(1, -2, 3);
            int factor = 0;
            uint expected = DimensionUtils.FromMLT(0, 0, 0);
            uint actual;
            actual = DimensionUtils.Times(a, factor);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TimesTest2()
        {
            uint a = DimensionUtils.FromMLT(1, -2, 3);
            int factor = -2;
            uint expected = DimensionUtils.FromMLT(-2, 4, -6);
            uint actual;
            actual = DimensionUtils.Times(a, factor);
            Assert.AreEqual(expected, actual);
        }
        [TestMethod()]
        public void TimesTest3()
        {
            uint a = DimensionUtils.FromMLT(1, -2, 3);
            int factor = 1;
            uint expected = DimensionUtils.FromMLT(1, -2, 3);
            uint actual;
            actual = DimensionUtils.Times(a, factor);
            Assert.AreEqual(expected, actual);
        }
    }
}
