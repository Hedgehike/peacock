using System;
using System.Collections.Generic;
using System.Linq;

namespace Abide.RecordProviders
{
    class ByteSortingComparer : IComparer<Byte[]>
    {
        public Func<byte[], byte[], int> Comparator { get; set; } = (_, __) => 0;
        public SortingRecordProvider.Order Order { get; set; }

        public int Compare(byte[] x, byte[] y)
        {
            if (Order == SortingRecordProvider.Order.DESCENDING) return -1*Comparator.Invoke(x, y);
            return Comparator.Invoke(x, y);
        }
    }

    public class SortingRecordProvider : RecordProviderBase
    {
        private readonly Order order;
        private readonly string orderBy;

        public SortingRecordProvider(string orderBy, IRecordProvider provider, Order order = Order.ASCENDING)
            : base(provider)
        {
            this.orderBy = orderBy;
            if (!provider.MetaData.ColumnDescriptors.ContainsKey(orderBy))
            {
                throw new MalformedQueryException($"Could not find field named {orderBy}");
            }
            this.order = order;
        }

        public override IEnumerable<byte[]> Read()
        {
            var resultSet = base.Read().ToList();
            var orderFieldType = provider.MetaData.ColumnDescriptors[orderBy].Type;
            var offset = provider.MetaData.ColumnDescriptors[orderBy].Offset;
            var width = provider.MetaData.ColumnDescriptors[orderBy].Width;
            Func<byte[], byte[], int> comparator;
            ByteSortingComparer byteSortingComparer = new ByteSortingComparer();
            switch (orderFieldType)
            {
                case ColumnType.String:
                    comparator = (a, b) =>
                    {
                        for (int i = offset; i < width; i++)
                        {
                            if (a[i] < b[i]) return -1;
                            if (a[i] > b[i]) return 1;
                        }
                        return 0;
                    };
                    break;
                case ColumnType.Int:
                    comparator = (a, b) => BitConverter.ToInt32(a, offset).CompareTo(BitConverter.ToInt32(b, offset));
                    break;
                case ColumnType.Float:
                    comparator = (a, b) => BitConverter.ToSingle(a, offset).CompareTo(BitConverter.ToSingle(b, offset));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            byteSortingComparer.Comparator = comparator;
            byteSortingComparer.Order = order;
            resultSet.Sort(byteSortingComparer);
            return resultSet;
        }

        public enum Order
        {
            ASCENDING,
            DESCENDING
        }
    }
}