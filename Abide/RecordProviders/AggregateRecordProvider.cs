using System.Collections.Generic;

namespace Abide
{
    public abstract class AggregateRecordProvider : RecordProviderBase
    {
        protected readonly string aggregatedField;
        protected readonly string groupBy;

        protected AggregateRecordProvider(string groupBy, string aggregatedField, IRecordProvider provider)
            : base(provider)
        {
            this.groupBy = groupBy;
            this.aggregatedField = aggregatedField;

            if (!provider.MetaData.ColumnDescriptors.ContainsKey(groupBy))
            {
                throw new MalformedQueryException($"Aggregated table does not contain a field '{groupBy}'");
            }
            if (!provider.MetaData.ColumnDescriptors.ContainsKey(aggregatedField))
            {
                throw new MalformedQueryException($"Aggregated table does not contain a field '{aggregatedField}'");
            }

            MetaData = new RecordMetaData();
            MetaData.AddField(groupBy, ColumnType.String, provider.MetaData.ColumnDescriptors[groupBy].Width);

            if (provider.MetaData.ColumnDescriptors.Count > 2)
            {
                this.provider = new SelectionRecordProvider(new List<string> {groupBy, aggregatedField}, provider);
            }
        }

        public override RecordMetaData MetaData { get; }
        public abstract override IEnumerable<byte[]> Read();
    }
}