using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Documents;

namespace Abide.RecordProviders
{
    public class ComputedFieldProvider : RecordProviderBase
    {
        private readonly Func<RecordMetaData, byte[], dynamic> computation;
        private readonly string fieldName;
        private readonly ColumnType type;

        public ComputedFieldProvider(string fieldName, ColumnType type,
            Func<RecordMetaData, byte[], dynamic> computation, IRecordProvider provider) : base(provider)
        {
            this.fieldName = fieldName;
            this.type = type;
            this.computation = computation;
            MetaData = new RecordMetaData();
            foreach (var columnDescriptor in provider.MetaData.ColumnDescriptors)
            {
                MetaData.ColumnDescriptors.Add(columnDescriptor.Key, columnDescriptor.Value);
            }
            MetaData.AddField(fieldName, type);
        }

        public override RecordMetaData MetaData { get; }

        public override IEnumerable<byte[]> Read()
        {
            foreach (var row in provider.Read())
            {

                dynamic result;
                try
                {
                    result = computation.Invoke(MetaData, row);
                }
                catch (Exception e)
                {
                    throw new ArgumentException("Provided computation function threw an exception", e);
                }
                var resultBytes = BitConverter.GetBytes(result);
                var buffer = new byte[row.Length + resultBytes.Length];
                for (int i = 0; i < row.Length; i++)
                {
                    buffer[i] = row[i];
                }
                for (int i = 0; i < resultBytes.Length; i++)
                {
                    buffer[MetaData.ColumnDescriptors[fieldName].Offset + i] = resultBytes[i];
                }
                yield return buffer;
            }
        }
    }
}