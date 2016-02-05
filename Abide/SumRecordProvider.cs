using System;
using System.Collections.Generic;

namespace Abide
{
    internal class SumRecordProvider : AggregateRecordProvider
    {
        public SumRecordProvider(string groupBy, string aggregatedField, IRecordProvider provider)
            : base(groupBy, aggregatedField, provider)
        {
            MetaData.AddField(aggregatedField, provider.MetaData.ColumnDescriptors[aggregatedField].Type);
        }

        public override IEnumerable<byte[]> Read()
        {
            Dictionary<byte[], float> result = new Dictionary<byte[], float>(new ByteArrayComparer());
            foreach (byte[] row in provider.Read())
            {
                var key = new byte[MetaData.ColumnDescriptors[groupBy].Width];
                var offset = provider.MetaData.ColumnDescriptors[groupBy].Offset;
                for (int i = 0; i < key.Length; i++) key[i] = row[offset + i];

                ColumnData aggregatedColumn = provider.MetaData.ColumnDescriptors[aggregatedField];
                offset = aggregatedColumn.Offset;

                if (result.ContainsKey(key))
                {
                    result[key] += BitConverter.ToSingle(row, offset);
                }
                else
                {
                    result.Add(key, BitConverter.ToSingle(row, offset));
                }
            }


            List<byte[]> resultList = new List<byte[]>();
            foreach (KeyValuePair<byte[], float> pair in result)
            {
                var buffer = new byte[MetaData.ColumnDescriptors[groupBy].Width + sizeof (float)];

                for (int i = 0; i < pair.Key.Length; i++) buffer[i] = pair.Key[i];

                var value = BitConverter.GetBytes(pair.Value);

                for (int i = 0; i < sizeof (float); i++) buffer[pair.Key.Length + i] = value[i];

                resultList.Add(buffer);
            }
            return resultList;
        }
    }
}