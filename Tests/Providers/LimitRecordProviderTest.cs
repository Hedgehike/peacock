using System;
using System.Linq;
using Abide;
using Abide.RecordProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Providers;

namespace Tests
{
    [TestClass]
    public class LimitRecordProviderTest
    {
        [TestMethod]
        public void TestEmpty()
        {
            var provider = new LimitRecordProvider(5, new EmptyRecordProvider());
            Assert.AreEqual(false, provider.Read().Any());
        }

        [TestMethod]
        public void TestSingle()
        {
            var provider = new RecordParser(
                new LimitRecordProvider(5, new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 2.5f),
                })));
            Assert.AreEqual(provider.ParseData().Count(), 1);
        }

        [TestMethod]
        public void TestMultiple()
        {
            var provider = new RecordParser(
                new LimitRecordProvider(3, new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 1f),
                    new Tuple<string, int, float>("aaa", 1, 2f),
                    new Tuple<string, int, float>("bbb", 1, 1f),
                    new Tuple<string, int, float>("bbb", 1, 3f),
                    new Tuple<string, int, float>("bbb", 1, 3f),
                })));
            Assert.AreEqual(3, provider.ParseData().Count());
            Assert.AreEqual("aaa", (string)provider.ParseData().First()["mockString"]);
            Assert.AreEqual("bbb", (string)provider.ParseData().Skip(2).First()["mockString"]);
            Assert.AreEqual(1f, (float)provider.ParseData().Skip(2).First()["mockFloat"]);
        }
    }
}
