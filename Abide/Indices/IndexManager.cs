using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Abide
{
    public class IndexManager
    {
        private readonly FileSystemScanner fileSystemScanner;
        private readonly List<string> indices = new List<string>();

        public IndexManager(FileSystemScanner fileSystemScanner)
        {
            indices.AddRange(
                fileSystemScanner.FilesMatching("^index_[a-zA-Z]*.dat$")
                    .Select(name => name.Split('_')[1].Split('.')[0]));
        }

        public bool HasIndex(string property)
        {
            return indices.Contains(property);
        }

        public IEnumerable<string> AvailableIndices()
        {
            return indices;
        }

        public IDictionary GetIndex(string property)
        {
            IDictionary result;
            var keyWidth =
                ((WidthAttribute) typeof (Record).GetProperty(property).GetCustomAttribute(typeof (WidthAttribute)))
                    .Width;
            using (var reader = new BinaryReader(new FileStream($"index_{property}.dat", FileMode.Open)))
            {
                result = (IDictionary) new IndexReader().Read(reader, keyWidth);
            }
            return result;
        }

        public void CreateIndex(string property)
        {
            PropertyInfo propertyInfo = typeof (Record).GetProperty(property);
            Dictionary<byte[], IList<long>> buckets = new Dictionary<byte[], IList<long>>(new ByteArrayComparer());
            var offset = ((OffsetAttribute) propertyInfo.GetCustomAttribute(typeof (OffsetAttribute))).Offset;
            var width = ((WidthAttribute) propertyInfo.GetCustomAttribute(typeof (WidthAttribute))).Width;
            using (var reader = new BinaryReader(new FileStream("orders.dat", FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var readerPosition = reader.BaseStream.Position;

                    reader.BaseStream.Seek(offset, SeekOrigin.Current);
                    var propertyBuffer = reader.ReadBytes(width);
                    reader.BaseStream.Seek(Record.RECORD_WIDTH - offset - width, SeekOrigin.Current);

                    if (buckets.ContainsKey(propertyBuffer))
                    {
                        var bucket = buckets[propertyBuffer];
                        ((IList) bucket).Add(readerPosition);
                    }
                    else
                    {
                        buckets[propertyBuffer] = new List<long> {readerPosition};
                    }
                }
            }

            using (var writer = new BinaryWriter(new FileStream($"index_{property}.dat", FileMode.Create)))
            {
                var formatter = new IndexWriter(buckets);
                formatter.Serialize(writer);
            }

            indices.Add(property);
        }
    }
}