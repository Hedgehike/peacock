using System;
using System.Linq;
using Abide;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Providers
{
    [TestClass]
    public class CountRecordProviderTest
    {
        [TestMethod]
        public void TestEmpty()
        {
            var provider = new CountRecordProvider("mockString", "mockFloat", new EmptyRecordProvider());
            Assert.AreEqual(false, provider.Read().Any());
        }

        [TestMethod]
        [ExpectedException(typeof(MalformedQueryException))]
        public void TestMalformed()
        {
            var provider = new CountRecordProvider("nonExistent", "mockFloat", new EmptyRecordProvider());
            provider.Read();
        }

        [TestMethod]
        public void TestSingle()
        {
            var provider = new RecordParser(
                new CountRecordProvider("mockString", "mockFloat", new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 2.5f),
                })));
            Assert.AreEqual(provider.ParseData().Count(), 1);
            Assert.AreEqual((int)provider.ParseData().First()["count_of_mockFloat"], 1);

        }

        [TestMethod]
        public void TestMultiple()
        {
            var provider = new RecordParser(
                new CountRecordProvider("mockString", "mockFloat", new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 1f),
                    new Tuple<string, int, float>("aaa", 1, 2f),
                    new Tuple<string, int, float>("bbb", 1, 1f),
                    new Tuple<string, int, float>("bbb", 1, 3f),
                    new Tuple<string, int, float>("bbb", 1, 3f),
                })));
            Assert.AreEqual(2, provider.ParseData().Count());
            Assert.AreEqual("aaa", (string)provider.ParseData().First()["mockString"]);
            Assert.AreEqual("bbb", (string)provider.ParseData().Skip(1).First()["mockString"]);
            Assert.AreEqual(2, (int)provider.ParseData().First()["count_of_mockFloat"]);
            Assert.AreEqual(3, (int)provider.ParseData().Skip(1).First()["count_of_mockFloat"]);
        }
    }
}

