using System.Collections.Generic;

namespace Tests.Providers
{
    public class SingleRecordProvider : MockRecordProvider
    {
        public override IEnumerable<byte[]> Read()
        {
            yield return createRecord("AAA", 2, 3.5f);
        }
    }
}