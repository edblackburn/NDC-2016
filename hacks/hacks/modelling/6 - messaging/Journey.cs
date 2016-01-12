using System;
using System.Collections.Generic;
using System.Linq;
using hacks.modelling.value_objects;

namespace hacks.modelling.messaging
{
    public class Journey : IEquatable<Journey>, IComparable<Journey>
    {
        private readonly List<DeviceTappedCommand> _taps = new List<DeviceTappedCommand>();

        private short _fare;

        private readonly Func<List<DeviceTappedCommand>, DeviceTappedCommand> _origin =
            messages => messages.FirstOrDefault();

        private readonly Func<List<DeviceTappedCommand>, DeviceTappedCommand> _destination =
            messages => messages.LastOrDefault();

        public Journey() : this(Guid.NewGuid())
        {
        }

        internal Journey(Guid accountId)
        {
            AccountId = accountId;
        }

        public Guid JourneyId { get; } = SeqGuid.NewGuid();

        public Guid AccountId { get; }

        internal void RecieveTap(DeviceTappedCommand tap)
        {
            _taps.Add(tap);
            _taps.Sort(new TapComparer());
        }

        internal void AssignFare(Fares fares)
        {
            var originDestination = GetOriginDestination();
            if (OriginDestination.HasNoJourney(originDestination)) return;
            _fare = fares(originDestination);
        }

        private OriginDestination GetOriginDestination()
        {
            var origin = _origin(_taps);
            var destination = _destination(_taps) ?? origin;

            if (null == origin && null == destination)
            {
                return OriginDestination.NoJourney();
            }

            var originDestination = OriginDestination.OriginToDestination(origin.Location, destination.Location);

            return originDestination;
        }

        internal dynamic Project()
        {
            return new
            {
                JourneyId,
                AccountId,
                OriginDestination = GetOriginDestination(),
                Fare = _fare
            };
        }

        public bool Equals(Journey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return JourneyId.Equals(other.JourneyId);
        }

        public int CompareTo(Journey other)
        {
            return JourneyId.CompareTo(other.JourneyId);
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
            return JourneyId.GetHashCode();
        }
    }
}