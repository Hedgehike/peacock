using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Abide
{
    public class IndexWriter
    {
        private readonly Dictionary<byte[], IList<long>> buckets;
        private readonly long indexWidth;
        private readonly long maxBuckets;

        public IndexWriter(Dictionary<byte[], IList<long>> buckets)
        {
            this.buckets = buckets;
            maxBuckets = buckets.Values.Max(v => v.Count);
            indexWidth = sizeof (long)*maxBuckets + buckets.Keys.First().Length*sizeof (byte);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(indexWidth);
            foreach (var key in buckets.Keys)
            {
                writer.Write(key);
                var bucket = buckets[key];
                for (int i = 0; i < bucket.Count; i++)
                {
                    writer.Write(bucket[i]);
                }
                for (int i = bucket.Count; i < maxBuckets; i++)
                {
                    writer.Write(long.MaxValue);
                }
            }
        }
    }
}