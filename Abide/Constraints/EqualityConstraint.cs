namespace Abide
{
    public class EqualityConstraint : IWhereQueryConstraint
    {
        public EqualityConstraint(string property, dynamic value, IByteConverter byteConverter)
        {
            Value = byteConverter.Convert(value);
            Property = property;
        }

        public byte[] Value { get; }

        public string Property { get; }

        public bool IsValid(byte[] record, RecordMetaData metaData)
        {
            if (!metaData.ColumnDescriptors.ContainsKey(Property))
            {
                throw new MalformedQueryException($"No field named {Property} found.");
            }
            var offset = metaData.ColumnDescriptors[Property].Offset;
            var width = metaData.ColumnDescriptors[Property].Width;
            for (int i = 0; i < width; i++)
            {
                if (i == Value.Length) return record[offset + i] == 0;
                if (record[offset + i] != Value[i]) return false;
            }
            return true;
        }
    }
}