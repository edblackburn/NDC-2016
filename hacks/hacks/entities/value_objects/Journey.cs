using System;
using NUnit.Framework;

namespace hacks.entities.value_objects
{
    public struct OriginDestination : IEquatable<OriginDestination>
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

    internal class Journey
    {
        private readonly OriginDestination _originDestination;
        private short _fare;

        public Journey(OriginDestination originDestination)
        {
            _originDestination = originDestination;
        }

        internal void AssignFare(Fares fares)
        {
            _fare = fares(_originDestination);
        }

        internal dynamic Export()
        {
            return new
            {
                _originDestination.Origin,
                _originDestination.Destination,
                Fare = _fare
            };
        }
    }

    internal delegate short Fares(OriginDestination originDestination);

    [TestFixture]
    public class when_journey_is_exported
    {
        [Test]
        public void should_include_fare()
        {
            const string origin = "Bank";
            const string destination = "Western Gateway";
            const short fare = 10;

            var jny = new Journey(OriginDestination.OriginToDestination(origin, destination));

            jny.AssignFare(od => fare);

            dynamic export = jny.Export();

            Assert.That(export.Origin, Is.EqualTo(origin));
            Assert.That(export.Destination, Is.EqualTo(destination));
            Assert.That(export.Fare, Is.EqualTo(fare));
        }
    }
}