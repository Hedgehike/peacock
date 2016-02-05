using System;
using System.Collections.Generic;

namespace Abide
{
    public class CountRecordProvider : AggregateRecordProvider
    {
        public CountRecordProvider(string groupBy, string aggregatedField, IRecordProvider provider)
            : base(groupBy, aggregatedField, provider)
        {
            MetaData.ColumnDescriptors[$"count_of_{aggregatedField}"] = new ColumnData
            {
                Width = sizeof (int),
                Offset = provider.MetaData.ColumnDescriptors[groupBy].Width,
                Type = ColumnType.Int
            };
        }

        public override IEnumerable<byte[]> Read()
        {
            Dictionary<byte[], int> result = new Dictionary<byte[], int>(new ByteArrayComparer());
            foreach (byte[] row in provider.Read())
            {
                var key = new byte[MetaData.ColumnDescriptors[groupBy].Width];
                var offset = provider.MetaData.ColumnDescriptors[groupBy].Offset;
                for (int i = 0; i < key.Length; i++) key[i] = row[offset + i];
                if (result.ContainsKey(key))
                {
                    result[key]++;
                }
                else
                {
                    result.Add(key, 1);
                }
            }


            List<byte[]> resultList = new List<byte[]>();
            foreach (KeyValuePair<byte[], int> pair in result)
            {
                var buffer = new byte[MetaData.ColumnDescriptors[groupBy].Width + sizeof (int)];

                for (int i = 0; i < pair.Key.Length; i++) buffer[i] = pair.Key[i];

                var value = BitConverter.GetBytes(pair.Value);

                for (int i = 0; i < sizeof (int); i++) buffer[pair.Key.Length + i] = value[i];

                resultList.Add(buffer);
            }
            return resultList;
        }
    }
}