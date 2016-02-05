using System;
using System.Collections.Generic;
using System.Linq;
using Abide;
using Abide.RecordProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Providers
{
    [TestClass]
    public class SortingRecordProviderTest
    {
        [TestMethod]
        public void TestEmpty()
        {
            var provider = new SortingRecordProvider("mockString",new EmptyRecordProvider());
            Assert.AreEqual(false, provider.Read().Any());
        }

        [TestMethod]
        [ExpectedException(typeof(MalformedQueryException))]
        public void TestMalformed()
        {
            var provider = new SortingRecordProvider("nonExistent", new EmptyRecordProvider());
            provider.Read();
        }

        [TestMethod]
        public void TestSortByString()
        {
            var provider = new RecordParser(
                new SortingRecordProvider("mockString",
                new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("bbb", 1, 2.5f),
                    new Tuple<string, int, float>("bab", 1, 2.5f),
                    new Tuple<string, int, float>("ccc", 1, 2.5f),
                    new Tuple<string, int, float>("aaa", 1, 2.5f),
                })));
            var results = provider.ParseData().ToArray();
            Assert.AreEqual(provider.ParseData().Count(), 4);
            Assert.AreEqual("aaa", (string)results[0]["mockString"]);
            Assert.AreEqual("bab", (string)results[1]["mockString"]);
            Assert.AreEqual("bbb", (string)results[2]["mockString"]);
            Assert.AreEqual("ccc", (string)results[3]["mockString"]);
        }

        [TestMethod]
        public void TestSortByInt()
        {
            var provider = new RecordParser(
                new SortingRecordProvider("mockInt",
                new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("bbb", 1, 2.5f),
                    new Tuple<string, int, float>("bab", 2, 2.5f),
                    new Tuple<string, int, float>("ccc", 3, 2.5f),
                    new Tuple<string, int, float>("aaa", 4, 2.5f),
                })));
            var results = provider.ParseData().ToArray();
            Assert.AreEqual(provider.ParseData().Count(), 4);
            Assert.AreEqual("bbb", (string)results[0]["mockString"]);
            Assert.AreEqual("bab", (string)results[1]["mockString"]);
            Assert.AreEqual("ccc", (string)results[2]["mockString"]);
            Assert.AreEqual("aaa", (string)results[3]["mockString"]);
        }

        [TestMethod]
        public void TestSortByFloat()
        {
            var provider = new RecordParser(
                new SortingRecordProvider("mockFloat",
                new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("bbb", 1, -5f),
                    new Tuple<string, int, float>("bab", 1, 3f),
                    new Tuple<string, int, float>("ccc", 1, 4f),
                    new Tuple<string, int, float>("aaa", 1, 5f),
                })));
            var results = provider.ParseData().ToArray();
            Assert.AreEqual(provider.ParseData().Count(), 4);
            Assert.AreEqual("bbb", (string)results[0]["mockString"]);
            Assert.AreEqual("bab", (string)results[1]["mockString"]);
            Assert.AreEqual("ccc", (string)results[2]["mockString"]);
            Assert.AreEqual("aaa", (string)results[3]["mockString"]);
        }
    }
}
