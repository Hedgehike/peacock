namespace Abide
{
    public interface IWhereQueryConstraint
    {
        string Property { get; }
        bool IsValid(byte[] record, RecordMetaData metaData);
    }
}