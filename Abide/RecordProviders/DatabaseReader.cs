using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Abide
{
    public abstract class DatabaseReader : WhereRecordProvider
    {
        private readonly string filename;
        private readonly IndexManager indexManager;
        private readonly QueryMode queryMode;
        private ByteConverter byteConverter;
        private RecordMetaData physicalRecordData = null;
        private int physicalRecordWidth;
        protected RecordMetaData recordMetaData;
        private List<string> selectionFields = new List<string>();


        protected DatabaseReader(string filename, IndexManager indexManager,
            IEnumerable<IWhereQueryConstraint> constraints, QueryMode queryMode = QueryMode.Auto)
            : base(null, constraints)
        {
            byteConverter = new ByteConverter();
            this.filename = filename;
            this.indexManager = indexManager;
            this.queryMode = queryMode;
        }

        protected abstract int RecordWidth { get; set; }

        public override RecordMetaData MetaData => recordMetaData;

        public override IEnumerable<IRecordProvider> DownstreamSteps() => new List<IRecordProvider>();

        public override IEnumerable<byte[]> Read()
        {
            return TableScan();
        }

        public IEnumerable<byte[]> TableScan()
        {
            if (queryMode != QueryMode.Sequential &&
                constraints.Any(
                    constraint => indexManager.HasIndex(constraint.Property) && constraint is EqualityConstraint))
            {
                var constraint =
                    constraints.First(c => indexManager.HasIndex(c.Property) && c is EqualityConstraint);
                return IndexedTableScan(constraint as EqualityConstraint);
            }
            return SequentialTableScan();
        }

        public IEnumerable<byte[]> SequentialTableScan()
        {
            using (var reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                byte[] buffer;
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    buffer = reader.ReadBytes(RecordWidth);
                    if (constraints.All(constraint => constraint.IsValid(buffer, recordMetaData)))
                    {
                        yield return buffer;
                    }
                }
            }
        }

        private IEnumerable<byte[]> IndexedTableScan(EqualityConstraint equalityConstraint)
        {
            List<byte[]> resultList = new List<byte[]>();
            var offsets = indexManager.GetIndex(equalityConstraint.Property);
            byte[] buffer;
            var whereQueryConstraints =
                constraints.Where(c => c != equalityConstraint) as IList<IWhereQueryConstraint> ?? constraints.ToList();
            using (var reader = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                foreach (long recordOffset in offsets[equalityConstraint.Value] as IEnumerable<long>)
                {
                    reader.BaseStream.Seek(recordOffset, SeekOrigin.Begin);
                    buffer = reader.ReadBytes(RecordWidth);
                    if (whereQueryConstraints.All(constraint => constraint.IsValid(buffer, recordMetaData)))
                    {
                        yield return buffer;
                    }
                }
            }
        }
    }
}