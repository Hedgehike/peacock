namespace Abide
{
    public class StringEqualsConstraint : EqualityConstraint
    {
        public StringEqualsConstraint(string property, string value, IByteConverter byteConverter)
            : base(property, value, byteConverter)
        {
        }
    }
}