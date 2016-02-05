using System.Collections.Generic;
using System.Linq;

namespace Abide
{
    public class JoinRecordProvider : IRecordProvider
    {
        private readonly string leftField;
        private readonly IRecordProvider leftProvider;
        private readonly string rightField;
        private readonly IRecordProvider rightProvider;

        public JoinRecordProvider(IRecordProvider leftProvider, IRecordProvider rightProvider, string leftField,
            string rightField)
        {
            this.leftProvider = leftProvider;
            this.rightProvider = rightProvider;
            this.leftField = leftField;
            this.rightField = rightField;

            if (!leftProvider.MetaData.ColumnDescriptors.ContainsKey(leftField))
            {
                throw new MalformedQueryException($"No field named {leftField} found.");
            }
            if (!rightProvider.MetaData.ColumnDescriptors.ContainsKey(rightField))
            {
                throw new MalformedQueryException($"No field named {rightField} found.");
            }
            var leftDescriptor = leftProvider.MetaData.ColumnDescriptors[leftField];
            var rightDescriptor = rightProvider.MetaData.ColumnDescriptors[rightField];
            if (leftDescriptor.Type !=
                rightDescriptor.Type)
            {
                throw new MalformedQueryException($"Field types {leftDescriptor.Type} and {rightDescriptor.Type} dont't match.");
            }

            var allKeys =
                leftProvider.MetaData.ColumnDescriptors.Keys.Union(rightProvider.MetaData.ColumnDescriptors.Keys);
            MetaData = new RecordMetaData();
            foreach (var key in allKeys)
            {
                if (leftProvider.MetaData.ColumnDescriptors.Keys.Contains(key))
                {
                    MetaData.AddField(
                        key,
                        leftProvider.MetaData.ColumnDescriptors[key].Type,
                        leftProvider.MetaData.ColumnDescriptors[key].Width
                        );
                }
                else
                {
                    MetaData.AddField(
                        key,
                        rightProvider.MetaData.ColumnDescriptors[key].Type,
                        rightProvider.MetaData.ColumnDescriptors[key].Width
                        );
                }
            }
        }

        public IEnumerable<byte[]> Read()
        {
            //TODO not nested loops join
            var leftRecords = leftProvider.Read();
            var rightRecords = rightProvider.Read();            

            var width = leftProvider.MetaData.ColumnDescriptors[leftField].Width;

            var rightHash = new Dictionary<byte[], byte[]>(new ByteArrayComparer());
            foreach (byte[] rightRecord in rightRecords)
            {
                var buffer = new byte[width];
                for (var i = 0; i < width; i++)
                {
                    buffer[i] = rightRecord[rightProvider.MetaData.ColumnDescriptors[rightField].Offset + i];
                }
                rightHash.Add(buffer, rightRecord);
            }

            foreach (byte[] leftRecord in leftRecords)
            {
                var buffer = new byte[leftProvider.MetaData.RecordWitdh + rightProvider.MetaData.RecordWitdh];

                var leftKey = new byte[width];
                for (int i = 0; i < width; i++)
                    leftKey[i] = leftRecord[leftProvider.MetaData.ColumnDescriptors[leftField].Offset + i];
                if (!rightHash.ContainsKey(leftKey)) continue;
                var rightRecord = rightHash[leftKey];
                foreach (var key in MetaData.ColumnDescriptors.Keys)
                {
                    CopyFromEither(key, ref buffer, leftRecord, rightRecord);
                }
                yield return buffer;
            }
        }

        public RecordMetaData MetaData { get; }

        public IEnumerable<IRecordProvider> DownstreamSteps()
        {
            return leftProvider.DownstreamSteps().Concat(rightProvider.DownstreamSteps());
        }

        private void CopyFromEither(string key, ref byte[] buffer, byte[] left, byte[] right)
        {
            var myOffset = MetaData.ColumnDescriptors[key].Offset;
            var width = MetaData.ColumnDescriptors[key].Width;
            long theirOffset;
            if (leftProvider.MetaData.ColumnDescriptors.Keys.Contains(key))
            {
                theirOffset = leftProvider.MetaData.ColumnDescriptors[key].Offset;
                for (int i = 0; i < width; i++) buffer[myOffset + i] = left[theirOffset + i];
            }
            else
            {
                theirOffset = rightProvider.MetaData.ColumnDescriptors[key].Offset;
                for (int i = 0; i < width; i++) buffer[myOffset + i] = right[theirOffset + i];
            }
        }
    }
}