using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AutoMapper;
using NUnit.Framework;

namespace hacks.entities
{
    namespace anemic
    {
        internal class Journey
        {
            public string Origin { get; set; }
            public string Destination { get; set; }
            public short Fare { get; set; }
        }

        internal class GenerateFare
        {
            private readonly INetwork _network;

            public GenerateFare(INetwork network)
            {
                _network = network;
            }

            internal void AssignFare(Journey jny)
            {
                jny.Fare = _network.GetFare(jny.Origin, jny.Destination);
            }
        }

        internal interface INetwork
        {
            short GetFare(string origin, string destination);
        }
    }

    namespace fat_model
    {
        internal class Journey
        {
            public Journey(string origin, string destination)
            {
                Origin = origin;
                Destination = destination;
            }

            public string Origin { get; }
            public string Destination { get; }
            public short Fare { get; private set; }

            internal void AssignFare(Func<string, string, short> network)
            {
                Fare = network(Origin, Destination);
            }
        }
    }

    namespace tell_dont_ask_model
    {
        internal class Journey
        {
            private readonly string _origin;
            private readonly string _destination;
            private short _fare;

            public Journey(string origin, string destination)
            {
                _origin = origin;
                _destination = destination;
            }

            internal void AssignFare(Fares fares)
            {
                _fare = fares(_origin, _destination);
            }

            internal dynamic Export()
            {
                return new {Origin = _origin, Destination = _destination, Fare = _fare};
            }
        }

        internal delegate short Fares(string origin, string destination);

        [TestFixture]
        public class when_journey_is_exported
        {
            [Test]
            public void should_incude_fare()
            {
                const string origin = "Bank";
                const string destination = "Western Gateway";
                const short fare = 10;

                var jny = new Journey(origin, destination);

                jny.AssignFare((o, d) => fare);

                dynamic export = jny.Export();

                Assert.That(export.Origin, Is.EqualTo(origin));
                Assert.That(export.Destination, Is.EqualTo(destination));
                Assert.That(export.Fare, Is.EqualTo(10));
            }
        }
    }

    namespace explicit_interfaces_for_behaviour
    {
        internal class Journey : IEquatable<Journey>, IComparable<Journey>
        {
            private readonly Guid _id = SeqGuid.NewGuid();
            private readonly string _origin;
            private readonly string _destination;
            private short _fare;

            public Journey(string origin, string destination)
            {
                _origin = origin;
                _destination = destination;
            }

            internal void AssignFare(Fares fares)
            {
                _fare = fares(_origin, _destination);
            }

            internal dynamic Export()
            {
                return new {Origin = _origin, Destination = _destination, Fare = _fare};
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

        internal class SeqGuid
        {
            [DllImport("rpcrt4.dll", SetLastError = true)]
            private static extern int UuidCreateSequential(out Guid guid);

            public static Guid NewGuid()
            {
                Guid guid;
                return UuidCreateSequential(out guid) == 0 ? guid : Guid.NewGuid();
            }
        }


        [TestFixture]
        public class when_journeys_are_compared
        {
            [Test]
            public void should_be_sortable()
            {
                const string bank = "Bank";
                const string westernGateway = "Western Gateway";
                const short fare = 10;

                var jny1 = new Journey(bank, westernGateway);
                var jny2 = new Journey(westernGateway, bank);

                jny1.AssignFare((o, d) => fare);
                jny2.AssignFare((o, d) => fare);

                var jourenys = new List<Journey>(new[] {jny2, jny1});
                jourenys.Sort();

                Assert.That(jourenys.First().Export().Origin, Is.EqualTo(bank));
                Assert.That(jourenys.First().Export().Destination, Is.EqualTo(westernGateway));
                Assert.That(jourenys.First().Export().Fare, Is.EqualTo(10));

                Assert.That(jourenys.Last().Export().Origin, Is.EqualTo(westernGateway));
                Assert.That(jourenys.Last().Export().Destination, Is.EqualTo(bank));
                Assert.That(jourenys.Last().Export().Fare, Is.EqualTo(10));
            }

            [Test]
            public void should_equate()
            {
                const string bank = "Bank";
                const string westernGateway = "Western Gateway";

                var jny1 = new Journey(bank, westernGateway);
                var jny2 = new Journey(westernGateway, bank);
                var jny3 = jny1;

                Assert.That(jny1, Is.EqualTo(jny1));
                Assert.That(jny1, Is.Not.EqualTo(jny2));
                Assert.That(jny3, Is.Not.EqualTo(jny2));
                Assert.That(jny3, Is.EqualTo(jny1));
            }
        }
    }

