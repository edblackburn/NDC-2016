using System;

namespace hacks.modelling.memento
{
    internal class JourneyMemento
    {
        public Guid AccountId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public short Fare { get; set; }
    }

    internal class Journey : IComparable<Journey>, IEquatable<Journey>
    {
        private readonly Guid _id = SeqGuid.NewGuid();
        private readonly JourneyMemento _memento;

        public Journey(JourneyMemento memento)
        {
            _memento = memento;
        }

        internal void AssignFare(Fares fares)
        {
            _memento.Fare = fares(_memento.Origin, _memento.Destination);
        }

        internal JourneyMemento Project()
        {
            return _memento;
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

    internal delegate short Fares(string origin, string destination);
}