using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Providers
{
    public class CollectionRecordProvider : MockRecordProvider
    {
        private readonly IEnumerable<Tuple<string, int, float>> values;

        public CollectionRecordProvider(IEnumerable<Tuple<string, int, float>> values, IEnumerable<string> columnNames = null) : base(columnNames)
        {
            this.values = values;
        }

        public IEnumerable<Tuple<string, int, float>> Values => values;

        public override IEnumerable<byte[]> Read()
        {
            return values.Select(tuple => createRecord(tuple.Item1, tuple.Item2, tuple.Item3));
        }
    }
}