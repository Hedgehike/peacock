using System.Collections.Generic;
using System.IO;

namespace Abide
{
    public class IndexReader
    {
        public IDictionary<byte[], IList<long>> Read(BinaryReader reader, int keyWidth)
        {
            var indexWidth = reader.ReadInt64();
            var buckets = new Dictionary<byte[], IList<long>>(new ByteArrayComparer());
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var bucket = new List<long>();
                var index = reader.ReadBytes(keyWidth);
                for (int i = 0; i < indexWidth - keyWidth; i += sizeof (long))
                {
                    var value = reader.ReadInt64();
                    if (value != long.MaxValue)
                    {
                        bucket.Add(value);
                    }
                    else
                    {
                        reader.BaseStream.Seek(indexWidth - keyWidth - i - sizeof (long), SeekOrigin.Current);
                        break;
                    }
                }
                buckets[index] = bucket;
            }
            return buckets;
        }
    }
}