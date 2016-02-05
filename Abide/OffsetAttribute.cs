using System;

namespace Abide
{
    public class OffsetAttribute : Attribute
    {
        public OffsetAttribute(int offset)
        {
            Offset = offset;
        }

        public int Offset { get; set; }
    }
}