    namespace memento
    {
        internal class JourneyMemento
        {
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

            internal JourneyMemento Export()
            {
                return Mapper.Map<JourneyMemento, JourneyMemento>(_memento);
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

        internal class GenerateFare
        {
            private readonly INetwork _network;

            public GenerateFare(INetwork network)
            {
                _network = network;
            }

            internal void AssignFare(JourneyMemento jny)
            {
                jny.Fare = _network.GetFare(jny.Origin, jny.Destination);
            }
        }

        internal interface INetwork
        {
            short GetFare(string origin, string destination);
        }

        internal delegate short Fares(string origin, string destination);

        internal class SeqGuid
        {
            [DllImport("rpcrt4.dll", SetLastError = true)]
            private static extern int UuidCreateSequential(out Guid guid);

            public static Guid NewGuid()
            {
                Guid guid;
                return UuidCreateSequential(out guid) == 0 ? guid : Guid.NewGuid();
            }
        }

        [TestFixture]
        public class when_journeys_are_compared
        {
            [Test]
            public void should_be_sortable()
            {
                const string bank = "Bank";
                const string westernGateway = "Western Gateway";
                const short fare = 10;

                var jny1Memento = new JourneyMemento
                {
                    Origin = bank,
                    Destination = westernGateway,
                    Fare = 0
                };

                var jny2Memento = new JourneyMemento
                {
                    Origin = westernGateway,
                    Destination = bank,
                    Fare = 0
                };

                var jny1 = new Journey(jny1Memento);
                var jny2 = new Journey(jny2Memento);

                jny1.AssignFare((o, d) => fare);
                jny2.AssignFare((o, d) => fare);

                var journeys = new List<Journey>(new[] {jny2, jny1});
                journeys.Sort();

                Assert.That(journeys.First().Export().Origin, Is.EqualTo(bank));
                Assert.That(journeys.First().Export().Destination, Is.EqualTo(westernGateway));
                Assert.That(journeys.First().Export().Fare, Is.EqualTo(10));

                Assert.That(journeys.Last().Export().Origin, Is.EqualTo(westernGateway));
                Assert.That(journeys.Last().Export().Destination, Is.EqualTo(bank));
                Assert.That(journeys.Last().Export().Fare, Is.EqualTo(10));
            }

            [Test]
            public void should_equate()
            {
                const string bank = "Bank";
                const string westernGateway = "Western Gateway";

                var jny1Memento = new JourneyMemento
                {
                    Origin = bank,
                    Destination = westernGateway,
                    Fare = short.MinValue
                };

                var jny2Memento = new JourneyMemento
                {
                    Origin = westernGateway,
                    Destination = bank,
                    Fare = short.MaxValue
                };
                var jny1 = new Journey(jny1Memento);
                var jny2 = new Journey(jny2Memento);
                var jny3 = jny1;

                Assert.That(jny1, Is.EqualTo(jny1));
                Assert.That(jny1, Is.Not.EqualTo(jny2));
                Assert.That(jny3, Is.Not.EqualTo(jny2));
                Assert.That(jny3, Is.EqualTo(jny1));
            }
        }
    }

    namespace value_objects
    {
        internal struct OriginDestination : IEquatable<OriginDestination>
        {
            public readonly string Origin;
            public readonly string Destination;

            public static OriginDestination HereToHere(string location)
            {
                return new OriginDestination(location, location);
            }

            public static OriginDestination OriginToDestination(string origin, string destination)
            {
                return new OriginDestination(origin, destination);
            }

            private OriginDestination(string origin, string destination)
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

                Origin = origin;
                Destination = destination;
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
            public void should_incude_fare()
            {
                const string origin = "Bank";
                const string destination = "Western Gateway";
                const short fare = 10;

                var jny = new Journey(OriginDestination.OriginToDestination(origin, destination));

                jny.AssignFare(od => fare);

                dynamic export = jny.Export();

                Assert.That(export.Origin, Is.EqualTo(origin));
                Assert.That(export.Destination, Is.EqualTo(destination));
                Assert.That(export.Fare, Is.EqualTo(10));
            }
        }
    }
}