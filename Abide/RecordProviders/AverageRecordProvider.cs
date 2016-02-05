using System;
using System.Collections.Generic;
using System.Linq;

namespace Abide
{
    public class AverageRecordProvider : AggregateRecordProvider
    {
        public AverageRecordProvider(string groupBy, string aggregatedField, IRecordProvider provider)
            : base(groupBy, aggregatedField, provider)
        {
            MetaData.ColumnDescriptors[$"avg_of_{aggregatedField}"] = new ColumnData
            {
                Width = sizeof (int),
                Offset = provider.MetaData.ColumnDescriptors[groupBy].Width,
                Type = ColumnType.Float
            };
        }

        public override IEnumerable<byte[]> Read()
        {
            Dictionary<byte[], Tuple<int, float>> result =
                new Dictionary<byte[], Tuple<int, float>>(new ByteArrayComparer());
            if (!provider.Read().Any()) throw new ArithmeticException("No records supplied to Average expression.");
            foreach (byte[] row in provider.Read())
            {
                var key = new byte[MetaData.ColumnDescriptors[groupBy].Width];
                var offset = provider.MetaData.ColumnDescriptors[groupBy].Offset;
                for (int i = 0; i < key.Length; i++) key[i] = row[offset + i];
                if (result.ContainsKey(key))
                {
                    result[key] = new Tuple<int, float>(
                        result[key].Item1 + 1,
                        result[key].Item2 +
                        BitConverter.ToSingle(row, provider.MetaData.ColumnDescriptors[aggregatedField].Offset)
                        );
                }
                else
                {
                    result.Add(key,
                        new Tuple<int, float>(1,
                            BitConverter.ToSingle(row, provider.MetaData.ColumnDescriptors[aggregatedField].Offset)));
                }
            }
            List<byte[]> resultList = new List<byte[]>();
            foreach (KeyValuePair<byte[], Tuple<int, float>> pair in result)
            {
                var buffer = new byte[MetaData.ColumnDescriptors[groupBy].Width + sizeof (int)];
                for (int i = 0; i < pair.Key.Length; i++) buffer[i] = pair.Key[i];
                var value = BitConverter.GetBytes(pair.Value.Item2/pair.Value.Item1);
                for (int i = 0; i < sizeof (int); i++) buffer[pair.Key.Length + i] = value[i];
                resultList.Add(buffer);
            }
            return resultList;
        }
    }
}