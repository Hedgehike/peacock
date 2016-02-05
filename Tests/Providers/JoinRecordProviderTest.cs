using System;
using System.Linq;
using Abide;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Providers
{
    [TestClass]
    public class JoinRecordProviderTest
    {

        [TestMethod]
        public void EmptyTest()
        {
            var provider = new RecordParser(
                new JoinRecordProvider(
                    new EmptyRecordProvider(),
                    new EmptyRecordProvider(),
                    "mockString",
                    "mockString"
                    ));
            Assert.AreEqual(false, provider.ParseData().Any());
        }

        [TestMethod]
        [ExpectedException(typeof (MalformedQueryException))]
        public void NonExistentFieldTest()
        {
            var provider = new RecordParser(
                new JoinRecordProvider(
                    new EmptyRecordProvider(),
                    new EmptyRecordProvider(),
                    "aaa",
                    "mockString"
                    ));
            provider.ParseData();
        }

        [TestMethod]
        [ExpectedException(typeof (MalformedQueryException))]
        public void MismatchedDataTypesTest()
        {
            var provider = new RecordParser(
                new JoinRecordProvider(
                    new EmptyRecordProvider(),
                    new EmptyRecordProvider(),
                    "mockInt",
                    "mockString"
                    ));
            provider.ParseData();
        }

        [TestMethod]
        public void ValidQueryTest()
        {
            var provider = new RecordParser(
                new JoinRecordProvider(
                    new CollectionRecordProvider(
                        new[]
                        {
                            new Tuple<string, int, float>("aaa", 1, 2f),
                            new Tuple<string, int, float>("bbb", 2, 2f),
                            new Tuple<string, int, float>("bbb", 1, 3f),
                        }
                        ),
                    new CollectionRecordProvider(
                        new[]
                        {
                            new Tuple<string, int, float>("aaa", 5, 7f),
                            new Tuple<string, int, float>("bbb", 6, 8f),
                        },
                        new[] {"mockString2", "mockInt2", "mockFloat2"}
                        ),
                    "mockString",
                    "mockString2"
                    )
                );
            var data = provider.ParseData().ToArray();
            Assert.AreEqual(3, data.Count());
            Assert.AreEqual(5, (int) data[0]["mockInt2"]);
            Assert.AreEqual(6, (int) data[1]["mockInt2"]);
            Assert.AreEqual(6, (int) data[2]["mockInt2"]);
            Assert.AreEqual(2, (int) data[1]["mockInt"]);
            Assert.AreEqual(1, (int) data[2]["mockInt"]);
        }

        [TestMethod]
        public void MissingRightKeyTest()
        {
            var provider = new RecordParser(
                new JoinRecordProvider(
                    new CollectionRecordProvider(
                        new[]
                        {
                            new Tuple<string, int, float>("aaa", 1, 2f),
                            new Tuple<string, int, float>("bbb", 2, 2f),
                            new Tuple<string, int, float>("ccc", 1, 3f),
                        }
                        ),
                    new CollectionRecordProvider(
                        new[]
                        {
                            new Tuple<string, int, float>("aaa", 5, 7f),
                            new Tuple<string, int, float>("bbb", 6, 8f),
                        },
                        new[] {"mockString2", "mockInt2", "mockFloat2"}
                        ),
                    "mockString",
                    "mockString2"
                    )
                );

            var data = provider.ParseData().ToArray();
            Assert.AreEqual(2, data.Count());
            Assert.AreEqual(5, (int)data[0]["mockInt2"]);
            Assert.AreEqual(6, (int)data[1]["mockInt2"]);
            Assert.AreEqual(2, (int)data[1]["mockInt"]);
        }
    }
}


