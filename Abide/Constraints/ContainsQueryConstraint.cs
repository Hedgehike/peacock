using System.Text;

namespace Abide
{
    class ContainsQueryConstraint : IWhereQueryConstraint
    {
        private readonly string search;

        public ContainsQueryConstraint(string property, string search)
        {
            Property = property;
            this.search = search;
        }

        public bool IsValid(byte[] record, RecordMetaData metaData)
        {
            string haystack = Encoding.ASCII.GetString(record, metaData.ColumnDescriptors[Property].Offset,
                metaData.ColumnDescriptors[Property].Width);
            return haystack.Contains(search);
        }

        public string Property { get; }
    }
}