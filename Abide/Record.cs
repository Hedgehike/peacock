using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Abide
{
    public class Record
    {
        public static int RECORD_WIDTH = 4 + 4 + 6 + 10 + 40 + sizeof (int) + sizeof (float) + sizeof (float) +
                                         sizeof (int);

        public static Dictionary<string, Tuple<int, int>> OffsetAndWidth;

        [Offset(0), Width(4)]
        public string Sha { get; set; }

        [Offset(4), Width(4)]
        public string Pct { get; set; }

        [Offset(8), Width(6)]
        public string Practice { get; set; }

        [Offset(14), Width(10)]
        public string Bnf_code { get; set; }

        [Offset(24), Width(40)]
        public string Bnf_name { get; set; }

        [Offset(64), Width(sizeof (int))]
        public int Items { get; set; }

        [Offset(64 + sizeof (int)), Width(sizeof (float))]
        public float Nic { get; set; }

        [Offset(64 + sizeof (int) + sizeof (float)), Width(sizeof (float))]
        public float Cost { get; set; }

        [Offset(64 + sizeof (int) + sizeof (float) + sizeof (float)), Width(sizeof (int))]
        public int Period { get; set; }

        public static Record FromBuffer(byte[] buffer)
        {
            Debug.Assert(buffer.Length == RECORD_WIDTH);
            Record record = new Record();
            var propertyInfos = typeof (Record).GetProperties();
            var maxLength =
                propertyInfos.Select(pI => ((WidthAttribute) pI.GetCustomAttribute(typeof (WidthAttribute))).Width)
                    .Max();
            var propertyBuffer = new byte[maxLength];
            foreach (var propertyInfo in propertyInfos)
            {
                var offset = ((OffsetAttribute) propertyInfo.GetCustomAttribute(typeof (OffsetAttribute))).Offset;
                var width = ((WidthAttribute) propertyInfo.GetCustomAttribute(typeof (WidthAttribute))).Width;
                for (int i = 0; i < width; i++) propertyBuffer[i] = buffer[offset + i];
                Type propertyType = propertyInfo.PropertyType;
                object value = null;
                if (propertyType == typeof (string))
                {
                    value = Encoding.ASCII.GetString(propertyBuffer);
                }
                else if (propertyType == typeof (int))
                {
                    value = BitConverter.ToInt32(propertyBuffer, 0);
                }
                else if (propertyType == typeof (float))
                {
                    value = BitConverter.ToSingle(propertyBuffer, 0);
                }
                propertyInfo.SetValue(record, value);
            }

            return record;
        }
    }
}