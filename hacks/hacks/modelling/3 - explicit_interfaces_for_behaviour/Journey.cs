using System;

namespace hacks.modelling.explicit_interfaces_for_behaviour
{
    internal delegate short Fares(string origin, string destination);

    internal class Journey : IEquatable<Journey>, IComparable<Journey>
    {
        private readonly Guid _id = SeqGuid.NewGuid();
        private readonly Guid _accountId;
        private readonly string _origin;
        private readonly string _destination;
        private short _fare;

        public Journey(Guid accountId, string origin, string destination)
        {
            _origin = origin;
            _destination = destination;
            _accountId = accountId;
        }

        internal void AssignFare(Fares fares)
        {
            _fare = fares(_origin, _destination);
        }

        internal dynamic Export()
        {
            return new
            {
                JourneyId = _id,
                AccountId = _accountId,
                Origin = _origin,
                Destination = _destination,
                Fare = _fare
            };
        }

        public bool Equals(Journey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _id.Equals(other._id);
        }

        public int CompareTo(Journey other)
        {
            return _id.CompareTo(other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Journey)) return false;
            return Equals((Journey) obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}