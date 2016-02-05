using System.Collections.Generic;
using System.Linq;

namespace Abide.RecordProviders
{
    public class LimitRecordProvider : RecordProviderBase
    {
        private readonly int limit;

        public LimitRecordProvider(int limit, IRecordProvider provider) : base(provider)
        {
            this.limit = limit;
        }

        public override IEnumerable<byte[]> Read()
        {
            return base.Read().Take(limit);
        }
    }
}