using System;
using System.Collections.Generic;

namespace Abide
{
    public class SelectionRecordProvider : RecordProviderBase
    {
        private readonly IEnumerable<string> fields;

        public SelectionRecordProvider(IEnumerable<string> fields, IRecordProvider provider) : base(provider)
        {
            this.fields = fields;
            MetaData = new RecordMetaData();
            foreach (string field in fields)
            {
                try
                {
                    var underlyingColumn = provider.MetaData.ColumnDescriptors[field];
                    MetaData.AddField(field, underlyingColumn.Type, underlyingColumn.Width);
                }
                catch (Exception e)
                {
                    throw new MalformedQueryException($"No field named {field} found.");                    
                }
                
            }
        }

        public override RecordMetaData MetaData { get; }

        public override IEnumerable<byte[]> Read()
        {
            foreach (byte[] record in provider.Read())
            {
                var buffer = new byte[MetaData.RecordWitdh];
                foreach (string field in fields)
                {
                    var myOffset = MetaData.ColumnDescriptors[field].Offset;
                    var width = MetaData.ColumnDescriptors[field].Width;
                    var theirOffset = provider.MetaData.ColumnDescriptors[field].Offset;
                    for (int i = 0; i < width; i++)
                    {
                        buffer[myOffset + i] = record[theirOffset + i];
                    }
                }
                yield return buffer;
            }
        }
    }
}