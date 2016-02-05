using System.Collections.Generic;

namespace Abide
{
    class OrderDatabaseReader : DatabaseReader
    {
        public OrderDatabaseReader(string filename, IndexManager indexManager, QueryMode queryMode,
            IEnumerable<IWhereQueryConstraint> constraints) : base(filename, indexManager, constraints, queryMode)
        {
            recordMetaData = new RecordMetaData
            {
                ColumnDescriptors = new Dictionary<string, ColumnData>
                {
                    {"sha", new ColumnData {Offset = 0, Width = 4, Type = ColumnType.String}},
                    {"pct", new ColumnData {Offset = 4, Width = 4, Type = ColumnType.String}},
                    {"practice", new ColumnData {Offset = 8, Width = 6, Type = ColumnType.String}},
                    {"bnf_code", new ColumnData {Offset = 14, Width = 10, Type = ColumnType.String}},
                    {"bnf_name", new ColumnData {Offset = 24, Width = 40, Type = ColumnType.String}},
                    {"items", new ColumnData {Offset = 64, Width = sizeof (int), Type = ColumnType.Int}},
                    {
                        "nic", new ColumnData {Offset = 64 + sizeof (int), Width = sizeof (float), Type = ColumnType.Float}
                    },
                    {
                        "cost",
                        new ColumnData
                        {
                            Offset = 64 + sizeof (int) + sizeof (float),
                            Width = sizeof (float),
                            Type = ColumnType.Float
                        }
                    },
                    {
                        "period",
                        new ColumnData
                        {
                            Offset = 64 + sizeof (int) + sizeof (float) + sizeof (int),
                            Width = sizeof (int),
                            Type = ColumnType.Int
                        }
                    }
                }
            };
            RecordWidth = recordMetaData.RecordWitdh;
        }

        protected override int RecordWidth { get; set; }
    }
}