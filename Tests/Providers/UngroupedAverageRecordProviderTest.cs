using System;
using System.Linq;
using Abide;
using Abide.RecordProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Providers
{
    [TestClass]
    public class UngroupedAverageRecordProviderTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void TestEmpty()
        {
            var provider = new UngroupedAverageProvider("mockFloat", new EmptyRecordProvider());
            provider.Read();
        }

        [TestMethod]
        [ExpectedException(typeof(MalformedQueryException))]
        public void TestMalformed()
        {
            var provider = new UngroupedAverageProvider("nonExistent", new EmptyRecordProvider());
            provider.Read();
        }

        [TestMethod]
        public void TestSingle()
        {
            var provider = new RecordParser(
                new UngroupedAverageProvider("mockFloat", new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 2.5f),
                })));
            Assert.AreEqual(provider.ParseData().Count(), 1);
            Assert.AreEqual((float)provider.ParseData().First()["avg_of_mockFloat"], 2.5f);

        }

        [TestMethod]
        public void TestMultiple()
        {
            var provider = new RecordParser(
                new UngroupedAverageProvider("mockFloat", new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 1, 1f),
                    new Tuple<string, int, float>("aaa", 1, 2f),
                    new Tuple<string, int, float>("bbb", 1, 1f),
                    new Tuple<string, int, float>("bbb", 1, 4f),
                })));
            Assert.AreEqual(1, provider.ParseData().Count());
            Assert.AreEqual(2f, (float)provider.ParseData().First()["avg_of_mockFloat"]);
        }
    }
}
