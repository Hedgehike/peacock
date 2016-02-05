using System;

namespace Abide
{
    public class WidthAttribute : Attribute
    {
        public WidthAttribute(int width)
        {
            Width = width;
        }

        public int Width { get; set; }
    }
}