using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using AutoMapper;
using NUnit.Framework;

namespace hacks.entities.memento
{
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
                if (obj.GetType() != typeof(Journey)) return false;
                return Equals((Journey)obj);
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

                var journeys = new List<Journey>(new[] { jny2, jny1 });
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
}