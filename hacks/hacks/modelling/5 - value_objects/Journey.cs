using System;
using Castle.Facilities.TypedFactory.Internal;

namespace hacks.modelling.value_objects
{

    /* 
    https://lostechies.com/jimmybogard/2007/06/25/generic-value-object-equality/
    */

    public delegate short Fares(OriginDestination originDestination);
    
    public class OriginDestination : IEquatable<OriginDestination>
    {
        public readonly string Origin;
        public readonly string Destination;

        public static OriginDestination HereToHere(string location)
        {
            ValidateArgs(location, location);
            return new OriginDestination(location, location);
        }

        public static OriginDestination OriginToDestination(string origin, string destination)
        {
            ValidateArgs(origin, destination);
            return new OriginDestination(origin, destination);
        }

        public static OriginDestination NoJourney()
        {
            return new OriginDestination(string.Empty, string.Empty);
        }

        private OriginDestination(string origin, string destination)
        {
            Origin = origin;
            Destination = destination;
        }

        internal static bool HasNoJourney(OriginDestination originDestination)
        {
            return string.Empty == originDestination.Origin &&
                   string.Empty == originDestination.Destination;
        }

        private static void ValidateArgs(string origin, string destination)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                const string originName = nameof(origin);
                throw new ArgumentException($"{originName} cannot be null or empty", originName);
            }

            if (string.IsNullOrWhiteSpace(destination))
            {
                const string destinationName = nameof(destination);
                throw new ArgumentException($"{destinationName} cannot be null or empty", destinationName);
            }
        }

        public bool Equals(OriginDestination other)
        {
            return string.Equals(Origin, other.Origin)
                   && string.Equals(Destination, other.Destination);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is OriginDestination && Equals((OriginDestination) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Origin.GetHashCode()*397) ^ Destination.GetHashCode();
            }
        }
    }
}