using System;
using NUnit.Framework;

namespace hacks.modelling.memento
{
    [TestFixture]
    public class when_journey_is_projected
    {
        [Test]
        public void should_include_fields()
        {
            const string bank = "Bank";
            const short fare = 10;

            var memento = new JourneyMemento
            {
                AccountId = Guid.NewGuid(),
                Origin = bank,
                Destination = bank,
                Fare = 0
            };

            var journey = new Journey(memento);
            
            journey.AssignFare((origin, destination) => fare);

            var projection = journey.Project();

            Assert.That(projection.AccountId, Is.EqualTo(memento.AccountId));
            Assert.That(projection.Origin, Is.EqualTo(bank));
            Assert.That(projection.Destination, Is.EqualTo(bank));
            Assert.That(projection.Fare, Is.EqualTo(10));
        }

        [Test]
        public void should_demonstrate_tinkering()
        {

            const string outerMongolia = "Outer Mongolia";
            const string bank = "Bank";
            const short fare = 10;

            var memento = new JourneyMemento
            {
                AccountId = Guid.NewGuid(),
                Origin = bank,
                Destination = bank,
                Fare = 0
            };

            var journey = new Journey(memento);
            memento.Destination = outerMongolia;
            journey.AssignFare((origin, destination) => fare);

            var projection = journey.Project();

            Assert.That(projection.Destination, Is.EqualTo(outerMongolia));
        }
    }
}