using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace hacks.entities.explicit_interfaces_for_behaviour
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
            return new { Origin = _origin, Destination = _destination, Fare = _fare };
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
            const string kingsCross = "Kings Cross";
            const string bank = "Bank";
            const string westernGateway = "Western Gateway";
            const short fare = 10;

            var jny1 = new Journey(kingsCross, bank);
            var jny2 = new Journey(bank, westernGateway);
            var jny3 = new Journey(westernGateway, bank);

            jny1.AssignFare((o, d) => fare);
            jny2.AssignFare((o, d) => fare);
            jny3.AssignFare((o, d) => fare);

            var jourenys = new List<Journey>(new[] {jny2, jny3, jny1 });
            jourenys.Sort(); 

            Assert.That(jourenys.First().Export().Origin, Is.EqualTo(kingsCross));
            Assert.That(jourenys.First().Export().Destination, Is.EqualTo(bank));
            Assert.That(jourenys.First().Export().Fare, Is.EqualTo(10));

            Assert.That(jourenys.ElementAt(1).Export().Origin, Is.EqualTo(bank));
            Assert.That(jourenys.ElementAt(1).Export().Destination, Is.EqualTo(westernGateway));
            Assert.That(jourenys.ElementAt(1).Export().Fare, Is.EqualTo(10));

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
