using System;
using System.Collections.Generic;
using System.Linq;
using Abide;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.Providers;

namespace Tests
{
    [TestClass]
    public class SelectionRecordProviderTest
    {
        [TestMethod]
        public void TestEmpty()
        {
            var provider = new SelectionRecordProvider(new [] {"mockString", "mockFloat"}, new EmptyRecordProvider());
            Assert.AreEqual(false, provider.Read().Any());
        }

        [TestMethod]
        public void TestNoFields()
        {
            var provider = new SelectionRecordProvider(new List<string>(), new EmptyRecordProvider());
            provider.Read();
        }

        [TestMethod]
        [ExpectedException(typeof(MalformedQueryException))]
        public void TestMalformed()
        {
            var provider = new SelectionRecordProvider(new [] {"nonExistent", "mockFloat"}, new EmptyRecordProvider());
            provider.Read();
        }

        [TestMethod]
        public void TestSingle()
        {
            var provider = new RecordParser(
                new SelectionRecordProvider( new [] {"mockString", "mockFloat"},
                new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 2.5f),
                })));
            Assert.AreEqual(provider.ParseData().Count(), 1);
            Assert.AreEqual(2, provider.ParseData().First().Keys.Count);
            Assert.AreEqual(true, provider.ParseData().First().Keys.Contains("mockString"));
            Assert.AreEqual(true, provider.ParseData().First().Keys.Contains("mockFloat"));
            Assert.AreEqual((float)provider.ParseData().First()["mockFloat"], 2.5f);

        }

        [TestMethod]
        public void TestMultiple()
        {
            var provider = new RecordParser(
                new SelectionRecordProvider(new[] { "mockString", "mockFloat" },
                new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 2.5f),
                    new Tuple<string, int, float>("bbb", 1, 2.5f),
                    new Tuple<string, int, float>("ccc", 1, 2.5f),
                    new Tuple<string, int, float>("ddd", 1, 2.5f),
                })));
            Assert.AreEqual(provider.ParseData().Count(), 4);
            Assert.AreEqual(2, provider.ParseData().First().Keys.Count);
            Assert.AreEqual(true, provider.ParseData().All( d => d.Keys.Contains("mockString")));
            Assert.AreEqual(true, provider.ParseData().All( d => d.Keys.Contains("mockFloat")));
        }
    }
}
