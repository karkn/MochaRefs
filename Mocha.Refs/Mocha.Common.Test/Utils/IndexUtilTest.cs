using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mocha.Common.Utils
{
    [TestClass]
    public class IndexUtilTest
    {
        [TestMethod]
        public void TestGetPageCount()
        {
            Assert.AreEqual(1, IndexUtil.GetPageCount(0, 10));
            Assert.AreEqual(1, IndexUtil.GetPageCount(10, 10));
            Assert.AreEqual(2, IndexUtil.GetPageCount(11, 10));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetPageCount_NegativeElemCount()
        {
            IndexUtil.GetPageCount(-1, 10);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetPageCount_ZeroPageSize()
        {
            IndexUtil.GetPageCount(10, 0);
        }
    }
}
