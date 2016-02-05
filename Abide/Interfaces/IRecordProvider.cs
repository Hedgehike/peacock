using System.Collections.Generic;

namespace Abide
{
    public interface IRecordProvider
    {
        RecordMetaData MetaData { get; }
        IEnumerable<byte[]> Read();
        IEnumerable<IRecordProvider> DownstreamSteps();
    }
}