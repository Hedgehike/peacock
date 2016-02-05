using System.Collections.Generic;
using System.Linq;

namespace Abide
{
    public class WhereRecordProvider : RecordProviderBase
    {
        protected List<IWhereQueryConstraint> constraints;

        private PushedDownstream pushedDownstream = PushedDownstream.NOT_TRIED;

        public WhereRecordProvider(IRecordProvider provider, IEnumerable<IWhereQueryConstraint> constraints)
            : base(provider)
        {
            this.constraints = constraints.ToList();
        }

        public void AddConstraints(IEnumerable<IWhereQueryConstraint> pConstraints)
        {
            constraints.AddRange(pConstraints);
        }

        public override IEnumerable<byte[]> Read()
        {
            if (pushedDownstream == PushedDownstream.NOT_TRIED)
            {
                pushedDownstream = TryPushDownstream();
            }
            if (pushedDownstream == PushedDownstream.SUCCEEDED)
            {
                foreach (var row in provider.Read())
                {
                    yield return row;
                }
            }
            else
            {
                var rows = provider.Read();
                foreach (var row in rows)
                {
                    if (constraints.All(constraint => constraint.IsValid(row, provider.MetaData))) yield return row;
                }
            }
        }

        private PushedDownstream TryPushDownstream()
        {
            var downstream = DownstreamSteps();
            foreach (var recordSource in downstream.Where(constraint => constraint is WhereRecordProvider))
            {
                foreach (
                    string key in
                        recordSource.MetaData.ColumnDescriptors.Keys.Where(
                            key => constraints.Any(c => c.Property == key)))
                {
                    ((WhereRecordProvider) recordSource).AddConstraints(constraints.Where(c => c.Property == key));
                    constraints = constraints.Where(c => c.Property != key).ToList();
                }
            }
            if (constraints.Any()) return PushedDownstream.PARTIAL;
            return PushedDownstream.SUCCEEDED;
        }

        private enum PushedDownstream
        {
            NOT_TRIED,
            PARTIAL,
            SUCCEEDED
        }
    }
}