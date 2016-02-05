using System.Collections.Generic;

namespace Abide
{
    public class RecordProviderBase : IRecordProvider
    {
        protected IRecordProvider provider;

        public RecordProviderBase(IRecordProvider provider)
        {
            this.provider = provider;
        }

        public virtual IEnumerable<byte[]> Read()
        {
            return provider.Read();
        }

        public virtual RecordMetaData MetaData => provider.MetaData;

        public virtual IEnumerable<IRecordProvider> DownstreamSteps()
        {
            var downstream = new List<IRecordProvider> {provider};
            downstream.AddRange(provider.DownstreamSteps());
            return downstream;
            ;
        }
    }
}