namespace Abide
{
    class Not : IWhereQueryConstraint
    {
        private readonly IWhereQueryConstraint constraint;

        public Not(IWhereQueryConstraint constraint)
        {
            Property = constraint.Property;
            this.constraint = constraint;
        }

        public bool IsValid(byte[] record, RecordMetaData metaData)
        {
            return !constraint.IsValid(record, metaData);
        }

        public string Property { get; }
    }
}