using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mocha.Common.Cache
{
    [TestClass]
    public class EntityCacheTest
    {
        [TestMethod]
        public void TestEntityCache()
        {
            var cache = new ObjectCache<string>();
            cache.Add("key", "value", 30);

            Assert.AreEqual(true, cache.Exists("key"));
            Assert.AreEqual("value", cache.Get("key"));

            cache.Remove("key");
            Assert.AreEqual(false, cache.Exists("key"));
            Assert.AreEqual(null, cache.Get("key"));
        }
    }
}
