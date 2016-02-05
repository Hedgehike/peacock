using System;
using System.Text;

namespace Abide
{
    public interface IByteConverter
    {
        byte[] Convert(string s);
        byte[] Convert(int i);
        byte[] Convert(float f);
        byte[] Convert(object o);
    }

    public class ByteConverter : IByteConverter
    {
        public byte[] Convert(string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        public byte[] Convert(int i)
        {
            return BitConverter.GetBytes(i);
        }

        public byte[] Convert(float f)
        {
            return BitConverter.GetBytes(f);
        }

        public byte[] Convert(object o)
        {
            throw new NotImplementedException();
        }
    }
}