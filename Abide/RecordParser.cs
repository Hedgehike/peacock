using System;
using System.Collections.Generic;
using System.Text;

namespace Abide
{
    public class RecordParser
    {
        private readonly IRecordProvider provider;
        private List<Dictionary<string, dynamic>> parsedData;
        private IEnumerable<byte[]> rawData;

        public RecordParser(IRecordProvider provider)
        {
            this.provider = provider;
        }

        public IEnumerable<Dictionary<string, dynamic>> ParseData()
        {
            if (parsedData == null)
            {
                rawData = provider.Read();
                parsedData = new List<Dictionary<string, dynamic>>();
                foreach (byte[] row in rawData)
                {
                    var record = new Dictionary<string, dynamic>();
                    foreach (KeyValuePair<string, ColumnData> columnDescriptor in provider.MetaData.ColumnDescriptors)
                    {
                        switch (columnDescriptor.Value.Type)
                        {
                            case ColumnType.String:
                                record.Add(columnDescriptor.Key,
                                    Encoding.ASCII.GetString(row, columnDescriptor.Value.Offset,
                                        columnDescriptor.Value.Width).TrimEnd('\0'));
                                break;
                            case ColumnType.Int:
                                record.Add(columnDescriptor.Key,
                                    BitConverter.ToInt32(row, columnDescriptor.Value.Offset));
                                break;
                            case ColumnType.Float:
                                record.Add(columnDescriptor.Key,
                                    BitConverter.ToSingle(row, columnDescriptor.Value.Offset));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    parsedData.Add(record);
                }
            }
            return parsedData;
        }
    }
}