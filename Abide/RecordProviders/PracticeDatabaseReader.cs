using System.Collections.Generic;

namespace Abide
{
    internal class PracticeDatabaseReader : DatabaseReader
    {
        public PracticeDatabaseReader(string filename, IndexManager indexManager,
            IEnumerable<IWhereQueryConstraint> constraints, QueryMode queryMode = QueryMode.Auto)
            : base(filename, indexManager, constraints, queryMode)
        {
            recordMetaData = new RecordMetaData();
            recordMetaData.AddField("date", ColumnType.Int);
            recordMetaData.AddField("id", ColumnType.String, 6);
            recordMetaData.AddField("name", ColumnType.String, 40);
            recordMetaData.AddField("center_name", ColumnType.String, 40);
            recordMetaData.AddField("address", ColumnType.String, 40);
            recordMetaData.AddField("city", ColumnType.String, 40);
            recordMetaData.AddField("region", ColumnType.String, 40);
            recordMetaData.AddField("postal_code", ColumnType.String, 10);
            RecordWidth = recordMetaData.RecordWitdh;
        }

        protected override int RecordWidth { get; set; }
    }
}