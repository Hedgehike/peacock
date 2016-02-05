using System;
using System.CodeDom;
using System.Linq;
using Abide;
using Abide.RecordProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Providers
{
    [TestClass]
    public class ComputedFieldProviderTest
    {
        [TestMethod]
        public void TestEmpty()
        {
            var provider = new ComputedFieldProvider("computedField", ColumnType.Float, (data, bytes) => null, new EmptyRecordProvider());
            Assert.AreEqual(false, provider.Read().Any());
        }


        [TestMethod]
        public void TestFloat()
        {
            var provider = new RecordParser(
                new ComputedFieldProvider("computedField", ColumnType.Float, (data, bytes) => 3.14f, 
                new CollectionRecordProvider(new []
                {
                    new Tuple<string, int, float>("aaa", 2, 2.5f), 
                })));
            Assert.AreEqual(provider.ParseData().Count(), 1);
            Assert.AreEqual((float)provider.ParseData().First()["computedField"], 3.14f);
        }


        [TestMethod]
        public void TestInt()
        {
            var provider = new RecordParser(
                new ComputedFieldProvider("computedField", ColumnType.Int, (data, bytes) => 3,
                new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 2, 2.5f),
                })));
            Assert.AreEqual(provider.ParseData().Count(), 1);
            Assert.AreEqual((int)provider.ParseData().First()["computedField"], 3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TextBadFunc()
        {
            var provider = new RecordParser(
                new ComputedFieldProvider("computedField", ColumnType.Int, (data, bytes) => { throw new Exception(); },
                new CollectionRecordProvider(new[]
                {
                    new Tuple<string, int, float>("aaa", 2, 2.5f),
                })));
            provider.ParseData();
        }

    }
}
