using System;
using System.Collections.Generic;

namespace Abide.RecordProviders
{
    public class UngroupedAverageProvider : RecordProviderBase
    {
        private readonly string field;

        public UngroupedAverageProvider(string field, IRecordProvider provider) : base(provider)
        {
            this.field = field;
            if (!provider.MetaData.ColumnDescriptors.ContainsKey(field))
            {
                throw new MalformedQueryException($"Could not find field named {field}");
            }
            MetaData = new RecordMetaData();
            MetaData.AddField($"avg_of_{field}", ColumnType.Float);
        }

        public override RecordMetaData MetaData { get; }

        public override IEnumerable<byte[]> Read()
        {
            float sum = 0;
            int count = 0;
            foreach (var row in provider.Read())
            {
                sum += BitConverter.ToSingle(row, provider.MetaData.ColumnDescriptors[field].Offset);
                count++;
            }
            if (count == 0)
            {
                throw new ArithmeticException("No rows to compute average");
            }
            return new[] {BitConverter.GetBytes(sum/count)};
        }
    }
}