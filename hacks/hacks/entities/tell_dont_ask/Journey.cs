using NUnit.Framework;

namespace hacks.entities.tell_dont_ask
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
            return new { Origin = _origin, Destination = _destination, Fare = _fare };
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
