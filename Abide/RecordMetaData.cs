using System;
using System.Collections.Generic;
using System.Linq;

namespace Abide
{
    public class RecordMetaData
    {
        public Dictionary<string, ColumnData> ColumnDescriptors { get; set; } = new Dictionary<string, ColumnData>();
        public int RecordWitdh => ColumnDescriptors.Values.Select(t => t.Width).Sum();

        public void AddField(string name, ColumnType type, int stringWidth = 6)
        {
            int offset;
            if (ColumnDescriptors.Values.Any())
            {
                var furthestColumn = ColumnDescriptors.Values.OrderByDescending(cd => cd.Offset).First();
                offset = furthestColumn.Offset + furthestColumn.Width;
            }
            else
            {
                offset = 0;
            }
            switch (type)
            {
                case ColumnType.String:
                    ColumnDescriptors.Add(name, new ColumnData
                    {
                        Offset = offset,
                        Width = stringWidth,
                        Type = ColumnType.String
                    });
                    break;
                case ColumnType.Int:
                    ColumnDescriptors.Add(name, new ColumnData
                    {
                        Offset = offset,
                        Width = sizeof (int),
                        Type = ColumnType.Int
                    });
                    break;
                case ColumnType.Float:
                    ColumnDescriptors.Add(name, new ColumnData
                    {
                        Offset = offset,
                        Width = sizeof (float),
                        Type = ColumnType.Float
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public struct ColumnData
    {
        public int Offset;
        public int Width;
        public ColumnType Type;
    }

    public enum ColumnType
    {
        String,
        Int,
        Float
    }
}