using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abide;

namespace Tests
{
    public class MockRecordProvider : IRecordProvider
    {
        protected byte[] createRecord(string mockString, int mockInt, float mockFloat)
        {
            byte[] buffer = new byte[10+sizeof(int)+sizeof(float)];
            for (var i = 0; i < 10; i++)
            {
                buffer[i] = Encoding.ASCII.GetBytes(mockString.PadRight(10, '\0'))[i];
            }
            for (var i = 0; i < sizeof (int); i++)
            {
                buffer[i + 10] = BitConverter.GetBytes(mockInt)[i];
            }
            for (var i = 0; i < sizeof(float); i++)
            {
                buffer[i + 10 + sizeof(int)] = BitConverter.GetBytes(mockFloat)[i];
            }
            return buffer;
        }
        public MockRecordProvider(IEnumerable<string> columnNames = null)
        {
           this.MetaData = new RecordMetaData();
           var names = (columnNames ?? new[] { "mockString", "mockInt", "mockFloat" }).ToArray();
           this.MetaData.AddField(names[0], ColumnType.String, 10);
           this.MetaData.AddField(names[1], ColumnType.Int);
           this.MetaData.AddField(names[2], ColumnType.Float);
        }

        public RecordMetaData MetaData { get; }
        public virtual IEnumerable<byte[]> Read()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRecordProvider> DownstreamSteps()
        {
            return new List<IRecordProvider>();
        }
    }

    class EmptyRecordProvider : MockRecordProvider
    {
        public override IEnumerable<byte[]> Read()
        {
            return new List<byte[]>();
        }
    }
}
