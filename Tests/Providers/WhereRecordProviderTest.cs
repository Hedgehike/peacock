using System;
using System.Collections.Generic;
using System.Linq;
using Abide;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Providers
{
    [TestClass]
    public class WhereRecordProviderTest
    {
        [TestMethod]
        public void TestEmpty()
        {
            var provider = new WhereRecordProvider(new EmptyRecordProvider(), new [] {new EqualityConstraint("mockString", "asd", new Abide.ByteConverter()), });
            provider.Read();
        }

        [TestMethod]
        [ExpectedException(typeof(MalformedQueryException))]
        public void TestInvalid()
        {
            var provider = new RecordParser(
                new WhereRecordProvider(new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 1f),
                    new Tuple<string, int, float>("aaa", 1, 2f),
                    new Tuple<string, int, float>("bbb", 1, 1f),
                    new Tuple<string, int, float>("bbb", 1, 4f),
                }), 
                new [] {new EqualityConstraint("asd", "asd", new ByteConverter()), }));
            provider.ParseData();
        }

        [TestMethod]
        public void TestNoConstraints()
        {
            var provider = new RecordParser(
                new WhereRecordProvider(new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 1f),
                    new Tuple<string, int, float>("aaa", 1, 2f),
                    new Tuple<string, int, float>("bbb", 1, 1f),
                    new Tuple<string, int, float>("bbb", 1, 4f),
                }), new List<IWhereQueryConstraint>()));
            Assert.AreEqual(4, provider.ParseData().Count());
        }

        [TestMethod]
        public void TestHasResult()
        {
            var provider = new RecordParser(
                new WhereRecordProvider(new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 1f),
                    new Tuple<string, int, float>("aaa", 1, 2f),
                    new Tuple<string, int, float>("bbb", 1, 1f),
                    new Tuple<string, int, float>("bbb", 1, 4f),
                }), 
                new [] {new EqualityConstraint("mockString", "aaa", new ByteConverter()), }));
            Assert.AreEqual(2, provider.ParseData().Count());
            Assert.IsTrue(provider.ParseData().All(d => (string)d["mockString"] == "aaa"));
        }

        [TestMethod]
        public void TestHasNoResult()
        {
            var provider = new RecordParser(
                new WhereRecordProvider(new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 1f),
                    new Tuple<string, int, float>("aaa", 1, 2f),
                    new Tuple<string, int, float>("bbb", 1, 1f),
                    new Tuple<string, int, float>("bbb", 1, 4f),
                }),
                new[] { new EqualityConstraint("mockString", "asd", new ByteConverter()), }));
            Assert.AreEqual(0, provider.ParseData().Count());
        }

        [TestMethod]
        public void TestMultipleConstraints()
        {
            var provider = new RecordParser(
                new WhereRecordProvider(new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 1f),
                    new Tuple<string, int, float>("aaa", 1, 2f),
                    new Tuple<string, int, float>("bbb", 1, 1f),
                    new Tuple<string, int, float>("bbb", 1, 4f),
                }),
                new[]
                {
                    new EqualityConstraint("mockString", "aaa", new ByteConverter()), 
                    new EqualityConstraint("mockFloat", 2f, new ByteConverter()), 
                }));
            Assert.AreEqual(1, provider.ParseData().Count());
            Assert.AreEqual(2f, (float)provider.ParseData().First()["mockFloat"]);
            Assert.AreEqual("aaa", (string)provider.ParseData().First()["mockString"]);
        }
    }
}
